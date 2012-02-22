using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for ResolutionAwareness.
	/// </summary>
	public class ResolutionAwareness
	{
		public ResolutionAwareness()
		{
		}

		public static int mod
		{
			get
			{
				if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
					return 2;
				else
					return 1;
			}
		}
	}
}
