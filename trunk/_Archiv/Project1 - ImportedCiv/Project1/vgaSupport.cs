using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for vgaSupport.
	/// </summary>
	public class vgaSupport
	{
	//	public static int width, height;
		public static void vga( System.Windows.Forms.Control.ControlCollection cs )
		{
		//	if ( width == 0 )
			if ( 
				System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 && 
				System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height > 320
				)
			{
				foreach( System.Windows.Forms.Control c in cs )
				{
					c.Width *= 2;
					c.Height *= 2;
					c.Left*= 2;
					c.Top *= 2;
				}
			}
		}
	}
}
