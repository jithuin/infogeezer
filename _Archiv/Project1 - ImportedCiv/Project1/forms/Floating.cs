using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
#if CF
using OpenNETCF.Win32;
#endif

namespace Floating
{
	/// <summary>
	/// Summary description for floating.
	/// </summary>
	public class popup : System.Windows.Forms.Form
	{
		public popup()
		{
			//
			// Required for Windows Form Designer support
			//
			//	InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.ControlBox = false;

#if CF
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
#endif

		/*	this.Closing += new CancelEventHandler( floating_Closing );
			this.Closed += new EventHandler( floating_Closed );
			this.Click += new EventHandler(floating_Click);*/
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
		/*private void InitializeComponent()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

			// 
			// floating
			// 
		/*	this.ClientSize = new System.Drawing.Size( 100, 100 );
			this.Location = new System.Drawing.Point( 100, 100 );*/
			/*this.Location = new System.Drawing.Point( 
			System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2, 
			System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2 
			);
		//	this.Text = "floating";
		}*/
#endregion
	}
}
