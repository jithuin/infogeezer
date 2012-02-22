using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for plans.
	/// </summary>
	public class roads
	{
		/// <summary></summary> 
		/// <param name="ori">x, y</param> 
		/// <param name="r">std r</param> 
		public static void drawCase( Graphics g, Point ori, Rectangle r ) 
		{
			int seen;
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ ori.X, ori.Y ] )
				seen = 0;
			else
				seen = 1;

			int tot = 0;

			for ( int i = 0; i < 8; i++ )
				if ( !Form1.game.grid[ ori.X, ori.Y ].roadDir[ i ].rail && Form1.game.grid[ ori.X, ori.Y ].roadDir[ i ].road )
				{
					g.DrawImage( Form1.roadBmp[ i ][ seen ], r, 0, 0, Form1.roadBmp[ i ][ seen ].Width, Form1.roadBmp[ i ][ seen ].Height, GraphicsUnit.Pixel, Form1.ia );
					tot++;
				}

			for ( int i = 0; i < 8; i++ )
				if ( Form1.game.grid[ ori.X, ori.Y ].roadDir[ i ].rail )
				{
					g.DrawImage( Form1.railBmp[ i ][ seen ], r, 0, 0, Form1.railBmp[ i ][ seen ].Width, Form1.railBmp[ i ][ seen ].Height, GraphicsUnit.Pixel, Form1.ia );
					tot++;
				}

			if ( tot == 0 )
			{
				if ( Form1.game.grid[ ori.X, ori.Y ].roadLevel == 2 ) 
					g.DrawImage( 
						Form1.railBmp[ 8 ][ seen ], 
						r, 
						0, 
						0, 
						Form1.railBmp[ 8 ][ seen ].Width, 
						Form1.railBmp[ 8 ][ seen ].Height, 
						GraphicsUnit.Pixel, 
						Form1.ia
						);
				else if ( Form1.game.grid[ ori.X, ori.Y ].roadLevel == 1 ) 
					g.DrawImage( 
						Form1.roadBmp[ 8 ][ seen ], 
						r, 
						0, 
						0, 
						Form1.roadBmp[ 8 ][ seen ].Width, 
						Form1.roadBmp[ 8 ][ seen ].Height, 
						GraphicsUnit.Pixel, 
						Form1.ia
						);
			}
		} 

		public static void setAll( Game game )
		{
			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y++ )
				{
					setOne( game, new Point( x, y ));
				/*	Form1.game.grid[ x, y ].roadDir = new structures.roadList();

					Point[] sqr = game.radius.returnSmallSqrInOrder( new Point( x, y ) );
					for ( int i = 0; i < sqr.Length; i ++ )
						if ( sqr[ i ].X >= 0 )
						{
							if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 2 )
								Form1.game.grid[ x, y ].roadDir[ i ].rail = true;
							else if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 1 )
								Form1.game.grid[ x, y ].roadDir[ i ].road = true;
						}*/
				}
		}

		public static void setAround( Game game, Point ori )
		{
		//	int x = ori.X, y = ori.Y;

			setOne( game, ori );

			Point[] sqr = game.radius.returnEmptySquare( ori, 1 );

			for ( int i = 0; i < sqr.Length; i++ )
				if ( sqr[ i ].X >= 0 )
					setOne( game, sqr[ i ] );

		/*	Form1.game.grid[ x, y ].roadDir = new structures.roadList();

			Point[] sqr = game.radius.returnSmallSqrInOrder( new Point( x, y ) );

			for ( int i = 0; i < sqr.Length; i++ )
				if ( sqr[ i ].X >= 0 )
				{
					if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 2 )
						Form1.game.grid[ x, y ].roadDir[ i ].rail = true;
					else if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 1 )
						Form1.game.grid[ x, y ].roadDir[ i ].road = true;
				}*/
		}
		public static void setOne( Game game, Point ori )
		{
			int x = ori.X, y = ori.Y;

			game.grid[ x, y ].roadDir = new structures.roadList[ 8 ];

			Point[] sqr = game.radius.returnSmallSqrInOrder( new Point( x, y ) );

			for ( int i = 0; i < sqr.Length; i++ )
				if ( sqr[ i ].X >= 0 )
				{
					if ( 
						game.grid[ x, y ].roadLevel == 2 && 
						game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel == 2 
						)
						game.grid[ x, y ].roadDir[ i ].rail = true;
					else if ( game.grid[ sqr[ i ].X, sqr[ i ].Y ].roadLevel >= 1 )
						game.grid[ x, y ].roadDir[ i ].road = true;
				}
		}
	}
}
