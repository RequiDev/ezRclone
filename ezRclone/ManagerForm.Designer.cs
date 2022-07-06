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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mountableTable = new System.Windows.Forms.DataGridView();
            this.clmnRemote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnDrive = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnNetworkDrive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmnAutoMount = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.gayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gayToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.autorunOnBootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mountableTable)).BeginInit();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.mountableTable);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(897, 336);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Remotes";
            // 
            // mountableTable
            // 
            this.mountableTable.AllowUserToAddRows = false;
            this.mountableTable.AllowUserToDeleteRows = false;
            this.mountableTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.mountableTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mountableTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnRemote,
            this.clmnPath,
            this.clmnDrive,
            this.clmnName,
            this.Column5,
            this.Column6,
            this.Column7,
            this.clmnNetworkDrive,
            this.clmnAutoMount});
            this.mountableTable.Location = new System.Drawing.Point(6, 22);
            this.mountableTable.MultiSelect = false;
            this.mountableTable.Name = "mountableTable";
            this.mountableTable.RowTemplate.Height = 25;
            this.mountableTable.Size = new System.Drawing.Size(885, 292);
            this.mountableTable.TabIndex = 0;
            this.mountableTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.mountableTable_CellValueChanged);
            // 
            // clmnRemote
            // 
            this.clmnRemote.HeaderText = "Remote Name";
            this.clmnRemote.Name = "clmnRemote";
            this.clmnRemote.Width = 99;
            // 
            // clmnPath
            // 
            this.clmnPath.HeaderText = "Remote Path";
            this.clmnPath.Name = "clmnPath";
            this.clmnPath.Width = 92;
            // 
            // clmnDrive
            // 
            this.clmnDrive.HeaderText = "Drive Letter";
            this.clmnDrive.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z"});
            this.clmnDrive.Name = "clmnDrive";
            this.clmnDrive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmnDrive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.clmnDrive.Width = 85;
            // 
            // clmnName
            // 
            this.clmnName.HeaderText = "Mounted Name";
            this.clmnName.Name = "clmnName";
            this.clmnName.Width = 106;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Time Mounted";
            this.Column5.Name = "Column5";
            this.Column5.ToolTipText = "See how long the drive has been mounted.";
            this.Column5.Width = 101;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Date Added";
            this.Column6.Name = "Column6";
            this.Column6.ToolTipText = "See when the drive was first added";
            this.Column6.Width = 87;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Remote Count";
            this.Column7.Name = "Column7";
            this.Column7.ToolTipText = "See how many times a drive was mounted.";
            // 
            // clmnNetworkDrive
            // 
            this.clmnNetworkDrive.HeaderText = "Network Drive";
            this.clmnNetworkDrive.Name = "clmnNetworkDrive";
            this.clmnNetworkDrive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmnNetworkDrive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.clmnNetworkDrive.ToolTipText = "Make the mount a network drive.";
            this.clmnNetworkDrive.Width = 98;
            // 
            // clmnAutoMount
            // 
            this.clmnAutoMount.HeaderText = "Auto Mount";
            this.clmnAutoMount.Name = "clmnAutoMount";
            this.clmnAutoMount.Width = 70;
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gayToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(916, 24);
            this.menuMain.TabIndex = 1;
            this.menuMain.Text = "menuStrip1";
            // 
            // gayToolStripMenuItem
            // 
            this.gayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gayToolStripMenuItem1,
            this.autorunOnBootToolStripMenuItem,
            this.minimizeToTrayToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.gayToolStripMenuItem.Name = "gayToolStripMenuItem";
            this.gayToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.gayToolStripMenuItem.Text = "Options";
            // 
            // gayToolStripMenuItem1
            // 
            this.gayToolStripMenuItem1.Name = "gayToolStripMenuItem1";
            this.gayToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.gayToolStripMenuItem1.Text = "Set rclone path";
            // 
            // autorunOnBootToolStripMenuItem
            // 
            this.autorunOnBootToolStripMenuItem.Name = "autorunOnBootToolStripMenuItem";
            this.autorunOnBootToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.autorunOnBootToolStripMenuItem.Text = "Autorun on boot";
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            this.minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            this.minimizeToTrayToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.minimizeToTrayToolStripMenuItem.Text = "Minimize to tray";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 372);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.MaximizeBox = false;
            this.Name = "ManagerForm";
            this.Text = "ezRclone Manager";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mountableTable)).EndInit();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private DataGridView mountableTable;
        private MenuStrip menuMain;
        private ToolStripMenuItem gayToolStripMenuItem;
        private ToolStripMenuItem gayToolStripMenuItem1;
        private ToolStripMenuItem autorunOnBootToolStripMenuItem;
        private ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private DataGridViewTextBoxColumn clmnRemote;
        private DataGridViewTextBoxColumn clmnPath;
        private DataGridViewComboBoxColumn clmnDrive;
        private DataGridViewTextBoxColumn clmnName;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewCheckBoxColumn clmnNetworkDrive;
        private DataGridViewCheckBoxColumn clmnAutoMount;
    }
}