using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ezRclone.Core;
using ezRclone.Services;
using ezRclone.ViewModels;

namespace ezRclone.UITests;

public class MainWindowTests
{
    /// <summary>
    /// Loads the real application resources and main window with sample data, visits every tab and fails on
    /// exceptions or binding errors. WPF allows only one Application per process, so this is a single test.
    /// </summary>
    [Fact]
    public void MainWindow_LoadsAllTabsWithoutBindingErrors()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                LoadMainWindow();
            }
            catch (Exception e)
            {
                failure = e;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure != null)
            throw new Exception("Loading the main window failed.", failure);
    }

    private static void LoadMainWindow()
    {
        var bindingErrors = new BindingErrorListener();
        PresentationTraceSources.Refresh();
        PresentationTraceSources.DataBindingSource.Listeners.Add(bindingErrors);
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;

        var settingsDirectory = Path.Combine(Path.GetTempPath(), "ezRclone.UITests", Guid.NewGuid().ToString("N"));
        var store = new SettingsStore(settingsDirectory);
        store.Save(new Settings
        {
            RclonePath = settingsDirectory,
            Mountables =
            {
                new Mountable { Remote = "gdrive", Name = "Google Drive", DriveLetter = "G", AutoMount = true },
                new Mountable { Remote = "nas", ExtraArguments = "--read-only" }
            }
        });

        var app = new App();
        app.InitializeComponent();

        var viewModel = new MainViewModel(store, new MountManager(), new FakeAutostart(), Dispatcher.CurrentDispatcher);
        var window = new MainWindow { DataContext = viewModel, ShowActivated = false };
        window.Show();
        window.UpdateLayout();

        // Without a drive letter this fails right away, which fills the failure message and the log.
        var failedRemote = viewModel.Remotes.Single(r => r.Remote == "nas");
        Assert.True(failedRemote.MountAsync().IsCompleted);
        DoEvents();
        Assert.Equal(MountStatus.Failed, failedRemote.Status);
        Assert.NotEmpty(viewModel.Log);

        var tabs = FindChild<TabControl>(window) ?? throw new InvalidOperationException("No TabControl found.");
        foreach (var remote in viewModel.Remotes)
        {
            viewModel.SelectedRemote = remote;
            tabs.SelectedIndex = 0;
            DoEvents();
        }

        foreach (var expander in FindChildren<Expander>(window))
            expander.IsExpanded = true;
        DoEvents();

        for (var i = 1; i < tabs.Items.Count; i++)
        {
            tabs.SelectedIndex = i;
            DoEvents();
        }

        window.Close();
        app.Shutdown();

        Assert.Empty(bindingErrors.Messages);
        Directory.Delete(settingsDirectory, true);
    }

    private static void DoEvents()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, () => frame.Continue = false);
        Dispatcher.PushFrame(frame);
    }

    private static T? FindChild<T>(DependencyObject parent) where T : DependencyObject =>
        FindChildren<T>(parent).FirstOrDefault();

    private static IEnumerable<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
                yield return match;

            foreach (var descendant in FindChildren<T>(child))
                yield return descendant;
        }
    }

    private sealed class FakeAutostart : IAutostart
    {
        public bool IsEnabled { get; private set; }
        public bool StartsHidden { get; private set; }

        public void Set(bool enabled, bool hidden)
        {
            IsEnabled = enabled;
            StartsHidden = hidden;
        }
    }

    private sealed class BindingErrorListener : TraceListener
    {
        public List<string> Messages { get; } = new();

        public override void Write(string? message)
        {
        }

        public override void WriteLine(string? message)
        {
            if (message != null)
                Messages.Add(message);
        }
    }
}
