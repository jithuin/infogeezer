using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for plans.
	/// </summary>
	public class Plans
	{
		Game game;
		public bool[,] grid;
		public structure[] list;

		public Plans( Game game )
		{
			this.game = game;
		}

		public struct structure
		{
			public byte player;
			public int unit;
			public Point[] way;
		}

		public void init()
		{
			grid = new bool[ game.width, game.height ];
			list = new structure[ 0 ];
		}

		public void add( byte player, int unit, Point[] way )
		{
			structure[] buffer = list;
			list = new structure[ buffer.Length + 1 ];

			for ( int i = 0; i < buffer.Length; i ++ )
				list[ i ] = buffer[ i ];

			list[ buffer.Length ].player = player;
			list[ buffer.Length ].unit = unit;
			list[ buffer.Length ].way = way;

			for ( int i = 0; i < way.Length; i ++ )
				grid[ way[ i ].X, way[ i ].Y ] = true;
		}

		public void remove( byte player, int unit )
		{
		}

		public void drawAllOnScreen( Graphics g, int sliHor, int sliVer ) 
		{ 

			//game.radius.re
		} 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="ori"> x, y</param>
		/// <param name="r"> the case itself</param>
		public void drawCase( Graphics g, Point ori, Rectangle r ) 
		{ 
			int diff = 3;

			if ( grid[ ori.X, ori.Y ] )
				for ( int i = 0; i < list.Length; i ++ ) 
					for ( int j = 0; j < list[ i ].way.Length; j ++ ) 
						if ( list[ i ].way[ j ].X == ori.X && list[ i ].way[ j ].Y == ori.Y ) 
						{ 
							int tot = 0;

							Point tl = game.radius.returnUpLeft( list[ i ].way[ j ] );
							if ( 
								tl.X != -1 &&
								(
								(
								j > 0 &&
								list[ i ].way[ j - 1 ] == tl
								) ||
								(
								j < list[ i ].way.Length - 1 &&
								list[ i ].way[ j + 1 ] == tl
								)
								)
								) 
							{ 
								Point[] r1 = new Point[ 4 ];
								r1[ 0 ] = new Point( r.Left,							r.Top - diff ); // top
								r1[ 1 ] = new Point( r.Left - diff,						r.Top ); // left
								r1[ 2 ] = new Point( r.Left + Form1.caseWidth / 2,		r.Top + Form1.caseHeight / 2 + diff ); // down
								r1[ 3 ] = new Point( r.Left + Form1.caseWidth / 2 + diff,	r.Top + Form1.caseHeight / 2 ); // right
								tot ++;

								g.FillPolygon( 
									new SolidBrush( Statistics.civilizations[ game.playerList[ list[ i ].player ].civType ].color ), 
									r1 
									);

								if ( tot == 2 )
									break;
							} 

							Point t = game.radius.returnUp( list[ i ].way[ j ] ); 
							if ( 
								t.X != -1 &&
								(
								(
								j > 0 &&
								list[ i ].way[ j - 1 ] == t
								) ||
								(
								j < list[ i ].way.Length - 1 &&
								list[ i ].way[ j + 1 ] == t
								)
								) 
								)
							{ 
								Point[] r1 = new Point[ 4 ];
								r1[ 0 ] = new Point( r.Left + Form1.caseWidth / 2 - diff,		r.Top - Form1.caseHeight / 2 - diff ); // tl
								r1[ 1 ] = new Point( r.Left + Form1.caseWidth / 2 + diff,		r.Top - Form1.caseHeight / 2 - diff ); // tr
								r1[ 2 ] = new Point( r.Left + Form1.caseWidth / 2 + diff,		r.Top + Form1.caseHeight / 2 + diff ); // dr
								r1[ 3 ] = new Point( r.Left + Form1.caseWidth / 2 - diff,		r.Top + Form1.caseHeight / 2 + diff ); // dl
								tot ++;

								g.FillPolygon( 
									new SolidBrush( Statistics.civilizations[ game.playerList[ list[ i ].player ].civType ].color ), 
									r1 
									); 

								if ( tot == 2 )
									break;
							} 

							Point tr = game.radius.returnUpRight( list[ i ].way[ j ] ); 
							if ( 
								tr.X != -1 &&
								(
								(
								j > 0 &&
								list[ i ].way[ j - 1 ] == tr
								) ||
								(
								j < list[ i ].way.Length - 1 &&
								list[ i ].way[ j + 1 ] == tr
								)
								) 
								)
							{ 
								Point[] r1 = new Point[ 4 ];
								r1[ 0 ] = new Point( r.Left + Form1.caseWidth,					r.Top - diff ); // top
								r1[ 1 ] = new Point( r.Left + Form1.caseWidth / 2 - diff,		r.Top + Form1.caseHeight / 2 ); // left
								r1[ 2 ] = new Point( r.Left + Form1.caseWidth / 2,				r.Top + Form1.caseHeight / 2 + diff ); // down
								r1[ 3 ] = new Point( r.Left + Form1.caseWidth + diff,			r.Top ); // right
								tot ++;

								g.FillPolygon( 
									new SolidBrush( Statistics.civilizations[ game.playerList[ list[ i ].player ].civType ].color ), 
									r1 
									);

								if ( tot == 2 )
									break;
							} 

							Point r0 = game.radius.returnRight( list[ i ].way[ j ] ); 
							if ( 
								r0.X != -1 &&
								(
								(
								j > 0 &&
								list[ i ].way[ j - 1 ] == r0
								) ||
								(
								j < list[ i ].way.Length - 1 &&
								list[ i ].way[ j + 1 ] == r0
								)
								) 
								)
							{ 
								Point[] r1 = new Point[ 4 ];
								r1[ 0 ] = new Point(	r.Left + Form1.caseWidth / 2 - diff,						r.Top + Form1.caseHeight / 2 + diff	); // tl
								r1[ 1 ] = new Point(	r.Left + Form1.caseWidth / 2 + Form1.caseWidth + diff,		r.Top + Form1.caseHeight / 2 + diff	); // tr
								r1[ 2 ] = new Point(	r.Left + Form1.caseWidth / 2 + Form1.caseWidth + diff,		r.Top + Form1.caseHeight / 2 - diff	); // dr
								r1[ 3 ] = new Point(	r.Left + Form1.caseWidth / 2 - diff,						r.Top + Form1.caseHeight / 2 - diff	); // dl

								g.FillPolygon( 
									new SolidBrush( Statistics.civilizations[ game.playerList[ list[ i ].player ].civType ].color ), 
									r1 
									);
							} 

							break;
						} 
		} 
	}
}
