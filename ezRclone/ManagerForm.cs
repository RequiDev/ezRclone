using ezRclone;
using Microsoft.Win32;
using DataGridViewDataErrorContexts = System.Windows.Forms.DataGridViewDataErrorContexts;

namespace ezRclone
{
    public partial class ManagerForm : Form
    {
        private readonly ezRclone _rclone;

        private readonly RegistryKey _rkRun = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)!;
        public ManagerForm(ezRclone rclone, List<Mountable> mountables)
        {
            _rclone = rclone;
            InitializeComponent();

            var rkRunValue = _rkRun.GetValue("ezRclone");
            autorunOnBootToolStripMenuItem.Checked = rkRunValue != null;

            if (rkRunValue != null)
            {
                bool containsHidden = rkRunValue.ToString()!.Contains("--hidden");
                hiddenToolStripMenuItem.Checked = containsHidden;
            }

            // iterate all drive letters and add them to the list
            //for (var c = 'A'; c <= 'Z'; c++)
            //{
            //    var drive = c.ToString();
            //    if (Directory.Exists(drive + ":\\"))
            //    {
            //        continue;
            //    }


            //}

            foreach (var mountable in mountables)
            {
                mountableTable.Rows.Add(mountable.Remote, mountable.Path, mountable.DriveLetter, mountable.Name, mountable.NetworkDrive, mountable.AutoMount);
            }
        }

        public bool ShouldHide()
        {
            return hiddenToolStripMenuItem.Checked;
        }

        private void mountableTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var rowIndex = e.RowIndex;
            var mountable = _rclone.GetMountable(rowIndex);
            var row = mountableTable.Rows[rowIndex];

            if (e.ColumnIndex == clmnNetworkDrive.Index)
            {
                if (row.Cells["clmnNetworkDrive"] is DataGridViewCheckBoxCell checkbox)
                {
                    mountable.NetworkDrive = checkbox.Value as bool? ?? false;
                }
            }
            else if (e.ColumnIndex == clmnAutoMount.Index)
            {
                if (row.Cells["clmnAutoMount"] is DataGridViewCheckBoxCell checkbox)
                {
                    mountable.AutoMount = checkbox.Value as bool? ?? false;
                }
            }
            else if (e.ColumnIndex == clmnPath.Index)
            {
                if (row.Cells["clmnPath"] is DataGridViewTextBoxCell textbox)
                {
                    mountable.Path = textbox.Value as string ?? string.Empty;
                }
            }
            else if (e.ColumnIndex == clmnName.Index)
            {
                if (row.Cells["clmnName"] is DataGridViewTextBoxCell textbox)
                {
                    mountable.Name = textbox.Value as string ?? string.Empty;
                }
            }
            else if (e.ColumnIndex == clmnDrive.Index)
            {
                if (row.Cells["clmnDrive"] is DataGridViewComboBoxCell comboBox)
                {
                    mountable.DriveLetter = comboBox.Value as string ?? string.Empty;
                }
            }

            _rclone.SaveConfig();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _rclone.SaveConfig();
        }

        private void mountableTable_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (mountableTable.IsCurrentCellDirty)
            {
                mountableTable.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void autorunOnBootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autorunOnBootToolStripMenuItem.Checked = !autorunOnBootToolStripMenuItem.Checked;

            if (autorunOnBootToolStripMenuItem.Checked)
            {
                string safePath = $"\"{Application.ExecutablePath}\"";
                _rkRun.SetValue("ezRclone", $"{(hiddenToolStripMenuItem.Checked ? safePath + " --hidden" : safePath)}");
            }
            else
            {
                _rkRun.DeleteValue("ezRclone", false);
            }
        }

        private void hiddenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hiddenToolStripMenuItem.Checked = !hiddenToolStripMenuItem.Checked;

            if (autorunOnBootToolStripMenuItem.Checked)
            {
                if (hiddenToolStripMenuItem.Checked)
                {
                    _rkRun.DeleteValue("ezRclone", false);
                    _rkRun.SetValue("ezRclone", $"\"{Application.ExecutablePath}\" --hidden");
                }
                else
                {
                    _rkRun.DeleteValue("ezRclone", false);
                    _rkRun.SetValue("ezRclone", $"\"{Application.ExecutablePath}\"");
                }
            }
        }

        private void setPathMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            _rclone.SetRclonePath(dlg.SelectedPath);
        }

        private void minimizeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _rclone.Exit(sender, e);
        }

        private void mountAllMenuItem_Click(object sender, EventArgs e)
        {
            _rclone.MountAll();
        }

        private void unmountAllMenuItem_Click(object sender, EventArgs e)
        {
            _rclone.UnmountAll();
        }
    }
}