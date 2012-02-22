using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for FrmCity.
	/// </summary>
	public class FrmCity : System.Windows.Forms.Form
	{
		ContextMenu cmMore;
		MenuItem miSetAsCapital, miRename, miEvacuate, miTransfertSlave;
		Rectangle[] peopleRects;

		public FrmCity( byte player1, int city1 ) //, string name1
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.Text = Form1.game.playerList[ player1 ].cityList[ city1 ].name;

			listConst = new ArrayList();

			getPFT1 = new getPFT(); 
			player = player1; 
			city = city1; 
			lbBuildings.Items.Clear();

			for ( byte i = 0; i < Form1.game.playerList[ player ].cityList[ city ].buildingList.Length; i ++ ) 
				if ( Form1.game.playerList[ player1 ].cityList[ city1 ].buildingList[ i ] )
				{
					listConst.Add( i );
					lbBuildings.Items.Add( Statistics.buildings[ i ].name );
				}

			int tot = 0;
			for ( int c = 1; c <= Form1.game.playerList[ player ].cityNumber; c ++ )
				if ( Form1.game.playerList[ player ].cityList[ c ].state != (byte)enums.cityState.dead )
					tot ++;

			if ( tot <= 1 )
			{
				cmdNext.Enabled = false; 
				cmdPrev.Enabled = false; 
			}

			cmdNext.Text = language.getAString( language.order.next );
			cmdPrev.Text = language.getAString( language.order.previous );

			miSetAsCapital = new MenuItem();
			miSetAsCapital.Text = language.getAString( language.order.citySetAsCapital );
			miSetAsCapital.Click += new EventHandler(cmdSetAsCapital_Click);
			miSetAsCapital.Enabled = false;

			miRename = new MenuItem();
			miRename.Text = language.getAString( language.order.cityRename );
			miRename.Click += new EventHandler( cmdRename_Click );

			miEvacuate = new MenuItem();
			miEvacuate.Text = language.getAString( language.order.cityEvacuate );
			miEvacuate.Click += new EventHandler( cmdEvacuate_Click );


			miTransfertSlave = new MenuItem();
			miTransfertSlave.Text = "Transfert slaves";//language.getAString( language.order.cityEvacuate );
			miTransfertSlave.Click += new EventHandler( miTransfertSlave_Click );

			cmMore = new ContextMenu();
			cmMore.MenuItems.Add( miSetAsCapital );
			cmMore.MenuItems.Add( miRename );
			cmMore.MenuItems.Add( miEvacuate );

			if ( Statistics.economies[ Form1.game.playerList[ player ].economyType ].supportSlavery )
			{
				cmMore.MenuItems.Add( miTransfertSlave );

				if ( Form1.game.playerList[ player ].cityList[ city ].slaves.total == 0 )
					miTransfertSlave.Enabled = false;
			}
			
			cmdBuy.Text = language.getAString( language.order.cityBuy );
			cmdBuild.Text = language.getAString( language.order.cityBuild );
			cmdMore.Text = language.getAString( language.order.cityMore );

			cmdBuild.Width = ( lbBuildings.Left - 8 * 4 ) / 3;
			cmdBuy.Width = ( lbBuildings.Left - 8 * 4 ) / 3;
			cmdMore.Width = ( lbBuildings.Left - 8 * 4 ) / 3;

			cmdBuild.Left = 8;
			cmdBuy.Left = cmdBuild.Right + 8;
			cmdMore.Left = cmdBuy.Right + 8;

			platformSpec.manageWindows.setDialogSize( this );

			platformSpec.resolution.set( this.Controls );

			initGraphVar();
			drawStaticBack();
			enableButtons();
			showInfo();
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
			this.cmdRename = new System.Windows.Forms.Button();
			this.cmdEvacuate = new System.Windows.Forms.Button();
			this.cmdBuild = new System.Windows.Forms.Button();
			this.lbBuildings = new System.Windows.Forms.ListBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.cmdBuy = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.cmdSetAsCapital = new System.Windows.Forms.Button();
			this.cmdNext = new System.Windows.Forms.Button();
			this.cmdPrev = new System.Windows.Forms.Button();
			this.cmdMore = new System.Windows.Forms.Button();
			// 
			// cmdRename
			// 
			this.cmdRename.Location = new System.Drawing.Point(96, 208);
			this.cmdRename.Text = "Rename";
			this.cmdRename.Click += new System.EventHandler( this.cmdRename_Click );
			this.cmdRename.Visible = false;
			// 
			// cmdBuild
			// 
			this.cmdBuild.Location = new System.Drawing.Point(8, 208);
			this.cmdBuild.Size = new System.Drawing.Size(48, 20);
			this.cmdBuild.Text = "Build";
			this.cmdBuild.Click += new System.EventHandler( this.cmdBuild_Click );

			// 
			// cmdBuy
			// 
			this.cmdBuy.Location = new System.Drawing.Point( 56, 208 );
			this.cmdBuy.Size = new System.Drawing.Size( 40, 20 );
			this.cmdBuy.Text = "Buy";
			this.cmdBuy.Click += new System.EventHandler( this.cmdBuy_Click );

			// 
			// cmdMore
			// 
			this.cmdMore.Location = new System.Drawing.Point( 96, 208 );
			this.cmdMore.Text = "More";
			this.cmdMore.Click += new EventHandler(cmdMore_Click);
			// 
			// cmdEvacuate
			// 
			this.cmdEvacuate.Location = new System.Drawing.Point(8, 184);
			this.cmdEvacuate.Size = new System.Drawing.Size(64, 20);
			this.cmdEvacuate.Text = "Evacuate";
			this.cmdEvacuate.Click += new System.EventHandler( this.cmdEvacuate_Click );
			this.cmdEvacuate.Visible = false;
			// 
			// lbBuildings
			// 
			this.lbBuildings.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular);
			this.lbBuildings.Location = new System.Drawing.Point(180, 30); //32
			this.lbBuildings.Size = new System.Drawing.Size(56, 200);
			this.lbBuildings.SelectedIndexChanged += new EventHandler(lbBuildings_SelectedIndexChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Size = new System.Drawing.Size(240, 296);
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point(0, 232);
			this.pictureBox2.Size = new System.Drawing.Size(240, 64);
			// 
			// cmdSetAsCapital
			// 
			this.cmdSetAsCapital.Enabled = false;
			this.cmdSetAsCapital.Location = new System.Drawing.Point(72, 184);
			this.cmdSetAsCapital.Size = new System.Drawing.Size(96, 20);
			this.cmdSetAsCapital.Text = "Set as capital";
			this.cmdSetAsCapital.Visible = false;
			this.cmdSetAsCapital.Click += new EventHandler(cmdSetAsCapital_Click);
			// 
			// cmdNext
			// 
		//	this.cmdNext.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
			this.cmdNext.Location = new System.Drawing.Point(128, 32);
			this.cmdNext.Size = new System.Drawing.Size(40, 20);
			this.cmdNext.Text = "Next";
			this.cmdNext.Click += new EventHandler(cmdNext_Click);
			// 
			// cmdPrev
			// 
		//	this.cmdPrev.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
			this.cmdPrev.Location = new System.Drawing.Point(8, 32);
			this.cmdPrev.Size = new System.Drawing.Size(40, 20);
			this.cmdPrev.Text = "Prev";
			this.cmdPrev.Click += new EventHandler(cmdPrev_Click);
			// 
			// FrmCity
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.cmdPrev);
			this.Controls.Add(this.cmdNext);
			this.Controls.Add(this.cmdSetAsCapital);
			this.Controls.Add(this.cmdMore);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.cmdBuy);
			this.Controls.Add(this.lbBuildings);
			this.Controls.Add(this.cmdBuild);
			this.Controls.Add(this.cmdEvacuate);
			this.Controls.Add(this.cmdRename);
			this.Controls.Add(this.pictureBox1);
			this.Text = platformSpec.main.gameName;
			this.Load += new System.EventHandler(this.FrmCity_Load);
			this.Closed += new System.EventHandler(this.FrmCity_Closed);
			this.Activated += new System.EventHandler(this.FrmCity_Activated);

		}
		#endregion

		private System.Windows.Forms.Button cmdRename;
		private System.Windows.Forms.Button cmdEvacuate;
		private System.Windows.Forms.Button cmdBuild;
		private System.Windows.Forms.ListBox lbBuildings;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button cmdBuy;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Button cmdSetAsCapital;
		private System.Windows.Forms.Button cmdPrev;
		private System.Windows.Forms.Button cmdNext;
		private System.Windows.Forms.Button cmdMore;

		byte player;
		int city;
		ArrayList listConst;
		getPFT getPFT1;

		int adjPair;
		int adjImpair;
		int cenHor, cenVer;

		SolidBrush infoBackBrush;
		Pen blackPen;
		Bitmap infoBmp, staticBack, viewBmp;
		Graphics g;

		SolidBrush blackBrush;

		private void initGraphVar()
		{
			infoBackBrush = new SolidBrush( Color.Wheat ); 
			blackPen = new Pen( Color.Black ); 
			infoBmp = new Bitmap( pictureBox2.Width, pictureBox2.Height ); 
			viewBmp = new Bitmap( pictureBox1.Width, pictureBox1.Height ); 
			blackBrush = new SolidBrush( Color.Black ); 
			
			cenVer = Form1.game.playerList[ player ].cityList[ city ].Y - 6; 
			cenHor = Form1.game.playerList[ player ].cityList[ city ].X - 1; 

			adjPair = Form1.caseWidth/4; //-22
			adjImpair = -Form1.caseWidth/4; // +1 position de départ
		}

		private void cmdRename_Click(object sender, System.EventArgs e)
		{  // rename
			userTextInput ui = new userTextInput( 
				language.getAString( language.order.uiRenameCity ),
				language.getAString( language.order.uiPleaseEntrerCityName ),
				Form1.game.playerList[ player ].cityList[ city ].name,
				language.getAString( language.order.ok ),
				language.getAString( language.order.cancel )
				);
			ui.ShowDialog();
			string name = ui.result;

			if ( name != "" ) 
			{ 
				Form1.game.playerList[ player ].cityList[ city ].name = name; 
				drawMap(); 
			} 
		}

		private void FrmCity_Load(object sender, System.EventArgs e)
		{ 
			drawMap();
		} 

		private void FrmCity_Activated(object sender, System.EventArgs e)
		{ 
			if ( infoBackBrush != null ) 
				showInfo(); 
		} 
		
	/*	public void initGraphicsVariable()
		{
			cenVer = Form1.game.playerList[ player ].cityList[ city ].Y - 6;
			cenHor = Form1.game.playerList[ player ].cityList[ city ].X - 1; 

			adjPair = Form1.caseWidth/4; //-22
			adjImpair = -Form1.caseWidth/4; // +1 position de départ
		}	*/
#region drawStaticBack
		public void drawStaticBack()
		{
			staticBack = new Bitmap( pictureBox1.Width, pictureBox1.Height );

			g = Graphics.FromImage( staticBack );

			g.Clear( Color.Black );

			/*	cenVer = Form1.game.playerList[ player ].cityList[ city ].Y - 6;
			cenHor = Form1.game.playerList[ player ].cityList[ city ].X - 1; 

			adjPair = Form1.caseWidth/4; //-22
			adjImpair = -Form1.caseWidth/4; // +1 position de départ*/

			#region drawing the terrain
			for ( int y = -1; y < 20; y++ )
				for ( int x = -1; x < 5; x++ )
				{
					if ( y + cenVer >= 0 && y + cenVer < Form1.game.height )
					{
						Rectangle r;

						if ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 == 0 )
							r = new Rectangle( Form1.caseWidth * x + ( ( y + cenVer ) % 2 ) * Form1.caseWidth / 2 + adjPair - 10,
							Form1.caseHeight / 2 * y - 20, 
							Form1.caseWidth + 20,  
							Form1.caseHeight + 20 ); // + ( cenVer %2) * -Form1.caseWidth/2
							else
							r = new Rectangle( Form1.caseWidth * x + ( ( y + cenVer ) % 2 ) * Form1.caseWidth / 2 + adjImpair - 10,
							Form1.caseHeight / 2 * y - 20, 
							Form1.caseWidth + 20,  
							Form1.caseHeight + 20 );

						Point caseOnMap;

						if ( x + cenHor < Form1.game.width && x + cenHor >= 0 )
							caseOnMap = new Point( x + cenHor, y + cenVer );
						else if ( x + cenHor < 0 )
							caseOnMap = new Point( x + cenHor + Form1.game.width, y + cenVer );
						else // if( x + cenHor >= Form1.game.width )
							caseOnMap = new Point( x + cenHor - Form1.game.width, y + cenVer );

						/*	if ( Form1.options.hideUndiscovered )
						{*/
						if ( 
						Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ caseOnMap.X, caseOnMap.Y ] || 
						Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 == Form1.game.curPlayerInd
						)
							g.DrawImage( 
							Statistics.terrains[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].type ].bmp, 
							r, 
							0, 
							0,  
							Form1.caseWidth + 20,  
							Form1.caseHeight + 20, 
							GraphicsUnit.Pixel, 
							Form1.ia
							);
						/*	}
						else
						{ // show all
							g.DrawImage( 
								Statistics.terrains[  Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].type ].bmp , 
								r, 
								0, 
								0,  
								Form1.caseWidth + 20,  
								Form1.caseHeight + 20, 
								GraphicsUnit.Pixel, 
								Form1.ia
								);
						}*/
					}
				}
	#endregion
		
			#region drawing city + selected + units
			for ( int y = -1; y < 18; y++ )
				for ( int x = -1; x < 5; x++ )
					if ( y + cenVer >= 0 && y + cenVer < Form1.game.height )
					{
						Rectangle r;

						if ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 == 0 )
							r = new Rectangle( Form1.caseWidth * x + ( ( y + cenVer ) % 2 ) * Form1.caseWidth / 2 + adjPair - 10,
							Form1.caseHeight / 2 * y - 20, 
							Form1.caseWidth + 20,  
							Form1.caseHeight + 20 ); // + ( cenVer %2) * -Form1.caseWidth/2
							else
							r = new Rectangle( Form1.caseWidth * x + ( ( y + cenVer ) % 2 ) * Form1.caseWidth / 2 + adjImpair - 10,
							Form1.caseHeight / 2 * y - 20, 
							Form1.caseWidth + 20,  
							Form1.caseHeight + 20 );

						Point caseOnMap;

						if ( x + cenHor < Form1.game.width && x + cenHor >= 0 )
							caseOnMap = new Point( x + cenHor, y + cenVer );
						else if ( x + cenHor < 0 )
							caseOnMap = new Point( x + cenHor + Form1.game.width, y + cenVer );
						else // if( x + cenHor >= Form1.game.width )
							caseOnMap = new Point( x + cenHor - Form1.game.width, y + cenVer );

						/*	if ( Form1.options.hideUndiscovered )
						{*/
						if ( 
						Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ caseOnMap.X, caseOnMap.Y ] || 
						Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 == Form1.game.curPlayerInd
						)
						{
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].roadLevel > 0 )	/*roads*/
								roads.drawCase( g, caseOnMap, r );

							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement > 0 )	/*civic imp*/
								g.DrawImage( Form1.civicImprovement[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement ], r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia );

							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city > 0 )	/*cities*/
								g.DrawImage( Form1.cityBmp[ Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityType, Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city ].citySize ]/*Form1.cityBmp[ 0, 0 ]*/ , r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia );
						}
						/*	}
						else
						{
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].roadLevel > 0 )	/*roads* /
								roads.drawCase( g, caseOnMap, r );
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement > 0 )	/*civic imp* /
								g.DrawImage(  Form1.civicImprovement[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement ], r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city > 0 )	/*cities* /
								g.DrawImage( Form1.cityBmp[ Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityType, Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city ].citySize ]/*Form1.cityBmp[ 0, 0 ]* /, r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia );
						}*/
					}
	#endregion

			#region city limits
			Pen pBlack = new Pen( Color.Black );
			int adjX = -Form1.caseWidth * 3 / 4, adjY = -Form1.caseHeight * 3 / 2;

			g.DrawLine( pBlack, // \
						Form1.caseWidth / 2 * 4 + adjX, //x
						Form1.caseHeight / 2 * 6 + adjY, // - 20 
						Form1.caseWidth / 2 * 5 + adjX, //x
						Form1.caseHeight / 2 * 7 + adjY// - 20
						);

			g.DrawLine( pBlack, // /
						Form1.caseWidth / 2 * 5 + adjX, //x
						Form1.caseHeight / 2 * 7 + adjY, // - 20 
						Form1.caseWidth / 2 * 6 + adjX, //x
						Form1.caseHeight / 2 * 6 + adjY// - 20
						);

			g.DrawLine( pBlack, // \\\
						Form1.caseWidth / 2 * 6 + adjX, //x
						Form1.caseHeight / 2 * 6 + adjY, // - 20 
						Form1.caseWidth / 2 * 9 + adjX, //x
						Form1.caseHeight / 2 * 9 + adjY// - 20
						);

			g.DrawLine( pBlack, // /
						Form1.caseWidth / 2 * 9 + adjX, //x
						Form1.caseHeight / 2 * 9 + adjY, // - 20 
						Form1.caseWidth / 2 * 8 + adjX, //x
						Form1.caseHeight / 2 * 10 + adjY// - 20
						);

			g.DrawLine( pBlack, // \
						Form1.caseWidth / 2 * 8 + adjX, //x
						Form1.caseHeight / 2 * 10 + adjY, // - 20 
						Form1.caseWidth / 2 * 9 + adjX, //x
						Form1.caseHeight / 2 * 11 + adjY// - 20
						);

			g.DrawLine( pBlack, // ///
						Form1.caseWidth / 2 * 9 + adjX, //x
						Form1.caseHeight / 2 * 11 + adjY, // - 20 
						Form1.caseWidth / 2 * 6 + adjX, //x
						Form1.caseHeight / 2 * 14 + adjY// - 20
						);

			g.DrawLine( pBlack, // \
						Form1.caseWidth / 2 * 6 + adjX, //x
						Form1.caseHeight / 2 * 14 + adjY, // - 20 
						Form1.caseWidth / 2 * 5 + adjX, //x
						Form1.caseHeight / 2 * 13 + adjY// - 20
						);

			g.DrawLine( pBlack, // /
						Form1.caseWidth / 2 * 5 + adjX, //x
						Form1.caseHeight / 2 * 13 + adjY, // - 20 
						Form1.caseWidth / 2 * 4 + adjX, //x
						Form1.caseHeight / 2 * 14 + adjY// - 20
						);

			g.DrawLine( pBlack, // \\\
						Form1.caseWidth / 2 * 4 + adjX, //x
						Form1.caseHeight / 2 * 14 + adjY, // - 20 
						Form1.caseWidth / 2 * 1 + adjX, //x
						Form1.caseHeight / 2 * 11 + adjY// - 20
						);

			g.DrawLine( pBlack, // /
						Form1.caseWidth / 2 * 1 + adjX, //x
						Form1.caseHeight / 2 * 11 + adjY, // - 20 
						Form1.caseWidth / 2 * 2 + adjX, //x
						Form1.caseHeight / 2 * 10 + adjY// - 20
						);

			g.DrawLine( pBlack, // \
						Form1.caseWidth / 2 * 2 + adjX, //x
						Form1.caseHeight / 2 * 10 + adjY, // - 20 
						Form1.caseWidth / 2 * 1 + adjX, //x
						Form1.caseHeight / 2 * 9 + adjY// - 20
						);

			g.DrawLine( pBlack, // ///
						Form1.caseWidth / 2 * 1 + adjX, //x
						Form1.caseHeight / 2 * 9 + adjY, // - 20 
						Form1.caseWidth / 2 * 4 + adjX, //x
						Form1.caseHeight / 2 * 6 + adjY// - 20
						);

	#endregion

			g = Graphics.FromImage( viewBmp );
		}
			
#endregion

#region Draw map

		public void drawMap()
		{
		//	g = Graphics.FromImage( viewBmp );
		//	g.Clear( Color.Black );
			g.DrawImage( 
				staticBack, 
				new Rectangle(
					0, 0,
					staticBack.Width, staticBack.Height 
					),
				0,
				0,
				staticBack.Width,
				staticBack.Height,
				GraphicsUnit.Pixel,
				Form1.ia
				);


			
/*			cenVer = Form1.game.playerList[ player ].cityList[ city ].Y - 6;
			cenHor = Form1.game.playerList[ player ].cityList[ city ].X - 1; // - ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 ) * Form1.caseWidth/2;

			adjPair = Form1.caseWidth/4; //-22
			adjImpair = -Form1.caseWidth/4; // +1 position de départ
*/
			bool[,] labored = new bool[ Form1.game.width, Form1.game.height ];
			labored[ Form1.game.playerList[ player ].cityList[ city ].pos.X, Form1.game.playerList[ player ].cityList[ city ].pos.Y ] = true;

			for ( int l = 0; l < Form1.game.playerList[ player ].cityList[ city ].laborOnField; l++ )
				labored[ Form1.game.playerList[ player ].cityList[ city ].laborPos[ l ].X, Form1.game.playerList[ player ].cityList[ city ].laborPos[ l ].Y ] = true;
		
	#region drawing the terrain
			/*for (int y = -1; y < 20; y ++)
				for (int x = -1; x < 5; x ++)
				{
					if ( y + cenVer >= 0 && y + cenVer < Form1.game.height )
					{
						Rectangle r;

						if ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 == 0 )
							r = new Rectangle(	Form1.caseWidth*x + ( (y+ cenVer) %2) *Form1.caseWidth/2 + adjPair - 10 ,
								Form1.caseHeight/2*y - 20, 
								Form1.caseWidth + 20,  
								Form1.caseHeight + 20); // + ( cenVer %2) * -Form1.caseWidth/2
						else
							r = new Rectangle(	Form1.caseWidth*x + ( (y+ cenVer) %2) *Form1.caseWidth/2 + adjImpair - 10 ,
								Form1.caseHeight/2*y - 20, 
								Form1.caseWidth + 20,  
								Form1.caseHeight + 20);

						Point caseOnMap;

						if ( x + cenHor < Form1.game.width && x + cenHor >= 0 )
							caseOnMap = new Point( x + cenHor, y + cenVer );
						else if( x + cenHor < 0 )
							caseOnMap = new Point( x + cenHor + Form1.game.width, y + cenVer );
						else // if( x + cenHor >= Form1.game.width )
							caseOnMap = new Point( x + cenHor - Form1.game.width, y + cenVer );

						if ( Form1.options.hideUndiscovered )
						{
							if ( 
								Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ caseOnMap.X, caseOnMap.Y ] || 
								Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 == Form1.game.curPlayerInd
								)
								g.DrawImage( 
									Statistics.terrains[  Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].type ].bmp , 
									r, 
									0, 
									0,  
									Form1.caseWidth + 20,  
									Form1.caseHeight + 20, 
									GraphicsUnit.Pixel, 
									Form1.ia
									);
						}
						else
						{ // show all
							g.DrawImage( 
								Statistics.terrains[  Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].type ].bmp , 
								r, 
								0, 
								0,  
								Form1.caseWidth + 20,  
								Form1.caseHeight + 20, 
								GraphicsUnit.Pixel, 
								Form1.ia
								);
						}
					}
				}*/
			#endregion
		
	#region drawing city + selected + units
			for (int y = -1; y < 18; y ++)
				for (int x = -1; x < 5; x ++)
				{
					if (y + cenVer >= 0 && y + cenVer < Form1.game.height )
					{
						Rectangle r;

						if ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 == 0 )
							r = new Rectangle(	Form1.caseWidth*x + ( (y+ cenVer) %2) *Form1.caseWidth/2 + adjPair - 10 ,
								Form1.caseHeight/2*y - 20, 
								Form1.caseWidth + 20,  
								Form1.caseHeight + 20); // + ( cenVer %2) * -Form1.caseWidth/2
						else
							r = new Rectangle(	Form1.caseWidth*x + ( (y+ cenVer) %2) *Form1.caseWidth/2 + adjImpair - 10 ,
								Form1.caseHeight/2*y - 20, 
								Form1.caseWidth + 20,  
								Form1.caseHeight + 20);

						Point caseOnMap;

						if ( x + cenHor < Form1.game.width && x + cenHor >= 0 )
							caseOnMap = new Point( x + cenHor, y + cenVer );
						else if( x + cenHor < 0 )
							caseOnMap = new Point( x + cenHor + Form1.game.width, y + cenVer );
						else // if( x + cenHor >= Form1.game.width )
							caseOnMap = new Point( x + cenHor - Form1.game.width, y + cenVer );

						/*if ( Form1.options.hideUndiscovered )
						{*/
							if ( 
								Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ caseOnMap.X, caseOnMap.Y ] || 
								Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 == Form1.game.curPlayerInd
								)
							{
							//	if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].roadLevel > 0 )	/*roads*/
							//		roads.drawCase( g, caseOnMap, r );
									//g.DrawImage(  Form1.roadBmp[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].roadLevel - 1 ], r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
							//	if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement > 0 )	/*civic imp*/
							//		g.DrawImage(  Form1.civicImprovement[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement ], r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
								if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].laborCity > 0 )	/*labor*/
									g.DrawImage(  Form1.laborBmp[ Statistics.terrains[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].type ].ew ] , r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
							//	if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city > 0 )	/*cities*/
							//		g.DrawImage( Form1.cityBmp[ Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityType, Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city ].citySize ]/*Form1.cityBmp[ 0, 0 ]*/ , r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia );
							}
					/*	}
						else
						{
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].roadLevel > 0 )	/*roads* /
								roads.drawCase( g, caseOnMap, r );
							//	g.DrawImage(  Form1.roadBmp[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].roadLevel - 1 ], r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement > 0 )	/*civic imp* /
								g.DrawImage(  Form1.civicImprovement[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].civicImprovement ], r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].laborCity > 0 )	/*labor* /
								g.DrawImage(  Form1.laborBmp[ Statistics.terrains[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].type ].ew ] , r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia);
							if ( Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city > 0 )	/*cities* /
								g.DrawImage( Form1.cityBmp[ Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityType, Form1.game.playerList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].territory - 1 ].cityList[ Form1.game.grid[ caseOnMap.X, caseOnMap.Y ].city ].citySize ]/*Form1.cityBmp[ 0, 0 ]* /, r, 0, 0, Form1.caseWidth + 20, Form1.caseHeight + 20, GraphicsUnit.Pixel, Form1.ia );
						}*/
					}
				}
			#endregion

	#region city limits
		/*	Pen pBlack = new Pen(Color.Black);

			int adjX = -Form1.caseWidth * 3/4  ; 
			int adjY = -Form1.caseHeight * 3/2 ;

			g.DrawLine( pBlack, // \
						Form1.caseWidth/2 * 4 + adjX, //x
						Form1.caseHeight/2 * 6 + adjY, // - 20 
						Form1.caseWidth/2 * 5 + adjX, //x
						Form1.caseHeight/2 * 7 + adjY// - 20
				);

			g.DrawLine( pBlack, // /
						Form1.caseWidth/2 * 5 + adjX, //x
						Form1.caseHeight/2 * 7 + adjY, // - 20 
						Form1.caseWidth/2 * 6 + adjX, //x
						Form1.caseHeight/2 * 6 + adjY// - 20
						);

			g.DrawLine( pBlack, // \\\
						Form1.caseWidth/2 * 6 + adjX, //x
						Form1.caseHeight/2 * 6 + adjY, // - 20 
						Form1.caseWidth/2 * 9 + adjX, //x
						Form1.caseHeight/2 * 9 + adjY// - 20
				);

			g.DrawLine( pBlack, // /
						Form1.caseWidth/2 * 9 + adjX, //x
						Form1.caseHeight/2 * 9 + adjY, // - 20 
						Form1.caseWidth/2 * 8 + adjX, //x
						Form1.caseHeight/2 * 10 + adjY// - 20
						);

			g.DrawLine( pBlack, // \
						Form1.caseWidth/2 * 8 + adjX, //x
						Form1.caseHeight/2 * 10 + adjY, // - 20 
						Form1.caseWidth/2 * 9 + adjX, //x
						Form1.caseHeight/2 * 11 + adjY// - 20
				);

			g.DrawLine( pBlack, // ///
				Form1.caseWidth/2 * 9 + adjX, //x
				Form1.caseHeight/2 * 11 + adjY, // - 20 
				Form1.caseWidth/2 * 6 + adjX, //x
				Form1.caseHeight/2 * 14 + adjY// - 20
				);

			g.DrawLine( pBlack, // \
				Form1.caseWidth/2 * 6 + adjX, //x
				Form1.caseHeight/2 * 14 + adjY, // - 20 
				Form1.caseWidth/2 * 5 + adjX, //x
				Form1.caseHeight/2 * 13 + adjY// - 20
				);

			g.DrawLine( pBlack, // /
				Form1.caseWidth/2 * 5 + adjX, //x
				Form1.caseHeight/2 * 13 + adjY, // - 20 
				Form1.caseWidth/2 * 4 + adjX, //x
				Form1.caseHeight/2 * 14 + adjY// - 20
				);

			g.DrawLine( pBlack, // \\\
				Form1.caseWidth/2 * 4 + adjX, //x
				Form1.caseHeight/2 * 14 + adjY, // - 20 
				Form1.caseWidth/2 * 1 + adjX, //x
				Form1.caseHeight/2 * 11 + adjY// - 20
				);

			g.DrawLine( pBlack, // /
				Form1.caseWidth/2 * 1 + adjX, //x
				Form1.caseHeight/2 * 11 + adjY, // - 20 
				Form1.caseWidth/2 * 2 + adjX, //x
				Form1.caseHeight/2 * 10 + adjY// - 20
				);

			g.DrawLine( pBlack, // \
				Form1.caseWidth/2 * 2 + adjX, //x
				Form1.caseHeight/2 * 10 + adjY, // - 20 
				Form1.caseWidth/2 * 1 + adjX, //x
				Form1.caseHeight/2 * 9 + adjY// - 20
				);

			g.DrawLine( pBlack, // ///
				Form1.caseWidth/2 * 1 + adjX, //x
				Form1.caseHeight/2 * 9 + adjY, // - 20 
				Form1.caseWidth/2 * 4 + adjX, //x
				Form1.caseHeight/2 * 6 + adjY// - 20
				);
*/
			#endregion

	#region food, prod and trade
			for ( int y = -1; y < 18; y++ )
				for ( int x = -1; x < 5; x++ )
					if ( y + cenVer >= 0 && y + cenVer < Form1.game.height )
					{
						Rectangle r;

						if ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 == 0 )
							r = new Rectangle( Form1.caseWidth * x + ( ( y + cenVer ) % 2 ) * Form1.caseWidth / 2 + adjPair - 10, Form1.caseHeight / 2 * y - 20, Form1.caseWidth + 20, Form1.caseHeight + 20 ); // + ( cenVer %2) * -Form1.caseWidth/2
						else
							r = new Rectangle( Form1.caseWidth * x + ( ( y + cenVer ) % 2 ) * Form1.caseWidth / 2 + adjImpair - 10, Form1.caseHeight / 2 * y - 20, Form1.caseWidth + 20, Form1.caseHeight + 20 );

						Point caseOnMap;

						if ( x + cenHor < Form1.game.width && x + cenHor >= 0 )
							caseOnMap = new Point( x + cenHor, y + cenVer );
						else if ( x + cenHor < 0 )
							caseOnMap = new Point( x + cenHor + Form1.game.width, y + cenVer );
						else // if( x + cenHor >= Form1.game.width )
							caseOnMap = new Point( x + cenHor - Form1.game.width, y + cenVer );

						if ( labored[ caseOnMap.X, caseOnMap.Y ] )
						{
							int food = getPFT.getCaseFood( caseOnMap.X, caseOnMap.Y ), 
							prod = getPFT.getCaseProd( caseOnMap.X, caseOnMap.Y ), 
							trade = getPFT.getCaseTrade( caseOnMap.X, caseOnMap.Y );

/*							if ( caseOnMap == Form1.game.playerList[ player ].cityList[ city ].pos ) 
								trade += Form1.game.playerList[ player ].cityList[ city ].population + Form1.game.playerList[ player ].cityList[ city ].nonLabor.totalOfOneType( (byte)peopleNonLabor.types.trader ) * 2;
*/
							int tot = food + prod + trade; 

							double diff = (double)(Form1.caseWidth) / ( tot+1 );
							for ( int i = 0; i < tot; i++ )
								if ( i < trade )
								{
									g.DrawImage( 
										Form1.tradeBmp, 
										new Rectangle( r.Left + (int)((i+1) * diff), r.Top + 20, Form1.tradeBmp.Width, Form1.tradeBmp.Height ),
										0,
										0, 
										Form1.tradeBmp.Width,
										Form1.tradeBmp.Height,
										GraphicsUnit.Pixel,
										Form1.ia
										);
								}
								else if ( i < trade + food )
								{
									g.DrawImage( 
										Form1.foodBmp, 
										new Rectangle( r.Left + (int)((i+1) * diff), r.Top + 20, Form1.tradeBmp.Width, Form1.tradeBmp.Height ),
										0,
										0, 
										Form1.tradeBmp.Width,
										Form1.tradeBmp.Height,
										GraphicsUnit.Pixel,
										Form1.ia
										);
								}
								else
								{
									g.DrawImage( 
										Form1.prodBmp, 
										new Rectangle( r.Left + (int)((i+1) * diff), r.Top + 20, Form1.tradeBmp.Width, Form1.tradeBmp.Height ),
										0,
										0, 
										Form1.tradeBmp.Width,
										Form1.tradeBmp.Height,
										GraphicsUnit.Pixel,
										Form1.ia
										);
								}
						}
					}
	#endregion

			g.DrawString( 
				Form1.game.playerList[ player ].cityList[ city ].name,
				new Font( "Tahoma", 19, FontStyle.Regular ),
				blackBrush,
				10,
				2
				);
			g.DrawString( 
				Form1.game.playerList[ player ].cityList[ city ].name,
				new Font( "Tahoma", 19, FontStyle.Regular ),
				new SolidBrush( Color.White ),
				9,
				1 
				);

			if ( Form1.game.playerList[ player ].cityList[ city ].state == (byte)enums.cityState.evacuating )
			{
				string evacuating = language.getAString( language.order.cityStateEvacuating );///"Evacuating!";
				Font evacuatingFont = new Font( "Tahoma", 19, FontStyle.Regular );
				SizeF evacuatingSize = g.MeasureString( evacuating, evacuatingFont );

				g.DrawString( 
					evacuating,
					evacuatingFont,
					blackBrush,
					lbBuildings.Left / 2 - evacuatingSize.Width / 2 + 1,
					60
					);
				g.DrawString( 
					evacuating,
					evacuatingFont,
					new SolidBrush( Color.Red ),
					lbBuildings.Left / 2 - evacuatingSize.Width / 2,
					60 - 1
					);
			}

	/*		int freeLabor = Form1.game.playerList[ player ].cityList[ city ].population - Form1.game.playerList[ player ].cityList[ city ].laborOnField;
			g.DrawString( freeLabor.ToString(), new Font( "Tahoma", 12, FontStyle.Regular ), new SolidBrush( Color.Red ), 8, 80 );
*/
#region draw pop
			int totPop = Form1.game.playerList[ player ].cityList[ city ].population + Form1.game.playerList[ player ].cityList[ city ].slaves.total; 
			double diff1 = (double)( lbBuildings.Left - 16 - Form1.peopleBmp.Width ) / ( totPop + 1 );
			int left = 8, top = cmdMore.Top - Form1.peopleBmp.Height;

			peopleRects = new Rectangle[ totPop ];

			for ( int i = 0; i < totPop; i ++ )
			{
				Rectangle rect = new Rectangle( left + (int)((i+1) * diff1), top, Form1.peopleBmp.Width, Form1.peopleBmp.Height );
						
				if ( i < Form1.game.playerList[ player ].cityList[ city ].laborOnField )
					g.DrawImage( 
						Form1.peopleBmp, 
						rect,
						0,
						0, 
						Form1.peopleBmp.Width,
						Form1.peopleBmp.Height,
						GraphicsUnit.Pixel,
						Form1.ia
						);
				else if ( i < Form1.game.playerList[ player ].cityList[ city ].population )
					g.DrawImage( 
						Form1.peopleNonLaborBmp[ Form1.game.playerList[ player ].cityList[ city ].nonLabor.list[ i - Form1.game.playerList[ player ].cityList[ city ].laborOnField ] ], 
						rect,
						0,
						0, 
						Form1.peopleBmp.Width,
						Form1.peopleBmp.Height,
						GraphicsUnit.Pixel,
						Form1.ia
						);
				else //if ( i - Form1.game.playerList[ player ].cityList[ city ].population < Form1.game.playerList[ player ].cityList[ city ].slaves.total )
					g.DrawImage( 
						Form1.slaveBmp[ Form1.game.playerList[ player ].cityList[ city ].slaves.list[ i - Form1.game.playerList[ player ].cityList[ city ].population ] ], 
						rect,
						0,
						0, 
						Form1.peopleBmp.Width,
						Form1.peopleBmp.Height,
						GraphicsUnit.Pixel,
						Form1.ia
						);

				peopleRects[ i ] = rect;
			}

#endregion

			pictureBox1.Image = viewBmp;
		//	pictureBox1.Update();
		//	this.Update();
		}
#endregion

#region listBox selected
		private void lbBuildings_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//if (lbBuildings.SelectedIndex == "Temple")
			//	Form1.game.playerList[ player ].cityList[ city ].BuildingList.temple = false;

			//lbBuildings.SelectedItem
			//MessageBox.Show( lbBuildings.SelectedIndex.ToString() + "\n" + lbBuildings.SelectedValue.ToString()  + "\n" + lbBuildings.SelectedItem.ToString());
		}
		#endregion

#region pictureBox1 click !Selection!
		private void pictureBox1_Click(object sender, System.EventArgs e)
		{
			System.Drawing.Point selected = new Point();
			System.Drawing.Point bob2;

			bob2 = pictureBox1.PointToClient(new Point(Form1.MousePosition.X - adjPair, Form1.MousePosition.Y));

			bool hitSomething = false;
			if ( bob2.Y >= peopleRects[ 0 ].Top && bob2.Y < peopleRects[ 0 ].Bottom )
				for ( int i = peopleRects.Length - 1; i >= 0 ; i -- )
					if ( bob2.X >= peopleRects[ i ].Left - peopleRects[ i ].Width && bob2.X < peopleRects[ i ].Right - peopleRects[ i ].Width )
					//if ( bob2.X >= peopleRects[ i ].X && bob2.X < peopleRects[ i ].X + peopleRects[ i ].Width )
					{
						if ( i < Form1.game.playerList[ player ].cityList[ city ].laborOnField )
						{ // nothig, to be: auto remove labor + add peep non labor
						}
						else if ( i < Form1.game.playerList[ player ].cityList[ city ].population )
						{
							Form1.game.playerList[ player ].cityList[ city ].nonLabor.switchNextType( i - Form1.game.playerList[ player ].cityList[ city ].laborOnField );
						}
						else //if ( i - Form1.game.playerList[ player ].cityList[ city ].population < Form1.game.playerList[ player ].cityList[ city ].slaves.total )
						{
							Form1.game.playerList[ player ].cityList[ city ].slaves.switchNextType( i - Form1.game.playerList[ player ].cityList[ city ].population );
						}

						hitSomething = true;
						break;
					}

			if ( !hitSomething )
			{
				int xFloor = Convert.ToInt16( Math.Floor( Convert.ToDouble( bob2.X / Form1.caseWidth ) ) );
				int yFloor = Convert.ToInt16( Math.Floor( Convert.ToDouble( ( bob2.Y ) / Form1.caseHeight ) ) );   //2 fois y de game.grid

				#region localisation du clik
				if ( bob2.X < xFloor * Form1.caseWidth + Form1.caseWidth / 2 && bob2.Y < yFloor * Form1.caseHeight + Form1.caseHeight / 2 )
				{
					//1er cadran
					if ( ( bob2.Y - yFloor * Form1.caseHeight ) >= -Form1.caseHeight * Convert.ToSingle( bob2.X - xFloor * Form1.caseWidth ) / Form1.caseWidth + 15 )
					{ //centre
						selected.X = xFloor + cenHor;
						selected.Y = 2 * yFloor + cenVer;
					}
					else
					{
						selected.X = xFloor - 1 + cenHor;
						selected.Y = 2 * yFloor - 1 + cenVer;
					}
				}
				else if ( bob2.X > xFloor * Form1.caseWidth + Form1.caseWidth / 2 && bob2.Y < yFloor * Form1.caseHeight + Form1.caseHeight / 2 )
				{
					// 2e cadran
					if ( ( bob2.Y - yFloor * Form1.caseHeight ) >= Form1.caseHeight * Convert.ToSingle( bob2.X - xFloor * Form1.caseWidth ) / Form1.caseWidth - 15 )
					{ //centre
						selected.X = xFloor + cenHor;
						selected.Y = 2 * yFloor + cenVer;
					}
					else
					{
						selected.X = xFloor + 1 - 1 + cenHor;
						selected.Y = 2 * yFloor - 1 + cenVer;
					}
				}
				else if ( bob2.X < xFloor * Form1.caseWidth + Form1.caseWidth / 2 && bob2.Y > yFloor * Form1.caseHeight + Form1.caseHeight / 2 )
				{
					// 3e cadran
					if ( ( bob2.Y - yFloor * Form1.caseHeight ) >= Form1.caseHeight * Convert.ToSingle( bob2.X - xFloor * Form1.caseWidth ) / Form1.caseWidth + 15 )
					{
						selected.X = xFloor - 1 + cenHor;
						selected.Y = 2 * yFloor + 1 + cenVer;
					}
					else //centre
						{
							selected.X = xFloor + cenHor;
							selected.Y = 2 * yFloor + cenVer;
						}
					}
					else
					{
						//4e cadran
						if ( ( bob2.Y - yFloor * Form1.caseHeight ) >= -Form1.caseHeight * Convert.ToSingle( bob2.X - xFloor * Form1.caseWidth ) / Form1.caseWidth + 45 )
						{
							selected.X = xFloor + 1 - 1 + cenHor;
							selected.Y = 2 * yFloor + 1 + cenVer;
						}
						else //centre
							{
								selected.X = xFloor + cenHor;
								selected.Y = 2 * yFloor + cenVer;
							}
						}
		#endregion
			
				if ( ( Form1.game.playerList[ player ].cityList[ city ].Y % 2 ) == 1 && ( selected.Y % 2 ) == 0 )
					selected.X++;

				if ( selected.X < 0 )
					selected.X += Form1.game.width;
				else if ( selected.X >= Form1.game.width )
					selected.X -= Form1.game.width;

				#region labor

						if ( selected.Y <= Form1.game.height && selected.Y > 0 && ( Form1.game.grid[ selected.X, selected.Y ].laborCity == 0 || Form1.game.grid[ selected.X, selected.Y ].laborCity == city ) && Form1.game.grid[ selected.X, selected.Y ].city == 0 && ( Form1.game.grid[ selected.X, selected.Y ].territory - 1 == player || Form1.game.grid[ selected.X, selected.Y ].type == (byte)enums.terrainType.sea ) )
						{
							bool withinRadius = false;
						//	Radius radius = new Radius();
							Point[] cr = Form1.game.radius.returnCityRadius( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y );

							for ( int i = 0; i < cr.Length; i++ )
								if ( cr[ i ] == selected )
									withinRadius = true;

							if ( withinRadius )
								setLabor( selected.X, selected.Y );
							else
								labor.addAllLabor( player, city );
						}
		#endregion
			
			}
			drawMap();
			showInfo();
		}

		private void setLabor ( int x, int y )
		{
			Point selected = new Point( x, y );
			if ( Form1.game.grid[ x, y ].laborCity == 0 )
			{ // add
				if ( Form1.game.playerList[ player ].cityList[ city ].laborOnField < Form1.game.playerList[ player ].cityList[ city ].population )
				{
					labor.addLaborAt( player, city, selected );
				}
				else
				{ // remove all, add all
					labor.removeAllLabor( player, city );
					labor.addAllLabor( player, city );
				}
			}
			else if ( Form1.game.grid[ selected.X, selected.Y ].laborCity == city )
			{ // remove
				labor.removeLaborAt( player, city, selected, true );
			}
			else
			{
				int bob = 0;
				bob ++;
			}
		}

		#endregion

#region buy
		private void cmdBuy_Click(object sender, System.EventArgs e)
		{ // buy construction
		/*	if ( 
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Unit ||
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Building
				) //Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.unit )
			{*/
				int cost = Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
			/*	if ( Form1.game.playerList[ player ].money >= cost )
				{*/
					switch (MessageBox.Show( 
						String.Format( "Do you want to pay {0} gold to complete the {1}?", cost, Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].name.ToLower() ), 
						"Buy",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.None,
						MessageBoxDefaultButton.Button1
						) )
					{
						case DialogResult.Yes:
							Form1.game.playerList[ player ].money -= cost;
							Form1.game.playerList[ player ].cityList[ city ].construction.points = Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].cost;
							break;
					}
		/*		}
				else
				{
					MessageBox.Show("Not enough money", "Sorry" );
				}*/
		//	}
		/*	else if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.building )
			{
				int cost = Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
				if ( Form1.game.playerList[ player ].money >= cost )
				{
					switch (MessageBox.Show( 
						String.Format( "Do you want to pay {0} gold to complete the {1}?", cost, Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].name.ToLower() ), 
						"Buy",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.None,
						MessageBoxDefaultButton.Button1
						) )
					{
						case DialogResult.Yes:
							Form1.game.playerList[ player ].money -= cost;
							Form1.game.playerList[ player ].cityList[ city ].construction.points = Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost;
						//	showInfo();
							break;
					}
				}
				else
				{
					MessageBox.Show("Not enough money", "Sorry" );
				}
			}*/

			showInfo();
		}
		#endregion

#region showInfo
		private void showInfo ()
		{
			Graphics g = Graphics.FromImage( infoBmp );
			g.FillRectangle( infoBackBrush, 0, 0, pictureBox2.Width - 1, pictureBox2.Height - 1 );
			g.DrawRectangle( blackPen, 0, 0, pictureBox2.Width - 1, pictureBox2.Height - 1 );
			Rectangle r = new Rectangle( 
				pictureBox2.Width - 70,
				0,
				70,
				50
				);

			if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Unit )//.type == (byte)enums.cityBuildType.unit )
			{
				g.DrawImage(
					((Stat.Unit) Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ]).bmp,
					r,
					0,
					0,
					70,
					50,
					GraphicsUnit.Pixel,
					Form1.ia
					);

				Font nameFont = new Font( "Tahoma", 8, FontStyle.Regular );
				SizeF nameSize = g.MeasureString( ((Stat.Unit) Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ]).name, nameFont );

				g.DrawString( 
					((Stat.Unit) Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ]).name, 
					nameFont,
					blackBrush,
					r.Left + 70/ 2 - nameSize.Width / 2,
					r.Bottom - 12
					);
			}
			else //if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.building )
			{
				Font nameFont = new Font( "Tahoma", 10, FontStyle.Regular );
				SizeF nameSize = g.MeasureString( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].name, nameFont );

				g.DrawString( 
					Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].name, 
					nameFont,
					blackBrush,
					pictureBox2.Width - nameSize.Width - 5,
					5
					);
			}

			int turnsLeft = count.turnsLeftToBuild( player, city );

		/*	if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.unit )
				turnsLeft = ( Statistics.units[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points ) / getPFT1.getCityProd( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ) + 1;
			else if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.building )// buildings
				turnsLeft = ( Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points ) / getPFT1.getCityProd( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ) + 1;
*/
			string strTL = "";
			if ( turnsLeft > 1 )
				strTL = " turns left";
			else if ( turnsLeft != -1 )
				strTL = " turn left";

			Font fontTL = new Font( "Tahoma", 10, FontStyle.Regular );
			string affTL = Convert.ToString( turnsLeft, 10 ) + strTL;
			SizeF sizeTL = g.MeasureString( affTL, fontTL );

			g.DrawString(
				affTL,
				fontTL,
				blackBrush,
				pictureBox2.Width - sizeTL.Width - 2,
				pictureBox2.Height - sizeTL.Height
				);
				
			g.DrawString( 
				"Prod: " + 
				Form1.game.playerList[ player ].cityList[ city ].production.ToString() +
				"  Food: " + 
				Form1.game.playerList[ player ].cityList[ city ].food.ToString() +
				"  Trade: " + 
				Form1.game.playerList[ player ].cityList[ city ].trade.ToString(), // getPFT1.getCityTrade( player, city ).ToString(), 
				fontTL,
				blackBrush,
				4, 
				0
				);
				
			int tlBeforeGrowth = (int)( Form1.game.playerList[ player ].cityList[ city ].population * Form1.foodPerPop + Form1.game.playerList[ player ].cityList[ city ].slaves.total*.5 - Form1.game.playerList[ player ].cityList[ city ].foodReserve ) / ( Form1.game.playerList[ player ].cityList[ city ].food + 1 );
			g.DrawString( 
				( tlBeforeGrowth > 1? String.Format( language.getAString( language.order.turnsBeforeGrowth ), tlBeforeGrowth ) :String.Format( language.getAString( language.order.turnBeforeGrowth ), tlBeforeGrowth ) ),
				fontTL,
				blackBrush,
				4, 
				14
				);

			int yo = 20;
			int pos = 0;
			if ( Form1.game.playerList[ player ].technos[ (byte)Form1.technoList.horseBreed ].researched )
			{
				g.DrawImage( 
					Form1.specialResBmp[ (byte)enums.speciaResources.horses - 1 ], 
					new Rectangle( pos * 20 - 15, yo, 70, 50 ),
					0,
					0,
					70,
					50,
					GraphicsUnit.Pixel,
					Form1.ia
					);
				pos ++;
			}
			if ( Form1.game.playerList[ player ].technos[ (byte)Form1.technoList.elephantBreed ].researched )
			{
				g.DrawImage( 
					Form1.specialResBmp[ (byte)enums.speciaResources.elephant - 1 ], 
					new Rectangle( pos * 20 - 15, yo, 70, 50 ),
					0,
					0,
					70,
					50,
					GraphicsUnit.Pixel,
					Form1.ia
					);
				pos ++;
			}
			if ( Form1.game.playerList[ player ].technos[ (byte)Form1.technoList.camelBreed ].researched )
			{
				g.DrawImage( 
					Form1.specialResBmp[ (byte)enums.speciaResources.camel - 1 ], 
					new Rectangle( pos * 20 - 15, yo, 70, 50 ),
					0,
					0,
					70,
					50,
					GraphicsUnit.Pixel,
					Form1.ia
					);
				pos ++;
			}

			for ( int i = 0; i < Statistics.resources.Length; i ++ )
				if ( 
					Form1.game.playerList[ player ].cityList[ city ].hasAccessToRessource( i ) && 
					Form1.game.playerList[ player ].technos[ Statistics.resources[ i ].techno ].researched 
					)
				{
					g.DrawImage( 
						Statistics.resources[ i ].bmp, 
						new Rectangle( pos * 20 - 15, yo, 70, 50 ),
						0,
						0,
						70,
						50,
						GraphicsUnit.Pixel,
						Form1.ia
						);
					pos ++;
				}
				
		/*	g.DrawString( 
				"Production: " + 
				getPFT1.getCityProd( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ).ToString() +
				"\nFood: " +
				getPFT1.getCityFood( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ).ToString() +
				"\nTrade: " + 
				getPFT1.getCityTrade( player, city ).ToString(), 
				fontTL,
				blackBrush,
				4, 
				0
				);*/


			pictureBox2.Image = infoBmp;

			cmdBuy.Enabled = 
				(
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Unit ||
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Building ||
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.SmallWonder
				) &&
				Form1.game.playerList[ player ].money >= Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;

			
		}
		#endregion

		private void cmdBuild_Click(object sender, System.EventArgs e) 
		{ 
			openBuildForm( player, city, Form1.game.playerList[ player ].cityList[ city ].production ); 
		} 

		private void openBuildForm(byte player, int city, int cityProd ) 
		{ 
			frmBuild frmB = new frmBuild( player, city, cityProd ); 
			string title = this.Text; 
			platformSpec.manageWindows.prepareForDialog( this ); //this.Text = ""; 
			frmB.ShowDialog(); 
			this.Text = title; 
		} 

		private void FrmCity_Closed( object sender, System.EventArgs e)
		{
		}

		private void cmdEvacuate_Click(object sender, System.EventArgs e)
		{ // evacuate
			if ( Form1.game.playerList[ player ].cityList[ city ].state == (byte)enums.cityState.evacuating )
			{
				Form1.game.playerList[ player ].cityList[ city ].state = (byte)enums.cityState.ok;
			}
			else
			{
				Form1.game.playerList[ player ].cityList[ city ].state = (byte)enums.cityState.evacuating;
			}
			drawMap();
			enableButtons();
		}

#region enableButtons
		private void enableButtons()
		{
			if ( !Form1.game.playerList[ player ].cityList[ city ].isCapitale )
			{
				cmdEvacuate.Enabled = true;
				miEvacuate.Enabled = true;

			}
			else
			{
				cmdEvacuate.Enabled = false;
				miEvacuate.Enabled = false;
			}

			if ( Form1.game.playerList[ player ].cityList[ city ].state == (byte)enums.cityState.evacuating )
			{

				if ( Form1.game.playerList[ player ].cityList[ city ].isCapitale )
				{
					Form1.game.playerList[ player ].cityList[ city ].state = (byte)enums.cityState.ok;
					cmdEvacuate.Text = language.getAString( language.order.cityEvacuate );
					miEvacuate.Text = language.getAString( language.order.cityEvacuate );
				}
				else
				{
					cmdEvacuate.Text = language.getAString( language.order.cityCancelEvacuation );
					miEvacuate.Text = language.getAString( language.order.cityCancelEvacuation );
				}
			}
			else
			{
				cmdEvacuate.Text = language.getAString( language.order.cityEvacuate );
				miEvacuate.Text = language.getAString( language.order.cityEvacuate );
			}
			
			if ( Form1.game.playerList[ player ].cityList[ city ].slaves.total == 0 )
				miTransfertSlave.Enabled = false;
		}

#endregion

#region cmdNext and cmdPrev
		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			int city1 = city;

			while ( true )
			{
				city1 += 1;

				if ( city1 > Form1.game.playerList[ player ].cityNumber )
					city1 = 1;

				if ( city1 == city )
					break;

				if ( Form1.game.playerList[ player ].cityList[ city1 ].state != (byte)enums.cityState.dead )
					break;
			}

			if ( city1 != city )
			{
				city = city1;
				redrawForm();
			}
			else
			{
				cmdNext.Enabled = false;
				cmdPrev.Enabled = false;
			}
		}
			
		private void cmdPrev_Click(object sender, System.EventArgs e)
		{
			int city1 = city;

			while ( true )
			{
				city1 -= 1;

				if ( city1 < 1 )
					city1 = Form1.game.playerList[ player ].cityNumber;

				if ( city1 == city )
					break;

				if ( Form1.game.playerList[ player ].cityList[ city1 ].state != (byte)enums.cityState.dead )
					break;
			}

			if ( city1 != city )
			{
				city = city1;
				redrawForm();
			}
			else
			{
				cmdNext.Enabled = false;
				cmdPrev.Enabled = false;
			}
		}	

#endregion

		private void redrawForm()
		{
			this.Text = Form1.game.playerList[ player ].cityList[ city ].name;
			listConst.Clear();
			lbBuildings.Items.Clear();

			for ( byte i = 0; i < Form1.game.playerList[ player ].cityList[ city ].buildingList.Length; i ++ ) 
				if ( Form1.game.playerList[ player ].cityList[ city ].buildingList[ i ] )
				{
					listConst.Add( i );
					lbBuildings.Items.Add( Statistics.buildings[ i ].name );
				}
			
			initGraphVar();
			drawStaticBack();
			drawMap();
			enableButtons();
			showInfo();
		}

		private void cmdMore_Click(object sender, EventArgs e)
		{
			cmMore.Show( cmdMore, new Point( 0, 0/*cmdMore.Width, cmdMore.Height*/ ) );
		}

		private void cmdSetAsCapital_Click(object sender, EventArgs e)
		{
			Form1.game.playerList[ player ].cityList[ Form1.game.playerList[ player ].capital ].isCapitale = false;
			Form1.game.playerList[ player ].cityList[ city ].isCapitale = true;
			enableButtons();
		}

		private void miTransfertSlave_Click(object sender, EventArgs e)
		{
			userNumberInput uni = new userNumberInput( 
				"Transfert slaves", 
				"How many slaves do you want to transfert? max: " + Form1.game.playerList[ player ].cityList[ city ].slaves.total.ToString(),
				1,
				Form1.game.playerList[ player ].cityList[ city ].slaves.total,
				"Ok",
				"Cancel"
				);

			uni.ShowDialog();

			if ( uni.result != -1 )
			{
				int[] order = new int[ Form1.game.playerList[ player ].cityNumber ];
				int tot = 0;
				for ( int c = 1; c <= Form1.game.playerList[ player ].cityNumber; c ++ )
					if ( !Form1.game.playerList[ player ].cityList[ c ].dead && c != city )
					{
						order[ tot ] = c;
						tot++;
					}
				string[] names = new string[ tot ];

				for ( int c = 0; c < names.Length; c++ )
				{
					names[ c ] = Form1.game.playerList[ player ].cityList[ order[ c ] ].name;
				}

				userChoice uc = new userChoice( 
					"Transfert slaves", 
					"Please choose where to send the slaves.",
					names,
					0,
					"Ok",
					"Cancel"
					);
				uc.ShowDialog();

				if ( uc.result != -1 )
				{
					int eta = Form1.game.playerList[ player ].slaves.moveSlave( city, order[ uc.result ], uni.result );

					MessageBox.Show( String.Format( "The slaves will be at destination in {0} turns.", eta ), "Slave transfert" );
				}
			}

			enableButtons();
		}
	}
}