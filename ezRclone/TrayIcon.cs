using System.Windows;
using ezRclone.Core;
using ezRclone.ViewModels;
using Forms = System.Windows.Forms;

namespace ezRclone;

/// <summary>
/// Notification area icon. WPF has none of its own, so this wraps the WinForms one.
/// </summary>
public sealed class TrayIcon : IDisposable
{
    private readonly MainViewModel _viewModel;
    private readonly Action _showWindow;
    private readonly Action _exit;
    private readonly Forms.NotifyIcon _notifyIcon;
    private readonly Forms.ContextMenuStrip _menu = new();

    public TrayIcon(MainViewModel viewModel, Action showWindow, Action exit)
    {
        _viewModel = viewModel;
        _showWindow = showWindow;
        _exit = exit;

        _menu.Opening += (_, e) =>
        {
            BuildMenu();
            e.Cancel = false;
        };
        BuildMenu();

        using var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/rclone.ico"))!.Stream;
        _notifyIcon = new Forms.NotifyIcon
        {
            Icon = new System.Drawing.Icon(iconStream),
            Text = "ezRclone",
            ContextMenuStrip = _menu,
            Visible = true
        };
        _notifyIcon.MouseClick += (_, e) =>
        {
            if (e.Button == Forms.MouseButtons.Left)
                _showWindow();
        };
    }

    public void Dispose()
    {
        // Hide first, otherwise the icon lingers until the user hovers over it.
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _menu.Dispose();
    }

    private void BuildMenu()
    {
        _menu.Items.Clear();

        var open = new Forms.ToolStripMenuItem("Open ezRclone", null, (_, _) => _showWindow());
        open.Font = new System.Drawing.Font(open.Font, System.Drawing.FontStyle.Bold);
        _menu.Items.Add(open);
        _menu.Items.Add(new Forms.ToolStripSeparator());

        foreach (var remote in _viewModel.Remotes)
        {
            var driveLetter = DriveLetters.Normalize(remote.DriveLetter);
            var text = driveLetter == null ? remote.DisplayName : $"{remote.DisplayName} ({driveLetter}:)";
            var item = new Forms.ToolStripMenuItem(text)
            {
                Checked = remote.IsActive,
                Enabled = remote.IsActive || (driveLetter != null && !remote.IsMissing),
                ToolTipText = remote.StatusText
            };
            item.Click += (_, _) => remote.ToggleCommand.Execute(null);
            _menu.Items.Add(item);
        }

        if (_viewModel.Remotes.Count > 0)
            _menu.Items.Add(new Forms.ToolStripSeparator());

        _menu.Items.Add("Mount all", null, (_, _) => _viewModel.MountAllCommand.Execute(null));
        _menu.Items.Add("Unmount all", null, (_, _) => _viewModel.UnmountAllCommand.Execute(null));
        _menu.Items.Add(new Forms.ToolStripSeparator());
        _menu.Items.Add("Exit", null, (_, _) => _exit());
    }
}
