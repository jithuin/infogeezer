using System;
using System.Windows.Forms;

namespace platformSpec
{
	/// <summary>
	/// Summary description for windowsSize.
	/// </summary>
	public class manageWindows
	{
		public static void setMainSize(Form ctrl)
		{
		}

		public static void setDialogSize(Form ctrl)
		{
#if CF
#else
			ctrl.FormBorderStyle = FormBorderStyle.FixedSingle;
			ctrl.MaximizeBox = false;
#endif
		}

		public static void setUserInputSize(Form ctrl)
		{
#if CF
			ctrl.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 3 / 4; 
#else
			ctrl.MaximizeBox = false;
			ctrl.FormBorderStyle = FormBorderStyle.FixedSingle;
		//	ctrl.ClientSize = new Size( ctrl.ClientSize.Height, 200
			ctrl.Width = 200;
#endif
		}

		public static void prepareForDialog(Form ctrl)
		{
#if CF
			ctrl.Text = "";
#endif
		}
	}
}
