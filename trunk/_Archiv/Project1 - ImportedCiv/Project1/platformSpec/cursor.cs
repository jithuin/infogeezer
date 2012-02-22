using System;
using System.Runtime.InteropServices;

namespace platformSpec
{
	/// <summary>
	/// Summary description for cursor.
	/// </summary>
	public class cursor
	{
#if CF
		[ DllImport( "coredll.dll" ) ]
		private static extern int LoadCursor(int zeroValue, int cursorID);
		[ DllImport( "coredll.dll" ) ]
		private static extern int SetCursor(int cursorHandle);
		static int wCursor = LoadCursor( 0, 32514 );
#endif

		public static bool showWaitCursor
		{
			set
			{
#if CF
				if ( value )
					SetCursor( wCursor );
				else
					SetCursor( 0 );
#endif
			}
		}
	}
}
