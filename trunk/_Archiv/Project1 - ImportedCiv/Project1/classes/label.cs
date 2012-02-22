using System;

namespace xycv_ppc
{
	public class labelS
	{
		public labelS( string Text, int x, int y )
		{
			text = Text;
			X = x;
			Y = y;
		}

		public string text;
		public int X;
		public int Y;
	}
	/// <summary>
	/// Summary description for label.
	/// </summary>
	public class label
	{
		public static labelS[] list;

		/*public struct structure
		{
			public string text;
			public int X;
			public int Y;
		}*/

		public static void initList( int length )
		{

			try
			{
				list = new labelS[ length ];
			}
			catch ( Exception e )
			{
				System.Windows.Forms.MessageBox.Show( "Error init label list, Length = " + length.ToString(), e.Message );
			//	Application.Exit();
			}
		}

		public static void create( string text, int X, int Y )
		{
			deleteAt( X, Y );

			labelS[] buffer = list;
			list = new labelS[ buffer.Length + 1 ];

			for ( int i = 0; i < buffer.Length; i ++ )
				list[ i ] = buffer[ i ];

			list[ buffer.Length ] = new labelS( text, X, Y );/*.X = X;
			list[ buffer.Length ].Y = Y;
			list[ buffer.Length ].text = text;*/
		}

		public static void deleteAt( int X, int Y )
		{
			int old = findAt( X, Y );
			if ( old != -1 )
			{
				delete( old );
			}
		}

		public static void delete( int ind )
		{
			labelS[] buffer = list;
			list = new labelS[ buffer.Length - 1 ];

			for ( int i = 0, j = 0; i < buffer.Length; i ++ )
				if ( i != ind )
				{
					list[ j ] = buffer[ i ];
					j ++;
				}
		}

		public static int findAt( int X, int Y )
		{
			for ( int i = 0; i < list.Length; i ++ )
				if ( list[ i ].X == X && list[ i ].Y == Y )
					return i;

			return -1;
		}
	}
}
