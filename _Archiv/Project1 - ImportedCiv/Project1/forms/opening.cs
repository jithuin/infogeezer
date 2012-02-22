
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for opening.
	/// </summary>
	public class opening : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pbBack;
		private System.Windows.Forms.Panel panel1, panel2;

		private System.Windows.Forms.TextBox tbPlayerName;
		private System.Windows.Forms.ComboBox cbYourNation;

		private System.Windows.Forms.TrackBar 
			tbWorldAge, tbPercWater, tbContinentSize, tbMapSize, tbDifficulty, tbNbrAis;

		private System.Windows.Forms.Label 
			lblWorldAge, lblWorldAgeRight, tbContinentSizeRight, lblPercWaterRight, lblContinentSize, lblMapSize,
			lblPercOfWater, lblDifficulty, lbYourName, lbYourNation, lblNbrAis, lblMapSizeRight;

		private Button 
			cmdNewGame, cmdLoadGame, cmdExit, cmdLoadMap, 
			butGenerateMap, butChooseAiNations, butFirstNextButton, butBackSecond, butFirstBackButton, cmdContinue;

		private System.Windows.Forms.CheckBox cbNiceBeginning, cbTutorialMode;
	//	private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;

	#region progress
		public static System.Windows.Forms.Label lblNewGameInfo = new System.Windows.Forms.Label();
		public static System.Windows.Forms.Panel panel3 = new Panel();
		public static System.Windows.Forms.ProgressBar progressBar1 = new System.Windows.Forms.ProgressBar(); //;
	#endregion

		string savePath;
		int width, height;
		bool playerNameModified/*, inGame*/;
		Form1 parent;

		string cnName, cnDesc;
		Color cnColor;
		byte cnImportCityListFrom;

		public opening( Form1 parent1/*, bool InGame*/ ) //  
		{
			this.ClientSize = new Size( 240, 320 );
	//		this.Menu = new MainMenu();

			parent = parent1;

            this.ControlBox = false;

			InitializeComponent();

			cmdNewGame.Text =	language.getAString( language.order.openingNewGame );
			cmdLoadGame.Text =	language.getAString( language.order.openingLoadGame );
			cmdLoadMap.Text =	language.getAString( language.order.openingLoadMap );
			cmdExit.Text =		language.getAString( language.order.openingExit );
			cmdContinue.Text =	language.getAString( language.order.openingContinue );

// panel1
			lbYourName.Text = language.getAString( language.order.nGYourName );
			lbYourNation.Text = language.getAString( language.order.nGYourNation );
			lblDifficulty.Text = language.getAString( language.order.nGDifficulty );

			butFirstBackButton.Text = language.getAString( language.order.nGBack );
			butFirstNextButton.Text = language.getAString( language.order.nGNext );
			
			cbNiceBeginning.Text = language.getAString( language.order.nGHaveANiceBeginningSite );
			cbTutorialMode.Text = language.getAString( language.order.nGTutorialMode );

// panel2
			butBackSecond.Text = language.getAString( language.order.nGBack );
			butGenerateMap.Text = language.getAString( language.order.nGGenerateMap );
			
			lblMapSize.Text = language.getAString( language.order.nGMapSize );
			lblContinentSize.Text = language.getAString( language.order.nGContinentSize );
			lblPercOfWater.Text = language.getAString( language.order.nGPercOfWater );
			lblWorldAge.Text = language.getAString( language.order.nGWorldAge );

// 

// end panels

			tbMapSize.Value = 1;
			tbContinentSize.Value = 1;

			tbWorldAge.Value = 0;
			tbWorldAge.Value = 1;

			tbNbrAis.Value = 8; // ais

			#region progress
			// 
			// panel3
			// 
			panel3.Location = new System.Drawing.Point( 8 * platformSpec.resolution.mod, 200 * platformSpec.resolution.mod );
			panel3.Size = new System.Drawing.Size( 224 * platformSpec.resolution.mod, 48 * platformSpec.resolution.mod );
			panel3.Visible = false;
			panel3.Parent = this;
			panel3.BringToFront();
			// 
			// lblNewGameInfo
			// 
			lblNewGameInfo.Location = new System.Drawing.Point( 16 * platformSpec.resolution.mod, 4 * platformSpec.resolution.mod );
			lblNewGameInfo.Size = new System.Drawing.Size( panel3.Width * 7 / 8, 16 * platformSpec.resolution.mod );
			lblNewGameInfo.Text = "";
			panel3.Controls.Add( progressBar1 );
			// 
			// progressBar1
			// 
			progressBar1.Location = new System.Drawing.Point( panel3.Width / 20, 22 * platformSpec.resolution.mod );
			progressBar1.Size = new System.Drawing.Size( panel3.Width * 18 / 20, 20 * platformSpec.resolution.mod );
			panel3.Controls.Add( lblNewGameInfo );
			#endregion

			playerNameModified = false;
			tbMapSize.Value = 0;

		/*	vgaSupport.vga( this.Controls );
			vgaSupport.vga( this.panel1.Controls );
			vgaSupport.vga( this.panel2.Controls );
			vgaSupport.vga( panel3.Controls );*/

			/*inGame = InGame;

			if ( inGame )
			{
				cmdContinue.Text = "Return to game"; 
				cmdContinue.Click += new EventHandler( returnToGame_Click);
			}
			else*/
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
			int space = 8 * platformSpec.resolution.mod;

			this.pbBack = new System.Windows.Forms.PictureBox();
			this.cmdNewGame = new Button();
			this.cmdLoadGame = new Button();
			this.cmdExit = new Button();
			this.cmdLoadMap = new Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.cbTutorialMode = new System.Windows.Forms.CheckBox();
			this.lblWorldAge = new System.Windows.Forms.Label();
			this.cbNiceBeginning = new System.Windows.Forms.CheckBox();
			this.lblMapSizeRight = new System.Windows.Forms.Label();
			this.butBackSecond = new Button();
			this.butGenerateMap = new Button();
			this.lblWorldAgeRight = new System.Windows.Forms.Label();
			this.tbWorldAge = new System.Windows.Forms.TrackBar();
			this.tbContinentSizeRight = new System.Windows.Forms.Label();
			this.lblPercWaterRight = new System.Windows.Forms.Label();
			this.tbPercWater = new System.Windows.Forms.TrackBar();
			this.tbContinentSize = new System.Windows.Forms.TrackBar();
			this.lblContinentSize = new System.Windows.Forms.Label();
			this.lblMapSize = new System.Windows.Forms.Label();
			this.lblPercOfWater = new System.Windows.Forms.Label();
			this.tbMapSize = new System.Windows.Forms.TrackBar();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tbNbrAis = new System.Windows.Forms.TrackBar();
			this.lblNbrAis = new System.Windows.Forms.Label();
			this.butFirstBackButton = new Button();
			this.lblDifficulty = new System.Windows.Forms.Label();
			this.tbPlayerName = new System.Windows.Forms.TextBox();
			this.cbYourNation = new System.Windows.Forms.ComboBox();
			this.lbYourNation = new System.Windows.Forms.Label();
			this.tbDifficulty = new System.Windows.Forms.TrackBar();
			this.lbYourName = new System.Windows.Forms.Label();
			this.butChooseAiNations = new Button();
			this.butFirstNextButton = new Button();
		//	this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
			this.cmdContinue = new Button();
			// 
			// pbBack
			// 
			this.pbBack.Size = this.ClientSize;

			#region main

			// 
			// cmdContinue
			// 
			this.cmdContinue.Enabled = false;
			this.cmdContinue.Height = cmdContinue.Height * 4 / 3 * platformSpec.resolution.mod;
			this.cmdContinue.Width = this.ClientSize.Width / 2;
			this.cmdContinue.Location = new System.Drawing.Point( this.ClientSize.Width / 4, this.ClientSize.Height * 3 / 8 - this.cmdContinue.Height / 2 );
			this.cmdContinue.Text = "Continue";
			this.cmdContinue.Click += new System.EventHandler(this.cmdContinue_Click);
			// 
			// cmdNewGame
			// 
			this.cmdNewGame.Height = this.cmdContinue.Height;
			this.cmdNewGame.Width = this.ClientSize.Width / 2;
			this.cmdNewGame.Location = new System.Drawing.Point( this.ClientSize.Width / 4, this.ClientSize.Height * 4 / 8 - this.cmdContinue.Height / 2 );
			this.cmdNewGame.Text = "New game...";
			this.cmdNewGame.Click += new System.EventHandler(this.cmdNewGame_Click);
			// 
			// cmdLoadMap
			// 
			this.cmdLoadMap.Enabled = false;
			this.cmdLoadMap.Height = this.cmdContinue.Height;
			this.cmdLoadMap.Location = new System.Drawing.Point( this.ClientSize.Width / 4, this.ClientSize.Height * 5 / 8 - this.cmdContinue.Height / 2 );
			this.cmdLoadMap.Width = this.ClientSize.Width / 2;
			this.cmdLoadMap.Text = "Load map...";
			this.cmdLoadMap.Click += new System.EventHandler(this.cmdLoadMap_Click);
			// 
			// cmdLoadGame
			// 
			this.cmdLoadGame.Height =  this.cmdContinue.Height;
			this.cmdLoadGame.Width = this.ClientSize.Width / 2;
			this.cmdLoadGame.Location = new System.Drawing.Point( this.ClientSize.Width / 4, this.ClientSize.Height * 6 / 8 - this.cmdContinue.Height / 2 );
			this.cmdLoadGame.Text = "Load game...";
			this.cmdLoadGame.Click += new System.EventHandler(this.cmdLoadGame_Click);
			// 
			// cmdExit
			// 
			this.cmdExit.Height = this.cmdContinue.Height;
			this.cmdExit.Width = this.ClientSize.Width / 2;
			this.cmdExit.Location = new System.Drawing.Point( this.ClientSize.Width / 4, this.ClientSize.Height * 7 / 8 - this.cmdContinue.Height / 2 );
			this.cmdExit.Text = "Exit";
			this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);

			#endregion

			#region panel 1
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.cbTutorialMode);
			this.panel1.Controls.Add(this.cbNiceBeginning);
			this.panel1.Controls.Add(this.tbNbrAis);
			this.panel1.Controls.Add(this.lblNbrAis);
			this.panel1.Controls.Add(this.butFirstBackButton);
			this.panel1.Controls.Add(this.lblDifficulty);
			this.panel1.Controls.Add(this.tbPlayerName);
			this.panel1.Controls.Add(this.cbYourNation);
			this.panel1.Controls.Add(this.lbYourNation);
			this.panel1.Controls.Add(this.tbDifficulty);
			this.panel1.Controls.Add(this.lbYourName);
	//		this.panel1.Controls.Add(this.butChooseAiNations);
			this.panel1.Controls.Add(this.butFirstNextButton);
			this.panel1.Location = new System.Drawing.Point(0, 48);
			this.panel1.Size = this.ClientSize;
			this.panel1.Visible = false;

			// 
			// lbYourName
			// 
	//		this.tbContinentSize.Location = new System.Drawing.Point( this.ClientSize.Width * 3 / 7, lblContinentSize.Top );
	//		this.tbContinentSize.Width = this.ClientSize.Width * 2 / 7;

			this.lbYourName.Location = new System.Drawing.Point( space, space );
			this.lbYourName.Width = ( this.ClientSize.Width - 3*space )/2;
			this.lbYourName.Height *= platformSpec.resolution.mod;

		//	this.lbYourName.Location = new System.Drawing.Point(-32, 16);
		//	this.lbYourName.Size = new System.Drawing.Size(124, 20);
			this.lbYourName.Text = "Your Name:";
			this.lbYourName.TextAlign = System.Drawing.ContentAlignment.TopRight; 
			// 
			// tbPlayerName
			// 
			this.tbPlayerName.Location = new System.Drawing.Point( lbYourName.Right + space, space );
			this.tbPlayerName.Width = ( this.ClientSize.Width - 3*space )/2;
			this.tbPlayerName.Height *= platformSpec.resolution.mod;
	//		this.tbPlayerName.Location = new System.Drawing.Point(100, 16);
	//		this.tbPlayerName.Text = "";
	//		this.tbPlayerName.Width += 20;
			this.tbPlayerName.LostFocus += new System.EventHandler(this.tbPlayerName_LostFocus);
			this.tbPlayerName.GotFocus += new System.EventHandler(this.tbPlayerName_GotFocus);
			this.tbPlayerName.TextChanged += new System.EventHandler(this.tbPlayerName_TextChanged);

			// 
			// lbYourNation
			// 
			this.lbYourNation.Location = new System.Drawing.Point( space, tbPlayerName.Bottom + space );
			this.lbYourNation.Width = ( this.ClientSize.Width - 3*space )/2;
			this.lbYourNation.Height *= platformSpec.resolution.mod;
	//		this.lbYourNation.Location = new System.Drawing.Point(-32, 48);
	//		this.lbYourNation.Size = new System.Drawing.Size(124, 20);
	//		this.lbYourNation.Text = "Your nation:";
			this.lbYourNation.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbYourNation
			// 
			this.cbYourNation.Location = new System.Drawing.Point( lbYourNation.Right + space, lbYourNation.Top );
			this.cbYourNation.Width = ( this.ClientSize.Width - 3*space )/2;
			this.cbYourNation.Height *= platformSpec.resolution.mod;
	//		this.cbYourNation.Location = new System.Drawing.Point(100, 48);
	//		this.cbYourNation.Width += 20;
			this.cbYourNation.SelectedIndexChanged += new System.EventHandler(this.cbYourNation_SelectedIndexChanged);

			// 
			// lblDifficulty
			// 
			this.lblDifficulty.Location = new System.Drawing.Point( space, cbYourNation.Bottom + space );
			this.lblDifficulty.Width = ( this.ClientSize.Width - 3*space )/2;
			this.lblDifficulty.Height *= platformSpec.resolution.mod;
		//	this.lblDifficulty.Location = new System.Drawing.Point(-32, 80);
		//	this.lblDifficulty.Size = new System.Drawing.Size(144, 20);
	//		this.lblDifficulty.Text = "Difficulty:";
			this.lblDifficulty.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbDifficulty
			// 
			this.tbDifficulty.Location = new System.Drawing.Point( lblDifficulty.Right + space, lblDifficulty.Top );
			this.tbDifficulty.Width = ( this.ClientSize.Width - 3*space )/2;
			this.tbDifficulty.Height *= platformSpec.resolution.mod;
			this.tbDifficulty.LargeChange = 1;
		//	this.tbDifficulty.Location = new System.Drawing.Point(112, 72);
			this.tbDifficulty.Maximum = 4;
		//	this.tbDifficulty.Size = new System.Drawing.Size(112, 45);
			this.tbDifficulty.Value = 2;

			// 
			// lblNbrAis
			// 
			this.lblNbrAis.Location = new System.Drawing.Point( 0, tbDifficulty.Bottom + space );
			this.lblNbrAis.Width = ( this.ClientSize.Width - 3*space )/4 + space;
			this.lblNbrAis.Height *= platformSpec.resolution.mod;
	//		this.lblNbrAis.Location = new System.Drawing.Point(-32, 120);
	//		this.lblNbrAis.Size = new System.Drawing.Size(88, 16);
	//		this.lblNbrAis.Text = "8 AIs:";
			this.lblNbrAis.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbNbrAis
			// 
			this.tbNbrAis.Location = new System.Drawing.Point( lblNbrAis.Right + space, lblNbrAis.Top );
			this.tbNbrAis.Width = ( this.ClientSize.Width - 3*space )*3/4;
			this.tbNbrAis.Height *= platformSpec.resolution.mod;
			this.tbNbrAis.LargeChange = 1;
	//		this.tbNbrAis.Location = new System.Drawing.Point(56, 112);
			this.tbNbrAis.Maximum = 18;
			this.tbNbrAis.Minimum = 1;
	//		this.tbNbrAis.Size = new System.Drawing.Size(168, 45);
			this.tbNbrAis.ValueChanged += new System.EventHandler(this.tbNbrAis_ValueChanged);
			this.tbNbrAis.Value = 8;

			
			// 
			// butChooseAiNations
			// hidden
	/*		this.butChooseAiNations.Location = new System.Drawing.Point( space, tbNbrAis.Bottom + space );
			this.butChooseAiNations.Width = (this.ClientSize.Width - 3 * space) / 2;
			this.butChooseAiNations.Height = this.butFirstNextButton.Height * 4 / 3 * platformSpec.resolution.mod;
			this.butChooseAiNations.Enabled = false;
	//		this.butChooseAiNations.Location = new System.Drawing.Point(104, 160);
	//		this.butChooseAiNations.Size = new System.Drawing.Size(120, 24);
			this.butChooseAiNations.Text = "Choose AI nations";
			this.butChooseAiNations.Visible = false;*/
			
			// 
			// cbNiceBeginning
			// 
			this.cbNiceBeginning.Checked = true;
			this.cbNiceBeginning.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbNiceBeginning.Location = new System.Drawing.Point( space, tbNbrAis.Bottom + space );
	//		this.cbNiceBeginning.Size = new System.Drawing.Size(200, 20);
			this.cbNiceBeginning.Width = this.ClientSize.Width;
			this.cbNiceBeginning.Height *= platformSpec.resolution.mod;
	//		this.cbNiceBeginning.Text = "Have a nice beginning site";
			// 
			// cbTutorialMode
			// 
	//		this.cbTutorialMode.Location = new System.Drawing.Point(24, 192);
			this.cbTutorialMode.Location = new System.Drawing.Point( space, cbNiceBeginning.Bottom + space );
			this.cbTutorialMode.Width = this.ClientSize.Width;
			this.cbTutorialMode.Height *= platformSpec.resolution.mod;
	//		this.cbTutorialMode.Size = new System.Drawing.Size(200, 20);
	//		this.cbTutorialMode.Text = "tutorial";

			// 
			// butFirstBackButton
			// 
			this.butFirstBackButton.Location = new System.Drawing.Point( space, cbTutorialMode.Bottom + space );
			this.butFirstBackButton.Width = (this.ClientSize.Width - 3 * space) / 2;
			this.butFirstBackButton.Height = this.butFirstBackButton.Height * 4 / 3 * platformSpec.resolution.mod;
	//		this.butFirstBackButton.Location = new System.Drawing.Point(16, 200);
	//		this.butFirstBackButton.Size = new System.Drawing.Size(72, 24);
	//		this.butFirstBackButton.Text = "Back";
			this.butFirstBackButton.Click += new System.EventHandler(this.butFirstBackButton_Click);
			// 
			// butFirstNextButton
			// 
			this.butFirstNextButton.Location = new System.Drawing.Point( butFirstBackButton.Right + space, butFirstBackButton.Top );
			this.butFirstNextButton.Width = (this.ClientSize.Width - 3 * space) / 2;
			this.butFirstNextButton.Height = this.butFirstNextButton.Height * 4 / 3 * platformSpec.resolution.mod;
	//		this.butFirstNextButton.Location = new System.Drawing.Point(152, 200);
	//		this.butFirstNextButton.Size = new System.Drawing.Size(72, 24);
	//		this.butFirstNextButton.Text = "next";
			this.butFirstNextButton.Click += new System.EventHandler(this.butFirstNextButton_Click);

			#endregion

			#region panel2

			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.lblWorldAge);
			this.panel2.Controls.Add(this.lblMapSizeRight);
			this.panel2.Controls.Add(this.butBackSecond);
			this.panel2.Controls.Add(this.butGenerateMap);
			this.panel2.Controls.Add(this.lblWorldAgeRight);
			this.panel2.Controls.Add(this.tbWorldAge);
			this.panel2.Controls.Add(this.tbContinentSizeRight);
			this.panel2.Controls.Add(this.lblPercWaterRight);
			this.panel2.Controls.Add(this.tbPercWater);
			this.panel2.Controls.Add(this.tbContinentSize);
			this.panel2.Controls.Add(this.lblContinentSize);
			this.panel2.Controls.Add(this.lblMapSize);
			this.panel2.Controls.Add(this.lblPercOfWater);
			this.panel2.Controls.Add(this.tbMapSize);
			this.panel2.Location = new System.Drawing.Point(0, 280);
			this.panel2.Size = this.ClientSize;
			this.panel2.Visible = false; 

			// 
			// lblMapSize
			// 
			this.lblMapSize.Location = new System.Drawing.Point( 0, space );
			this.lblMapSize.Width = this.ClientSize.Width * 3 / 7;
			this.lblMapSize.Height *= platformSpec.resolution.mod;
			this.lblMapSize.Text = "Size:";
			this.lblMapSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbMapSize
			// 
			this.tbMapSize.LargeChange = 1;
			this.tbMapSize.Location = new System.Drawing.Point(this.ClientSize.Width * 3 / 7, lblMapSize.Top );
			this.tbMapSize.Maximum = 2;
			this.tbMapSize.Width = this.ClientSize.Width * 2 / 7;
			this.tbMapSize.Height *= platformSpec.resolution.mod;
			this.tbMapSize.Value = 1;
			this.tbMapSize.ValueChanged += new System.EventHandler(this.tbMapSize_ValueChanged);
			// 
			// lblMapSizeRight
			// 
			this.lblMapSizeRight.Location = new System.Drawing.Point( this.ClientSize.Width * 5 / 7, lblMapSize.Top );
			this.lblMapSizeRight.Width = this.ClientSize.Width * 2 / 7;
			this.lblMapSizeRight.Height *= platformSpec.resolution.mod;
			this.lblMapSizeRight.Text = "Small";

			// 
			// lblContinentSize
			// 
			this.lblContinentSize.Location = new System.Drawing.Point( 0, tbMapSize.Bottom + space );
			this.lblContinentSize.Width = this.ClientSize.Width * 3 / 7;
			this.lblContinentSize.Height *= platformSpec.resolution.mod;
		//	this.lblContinentSize.Location = new System.Drawing.Point(-32, 56);
		//	this.lblContinentSize.Size = new System.Drawing.Size(144, 20);
			this.lblContinentSize.Text = "Continents size:";
			this.lblContinentSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbContinentSize
			// 
			this.tbContinentSize.Location = new System.Drawing.Point( this.ClientSize.Width * 3 / 7, lblContinentSize.Top );
			this.tbContinentSize.Width = this.ClientSize.Width * 2 / 7;
			this.tbContinentSize.Height *= platformSpec.resolution.mod;
			this.tbContinentSize.LargeChange = 1;
		//	this.tbContinentSize.Location = new System.Drawing.Point(112, 48);
			this.tbContinentSize.Maximum = 2;
		//	this.tbContinentSize.Size = new System.Drawing.Size(80, 45);
			this.tbContinentSize.Value = 1;
			this.tbContinentSize.ValueChanged += new System.EventHandler(this.tbContinentSize_ValueChanged);
			// 
			// tbContinentSizeRight
			// 
			this.tbContinentSizeRight.Location = new System.Drawing.Point( this.ClientSize.Width * 5 / 7, lblContinentSize.Top );
			this.tbContinentSizeRight.Width = this.ClientSize.Width * 2 / 7;
			this.tbContinentSizeRight.Height *= platformSpec.resolution.mod;
			this.tbContinentSizeRight.Text = "Normal";

			// 
			// lblPercOfWater
			// 
			this.lblPercOfWater.Location = new System.Drawing.Point( 0, tbContinentSize.Bottom + space );
			this.lblPercOfWater.Width = this.ClientSize.Width * 3 / 7;
			this.lblPercOfWater.Height *= platformSpec.resolution.mod;
	//		this.lblPercOfWater.Location = new System.Drawing.Point(-24, 96);
	//		this.lblPercOfWater.Size = new System.Drawing.Size(112, 20);
			this.lblPercOfWater.Text = "% of water:";
			this.lblPercOfWater.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbPercWater
			// 
			this.tbPercWater.Location = new System.Drawing.Point( this.ClientSize.Width * 3 / 7, lblPercOfWater.Top );
			this.tbPercWater.Width = this.ClientSize.Width * 2 / 7;
			this.tbPercWater.Height *= platformSpec.resolution.mod;
			this.tbPercWater.LargeChange = 1;
	//		this.tbPercWater.Location = new System.Drawing.Point(88, 88);
			this.tbPercWater.Maximum = 18;
			this.tbPercWater.Minimum = 10;
	//		this.tbPercWater.Size = new System.Drawing.Size(104, 45);
			this.tbPercWater.Value = 14;
			this.tbPercWater.ValueChanged += new System.EventHandler(this.tbPercWater_ValueChanged);
			// 
			// lblPercWaterRight
			// 
			this.lblPercWaterRight.Location = new System.Drawing.Point( this.ClientSize.Width * 5 / 7, lblPercOfWater.Top );
			this.lblPercWaterRight.Width = this.ClientSize.Width * 2 / 7;
			this.lblPercWaterRight.Height *= platformSpec.resolution.mod;
	//		this.lblPercWaterRight.Location = new System.Drawing.Point(192, 88);
	//		this.lblPercWaterRight.Size = new System.Drawing.Size(48, 20);
			this.lblPercWaterRight.Text = "70%";

			// 
			// lblWorldAge
			// 
			this.lblWorldAge.Location = new System.Drawing.Point( 0, tbPercWater.Bottom + space );
			this.lblWorldAge.Width = this.ClientSize.Width * 3 / 7;
			this.lblWorldAge.Height *= platformSpec.resolution.mod;
	//		this.lblWorldAge.Location = new System.Drawing.Point(-32, 136);
	//		this.lblWorldAge.Size = new System.Drawing.Size(144, 20);
			this.lblWorldAge.Text = "World age:";
			this.lblWorldAge.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tbWorldAge
			// 
			this.tbWorldAge.Location = new System.Drawing.Point( this.ClientSize.Width * 3 / 7, lblWorldAge.Top );
			this.tbWorldAge.Width = this.ClientSize.Width * 2 / 7;
			this.tbWorldAge.Height *= platformSpec.resolution.mod;
			this.tbWorldAge.LargeChange = 1;
	//		this.tbWorldAge.Location = new System.Drawing.Point(112, 128);
			this.tbWorldAge.Maximum = 2;
	//		this.tbWorldAge.Size = new System.Drawing.Size(80, 45);
			this.tbWorldAge.Value = 1;
			this.tbWorldAge.ValueChanged += new System.EventHandler(this.tbWorldAge_ValueChanged);
			// 
			// lblWorldAgeRight
			// 
			this.lblWorldAgeRight.Location = new System.Drawing.Point( this.ClientSize.Width * 5 / 7, lblWorldAge.Top );
			this.lblWorldAgeRight.Width = this.ClientSize.Width * 2 / 7;
			this.lblWorldAgeRight.Height *= platformSpec.resolution.mod;
	//		this.lblWorldAgeRight.Location = new System.Drawing.Point(192, 128);

			// 
			// butBackSecond
			// 
			this.butBackSecond.Location = new System.Drawing.Point( space, tbWorldAge.Bottom + space );
		//	this.butBackSecond.Size = new System.Drawing.Size(72, 24);
			this.butBackSecond.Width = (this.ClientSize.Width - 3 * space) / 2;
			this.butBackSecond.Height = this.butGenerateMap.Height * 4 / 3 * platformSpec.resolution.mod;
		//	this.butBackSecond.Text = "Back";
			this.butBackSecond.Click += new System.EventHandler(this.butBackSecond_Click);
			// 
			// butGenerateMap
			// 
			this.butGenerateMap.Location = new System.Drawing.Point( butBackSecond.Right + space, butBackSecond.Top );
			//	this.butGenerateMap.Location = new System.Drawing.Point(120, 216);
			this.butGenerateMap.Width = (this.ClientSize.Width - 3 * space) / 2;
			this.butGenerateMap.Height = this.butGenerateMap.Height * 4 / 3 * platformSpec.resolution.mod;
	//		this.butGenerateMap.Size = new System.Drawing.Size(104, 24);
		//	this.butGenerateMap.Text = "Generate map";
			this.butGenerateMap.Click += new System.EventHandler(this.butGenerateMap_Click);

			#endregion

			// 
			// opening
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.cmdContinue);
			this.Controls.Add(this.cmdLoadMap);
			this.Controls.Add(this.cmdExit);
			this.Controls.Add(this.cmdLoadGame);
			this.Controls.Add(this.cmdNewGame);
			this.Controls.Add(this.pbBack);
			this.Text = platformSpec.main.gameName;
			this.Load += new System.EventHandler(this.opening_Load);
			this.Activated += new System.EventHandler(this.opening_Activated);

		}
		#endregion

#region cmdNewGame_Click
		private void cmdNewGame_Click(object sender, System.EventArgs e)
		{ // new game
			this.Menu = new MainMenu();

			string playerName = Form1.options.lastPlayerName;

			panel1.Visible = true;
			panel1.Location = new Point( 0, 0 );
			this.Text = "Players options";
			
			if ( playerName.Length > 0 )
				tbPlayerName.Text = playerName;

			cbYourNation.Items.Clear();
			for ( int i = 0; i < Statistics.normalCivilizationNumber; i++ )
				cbYourNation.Items.Add( Statistics.civilizations[ i ].name );

			cbYourNation.Items.Add( language.getAString( language.order.nGCustomNation ) );

			Random r = new Random();

			cbYourNation.SelectedIndex = r.Next( Statistics.normalCivilizationNumber );
		}
			
#endregion

		private void cmdLoadGame_Click(object sender, System.EventArgs e)
		{ // load game
			if ( parent.loadGameFrom() )
			{
				this.Close();
				parent.BringToFront();
			}
		}

		private void cmdLoadMap_Click(object sender, System.EventArgs e)
		{ // load map
			if ( parent.LoadMap() )
				this.Close();
		}

		private void cmdExit_Click(object sender, System.EventArgs e)
		{ // quit
			parent.Close();
			this.Close();
			Application.Exit();
		}

#region butFirstNextButton_Click panel1 next
		private void butFirstNextButton_Click(object sender, System.EventArgs e)
		{ // panel1 next
			if ( tbNbrAis.Value == 1 )
			{
				switch ( MessageBox.Show( 
				"You selected 0 artificial intelligence to play against, it could be prety boring.\nDo you want to review your choice?",
				"0 Ais",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1
				) )
				{
					case DialogResult.Yes :
						break;

					case DialogResult.No :
						panel1.Visible = false;
						panel2.Visible = true;
						panel2.Location = new Point( 0, 0 );
						this.Text = "Map options";
						break;
				}
			}
			else
			{
				panel1.Visible = false;
				panel2.Visible = true;
				panel2.Location = new Point( 0, 0 );
				this.Text = "Map options";
			}
		}
			
#endregion

#region butGenerateMap_Click panel2 next // Create!
		private void butGenerateMap_Click(object sender, System.EventArgs e)
		{ // panel2 next
			int widthBase = 40, heightBase = 60;

			butBackSecond.Enabled = false;
			butGenerateMap.Enabled = false;

			switch ( tbMapSize.Value )
			{
				case -1 :
					width = widthBase / 2;
					height = heightBase / 2;
					break;

				case 0 :
					width = widthBase;
					height = heightBase;
					break;

				case 1 :
					width = widthBase * 3 / 2;
					height = heightBase * 3 / 2;
					break;

				default : // 2
					width = widthBase * 2;
					height = heightBase * 2;
					break;

				case 3 :
					width = widthBase * 3;
					height = heightBase * 3;
					break;

				case 4 :
					width = widthBase * 4;
					height = heightBase * 4;
					break;
			}

			//bool customNation = false;

			Form1.options.lastPlayerName = tbPlayerName.Text;

			if ( cbYourNation.SelectedIndex == cbYourNation.Items.Count - 1 )
			{
				if ( 
					parent.newGame( 
						width, 
						height, 
						(byte)( tbPercWater.Value * 5 ), 
						(byte)( tbNbrAis.Value ), 
						tbPlayerName.Text.TrimEnd( ' ' ), 
						(byte)cbYourNation.SelectedIndex, 
						(byte)tbDifficulty.Value, 
						(byte)tbContinentSize.Value, 
						(byte)tbWorldAge.Value, 
						true,
						cbNiceBeginning.Checked,
						cbTutorialMode.Checked,
						true,
						cnName,
						cnDesc,
						cnImportCityListFrom,
						cnColor,
						new byte[ 0 ]
						) 
					)
				{
					parent.BringToFront();
					this.Close();
				}
				else
				{
					butGenerateMap.Enabled = true;
					butBackSecond.Enabled = true;
				}
			}
			else
			{
				if ( 
					parent.newGame( 
						width, 
						height, 
						(byte)( tbPercWater.Value * 5 ), 
						(byte)( tbNbrAis.Value ), 
						tbPlayerName.Text.TrimEnd( ' ' ), 
						(byte)cbYourNation.SelectedIndex, 
						(byte)tbDifficulty.Value, 
						(byte)tbContinentSize.Value, 
						(byte)tbWorldAge.Value, 
						true,
						cbNiceBeginning.Checked,
						cbTutorialMode.Checked
						) 
					)
				{
					parent.BringToFront();
					this.Close();
				}
				else
				{
					butGenerateMap.Enabled = true;
					butBackSecond.Enabled = true;
				}
			}
		}
			
#endregion

		private void tbPlayerName_GotFocus( object sender, System.EventArgs e )
		{
			//inputPanel = new Microsoft.WindowsCE.Forms.InputPanel();
			try
			{
				platformSpec.main.showSip();
			//	inputPanel1.Enabled = true;
			}
			catch
			{
				MessageBox.Show( "The keyboard failed to appear, this is a known bug in  and can be solved by installing the .Net Compact Framework sp2.", ".Net Compact Framework error");
			}
		}

		private void tbPlayerName_LostFocus( object sender, System.EventArgs e )
		{ 
		/*	try
			{
			//	platformSpec.main.showSip();
			//	inputPanel1.Enabled = false;
			}
			catch
			{
			}*/
		}

		private void butBackSecond_Click(object sender, System.EventArgs e)
		{ // panel2 - back
			panel1.Visible = true;
			panel2.Visible = false;
			this.Text = "Players options";
		}

		private void tbNbrAis_ValueChanged(object sender, System.EventArgs e)
		{
			int aiNbr = tbNbrAis.Value - 1;
			/*if ( aiNbr > 1 )
			{*/
				lblNbrAis.Text = String.Format( language.getAString( language.order.nGAis ), aiNbr );
			//	lblNbrAis.Text =  aiNbr.ToString() + " AIs";
			/*}
			else
			{
			//	lblNbrAis.Text =  aiNbr.ToString() + " AI";
			}*/
		}

		private void tbMapSize_ValueChanged(object sender, System.EventArgs e)
		{
			switch ( tbMapSize.Value )
			{
				/*case 0:
					lblMapSizeRight.Text = "Tiny";
					break;*/
				case 0:
					lblMapSizeRight.Text = language.getAString( language.order.Small );
					break;
				case 1:
					lblMapSizeRight.Text = language.getAString( language.order.MediumSize );
					break;
				case 2:
					lblMapSizeRight.Text = language.getAString( language.order.Large );
					break;
				case 3:
					lblMapSizeRight.Text = language.getAString( language.order.Huge );
					break;
			}
		}

		private void tbPercWater_ValueChanged(object sender, System.EventArgs e)
		{
			lblPercWaterRight.Text = ( tbPercWater.Value * 5 ).ToString() + " %";
		}

		private void butFirstBackButton_Click(object sender, System.EventArgs e)
		{
			this.Menu = null;
			panel1.Visible = false;
			this.Text = platformSpec.main.gameName;
		}

#region cbYourNation_SelectedIndexChanged
		private void cbYourNation_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( cbYourNation.SelectedIndex == cbYourNation.Items.Count - 1 )
			{
				createCivStats ccs = new createCivStats();

				if ( cnName != null && cnName.Length > 0 )
				{
					ccs.tbNationName.Text = cnName;
					ccs.tbColors[ 0 ].Value = cnColor.R;
					ccs.tbColors[ 1 ].Value = cnColor.G;
					ccs.tbColors[ 2 ].Value = cnColor.B;
					ccs.cbCityList.SelectedIndex = cnImportCityListFrom;
				}

				string titre = this.Text;

				platformSpec.manageWindows.prepareForDialog( this ); //this.Text = ""; 

				ccs.ShowDialog();
				if ( ccs.resultAccepted )
				{
					cnName = ccs.tbNationName.Text.TrimEnd( " ".ToCharArray() );
					cnDesc = "";
					cnColor = Color.FromArgb( ccs.tbColors[ 0 ].Value, ccs.tbColors[ 1 ].Value, ccs.tbColors[ 2 ].Value );
					cnImportCityListFrom = (byte)ccs.cbCityList.SelectedIndex;
				}
				else
				{
					if ( cnName == null )
					{
						Random r = new Random();

						cbYourNation.SelectedIndex = r.Next( Statistics.normalCivilizationNumber );
					}
				}

				this.Text = titre;
			}
			else if ( !playerNameModified || tbPlayerName.Text == "" )
			{
				Random r = new Random();
				tbPlayerName.Text = Statistics.civilizations[ cbYourNation.SelectedIndex ].leaderNames[ r.Next( Statistics.civilizations[ cbYourNation.SelectedIndex ].leaderNames.Length ) ];
				playerNameModified = false;
			}
		}
			
#endregion

#region opening_Load
		private void opening_Load(object sender, System.EventArgs e)
		{
			string appPath = Form1.appPath;

			Bitmap backBmp;
			
			if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
				backBmp = new Bitmap(Form1.appPath + "\\images\\UI\\480 640.jpg" );
			else
				backBmp = new Bitmap(Form1.appPath + "\\images\\UI\\240 320.jpg" );

			Graphics g = Graphics.FromImage( backBmp );
			Font titleFont = new Font( "Tahoma", 26, FontStyle.Regular );
			SolidBrush titleBrush = new SolidBrush( Color.Black );

			string versionString = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

			if ( System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width > 320 )
				versionString += " vga";

			Font versionFont = new Font( "Tahoma", 10, FontStyle.Regular );
			SizeF versionSize = g.MeasureString( versionString, versionFont );
			g.DrawString( versionString, versionFont, new SolidBrush( Color.White ), 2, this.Height - (int)versionSize.Height + 1 );
			g.DrawString( versionString, versionFont, titleBrush, 1, this.Height - (int)versionSize.Height );

			pbBack.Image = backBmp;
			string lastGame = Form1.options.lastSavePath; // cfgOut.getValues.savePath;

			if ( 
				lastGame != null &&
				lastGame != "" && 
				new System.IO.FileInfo( lastGame ).Exists
				)
			{
				savePath = lastGame;
				cmdContinue.Enabled = true;
			}
			else if ( Form1.game != null ) // if ( Form1.game.playerList != null )
			{
				cmdContinue.Enabled = true;
			}
			else
			{
				cmdContinue.Enabled = false;
			}
		}
			
#endregion

#region cmdContinue_Click
		private void cmdContinue_Click(object sender, System.EventArgs e)
		{
			if ( Form1.game == null )
			{
				wC.show = true;

				if ( parent.loadGame( savePath ) )
				{
					this.Close();
					parent.BringToFront();
				}

				wC.show = false;
			}
			else
			{
				this.Close();
			}
		}
		#endregion
		
		private void opening_Activated(object sender, System.EventArgs e)
		{
		}

		private void tbPlayerName_TextChanged(object sender, System.EventArgs e)
		{
			playerNameModified = true;
		}

		private void tbContinentSize_ValueChanged(object sender, System.EventArgs e)
		{
			switch ( tbContinentSize.Value )
			{
				case 0:
					tbContinentSizeRight.Text = language.getAString( language.order.Small );
					break;
					
				case 1:
					tbContinentSizeRight.Text = language.getAString( language.order.MediumSize );
					break;
					
				case 2:
					tbContinentSizeRight.Text = language.getAString( language.order.Large );
					break;
			}
		}

		private void tbWorldAge_ValueChanged(object sender, System.EventArgs e)
		{
			switch ( tbWorldAge.Value )
			{
				case 0:
					lblWorldAgeRight.Text = language.getAString( language.order.Young );
					break;
					
				case 1:
					lblWorldAgeRight.Text = language.getAString( language.order.MediumAge );
					break;
					
				case 2:
					lblWorldAgeRight.Text = language.getAString( language.order.Old );
					break;
			}
		}
	}
}
