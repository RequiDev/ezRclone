using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Tomlyn;

namespace ezRclone
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new ezRclone());
            // Remote("google-drive", "G:", "Plex Media", "Google Drive");
        }

    }

    internal class ezRclone : ApplicationContext
    {
        private readonly List<Mountable> _mountables = new List<Mountable>();
        private readonly Dictionary<string, int> _mounts = new Dictionary<string, int>();

        private readonly NotifyIcon _trayIcon;
        private ManagerForm _managerForm;
        public ezRclone()
        {
            var rcloneConfig = File.ReadAllText(GetRCloneConfigPath());
            var toml = Toml.Parse(rcloneConfig);

            // foreach (var gay in toml.Tables)
            // {
            //     MessageBox.Show(gay.Name.ToString(), "", MessageBoxButtons.AbortRetryIgnore);
            // }

            var contextMenu = new ContextMenuStrip();

            // check if mounts.json exists in appdata
            var mountsFile = Application.UserAppDataPath + "\\mounts.json";
            if (File.Exists(mountsFile))
            {
                // load mounts.json
                _mountables = JsonConvert.DeserializeObject<List<Mountable>>(File.ReadAllText(mountsFile)) ?? new List<Mountable>();
            }

            contextMenu.Items.Add("Manage", null, (s, e) =>
            {
                if (_managerForm == null || _managerForm.IsDisposed)
                {
                    _managerForm = new ManagerForm(_mountables);
                }

                _managerForm.Show();
            });

            // local testing
            _mountables.Add(new Mountable
            {
                AutoMount = true,
                DriveLetter = "G",
                Name = "Google Drive",
                Remote = "google-drive",
                Path = "Plex Media",
                Mounted = false
            });
            
            _mountables.Add(new Mountable
            {
                AutoMount = true,
                DriveLetter = "P",
                Name = "Plex",
                Remote = "plex",
                Path = "/plex-media",
                Mounted = false,
                NetworkDrive = false
            });

            foreach (var mountable in _mountables)
            {
                var item = new ToolStripMenuItem(mountable.Name);
                item.DropDownItems.Add(new ToolStripMenuItem("Mount", null, (sender, args) => Mount(((ToolStripMenuItem)sender!), mountable)));
                item.DropDownItems.Add(new ToolStripMenuItem("Set Path", null, (sender, args) => {}));

                contextMenu.Items.Add(item);
            }

            contextMenu.Items.Add("Exit", null, Exit);
            _trayIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ezRclone.rclone.ico")!),
                ContextMenuStrip = contextMenu,
                Visible = true
            };
        }
        private void Exit(object? sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;
            Application.Exit();
        }

        private void Mount(ToolStripMenuItem sender, Mountable mountable)
        {
            if (_mounts.ContainsKey(mountable.DriveLetter))
            {
                Process.GetProcessById(_mounts[mountable.DriveLetter]).Kill(true);
                _mounts.Remove(mountable.DriveLetter);

                sender.Text = "Mount";
                return;
            }

            var psi = new ProcessStartInfo("rclone.exe", $"mount {mountable.Remote}:\"{mountable.Path}\" {mountable.DriveLetter}:")
            {
                CreateNoWindow = true
            };

            if (!string.IsNullOrEmpty(mountable.Name))
            {
                psi.Arguments += $" --volname \"{mountable.Name}\"";
            }

            if (mountable.NetworkDrive)
            {
                psi.Arguments += " --network-mode";
            }

            psi.Arguments += " --vfs-cache-mode writes";

            var p = Process.Start(psi);
            if (p == null)
            {
                return;
            }

            sender.Text = "Unmount";
            _mounts[mountable.DriveLetter] = p.Id;
        }

        private static string GetRCloneConfigPath()
        {
            var psi = new ProcessStartInfo("rclone.exe", "config file")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            var p = Process.Start(psi);
            if (p == null)
                return string.Empty;
            
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd().Replace("Configuration file is stored at:\n", string.Empty).Trim();
        }
    }
}