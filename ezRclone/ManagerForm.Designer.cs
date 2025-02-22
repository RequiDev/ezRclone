namespace ezRclone
{
    partial class ManagerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
            groupBox1 = new System.Windows.Forms.GroupBox();
            mountableTable = new System.Windows.Forms.DataGridView();
            clmnRemote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clmnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clmnDrive = new System.Windows.Forms.DataGridViewComboBoxColumn();
            clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clmnNetworkDrive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            clmnAutoMount = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            menuMain = new System.Windows.Forms.MenuStrip();
            gayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mountAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            unmountAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            setPathMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            autorunOnBootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            minimizeToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mountableTable).BeginInit();
            menuMain.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(mountableTable);
            groupBox1.Location = new System.Drawing.Point(12, 27);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(612, 336);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Remotes";
            // 
            // mountableTable
            // 
            mountableTable.AllowUserToAddRows = false;
            mountableTable.AllowUserToDeleteRows = false;
            mountableTable.AllowUserToResizeColumns = false;
            mountableTable.AllowUserToResizeRows = false;
            mountableTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            mountableTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            mountableTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { clmnRemote, clmnPath, clmnDrive, clmnName, clmnNetworkDrive, clmnAutoMount });
            mountableTable.Location = new System.Drawing.Point(6, 22);
            mountableTable.MultiSelect = false;
            mountableTable.Name = "mountableTable";
            mountableTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            mountableTable.Size = new System.Drawing.Size(600, 292);
            mountableTable.TabIndex = 0;
            mountableTable.CellValueChanged += mountableTable_CellValueChanged;
            mountableTable.CurrentCellDirtyStateChanged += mountableTable_CurrentCellDirtyStateChanged;
            // 
            // clmnRemote
            // 
            clmnRemote.HeaderText = "Remote Name";
            clmnRemote.Name = "clmnRemote";
            clmnRemote.ReadOnly = true;
            clmnRemote.Width = 99;
            // 
            // clmnPath
            // 
            clmnPath.HeaderText = "Remote Path";
            clmnPath.Name = "clmnPath";
            clmnPath.Width = 92;
            // 
            // clmnDrive
            // 
            clmnDrive.HeaderText = "Drive Letter";
            clmnDrive.Items.AddRange(new object[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });
            clmnDrive.Name = "clmnDrive";
            clmnDrive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            clmnDrive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            clmnDrive.Width = 85;
            // 
            // clmnName
            // 
            clmnName.HeaderText = "Mounted Name";
            clmnName.Name = "clmnName";
            clmnName.Width = 106;
            // 
            // clmnNetworkDrive
            // 
            clmnNetworkDrive.HeaderText = "Network Drive";
            clmnNetworkDrive.Name = "clmnNetworkDrive";
            clmnNetworkDrive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            clmnNetworkDrive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            clmnNetworkDrive.ToolTipText = "Make the mount a network drive.";
            clmnNetworkDrive.Width = 98;
            // 
            // clmnAutoMount
            // 
            clmnAutoMount.HeaderText = "Auto Mount";
            clmnAutoMount.Name = "clmnAutoMount";
            clmnAutoMount.Width = 70;
            // 
            // menuMain
            // 
            menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { gayToolStripMenuItem });
            menuMain.Location = new System.Drawing.Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new System.Drawing.Size(630, 24);
            menuMain.TabIndex = 1;
            menuMain.Text = "menuStrip1";
            // 
            // gayToolStripMenuItem
            // 
            gayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mountAllMenuItem, unmountAllMenuItem, setPathMenuItem, autorunOnBootToolStripMenuItem, saveToolStripMenuItem, minimizeToTrayToolStripMenuItem, exitToolStripMenuItem });
            gayToolStripMenuItem.Name = "gayToolStripMenuItem";
            gayToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            gayToolStripMenuItem.Text = "Options";
            // 
            // mountAllMenuItem
            // 
            mountAllMenuItem.Name = "mountAllMenuItem";
            mountAllMenuItem.Size = new System.Drawing.Size(160, 22);
            mountAllMenuItem.Text = "Mount All";
            mountAllMenuItem.Click += mountAllMenuItem_Click;
            // 
            // unmountAllMenuItem
            // 
            unmountAllMenuItem.Name = "unmountAllMenuItem";
            unmountAllMenuItem.Size = new System.Drawing.Size(160, 22);
            unmountAllMenuItem.Text = "Unmount All";
            unmountAllMenuItem.Click += unmountAllMenuItem_Click;
            // 
            // setPathMenuItem
            // 
            setPathMenuItem.Name = "setPathMenuItem";
            setPathMenuItem.Size = new System.Drawing.Size(160, 22);
            setPathMenuItem.Text = "Set rclone path";
            setPathMenuItem.Click += setPathMenuItem_Click;
            // 
            // autorunOnBootToolStripMenuItem
            // 
            autorunOnBootToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { hiddenToolStripMenuItem });
            autorunOnBootToolStripMenuItem.Name = "autorunOnBootToolStripMenuItem";
            autorunOnBootToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            autorunOnBootToolStripMenuItem.Text = "Autostart";
            autorunOnBootToolStripMenuItem.Click += autorunOnBootToolStripMenuItem_Click;
            // 
            // hiddenToolStripMenuItem
            // 
            hiddenToolStripMenuItem.Name = "hiddenToolStripMenuItem";
            hiddenToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            hiddenToolStripMenuItem.Text = "Hidden";
            hiddenToolStripMenuItem.Click += hiddenToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            minimizeToTrayToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            minimizeToTrayToolStripMenuItem.Text = "Minimize to tray";
            minimizeToTrayToolStripMenuItem.Click += minimizeToTrayToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // ManagerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(630, 372);
            Controls.Add(groupBox1);
            Controls.Add(menuMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
            MainMenuStrip = menuMain;
            MaximizeBox = false;
            Text = "ezRclone Manager";
            Load += ManagerForm_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mountableTable).EndInit();
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private DataGridView mountableTable;
        private MenuStrip menuMain;
        private ToolStripMenuItem gayToolStripMenuItem;
        private ToolStripMenuItem setPathMenuItem;
        private ToolStripMenuItem autorunOnBootToolStripMenuItem;
        private ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private DataGridViewTextBoxColumn clmnRemote;
        private DataGridViewTextBoxColumn clmnPath;
        private DataGridViewComboBoxColumn clmnDrive;
        private DataGridViewTextBoxColumn clmnName;
        private DataGridViewCheckBoxColumn clmnNetworkDrive;
        private DataGridViewCheckBoxColumn clmnAutoMount;
        private ToolStripMenuItem mountAllMenuItem;
        private ToolStripMenuItem unmountAllMenuItem;
        private ToolStripMenuItem hiddenToolStripMenuItem;
    }
}