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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mountableTable = new System.Windows.Forms.DataGridView();
            this.clmnRemote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnDrive = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnNetworkDrive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmnAutoMount = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.menuGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.gayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setPathMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autorunOnBootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mountableTable)).BeginInit();
            this.menuGrid.SuspendLayout();
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
            this.groupBox1.Size = new System.Drawing.Size(612, 336);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Remotes";
            // 
            // mountableTable
            // 
            this.mountableTable.AllowUserToAddRows = false;
            this.mountableTable.AllowUserToDeleteRows = false;
            this.mountableTable.AllowUserToResizeColumns = false;
            this.mountableTable.AllowUserToResizeRows = false;
            this.mountableTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.mountableTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mountableTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnRemote,
            this.clmnPath,
            this.clmnDrive,
            this.clmnName,
            this.clmnNetworkDrive,
            this.clmnAutoMount});
            this.mountableTable.ContextMenuStrip = this.menuGrid;
            this.mountableTable.Location = new System.Drawing.Point(6, 22);
            this.mountableTable.MultiSelect = false;
            this.mountableTable.Name = "mountableTable";
            this.mountableTable.RowTemplate.Height = 25;
            this.mountableTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.mountableTable.Size = new System.Drawing.Size(600, 292);
            this.mountableTable.TabIndex = 0;
            this.mountableTable.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.mountableTable_CellMouseDown);
            this.mountableTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.mountableTable_CellValueChanged);
            this.mountableTable.CurrentCellDirtyStateChanged += new System.EventHandler(this.mountableTable_CurrentCellDirtyStateChanged);
            // 
            // clmnRemote
            // 
            this.clmnRemote.HeaderText = "Remote Name";
            this.clmnRemote.Name = "clmnRemote";
            this.clmnRemote.ReadOnly = true;
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
            // menuGrid
            // 
            this.menuGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mountToolStripMenuItem});
            this.menuGrid.Name = "menuGrid";
            this.menuGrid.Size = new System.Drawing.Size(111, 26);
            // 
            // mountToolStripMenuItem
            // 
            this.mountToolStripMenuItem.Name = "mountToolStripMenuItem";
            this.mountToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.mountToolStripMenuItem.Text = "Mount";
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gayToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(630, 24);
            this.menuMain.TabIndex = 1;
            this.menuMain.Text = "menuStrip1";
            // 
            // gayToolStripMenuItem
            // 
            this.gayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setPathMenuItem,
            this.autorunOnBootToolStripMenuItem,
            this.minimizeToTrayToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.gayToolStripMenuItem.Name = "gayToolStripMenuItem";
            this.gayToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.gayToolStripMenuItem.Text = "Options";
            // 
            // setPathMenuItem
            // 
            this.setPathMenuItem.Name = "setPathMenuItem";
            this.setPathMenuItem.Size = new System.Drawing.Size(180, 22);
            this.setPathMenuItem.Text = "Set rclone path";
            this.setPathMenuItem.Click += new System.EventHandler(this.setPathMenuItem_Click);
            // 
            // autorunOnBootToolStripMenuItem
            // 
            this.autorunOnBootToolStripMenuItem.Name = "autorunOnBootToolStripMenuItem";
            this.autorunOnBootToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.autorunOnBootToolStripMenuItem.Text = "Autostart";
            this.autorunOnBootToolStripMenuItem.Click += new System.EventHandler(this.autorunOnBootToolStripMenuItem_Click);
            // 
            // minimizeToTrayToolStripMenuItem
            // 
            this.minimizeToTrayToolStripMenuItem.Name = "minimizeToTrayToolStripMenuItem";
            this.minimizeToTrayToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.minimizeToTrayToolStripMenuItem.Text = "Minimize to tray";
            this.minimizeToTrayToolStripMenuItem.Click += new System.EventHandler(this.minimizeToTrayToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 372);
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
            this.menuGrid.ResumeLayout(false);
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
        private ToolStripMenuItem setPathMenuItem;
        private ToolStripMenuItem autorunOnBootToolStripMenuItem;
        private ToolStripMenuItem minimizeToTrayToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ContextMenuStrip menuGrid;
        private ToolStripMenuItem mountToolStripMenuItem;
        private DataGridViewTextBoxColumn clmnRemote;
        private DataGridViewTextBoxColumn clmnPath;
        private DataGridViewComboBoxColumn clmnDrive;
        private DataGridViewTextBoxColumn clmnName;
        private DataGridViewCheckBoxColumn clmnNetworkDrive;
        private DataGridViewCheckBoxColumn clmnAutoMount;
    }
}