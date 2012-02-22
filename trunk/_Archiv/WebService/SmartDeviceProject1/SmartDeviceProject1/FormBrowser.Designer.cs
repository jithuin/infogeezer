namespace SmartDeviceProject1
{
    partial class FormBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.miSoft = new System.Windows.Forms.MenuItem();
            this.miMenu = new System.Windows.Forms.MenuItem();
            this.miBack = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.miOptions = new System.Windows.Forms.MenuItem();
            this.miAbout = new System.Windows.Forms.MenuItem();
            this.miExit = new System.Windows.Forms.MenuItem();
            this.tbSiteAddres = new System.Windows.Forms.TextBox();
            this.tbSiteBody = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.miSoft);
            this.mainMenu1.MenuItems.Add(this.miMenu);
            // 
            // miSoft
            // 
            this.miSoft.Text = "OK";
            this.miSoft.Click += new System.EventHandler(this.miSoft_Click);
            // 
            // miMenu
            // 
            this.miMenu.MenuItems.Add(this.miBack);
            this.miMenu.MenuItems.Add(this.menuItem1);
            this.miMenu.MenuItems.Add(this.miOptions);
            this.miMenu.MenuItems.Add(this.miAbout);
            this.miMenu.MenuItems.Add(this.miExit);
            this.miMenu.Text = "Menu";
            // 
            // miBack
            // 
            this.miBack.Text = "Back";
            this.miBack.Click += new System.EventHandler(this.miBack_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.menuItem2);
            this.menuItem1.MenuItems.Add(this.menuItem3);
            this.menuItem1.Text = "View";
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Browser";
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "Slide";
            // 
            // miOptions
            // 
            this.miOptions.Text = "Options";
            // 
            // miAbout
            // 
            this.miAbout.Text = "About";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // miExit
            // 
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // tbSiteAddres
            // 
            this.tbSiteAddres.Location = new System.Drawing.Point(3, 3);
            this.tbSiteAddres.Name = "tbSiteAddres";
            this.tbSiteAddres.Size = new System.Drawing.Size(219, 21);
            this.tbSiteAddres.TabIndex = 0;
            // 
            // tbSiteBody
            // 
            this.tbSiteBody.Location = new System.Drawing.Point(4, 31);
            this.tbSiteBody.Multiline = true;
            this.tbSiteBody.Name = "tbSiteBody";
            this.tbSiteBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSiteBody.Size = new System.Drawing.Size(218, 268);
            this.tbSiteBody.TabIndex = 1;
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.tbSiteBody);
            this.Controls.Add(this.tbSiteAddres);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "FormBrowser";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbSiteAddres;
        private System.Windows.Forms.MenuItem miSoft;
        private System.Windows.Forms.MenuItem miMenu;
        private System.Windows.Forms.TextBox tbSiteBody;
        private System.Windows.Forms.MenuItem miBack;
        private System.Windows.Forms.MenuItem miExit;
        private System.Windows.Forms.MenuItem miAbout;
        private System.Windows.Forms.MenuItem miOptions;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
    }
}

