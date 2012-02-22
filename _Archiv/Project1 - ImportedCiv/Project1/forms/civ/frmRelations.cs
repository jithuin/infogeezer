using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frmRelations.
	/// </summary>
	public class frmRelations : System.Windows.Forms.Form
	{ 
		private System.Windows.Forms.PictureBox picBox; 
		private System.Windows.Forms.MenuItem cmdGeneral, cmdSpies, cmdCouncil; 
		byte[] players; 
		int posAtTop, spySelected, spyWidth = 160, xo = 8, yo, yDiff = 50 * platformSpec.resolution.mod;
		byte player, currentType, maxPlayersOnScreen = 4; 
		Button[] contact, spyDetail, spyAdd;
		Button spyAddCi, councilPropose;
		SpyBar[] sbNation;
		SpyBar sbCounterInt;
		Graphics g;
		Bitmap back;
		Timer tmr, butDelay;
	//	keyInPut keyIn;
		ComboBox councilCB;
		bool treatyProposed;
	
		public frmRelations( byte player )
		{
			this.player = player;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		/*	picBox = new PictureBox();
			picBox.Width = this.Width;
			picBox.Height = this.Height;
			this.Controls.Add( picBox );*/

		/*	tabCouncil.Controls.Add( picBox );
			tabSpies.Controls.Add( picBox );*/

			MainMenu menu = new MainMenu();
			
			cmdGeneral = new MenuItem();
			cmdGeneral.Click += new System.EventHandler( this.cmdGeneral_Click );
			cmdGeneral.Text = language.getAString( language.order.rfGeneral );
			menu.MenuItems.Add( cmdGeneral );

			MenuItem mis0 = new MenuItem();
			mis0.Text = "|";
			mis0.Enabled = false;
			menu.MenuItems.Add( mis0 );

			cmdSpies = new MenuItem();
			cmdSpies.Click += new System.EventHandler( this.cmdSpies_Click );
			cmdSpies.Text = language.getAString( language.order.rfSpies );
			menu.MenuItems.Add( cmdSpies );

			MenuItem mis1 = new MenuItem();
			mis1.Text = "|";
			mis1.Enabled = false;
			menu.MenuItems.Add( mis1 );

			cmdCouncil = new MenuItem(); 
			cmdCouncil.Click += new System.EventHandler( this.cmdCouncil_Click );
			cmdCouncil.Text = language.getAString( language.order.rfCouncil );
			menu.MenuItems.Add( cmdCouncil );

			this.Text = language.getAString( language.order.relation );

			this.Menu = menu;
			picBox.Size = this.ClientSize;


			int pos = 0; 
			byte[] tempPlayers = new byte[ Form1.game.playerList.Length - 1 ]; 
			for ( int p = 0; p < Form1.game.playerList.Length; p ++ ) 
				if ( 
					p != player && 
					!Form1.game.playerList[ p ].dead && 
					Form1.game.playerList[ player ].foreignRelation[ p ].madeContact 
					) 
				{ 
					tempPlayers[ pos ] = (byte)p; 
					pos++; 
				} 

			players = new byte[ pos ]; 
			for ( int p = 0; p < pos; p++ ) 
				players[ p ] = tempPlayers[ p ]; 

			back = new Bitmap( picBox.Width, picBox.Height );
			g = Graphics.FromImage( back );

			sbNation = new SpyBar[ players.Length ]; // players.Length - posAtTop > maxPlayersOnScreen ? maxPlayersOnScreen : players.Length - posAtTop ];			
			for ( int p = 0; p < players.Length; p++ )
			{
				int spyInNation = Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.people ].nbr + Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.gov ].nbr + Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.military ].nbr + Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.science ].nbr;

				sbNation[ p ] = new SpyBar( this.Width - 8 - spyWidth, p * yDiff + yo, spyWidth );
				sbNation[ p ].backColor = Color.Wheat;
				sbNation[ p ].enabled = true;
				sbNation[ p ].spyNbr = spyInNation;
				sbNation[ p ].picBox.MouseDown += new MouseEventHandler( sbNation_Click );
			}

		//	keyIn = new keyInPut();
			tmr = new Timer();
			tmr.Interval = 5;
			tmr.Tick += new EventHandler( tmr_Tick );
			tmr.Enabled = true;

			platformSpec.resolution.set( this.Controls );

			posAtTop = 0;
			currentType = 0;
			changeType();
			enabledTypes();
		//	drawBack();
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
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
			this.picBox = new System.Windows.Forms.PictureBox();
		/*	this.cmdGeneral = new System.Windows.Forms.Button();
			this.cmdSpies = new System.Windows.Forms.Button();
			this.cmdCouncil = new System.Windows.Forms.Button();


			// 
			// cmdGeneral
			// 
			this.cmdGeneral.Location = new System.Drawing.Point( 0, 273 );
			this.cmdGeneral.Size = new System.Drawing.Size( 80, 24 );
			platformSpec.resolution.setOne( cmdGeneral );
			this.cmdGeneral.Click += new System.EventHandler( this.cmdGeneral_Click );

			// 
			// cmdSpies
			// 
			this.cmdSpies.Location = new System.Drawing.Point( 80, 273 );
			this.cmdSpies.Size = new System.Drawing.Size( 80, 24 );
			platformSpec.resolution.setOne( cmdSpies );
			this.cmdSpies.Click += new System.EventHandler( this.cmdSpies_Click );

			// 
			// cmdCouncil
			// 
			this.cmdCouncil.Location = new System.Drawing.Point( 160, 273 );
			this.cmdCouncil.Size = new System.Drawing.Size( 80, 24 );
			platformSpec.resolution.setOne( cmdCouncil );
			this.cmdCouncil.Click += new System.EventHandler( this.cmdCouncil_Click );*/

			// 
			// picBox
			// 
	//		this.picBox.Size = new System.Drawing.Size( this.ClientSize.Width, this.ClientSize.Height - cmdGeneral.Height );
			this.picBox.Click += new EventHandler(picBox_Click);

			// 
			// frmRelations
			// 
			this.ClientSize = new System.Drawing.Size( 240, 296 );
		//	this.Controls.Add( this.cmdCouncil );
		//	this.Controls.Add( this.cmdSpies );
		//	this.Controls.Add( this.cmdGeneral );
			this.Controls.Add( this.picBox );
		//	this.KeyDown += new KeyEventHandler(frmRelations_KeyDown);
		}
		#endregion

		private void picBox_Click(object sender, EventArgs e)
		{
			System.Drawing.Point pntClicked = picBox.PointToClient( new Point( frmRelations.MousePosition.X, frmRelations.MousePosition.Y ) );

			if ( posAtTop > 0 && pntClicked.Y > yo - 10 && pntClicked.Y < yo )
			{
				posAtTop --;
				changeType();
			}
			else if ( players.Length - maxPlayersOnScreen > posAtTop && pntClicked.Y > yo + ( yDiff * maxPlayersOnScreen ) && pntClicked.Y < yo + ( yDiff * maxPlayersOnScreen ) + 10 )
			{
				posAtTop ++;
				changeType();
			}
		}

		private void cmdGeneral_Click(object sender, System.EventArgs e)
		{
			currentType = (byte)types.general;
			enabledTypes();
		}
		private void cmdCouncil_Click(object sender, System.EventArgs e)
		{
			currentType = (byte)types.council;
			enabledTypes();
		}
		private void cmdSpies_Click(object sender, System.EventArgs e)
		{
			currentType = (byte)types.spies;
			enabledTypes();
		}
		private void enabledTypes()
		{
			changeType();
			cmdGeneral.Enabled = true;
			cmdSpies.Enabled = true;


			bool someoneHMSE = false;
			for ( int p0 = 0; p0 < Form1.game.playerList.Length && !someoneHMSE; p0++ )
				if ( !Form1.game.playerList[ p0 ].dead )
					for ( int p1 = 0; p1 < Form1.game.playerList.Length && !someoneHMSE; p1++ )
						if ( p0 != p1 && !Form1.game.playerList[ p1 ].dead && !Form1.game.playerList[ p0 ].foreignRelation[ p1 ].madeContact )
							someoneHMSE = true;

			if ( someoneHMSE )
				cmdCouncil.Enabled = false;
			else
				cmdCouncil.Enabled = true;


			switch ( currentType )
			{
				case (byte)types.general :
					cmdGeneral.Enabled = false;
					break;
					
				case (byte)types.council :
					cmdCouncil.Enabled = false;
					break;
					
				case (byte)types.spies :
					cmdSpies.Enabled = false;
					break;
			}

		}
		private enum types 
		{ 
			general, 
			spies, 
			council 
		} 


#region draw and place
		private void changeType()
		{
			this.Controls.Clear();
			this.Controls.Add( picBox );
	//		this.Controls.Add( this.cmdCouncil );
	//		this.Controls.Add( this.cmdSpies );
	//		this.Controls.Add( this.cmdGeneral );
	//		cmdCouncil.BringToFront();
	//		cmdSpies.BringToFront();
	//		cmdGeneral.BringToFront();
			

			g.Clear( Color.Wheat );

			Font pnFont = new Font( "Tahoma", 12, FontStyle.Regular ),
				pFont = new Font( "Tahoma", 10, FontStyle.Regular );
			Brush blackBrush = new SolidBrush( Color.Black );
		//	int spyWidth = 100;

		#region once
			switch ( currentType )
			{
				case (byte)types.general :
					yo = 62;
					//yo = 4;
					contact = new Button[ players.Length - posAtTop > maxPlayersOnScreen ? maxPlayersOnScreen : players.Length - posAtTop ];
					g.DrawLine( new Pen( Color.Black ), 0, yo - 10, this.Width, yo - 10 );
					break;

				case (byte)types.council :
					yo = 62;
					g.DrawLine( new Pen( Color.Black ), 0, yo - 10, this.Width, yo - 10 );
					break;

				case (byte)types.spies :
					spyDetail = new Button[ players.Length - posAtTop > maxPlayersOnScreen ? maxPlayersOnScreen : players.Length - posAtTop ];
					spyAdd = new Button[ players.Length - posAtTop > maxPlayersOnScreen ? maxPlayersOnScreen : players.Length - posAtTop ];

					yo = 62;
					SizeF ciSize = g.MeasureString( language.getAString( language.order.counterIntelligence ), new Font( "Tahoma", 9, FontStyle.Regular ) );
					g.DrawString( language.getAString( language.order.counterIntelligence ), new Font( "Tahoma", 9, FontStyle.Regular ), new SolidBrush( Color.Black ), 4, 4 );

					g.DrawLine( new Pen( Color.Black ), 0, yo - 10, this.Width, yo - 10 );
					int counterSpy = Form1.game.playerList[ player ].counterIntNbr;

					int cis = 0;
					if ( sbCounterInt != null )
						cis = sbCounterInt.selected;

					sbCounterInt = new SpyBar( 4, 4 + (int)ciSize.Height, this.Width - 8 );
					sbCounterInt.picBox.Parent = this;
					sbCounterInt.picBox.BringToFront();
					sbCounterInt.enabled = true;
					sbCounterInt.spyNbr = counterSpy;
					sbCounterInt.backColor = Color.Wheat;
					sbCounterInt.selected = cis;
					sbCounterInt.drawSpies();
					sbCounterInt.drawAff();
					sbCounterInt.picBox.MouseDown += new MouseEventHandler( picBoxCI_MouseDown );
					sbCounterInt.picBox.MouseUp += new MouseEventHandler( picBoxCI_MouseDown );

					spyAddCi = new Button();
					spyAddCi.Top = sbCounterInt.Height; // +sbCounterInt.y;
					spyAddCi.Text = "+";
					spyAddCi.Height = 16;
					spyAddCi.Width = spyAddCi.Height;
					spyAddCi.Left = this.Width - spyAddCi.Width - 8;
					spyAddCi.Parent = this;
				//	spyAddCi.Enabled = false;
					spyAddCi.BringToFront();
					spyAddCi.Click += new EventHandler( cmdAddCi_Click );
#if !CF
			spyAddCi.FlatStyle = FlatStyle.System;
#endif
					break;
			}
		#endregion once

		#region loop
			for ( int p = posAtTop, pos = 0; p < players.Length && pos < maxPlayersOnScreen; p++, pos++ )
			{
				g.FillRectangle( 
				new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ players[ p ] ].civType ].color ), 
				0, 
				yo + ( yDiff * pos ),
				xo - 2,
				yDiff 
				);
				g.DrawRectangle( 
				new Pen( Color.Black ), 
				-1, 
				yo + ( yDiff * pos ), 
				xo - 1, 
				yDiff 
				);

				g.DrawString( 
				Statistics.civilizations[ Form1.game.playerList[ players[ p ] ].civType ].name, 
				pnFont, 
				blackBrush, 
				xo, 
				yo + ( yDiff * pos ) 
				);
				g.DrawString( 
				Form1.relationPol[ Form1.game.playerList[ players[ p ] ].foreignRelation[ player ].politic ], 
				pFont, 
				blackBrush, 
				xo, 
				yo + ( yDiff * pos ) + g.MeasureString( Statistics.civilizations[ Form1.game.playerList[ players[ p ] ].civType ].name, pnFont ).Height 
				);

				switch ( currentType )
				{
					case (byte)types.general :
						contact[ pos ] = new Button();
						contact[ pos ].Text = language.getAString( language.order.contactNation );//"Contact";
						contact[ pos ].Left = this.Width - contact[ pos ].Width - xo;
						contact[ pos ].Top = pos * yDiff + yo;

						this.Controls.Add( contact[ pos ] );
						platformSpec.resolution.setOne( contact[ pos ] );

						contact[ pos ].BringToFront();
						contact[ pos ].Click += new EventHandler( contact_Click );
#if !CF
			contact[ pos ].FlatStyle = FlatStyle.System;
#endif
						break;

					case (byte)types.council :
						break;

					case (byte)types.spies :
						int spyInNation = 
						Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.people ].nbr + 
						Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.gov ].nbr + 
						Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.military ].nbr + 
						Form1.game.playerList[ player ].foreignRelation[ players[ p ] ].spies[ (byte)enums.spyType.science ].nbr;

						sbNation[ p ].x = this.Width - 8 - spyWidth;
						sbNation[ p ].y = pos * yDiff + yo;

						sbNation[ p ].picBox.Parent = this;
						platformSpec.resolution.setOne( sbNation[ p ].picBox );

						sbNation[ p ].picBox.BringToFront();
						sbNation[ p ].enabled = true;
						sbNation[ p ].spyNbr = spyInNation;
						sbNation[ p ].drawSpies();
						sbNation[ p ].drawAff();
						sbNation[ p ].picBox.MouseDown += new MouseEventHandler( sbNation_Click );
						sbNation[ p ].picBox.MouseUp += new MouseEventHandler( sbNation_Click );
						
						spyDetail[ pos ] = new Button();
						spyDetail[ pos ].Top = pos * yDiff + yo + sbNation[ pos ].Height;
						spyDetail[ pos ].Text = language.getAString( language.order.Details );//"Details";
						spyDetail[ pos ].Height = 16;
						spyDetail[ pos ].Left = this.Width - spyDetail[ pos ].Width - 4;

						spyDetail[ pos ].Parent = this;
						platformSpec.resolution.setOne( spyDetail[ pos ] );

						spyDetail[ pos ].BringToFront();
						spyDetail[ pos ].Click += new EventHandler( spyDetail_Click );
#if !CF
			spyDetail[ pos ].FlatStyle = FlatStyle.System;
#endif

						spyAdd[ pos ] = new Button();
						spyAdd[ pos ].Top = spyDetail[ pos ].Top; // p * yDiff + yo + 120;
						spyAdd[ pos ].Text = "+";
						spyAdd[ pos ].Height = 16;
						spyAdd[ pos ].Width = spyAdd[ pos ].Height;
						spyAdd[ pos ].Left = this.Width - spyDetail[ pos ].Width - spyAdd[ pos ].Width - 8;

						spyAdd[ pos ].Parent = this;
						platformSpec.resolution.setOne( spyAdd[ pos ] );

						spyAdd[ pos ].Enabled = false;
						spyAdd[ pos ].BringToFront();
						spyAdd[ pos ].Click += new EventHandler( spyAdd_Click );
#if !CF
			spyAdd[ pos ].FlatStyle = FlatStyle.System;
#endif
						break;
				}
			}
		#endregion loop

			if ( currentType == (byte)types.spies  )
				spyEnableWHTB();

			if ( posAtTop > 0 )
			{
				g.FillRectangle( new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ players[ posAtTop - 1 ] ].civType ].color ), 0, yo + ( yDiff * 0 ) - 10, xo - 2, 10 );
				g.DrawRectangle( new Pen( Color.Black ), -1, yo + ( yDiff * 0 ) - 10, xo - 1, 10 );
			}
			if ( players.Length - maxPlayersOnScreen > posAtTop )
			{
				g.FillRectangle( new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ players[ posAtTop + maxPlayersOnScreen ] ].civType ].color ), 0, yo + ( yDiff * maxPlayersOnScreen ), xo - 2, 11 );
				g.DrawRectangle( new Pen( Color.Black ), -1, yo + ( yDiff * maxPlayersOnScreen ), xo - 1, 11 );
			}
 
			picBox.Image = back;
		}

#endregion

#region contact_Click
		private void contact_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			for ( int p = posAtTop, pos = 0; p < players.Length && pos < maxPlayersOnScreen; p++, pos++ )
				if ( btn.Top == contact[ pos ].Top )
				{
					popContactPlayer cp = new popContactPlayer( player, players[ p ] );

					cp.ShowDialog();
					enabledTypes();
					break;
				}
		}
			
		#endregion

#region spyEnableWHTB
		private void spyEnableWHTB() // byte sender)
		{
			if ( spySelected == 100 )
			{
				if ( sbCounterInt.selected > 0 )
				{
					for ( int i = 0; i < sbNation.Length; i++ )
						if ( sbNation[ i ].selected != 0 )
						{
							sbNation[ i ].selected = 0;
							sbNation[ i ].drawAff();
						}

					for ( int i = 0; i < spyAdd.Length; i++ )
						spyAdd[ i ].Enabled = true;
				}
				else
				{
					for ( int i = 0; i < spyAdd.Length; i++ )
						spyAdd[ i ].Enabled = false;
				}
				
				spyAddCi.Enabled = false;
			}
			else
			{
				if ( sbNation[ spySelected ].selected > 0 )
				{
					for ( int i = 0; i < sbNation.Length; i++ )
						if ( i != spySelected )
							if ( sbNation[ i ].selected != 0 )
							{
								sbNation[ i ].selected = 0;
								sbNation[ i ].drawAff();
							}

					for ( int i = 0; i < spyAdd.Length; i++ )
						if ( i + posAtTop != spySelected )
							spyAdd[ i ].Enabled = true;
						else
							spyAdd[ i ].Enabled = false;

					//sbCounterInt.selected = 0;
					spyAddCi.Enabled = true;
				}
				else
				{
					spyAddCi.Enabled = false;
					for ( int i = 0; i < spyAdd.Length; i++ )
						spyAdd[ i ].Enabled = false;
				}

				sbCounterInt.selected = 0;
				sbCounterInt.drawSpies();
				sbCounterInt.drawAff();
			}
		}
#endregion

#region spyAdd_Click
		private void spyAdd_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			for ( int p = posAtTop, pos = 0; p < players.Length && pos < maxPlayersOnScreen; p++, pos++ )
				if ( btn.Top == spyAdd[ pos ].Top )
				{
					if ( spySelected == 100 )
					{ // from CI to a player
						addSpyTo( player, players[ pos + posAtTop ], sbCounterInt.selected );
						Form1.game.playerList[ player ].counterIntNbr -= sbCounterInt.selected;
						sbCounterInt.spyNbr -= sbCounterInt.selected;
						sbNation[ pos + posAtTop ].spyNbr += sbCounterInt.selected;
						sbCounterInt.selected = 0;

						sbCounterInt.drawSpies();
						sbCounterInt.drawAff();
						sbNation[ pos + posAtTop ].drawSpies();
						sbNation[ pos + posAtTop ].drawAff();
					}
					else
					{
						removeSpyFrom(	Form1.game.curPlayerInd, players[ spySelected ],		sbNation[ spySelected ].selected );
						addSpyTo(		Form1.game.curPlayerInd, players[ pos + posAtTop ],		sbNation[ spySelected ].selected );
						sbNation[ spySelected ].spyNbr -= sbNation[ spySelected ].selected;
						sbNation[ pos + posAtTop ].spyNbr += sbNation[ spySelected ].selected;
						sbNation[ spySelected ].selected = 0;
						sbNation[ spySelected ].drawSpies();
						sbNation[ spySelected ].drawAff();
						sbNation[ pos + posAtTop ].drawSpies();
						sbNation[ pos + posAtTop ].drawAff();
					}

					spyEnableWHTB();
				}
		}
			
#endregion

#region spyDetail_Click		
		private void spyDetail_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			for ( int p = posAtTop, pos = 0; p < players.Length && pos < maxPlayersOnScreen; p++, pos++ )
				if ( btn.Top == spyDetail[ pos ].Top )
				{
					string titre = this.Text;

					platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";

					frmSpyDetail fsd = new frmSpyDetail( players[ p ] );

					fsd.ShowDialog();
					this.Text = titre;
					break;
				}
		} 
#endregion

#region addSpyTo
		public static void addSpyTo(byte player, byte toPlayer, int nbr)
		{
			int[] ori = new int[ 4 ];
			int[] add = new int[ 4 ];
			int tot = 0;

			for ( int i = 0; i < 4; i++ )
			{
				ori[ i ] = Form1.game.playerList[ player ].foreignRelation[ toPlayer ].spies[ i ].nbr;
				if ( ori[ i ] == 0 )
					ori[ i ] = 1;

				tot += ori[ i ];
			}

			for ( int i = 0; i < 4; i++ )
				add[ i ] = ori[ i ] * nbr / tot;

			Random r = new Random();

			while ( add[ 0 ] + add[ 1 ] + add[ 2 ] + add[ 3 ] > nbr )
			{
				int ran = r.Next( 4 );

				if ( add[ ran ] > 0 )
					add[ ran ]--;
			}

			while ( add[ 0 ] + add[ 1 ] + add[ 2 ] + add[ 3 ] < nbr )
				add[ r.Next( 4 ) ]++;

			for ( int i = 0; i < 4; i++ )
				Form1.game.playerList[ player ].foreignRelation[ toPlayer ].spies[ i ].nbr += add[ i ];
		}
#endregion

#region removeSpyFrom
		public static void removeSpyFrom(byte player, byte fromPlayer, int nbr)
		{
			int[] ori = new int[ 4 ];
			int[] remove = new int[ 4 ];
			int tot = 0;

			for ( int i = 0; i < 4; i++ )
			{
				ori[ i ] = Form1.game.playerList[ player ].foreignRelation[ fromPlayer ].spies[ i ].nbr;
				tot += ori[ i ];
			}

			for ( int i = 0; i < 4; i++ )
				remove[ i ] = ori[ i ] * nbr / tot;

			Random r = new Random();

			while ( remove[ 0 ] + remove[ 1 ] + remove[ 2 ] + remove[ 3 ] > nbr )
			{ // si en enleve trop
				int ran = r.Next( 4 );

				if ( remove[ ran ] > 0 )
					remove[ ran ]--;
			}

			while ( remove[ 0 ] + remove[ 1 ] + remove[ 2 ] + remove[ 3 ] < nbr )
			{ // en enleve pas assez
				int ran = r.Next( 4 );

				if ( ori[ ran ] > remove[ ran ] )
					remove[ ran ]++;
			}

			for ( int i = 0; i < 4; i++ )
				Form1.game.playerList[ player ].foreignRelation[ fromPlayer ].spies[ i ].nbr -= remove[ i ];
		}
#endregion

#region sbNation_Click
		private void sbNation_Click(object sender, MouseEventArgs e) 
		{ 
			PictureBox btn = (PictureBox)sender; 

			for ( int p = posAtTop, pos = 0; p < players.Length && pos < maxPlayersOnScreen; p++, pos++ ) 
				if ( btn.Top == sbNation[ p ].picBox.Top ) 
				{ 
					spySelected = p; //pos + posAtTop; 
					spyEnableWHTB();// (byte)(p) ); //pos + posAtTop
				} 
		} 
#endregion

#region drawCounterIntBox
	/*	private void counterIntBox()
		{*/
		/*	SizeF ciSize = g.MeasureString( "Counter-intelligence", new Font( "Tahoma", 9, FontStyle.Regular ) );
			g.DrawString( "Counter-intelligence", new Font( "Tahoma", 9, FontStyle.Regular ), new SolidBrush( Color.Black ), 4, 4 );*/
/*
			int counterSpy = Form1.game.playerList[ player ].counterIntNbr; 
			sbCounterInt = new SpyBar( 4, 4 /*+ (int)ciSize.Height*//*, this.Width - 8 );
			sbCounterInt.picBox.Parent = this;
			sbCounterInt.picBox.BringToFront();
			sbCounterInt.enabled = true;
			sbCounterInt.spyNbr = counterSpy;
			sbCounterInt.backColor = Color.Wheat;
			sbCounterInt.drawSpies();
			sbCounterInt.drawAff();
			sbCounterInt.picBox.MouseDown += new MouseEventHandler( picBoxCI_MouseDown );

			spyAddCi = new Button();
		//	spyAddCi.Top = picBox2.Top + 4;
			spyAddCi.Text = "+";
			spyAddCi.Height = 16;
			spyAddCi.Width = spyAddCi.Height;
			spyAddCi.Left = this.Width - spyAddCi.Width - 8;
			spyAddCi.Parent = this;
			spyAddCi.Enabled = false;
			spyAddCi.BringToFront();
			spyAddCi.Click += new EventHandler( cmdAddCi_Click );
		}*/
#endregion
		
#region cmdAddCi_Click
		private void cmdAddCi_Click(object sender, EventArgs e)
		{ // from a player to CI
			removeSpyFrom( player, players[ spySelected ], sbNation[ spySelected ].selected );
			sbNation[ spySelected ].spyNbr -= sbNation[ spySelected ].selected;
			sbCounterInt.spyNbr += sbNation[ spySelected ].selected;
			Form1.game.playerList[ player ].counterIntNbr += sbNation[ spySelected ].selected;
			sbNation[ spySelected ].selected = 0;

			sbNation[ spySelected ].drawSpies();
			sbNation[ spySelected ].drawAff();
			sbCounterInt.drawSpies();
			sbCounterInt.drawAff();
			spyEnableWHTB();
		}
#endregion

#region picBoxCI_MouseDown
		private void picBoxCI_MouseDown(object sender, MouseEventArgs e)
		{
			spySelected = 100;
			spyEnableWHTB(); // 100 );
		}
#endregion

#region timer
		private void tmr_Tick(object sender, EventArgs e)
		{
			/*switch ( //   platformSpec.keys.getDirection ) //keyIn.testButtons() )
			{
				case 1 :
					if ( posAtTop < players.Length - maxPlayersOnScreen )
					{
						posAtTop++;
						changeType();
					}

					break;

				case 2 :
					if ( posAtTop > 0 )
					{
						posAtTop--;
						changeType();
					}

					break;
			}*/
		}

#endregion

		//private void enableTypes

	//	private void frmRelations_KeyDown(object sender, KeyEventArgs e)
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch ( e.KeyCode )
			{
				case System.Windows.Forms.Keys.Down:
					if ( posAtTop < players.Length - maxPlayersOnScreen )
					{
						posAtTop++;
						changeType();
					}
					break;

				case System.Windows.Forms.Keys.Up :
					if ( posAtTop > 0 )
					{
						posAtTop--;
						changeType();
					}
					break;
			}
		}
	}
}
