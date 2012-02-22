namespace WindowsFormsApplication7
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItemfile = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemopen = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemexit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemfont = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxSize = new System.Windows.Forms.ToolStripComboBox();
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxType = new System.Windows.Forms.ToolStripComboBox();
            this.customControl11 = new WindowsFormsApplication7.CustomControl1();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemfile,
            this.ToolStripMenuItemfont});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(368, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuItemfile
            // 
            this.ToolStripMenuItemfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemopen,
            this.ToolStripMenuItemexit});
            this.ToolStripMenuItemfile.Name = "ToolStripMenuItemfile";
            this.ToolStripMenuItemfile.Size = new System.Drawing.Size(35, 20);
            this.ToolStripMenuItemfile.Text = "File";
            // 
            // ToolStripMenuItemopen
            // 
            this.ToolStripMenuItemopen.Name = "ToolStripMenuItemopen";
            this.ToolStripMenuItemopen.Size = new System.Drawing.Size(111, 22);
            this.ToolStripMenuItemopen.Text = "Open";
            this.ToolStripMenuItemopen.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // ToolStripMenuItemexit
            // 
            this.ToolStripMenuItemexit.Name = "ToolStripMenuItemexit";
            this.ToolStripMenuItemexit.Size = new System.Drawing.Size(111, 22);
            this.ToolStripMenuItemexit.Text = "Exit";
            this.ToolStripMenuItemexit.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ToolStripMenuItemfont
            // 
            this.ToolStripMenuItemfont.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sizeToolStripMenuItem,
            this.typeToolStripMenuItem});
            this.ToolStripMenuItemfont.Name = "ToolStripMenuItemfont";
            this.ToolStripMenuItemfont.Size = new System.Drawing.Size(41, 20);
            this.ToolStripMenuItemfont.Text = "Font";
            // 
            // sizeToolStripMenuItem
            // 
            this.sizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxSize});
            this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
            this.sizeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sizeToolStripMenuItem.Text = "Size";
            // 
            // toolStripComboBoxSize
            // 
            this.toolStripComboBoxSize.Items.AddRange(new object[] {
            "6",
            "8",
            "10",
            "12",
            "14",
            "16",
            "18",
            "20"});
            this.toolStripComboBoxSize.Name = "toolStripComboBoxSize";
            this.toolStripComboBoxSize.Size = new System.Drawing.Size(121, 21);
            this.toolStripComboBoxSize.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxSize_SelectedIndexChanged);
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxType});
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.typeToolStripMenuItem.Text = "Type";
            // 
            // toolStripComboBoxType
            // 
            this.toolStripComboBoxType.Items.AddRange(new object[] {
            "Arial",
            "Times New Roman",
            "Calibri",
            "Tahoma"});
            this.toolStripComboBoxType.Name = "toolStripComboBoxType";
            this.toolStripComboBoxType.Size = new System.Drawing.Size(121, 21);
            this.toolStripComboBoxType.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxType_SelectedIndexChanged);
            // 
            // customControl11
            // 
            this.customControl11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customControl11.Location = new System.Drawing.Point(0, 24);
            this.customControl11.Name = "customControl11";
            this.customControl11.Size = new System.Drawing.Size(368, 313);
            this.customControl11.TabIndex = 0;
            this.customControl11.Text = "Hello World!";
            this.customControl11.Resize += new System.EventHandler(this.customControl11_Resize);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 337);
            this.Controls.Add(this.customControl11);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControl1 customControl11;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemfile;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemopen;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemexit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemfont;
        private System.Windows.Forms.ToolStripMenuItem sizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxSize;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxType;
    }
}

