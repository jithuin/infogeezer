using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using OpenNETCF.Win32;
//using Floating;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for popContactPlayer.
	/// </summary>
	public class popContactPlayer : System.Windows.Forms.Form // Floating.popup // 
	{
	
		byte player, other;
		public popContactPlayer( byte player, byte other)
		{

			/*	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;*/
			platformSpec.setFloatingWindow.before( this );
			platformSpec.manageWindows.setUserInputSize( this );
			this.ControlBox = false;

	#region heart
			this.Text = "Contact";
			this.player = player;
			this.other = other;

			int ySpaces = 8, butHeight = 24, butWidth = 100;
			byte butPos = 0;
			this.Width = butWidth + 2 * ySpaces;
			Button cmdNegotiate = new Button();

			cmdNegotiate.Text = "Negotiate";
			cmdNegotiate.Width = butWidth;
			cmdNegotiate.Height = butHeight;
			cmdNegotiate.Left = this.Width / 2 - cmdNegotiate.Width / 2;
			cmdNegotiate.Top = butPos * ( butHeight + ySpaces ) + ySpaces; //this.Height * butPos / 5 - cmdNegotiate.Height / 2 + 11;
			cmdNegotiate.Click += new EventHandler( cmdNegotiate_Click );
#if !CF
			cmdNegotiate.FlatStyle = FlatStyle.System;
#endif
			butPos++;

			if ( Form1.game.playerList[ player ].foreignRelation[ other ].politic != (byte)Form1.relationPolType.war )
			{
				Button cmdDeclareWar = new Button();

				cmdDeclareWar.Text = "Declare war!";
				cmdDeclareWar.Width = butWidth;
				cmdDeclareWar.Height = butHeight;
				cmdDeclareWar.Left = this.Width / 2 - cmdDeclareWar.Width / 2;
				cmdDeclareWar.Top = butPos * ( butHeight + ySpaces ) + ySpaces; //this.Height * butPos / 5 - cmdDeclareWar.Height / 2 + 11;
				cmdDeclareWar.Click += new EventHandler( cmdDeclareWar_Click );
#if !CF
			cmdDeclareWar.FlatStyle = FlatStyle.System;
#endif
				butPos++;
				this.Controls.Add( cmdDeclareWar );
			}

			Button cmdCancel = new Button();

			cmdCancel.Text = "Cancel";
			cmdCancel.Width = butWidth;
			cmdCancel.Height = butHeight;
			cmdCancel.Left = this.Width / 2 - cmdCancel.Width / 2;
			cmdCancel.Top = butPos * ( butHeight + ySpaces ) + ySpaces; //this.Height * 4 / 5 - cmdCancel.Height / 2 + 11;
			cmdCancel.Click += new EventHandler( cmdCancel_Click );
#if !CF
			cmdCancel.FlatStyle = FlatStyle.System;
#endif
			this.Controls.Add( cmdNegotiate );
			this.Controls.Add( cmdCancel );
			cmdNegotiate.BringToFront();
			cmdCancel.BringToFront(); 

		/*	Size cs = new Size( (int)(butWidth + 2 * ySpaces), (int)(cmdCancel.Bottom + ySpaces) );
			this.ClientSize = cs;*/
		/*	int tempW = (int)(butWidth + 2 * ySpaces);
			int tempH = (int)();  = tempW;
			this.Width = tempH;*/
			this.Height = cmdCancel.Bottom + ySpaces;
	#endregion

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

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		private void cmdNegotiate_Click(object sender, EventArgs e)
		{

			frmNegociation civNegFrm = new frmNegociation( player, other, -1 );

			if ( aiNego.acceptToInitNego( civNegFrm.list, 0 ) )
			{
				string titre = this.Text;
				platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";
				civNegFrm.ShowDialog();
				this.Text = titre;
				this.Close();
			}
			else
				System.Windows.Forms.MessageBox.Show( 
					String.Format( language.getAString( language.order.tradeRefusesTo ), Form1.game.playerList[ other ].playerName ), 
					String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[ other ].civName )
					);
		}
		private void cmdCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		private void cmdDeclareWar_Click(object sender, EventArgs e)
		{
			aiPolitics.declareWar( player, other );
			this.Close();
		}
	}
}
