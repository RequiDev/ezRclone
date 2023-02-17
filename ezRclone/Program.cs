using System.Diagnostics;
using System.Linq;
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

    public class Settings
    {
        public string RclonePath { get; set; } = string.Empty;
        public List<Mountable> Mountables { get; set; } = new List<Mountable>();
    }

    public class ezRclone : ApplicationContext
    {
        private readonly Settings _settings = new Settings();
        private readonly Dictionary<string, int> _mounts = new Dictionary<string, int>();

        private readonly NotifyIcon _trayIcon;
        private ManagerForm? _managerForm;

        public void SaveConfig()
        {
            var settingsFile = Application.UserAppDataPath + "\\settings.json";
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(_settings, Formatting.Indented));
        }

        public ezRclone()
        {
            var contextMenu = new ContextMenuStrip();

            // check if mounts.json exists in appdata
            var settingsFile = Application.UserAppDataPath + "\\settings.json";
            if (File.Exists(settingsFile))
            {
                // load mounts.json
                _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFile)) ?? new Settings();
            }

            contextMenu.Items.Add("Manage", null, OpenManager);

            ReloadMountables();

            var rcloneProcesses = Process.GetProcessesByName("rclone");
            foreach (var p in rcloneProcesses)
            {
                foreach (var mountable in _settings.Mountables)
                {
                    var arguments = p.GetCommandLine();
                    if (arguments.Contains(mountable.Remote) && arguments.Contains("mount"))
                    {
                        _mounts.Add(mountable.Remote, p.Id);

                        p.Exited += (_, _) =>
                        {
                            _mounts.Remove(mountable.Remote); // make sure we're aware of the process being gone.
                        };
                    }
                }
            }

            foreach (var mountable in _settings.Mountables)
            {
                var item = new ToolStripMenuItem(mountable.Name)
                {
                    Name = mountable.Remote
                };

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
                        if (IsMounted(mountable))
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

                item.DropDownItems.Add(new ToolStripMenuItem("Mount", null, OnClick)
                {
                    Name = "itmMount"
                });
                // item.DropDownItems.Add(new ToolStripMenuItem("Set Path", null, (sender, args) => {}));
                
                contextMenu.Items.Add(item);

                contextMenu.Opened += (sender, args) =>
                {
                    foreach (var m in _settings.Mountables)
                    {
                        if (contextMenu.Items[m.Remote] is ToolStripMenuItem itm)
                        {
                            itm.DropDownItems["itmMount"].Text = IsMounted(m) ? "Unmount" : "Mount";
                        }
                    }
                };

                if (mountable.AutoMount && !IsMounted(mountable))
                {
                    Mount(mountable);
                }
            }

            contextMenu.Items.Add("Mount All", null, (_, _) => MountAll());
            contextMenu.Items.Add("Unmount All", null, (_, _) => UnmountAll());

            contextMenu.Items.Add("Exit", null, Exit);
            _trayIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ezRclone.rclone.ico")!),
                ContextMenuStrip = contextMenu,
                Visible = true
            };
            _trayIcon.DoubleClick += OpenManager;

            OpenManager(null, null);
        }

        private void OpenManager(object? s, EventArgs? e)
        {
            if (_managerForm == null || _managerForm.IsDisposed)
            {
                _managerForm = new ManagerForm(this, _settings.Mountables);
            }

            _managerForm.Show();
        }

        public Mountable GetMountable(int index)
        {
            return _settings.Mountables[index];
        }

        public void Exit(object? sender, EventArgs e)
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
            var psi = new ProcessStartInfo(Path.Combine(_settings.RclonePath, "rclone.exe"), $"mount {mountable.Remote}:\"{mountable.Path}\" {mountable.DriveLetter}:")
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

            if (p.WaitForExit(1000))
            {
	            return;
            }

            p.Exited += (_, _) =>
            {
                _mounts.Remove(mountable.Remote); // make sure we're aware of the process being gone.
            };

            _mounts[mountable.Remote] = p.Id;
        }

        private string? _rcloneConfigPath;
        private string GetRCloneConfigPath()
        {
            if (!string.IsNullOrEmpty(_rcloneConfigPath))
            {
                return _rcloneConfigPath;
            }

            try
            {
                var p = Process.Start(new ProcessStartInfo(Path.Combine(_settings.RclonePath, "rclone.exe"), "config file")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                });

                if (p == null)
                    return string.Empty;

                p.WaitForExit();
                _rcloneConfigPath = p.StandardOutput.ReadToEnd().Replace("Configuration file is stored at:\n", string.Empty).Trim();

                return _rcloneConfigPath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void SetRclonePath(string rclonePath)
        {
            _settings.RclonePath = rclonePath;
            SaveConfig();

            ReloadMountables();
        }

        private void ReloadMountables()
        {
            var rcloneConfigPath = GetRCloneConfigPath();
            if (!File.Exists(rcloneConfigPath))
                return;
            
            var rcloneConfig = File.ReadAllText(rcloneConfigPath);
            var toml = Toml.Parse(rcloneConfig);

            foreach (var remote in toml.Tables)
            {
                if (remote.Name == null)
                    continue;

                var remoteName = remote.Name.ToString();

                if (_settings.Mountables.FirstOrDefault(m => m.Remote == remoteName) != null)
                    continue;

                _settings.Mountables.Add(new Mountable
                {
                    AutoMount = true,
                    DriveLetter = string.Empty,
                    Name = remoteName,
                    NetworkDrive = false,
                    Path = string.Empty,
                    Remote = remoteName
                });
            }
        }

        public void MountAll()
        {
            foreach (var mountable in _settings.Mountables.Where(mountable => !IsMounted(mountable)))
            {
                Mount(mountable);
            }
        }

        public void UnmountAll()
        {
            foreach (var mountable in _settings.Mountables.Where(IsMounted))
            {
                Unmount(mountable);
            }
        }
    }
}