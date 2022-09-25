using ezRclone;
using DataGridViewDataErrorContexts = System.Windows.Forms.DataGridViewDataErrorContexts;

namespace ezRclone
{
    public partial class ManagerForm : Form
    {
        private readonly ezRclone _rclone;

        private readonly List<int> _checkboxes = new List<int>();

        public ManagerForm(ezRclone rclone, List<Mountable> mountables)
        {
            _rclone = rclone;
            InitializeComponent();

            _checkboxes.Add(clmnAutoMount.Index);
            _checkboxes.Add(clmnNetworkDrive.Index);

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
                mountableTable.Rows.Add(mountable.Remote, mountable.Path, mountable.DriveLetter, mountable.Name, DateTime.Now, DateTime.Now, 0, mountable.NetworkDrive, mountable.AutoMount);
            }
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
    }
}