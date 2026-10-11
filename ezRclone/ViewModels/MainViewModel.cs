using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using ezRclone.Core;
using ezRclone.Services;

namespace ezRclone.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly SettingsStore _store;
    private readonly Settings _settings;
    private readonly MountManager _mountManager;
    private readonly IAutostart _autostart;

    private RemoteViewModel? _selectedRemote;
    private string _rcloneStatus = string.Empty;
    private bool _isRefreshing;
    private bool _startHidden;

    public MainViewModel(SettingsStore store, MountManager mountManager, IAutostart autostart, Dispatcher dispatcher)
    {
        _store = store;
        _settings = store.Load();
        _mountManager = mountManager;
        _autostart = autostart;
        _startHidden = autostart.StartsHidden;

        _mountManager.RcloneDirectory = _settings.RclonePath;
        _mountManager.StateChanged += (_, state) => dispatcher.BeginInvoke(() => OnStateChanged(state));

        foreach (var mountable in _settings.Mountables)
            Remotes.Add(new RemoteViewModel(mountable, mountManager, Save));
        Remotes.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasRemotes));
            OnPropertyChanged(nameof(Summary));
        };
        _selectedRemote = Remotes.FirstOrDefault();

        MountAllCommand = new AsyncRelayCommand(MountAllAsync, () => Remotes.Any(CanAutoMount));
        UnmountAllCommand = new AsyncRelayCommand(UnmountAllAsync, () => Remotes.Any(r => r.IsActive));
        RefreshRemotesCommand = new AsyncRelayCommand(RefreshRemotesAsync, () => !IsRefreshing);
        BrowseRclonePathCommand = new RelayCommand(BrowseRclonePath);
        RemoveRemoteCommand = new RelayCommand(RemoveSelectedRemote,
            () => SelectedRemote is { IsMissing: true, IsActive: false });
    }

    public ObservableCollection<RemoteViewModel> Remotes { get; } = new();

    public RemoteViewModel? SelectedRemote
    {
        get => _selectedRemote;
        set => SetProperty(ref _selectedRemote, value);
    }

    public bool HasRemotes => Remotes.Count > 0;

    public string Summary
    {
        get
        {
            var mounted = Remotes.Count(r => r.Status == MountStatus.Mounted);
            return $"{Remotes.Count} {(Remotes.Count == 1 ? "remote" : "remotes")} · {mounted} mounted";
        }
    }

    /// <summary>
    /// Folder containing rclone.exe, empty to use rclone from PATH.
    /// </summary>
    public string RclonePath
    {
        get => _settings.RclonePath;
        set
        {
            var path = (value ?? string.Empty).Trim().Trim('"');
            if (path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                path = System.IO.Path.GetDirectoryName(path) ?? string.Empty;

            if (path == _settings.RclonePath)
                return;

            _settings.RclonePath = path;
            _mountManager.RcloneDirectory = path;
            Save();
            OnPropertyChanged();
            _ = RefreshRemotesAsync();
        }
    }

    public string RcloneStatus
    {
        get => _rcloneStatus;
        private set => SetProperty(ref _rcloneStatus, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        private set => SetProperty(ref _isRefreshing, value);
    }

    public bool StartWithWindows
    {
        get => _autostart.IsEnabled;
        set
        {
            _autostart.Set(value, _startHidden);
            OnPropertyChanged();
        }
    }

    public bool StartHidden
    {
        get => _startHidden;
        set
        {
            if (!SetProperty(ref _startHidden, value))
                return;

            if (_autostart.IsEnabled)
                _autostart.Set(true, value);
        }
    }

    public ICommand MountAllCommand { get; }
    public ICommand UnmountAllCommand { get; }
    public ICommand RefreshRemotesCommand { get; }
    public ICommand BrowseRclonePathCommand { get; }
    public ICommand RemoveRemoteCommand { get; }

    /// <summary>
    /// Loads the remotes from rclone, takes over mounts that survived a restart and mounts the auto-mount remotes.
    /// </summary>
    public async Task InitializeAsync()
    {
        await RefreshRemotesAsync();

        var remotes = Remotes.Select(r => r.Remote).ToArray();
        await Task.Run(() => ExistingMounts.Adopt(_mountManager, remotes));

        await Task.WhenAll(Remotes.Where(r => r.AutoMount && CanAutoMount(r)).Select(r => r.MountAsync()));
    }

    public async Task RefreshRemotesAsync()
    {
        if (IsRefreshing)
            return;

        IsRefreshing = true;
        try
        {
            var rclone = new Rclone(_settings.RclonePath);
            var version = await rclone.GetVersionAsync();
            MergeRemotes(await rclone.ListRemotesAsync());
            RcloneStatus = $"Using {version}";
        }
        catch (RcloneException e)
        {
            RcloneStatus = e.Message;
        }
        finally
        {
            IsRefreshing = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public Task MountAllAsync() => Task.WhenAll(Remotes.Where(CanAutoMount).Select(r => r.MountAsync()));

    public Task UnmountAllAsync() => _mountManager.UnmountAllAsync();

    private static bool CanAutoMount(RemoteViewModel remote) =>
        !remote.IsActive && !remote.IsMissing && DriveLetters.Normalize(remote.DriveLetter) != null;

    private void MergeRemotes(IReadOnlyList<RemoteInfo> remotes)
    {
        var added = false;
        foreach (var info in remotes)
        {
            var remote = Remotes.FirstOrDefault(r => r.Remote == info.Name);
            if (remote == null)
            {
                var mountable = new Mountable { Remote = info.Name, Name = info.Name, AutoMount = true };
                _settings.Mountables.Add(mountable);
                remote = new RemoteViewModel(mountable, _mountManager, Save);
                Remotes.Add(remote);
                added = true;
            }

            remote.Type = info.Type;
            remote.IsMissing = false;
        }

        foreach (var remote in Remotes.Where(r => remotes.All(info => info.Name != r.Remote)))
            remote.IsMissing = true;

        if (added)
            Save();

        SelectedRemote ??= Remotes.FirstOrDefault();
    }

    private void RemoveSelectedRemote()
    {
        if (SelectedRemote is not { IsMissing: true, IsActive: false } remote)
            return;

        _settings.Mountables.Remove(remote.Mountable);
        Remotes.Remove(remote);
        Save();
        SelectedRemote = Remotes.FirstOrDefault();
    }

    private void BrowseRclonePath()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select rclone.exe",
            Filter = "rclone|rclone.exe|Programs|*.exe",
            InitialDirectory = Directory.Exists(RclonePath) ? RclonePath : string.Empty
        };

        if (dialog.ShowDialog() == true)
            RclonePath = dialog.FileName;
    }

    private void OnStateChanged(MountState state)
    {
        Remotes.FirstOrDefault(r => r.Remote == state.Remote)?.Apply(state);
        OnPropertyChanged(nameof(Summary));
    }

    private void Save() => _store.Save(_settings);
}
