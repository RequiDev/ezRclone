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
        }

    }

    public class ezRclone : ApplicationContext
    {
        private readonly List<Mountable> _mountables = new List<Mountable>();
        private readonly Dictionary<string, int> _mounts = new Dictionary<string, int>();

        private readonly NotifyIcon _trayIcon;
        private ManagerForm? _managerForm;

        public void SaveConfig()
        {
            var mountsFile = Application.UserAppDataPath + "\\mounts.json";
            File.WriteAllText(mountsFile, JsonConvert.SerializeObject(_mountables));
        }

        public ezRclone()
        {
            var contextMenu = new ContextMenuStrip();

            // check if mounts.json exists in appdata
            var mountsFile = Application.UserAppDataPath + "\\mounts.json";
            if (File.Exists(mountsFile))
            {
                // load mounts.json
                _mountables = JsonConvert.DeserializeObject<List<Mountable>>(File.ReadAllText(mountsFile)) ?? new List<Mountable>();
            }

            var rcloneConfig = File.ReadAllText(GetRCloneConfigPath());
            var toml = Toml.Parse(rcloneConfig);

            foreach (var remote in toml.Tables)
            {
                if (remote.Name == null)
                    continue;

                var remoteName = remote.Name.ToString();

                if (_mountables.FirstOrDefault(m => m.Remote == remoteName) != null)
                    continue;

                _mountables.Add(new Mountable
                {
                    AutoMount = true,
                    DriveLetter = string.Empty,
                    Mounted = false,
                    Name = remoteName,
                    NetworkDrive = false,
                    Path = string.Empty,
                    Remote = remoteName
                });
            }

            contextMenu.Items.Add("Manage", null, OpenManager);

            foreach (var mountable in _mountables)
            {
                var item = new ToolStripMenuItem(mountable.Name);

                void OnClick(object? sender, EventArgs args)
                {
                    if (string.IsNullOrEmpty(mountable.DriveLetter))
                    {
                        MessageBox.Show("Can't mount drive without a letter.", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    if (sender is ToolStripMenuItem toolstripMenuItem)
                    {
                        if (_mounts.ContainsKey(mountable.Remote))
                        {
                            Unmount(mountable);
                            toolstripMenuItem.Text = "Mount";
                        }
                        else
                        {
                            Mount(mountable);
                            toolstripMenuItem.Text = "Unmount";
                        }
                    }
                }

                item.DropDownItems.Add(new ToolStripMenuItem("Mount", null, OnClick));
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
            _trayIcon.DoubleClick += OpenManager;
        }

        private void OpenManager(object? s, EventArgs e)
        {
            if (_managerForm == null || _managerForm.IsDisposed)
            {
                _managerForm = new ManagerForm(this, _mountables);
            }

            _managerForm.Show();
        }

        public Mountable GetMountable(int index)
        {
            return _mountables[index];
        }

        private void Exit(object? sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;
            Application.Exit();
        }

        public bool IsMounted(Mountable mountable)
        {
            return _mounts.ContainsKey(mountable.Remote);
        }

        public void Unmount(Mountable mountable)
        {
            Process.GetProcessById(_mounts[mountable.Remote]).Kill(true);
            _mounts.Remove(mountable.Remote);
        }

        public void Mount(Mountable mountable)
        {
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

            _mounts[mountable.Remote] = p.Id;
        }

        private string _rcloneConfigPath;
        private string GetRCloneConfigPath()
        {
            if (!string.IsNullOrEmpty(_rcloneConfigPath))
            {
                return _rcloneConfigPath;
            }
            
            var psi = new ProcessStartInfo("rclone.exe", "config file")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            var p = Process.Start(psi);
            if (p == null)
                return string.Empty;
                
            p.WaitForExit();
            _rcloneConfigPath = p.StandardOutput.ReadToEnd().Replace("Configuration file is stored at:\n", string.Empty).Trim();

            return _rcloneConfigPath;
        }
    }
}