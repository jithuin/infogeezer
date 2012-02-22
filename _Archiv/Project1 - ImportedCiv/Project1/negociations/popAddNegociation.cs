using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using OpenNETCF.Win32;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for popAddNegociation.
	/// </summary>
	public class popAddNegociation : System.Windows.Forms.Form
	{
		TreeView tv;
		Button cmdOk, cmdCancel;
		public bool accepted;
		negoList list;

		public int result, resultInd;
		public byte resultPlayer;

		public popAddNegociation( byte[] Players, negoList List )
		{
			platformSpec.setFloatingWindow.before( this );
			list = List; 

			foreach ( byte p in list.players )
				if ( p != Form1.game.curPlayerInd )
				{
					this.Text = String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[ p ].civName );
					break;
				}

			int space = 8;
			this.ControlBox = false; 

			this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 9 / 10;
			this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * 4 / 5;

			cmdOk = new Button();
			cmdOk.Width = ( this.Width - space * 3 ) / 2;
			cmdOk.Left = space;
		//	cmdOk.Top = this.Height - space - cmdOk.Height;
			cmdOk.Click += new EventHandler(cmdOk_Click);
			cmdOk.Text = language.getAString( language.order.ok );
			cmdOk.Enabled = false;
#if !CF
			cmdOk.FlatStyle = FlatStyle.System;
#endif
			this.Controls.Add( cmdOk );

			cmdCancel = new Button();
			cmdCancel.Width = ( this.Width - space * 3 ) / 2;
			cmdCancel.Left = cmdOk.Right + space;
		//	cmdCancel.Top = this.Height - space - cmdOk.Height;
			cmdCancel.Click += new EventHandler( cmdCancel_Click );
			cmdCancel.Text = language.getAString( language.order.cancel );
#if !CF
			cmdCancel.FlatStyle = FlatStyle.System;
#endif
			this.Controls.Add( cmdCancel );

			tv = new TreeView();
			tv.Top = space;
			tv.Left = space;
			tv.Width = this.Width - space * 2;
		//	tv.Height = cmdOk.Top - space * 2;
			tv.AfterSelect += new TreeViewEventHandler( tv_AfterSelect );
			this.Controls.Add( tv );

			platformSpec.setFloatingWindow.after( this );

			#region floating
/*
			// Mémorise la zone cliente avant le changement de style
			Size aireAvant = this.ClientSize;

			// Récupèrer le handle de la fenêtre
			IntPtr hWnd = Win32Window.FromControl( this );

			// Liste des styles à enlever (voir article)
			int enleves = (int)( Win32Window.WindowStyle.WS_SYSMENU | Win32Window.WindowStyle.WS_MINIMIZEBOX | Win32Window.WindowStyle.WS_MAXIMIZEBOX );

			// Liste des styles à ajouter
			int ajoutes = (int)( Win32Window.WindowStyle.WS_CAPTION | Win32Window.WindowStyle.WS_POPUP );

			// Mise à jour du style de la fenêtre
			Win32Window.UpdateWindowStyle( hWnd, enleves, ajoutes );

			// Mesure l'aide cliente après le changement de style
			Size aireApres = this.ClientSize;

			// Restitue la taille de l'aire cliente
			aireApres.Width += aireAvant.Width - aireApres.Width;
			aireApres.Height += aireAvant.Height - aireApres.Height;
			this.ClientSize = aireApres;
			this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
			this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
			*/
			#endregion

			platformSpec.manageWindows.setDialogSize( this );

			this.Top += this.Height / 10;

			cmdOk.Top = this.ClientSize.Height - space - cmdOk.Height;
			cmdCancel.Top = this.ClientSize.Height - space - cmdOk.Height;
			tv.Height = cmdOk.Top - space * 2;

			platformSpec.resolution.set( this.Controls );

			list = List;
			populateTree();
		}

	/*	TreeNode nTreaties;
		TreeNode nDemand;
		TreeNode nGive;
		TreeNode nThreat;*/
		private void populateTree()
		{
		/*	bool stillAtWar = ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.war );

			if ( stillAtWar )
				for ( int i = 0; i < bilateral.Length; i++ )
					if ( 
						list.list[ i ].type == (byte)nego.infoType.politicTreaty && 
						userInBilateral[ i ]
						)
					{
						stillAtWar = true;
						break;
					}*/

			TreeNode[] tnMain = new TreeNode[ 4 ];
			tnMain[ 0 ] = new TreeNode( language.getAString( language.order.negoTreaty ) );
			tnMain[ 1 ] = new TreeNode( language.getAString( language.order.negoGive ) );
			tnMain[ 1 ].Tag = (int)0;
			tnMain[ 2 ] = new TreeNode( language.getAString( language.order.negoDemand ) );
			tnMain[ 2 ].Tag = (int)1;
			tnMain[ 3 ] = new TreeNode( language.getAString( language.order.negoThreat ) );

			// treaties
			TreeNode[] tnTreaties = new TreeNode[ 6 ];
			tnTreaties[ 0 ] = new TreeNode( language.getAString( language.order.negoTreatyPolitic ) );
			tnTreaties[ 1 ] = new TreeNode( language.getAString( language.order.negoTreatyEconomic ) );//language.order. ) );
			tnTreaties[ 2 ] = new TreeNode( language.getAString( language.order.negoTreatyBreakAlliance ) );
			tnTreaties[ 3 ] = new TreeNode( language.getAString( language.order.negoTreatyWarOn) );
			tnTreaties[ 4 ] = new TreeNode( language.getAString( language.order.negoTreatyEmbargoOn ) );
			tnTreaties[ 5 ] = new TreeNode( language.getAString( language.order.negoTreatyVote ) );
			
			TreeNode[][] tnGive = new TreeNode[ 2 ][];
			for ( int i = 0; i < tnGive.Length; i ++ )
			{
				tnGive[ i ] = new TreeNode[ 8 ];
			}

			for ( int p = 0, n = 1; p < 2; p++, n++ )
			{
				tnGive[ p ][ 0 ] = new TreeNode( language.getAString( language.order.negoGDCity ) );
				tnGive[ p ][ 1 ] = new TreeNode( language.getAString( language.order.negoGDContact ) );
				tnGive[ p ][ 2 ] = new TreeNode( language.getAString( language.order.negoGDMap ) );
				tnGive[ p ][ 3 ] = new TreeNode( language.getAString( language.order.negoGDTechno ) );
				tnGive[ p ][ 4 ] = new TreeNode( language.getAString( language.order.negoGDRegion ) );
				tnGive[ p ][ 5 ] = new TreeNode( language.getAString( language.order.negoGDMoney ) );
				tnGive[ p ][ 5 ].Tag = (int)-1;
				tnGive[ p ][ 6 ] = new TreeNode( language.getAString( language.order.negoGDMoneyPerTurn ) );
				tnGive[ p ][ 6 ].Tag = (int)-2;
				tnGive[ p ][ 7 ] = new TreeNode( language.getAString( language.order.negoGDResource ) );
			}

			for ( int i = 0; i < list.Length; i++ )
				if ( !list.list[ i ].accepted )
				{
					if ( list.list[ i ].type == (byte)nego.infoType.politicTreaty )
					{
						TreeNode newTN = new TreeNode( Form1.relationPol[ list.list[ i ].info ] );

						if ( list.conflicted( i ) )
							newTN.Text += "*";
						
						newTN.Tag = i;
						tnTreaties[ 0 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.economicTreaty )
					{
						TreeNode newTN = new TreeNode( "eco treaty" );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnTreaties[ 1 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.breakAllianceWith )
					{
						TreeNode newTN = new TreeNode( Form1.game.playerList[ list.list[ i ].info ].playerName );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnTreaties[ 2 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.warOn )
					{
						TreeNode newTN = new TreeNode( Form1.game.playerList[ list.list[ i ].info ].playerName );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnTreaties[ 3 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.embargoOn )
					{
						TreeNode newTN = new TreeNode( Form1.game.playerList[ list.list[ i ].info ].playerName );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnTreaties[ 4 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveCity )
					{
						TreeNode newTN = new TreeNode( Form1.game.playerList[ list.players[ list.list[ i ].player ] ].cityList[ list.list[ i ].info ].name );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnGive[ list.list[ i ].player ][ 0 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveContactWith )
					{
						TreeNode newTN = new TreeNode( Form1.game.playerList[ list.list[ i ].info ].civName );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnGive[ list.list[ i ].player ][ 1 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveMap )
					{
						TreeNode newTN;

						if ( list.list[ i ].info == 0 )
							newTN = new TreeNode( language.getAString( language.order.negoGiveWorldMap ) );
						else
							newTN = new TreeNode( language.getAString( language.order.negoGiveTerritoryMap ) );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnGive[ list.list[ i ].player ][ 2 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveTechno )
					{
						TreeNode newTN = new TreeNode( Statistics.technologies[ list.list[ i ].info ].name );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnGive[ list.list[ i ].player ][ 3 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveRegion )
					{
						TreeNode newTN = new TreeNode( String.Format( language.getAString( language.order.ancientRegionName ), Form1.game.playerList[ list.list[ i ].info ].civName ) );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnGive[ list.list[ i ].player ][ 4 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveResource )
					{
						TreeNode newTN = new TreeNode( Statistics.resources[ list.list[ i ].info ].name );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnGive[ list.list[ i ].player ][ 7 ].Nodes.Add( newTN );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.threat )
					{ // special
						TreeNode newTN = new TreeNode( "some threat" );

						if ( list.conflicted( i ) )
							newTN.Text += "*";

						newTN.Tag = i;
						tnMain[ 3 ].Nodes.Add( newTN );
					}
				}
			
			for ( int p = 0; p < 2; p ++ )
			{
				if ( list.canGiveMoney[ p ] - list.giveMoney[ p ] > 0 )
				{
					long max = list.canGiveMoney[ p ] - list.giveMoney[ p ];
					int[] customs;

					#region customs
					if ( max > 5000 )
					{
						customs = new int[ 4 ];
						customs[ 0 ] = 100;
						customs[ 1 ] = 500;
						customs[ 2 ] = 2000;
						customs[ 3 ] = 5000;
					}
					else if ( max > 1000 )
					{
						customs = new int[ 4 ];
						customs[ 0 ] = 100;
						customs[ 1 ] = 200;
						customs[ 2 ] = 500;
						customs[ 3 ] = 1000;
					}
					else if ( max > 100 )
					{
						customs = new int[ 3 ];
						customs[ 0 ] = 20;
						customs[ 1 ] = 50;
						customs[ 2 ] = 100;
					}
					else if ( max > 50 )
					{
						customs = new int[ 2 ];
						customs[ 0 ] = 20;
						customs[ 1 ] = 50;
					}
					else
					{
						customs = new int[ 0 ];
					}

					TreeNode[] newTN = new TreeNode[ customs.Length + 2 ];

					for ( int j = 0; j < customs.Length; j++ )
					{
						newTN[ j ] = new TreeNode( String.Format( language.getAString( language.order.gold ), customs[ j ] ) );
						newTN[ j ].Tag = customs[ j ];
					}
		
					#endregion

					newTN[ newTN.Length - 2 ] = new TreeNode( String.Format( language.getAString( language.order.gold ), max ) );// = new TreeNode( list.list[ i ].info.ToString() );
					newTN[ newTN.Length - 2 ].Tag = (int)max;
					newTN[ newTN.Length - 1 ] = new TreeNode( language.getAString( language.order.SpecifyTheAmount ) );// = new TreeNode( list.list[ i ].info.ToString() );
					newTN[ newTN.Length - 1 ].Tag = (int)( -1 * max );
					foreach ( TreeNode tn in newTN )
						tnGive[ p ][ 5 ].Nodes.Add( tn );
				}

				if ( list.canGivePerTurn[ p ] - list.moneyPerTurn[ p ] > 0 )
				{
					long max = list.canGivePerTurn[ p ] - list.moneyPerTurn[ p ];
					int[] customs;

					#region customs
					if ( max > 50 )
					{
						customs = new int[ 3 ];
						customs[ 0 ] = 15;
						customs[ 1 ] = 25;
						customs[ 2 ] = 50;
					}
					else if ( max > 25 )
					{
						customs = new int[ 3 ];
						customs[ 0 ] = 5;
						customs[ 1 ] = 10;
						customs[ 2 ] = 25;
					}
					else if ( max > 10 )
					{
						customs = new int[ 2 ];
						customs[ 0 ] = 5;
						customs[ 1 ] = 10;
					}
					else if ( max > 5 )
					{
						customs = new int[ 1 ];
						customs[ 0 ] = 5;
					}
					else
					{
						customs = new int[ 0 ];
					}

					TreeNode[] newTN = new TreeNode[ customs.Length + 2 ];

					for ( int j = 0; j < customs.Length; j++ )
					{
						newTN[ j ] = new TreeNode( String.Format( language.getAString( language.order.goldPerTurn ), customs[ j ] ) );
						newTN[ j ].Tag = customs[ j ];
					}
		
					#endregion

					newTN[ newTN.Length - 2 ] = new TreeNode( String.Format( language.getAString( language.order.goldPerTurn ), max ) );
					newTN[ newTN.Length - 2 ].Tag = (int)max;
					newTN[ newTN.Length - 1 ] = new TreeNode( language.getAString( language.order.SpecifyTheAmount ) );
					newTN[ newTN.Length - 1 ].Tag = (int)( -1 * max );
					foreach ( TreeNode tn in newTN )
						tnGive[ p ][ 6 ].Nodes.Add( tn );
				}
			}

			foreach ( TreeNode tn in tnTreaties )
				if ( tn.Nodes.Count > 0 )
					tnMain[ 0 ].Nodes.Add( tn );

			for ( int p = 0, n = 1; p < 2; p++, n++ )
				foreach ( TreeNode tn in tnGive[ p ] )
					if ( tn.Nodes.Count > 0 )
						tnMain[ n ].Nodes.Add( tn );

			foreach ( TreeNode tn in tnMain )
				if ( tn.Nodes.Count > 0 )
					tv.Nodes.Add( tn );
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			accepted = true;
			result = (int)tv.SelectedNode.Tag;
		//	int parent = (int)tv.SelectedNode.Parent.Tag;

			if ( tv.SelectedNode.Parent.Tag != null )
			{
				int parent = (int)tv.SelectedNode.Parent.Tag;
				if ( result < 0 )
				{
					long max = result * -1;
					string text;

					if ( parent == -1 )
						text = string.Format( language.getAString( language.order.uiSpecifyTheAmountOnce ), max );
					else
						text = string.Format( language.getAString( language.order.uiSpecifyTheAmountPerTurn ), max );

					userNumberInput ui = new userNumberInput( language.getAString( language.order.SpecifyTheAmount ), text, 0, max, language.getAString( language.order.ok ), language.getAString( language.order.cancel ) );

					ui.ShowDialog();
					if ( ui.result < 0 )
					{ // cancel
						resultInd = 0;
						accepted = false;
					}
					else
					{
						resultInd = ui.result;
					}
				}
				else
				{
					resultInd = (int)tv.SelectedNode.Tag;
				}

				result = parent;

				resultPlayer = (byte)(int)tv.SelectedNode.Parent.Parent.Tag;
			}

			if ( accepted )
				this.Close();
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			accepted = false;
			this.Close();
		}
		private void tv_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if ( tv.SelectedNode.Nodes.Count > 0 )
				cmdOk.Enabled = false;
			else
				cmdOk.Enabled = true;
		}
	}
}
