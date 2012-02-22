namespace WindowsFormsApplication3
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.components = new System.ComponentModel.Container();
            this.fdOpen = new System.Windows.Forms.OpenFileDialog();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.chFileName = new System.Windows.Forms.ColumnHeader();
            this.cmLvFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiInvertChecking = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiMakeChecked = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiDeleteSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiDeleteChecked = new System.Windows.Forms.ToolStripMenuItem();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miNew = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.miMakeAll = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmLvFiles.SuspendLayout();
            this.msMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fdOpen
            // 
            this.fdOpen.DefaultExt = "MEO";
            this.fdOpen.FileName = "openFileDialog1";
            this.fdOpen.Filter = "MEO files|*.MEO|All files|*.*";
            this.fdOpen.Multiselect = true;
            // 
            // lvFiles
            // 
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFileName});
            this.lvFiles.ContextMenuStrip = this.cmLvFiles;
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.Location = new System.Drawing.Point(0, 24);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(458, 378);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // chFileName
            // 
            this.chFileName.Text = "FileName";
            this.chFileName.Width = 433;
            // 
            // cmLvFiles
            // 
            this.cmLvFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiCheck,
            this.cmiInvertChecking,
            this.cmiMakeChecked,
            this.cmiDeleteSelected,
            this.cmiDeleteChecked});
            this.cmLvFiles.Name = "cmLvFiles";
            this.cmLvFiles.Size = new System.Drawing.Size(162, 114);
            // 
            // cmiCheck
            // 
            this.cmiCheck.Name = "cmiCheck";
            this.cmiCheck.Size = new System.Drawing.Size(161, 22);
            this.cmiCheck.Text = "Check";
            this.cmiCheck.Click += new System.EventHandler(this.cmiCheck_Click);
            // 
            // cmiInvertChecking
            // 
            this.cmiInvertChecking.Name = "cmiInvertChecking";
            this.cmiInvertChecking.Size = new System.Drawing.Size(161, 22);
            this.cmiInvertChecking.Text = "Invert Checking";
            this.cmiInvertChecking.Click += new System.EventHandler(this.cmiInvertChecking_Click);
            // 
            // cmiMakeChecked
            // 
            this.cmiMakeChecked.Name = "cmiMakeChecked";
            this.cmiMakeChecked.Size = new System.Drawing.Size(161, 22);
            this.cmiMakeChecked.Text = "Make Checked";
            this.cmiMakeChecked.Click += new System.EventHandler(this.cmiMakeChecked_Click);
            // 
            // cmiDeleteSelected
            // 
            this.cmiDeleteSelected.Name = "cmiDeleteSelected";
            this.cmiDeleteSelected.Size = new System.Drawing.Size(161, 22);
            this.cmiDeleteSelected.Text = "Delete Selected";
            this.cmiDeleteSelected.Click += new System.EventHandler(this.cmiDeleteSelected_Click);
            // 
            // cmiDeleteChecked
            // 
            this.cmiDeleteChecked.Name = "cmiDeleteChecked";
            this.cmiDeleteChecked.Size = new System.Drawing.Size(161, 22);
            this.cmiDeleteChecked.Text = "Delete Checked";
            this.cmiDeleteChecked.Click += new System.EventHandler(this.cmiDeleteChecked_Click);
            // 
            // msMain
            // 
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.miEdit});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(458, 24);
            this.msMain.TabIndex = 1;
            this.msMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNew,
            this.miOpen,
            this.miExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // miNew
            // 
            this.miNew.Name = "miNew";
            this.miNew.Size = new System.Drawing.Size(159, 22);
            this.miNew.Text = "New";
            this.miNew.Click += new System.EventHandler(this.miNew_Click);
            // 
            // miOpen
            // 
            this.miOpen.Name = "miOpen";
            this.miOpen.Size = new System.Drawing.Size(159, 22);
            this.miOpen.Text = "Open Source...";
            this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(159, 22);
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // miEdit
            // 
            this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMakeAll});
            this.miEdit.Name = "miEdit";
            this.miEdit.Size = new System.Drawing.Size(37, 20);
            this.miEdit.Text = "Edit";
            // 
            // miMakeAll
            // 
            this.miMakeAll.Name = "miMakeAll";
            this.miMakeAll.Size = new System.Drawing.Size(136, 22);
            this.miMakeAll.Text = "Make All...";
            this.miMakeAll.Click += new System.EventHandler(this.miMakeAll_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 402);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(458, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(38, 17);
            this.lbStatus.Text = "Ready";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 424);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.msMain);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.msMain;
            this.Name = "Form1";
            this.Text = "Form1";
            this.cmLvFiles.ResumeLayout(false);
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog fdOpen;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader chFileName;
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miNew;
        private System.Windows.Forms.ToolStripMenuItem miOpen;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
        private System.Windows.Forms.ToolStripMenuItem miMakeAll;
        private System.Windows.Forms.ContextMenuStrip cmLvFiles;
        private System.Windows.Forms.ToolStripMenuItem cmiCheck;
        private System.Windows.Forms.ToolStripMenuItem cmiInvertChecking;
        private System.Windows.Forms.ToolStripMenuItem cmiMakeChecked;
        private System.Windows.Forms.ToolStripMenuItem cmiDeleteChecked;
        private System.Windows.Forms.ToolStripMenuItem cmiDeleteSelected;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
    }
}

