using System;
using System.Runtime.InteropServices;
using platformSpec;


namespace xycv_ppc
{
	/// <summary>
	/// Summary description for // waitCursor.
	/// </summary>
	public class wC
	{

/*		[DllImport("coredll.dll")]
			private static extern int LoadCursor(int zeroValue, int cursorID);
		[DllImport("coredll.dll")]
			private static extern int SetCursor(int cursorHandle);

		static int wCursor = LoadCursor(0, 32514);
*/
		public static bool show
		{
			set
			{
				platformSpec.cursor.showWaitCursor = value;
			/*	if ( value )
					SetCursor( wCursor ); 
				else
					SetCursor( 0 ); */
			}
		}

	}
}
