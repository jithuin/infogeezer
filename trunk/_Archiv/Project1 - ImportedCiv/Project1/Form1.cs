using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		public static Game game;
		public static Options options;
		public static Sounds.SoundPlayer soundPlayer;
	//	public static Display display;

		public Form1( bool first )
		{
			wC.show = true; 
			appPath = platformSpec.main.appPath;
		
			InitializeComponent();

			options = Options.load();

#if CF
			soundPlayer = new Sounds.OpenPlayer();
#else
			soundPlayer = new Sounds.WinSound();
#endif
/*

			this.cmdUnitLeft.Image = new Bitmap(Form1.appPath + "cmdLeft.jpg" );
			this.cmdTransportUnitLeft.Image = new Bitmap(Form1.appPath + "cmdLeft.jpg" );
			this.cmdUnitRight.Image = new Bitmap(Form1.appPath + "cmdRight.jpg" );
			this.cmdTransportUnitRight.Image = new Bitmap(Form1.appPath + "cmdRight.jpg" );		*/
	/*		if ( platformSpec.main.isPPC )
			{
				this.cmdUnitLeft.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitLeft.Image" ) ) );
				this.cmdUnitRight.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitRight.Image" ) ) );
				this.cmdTransportUnitRight.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitRight.Image" ) ) );
				this.cmdTransportUnitLeft.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitLeft.Image" ) ) );
			}	
				
*/
 
			this.pictureBox2.Location = new Point( this.Width - pictureBox2.Width, this.Height - pictureBox2.Height );
			this.pictureBox3.Location = new Point( 0, this.Height - this.pictureBox3.Height );
			this.pictureBox4.Location = new Point( 0, this.Height - this.pictureBox3.Height - this.pictureBox4.Height );
			this.cmdUnitLeft.Location = new Point( 0, this.Height - this.pictureBox3.Height / 2 - this.cmdUnitLeft.Height / 2 );
			this.cmdUnitRight.Location = new Point( pictureBox3.Width - cmdUnitRight.Width, this.Height - this.pictureBox3.Height / 2 - this.cmdUnitLeft.Height / 2 );
			this.cmdTransportUnitLeft.Location = new Point( 0, this.Height - this.pictureBox3.Height - this.pictureBox4.Height / 2 - this.cmdUnitLeft.Height / 2 );
			this.cmdTransportUnitRight.Location = new Point( pictureBox4.Width - cmdUnitRight.Width, this.Height - this.pictureBox3.Height - this.pictureBox4.Height / 2 - this.cmdUnitLeft.Height / 2 );

			//this.Resize()

			// savePath = "";

	/*		waitingTmr = new System.Windows.Forms.Timer();
			waitingTmr.Interval = 1;
			waitingTmr.Enabled = false;
			waitingTmr.Tick += new System.EventHandler( waitingTmr_tick );*/

			getPFT1 = new getPFT();
		//	initStatistics.civilizations();
			Statistics.initCivilizations();

			//vga
/*			this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
			this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
*/
			pictureBox1.Size = this.Size;
			pictureBox2.Left = this.Width - pictureBox2.Width;
			pictureBox2.Top = this.Height - pictureBox2.Height;
			
			pictureBox3.Top = this.ClientSize.Height - pictureBox3.Height;
			pictureBox4.Top = this.ClientSize.Height - pictureBox4.Height - pictureBox3.Height;

			cmdTransportUnitLeft.Top = pictureBox4.Top + ( pictureBox4.Height - cmdTransportUnitLeft.Height ) / 2;
			cmdTransportUnitRight.Top = pictureBox4.Top + ( pictureBox4.Height - cmdTransportUnitRight.Height ) / 2;
			cmdUnitLeft.Top = pictureBox3.Top + ( pictureBox3.Height - cmdUnitLeft.Height ) / 2;
			cmdUnitRight.Top = pictureBox3.Top + ( pictureBox3.Height - cmdUnitRight.Height ) / 2;

			setLanguage();
			wC.show = false;

			platformSpec.manageWindows.setMainSize( this );

			if ( first )
				showOpening( first );
		}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof(Form1) );

			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mIFile = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.mISaveAs = new System.Windows.Forms.MenuItem();
			this.mISave = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem25 = new System.Windows.Forms.MenuItem();
			this.menuItem26 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mIUnit = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mIActions = new System.Windows.Forms.MenuItem();
			this.mICiv = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.menuItem24 = new System.Windows.Forms.MenuItem();
			this.menuItem27 = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem17 = new System.Windows.Forms.MenuItem();
			this.mIQuestion = new System.Windows.Forms.MenuItem();
			this.mIAbout = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.mILabels = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.mIUnitInfo = new System.Windows.Forms.MenuItem();
			this.mITerrainInfo = new System.Windows.Forms.MenuItem();
			this.miTest = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.cmdOk = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cmdNext = new System.Windows.Forms.Button();
			this.cmdPrevious = new System.Windows.Forms.Button();
			this.tmrResize = new System.Windows.Forms.Timer();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.cmdUnitLeft = new System.Windows.Forms.PictureBox();
			this.cmdUnitRight = new System.Windows.Forms.PictureBox();
			this.cmdTransportUnitRight = new System.Windows.Forms.PictureBox();
			this.cmdTransportUnitLeft = new System.Windows.Forms.PictureBox();
			this.pictureBox4 = new System.Windows.Forms.PictureBox();
			this.tmrShowUnit = new System.Windows.Forms.Timer();
			this.cmdChangeMiniMap = new System.Windows.Forms.Button();
			this.cmdUp = new System.Windows.Forms.Button();
			this.cmdLeft = new System.Windows.Forms.Button();
			this.cmdRight = new System.Windows.Forms.Button();
			this.cmdDown = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.mISatellites = new System.Windows.Forms.MenuItem();
			this.mITransport = new System.Windows.Forms.MenuItem();

			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add( this.mIFile );
			this.mainMenu1.MenuItems.Add( this.mIUnit );
			this.mainMenu1.MenuItems.Add( this.mICiv );
			this.mainMenu1.MenuItems.Add( this.menuItem17 );
			this.mainMenu1.MenuItems.Add( this.mIQuestion );
			this.mainMenu1.MenuItems.Add( this.menuItem15 );
			this.mainMenu1.MenuItems.Add( this.menuItem14 );
			this.mainMenu1.MenuItems.Add( this.menuItem13 );

			// 
			// mIFile
			// 
			this.mIFile.MenuItems.Add( this.menuItem10 );
			this.mIFile.MenuItems.Add( this.mISaveAs );
			this.mIFile.MenuItems.Add( this.mISave );
			this.mIFile.MenuItems.Add( this.menuItem3 );
			this.mIFile.MenuItems.Add( this.menuItem25 );
			this.mIFile.MenuItems.Add( this.menuItem26 );
			this.mIFile.MenuItems.Add( this.menuItem4 );
			this.mIFile.Text = "File";
			this.mIFile.Popup += new System.EventHandler( this.mIFile_Popup );

			// 
			// menuItem10
			// 
			this.menuItem10.Text = "Load game...";
			this.menuItem10.Click += new System.EventHandler( this.menuItem10_Click );

			// 
			// mISaveAs
			// 
			this.mISaveAs.Text = "Save as...";
			this.mISaveAs.Click += new System.EventHandler( this.mISaveAs_Click );

			// 
			// mISave
			// 
			this.mISave.Text = "Save";
			this.mISave.Click += new System.EventHandler( this.mISave_Click );

			// 
			// menuItem3
			// 
			this.menuItem3.Text = "-";

			// 
			// menuItem25
			// 
			this.menuItem25.Text = "Options...";
			this.menuItem25.Click += new System.EventHandler( this.menuItem25_Click );

			// 
			// menuItem26
			// 
			this.menuItem26.Text = "Main menu";
			this.menuItem26.Click += new System.EventHandler( this.menuItem26_Click );

			// 
			// menuItem4
			// 
			this.menuItem4.Text = "Exit";
			this.menuItem4.Click += new System.EventHandler( this.menuItem4_Click );

			// 
			// mITransport
			// 
			this.mITransport.Text = "Transport";

			// 
			// mIUnit
			// 
			this.mIUnit.MenuItems.Add( this.menuItem5 );
			this.mIUnit.MenuItems.Add( this.mIActions );
			this.mIUnit.MenuItems.Add( this.mITransport );
			this.mIUnit.Text = "Unit";
			this.mIUnit.Popup += new System.EventHandler( this.mIUnit_Popup );

			// 
			// menuItem5
			// 
			this.menuItem5.Text = "Build";

			// 
			// mIActions
			// 
			this.mIActions.Text = "Action";

			// 
			// mICiv
			// 
			this.mICiv.Enabled = false;
			this.mICiv.MenuItems.Add( this.menuItem19 );
			this.mICiv.MenuItems.Add( this.menuItem24 );
			this.mICiv.MenuItems.Add( this.menuItem27 );
			this.mICiv.MenuItems.Add( this.menuItem12 );
			this.mICiv.MenuItems.Add( this.mISatellites );
			this.mICiv.Text = "Civ";
			this.mICiv.Popup += new System.EventHandler( this.mICiv_Popup );

			// 
			// menuItem19
			// 
			this.menuItem19.Text = "Science";
			this.menuItem19.Click += new System.EventHandler( this.menuItem19_Click );

			// 
			// menuItem24
			// 
			this.menuItem24.Text = "Budget";
			this.menuItem24.Click += new System.EventHandler( this.menuItem24_Click );

			// 
			// menuItem27
			// 
			this.menuItem27.Text = "Relations";
			this.menuItem27.Click += new System.EventHandler( this.menuItem27_Click );

			// 
			// menuItem12
			// 
			this.menuItem12.Text = "Info";
			this.menuItem12.Click += new System.EventHandler( this.menuItem12_Click );

			// 
			// menuItem17
			// 
			this.menuItem17.Text = "Turn";
			this.menuItem17.Click += new System.EventHandler( this.menuItem17_Click );

			// 
			// mIQuestion
			// 
			this.mIQuestion.MenuItems.Add( this.mIAbout );
			this.mIQuestion.MenuItems.Add( this.menuItem8 );
			this.mIQuestion.MenuItems.Add( this.mILabels );
			this.mIQuestion.MenuItems.Add( this.menuItem6 );
			this.mIQuestion.MenuItems.Add( this.mIUnitInfo );
			this.mIQuestion.MenuItems.Add( this.mITerrainInfo );
			this.mIQuestion.MenuItems.Add( this.miTest );
			this.mIQuestion.Text = " ? ";
			this.mIQuestion.Popup += new System.EventHandler( this.mIQuestion_Popup );

			// 
			// mIAbout
			// 
			this.mIAbout.Text = "About...";
			this.mIAbout.Click += new System.EventHandler( this.mIAbout_Click );

			// 
			// menuItem8
			// 
			this.menuItem8.Text = "Encyclopedia";
			this.menuItem8.Click += new System.EventHandler( this.menuItem8_Click );

			// 
			// mILabels
			// 
			this.mILabels.Text = "Labels";

			// 
			// menuItem6
			// 
			this.menuItem6.Text = "-";

			// 
			// mIUnitInfo
			// 
			this.mIUnitInfo.Text = "Infos on units";

			// 
			// mITerrainInfo
			// 
			this.mITerrainInfo.Text = "Info on terrain";
			this.mITerrainInfo.Click += new System.EventHandler( this.mITerrainInfo_Click );

			// 
			// miTest
			// 
			this.miTest.Enabled = false;
			this.miTest.Text = "Test";
			this.miTest.Click += new System.EventHandler( this.miTest_Click );

			// 
			// menuItem15
			// 
			this.menuItem15.Enabled = false;
			this.menuItem15.Text = "|";

			// 
			// menuItem14
			// 
			this.menuItem14.Text = "Prev";
			this.menuItem14.Click += new System.EventHandler( this.menuItem14_Click );

			// 
			// menuItem13
			// 
			this.menuItem13.Text = "Next";
			this.menuItem13.Click += new System.EventHandler( this.menuItem13_Click );

			// 
			// mISatellites
			// 
			this.mISatellites.Text = "Satellites";
			this.mISatellites.Click += new EventHandler(mISatellites_Click);

			// 
			// cmdOk
			// 
			this.cmdOk.Visible = false;
			this.cmdOk.Click += new System.EventHandler( this.cmdOk_Click );

			// 
			// pictureBox1
			// 
			this.pictureBox1.Visible = false;
			this.pictureBox1.Click += new System.EventHandler( this.pictureBox1_Click );

			// 
			// label1
			// 
			this.label1.Text = "label1";
			this.label1.Visible = false;

			// 
			// label2
			// 
			this.label2.Text = "label2";
			this.label2.Visible = false;

			// 
			// cmdNext
			// 
			this.cmdNext.Text = ">";
			this.cmdNext.Visible = false;

			// 
			// cmdPrevious
			// 
			this.cmdPrevious.Text = "<";
			this.cmdPrevious.Visible = false;

			// 
			// tmrResize
			// 
			this.tmrResize.Interval = 200;
			this.tmrResize.Tick += new EventHandler(tmrResize_Tick);
			this.tmrResize.Enabled = false;

			// 
			// pictureBox2
			// 
			this.pictureBox2.Size = new System.Drawing.Size( 84, 54 );
			this.pictureBox2.Click += new System.EventHandler( this.pictureBox2_Click );

			// 
			// pictureBox3
			// 
			this.pictureBox3.Size = new System.Drawing.Size( 152, 56 );
			this.pictureBox3.Visible = false;
			this.pictureBox3.Click += new System.EventHandler( this.pictureBox3_Clicked );

			// 
			// cmdUnitLeft
			// 
			this.cmdUnitLeft.Size = new System.Drawing.Size( 8, 30 );
			this.cmdUnitLeft.Visible = false;
			this.cmdUnitLeft.Click += new System.EventHandler( this.cmdUnitLeft_Clicked );

			// 
			// cmdUnitRight
			// 
			this.cmdUnitRight.Size = new System.Drawing.Size( 8, 30 );
			this.cmdUnitRight.Visible = false;
			this.cmdUnitRight.Click += new System.EventHandler( this.cmdUnitRight_Clicked );

			// 
			// cmdTransportUnitRight
			// 
			this.cmdTransportUnitRight.Size = new System.Drawing.Size( 8, 30 );
			this.cmdTransportUnitRight.Visible = false;
			this.cmdTransportUnitRight.Click += new System.EventHandler( this.cmdTransportUnitRight_Clicked );

			// 
			// cmdTransportUnitLeft
			// 
			this.cmdTransportUnitLeft.Size = new System.Drawing.Size( 8, 30 );
			this.cmdTransportUnitLeft.Visible = false;
			this.cmdTransportUnitLeft.Click += new System.EventHandler( this.cmdTransportUnitLeft_Clicked );

			// 
			// pictureBox4
			// 
			this.pictureBox4.Size = new System.Drawing.Size( 152, 56 );
			this.pictureBox4.Visible = false;
			this.pictureBox4.Click += new System.EventHandler( this.pictureBox4_Clicked );

			// 
			// tmrShowUnit
			// 
			this.tmrShowUnit.Tick += new System.EventHandler( this.tmrShowUnit_Tick );

			// 
			// cmdChangeMiniMap
			// 
			this.cmdChangeMiniMap.Text = "button1";
			this.cmdChangeMiniMap.Visible = false;
//			this.cmdChangeMiniMap.Click += new System.EventHandler( this.cmdChangeMiniMap_Click );

			// 
			// cmdUp
			// 
			this.cmdUp.Width = this.cmdUp.Height;
			this.cmdUp.Text = "A";
			this.cmdUp.Click += new System.EventHandler( this.cmdUp_Click );
			this.cmdUp.Location = new Point( this.cmdUp.Width, 20 );

			// 
			// cmdLeft
			// 
			this.cmdLeft.Text = "<";
			this.cmdLeft.Click += new System.EventHandler( this.cmdLeft_Click );
			this.cmdLeft.Location = new Point( 0, 20 + this.cmdUp.Height );
			this.cmdLeft.Width = this.cmdUp.Height;

			// 
			// cmdRight
			// 
			this.cmdRight.Text = ">";
			this.cmdRight.Click += new System.EventHandler( this.cmdRight_Click );
			this.cmdRight.Location = new Point( this.cmdUp.Width * 2, 20 + this.cmdUp.Height );
			this.cmdRight.Width = this.cmdUp.Height;

			// 
			// cmdDown
			// 
			this.cmdDown.Text = "V";
			this.cmdDown.Click += new System.EventHandler( this.cmdDown_Click );
			this.cmdDown.Location = new Point( this.cmdUp.Width, 20 + this.cmdUp.Height * 2 );
			this.cmdDown.Width = this.cmdUp.Height;

			// 
			// cmdCancel
			// 
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Visible = false;

			// 
			// Form1
			// 
			this.Controls.Add( this.cmdCancel );
			this.Controls.Add( this.cmdDown );
			this.Controls.Add( this.cmdRight );
			this.Controls.Add( this.cmdLeft );
			this.Controls.Add( this.cmdUp );
			this.Controls.Add( this.cmdChangeMiniMap );
			this.Controls.Add( this.cmdTransportUnitRight );
			this.Controls.Add( this.cmdTransportUnitLeft );
			this.Controls.Add( this.pictureBox4 );
			this.Controls.Add( this.cmdUnitRight );
			this.Controls.Add( this.cmdUnitLeft );
			this.Controls.Add( this.pictureBox3 );
			this.Controls.Add( this.pictureBox2 );
			this.Controls.Add( this.cmdPrevious );
			this.Controls.Add( this.cmdNext );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.cmdOk );
			this.Controls.Add( this.pictureBox1 );
			this.Menu = this.mainMenu1;
			this.Text = platformSpec.main.gameName;
		//	this.Resize += new System.EventHandler( this.Form1_Resize );
			this.Activated += new System.EventHandler( this.Form1_Activated );
			this.Deactivate += new System.EventHandler( this.Form1_Deactivate );
		//	this.KeyDown += new KeyEventHandler( Form1_KeyDown );

			if ( platformSpec.main.isPPC )
			{
				this.cmdUnitLeft.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitLeft.Image" ) ) );
				this.cmdUnitRight.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitRight.Image" ) ) );
				this.cmdTransportUnitRight.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitRight.Image" ) ) );
				this.cmdTransportUnitLeft.Image = ( (System.Drawing.Image)( resources.GetObject( "cmdUnitLeft.Image" ) ) );
			}
		}
		#endregion

#region static Main
		static void Main( string[] args ) 
		{ 

#if CF
				if ( platformSpec.main.deviceMemory < 6 * ( 1024 * 1024 ) ) 
					MessageBox.Show( 
						"You currently have less than 6 mb of program memory available, it is recommanded to have at least 7 mb. The game will start anyway, but is at risk to fail.\n\nThank you for your attention.", 
						"Memory allocation" 
						); 

				if ( 
					System.Environment.Version.Build < 3316 && 
					System.Environment.Version.Major <= 1 /* && 
					System.Environment.Version.Minor == 0 */
					) 
					MessageBox.Show( 
						String.Format( "You appear to have an older version of the .NET Compact Framework, make sure you have at least v.1 sp2. Use the link in the download section of www.pockethumanity.com to get the latest version.\n{0}.{1}.{2}.{3}", 
						System.Environment.Version.Major, System.Environment.Version.Minor, System.Environment.Version.Build, System.Environment.Version.Revision ),
						".NET CF version" 
						); 
#endif

			Application.Run( new Form1( true ) );
		} 
#endregion

		private void showOpening( bool inGame ) 
		{ 
			this.Update(); 
			wC.show = true; 
			opening op = new opening( this ); 
			string name = this.Text; 
			platformSpec.manageWindows.prepareForDialog( this ); // platformSpec.manageWindows.prepareForDialog( this ); //this.Text = ""; 
			wC.show = false; 
			op.ShowDialog(); 
			this.Text = name;
		} 

#region setLanguage
		private void setLanguage()
		{
			curLanguage = language.current;

			mIFile.Text = language.getAString( language.order.file );
			mIUnit.Text = language.getAString( language.order.unit );
			mICiv.Text = language.getAString( language.order.civ );
			menuItem17.Text = language.getAString( language.order.endTurn );
			mIQuestion.Text = language.getAString( language.order.questionMark );

			menuItem14.Text = language.getAString( language.order.previous );
			menuItem13.Text = language.getAString( language.order.next );

			menuItem10.Text = language.getAString( language.order.fileLoad );
			mISaveAs.Text = language.getAString( language.order.fileSaveAs );
			mISave.Text = language.getAString( language.order.fileSave );
			menuItem25.Text = language.getAString( language.order.fileOption );
			menuItem26.Text = language.getAString( language.order.fileMainMenu );
			menuItem4.Text = language.getAString( language.order.openingExit );

			mIActions.Text = language.getAString( language.order.action );
			menuItem5.Text = language.getAString( language.order.build );
			
			menuItem19.Text = language.getAString( language.order.science );
			menuItem24.Text = language.getAString( language.order.budjet );
			menuItem27.Text = language.getAString( language.order.relation );
			menuItem12.Text = language.getAString( language.order.info );
			mISatellites.Text = language.getAString( language.order.satellites );

			//	menuItem7.Text = language.getAString( (int)language.order );
			
			mIAbout.Text = language.getAString( language.order.about );
			mILabels.Text = language.getAString( language.order.labels );
			menuItem8.Text = language.getAString( language.order.encyclopedia );
			
			mIUnitInfo.Text = language.getAString( language.order.infosOnUnit );
			
			cmdCancel.Text = language.getAString( language.order.cancel );
		} 
			
#endregion

#region défénition des variables	

	#region variables

	//	public static byte curPlayer;
		static int curUnit;
		public static byte totalBuildings = 3, totalTechno = 33;
		static byte moveFraction = 3;
		public static byte foodPerPop = 10;
		public static byte goldPerSpy = 3;

		public static uint globalPollution;
		public static string appPath;
		public Point pntToBombard;

		Font cityNameFont, cityPopFont;
		SolidBrush cityNameBrush, cityPopBrush, popBackBrush;
		Pen cityPopPen;

		public static byte nbrUnit;

		byte curAi, selectState;

		double miniMapAdjX, miniMapAdjY;
		int unitAffSli, transportAffSli;
	
		public static int sliHor, sliVer;
		public static int caseWidth = 50, caseHeight = 30;

		public static string ralf;

		public static structures.sSelected selected;

		public static Bitmap[,] cityBmp, cityBmpUnseen;
		public static Bitmap[][] roadBmp, railBmp, riverBmp;
		Bitmap miniMap;
		Bitmap miniMapFront;
		Bitmap miniMapAff;
		public static Bitmap[] 
			civicImprovement, 
			militaryImprovement, 
			laborBmp, 
			specialResBmp, 
			civicImprovementUnseen, 
			militaryImprovementUnseen, 
			laborBmpUnseen, 
			specialResBmpUnseen;
		
		public static Bitmap foodBmp, prodBmp, tradeBmp, peopleBmp;
		public static Bitmap[] peopleNonLaborBmp, slaveBmp;

		public static string[] relationPol = 
		{
			"Peace",
			"War",
			"CeaseFire",
			"Alliance",
			"Protector",
			"Protected"
		};

		Bitmap viewBmp;
		Bitmap caseSelected;

		static Bitmap terrainView;
		public static int oldSliVer, oldSliHor;
		bool somethingHaseBeenBuilt;

		Graphics g;
		SolidBrush blackBrush;
		SolidBrush whiteBrush;
		Rectangle r;
		public static System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();

		getPFT getPFT1;

		public string[] unitActionAff;

		structures.drawRegion drawRect;

		public static Bitmap spyBmp;
		
		public static uint globalNukePollution;
		public static uint globalStandardPollution;

		public static System.Collections.BitArray[] regionValidToBuildCity; 

		private string curLanguage;
		public static Bitmap nukeExplosion, pollution;
	
		public static Color defaultBackColor = Color.Wheat;
		
		#endregion

	#region structures
	
	/*	public struct UnitList
		{
			public byte owner;
			public int unit;
		}*/

	/*	public struct sUnitStats
		{
			public string name, description;
			public byte attack;
			public byte defense;
			public byte move;
			public enums.speciality speciality;
			public byte terrain;
			public byte transport;
			public int cost; 
			public byte disponibility; //tech
			public byte obselete; //unit
			public Bitmap bmp;

			public byte sight;
			public byte range;

			public bool entrenchable;
			public bool highSeaSync;
			public bool stealth;
			public byte neededResource;
		}*/

	/*	public struct structures.sSelected
		{
			public int X;
			public int Y;
			public byte state;
			public byte name;
			public byte owner;
			public int unit;
			public byte unitInTransport;
		}*/

	/*	public struct sTerrainStats
		{
			public string name, description;

			public byte food, trade, production,
				mineBonus, irrigationBonus, roadBonus;

			public byte riverTradeBonus;
			public byte riverFoodBonus;
			public byte ew;
			public byte move;
			public byte game.difficulty;
			public byte defense;
			public Bitmap bmp, bmpUnseen;
			public Color color;

			public bool canBuildIrrigation { get { return irrigationBonus > 0; } }
			public bool canBuildMine { get { return mineBonus > 0; } }

		}	*/

	/*	public struct sBuildingStats
		{
			public string name, description;
			public int cost;
			public int effect;
			public byte disponibility;
			public byte upkeep;
		}*/

	/*	public struct sTechnoStats
		{
			public string name, description;
			public byte[] needs;
			public int cost;
			public byte line;
			public int posOnLine;
			public string shortDesc;
			public bool canBeResearched;
			public Rectangle rect;

			public int militaryValue;
		}*/

	/*	public struct Stat.Civilization
		{
			public string name, description;
			public Color color;
			public byte cityType;
			public string[] leaderNames;
			public string[] cityNames;
		}*/

	/*	public struct structures.sForeignRelation
		{
			public bool madeContact;
			public byte politic;
			public byte economic;
			public int quality;
			public bool embargo;
			public structures.sSpies[] spies;
		}*/
	#endregion

	#region enums
		public enum unitState : byte
		{
			idle,
			dead,
			sleep,
			moving,
			fortified,
			inTransport,
			wait,
			turnCompleted,
			buildingRoad,
			buildingIrrigation,

			buildingMine, // 10
			buildingFort,
			buildingAirport,
			buildingMineFld,
			goToBuildCity,
			goToImprove,
			automate,
			goToAttack,
			goToDefend,
			goToFrontier,

			goToBoat, // 20 
			waitingForBoat, 
			transportGoToDrop, 
			transportWaitingToFill, 
			transportGoToPickUp, 
			exploring, 
			planeRebase, 
			planeAirDefense,  
			goTo ,
			goToJoinCity
		}


		public enum buildingList : byte
		{
			temple, 
			barrack,
			library,
			walls,  
			/*granary,
			marketPlace,
			courtHouse,
			aqueduct,
			bank,
			cathedral,
			university,
			colosseum,
			factory,
			policeStation,
			recyclingCenter,
			manufacturingPlant,
			coalPlant,
			hydroPlant,
			nuclearPlant,
			solarPlant,
			hospital,
			laboratory,
			massTransit,
			sam,
			coastalForteress,
			harbor,
			offshorePlatform,
			airport,
			civilDefense,
			stockExchange,
			commercialDock, */
			totp1
		}
		public enum civicImprovementChoice : byte
		{
			irrigation=1, mine
		}
	/*	public enum Stat.GovernementsList : byte
		{
			despotism, 
			feodalism, 
			monarchie, 
			republic, 
			democratie, 
			communism, 
			protectorate, 
			totp1
		}*/
		public enum technoList : byte
		{
			initial,
			writing, 
			masonery, 
			iron, 
			bronze, 
			pottery, 
			wheel, 
			mapMaking, 
			math,
			coastalNavigation,

			elephantBreed, // 10
			horseBreed,
			warElephant,
			attelage,
			chivalery,
			philosophy,
			monarchy,
			ancientDemocraty,
			oligarchy,
			steamPower,

			flight, // 20
			explosives,
			camelBreed, 
		/*	futureTechno1,
			futureTechno2,
			futureTechno3,
			futureTechno4,
			futureTechno5,*/
			totp1

			/*
			navigation, 
			alphabet,
			warrior, 
			ceremonialBurial, 
			construction, 
			engineering, 
			gunpowder, 
			astronomy, 
			economics, 
			magnetism, 
			nationalism, 
			steamPower, 
			electricity, 
			sanitation, 
			espionage, 
			refining, 
			replaceableParts, 
			flight, 
			radio, 
			rocketry, 
			fission, 
			recycling, 
			smartWeapons, 
			internet,
			cloning,  
			philosophy
			*/
		}

	/*	public enum terrainType : byte
		{
			sea,
			plain,
			forest,
			hill, 
			mountain, 
			coast, 
			prairie, 
			swamp,
			jungle, 
			tundra, 
			glacier, 
			desert, 
			totp1
		}*/
		public enum unitType : byte
		{
			colon, 
			warrior, 
			phalax, 
			knight, 
			camelWarrior, 
			cavalier, 
			legion, 
			elephant,
			frigate,
			catapult,
			fighter,
			bomber,
			nukeTactical, 
			nukeBomb,
			nukeICBM,
			cruiseMissile,
			cargoPlane,
			carrier,
			totp1
		}
		public enum relationPolType : byte
		{
			peace,
			war,
			ceaseFire,
			alliance,
			Protector,
			Protected, 
			totp1
		}
		#endregion

		#region VS automatiques
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mIFile;
		private System.Windows.Forms.Button cmdOk;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem mISaveAs;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem mICiv;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.Button cmdNext;
		private System.Windows.Forms.Button cmdPrevious;
		private System.Windows.Forms.Timer tmrResize;
		private System.Windows.Forms.MenuItem menuItem24;
		private System.Windows.Forms.MenuItem menuItem25;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.PictureBox cmdUnitLeft;
		private System.Windows.Forms.PictureBox cmdUnitRight;
		private System.Windows.Forms.PictureBox cmdTransportUnitRight;
		private System.Windows.Forms.PictureBox cmdTransportUnitLeft;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.MenuItem menuItem27;
		private System.Windows.Forms.MenuItem mISave;
		private System.Windows.Forms.MenuItem mISatellites;
		private System.Windows.Forms.MenuItem menuItem26;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.PictureBox pictureBox1;
		private MenuItem mIFortification;
		private MenuItem mIAirport;
		private System.Windows.Forms.MenuItem mIUnit;
		private System.Windows.Forms.MenuItem mIActions;
		private System.Windows.Forms.Timer tmrShowUnit;
		private System.Windows.Forms.Button cmdChangeMiniMap;
		private System.Windows.Forms.MenuItem mIQuestion;
		private System.Windows.Forms.Button cmdUp;
		private System.Windows.Forms.Button cmdLeft;
		private System.Windows.Forms.Button cmdRight;
		private System.Windows.Forms.Button cmdDown;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mIAbout;
		private System.Windows.Forms.MenuItem mITerrainInfo;
		private System.Windows.Forms.MenuItem mIUnitInfo;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem mILabels;
		private System.Windows.Forms.MenuItem miTest;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.MenuItem mIMineField;
		private System.Windows.Forms.MenuItem mITransport;

		#endregion
		
#endregion

#region Ptits boutons

	#region load map button
		private void menuItem2_Click(object sender, System.EventArgs e)
		{
		/*	if ( LoadMap() )
			{
				//	// waitCursor wC = new // waitCursor();
				wC.show = true;

			// InitGraphics();
			/*	initUnitStats();
				initTerrainStats();
				initTechnoStats();
				initBuildingStats();
			//	initStatistics.civilizations();
				initResourceStats();*
				
				Statistics.initAllBut();

				drawMiniMap();
				pictureBox1.Enabled = true;

				game.playerList = new PlayerList[ Form1.game.playerList.Length ];

				//				nbr civ name				food	prod	trade	Sci		happy	wealth
				enterNewPlayer(	0,	0,	"Xymus",			1,		1,		1,		5,		1,		4);
				enterNewPlayer(	1,	1,	"Bob Moon",			1,		1,		1,		5,		1,		4);
				enterNewPlayer(	2,	0,	"George W Bush",	1,		1,		1,		5,		1,		4);

				createUnit( 5, 14, Form1.game.curPlayerInd, (byte)Form1.unitType.colon );
				/*createUnit( 5, 15, Form1.game.curPlayerInd, (byte)Form1.unitType.elephant );
				createUnit( 5, 16, Form1.game.curPlayerInd, (byte)Form1.unitType.knight );
				createUnit( 6, 15, Form1.game.curPlayerInd, (byte)Form1.unitType.legion );
				createUnit( 5, 15, Form1.game.curPlayerInd, (byte)Form1.unitType.cavalier );

				createUnit ( 2, 13, Form1.game.curPlayerInd, (byte)Form1.unitType.frigate );*/

				/*createUnit( 4, 10, 1, (byte)Form1.unitType.knight );
				createUnit( 5, 10, 1, (byte)Form1.unitType.elephant );*/
				/*createUnit( 6, 10, 1, (byte)Form1.unitType.warrior );
				createUnit( 6, 11, 1, (byte)Form1.unitType.cavalier );
				createUnit( 5, 9, 1, (byte)Form1.unitType.legion );*/
				/*createCity( 8, 10, 1, false, "thebes");
				createCity( 27, 50, 1, false, "roger!"  );*
				createUnit( 10, 20, 1, (byte)Form1.unitType.colon );
				/*createUnit( 7, 14, 1, (byte)Form1.unitType.colon );
				createUnit( 9, 9, 1, (byte)Form1.unitType.colon );*
				
				//   platformSpec.keys.Set( this );//keyIn = new keyInPut();
				// timer1.Enabled = true;
				mICiv.Enabled = true;

			/*	waitingTmr = new System.Windows.Forms.Timer();
				waitingTmr.Interval = 1;
				waitingTmr.Enabled = false;
				waitingTmr.Tick += new System.EventHandler( waitingTmr_tick );*

				showUnit( Form1.game.curPlayerInd, 1 );

				wC.show = false;
				DrawMap();
				showInfo();

				// test pol
				/*game.playerList[0].relationPol[1] = (byte)relationPolType.war;
				game.playerList[1].relationPol[0] = (byte)relationPolType.war;*
			}*/
		}
		#endregion

	#region go dir
		public void goUp()
		{
			if (sliVer > 0)
			{
				sliVer -= 2;
				DrawMap();
			}
		}

		public void goDown()
		{
			if (sliVer < game.height - 12 )//- 16)
			{
				sliVer += 2;
				DrawMap();
			}
		}

		public void goLeft()
		{
			sliHor -= 1;
			DrawMap();

			if (sliHor < 0)
				sliHor = game.width - 1;
		}

		public void goRight()
		{
			sliHor += 1;
			DrawMap();

			if (sliHor > game.width)
				sliHor = 1;
		}
		#endregion

	#region cmd direction

		private void cmdUp_Click(object sender, System.EventArgs e)
		{
			goUp();
		}

		private void cmdRight_Click(object sender, System.EventArgs e)
		{
			goRight();
		}

		private void cmdDown_Click(object sender, System.EventArgs e)
		{
			goDown();
		}

		private void cmdLeft_Click(object sender, System.EventArgs e)
		{
			goLeft();
		}

		#endregion

#endregion

#region Initialize Graphics
		public void InitGraphics()
		{	
			try
			{	
			//	variableInitialized = true;

				cityBmp  = new Bitmap[2,3];
				cityBmpUnseen  = new Bitmap[2,3];
				for ( int i = 0; i < 2; i ++ )
					for ( int j = 0; j < 3; j ++ )
					{
						cityBmp[i,j] = new Bitmap(Form1.appPath + "\\images\\cities\\city" + i.ToString() + j.ToString() + ".png");
						cityBmpUnseen[i,j] =  new Bitmap(Form1.appPath + "\\images\\cities\\city" + i.ToString() + j.ToString() + " oos.png"); // drawUnseen( cityBmp[i,j] );
					}

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 0: city bmps", e.Message );
				Application.Exit();
			}

			try
			{	
				laborBmp = new Bitmap[2];
				laborBmp[ 0 ] = new Bitmap(Form1.appPath + "\\images\\labor0.png");
				laborBmp[ 1 ] = new Bitmap(Form1.appPath + "\\images\\labor1.png");
				laborBmpUnseen = new Bitmap[2];
				laborBmpUnseen[ 0 ] = new Bitmap(Form1.appPath + "\\images\\labor0 oos.png"); // drawUnseen( laborBmp[ 0 ] );
				laborBmpUnseen[ 1 ] = new Bitmap(Form1.appPath + "\\images\\labor1 oos.png"); // drawUnseen( laborBmp[ 1 ] );
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 1: labor bmps", e.Message );
				Application.Exit();
			}

			try
			{	
				//structures
				Statistics.units = new Stat.Unit[ (int)Form1.unitType.totp1 ]; // + d unités
				Statistics.terrains = new Stat.Terrain[ (int)enums.terrainType.totp1 ]; // adjust
				Statistics.technologies = new Stat.Technology[ (int)Form1.technoList.totp1  ]; //
				Statistics.buildings = new Stat.Building[ (int)buildingList.totp1 ]; //
				Statistics.resources = new Stat.Resource[ (byte)enums.ressource.tot ]; //structures.Statistics.resources[ (byte)enums.ressource.tot ]; //

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 2: init stats", e.Message );
				Application.Exit();
			}
			try
			{	
				move.stackBuffer = new UnitList[ 0 ];

		//		Form1.game.curPlayerInd = 0;

				//terrainImprovement
				civicImprovement = new Bitmap[3];
				civicImprovement[ (byte)civicImprovementChoice.mine] = new Bitmap(Form1.appPath + "\\images\\case imps\\updMine.png");
				civicImprovement[ (byte)civicImprovementChoice.irrigation] = new Bitmap(Form1.appPath + "\\images\\case imps\\updIrrigate.png");
				civicImprovementUnseen = new Bitmap[3];
				civicImprovementUnseen[ (byte)civicImprovementChoice.mine ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updMine oos.png"); //drawUnseen( civicImprovement[ (byte)civicImprovementChoice.mine ] );
				civicImprovementUnseen[ (byte)civicImprovementChoice.irrigation ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updIrrigate oos.png"); //drawUnseen( civicImprovement[ (byte)civicImprovementChoice.irrigation ] );
				
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 3: civicImprovements", e.Message );
				Application.Exit();
			}

			try
			{	
				militaryImprovement = new Bitmap[3];
				militaryImprovement[ (byte)enums.militaryImprovement.fort - 1 ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updFortMed.png");
				militaryImprovement[ (byte)enums.militaryImprovement.airport - 1 ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updAirport.png");
				militaryImprovement[ (byte)enums.militaryImprovement.mineField - 1 ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updMineField.png");
				militaryImprovementUnseen = new Bitmap[3];
				militaryImprovementUnseen[ (byte)enums.militaryImprovement.fort - 1 ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updFortMed oos.png"); //drawUnseen( militaryImprovement[ (byte)enums.militaryImprovement.fort - 1 ] );
				militaryImprovementUnseen[ (byte)enums.militaryImprovement.airport - 1 ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updAirport oos.png"); //drawUnseen( militaryImprovement[ (byte)enums.militaryImprovement.airport - 1 ] );
				militaryImprovementUnseen[ (byte)enums.militaryImprovement.mineField - 1 ] = new Bitmap(Form1.appPath + "\\images\\case imps\\updMineField oos.png"); //drawUnseen( militaryImprovement[ (byte)enums.militaryImprovement.mineField - 1 ] );

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 4: militaryImprovements", e.Message );
				Application.Exit();
			}
			try
			{	
				viewBmp = new Bitmap( this.Width, this.Height );
				
				terrainView = new Bitmap( this.Width, this.Height );
				oldSliVer = -1;
				oldSliHor = -1;
				somethingHaseBeenBuilt = false;

				caseSelected = new Bitmap(Form1.appPath + "\\images\\caseSelected.png");

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 5: caseSelected.bmp and insiders", e.Message );
				Application.Exit();
			}
			try
			{	
				specialResBmp = new Bitmap[ 3 ];
				specialResBmp[ 0 ] = new Bitmap(Form1.appPath + "\\images\\resources\\RHorse.png");
				specialResBmp[ 1 ] = new Bitmap(Form1.appPath + "\\images\\resources\\RElephant.png");
				specialResBmp[ 2 ] = new Bitmap(Form1.appPath + "\\images\\resources\\RCamel.png");
				specialResBmpUnseen = new Bitmap[ 3 ];
				specialResBmpUnseen[ 0 ] = new Bitmap(Form1.appPath + "\\images\\resources\\RHorse oos.png"); // drawUnseen( specialResBmp[ 0 ] );
				specialResBmpUnseen[ 1 ] = new Bitmap(Form1.appPath + "\\images\\resources\\RElephant oos.png"); // drawUnseen( specialResBmp[ 1 ] );
				specialResBmpUnseen[ 2 ] = new Bitmap(Form1.appPath + "\\images\\resources\\RCamel oos.png"); // drawUnseen( specialResBmp[ 2 ] );

				selected = new structures.sSelected();

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 6: horse/camel/elephant", e.Message );
				Application.Exit();
			}
			try
			{	
				g = Graphics.FromImage( viewBmp );

				blackBrush = new SolidBrush( Color.Black );
				whiteBrush = new SolidBrush( Color.White );
				cityPopFont = new Font("Tahoma", platformSpec.resolution.getStaticSizeForFont( 11 ), FontStyle.Regular);
				cityNameBrush = new SolidBrush(Color.White);
				cityPopBrush = new SolidBrush(Color.Black);
				cityPopPen = new Pen( Color.Black );
				//popBackBrush = new SolidBrush( Statistics.civilizations[ game.playerList[ cityOwner ].civType ].color );

				spyBmp = new Bitmap( Form1.appPath + "\\images\\Spy.png" );

				nukeExplosion = new Bitmap(Form1.appPath + "\\images\\nukeExplosion.png" );
				pollution = new Bitmap(Form1.appPath + "\\images\\pollution.png" );
				
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 7: spy/nuke/pollution and brushes", e.Message );
				Application.Exit();
			}
			try
			{	
				riverBmp = new Bitmap[ 8 ][];
				for ( int i = 0; i < riverBmp.Length; i ++ )
				{
					riverBmp[ i ] = new Bitmap[ 2 ];
					riverBmp[ i ][ 0 ] = new Bitmap( String.Format( "{0}\\images\\rivers\\river{1}.png",Form1.appPath, i ) );
					riverBmp[ i ][ 1 ] = new Bitmap( String.Format( "{0}\\images\\rivers\\river{1} oos.png",Form1.appPath, i ) ); //drawUnseen( riverBmp[ i ][ 0 ] );;
				}

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 8: river bitmaps", e.Message );
				Application.Exit();
			}
			try
			{	
				roadBmp = new Bitmap[ 9 ][ ];
				for ( int i = 0; i < roadBmp.Length; i++ )
				{
					roadBmp[ i ] = new Bitmap[ 2 ];
					roadBmp[ i ][ 0 ] = new Bitmap( String.Format( "{0}\\images\\roads\\road{1}.png",Form1.appPath, i ) );
					roadBmp[ i ][ 1 ] = new Bitmap( String.Format( "{0}\\images\\roads\\road{1} oos.png",Form1.appPath, i ) ); //drawUnseen( roadBmp[ i ][ 0 ] );;
				}

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 9: roads", e.Message );
				Application.Exit();
			}
			try
			{	
				railBmp = new Bitmap[ 9 ][ ];
				for ( int i = 0; i < railBmp.Length; i++ )
				{
					railBmp[ i ] = new Bitmap[ 2 ];
					railBmp[ i ][ 0 ] = new Bitmap( String.Format( "{0}\\images\\rails\\rail{1}.png",Form1.appPath, i ) );
					railBmp[ i ][ 1 ] = new Bitmap( String.Format( "{0}\\images\\rails\\rail{1} oos.png",Form1.appPath, i ) ); //drawUnseen( railBmp[ i ][ 0 ] );;
				}

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 10: rails", e.Message );
				Application.Exit();
			}
			try
			{	
				prodBmp = new Bitmap(Form1.appPath + "\\images\\prod.png" );
				foodBmp = new Bitmap(Form1.appPath + "\\images\\food.png" );
				tradeBmp = new Bitmap(Form1.appPath + "\\images\\trade.png" );

				peopleBmp = new Bitmap(Form1.appPath + "\\images\\people.png" );
			/*	slaverBmp = new Bitmap(Form1.appPath + "\\slaver.png" );
				slaveFoodBmp = new Bitmap(Form1.appPath + "\\slaveFood.png" );
				slaveProdBmp = new Bitmap(Form1.appPath + "\\slaveProd.png" );*/


			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 11: food, prod, trade and people.png", e.Message );
				Application.Exit();
			}
			try
			{	
				try
				{
					peopleNonLaborBmp = new Bitmap[ (byte)peopleNonLabor.types.totp1 ];
					slaveBmp = new Bitmap[ (byte)citySlavery.types.totp1 ];
				}
				catch ( Exception e )
				{
					MessageBox.Show( "Error loading main graphics, 12: slaves, non labor and labels init\nBitmap arrays", e.Message );
					Application.Exit();
				}

				try
				{
					peopleNonLaborBmp[ (byte)peopleNonLabor.types.slaver ] = new Bitmap(Form1.appPath + "\\images\\slaver.png" );
					peopleNonLaborBmp[ (byte)peopleNonLabor.types.entertainer ] = new Bitmap(Form1.appPath + "\\images\\entertainer.png" );
					peopleNonLaborBmp[ (byte)peopleNonLabor.types.trader ] = new Bitmap(Form1.appPath + "\\images\\trader.png" );
				}
				catch ( Exception e )
				{
					MessageBox.Show( "Error loading main graphics, 12: slaves, non labor and labels init\nFill peopleNonLaborBmp", e.Message );
					Application.Exit();
				}

				try
				{				
					slaveBmp[ (byte)citySlavery.types.food ] = new Bitmap(Form1.appPath + "\\images\\slaveFood.png" );
					slaveBmp[ (byte)citySlavery.types.prod ] = new Bitmap(Form1.appPath + "\\images\\slaveProd.png" );

					unitAffSli = 0;
					transportAffSli = 0;
				}
				catch ( Exception e )
				{
					MessageBox.Show( "Error loading main graphics, 12: slaves, non labor and labels init\nslaveBmp", e.Message );
					Application.Exit();
				}

				label.initList( 0 );

			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 12: slaves, non labor and labels init\nothers", e.Message );
				Application.Exit();
			}
			try
			{	
				unitActionAff = new string[ 30 ];
				unitActionAff[ (byte)Form1.unitState.idle ] = "i";
				unitActionAff[ (byte)Form1.unitState.fortified ] = "f";
				unitActionAff[ (byte)Form1.unitState.inTransport ] = "t";
				unitActionAff[ (byte)Form1.unitState.moving ] = "g";
				unitActionAff[ (byte)Form1.unitState.sleep ] = "s";
				unitActionAff[ (byte)Form1.unitState.wait ] = "w";
				unitActionAff[ (byte)Form1.unitState.turnCompleted ] = "c";

				unitActionAff[ (byte)Form1.unitState.buildingAirport ] = "A";
				unitActionAff[ (byte)Form1.unitState.buildingMineFld ] = "m";
				unitActionAff[ (byte)Form1.unitState.buildingFort ] = "F";
				unitActionAff[ (byte)Form1.unitState.buildingIrrigation ] = "I";
				unitActionAff[ (byte)Form1.unitState.buildingMine ] = "M";
				unitActionAff[ (byte)Form1.unitState.buildingRoad ] = "R";
				
				unitActionAff[ (byte)Form1.unitState.automate ] = "a";
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error loading main graphics, 13: unit action s", e.Message );
				this.Close();
			}
		}
		#endregion

#region Draw map 
		public void DrawMap()
		{
			if ( 
				this.Size.Width != 0 && this.Size.Height != 0 && 
				this.ClientSize.Width != 0 && this.ClientSize.Height != 0 
				)
			{
			/*try
			{*/
				//	Application.DoEvents(); 
				byte player = Form1.game.curPlayerInd;

				drawRect.xMin = -1;
				drawRect.yMin = -1;
				drawRect.xMax = this.ClientSize.Width / 50 + 1;
				drawRect.yMax = this.ClientSize.Height / 15 + 2;

				/*	if ( System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width > 320 ) 
				{
					drawRect.xMax = 10; 
					drawRect.yMax = 36; 
				}
				else
				{
					drawRect.xMax =  5; 
					drawRect.yMax = 18; 
				}*/
				if ( oldSliHor != sliHor || oldSliVer != sliVer )
				{
					viewBmp.Dispose();
					viewBmp = new Bitmap( this.ClientSize.Width, this.ClientSize.Height );

					g.Dispose();
					g = Graphics.FromImage( viewBmp );
					g.Clear( Color.Black ); 

					#region drawing the terrain
					if ( options.hideUndiscovered )
					{
						for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
							for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
								if ( y + sliVer >= 0 && y + sliVer < game.height )
									if ( x + sliHor < game.width && x + sliHor >= 0 )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] )
											g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor, y + sliVer ].type ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
										else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ] )
											g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor, y + sliVer ].type ].bmpUnseen, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
									}
									else if ( x + sliHor < 0 )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor + game.width, y + sliVer ] )
											g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor + game.width, y + sliVer ].type ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
										else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor + game.width, y + sliVer ] )
											g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor + game.width, y + sliVer ].type ].bmpUnseen, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
									}
									else if ( x + sliHor >= game.width )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor - game.width, y + sliVer ] )
											g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor - game.width, y + sliVer ].type ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
										else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor - game.width, y + sliVer ] )
											g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor - game.width, y + sliVer ].type ].bmpUnseen, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
									}
							}
					}
					else
					{
						for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
							for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
								if ( y + sliVer >= 0 && y + sliVer < game.height )
									if ( x + sliHor < game.width && x + sliHor >= 0 )
									{
										g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor, y + sliVer ].type ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
									}
									else if ( x + sliHor < 0 )
									{
										g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor + game.width, y + sliVer ].type ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
									}
									else if ( x + sliHor >= game.width )
									{
										g.DrawImage( Statistics.terrains[ game.grid[ x + sliHor - game.width, y + sliVer ].type ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
									}
							}
					}

					#region show game.grid

					if ( options.showGrid )
					{
						for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
							for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
							{
								Point[] pntSelected = new Point[] {
									new Point( x * caseWidth, y * caseHeight + caseHeight / 2 ), new Point( x * caseWidth + caseWidth / 2, y * caseHeight ), new Point( x * caseWidth + caseWidth, y * caseHeight + caseHeight / 2 ), new Point( x * caseWidth + caseWidth / 2, y * caseHeight + caseHeight ),
								};

								g.DrawPolygon( new Pen( Color.Black ), pntSelected );
							}
					}
					#endregion
					#endregion
		
					#region drawing city + selected + units
					if ( options.hideUndiscovered )
					{ // show only discovered
						for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
							for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
								if ( y + sliVer >= 0 && y + sliVer < game.height )
								{
									r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
									if ( x + sliHor < game.width && x + sliHor >= 0 )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ] || game.grid[ x + sliHor, y + sliVer ].territory - 1 == Form1.game.curPlayerInd )
											drawStaticCaseStuff( x, y );
									}
									else if ( x + sliHor < 0 )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor + game.width, y + sliVer ] || game.grid[ x + sliHor + game.width, y + sliVer ].territory - 1 == Form1.game.curPlayerInd )
											drawStaticCaseStuff( x + game.width, y );
									}
									else if ( x + sliHor >= game.width )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor - game.width, y + sliVer ] || game.grid[ x + sliHor - game.width, y + sliVer ].territory - 1 == Form1.game.curPlayerInd )
											drawStaticCaseStuff( x - game.width, y );
									}
								}
					}
					else
					{ // show all
						for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
							for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
								if ( y + sliVer >= 0 && y + sliVer < game.height )
								{
									r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
									if ( x + sliHor < game.width && x + sliHor >= 0 )
										drawStaticCaseStuff( x, y );
									else if ( x + sliHor < 0 )
										drawStaticCaseStuff( x + game.width, y );
									else if ( x + sliHor >= game.width )
										drawStaticCaseStuff( x - game.width, y );
								}
					}
					#endregion
		
					terrainView.Dispose();
					terrainView = new Bitmap( viewBmp );

					oldSliHor = sliHor;
					oldSliVer = sliVer;
				}
				else
				{
					viewBmp = new Bitmap( terrainView );
					g = Graphics.FromImage( viewBmp );
				}

				#region plans
				for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
					for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
						if ( y + sliVer >= 0 && y + sliVer < game.height - 1 )
							if ( x + sliHor < game.width && x + sliHor >= 0 )
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2, caseHeight / 2 * y, caseWidth, caseHeight );
								if ( game.plans.grid[ x + sliHor, y + sliVer ] )
									game.plans.drawCase( g, new Point( x + sliHor, y + sliVer ), r );
							}
							else if ( x + sliHor < 0 )
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2, caseHeight / 2 * y, caseWidth, caseHeight );
								if ( game.plans.grid[ x + sliHor + game.width, y + sliVer ] )
									game.plans.drawCase( g, new Point( x + sliHor + game.width, y + sliVer ), r );
							}
							else
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2, caseHeight / 2 * y, caseWidth, caseHeight );
								if ( game.plans.grid[ x + sliHor - game.width, y + sliVer ] )
									game.plans.drawCase( g, new Point( x + sliHor - game.width, y + sliVer ), r );
							}
							
				#endregion

				#region //////
				/*for (int y = drawRect.yMin; y < drawRect.yMax; y ++)
					for (int x = drawRect.xMin; x < drawRect.xMax; x ++)
						if ( x < game.height - 1 && y + sliVer >= 0 && y + sliVer < game.height - 1 )
							if ( x + sliHor < game.width && x + sliHor >= 0 )
							{
								r = new Rectangle(caseWidth*x+ ( (y + sliVer) %2) *caseWidth/2, caseHeight/2*y, caseWidth, caseHeight );
							
								if ( plans.grid[ x + sliHor, y + sliVer ] )
									plans.drawCase( g, new Point( x + sliHor, y + sliVer ), r );
							}
							else if( x + sliHor < 0 )
							{
								r = new Rectangle(caseWidth*x+ ( (y + sliVer) %2) *caseWidth/2, caseHeight/2*y, caseWidth, caseHeight );
							
								if ( plans.grid[ x + sliHor, y + sliVer ] )
									plans.drawCase( g, new Point( x + sliHor + game.width, y + sliVer ), r );
							}
							else
							{
								r = new Rectangle(caseWidth*x+ ( (y + sliVer) %2) *caseWidth/2, caseHeight/2*y, caseWidth, caseHeight );
							
								if ( plans.grid[ x + sliHor, y + sliVer ] )
							}
							*/
							
				#endregion

				#region drawing limit
				if ( options.hideUndiscovered )
				{
					for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
						for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
							if ( y + sliVer >= 0 && y + sliVer < game.height - 1 )
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
								if ( x + sliHor < game.width && x + sliHor >= 0 )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ] )
										draw.borders.draw( new Point( x + sliHor, y + sliVer ), new Point( r.Left, r.Top ), g, options.frontierType );
								}
								else if ( x + sliHor < 0 )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor + game.width, y + sliVer ] )
										draw.borders.draw( new Point( x + sliHor + game.width, y + sliVer ), new Point( r.Left, r.Top ), g, options.frontierType );
								}
								else
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor - game.width, y + sliVer ] )
										draw.borders.draw( new Point( x + sliHor - game.width, y + sliVer ), new Point( r.Left, r.Top ), g, options.frontierType );
								}
							}
				}
				else
				{
					for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
						for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
							if ( y + sliVer >= 0 && y + sliVer < game.height - 1 )
							{
								r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
								if ( x + sliHor < game.width && x + sliHor >= 0 )
									draw.borders.drawAll( new Point( x + sliHor, y + sliVer ), new Point( r.Left, r.Top ), g, options.frontierType );
								else if ( x + sliHor < 0 )
									draw.borders.drawAll( new Point( x + sliHor + game.width, y + sliVer ), new Point( r.Left, r.Top ), g, options.frontierType );
								else
									draw.borders.drawAll( new Point( x + sliHor - game.width, y + sliVer ), new Point( r.Left, r.Top ), g, options.frontierType );
							}
				}
				#endregion

				#region drawing change
				if ( options.hideUndiscovered )
				{ // show only discovered
					for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
						for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
						{
							r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
							if ( y + sliVer >= 0 && y + sliVer < game.height )
								if ( x + sliHor < game.width && x + sliHor >= 0 )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ] || game.grid[ x + sliHor, y + sliVer ].territory - 1 == Form1.game.curPlayerInd )
										drawChangeCaseStuff( x, y );
								}
								else if ( x + sliHor < 0 )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor + game.width, y + sliVer ] || game.grid[ x + sliHor + game.width, y + sliVer ].territory - 1 == Form1.game.curPlayerInd )
										drawChangeCaseStuff( x + game.width, y );
								}
								else if ( x + sliHor >= game.width )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor - game.width, y + sliVer ] || game.grid[ x + sliHor - game.width, y + sliVer ].territory - 1 == Form1.game.curPlayerInd )
										drawChangeCaseStuff( x - game.width, y );
								}
						}
				}
				else
				{ // show all
					for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
						for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
						{
							r = new Rectangle( caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 - 10, caseHeight / 2 * y - 20, caseWidth + 20, caseHeight + 20 );
							if ( y + sliVer >= 0 && y + sliVer < game.height )
							{
								if ( x + sliHor < game.width && x + sliHor >= 0 )
									drawChangeCaseStuff( x, y );
								else if ( x + sliHor < 0 )
									drawChangeCaseStuff( x + game.width, y );
								else if ( x + sliHor >= game.width )
									drawChangeCaseStuff( x - game.width, y );
							}
						}
				}
				#endregion

				#region draw labels
				if ( options.showLabels )
					for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
						for ( int x = drawRect.xMin - 3; x < drawRect.xMax + 3; x++ )
						{
							if ( y + sliVer >= 0 && y + sliVer < game.height )
							{
								int labelInd = -1;

								if ( x + sliHor < game.width && x + sliHor >= 0 )
								{
									labelInd = label.findAt( x + sliHor, y + sliVer );
								}
								else if ( x + sliHor < 0 )
								{
									labelInd = label.findAt( x + sliHor + game.width, y + sliVer );
								}
								else if ( x + sliHor >= game.width )
								{
									labelInd = label.findAt( x + sliHor - game.width, y + sliVer );
								}

								if ( labelInd != -1 )
								{
									Font labelFont = new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 13 ), FontStyle.Regular );
									SizeF size = g.MeasureString( label.list[ labelInd ].text, labelFont );

									g.DrawString( label.list[ labelInd ].text, labelFont, blackBrush, caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 + caseWidth / 2 - size.Width / 2 + 1, caseHeight / 2 * y + caseHeight / 2 - size.Height / 2 + 1 );
									g.DrawString( label.list[ labelInd ].text, labelFont, whiteBrush, caseWidth * x + ( ( y + sliVer ) % 2 ) * caseWidth / 2 + caseWidth / 2 - size.Width / 2, caseHeight / 2 * y + caseHeight / 2 - size.Height / 2 );
								}
							}
						}
				#endregion

				#region city info
				for ( int y = drawRect.yMin; y < drawRect.yMax; y++ )
					for ( int x = drawRect.xMin; x < drawRect.xMax; x++ )
					{
						//	r = new Rectangle(caseWidth*x+ ( (y + sliVer) %2) *caseWidth/2 - 10, caseHeight/2*y - 20, caseWidth + 20, caseHeight + 20);
						if ( options.hideUndiscovered )
						{ // show only discovered
							if ( y + sliVer >= 0 && y + sliVer < game.height )
								if ( x + sliHor < game.width && x + sliHor >= 0 )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] )
									{
										if ( game.grid[ x + sliHor, y + sliVer ].city > 0 )
											drawCityInfo( x, x, y );
									}
									else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ] )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer ].city > 0 )
											drawCityInfoUnseen( x, x, y );
									}
								}
								else if ( x + sliHor < 0 )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor + game.width, y + sliVer ] )
									{
										if ( game.grid[ x + sliHor + game.width, y + sliVer ].city > 0 )
											drawCityInfo( x + game.width, x, y );
									}
									else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor + game.width, y + sliVer ] )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor + game.width, y + sliVer ].city > 0 )
											drawCityInfoUnseen( x + game.width, x, y );
									}
								}
								else if ( x + sliHor >= game.width )
								{
									if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor - game.width, y + sliVer ] )
									{
										if ( game.grid[ x + sliHor - game.width, y + sliVer ].city > 0 )
											drawCityInfo( x - game.width, x, y );
									}
									else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor - game.width, y + sliVer ] )
									{
										if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor - game.width, y + sliVer ].city > 0 )
											drawCityInfoUnseen( x - game.width, x, y );
									}
								}
						}
						else
						{
							if ( y + sliVer >= 0 && y + sliVer < game.height )
								if ( x + sliHor < game.width && x + sliHor >= 0 )
								{
									if ( game.grid[ x + sliHor, y + sliVer ].city > 0 )
										drawCityInfo( x, x, y );
								}
								else if ( x + sliHor < 0 )
								{
									if ( game.grid[ x + sliHor + game.width, y + sliVer ].city > 0 )
										drawCityInfo( x + game.width, x, y );
								}
								else if ( x + sliHor >= game.width )
								{
									if ( game.grid[ x + sliHor - game.width, y + sliVer ].city > 0 )
										drawCityInfo( x - game.width, x, y );
								}
						}
					}
				#endregion

				if ( selectState == (byte)enums.selectionState.nukeExplosion )
				{
					//	Bitmap nukeExplosion = new Bitmap(Form1.appPath + "\\nukeExplosion.png"  );
					g.DrawImage( 
					nukeExplosion, 
					new Rectangle( 
						45,//	this.Width / 2 - nukeExplosion.Width / 2, 
						10, 
						nukeExplosion.Width, 
						nukeExplosion.Height 
						), 
					0, 0, 
					nukeExplosion.Width, 
					nukeExplosion.Height, 
					GraphicsUnit.Pixel, ia 
					);
				}

				// top left
				g.DrawLine( new Pen( Color.Gray ), 17, 20, 25, 20 );
				g.DrawLine( new Pen( Color.Gray ), 20, 17, 20, 25 );

				// down left
				g.DrawLine( new Pen( Color.Gray ), 17, this.ClientSize.Height - 20, 25, this.ClientSize.Height - 20 );
				g.DrawLine( new Pen( Color.Gray ), 20, this.ClientSize.Height - 17, 20, this.ClientSize.Height - 25 );

				// top right
				g.DrawLine( new Pen( Color.Gray ), this.ClientSize.Width - 17, 20, this.ClientSize.Width - 25, 20 );
				g.DrawLine( new Pen( Color.Gray ), this.ClientSize.Width - 20, 17, this.ClientSize.Width - 20, 25 );

				#region draw battery
#if CF
				if ( options.showBatteryStatus && platformSpec.main.isPPC )
				{
					int perc = platformSpec.main.battryLife; 

					if ( perc != -1 )
					{
						if ( perc > 100 )
							perc = 100;

						Rectangle rBatt = new Rectangle( 8,  6, 100 / 3, 8 );

						g.FillRectangle( new SolidBrush( Color.WhiteSmoke ), rBatt );

						if ( perc > 50 )
							g.FillRectangle( new SolidBrush( Color.Green ), 8, 6, perc / 3, 8 );
						else if ( perc > 20 )
							g.FillRectangle( new SolidBrush( Color.Yellow ), 8, 6, perc / 3, 8 );
						else
							g.FillRectangle( new SolidBrush( Color.Red ), 200, 6, perc / 3, 8 );

						g.DrawRectangle( new Pen( Color.Black ), rBatt );
					}
				}
#endif
				#endregion

				#region write year + money
		//		int year = -4000 + game.curTurn * 20;
				string yearStr = game.yearString(); // = "year " + Convert.ToString( year, 10 );
/*
				if ( year < 0 )
					yearStr = Convert.ToString( game.year * -1, 10 ) + " BC";
				else
					yearStr = game.year.ToString() + " AD";*/

				Font yearFont = new Font( "Tahoma", 10, FontStyle.Regular );
				SizeF yearSize = g.MeasureString( yearStr, yearFont );

				g.DrawString( yearStr, yearFont, blackBrush, this.Width - yearSize.Width - 5 + 1, pictureBox2.Top - yearSize.Height - 3 + 1 );
				g.DrawString( yearStr, yearFont, whiteBrush, this.Width - yearSize.Width - 5, pictureBox2.Top - yearSize.Height - 3 );

				//	int nationTrade = getPFT1.getNationTrade( Form1.game.curPlayerInd ); 
				SolidBrush moneyBrush;

				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * game.playerList[ Form1.game.curPlayerInd ].preferences.reserve / 100 > 0 )
					moneyBrush = new SolidBrush( Color.White );
				else if ( game.playerList[ Form1.game.curPlayerInd ].preferences.reserve < -1 * ( Form1.game.playerList[ Form1.game.curPlayerInd ].money * 100 ) / ( ( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade == 0 ? 1 : Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade ) * 10 ) )
					moneyBrush = new SolidBrush( Color.Red );
				else
					moneyBrush = new SolidBrush( Color.Yellow );

				string money = game.playerList[ Form1.game.curPlayerInd ].money.ToString() + " gold";
				SizeF moneySize = g.MeasureString( money, yearFont );

				g.DrawString( money, yearFont, blackBrush, this.ClientSize.Width - moneySize.Width - 5 + 1, pictureBox2.Top - yearSize.Height - moneySize.Height - 2 + 1 );
				g.DrawString( money, yearFont, moneyBrush, this.ClientSize.Width - moneySize.Width - 5, pictureBox2.Top - yearSize.Height - moneySize.Height - 2 ); 
				#endregion

			//	if ( game.playerList[ Form1.game.curPlayerInd ].playerName.StartsWith( "Alexis Laf" ) )
			//	{
					g.DrawString( selected.X.ToString(), yearFont, new SolidBrush( Color.WhiteSmoke ), 5, 180 );
					g.DrawString( selected.Y.ToString(), yearFont, new SolidBrush( Color.WhiteSmoke ), 5, 205 );
			//	}

				if ( selectState == (byte)enums.selectionState.bombard )
					drawSelectState( g, language.getAString( language.order.ssBombard ) );
				else if ( selectState == (byte)enums.selectionState.goTo )
					drawSelectState( g, language.getAString( language.order.ssGoTo ) );
				else if ( selectState == (byte)enums.selectionState.launchMissile )
					drawSelectState( g, language.getAString( language.order.ssLaunchMissile ) );
				else if ( selectState == (byte)enums.selectionState.planeRebase )
					drawSelectState( g, language.getAString( language.order.ssPlaneRebase ) );
				else if ( selectState == (byte)enums.selectionState.planeRecon )
					drawSelectState( g, language.getAString( language.order.ssPlaneRecon ) );

				float pos = 4 + 20;

				if ( game.playerList[ Form1.game.curPlayerInd ].see[ selected.X, selected.Y ] )
				{
					if ( game.grid[ selected.X, selected.Y ].territory > 0 )
					{
						#region nation name
						SizeF nationName = g.MeasureString( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].civType ].name, yearFont );

						g.DrawString( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].civType ].name, yearFont, blackBrush, this.Width - nationName.Width - 5 + 1, pos + 1 );
						g.DrawString( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].civType ].name, yearFont, whiteBrush, this.Width - nationName.Width - 5, pos );
						pos += nationName.Height;
						#endregion
					}

					if ( game.grid[ selected.X, selected.Y ].city > 0 )
					{

						#region city name
						SizeF cityName = g.MeasureString( game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].name, yearFont );

						g.DrawString( game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].name, yearFont, blackBrush, this.Width - cityName.Width - 5 + 1, pos + 1 );
						g.DrawString( game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].name, yearFont, whiteBrush, this.Width - cityName.Width - 5, pos );
						pos += cityName.Height;
						#endregion

						if ( game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd )
						{
							#region build text

							string buildText = "Building " + game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].construction.list[ 0 ].name.ToLower();

						/*	string buildText;

							if ( game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].construction.list[ 0 ].type == 1 )
							{ // unit
								buildText = "Building " + Statistics.units[ game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].construction.list[ 0 ].ind ].name.ToLower();
							}
							else if ( game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].construction.list[ 0 ].type == 2 )
							{ // building
								buildText = "Building " + Statistics.buildings[ game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].cityList[ game.grid[ selected.X, selected.Y ].city ].construction.list[ 0 ].ind ].name.ToLower();
							}
							else
							{ // wealth
								buildText = "Building wealth";
							}*/

							SizeF buildName = g.MeasureString( buildText, yearFont );

							g.DrawString( buildText, yearFont, blackBrush, this.Width - buildName.Width - 5 + 1, pos + 1 );
							g.DrawString( buildText, yearFont, whiteBrush, this.Width - buildName.Width - 5, pos );
							pos += buildName.Height;
							#endregion

							#region turns left
							int turnsLeft = count.turnsLeftToBuild( (byte)( game.grid[ selected.X, selected.Y ].territory - 1 ), game.grid[ selected.X, selected.Y ].city );

							if ( turnsLeft != -1 )
							{
								string turnText;

								if ( turnsLeft < 2 )
									turnText = turnsLeft.ToString() + " turn left";
								else
									turnText = turnsLeft.ToString() + " turns left";

								SizeF turnSize = g.MeasureString( turnText, yearFont );

								g.DrawString( turnText, yearFont, blackBrush, this.Width - turnSize.Width - 5 + 1, pos + 1 );
								g.DrawString( turnText, yearFont, whiteBrush, this.Width - turnSize.Width - 5, pos );
								pos += turnSize.Height;
							}
							#endregion
						}
					}
				}
				else if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ selected.X, selected.Y ] )
				{
					//float pos = 4;
					if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory > 0 )
					{
						#region nation name
						SizeF nationName = g.MeasureString( Statistics.civilizations[ game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory - 1 ].civType ].name, yearFont );

						g.DrawString( Statistics.civilizations[ game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory - 1 ].civType ].name, yearFont, blackBrush, this.Width - nationName.Width - 5 + 1, pos + 1 );
						g.DrawString( Statistics.civilizations[ game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory - 1 ].civType ].name, yearFont, whiteBrush, this.Width - nationName.Width - 5, pos );
						pos += nationName.Height;
						#endregion
					}

					if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].city > 0 )
					{
						#region city name
						SizeF cityName = g.MeasureString( game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory - 1 ].cityList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].city ].name, yearFont );

						g.DrawString( game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory - 1 ].cityList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].city ].name, yearFont, blackBrush, this.Width - cityName.Width - 5 + 1, pos + 1 );
						g.DrawString( game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].territory - 1 ].cityList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].city ].name, yearFont, whiteBrush, this.Width - cityName.Width - 5, pos );
						pos += cityName.Height;
						#endregion
					}

					#region last seen
					int turnsAgo = game.curTurn - game.playerList[ Form1.game.curPlayerInd ].lastSeen[ selected.X, selected.Y ].turn;
					string taText;

					if ( turnsAgo < 2 )
						taText = turnsAgo.ToString() + " turn ago";
					else
						taText = turnsAgo.ToString() + " turns ago";

					SizeF taSize = g.MeasureString( taText, yearFont );

					g.DrawString( taText, yearFont, blackBrush, this.Width - taSize.Width - 5 + 1, pos + 1 );
					g.DrawString( taText, yearFont, whiteBrush, this.Width - taSize.Width - 5, pos );
					pos += taSize.Height;
					#endregion
				}

				pictureBox1.Image = viewBmp;
				pictureBox1.Update();
				affMiniMap();
				showInfo();
			/*}
			catch
			{
				MessageBox.Show( "Error drawing map" );
			}*/
			}
		}

	#region draw city info
		public void drawCityInfo ( int x1, int x, int y )
		{
			byte cityOwner = (byte)(game.grid[ x1 + sliHor, y + sliVer ].territory - 1);
			int city = game.grid[ x1 + sliHor, y + sliVer ].city;

			if ( game.playerList[ cityOwner ].cityList[ city ].isCapitale )
				cityNameFont = new Font("Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Underline);
			else
				cityNameFont = new Font("Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Regular);

			popBackBrush = new SolidBrush( Statistics.civilizations[ game.playerList[ cityOwner ].civType ].color );

			int pop = game.playerList[ cityOwner ].cityList[ city ].population + game.playerList[ cityOwner ].cityList[ city ].slaves.total;
			
			SizeF popSize = g.MeasureString( 
				pop.ToString(), 
				cityPopFont
				);

			SizeF nameSize = g.MeasureString( 
				game.playerList[ cityOwner ].cityList[ city ].name, 
				cityNameFont
				);

			Rectangle rectPop = new Rectangle( 
				caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + 31 + 5, 
				caseHeight/2*y - 8 - 5, 
				(int)popSize.Width + 3, 
				(int)popSize.Height - 2 
				); 
		
			g.FillRectangle( popBackBrush, rectPop );
			if ( game.playerList[ cityOwner ].cityList[ city ].slaves.total > 0 )
			{
				double slaveProp = (double)game.playerList[ cityOwner ].cityList[ city ].slaves.total / (double)pop;
				g.FillRectangle(
					new SolidBrush( Color.Red ), 
					new Rectangle( 
						rectPop.X,
						rectPop.Y + rectPop.Height * 4 / 5,
						(int)( rectPop.Width * slaveProp),
						rectPop.Height / 5 + 1
					)
				); 
			}
			g.DrawRectangle( cityPopPen, rectPop ); 
									
			g.DrawString( 
				pop.ToString(),
				cityPopFont, cityPopBrush, 
				rectPop.X + 2, 
				rectPop.Y - 1
				);

			g.DrawString(	
				game.playerList[ cityOwner ].cityList[ city ].name, 
				cityNameFont, 
				cityPopBrush, 
				caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + caseWidth / 2 - nameSize.Width / 2 + 1 /*+ 5*/, // + 14
				caseHeight/2*y + 22 + 1 - 5
				);

			g.DrawString(	
				game.playerList[ cityOwner ].cityList[ city ].name, 
				cityNameFont, 
				cityNameBrush, 
				caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + caseWidth / 2 - nameSize.Width / 2 /*+ 5*/, // + 15
				caseHeight/2*y + 22 - 5
				);
		}
		#endregion

	#region draw city info unseen
		public void drawCityInfoUnseen ( int x1, int x, int y )
		{
			byte cityOwner = (byte)( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x1 + sliHor, y + sliVer ].territory - 1 );
			if ( !game.playerList[ cityOwner ].dead )
			{
				int city = game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x1 + sliHor, y + sliVer ].city;

				if ( game.playerList[ cityOwner ].cityList[ city ].isCapitale )
					cityNameFont = new Font("Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Underline);
				else
					cityNameFont = new Font("Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Regular);

				popBackBrush = new SolidBrush( Statistics.civilizations[ game.playerList[ cityOwner ].civType ].color );

				SizeF popSize = g.MeasureString( 
					game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x1 + sliHor, y + sliVer ].cityPop.ToString(), 
					cityPopFont
					);

				SizeF nameSize = g.MeasureString( 
					game.playerList[ cityOwner ].cityList[ city ].name, 
					cityNameFont
					);

				Rectangle rectPop = new Rectangle( 
					caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + 29 + 2, 
					caseHeight/2*y - 4 - 4,  //	
					(int)popSize.Width + 3, 
					(int)popSize.Height - 2 // 16 
					);
		
				g.FillRectangle( popBackBrush, rectPop ); 
				g.DrawRectangle( cityPopPen, rectPop ); 
									
				g.DrawString( 
					game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x1 + sliHor, y + sliVer ].cityPop.ToString(),
					cityPopFont, cityPopBrush, 
					rectPop.X + 2, 
					rectPop.Y - 1
					);

				g.DrawString(	game.playerList[ cityOwner ].cityList[ city ].name, 
					cityNameFont, cityPopBrush, 
					caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + caseWidth / 2 - nameSize.Width / 2 + 1, // + 14
					caseHeight/2*y + 22 + 1);

				g.DrawString(	game.playerList[ cityOwner ].cityList[ city ].name, 
					cityNameFont, cityNameBrush, 
					caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + caseWidth / 2 - nameSize.Width / 2, // + 15
					caseHeight/2*y + 22);
			}
		}
		#endregion

	#region draw unit info
		public void drawUnitInfo ( int x, int y )
		{
			Rectangle rectEllipse = new Rectangle(
				r.X + 43 - 30,
				r.Y + 2,
				12,
				15
				);

			g.FillEllipse( 
				new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].stack[ game.grid[ x + sliHor, y + sliVer ].stackPos - 1 ].player.player ].civType ].color),
				rectEllipse
				);

			g.DrawEllipse(
				new Pen( Color.Black ),
				rectEllipse
				);

			if ( game.grid[ x + sliHor, y + sliVer ].stack[ game.grid[ x + sliHor, y + sliVer ].stackPos - 1 ].player.player == Form1.game.curPlayerInd )
			{
				Font actionFont = new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 8 ), FontStyle.Regular );
				SizeF actionSize = g.MeasureString( unitActionAff[ game.grid[ x + sliHor, y + sliVer ].stack[ game.grid[ x + sliHor, y + sliVer ].stackPos - 1 ].state ], actionFont );
				g.DrawString(
					unitActionAff[ game.grid[ x + sliHor, y + sliVer ].stack[ game.grid[ x + sliHor, y + sliVer ].stackPos - 1 ].state ],
					actionFont,
					blackBrush,
					r.X + 49 - actionSize.Width / 2 - 30,
					r.Y + 3
					);
			}

			for ( int i = 1; i <= game.grid[ x + sliHor, y + sliVer ].stack[ game.grid[ x + sliHor, y + sliVer ].stackPos - 1 ].health; i++)
			{
				Rectangle rectHealth = new Rectangle(
					r.X + 47 - 30,
					r.Y + 12 + 5 * i,
					3,
					5
					);

				g.FillRectangle(
					new SolidBrush( Color.GreenYellow ),
					rectHealth
					);

				g.DrawRectangle(
					new Pen( Color.Black),
					rectHealth
					);

			}
		}
		#endregion

	#region draw static case stuff
		public void drawStaticCaseStuff ( int x, int y )
		{
			if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] || !options.hideUndiscovered )
			{
				if ( game.grid[ x + sliHor, y + sliVer ].river )
					rivers.drawCase( g, new Point( x + sliHor, y + sliVer ), r );

				if ( game.grid[ x + sliHor, y + sliVer ].polluted )
					g.DrawImage( 
						pollution, 
						r, 
						0, 
						0, 
						caseWidth + 20, 
						caseHeight + 20, 
						GraphicsUnit.Pixel, 
						ia
						);
				/*	g.DrawImage( 
						roadBmp[ game.grid[ x + sliHor, y + sliVer].roadLevel - 1 ], 
						r, 
						0, 
						0, 
						caseWidth + 20, 
						caseHeight + 20, 
						GraphicsUnit.Pixel, 
						ia
						);*/

				/*civic imp*/
				if ( game.grid[ x + sliHor, y + sliVer].civicImprovement > 0 )
					g.DrawImage( civicImprovement[ game.grid[ x + sliHor, y + sliVer ].civicImprovement ], r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );

				/*roads*/
				if ( game.grid[ x + sliHor, y + sliVer ].roadLevel > 0 )
					roads.drawCase( g, new Point( x + sliHor, y + sliVer ), r );

				/*military imp*/
				if ( game.grid[ x + sliHor, y + sliVer].militaryImprovement > 0 )
					g.DrawImage( militaryImprovement[ game.grid[ x + sliHor, y + sliVer].militaryImprovement - 1 ], r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia);

				/*labor*/
				if ( game.grid[ x+ sliHor, y+ sliVer].laborCity > 0 )
					g.DrawImage(  laborBmp[ Statistics.terrains[ game.grid[ x+ sliHor, y+ sliVer ].type ].ew ] , r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
			}
			else  //Unseen
			{
				if ( game.grid[ x + sliHor, y + sliVer ].river )
					rivers.drawCase( g, new Point( x + sliHor, y + sliVer ), r );
				/*	g.DrawImage( 
						roadBmpUnseen[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer].road - 1 ], 
						r, 
						0, 
						0, 
						caseWidth + 20, 
						caseHeight + 20, 
						GraphicsUnit.Pixel, 
						ia
						);*/
/*civic imp*/
				if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer].civicImp > 0 )
					g.DrawImage( civicImprovementUnseen[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer ].civicImp ], r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );

				/*roads*/
				if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer ].road > 0 )
					roads.drawCase( g, new Point( x + sliHor, y + sliVer ), r );

/*military imp*/
				if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer].militaryImp > 0 )
					g.DrawImage( militaryImprovementUnseen[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer].militaryImp - 1 ], r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia);
 // idea /*labor*/
					/*	if ( game.grid[ x+ sliHor, y+ sliVer].laborCity > 0 )
									g.DrawImage(  laborBmpUnseen[ Statistics.terrains[ game.grid[ x+ sliHor, y+ sliVer ].type ].ew ] , r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);*/
			}
	}
	#endregion

	#region draw change case stuff
		public void drawChangeCaseStuff ( int x, int y )
		{
			if ( game.grid[ x + sliHor, y + sliVer].selectedLevel > 0 )
			{	/*bomb*/
				Point[] pntSelected = new Point[]
				{
					new Point( r.Left + 10 + 1,					r.Top + 20 + caseHeight / 2 + 1 ),
					new Point( r.Left + 10 + caseWidth / 2,		r.Top + 20 + 1 ),
					new Point( r.Left + 10 + caseWidth + 1,		r.Top + 20 + caseHeight / 2 + 1 ),
					new Point( r.Left + 10 + caseWidth / 2,		r.Top + 20 + caseHeight ),
				};

				if ( game.grid[ x + sliHor, y + sliVer].selectedLevel == 1 ) // bombards, nuke, planes, etc
				{
					g.DrawPolygon( new Pen( Color.Red ), pntSelected );
				}
				else if ( game.grid[ x + sliHor, y + sliVer].selectedLevel == 2 )
				{
					g.DrawPolygon( new Pen( Color.White ), pntSelected );
				}
			}

			if ( game.grid[ x + sliHor, y + sliVer ].resources >= 100 )
			{ // resources
				if ( game.playerList[ Form1.game.curPlayerInd ].technos[ Statistics.resources[ game.grid[ x + sliHor, y + sliVer ].resources - 100 ].techno ].researched )
				{
					if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] )
						g.DrawImage( Statistics.resources[ game.grid[ x + sliHor, y + sliVer ].resources - 100 ].bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
					else
						g.DrawImage( Statistics.resources[ game.grid[ x + sliHor, y + sliVer ].resources - 100 ].bmpUnseen, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia );
				}
			}
			else if ( game.grid[ x + sliHor, y + sliVer].resources > 0 )
			{ /*special*/
				if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] )
					g.DrawImage( specialResBmp[ game.grid[ x + sliHor, y + sliVer].resources - 1 ], r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia);
				else
					g.DrawImage( specialResBmpUnseen[ game.grid[ x + sliHor, y + sliVer].resources - 1 ], r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia);
			}

			if ( selected.X == x + sliHor && selected.Y == y + sliVer ) /*selected*/
				g.DrawImage( caseSelected , r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia);

		#region cities
			if ( game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] || !options.hideUndiscovered )
			{
				if ( game.grid[ x + sliHor, y + sliVer].city > 0 )
					g.DrawImage( 
						cityBmp[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].cityType, game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].cityList[ game.grid[ x + sliHor, y + sliVer ].city ].citySize ], 
						r, 
						0, 
						0, 
						caseWidth + 20, 
						caseHeight + 20, 
						GraphicsUnit.Pixel, 
						ia 
						); 
			}
			else
			{
				if ( game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer ].city > 0 ) 
					g.DrawImage( 
					cityBmpUnseen[ game.playerList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer ].territory - 1 ].cityType, game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].cityList[ game.playerList[ Form1.game.curPlayerInd ].lastSeen[ x + sliHor, y + sliVer ].city ].citySize ], 
						r, 
						0, 
						0, 
						caseWidth + 20, 
						caseHeight + 20, 
						GraphicsUnit.Pixel, 
						ia 
						); 
			}
			#endregion

			if ( 
				game.grid[ x + sliHor, y + sliVer].stack.Length > 0 && 
				(
					game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ] ||
					!options.hideUndiscovered
				)
				)
			{ /*units*/
				g.DrawImage( game.grid[ x + sliHor, y + sliVer ].stack[ game.grid[  x + sliHor, y + sliVer ].stackPos - 1 ].typeClass.bmp, r, 0, 0, caseWidth + 20, caseHeight + 20, GraphicsUnit.Pixel, ia);
				drawUnitInfo( x, y );
			}
		}
		#endregion

	#region draw limit
		private void drawLimit( int x, int y )
		{
			Point[] dCases = Form1.game.radius.return2DownCases( x + sliHor, y + sliVer );

			if ( options.hideUndiscovered )
			{
				#region limit right
				if ( game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory != game.grid[ x + sliHor, y + sliVer].territory )
				{
					if ( 
						game.playerList[ Form1.game.curPlayerInd ].see[ dCases[ 0 ].X, dCases[ 0 ].Y ] ||
						game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ]
						)
					{
						Point topLeft = new Point( 
							r.X + 10,
							r.Y + 20 + caseHeight/2 - 3
							);
						Point downLeft = new Point(
							r.X + 10 - 4,
							r.Y + 20 + caseHeight/2
							);
						Point topRight = new Point(
							r.X + 10 + caseWidth / 2 + 4,
							r.Y + 20 + caseHeight
							);
						Point downRight = new Point( 
							r.X + 10 + caseWidth / 2,
							r.Y + 20 + caseHeight + 3
							);

						Point[] frontier = {
											   topLeft,
											   downLeft,
											   downRight,
											   topRight
										   };

						if ( 
							game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory > 0 && 
							game.grid[ x + sliHor, y + sliVer].territory > 0 
							) 
						{
							if ( options.frontierType == 0 )
							{
								if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.war )
									g.FillPolygon( new SolidBrush( Color.Red ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.peace )
									g.FillPolygon( new SolidBrush( Color.White ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.ceaseFire )
									g.FillPolygon( new SolidBrush( Color.Gray ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance )
									g.FillPolygon( new SolidBrush( Color.Gold ), frontier );
							}
							else
							{
								Point middleLeft = new Point(
									r.X + 10 - 2,
									r.Y + 20 + caseHeight/2 - 1
									);
								Point middleRight = new Point( 
									r.X + 10 + caseWidth / 2 + 2,
									r.Y + 20 + caseHeight + 2
									);

								Point[] frontierTop = {
														  topLeft,
														  middleLeft,
														  middleRight,
														  topRight
													  };

								Point[] frontierDown = {
														   middleLeft,
														   downLeft,
														   downRight,
														   middleRight
													   };

								g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].civType ].color ), frontierTop );
								g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].civType ].color ), frontierDown );
							}
				
						}
						else if ( game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory == 0 )
							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].civType ].color ), frontier );
						else if ( game.grid[ x + sliHor, y + sliVer].territory == 0 )
							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].civType ].color ), frontier );
					}
					else if ( 
						game.playerList[ Form1.game.curPlayerInd ].discovered[ dCases[ 0 ].X, dCases[ 0 ].Y ] ||
						game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ]
						)
					{
						Point topLeft = new Point( 
							r.X + 10,
							r.Y + 20 + caseHeight/2 - 3
							);
						Point downLeft = new Point(
							r.X + 10 - 4,
							r.Y + 20 + caseHeight/2
							);
						Point topRight = new Point(
							r.X + 10 + caseWidth / 2 + 4,
							r.Y + 20 + caseHeight
							);
						Point downRight = new Point( 
							r.X + 10 + caseWidth / 2,
							r.Y + 20 + caseHeight + 3
							);

						Point[] frontier = {
												topLeft,
												downLeft,
												downRight,
												topRight
											};

						if ( 
							game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory > 0 && 
							game.grid[ x + sliHor, y + sliVer].territory > 0 
							) 
						{
							if ( options.frontierType == 0 )
							{
								if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.war )
									g.FillPolygon( new SolidBrush( unseenColor( Color.Red ) ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.peace )
									g.FillPolygon( new SolidBrush( unseenColor( Color.White ) ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.ceaseFire )
									g.FillPolygon( new SolidBrush( unseenColor( Color.Gray ) ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance )
									g.FillPolygon( new SolidBrush( unseenColor( Color.Gold ) ), frontier );
							}
							else
							{
								Point middleLeft = new Point(
									r.X + 10 - 2,
									r.Y + 20 + caseHeight/2 - 1
									);
								Point middleRight = new Point( 
									r.X + 10 + caseWidth / 2 + 2,
									r.Y + 20 + caseHeight + 2
									);

								Point[] frontierTop = {
															topLeft,
															middleLeft,
															middleRight,
															topRight
														};

								Point[] frontierDown = {
															middleLeft,
															downLeft,
															downRight,
															middleRight
														};

								g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].civType ].color ) ), frontierTop );
								g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].civType ].color ) ), frontierDown );
							}
				
						}
						else if ( game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory == 0 )
							g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].civType ].color ) ), frontier );
						else if ( game.grid[ x + sliHor, y + sliVer].territory == 0 )
							g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].civType ].color ) ), frontier );
					}
				}
				#endregion

				#region limit left
				if ( game.grid[ dCases[ 1 ].X, dCases[ 0 ].Y ].territory != game.grid[ x + sliHor, y + sliVer ].territory )
				{
					if ( 
						game.playerList[ Form1.game.curPlayerInd ].see[ dCases[ 1 ].X, dCases[ 1 ].Y ] ||
						game.playerList[ Form1.game.curPlayerInd ].see[ x + sliHor, y + sliVer ]
						)
					{ // right
						Point topRight = new Point( 
							r.X + 10 + caseWidth,
							r.Y + 20 + caseHeight/2 - 3
							);
						Point downRight = new Point(
							r.X + 10 + caseWidth + 4,
							r.Y + 20 + caseHeight/2
							);
						Point topLeft = new Point(
							r.X + 10 + caseWidth / 2 - 4,
							r.Y + 20 + caseHeight
							);
						Point downLeft = new Point( 
							r.X + 10 + caseWidth / 2,
							r.Y + 20 + caseHeight  + 3
							);
			   
						Point[] frontier = {
											   topLeft,
											   downLeft,
											   downRight,
											   topRight
										   };

						//g.FillPolygon( new SolidBrush( Color.Green ), frontier );
						if ( game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory > 0 && game.grid[ x + sliHor, y + sliVer].territory > 0)
						{
							if ( options.frontierType == 0 )
							{
								if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.war )
									g.FillPolygon( new SolidBrush( Color.Red ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.peace )
									g.FillPolygon( new SolidBrush( Color.White ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.ceaseFire )
									g.FillPolygon( new SolidBrush( Color.Gray ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance )
									g.FillPolygon( new SolidBrush( Color.Gold ), frontier );
							}
							else
							{
								Point middleLeft = new Point(
									r.X + 10 + caseWidth / 2 - 2, 
									r.Y + 20 + caseHeight + 1
									);
								Point middleRight = new Point( 
									r.X + 10 + caseWidth + 2, 
									r.Y + 20 + caseHeight/2 - 2
									);

								Point[] frontierTop = {
														  topLeft,
														  middleLeft,
														  middleRight,
														  topRight
													  };

								Point[] frontierDown = {
														   middleLeft,
														   downLeft,
														   downRight,
														   middleRight
													   };

								g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].civType ].color ), frontierTop );
								g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].civType ].color ), frontierDown );
							}
						}
						else if ( game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory == 0 )
						{
							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].civType ].color ), frontier );
						}
						else if ( game.grid[ x + sliHor, y + sliVer].territory == 0 )
						{
							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].civType ].color ), frontier );
						}
					}
					else if ( 
						game.playerList[ Form1.game.curPlayerInd ].discovered[ dCases[ 1 ].X, dCases[ 1 ].Y ] ||
						game.playerList[ Form1.game.curPlayerInd ].discovered[ x + sliHor, y + sliVer ]
						)
					{ // right
						Point topRight = new Point( 
							r.X + 10 + caseWidth,
							r.Y + 20 + caseHeight/2 - 3
							);
						Point downRight = new Point(
							r.X + 10 + caseWidth + 4,
							r.Y + 20 + caseHeight/2
							);
						Point topLeft = new Point(
							r.X + 10 + caseWidth / 2 - 4,
							r.Y + 20 + caseHeight
							);
						Point downLeft = new Point( 
							r.X + 10 + caseWidth / 2,
							r.Y + 20 + caseHeight  + 3
							);
			   
						Point[] frontier = {
											   topLeft,
											   downLeft,
											   downRight,
											   topRight
										   };

						if ( game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory > 0 && game.grid[ x + sliHor, y + sliVer].territory > 0)
						{
							if ( options.frontierType == 0 )
							{
								if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.war )
									g.FillPolygon( new SolidBrush( unseenColor( Color.Red ) ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.peace )
									g.FillPolygon( new SolidBrush( unseenColor( Color.White ) ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.ceaseFire )
									g.FillPolygon( new SolidBrush( unseenColor( Color.Gray ) ), frontier );
								else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance )
									g.FillPolygon( new SolidBrush( unseenColor( Color.Gold ) ), frontier );
							}
							else
							{
								Point middleLeft = new Point(
									r.X + 10 + caseWidth / 2 - 2, 
									r.Y + 20 + caseHeight + 1
									);
								Point middleRight = new Point( 
									r.X + 10 + caseWidth + 2, 
									r.Y + 20 + caseHeight/2 - 2
									);

								Point[] frontierTop = {
														  topLeft,
														  middleLeft,
														  middleRight,
														  topRight
													  };

								Point[] frontierDown = {
														   middleLeft,
														   downLeft,
														   downRight,
														   middleRight
													   };

								g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].civType ].color ) ), frontierTop );
								g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].civType ].color ) ), frontierDown );
							}
						}
						else if ( game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory == 0 )
						{
							g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].civType ].color ) ), frontier );
						}
						else if ( game.grid[ x + sliHor, y + sliVer].territory == 0 )
						{
							g.FillPolygon( new SolidBrush( unseenColor( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].civType ].color ) ), frontier );
						}
					}
				}
				#endregion
			}
			else
			{
				#region limit right
				if ( game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory != game.grid[ x + sliHor, y + sliVer].territory)
				{
					Point topLeft = new Point( 
						r.X + 10,
						r.Y + 20 + caseHeight/2 - 3
						);
					Point downLeft = new Point(
						r.X + 10 - 4,
						r.Y + 20 + caseHeight/2
						);
					Point topRight = new Point(
						r.X + 10 + caseWidth / 2 + 4,
						r.Y + 20 + caseHeight
						);
					Point downRight = new Point( 
						r.X + 10 + caseWidth / 2,
						r.Y + 20 + caseHeight + 3
						);

					Point[] frontier = {
										   topLeft,
										   downLeft,
										   downRight,
										   topRight
									   };

					if ( game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory > 0 && game.grid[ x + sliHor, y + sliVer].territory > 0)
					{
						if ( options.frontierType == 0 )
						{
							if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.war )
								g.FillPolygon( new SolidBrush( Color.Red ), frontier );
							else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.peace )
								g.FillPolygon( new SolidBrush( Color.White ), frontier );
							else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.ceaseFire )
								g.FillPolygon( new SolidBrush( Color.Gray ), frontier );
							else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance )
								g.FillPolygon( new SolidBrush( Color.Gold ), frontier );
						}
						else
						{
							Point middleLeft = new Point(
								r.X + 10 - 2,
								r.Y + 20 + caseHeight/2 - 1
								);
							Point middleRight = new Point( 
								r.X + 10 + caseWidth / 2 + 2,
								r.Y + 20 + caseHeight + 2
								);

							Point[] frontierTop = {
													  topLeft,
													  middleLeft,
													  middleRight,
													  topRight
												  };

							Point[] frontierDown = {
													   middleLeft,
													   downLeft,
													   downRight,
													   middleRight
												   };

							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].civType ].color ), frontierTop );
							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].civType ].color ), frontierDown );
						}
					
					}
					else if ( game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory == 0 )
					{
						g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].civType ].color ), frontier );
					}
					else if ( game.grid[ x + sliHor, y + sliVer].territory == 0 )
					{
						g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 0 ].X, dCases[ 0 ].Y ].territory - 1 ].civType ].color ), frontier );
					}
				}
				#endregion

				#region limit left
				if ( game.grid[ dCases[ 1 ].X, dCases[ 0 ].Y ].territory != game.grid[ x + sliHor, y + sliVer].territory)
				{ // right
					Point topRight = new Point( 
						r.X + 10 + caseWidth,
						r.Y + 20 + caseHeight/2 - 3
						);
					Point downRight = new Point(
						r.X + 10 + caseWidth + 4,
						r.Y + 20 + caseHeight/2
						);
					Point topLeft = new Point(
						r.X + 10 + caseWidth / 2 - 4,
						r.Y + 20 + caseHeight
						);
					Point downLeft = new Point( 
						r.X + 10 + caseWidth / 2,
						r.Y + 20 + caseHeight  + 3
						);
		   
					Point[] frontier = {
										   topLeft,
										   downLeft,
										   downRight,
										   topRight
									   };

					//g.FillPolygon( new SolidBrush( Color.Green ), frontier );
					if ( game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory > 0 && game.grid[ x + sliHor, y + sliVer].territory > 0)
					{
						if ( options.frontierType == 0 )
						{
							if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.war )
								g.FillPolygon( new SolidBrush( Color.Red ), frontier );
							else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.peace )
								g.FillPolygon( new SolidBrush( Color.White ), frontier );
							else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.ceaseFire )
								g.FillPolygon( new SolidBrush( Color.Gray ), frontier );
							else if ( game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].foreignRelation[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance )
								g.FillPolygon( new SolidBrush( Color.Gold ), frontier );
						}
						else
						{
							Point middleLeft = new Point(
								r.X + 10 + caseWidth / 2 - 2, 
								r.Y + 20 + caseHeight + 1
								);
							Point middleRight = new Point( 
								r.X + 10 + caseWidth + 2, 
								r.Y + 20 + caseHeight/2 - 2
								);

							Point[] frontierTop = {
													  topLeft,
													  middleLeft,
													  middleRight,
													  topRight
												  };

							Point[] frontierDown = {
													   middleLeft,
													   downLeft,
													   downRight,
													   middleRight
												   };

							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer ].territory - 1 ].civType ].color ), frontierTop );
							g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].civType ].color ), frontierDown );
						}
					}
					else if ( game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory == 0 )
					{
						g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ x + sliHor, y + sliVer].territory - 1 ].civType ].color ), frontier );
					}
					else if ( game.grid[ x + sliHor, y + sliVer].territory == 0 )
					{
						g.FillPolygon( new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ dCases[ 1 ].X, dCases[ 1 ].Y ].territory - 1 ].civType ].color ), frontier );
					}
				}
				#endregion
			}
		}
	#endregion

		private void drawSelectState( Graphics g, string text )
		{
			Font f = new Font( "Tahoma", 11, FontStyle.Bold );
			SizeF sText = g.MeasureString( text, f );

			g.DrawString( text, f,new SolidBrush( Color.Black ), this.Width / 2 - sText.Width / 2 + 1, 40 + 1 );
			g.DrawString( text, f,new SolidBrush( Color.White ), this.Width / 2 - sText.Width / 2, 40 );
		}

#endregion

#region Draw miniMap 

		public void drawMiniMap() 
		{ 
			int mmw = pictureBox2.Width - 2; 
			int mmh = pictureBox2.Height - 2; 

			miniMap = new Bitmap( pictureBox2.Width , pictureBox2.Height ); 
			Graphics gm = Graphics.FromImage( miniMap );
			gm.Clear( Color.Black );
			gm.Dispose();
			if ( options.miniMapType == 0 )
			{ // terrain type

				for ( int y = 0; y < mmh; y ++ )
					for ( int x = 0; x < mmw; x ++ )
						if ( !options.hideUndiscovered || game.playerList[ Form1.game.curPlayerInd ].discovered[ x * game.width / mmw, y * game.height / mmh ] )
							miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ game.grid[ x * game.width / mmw, y * game.height / mmh ].type ].color );

				for ( int p = 0; p < game.playerList.Length; p ++ )
					for ( int c = 1; c <= game.playerList[ p ].cityNumber; c ++ )
						if ( !options.hideUndiscovered || game.playerList[ Form1.game.curPlayerInd ].discovered[ game.playerList[ p ].cityList[ c ].X, game.playerList[ p ].cityList[ c ].Y ] )
						{
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1, 
								Statistics.civilizations[ game.playerList[ p ].civType ].color
								);
					
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 - 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 - 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 - 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 - 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 - 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 - 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 + 1, 
								Color.Black 
								); 
						}
			}
			else
			{ // borders
			/*	miniMap = new Bitmap( pictureBox2.Width , pictureBox2.Height ); 
				Graphics gm = Graphics.FromImage( miniMap );
				gm.Clear( Color.Black );
				gm.Dispose();*/

				for ( int y = 0; y < mmh; y ++ )
					for ( int x = 0; x < mmw; x ++ )
						if ( 
							!options.hideUndiscovered || 
							game.playerList[ Form1.game.curPlayerInd ].discovered[ x * game.width / mmw, y * game.height / mmh ] 
							)
						{
							if ( 
								Statistics.terrains[ game.grid[ x * game.width / mmw, y * game.height / mmh ].type ].ew == 0 || 
								game.grid[ x * game.width / mmw, y * game.height / mmh ].territory == 0 
								) // si il y a de l eau
								miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ game.grid[ x * game.width / mmw, y * game.height / mmh ].type ].color );
							else
								miniMap.SetPixel( x + 1, y + 1, Statistics.civilizations[ game.playerList[ game.grid[ x * game.width / mmw, y * game.height / mmh ].territory - 1 ].civType ].color );
						}

				for ( int p = 0; p < game.playerList.Length; p ++ )
					for ( int c = 1; c <= game.playerList[ p ].cityNumber; c ++ )
						if ( !options.hideUndiscovered || game.playerList[ Form1.game.curPlayerInd ].discovered[ game.playerList[ p ].cityList[ c ].X, game.playerList[ p ].cityList[ c ].Y ] )
						{
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1, 
								Color.White 
								);
					
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 - 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 - 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 - 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 - 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 - 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1 - 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 + 1, 
								Color.Black 
								); 
							miniMap.SetPixel( 
								game.playerList[ p ].cityList[ c ].X * mmw / game.width + 1, 
								game.playerList[ p ].cityList[ c ].Y * mmh / game.height + 1 + 1, 
								Color.Black 
								); 
						}
			}

			miniMapAdjX = (double)game.width / (double)pictureBox2.Width; 
			miniMapAdjY = (double)game.height / (double)pictureBox2.Height; 
		}

		private void affMiniMap()
		{ // cmdChangeMiniMap

			miniMapAff = new Bitmap( miniMap ); 

			Graphics gMiniMap = Graphics.FromImage( miniMapAff ); 

			Rectangle dispRect = new Rectangle( 
				(int)Math.Floor( sliHor / miniMapAdjX ), 
				(int)Math.Floor( sliVer / miniMapAdjY ), 
				(int)Math.Floor( 4 / miniMapAdjX ), 
				(int)Math.Floor( 18 / miniMapAdjY ) 
				); 
			Rectangle dispRect1 = new Rectangle( 
				(int)Math.Floor(( sliHor + game.width) / miniMapAdjX ), 
				(int)Math.Floor( sliVer / miniMapAdjY ), 
				(int)Math.Floor( 4 / miniMapAdjX ), 
				(int)Math.Floor( 18 / miniMapAdjY ) 
				); 
			Rectangle dispRect2 = new Rectangle( 
				(int)Math.Floor( (sliHor - game.width) / miniMapAdjX ), 
				(int)Math.Floor( sliVer / miniMapAdjY ), 
				(int)Math.Floor( 4 / miniMapAdjX ), 
				(int)Math.Floor( 18 / miniMapAdjY ) 
				); 

			gMiniMap.DrawRectangle( new Pen( Color.White ), dispRect ); 
			gMiniMap.DrawRectangle( new Pen( Color.White ), dispRect1 ); 
			gMiniMap.DrawRectangle( new Pen( Color.White ), dispRect2 ); 
			gMiniMap.DrawRectangle( new Pen( Color.DarkGray ), new Rectangle( 0, 0, pictureBox2.Width - 1, pictureBox2.Height - 1 ) ); 
			pictureBox2.Image = miniMapAff; 
		}

	/*	private void cmdChangeMiniMap_Click(object sender, System.EventArgs e)
		{
			if ( options.miniMapType == 0 )
				options.miniMapType = 1;
			else
				options.miniMapType = 0;

			affMiniMap();
		}*/
		#endregion

#region new game
		
		public bool newGame(int width, int height, byte percWater, byte nbrPlayer1, string playerName, byte civ, byte difficulty1, byte contSize, byte age, bool isInCenter, bool niceSite, bool tut )
		{ 
			return newGame( width, height, percWater, nbrPlayer1, playerName, civ, difficulty1, contSize, age, isInCenter, niceSite, tut, false, "", "", 0, Color.Empty, null );

		}
		public bool newGame( int width, int height, byte percWater, byte nbrPlayer1, string playerName, byte civ, byte difficulty1, byte contSize, byte age, bool isInCenter, bool niceSite, bool tut, bool customNation, string customNationName, string customNationDesc, byte customNationCityList, Color customNationColor, byte[] aiCivs )
		{
			showProgress.showProgressInd = true;
			showProgress.lblInfo = "Loading statistics and bitmaps...";
			showProgress.progressBar = 1;

			wC.show = true; 

			if ( customNation )
			{
				Stat.Civilization[] buffer = Statistics.civilizations;
				Statistics.civilizations = new Stat.Civilization[ buffer.Length + 1 ];

				for ( int i = 0; i < buffer.Length; i ++ )
					Statistics.civilizations[ i ] = buffer[ i ];

				
				Statistics.civilizations[ buffer.Length ] = new Stat.Civilization( (byte)buffer.Length );
				
				Statistics.civilizations[ buffer.Length ].name = customNationName;
				Statistics.civilizations[ buffer.Length ].description = customNationDesc;
				Statistics.civilizations[ buffer.Length ].cityNames = Statistics.civilizations[ customNationCityList ].cityNames;
				Statistics.civilizations[ buffer.Length ].color = customNationColor;

				civ = (byte)buffer.Length;
			}

			/*if ( aiCivs == null )
			{
			}*/
			game = new Game( width, height );

			Tutorial.init( tut );
			//	tutorialMode = tutorial; 
			game.difficulty = difficulty1; 
			game.savePath = ""; 

			sliHor = 0;
			sliVer = 0;
			game.curTurn = 0;

			options.setDefaults();

			if ( !Statistics.variableInitialized )
			{
				showProgress.lblInfo = "Loading graphics...";
				InitGraphics();

				Statistics.initAllBut();
			}

			showProgress.lblInfo = "Generating map...";
			
			game.caseImps = new structures.caseImprovement[ 0 ];
			
			showProgress.progressBar = 10;

			mapGenerator mapGen = new mapGenerator( game );
			if ( mapGen.generateMap( width, height, percWater, contSize, age ) )
			{
				showProgress.progressBar = 50;
				showProgress.lblInfo = "Locating continents...";

				mapGen.findContinents( width, height );
				int[] contSize1 = mapGen.getContSize();

				showProgress.progressBar = 57;
				showProgress.lblInfo = "Set specials...";
				mapGen.setSpecials();

				showProgress.progressBar = 59;
				showProgress.lblInfo = "Set resources...";
				mapGen.setResources( width * 7 * ( 100 - percWater ) / ( 40 * 100 ) );

				showProgress.progressBar = 62;
				showProgress.lblInfo = "Set rivers...";
				mapGen.setRivers( 10 * ( width / 40 ) ^ 2 * ( 100 - percWater ) / 100 );

				showProgress.progressBar = 65;
				showProgress.lblInfo = "Finished generating map"; // Map generated"; // "";

				for (int x = 0; x < game.width; x ++ )
					for (int y = 0; y < game.height; y ++ )
						game.grid[ x, y ].stack = new UnitList[ 0 ];
			
				Random R = new Random();
				game.radius.setIsNextToWater(); 

				#region generer les case cotiere possible
				Point[] possibleCases = new Point[ width * 8 * height / 10 ];
				int d = 0;

				for ( int x = 0; x < width; x ++)
					for ( int y = height / 7; y < 6 * height / 7; y ++ )
						if ( 
							game.grid[ x, y ].type == (byte)enums.terrainType.plain && 
				//			game.radius.isNextToWater( x, y ) &&
							game.grid[ x, y ].resources == 0
							)
						{ 
							Case[] cases = game.radius.returnSquareCases( x, y, 1 );
							bool nextToOcean = false;

				//			for ( int c = 0; c < cases.Length; c ++ )
							foreach ( Case c in cases )
								if ( c.water )
								{
									nextToOcean = true;
									break;
								}

							if ( nextToOcean )
							{
								possibleCases[ d ] = new Point( x, y ); 
								d ++; 
							}
						} 

				/*
						if ( 
							game.grid[ x, y ].type == (byte)enums.terrainType.plain && 
							game.radius.isNextToWater( x, y ) &&
							game.grid[ x, y ].resources == 0
							)
						{ 
							possibleCases[ d ] = new Point( x, y ); 
							d ++; 
						} 
				*/

				Point[] possibleCases1 = new Point[ d ];

				for ( int i = 0; i < possibleCases1.Length; i ++ )
					possibleCases1[ i ] = possibleCases[ i ];

				#endregion
			
				showProgress.progressBar = 70;
				//	showProgress.lblInfo = 

				game.playerList = new PlayerList[ nbrPlayer1 ];

				game.playerList[ game.curPlayerInd ] = new PlayerList( game, 0, civ,	playerName,		1,		1,		1,		50,		1,		50);

				#region human player

				bool foundStart = false;
				for ( int j = 0; !foundStart; j ++ )
				{
					Point startPos = possibleCases1[ R.Next( possibleCases1.Length ) ];

					if ( 
						!isInCenter || 
						( startPos.X > game.width / 5 && startPos.X < game.width * 4 / 5 )
						)
					{
						if ( niceSite )
						{
							if ( contSize1[ game.grid[ startPos.X, startPos.Y ].continent ] > 100 )
							{
								Point[] sqr = game.radius.returnSquare( startPos.X, startPos.Y, 4 );
								int tot = 0;

								for ( int k = 0; k < sqr.Length; k ++ )
									if ( 
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == game.grid[ startPos.X, startPos.Y ].continent && 
										(
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.glacier ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.jungle ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.mountain ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.swamp ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.tundra ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.desert
										)
										)
									{
										if ( k > 8 )
											tot ++; 
										else
										{
											tot += 2000;
											break;
										}

									}

								if ( tot < 5 + j * 2 )
								{
								//	createUnit( startPos.X, startPos.Y, 0, (byte)unitType.colon );
									game.curPlayer.createUnit( startPos.X, startPos.Y, (byte)unitType.colon );
									foundStart = true;
								}
							}
						}
						else
						{
							if ( contSize1[ game.grid[ startPos.X, startPos.Y ].continent ] > 30 )
							{
								game.curPlayer.createUnit( startPos.X, startPos.Y, (byte)unitType.colon );
						//		createUnit( startPos.X, startPos.Y, 0, (byte)unitType.colon );
								foundStart = true;
							}
						}
					}

					if ( j > possibleCases1.Length * 2 ) // 3
						return false;
					//	MessageBox.Show( "Failed finding" );
				}
				#endregion

				showProgress.progressBar = 70 + 5;
				showProgress.lblInfo = "Player info...";

				for ( byte i = 1; i < Form1.game.playerList.Length; i ++ )
				{
					#region ai players

					int civType = 0;
					if ( aiCivs == null || aiCivs.Length == 0 ) // )
					{
						bool found = false;
						while ( !found )
						{
							civType = (byte)R.Next( Statistics.normalCivilizationNumber );
							found = true;
							for ( int k = 0; k < i; k++ )
							{
								if ( civType == game.playerList[ k ].civType )
								{
									found = false;
									break;
								}
							}
						}
					}
					else
					{
						civType = aiCivs[ i - 1 ];
					}

					Random r = new Random();
			//		enterNewPlayer( i, (byte)civType, Statistics.civilizations[ civType ].leaderNames[ r.Next( Statistics.civilizations[ civType ].leaderNames.Length ) ], 1, 1, 1, 50, 0, 50 );
					game.playerList[ i ] = new PlayerList( game,	i, (byte)civType,	Statistics.civilizations[ civType ].leaderNames[ r.Next( Statistics.civilizations[ civType ].leaderNames.Length ) ],		1,		1,		1,		50,		0,		50);

					foundStart = false;
					for ( int j = 0; !foundStart; j ++ )
					{
						if ( j > 1000 )
						{
							int aiOld = Form1.game.playerList.Length - 1;
							int aiNew = i - 1;
				//			Form1.game.playerList.Length = (byte)( i );

							PlayerList[] buffer = game.playerList;
							game.playerList = new PlayerList[ aiNew ];

							for ( int k = 0; k < game.playerList.Length; k ++ )
								game.playerList[ k ] = buffer[ k ];

							MessageBox.Show(
								"Due to the specification of the terrain the number of AIs has been reduced from " + aiOld.ToString() + " to " + aiNew.ToString() + ".", 
								"Map generation exception"
								); 
 
							break; 
						}

						Point startPos = possibleCases1[ R.Next( possibleCases1.Length ) ];

						if ( game.grid[ startPos.X, startPos.Y ].stack.Length == 0 )
						{
							if ( contSize1[ game.grid[ startPos.X, startPos.Y ].continent ] > 30 )
							{
								Point[] sqr = game.radius.returnSquare( startPos.X, startPos.Y, 7 );
								bool foundUnit = false;

								for ( int k = 0; k < sqr.Length; k ++ )
									if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length > 0 )
									{
										foundUnit = true;
										break;
									}

								if ( !foundUnit )
								{
									game.playerList[ i ].createUnit( startPos.X, startPos.Y, (byte)unitType.colon );
						//			createUnit( startPos.X, startPos.Y, i, (byte)unitType.colon );
									foundStart = true;
								}
							}
						}
					
					}
					#endregion
				
					showProgress.progressBar = 70 + 5 + i * ( 20 / Form1.game.playerList.Length );
					showProgress.lblInfo = "AI info generated";
				}

				label.initList( 0 );

				game.rvtbc = new Rvtbc( game );//.init(); 
				game.wonderList = new WonderList( Statistics.wonders.Length );
				game.plans.init();
				Tutorial.init( tut );

				drawMiniMap();
				showProgress.progressBar = 95; 
				showProgress.lblInfo = "Finalizing"; 

				//   platformSpec.keys.Set( this );//keyIn = new keyInPut(); 
				// timer1.Enabled = true; 
				mICiv.Enabled = true; 

				pictureBox1.Visible = true; 
				pictureBox1.Enabled = true; 

				unitOrder.setOrder( Form1.game.curPlayerInd );
				showUnit( Form1.game.curPlayerInd, 1 ); 

				wC.show = false;
				guiEnabled = true; 

				showProgress.progressBar = 100; 
				showProgress.lblInfo = "Completed"; 
				sight.setAllSight( Form1.game.curPlayerInd ); 

				showProgress.showProgressInd = false; 

				Tutorial.show( this, Tutorial.order.justStarted );
				return true; 
			}
			else
			{
				showProgress.showProgressInd = false;
				wC.show = false;
				MessageBox.Show( "Generation failed, try more or less water." );
				return false;
			}
		}

		/*
		public bool newGame(int width, int height, byte percWater, byte nbrPlayer1, string playerName, byte civ, byte difficulty1, byte contSize, byte age, bool isInCenter, bool niceSite, bool tut )
		{ 
			return newGame( width, height, percWater, nbrPlayer1, playerName, civ, difficulty1, contSize, age, isInCenter, niceSite, tut, false, "", "", 0, Color.Empty, null );
		}
		public bool newGame( int width, int height, byte percWater, byte nbrPlayer1, string playerName, byte civ, byte difficulty1, byte contSize, byte age, bool isInCenter, bool niceSite, bool tut, bool customNation, string customNationName, string customNationDesc, byte customNationCityList, Color customNationColor, byte[] aiCivs )
		{
			if ( !Statistics.variableInitialized )
			{
				showProgress.lblInfo = "Loading graphics...";
				InitGraphics();

				Statistics.initAllBut();
			}

			Game tempGame = Game.newGame( width, height, percWater, nbrPlayer1, playerName, civ, difficulty1, contSize, age, isInCenter, niceSite, tut, customNation, customNationName, customNationDesc, customNationCityList, customNationColor, aiCivs );

			if ( tempGame != null )
			{
				game = tempGame;
				
				showOnScreenDPad = options.showOnScreenDPad;
				
				sliHor = 0;
				sliVer = 0;

				drawMiniMap();

				mICiv.Enabled = true; 

				pictureBox1.Visible = true; 
				pictureBox1.Enabled = true; 

				unitOrder.setOrder( Form1.game.curPlayerInd ); // game ); //curPlayer );

				guiEnabled = true; 

				wC.show = false;

				showProgress.progressBar = 100; 
				showProgress.lblInfo = "Completed"; 
				sight.setAllSight( Form1.game.curPlayerInd ); 

				showUnit( Form1.game.curPlayerInd, 1 ); 

				showProgress.showProgressInd = false; 

				Tutorial.show( this, Tutorial.order.justStarted );


				return true;
			}
			else
			{
				return false;
			}
		}*/
		#endregion

#region enterNewPlayer
	/*	public void enterNewPlayer( byte playerNbr, byte civType, string playerName, sbyte prefFood, sbyte prefProd, sbyte prefTrade, sbyte prefScience, sbyte prefHapiness, sbyte prefWealth )
		{
			game.playerList[ playerNbr ] = new PlayerList( game, playerNbr ); 
			game.playerList[ playerNbr ].lastSeen = new structures.lastSeen[ game.width, game.height ];

			game.playerList[ playerNbr ].civType = civType;
			game.playerList[ playerNbr ].playerName = playerName;
			game.playerList[ playerNbr ].money = 0;
			game.playerList[ playerNbr ].currentResearch = 0;

			game.playerList[ playerNbr ].unitList = new UnitList[ 10 ];
			game.playerList[ playerNbr ].cityList = new CityList[ 5 ];
			game.playerList[ playerNbr ].foreignRelation = new structures.sForeignRelation[ Form1.game.playerList.Length ];

			for ( int i = 0; i < Form1.game.playerList.Length; i ++ )
			{
				game.playerList[ playerNbr ].foreignRelation[ i ].quality = 100;
				game.playerList[ playerNbr ].foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
				
				for ( int j = 0; j < 4; j ++ )
				{
					game.playerList[ playerNbr ].foreignRelation[ i ].spies[ j ] = new xycv_ppc.structures.sSpies();
					game.playerList[ playerNbr ].foreignRelation[ i ].spies[ j ].nbr = 0;
				}
			}
	
			game.playerList[ playerNbr ].technos = new xycv_ppc.structures.technoList[ Statistics.technologies.Length ];

			game.playerList[ playerNbr ].unitNumber = 0;
			game.playerList[ playerNbr ].cityNumber = 0;
			game.playerList[ playerNbr ].technos[ 0 ].pntDiscovered = 0;
			
			game.playerList[ playerNbr ].preferences.laborFood = prefFood;
			game.playerList[ playerNbr ].preferences.laborProd = prefProd;
			game.playerList[ playerNbr ].preferences.laborTrade = prefTrade;
			game.playerList[ playerNbr ].preferences.science = prefScience;
			game.playerList[ playerNbr ].preferences.reserve = prefWealth;
			game.playerList[ playerNbr ].discovered = new bool[ game.width, game.height ];
			game.playerList[ playerNbr ].see = new bool[ game.width, game.height ];
			//	game.playerList[ playerNbr ].unitList = new UnitList[ 50 ];
			//	game.playerList[ playerNbr ].foreignRelation = new structures.sForeignRelation[ Form1.game.playerList.Length ];
			
			game.playerList[ playerNbr ].setResourcesAccess();
			game.playerList[ playerNbr ].memory = new Memory( playerNbr, 0 );
		}*/

#endregion

#region Load map
		public bool LoadMap()
		{
	/*		sliHor = 0;
			sliVer = 0;
			game.difficulty = 1;
			string mapFile;
			OpenFileDialog ofd = new OpenFileDialog ();
			ofd.Filter = "xymus' map files (*.map)|*.map|All files (*.*)|*.*";
			if (ofd.ShowDialog () == DialogResult.OK) 
			{
				mapFile = ofd.FileName;
				Stream s = new FileStream( mapFile, FileMode.Open );
				game.width = s.ReadByte();
				game.height = s.ReadByte();
				
				game.grid = new structures.sGrid[game.width, game.height];
				byte read;

				if (s.CanRead)
					for (int x = 0; x < game.width; x ++ )
						for (int y = 0; y < game.height; y ++ )
						{
							game.grid[ x, y ].stack = new UnitList[ 0 ];
							read = Convert.ToByte( s.ReadByte() );
							game.grid[ x, y ].type = read;
						}

				s.Flush();
				s.Close();
				//Dispose(read);

				//	// waitCursor wC = new // waitCursor();
				wC.show = true;

				InitGraphics();
			/*	initUnitStats();
				initTerrainStats();
				initTechnoStats();
				initBuildingStats();*
				Statistics.initAllBut();
			//	initStatistics.civilizations();
				drawMiniMap();
				pictureBox1.Enabled = true;
				
				game = new Game();

				game.playerList = new PlayerList[ Form1.game.playerList.Length ];

				//				nbr civ name				food	prod	trade	Sci		happy	wealth
				enterNewPlayer(	0,	0,	"Xymus",			1,		1,		1,		5,		1,		4);
				enterNewPlayer(	1,	1,	"Bob Moon",			1,		1,		1,		5,		1,		4);
				enterNewPlayer(	2,	0,	"George W Bush",	1,		1,		1,		5,		1,		4);

				createUnit( 5, 14, Form1.game.curPlayerInd, (byte)Form1.unitType.colon );
				createUnit( 10, 20, 1, (byte)Form1.unitType.colon );

				//   platformSpec.keys.Set( this );//keyIn = new keyInPut();
				// timer1.Enabled = true;
				mICiv.Enabled = true;

			/*	waitingTmr = new System.Windows.Forms.Timer();
				waitingTmr.Interval = 1;
				waitingTmr.Enabled = false;
				waitingTmr.Tick += new System.EventHandler( waitingTmr_tick );*

				showUnit( Form1.game.curPlayerInd, 1 );

				wC.show = false;
				DrawMap();
				showInfo();

				cmdOk.Enabled = true;
				pictureBox1.Visible = true;
				return true;
			}
			else*/
				return false;
		}
#endregion

#region save game

	#region save As - menu item
		private void mISaveAs_Click(object sender, System.EventArgs e)
		{ // save as
			game.saveAsGetPath();
		/*	
			guiEnabled = false; 
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Pocket Humanity saves .phs|*.phs|All files (*.*)|*.*"; //|Pocket Humanity game file (*.phf)|*.phf

			if( sfd.ShowDialog() == DialogResult.OK )
			{
				wC.show = true;
				game.savePath = sfd.FileName;
				game.save();
				options.save();
				wC.show = false;
			}
			guiEnabled = true;
		*/
/*
			string path = FrmSaveGame.getNow( @"\My Documents\", false, game.savePath );

			if ( path == "" ) 
			{ 
				path = userTextInput.getNow(
					language.getAString( language.order.filesNewFileDialogTitle ),
					language.getAString( language.order.filesNewFileDialogText ),
					game.possibleSaveName,
					language.getAString( language.order.ok ),
					language.getAString( language.order.cancel )
					);

				if ( path.Length > 0 && path != null )
					path = @"\My Documents\" + path + ".phs";
			} 

			if ( path != null && path.Length > 0 )
			{
				wC.show = true;

				game.savePath = path; // sfd.FileName;
				game.save();
				options.save();

				wC.show = false;
			}*/
		}	
		#endregion

	#region save - menu item
		private void mISave_Click(object sender, System.EventArgs e)
		{ // save
			guiEnabled = false; //( false );
		/*	if ( game.savePath == "" )
			{*/
				game.save();
			//	mISaveAs_Click( null, System.EventArgs.Empty );
			/*	SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Pocket Humanity saves .phs|*.phs|All files (*.*)|*.*"  ;

				if( sfd.ShowDialog() == DialogResult.OK )
				{
					wC.show = true;
					game.savePath = sfd.FileName;
					game.save();
					options.save();
					wC.show = false;
				}*/
		/*	}
			else
			{
				wC.show = true;
				game.save();
				options.save();
				wC.show = false;
			}*/
			guiEnabled = true;
		}
		#endregion

#endregion

#region load game

	#region load game from
		public bool loadGameFrom()
		{
		/*	OpenFileDialog ofd = new OpenFileDialog ();
			ofd.Filter = "Pocket Humanity saves (*.phs)|*.phs|All files (*.*)|*.*"; //|Pocket Humanity game file (*.phf)|*.phf

			if ( ofd.ShowDialog () == DialogResult.OK ) 
			{
				guiEnabled = false;
				wC.show = true;
				this.Update();

				if ( loadGame( ofd.FileName ) )
				{
					guiEnabled = true;
					wC.show = false;

					return true;
				}
				else return false;

			}
			return false;*/

			string fileName = FrmLoadGame.getNow(); // options.savesDirectoryFullPath );

			wC.show = true;
			if ( 
				fileName != null && 
				fileName.Length != 0 && 
				loadGame( fileName )
				)
			{
				guiEnabled = true;
				wC.show = false;

				return true;
			}
			
			wC.show = false;
			return false;

		}
		#endregion

	#region load game
		public bool loadGame( string sp )
		{
			if ( !Statistics.variableInitialized )
			{
				InitGraphics();

				Statistics.initAllBut();
			}

			Game tempGame = Game.load( sp );

			if ( tempGame != null )
			{
				game = tempGame;

				selected.state = 0;
				selected.unit = 0;
				pictureBox3.Visible = false;
				pictureBox4.Visible = false;

				drawMiniMap(); 
				pictureBox1.Enabled = true; 
				pictureBox1.Visible = true; 
				mICiv.Enabled = true; 

				//   platformSpec.keys.Set( this );//keyIn = new keyInPut(); 
				// timer1.Enabled = true;
				unitOrder.setOrder( Form1.game.curPlayerInd );
			
				oldSliVer = -5; 

				if ( !showNextUnitNow( 0, 0 ) ) 
					DrawMap(); 

				guiEnabled = true; 
				wC.show = false; 

				return true;
			}
			else
			{
				wC.show = false; 
				return false;
			}
		}
		#endregion

	#region menu item 10
		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			loadGameFrom();
		}
		#endregion

#endregion

#region pictureBoxes	# # #	

		/*enum eic : byte
		{
			Localization,
			Adjusting,
			Normal,
			Bombard,
			Missile,
			Rebase,
			Recon,
			GoTo,
			Limites,
			Else
		}*/

	#region pictureBox1 click !Selection! main
		private void pictureBox1_Click(object sender, System.EventArgs e)
		{
			System.Drawing.Point bob2 = pictureBox1.PointToClient(new Point(Form1.MousePosition.X, Form1.MousePosition.Y));

			int xFloor = (int)Math.Floor( (double)bob2.X / caseWidth ),
				yFloor = (int)Math.Floor( (double)bob2.Y / caseHeight ),	// 2 fois y de game.grid
				selectedX, selectedY; 

			#region localisation du clik
			if ( 
				bob2.X < xFloor*caseWidth + caseWidth/2 && 
				bob2.Y < yFloor*caseHeight+caseHeight/2
				)
			{ //1er cadran
				if( (bob2.Y - yFloor*caseHeight) >= -caseHeight*Convert.ToSingle(bob2.X - xFloor*caseWidth)/caseWidth +15 ) 
				{ //centre
					selectedX = xFloor + sliHor;
					selectedY = 2*yFloor + sliVer;
				}
				else 
				{
					selectedX = xFloor-1 + sliHor;
					selectedY = 2*yFloor-1 + sliVer;
				}
			}
			else if(
				bob2.X > xFloor*caseWidth+ caseWidth/2 && 
				bob2.Y < yFloor*caseHeight+caseHeight/2
				) 
			{ // 2e cadran
				if( (bob2.Y - yFloor*caseHeight) >= caseHeight*Convert.ToSingle(bob2.X - xFloor*caseWidth)/caseWidth -15 ) 
				{ //centre
					selectedX = xFloor + sliHor;
					selectedY = 2*yFloor + sliVer;
				}
				else 
				{
					selectedX = xFloor+1-1 + sliHor;
					selectedY = 2*yFloor-1 + sliVer;
				}
			}
			else if(
				bob2.X < xFloor*caseWidth+ caseWidth/2 && 
				bob2.Y > yFloor*caseHeight+caseHeight/2
				) 
			{ // 3e cadran
				if( (bob2.Y - yFloor*caseHeight) >= caseHeight*Convert.ToSingle(bob2.X - xFloor*caseWidth)/caseWidth +15 ) 
				{
					selectedX = xFloor-1 + sliHor;
					selectedY = 2*yFloor+1 + sliVer;
				}
				else //centre
				{
					selectedX = xFloor + sliHor;
					selectedY = 2*yFloor + sliVer;
				}
			}
			else 
			{ //4e cadran
				if( (bob2.Y - yFloor*caseHeight) >= -caseHeight*Convert.ToSingle(bob2.X - xFloor*caseWidth)/caseWidth +45 ) 
				{
					selectedX = xFloor+1-1 + sliHor;
					selectedY = 2*yFloor+1 + sliVer;
				}
				else //centre
				{
					selectedX = xFloor + sliHor;
					selectedY = 2*yFloor + sliVer;
				}
			}
			#endregion

			#region ajustement aux limites

			if (selectedX < 0)
				selectedX += game.width;
			else if (selectedX >= game.width)
				selectedX -= game.width;

			if (selectedY >= game.height)
				selectedY = game.height - 1;
			else if (selectedY < 0)
				selectedY = 0;	

			#endregion

			selected.X = selectedX;
			selected.Y = selectedY;

			#region selectionState.normal
			if ( selectState == (byte)enums.selectionState.normal )
			{
			/*	try
				{*/
				// position originales de l'unité
				ai Ai = new ai();

				drawRect.xMin = -1; 
				drawRect.yMin = -1; 
				drawRect.xMax =  5; 
				drawRect.yMax = 18; 
				
				int openCity = -1;
				for (int y = drawRect.yMin; y < drawRect.yMax && openCity == -1; y ++)
					for (int x = drawRect.xMin; x < drawRect.xMax && openCity == -1; x ++)
						if ( y + sliVer >= 0 && y + sliVer < game.height )
							if ( x + sliHor < game.width && x + sliHor >= 0 )
							{
								if ( 
									game.grid[ x + sliHor, y + sliVer ].territory - 1 == Form1.game.curPlayerInd &&
									game.grid[ x + sliHor, y + sliVer ].city > 0 
									) 
									if ( isInRectPop( x, x, y, bob2 ) ) 
										openCity = game.grid[ x + sliHor, y + sliVer ].city; 
							} 
							else if( x + sliHor < 0 )
							{
								if ( 
									game.grid[ x + sliHor + game.width, y + sliVer ].territory - 1 == Form1.game.curPlayerInd &&
									game.grid[ x + sliHor + game.width, y + sliVer ].city > 0 
									) 
									if ( isInRectPop( x, x + game.width, y, bob2 ) ) 
										openCity = game.grid[ x + sliHor + game.width, y + sliVer ].city; 
							}
							else if( x + sliHor >= game.width )
							{
								if ( 
									game.grid[ x + sliHor - game.width, y + sliVer ].territory - 1 == Form1.game.curPlayerInd &&
									game.grid[ x + sliHor - game.width, y + sliVer ].city > 0 
									) 
									if ( isInRectPop( x, x - game.width, y, bob2 ) ) 
										openCity = game.grid[ x + sliHor - game.width, y + sliVer ].city; 
							}

				if ( openCity != -1 )				
				{
					openCityFrm( Form1.game.curPlayerInd, openCity );
				}
				#region	m  unit
				else if ( 
					selected.state == 1 &&
					selected.unit > 0 &&
					( 
					game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || 
					game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 
					) &&
					(
					game.radius.isNextTo( game.playerList[ selected.owner ].unitList[ selected.unit ].pos, new Point( selected.X, selected.Y ) ) // ||
					//game.playerList[ selected.owner ].unitList[ selected.unit ].pos == new Point( selected.X, selected.Y )
					)
					)	// si il y a une unite de selectionnée // si il lui reste des pts de mouvements
				{
					int xUnit = game.playerList[ selected.owner ].unitList[ selected.unit ].X;
					int yUnit = game.playerList[ selected.owner ].unitList[ selected.unit ].Y;

					bool declareWar = false;
					if ( 
						game.grid[ selected.X, selected.Y ].city > 0 && 
						game.grid[ selected.X, selected.Y ].territory - 1 != Form1.game.curPlayerInd &&
						(
							game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ selected.X, selected.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace ||
							game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ selected.X, selected.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
							game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ selected.X, selected.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
						)
						)
					{
					//	if ( Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew == Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain )
						if ( 
							MessageBox.Show( 
							"You are about to attack " + Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].territory - 1 ].civType ].name + " without any provocation.  Doing so will cause a war for which you will be held responsible by other nations.  Do you want to proceed?", 
							"City invasion", 
							MessageBoxButtons.YesNo, 
							MessageBoxIcon.None, 
							MessageBoxDefaultButton.Button1 
							)== DialogResult.Yes 
							)
						{
							declareWar = true;
							aiPolitics.declareWar( 
								Form1.game.curPlayerInd, 
								(byte)(game.grid[ selected.X, selected.Y ].territory - 1)
								);
						}
					}
					else if ( game.radius.caseOccupiedByRelationType( selected.X, selected.Y, Form1.game.curPlayerInd, false, true, true, false, false, false ) )
					{
						byte playerAttacked = 255; // a changer

						for ( int u = 0; u < Form1.game.grid[ selected.X, selected.Y ].stack.Length; u ++ )
							if ( 
								Form1.game.grid[ selected.X, selected.Y ].stack[ u ].player.player != Form1.game.curPlayerInd &&
								(
								Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ Form1.game.grid[ selected.X, selected.Y ].stack[ u ].player.player ].politic == (byte)Form1.relationPolType.peace ||
								Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ Form1.game.grid[ selected.X, selected.Y ].stack[ u ].player.player ].politic == (byte)Form1.relationPolType.ceaseFire
								)
								)
							{
								playerAttacked = Form1.game.grid[ selected.X, selected.Y ].stack[ u ].player.player;
								break;
							}

						if ( 
							MessageBox.Show( 
							"You are about to attack " + Statistics.civilizations[ game.playerList[ playerAttacked ].civType ].name + " without any provocation.  Doing so will cause a war for which you will be held responsible by other nations.  Do you want to proceed?", 
							"First fight", 
							MessageBoxButtons.YesNo, 
							MessageBoxIcon.None, 
							MessageBoxDefaultButton.Button1 
							)== DialogResult.Yes 
							) 
						{
							declareWar = true; 
							aiPolitics.declareWar( 
								Form1.game.curPlayerInd, 
								playerAttacked 
								); 
						}
					}
					else
						declareWar = true;

					if ( declareWar )
					{
						int ennemy = ai.strongestEnnemy( selected.X, selected.Y, Form1.game.curPlayerInd );
						if ( 
							ennemy != 0 && 
							(
								Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew == game.playerList[ selected.owner ].unitList[ selected.unit ].typeClass.terrain || 
								(
									game.playerList[ selected.owner ].unitList[ selected.unit ].typeClass.terrain == 0 &&
									(
										game.grid[ selected.X, selected.Y ].city > 0 ||
										game.grid[ selected.X, selected.Y ].river
									)
								)
							) 
							) // game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos * 2 - 2] ].relationPol[ Form1.game.curPlayerInd ] == (byte)relationPolType.war
						{ // attaque unit

							attackUnit( 
								selected.owner, 
								selected.unit, 
								game.grid[ selected.X, selected.Y ].stack[ ennemy - 1 ].player.player, 
								game.grid[ selected.X, selected.Y ].stack[ ennemy - 1 ].ind
								);

							unitAffSli = game.grid[ selected.X, selected.Y ].stack.Length - 3;

							if ( unitAffSli < 0 )
								unitAffSli = 0;
						}
						else if ( 
							Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew == Statistics.units[game.playerList[ selected.owner ].unitList[ selected.unit ].type ].terrain || 
							game.grid[ selected.X, selected.Y ].river ||
							(
							game.grid[ selected.X, selected.Y ].city > 0 &&
							(
							game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd ||
							game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ selected.X, selected.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ selected.X, selected.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected
							)
							)
							) // si la case destination est compatible (ew)
						{ // terre - terre // mer - mer // mer - ville
							/*	if ( radius.isInSquare( xUnit, yUnit, 1, selected.X, selected.Y ) )
								{*/
							UnitMove( selected.owner, selected.unit, xUnit, yUnit, selected.X, selected.Y );
							unitAffSli = game.grid[ selected.X, selected.Y ].stack.Length - 3;
							if ( unitAffSli < 0 )
								unitAffSli = 0;
							//}

							if ( 
								game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft == 0 && 
								game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction == 0 
								)
							{
								showNextUnit( selected.owner, 0 );
							}
							enableMenus();
							showInfo();
						}
						else if ( 
							Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew != Statistics.units[game.playerList[ selected.owner ].unitList[ selected.unit ].type ].terrain && 
							transport.spaceInATransport( Form1.game.curPlayerInd, selected.X, selected.Y ) )
						{ // terre -> transport
							int wstj = transport.askForWhichShipToJoin( Form1.game.curPlayerInd, selected.X, selected.Y );
							if ( wstj != -1 )
							{
								move.UnitMove2Transport( selected.owner, selected.unit, wstj );
								unitAffSli = game.grid[ selected.X, selected.Y ].stack.Length - 3;
								if ( unitAffSli < 0 )
									unitAffSli = 0;

								if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft == 0 && game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction == 0 )
								{
									showNextUnit( selected.owner, 0 );
								}
								enableMenus();
								showInfo();
							}
						}
						else if ( 
							Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew != Statistics.units[game.playerList[ selected.owner ].unitList[ selected.unit ].type ].terrain && 
							Statistics.units[game.playerList[ selected.owner ].unitList[ selected.unit ].type ].speciality != enums.speciality.carrier &&
							game.playerList[ selected.owner ].unitList[ selected.unit].transported > 0 &&
							transport.canDisembarkAUnit( Form1.game.curPlayerInd, selected.unit ) 
							)
						{ // transport -> terre	
							int result = transport.askForWhichUnitToDisembark( Form1.game.curPlayerInd, selected.unit );

							if ( result == 100 )//game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport.Length )
							{
								for ( int i = game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported - 1; i >= 0; i-- )
									if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] ].moveLeft > 0 || game.playerList[ Form1.game.curPlayerInd ].unitList[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] ].moveLeftFraction > 0 )
										move.disembarkUnit( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ], selected.unit, new Point( selected.X, selected.Y ) );

								DrawMap();
							}
							else if ( result != -1 )
							{
								move.disembarkUnit( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ result /*selected.unitInTransport*/ ], selected.unit, new Point( selected.X, selected.Y ) );
								unitAffSli = game.grid[ selected.X, selected.Y ].stack.Length - 3;

								if ( unitAffSli < 0 )
									unitAffSli = 0;

								if ( 
									game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft == 0 && 
									game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction == 0 
									)
									showNextUnit( selected.owner, 0 );

								enableMenus();
								showInfo();
							}
						}
						else
						{
							selected.state = 0;
						}
					}
				}
				else if ( 
					selected.state == 1 &&
					selected.unit > 0 &&
					game.radius.isNextTo( game.playerList[ selected.owner ].unitList[ selected.unit ].pos, new Point( selected.X, selected.Y ) ) &&
					Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew != Statistics.units[game.playerList[ selected.owner ].unitList[ selected.unit ].type ].terrain && 
					game.playerList[ selected.owner ].unitList[ selected.unit].transported > 0 &&
							Statistics.units[game.playerList[ selected.owner ].unitList[ selected.unit ].type ].speciality != enums.speciality.carrier &&
					transport.canDisembarkAUnit( Form1.game.curPlayerInd, selected.unit ) 
					)
				{ // transport -> terre	
					int result = transport.askForWhichUnitToDisembark( Form1.game.curPlayerInd, selected.unit );

					if ( result == 100 ) //game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport.Length )
					{
						for ( int i = game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported - 1; i >= 0; i-- )
							if ( 
								game.playerList[ Form1.game.curPlayerInd ].unitList[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] ].moveLeft > 0 || 
								game.playerList[ Form1.game.curPlayerInd ].unitList[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] ].moveLeftFraction > 0 
								)
								move.disembarkUnit( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ], selected.unit, new Point( selected.X, selected.Y ) );

						DrawMap();
					}
					else if ( result != -1 )
					{
						move.disembarkUnit( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ result/* selected.unitInTransport*/ ], selected.unit, new Point( selected.X, selected.Y ) );
						unitAffSli = game.grid[ selected.X, selected.Y ].stack.Length - 3;

						if ( unitAffSli < 0 )
							unitAffSli = 0;

						if ( 
							game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft == 0 && 
							game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction == 0 
							)
							showNextUnit( selected.owner, 0 );

						enableMenus();
						showInfo();
					}
				}
		
					#endregion

				#region selection unit
				else if ( game.grid[ selected.X, selected.Y ].stack.Length > 0 ) //  && unitMoved == false
				{	// units
					//selected.state = 0;
					int myUnit;

					if ( game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player != Form1.game.curPlayerInd )
						myUnit = Ai.unitMine( selected.X, selected.Y );
					else
						myUnit = game.grid[ selected.X, selected.Y ].stackPos;

					if ( // selection city, double click
						game.grid[ selected.X, selected.Y ].city > 0 && 
						selected.state == 1 && 
						selected.owner == game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player && 
						selected.unit == game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind && //meme unit, meme state, meme owner
						/*game.playerList[ selected.owner ].unitList[ selected.unit ].X == xUnit && 
						game.playerList[ selected.owner ].unitList[ selected.unit ].Y == yUnit &&*/
						game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd ) // meme position
					{
						openCityFrm( (byte)(game.grid[ selected.X, selected.Y ].territory - 1), game.grid[ selected.X, selected.Y ].city );
					}
					else if ( myUnit != 0 )
					{
						selected.state = 1;
						selected.owner = (byte)game.grid[ selected.X, selected.Y ].stack [ myUnit - 1 ].player.player;
						selected.unit = game.grid[ selected.X, selected.Y ].stack [ myUnit - 1 ].ind;
						selected.unitInTransport = 0;
						game.grid[ selected.X, selected.Y ].stackPos = myUnit;

						if ( 
							selected.owner == Form1.game.curPlayerInd && 
							game.playerList[ selected.owner ].unitList[ selected.unit ].state != (byte)Form1.unitState.turnCompleted &&
							( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state == (byte)Form1.unitState.buildingRoad ||
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state == (byte)Form1.unitState.buildingIrrigation ||
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state == (byte)Form1.unitState.buildingMine ||
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state == (byte)Form1.unitState.buildingFort ||
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state == (byte)Form1.unitState.buildingAirport ||
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state == (byte)Form1.unitState.buildingMineFld
							)
							)
						{
							string building = "";

							switch( game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state )
							{
								case (byte)Form1.unitState.buildingRoad:
									if ( game.grid[ selected.X, selected.Y ].roadLevel == 0 )
										building = "road";
									else
										building = "railroad";

									break;

								case (byte)Form1.unitState.buildingIrrigation:
									building = "irrigation";
									break;

								case (byte)Form1.unitState.buildingMine:
									building = "mine";
									break;

								case (byte)Form1.unitState.buildingFort:
									building = "fort";
									break;

								case (byte)Form1.unitState.buildingAirport:
									building = "airport";
									break;

								case (byte)Form1.unitState.buildingMineFld:
									building = "mine field";
									break;

							}

							if ( MessageBox.Show( 
								"Do you want to cancel this unit's order?\n" + caseImprovement.getTurnLeftString( Form1.game.curPlayerInd, selected.unit) + " left", 
								"Building " + building, 
								MessageBoxButtons.YesNo, 
								MessageBoxIcon.None, 
								MessageBoxDefaultButton.Button1 ) == DialogResult.Yes )
							{
								caseImprovement.removeUnitFromCaseImps( Form1.game.curPlayerInd, selected.unit );
							}
							else
							{ // no
								selected.state = 0;
							}
						}
						else
						{
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state = (byte)Form1.unitState.idle;
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].automated = false;
						}
					}
					enableMenus();

					//showInfo();
					// label2.Visible = true;
					// label2.Text = Statistics.units [ game.playerList[ selected.owner ].unitList[ selected.unit ].type ].name + "\nmove left: " + game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft.ToString() + "\na/d/m";
				} 
					#endregion

					#region selection city
				else if( game.grid[ selected.X, selected.Y ].city > 0)
				{	// cities
					if ( selected.state == 2 && selected.owner == game.grid[ selected.X, selected.Y ].territory - 1 && selected.unit == game.grid[ selected.X, selected.Y].city)
					{
						openCityFrm( selected.owner, selected.unit );
					}

					selected.state = 2;
					selected.owner = 0;
					selected.unit = game.grid[ selected.X, selected.Y].city;  // changer lke bye

					//label2.Visible = true;
					//showInfo();
					//label2.Text = cityList[ selected.owner, selected.unit ].name ; // + "\nmove left: " + unitList[unitSelected[1], unitSelected[2], (byte)unitListChoice.moveLeft].ToString() + "\na/d/m";
				}
					#endregion

				else // terrain
				{
					selected.state = 3;
					//showInfo();
				}
			/*}
			catch ( System.Exception ex )
			{
				MessageBox.Show( ex.Message + "\nMore info: " + eic.Normal.ToString() );
				DrawMap();
			}*/	
			}
			#endregion

			#region selectionState.bombard
			else if ( selectState == (byte)enums.selectionState.bombard ) 
			{
				/*try
				{*/
				selectState = 0;
				if ( game.grid[ selected.X, selected.Y ].selectedLevel == 2 )
				{
					for ( int x = 0; x < game.width; x ++ )
						for ( int y = 0; y < game.height; y ++ )
							game.grid[ x, y ].selectedLevel = 0;

					if ( !game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].dead )
					{
						bombardCase( Form1.game.curPlayerInd, curUnit, selected.X, selected.Y, "click" );

						if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.cruiseMissile )
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].kill();
							//	unitDelete( Form1.game.curPlayerInd, selected.unit );
					}

					guiEnabled = true;
				}

				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						game.grid[ x, y ].selectedLevel = 0;

				guiEnabled = true;
					
		/*	}
			catch ( System.Exception ex )
			{
				MessageBox.Show( ex.Message + "\nMore info: " + eic.Bombard.ToString() );
				DrawMap();
			}	*/
			}
				#endregion

			#region selectionState.launchMissile
			else if ( selectState == (byte)enums.selectionState.launchMissile ) 
			{
				/*try
				{*/
				/*if ( 
					Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeBomb ||
					Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeMissile
					) 
				{ 
				}
				else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeIC	) 
				{ 
				}*/

				if ( 
					game.grid[ selected.X, selected.Y ].selectedLevel > 0 || 
					Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeIC 
					) 
				{
					for ( int x = 0; x < game.width; x ++ )
						for ( int y = 0; y < game.height; y ++ )
							game.grid[ x, y ].selectedLevel = 0;

					for ( int r = 0; r < 2; r ++ )
					{
						Point[] sqr = game.radius.returnEmptySquare( new Point( selected.X, selected.Y ), r );

						for ( int i = 0; i < sqr.Length; i ++ )
							game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 2;
					}
					zoomOn( new Point( selected.X, selected.Y ) );

					DialogResult res;

					if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeIC )
						res = MessageBox.Show( 
							"Are you sure you want to nuke this location? The missile will hit at the beginning of your next turn.", 
							"Launching!",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question,
							MessageBoxDefaultButton.Button1
							);
					else
						res = MessageBox.Show( 
							"Are you sure you want to nuke this location?", 
							"Launching!",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question,
							MessageBoxDefaultButton.Button1
							);

					if ( res == DialogResult.Yes )
					{
						guiEnabled = false;
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].kill();
					//	unitDelete( Form1.game.curPlayerInd, selected.unit );

						for ( int x = 0; x < game.width; x ++ )
							for ( int y = 0; y < game.height; y ++ )
								game.grid[ x, y ].selectedLevel = 0;

						selectState = (byte)enums.selectionState.normal;

						if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeIC )
						{
							game.playerList[ Form1.game.curPlayerInd ].memory.add( (byte)enums.playerMemory.launchedICBM, new int[] { selected.X, selected.Y } );
							DrawMap();
						}
						else
							nukeCase( Form1.game.curPlayerInd, selected.unit, new Point( selected.X, selected.Y ) );

						cmdCancel.Visible = false;
						guiEnabled = true;
					}
					else if ( res == DialogResult.No )
					{
						mILaunch_Click( this, EventArgs.Empty );
					}
					else // cancel
					{
						for ( int x = 0; x < game.width; x ++ )
							for ( int y = 0; y < game.height; y ++ )
								game.grid[ x, y ].selectedLevel = 0;

						selectState = (byte)enums.selectionState.normal;
						cmdCancel.Visible = false;
						guiEnabled = true;
						DrawMap();
					}
				}
				else
				{
				}
			/*}
			catch ( System.Exception ex )
			{
				MessageBox.Show( ex.Message + "\nMore info: " + eic.Missile.ToString() );
				DrawMap();
			}*/	
				
				/*
					for ( int x = 0; x < game.width; x ++ )
						for ( int y = 0; y < game.height; y ++ )
							game.grid[ x, y ].selectedLevel = 0;

					bombardCase( Form1.game.curPlayerInd, curUnit, selected.X, selected.Y, "click" );
					guiEnabled = false;
				}

				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						game.grid[ x, y ].selectedLevel = 0;

				selectState = 0;
				guiEnabled = true;*/
			}
				#endregion

			#region selectionState.planeRebase
			else if ( selectState == (byte)enums.selectionState.planeRebase )
			{
				/*try
				{*/
				if ( game.grid[ selected.X, selected.Y ].selectedLevel == 2 )
					UnitMove( 
						Form1.game.curPlayerInd, 
						selected.unit, 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y,
						selected.X, 
						selected.Y
						);

				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						game.grid[ x, y ].selectedLevel = 0;

				selectState = 0;
				guiEnabled = true;
		/*	}
			catch ( System.Exception ex )
			{
				MessageBox.Show( ex.Message + "\nMore info: " + eic.Rebase.ToString() );
				DrawMap();
			}	*/
			}
				#endregion 

			#region selectionState.planeRecon
			else if ( selectState == (byte)enums.selectionState.planeRecon )
			{
				/*try
				{*/
				if ( game.grid[ selected.X, selected.Y ].selectedLevel == 1 )
				{
					if ( 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].typeClass.speciality == enums.speciality.bomber ||
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].typeClass.speciality == enums.speciality.fighter
						)
						interceptPlaneMovement( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ], 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].onCase, 
							game.grid[ selected.X, selected.Y ]
							);

					if ( !game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].dead )
					{
						sight.seeByPlaneAt( Form1.game.curPlayerInd, selected.unit, new Point( selected.X, selected.Y ) );
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].moveLeft --;

						for ( int x = 0; x < game.width; x ++ )
							for ( int y = 0; y < game.height; y ++ )
								game.grid[ x, y ].selectedLevel = 0;

						selectState = 0;
						guiEnabled = true;
					}

					if ( 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].dead ||
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].moveLeft == 0 
						)
						showNextUnit( Form1.game.curPlayerInd, selected.unit );
					else
						DrawMap();
				}
				else
				{
					selectState = 0;
					guiEnabled = true;

					DrawMap();
				}
			/*}
			catch ( System.Exception ex )
			{
				MessageBox.Show( ex.Message + "\nMore info: " + eic.Recon.ToString() );
				DrawMap();
			}	*/
			}
				#endregion 

			#region selectionState.goTo
			else if ( selectState == (byte)enums.selectionState.goTo ) 
			{
				/*try
				{*/
				bool validSelection = false;

				if ( 
					game.grid[ selected.X, selected.Y ].continent == 
					game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].continent ||
					game.grid[ selected.X, selected.Y ].city > 0 ||
					game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].city > 0
					)
				{
					bool attack;
					if ( 
						(
						game.grid[ selected.X, selected.Y ].territory != 0 &&
						game.grid[ selected.X, selected.Y ].territory != Form1.game.curPlayerInd &&
						game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ selected.X, selected.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war
						) || 
						( 
						game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].territory != 0 &&
						game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].territory != Form1.game.curPlayerInd &&
						game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war
						)
						)
						attack = true;
					else
						attack = false;

					Point[] way = game.radius.findWayTo( 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y,
						selected.X,
						selected.Y,
						Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain,
						Form1.game.curPlayerInd,
						attack
						);

					if ( way[ 0 ].X >= 0 )
					{
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].dest = new Point( selected.X, selected.Y );
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state = (byte)Form1.unitState.goTo;
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].automated = true;
						selectState = (byte)enums.selectionState.normal;
					}
					else
					//if ( !validSelection )
						if ( 
							MessageBox.Show( 
							"The selected destination is invalid.  Do you want to select another destination?", 
							"Invalid destination", 
							MessageBoxButtons.YesNo, 
							MessageBoxIcon.None, 
							MessageBoxDefaultButton.Button1 
							) == DialogResult.No 
							)
						{
							selectState = (byte)enums.selectionState.normal;
						}
					}
			/*	}
				catch ( System.Exception ex )
				{
					MessageBox.Show( ex.Message + "\nMore info: " + eic.GoTo.ToString() );
					DrawMap();
				}	*/
			}
			#endregion


			#region changer sli.. limites


			/*label1.Visible = true;
			label1.Text = selected.X.ToString() + ", " + selected.Y.ToString();

			if ( selected.X - sliHor < 1 ) // x min
			{
				sliHor -= 1;
			}
			else if ( selected.X - sliHor > 3 ) // x max // - ( ( selected.Y + 1 ) % 2 ) 
			{
				sliHor += 1;
			}
			else if ( selected.Y - sliVer <= 2 ) // y min
			{
				sliVer -= 4;
			}
			else if ( selected.Y - sliVer > 13 ) // y max
			{
				sliVer += 4;
			}
			else
			{
				// label1.Visible = false;
			}*/
			#endregion

			if ( bob2.X < 20 )
				sliHor -= 2;
			else if ( bob2.X > this.ClientSize.Width - 20 ) 
				sliHor += 2;

			if ( bob2.Y < 20 )
				sliVer -= 4;
			else if ( bob2.Y > this.ClientSize.Height - 20 ) 
				sliVer += 4;

			if ( sliVer < 0 )
				sliVer = 1;
			else if ( sliVer > game.height - 16 ) 
				sliVer = game.height - 16;

			if ( sliHor < 0 )
				sliHor = game.width - 1;
			else if ( sliHor > game.width ) 
				sliHor = 1;

			showInfo();
			enableMenus();
			DrawMap();

		/*	}
			catch ( System.Exception ex )
			{
				MessageBox.Show( ex.Message + "\nMore info: " + eic.Else.ToString() );
				DrawMap();
			}*/
		}

		private bool isInRectPop( int x1, int x, int y, Point click ) 
		{
			SizeF popSize = g.MeasureString( 
				game.playerList[ Form1.game.grid[ x + sliHor, y + sliVer ].territory - 1 ].cityList[ Form1.game.grid[ x + sliHor, y + sliVer ].city ].population.ToString(), 
				cityPopFont
				);

			int xMax = caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + 31 + 5 + ( (int)popSize.Width + 3 ),
				xMin = caseWidth * x + ( (y + sliVer) % 2) * caseWidth / 2 + 31 + 5,
				yMax = caseHeight/2*y - 8 - 5 + ( (int)popSize.Height - 2 ),
				yMin = caseHeight/2*y - 8 - 5;

			if (
				click.X > xMin && 
				click.X < xMax &&  
				click.Y > yMin &&  
				click.Y < yMax 
				)
				return true;
			else
				return false;
		}
		
		#endregion

	#region pictureBox2 click !Selection! mini map
		private void pictureBox2_Click(object sender, System.EventArgs e)
		{
			Point pntClicked = pictureBox2.PointToClient(new Point(Form1.MousePosition.X, Form1.MousePosition.Y));

			label1.Text = pntClicked.X.ToString() + "; " + pntClicked.Y.ToString() + ". " + miniMapAdjX.ToString();
			sliHor = (int)Math.Floor( pntClicked.X * miniMapAdjX ) - 2;
			sliVer = 2 * (int)Math.Floor( pntClicked.Y * miniMapAdjY/2 ) - 8;

			if ( sliVer > game.height - 16 )
				sliVer = game.height - 16;
			else if ( sliVer < 0 )
				sliVer = 0;

			if ( sliHor > game.width )
				sliHor -= game.width;
			else if ( sliHor < 0 )
				sliHor += game.width - 1;

			DrawMap();
		}
		#endregion

	#region pictureBox3 click !Selection! unit list
		private void pictureBox3_Clicked(object sender, System.EventArgs e)
		{
			Point pntClicked = pictureBox3.PointToClient(new Point(Form1.MousePosition.X, Form1.MousePosition.Y));

			for ( byte i = 1; i <= game.grid[ selected.X, selected.Y ].stack.Length - unitAffSli; i ++ )
			{
				if ( pntClicked.X > i * 50 - 60 && pntClicked.X < i * 50 - 10 )
				{
					if ( game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli) - 1 ].player.player == Form1.game.curPlayerInd )
					{
						selected.owner = (byte)game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli) - 1 ].player.player; // game.grid[ selected.X, selected.Y ].stack.Length
						selected.unit = game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli) - 1 ].ind; 

						game.grid[ selected.X, selected.Y ].stackPos = ( i + unitAffSli) ;
						break;
					}
					else
					{
						selected.state = 0;
					}
				}
			}
	
			enableMenus();
			showInfo();
			DrawMap();
			//pictureBox1.Focus();

		
		}
		#endregion

	#region pictureBox4 click !Selection! unit transport list
		private void pictureBox4_Clicked(object sender, System.EventArgs e)
		{
			Point pntClicked = pictureBox4.PointToClient(new Point(Form1.MousePosition.X, Form1.MousePosition.Y));

			for ( byte i = 0; i < game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].transported - transportAffSli; i ++ )
			{
				if ( pntClicked.X > ( i + 1 ) * 50 - 60 && pntClicked.X < ( i + 1 ) * 50 - 10 )
				{
					/*if ( game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli) - 1 ].player.player == Form1.game.curPlayerInd )
					{*/
					//selected.owner = (byte)game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli) - 1 ].player.player; 
					//selected.unit = game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].unit ].transport[ i + transportAffSli ]; 

					selected.unitInTransport = (byte)(i + transportAffSli);

					//game.grid[ selected.X, selected.Y ].stackPos = ( i + unitAffSli) ;
					break;
					/*}
					else
					{
						selected.state = 0;
					}*/
				}
			}
	
			enableMenus();
			showInfo();
			DrawMap();
			//pictureBox1.Focus();

		
		}
		#endregion

#endregion

#region unit movement 

		public void UnitMove ( UnitList unit, Case dest ) 
		{ 
			UnitMove ( 
				unit.player.player, 
				unit.ind, 
				unit.pos.X, 
				unit.pos.Y, 
				dest.X, 
				dest.Y 
				); 
		} 
		public void UnitMove ( byte owner, int unit, Point dest ) 
		{ 
			UnitMove ( 
				owner, 
				unit, 
				game.playerList[ owner ].unitList[ unit ].pos.X, 
				game.playerList[ owner ].unitList[ unit ].pos.Y, 
				dest.X, 
				dest.Y 
				); 
		} 
		public void UnitMove ( byte owner, int unit, int xUnit, int yUnit, int xDest, int yDest ) 
		{ 
			if ( 
				game.playerList[ owner ].unitList[ unit ].typeClass.speciality == enums.speciality.bomber ||
				game.playerList[ owner ].unitList[ unit ].typeClass.speciality == enums.speciality.fighter
				)
				interceptPlaneMovement( 
					game.playerList[ owner ].unitList[ unit ], 
					game.playerList[ owner ].unitList[ unit ].onCase, 
					game.grid[ xDest, yDest ]
					);

			if ( !game.playerList[ owner ].unitList[ unit ].dead )
			{
				Point dest = new Point( xDest, yDest );
				Point ori = new Point( xUnit, yUnit );

				#region take control of the city
				if ( 
					game.grid[ xDest, yDest ].city > 0 && 
					game.playerList[ owner ].foreignRelation[ game.grid[ xDest, yDest ].territory - 1 ].politic == (byte)relationPolType.war 
					)
				{ // control city
					game.playerList[ game.grid[ xDest, yDest ].territory - 1 ].cityList[ game.grid[ xDest, yDest ].city ].transfertToPlayer( game.playerList[ owner ] );

				/*	byte ancientOwner = (byte)(game.grid[ xDest, yDest ].territory - 1);
					int ancientCity = game.grid[ xDest, yDest ].city;
					game.playerList[ ancientOwner ].cityList[ game.grid[ xDest, yDest ].city ].state = (byte)enums.cityState.dead;

					game.playerList[ owner ].cityNumber ++;
				
					if ( game.playerList[ owner ].cityNumber >= game.playerList[ owner ].cityList.Length )
					{
						CityList[] cityListBuffer = game.playerList[ owner ].cityList;
						game.playerList[ owner ].cityList = new CityList[ cityListBuffer.Length + 5 ];

						for ( int i = 0; i < cityListBuffer.Length; i ++ )
							game.playerList[ owner ].cityList[ i ] = cityListBuffer[ i ];
					}

					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ] = new CityList( game.playerList[ owner ], game.playerList[ owner ].cityNumber );

					getTreasure( owner, ancientOwner, game.grid[ xDest, yDest ].city );

					labor.removeAllLabor( ancientOwner, game.grid[ xDest, yDest ].city );

					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].buildingList = 
						game.playerList[ ancientOwner ].cityList[ ancientCity ].buildingList;

					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].foodReserve = 
						game.playerList[ ancientOwner ].cityList[ ancientCity ].foodReserve;
							
					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].name = 
						game.playerList[ ancientOwner ].cityList[ ancientCity ].name;
				
					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].population = 
						game.playerList[ ancientOwner ].cityList[ ancientCity ].population;
				
					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].X = 
						game.playerList[ ancientOwner ].cityList[ ancientCity ].X;
				
					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].Y = 
						game.playerList[ ancientOwner ].cityList[ ancientCity ].Y;
				
					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].originalOwner =
						game.playerList[ ancientOwner ].cityList[ ancientCity ].originalOwner;

					game.playerList[ game.grid[ xDest, yDest ].territory - 1 ].cityList[ ancientCity ].state = (byte)enums.cityState.dead;

					//	game.grid[ xDest, yDest ].territory - 1 = owner;
					game.grid[ xDest, yDest ].city = game.playerList[ owner ].cityNumber;

					if ( game.playerList[ owner ].economyType == (byte)enums.economies.slaveryRacial && game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].population > 0 )
					{
						int slavesToTransfert = game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].population / 2;
						game.playerList[ owner ].slaves.addSlave( slavesToTransfert );
						game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].population -= (byte)slavesToTransfert;
						MessageBox.Show( 
							String.Format( language.getAString( language.order.slaveryAquiredFromCity ), slavesToTransfert, game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].name ), 
							language.getAString( language.order.slaveryTitle ) 
							);
					}

					ai.chooseConstruction( owner, game.playerList[ owner ].cityNumber );

					game.frontier.setFrontiers();

					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].laborPos = new Point[ 0 ];
					labor.addAllLabor( owner, game.playerList[ owner ].cityNumber );
					game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].setHasDirectAccessToRessource();

					bool oneCityLeft = false;
					for ( int i = 1; i <= game.playerList[ ancientOwner ].cityNumber; i ++ )
						if ( game.playerList[ ancientOwner ].cityList[ i ].state != (byte)enums.cityState.dead )
						{
							oneCityLeft = true;
							break;
						}

					if ( !oneCityLeft )
					{
					/*	game.playerList[ ancientOwner ].dead = true;

						for ( int i = 1; i <= game.playerList[ ancientOwner ].unitNumber; i ++ )
							if ( !game.playerList[ ancientOwner ].unitList[ i ].dead )
								unitDelete( ancientOwner, i );

						playerLost( ancientOwner, owner );*

						game.playerList[ ancientOwner ].kill( game.playerList[ owner ] );

					}
					else if ( game.playerList[ ancientOwner ].cityList[ ancientCity ].isCapitale )
					{
						bool found = false;
						for ( int c = 1; c <= game.playerList[ ancientOwner ].cityNumber && !found; c ++ )
							if ( game.playerList[ ancientOwner ].cityList[ c ].state != (byte)enums.cityState.dead )
							{
								found = true; 
								game.playerList[ ancientOwner ].cityList[ c ].isCapitale = true; 
							}

						if ( !found )
							MessageBox.Show( "error in finding a capital" );
					}

					if ( owner == Form1.game.curPlayerInd )
						openCityFrm( owner, game.playerList[ owner ].cityNumber );*/

				} // end city invasion
				#endregion 
  
				for ( int i = 1; i <= game.grid[ xUnit, yUnit].stack.Length; i ++ ) 
					if ( 
						game.grid[ xUnit, yUnit ].stack[ i - 1 ].player.player == owner && 
						game.grid[ xUnit, yUnit ].stack[ i - 1 ].ind == unit 
						) 
					{ 
						game.grid[ xUnit, yUnit ].stackPos = i; 
						break; 
					} 

				game.playerList[ owner ].unitList[ unit ].X = xDest; 
				game.playerList[ owner ].unitList[ unit ].Y = yDest; 

				if ( Statistics.units[ game.playerList[ owner ].unitList[ unit ].type ].terrain < 2 )
					move.payThrough( owner, unit, ori, dest );
				else
					move.payThis( owner, unit, 1 );

				move.moveUnitToCase( xDest, yDest, owner, unit ); 
				move.moveUnitFromCase( xUnit, yUnit, owner, unit ); 

				game.grid[ xDest, yDest ].stackPos = game.grid[ xDest, yDest ].stack.Length; 
				game.grid[ xUnit, yUnit ].stackPos = 1; 

				#region special resources
				if ( game.grid[  xDest, yDest ].resources > 0 && game.grid[  xDest, yDest ].resources < 100 )
				{
					if ( game.grid[  xDest, yDest ].resources == (byte)enums.speciaResources.horses )
					{
						if ( !game.playerList[ owner ].technos[ (byte)Form1.technoList.horseBreed ].researched )
						{
							game.playerList[ owner ].technos[ (byte)Form1.technoList.horseBreed ].researched = true;

							if ( Form1.game.curPlayerInd == owner )
							{
								DrawMap();
								MessageBox.Show( "You just discovered horses.", "Rare resources" );
							}
						}
					}
					else if ( game.grid[  xDest, yDest ].resources == (byte)enums.speciaResources.elephant )
					{
						if ( !game.playerList[ owner ].technos[ (byte)Form1.technoList.elephantBreed ].researched )
						{
							game.playerList[ owner ].technos[ (byte)Form1.technoList.elephantBreed ].researched = true;

							if ( Form1.game.curPlayerInd == owner )
							{
								DrawMap();
								MessageBox.Show( "You just discovered elephants.", "Rare resources" );
							}
						}
					}
					else if ( game.grid[  xDest, yDest ].resources == (byte)enums.speciaResources.camel )
					{
						if ( !game.playerList[ owner ].technos[ (byte)Form1.technoList.camelBreed ].researched )
						{
							game.playerList[ owner ].technos[ (byte)Form1.technoList.camelBreed ].researched = true;

							if ( Form1.game.curPlayerInd == owner )
							{
								DrawMap();
								MessageBox.Show( "You just discovered camels.", "Rare resources" );
							}
						}
					}

					game.grid[  xDest, yDest ].resources = 0;
				}
				#endregion

				ai Ai = new ai();

				if ( game.grid[ xDest, yDest ].militaryImprovement == (byte)enums.militaryImprovement.mineField )
				{
					Random r = new Random();
					if ( r.Next( 2 ) == 0 )
					{
						game.playerList[ owner ].unitList[ unit ].health --;

						if ( game.playerList[ owner ].unitList[ unit ].health == 0 )
							game.playerList[ owner ].unitList[ unit ].kill();
						//	unitDelete( owner, unit );
					}
				}

				if ( game.playerList[ owner ].isInWar && game.playerList[ owner ].unitList[ unit ].state != (byte)Form1.unitState.dead )
				{
					UnitList[] ennemyArt = Ai.fortEnnemyAtillery( new Point( xDest, yDest ), owner );

					for ( int i = 0; i < ennemyArt.Length; i ++ )
						bombardCase( ennemyArt[ i ].player.player, ennemyArt[ i ].ind, xDest, yDest, "in range"  );// );//owner, unit, xDest, yDest, "in range"  );
				}

				sight.setUnitSight( owner, unit );
			}
		}
		#endregion

#region unit Creation
		/*public void createUnit( int x, int y, byte player, byte unitType )
		{
			if ( Statistics.units[ unitType ].speciality != enums.speciality.builder )
			{
				int mf = count.militaryFunding( player );
				int un = Form1.game.playerList[ player ].totMilitaryUnits; //count.militaryUnits( player );
				sbyte nextMFp = 0;
				bool mod = true;

				getPFT gp = new getPFT(); 

				if ( un == 0 ) 
				{ 
					nextMFp = (sbyte)( 3 * 100 / Form1.game.playerList[ player ].totalTrade + 1 ); 
				} 
				else // game.playerList[ player ].preferences.military 
				{ 
					if ( un <= mf / 3 ) // over yellow 
						nextMFp = (sbyte)( 3 * 100 * ( un + 1 ) / Form1.game.playerList[ player ].totalTrade + 1 ); 
					else if ( un <= mf * 6 / ( 3 * 5 ) ) // over red 
						nextMFp = (sbyte)( 3 * 100 * 4 * ( un + 1 ) / ( 5 * Form1.game.playerList[ player ].totalTrade ) + 1 ); 
					else 
						mod = false;
				}

				if ( mod )
					aiPref.setMilitary( player, nextMFp );
			}

			game.playerList[ player ].unitNumber ++;

			if ( game.playerList[ player ].unitNumber >= game.playerList[ player ].unitList.Length )
			{
				UnitList[] unitListBuffer = game.playerList[ player ].unitList;
				game.playerList[ player ].unitList = new UnitList[ unitListBuffer.Length + 10 ];

				for ( int i = 0; i < unitListBuffer.Length; i ++ )
					game.playerList[ player ].unitList[ i ] = unitListBuffer[ i ];
			}

			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ] = new UnitList( game.playerList[ player ], game.playerList[ player ].unitNumber );

			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].X = x;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].Y = y;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].type = unitType;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].moveLeft = (sbyte)Statistics.units[ unitType].move;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].moveLeftFraction = 0;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].state = (byte)Form1.unitState.idle;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].health = 3;
			game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].level = 1;

			// transport
			if ( Statistics.units[ unitType ].transport > 0 )
			{
				game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].transport = new int[ Statistics.units[ unitType ].transport ];
				game.playerList[ player ].unitList[ game.playerList[ player ].unitNumber ].transported = 0;
			}		

			move.moveUnitToCase( x, y, player, game.playerList[ player ].unitNumber );

			sight.discoverRadius( x, y, Statistics.units[ unitType ].sight, player );

			#region special resources
			if ( game.grid[ x, y ].resources > 0 && game.grid[ x, y ].resources < 100 )
			{
				if ( game.grid[  x, y ].resources == (byte)enums.speciaResources.horses )
				{
					if ( !game.playerList[ player ].technos[ (byte)Form1.technoList.horseBreed ].researched )
					{
						game.playerList[ player ].technos[ (byte)Form1.technoList.horseBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered horses.", "Rare resources" );
						}
					}
				}
				else if ( game.grid[  x, y ].resources == (byte)enums.speciaResources.elephant )
				{
					if ( !game.playerList[ player ].technos[ (byte)Form1.technoList.elephantBreed ].researched )
					{
						game.playerList[ player ].technos[ (byte)Form1.technoList.elephantBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered elephants.", "Rare resources" );
						}
					}
				}
				else if ( game.grid[  x, y ].resources == (byte)enums.speciaResources.camel )
				{
					if ( !game.playerList[ player ].technos[ (byte)Form1.technoList.camelBreed ].researched )
					{
						game.playerList[ player ].technos[ (byte)Form1.technoList.camelBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered camels.", "Rare resources" );
						}
					}
				}

				game.grid[  x, y ].resources = 0;
			}
			#endregion

		}*/
		#endregion

#region unit Deletion
	/*	public void unitDelete( byte player, int unit )
		{
			for ( int i = 0; i < game.playerList[ player ].unitList[ unit ].transported; i ++ )
				game.playerList[ player ].unitList[ game.playerList[ player ].unitList[ unit ].transport[ i ] ].state = (byte)Form1.unitState.dead;

			int x = game.playerList[ player ].unitList[ unit ].X;
			int y = game.playerList[ player ].unitList[ unit ].Y;

		/*	for ( int i = 0; i < game.caseImps.Length; i ++ )
				if ( game.caseImps[ i ].owner == player )
					for ( int u = 0; u < game.caseImps[ i ].units.Length; u ++ )
						if ( game.caseImps[ i ].units[ u ].ind == unit )*

			if ( game.playerList[ player ].unitList[ unit ].typeClass.speciality == enums.speciality.builder )
				caseImprovement.removeUnitFromCaseImps( player, unit );

			if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.inTransport )
				transport.removeUnitFromTransport( player, unit, transport.findTransporterOf( player, unit ) );
			else
			{
				move.moveUnitFromCase( x, y, player, unit );
				game.grid[ x, y ].stackPos = game.grid[ x, y ].stack.Length;
			}

			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.dead;

		/*	bool lost = true;
			if ( !game.playerList[ player ].dead )
				for ( int i = 1; i < game.playerList[ player ].unitNumber; i ++ )
					if ( game.playerList[ player ].unitList[ i ].state != (byte)Form1.unitState.dead )
					{
						lost = false;
						break;
					}

			if ( lost )
				playerLost( ancientOwner, -1 );*
		}*/
#endregion

#region enable menus
		private void enableMenus()
		{
			if ( 
				selected.state == 1 &&
				selected.owner == Form1.game.curPlayerInd && 
				selected.unit > 0 &&
				Statistics.units[ game.playerList[ selected.owner ].unitList[ selected.unit ].type ].speciality > enums.speciality.builder && 
				( 
					game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || 
					game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 
				)
				)
				menuItem5.Enabled = true;
			else
				menuItem5.Enabled = false;
		}
		#endregion

#region city creation

	#region create city
		private void createCity( int x, int y, byte owner, int unit, bool delUnit, string name )
		{
			game.playerList[ owner ].cityNumber ++; 

			if ( game.playerList[ owner ].cityList.Length <= game.playerList[ owner ].cityNumber )
			{
				CityList[] buffer = game.playerList[ owner ].cityList;
				game.playerList[ owner ].cityList = new CityList[ game.playerList[ owner ].cityNumber + 5 ];

				for ( int i = 0; i < buffer.Length; i ++ )
					game.playerList[ owner ].cityList[ i ] = buffer[ i ];
			}

			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ] = new CityList( game.playerList[ owner ], game.playerList[ owner ].cityNumber );

			if ( 
				game.grid[ x, y ].type == (byte)enums.terrainType.forest ||
				game.grid[ x, y ].type == (byte)enums.terrainType.jungle 
				)
			{
				game.grid[ x, y ].type = (byte)enums.terrainType.plain;
			}

			game.grid[ x, y ].city = game.playerList[ owner ].cityNumber;
		//	game.grid[ x, y ].territory - 1 = owner;

			if ( delUnit )
				game.playerList[ owner ].unitList[ unit ].kill();
		//		unitDelete( owner, unit );

			if ( game.playerList[ owner ].cityNumber == 1 )
				game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].isCapitale = true;

			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].name = name;
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].X = x;
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].Y = y;
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].originalOwner = owner;
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].buildingList = new bool[ Statistics.buildings.Length ];
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].population = 1;
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].foodReserve = 0;
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].laborOnField = 0; /// a changer
			
			//ai AI = new ai();
			ai.chooseConstruction( owner, game.playerList[ owner ].cityNumber );

			caseImprovement.killAllImprovementsAt( new Point( x, y ) );

			game.grid[ x, y ].roadLevel = 1;
			roads.setAround( game, new Point(x, y) );

			game.frontier.setFrontiers();
			
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].laborPos = new Point[ 0 ];
			labor.autoAddLabor( owner, game.playerList[ owner ].cityNumber );
			game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].setHasDirectAccessToRessource();
			game.playerList[ owner ].invalidateTrade();
			
			game.rvtbc.doit();
		}
		#endregion

		public void openCityFrm( byte owner, int city )
		{
			open.cityFrm( owner, city, this );
			/*wC.show = true;
			FrmCity frm2 = new FrmCity( owner, city);

			string title = this.Text;
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";
			wC.show = false;
			frm2.ShowDialog();
			this.Text = title;*/
		}

		private void mICity_Click(object sender, System.EventArgs e) 
		{ // build city
			if ( 
				game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || 
				game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 
				)
			{
				if ( game.playerList[ Form1.game.curPlayerInd ].cityNumber == 0 )
					Tutorial.show( this, Tutorial.order.firstCity );

				guiEnabled = false;
				userTextInput ui = new userTextInput( language.getAString( language.order.uiNewCity ), language.getAString( language.order.uiPleaseEntrerCityName ), findName( Form1.game.curPlayerInd ), language.getAString( language.order.ok ), language.getAString( language.order.cancel ) );
			//	string name = ui.showInputRequest( null, findName( Form1.game.curPlayerInd ) );
				ui.ShowDialog();
				string name = ui.result;

				if ( name != "" )
				{
					createCity( selected.X, selected.Y, Form1.game.curPlayerInd, selected.unit, true, name );
					openCityFrm( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].cityNumber );
					selected.state = 2;
					selected.owner = Form1.game.curPlayerInd;
					selected.unit = game.grid[ selected.X, selected.Y ].city;
					oldSliVer = -1;
					DrawMap();
					showInfo();
				}
				else
				{
					game.playerList[ Form1.game.curPlayerInd ].posInCityFile--;
				}

				guiEnabled = true; 

			}
		}

		private string findName( byte player ) 
		{ 
			while ( game.playerList[ player ].posInCityFile >= Statistics.civilizations[ game.playerList[ player ].civType ].cityNames.Length )
				game.playerList[ player ].posInCityFile -= (byte)Statistics.civilizations[ game.playerList[ player ].civType ].cityNames.Length;
			
			game.playerList[ player ].posInCityFile ++;
			return Statistics.civilizations[ game.playerList[ player ].civType ].cityNames[ game.playerList[ player ].posInCityFile - 1 ]; 
		/*	for ( int i = 1; true; i ++ ) 
				if ( game.playerList[ owner ].cityNumber < Statistics.civilizations[ game.playerList[ owner ].civType ].cityNames.Length * i ) 
					return Statistics.civilizations[ game.playerList[ owner ].civType ].cityNames[ game.playerList[ owner ].cityNumber - Statistics.civilizations[ game.playerList[ owner ].civType ].cityNames.Length * ( i - 1 ) ]; 
		*/
		} 
#endregion

#region show info

		public void showInfo()
		{
			pictureBox4.Visible = false;
			pictureBox3.Visible = false;

			if ( 
				selected.state != 0 && 
				game.grid[ selected.X, selected.Y ].stack.Length > 0 &&
				game.playerList[ Form1.game.curPlayerInd ].see[ selected.X, selected.Y ]
				)
			{
				pictureBox3.Visible = true;

				#region populate pictureBox3
				Bitmap unitListBmp = new Bitmap(Form1.appPath + "\\images\\UI\\infoBack.jpg" );
				Graphics gUnitList = Graphics.FromImage( unitListBmp );

				for ( int i = 1; i <=  game.grid[ selected.X, selected.Y ].stack.Length - unitAffSli; i ++ ) // game.grid[ selected.X, selected.Y ].stack.Length // game.grid[ selected.X, selected.Y ].stack.Length
				{
					if ( game.grid[ selected.X, selected.Y ].stackPos == i + unitAffSli )
					{
						gUnitList.DrawRectangle(
							new Pen( Color.White ),
							new Rectangle(
							i * 50 - 47,
							3,
							44,
							45
							)	
							);
					}

					gUnitList.DrawImage( 
						game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].typeClass.bmp,
						new Rectangle(
						i * 50 - 60,
						0,
						70,
						50),
						0,
						0,
						70,
						50,
						GraphicsUnit.Pixel,
						ia
						);

					SizeF bob4 = gUnitList.MeasureString( 
						game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].typeClass.name,
						new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 7 ), FontStyle.Regular) );

					if ( game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player == Form1.game.curPlayerInd )
					{
						Point[] trapeze1 = new Point[5];
						trapeze1[ 0 ] = new Point( i*50 - 47, 50);
						trapeze1[ 1 ] = new Point( i*50 - 47, 29);
						trapeze1[ 2 ] = new Point( i*50 - 39, 29);
						trapeze1[ 3 ] = new Point( i*50 - 43 + (int)bob4.Width, 42);
						trapeze1[ 4 ] = new Point( i*50 - 43 + (int)bob4.Width, 50);

						gUnitList.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player ].civType ].color),
							trapeze1
							);

						gUnitList.DrawPolygon(
							new Pen( Color.Black ),
							trapeze1
							);

						// m toString
						gUnitList.DrawString(
							game.playerList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].ind ].moveLeft.ToString(),
							new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Regular),
							new SolidBrush( Color.Black ), 
							i * 50 - 45,
							28
							);

						// m fraction
						if ( game.playerList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].ind ].moveLeftFraction > 0 )
						{
							gUnitList.DrawString(
								game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].ind ].moveLeftFraction.ToString() + "/3",
								new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 6 ), FontStyle.Regular),
								new SolidBrush( Color.Black ), 
								i * 50 - 38,
								34
								);

						}

						Rectangle rectEllipse = new Rectangle(
							i * 50 - 17,
							4,
							12,
							15
							);

						gUnitList.FillEllipse(
							new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player ].civType ].color),
							rectEllipse
							);

						gUnitList.DrawEllipse(
							new Pen( Color.Black ),
							rectEllipse
							);

						gUnitList.DrawString(
							unitActionAff[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].ind ].state ],
							new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 8 ), FontStyle.Regular),
							new SolidBrush( Color.Black ),
							i * 50 - 14,
							5
							);


					}
					else
					{
						Rectangle rectName = new Rectangle( 
							i * 50-47, 
							40, 
							(int)bob4.Width + 3,
							10
							);

						gUnitList.FillRectangle(
							new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player ].civType ].color),
							rectName
							);

						gUnitList.DrawRectangle(
							new Pen( Color.Black ),
							rectName
							);
							
					}

					// name unit
					gUnitList.DrawString(
						game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].typeClass.name,
						new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 7 ), FontStyle.Regular),
						new SolidBrush( Color.Black ), 
						i * 50 - 45,
						40
						);

					pictureBox3.Image = unitListBmp;

					pictureBox3.Visible = true;

					if ( game.grid[ selected.X, selected.Y ].stack.Length - 3 > unitAffSli )
						cmdUnitRight.Visible = true;
					else
						cmdUnitRight.Visible = false;


					if ( 0 == unitAffSli )
						cmdUnitLeft.Visible = false;
					else
						cmdUnitLeft.Visible = true;
				}

				#endregion

				#region populate pictureBox4

				if ( selected.owner == Form1.game.curPlayerInd )
					if ( 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ] != null &&
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 
						)
					{ // si un transport est sélectionné
						pictureBox4.Visible = true;

						Bitmap transportListBmp = new Bitmap(Form1.appPath + "\\images\\UI\\infoBack.jpg" );
						Graphics gTransportList = Graphics.FromImage( transportListBmp );

						for ( int i = 0; i < game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].transported - transportAffSli; i ++ ) // game.grid[ selected.X, selected.Y ].stack.Length // game.grid[ selected.X, selected.Y ].stack.Length // game.grid[ selected.X, selected.Y ].stack.Length
						{
							
							if ( selected.unitInTransport == i + transportAffSli ) 
							{ 
								gTransportList.DrawRectangle( 
									new Pen( Color.White ), 
									new Rectangle( 
										( i + 1 ) * 50 - 47, 
										3, 
										44, 
										45 
									) 
									); 
							} 

							gTransportList.DrawImage( 
								Statistics.units[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].type ].bmp,
								new Rectangle( 
									( i + 1 ) * 50 - 60, 
									0, 
									70, 
									50 
								), 
								0, 
								0, 
								70, 
								50, 
								GraphicsUnit.Pixel, 
								ia 
								); 

							SizeF bob4 = gUnitList.MeasureString( 
								Statistics.units[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].type ].name,
								new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 7 ), FontStyle.Regular) );

							/*if ( game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player == Form1.game.curPlayerInd )
								{ // si joueur présent*/
							Point[] trapeze1 = new Point[5];
							trapeze1[ 0 ] = new Point( ( i + 1 )*50 - 47, 50);
							trapeze1[ 1 ] = new Point( ( i + 1 )*50 - 47, 29);
							trapeze1[ 2 ] = new Point( ( i + 1 )*50 - 39, 29);
							trapeze1[ 3 ] = new Point( ( i + 1 )*50 - 43 + (int)bob4.Width, 42);
							trapeze1[ 4 ] = new Point( ( i + 1 )*50 - 43 + (int)bob4.Width, 50);
						
							gTransportList.FillPolygon(  // Statistics.units[ unitList[ game.grid[ selected.X, selected.Y ].stack[ ( i + unitAffSli ) - 1 ].player.player, game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].unit ].transport[ i + transportAffSli ] ].type ]
								new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].civType ].color),
								trapeze1
								);

							gTransportList.DrawPolygon(
								new Pen( Color.Black ),
								trapeze1
								);

							// m toString
							gTransportList.DrawString(
								game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].moveLeft.ToString(),
								new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Regular),
								new SolidBrush( Color.Black ), 
								( i + 1 ) * 50 - 45,
								28
								);

							// m fraction
							if ( game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].moveLeftFraction > 0 )
							{
								gTransportList.DrawString(
									game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].moveLeftFraction.ToString() + "/3",
									new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 6 ), FontStyle.Regular),
									new SolidBrush( Color.Black ), 
									( i + 1 ) * 50 - 38,
									34
									);

							}

							Rectangle rectEllipse = new Rectangle(
								( i + 1 ) * 50 - 17,
								4,
								12,
								15
								);

							gTransportList.FillEllipse(
								new SolidBrush( Statistics.civilizations[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].civType ].color),
								rectEllipse
								);

							gTransportList.DrawEllipse(
								new Pen( Color.Black ),
								rectEllipse
								);

							gTransportList.DrawString(
								unitActionAff[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].state ],
								new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 8 ), FontStyle.Regular),
								new SolidBrush( Color.Black ),
								( i + 1 ) * 50 - 14,
								5
								);

							// name unit
							gTransportList.DrawString(
								Statistics.units[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transport[ i + transportAffSli ] ].type ].name,
								new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 8 ), FontStyle.Regular),
								new SolidBrush( Color.Black ), 
								( i + 1 ) * 50 - 45,
								40
								);
						}

						pictureBox4.Image = transportListBmp;

						if ( game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y].stackPos - 1 ].ind ].transported - 3 > transportAffSli )
							cmdTransportUnitRight.Visible = true;
						else
							cmdTransportUnitRight.Visible = false;


						if ( 0 == transportAffSli )
							cmdTransportUnitLeft.Visible = false;
						else
							cmdTransportUnitLeft.Visible = true;

						//pictureBox4.Visible = true;
					}
					else
					{
						cmdTransportUnitLeft.Visible = false;
						cmdTransportUnitRight.Visible = false;
						pictureBox4.Visible = false;
					}
				#endregion

			}
			else
			{
				pictureBox3.Visible = false;
				cmdUnitRight.Visible = false;
				cmdUnitLeft.Visible = false;
				cmdTransportUnitLeft.Visible = false;
				cmdTransportUnitRight.Visible = false;
				pictureBox4.Visible = false;
			}
			
		}
		#endregion

#region build cmds

	#region general build

		#region civic
		private void buildIrrigation( byte player, int unit )
		{
			caseImprovement.addCaseImps( 
				game.playerList[ player ].unitList[ unit ].pos,
				1,
				(byte)civicImprovementChoice.irrigation,
				player,
				unit
				);	
			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.buildingIrrigation;
		}

		private void buildMine ( byte player, int unit  )
		{
			caseImprovement.addCaseImps( 
				game.playerList[ player ].unitList[ unit ].pos,
				1,
				(byte)civicImprovementChoice.mine,
				player,
				unit
				);	
			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.buildingMine;
		}

		private void buildRoad ( byte player, int unit  )
		{
			if ( game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].roadLevel == 0 )
			{
				caseImprovement.addCaseImps( 
					game.playerList[ player ].unitList[ unit ].pos,
					0,
					1, // road
					player,
					unit
					);	
			}
			else if ( game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].roadLevel == 1 )
			{
				caseImprovement.addCaseImps( 
					game.playerList[ player ].unitList[ unit ].pos,
					0,
					2, // railroad
					player,
					unit
					);	
			}

			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.buildingRoad;
		}
		#endregion

		#region military
		private void buildAirport ( byte player, int unit  )
		{
			caseImprovement.addCaseImps( 
				game.playerList[ player ].unitList[ unit ].pos,
				2,
				(byte)enums.militaryImprovement.airport,
				player,
				unit
				);	
			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.buildingAirport;
		}

		private void buildFort ( byte player, int unit  )
		{
			caseImprovement.addCaseImps( 
				game.playerList[ player ].unitList[ unit ].pos,
				2,
				(byte)enums.militaryImprovement.fort,
				player,
				unit
				);	
			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.buildingFort;
		}

		private void buildMineFld ( byte player, int unit  )
		{
			caseImprovement.addCaseImps( 
				game.playerList[ player ].unitList[ unit ].pos,
				2,
				(byte)enums.militaryImprovement.mineField,
				player,
				unit
				);	

			game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.buildingMineFld;
		}
		#endregion

	#endregion

	#region mIs

		#region civic
		private void mIIrrigate_Click(object sender, System.EventArgs e)
		{ // Irrigate

			if ( game.radius.isNextToIrrigation( selected.X, selected.Y ) && (game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0) )
			{
				buildIrrigation( Form1.game.curPlayerInd, selected.unit );
				selected.state = 0;
				showNextUnit( selected.owner, selected.unit );
				showInfo();
				enableMenus();
				DrawMap();
			}
		}

		private void mIMine_Click(object sender, System.EventArgs e)
		{ // mine

			if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 )
			{
				/*game.grid[ selected.X, selected.Y ].civicImprovement = (byte)civicImprovementChoice.mine;
				game.playerList[ selected.owner ].unitList[  selected.unit ].moveLeft = 0;*/

				buildMine ( Form1.game.curPlayerInd, selected.unit );
				selected.state = 0;
				showNextUnit( selected.owner, selected.unit );
				showInfo();
				enableMenus();
				DrawMap();
			}
		}

		private void mIRoad_Click(object sender, System.EventArgs e)
		{ // road
			if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 )
			{ 
				/*game.grid[ selected.X, selected.Y ].roadLevel = 1;
				game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft = 0;*/
				buildRoad ( Form1.game.curPlayerInd, selected.unit );
				selected.state = 0;
				showNextUnit( selected.owner, selected.unit );

				showInfo();
				enableMenus();
				DrawMap();
			}			
		}
		#endregion
		
		#region military
		private void mIMineFld_Click(object sender, EventArgs e)
		{
			if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 )
			{ 
				buildMineFld ( Form1.game.curPlayerInd, selected.unit );
				selected.state = 0;
				showNextUnit( selected.owner, selected.unit );

				showInfo();
				enableMenus();
				DrawMap();
			}	
		}

		private void mIAirport_Click(object sender, EventArgs e)
		{
			if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 )
			{ 
				buildAirport ( Form1.game.curPlayerInd, selected.unit );
				selected.state = 0;
				showNextUnit( selected.owner, selected.unit );

				showInfo();
				enableMenus();
				DrawMap();
			}	
		}

		private void mIFort_Click(object sender, EventArgs e)
		{
			if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 )
			{ 
				buildFort ( Form1.game.curPlayerInd, selected.unit );
				selected.state = 0;
				showNextUnit( selected.owner, selected.unit );

				showInfo();
				enableMenus();
				DrawMap();
			}	
		}
		#endregion

	#endregion

#endregion

#region end turn # # # 

	#region end turn cmd
		private void menuItem17_Click(object sender, System.EventArgs e)
		{  // end turn
			bool found = false;

			for ( int i = 1; i <= game.playerList[ Form1.game.curPlayerInd ].unitNumber; i ++ )
				if ( 
					game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.idle && 
					( 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].moveLeft > 0 || 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].moveLeftFraction > 0 
					) &&
					!game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].automated
					)
				{
					found = true;
					break;
				}

			if ( found )
			{
				if ( MessageBox.Show( "You still have at least one unit which has no orders.  Are you sure you want to end this turn?", "Ending turn", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1 ) == DialogResult.Yes )
					found = false;
				else
					showNextUnitNow( Form1.game.curPlayerInd, 0 );
			}

			if ( !found ) 
			{ 
				endTurn();
			} 
		}
	#endregion


	#region endTurn itself
		private void endTurn()
		{
			guiEnabled = false; 
			wC.show = true; 
			Application.DoEvents(); 
			this.Update(); 

			if ( 
				options.autosave && 
				game.curTurn % 5 == 4 
				)
				game.autosave();

			bool interruptEndOfTurn = false;
			for (int unit = 1; unit <= game.playerList[ Form1.game.curPlayerInd ].unitNumber; unit ++ ) 
				if ( 
					game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].automated == true &&
					(
					game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].moveLeft > 0 ||
					game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].moveLeftFraction > 0 
					)
					) 
					if ( 
						Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].type ].speciality == enums.speciality.builder &&
						game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].state != (byte)Form1.unitState.goTo
						)
					{
						automatedSettler( Form1.game.curPlayerInd, ref unit ); 
					}
					else
					{
						if ( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].pos.X == game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].dest.X &&
							game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].pos.Y == game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].dest.Y
							)
						{
							game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].state = (byte)Form1.unitState.idle;
							game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].automated = false;
						}
						else
						{
							#region not there yet

							Point[] way = game.radius.findWayTo( 
								game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].X, 
								game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].Y, 
								game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].dest.X, 
								game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].dest.Y, 
								Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].type ].terrain, 
								curAi, 
								true 
								); 
							int m = 0;

							if ( way[ 0 ].X != -1 )
								while ( 
									game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].state != (byte)Form1.unitState.dead && 
									( 
									game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].moveLeft > 0 || 
									game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].moveLeftFraction > 0 
									) 
									) 
								{ 
									if ( 
										game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].X == way[ way.Length - 1 ].X && 
										game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].Y == way[ way.Length - 1 ].Y 
										)
									{
										interruptEndOfTurn = true;
										unit --; 
										break; 
									}
									else if ( game.radius.caseOccupiedByRelationType( way[ m ].X, way[ m ].Y, curAi, true, false, false, false, false, false ) )
									{
										/*
										int ennemy = ai.strongestEnnemy( way[ m ].X, way[ m ].Y, curAi ); 
										attackUnit( Form1.game.curPlayerInd, unit, game.grid[ way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].player.player, game.grid[  way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].unit );
										*/	
										interruptEndOfTurn = true;
										game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].automated = false;
										game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].state = (byte)Form1.unitState.idle;
									//	unit --; 
										break; 
									}
									else
									{
										UnitMove( 
											Form1.game.curPlayerInd, 
											unit, 
											game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].X, 
											game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].Y,
											way[ m ].X,
											way[ m ].Y
											);
										m ++;
									}

								}
							#endregion
						}
					}
	
			if ( !interruptEndOfTurn )
			{
				for ( curAi = 1; curAi < Form1.game.playerList.Length; curAi ++ )
					if ( !game.playerList[ curAi ].dead )
					{
					//	int Form1.game.playerList[ curAi ].totalTrade =	Form1.game.playerList[ curAi ].totalTrade;

						#region techno + money
						if ( game.playerList[ curAi ].cityNumber > 0 )
						{
							game.playerList[ curAi ].technos[ game.playerList[ curAi ].currentResearch ].pntDiscovered += 1 + Form1.game.playerList[ curAi ].totalTrade * game.playerList[ curAi ].preferences.science /100; //getPFT1.getNationScience( curAi );
							game.playerList[ curAi ].money += (int)( Form1.game.playerList[ curAi ].totalTrade * game.playerList[ curAi ].preferences.reserve /100 );
						}

						if ( game.playerList[ curAi ].technos[ game.playerList[ curAi ].currentResearch ].pntDiscovered >= Statistics.technologies[ game.playerList[ curAi ].currentResearch ].cost )
						{ // nouvelle tecnologie
							game.playerList[ curAi ].aquireTechno(); 	//	nego.aquireTechno( curAi, game.playerList[ curAi ].currentResearch );
							/*game.playerList[ curAi ].technos[ game.playerList[ curAi ].currentResearch ].researched = true ;

							ai Ai = new ai();
							byte[] technos = Ai.returnDisponibleTechnologies( Form1.game.curPlayerInd );
							if ( technos.Length > 0 )
							{
								byte nextTechnos = Ai.randomTechnology( curAi );
								game.playerList[ curAi ].currentResearch = nextTechnos;
							}
							else
							{
								game.playerList[ curAi ].currentResearch = 0;
								game.playerList[ curAi ].technos[ game.playerList[ curAi ].currentResearch ].pntDiscovered = 0;
							}*/
							//	game.playerList[ curAi ].technos[ nextTechnos ] += Statistics.technologies[ nextTechnos ].cost;
						} 
						#endregion

						#region units
						for (int i = 1; i <= game.playerList[ curAi ].unitNumber; i++) // 
							if ( game.playerList[ curAi ].unitList[ i ].state != (byte)Form1.unitState.dead ) 
							{
								if ( 
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.buildingRoad ||
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.buildingIrrigation ||
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.buildingMine ||
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.buildingFort ||
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.buildingAirport ||
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.buildingMineFld
									)
								{
									caseImprovement.advImprovement( curAi, i );
								}
								else if (
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.goToBuildCity || 
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.goToImprove ||
									game.playerList[ curAi ].unitList[ i ].state == (byte)Form1.unitState.moving
									)
								{
								}
								else if (
									game.playerList[ curAi ].unitList[ i ].state != (byte)Form1.unitState.fortified && 
									game.playerList[ curAi ].unitList[ i ].state != (byte)Form1.unitState.sleep 
									)
								{
								//	game.playerList[ curAi ].unitList[ i ].state = (byte)Form1.unitState.idle;
								}

								game.playerList[ curAi ].unitList[ i ].moveLeft = (sbyte)Statistics.units[ game.playerList[ curAi ].unitList[ i ].type ].move;
								game.playerList[ curAi ].unitList[ i ].moveLeftFraction = 0;

							}
						#endregion

						#region cities
						for (int i = 1; i <= game.playerList[ curAi ].cityNumber; i++) //
							if( game.playerList[ curAi ].cityList[ i ].state == (byte)enums.cityState.ok )
							{
								#region
								// food
								game.playerList[ curAi ].cityList[ i ].foodReserve += (int)(Form1.game.playerList[ curAi ].cityList[ i ].food - game.playerList[ curAi ].cityList[ i ].population - game.playerList[ curAi ].cityList[ i ].slaves.total * .5);

								if ( game.playerList[ curAi ].cityList[ i ].foodReserve > game.playerList[ curAi ].cityList[ i ].population * foodPerPop )
								{
									game.playerList[ curAi ].cityList[ i ].foodReserve -= game.playerList[ curAi ].cityList[ i ].population * foodPerPop;
									game.playerList[ curAi ].cityList[ i ].population ++;
									labor.autoAddLabor( curAi, i );

									game.frontier.setFrontiers();
								}
								else if ( game.playerList[ curAi ].cityList[ i ].foodReserve < 0 )
								{
									labor.addAllLabor( curAi, i );
									if ( game.playerList[ curAi ].cityList[ i ].population > 1 )
									{
										labor.autoRemoveLabor( curAi, i, true );

										game.playerList[ curAi ].cityList[ i ].population --;
									}
									else
									{
										//	deleteCity( curAi, i );
										game.playerList[ curAi ].cityList[ i ].kill();

										continue;
									}

									game.frontier.setFrontiers();
								}

						/*		if ( game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].type == 0 )
								{
								//	ai Ai = new ai();
									ai.chooseConstruction( curAi, i );
								}*/

								if ( game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].type == 3 )
								{ // wealth
									game.playerList[ curAi ].money += Form1.game.playerList[ curAi ].cityList[ i ].production;
									ai.chooseConstruction( curAi, i );
								}
								else
								{ // construction
									// construction points
									game.playerList[ curAi ].cityList[ i ].construction.points += Form1.game.playerList[ curAi ].cityList[ i ].production;

									// units
									int costLeft = game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].cost - game.playerList[ curAi ].cityList[ i ].construction.points;

									if ( costLeft <= 0 )
									{
										game.playerList[ curAi ].cityList[ i ].buildConstruction( this );
									}
								}

								labor.addAllLabor( curAi, i );
								#endregion
							}
							else if ( game.playerList[ curAi ].cityList[ i ].state == (byte)enums.cityState.evacuating )
							{
								#region
								for ( int k = 0; k < 2; k ++ )
								{
									game.playerList[ curAi ].cityList[ i ].population --;

									if ( game.playerList[ curAi ].cityList[ i ].population == 0 )
									{
										game.grid[ game.playerList[ curAi ].cityList[ i ].X, game.playerList[ curAi ].cityList[ i ].Y ].city = 0;
										game.playerList[ curAi ].cityList[ i ].state = (byte)enums.cityState.dead;
									//	createUnit( game.playerList[ curAi ].cityList[ i ].X, game.playerList[ curAi ].cityList[ i ].Y, Form1.game.curPlayerInd, (byte)Form1.unitType.colon );
										game.playerList[ Form1.game.curPlayerInd ].createUnit( game.playerList[ curAi ].cityList[ i ].X, game.playerList[ curAi ].cityList[ i ].Y, (byte)Form1.unitType.colon ); //createUnit( startPos.X, startPos.Y, (byte)unitType.colon );
										break;
									}
								}
								#endregion
							}
						#endregion

						#region negos
						for ( int i = 0; i < game.playerList.Length; i ++ )
						{
							//if (  )
						}
						#endregion

						defaultIni( curAi, Form1.game.playerList[ curAi ].totalTrade ); 

						#region declare war and peace

						int totAllies = 0, totWars = 0, totIsWinning = 0, totTechno = 0, totPlayers = 0, playerTechno = count.technoNumber( curAi );
						int[] allIsWinning = new int[ game.playerList.Length ];

						for ( int i = 0; i < game.playerList.Length; i ++ )
							if ( 
								i != curAi && 
								!game.playerList[ i ].dead && 
								game.playerList[ curAi ].foreignRelation[ i ].madeContact 
								) 
							{ 
								totPlayers ++; 
								totTechno += count.technoNumber( (byte)i ); 
								int twiw = ai.whoIsWinning( curAi, (byte)i ); 

								allIsWinning[ i ] = twiw; 
								allIsWinning[ i ] -= 10; 

								if ( game.playerList[ curAi ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war ) 
								{ 
									totWars ++; 
									totIsWinning += twiw; 
									totIsWinning -= 10; 
								} 
								else if ( 
									game.playerList[ curAi ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.alliance || 
									game.playerList[ curAi ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.Protector 
									) 
								{ 
									totAllies ++; 
								} 
							} 

						for ( int i = 0; i < game.playerList.Length; i ++ )
							if ( 
								i != curAi && 
								!game.playerList[ i ].dead && 
								game.playerList[ curAi ].foreignRelation[ i ].madeContact 
								)
							{
								if ( game.playerList[ curAi ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
								{ 
									if ( i != Form1.game.curPlayerInd )
										if ( 
											allIsWinning[ i ] < - 2 && 
											totIsWinning < - 2 * totWars - totAllies * 2 
											) 
											if ( aiPolitics.acceptPeaceOffer( curAi, (byte)i ) ) 
												aiPolitics.declarePeace( curAi, (byte)i ); 
								} 
								else if ( game.playerList[ curAi ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.ceaseFire ) 
								{ 
								}
								else if ( game.playerList[ curAi ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.peace )
								{
									if ( 
										( // reason to stay in war 
										count.technoNumber( (byte)i ) > count.technoNumber( curAi ) + 1 || 
										allIsWinning[ curAi ] > 9 
										) && 
										( // not too much wars 
										totWars < totAllies || 
										totWars < totPlayers / 3 
										) &&
										ai.whoIsWinning( curAi, (byte)i ) > 10
									//	game.playerList[ curAi ].foreignRelation[ i ].quality < 20
										) 
										aiPolitics.declareWar( curAi, (byte)i );
								} 
							}

						#endregion

						aiPref.setPrefs( curAi, Form1.game.playerList[ curAi ].totalTrade ); 

						for ( byte p = 0; p < game.playerList.Length; p ++ )
							if ( 
								p != curAi && 
								!game.playerList[ p ].dead && 
								game.playerList[ curAi ].foreignRelation[ p ].madeContact 
								)
								aiNego.initNego( curAi, p );

						#region iaUnit
						for ( curUnit = 1; curUnit <= game.playerList[ curAi ].unitNumber; curUnit ++ )
						{ // verification du nombre pour l unité
							if ( game.playerList[ curAi ].unitList[ curUnit ].state != (byte)Form1.unitState.dead )
								if ( 
									game.playerList[ curAi ].unitList[ curUnit ].moveLeft > 0 ||
									game.playerList[ curAi ].unitList[ curUnit ].moveLeftFraction > 0 
									)
								{ // si l unité est vivante
							
									#region settler
									if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.builder )
									{ // si c est un settler
										if ( game.playerList[ curAi ].unitList[ curUnit ].state != (byte)Form1.unitState.inTransport )
											automatedSettler( curAi, ref curUnit );
									}
									#endregion 

									#region canons
									else if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bombard ) 
									{ 

										#region go to
										if ( 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToAttack || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToBoat || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToDefend || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToFrontier
											)
										{
											if ( 
												game.playerList[ curAi ].unitList[ curUnit ].X == game.playerList[ curAi ].unitList[ curUnit ].dest.X &&
												game.playerList[ curAi ].unitList[ curUnit ].Y == game.playerList[ curAi ].unitList[ curUnit ].dest.Y
												)
											{
												if ( game.playerList[ curAi ].unitList[ curUnit ].state ==		(byte)Form1.unitState.goToAttack ) 
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.idle;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.goToBoat ) 
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.waitingForBoat;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.goToDefend ) 
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.idle;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.goToFrontier )
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.idle;

												curUnit --; 
											} 
											else 
											{ 
												#region not there yet
												Point[] way = game.radius.findWayTo( 
													game.playerList[ curAi ].unitList[ curUnit ].X, 
													game.playerList[ curAi ].unitList[ curUnit ].Y, 
													game.playerList[ curAi ].unitList[ curUnit ].dest.X, 
													game.playerList[ curAi ].unitList[ curUnit ].dest.Y, 
													Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].terrain, 
													curAi, 
													true 
													); 
												int m = 0;

												if ( way[ 0 ].X != -1 )
													while ( 
														game.playerList[ curAi ].unitList[ curUnit ].state != (byte)Form1.unitState.dead && 
														( 
														game.playerList[ curAi ].unitList[ curUnit ].moveLeft > 0 || 
														game.playerList[ curAi ].unitList[ curUnit ].moveLeftFraction > 0 
														) 
														) 
													{ 
														if ( 
															game.playerList[ curAi ].unitList[ curUnit ].X == way[ way.Length - 1 ].X && 
															game.playerList[ curAi ].unitList[ curUnit ].Y == way[ way.Length - 1 ].Y 
															)
														{
															curUnit --; 
															break; 
														}
															
														if ( game.radius.caseOccupiedByRelationType( way[ m ].X, way[ m ].Y, curAi, true, false, false, false, false, false ) )
														{
															int ennemy = ai.strongestEnnemy( way[ m ].X, way[ m ].Y, curAi ); 
															attackUnit( curAi, curUnit, game.grid[ way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].player.player, game.grid[  way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].ind );
														
															curUnit --; 
															break; 
														}
														else
														{
															UnitMove( 
																curAi, 
																curUnit, 
																game.playerList[ curAi ].unitList[ curUnit ].X, 
																game.playerList[ curAi ].unitList[ curUnit ].Y,
																way[ m ].X,
																way[ m ].Y
																);
															m  ++;
														}

													}
												#endregion
											}
										}
										#endregion

										else //if //( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToAttack )
										{
											Point caseToBombard = aiWarBomber.findCaseToBombard( curAi, curUnit ); 

											if ( caseToBombard.X != -1 ) 
											{
												bombardCase( curAi, curUnit, caseToBombard.X, caseToBombard.Y ); 
												curUnit --;
											}
											else 
											{
												Point dest = aiWarCanon.whereToMoveCanon( curAi, curUnit );

												if ( dest.X != -1 )
												{
													game.playerList[ curAi ].unitList[ curUnit ].dest = dest;
													game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.goToAttack; 
													curUnit --;
												}
											}
										}
											
									} 
									#endregion 

									#region planes
									else if ( 
										Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bomber ||
										Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.fighter 
										) 
									{ 
										if ( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.planeRebase )
										{
											if ( 
												(
													game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].city > 0 ||
													game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].militaryImprovement == (byte)enums.militaryImprovement.airport 
												) &&
												game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].territory > 0 &&
												(
													game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].territory - 1 == curAi ||
													game.playerList[ curAi ].foreignRelation[ game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
													game.playerList[ curAi ].foreignRelation[ game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
													game.playerList[ curAi ].foreignRelation[ game.grid[ game.playerList[ curAi ].unitList[ curUnit ].dest.X, game.playerList[ curAi ].unitList[ curUnit ].dest.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
												) &&
												(
													(
														Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bomber &&
														aiWarBomber.findCaseToBombard( curAi, Form1.game.playerList[ curAi ].unitList[ curUnit ].dest, Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].range ).X != -1 
													) ||
													//(
													Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.fighter //&&
													//	aiWarBomber.findCaseToBombard( curAi, Form1.game.playerList[ curAi ].unitList[ curUnit ].dest, Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].range ).X != -1 
												//	)
												)
												)
											{
												Point[] way = aiPlanes.findWayToBase( 
													curAi, 
													Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].range,
													Form1.game.playerList[ curAi ].unitList[ curUnit ].pos,
													Form1.game.playerList[ curAi ].unitList[ curUnit ].dest
													);

												if ( way[ 0 ].X >= 0 )
												{
													for ( 
														int step = 0; 
														step < way.Length &&
														(
														Form1.game.playerList[ curAi ].unitList[ curUnit ].moveLeft > 0 ||
														Form1.game.playerList[ curAi ].unitList[ curUnit ].moveLeftFraction > 0
														); 
														step ++ 
														)
														UnitMove( curAi, curUnit, way[ step ] );

													if ( Form1.game.playerList[ curAi ].unitList[ curUnit ].pos == Form1.game.playerList[ curAi ].unitList[ curUnit ].dest )
													{
														Form1.game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.idle;
														curUnit --;
													}
												}
												else
												{
													Form1.game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.idle;
													curUnit --;
												}
											}
											else
											{
												Form1.game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.idle;
												curUnit --;
											}
										}
										else if ( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.idle )
										{
											Point pnt = aiWarBomber.findCaseToBombard( curAi, curUnit );

											if ( pnt.X == -1 )
											{
												structures.memory mem = game.playerList[ curAi ].memory.findInTheLast( 
													(byte)enums.playerMemory.rebaseBomber,
													10
													);

												bool foundDest = false;
												if ( mem.ind.Length == 0 ) // not found
												{
												}
												else
												{
													Point[] way = aiPlanes.findWayToBase( 
														curAi, 
														Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].range,
														Form1.game.playerList[ curAi ].unitList[ curUnit ].pos,
														new Point( mem.ind[ 0 ], mem.ind[ 1 ] )
														);

													if ( way[ 0 ].X >= 0 )
													{ // valid
														Form1.game.playerList[ curAi ].unitList[ curUnit ].dest = new Point( mem.ind[ 0 ], mem.ind[ 1 ] );
														Form1.game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.planeRebase;
														curUnit --;
														foundDest = true;
													}
												}

												if ( !foundDest )
												{
													if ( 
														(
															Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bomber && 
															ai.isInWar( curAi ) &&
															game.playerList[ curAi ].memory.findInTheLast( (byte)enums.playerMemory.noWhereToRebaseBomber, 1 ).ind.Length == 0
														) ||
														(
															Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.fighter && 
															game.playerList[ curAi ].memory.findInTheLast( (byte)enums.playerMemory.noWhereToRebaseFighter, 1 ).ind.Length == 0
														) 
														)
													{
														Point dest;
														if ( Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bomber )
															dest = aiWarBomber.findWhereToRebase( curAi, curUnit );
														else
															dest = aiWarFighter.findWhereToRebase( curAi, curUnit );

														if ( dest.X == -1 )
														{
															if ( Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bomber )
															{
																game.playerList[ curAi ].memory.findAndDeleteLatest( (byte)enums.playerMemory.noWhereToRebaseBomber );
																game.playerList[ curAi ].memory.add( 
																	(byte)enums.playerMemory.noWhereToRebaseBomber,
																	new int[] { game.grid[ Form1.game.playerList[ curAi ].unitList[ curUnit ].X, Form1.game.playerList[ curAi ].unitList[ curUnit ].Y ].continent }
																	);
															}
															else
															{
																game.playerList[ curAi ].memory.findAndDeleteLatest( (byte)enums.playerMemory.noWhereToRebaseFighter );
																game.playerList[ curAi ].memory.add( 
																	(byte)enums.playerMemory.noWhereToRebaseFighter,
																	new int[] { game.grid[ Form1.game.playerList[ curAi ].unitList[ curUnit ].X, Form1.game.playerList[ curAi ].unitList[ curUnit ].Y ].continent }
																	);
															}
														}
														else
														{ // valid
															if ( Statistics.units[ Form1.game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.bomber )
															{
																game.playerList[ curAi ].memory.findAndDeleteLatest( (byte)enums.playerMemory.rebaseBomber );
																game.playerList[ curAi ].memory.add( 
																	(byte)enums.playerMemory.rebaseBomber,
																	new int[] { dest.X, dest.Y }
																	);
															}
															else
															{
																game.playerList[ curAi ].memory.findAndDeleteLatest( (byte)enums.playerMemory.rebaseFighter );
																game.playerList[ curAi ].memory.add( 
																	(byte)enums.playerMemory.rebaseFighter,
																	new int[] { dest.X, dest.Y }
																	);
															}
															
															Form1.game.playerList[ curAi ].unitList[ curUnit ].dest = dest;
															Form1.game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.planeRebase;
															curUnit--;
														}
													}
												}
											}
											else
											{ // bombard it
												bombardCase( curAi, curUnit, pnt.X, pnt.Y );
												Form1.game.playerList[ curAi ].unitList[ curUnit ].moveLeft--;
												curUnit --;
											}
										}
									} 
									#endregion 
							
									#region aaGun
									else if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.aaGun )
									{
									}
									#endregion 

									#region nuke or missile
									else if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.cruiseMissile ) 
									{ 
									}

									else if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.nukeBomb ) 
									{ 
									}

									else if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.nukeIC ) 
									{ 
									}

									else if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].speciality == enums.speciality.nukeMissile ) 
									{ 
									}
										#endregion 

									#region unitée de guerre 
									else // if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit].type ].attack > 0 )
									{ // si c'est une unitée de guerre 
										ai Ai = new ai(); 
										int m = 0; 

										#region go to
										if ( 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToAttack || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToBoat || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToDefend || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.goToFrontier || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.transportGoToDrop || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.transportGoToPickUp
											)
										{
											if ( 
												game.playerList[ curAi ].unitList[ curUnit ].X == game.playerList[ curAi ].unitList[ curUnit ].dest.X &&
												game.playerList[ curAi ].unitList[ curUnit ].Y == game.playerList[ curAi ].unitList[ curUnit ].dest.Y
												)
											{
												if ( game.playerList[ curAi ].unitList[ curUnit ].state ==		(byte)Form1.unitState.goToAttack ) 
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.idle;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.goToBoat ) 
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.waitingForBoat;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.goToDefend ) 
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.idle;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.goToFrontier )
													game.playerList[ curAi ].unitList[ curUnit ].state =			(byte)Form1.unitState.idle;
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.transportGoToDrop )
												{ 
													#region 
													bool unitLeft = false; 
													Point disembarkTo; 

													if ( game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].city > 0 )
													{ 
														disembarkTo = new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y );
													} 
													else 
													{ 
														int[] conts = transport.nearWhatCont( curAi, curUnit ); 
														
														int  totSettlers = 0;
														for ( int i = 0; i < game.playerList[ curAi ].unitList[ curUnit ].transported; i ++ ) 
															if ( Statistics.units[ game.playerList[ curAi ].unitList[ game.playerList[ curAi ].unitList[ curUnit ].transport[ i ] ].type ].speciality == enums.speciality.builder ) 
																totSettlers ++; 
	 
														Point wtdmu = new Point( -1, -1 ); 
														if ( totSettlers > 0 ) 
														{ 
															for ( int cont = 0; cont < conts.Length; cont ++ )
															{
																wtdmu = ai.findCitySite( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y, curAi, conts[ cont ] );
																
																if ( wtdmu.X != -1 )
																	if ( game.radius.isNextTo( wtdmu, game.playerList[ curAi ].unitList[ curUnit ].pos ) ) // ok
																		break;
																	else
																		wtdmu.X = -1;
															}
															
															if ( wtdmu.X == -1 )
																for ( int cont = 0; cont < conts.Length; cont ++ )
																{
																	wtdmu = ai.findCitySite( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y, curAi, conts[ cont ] );
																
																	if ( wtdmu.X != -1 )
																		break;
																}
														} 
														else
															wtdmu = aiTransport.whereToDisembarkMilitaryUnits( curAi, curUnit );
														
														if ( wtdmu.X != -1 ) 
														{ 
															if ( game.radius.isNextTo( wtdmu, game.playerList[ curAi ].unitList[ curUnit ].pos ) )
															{
																disembarkTo = wtdmu;
															}
															else
															{
																structures.pntWithDist[] pwd = game.radius.findNearestCaseTo( 
																	game.playerList[ curAi ].unitList[ curUnit ].X, 
																	game.playerList[ curAi ].unitList[ curUnit ].Y, 
																	wtdmu.X, 
																	wtdmu.Y, 
																	1, 
																	curAi, 
																	false, 
																	true 
																	); 

																disembarkTo = new Point( pwd[ 0 ].X, pwd[ 0 ].Y );
															}
														}
														else
															disembarkTo = new Point( -1, -1 );
													}

													if ( disembarkTo.X != -1 ) 
													{ 
														for ( int i = 0; i < game.playerList[ curAi ].unitList[ curUnit ].transported; i ++ )
															if ( 
																game.playerList[ curAi ].unitList[ game.playerList[ curAi ].unitList[ curUnit ].transport[ i ] ].moveLeft > 0 ||
																game.playerList[ curAi ].unitList[ game.playerList[ curAi ].unitList[ curUnit ].transport[ i ] ].moveLeftFraction > 0 
																) 
																move.disembarkUnit( curAi, game.playerList[ curAi ].unitList[ curUnit ].transport[ i ], curUnit, disembarkTo );
															else 
																unitLeft = true; 
													}
													/*else
													{
													}*/

													if ( !unitLeft )
														game.playerList[ curAi ].unitList[ curUnit ].state =	(byte)Form1.unitState.idle;

													curUnit = 1;
													#endregion
												} 
												else if ( game.playerList[ curAi ].unitList[ curUnit ].state ==	(byte)Form1.unitState.transportGoToPickUp )
												{
													game.playerList[ curAi ].unitList[ curUnit ].state =	(byte)Form1.unitState.transportWaitingToFill;
													
													#region //ed
													/*if ( game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].city > 0 )
													{ // if is on a city
														for ( int i = 1; i <= game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack.Length; i ++ )
															if ( game.playerList[ curAi ].unitList[ curUnit ].transported < Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].transport )
															{ 
																if ( 
																	game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].player.player == curAi && 
																	game.playerList[ curAi ].unitList[ game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].unit ].state == (byte)Form1.unitState.waitingForBoat 
																	) 
																	move.UnitMove2Transport( curAi, game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].unit, curUnit );
															} 
															else 
																break; 
													}

													// look around even if it is in a city
													Point[] sqr = game.radius.returnEmptySquare( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y, 1 );

													for ( int k = 0; k < sqr.Length; k ++ ) 
													{ 
														for ( int i = 1; i <= game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length; i ++ )
															if ( game.playerList[ curAi ].unitList[ curUnit ].transported < Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].transport )
															{ 
																if ( 
																	game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].player.player == curAi &&
																	game.playerList[ curAi ].unitList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].unit ].state == (byte)Form1.unitState.waitingForBoat &&
																	( 
																	game.playerList[ curAi ].unitList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].unit ].moveLeft > 0 ||
																	game.playerList[ curAi ].unitList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].unit ].moveLeftFraction > 0
																	) 
																	) 
																	move.UnitMove2Transport( curAi, game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].unit, curUnit );
															} 
															else 
																break; 
													} 

													if ( game.playerList[ curAi ].unitList[ curUnit ].transported == Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].transport )
													{
														game.playerList[ curAi ].unitList[ curUnit ].state =	(byte)Form1.unitState.transportGoToDrop;

														int totMilitaryUnits = 0; totSettlers = 0;
														for ( int i = 0; i < game.playerList[ curAi ].unitList[ curUnit ].transported; i ++ ) 
															if ( Statistics.units[ game.playerList[ curAi ].unitList[ game.playerList[ curAi ].unitList[ curUnit ].transport[ i ] ].type ].speciality == enums.speciality.builder ) 
																totSettlers ++; 
															else 
																totMilitaryUnits ++; 
	 
														Point wtdmu;
														if ( totSettlers > 0 ) 
															wtdmu = aiTransport.whereToDisembarkMilitaryUnits( curAi, curUnit );
														else
															wtdmu = ai.findCitySiteWorldWide( game.playerList[ curAi ].unitList[ curUnit ].pos, curAi );

														game.playerList[ curAi ].unitList[ curUnit ].dest = transport.destOnWater( Form1.game.curPlayerInd, curUnit, wtdmu );
													}
													else
													{
														game.playerList[ curAi ].unitList[ curUnit ].state =	(byte)Form1.unitState.transportWaitingToFill;
													}*/
													#endregion
												} 

												curUnit --; 
											} 
											else 
											{ 
												#region not there yet
												Point[] way = game.radius.findWayTo( 
													game.playerList[ curAi ].unitList[ curUnit ].X, 
													game.playerList[ curAi ].unitList[ curUnit ].Y, 
													game.playerList[ curAi ].unitList[ curUnit ].dest.X, 
													game.playerList[ curAi ].unitList[ curUnit ].dest.Y, 
													Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].terrain, 
													curAi, 
													true 
													); 

												if ( way[ 0 ].X != -1 )
													while ( 
														game.playerList[ curAi ].unitList[ curUnit ].state != (byte)Form1.unitState.dead && 
														( 
														game.playerList[ curAi ].unitList[ curUnit ].moveLeft > 0 || 
														game.playerList[ curAi ].unitList[ curUnit ].moveLeftFraction > 0 
														) 
														) 
													{ 
														if ( 
															game.playerList[ curAi ].unitList[ curUnit ].X == way[ way.Length - 1 ].X && 
															game.playerList[ curAi ].unitList[ curUnit ].Y == way[ way.Length - 1 ].Y 
															)
														{
															curUnit --; 
															break; 
														}
															
														if ( game.radius.caseOccupiedByRelationType( way[ m ].X, way[ m ].Y, curAi, true, false, false, false, false, false ) )
														{
															int ennemy = ai.strongestEnnemy( way[ m ].X, way[ m ].Y, curAi ); 
															attackUnit( curAi, curUnit, game.grid[ way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].player.player, game.grid[  way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].ind );
														
															curUnit --; 
															break; 
														}
														else
														{
															UnitMove( 
																curAi, 
																curUnit, 
																game.playerList[ curAi ].unitList[ curUnit ].X, 
																game.playerList[ curAi ].unitList[ curUnit ].Y,
																way[ m ].X,
																way[ m ].Y
																);
															m ++;
														}

													}
												#endregion
											}
										}
											#endregion

										#region waitingForBoat
										else if ( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.waitingForBoat )
										{
										/*	if ( transport.spaceInATransport( curAi, game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ) )
											{
												move.UnitMove2Transport( curAi, curUnit, transport.randomTransportWithFreeSpace( curAi, game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ) );
											}
											else 
											{
												Point[] sqr = game.radius.returnEmptySquare( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y, 1 );

												for ( int k = 0; k < sqr.Length; k ++ )
													if ( transport.spaceInATransport( curAi, sqr[ k ].X, sqr[ k ].Y ) )
														move.UnitMove2Transport( curAi, curUnit, transport.randomTransportWithFreeSpace( curAi, sqr[ k ].X, sqr[ k ].Y ) );
											}*/
										}
											#endregion
		
										#region inTransport
										else if ( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.inTransport ) 
										{ 
										} 
										#endregion
		
										#region transportWaitingToFill
										else if ( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.transportWaitingToFill ) 
										{ 
											if ( game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].city > 0 )
												for ( // if is on a city
													int i = 1; 
													i <= game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack.Length &&
													game.playerList[ curAi ].unitList[ curUnit ].transported < Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].transport; 
													i ++ 
													) 
														if ( 
															game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].player.player == curAi && 
															(
															game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].state == (byte)Form1.unitState.waitingForBoat || 
															game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].state == (byte)Form1.unitState.waitingForBoat // warning
															)
															) 
															move.UnitMove2Transport( 
																curAi, 
																game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].stack[ i - 1 ].ind, 
																curUnit 
																); 

											// look around even if it is in a city
											Point[] sqr = game.radius.returnEmptySquare( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y, 1 );

											for ( int k = 0; k < sqr.Length; k ++ ) 
												for ( int i = 1; 
													i <= game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length && 
													game.playerList[ curAi ].unitList[ curUnit ].transported < Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].transport; 
													i ++ )
														if ( 
															game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].player.player == curAi &&
															game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].state == (byte)Form1.unitState.waitingForBoat &&
															( 
															game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].moveLeft > 0 ||
															game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].moveLeftFraction > 0
															) 
															) 
															move.UnitMove2Transport( 
																curAi, 
																game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i - 1 ].ind, 
																curUnit 
																);

											if ( game.playerList[ curAi ].unitList[ curUnit ].transported == Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].transport )
											{
												game.playerList[ curAi ].unitList[ curUnit ].state =	(byte)Form1.unitState.transportGoToDrop;

												int totMilitaryUnits = 0, totSettlers = 0;
												for ( int i = 0; i < game.playerList[ curAi ].unitList[ curUnit ].transported; i ++ ) 
													if ( Statistics.units[ game.playerList[ curAi ].unitList[ game.playerList[ curAi ].unitList[ curUnit ].transport[ i ] ].type ].speciality == enums.speciality.builder ) 
														totSettlers ++; 
													else 
														totMilitaryUnits ++; 
	 
												Point wtdmu;
												if ( totSettlers > 0 ) 
												{
													wtdmu = ai.findCitySiteWorldWide( game.playerList[ curAi ].unitList[ curUnit ].pos, curAi );

													game.playerList[ curAi ].memory.add( 
														(byte)enums.playerMemory.destDropSettler, 
														new int[] { wtdmu.X, wtdmu.Y, curUnit } 
														); // x, y, unit
												}
												else
												{
													wtdmu = aiTransport.whereToDisembarkMilitaryUnits( curAi, curUnit );

													game.playerList[ curAi ].memory.add( 
														(byte)enums.playerMemory.destDropMilitary, 
														new int[] { wtdmu.X, wtdmu.Y, curUnit } 
														); // x, y, unit
												}

												game.playerList[ curAi ].unitList[ curUnit ].dest = transport.destOnWater( curAi, curUnit, wtdmu );
												curUnit --;
											}
											else
											{
												game.playerList[ curAi ].unitList[ curUnit ].state =	(byte)Form1.unitState.exploring;
												curUnit --;
											}
										} 
										#endregion

										#region exploring
										else if ( game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.exploring )
										{
										/*	if ( ai.isInWar( curAi ) )
											{
												Form1.game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.idle;
												curUnit --;
											}
											else
											{*/
												Point tempDest = ai.findWhereToExplore( curAi, curUnit );

												if ( tempDest.X >= 0 ) 
												{ 
													Form1.game.playerList[ curAi ].unitList[ curUnit ].dest = tempDest; 
													
													Point[] way = game.radius.findWayTo( 
														game.playerList[ curAi ].unitList[ curUnit ].X, 
														game.playerList[ curAi ].unitList[ curUnit ].Y, 
														game.playerList[ curAi ].unitList[ curUnit ].dest.X, 
														game.playerList[ curAi ].unitList[ curUnit ].dest.Y, 
														Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].terrain, 
														curAi, 
														false 
														); 

												//	m  = 0;
													if ( way[ 0 ].X != -1 )
														while ( 
															game.playerList[ curAi ].unitList[ curUnit ].state != (byte)Form1.unitState.dead && 
															( 
															game.playerList[ curAi ].unitList[ curUnit ].moveLeft > 0 || 
															game.playerList[ curAi ].unitList[ curUnit ].moveLeftFraction > 0 
															) 
															) 
														{ 
															if ( 
																game.playerList[ curAi ].unitList[ curUnit ].X == way[ way.Length - 1 ].X && 
																game.playerList[ curAi ].unitList[ curUnit ].Y == way[ way.Length - 1 ].Y 
																) // at dest
															{
																curUnit --; 
																break; 
															}
															
															if ( game.radius.caseOccupiedByRelationType( way[ m ].X, way[ m ].Y, curAi, true, false, false, false, false, false ) )
															{
																int ennemy = ai.strongestEnnemy( way[ m ].X, way[ m ].Y, curAi ); 
																attackUnit( curAi, curUnit, game.grid[ way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].player.player, game.grid[  way[ m ].X, way[ m ].Y ].stack[ ennemy - 1 ].ind );
														
																curUnit --; 
																break; 
															}
															else
															{
																UnitMove( 
																	curAi, 
																	curUnit, 
																	game.playerList[ curAi ].unitList[ curUnit ].X, 
																	game.playerList[ curAi ].unitList[ curUnit ].Y,
																	way[ m ].X,
																	way[ m ].Y
																	);
																m  ++;
															}

														}
												}
												else
												{ // nothing to explore
													game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified ;
												}
										//	}
										}
											#endregion

										#region idle, fortified // , turnCompleted 
										else if ( 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.idle || 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.fortified /*|| 
											game.playerList[ curAi ].unitList[ curUnit ].state == (byte)Form1.unitState.turnCompleted*/ 
											) 
										{ 
											if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].terrain == 1 ) 
											{ // land unit 
												if ( Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].attack >= Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].defense )
												{ // attacker 
													if ( ai.isInWar( curAi ) ) 
													{ 
														#region attack
														Point destToAttack = aiWar.findEnnemyToAttack( curAi, curUnit );

														if ( destToAttack.X != -1 )
														{
															bool foundDest = false;
															if ( game.grid[ destToAttack.X, destToAttack.Y ].continent == game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].continent )
															{ // meme cont yé 
																Point[] way = game.radius.findWayTo( 
																	game.playerList[ curAi ].unitList[ curUnit ].X,
																	game.playerList[ curAi ].unitList[ curUnit ].Y,
																	destToAttack.X,
																	destToAttack.Y,
																	Statistics.units[ game.playerList[ curAi ].unitList[ curUnit ].type ].terrain,
																	curAi,
																	true
																	);

																if ( way[ 0 ].X != -1 ) 
																{ // can attack unit found 
																	foundDest = true; 
																	game.playerList[ curAi ].unitList[ curUnit ].dest = destToAttack;
																	game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.goToAttack;
																	curUnit --; 
																} 
															} 
														
															if ( !foundDest ) // diff continent or need boat 
																if ( game.playerList[ curAi ].canBuildTransport )
																	if ( !findTransport( curAi, ref curUnit ) )
																		if ( !buildTransport( curAi, ref curUnit ) )
																			game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified;

															#region garbage
																/*#region find boat 

																bool foundBoat = false; 
																int pos = 0; 
																int[] dist = new int[ 100 ]; 
																int[] units = new int[ 100 ]; 

																for ( int unit = 1; unit <= game.playerList[ curAi ].unitNumber; unit ++ )
																	if ( 
																		Statistics.units[ game.playerList[ curAi ].unitList[ unit ].type ].transport > 0 &&
																		game.playerList[ curAi ].unitList[ unit ].transported < Statistics.units[ game.playerList[ curAi ].unitList[ unit ].type ].transport &&
																		(
																		game.playerList[ curAi ].unitList[ unit ].state == (byte)Form1.unitState.exploring ||
																		game.playerList[ curAi ].unitList[ unit ].state == (byte)Form1.unitState.transportWaitingToFill
																		)
																		)
																	{
																		units[ pos ] = unit;
																		dist[ pos ] = game.radius.getDistWith( 
																			new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ), 
																			new Point( game.playerList[ curAi ].unitList[ unit ].X, game.playerList[ curAi ].unitList[ unit ].Y )
																			);
																		pos ++;
																	}

																if ( pos > 0 )
																{
																	int[] dist1 = new int[ pos ];
																	for ( int i = 0; i < pos; i ++ )
																		dist1[ i ] = dist[ i ];

																	int[] order = count.ascOrder( dist1 );

																	if ( dist1[ order[ 0 ] ] <= 1 )
																	{
																		foundBoat = true; //units[ order.Length - 1 ];

																		move.UnitMove2Transport( curAi, curUnit, units[ order.Length - 1 ] );
																	}
																	else if ( dist1[ order[ 0 ] ] < 100 )
																	{
																		if ( game.radius.isNextToWater( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ) )
																		{
																			game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.waitingForBoat;
																			
																			game.playerList[ curAi ].unitList[ units[ order[ 0 ] ] ].dest = transport.destOnWater( 
																				curAi, 
																				curUnit, 
																				new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ) );

																			game.playerList[ curAi ].unitList[ units[ order[ 0 ] ] ].state = (byte)Form1.unitState.transportGoToPickUp;
																		}
																		else
																		{
																			game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.waitingForBoat;
																			game.playerList[ curAi ].unitList[ units[ order[ 0 ] ] ].state = (byte)Form1.unitState.transportGoToPickUp;
																			
																			Point caseToMeet = 
																				game.radius.caseToMeetBoat(
																				curAi,
																				new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ),
																				new Point( game.playerList[ curAi ].unitList[ units[ order[ 0 ] ] ].X, game.playerList[ curAi ].unitList[ units[ order[ 0 ] ] ].Y )
																				);

																			game.playerList[ curAi ].unitList[ curUnit ].dest = caseToMeet;
																			
																			game.playerList[ curAi ].unitList[ units[ order[ 0 ] ] ].dest = transport.destOnWater( curAi, curUnit, caseToMeet );

																			curUnit --; 
																		}

																		foundBoat = true; 
																	}
																} 

																#endregion

																#region build boat
																if ( !foundBoat && ai.canBuildTransport( curAi ) ) 
																{ 
																	bool foundCity = false;
																	int foundCityInt = -1;
																	int[] dists = new int[ Form1.game.playerList[ curAi ].cityNumber ]; 
																	int[] cities = new int[ Form1.game.playerList[ curAi ].cityNumber ]; 
																	int pos1 = 0;

																	for ( int i = 1; i <= Form1.game.playerList[ curAi ].cityNumber; i ++ ) 
																		if ( 
																			Form1.game.playerList[ curAi ].cityList[ i ].state == (byte)enums.cityState.ok &&
																			game.radius.isNextToWater( Form1.game.playerList[ curAi ].cityList[ i ].X, Form1.game.playerList[ curAi ].cityList[ i ].Y )/*&& 
																			Form1.game.grid[ Form1.game.playerList[ curAi ].cityList[ i ].X, Form1.game.playerList[ curAi ].cityList[ i ].Y ].continent == Form1.game.grid[ game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ].continent */
																			/*)
																		{
																			if (
																				Form1.game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.unit &&
																				Statistics.units[ Form1.game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].ind ].transport > 0
																				)
																			{
																				pos1 = 0;
																				foundCity = false;
																				break;
																			}
																			else if ( 
																				Form1.game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.unit || 
																				Form1.game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.building || 
																				Form1.game.playerList[ curAi ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.wealth 
																				) 
																			{ 
																				dists[ pos1 ] = game.radius.getDistWith( 
																					new Point( Form1.game.playerList[ curAi ].cityList[ i ].X, Form1.game.playerList[ curAi ].cityList[ i ].Y ),
																					new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y )
																					);
																				cities[ pos1 ] = i;
																				pos1 ++;
																			} 
																		}

																	if ( pos1 > 0 ) 
																	{ // at least 1 city can build the transport 

																		int[] dists1 = new int[ pos1 ]; 
																		for ( int i = 0; i < dists1.Length; i ++ ) 
																			dists1[ i ] = dists[ i ]; 

																		int[] order = count.ascOrder( dists1 ); 

																		for ( int j = 0; j < dists1.Length; j ++ ) 
																			if ( 
																				Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.wealth || 
																				Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.nothing || 
																				( 
																				Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.unit &&
																				Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.points * 2 < Statistics.units[ Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].ind ].cost 
																				)|| 
																				( 
																				Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.building &&
																				Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.points * 2 < Statistics.buildings[ Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].ind ].cost 
																				) 
																				) 
																			{
																				foundCityInt = cities[ order[ j ] ];
																				ai.buildRandomTransport( curAi, cities[ order[ j ] ] ); 

																				if ( game.playerList[ curAi ].money > 2 * Statistics.units[ Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.points ) 
																					aiTown.buyContruction( curAi, cities[ order[ j ] ] );

																				foundCity = true; 
																				break; 
																			}

																		if ( !foundCity )
																			for ( int j = 0; j < dists1.Length; j ++ ) 
																				if ( 
																					Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.unit || 
																					Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.building
																					) 
																				{ 
																					foundCityInt = cities[ order[ j ] ];
																					ai.buildRandomTransport( curAi, cities[ order[ j ] ] ); 

																					if ( game.playerList[ curAi ].money > 2 * Statistics.units[ Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ curAi ].cityList[ cities[ order[ j ] ] ].construction.points ) 
																						aiTown.buyContruction( curAi, cities[ order[ j ] ] );

																					foundCity = true; 
																					break; 
																				} 
																	} 

																	if ( !foundCity ) 
																		game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified; 
																	else if ( foundCityInt != -1 )
																	{
																		Point[] way = game.radius.findWayTo( 
																			game.playerList[ curAi ].unitList[ curUnit ].X, 
																			game.playerList[ curAi ].unitList[ curUnit ].Y, 
																			Form1.game.playerList[ curAi ].cityList[ foundCityInt ].X, 
																			Form1.game.playerList[ curAi ].cityList[ foundCityInt ].Y, 
																			1,
																			curAi, 
																			false
																			);

																		if ( way[ 0 ].X != -1 ) 
																		{ 
																			game.playerList[ curAi ].unitList[ curUnit ].dest = new Point( Form1.game.playerList[ curAi ].cityList[ foundCityInt ].X, Form1.game.playerList[ curAi ].cityList[ foundCityInt ].Y );
																			game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.goToBoat;
																			curUnit --;
																		} 
																	}
																} 
																#endregion */
																#endregion
														} 
														#endregion 
													} 
													else 
													{ 
														#region not in war, waiting or moving
														if ( game.grid[ game.playerList[ curAi ].unitList[ curUnit].X, game.playerList[ curAi ].unitList[ curUnit].Y ].city > 0 )
														{
															game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified;
														}
														else
														{	
															Point dest = aiWar.returnLandFrontierToAttack( 
																curAi, 
																aiWar.whichEnnemyToAttack( curAi, new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y ) ),
																new Point( game.playerList[ curAi ].unitList[ curUnit ].X, game.playerList[ curAi ].unitList[ curUnit ].Y )
																);
																
															if ( dest.X != -1 )
															{
																game.playerList[ curAi ].unitList[ curUnit ].dest = dest;
																game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.goToFrontier;
																curUnit --;
															}
															else
															{
																game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.exploring;
																curUnit --;
															}
														}
														#endregion 
													} 
												} 
												else 
												{ 
													#region defense

													Point[] way = aiWar.wayToSiteToDefend( curAi, curUnit );

													if ( way[ 0 ].X == -2 )
													{
														game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified;
													}
													else if ( way[ 0 ].X != -1 )
													{
														game.playerList[ curAi ].unitList[ curUnit ].dest = way[ way.Length - 1 ];
														game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.goToDefend;
														curUnit --;
													}
													else
													{
														game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified;
													}
													#endregion 
												} 
											}
											else
											{ // boat
												bool setToTWtF = false;
												for ( int rad = 0; rad < 2 && !setToTWtF; rad ++ )
												{
													Point[] sqr = game.radius.returnEmptySquare( game.playerList[ curAi ].unitList[ curUnit ].pos, rad );

													for ( int k = 0; k < sqr.Length && !setToTWtF; k ++ )
														for ( int i = 0; i < game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length && !setToTWtF; i ++ )
															if ( 
																game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i ].player.player == curAi &&
																game.playerList[ curAi ].unitList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i ].ind ].state == (byte)Form1.unitState.waitingForBoat &&
																(
																game.playerList[ curAi ].unitList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i ].ind ].moveLeft > 0 ||
																game.playerList[ curAi ].unitList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ i ].ind ].moveLeftFraction > 0 
																)
																)
															{
																setToTWtF = true;
															//	game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.transportWaitingToFill; 
																break;
															}
												}

												if ( setToTWtF )
												{
													game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.transportWaitingToFill; 
													curUnit --;
												}
												else 
												{
													game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.exploring; 
													curUnit --; 
												}
												
											//	curUnit --; 

											/*	game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.transportWaitingToFill; 
												curUnit --; */
												/*if ( game.playerList[ curAi ].unitList[ curUnit ].transported > 0 ) 
												{ // transport units 
												/*	game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.transportGoToDrop; 
													curUnit --; *
												} 
												else 
												{ // empty 
													game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.exploring; 
													curUnit --; 
												} */
											} 
										} 
										#endregion

										/*} 
										else 
										{ // si pas en guerre 
											game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified;
										} */ 
										#endregion 

								} // end war unit 

								/*	else
									{ // pas settler ni unite de guerre, diplomate?? 
										game.playerList[ curAi ].unitList[ curUnit ].state = (byte)Form1.unitState.fortified;
									}*/
								} // end m > 0
						}

					#endregion

						for ( int city = 1; city <= game.playerList[ curAi ].cityNumber; city ++ )
							if ( aiTown.shouldBuyDefenseMilitaryUnit( curAi, city ) )
							{
								aiTown.buildRandomDefensiveUnit( curAi, city );

								if ( aiTown.canBuyContruction( curAi, city ) )
									aiTown.buyContruction( curAi, city );
							}
					}

				game.verifyState();

				if ( game.state == Game.State.playerLost )
				{
				}
			/*	else if ( game.state == Game.State.playerWon )
				{
				}	*/
				else
				{

					game.curTurn ++;

					#region playerIni

							wC.show = false;

						//	int nationalTrade1 = Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade;

							if ( game.playerList[ Form1.game.curPlayerInd ].cityNumber > 0 )
							{
								game.playerList[ Form1.game.curPlayerInd ].technos[ game.playerList[ Form1.game.curPlayerInd ].currentResearch ].pntDiscovered += 1 + Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * game.playerList[ Form1.game.curPlayerInd ].preferences.science /100; //getPFT1.getNationScience( curAi );
								game.playerList[ Form1.game.curPlayerInd ].money += (int)( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * game.playerList[ Form1.game.curPlayerInd ].preferences.reserve / 100 );
							}

							#region units reset move
							for (int i = 1; i <= game.playerList[ Form1.game.curPlayerInd ].unitNumber; i++) // 
							{
								if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state != (byte)Form1.unitState.dead ) 
								{
									if ( 
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.buildingRoad ||
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.buildingIrrigation ||
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.buildingMine ||
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.buildingFort ||
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.buildingAirport ||
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state == (byte)Form1.unitState.buildingMineFld
										)
									{
										caseImprovement.advImprovement( Form1.game.curPlayerInd, i );
									}
									else if (
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state != (byte)Form1.unitState.fortified && 
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state != (byte)Form1.unitState.sleep && 
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state != (byte)Form1.unitState.inTransport
										)
									{
										game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].state = (byte)Form1.unitState.idle;
									}

									game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].moveLeft = (sbyte)Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].type ].move;
									game.playerList[ Form1.game.curPlayerInd ].unitList[ i ].moveLeftFraction = 0;
								}
							}
							#endregion

							#region technologies
							if ( game.playerList[ Form1.game.curPlayerInd ].technos[ game.playerList[ Form1.game.curPlayerInd ].currentResearch ].pntDiscovered >= Statistics.technologies[ game.playerList[ Form1.game.curPlayerInd ].currentResearch ].cost )
							{
								if ( !game.playerList[ Form1.game.curPlayerInd ].technos[ 0 ].researched )
									Tutorial.show( this, Tutorial.order.discoveringTechnologies );

								game.playerList[ Form1.game.curPlayerInd ].aquireTechno();
							//	nego.aquireTechno( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].currentResearch );
							}

							#endregion

						#endregion

					#region playerCities

							for ( int i = 1; i <= game.playerList[ Form1.game.curPlayerInd ].cityNumber; i++ )
							{
								#region normal
								if( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].state == (byte)enums.cityState.ok )
								{
									// bouffe
									game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].foodReserve += (int)( Form1.game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].food - game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population - game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].slaves.total * .5 );

									if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].foodReserve > game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population * foodPerPop )
									{
										game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].foodReserve -= game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population * foodPerPop;
										game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population ++;

										labor.autoAddLabor( Form1.game.curPlayerInd, i );
										
										game.frontier.setFrontiers();
									}
									else if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].foodReserve < 0 )
									{
										game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population --;

										if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population == 0 )
										{
									//		game.playerList[ curAi ].cityList[ i ].kill();
									//		deleteCity( Form1.game.curPlayerInd, i );
											Form1.game.curPlayer.cityList[ i ].kill();
											continue;
										}

										game.frontier.setFrontiers();
									}

									if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].construction.list[ 0 ].type == 3 )
									{ // wealth
										game.playerList[ Form1.game.curPlayerInd ].money += Form1.game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].production;
									}
									else
									{
										game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].construction.points += Form1.game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].production;
									
										int costLeft = game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].construction.list[ 0 ].cost - game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].construction.points;

										if ( costLeft <= 0 )
											game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].buildConstruction( this );
									}
								}
									#endregion

								#region evacuating
								else if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].state == (byte)enums.cityState.evacuating )
								{ // evacuating
									for ( int k = 0; k < 2; k ++ )
									{
										game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population --;

										if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].population == 0 )
										{
											game.grid[ game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].X, game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].Y ].city = 0;
											game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].state = (byte)enums.cityState.dead;
											game.playerList[ Form1.game.curPlayerInd ].createUnit( game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].X, game.playerList[ Form1.game.curPlayerInd ].cityList[ i ].Y, (byte)Form1.unitType.colon );
											break;
										}
									}
								}
								#endregion
							}

					#endregion

					defaultIni( Form1.game.curPlayerInd, -1 );
				}

				unitOrder.setOrder( Form1.game.curPlayerInd );
			}
			else
				MessageBox.Show( "Some of your automated units can still move.", "End of turn interrupted" ); 

			#region final

			if ( game.state == Game.State.playerLost )
			{
				MessageBox.Show( language.getAString( language.order.endGameYouLost ) );

				/**
				 * 
				 * year
				 * maxPop ?
				 * 
				**/

				game = null;
				showOpening( false );
			}
			else
			{
				enableMenus();
				showInfo();

				//	waitingTmr.Enabled = false;
				oldSliVer = -1;

				//	frontier.setFrontiers();
				drawMiniMap();
				guiEnabled = true;
				if ( !showNextUnitNow( 0, 0 ) )
					DrawMap();

				wC.show = false;
			}
			//	PInvokeLibrary.WaveOut.play( "\\Windows\\notify.wav" );
			#endregion

		}
	#endregion


	#region choose city or improve
		private void chooseCityOrImprove( byte player, int unit )
		{
		//	ai Ai = new ai();

			Point citySite = ai.findCitySite( 
				game.playerList[ player ].unitList[ unit ].X, 
				game.playerList[ player ].unitList[ unit ].Y, 
				player 
				);

			if ( citySite != new Point( -1, -1 ) )
			{ // site valide
				game.playerList[ player ].unitList[ unit ].dest = citySite;
				game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToBuildCity;

				verifyPosition( player, unit );
			}
			else
			{
				chooseImprove( player, unit );
			}

		}
		#endregion

	#region choose improve
		private bool chooseImprove( byte player, int unit  )
		{
			Point improveSite = ai.findCaseToImprove( 
				game.playerList[ player ].unitList[ unit ].X, 
				game.playerList[ player ].unitList[ unit ].Y, 
				player 
				);

			if ( improveSite != new Point( -1, -1 ) )
			{ // site valide
				game.playerList[ player ].unitList[ unit ].dest = improveSite;
				game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToImprove;
			}
			else
			{
				game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle; //.turnCompleted; testingfr
				game.playerList[ player ].unitList[ unit ].moveLeft = 0;
			}

			return false;
		}
		#endregion

	#region verify position
		private void verifyPosition( byte player, int unit )
		{
			ai Ai = new ai();
			if (
				game.playerList[ player ].unitList[ unit ].X == game.playerList[ player ].unitList[ unit ].dest.X &&
				game.playerList[ player ].unitList[ unit ].Y == game.playerList[ player ].unitList[ unit ].dest.Y 
				)
			{
				if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBuildCity )
				{ 
					if ( ai.siteIsValidForCity( game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y, player ) )
						createCity( 
							game.playerList[ player ].unitList[ unit ].X,
							game.playerList[ player ].unitList[ unit ].Y,
							player,
							curUnit,
							true, // l'unité sera automatiquement deleté
							findName( player )
							);
					else
						chooseCityOrImprove( player, unit );
				}
				else if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToImprove )
					switch ( ai.chooseImprovement( game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y, player ) )
					{
						case 0:
							buildRoad( player, curUnit );
							break;
						case 1:
							buildIrrigation( player, curUnit );
							break;
						case 2:
							buildMine( player, curUnit );
							break;
						case -1: // erreur
							chooseCityOrImprove( player, unit );
							break;
					}
			}
			else
			{
				Point[] pntWay = game.radius.findWayTo(
					game.playerList[ player ].unitList[ unit ].X,
					game.playerList[ player ].unitList[ unit ].Y,
					game.playerList[ player ].unitList[ unit ].dest.X,
					game.playerList[ player ].unitList[ unit ].dest.Y,
					1,
					player,
					false
					);

				if ( pntWay[ 0 ] != new Point( -1, -1) )
				{
					if ( 
						!ai.caseOccupied( pntWay[ 0 ].X, pntWay[ 0 ].Y, curAi ) && 
						( 
						game.grid[ pntWay[ 0 ].X, pntWay[ 0 ].Y ].city == 0 || 
						game.grid[ pntWay[ 0 ].X, pntWay[ 0 ].Y ].territory - 1 == curAi || 
						game.playerList[ player ].foreignRelation[ game.grid[ pntWay[ 0 ].X, pntWay[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.alliance || 
						game.playerList[ player ].foreignRelation[ game.grid[ pntWay[ 0 ].X, pntWay[ 0 ].Y ].territory - 1 ].politic == (byte)relationPolType.Protected || 
						game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].territory - 1 != curAi 
						) 
						) 
						UnitMove( 
							player, 
							unit, 
							game.playerList[ player ].unitList[ unit ].X, 
							game.playerList[ player ].unitList[ unit ].Y, 
							pntWay[ 0 ].X, 
							pntWay[ 0 ].Y 
							); 
					else 
						chooseCityOrImprove( player, unit ); 
				}
				else
					chooseCityOrImprove( player, unit );
			}
		}
		#endregion

	#region automatedSettler
		/// <summary> 
		/// unit byRef 
		/// </summary> 
		/// <param name="player"></param> 
		/// <param name="unit"></param> 
		private void automatedSettler( byte player, ref int unit ) 
		{ 
			if ( 
				game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBuildCity || 
				game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToImprove || 
				game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBoat || 
				game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToJoinCity
				)
			{
				#region
				bool okToContinue = true;
/*
				bool b0 = game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBuildCity;
				bool b1 = !game.radius.caseOccupiedByRelationType( game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y, player, game.radius.relationTypeListNonAllies );
				bool b2 = Form1.regionValidToBuildCity[ game.playerList[ player ].unitList[ unit ].dest.X ].Get( game.playerList[ player ].unitList[ unit ].dest.Y );
				*/	
				if ( 
					game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBuildCity &&
					!game.radius.caseOccupiedByRelationType( game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y, player, game.radius.relationTypeListNonAllies ) &&
					Form1.regionValidToBuildCity[ game.playerList[ player ].unitList[ unit ].dest.X ].Get( game.playerList[ player ].unitList[ unit ].dest.Y )
					)
				{ // yay 
				}
				else if ( 
					game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToImprove  &&
					!game.radius.caseOccupiedByRelationType( game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y, player, game.radius.relationTypeListNonAllies ) &&
					-1 != ai.chooseImprovement( game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y, player )
					)
				{ // yay 
				}
				else if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBoat )
				{
				}
				else if ( 
					game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToJoinCity &&
					Form1.game.grid[ game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y ].city > 0 &&
					Form1.game.grid[ game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y ].territory - 1 == player
					)
				{ // yay
				}
				else // find new dest
				{
					game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle; 
					unit --; 
					okToContinue = false; 
				}

				if ( okToContinue )
				{
					if ( game.playerList[ player ].unitList[ unit ].X == game.playerList[ player ].unitList[ unit ].dest.X && game.playerList[ player ].unitList[ unit ].Y == game.playerList[ player ].unitList[ unit ].dest.Y )
					{ // at dest
						if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBuildCity )
						{
							createCity( game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y, player, unit, true, findName( player ) );
						}
						else if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToImprove )
						{
							switch ( ai.chooseImprovement( game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y, player ) )
							{
								case 0 :
									buildRoad( player, unit );
									break;

								case 1 :
									buildIrrigation( player, unit );
									break;

								case 2 :
									buildMine( player, unit );
									break;
								/*	case -1: // erreur
										//	chooseCityOrImprove( player, unit );
											break;*/
								}
						}
						else if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBoat )
						{
							game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle;
							unit--;
						}
						else if ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToJoinCity )
						{
							game.playerList[ player ].cityList[ game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].city ].population++;
							labor.autoAddLabor( player, game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].city );
							game.playerList[ player ].unitList[ unit ].kill();
					//		unitDelete( player, unit );
						}

						//	unit--;
						//break;
					}
					else
					{
						int m = 0;
						Point[] way = game.radius.findWayTo( game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y, game.playerList[ player ].unitList[ unit ].dest.X, game.playerList[ player ].unitList[ unit ].dest.Y, 1, player, false );

						if ( way[ 0 ].X != -1 )
							while ( ( game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToBuildCity || game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.goToImprove ) && ( game.playerList[ player ].unitList[ unit ].moveLeft > 0 || game.playerList[ player ].unitList[ unit ].moveLeftFraction > 0 ) )
							{
								if ( game.playerList[ player ].unitList[ unit ].X == game.playerList[ player ].unitList[ unit ].dest.X && game.playerList[ player ].unitList[ unit ].Y == game.playerList[ player ].unitList[ unit ].dest.Y )
								{
									unit--;
									break;
								}
								else
								{
									UnitMove( player, unit, game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y, way[ m ].X, way[ m ].Y );
									m ++;
								}
							}
					}
				}
				#endregion
			}
			else if ( 
				game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.idle || 
				game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.automate 
				)
			{
				Point citySite;
				if ( 
					player != Form1.game.curPlayerInd &&
					game.playerList[ player ].memory.mayBeAbleToFindCitySiteOnCont( game.playerList[ player ].unitList[ unit ].pos )
					)
				{
					citySite = ai.findCitySite( 
						game.playerList[ player ].unitList[ unit ].X, 
						game.playerList[ player ].unitList[ unit ].Y, 
						player 
						);

					if ( citySite.X == -1 )
						game.playerList[ player ].memory.addCitySiteNotFoundOnCont( game.playerList[ player ].unitList[ unit ].pos );
				}
				else
					citySite = new Point( -1, -1 );

				if ( citySite.X != -1 )
				{ // site valide
					game.playerList[ player ].unitList[ unit ].dest = citySite;
					game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToBuildCity;
					unit --;
				}
				else
				{
					bool found = false;
					Random r = new Random();
					if ( 
						player != Form1.game.curPlayerInd &&
						r.Next( 1000 ) < 5 && 
						game.playerList[ player ].canBuildTransport &&
						game.playerList[ player ].memory.mayBeAbleToFindCitySiteWW( game.playerList[ player ].unitList[ unit ].pos )
						)
						if ( ai.findCitySiteWorldWide( game.playerList[ player ].unitList[ unit ].pos, player ).X != -1 &&
							( 
							findTransport( player, ref unit ) || 
							buildTransport( player, ref unit )
							)
							)
							{
							}
					
					if ( !found )
					{
						Point improveSite = ai.findCaseToImprove( 
							game.playerList[ player ].unitList[ unit ].X, 
							game.playerList[ player ].unitList[ unit ].Y, 
							player 
							);

						if ( improveSite != new Point( -1, -1 ) )
						{ // site valide
							game.playerList[ player ].unitList[ unit ].dest = improveSite;
							game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToImprove;
							unit--;
							found = true;
						}
						else if ( 
							player != Form1.game.curPlayerInd &&
							game.playerList[ player ].canBuildTransport 
							)
						{
							if ( game.playerList[ player ].memory.mayBeAbleToFindCitySiteWW( game.playerList[ player ].unitList[ unit ].pos ) )
								if ( ai.findCitySiteWorldWide( game.playerList[ player ].unitList[ unit ].pos, player ).X != -1 )
								{
									found = false;
									if ( 
										findTransport( player, ref unit ) ||
										buildTransport( player, ref unit ) 
										)
										{
										//	unit--;
											found = true;
										}
								/*	if ( !findTransport( player, ref unit ) )
										if ( !buildTransport( player, ref unit ) )
										{
											Point dest = ai.findNearestPlayerCity( player, unit );
											if ( dest.X != -1 )
											{
												game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToJoinCity;//.idle;//turnCompleted;
												game.playerList[ player ].unitList[ unit ].dest = dest;
												unit--;
											}
											else
											{
												game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle;//turnCompleted;
												game.playerList[ player ].unitList[ unit ].moveLeft = 0;
											}

										}*/
								}
								else
								{
									game.playerList[ player ].memory.addCitySiteNotFoundWW();
								}
						}
						else if ( player == Form1.game.curPlayerInd )
						{
							game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle;
							game.playerList[ player ].unitList[ unit ].automated = false;
						}

						if ( 
							player != Form1.game.curPlayerInd &&
							!found
							)
						{
							Point dest = ai.findNearestPlayerCity( player, unit );

							if ( dest.X != -1 )
							{
								game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToJoinCity;//.idle;//turnCompleted;
								game.playerList[ player ].unitList[ unit ].dest = dest;
								game.playerList[ player ].memory.addSentToJoinedCity( Form1.game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].continent );
								unit--;
							}
							else
							{
								game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle;//turnCompleted;
								game.playerList[ player ].unitList[ unit ].moveLeft = 0;
							}
						}
					}
				}
			}
		} 
		#endregion

	#region findTransport
		private bool findTransport( byte player, ref int unit )
		{
			int pos = 0; 
			int[] dist = new int[ 100 ], units = new int[ 100 ]; 

			for ( int u = 1; u <= game.playerList[ player ].unitNumber; u ++ )
				if ( 
					Statistics.units[ game.playerList[ player ].unitList[ u ].type ].transport > 0 &&
					game.playerList[ player ].unitList[ u ].transported < Statistics.units[ game.playerList[ player ].unitList[ u ].type ].transport &&
					(
						game.playerList[ player ].unitList[ u ].state == (byte)Form1.unitState.exploring ||
						game.playerList[ player ].unitList[ u ].state == (byte)Form1.unitState.transportWaitingToFill ||
						game.playerList[ player ].unitList[ u ].state == (byte)Form1.unitState.fortified ||
						game.playerList[ player ].unitList[ u ].state == (byte)Form1.unitState.idle || 
						game.playerList[ player ].unitList[ u ].state == (byte)Form1.unitState.turnCompleted
					)
					)
				{
					units[ pos ] = u;
					dist[ pos ] = game.radius.getDistWith( 
						game.playerList[ player ].unitList[ unit ].pos, 
						new Point( game.playerList[ player ].unitList[ u ].X, game.playerList[ player ].unitList[ u ].Y )
						);
					pos ++;
				}

			if ( pos > 0 )
			{
				int[] dist1 = new int[ pos ];
				for ( int i = 0; i < pos; i ++ )
					dist1[ i ] = dist[ i ];

				int[] order = count.ascOrder( dist1 );

				if ( dist1[ order[ 0 ] ] <= 1 )
				{
					move.UnitMove2Transport( player, unit, units[ order[ 0 ] ] );
					return true;
				}
				else if ( dist1[ order[ 0 ] ] < 100 )
				{
					if ( game.radius.isNextToWater( game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ) )
					{
						// unit waiting on place
						game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.waitingForBoat;
							
						// set boat dest					
						game.playerList[ player ].unitList[ units[ order[ 0 ] ] ].dest = transport.destOnWater( 
							player, 
							unit, 
							game.playerList[ player ].unitList[ unit ].pos 
							);

						game.playerList[ player ].unitList[ units[ order[ 0 ] ] ].state = (byte)Form1.unitState.transportGoToPickUp;
					}
					else
					{
						game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToBoat;
						game.playerList[ player ].unitList[ units[ order[ 0 ] ] ].state = (byte)Form1.unitState.transportGoToPickUp;
																		
						Point caseToMeet = 
							game.radius.caseToMeetBoat(
							player,
							game.playerList[ player ].unitList[ unit ].pos,
							new Point( game.playerList[ player ].unitList[ units[ order[ 0 ] ] ].X, game.playerList[ player ].unitList[ units[ order[ 0 ] ] ].Y )
							);

						game.playerList[ player ].unitList[ unit ].dest = caseToMeet;
																		
						game.playerList[ player ].unitList[ units[ order[ 0 ] ] ].dest = transport.destOnWater( player, unit, caseToMeet );

						unit --; 
					}

					return true;
				}
			} 
			return false;
		}
		#endregion

	#region build boat
		private bool buildTransport( byte player, ref int unit )
		{
			bool foundCity = false;
			int foundCityInt = -1;
			int[] dists = new int[ Form1.game.playerList[ player ].cityNumber ]; 
			int[] cities = new int[ Form1.game.playerList[ player ].cityNumber ]; 
			int pos1 = 0;

			for ( int i = 1; i <= Form1.game.playerList[ player ].cityNumber; i ++ ) 
				if ( 
					Form1.game.playerList[ player ].cityList[ i ].state == (byte)enums.cityState.ok &&
					game.radius.isNextToWater( Form1.game.playerList[ player ].cityList[ i ].X, Form1.game.playerList[ player ].cityList[ i ].Y )
				//	Form1.game.grid[ Form1.game.playerList[ player ].cityList[ i ].X, Form1.game.playerList[ player ].cityList[ i ].Y ].continent == Form1.game.grid[ game.playerList[ player ].unitList[ unit ].X, game.playerList[ player ].unitList[ unit ].Y ].continent
					)
				{
					if (
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ] is Stat.Unit &&
				//		Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.unit &&
						((Stat.Unit)Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ]).transport > 0
						)
					{
						pos1 = 0;
						foundCity = false;
						
						Point[] way = game.radius.findWayTo( 
							game.playerList[ player ].unitList[ unit ].X, 
							game.playerList[ player ].unitList[ unit ].Y, 
							Form1.game.playerList[ player ].cityList[ i ].X, 
							Form1.game.playerList[ player ].cityList[ i ].Y, 
							1, 
							player, 
							false 
							); 

						if ( way[ 0 ].X != -1 ) 
						{ 
							if ( 
								game.playerList[ player ].unitList[ unit ].pos.X == Form1.game.playerList[ player ].cityList[ i ].pos.X && 
								game.playerList[ player ].unitList[ unit ].pos.Y == Form1.game.playerList[ player ].cityList[ i ].pos.Y 
								)
							{
								//	game.playerList[ player ].unitList[ unit ].dest = Form1.game.playerList[ player ].cityList[ foundCityInt ].pos; //new Point( Form1.game.playerList[ player ].cityList[ foundCityInt ].X, Form1.game.playerList[ player ].cityList[ foundCityInt ].Y );
								game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.waitingForBoat;
							}
							else
							{
								game.playerList[ player ].unitList[ unit ].dest = Form1.game.playerList[ player ].cityList[ i ].pos;
								game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToBoat;
								unit --;
							}
							return true;
						} 
						break;
					}
					else if ( 
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ] is Stat.Unit &&
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ] is Stat.Building &&
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ] is Stat.Wealth /*&&
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.unit || 
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.building || 
						Form1.game.playerList[ player ].cityList[ i ].construction.list[ 0 ].type == (byte)enums.constructionType.wealth 
						*/) 
					{ 
						dists[ pos1 ] = game.radius.getDistWith( 
							new Point( Form1.game.playerList[ player ].cityList[ i ].X, Form1.game.playerList[ player ].cityList[ i ].Y ),
							game.playerList[ player ].unitList[ unit ].pos
							);
						cities[ pos1 ] = i;
						pos1 ++;
					} 
				}

			if ( pos1 > 0 ) // at least 1 city can build the transport  
			{
				int[] dists1 = new int[ pos1 ]; 
				for ( int i = 0; i < dists1.Length; i ++ ) 
					dists1[ i ] = dists[ i ]; 

				int[] order = count.ascOrder( dists1 ); 

				for ( int j = 0; j < dists1.Length; j ++ ) 
					if ( 
						!(Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat.Wonder) &&
						!(Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat.SmallWonder) &&
						(
							Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat.Wealth ||
							Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.points * 2 < Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].cost 
						)
					//	Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat.Wealth || //.type == (byte)enums.constructionType.wealth || 
					//	Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat. || //.type == (byte)enums.constructionType.nothing || 
						/*( 
						Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.unit &&
						Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.points * 2 < Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].cost 
						)|| 
						( 
						Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].type == (byte)enums.constructionType.building &&
						Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.points * 2 < Statistics.buildings[ Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].ind ].cost 
						) */
						) 
					{
						foundCityInt = cities[ order[ j ] ];
						ai.buildRandomTransport( player, cities[ order[ j ] ] ); 

						if ( game.playerList[ player ].money > 2 * Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].cost - Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.points ) 
							aiTown.buyContruction( player, cities[ order[ j ] ] );

						foundCity = true; 
						break; 
					}

				if ( !foundCity )
					for ( int j = 0; j < dists1.Length; j ++ ) 
						if ( 
							Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat.Unit || //.type //== (byte)enums.constructionType.unit || 
							Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ] is Stat.Building  //.type //== (byte)enums.constructionType.building
							) 
						{ 
							foundCityInt = cities[ order[ j ] ];
							ai.buildRandomTransport( player, cities[ order[ j ] ] ); 

							if ( game.playerList[ player ].money > 2 * Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.list[ 0 ].cost - Form1.game.playerList[ player ].cityList[ cities[ order[ j ] ] ].construction.points ) 
								aiTown.buyContruction( player, cities[ order[ j ] ] );

							foundCity = true; 
							break; 
						} 
			} 

			if ( !foundCity ) 
				game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.fortified; 
			else if ( foundCityInt != -1 )
			{
				Point[] way = game.radius.findWayTo( 
					game.playerList[ player ].unitList[ unit ].X, 
					game.playerList[ player ].unitList[ unit ].Y, 
					Form1.game.playerList[ player ].cityList[ foundCityInt ].X, 
					Form1.game.playerList[ player ].cityList[ foundCityInt ].Y, 
					1,
					player, 
					false
					);

				if ( way[ 0 ].X != -1 ) 
				{ 
					if ( 
						game.playerList[ player ].unitList[ unit ].pos.X == Form1.game.playerList[ player ].cityList[ foundCityInt ].pos.X && 
						game.playerList[ player ].unitList[ unit ].pos.Y == Form1.game.playerList[ player ].cityList[ foundCityInt ].pos.Y 
						)
					{
					//	game.playerList[ player ].unitList[ unit ].dest = Form1.game.playerList[ player ].cityList[ foundCityInt ].pos; //new Point( Form1.game.playerList[ player ].cityList[ foundCityInt ].X, Form1.game.playerList[ player ].cityList[ foundCityInt ].Y );
						game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.waitingForBoat;
					}
					else
					{
						game.playerList[ player ].unitList[ unit ].dest = Form1.game.playerList[ player ].cityList[ foundCityInt ].pos; //new Point( Form1.game.playerList[ player ].cityList[ foundCityInt ].X, Form1.game.playerList[ player ].cityList[ foundCityInt ].Y );
						game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.goToBoat;
						unit --;
					}
					return true;
				} 
			}

			return false;
		}
		#endregion 
		
		private void setTransportDropDest( byte player, int unit )
		{
			Point destToAttack = aiWar.findEnnemyToAttack( player, unit, 0 );
		}
	
#endregion

#region attack unit # # #

	#region attack unit

		private void attackUnit( UnitList attacker, UnitList defender )
		{
			attackUnit( 
				attacker.player.player, attacker.ind, 
				defender.player.player, defender.ind 
				);
		}

		private void attackUnit( byte ownerAtt, int unitAtt, byte ownerDef, int unitDef )
		{
			if ( 
				ownerDef == Form1.game.curPlayerInd || 
				ownerAtt == Form1.game.curPlayerInd ||
				(
					game.playerList[ Form1.game.curPlayerInd ].see[ 
						game.playerList[ ownerAtt ].unitList[ unitAtt ].X, 
						game.playerList[ ownerAtt ].unitList[ unitAtt ].Y
						] && 
					game.playerList[ Form1.game.curPlayerInd ].see[ 
						game.playerList[ ownerDef ].unitList[ unitDef ].X, 
						game.playerList[ ownerDef ].unitList[ unitDef ].Y
						] 
				)
				)
				if ( game.playerList[ ownerAtt ].technos[ (int)Form1.technoList.explosives ].researched )
					soundPlayer.play( "machineGuns.wav", true );
				else
					soundPlayer.play( "swords.wav", true );


			int delayInCombat = 600;
			int defense, attaque;
		/*	byte curAttOwner, curDefOwner;
			int curAttUnit, curDefUnit;*/

			attaque = Statistics.units[ game.playerList[ ownerAtt ].unitList[ unitAtt ].type ].attack * 100 ;

			defense = 
				Statistics.units[ game.playerList[ ownerDef ].unitList[ unitDef ].type ].defense * 
				Statistics.terrains[ game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].type ].defense;

			if ( 
				game.playerList[ ownerDef ].unitList[ unitDef ].typeClass.terrain == 2 || 
				game.playerList[ ownerDef ].unitList[ unitDef ].typeClass.terrain == 0 && game.playerList[ ownerAtt ].unitList[ unitAtt ].typeClass.terrain == 1
				)
				defense /= 3;


			if ( game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].militaryImprovement == (byte)enums.militaryImprovement.fort ) 
				defense = 150 * defense / 100; 
			else if ( 
				Statistics.units[ game.playerList[ ownerDef ].unitList[ unitDef ].type ].terrain == 0 && 
				game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].city > 0 
				) 
				defense = 50 * defense / 100; 
			else if ( 
				Statistics.units[ game.playerList[ ownerDef ].unitList[ unitDef ].type ].terrain == 1 && 
				game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].city > 0 
				) 
				defense = 150 * defense / 100; 

			if ( ownerAtt == Form1.game.curPlayerInd )
				switch ( game.difficulty )
				{
					case 0:
						defense *= 6;
						attaque *= 10;
						break;

					case 1:
						defense *= 8;
						attaque *= 10;
						break;

					case 3:
						defense *= 14;
						attaque *= 10;
						break;

					case 4:
						defense *= 18;
						attaque *= 10;
						break;
				}
			else if ( ownerDef == Form1.game.curPlayerInd )
				switch ( game.difficulty )
				{
					case 0:
						defense *= 18;
						attaque *= 10;
						break;

					case 1:
						defense *= 14;
						attaque *= 10;
						break;

					case 3:
						defense *= 8;
						attaque *= 10;
						break;

					case 4:
						defense *= 6;
						attaque *= 10;
						break;
				}
			
			/*	
			curAttOwner = ownerAtt;
			curAttUnit = unitAtt;
			curDefOwner = ownerDef;
			curDefUnit = unitDef;
			*/

			for ( int i = 1; i <= game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stack.Length;  i ++ )
				if ( 
					game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stack[ i - 1 ].ind == unitAtt && 
					game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stack[ i - 1 ].player.player == ownerAtt 
					)
				{
					game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stackPos = i;
					break;
				}
			
			for ( int i = 1; i <= game.grid[ game.playerList[ ownerDef].unitList[ unitDef ].X, game.playerList[ ownerDef].unitList[ unitDef ].Y ].stack.Length;  i ++ )
				if ( 
					game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].stack[ i - 1 ].ind == unitDef && 
					game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].stack[ i - 1 ].player.player == ownerDef 
					)
				{
					game.grid[ game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y ].stackPos = i;
					break;
				}

			#region while
			while ( 
				game.playerList[ ownerDef ].unitList[ unitDef ].health > 0 && 
				game.playerList[ ownerAtt ].unitList[ unitAtt ].health > 0 
				)
			{
				Random R = new Random();
				double random = R.NextDouble();

				if ( attaque * random * 2 > defense + .2 )
					game.playerList[ ownerDef ].unitList[ unitDef ].health --;
				else if( attaque * random * 2 < defense - .2 )
					game.playerList[ ownerAtt ].unitList[ unitAtt ].health --;

				if ( 
					ownerDef == Form1.game.curPlayerInd || 
					ownerAtt == Form1.game.curPlayerInd
					)
				{
					zoomOnUnit( ownerDef, unitDef );
					pictureBox1.Update();
					Application.DoEvents();
					Thread.Sleep( delayInCombat );
				}
				else if ( 
					game.playerList[ Form1.game.curPlayerInd ].see[ 
					game.playerList[ ownerAtt ].unitList[ unitAtt ].X, 
					game.playerList[ ownerAtt ].unitList[ unitAtt ].Y
					] && 
					game.playerList[ Form1.game.curPlayerInd ].see[ 
					game.playerList[ ownerDef ].unitList[ unitDef ].X, 
					game.playerList[ ownerDef ].unitList[ unitDef ].Y
					] 
					)
				{
					zoomOnUnit( ownerDef, unitDef );
					Application.DoEvents();
					pictureBox1.Update();
					Thread.Sleep( delayInCombat );
				}
			}
			#endregion

			if ( ownerAtt == Form1.game.curPlayerInd )
			{
				#region attacker == player

				if ( game.playerList[ ownerDef ].unitList[ unitDef ].health <= 0 )
				{ // attack win
					game.playerList[ ownerDef ].unitList[ unitDef ].kill();
				//	game.playerList[ ownerDef ].unitList[ unitDef ].kill();
					
					ai Ai = new ai();
					if ( ai.strongestEnnemy( game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y, ownerAtt) == 0 )
						UnitMove( 
							ownerAtt, 
							unitAtt, 
							game.playerList[ ownerAtt ].unitList[ unitAtt ].X, 
							game.playerList[ ownerAtt ].unitList[ unitAtt ].Y,
							game.playerList[ ownerDef ].unitList[ unitDef ].X, 
							game.playerList[ ownerDef ].unitList[ unitDef ].Y
							);
					else
						move.payThis( ownerAtt, unitAtt, 1 );

					if ( game.playerList[ ownerAtt ].unitList[ unitAtt ].moveLeft > 0 || game.playerList[ ownerAtt ].unitList[ unitAtt ].moveLeftFraction > 0 )
						showUnit( ownerAtt, unitAtt );
					else
						showNextUnit( ownerAtt, unitAtt );
				}
				else if ( game.playerList[ ownerAtt ].unitList[ unitAtt ].health <= 0 )
				{
					selected.state = 0;
					game.playerList[ ownerAtt ].unitList[ unitAtt ].kill();
			//		game.playerList[ ownerAtt ].unitList[ unitAtt ].kill();
					showNextUnit( ownerAtt, unitAtt );
				}
				#endregion
			}
			else if ( ownerDef == Form1.game.curPlayerInd )
			{
				#region defender == player

				if ( game.playerList[ ownerDef ].unitList[ unitDef ].health <= 0 )
				{
					game.playerList[ ownerDef ].unitList[ unitDef ].kill();
			//		game.playerList[ ownerDef ].unitList[ unitDef ].kill();
						
					ai Ai = new ai();
					if ( ai.strongestEnnemy( game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y, ownerAtt) == 0 )
						UnitMove( 
							ownerAtt, 
							unitAtt, 
							game.playerList[ ownerAtt ].unitList[ unitAtt ].X, 
							game.playerList[ ownerAtt ].unitList[ unitAtt ].Y,
							game.playerList[ ownerDef ].unitList[ unitDef ].X, 
							game.playerList[ ownerDef ].unitList[ unitDef ].Y
							);
					else
						move.payThis( ownerAtt, unitAtt, 1 );
				}
				else if ( game.playerList[ ownerAtt ].unitList[ unitAtt ].health <= 0 )
				{
					game.playerList[ ownerAtt ].unitList[ unitAtt ].kill();
			//		game.playerList[ ownerAtt ].unitList[ unitAtt ].kill();
				}
				#endregion
			}
			else
			{ // computer vs computer
				#region else
				if ( game.playerList[ ownerDef ].unitList[ unitDef ].health <= 0 )
				{
			//		game.playerList[ ownerAtt ].unitList[ unitAtt ].kill();
					game.playerList[ ownerDef ].unitList[ unitDef ].kill();
						
					ai Ai = new ai();
					if ( ai.strongestEnnemy( game.playerList[ ownerDef ].unitList[ unitDef ].X, game.playerList[ ownerDef ].unitList[ unitDef ].Y, ownerAtt) == 0 )
						UnitMove( 
							ownerAtt, 
							unitAtt, 
							game.playerList[ ownerAtt ].unitList[ unitAtt ].X, 
							game.playerList[ ownerAtt ].unitList[ unitAtt ].Y,
							game.playerList[ ownerDef ].unitList[ unitDef ].X, 
							game.playerList[ ownerDef ].unitList[ unitDef ].Y
							);
					else
						move.payThis( ownerAtt, unitAtt, 1 );
				}
				else if ( game.playerList[ ownerAtt ].unitList[ unitAtt ].health <= 0 )
				{
				//	selected.state = 0;
					game.playerList[ ownerAtt ].unitList[ unitAtt ].kill();
				}
				#endregion
			}
		}
		#endregion

	#region bombard case

		private void interceptPlaneMovement( UnitList unit, Case ori, Case dest )
		{
			// around lauch
			Case[] sqr = game.radius.returnSquareCases( ori, UnitList.HIGHEST_AAGUN_RANGE );

			for ( int k = 0; k < sqr.Length; k ++ )
				for ( int u = 0; u < sqr[ k ].stack.Length; u ++ )
					if ( 
						unit.player != sqr[ k ].stack[ u ].player &&
						unit.player.isAtWarWith( sqr[ k ].stack[ u ].player ) &&
						sqr[ k ].stack[ u ].typeClass.speciality == enums.speciality.aaGun &&
						sqr[ k ].stack[ u ].anyMoveLeft &&
						sqr[ k ].stack[ u ].typeClass.range >= game.radius.getDistWith( sqr[ k ], ori )
						)
					{
						attackUnit( sqr[ k ].stack[ u ], unit );

						if ( unit.dead )
							break;
					}

			if ( !unit.dead )
			{
				// around case to bomb
				sqr = game.radius.returnSquareCases( dest, UnitList.HIGHEST_AAGUN_RANGE );

				for ( int k = 0; k < sqr.Length; k ++ )
					for ( int u = 0; u < sqr[ k ].stack.Length; u ++ )
						if ( 
							unit.player != sqr[ k ].stack[ u ].player &&
							unit.player.isAtWarWith( sqr[ k ].stack[ u ].player ) &&
							sqr[ k ].stack[ u ].typeClass.speciality == enums.speciality.aaGun &&
							sqr[ k ].stack[ u ].anyMoveLeft &&
							sqr[ k ].stack[ u ].typeClass.range >= game.radius.getDistWith( sqr[ k ], dest )
							)
						{
							attackUnit( sqr[ k ].stack[ u ], unit );

							if ( unit.dead )
								break;
						}
			}
		}
		
	/*	private void bombardCaseByPlane( UnitList unit, Case dest )
		{
			interceptPlaneMovement( unit, unit.pos, dest );

			if ( !unit.dead )
				bombardCase( unit.player.player, unit.ind, x, y, null );
		}*/
		
		private void bombardCase( byte ownerAtt, int unitAtt, int x, int y )
		{
			bombardCase( ownerAtt, unitAtt, x, y, null );
		}

		private void bombardCase( byte ownerAtt, int unitAtt, int x, int y, object sender )
		{
			soundPlayer.play( "bomb.wav", true );

			if ( 
				game.playerList[ ownerAtt ].unitList[ unitAtt ].typeClass.speciality == enums.speciality.bomber ||
				game.playerList[ ownerAtt ].unitList[ unitAtt ].typeClass.speciality == enums.speciality.fighter
				)
				interceptPlaneMovement( 
					game.playerList[ ownerAtt ].unitList[ unitAtt ], 
					game.playerList[ ownerAtt ].unitList[ unitAtt ].onCase, 
					game.grid[ x, y ]
					);

			if ( !game.playerList[ ownerAtt ].unitList[ unitAtt ].dead )
			{
				int attaque;
				byte curAttOwner;
				int curAttUnit;

				attaque = Statistics.units[ game.playerList[ ownerAtt ].unitList[ unitAtt ].type ].attack;
				curAttOwner = ownerAtt;
				curAttUnit = unitAtt;
				pntToBombard = new Point( x, y );
				//attackState = 1;

				// set stackPos for bombarding unit
				for ( int i = 1; i <= game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stack.Length;  i ++ )
					if ( game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stack[ i - 1 ].ind == unitAtt && game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stack[ i - 1 ].player.player == ownerAtt )
					{
						game.grid[ game.playerList[ ownerAtt ].unitList[ unitAtt ].X, game.playerList[ ownerAtt ].unitList[ unitAtt ].Y ].stackPos = i;
						break;
					}

				move.payThis( ownerAtt, unitAtt, 1 ); 
				//	bool[] offended = new bool[ Form1.game.playerList.Length ]; 
			
				#region player - computer
				for ( int k = 0; k < 3; k ++ )
				{
					Random R = new Random();
					double random = R.NextDouble();

					//	selectState = (byte)enums.selectionState.normal;

					#region bomb unit
					for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i ++ )
						if ( attaque * random * 2 > game.grid[ x, y ].stack[ i - 1 ].typeClass.defense )
						{
							game.grid[ x, y ].stack[ i - 1 ].health --;

							if ( game.grid[ x, y ].stack[ i - 1 ].health <= 0 )
							{
							/*	unitDelete( 
									game.grid[ x, y ].stack[ i - 1 ].player.player, 
									game.grid[ x, y ].stack[ i - 1 ].ind 
									);*/
								game.grid[ x, y ].stack[ i - 1 ].kill();
							}
						}
					#endregion

					#region civic improvement
					if ( attaque * random * 10 > 1 )
					{
						if ( game.grid[ x, y ].civicImprovement > 0 && game.grid[ x, y ].roadLevel > 0 )
						{
							if ( R.Next( 2 ) == 0 )
								game.grid[ x, y ].civicImprovement = 0;
							else
							{
								game.grid[ x, y ].roadLevel = 0;
								roads.setAround( game, new Point(x, y) );
							}
						}
						else if ( game.grid[ x, y ].civicImprovement > 0 )
						{
							game.grid[ x, y ].civicImprovement = 0;
						}
						else if ( game.grid[ x, y ].roadLevel > 0 )
						{
							game.grid[ x, y ].roadLevel = 0;
							roads.setAround( game, new Point(x, y) );
						}
					}
					#endregion

					if ( 
						selectState > 3 || 
						(
							game.grid[ x, y ].stack.Length == 0 && 
							game.grid[ x, y ].civicImprovement == 0 && 
							game.grid[ x, y ].roadLevel == 0 
						) 
						)
					{
						guiEnabled = true;
					}

					DrawMap();
					Thread.Sleep( 600 );
				}
				#endregion

				if ( 
					game.grid[ x, y ].territory > 0 &&
					game.grid[ x, y ].territory - 1 != ownerAtt
					)
					game.playerList[ game.grid[ x, y ].territory - 1 ].memory.add( (byte)enums.playerMemory.beenBombed, new int[] { x, y, ownerAtt } );
			}
		}
		#endregion

		private void nukeCase( byte player, int unit, Point dest/*, byte power*/ )
		{
		//	unitDelete( unitsToDestroy[ j ].owner, unitsToDestroy[ j ].unit );
			bool inPlayerSight = ( player == Form1.game.curPlayerInd ); 
			for ( int r = 0; r <= 1 && !inPlayerSight; r ++ ) 
			{ 
				Point[] sqr = game.radius.returnEmptySquare( dest, r ); 
				for ( int k = 0; k < sqr.Length && !inPlayerSight; k ++ ) 
					if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ dest.X, dest.Y ] ) 
						inPlayerSight = true; 
			} 

			oldSliHor = -4;
			if ( inPlayerSight )
				zoomOn( dest );

			if ( inPlayerSight )
			{
				oldSliHor = -4;
				selectState = (byte)enums.selectionState.nukeExplosion;
				zoomOn( dest );
				selectState = (byte)enums.selectionState.normal;

				soundPlayer.play( "nuke.wav", true );
			}

			UnitList[] unitsToDestroy = new UnitList[ 10 ];
			int pos = 0;

			for ( int r = 0; r <= 1; r ++ )
			{
				Point[] sqr = game.radius.returnEmptySquare( dest, r );
				for ( int k = 0; k < sqr.Length; k ++ )
				{
					game.grid[ sqr[ k ].X, sqr[ k ].Y ].militaryImprovement = 0;
					game.grid[ sqr[ k ].X, sqr[ k ].Y ].civicImprovement = 0;

					if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 )
					{
						if ( game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].cityList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].city ].population > 2 )
						{
							game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].cityList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].city ].population /= 2;
						}
						else
						{
							game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].cityList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].city ].state = (byte)enums.cityState.dead;
							game.grid[ sqr[ k ].X, sqr[ k ].Y ].city = 0;
						}
					}
					else if ( Statistics.terrains[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == 1 )
						game.grid[ sqr[ k ].X, sqr[ k ].Y ].polluted = true;

					for ( int u = 0; u < game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length; u ++ )
					{
						if ( pos == unitsToDestroy.Length )
						{
							UnitList[] buffer = unitsToDestroy;
							unitsToDestroy = new UnitList[ buffer.Length + 10 ];

							for ( int i = 0; i < buffer.Length; i ++ )
								unitsToDestroy[ i ] = buffer[ i ];
						}

						unitsToDestroy[ pos ] = game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ u ];
						pos ++;
					}
				}
			}

			for ( int j = 0; j < pos; j ++ )
			{
				if ( 
					unitsToDestroy[ j ].player.player != player &&
					(
					game.playerList[ player ].foreignRelation[ unitsToDestroy[ j ].player.player ].politic == (byte)Form1.relationPolType.ceaseFire ||
					game.playerList[ player ].foreignRelation[ unitsToDestroy[ j ].player.player ].politic == (byte)Form1.relationPolType.peace ||
					game.playerList[ player ].foreignRelation[ unitsToDestroy[ j ].player.player ].politic == (byte)Form1.relationPolType.Protector 
					)
					)
					aiPolitics.declareWar( player, unitsToDestroy[ j ].player.player );

			//	unitDelete( unitsToDestroy[ j ].player.player, unitsToDestroy[ j ].ind );
				unitsToDestroy[ j ].kill();
			}
			
			oldSliHor = -4;
			if ( inPlayerSight )
				zoomOn( dest );
		}

#endregion

#region exit
		private void menuItem4_Click(object sender, System.EventArgs e)
		{ // exit
			//	switch ( MessageBox.Show( "Do you want to save your game before you leave?", "Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 ) )
			switch ( MessageBox.Show( language.getAString( language.order.saveYourGameQ ), language.getAString( language.order.saveYourGameTitle ), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 ) )
			{
				case DialogResult.Yes:
					bool saveSuccessful = false, abortExit = false;

					while ( !saveSuccessful )
						saveSuccessful = game.save();

					/*	if ( game.savePath == "" )
						{ // save as

							saveSuccessful = game.saveAsGetPath();
							
							if( !saveSuccessful )
								game.savePath = "";
						}
						else
						{ // save
							wC.show = true;

							if ( game.save() )
								saveSuccessful = true;
							else
								game.savePath = "";

							wC.show = false;
						}*/

					if ( !abortExit )
						this.Close();

					break;
					
				case DialogResult.No:
					this.Close();
					break;
			}

		}
		#endregion

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			soundPlayer.stop();
			base.OnClosing (e);
		}		

#region show unit
	
	#region show next / previous unit delayed
		public void showNextUnit(byte player, int unit)
		{
			/*bool found = false;

			for ( int i = unit + 1; i <= game.playerList[ owner ].unitNumber; i ++ )
				if ( 
					game.playerList[ owner ].unitList[ i ].state == (byte)Form1.unitState.idle && 
					( 
					game.playerList[ owner ].unitList[ i ].moveLeft > 0 || 
					game.playerList[ owner ].unitList[ i ].moveLeftFraction > 0 
					) &&
					!game.playerList[ owner ].unitList[ i ].automated
					)
				{
					startShowTimer( i ); // showUnit( owner, i );
					found = true;
					break;
				}

			if ( !found )
				for ( int i = 1; i <= unit; i ++ )
					if ( 
						game.playerList[ owner ].unitList[ i ].state == (byte)Form1.unitState.idle && 
						( game.playerList[ owner ].unitList[ i ].moveLeft > 0 || game.playerList[ owner ].unitList[ i ].moveLeftFraction > 0 )  &&
						!game.playerList[ owner ].unitList[ i ].automated)
					{
						startShowTimer( i ); // showUnit( owner, i );
						break;
					}*/

			int u = unitOrder.nextUnit( player, unit );

			if ( u != -1 )
			{
				startShowTimer( u );
		//		return true;
			}
		/*	else
				return false;*/
		}

		public void showPrevUnit(byte player, int unit)
		{
		/*	bool found = false;

			for ( int i = unit - 1; i > 0; i -- )
				if ( game.playerList[ owner ].unitList[ i ].state == (byte)Form1.unitState.idle && ( game.playerList[ owner ].unitList[ i ].moveLeft > 0 || game.playerList[ owner ].unitList[ i ].moveLeftFraction > 0 ) && !game.playerList[ owner ].unitList[ i ].automated )
				{
					startShowTimer( i ); // showUnit( owner, i );
					found = true;
					break;
				}

			if ( !found )
				for ( int i = game.playerList[ owner ].unitNumber; i >= unit; i -- )
					if ( game.playerList[ owner ].unitList[ i ].state == (byte)Form1.unitState.idle && ( game.playerList[ owner ].unitList[ i ].moveLeft > 0 || game.playerList[ owner ].unitList[ i ].moveLeftFraction > 0 ) && !game.playerList[ owner ].unitList[ i ].automated )
					{
						startShowTimer( i ); // showUnit( owner, i );
						break;
					}*/

			int u = unitOrder.prevUnit( player, unit );

			if ( u != -1 )
			{
				startShowTimer( u );
			//	return true;
			}
		/*	else
				return false;*/
		}
		#endregion
	
	#region show next / previous unit Now

		public bool showNextUnitNow( byte player, int unit, Point pos )
		{
			return showNextUnitNow( player, unit );
		}
		public bool showNextUnitNow(byte player, int unit)
		{
			int u = unitOrder.nextUnit( player, unit );

			if ( u != -1 )
			{
				showUnit( player, u );
				return true;
			}
			else
				return false;
		}

		public void showPrevUnitNow(byte player, int unit)
		{

			int u = unitOrder.prevUnit( player, unit );

			if ( u != -1 )
				showUnit( player, u );
		}
		#endregion

		private void startShowTimer( int unit )
		{
			curUnit = unit;
			//tmrShowUnit.Interval = 100;
			tmrShowUnit.Enabled = true;
		}

		private void tmrShowUnit_Tick(object sender, System.EventArgs e)
		{
			tmrShowUnit.Enabled = false;
			showUnit( Form1.game.curPlayerInd, curUnit );
		}

		#region show unit
		public void showUnit( byte owner, int unit )
		{
			if ( unit  != 0 )
			{
				sliHor = game.playerList[ owner ].unitList[ unit ].X - 2;
				sliVer = (int)Math.Floor( (double)game.playerList[ owner ].unitList[ unit ].Y / 2) * 2 - 8;

				if ( sliVer > game.height - 16 )
					sliVer = game.height - 16;
				else if ( sliVer < 0 )
					sliVer = 0;

				if ( sliHor > game.width )
					sliHor -= game.width;
				else if ( sliHor < 0 )
					sliHor += game.width; // - 1

				selected.owner = owner;
				selected.unit = unit;
				selected.state = 1;
				selected.X = game.playerList[ owner ].unitList[ unit ].X;
				selected.Y = game.playerList[ owner ].unitList[ unit ].Y;

				for ( int i = 1; i <= game.grid[ selected.X, selected.Y ].stack.Length; i ++ )
				{
					if ( 
						game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player == selected.owner &&
						game.grid[ selected.X, selected.Y ].stack[ i - 1 ].ind == selected.unit
						)
					{
						game.grid[ selected.X, selected.Y ].stackPos = i;
						break;
					}
				}

				enableMenus();
				showInfo();
				DrawMap();

				if ( 
					owner == Form1.game.curPlayerInd && 
					game.playerList[ Form1.game.curPlayerInd ].cityNumber > 0 &&
					game.playerList[ Form1.game.curPlayerInd ].unitList[ unit ].type == (int)Form1.unitType.colon
					)
					Tutorial.show( this, Tutorial.order.secondSettler );
			}
		}
		#endregion

		/// <summary>
		/// Including DrawMap();
		/// </summary>
		/// <param name="player"></param>
		/// <param name="unit"></param>
		public void zoomOnUnit( byte player, int unit )
		{
			zoomOn( game.playerList[ player ].unitList[ unit ].pos );

		/*	sliHor = game.playerList[ player ].unitList[ unit ].X - (int)(this.ClientSize.Width / ( caseWidth * 2 ) ); // - 2;
			sliVer = (int)Math.Floor( game.playerList[ player ].unitList[ unit ].Y / 2) * 2 - (int)(this.ClientSize.Height / ( caseHeiht * 4 ) ); // - 8;

			if ( sliVer > game.height - 16 )
				sliVer = game.height - 16;
			else if ( sliVer < 0 )
				sliVer = 0;

			if ( sliHor > game.width )
				sliHor -= game.width;
			else if ( sliHor < 0 )
				sliHor += game.width;

			DrawMap();*/
		}

		public void zoomOn( Point pnt )
		{
	//		sliHor = pnt.X - 2;
//			sliVer = (int)( pnt.Y / 2) * 2 - 4; 
			
			sliHor = pnt.X - (int)(this.ClientSize.Width / ( caseWidth * 2 ) ); // - 2;
			sliVer = (int)( pnt.Y / 2) * 2 - (int)(this.ClientSize.Height / ( caseHeight * 4 ) ); // - 8;


			if ( sliVer > game.height - 16 )
				sliVer = game.height - 16;
			else if ( sliVer < 0 )
				sliVer = 0;

			if ( sliHor > game.width )
				sliHor -= game.width;
			else if ( sliHor < 0 )
				sliHor += game.width;

			DrawMap();
		}
		
		/// <summary>
		/// Including DrawMap();
		/// </summary>
		/// <param name="player"></param>
		/// <param name="city"></param>
		public void zoomOnCity( byte player, int city )
		{
			zoomOn( game.playerList[ player ].cityList[ city ].pos );

		/*	sliHor = game.playerList[ player ].cityList[ city ].X - 2;
			sliVer = (int)Math.Floor( game.playerList[ player ].cityList[ city ].Y / 2) * 2 - 4; //  - 8;

			if ( sliVer > game.height - 16 )
				sliVer = game.height - 16;
			else if ( sliVer < 0 )
				sliVer = 0;

			if ( sliHor > game.width )
				sliHor -= game.width;
			else if ( sliHor < 0 )
				sliHor += game.width;

			DrawMap();*/
		}

		#endregion

#region cmdok
		private void cmdOk_Click(object sender, System.EventArgs e)
		{
			
		/*	
			MessageBox.Show( game.radius.getDistWith( new Point( (int)selected.X, 
			(int)selected.Y ), 
			new Point( 2, 
			24 )
			).ToString() );
		*/
			
			
		/*
		//	Radius radius = new Radius();
			structures.pntWithDist[] way = game.radius.findNearestCaseTo( 
				(int)selected.X, 
				(int)selected.Y, 
				(int)game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].X, 
				(int)game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].Y, 
				(byte)1, 
				(byte)0, 
				true 
				);

			if ( way.Length > 0 )
			{
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
					{
						game.grid[ x, y ].selectedLevel = 0;
					}

				game.grid[ way[ 0 ].X, way[ 0 ].Y ].selectedLevel = 1;
				if ( way.Length > 1 )
				game.grid[ way[ 1 ].X, way[ 1 ].Y ].selectedLevel = 2;

				DrawMap();
			}
			else
			{
				MessageBox.Show( " 0 " );
			}
		*/
			
			
		/*	
		//	Radius radius = new Radius();
			System.Drawing.Point[] way = game.radius.findWayTo( 
				(int)selected.X, 
				(int)selected.Y, 
				2,24,
				//(int)game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].X, 
				//(int)game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].Y,
				(byte)(game.grid[ selected.X, selected.Y ].territory - 1),
				(byte)0
				);
			
			structures.pntWithDist[] case1 = game.radius.findNearestCaseTo( 
				(int)selected.X, 
				(int)selected.Y, 
				2,24,
				(byte)1, 
				(byte)( game.grid[ selected.X, selected.Y ].territory - 1 ), 
				true 
				);

			if ( way[ 0 ] != new Point( -1, -1 ) )
			{
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
					{
						game.grid[ x, y ].selectedLevel = 0;
					}

				for ( int i = 0; i < way.Length; i ++ )
					game.grid[ way[ i ].X, way[ i ].Y ].selectedLevel = 1;

				game.grid[ case1[ 0 ].X, case1[ 0 ].Y ].selectedLevel = 2;

				DrawMap();
			}
			else
			{
				MessageBox.Show( " -1, -1 " );
			}
		*/

		}
		#endregion

#region discover radius
	/*	public void discoverRadius( int x, int y, int radius1, byte player )
		{
			Point[] sqr;
			if ( 
				game.grid[ x, y ].type == (byte)enums.terrainType.mountain || 
				game.grid[ x, y ].type == (byte)enums.terrainType.hill 
				)
				sqr = game.radius.returnSquare( x, y, radius1 + 1 );
			else
				sqr = game.radius.returnSquare( x, y, radius1 );

			game.playerList[ player ].discovered[ x , y ] = true;

			for ( int i = 0; i < sqr.Length; i ++ )
			{
				game.playerList[ player ].discovered[ sqr[ i ].X , sqr[ i ].Y ] = true;

				if ( 
					game.grid[ sqr[ i ].X , sqr[ i ].Y ].city > 0 && 
					game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 != player && 
					!game.playerList[ player ].foreignRelation[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 ].madeContact
					)// si il y a une ville autre que la sienne
				{
					game.playerList[ player ].foreignRelation[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 ].madeContact = true;
					game.playerList[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 ].foreignRelation[ player ].madeContact = true;
				}

				if (
					game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack.Length > 0
					)
				{
					for ( int j = 1; j <= game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack.Length; j ++ )
					{
						if (
							game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player != player &&
							( !game.playerList[ player ].foreignRelation[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].madeContact || 
							!game.playerList[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].foreignRelation[ player ].madeContact )
							)
						{
							game.playerList[ player ].foreignRelation[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].madeContact = true;
							game.playerList[ game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].foreignRelation[ player ].madeContact = true;
						}
					}
				}

				if ( game.grid[ sqr[ i ].X , sqr[ i ].Y ].type == (byte)enums.terrainType.sea || game.grid[  sqr[ i ].X , sqr[ i ].Y ].type == (byte)enums.terrainType.coast )
				{
					Point[] sqr2 = game.radius.returnEmptySquare( sqr[ i ].X , sqr[ i ].Y, 1 );
					for ( int j = 0; j < sqr2.Length; j ++ )
					{
						game.playerList[ player ].discovered[ sqr2[ j ].X , sqr2[ j ].Y ] = true;
					}
				}
			}
		}*/
		#endregion

#region Popup	# # #	

	#region mIUnit - popup - Unit
		private void mIUnit_Popup(object sender, EventArgs e)
		{
			if ( 
				selected.state == 1 &&
				selected.owner == Form1.game.curPlayerInd/* &&
				( 
				game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || 
				game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 
				)*/
				)
			{
				if ( game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeft > 0 || game.playerList[ selected.owner ].unitList[ selected.unit ].moveLeftFraction > 0 )
				{
					menuItem5.MenuItems.Clear();
					if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.builder && game.grid[ selected.X, selected.Y ].city == 0 && ( game.grid[ selected.X, selected.Y ].territory == 0 || game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd ) ) // add terrain.ew == unit.ew
						{ // builder
							menuItem5.Enabled = true;

							#region minefield
							if ( game.playerList[ Form1.game.curPlayerInd ].technos[ (byte)Form1.technoList.explosives ].researched )
							{
								MenuItem mIMineFld = new MenuItem();

								mIMineFld.Text = "Mine field";
								menuItem5.MenuItems.Add( mIMineFld );
								if ( game.grid[ selected.X, selected.Y ].militaryImprovement != (byte)enums.militaryImprovement.mineField )
								{
									mIMineFld.Text = "Remove mines " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.mineField, Form1.game.curPlayerInd, selected.unit );
									mIMineFld.Enabled = true;
									mIMineFld.Click += new EventHandler( mIRemoveMines_Click );
								}
								else if ( caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.mineField, Form1.game.curPlayerInd ) )
								{
									mIMineFld.Text += " " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.mineField, Form1.game.curPlayerInd, selected.unit );
									mIMineFld.Enabled = true;
									mIMineFld.Click += new EventHandler( mIMineFld_Click );
								}
								else
								{
									mIMineFld.Enabled = false;
								}
							}
							#endregion

							#region airport
							if ( game.playerList[ Form1.game.curPlayerInd ].technos[ (byte)Form1.technoList.flight ].researched )
							{
								MenuItem mIAirport = new MenuItem();

								mIAirport.Text = "Airport";
								mIAirport.Click += new EventHandler( mIAirport_Click );
								menuItem5.MenuItems.Add( mIAirport );
								if ( game.grid[ selected.X, selected.Y ].militaryImprovement != (byte)enums.militaryImprovement.airport && caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.airport, Form1.game.curPlayerInd ) )
								{
									mIAirport.Text += " " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.airport, Form1.game.curPlayerInd, selected.unit );
									mIAirport.Enabled = true;
								}
								else
								{
									mIAirport.Enabled = false;
								}
							}
							#endregion

							#region fort
							if ( game.playerList[ Form1.game.curPlayerInd ].technos[ (byte)Form1.technoList.masonery ].researched )
							{
								MenuItem mISeparator = new MenuItem();

								mISeparator.Text = "-";

								MenuItem mIFort = new MenuItem();

								mIFort.Text = "Fortification";
								mIFort.Click += new EventHandler( mIFort_Click );
								menuItem5.MenuItems.Add( mIFort );
								menuItem5.MenuItems.Add( mISeparator );
								if ( game.grid[ selected.X, selected.Y ].militaryImprovement != (byte)enums.militaryImprovement.fort && caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.fort, Form1.game.curPlayerInd ) )
								{
									mIFort.Text += " " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 2, (byte)enums.militaryImprovement.fort, Form1.game.curPlayerInd, selected.unit );
									mIFort.Enabled = true;
								}
								else
								{
									mIFort.Enabled = false;
								}
							}
							#endregion

							#region isNextToCity
				//		//	Radius radius = new Radius();
							Point[] sqr = game.radius.returnEmptySquare( selected.X, selected.Y, 1 );
							bool isNextToCity = false;

							for ( int i = 0; i < sqr.Length; i++ )
							{
								if ( game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 )
								{
									isNextToCity = true;
									break;
								}
							}
							#endregion

							#region build city
							MenuItem mICity = new MenuItem();

							if ( !isNextToCity )
							{
								mICity.Text = "City";
								menuItem5.MenuItems.Add( mICity );
								mICity.Click += new EventHandler( mICity_Click );

								MenuItem mISeparator = new MenuItem();

								mISeparator.Text = "-";
								menuItem5.MenuItems.Add( mISeparator );
								//mICity.Click +=new EventHandler(mICity_Click);
							}
							#endregion

							#region build road
							MenuItem mIRoad = new MenuItem();

							if ( game.grid[ selected.X, selected.Y ].roadLevel == 0 && caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 0, 1, Form1.game.curPlayerInd ) )
							{ // road
								mIRoad.Text = "Road " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 0, 0, Form1.game.curPlayerInd, selected.unit );
							}
							else if ( game.grid[ selected.X, selected.Y ].roadLevel == 1 && caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 0, 2, Form1.game.curPlayerInd ) && game.playerList[ Form1.game.curPlayerInd ].technos[ (byte)Form1.technoList.steamPower ].researched )
							{ // rail road
								mIRoad.Text = "Railroad " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 0, 0, Form1.game.curPlayerInd, selected.unit );
							}
							else
							{
								mIRoad.Text = "Road";
								mIRoad.Enabled = false;
							}

							menuItem5.MenuItems.Add( mIRoad );
							mIRoad.Click += new EventHandler( mIRoad_Click );

							#endregion

							#region mine + irrigation
							MenuItem mIMine = new MenuItem();

							mIMine.Text = "Mine";
							if ( game.grid[ selected.X, selected.Y ].civicImprovement != (byte)civicImprovementChoice.mine && caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 1, (byte)civicImprovementChoice.mine, Form1.game.curPlayerInd ) )
							{
								mIMine.Enabled = true;
								mIMine.Text += " " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 1, (byte)civicImprovementChoice.mine, Form1.game.curPlayerInd, selected.unit );
								mIMine.Click += new EventHandler( mIMine_Click );
							}
							else
							{
								mIMine.Enabled = false;
							}

							MenuItem mIIrrigate = new MenuItem();

							mIIrrigate.Text = "Irrigate";
							if ( game.grid[ selected.X, selected.Y ].civicImprovement != (byte)civicImprovementChoice.irrigation && game.radius.isNextToIrrigation( selected.X, selected.Y ) && caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 1, (byte)civicImprovementChoice.irrigation, Form1.game.curPlayerInd ) && ( game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.plain || game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.prairie || game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.desert || game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.tundra ) )
							{
								mIIrrigate.Enabled = true;
								mIIrrigate.Text += " " + caseImprovement.getTurnLeftStringPar( new Point( selected.X, selected.Y ), 1, (byte)civicImprovementChoice.irrigation, Form1.game.curPlayerInd, selected.unit );
								mIIrrigate.Click += new EventHandler( mIIrrigate_Click );
							}
							else
							{
								mIIrrigate.Enabled = false;
							}

							menuItem5.MenuItems.Add( mIIrrigate );
							menuItem5.MenuItems.Add( mIMine );
							#endregion

						}
						else
						{
							menuItem5.MenuItems.Clear();

							MenuItem mIEmpty = new MenuItem();

							menuItem5.MenuItems.Add( mIEmpty );
							menuItem5.Enabled = false;
						}

						mIActions.MenuItems.Clear();
						if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain == 2 )
						{
							MenuItem mIChangeAirport = new MenuItem();

							mIChangeAirport.Text = "Rebase";
							mIActions.MenuItems.Add( mIChangeAirport );
							mIChangeAirport.Click += new EventHandler( mIChangeAirport_Click );
						}

						/*	if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].transport > 0 )
				{
					MenuItem mITransport = new MenuItem(); 
					mIChangeAirport.Text = "Rebase"; 
					mIActions.MenuItems.Add( mIChangeAirport ); 
					mIChangeAirport.Click += new EventHandler(mIChangeAirport_Click); 
				}*/

						#region action - menu depending on speciality
						if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.builder )
						{
							MenuItem mIFindCitySite = new MenuItem();

							mIFindCitySite.Text = "Suggest a site to build city";
							mIFindCitySite.Click += new EventHandler( mIFindCitySite_Click );
							mIActions.MenuItems.Add( mIFindCitySite );

							MenuItem mIAutomateUnit = new MenuItem();

							mIAutomateUnit.Text = "Automate unit";
							mIAutomateUnit.Click += new EventHandler( mIAutomateUnit_Click );
							mIActions.MenuItems.Add( mIAutomateUnit );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == 0 )
						{
							MenuItem mIFortify = new MenuItem();

							if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].entrenchable )
								mIFortify.Text = "Entrench";
							else
								mIFortify.Text = "Defensive position";

							mIActions.MenuItems.Add( mIFortify );
							mIFortify.Click += new EventHandler( mIFortify_Click );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );
							if ( game.playerList[ Form1.game.curPlayerInd ].govType.oppresive )
								if ( game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].territory - 1 == Form1.game.curPlayerInd && game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].city > 0 )
									if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ game.grid[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].X, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].Y ].city ].rioting )
									{
										MenuItem mIRepress = new MenuItem();

										mIRepress.Text = "Repress riot";
										mIActions.MenuItems.Add( mIRepress );
										mIRepress.Click += new EventHandler( mIRepress_Click );
									}
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.bombard )
						{
							MenuItem mIFortify = new MenuItem();

							if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].entrenchable )
								mIFortify.Text = "Entrench";
							else
								mIFortify.Text = "Defensive position";

							mIActions.MenuItems.Add( mIFortify );
							mIFortify.Click += new EventHandler( mIFortify_Click );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );

							MenuItem mIBombard = new MenuItem();

							mIBombard.Text = "Bombard";
							mIActions.MenuItems.Add( mIBombard );
							mIBombard.Click += new EventHandler( mIBombard_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.fighter )
						{
							/*	MenuItem mIChangeAirport = new MenuItem(); 
					mIChangeAirport.Text = "Rebase"; 
					mIActions.MenuItems.Add( mIChangeAirport ); 
					mIChangeAirport.Click += new EventHandler(mIChangeAirport_Click); */
							MenuItem mIAirDefense = new MenuItem();

							mIAirDefense.Text = "Air defense";
							mIActions.MenuItems.Add( mIAirDefense );
							mIAirDefense.Click += new EventHandler( mIAirDefense_Click );

							MenuItem mIAirBombard = new MenuItem();

							mIAirBombard.Text = "Bombard";
							mIActions.MenuItems.Add( mIAirBombard );
							mIAirBombard.Click += new EventHandler( mIAirBombard_Click );

							MenuItem mIRecon = new MenuItem();

							mIRecon.Text = "Recon mission";
							mIActions.MenuItems.Add( mIRecon );
							mIRecon.Click += new EventHandler( mIRecon_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.bomber )
						{
							MenuItem mIAirBombard = new MenuItem();
							mIAirBombard.Text = "Bombard";
							mIActions.MenuItems.Add( mIAirBombard );
							mIAirBombard.Click += new EventHandler( mIAirBombard_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeMissile )
						{
							MenuItem mILaunch = new MenuItem();

							mILaunch.Text = "Launch!";
							mIActions.MenuItems.Add( mILaunch );
							mILaunch.Click += new EventHandler( mILaunch_Click );

							MenuItem mIInstallOnShip = new MenuItem();

							mIInstallOnShip.Text = "Install on ship";
							mIActions.MenuItems.Add( mIInstallOnShip );
							mIInstallOnShip.Click += new EventHandler( mIInstallOnShip_Click );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeBomb )
						{
							/*	MenuItem mIChangeAirport = new MenuItem();
					mIChangeAirport.Text = "Rebase";
					mIActions.MenuItems.Add( mIChangeAirport );
					mIChangeAirport.Click += new EventHandler(mIChangeAirport_Click);*/
							MenuItem mIDrop = new MenuItem();

							mIDrop.Text = "Drop!";
							mIActions.MenuItems.Add( mIDrop );
							mIDrop.Click += new EventHandler( mILaunch_Click );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeIC )
						{
							MenuItem mILaunch = new MenuItem();

							mILaunch.Text = "Launch!";
							mIActions.MenuItems.Add( mILaunch );
							mILaunch.Click += new EventHandler( mILaunch_Click );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.cruiseMissile )
						{
							MenuItem mILaunchStd = new MenuItem();

							mILaunchStd.Text = "Launch";
							mIActions.MenuItems.Add( mILaunchStd );
							mILaunchStd.Click += new EventHandler( mIBombard_Click );

							MenuItem mIInstallOnShip = new MenuItem();

							mIInstallOnShip.Text = "Install on ship";
							mIActions.MenuItems.Add( mIInstallOnShip );
							mIInstallOnShip.Click += new EventHandler( mIInstallOnShip_Click );

							MenuItem mISleep = new MenuItem();

							mISleep.Text = "Sleep";
							mIActions.MenuItems.Add( mISleep );
							mISleep.Click += new EventHandler( mISleep_Click );
						}


						#endregion
				
						if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain <= 1 )
						{
							MenuItem mISetGoTo = new MenuItem();

							mISetGoTo.Text = "Go to";
							mISetGoTo.Enabled = false;
							mISetGoTo.Click += new EventHandler( mISetGoTo_Click );
							mIActions.MenuItems.Add( mISetGoTo );
						}

						#region sell unit and upgrade
						if ( game.grid[ selected.X, selected.Y ].city > 0 && game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd )
						{
							if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.builder )
							{
								MenuItem mIJoinCity = new MenuItem();

								mIJoinCity.Text = String.Format( language.getAString( language.order.miJoinCity ) );//"Join city";
								mIJoinCity.Click += new EventHandler( mIJoinCity_Click );
								mIActions.MenuItems.Add( mIJoinCity );
							}
							else
							{
								MenuItem mISellUnit = new MenuItem();

								mISellUnit.Text = String.Format( language.getAString( language.order.miSellFor ), sell.unitReturn( Form1.game.curPlayerInd, selected.unit ) );;//"Sell for " + sell.unitReturn( Form1.game.curPlayerInd, selected.unit ) + " gold";
								mISellUnit.Click += new EventHandler( mISellUnit_Click );
								if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 )
									mISellUnit.Enabled = false;

								mIActions.MenuItems.Add( mISellUnit );
							}

							if ( 
								Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].obselete != 0 && 
								game.playerList[ Form1.game.curPlayerInd ].technos[ Statistics.units[ Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].obselete ].disponibility ].researched
								)
							{
								MenuItem mIUpdradeUnit = new MenuItem();

								mIUpdradeUnit.Text = String.Format( language.getAString( language.order.miUpgradeUnit ), Statistics.units[ Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].obselete ].name ); //"Sell for " + sell.unitReturn( Form1.game.curPlayerInd, selected.unit ) + " gold";
								mIUpdradeUnit.Click += new EventHandler( mIUpdradeUnit_Click );
								if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 )
									mIUpdradeUnit.Enabled = false;

								mIActions.MenuItems.Add( mIUpdradeUnit );
							}
						}
						#endregion

						#region transport //
					/*	if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 && Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew == 1 )
						{
							MenuItem mIUnloadUnits = new MenuItem();

							mIUnloadUnits.Text = "Unload here";

							int un = 0;

							for ( int i = 0; i < game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported; i++ )
							{
								MenuItem mIUnloadUnit = new MenuItem();

								mIUnloadUnit.Text = uiWrap.unitInfo( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] );
								mIUnloadUnit.Click += new EventHandler( mIUnloadUnit_Click );
								mIUnloadUnits.MenuItems.Add( mIUnloadUnit );
								un++;
							}

							if ( un == 0 )
							{
								mIUnloadUnits.Enabled = false;
							}
							else if ( un > 1 )
							{
								MenuItem mIUnloadAllUnit = new MenuItem();

								mIUnloadAllUnit.Text = "All";
								mIUnloadAllUnit.Click += new EventHandler( mIUnloadAllUnit_Click );
								mIUnloadUnits.MenuItems.Add( mIUnloadAllUnit );
							}

							mIActions.MenuItems.Add( mIUnloadUnits );
						}
						else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain == 1 && transport.spaceInATransport( Form1.game.curPlayerInd, selected.X, selected.Y ) )
						{
							int[] tranports = transport.withFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y );
							MenuItem mIBoardShips = new MenuItem();

							mIBoardShips.Text = "Board";
							for ( int i = 0; i < tranports.Length; i++ )
							{
								MenuItem mIBoardShip = new MenuItem();

								mIBoardShip.Text = uiWrap.unitInfo( Form1.game.curPlayerInd, tranports[ i ] );
								mIBoardShip.Click += new EventHandler( mIBoardShip_Click );
								mIBoardShips.MenuItems.Add( mIBoardShip );
							}

							mIActions.MenuItems.Add( mIBoardShips );
						}*/
						#endregion

						MenuItem mICompleteTurn = new MenuItem();

						mICompleteTurn.Text = "Complete turn";
						mICompleteTurn.Click += new EventHandler( mICompleteTurn_Click );
						mIActions.MenuItems.Add( mICompleteTurn );
						if ( mIActions.MenuItems.Count > 0 )
						{
							mIActions.Enabled = true;
						}
						else
						{
							MenuItem mIEmpty = new MenuItem();

							mIActions.MenuItems.Add( mIEmpty );
							mIActions.Enabled = false;
						}
					}
					else
					{
						MenuItem mIEmpty = new MenuItem();
						menuItem5.MenuItems.Add( mIEmpty );
						MenuItem mIEmpty1 = new MenuItem();
						mIActions.MenuItems.Add( mIEmpty1 );
						menuItem5.Enabled = false;
						mIActions.Enabled = false;
					}

					#region transport
						mITransport.MenuItems.Clear();
						if ( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 && 
							Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew == 1 
							)
						{
							int un = 0;

							for ( int i = 0; i < game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported; i++ )
							{
								MenuItem mIUnloadUnit = new MenuItem();
								mIUnloadUnit.Text = uiWrap.unitInfo( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] );
								mIUnloadUnit.Click += new EventHandler( mIUnloadUnit_Click );
								mITransport.MenuItems.Add( mIUnloadUnit );
								un++;
							}

							if ( un == 0 )
							{
								mITransport.Enabled = false;
							}
							else if ( un > 1 )
							{
								MenuItem mIUnloadAllUnit = new MenuItem();
								mIUnloadAllUnit.Text = "All";
								mIUnloadAllUnit.Click += new EventHandler( mIUnloadAllUnit_Click );
								mITransport.MenuItems.Add( mIUnloadAllUnit );
								mITransport.Enabled = true;
							}

							mITransport.Text = "Unload unit";
						}
						else if ( 
							(
							Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain == 1 && 
							transport.spaceInATransport( Form1.game.curPlayerInd, selected.X, selected.Y ) 
							) ||
							(
							Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain == 2 && 
							transport.spaceInACarrier( Form1.game.curPlayerInd, selected.X, selected.Y ) 
							)
							)
						{
							int[] tranports;
							
							if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain == 1 )
								tranports = transport.withFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y );
							else
								tranports = transport.carrierWithFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y );

							for ( int i = 0; i < tranports.Length; i++ )
							{
								MenuItem mIBoardShip = new MenuItem();

								mIBoardShip.Text = uiWrap.unitInfo( Form1.game.curPlayerInd, tranports[ i ] );
								mIBoardShip.Click += new EventHandler( mIBoardShip_Click );
								mITransport.MenuItems.Add( mIBoardShip );
							}

							mITransport.Enabled = true;
							mITransport.Text = "Load on transport";
						}
					/*	else if ( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 && 
							Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].ew == 0 && 
							game.radius.isNextToLand( selected.X, selected.Y ) 
							)
						{ 
							Point[] sqr = game.radius.returnEmptySquare( selected.X, selected.Y, 1 );
							byte[] rtl = game.radius.returnRelationTypeList( true, true, true, false, true, false );
							bool oneCaseValid = false;

							for ( int k = 0; k < sqr.Length; k ++ )
								if ( 
									Statistics.terrains[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == 1 && 
									!game.radius.caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, Form1.game.curPlayerInd, rtl )
									)
								{
									oneCaseValid = true;
									break;
								}

							if ( oneCaseValid )
							{
								int[] tranports = transport.withFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y );

								for ( int i = 0; i < tranports.Length; i++ )
								{
									MenuItem mIBoardShip = new MenuItem();
									mIBoardShip.Text = uiWrap.unitInfo( Form1.game.curPlayerInd, tranports[ i ] );
									mIBoardShip.Click += new EventHandler( mIBoardShip_Click );
									mITransport.MenuItems.Add( mIBoardShip );
								}

								mITransport.Enabled = true;
								mITransport.Text = "Unload unit to land";
							}
						}	*/
					/*	else if ( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 && 
							(
								Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality != enums.speciality.carrier && 
								transport.withFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y ) > 1 
							) /*||
							(
								Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.carrier && 
								transport.carrierWithFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y ) > 1 
							)/
							)
						{
							int[] tranports;
							
							if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].terrain == 1 )
								tranports = transport.withFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y );
							else
								tranports = transport.carrierWithFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y );

							for ( int i = 0; i < tranports.Length; i++ )
							{
								MenuItem mIBoardShip = new MenuItem();

								mIBoardShip.Text = uiWrap.unitInfo( Form1.game.curPlayerInd, tranports[ i ] );
								mIBoardShip.Click += new EventHandler( mIBoardShip_Click );
								mITransport.MenuItems.Add( mIBoardShip );
							}

							mITransport.Enabled = true;
							mITransport.Text = "Load on transport";
						}*/

					/*	if ( 
							game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported > 0 && 
							(
								Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality != enums.speciality.carrier && 
								transport.withFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y ) > 1 
							) ||
							(
								Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.carrier && 
								transport.carrierWithFreeSpace( Form1.game.curPlayerInd, selected.X, selected.Y ) > 1 
							)
							)
						{
						}*/

						if ( mITransport.MenuItems.Count == 0 )
						{
							MenuItem mIEmpty = new MenuItem();

							mITransport.MenuItems.Add( mIEmpty );
							mITransport.Text = "Transport";
							mITransport.Enabled = false;
						}
			#endregion
			}
			else
			{
				MenuItem mIEmpty0 = new MenuItem();
				menuItem5.MenuItems.Add( mIEmpty0 );
				MenuItem mIEmpty1 = new MenuItem();
				mIActions.MenuItems.Add( mIEmpty1 );
				MenuItem mIEmpty2 = new MenuItem();
				mITransport.MenuItems.Add( mIEmpty2 );

				menuItem5.Enabled = false;
				mIActions.Enabled = false;
				mITransport.Enabled = false;
			}
		}
	#endregion

	#region mIFile - popup - File
		private void mIFile_Popup(object sender, EventArgs e)
		{
			if ( game.savePath == "" )
				mISave.Enabled = false;
			else
				mISave.Enabled = true;
		}
		#endregion

	#region mIQuestion - popup - ?
		private void mIQuestion_Popup(object sender, EventArgs e)
		{
#if DEBUG
			miTest.Enabled = true;
#else
			miTest.Enabled = false;
#endif
	/*		if ( game.playerList[ Form1.game.curPlayerInd ].playerName.StartsWith( "Alexis Laf" ) )
				miTest.Enabled = true;
			else
				miTest.Enabled = false;*/

			if ( game.grid[ selected.X, selected.Y ].stack.Length > 0 )
			{
				mIUnitInfo.MenuItems.Clear();
				mIUnitInfo.Enabled = true;

				bool[] typeUsed = new bool[ Statistics.units.Length ];

				for ( int i = 1; i <= game.grid[ selected.X, selected.Y ].stack.Length; i ++ )
				{
					int type = game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].ind ].type;

					if ( !typeUsed[ type ] )
					{
						typeUsed[ type ] = true;
						MenuItem mINew = new MenuItem();

					//	string bob6 = language.getAString( language.order.infosOn );
						
					//	mINew.Text = String.Format( bob6, Statistics.units[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].typeClass.name.ToLower() );

					/*	string bob6 = String.Format( language.getAString( language.order.infosOn ), Statistics.units[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].typeClass.name.ToLower() );
						mINew.Text = bob6;*/

						mINew.Text = "Infos on " + game.grid[ selected.X, selected.Y ].stack[ i - 1 ].typeClass.name.ToLower();
						mINew.Click += new EventHandler(mIUnitsInfo_Click);
						mIUnitInfo.MenuItems.Add( mINew );
					}

					if ( game.grid[ selected.X, selected.Y ].stack[ i - 1 ].typeClass.transport > 0 )
						for ( int h = 0; h < game.grid[ selected.X, selected.Y ].stack[ i - 1 ].transported; h ++ )
						{
							type = game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.playerList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ i - 1 ].ind ].transport[ h ] ].type;

							if ( !typeUsed[ type ] )
							{
								typeUsed[ type ] = true;
								MenuItem mINew1 = new MenuItem();
								mINew1.Text = String.Format( language.getAString( language.order.infosOn ), Statistics.units[ type ].name.ToLower() );
							//	mINew1.Text = "Infos on " + Statistics.units[ type ].name.ToLower();
								mINew1.Click += new EventHandler(mIUnitsInfo_Click);
								mIUnitInfo.MenuItems.Add( mINew1 );
							}
						}
				}
			}
			else
			{
				mIUnitInfo.Enabled = false;
				MenuItem mIEmpty = new MenuItem();
				mIUnitInfo.MenuItems.Add( mIEmpty );
			}

			int labelInd = label.findAt( selected.X, selected.Y );
			mILabels.MenuItems.Clear();

			if ( labelInd != -1 )
			{ // already a label
				MenuItem mIDelete = new MenuItem();
				mIDelete.Text = language.getAString( language.order.labelDelete ); //"Delete";
				mIDelete.Click += new EventHandler(mIDelete_Click);

				MenuItem mIRename = new MenuItem();
				mIRename.Text = language.getAString( language.order.labelRename ); //"Rename";
				mIRename.Click += new EventHandler(mICreate_Click);

				mILabels.MenuItems.Add( mIDelete );
				mILabels.MenuItems.Add( mIRename );
			}
			else
			{ // no labels
				MenuItem mICreate = new MenuItem();
				mICreate.Text = language.getAString( language.order.labelCreate ); //"Create here";
				mICreate.Click += new EventHandler(mICreate_Click);
				mILabels.MenuItems.Add( mICreate );
			}
			
			mITerrainInfo.Text = String.Format( language.getAString( language.order.infosOn ), Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].name.ToLower() );//"Info on " + Statistics.terrains[ game.grid[ selected.X, selected.Y ].type ].name.ToLower();
		}
#endregion

#region cheat
		/*MenuItem miCheats = new MenuItem( "Cheats" );
		this.mIQuestion*/
#endregion

	#region mICiv - popup - civ
		private void mICiv_Popup(object sender, EventArgs e)
		{
			bool madeContact = false;

			for ( int p = 0; p < game.playerList.Length; p++ )
				if ( p != Form1.game.curPlayerInd && !game.playerList[ p ].dead && game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ p ].madeContact )
				{
					madeContact = true;
					break;
				}

			bool hasAtLeastOneCity = false;

			for ( int c = 1; c <= game.playerList[ Form1.game.curPlayerInd ].cityNumber; c++ )
				if ( game.playerList[ Form1.game.curPlayerInd ].cityList[ c ].state != (byte)enums.cityState.dead )
				{
					hasAtLeastOneCity = true;
					break;
				}

			menuItem27.Enabled = madeContact;
			menuItem19.Enabled = hasAtLeastOneCity;
			menuItem24.Enabled = hasAtLeastOneCity;
			menuItem7.Enabled = hasAtLeastOneCity;
			menuItem12.Enabled = hasAtLeastOneCity;

#if DEBUG
			mISatellites.Enabled = true;
#else
			mISatellites.Enabled = false;
#endif
		}

		#endregion

#endregion

#region guiEnabled
		private bool guiEnabled
		{
			set
			{
				pictureBox1.Enabled = value;
				pictureBox2.Enabled = value;
				pictureBox3.Enabled = value;
				pictureBox4.Enabled = value;

				cmdUp.Enabled = value;
				cmdDown.Enabled = value;
				cmdLeft.Enabled = value;
				cmdRight.Enabled = value;

				cmdTransportUnitRight.Enabled = value; 
				cmdTransportUnitLeft.Enabled = value; 
				cmdUnitRight.Enabled = value; 
				cmdUnitRight.Enabled = value; 

				// timer1.Enabled = value; 
				mIFile.Enabled = value; 
				menuItem13.Enabled = value; 
				menuItem14.Enabled = value; 
				menuItem17.Enabled = value; 
				mICiv.Enabled = value; 
				mIUnit.Enabled = value; 
				mIQuestion.Enabled = value; 
			}
		}
		
		private bool toolBarEnabled
		{
			set
			{
			/*	pictureBox3 = value;
				pictureBox4.Enabled = value;*/

			/*	cmdUp.Enabled = value;
				cmdDown.Enabled = value;
				cmdLeft.Enabled = value;
				cmdRight.Enabled = value;*/

				cmdTransportUnitRight.Enabled = value; 
				cmdTransportUnitLeft.Enabled = value; 
				cmdUnitRight.Enabled = value; 
				cmdUnitRight.Enabled = value; 

			//	// timer1.Enabled = value; 
				mIFile.Enabled = value; 
				menuItem13.Enabled = value; 
				menuItem14.Enabled = value; 
				menuItem17.Enabled = value; 
				mICiv.Enabled = value; 
				mIUnit.Enabled = value; 
				mIQuestion.Enabled = value; 
			}
		}
		#endregion

#region form1 activated
		private void Form1_Activated(object sender, System.EventArgs e)
		{
			try
			{
				if ( game.playerList.Length > 0 )
				{
				DrawMap();
				affMiniMap();

				showInfo();

				showOnScreenDPad = options.showOnScreenDPad;

			//	if ( keyIn != null )
					// timer1.Enabled = true;
				}
			}
			catch
			{
			}
		}

		private void Form1_Deactivate(object sender, EventArgs e)
		{
			// timer1.Enabled = false;
		}
		#endregion

#region cmdUnist right/Left
		private void cmdUnitRight_Clicked(object sender, System.EventArgs e)
		{
			if ( unitAffSli < game.grid[ selected.X, selected.Y ].stack.Length - 3 )
			{
				unitAffSli ++;
				showInfo();
			}
		}

		private void cmdUnitLeft_Clicked(object sender, System.EventArgs e)
		{
			if ( unitAffSli > 0 )
			{
				unitAffSli --;
				showInfo();
			}
		}

		private void cmdTransportUnitRight_Clicked(object sender, System.EventArgs e)
		{
			if ( transportAffSli < game.playerList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].player.player ].unitList[ game.grid[ selected.X, selected.Y ].stack[ game.grid[ selected.X, selected.Y ].stackPos - 1 ].ind ].transported - 3 )
			{
				transportAffSli ++;
				showInfo();
			}
		}

		private void cmdTransportUnitLeft_Clicked(object sender, System.EventArgs e)
		{
			if ( transportAffSli > 0 )
			{
				transportAffSli --;
				showInfo();
			}
		}
		#endregion

#region next / previous cmds
		private void menuItem14_Click(object sender, System.EventArgs e)
		{
			if ( selected.unit <= game.playerList[ Form1.game.curPlayerInd ].unitNumber )
				showPrevUnitNow( Form1.game.curPlayerInd, selected.unit );
			else
				showPrevUnitNow( Form1.game.curPlayerInd, 0 );
		}

		private void menuItem13_Click(object sender, System.EventArgs e)
		{
			if ( selected.unit <= game.playerList[ Form1.game.curPlayerInd ].unitNumber )
				showNextUnitNow( Form1.game.curPlayerInd, selected.unit );
			else
				showNextUnitNow( Form1.game.curPlayerInd, 0 );
		}
#endregion 

#region get treasure
	/*	private void getTreasure( byte attacker, byte defender, int city )
		{
			byte[] posTechno = new byte[ Statistics.technologies.Length ];
			byte nbr = 0;
			for ( int i = 1; i < Statistics.technologies.Length; i ++ )
			{
				if ( 
					game.playerList[ attacker ].technos[ i ].researched &&
					!game.playerList[ defender ].technos[ i ].researched
					)
				{
					posTechno[ nbr ] = (byte)i;
					nbr ++;
				}
			}

			if ( nbr > 0 )
			{
				Random r = new Random();

				byte technoChose = posTechno[ r.Next( nbr - 1 ) + 1 ];

				game.playerList[ attacker ].technos[ technoChose ].researched = true;
			}

			if ( 
				(game.playerList[ defender ].cityList[ city ].population > 6 || game.playerList[ defender ].cityList[ city ].isCapitale ) &&
				game.playerList[ defender ].technos[ (byte)Form1.technoList.mapMaking ].researched )
			{
				for ( int x = 0; x < Form1.game.width; x ++ )
					for ( int y = 0; y < Form1.game.height; y ++ )
					{
						if ( Form1.game.grid[ x, y ].territory - 1 == defender || Form1.game.playerList[ defender ].discovered[ x, y ] )
						{
							Form1.game.playerList[ attacker ].discovered[ x, y ] = true;
						}
					}
			}
			else if ( game.playerList[ defender ].cityList[ city ].population > 3 &&
				game.playerList[ defender ].technos[ (byte)Form1.technoList.mapMaking ].researched  )
			{
				for ( int x = 0; x < Form1.game.width; x ++ )
					for ( int y = 0; y < Form1.game.height; y ++ )
					{
						if ( Form1.game.grid[ x, y ].territory - 1 == defender )
						{
							Form1.game.playerList[ attacker ].discovered[ x, y ] = true;
						}
					}
			}
		}*/
		#endregion

#region on screen dpad
		public bool showOnScreenDPad
		{
			set
			{
				cmdUp.Visible = value;
				cmdLeft.Visible = value;
				cmdDown.Visible = value;
				cmdRight.Visible = value;
			}
			get
			{
				return cmdUp.Enabled;
			}
		}
		#endregion

#region set territory
		private void setTerritory( Point pos, byte player, byte population, bool empty )
		{
			/*	if ( empty )
				{
					Point[] sqr = game.radius.returnSquare( pos.X, pos.Y, population / 2 );
					for ( int d = 0; d < sqr.Length; d ++ )
						if ( 
							game.grid[ sqr[ d ].X, sqr[ d ].Y ].territory == 0 && 
							game.grid[ sqr[ d ].X, sqr[ d ].Y ].type != (byte)enums.terrainType.sea
							)
							if ( game.radius.isOkForExpand( sqr[ d ], player, game.grid[ pos.X, pos.Y ].continent ) )
							{
								game.grid[ sqr[ d ].X, sqr[ d ].Y ].territory = (byte)(player + 1);
								discoverRadius( sqr[ d ].X, sqr[ d ].Y, 1, player );
							}
				}
				else
					for ( int i = 0; i <= population / 2; i ++ )
					{
						Point[] sqr = game.radius.returnSquare( pos.X, pos.Y, i );
						for ( int d = 0; d < sqr.Length; d ++ )
							if ( 
								game.grid[ sqr[ d ].X, sqr[ d ].Y ].territory == 0 && 
								game.grid[ sqr[ d ].X, sqr[ d ].Y ].type != (byte)enums.terrainType.sea
								)
								if ( game.radius.isOkForExpand( sqr[ d ], player, game.grid[ pos.X, pos.Y ].continent ) )
								{
									game.grid[ sqr[ d ].X, sqr[ d ].Y ].territory = (byte)(player + 1);
									discoverRadius( sqr[ d ].X, sqr[ d ].Y, 1, player );
								}
					}*/
			game.frontier.setFrontiers();

		}
		#endregion

#region go to open menu
		private void menuItem26_Click(object sender, System.EventArgs e) 
		{ // go to open menu
		switch ( MessageBox.Show( language.getAString( language.order.saveYourGameQ ), language.getAString( language.order.saveYourGameTitle ), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 ) )
			{ 
				case DialogResult.Yes: 
					game.save();
					showOpening( false );
					break;
					
				case DialogResult.No:
					showOpening( false );
					break;
			}
		}
#endregion

#region open options menu
		private void menuItem25_Click(object sender, System.EventArgs e)
		{ // options
			string titre = this.Text;
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";
		//	byte oldMapType = options.miniMapType;
			bool oldHide = options.hideUndiscovered;
			Options.MiniMapTypes oldMMT = options.miniMapType; 

			FrmOptions frmOptions = new FrmOptions();
			frmOptions.ShowDialog();

			if ( 
				oldHide != options.hideUndiscovered || 
				oldMMT != options.miniMapType
				)
			{
				wC.show = true;
				drawMiniMap();

				oldSliHor = -2;
				DrawMap();
				wC.show = false;
			}
			if ( 
				curLanguage  == null ||
				curLanguage != language.current 
				)
			{
				setLanguage();
			}

			this.Text = titre;
		}
#endregion

#region defaultIni
		private void defaultIni( byte player, int nationTrade )
		{
		//	sight.clearSight( player );
			sight.setAllSight( player );

			if ( nationTrade == -1 )
			{
				nationTrade = Form1.game.playerList[ player ].totalTrade;
			}

		#region spies
			int spyTot = game.playerList[ player ].counterIntNbr;
			Random r = new Random();

			for ( int i = 0; i < game.playerList.Length; i ++ )
				if ( i != player )
				{
					for ( int j = 0; j < 4; j ++ )
					{
						spyTot += game.playerList[ player ].foreignRelation[ i ].spies[ j ].nbr;
					}
				}

			int spyGoal = ( nationTrade * game.playerList[ player ].preferences.intelligence / 100 ) / goldPerSpy;

			int diff = spyGoal - spyTot;
			int addSpy = diff * 75 / 100;

			if ( addSpy > 0 )
			{
				int[] count1 = new int[ game.playerList.Length ];
				int tot = game.playerList[ player ].counterIntNbr + 1;
				for ( int i = 0; i < game.playerList.Length; i ++ )
					if ( game.playerList[ player ].foreignRelation[ i ].madeContact )
						for ( int j = 0; j < 4; j ++ )
						{
							count1[ i ] += game.playerList[ player ].foreignRelation[ i ].spies[ j ].nbr + 1;
							tot += count1[ i ];
						}

				int[] add = new int[ game.playerList.Length ];
				int addCI = game.playerList[ player ].counterIntNbr * addSpy / tot;
				int endTot = addCI;

				for ( int i = 0; i < game.playerList.Length; i ++ ) 
				{ 
					add[ i ] = count1[ i ] * addSpy / tot; 
					endTot += add[ i ]; 
				} 

				Random ra = new Random();

				while ( endTot > addSpy )
				{
					int ran = ra.Next( game.playerList.Length );
					if ( ran == player )
					{
						if ( addCI > 0 )
							addCI --;
					}
					else if ( add[ ran ] > 0 )
						add[ ran ] --;

					endTot = addCI;
					for ( int i = 0; i < game.playerList.Length; i ++ )
						endTot += add[ i ];
				}

				while ( endTot < addSpy )
				{
					int ran = ra.Next( game.playerList.Length );
					if ( ran == player )
						addCI ++; 
					else if ( game.playerList[ player ].foreignRelation[ ran ].madeContact )
						add[ ran ] ++;
					
					endTot = addCI;
					for ( int i = 0; i < game.playerList.Length; i ++ )
						endTot += add[ i ];
				}

				for ( int i = 0; i < game.playerList.Length; i ++ )
					if ( i != player && add[ i ] > 0 )
						frmRelations.addSpyTo( player, (byte)i, add[ i ] );

				game.playerList[ player ].counterIntNbr += addCI;
			}
			else if ( addSpy < 0 )
			{
				int[] count1 = new int[ game.playerList.Length ];
				int tot = game.playerList[ player ].counterIntNbr;

				for ( int i = 0; i < game.playerList.Length; i ++ )
					for ( int j = 0; j < 4; j ++ )
					{
						count1[ i ] += game.playerList[ player ].foreignRelation[ i ].spies[ j ].nbr;
						tot += game.playerList[ player ].foreignRelation[ i ].spies[ j ].nbr;
					}

				int[] remove = new int[ game.playerList.Length ];
				int removeCI = game.playerList[ player ].counterIntNbr * addSpy / tot * -1;
				int endTot = removeCI;
				for ( int i = 0; i < game.playerList.Length; i ++ )
				{
					remove[ i ] = count1[ i ] * addSpy / tot * -1; // pos
					endTot += remove[ i ]; // pos
				}
				// addSpy neg, 
				while ( endTot < addSpy * -1 )
				{ // en enlever + 
					int ran = r.Next( game.playerList.Length );
					if ( ran == player )
					{
						if ( game.playerList[ player ].counterIntNbr > removeCI )
							removeCI++;
					}
					else if ( count1[ ran ] > remove[ ran ] )
						remove[ ran ]++; 

					endTot = removeCI; 
					for ( int i = 0; i < game.playerList.Length; i ++ ) 
						endTot += remove[ i ]; 
				} 

			//	Random ra = new Random();
				while ( endTot > addSpy * -1 )
				{ // en enlever moins
					int ran = r.Next( game.playerList.Length );
					if ( ran == player )
						if ( removeCI > 0 )
							removeCI --;

					if ( remove[ ran ] > 0 )
						remove[ ran ] --;
					
					endTot = removeCI; 
					for ( int i = 0; i < game.playerList.Length; i ++ ) 
						endTot += remove[ i ]; 
				}

				game.playerList[ player ].counterIntNbr -= removeCI; 

				for ( int i = 0; i < game.playerList.Length; i ++ ) 
					if ( i != player && remove[ i ] > 0 )
						frmRelations.removeSpyFrom( player, (byte)i, remove[ i ] );
			} 
			#endregion

			Form1.game.playerList[ player ].slaves.endOfTurn();
			spyEffect.go( player, this );

			/*
			game.playerList[ player ].Sats.spy.endTurn();
			for ( int s = 0; s < game.playerList[ player ].Sats.spy.list.Length; s ++ )
				sight.setSpySatSight(
					player,
					new Point( game.playerList[ player ].Sats.spy.list[ s ].pos, game.playerList[ player ].Sats.spy.list[ s ].path[ game.playerList[ player ].Sats.spy.list[ s ].pos ] ),
					5
					);
			*/

		#region military funding

			int mf = count.militaryFunding( player ); 
			int un = Form1.game.playerList[ player ].totMilitaryUnits; //count.militaryUnits( player ); 

			if ( un < mf / 3 )
			{ // regenarate units
				if ( game.playerList[ player ].technos[ (byte)Form1.technoList.philosophy ].researched )
				for ( int i = 1; i <= game.playerList[ player ].unitNumber; i ++ )
					if ( 
						(
						game.playerList[ player ].unitList[ i ].state == (byte)Form1.unitState.idle ||
						game.playerList[ player ].unitList[ i ].state == (byte)Form1.unitState.turnCompleted ||
						game.playerList[ player ].unitList[ i ].state == (byte)Form1.unitState.fortified ||
						game.playerList[ player ].unitList[ i ].state == (byte)Form1.unitState.sleep
						) &&
						game.playerList[ player ].unitList[ i ].health < 3 &&
						game.grid[ game.playerList[ player ].unitList[ i ].X, game.playerList[ player ].unitList[ i ].Y ].territory - 1 == player
						)
					{
						if (
							game.playerList[ player ].unitList[ i ].moveLeft > 0 ||
							game.playerList[ player ].unitList[ i ].moveLeftFraction > 0
							)
						{
							game.playerList[ player ].unitList[ i ].health ++;
						}
						else if ( game.grid[ game.playerList[ player ].unitList[ i ].X, game.playerList[ player ].unitList[ i ].Y ].city > 0 )
						{
							game.playerList[ player ].unitList[ i ].health ++;

							if ( game.playerList[ player ].unitList[ i ].health < 3 )
								game.playerList[ player ].unitList[ i ].health ++;

							if ( game.playerList[ player ].unitList[ i ].health < 3 )
								game.playerList[ player ].unitList[ i ].health ++;
						}
					}
					else if ( 
						game.playerList[ player ].unitList[ i ].state == (byte)Form1.unitState.inTransport &&
						game.playerList[ player ].unitList[ i ].health < 3 &&
						(
							game.playerList[ player ].unitList[ i ].moveLeft > 0 ||
							game.playerList[ player ].unitList[ i ].moveLeftFraction > 0
						)
						)
					{
						game.playerList[ player ].unitList[ i ].health ++;
					}
			}
			else if ( un > mf * 6 / 3 / 5 )
			{ // delete units
				int diff1 = un - mf / 3;
			}

		#endregion

		#region culture
			int cultureDiff = game.playerList[ player ].culture * - 10 / 100 + game.playerList[ player ].preferences.culture * nationTrade;
			if ( cultureDiff < game.playerList[ player ].culture )
			{
				game.playerList[ player ].culture -= cultureDiff;
			}
			else
			{
				game.playerList[ player ].culture = 0;
			}
			#endregion
 
		#region relation
			if ( player != Form1.game.curPlayerInd )
			{
				for ( int j = 0; j < game.playerList.Length; j ++ )
					if ( game.playerList[ player ].foreignRelation[ j ].madeContact && !game.playerList[ j ].dead )
					{
						byte technoNbr = 0;
						for ( int techno = 1; techno < Statistics.technologies.Length; techno ++ )
							if ( game.playerList[ player ].technos[ techno ].researched != game.playerList[ j ].technos[ techno ].researched )
							{
								technoNbr ++;
							}

						if ( technoNbr > 0 )
						{
							 // initiate nego
						}
					}
			}
		#endregion

		#region sync ships
			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber; i ++ )
				if ( 
					Statistics.units[ Form1.game.playerList[ player ].unitList[ i ].type ].highSeaSync && 
					Form1.game.grid[ Form1.game.playerList[ player ].unitList[ i ].X, Form1.game.playerList[ player ].unitList[ i ].Y ].type ==(byte)enums.terrainType.sea 
					)
				{
					Random ran = new Random();
				/*	if ( ran.Next( 100 ) > 75 )
					{
						if ( player == Form1.game.curPlayerInd )
						{
							string message = "One of your shps has sunk in the ocean";

							if ( Form1.game.playerList[ player ].unitList[ i ].transported > 0 )
							{
								if ( Form1.game.playerList[ player ].unitList[ i ].transported > 1 )
									message += " killing all its passengers:\n";
								else
									message += " killing all its passenger:\n";

								for ( int j = 1; j <= Form1.game.playerList[ player ].unitList[ i ].transported; j ++ )
									message += "  " + uiWrap.unitInfo( player, Form1.game.playerList[ player ].unitList[ i ].transport[ j ] );
							}
							else
								message += "."; 

							MessageBox.Show( message, "Bad news" ); 
						} 

						
						unitDelete( player, i );
					}*/
				}
		#endregion
			
			aiRelations.endTurn( player ); 

			game.playerList[ player ].setResourcesAccess();
			game.frontier.setFrontiers();
			
			structures.memory[] mem = game.playerList[ player ].memory.findAllSince( (byte)enums.playerMemory.launchedICBM, 1 );
			for ( int i = mem.Length - 1; i >= 0 ; i -- )
				nukeCase( player, -1, new Point( mem[ i ].ind[ 0 ], mem[ i ].ind[ 1 ] ) );
		}
#endregion

#region drawUnseen
		private Bitmap drawUnseen ( Bitmap ori )
		{
			Bitmap retour = new Bitmap( ori );

			Color initColor = retour.GetPixel( 0, 0 );

			for ( int x = 0; x < retour.Width; x ++ )
				for ( int y = 0; y < retour.Height; y ++ )
				{
					Color curColor = retour.GetPixel( x, y );

					if ( 
						curColor.R != initColor.R && 
						curColor.G != initColor.G && 
						curColor.B != initColor.B 
						)
						retour.SetPixel( x, y, unseenColor( curColor ) );
				}

			return retour;
		}

		private Color unseenColor( Color ori )
		{
			byte perc = 45;
			Color retour = Color.FromArgb( 
					(byte)( ori.R * perc / 100 ), 
					(byte)( ori.G * perc / 100 ), 
					(byte)( ori.B * perc / 100 ) 
					);

			return retour;
		}
#endregion

#region Mi s

		#region mI Civ...
		private void menuItem19_Click(object sender, System.EventArgs e)
		{	
			string titre = this.Text;
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";

			sciTree sciTree1 = new sciTree();
			sciTree1.ShowDialog();

			//   platformSpec.keys.Set( this );

			this.Text = titre;
		}

		private void menuItem24_Click(object sender, System.EventArgs e)
		{
			string titre = this.Text;
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";

			civPref civPref1 = new civPref();
			civPref1.ShowDialog();

			this.Text = titre;
		}

		private void menuItem27_Click(object sender, System.EventArgs e)
		{
			string titre = this.Text;
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";

			frmRelations frmCivRel = new frmRelations( Form1.game.curPlayerInd );
			frmCivRel.ShowDialog();

			this.Text = titre;
		}

		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			string titre = this.Text;
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";

			civInfo ci = new civInfo();
			ci.ShowDialog();

			this.Text = titre;
		}
		#endregion

		#region mIFindCitySite
		private void mIFindCitySite_Click(object sender, EventArgs e)
		{
			//	ai Ai = new ai();

			Point bob9 = ai.findCitySite( selected.X, selected.Y, Form1.game.curPlayerInd );

			if ( bob9 != new Point( -1, -1 ) )
			{
				game.grid[ bob9.X, bob9.Y ].selectedLevel = 1;

				DrawMap();

				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y< game.height; y ++ )
						game.grid[ x, y ].selectedLevel = 0;
			}
			else
			{
				MessageBox.Show( "We didn't find any valid site in proximity of this unit.", "Sorry" );
			}
		}
		#endregion

		#region mI fighters

		#region mIChangeAirport_Click
		private void mIChangeAirport_Click(object sender, EventArgs e)
		{
			Point[] sqr = game.radius.returnSquare( selected.X, selected.Y, Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].range * 2 );
			for ( int i = 0; i < sqr.Length; i ++ )
			{
				if ( 
					( game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 && 
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 == Form1.game.curPlayerInd ) ||
					( game.grid[ sqr[ i ].X, sqr[ i ].Y ].militaryImprovement == (byte)enums.militaryImprovement.airport &&
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 == Form1.game.curPlayerInd )
					)
				{
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 2;
				}
				else
				{
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
				}
			}
			selectState = (byte)enums.selectionState.planeRebase;
			/*guiEnabled = false;
			// timer1.Enabled = true;*/
			DrawMap();
		}
		#endregion

		#region mIAirDefense_Click
		private void mIAirDefense_Click(object sender, EventArgs e)
		{
			Point[] sqr = game.radius.returnSquare( 
				selected.X, 
				selected.Y, 
				Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].range / 2
				);

			for ( int i = 0; i < sqr.Length; i ++ )
			{
				if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ sqr[ i ].X, sqr[ i ].Y ] )
				{
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
				}
			}

			DrawMap();
			
			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
				{
					game.grid[ x, y ].selectedLevel = 0;
				}

			showNextUnit( Form1.game.curPlayerInd, selected.unit );
		}
		#endregion

		#region mIAirBombard_Click
		private void mIAirBombard_Click(object sender, EventArgs e)
		{
			Point[] sqr = game.radius.returnSquare( 
				selected.X, 
				selected.Y, 
				Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].range 
				);

			for ( int i = 0; i < sqr.Length; i ++ )
			{
				if ( 
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel > 0 || 
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].civicImprovement > 0 || 
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].militaryImprovement > 0 ||
					( 
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 && 
					game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 ].politic == (byte)relationPolType.war
					)
					)
				{
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
				}
				else
				{
					for ( int j = 1; j < game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack.Length; j ++ )
						if (game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].player.player ].politic == (byte)relationPolType.war )
						{
							game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
							break;
						}

					game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 2;
				}
			}

			selectState = (byte)enums.selectionState.bombard;
			DrawMap();
		}
		#endregion

		#region mIRecon_Click
		private void mIRecon_Click(object sender, EventArgs e)
		{
			Point[] sqr = game.radius.returnSquare( selected.X, selected.Y, Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].range * 2 );
			for ( int i = 0; i < sqr.Length; i ++ )
			{
				game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
			}

			selectState = (byte)enums.selectionState.planeRecon;
			DrawMap();
		}
		#endregion

		#endregion

		#region mIBombard
		private void mIBombard_Click(object sender, EventArgs e)
		{
			toolBarEnabled = false;
			Point[] sqr = game.radius.returnSquare( 
				selected.X, 
				selected.Y, 
				Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].range 
				);

			for ( int i = 0; i < sqr.Length; i ++ )
				if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ sqr[ i ].X, sqr[ i ].Y ] )
					if ( 
						game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel > 0 || 
						game.grid[ sqr[ i ].X, sqr[ i ].Y ].civicImprovement > 0 || 
						game.grid[ sqr[ i ].X, sqr[ i ].Y ].militaryImprovement > 0 ||
						( 
						game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 && 
						game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 ].politic == (byte)relationPolType.war 
						)
						)
					{
						game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 2;
					}
					else // units
					{
						for ( int j = 1; j < game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack.Length; j ++ )
							if (game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].player.player ].politic == (byte)relationPolType.war )
							{
								game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
								break;
							}

						game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 1;
					}

			selectState = (byte)enums.selectionState.bombard; 
			curUnit = selected.unit; 
			DrawMap(); 
		}
		#endregion

		#region mIs missiles nuke / conv
		private void mILaunch_Click(object sender, EventArgs e)
		{
			toolBarEnabled = false;
			pictureBox1.Enabled = true;
			pictureBox2.Enabled = true;
			pictureBox3.Visible = false;

			cmdCancel.Visible = true;
			cmdCancel.Click	+= new EventHandler(cmdCancel_Click);

			selectState =(byte)enums.selectionState.launchMissile;

			if ( 
				Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeBomb ||
				Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeMissile
				) 
			{ 
				for ( int r = 1; r < Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].range; r ++ )
				{ 
					Point[] sqr = game.radius.returnEmptySquare( 
						game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].pos, 
						r 
						); 

					for ( int i = 0; i < sqr.Length; i ++ ) 
						if ( game.playerList[ Form1.game.curPlayerInd ].discovered[ sqr[ i ].X, sqr[ i ].Y ] )
							game.grid[ sqr[ i ].X, sqr[ i ].Y ].selectedLevel = 2; 
				} 
				DrawMap();
			} 
			else if ( Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].speciality == enums.speciality.nukeIC )
			{
				for ( int x = 0; x < game.width; x ++ ) 
					for ( int y = 0; y < game.height; y ++ ) 
						game.grid[ x, y ].selectedLevel = 2; 
				DrawMap();
			}
		}

		private void mIDrop_Click(object sender, EventArgs e)
		{

		}

		private void mIInstallOnShip_Click(object sender, EventArgs e)
		{

		}

		private void mILaunchStd_Click(object sender, EventArgs e)
		{

		}
		#endregion

		#region mI label
		private void mICreate_Click(object sender, EventArgs e)
		{
			userTextInput ui = new userTextInput(
				language.getAString( language.order.uiNameLabel ),
				language.getAString( language.order.uiPleaseNameLabel ),
				"",
				language.getAString( language.order.ok ),
				language.getAString( language.order.cancel )
				);
			ui.ShowDialog();
			string rep = ui.result;

			if ( rep != "" )
			{
				label.create( rep, selected.X, selected.Y );

				if ( !options.showLabels )
					System.Windows.Forms.MessageBox.Show( 
						language.getAString( language.order.uiLabelHidden ),
						language.getAString( language.order.uiNameLabel )
						);
			}

			DrawMap();
		}

		private void mIDelete_Click(object sender, EventArgs e)
		{
			label.deleteAt( selected.X, selected.Y );
			DrawMap();
		}
		#endregion

		#region mI infos
		private void mIUnitsInfo_Click( object sender, EventArgs e)
		{
			MenuItem mIBob = (MenuItem)sender;
			string unitName = mIBob.Text.Remove( 0, 9 );

			for ( int i = 0; i < Statistics.units.Length; i ++ )
				if ( Statistics.units[ i ].name.ToLower() == unitName )
				{
					frmInfo fi = new frmInfo( enums.infoType.units, i );
					fi.ShowDialog();
					break;
				}
				
		}

		private void mITerrainInfo_Click(object sender, System.EventArgs e)
		{
			frmInfo fi = new frmInfo( enums.infoType.terrain, game.grid[ selected.X, selected.Y ].type );
			fi.ShowDialog();
		}
		#endregion

		private void mIFortify_Click(object sender, System.EventArgs e) 
		{ 
			game.playerList[ selected.owner ].unitList[ selected.unit ].state = (byte)Form1.unitState.fortified; 
			showNextUnit( Form1.game.curPlayerInd, selected.unit ); 
			DrawMap(); 
		} 

		private void mISleep_Click(object sender, EventArgs e) 
		{ 
			game.playerList[ selected.owner ].unitList[ selected.unit ].state = (byte)Form1.unitState.sleep; 
			showNextUnit( Form1.game.curPlayerInd, selected.unit ); 
			DrawMap(); 
		} 

		private void mIAbout_Click(object sender, EventArgs e)
		{
			MessageBox.Show( 
				"Pocket Humanity\n\nCreated by:\n	Alexis Laferrière\n\nVersion " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\n\nFeedbacks are welcome at: xymus@pockethumanity.com", // 0.72\n
				"About Pocket Humanity", 
				MessageBoxButtons.OK, 
				MessageBoxIcon.None, 
				MessageBoxDefaultButton.Button1 
				);
		}

		private void mIAutomateUnit_Click(object sender, EventArgs e)
		{
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state = (byte)Form1.unitState.idle;
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].automated = true;
			showNextUnit( Form1.game.curPlayerInd, selected.unit );

			DrawMap();
		}

		private void menuItem8_Click(object sender, System.EventArgs e) 
		{ // infotree 
			string titre = this.Text; 
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = ""; 

			frmInfoTree frmIT = new frmInfoTree(); 
			frmIT.ShowDialog(); 

			this.Text = titre; 
		} 

		private void mIJoinCity_Click(object sender, EventArgs e) 
		{ 
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].kill();
		//	unitDelete( Form1.game.curPlayerInd, selected.unit ); 

			game.playerList[ Form1.game.curPlayerInd ].cityList[ game.grid[ selected.X, selected.Y ].city ].population ++; 
			labor.autoAddLabor( Form1.game.curPlayerInd, game.grid[ selected.X, selected.Y ].city ); 

			if ( !showNextUnitNow( Form1.game.curPlayerInd, selected.unit ) ) 
				DrawMap(); 
		} 

#region mIBoardShip
		private void mIBoardShip_Click(object sender, EventArgs e)
		{
			MenuItem Sender = (MenuItem)sender;

			for ( int i = 0; i < game.grid[ selected.X, selected.Y ].stack.Length; i++ ) // tentative
				if ( game.grid[ selected.X, selected.Y ].stack[ i ].player.player == Form1.game.curPlayerInd )
					if ( Sender.Text == uiWrap.unitInfo( Form1.game.curPlayerInd, game.grid[ selected.X, selected.Y ].stack[ i ].ind ) )
					{ // i = transport, selected - 1 == unit
						int transportPos = i;

						if ( i > game.grid[ selected.X, selected.Y ].stackPos - 1 )
							transportPos--;

						move.UnitMove2Transport( Form1.game.curPlayerInd, selected.unit, game.grid[ selected.X, selected.Y ].stack[ i ].ind );

						
						if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ game.grid[ selected.X, selected.Y ].stack[ transportPos ].ind ].moveLeft > 0 )
						{
							selected.unit = game.grid[ selected.X, selected.Y ].stack[ transportPos ].ind;
							selected.unitInTransport = 0;
							DrawMap();
						}
						else if ( !showNextUnitNow( Form1.game.curPlayerInd, selected.unit, new Point( selected.X, selected.Y ) ) )
							DrawMap();

						break;
					}

		}
			
#endregion

		private void mIUnloadUnit_Click(object sender, EventArgs e)
		{
			MenuItem Sender = (MenuItem)sender;

			for ( int i = 0; i < game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported; i ++ )
				if ( Sender.Text == uiWrap.unitInfo( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] ) )
				{
					move.disembarkUnit( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ], selected.unit, new Point( selected.X, selected.Y ) );
					break;
				}

			//if ( !showNextUnitNow( Form1.game.curPlayerInd, selected.unit, new Point( selected.X, selected.Y ) ) )
			DrawMap();
		}

		private void mISellUnit_Click(object sender, EventArgs e)
		{
			if ( MessageBox.Show( 
				"Are you sure you want to sell this " + Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].name + " for " + sell.unitReturn( Form1.game.curPlayerInd, selected.unit ) + " gold?",
				"Selling unit",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.None,
				MessageBoxDefaultButton.Button1
				) == DialogResult.Yes )
			{
				game.playerList[ Form1.game.curPlayerInd ].money += sell.unitReturn( Form1.game.curPlayerInd, selected.unit );
				game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].kill();
		//		unitDelete( Form1.game.curPlayerInd, selected.unit );

				selected.state = 0;

				if ( !showNextUnitNow( Form1.game.curPlayerInd, selected.unit, new Point( selected.X, selected.Y ) ) )
					DrawMap();
			}
		}

		private void mICompleteTurn_Click(object sender, EventArgs e)
		{
		/*	game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].moveLeft = 0;
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].moveLeftFraction = 0;	*/

		//	game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].state = (byte)Form1.unitState.turnCompleted;
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].moveLeft = 0;
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].moveLeftFraction = 0;
			showNextUnitNow( Form1.game.curPlayerInd, selected.unit, new Point( selected.X, selected.Y ) );
		}

		private void miTest_Click(object sender, System.EventArgs e)
		{ // test 
		/*	for ( int x = 0; x < game.width; x ++ ) 
				for ( int y = 0; y < game.height; y ++ ) 
					game.grid[ x, y ].selectedLevel = 0; */
#region test cmded
/* 
			Point[] region = game.radius.regionInvalidForCity( new Point( selected.X, selected.Y ) ); 
 
			for ( int i = 0; i < region.Length; i ++ ) 
				game.grid[ region[ i ].X, region[ i ].Y ].selectedLevel = 1; 
*/ 
			
		/*	Point[] way = game.radius.findWayTo( 
				selected.X, 
				selected.Y, /*29,52,/* 
				game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].X, 
				game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].Y,*/ 
			/*	0, 
				0, 
				true 
				); 
				*/
		//	aiWar.wayToSiteToDefend( Form1.game.curPlayerInd, new Point( selected.X, selected.Y ) ); 
 
				/* 
				planes.findWayToBase( 
				0, 
				8, 
				game.grid[ selected.X, selected.Y ].city, 
				1 
				); 
				*/ 
		/*	if ( game.radius.isNextToWater( selected.X, selected.Y ) )
				MessageBox.Show( "isNextToWater" );
			else
				MessageBox.Show( "is NOT NextToWater" );*/
				
		/*	Point p = ai.findCitySiteWorldWide( new Point( selected.X, selected.Y ), Form1.game.curPlayerInd );
			
			if ( p.X >= 0 )
				game.grid[ p.X, p.Y ].selectedLevel = 2; */

		/*	if (
				Form1.game.grid[ selected.X, selected.Y ].territory != 0 &&
				!game.playerList[ Form1.game.grid[ selected.X, selected.Y ].territory - 1 ].technos[ (byte)Form1.technoList.coastalNavigation ].researched 
				)
				nego.aquireTechno( (byte)( Form1.game.grid[ selected.X, selected.Y ].territory - 1 ), (byte)Form1.technoList.coastalNavigation );
			//	game.playerList[ Form1.game.grid[ selected.X, selected.Y ].territory - 1 ].technos.
 */
		//	way;

			/*int bob99;

			bob99 = 20;*/

		/*	Point[] way = game.radius.findWayTo( 
				new Point( selected.X, selected.Y ), 
				//new Point( 35, 38 ), 
				game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].pos, //new Point( game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].X, game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].Y ), // 
				Statistics.terrains[ Form1.game.grid[ selected.X, selected.Y ].type ].ew, // terrain 
				0, // player 
				false 
				); 
				
	
		/*	Point[] way = aiPlanes.findWayToBase( 0,8,
				new Point( selected.X, selected.Y ),
				game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].pos
				);*/

	/*		if ( way[ 0 ].X >= 0 ) 
			{
				for ( int i = 0; i < way.Length; i ++ ) 
					game.grid[ way[ i ].X, way[ i ].Y ].selectedLevel = 2; 

				game.grid[ way[ 0 ].X, way[ 0 ].Y ].selectedLevel = 1; 
			}

		//	unitDelete( 1, 39 );
			
	 /*aiTransport.whereToDisembarkMilitaryUnits( 0, new Point( selected.X, selected.Y ) ); */

		//	ai Ai = new ai(); 
		/*	Point p = aiWarBomber.findWhereToRebase( 0, 8, new Point( selected.X, selected.Y ) ); 
			game.grid[ p.X, p.Y ].selectedLevel = 2;  
			DrawMap();

			MessageBox.Show( p.X.ToString() + ", " + p.Y.ToString() );*/

	/*		MessageBox.Show( 
				game.radius.getDistWith( 
				new Point( selected.X, selected.Y ), 
				new Point( game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].X, game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].Y )
				).ToString()
				);*/

		///	MessageBox.Show( game.radius.getDistWith( new Point( selected.X, selected.Y ), new Point( game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].X, game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].Y ) ).ToString() );
		//	aiPref.setPrefs( Form1.game.curPlayerInd );

	/*		
			for ( byte player = 0; player < game.playerList.Length; player ++ )
				for ( int city = 1; city <= game.playerList[ player ].cityNumber; city ++ )
	*/		//		labor.addAllLabor( 7, 7 );
	

/*			Random ran = new Random();

			MessageBox.Show( ran.Next( 3 ).ToString() );*/

		/*	game.grid[ selected.X, selected.Y ].roadLevel = 1;
			if ( 
				game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.hill || 
				game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.mountain || 
				game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.forest || 
				game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.jungle || 
				game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.tundra 
				)
				game.grid[ selected.X, selected.Y ].civicImprovement = (byte)Form1.civicImprovementChoice.mine;
			else
				game.grid[ selected.X, selected.Y ].civicImprovement = (byte)Form1.civicImprovementChoice.irrigation;*/

		//	language.populate(); 
		//	MessageBox.Show( language.getAString( language.order.Old ) ); 

		/*	aiWarBomber.findWhereToRebase( 0, 8, new Point( selected.X, selected.Y ) );
			MessageBox.Show( "Done" );*/

		//	createUnit( selected.X, selected.Y, 0, 0 );

		/*	MessageBox.Show( 
				System.Environment.Version.Major.ToString() + "; " +
				System.Environment.Version.Minor.ToString() + "; " +
				System.Environment.Version.Build.ToString() + "; " +
				System.Environment.Version.Revision.ToString()
				);*/


		/*	int b = ( 6 > 2 ? 1 : 0 ); 
			MessageBox.Show( b.ToString() ); 

			b = ( 6 < 2 ? 1 : 0 ); 
			MessageBox.Show( b.ToString() ); /**/

			/*createUnit( 
				selected.X, 
				selected.Y,
				Form1.game.curPlayerInd,
				(byte)Form1.unitType.nukeICBM
				);*/

			/*Point[] way = game.radius.findWayTo( 
					new Point( selected.X, selected.Y ), 
					game.playerList[ Form1.game.curPlayerInd ].cityList[ 1 ].pos, 
					Statistics.terrains[ Form1.game.grid[ selected.X, selected.Y ].type ].ew, 
					0, 
					false 
					);

			if ( way[ 0 ].X != -1 )
			{
				Point[] way1 = new Point[ way.Length + 1 ];
				way1[ 0 ] = new Point( selected.X, selected.Y );

				for ( int i = 0; i < way.Length; i ++ )
					way1[ i + 1 ] = way[ i ];

				plans.add( 0, selected.unit, way1 );


				DrawMap();
			}*/

		//	game.grid[ selected.X, selected.Y ].river = true;

		//	Random R = new Random();
		//	game.grid[ selected.X, selected.Y ].resources = (byte)( 100 + R.Next( Statistics.resources.Length ) );

		/*	createUnit( 
				selected.X, 
				selected.Y,
				0,
				(byte)Form1.unitType.fighter
			);
			createUnit( selected.X, selected.Y, 0, (byte)Form1.unitType.legion );*/

			
			/*Random ran = new Random();
				//int x = selected.X;
				///int y = selected.Y;

				if ( 
					Form1.game.grid[ selected.X, selected.Y ].type != (byte)enums.terrainType.sea &&
					Form1.game.grid[ selected.X, selected.Y ].type != (byte)enums.terrainType.coast &&
					Form1.game.grid[ selected.X, selected.Y ].type != (byte)enums.terrainType.mountain &&
					Form1.game.grid[ selected.X, selected.Y ].type != (byte)enums.terrainType.hill &&
					Form1.game.grid[ selected.X, selected.Y ].type != (byte)enums.terrainType.glacier 
					)
				{
					Point[] way = new Point[ 20 ];
					for ( int p = 0; p < way.Length; p ++ )
						way[ p ].X = -1;

					bool finished = false;
					way[ 0 ] = new Point( -1, -1 );
					way[ 1 ] = new Point( selected.X, selected.Y );

					for ( int p = 1; !finished && p < way.Length + 1; p ++ )
					{
						//	Form1.game.grid[ curPnt.X, curPnt.Y ].river = true;
						Point[] sqr = game.radius.returnEmptySquare( way[ p ], 1 );

						for ( int k = 0; k < sqr.Length; k ++ )
							if ( 
								sqr[ k ] != way[ p - 1 ] &&
								(
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].river || 
								Statistics.terrains[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == 0
								)
								)
							{
								finished = true;
								break;
							}

						if ( !finished )
						{
							int tot = 0;
							Point[] possibilities = new Point[ sqr.Length ];
							for ( int k = 0; k < sqr.Length; k ++ )
								if ( 
									Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.sea &&
									Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.coast &&
									Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.mountain &&
									Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.hill &&
									Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.glacier &&
									!game.radius.isNextTo( sqr[ k ], way[ p - 1 ] )
									)
								{
									bool alreadyUsed = false;
									for ( int j = 1; j < p; j ++ )
										if ( way[ j ] == sqr[ k ] )
											alreadyUsed = true;

									if ( !alreadyUsed )
									{
										possibilities[ tot ] = sqr[ k ];
										tot ++;
									}
								}

							if ( tot > 0 )
								way[ p + 1 ] = possibilities[ ran.Next( tot ) ];
							else
								break;
						}
					}

					if ( finished )
					{
						for ( int p = 1; p < way.Length; p ++ )
							if ( way[ p ].X >= 0 && way[ p ].Y >= 0 )
								Form1.game.grid[ way[ p ].X, way[ p ].Y ].river = true;
							else
								break;
					}
				}*/

		//	game.grid[ selected.X, selected.Y ].river = 15;

		/*	Point[] sqr = game.radius.returnSmallSqrInOrder( new Point( selected.X, selected.Y ) );
			for ( int k = 0; k < sqr.Length; k ++ )
				if ( sqr[ k ].X >= 0 )
					if ( sqr[ k ] == game.radius.returnDownRight( new Point( selected.X, selected.Y ) ) )
					{
						game.grid[ selected.X, selected.Y ].river = (byte)(k + 1);
						game.grid[ sqr[ k ].X, sqr[ k ].Y ].river = 15;
					}*/

		/* 	byte d = 0b10;
			MessageBox.Show( d.ToString() );*/

		/*	frmRelations fr = new frmRelations( Form1.game.curPlayerInd ); 
			fr.ShowDialog(); */
			
		/*	popContactPlayer cn = new popContactPlayer( 0, 1 );
			cn.ShowDialog();*/

		/*	for ( int p1 = 0; p1 < game.playerList.Length; p1 ++ )
				for ( int p0 = 0; p0 < game.playerList.Length; p0 ++ )
					game.playerList[ p0 ].foreignRelation[ p1 ].madeContact = true;	*/

		/*	unitDelete( 5, 11 );
			createUnit( selected.X, selected.Y, (byte)(game.grid[ selected.X, selected.Y ].territory - 1), (byte)Form1.unitType.bomber );
*/
		/*	userChoice uuu = new userChoice( "bob", "Allo comment ca va, moi pas pire et toi... ", new string[] { "aa", "bb", "cc", "dd" }, 1, "Ok", "Cancel" );
			uuu.ShowDialog();
			int res = uuu.result;
			MessageBox.Show( res.ToString() );
			
			userTextInput iii = new userTextInput( "bob", "Allo comment ca va, moi pas pire et toi... Allo comment ca va, moi pas pire et toi... ", "bob moon", "Ok", "Cancel" );
			iii.ShowDialog();
			MessageBox.Show( iii.result );

			userNumberInput ttt = new userNumberInput( "bob", "Allo comment ca va, moi pas pire et toi... Allo comment ca va, moi pas pire et toi... ", 245, 500, "Ok", "Cancel" );
			ttt.ShowDialog();
			res = ttt.result;
			MessageBox.Show( res.ToString() );*/

		/*	Point p = aiWarBomber.findCaseToBombard( 5, 11 );
			if ( p.X != -1 )
				game.grid[ p.X, p.Y ].selectedLevel = 2;*/

		/*	Point[] way = aiPlanes.findWayToBase( 
				5, 
				6, 
				new Point( 36, 31 ), 
				new Point( 29, 32 )
				);

			if ( way[ 0 ].X >= 0 ) 
			{
				for ( int i = 0; i < way.Length; i ++ ) 
					game.grid[ way[ i ].X, way[ i ].Y ].selectedLevel = 2; 

				game.grid[ way[ 0 ].X, way[ 0 ].Y ].selectedLevel = 1; 
			}*/

		/*	frmNegociation fn = new frmNegociation( 0, 1 );
			fn.ShowDialog();
			
			for ( int x = 0; x < game.width; x++ )
				for ( int y = 0; y < game.height; y++ )
					if ( game.grid[ x, y ].militaryImprovement == (byte)enums.militaryImprovement.airport )
						game.grid[ x, y ].militaryImprovement = 0;*/

		/*	if ( game.playerList[ Form1.game.curPlayerInd ].Sats == null )
			{
				Random r = new Random();

				game.playerList[ Form1.game.curPlayerInd ].Sats = new sats( 0 );

				for ( int i = 0; i < 6; i ++ )
					game.playerList[ Form1.game.curPlayerInd ].Sats.spy.addSat( r.Next( Form1.game.width ) );

			//	game.playerList[ Form1.game.curPlayerInd ].Sats.spy.addSat();
			}
			else
			{
				game.playerList[ Form1.game.curPlayerInd ].Sats.spy.endTurn();

				for ( int s = 0; s < game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list.Length; s++ )
					sight.setSpySatSight( 0, new Point( game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].pos, game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].path[ game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].pos ] ), 5 );
			}*/

		/*	for ( int u = 1; u <= game.playerList[ 3 ].unitNumber; u ++ )
				for ( int t = 0; t < game.playerList[ 3 ].unitList[ u ].transported; t ++ )
					if ( game.playerList[ 3 ].unitList[ u ].transport[ t ] == 35 )
					{
						MessageBox.Show( "Found unit! yay transport:" + u.ToString() );
					//	game.playerList[ 3 ].unitList[ 23 ].state = (byte)Form1.unitState.inTransport;
					}*/

		//	createUnit( selected.X, selected.Y, (byte)0, (byte)Form1.unitType.bomber );

		//	unitDelete( 0, 1 );

		/*	popAddNegociation pan = new popAddNegociation(
				new byte[] { 0, 1 },
				nego.returnPossibleBilateralExchanges( 0, 1 ),
				nego.returnPossibleUnilateralExchanges( 0, 1 ),
				nego.returnPossibleUnilateralExchanges( 1, 0 )
				);
			pan.ShowDialog();	*/

		/*	frmNegociation fn = new frmNegociation( 0, 1, -1 );
			fn.ShowDialog();	*/

			/*for ( int i = game.grid[ selected.X, selected.Y ].stack.Length - 1; i >= 0; i-- )
			{
				unitDelete( game.grid[ selected.X, selected.Y ].stack[ i ].player.player, game.grid[ selected.X, selected.Y ].stack[ i ].unit );
			}*/


		/*	for ( int p = 0; p < game.playerList.Length; p ++ )
				for ( int u = 1; u <= game.playerList[ p ].unitNumber; u++ )
					if ( game.playerList[ p ].unitList[ u ].pos == new Point( 0, 0 ) )
						game.playerList[ p ].unitList[ u ].state = (byte)Form1.unitState.dead;*/

#endregion

		//	MessageBox.Show( Form1.game.grid[ selected.X, selected.Y ].roadLevel.ToString() );

		//	game.playerList[ Form1.game.curPlayerInd ].cityList[ game.grid[ selected.X, selected.Y ].city ].slaves.add( 3 );

		//	MessageBox.Show( "This will prevent the 
		/*	Random r = new Random();
			switch ( r.Next( 3 ) )
			{
				case 0:
					createUnit( selected.X, selected.Y, (byte)0, (byte)Form1.unitType.nukeBomb );
					break;

				case 1 :
					createUnit( selected.X, selected.Y, (byte)0, (byte)Form1.unitType.nukeICBM );
					break;

				case 2 :
					createUnit( selected.X, selected.Y, (byte)0, (byte)Form1.unitType.nukeTactical );
					break;
			}*/

		/*	string t = "";
			for ( int i = 0; i < Form1.game.grid[ selected. X, selected.Y ].roadDir.Length; i ++ )
			{
				t += i.ToString() + " ";

				if ( Form1.game.grid[ selected. X, selected.Y ].roadDir[ i ].rail )
					t += "rail";
				else if ( Form1.game.grid[ selected. X, selected.Y ].roadDir[ i ].road )
					t += "road";
				else
					t += "n";

				t += "\n";
			}

			Point[] sqr = game.radius.returnSmallSqrInOrder( new Point( selected.X, selected.Y ) );

			for ( int i = 0; i < sqr.Length; i++ )
			{
				t += i.ToString() + " ";

				if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 2 )
					t += "rail";
				else if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 1 )
					t += "road";
				else
					t += "n";

				t += "\n";

			}

			string title;

			if ( Form1.game.grid[ selected.X, selected.Y ].roadLevel == 2 )
				title = "Rail";
			else if ( Form1.game.grid[ selected.X, selected.Y].roadLevel == 1 )
				title = "road";
			else
				title = "n";

			roads.setOne( new Point( selected.X, selected.Y ) );

			MessageBox.Show( t, title );	*/

	/*		if ( unitOrder.list == null )
				unitOrder.setOrder( Form1.game.curPlayerInd );
			else
				MessageBox.Show( unitOrder.nextUnit( Form1.game.curPlayerInd, selected.unit ).ToString() );	*/

	/*		statsLoader.Loader loader = new xycv_ppc.statsLoader.Loader( this );
			loader.startAsync();
			loader.showOrComplete();
			MessageBox.Show( "Fini" );*/

	//		roads.setOne( game, new Point( selected.X, selected.Y ) );

		/*	caseImprovement.destroyCaseImps( 0 );
			caseImprovement.destroyCaseImps( 0 );
			caseImprovement.destroyCaseImps( 0 );
			caseImprovement.destroyCaseImps( 0 );*/

	/*		structures.caseImprovement[] buffer = game.caseImps;
			game.caseImps = new structures.caseImprovement[ buffer.Length - 4 ];

			for ( int i = 0; i < game.caseImps.Length; i ++ )
				game.caseImps[ i ] = buffer[ i + 4 ];*/

	//		game.curPlayer.kill( game.playerList[ 1 ] );

			game.curPlayer.unitList[ 1 ].kill( game.playerList[ 1 ]);
		}

		private void mIRepress_Click(object sender, EventArgs e)
		{

		}

		private void mIRemoveMines_Click(object sender, EventArgs e)
		{

		}

		private void mISetGoTo_Click(object sender, EventArgs e)
		{
			selectState = (byte)enums.selectionState.goTo;
		}

#endregion
		
#region give city
	/*	public static void giveCity( byte receiver, byte giver, int city )
		{
			int x = Form1.game.playerList[ giver ].cityList[ city ].X,
				y = Form1.game.playerList[ giver ].cityList[ city ].Y;

		#region move units

			Case nearestCity = null;
			int lastDist = game.width * 5;

			for ( int i = 0; i < Form1.game.playerList[ giver ].cityNumber; i ++ )
				if ( nearestCity == null || game.radius.getDistWith( Form1.game.playerList[ giver ].cityList[ i ].onCase, Form1.game.playerList[ giver ].cityList[ city ].onCase ) < lastDist )
				{
					nearestCity = Form1.game.playerList[ giver ].cityList[ i ].onCase;
					lastDist = game.radius.getDistWith( Form1.game.playerList[ giver ].cityList[ i ].onCase, Form1.game.playerList[ giver ].cityList[ city ].onCase );
				}


			if ( nearestCity != null )
				for ( int u = Form1.game.grid[ x, y ].stack.Length - 1; u >= 0; u -- )
					UnitMove( Form1.game.grid[ x, y ].stack[ u ], nearestCity );

		#endregion

			Form1.game.playerList[ receiver ].cityNumber ++;
			
			if ( Form1.game.playerList[ receiver ].cityNumber >= Form1.game.playerList[ receiver ].cityList.Length )
			{
				CityList[] cityListBuffer = Form1.game.playerList[ receiver ].cityList;
				Form1.game.playerList[ receiver ].cityList = new CityList[ cityListBuffer.Length + 5 ];

				for ( int i = 0; i < cityListBuffer.Length; i ++ )
					Form1.game.playerList[ receiver ].cityList[ i ] = cityListBuffer[ i ];
			}

			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ] = new CityList( Form1.game.playerList[ receiver ], Form1.game.playerList[ receiver ].cityNumber );

			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].buildingList = 
				Form1.game.playerList[ giver ].cityList[ city ].buildingList;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].foodReserve = 
				Form1.game.playerList[ giver ].cityList[ city ].foodReserve;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].laborOnField = 
				Form1.game.playerList[ giver ].cityList[ city ].laborOnField;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].laborPos = 
				Form1.game.playerList[ giver ].cityList[ city ].laborPos;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].name = 
				Form1.game.playerList[ giver ].cityList[ city ].name;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].population = 
				Form1.game.playerList[ giver ].cityList[ city ].population;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].X = 
				Form1.game.playerList[ giver ].cityList[ city ].X;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].Y = 
				Form1.game.playerList[ giver ].cityList[ city ].Y;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].originalOwner =
				Form1.game.playerList[ giver ].cityList[ city ].originalOwner;

			Form1.game.playerList[ giver ].cityList[ city ].state = (byte)enums.cityState.dead;

			//	Form1.game.grid[ x, y ].territory - 1 = receiver;
			Form1.game.grid[ x, y ].city = Form1.game.playerList[ receiver ].cityNumber;

			for ( int u = Form1.game.grid[ Form1.game.playerList[ giver ].cityList[ city ].X, Form1.game.playerList[ giver ].cityList[ city ].Y ].stack.Length - 1; u >= 0; u-- )
			{
				// to do: move unit or delete them
			}

			Form1.game.frontier.setFrontiers();
			sight.setTerritorySight( receiver );

			//	game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].laborPos = new Point[ 0 ];
			labor.removeAllLabor( receiver, Form1.game.playerList[ receiver ].cityNumber );
			labor.addAllLabor( receiver, Form1.game.playerList[ receiver ].cityNumber );
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].setHasDirectAccessToRessource();

			if ( receiver == Form1.game.curPlayerInd )
				uiWrap.choseConstruction( receiver, Form1.game.playerList[ receiver ].cityNumber, true );
			else
				ai.chooseConstruction( receiver, Form1.game.playerList[ receiver ].cityNumber );
		}*/
		#endregion

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					game.grid[ x, y ].selectedLevel = 0;

			selectState = (byte)enums.selectionState.normal;
			cmdCancel.Visible = false;
			guiEnabled = true;
			DrawMap();
		}

#region resize
	//	private void Form1_Resize(object sender, EventArgs e)

		protected override void OnResize(EventArgs e)
		{
			base.OnResize( e );

			if ( this.Width != 0 && this.Height != 0 )
			{
				guiEnabled = false;
				tmrResize.Enabled = false;
				tmrResize.Enabled = true;

			/*	this.pictureBox1.Size = this.ClientSize;
				this.pictureBox2.Location = new Point( this.Width - pictureBox2.Width - 4, this.ClientSize.Height - pictureBox2.Height - 4 );
				pictureBox2.Size = new Size( (int)( this.Width * 0.35 ), (int)( this.Width * 0.225 ) );
				if ( pictureBox2.Width > 200 )
					pictureBox2.Size = new Size( 200, (int)( 200 * .64 ) );

				pictureBox2.Left = this.ClientSize.Width - pictureBox2.Width - 4;
				pictureBox2.Top = this.ClientSize.Height - pictureBox2.Height - 4;
				pictureBox3.Top = this.ClientSize.Height - pictureBox3.Height;
				pictureBox4.Top = this.ClientSize.Height - pictureBox4.Height - pictureBox3.Height;
				cmdUnitLeft.Top = this.ClientSize.Height - pictureBox3.Height / 2 - cmdUnitLeft.Height / 2;
				cmdUnitRight.Top = this.ClientSize.Height - pictureBox3.Height / 2 - cmdUnitLeft.Height / 2;
				cmdTransportUnitLeft.Top = this.ClientSize.Height - pictureBox4.Height / 2 - cmdUnitLeft.Height / 2 - pictureBox3.Height;
				cmdTransportUnitRight.Top = this.ClientSize.Height - pictureBox4.Height / 2 - cmdUnitLeft.Height / 2 - pictureBox3.Height;
				
				if ( g != null )
				{
					oldSliVer = -4;
					oldSliHor = -2;
					DrawMap();
				}*/
			}
			else
			{
				tmrResize.Enabled = false;
			}
		}

		private void tmrResize_Tick(object sender, EventArgs e)
		{
			tmrResize.Enabled = false;
			
			this.pictureBox1.Size = this.ClientSize;
			this.pictureBox2.Location = new Point( this.Width - pictureBox2.Width - 4, this.ClientSize.Height - pictureBox2.Height - 4 );
			pictureBox2.Size = new Size( (int)( this.Width * 0.35 ), (int)( this.Width * 0.225 ) );
			
			if ( pictureBox2.Width > 200 )
				pictureBox2.Size = new Size( 200, (int)( 200 * .64 ) );

			pictureBox2.Left = this.ClientSize.Width - pictureBox2.Width - 4;
			pictureBox2.Top = this.ClientSize.Height - pictureBox2.Height - 4;
			pictureBox3.Top = this.ClientSize.Height - pictureBox3.Height;
			pictureBox4.Top = this.ClientSize.Height - pictureBox4.Height - pictureBox3.Height;
			
			cmdUnitLeft.Top = this.ClientSize.Height - pictureBox3.Height / 2 - cmdUnitLeft.Height / 2;
			cmdUnitRight.Top = this.ClientSize.Height - pictureBox3.Height / 2 - cmdUnitLeft.Height / 2;
			cmdTransportUnitLeft.Top = this.ClientSize.Height - pictureBox4.Height / 2 - cmdUnitLeft.Height / 2 - pictureBox3.Height;
			cmdTransportUnitRight.Top = this.ClientSize.Height - pictureBox4.Height / 2 - cmdUnitLeft.Height / 2 - pictureBox3.Height;
			

			if ( g != null && game != null )
			{
				oldSliVer = -4;
				oldSliHor = -2;

				drawMiniMap();
				DrawMap();
			}

			guiEnabled = true;
		}
#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
		//	base.OnPaint( e );
			e.Graphics.Clear( Color.Black );
		}

		/*	private void playerIsDead
							MessageBox.Show( 
								"You have been destroyed by " + game.playerList[ owner ].playerName,  
								"You lost the war"
								);

							game.playerList = null;
						
							showOpening( false );*/

/*		private void playerLost( int player, int killer )
		{
			/*game.playerList[ player ].dead = true;
			if ( killer == -1 )
				MessageBox.Show( 
					game.playerList[ player ].playerName + " of " + Statistics.civilizations[ game.playerList[ ancientOwner ].civType ].name + "."
					);*/
	/*		if ( killer == Form1.game.curPlayerInd )
				MessageBox.Show( 
				"You have destroyed " + game.playerList[ player ].playerName + " of " + Statistics.civilizations[ game.playerList[ player ].civType ].name + ".",  
					"You won the war"
					);
			else
				MessageBox.Show( 
				game.playerList[ player ].playerName + " of " + Statistics.civilizations[ game.playerList[ player ].civType ].name + " has been destroyed...",  
				game.playerList[ killer ].playerName + " won the war"
					);*
		}*/

		private void mIUnloadAllUnit_Click(object sender, EventArgs e)
		{
			//MenuItem Sender = (MenuItem)sender;

			for ( int i = game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transported - 1; i >= 0; i -- )
				//if ( game.playerList[ Form1.game.curPlayerInd ].unitList[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ] ].mo )
				move.disembarkUnit( Form1.game.curPlayerInd, game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].transport[ i ], selected.unit, new Point( selected.X, selected.Y ) );

			DrawMap();
		}

		private void mIUpdradeUnit_Click(object sender, EventArgs e)
		{
/*			int x = selected.X, y = selected.Y;
			byte unitType = Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].obselete;
			
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].kill();
		//	unitDelete( Form1.game.curPlayerInd, selected.unit );
			createUnit( x, y, Form1.game.curPlayerInd, unitType );

			game.playerList[ Form1.game.curPlayerInd ].unitList[ game.playerList[ Form1.game.curPlayerInd ].unitNumber ].moveLeft = 0;
*/
			game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].upgrade();

			showNextUnit( Form1.game.curPlayerInd, selected.unit );
		//	DrawMap();
		}

		private void mISatellites_Click(object sender, EventArgs e)
		{
			frmSatellites fs = new frmSatellites();
			fs.ShowDialog();
		}

	//	private void Form1_KeyDown(object sender, KeyEventArgs e)

#region OnKeyDown
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch ( e.KeyCode )//keyIn.testButtons( ) )
			{
				case System.Windows.Forms.Keys.Down:
					goDown();
					break;

				case System.Windows.Forms.Keys.Up :
					goUp();
					break;

				case System.Windows.Forms.Keys.Right :
					goRight();
					break;

				case System.Windows.Forms.Keys.Left :
					goLeft();
					break;

				case System.Windows.Forms.Keys.C :
					if ( 
						selected.state == 1 && 
						selected.unit > 0 && 
						selected.owner == Form1.game.curPlayerInd && 
						Statistics.units[ game.playerList[ Form1.game.curPlayerInd ] .unitList[ selected.unit ].type ].speciality == enums.speciality.builder &&
						game.grid[ selected.X, selected.Y ].city == 0 && 
						( 
							game.grid[ selected.X, selected.Y ].territory == 0 || 
							game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd 
						) &&
						!game.radius.isNextToCity( new Point( selected.X, selected.Y ) )
						)
						mICity_Click( "key C", new System.EventArgs() );
					break; // build city

				case System.Windows.Forms.Keys.I :
					if ( 
						selected.state == 1 && 
						selected.unit > 0 && 
						selected.owner == Form1.game.curPlayerInd && 
						Statistics.units[ game.playerList[ Form1.game.curPlayerInd ] .unitList[ selected.unit ].type ].speciality == enums.speciality.builder &&
						game.grid[ selected.X, selected.Y ].city == 0 && 
						( 
							game.grid[ selected.X, selected.Y ].territory == 0 || 
							game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd 
						) &&
						game.grid[ selected.X, selected.Y ].civicImprovement != (byte)civicImprovementChoice.irrigation && 
						game.radius.isNextToIrrigation( selected.X, selected.Y ) && 
						caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 1, (byte)civicImprovementChoice.irrigation, Form1.game.curPlayerInd ) && 
						(
							game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.plain || 
							game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.prairie || 
							game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.desert || 
							game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.tundra 
						)
						)
						mIIrrigate_Click( "key I", new System.EventArgs() );
					break; // build city

				case System.Windows.Forms.Keys.M :
					if ( 
						selected.state == 1 && 
						selected.unit > 0 && 
						selected.owner == Form1.game.curPlayerInd && 
						Statistics.units[ game.playerList[ Form1.game.curPlayerInd ] .unitList[ selected.unit ].type ].speciality == enums.speciality.builder &&
						game.grid[ selected.X, selected.Y ].city == 0 && 
						( 
							game.grid[ selected.X, selected.Y ].territory == 0 || 
							game.grid[ selected.X, selected.Y ].territory - 1 == Form1.game.curPlayerInd 
						) &&
						game.grid[ selected.X, selected.Y ].civicImprovement != (byte)civicImprovementChoice.mine && 
						caseImprovement.canBuildImprovement( new Point( selected.X, selected.Y ), 1, (byte)civicImprovementChoice.mine, Form1.game.curPlayerInd )
						)
						mIMine_Click( "key M", new System.EventArgs() );
					break; // build city
			}
		}
#endregion

	}
}
