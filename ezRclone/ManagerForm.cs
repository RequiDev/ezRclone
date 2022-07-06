using ezRclone;

namespace ezRclone
{
    public partial class ManagerForm : Form
    {
        public ManagerForm(List<Mountable> mountables)
        {
            InitializeComponent();

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
            if (e.ColumnIndex == clmnNetworkDrive.Index)
            {

            }
        }
    }
}