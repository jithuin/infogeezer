namespace SmartDeviceProject1
{
    partial class FormStart
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
            this.miStart = new System.Windows.Forms.MenuItem();
            this.miBrowser = new System.Windows.Forms.MenuItem();
            this.miSlide = new System.Windows.Forms.MenuItem();
            this.miExit = new System.Windows.Forms.MenuItem();
            this.lbStartText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.miStart);
            this.mainMenu1.MenuItems.Add(this.miExit);
            // 
            // miStart
            // 
            this.miStart.MenuItems.Add(this.miBrowser);
            this.miStart.MenuItems.Add(this.miSlide);
            this.miStart.Text = "Start";
            // 
            // miBrowser
            // 
            this.miBrowser.Text = "Browser";
            this.miBrowser.Click += new System.EventHandler(this.miBrowser_Click);
            // 
            // miSlide
            // 
            this.miSlide.Text = "Slide";
            this.miSlide.Click += new System.EventHandler(this.miSlide_Click);
            // 
            // miExit
            // 
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // lbStartText
            // 
            this.lbStartText.Location = new System.Drawing.Point(4, 4);
            this.lbStartText.Name = "lbStartText";
            this.lbStartText.Size = new System.Drawing.Size(233, 264);
            this.lbStartText.Text = "SW Version";
            // 
            // FormStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lbStartText);
            this.Menu = this.mainMenu1;
            this.Name = "FormStart";
            this.Text = "FormStart";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem miStart;
        private System.Windows.Forms.MenuItem miBrowser;
        private System.Windows.Forms.MenuItem miSlide;
        private System.Windows.Forms.MenuItem miExit;
        private System.Windows.Forms.Label lbStartText;
    }
}