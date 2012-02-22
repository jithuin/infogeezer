using System;
#if CF
using OpenNETCF.Win32;
#endif

namespace platformSpec
{
	/// <summary>
	/// Summary description for setFloatingWindow.
	/// </summary>
	public class setFloatingWindow
	{
		public static void before(System.Windows.Forms.Form ctrl)
		{
#if CF
			ctrl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
#else
			manageWindows.setDialogSize( ctrl );
#endif
		}

		public static void after(System.Windows.Forms.Form ctrl)
		{
#if CF

			ctrl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; //.None;
			// Mémorise la zone cliente avant le changement de style
			System.Drawing.Size aireBefore = ctrl.ClientSize;

			// Récupèrer le handle de la fenêtre
			IntPtr hWnd = Win32Window.FromControl( ctrl );

			int toRemove = (int)( WS.SYSMENU | WS.MINIMIZEBOX | WS.MAXIMIZEBOX ),
				toAdd = (int)( WS.CAPTION | WS.POPUP );

			Win32Window.UpdateWindowStyle( hWnd, toRemove, toAdd );
	/*		System.Drawing.Size aireAfter = ctrl.ClientSize;
			
			aireAfter.Width += aireBefore.Width - aireAfter.Width;
			aireAfter.Height += aireBefore.Height - aireAfter.Height;		*/
	//		ctrl.ClientSize = aireAfter;

	/*		// Mémorise la zone cliente avant le changement de style
			System.Drawing.Size aireAvant = ctrl.ClientSize;

			// Récupèrer le handle de la fenêtre
			IntPtr hWnd = Win32Window.FromControl( ctrl );

			// Liste des styles à enlever (voir article)
			int enleves = (int)( Win32Window.WindowStyle.WS_SYSMENU | Win32Window.WindowStyle.WS_MINIMIZEBOX | Win32Window.WindowStyle.WS_MAXIMIZEBOX );

			// Liste des styles à ajouter
			int ajoutes = (int)( Win32Window.WindowStyle.WS_CAPTION | Win32Window.WindowStyle.WS_POPUP );

			// Mise à jour du style de la fenêtre
			Win32Window.UpdateWindowStyle( hWnd, enleves, ajoutes );

			// Mesure l'aide cliente après le changement de style
			System.Drawing.Size aireApres = ctrl.ClientSize;

			// Restitue la taille de l'aire cliente
			aireApres.Width += aireAvant.Width - aireApres.Width;
			aireApres.Height += aireAvant.Height - aireApres.Height;
			ctrl.ClientSize = aireApres;
			ctrl.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - ctrl.Width / 2;
			ctrl.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - ctrl.Height / 2;*/
#endif
		}
	}
}
