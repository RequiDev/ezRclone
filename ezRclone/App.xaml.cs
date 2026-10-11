using System.Windows;
using System.Windows.Threading;
using ezRclone.Core;
using ezRclone.Services;
using ezRclone.ViewModels;

namespace ezRclone;

public partial class App
{
    private SingleInstance? _singleInstance;
    private MainViewModel? _viewModel;
    private TrayIcon? _trayIcon;
    private MainWindow? _window;
    private bool _isExiting;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        var startHidden = e.Args.Contains(RegistryAutostart.HiddenArgument, StringComparer.OrdinalIgnoreCase);
        _singleInstance = new SingleInstance();
        if (!_singleInstance.IsFirstInstance)
        {
            // Starting ezRclone again brings up the running instance, unless it is the hidden autostart launch.
            if (!startHidden)
                _singleInstance.RequestShow();

            _singleInstance.Dispose();
            Shutdown();
            return;
        }

        _singleInstance.ListenForShowRequests(() => Dispatcher.BeginInvoke(ShowMainWindow));

        _viewModel = new MainViewModel(new SettingsStore(SettingsStore.DefaultDirectory), new MountManager(),
            new RegistryAutostart(), Dispatcher);
        // Failures are visible in the window already, only notify when it is not in front.
        _trayIcon = new TrayIcon(_viewModel, ShowMainWindow, ExitApplication,
            () => _window is not { IsVisible: true, IsActive: true });

        if (!startHidden)
            ShowMainWindow();

        await _viewModel.InitializeAsync();
    }

    public void ShowMainWindow()
    {
        if (_window == null)
        {
            _window = new MainWindow { DataContext = _viewModel };
            _window.Closing += (_, args) =>
            {
                // Closing the window only hides it, ezRclone keeps running in the tray.
                if (_isExiting)
                    return;

                args.Cancel = true;
                _window.Hide();
            };
        }

        _window.Show();
        if (_window.WindowState == WindowState.Minimized)
            _window.WindowState = WindowState.Normal;
        _window.Activate();
    }

    public void ExitApplication()
    {
        _isExiting = true;
        _trayIcon?.Dispose();
        _window?.Close();
        _singleInstance?.Dispose();
        Shutdown();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.Exception.Message, "ezRclone", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
