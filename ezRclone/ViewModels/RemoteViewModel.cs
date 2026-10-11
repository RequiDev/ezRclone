using System.Runtime.CompilerServices;
using System.Windows.Input;
using ezRclone.Core;

namespace ezRclone.ViewModels;

public sealed class RemoteViewModel : ObservableObject
{
    private readonly MountManager _mountManager;
    private readonly Action _save;

    private string _type = string.Empty;
    private bool _isMissing;
    private MountState _state;
    private IReadOnlyList<string> _availableDriveLetters;

    public RemoteViewModel(Mountable mountable, MountManager mountManager, Action save)
    {
        Mountable = mountable;
        _mountManager = mountManager;
        _save = save;
        _state = mountManager.GetState(mountable.Remote);
        _availableDriveLetters = DriveLetters.GetAvailable(DriveLetters.GetUsed(), mountable.DriveLetter);

        MountCommand = new AsyncRelayCommand(MountAsync, () => !IsActive);
        UnmountCommand = new AsyncRelayCommand(UnmountAsync, () => IsActive);
        ToggleCommand = new AsyncRelayCommand(() => IsActive ? UnmountAsync() : MountAsync(),
            () => Status is not (MountStatus.Mounting or MountStatus.Unmounting));
    }

    public Mountable Mountable { get; }

    public string Remote => Mountable.Remote;

    public string DisplayName => string.IsNullOrWhiteSpace(Mountable.Name) ? Mountable.Remote : Mountable.Name;

    public string Subtitle
    {
        get
        {
            var drive = DriveLetters.Normalize(Mountable.DriveLetter) is { } letter ? $"{letter}:" : "No drive letter";
            return string.IsNullOrEmpty(Type) ? drive : $"{drive} · {Type}";
        }
    }

    public string Type
    {
        get => _type;
        set
        {
            if (SetProperty(ref _type, value))
                OnPropertyChanged(nameof(Subtitle));
        }
    }

    /// <summary>
    /// True when rclone no longer knows this remote, e.g. after it was deleted with <c>rclone config</c>.
    /// </summary>
    public bool IsMissing
    {
        get => _isMissing;
        set => SetProperty(ref _isMissing, value);
    }

    public string DriveLetter
    {
        get => Mountable.DriveLetter;
        set
        {
            // The drive letter combo box briefly pushes null while its item list is being replaced.
            if (value != null)
                Update(DriveLetters.Normalize(value) ?? string.Empty, Mountable.DriveLetter, v => Mountable.DriveLetter = v,
                    nameof(Subtitle));
        }
    }

    public string Path
    {
        get => Mountable.Path;
        set => Update((value ?? string.Empty).Trim(), Mountable.Path, v => Mountable.Path = v);
    }

    public string Name
    {
        get => Mountable.Name;
        set => Update((value ?? string.Empty).Trim(), Mountable.Name, v => Mountable.Name = v, nameof(DisplayName));
    }

    public bool AutoMount
    {
        get => Mountable.AutoMount;
        set => Update(value, Mountable.AutoMount, v => Mountable.AutoMount = v);
    }

    public bool NetworkDrive
    {
        get => Mountable.NetworkDrive;
        set => Update(value, Mountable.NetworkDrive, v => Mountable.NetworkDrive = v);
    }

    public string VfsCacheMode
    {
        get => Mountable.VfsCacheMode;
        set => Update(value ?? string.Empty, Mountable.VfsCacheMode, v => Mountable.VfsCacheMode = v);
    }

    public string ExtraArguments
    {
        get => Mountable.ExtraArguments;
        set => Update((value ?? string.Empty).Trim(), Mountable.ExtraArguments, v => Mountable.ExtraArguments = v);
    }

    public IReadOnlyList<string> VfsCacheModes => MountArguments.VfsCacheModes;

    public IReadOnlyList<string> AvailableDriveLetters
    {
        get => _availableDriveLetters;
        private set => SetProperty(ref _availableDriveLetters, value);
    }

    public string CommandPreview
    {
        get
        {
            if (DriveLetters.Normalize(Mountable.DriveLetter) == null)
                return "Pick a drive letter to see the rclone command.";

            var arguments = MountArguments.Build(Mountable).Select(a => a.Contains(' ') || a.Length == 0 ? $"\"{a}\"" : a);
            return "rclone " + string.Join(' ', arguments);
        }
    }

    public MountStatus Status => _state.Status;

    public string? StatusMessage => _state.Message;

    public bool HasStatusMessage => !string.IsNullOrEmpty(_state.Message);

    public string StatusText => Status switch
    {
        MountStatus.Mounting => "Mounting…",
        MountStatus.Mounted => DriveLetters.Normalize(Mountable.DriveLetter) is { } letter ? $"Mounted on {letter}:" : "Mounted",
        MountStatus.Unmounting => "Unmounting…",
        MountStatus.Failed => "Mount failed",
        _ => "Not mounted"
    };

    public bool IsActive => _state.IsActive;

    public bool IsEditable => !IsActive;

    public string ToggleText => IsActive ? "Unmount" : "Mount";

    public ICommand MountCommand { get; }
    public ICommand UnmountCommand { get; }
    public ICommand ToggleCommand { get; }

    public Task MountAsync() => _mountManager.MountAsync(Mountable);

    public Task UnmountAsync() => _mountManager.UnmountAsync(Remote);

    public void RefreshDriveLetters()
    {
        AvailableDriveLetters = DriveLetters.GetAvailable(DriveLetters.GetUsed(), Mountable.DriveLetter);
    }

    internal void Apply(MountState state)
    {
        _state = state;
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(HasStatusMessage));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(IsEditable));
        OnPropertyChanged(nameof(ToggleText));
        CommandManager.InvalidateRequerySuggested();
    }

    private void Update<T>(T value, T current, Action<T> assign, string? alsoChanged = null,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(value, current))
            return;

        assign(value);
        _save();

        OnPropertyChanged(propertyName);
        OnPropertyChanged(nameof(CommandPreview));
        if (alsoChanged != null)
            OnPropertyChanged(alsoChanged);
    }
}
