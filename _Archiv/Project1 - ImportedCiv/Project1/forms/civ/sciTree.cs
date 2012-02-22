using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for sciTree.
	/// </summary>
	public class sciTree : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button cmdUp;
		private System.Windows.Forms.Button cmdDown;

		//Graphics g;
		SolidBrush availableBrush, researchedBrush, currentBrush, futureBrush, specialBrush, fontBrush;

		int adjVer, adjHor;
		private System.Windows.Forms.PictureBox pictureBox2;
		SizeF[] technoNameSize;
	//	private System.Windows.Forms.Timer timer1;
		byte curTechno;

		static byte defaultVer = 24;
		static byte defaultHor = 24;

		Bitmap sciTreeBmp;
		Graphics g;

	//	keyInPut keyIn;
	
		public sciTree()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		/*	this.ControlBox = true;
			this.MinimizeBox = false;*/

			adjHor = this.ClientSize.Width / 2;

			//   platformSpec.keys.Set( this );
			this.Closing += new CancelEventHandler(sciTree_Closing);

			currentBrush =		new SolidBrush( Color.LimeGreen );
			researchedBrush =	new SolidBrush( Color.MediumSeaGreen );
			availableBrush =	new SolidBrush( Color.CornflowerBlue );
			futureBrush =		new SolidBrush( Color.LightGray );
			specialBrush =		new SolidBrush( Color.BurlyWood );
			fontBrush =			new SolidBrush( Color.Black );

			technoNameSize = new SizeF[ Statistics.technologies.Length ];

			cmdCCR = new Button();
			cmdCCR.Parent = this;
			cmdCCR.Click += new EventHandler( cmdCCR_Click );
#if !CF
			cmdCCR.FlatStyle = FlatStyle.System;
#endif
			cmdInfo = new Button();
			cmdInfo.Parent = this;
#if !CF
			cmdInfo.FlatStyle = FlatStyle.System;
#endif
			cmdInfo.Click += new EventHandler(cmdInfo_Click);

			curTechno = Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch;
			showTechnoInfo( curTechno );

			if ( !Form1.options.showOnScreenDPad )
			{
				cmdUp.Visible = false;
				cmdDown.Visible = false;
			}
			else
			{
				verifyCmds();
			}

		//	keyIn = new keyInPut();
		//	timer1.Enabled = true;

			sciTreeBmp = new Bitmap( 240, 296); 
		//	g = Graphics.FromImage( sciTreeBmp );

			drawTree();
			int nt = count.technoNumber( Form1.game.curPlayerInd );

			if (
				Form1.game.curTurn == 0 && 
				count.technoNumber( Form1.game.curPlayerInd ) == 0
				)
				MessageBox.Show( "You won't be able to research any technology until you build a city and complete one turn.", "No scientist yet" );
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
			this.cmdUp = new System.Windows.Forms.Button();
			this.cmdDown = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
		//	this.timer1 = new System.Windows.Forms.Timer();
			// 
			// cmdUp
			// 
			this.cmdUp.Location = new System.Drawing.Point(208, 8);
			this.cmdUp.Size = new System.Drawing.Size(24, 24);
			this.cmdUp.Text = "A";
			this.cmdUp.Click += new System.EventHandler(this.cmdUp_Click);
			// 
			// cmdDown
			// 
			this.cmdDown.Location = new System.Drawing.Point(208, 200);
			this.cmdDown.Size = new System.Drawing.Size(24, 24);
			this.cmdDown.Text = "V";
			this.cmdDown.Click += new System.EventHandler(this.cmdDown_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Size = new System.Drawing.Size(240, 296);
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point(0, 232);
			this.pictureBox2.Size = new System.Drawing.Size(240, 64);
			// 
			// timer1
			// 
		/*	this.timer1.Interval = 20;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);*/
			// 
			// sciTree
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.cmdDown);
			this.Controls.Add(this.cmdUp);
			this.Controls.Add(this.pictureBox1);
			this.Load += new System.EventHandler(this.sciTree_Load);
			this.Closed += new System.EventHandler( this.sciTree_Closed );
			this.Text = "Science tree";
		//	this.KeyDown += new KeyEventHandler( this.sciTree_KeyDown );

		}
		#endregion

#region draw Tree
		private void drawTree()
		{
			//g = null;

			try 
			{
				sciTreeBmp = new Bitmap( pictureBox1.Width, pictureBox1.Height );
				g = Graphics.FromImage( sciTreeBmp );
				g.Clear( Color.White );
				Font sciTreeFont = new Font( "tahoma", 9, FontStyle.Regular );

				for ( byte i = 1; i < Statistics.technologies.Length; i++ )
				{
				/*	Rectangle r = new Rectangle (
						Statistics.technologies[ i ].rect.Left + adjHor, 
						Statistics.technologies[ i ].rect.Top + adjVer, 
						Statistics.technologies[ i ].rect.Width, 
						Statistics.technologies[ i ].rect.Height
						);*/
					Rectangle r = getDrawR( i );
					/*new Rectangle (
						Statistics.technologies[ i ].rect.Left + adjHor, 
						pictureBox2.Top - Statistics.technologies[ i ].rect.Top + adjVer, 
						Statistics.technologies[ i ].rect.Width, 
						Statistics.technologies[ i ].rect.Height
						);*/

#region lines
					Pen techPen = new Pen( Color.Black );
					for ( byte j = 0; j < 3; j ++ )
						if ( Statistics.technologies[ i ].needs[ j ] != 0 )
						{
							Rectangle rLinks = getDrawR( Statistics.technologies[ i ].needs[ j ] );
							g.DrawLine( 
								techPen, 
								r.Left + r.Width / 2,
								r.Bottom,
								rLinks.Left + rLinks.Width / 2,
								rLinks.Top
							/*	Statistics.technologies[ Statistics.technologies[ i ].needs[ j ] ].rect.Left + Statistics.technologies[ Statistics.technologies[ i ].needs[ j ] ].rect.Width / 2 + adjHor,
								pictureBox2.Top - Statistics.technologies[ Statistics.technologies[ i ].needs[ j ] ].rect.Top + adjVer*/
								);
						}
						else
							break;
#endregion

					if ( r.Bottom > 0 && r.Top < pictureBox1.Height )
					{ // draw the rectangle only if it is within the picbox
						if ( Form1.game.playerList[ 0 ].technos[ i ].researched == true ) // needs 3
						{
							g.FillRectangle( researchedBrush, r);
							g.DrawString( 
								" " + Statistics.technologies[ i ].name, 
								sciTreeFont, 
								fontBrush,
								r
								);
						}
						else if ( !Statistics.technologies[ i ].canBeResearched )
						{
							g.FillRectangle( specialBrush, r );
							g.DrawString( " " + Statistics.technologies[ i ].name, sciTreeFont, fontBrush, r);
						}
						else if ( Form1.game.playerList[ 0 ].currentResearch == i )
						{
							int perc = r.Width * Form1.game.playerList[ 0 ].technos[ i ].pntDiscovered / Statistics.technologies[ i ].cost;

							g.FillRectangle( researchedBrush, new Rectangle(
								r.Left,
								r.Top,
								perc,
								r.Height
								)
								);

							g.FillRectangle( currentBrush, new Rectangle(
								r.Left + perc,
								r.Top,
								r.Width - perc,
								r.Height
								)
								);
							g.DrawLine(
								new Pen( Color.DarkGreen ),
								r.Left + perc,
								r.Top,
								r.Left + perc,
								r.Bottom
								);
						//	g.FillRectangle( currentBrush, r );
							g.DrawString( " " + Statistics.technologies[ i ].name, sciTreeFont, fontBrush, r);
						}
						else if ( Form1.game.playerList[ 0 ].technos[ i ].pntDiscovered > 0 )
						{
							int perc = r.Width * Form1.game.playerList[ 0 ].technos[ i ].pntDiscovered / Statistics.technologies[ i ].cost;

							g.FillRectangle( researchedBrush, new Rectangle(
								r.Left,
								r.Top,
								perc,
								r.Height
								)
								);

							g.FillRectangle( availableBrush, new Rectangle(
								r.Left + perc,
								r.Top,
								r.Width - perc,
								r.Height
								)
								);
							g.DrawLine(
								new Pen( Color.DarkBlue ),
								r.Left + perc,
								r.Top,
								r.Left + perc,
								r.Bottom
								);

							g.DrawString( " " + Statistics.technologies[ i ].name, sciTreeFont, fontBrush, r);
						}
						else if ( Form1.game.playerList[ 0 ].technos[ Statistics.technologies[ i ].needs[ 0 ] ].researched && Form1.game.playerList[ 0 ].technos[ Statistics.technologies[ i ].needs[ 1 ] ].researched && Form1.game.playerList[ 0 ].technos[ Statistics.technologies[ i ].needs[ 2 ] ].researched ) // needs 3
						{
							g.FillRectangle( availableBrush, r);
							g.DrawString( " " + Statistics.technologies[ i ].name, sciTreeFont, fontBrush, r);
						}
						else
						{
							g.FillRectangle( futureBrush, r);
							g.DrawString( " " + Statistics.technologies[ i ].name, sciTreeFont, fontBrush, r);
						}

						if ( curTechno == i )
						{
							g.DrawRectangle( new Pen( Color.LightGray ), r);
						}
						else
						{
							g.DrawRectangle( new Pen( Color.Black ), r);
						}
					}
				}

				pictureBox1.Image = sciTreeBmp;
			}
			finally
			{
				/*sciTreeBmp = null;
				g = null;*/
			}
		}
	#endregion

#region picture box 1 click
		private void pictureBox1_Click(object sender, System.EventArgs e)
		{
			System.Drawing.Point pClick;
			pClick = pictureBox1.PointToClient(new Point(Form1.MousePosition.X, Form1.MousePosition.Y)); //bob1);

			for ( byte i = 1; i < Statistics.technologies.Length; i++ )
			{
				/*Rectangle r = new Rectangle (
					Statistics.technologies[ i ].rect.Left + adjHor, 
					Statistics.technologies[ i ].rect.Top + adjVer, 
					Statistics.technologies[ i ].rect.Width, 
					Statistics.technologies[ i ].rect.Height
					);*/
				Rectangle r = getDrawR( i );
				/*new Rectangle(
					Statistics.technologies[ i ].rect.Left + adjHor, 
					pictureBox2.Top - Statistics.technologies[ i ].rect.Top + adjVer, 
					Statistics.technologies[ i ].rect.Width, 
					Statistics.technologies[ i ].rect.Height
					);*/

				if ( 
					pClick.X >= r.Left && 
					pClick.X <= r.Right && 
					pClick.Y >= r.Top && 
					pClick.Y <= r.Bottom
					)
				{
					curTechno = i;
					showTechnoInfo( i );
					drawTree();
					break;
				}
			}
		}
	#endregion

#region show techno info
		Button cmdCCR, cmdInfo;
		Bitmap infoBmp;
		private void showTechnoInfo( byte techno )
		{
			infoBmp = new Bitmap( pictureBox2.Width, pictureBox2.Height );
			Graphics g = Graphics.FromImage( infoBmp );
			g.FillRectangle( new SolidBrush( Color.Wheat ), 0, 0, pictureBox2.Width - 1, pictureBox2.Height - 1 );
			g.DrawRectangle( new Pen( Color.Black ), 0, 0, pictureBox2.Width - 1, pictureBox2.Height - 1 );
			g.DrawString( 
				Statistics.technologies[ techno ].name, 
				new Font( "Tahoma", 10, FontStyle.Regular ),
				new SolidBrush( Color.Black ),
				50, 
				5
				);

		/*	cmdCCR = new Button();
			cmdCCR.Parent = this;*/
			cmdCCR.BringToFront();
			cmdCCR.Width = 80 * platformSpec.resolution.mod;
			cmdCCR.Height = 20 * platformSpec.resolution.mod;
			cmdCCR.Left = pictureBox2.Width - cmdCCR.Width - 5;
			cmdCCR.Top = pictureBox2.Top + 5;
#if !CF
			cmdCCR.FlatStyle = FlatStyle.System;
#endif

		/*	
			cmdInfo = new Button();
			cmdInfo.Parent = this;*/
			cmdInfo.BringToFront();
			cmdInfo.Height = 20 * platformSpec.resolution.mod;
			cmdInfo.Width = cmdInfo.Height;
			cmdInfo.Left = pictureBox2.Width - cmdCCR.Width - cmdInfo.Width - 10;
			cmdInfo.Top = pictureBox2.Top + 5;
			cmdInfo.Text = "?";

			if ( techno == Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch )
			{
				cmdCCR.Text = language.getAString( language.order.stResearching ); // "Researching";
				cmdCCR.Enabled = false;

				getPFT getPFT1 = new getPFT();

				float sciTurn; 
				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.science != 0 )
					sciTurn = ( Statistics.technologies[ Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch ].cost - Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch ].pntDiscovered ) * 100 / ( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.science );
				else
					sciTurn = 999;

				string strNTurn;
				if ( Convert.ToInt32( sciTurn ) > 1 )
					strNTurn = String.Format( language.getAString( language.order.turnLeft ), Convert.ToInt32( sciTurn ) );
				else
					strNTurn = String.Format( language.getAString( language.order.turnsLeft ), Convert.ToInt32( sciTurn ) );

				SizeF strSize = g.MeasureString( 
					strNTurn,
					new Font( "Tahoma", 8, FontStyle.Regular )
					);
				g.DrawString( 
					strNTurn,
					new Font( "Tahoma", 8, FontStyle.Regular ),
					new SolidBrush( Color.Black ),
					pictureBox2.Width - 5 - strSize.Width,
					cmdCCR.Bottom - pictureBox2.Top
					);
			}
			else if ( !Statistics.technologies[ techno ].canBeResearched )
			{
				if ( Form1.game.playerList[ 0 ].technos[ techno ].researched )
				{
					cmdCCR.Text = language.getAString( language.order.stFound ); // "Found";
					cmdCCR.Enabled = false;
				}
				else
				{
					cmdCCR.Text = language.getAString( language.order.stUnknown ); // "Unknown";
					cmdCCR.Enabled = false;
				}
			}
			else if ( Form1.game.playerList[ 0 ].technos[ techno ].researched )
			{
				cmdCCR.Text = language.getAString( language.order.stResearched ); // "Researched";
				cmdCCR.Enabled = false;
			}
			else if ( 
				Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ Statistics.technologies[ techno ].needs[ 0 ] ].researched && 
				Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ Statistics.technologies[ techno ].needs[ 1 ] ].researched && 
				Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ Statistics.technologies[ techno ].needs[ 2 ] ].researched
				)
			{
				cmdCCR.Text = language.getAString( language.order.stResearch ); // "Research";
				cmdCCR.Enabled = true;

				getPFT getPFT1 = new getPFT();

			/*	float sciTurn;
				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ techno ].pntDiscovered > 0 )*/
				float sciTurn;
				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.science != 0 )
					sciTurn = ( Statistics.technologies[ techno ].cost - Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ techno ].pntDiscovered ) * 100 / ( ( Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade * Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.science ) );
				else
					sciTurn = 999;
			/*	else
					sciTurn = Statistics.technologies[ techno ].cost * 10 / ( getPFT1.getNationTrade( Form1.game.curPlayerInd ) * Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.science );
*/
				string strNTurn;
				if ( Convert.ToInt32( sciTurn ) > 1 )
					strNTurn = String.Format( language.getAString( language.order.turnLeft ), Convert.ToInt32( sciTurn ) );
				else
					strNTurn = String.Format( language.getAString( language.order.turnsLeft ), Convert.ToInt32( sciTurn ) );


				SizeF strSize = g.MeasureString( 
					strNTurn,
					new Font( "Tahoma", 8, FontStyle.Regular )
					);
				g.DrawString( 
					strNTurn,
					new Font( "Tahoma", 8, FontStyle.Regular ),
					new SolidBrush( Color.Black ),
					pictureBox2.Width - 5 - strSize.Width,
					cmdCCR.Bottom - pictureBox2.Top
					);

			}
			else
			{
				cmdCCR.Text = language.getAString( language.order.stUnavailable ); // "Unavailable";
				cmdCCR.Enabled = false;
			}

			int j = 0;
			for ( int i = 0; i < Form1.nbrUnit; i ++ )
			{
				if ( Statistics.units[ i ].disponibility == techno )
				{
					g.DrawImage(
						Statistics.units[ i ].bmp,
						new Rectangle( 
							40 + j * 50, 
							pictureBox2.Height - 50, 
							70, 
							50 
							),
						0,
						0,
						70,
						50,
						GraphicsUnit.Pixel,
						Form1.ia
						);

					Font nameFont = new Font( "Tahoma", 8, FontStyle.Regular );
					SizeF nameSize = g.MeasureString( Statistics.units[ i ].name, nameFont );

					g.DrawString( 
						Statistics.units[ i ].name, 
						nameFont,
						new SolidBrush( Color.Black ),
						50 + j * 50 + 50 / 2 - nameSize.Width / 2,
						pictureBox2.Height - nameSize.Height - 2
						);

					j ++;
				}
			}

			if ( Statistics.technologies[ techno ].shortDesc != "" )
			{
				string strShortDesc = Statistics.technologies[ techno ].shortDesc;
				Font shortDescFont = new Font( "Tahoma", 10, FontStyle.Regular );
				SizeF shortDescSize = g.MeasureString( strShortDesc, shortDescFont );

				g.DrawString( 
					strShortDesc,
					shortDescFont,
					new SolidBrush( Color.Black ),
					pictureBox2.Width - shortDescSize.Width - 3,
					pictureBox2.Height - shortDescSize.Height - 2
					);
			}

			pictureBox2.Image = infoBmp;
		}
#endregion

		private void panel1CmdOk_Click(object sender, System.EventArgs e)
		{
		}

		private void sciTree_Closed( object sender, System.EventArgs e)
		{
		//	timer1.Enabled = false;
		//	this.Dispose();
		}

		private void cmdUp_Click(object sender, System.EventArgs e)
		{
			adjVer += 24;
			drawTree();
			verifyCmds();
		}

		private void cmdDown_Click(object sender, System.EventArgs e)
		{
			if ( adjVer > 0 )
			{
				adjVer -= 24;
				drawTree();
				verifyCmds();
			}
		}

		private void verifyCmds()
		{
			if ( adjVer <= 0 )
				cmdDown.Visible = false;
			else
				cmdDown.Visible = true;
		}

		private void cmdCCR_Click( object sender, EventArgs e )
		{
			Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch = curTechno;
			//Form1.game.playerList[ Form1.game.curPlayerInd ].researchPoints = Statistics.technologies[ curTechno ].cost;
			showTechnoInfo( curTechno );
			drawTree();
		}

		
	/*	protected override void OnPaint(PaintEventArgs e)
		{
		//	base.OnPaint( e );
			e.Graphics.Clear( Color.White );
		}*/


		protected override void OnResize(EventArgs e)
		{
			base.OnResize( e );

			if ( ClientSize.Width > 0 && ClientSize.Height > 0 && currentBrush != null )
			{
				pictureBox2.Width = this.ClientSize.Width;
				pictureBox2.Height = 64 * platformSpec.resolution.mod;
				pictureBox2.Top = this.ClientSize.Height - pictureBox2.Height;

				pictureBox1.Width = this.ClientSize.Width;
				pictureBox1.Height = this.ClientSize.Height - pictureBox2.Height;

				adjHor = this.ClientSize.Width / 2;

				drawTree();
				showTechnoInfo( curTechno );
			}
		}

	/*	private void timer1_Tick(object sender, System.EventArgs e)
		{
			.set();//
	
			switch ( //   platformSpec.keys.getDirection ) //keyIn.testButtons() )
			{
				case 1:
					if ( adjVer > 0 )
					{
						adjVer -= defaultVer;
						drawTree();
					}
					break;

				case 2:
					adjVer += defaultVer;
					drawTree();
					break;

				case 3:
					adjHor -= defaultHor;
					drawTree();
					break;

				case 4:
					adjHor += defaultHor;
					drawTree();
					break;
			}
			
		}*/

		//private void sciTree_KeyDown(object sender, KeyEventArgs e)
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch ( e.KeyCode )//keyIn.testButtons( ) )
			{
				case System.Windows.Forms.Keys.Down :
					if ( adjVer > 0 )
					{
						adjVer -= defaultVer;
						drawTree();
					}
					break;

				case System.Windows.Forms.Keys.Up :
					adjVer += defaultVer;
					drawTree();
					break;

				case System.Windows.Forms.Keys.Right :
					adjHor -= defaultHor;
					drawTree();
					break;

				case System.Windows.Forms.Keys.Left :
					adjHor += defaultHor;
					drawTree();
					break;
			}
		}

		private void cmdInfo_Click(object sender, EventArgs e)
		{
			frmInfo fi = new frmInfo( enums.infoType.techno, curTechno );

			string oldText = this.Text;
			this.Text = "";
			fi.ShowDialog();
			this.Text = oldText;
		}

		private void sciTree_Load(object sender, System.EventArgs e)
		{
		}

		private void sciTree_Closing(object sender, CancelEventArgs e)
		{
		//	//   platformSpec.keys.Set( Form1 );
		}

		private Rectangle getDrawR( int techno )
		{
			return new Rectangle (
				Statistics.technologies[ techno ].rect.Left * platformSpec.resolution.mod + adjHor, 
				pictureBox2.Top - Statistics.technologies[ techno ].rect.Top * platformSpec.resolution.mod + adjVer, 
				Statistics.technologies[ techno ].rect.Width, /* * platformSpec.resolution.sizeMod*/
				Statistics.technologies[ techno ].rect.Height // * platformSpec.resolution.mod
				);
		}
	}
}
