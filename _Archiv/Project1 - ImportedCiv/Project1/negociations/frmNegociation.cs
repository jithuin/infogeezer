using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for civNegociation.
	/// </summary>
	public class frmNegociation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ListBox[] lb;
		private System.Windows.Forms.Button cmdPropose;
		private System.Windows.Forms.Button cmdDiscuss;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.Button cmdRefuse;
		private System.Windows.Forms.Button cmdRemove;

//		static byte dealLenght = 25;
		byte curLB/*, winningWarByte*/;
		byte[] players;
		public negoList list;
		int space = 8;
		int[][] lbContent;

		public frmNegociation( byte player, byte other, sbyte voteFor )
		{
			//
			// Required for Windows Form Designer support
			//
			lb = new System.Windows.Forms.ListBox[ 2 ];
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.cmdPropose.Text = language.getAString( language.order.Propose );
			this.cmdRefuse.Text = language.getAString( language.order.Refuse );
			this.cmdDiscuss.Text = language.getAString( language.order.Discuss );
			this.cmdAdd.Text = language.getAString( language.order.Add );
			this.cmdRemove.Text = language.getAString( language.order.Remove );

			players = new byte[ 2 ];
			players[ 0 ] = player;
			players[ 1 ] = other;

			list = new negoList( players );

			cmdRefuse.Left = space;
			cmdRefuse.Top = this.Height - cmdRefuse.Height - space;
			cmdRefuse.Width = ( this.Width - 4 * space ) / 3;

			cmdDiscuss.Left = cmdRefuse.Right + space;
			cmdDiscuss.Top = this.Height - cmdRefuse.Height - space;
			cmdDiscuss.Width = ( this.Width - 4 * space ) / 3;

			cmdPropose.Enabled = false;
			cmdPropose.Left = cmdDiscuss.Right + space;
			cmdPropose.Top = this.Height - cmdRefuse.Height - space;
			cmdPropose.Width = ( this.Width - 4 * space ) / 3;

			cmdAdd.Width = ( this.Width - 4 * space ) / 3;
			cmdAdd.Left = (this.Width - cmdAdd.Width * 2)/3;
			cmdAdd.Top = cmdRefuse.Top - space - cmdAdd.Height;

			cmdRemove.Enabled = false;
			cmdRemove.Width = ( this.Width - 4 * space ) / 3;
			cmdRemove.Left = cmdAdd.Right + ( this.Width - cmdAdd.Width * 2 ) / 3;
			cmdRemove.Top = cmdRefuse.Top - space - cmdAdd.Height;

			lb[ 0 ].Left = space;
			lb[ 0 ].Width = (this.Width - 3 * space)/2;
			lb[ 0 ].Top = cmdAdd.Top - space - lb[ 0 ].Height;

			lb[ 1 ].Left = lb[ 0 ].Right + space;
			lb[ 1 ].Width = (this.Width - 3 * space)/2;
			lb[ 1 ].Top = cmdAdd.Top - space - lb[ 1 ].Height;

			lbContent = new int[ 2 ][ ];
			for ( int i = 0; i < players.Length; i++ )
				lbContent[ i ] = new int[ 100 ];

	#region init var

		/*	negos.treaties = new bool[ (byte)Form1.relationPolType.totp1 ];
			negos.threats = new bool[ 3 ];
			negos.playerGive = new structures.negoGive[ 2 ];
			lbContent = new structures.intByte[ 2 ][ ];
			for ( int i = 0; i < players.Length; i++ )
			{
				negos.playerGive[ i ].moneyLeft = Form1.game.playerList[ players[ i ] ].money;
				negos.playerGive[ i ].moneyGive = 0;
				negos.playerGive[ i ].technoGive = new bool[ Statistics.technologies.Length ];
				negos.playerGive[ i ].cityGive = new bool[ Form1.game.playerList[ players[ i ] ].cityNumber + 1 ];
				negos.playerGive[ i ].regionGive = new bool[ Form1.game.playerList.Length ];
				negos.playerGive[ i ].contactGive = new bool[ Form1.game.playerList.Length ];

				lbContent[ i ] = new structures.intByte[ 0 ];
			}

			byte winningWarByte = ai.whoIsWinning( players[ 0 ], players[ 1 ] );
			bool winningWarBool;

			if ( winningWarByte > 10 )
				winningWarBool = true;
			else
				winningWarBool = false;*/
	#endregion
			
			platformSpec.resolution.set( this.Controls );

			cmdRemove.Enabled = false;
			drawTop();
			populateLBs();
			enableWHTB();
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.lb[ 0 ] = new System.Windows.Forms.ListBox();
			this.lb[ 1 ] = new System.Windows.Forms.ListBox();
			this.cmdPropose = new System.Windows.Forms.Button();
			this.cmdRefuse = new System.Windows.Forms.Button();
			this.cmdDiscuss = new System.Windows.Forms.Button();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdRemove = new System.Windows.Forms.Button();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Size = new System.Drawing.Size(this.Width, this.Height);
			// 
			// lb[ 0 ]
			// 
			this.lb[ 0 ].Location = new System.Drawing.Point(8, 80);
			this.lb[ 0 ].Size = new System.Drawing.Size(104, 145);
			this.lb[ 0 ].SelectedIndexChanged += new System.EventHandler(lbs_SelectedIndexChanged);
			// 
			// lb[ 1 ]
			// 
			this.lb[ 1 ].Location = new System.Drawing.Point(128, 80);
			this.lb[ 1 ].Size = new System.Drawing.Size(104, 145);
			this.lb[ 1 ].SelectedIndexChanged += new System.EventHandler(lbs_SelectedIndexChanged);
			// 
			// cmdPropose
			// 
			this.cmdPropose.Location = new System.Drawing.Point(176, 264);
			this.cmdPropose.Size = new System.Drawing.Size(56, 24);
			this.cmdPropose.Text = "Propose";
			this.cmdPropose.Click += new EventHandler(cmdPropose_Click);
			// 
			// cmdRefuse
			// 
			this.cmdRefuse.Location = new System.Drawing.Point(8, 264);
			this.cmdRefuse.Size = new System.Drawing.Size(56, 24);
			this.cmdRefuse.Text = "Refuse";
			this.cmdRefuse.Click += new EventHandler(cmdRefuse_Click);
			// 
			// cmdDiscuss
			// 
			this.cmdDiscuss.Location = new System.Drawing.Point(80, 264);
			this.cmdDiscuss.Size = new System.Drawing.Size(80, 24);
			this.cmdDiscuss.Text = "Discuss";
			this.cmdDiscuss.Click += new EventHandler(cmdDiscuss_Click);
			// 
			// cmdAdd
			// 
			this.cmdAdd.Location = new System.Drawing.Point(40, 232);
			this.cmdAdd.Size = new System.Drawing.Size(56, 24);
			this.cmdAdd.Text = "Add...";
			this.cmdAdd.Click += new EventHandler(cmdAdd_Click);
			// 
			// cmdRemove
			// 
			this.cmdRemove.Enabled = false;
			this.cmdRemove.Location = new System.Drawing.Point(144, 232);
			this.cmdRemove.Size = new System.Drawing.Size(56, 24);
			this.cmdRemove.Text = "Remove";
			this.cmdRemove.Click += new EventHandler(cmdRemove_Click);
			// 
			// civNegociation
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.cmdRemove);
			this.Controls.Add(this.cmdAdd);
			this.Controls.Add(this.cmdDiscuss);
			this.Controls.Add(this.cmdRefuse);
			this.Controls.Add(this.cmdPropose);
			this.Controls.Add(this.lb[ 1 ]);
			this.Controls.Add(this.lb[ 0 ]);
			this.Controls.Add(this.pictureBox1);
			this.Text = "Negociation";

		}
		#endregion

	#region draw top
		private void drawTop ()
		{
			Bitmap topBmp = new Bitmap( pictureBox1.Width, pictureBox1.Height );
			Graphics g = Graphics.FromImage( topBmp );
			SolidBrush blackBrush = new SolidBrush( Color.Black );

			Font pnFont = new Font( "Tahoma", 12, FontStyle.Regular ),
				midFont = new Font( "Tahoma", 9, FontStyle.Regular ),
				goldFont = new Font( "Tahoma", 9, FontStyle.Regular );

			g.Clear( Form1.defaultBackColor );

			g.FillRectangle(
				new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ players[ 0 ] ].civType ].color ),
				-1,
				space,
				space,
				lb[ 0 ].Top - 2 * space
				);
			g.DrawRectangle(
				new Pen( Color.Black ),
				-1,
				space,
				space,
				lb[ 0 ].Top - 2 * space
				);
			g.DrawString(
				Statistics.civilizations[ Form1.game.playerList[ players[ 0 ] ].civType ].name,
				pnFont,
				blackBrush,
				space,
				space
				);


			g.FillRectangle(
				new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ players[ 1 ] ].civType ].color ),
				this.Width - space,
				space,
				space,
				lb[ 0 ].Top - 2 * space
				);
			g.DrawRectangle(
				new Pen( Color.Black ),
				this.Width - space,
				space,
				space,
				lb[ 0 ].Top - 2 * space
				);
			SizeF spn = g.MeasureString(
				Statistics.civilizations[ Form1.game.playerList[ players[ 1 ] ].civType ].name,
				pnFont
				);
			g.DrawString(
				Statistics.civilizations[ Form1.game.playerList[ players[ 1 ] ].civType ].name,
				pnFont,
				blackBrush,
				this.Width - space - spn.Width,
				space
				);




		/*	g.FillRectangle(
				new SolidBrush( Color.White ),
				new Rectangle(
				0, 
				0, 
				topBmp.Width, 
				topBmp.Height
				)
				);

			SizeF aiNameSize = g.MeasureString(
				Form1.game.playerList[ other ].playerName,
				new Font( "Tahoma", 12, FontStyle.Regular )
				);
			SizeF aiCivSize = g.MeasureString(
				Statistics.civilizations[ Form1.game.playerList[ other ].civType ].name,
				new Font( "Tahoma", 10, FontStyle.Regular )
				);
			SizeF playerNameSize = g.MeasureString(
				Form1.game.playerList[ player ].playerName,
				new Font( "Tahoma", 12, FontStyle.Regular )
				);
			SizeF playerCivSize = g.MeasureString(
				Statistics.civilizations[ Form1.game.playerList[ player ].civType ].name,
				new Font( "Tahoma", 10, FontStyle.Regular )
				);

			g.FillPolygon( 
				new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ player ].civType ].color ),
				new Point[]
			{
				new Point( 0, 0 ),
				new Point( (int)playerNameSize.Width + 4, 0 ),
				new Point( (int)playerNameSize.Width + 4, (int)playerNameSize.Height + 2 ),
				new Point( (int)playerCivSize.Width + 4, (int)playerNameSize.Height + (int)playerCivSize.Height ),
				new Point( 0, (int)playerNameSize.Height + (int)playerCivSize.Height )
			}
				);

			g.FillPolygon( 
				new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ other ].civType ].color ),
				new Point[]
			{
				new Point( pictureBox1.Width, 0 ),
				new Point( pictureBox1.Width - ((int)aiNameSize.Width + 4), 0 ),
				new Point( pictureBox1.Width - ((int)aiNameSize.Width + 4), (int)aiNameSize.Height + 2 ),
				new Point( pictureBox1.Width - ((int)aiCivSize.Width + 4), (int)aiNameSize.Height + (int)aiCivSize.Height ),
				new Point( pictureBox1.Width, (int)aiNameSize.Height + (int)aiCivSize.Height )
			}
				);

			//player name
			g.DrawString( 
				Form1.game.playerList[ player ].playerName, 
				new Font( "Tahoma", 12, FontStyle.Regular ), 
				new SolidBrush( Color.Black ), 
				2, 
				0 
				);

			// ordi name
			g.DrawString( 
				Form1.game.playerList[ other ].playerName,
				new Font( "Tahoma", 12, FontStyle.Regular ),
				new SolidBrush( Color.Black ),
				topBmp.Width - aiNameSize.Width - 2,
				0
				);

			//player civ
			g.DrawString( 
				Statistics.civilizations[ Form1.game.playerList[ player ].civType ].name, 
				new Font( "Tahoma", 10, FontStyle.Regular ), 
				new SolidBrush( Color.Black ), 
				2, 
				aiNameSize.Height// + 6
				);

			// ordi civ
			g.DrawString( 
				Statistics.civilizations[ Form1.game.playerList[ other ].civType ].name,
				new Font( "Tahoma", 10, FontStyle.Regular ),
				new SolidBrush( Color.Black ),
				topBmp.Width - aiCivSize.Width - 2,
				aiNameSize.Height// + 6
				);

			//player money
			g.DrawString( 
				Form1.game.playerList[ player ].money.ToString() + " gold", 
				new Font( "Tahoma", 9, FontStyle.Regular ), 
				new SolidBrush( Color.Black ), 
				2, 
				1 + aiNameSize.Height + aiCivSize.Height// + 6
				);

			// ordi money
			SizeF aiMoneySize = g.MeasureString(
				Form1.game.playerList[ other ].money.ToString() + " gold",
				new Font( "Tahoma", 9, FontStyle.Regular )
				);
			g.DrawString( 
				Form1.game.playerList[ other ].money.ToString() + " gold",
				new Font( "Tahoma", 9, FontStyle.Regular ),
				new SolidBrush( Color.Black ),
				topBmp.Width - aiMoneySize.Width - 2,
				1 + aiNameSize.Height + aiCivSize.Height// + 6
				);*/

			#region center
			// relation quality
		/*	SizeF relationSize = g.MeasureString(
				"angry",
				new Font( "Tahoma", 10, FontStyle.Regular )
				);
			g.DrawString( 
				"angry",
				new Font( "Tahoma", 10, FontStyle.Regular ),
				new SolidBrush( Color.Black ),
				topBmp.Width / 2 - relationSize.Width / 2,
				topBmp.Height - relationSize.Height
				);*/

			// state = war, peace
			SizeF stateSize = g.MeasureString(
			Form1.relationPol[ Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic ],
				midFont
				);
			g.DrawString( 
			Form1.relationPol[ Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic ],
				midFont,
				new SolidBrush( Color.Black ),
				this.Width / 2 - stateSize.Width / 2,
				lb[ 0 ].Top - space - stateSize.Height
			//	topBmp.Height - relationSize.Height - stateSize.Height// - 6
				);
			#endregion

			pictureBox1.Image = topBmp;
		}
	#endregion

	/*	
		public enum infoType : byte
		{
			politicTreaty,
			economicTreaty,
			threat,
			breakAllianceWith,
			embargoOn,
			warOn,
			giveCity,
			giveRegion,
			giveTechno,
			giveContactWith,
			giveMoney,
			giveMoneyPerTurn,
			givePercBrut,
			giveMap,
			votes
		}	
		*/

		private void cmdRefuse_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmdDiscuss_Click(object sender, EventArgs e)
		{
			negoList tempList = aiNego.fillProp( list, 1 );

			if ( tempList != null )
				list = tempList;
			else
				MessageBox.Show( 
					String.Format( language.getAString( language.order.proposingRefusesAll ), Form1.game.playerList[ players[ 1 ] ].playerName ), 
					String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[  players[ 1 ] ].civName ) 
					);

			populateLBs();
		}

		private void cmdPropose_Click(object sender, EventArgs e)
		{
			long[] gain = list.getTotalGain();

			if ( gain[ 1 ] > 0 )
			{ // accepted
				MessageBox.Show( String.Format( language.getAString( language.order.proposingAccept ), Form1.game.playerList[ players[ 1 ] ].playerName ), String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[  players[ 1 ] ].civName ) );
				list.executeExchange();
				this.Close();
			}
			else
			{
				negoList tempList = aiNego.fillProp( list, 1 );
			//	userChoice ui;

				if ( tempList != null )
				{
					list = tempList;
					populateLBs();
					MessageBox.Show(
						String.Format( language.getAString( language.order.proposingRefusesPropose ), Form1.game.playerList[ players[ 1 ] ].playerName ), 
						String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[ players[ 1 ] ].civName ) 
						);
				}
				else
				{
					MessageBox.Show(
						String.Format( language.getAString( language.order.proposingRefusesAll ), Form1.game.playerList[ players[ 1 ] ].playerName ), 
						String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[ players[ 1 ] ].civName ) 
						);
				}
			}
		}

#region cmdAdd_Click
		private void cmdAdd_Click(object sender, EventArgs e)
		{
			popAddNegociation pan = new popAddNegociation( players, list );

			pan.ShowDialog();
			
			if ( pan.accepted )
			{
				if ( pan.result == -1 )
					list.addToGiveMoney( pan.resultPlayer, pan.resultInd );//list.giveMoney[ pan.resultPlayer ] += pan.resultInd;
				else if ( pan.result == -2 )
					list.addToMoneyPerTurn( pan.resultPlayer, pan.resultInd );//list.moneyPerTurn[ pan.resultPlayer ] += pan.resultInd;
				else
					list.add( pan.result );

				populateLBs();
				enableWHTB();
			}
		
		}

#endregion

#region cmdRemove_Click
		private void cmdRemove_Click(object sender, EventArgs e)
		{
			if ( lb[ curLB ].SelectedIndex != -1 )
			{
				if ( lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] == -1 )
				{
					list.giveMoney[ curLB ] = 0;
				}
				else if ( lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] == -2 )
				{
					list.moneyPerTurn[ curLB ] = 0;
				}
				else
				{
					list.remove( lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] );
				}
			/*	if ( list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].type == (byte)showInLB.bi )
				{
					usedInBilateral[ list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].info ] = false;
				}
				else if ( list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].type == (byte)showInLB.uni )
				{
					usedInUnilateral[ curLB ][ list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].info ] = false;
				}
				else if ( list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].type == (byte)showInLB.giveMoney )
				{
					moneyGiven[ curLB ] = 0;
				}
				else if ( list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].type == (byte)showInLB.moneyPerTurn )
				{
					moneyPerTurn[ curLB ] = 0;
				}*/

				populateLBs();
				enableWHTB();
			}
		}

#endregion

#region add
		/*private void add(int ind)
		{
			list.add( ind );
			/*int pos = 0;

			for ( int i = 0; i < bilateral.Length; i++ )
				if ( list.list[ i ].type == thing.type && list.list[ i ].info == thing.info )
				{
					usedInBilateral[ i ] = true;
					pos = i;
					break;
				}

			int p = playerPos;

			for ( int i = 0; i < unilateral[ p ].Length; i++ )
				if ( list.list[ i ].type == thing.type && list.list[ i ].info == thing.info )
				{
					usedInUnilateral[ p ][ i ] = true;
					break;
				}

			if ( thing.type == (byte)nego.infoType.giveRegion )
				for ( int i = 0; i < unilateral[ p ].Length; i++ )
					if ( 
					list.list[ i ].type == (byte)nego.infoType.giveCity && 
					Form1.game.playerList[ players[ p ] ].cityList[ list.list[ i ].info ].originalOwner == thing.info 
					)
						usedInUnilateral[ p ][ i ] = false;

			if ( thing.type == (byte)nego.infoType.politicTreaty || thing.type == (byte)nego.infoType.economicTreaty )
				for ( int i = 0; i < unilateral[ p ].Length; i++ )
					if ( 
					i != pos && 
					thing.type == list.list[ i ].type
					)
						usedInUnilateral[ p ][ i ] = false;*/
		//}*/

#endregion

	/*	private enum showInLB
		{
			bi,
			uni,
			giveMoney,
			moneyPerTurn
		}*/

#region populateLBs
		public void populateLBs()
		{

			for ( int p = 0; p < players.Length; p++ )
			{
				int posInLBC = 0;
				lb[ p ].Items.Clear();
				if ( list.giveMoney[ p ] > 0 )
				{
					lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveMoney ), list.giveMoney[ p ] ) );
					
					lbContent[ p ][ posInLBC ] = -1; //(byte)nego.infoType.giveMoney;
					posInLBC++;
				}

				if ( list.moneyPerTurn[ p ] > 0 )
				{
					lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveMoneyPerTurn ), list.moneyPerTurn[ p ], 20 ) );
					
					lbContent[ p ][ posInLBC ] = -2; //(byte)nego.infoType.giveMoneyPerTurn;
					posInLBC++;
				}

				for ( int i = 0; i < list.Length; i++ )
					if ( 
						list.list[ i ].accepted && 
						(
						list.list[ i ].player == 2 || 
						list.list[ i ].player == p
						)
						)
					{
						if ( lbContent[ p ].Length <= posInLBC )
						{
							int[] buffer = lbContent[ p ];

							lbContent[ p ] = new int[ buffer.Length + 50 ];

							for ( int j = 0; j < buffer.Length; j++ )
								lbContent[ p ][ j ] = buffer[ j ];
						}

						lbContent[ p ][ posInLBC ] = i;
					/*	list.list[ lbContent[ p ][ posInLBC ] ].type = (byte)showInLB.bi;
						list.list[ lbContent[ p ][ posInLBC ] ].info = i; //list.list[ i ];*/
						posInLBC++;

						if ( list.list[ i ].type == (byte)nego.infoType.breakAllianceWith )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveBreakAllianceWith ), Statistics.civilizations[ Form1.game.playerList[ list.list[ i ].info ].civType ].name ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.warOn )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveWarOn ), Statistics.civilizations[ Form1.game.playerList[ list.list[ i ].info ].civType ].name ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.embargoOn )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveEmbargoOn ), Statistics.civilizations[ Form1.game.playerList[ list.list[ i ].info ].civType ].name ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.politicTreaty )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGivePoliticTreaty ), Form1.relationPol[ list.list[ i ].info ] ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.economicTreaty )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveEconomicTreaty ), list.list[ i ].info ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.votes )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveVotes ), list.list[ i ].info ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.giveCity )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveCity ), Form1.game.playerList[ players[ p ] ].cityList[ list.list[ i ].info ].name ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.giveContactWith )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveContactWith ), Statistics.civilizations[ Form1.game.playerList[ list.list[ i ].info ].civType ].name ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.giveMap )
						{
							if ( list.list[ i ].info == 0 ) // world map
								lb[ p ].Items.Add( language.getAString( language.order.negoGiveWorldMap ) );
							else // territory map
								lb[ p ].Items.Add( language.getAString( language.order.negoGiveTerritoryMap ) );
						}
						else if ( list.list[ i ].type == (byte)nego.infoType.giveTechno )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveTechno ), Statistics.technologies[ list.list[ i ].info ].name ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.threat )
						{
							if ( list.list[ i ].info == 0 ) // war
								lb[ p ].Items.Add( language.getAString( language.order.negoGiveThreatWar ) );
							else if ( list.list[ i ].info == 1 ) // embargo
								lb[ p ].Items.Add( language.getAString( language.order.negoGiveThreatEmbargo ) );
							else // alliance
								lb[ p ].Items.Add( language.getAString( language.order.negoGiveThreatBreakAlliance ) );
						}
						else if ( list.list[ i ].type == (byte)nego.infoType.giveRegion )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveRegion ), Form1.game.playerList[ list.list[ i ].info ].civName ) );
						else if ( list.list[ i ].type == (byte)nego.infoType.giveResource )
							lb[ p ].Items.Add( String.Format( language.getAString( language.order.negoGiveResource ), Statistics.resources[ list.list[ i ].info ].name ) );
						else
							lb[ p ].Items.Add( String.Format( "Info missing! {0}:{1}", list.list[ i ].type, list.list[ i ].info ) );
					}
			}

			enableWHTB();
		}
			
#endregion

#region lbs_SelectedIndexChanged
		private void lbs_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			for ( int p = 0; p < players.Length; p++ )
				if ( lb[ p ].Focused )
				{
					curLB = (byte)p;
					lb[ ( p + 1 ) % 2 ].SelectedIndex = -1;
				}

			if ( lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] == -1 ||
				lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] == -2 ||
				!list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].cantBeRemoved )
				cmdRemove.Enabled = true;
			else
				cmdRemove.Enabled = false;

			enableWHTB();
		}
			
#endregion

		private void enableWHTB()
		{
			if ( lb[ curLB ].SelectedIndex >= 0 &&
				(
				lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] < 0 ||
				!list.list[ lbContent[ curLB ][ lb[ curLB ].SelectedIndex ] ].cantBeRemoved 
				)
				)
				cmdRemove.Enabled = true;
			else
				cmdRemove.Enabled = false;

			if ( lb[ 0 ].Items.Count > 0 || lb[ 1 ].Items.Count > 0 )
			{
				cmdPropose.Enabled = true;
				cmdDiscuss.Enabled = true;
			}
			else
			{
				cmdPropose.Enabled = false;
				cmdDiscuss.Enabled = false;
			}
		}

#region find
	/*	private structures.intByte find ( structures.intByte thing, int playerPos )
		{
			structures.intByte res = new structures.intByte();

			for ( int i = 0; i < bilateral.Length; i++ )
				if ( list.list[ i ].type == thing.type && list.list[ i ].info == thing.info )
				{
					res.type = 0;
					res.info = i;
					return res;
				}

			int p = playerPos;

			for ( int i = 0; i < unilateral[ p ].Length; i++ )
				if ( list.list[ i ].type == thing.type && list.list[ i ].info == thing.info )
				{
					res.type = 1;
					res.info = i;
					return res;
				}

			res.info = -1;
			return res;
		}*/
			
#endregion

		/*private void EWHTB()
		{
			if ( lb[ 0 ].Items.Count + lb[ 1 ].Items.Count > 0 )
			{
				cmdPropose.Enabled = true;
			}
		}*/
	}
}