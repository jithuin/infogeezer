namespace SmartDeviceProject2
{
	partial class Form2
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
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.customControl11 = new SmartDeviceProject2.RichTextControl();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add(this.menuItem1);
			this.mainMenu1.MenuItems.Add(this.menuItem2);
			// 
			// menuItem1
			// 
			this.menuItem1.MenuItems.Add(this.menuItem3);
			this.menuItem1.MenuItems.Add(this.menuItem4);
			this.menuItem1.Text = "Change";
			// 
			// menuItem3
			// 
			this.menuItem3.Text = "egyik";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "masik";
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "Exit";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// customControl11
			// 
			this.customControl11.Location = new System.Drawing.Point(4, 4);
			this.customControl11.Name = "customControl11";
			this.customControl11.Size = new System.Drawing.Size(202, 120);
			this.customControl11.TabIndex = 0;
			this.customControl11.Text = "Hello world";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(4, 131);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(212, 21);
			this.textBox1.TabIndex = 1;
			// 
			// listBox1
			// 
			this.listBox1.Items.Add("1");
			this.listBox1.Items.Add("2");
			this.listBox1.Items.Add("3");
			this.listBox1.Items.Add("4");
			this.listBox1.Items.Add("5");
			this.listBox1.Items.Add("6");
			this.listBox1.Items.Add("7");
			this.listBox1.Items.Add("8");
			this.listBox1.Items.Add("9");
			this.listBox1.Items.Add("10");
			this.listBox1.Location = new System.Drawing.Point(4, 158);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(233, 86);
			this.listBox1.TabIndex = 2;
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(240, 268);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.customControl11);
			this.Menu = this.mainMenu1;
			this.Name = "Form2";
			this.Text = "Form2";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private RichTextControl customControl11;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ListBox listBox1;
	}
}