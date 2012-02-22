using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for ennemyPlans.
	/// </summary>
	public class ennemyPlans
	{		
		public static structure[] list;

		public struct structure
		{
			public byte player;
			public int unit;
			public Point[] way;
			public byte action;
		}

		public static void add( byte player, int unit )
		{
			/*deleteAt( X, Y );

			structure[] buffer = list;
			list = new structure[ buffer.Length + 1 ];

			for ( int i = 0; i < buffer.Length; i ++ )
				list[ i ] = buffer[ i ];

			list[ buffer.Length ].X = X;
			list[ buffer.Length ].Y = Y;
			list[ buffer.Length ].text = text;*/
		}

		public static void deleteAll()
		{
		}

		public static void delete( byte player, int unit )
		{
		/*	int old = findAt( X, Y );
			if ( old != -1 )
			{
				delete( old );
			}*/
		}

		public static void delete( int ind )
		{
		/*	structure[] buffer = list;
			list = new structure[ buffer.Length - 1 ];

			for ( int i = 0, j = 0; i < buffer.Length; i ++ )
				if ( i != ind )
				{
					list[ j ] = buffer[ i ];
					j ++;
				}*/
		}
	}
}
