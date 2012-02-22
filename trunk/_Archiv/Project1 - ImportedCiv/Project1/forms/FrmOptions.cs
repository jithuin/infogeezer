using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for options.
	/// </summary>
	public class FrmOptions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.CheckBox cbLabels;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.CheckBox cbAutosave;
		private System.Windows.Forms.CheckBox cbSpiesCommon;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Button cmdLanguage;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.CheckBox checkBox1;
	
		public FrmOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
	/*		this.ControlBox = true;
			this.MinimizeBox = false;*/

			comboBox1.SelectedIndex = (int)Form1.options.frontierType;
			comboBox2.SelectedIndex = (int)Form1.options.miniMapType;

			checkBox3.Checked =		Form1.options.hideUndiscovered;
			checkBox2.Checked =		Form1.options.showBatteryStatus;
			checkBox1.Checked =		Form1.options.showOnScreenDPad;
			checkBox4.Checked =		Form1.options.showGrid;
			cbLabels.Checked =		Form1.options.showLabels;
			cbSpiesCommon.Checked = Form1.options.autosave;
			cbAutosave.Checked =	Form1.options.autosave;

			cmdLanguage.Enabled = language.moreThanOneLanguageFileDetected;

			platformSpec.resolution.set( this ); // .Controls );
			platformSpec.resolution.set( this.tabPage1.Controls );
			platformSpec.resolution.set( this.tabPage2.Controls );

		/*	vgaSupport.vga( this.Controls );
			vgaSupport.vga( this.tabPage1.Controls );
			vgaSupport.vga( this.tabPage2.Controls );*/

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.cbLabels = new System.Windows.Forms.CheckBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.cmdLanguage = new System.Windows.Forms.Button();
			this.cbAutosave = new System.Windows.Forms.CheckBox();
			this.cbSpiesCommon = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.checkBox6 = new System.Windows.Forms.CheckBox();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(240, 296);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.checkBox6);
			this.tabPage1.Controls.Add(this.checkBox5);
			this.tabPage1.Controls.Add(this.cbLabels);
			this.tabPage1.Controls.Add(this.checkBox4);
			this.tabPage1.Controls.Add(this.comboBox2);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.comboBox1);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.checkBox3);
			this.tabPage1.Location = new System.Drawing.Point(4, 4);
			this.tabPage1.Size = new System.Drawing.Size(232, 270);
			this.tabPage1.Text = "Map";
			// 
			// cbLabels
			// 
			this.cbLabels.Location = new System.Drawing.Point(8, 128);
			this.cbLabels.Size = new System.Drawing.Size(224, 16);
			this.cbLabels.Text = "Show labels";
			// 
			// checkBox4
			// 
			this.checkBox4.Location = new System.Drawing.Point(8, 104);
			this.checkBox4.Size = new System.Drawing.Size(224, 16);
			this.checkBox4.Text = "Show game.grid";
			// 
			// comboBox2
			// 
			this.comboBox2.Items.Add("Show terrain");
			this.comboBox2.Items.Add("Show territory");
			this.comboBox2.Location = new System.Drawing.Point(88, 40);
			this.comboBox2.Size = new System.Drawing.Size(128, 22);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 48);
			this.label2.Size = new System.Drawing.Size(80, 20);
			this.label2.Text = "Map type:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBox1
			// 
			this.comboBox1.Items.Add("Relation type");
			this.comboBox1.Items.Add("Neighbour colors");
			this.comboBox1.Location = new System.Drawing.Point(88, 8);
			this.comboBox1.Size = new System.Drawing.Size(128, 22);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 16);
			this.label1.Size = new System.Drawing.Size(80, 20);
			this.label1.Text = "Frontier type:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkBox3
			// 
			this.checkBox3.Location = new System.Drawing.Point(8, 80);
			this.checkBox3.Size = new System.Drawing.Size(224, 16);
			this.checkBox3.Text = "Hide undiscovered map";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.cmdLanguage);
			this.tabPage2.Controls.Add(this.cbAutosave);
			this.tabPage2.Controls.Add(this.cbSpiesCommon);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.checkBox2);
			this.tabPage2.Controls.Add(this.checkBox1);
			this.tabPage2.Location = new System.Drawing.Point(4, 4);
			this.tabPage2.Size = new System.Drawing.Size(232, 270);
			this.tabPage2.Text = "Misc";
			// 
			// cmdLanguage
			// 
			this.cmdLanguage.Location = new System.Drawing.Point(24, 224);
			this.cmdLanguage.Size = new System.Drawing.Size(112, 20);
			this.cmdLanguage.Text = "Change language";
			this.cmdLanguage.Click += new System.EventHandler(this.cmdLanguage_Click);
			// 
			// cbAutosave
			// 
			this.cbAutosave.Location = new System.Drawing.Point(4, 56);
			this.cbAutosave.Size = new System.Drawing.Size(228, 16);
			this.cbAutosave.Text = "Autosave every 5 turn";
			// 
			// cbSpiesCommon
			// 
			this.cbSpiesCommon.Location = new System.Drawing.Point(4, 32);
			this.cbSpiesCommon.Size = new System.Drawing.Size(224, 16);
			this.cbSpiesCommon.Text = "Hide common spies actions notification";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(22, 80);
			this.label3.Size = new System.Drawing.Size(210, 48);
			this.label3.Text = "Show directional controls on screen (applies to main window and science tree)";
			// 
			// checkBox2
			// 
			this.checkBox2.Location = new System.Drawing.Point(4, 8);
			this.checkBox2.Size = new System.Drawing.Size(224, 16);
			this.checkBox2.Text = "Show battery status";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(4, 80);
			this.checkBox1.Size = new System.Drawing.Size(16, 16);
			// 
			// checkBox5
			// 
			this.checkBox5.Location = new System.Drawing.Point(8, 152);
			this.checkBox5.Size = new System.Drawing.Size(224, 16);
			this.checkBox5.Text = "Show our plans";
			// 
			// checkBox6
			// 
			this.checkBox6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
			this.checkBox6.Location = new System.Drawing.Point(8, 176);
			this.checkBox6.Size = new System.Drawing.Size(224, 16);
			this.checkBox6.Text = "Show enemy plans";
			// 
			// options
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.tabControl1);
			this.Text = "options";
			this.Closed += new System.EventHandler(this.options_Closed);

		}
		#endregion

		private void options_Closed(object sender, EventArgs e)
		{
			Form1.options.miniMapType =				(Options.MiniMapTypes)comboBox2.SelectedIndex;
			Form1.options.frontierType =			(Options.FrontierTypes)comboBox1.SelectedIndex;
			Form1.options.hideUndiscovered =		checkBox3.Checked;
			Form1.options.showBatteryStatus =		checkBox2.Checked;
			Form1.options.showOnScreenDPad =		checkBox1.Checked;
			Form1.options.showGrid =				checkBox4.Checked;
			Form1.options.showLabels =				cbLabels.Checked;
			Form1.options.autosave =				cbSpiesCommon.Checked;
			Form1.options.autosave =				cbAutosave.Checked;
		}

		private void cmdLanguage_Click(object sender, System.EventArgs e)
		{
			language.populate( true );
		}
	}
}
