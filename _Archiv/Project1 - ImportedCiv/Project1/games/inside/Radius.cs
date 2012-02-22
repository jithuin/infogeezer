using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for radius.
	/// </summary>
	public class Radius
	{
		Game game;

		public Radius( Game game )
		{
			this.game = game;
		}

#region return city radius
		public  Point[] returnCityRadius( int x1, int y1 )
		{
			Point[] retour = new Point[ 20 ];

			if ( y1 % 2 == 1)
			{
				int j = 0;
				int x = x1 - 1;
				for ( int y = y1 - 2; y <= y1 + 2; y ++ )
				{
					retour[ j ] = new Point( x, y );
					j++;
				}

				x = x1;
				for ( int y = y1 - 3; y <= y1 + 3; y ++ )
				{
					if ( y != y1 )
					{
						retour[ j ] = new Point( x, y );
						j++;
					}
				}

				x = x1 + 1;
				for ( int y = y1 - 3; y <= y1 + 3; y ++ )
				{
					retour[ j ] = new Point( x, y );
					j++;
				}

				x = x1 + 2;
				for ( int y = y1 - 1; y <= y1 + 1; y += 2 )
				{
					retour[ j ] = new Point( x, y );
					j++;
				}
			}
			else if ( y1 % 2 == 0)
			{
				int j = 0;
				int x = x1 - 2;
				for ( int y = y1 - 1; y <= y1 + 1; y += 2 )
				{
					retour[ j ] = new Point( x, y );
					j++;
				}

				x = x1 - 1;
				for ( int y = y1 - 3; y <= y1 + 3; y ++ )
				{
					retour[ j ] = new Point( x, y );
					j++;
				}

				x = x1;
				for ( int y = y1 - 3; y <= y1 + 3; y ++ )
				{
					if ( y != y1 )
					{
						retour[ j ] = new Point( x, y );
						j++;
					}
				}

				x = x1 + 1;
				for ( int y = y1 - 2; y <= y1 + 2; y ++ )
				{
					retour[ j ] = new Point( x, y );
					j++;
				}
			}
			
			// vérification des limites
			for ( int j = 0; j < retour.Length; j++)
			{
				if ( retour[ j ].X < 0)
					retour[ j ].X += game.width;
				else if ( retour[ j ].X >= game.width)
					retour[ j ].X -= game.width;

				if ( retour[ j ].Y >= game.height)
				{
					retour[ j ].Y = - 1;
					retour[ j ].X = - 1;
				}
				else if ( retour[ j ].Y < 0)
				{
					retour[ j ].Y = - 1;	
					retour[ j ].X = - 1;
				}
			}
			
			Point[] retour1 = verifyPointFound( retour );

			return retour1;

		}
		#endregion

#region return 2 down cases + dir
		public Point[] return2DownCases( int x1, int y1 )
		{
			Point[] retour = new Point[ 2 ];

			if ( y1 % 2 == 1)
			{
				retour[ 0 ] = new Point( x1, y1 + 1 );
				retour[ 1 ] = new Point( x1 + 1, y1 + 1 );
			}
			else if ( y1 % 2 == 0)
			{
				retour[ 0 ] = new Point( x1 - 1, y1 + 1 );
				retour[ 1 ] = new Point( x1, y1 + 1 );
			}

			return verifyLimits( retour );
		}

	#region sides
		public  Point returnDownLeft( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			if ( y1 % 2 == 1)
				retour = new Point( x1, y1 + 1 );
			else// if ( y1 % 2 == 0)
				retour = new Point( x1 - 1, y1 + 1 );

			return verifyPoint( retour );
		}

		public  Point returnDownRight( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			if ( y1 % 2 == 1)
				retour = new Point( x1 + 1, y1 + 1 );
			else //if ( y1 % 2 == 0)
				retour = new Point( x1, y1 + 1 );

			return verifyPoint( retour );
		}

		public  Point returnUpRight( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			if ( y1 % 2 == 1)
				retour = new Point( x1 + 1, y1 - 1 );
			else //if ( y1 % 2 == 0)
				retour = new Point( x1, y1 - 1 );

			return verifyPoint( retour );
		}

		public  Point returnUpLeft( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			if ( y1 % 2 == 1)
				retour = new Point( x1, y1 - 1 );
			else 
				retour = new Point( x1 - 1, y1 - 1 );

			return verifyPoint( retour );
		}
		#endregion

	#region corners
		public  Point returnUp( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			retour = new Point( x1, y1 - 2 );

			return verifyPoint( retour );
		}

		public  Point returnLeft( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			retour = new Point( x1 - 1, y1 );

			return verifyPoint( retour );
		}

		public  Point returnRight( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			retour = new Point( x1 + 1, y1 );

			return verifyPoint( retour );
		}

		public  Point returnDown( Point pos )
		{
			int x1 = pos.X, y1 = pos.Y;
			Point retour;

			retour = new Point( x1, y1 + 2 );

			return verifyPoint( retour );
		}
		#endregion

#endregion

		/// <summary>
		/// Values are decaled, use pos - 1
		/// </summary>
		/// <param name="ori"></param>
		/// <returns></returns>
		public Point[] returnSmallSqrInOrder( Point ori )
		{
			Point[] sqr = new Point[ 8 ];

			sqr[ 0 ] = returnUpLeft( ori );
			sqr[ 1 ] = returnUp( ori );
			sqr[ 2 ] = returnUpRight( ori );
			sqr[ 3 ] = returnRight( ori );
			sqr[ 4 ] = returnDownRight( ori );
			sqr[ 5 ] = returnDown( ori );
			sqr[ 6 ] = returnDownLeft( ori );
			sqr[ 7 ] = returnLeft( ori );

			return sqr;

			/*Point TL = returnUpLeft( ori );
			Point T = returnUp( ori );
			Point TR = returnUpRight( ori );
			Point R = returnRight( ori );
			Point DR = returnDownRight( ori );
			Point D = returnDown( ori );
			Point DL = returnDownLeft( ori );
			Point L = returnLeft( ori );*/
		}

#region verify point
		private  Point verifyPoint( Point retour )
		{
			if ( retour.X < 0)
				retour.X += game.width;
			else if ( retour.X >= game.width)
				retour.X -= game.width;

			if ( retour.Y >= game.height)
			{
				retour.Y = - 1;
				retour.X = - 1;
			}
			else if ( retour.Y < 0)
			{
				retour.Y = - 1;	
				retour.X = - 1;
			}

			return retour;
		}
	#endregion

#region return square
		public  Point[] returnSquare( int xo, int yo, int rayon )
		{
			Point[] retour = new Point[ ( 2*rayon + 1) * ( 2*rayon + 1) - 1 ];
			int pntCounter = 0;

			for ( int radius = 1; radius <= rayon; radius ++ )
			{
				if ( yo%2 == 1) // impair
				{
					retour[ pntCounter] = new Point( xo - radius , yo );
					pntCounter ++;

					int i = 0;
					for ( int x = xo - radius + 1; x <= xo; x++ )
					{
						retour[pntCounter] = new Point( x, yo - i*2 - 2 );
						pntCounter++;
						retour[pntCounter] = new Point( x, yo - i*2 - 1 );
						pntCounter++;
						retour[pntCounter] = new Point( x, yo + i*2 + 2 );
						pntCounter++;
						retour[pntCounter] = new Point( x, yo + i*2 + 1 );
						pntCounter++;
				//		pntCounter += 4;
						i++;
					}
				
					i = radius;
					for ( int x = xo + 1; x <= xo + radius - 1; x++ )
					{ // bo
						retour[ pntCounter] = new Point( x, yo - i*2 + 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 + 2 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 2 );
						pntCounter ++;
				//		pntCounter += 4;
						i--;
					}

					retour[ pntCounter] = new Point( xo + radius, yo );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo + radius, yo-1 );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo + radius, yo+1 );
					pntCounter ++;
			//		pntCounter += 3;
				}
				else // pair
				{
					retour[ pntCounter] = new Point( xo + radius , yo );
					pntCounter ++;

					int i = 0;
					for ( int x = xo + radius - 1; x >= xo; x-- )
					{
						retour[ pntCounter] = new Point( x, yo - i*2 - 2);
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 + 2);
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 + 1 );
						pntCounter ++;
				//		pntCounter += 4;
						i++;
					}
				
					i = radius;
					for ( int x = xo - 1; x >= xo - radius + 1; x-- )
					{
						retour[ pntCounter] = new Point( x, yo - i*2 + 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 + 2 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 2 );
						pntCounter ++;
				//		pntCounter += 4;
						i--;
					}

					retour[ pntCounter] = new Point( xo - radius, yo );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo - radius, yo-1 );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo - radius, yo+1 );
					pntCounter ++;
			//		pntCounter += 3;

				}
			}

			// vérification des limites
			for ( int j = 0; j < retour.Length; j++) // tester et enlever le =
			{
				if ( retour[ j ].X < 0)
					retour[ j ].X += game.width;
				else if ( retour[ j ].X >= game.width)
					retour[ j ].X -= game.width;

				if ( retour[ j ].Y >= game.height)
				{
					retour[ j ].Y = - 1;
					retour[ j ].X = - 1;
				}
				else if ( retour[ j ].Y < 0)
				{
					retour[ j ].Y = - 1;	
					retour[ j ].X = - 1;
				}
			}

			Point[] retour1 = verifyPointFound( retour );
			return retour1;
		}
		
		public  Point[] returnSquare( Point pos, int rayon )
		{
			return returnSquare( pos.X, pos.Y, rayon );
		}
		#endregion

		#region return square case
		public  Case[] returnSquareCases( int xo, int yo, int rayon )
		{
			Point[] retour = returnSquare( xo, yo, rayon );
			Case[] cases = new Case[ retour.Length ];

			for ( int i = 0; i < cases.Length; i ++ )
				cases[ i ] = game.grid[ retour[ i ].X, retour[ i ].Y ];

			return cases;
		}
		
		public  Case[] returnSquareCases( Point pos, int rayon )
		{
			return returnSquareCases( pos.X, pos.Y, rayon );
		}

		public  Case[] returnSquareCases( Case pos, int rayon )
		{
			return returnSquareCases( pos.X, pos.Y, rayon );
		}
		#endregion

#region verify point found
		private  Point[] verifyPointFound( Point[] retour )
		{
			Point[] pntBuffer = new Point[ retour.Length ];
			int j = 0;

			for ( int i = 0; i < retour.Length; i ++ )
				if ( retour[ i ] != new Point( -1, -1 ) )
				{
					pntBuffer[ j ] = retour[ i ];
					j ++;
				}

			Point[] retour1 = new Point[ j ];
			for ( int i = 0; i < retour1.Length; i ++ )
				retour1[ i ] = pntBuffer[ i ];

			return retour1;
		}
		#endregion

#region verify limits
		private  Point[] verifyLimits( Point[] retour )
		{
			for ( int j = 0; j < retour.Length; j++ )
			{
				while ( retour[ j ].X < 0 )
					retour[ j ].X += game.width;

				while ( retour[ j ].X >= game.width)
					retour[ j ].X -= game.width;

				if ( retour[ j ].Y >= game.height)
				{
					retour[ j ].Y = - 1;
					retour[ j ].X = - 1;
				}
				else if ( retour[ j ].Y < 0)
				{
					retour[ j ].Y = - 1;	
					retour[ j ].X = - 1;
				}
			}

			return retour;
		}
		#endregion

#region return empty square 
		public  Point[] returnEmptySquare( Point pos, int radius )
		{
			return returnEmptySquare( pos.X, pos.Y, radius );
		}

		public  Point[] returnEmptySquare( int xo, int yo, int radius )
		{
			if ( radius == 0 )
			{
				Point[] retour1 = new Point[ 1 ];
				retour1[ 0 ] = new Point( xo, yo );
				return retour1;
			}

			Point[] retour = new Point[ radius * 8 ];
			int pntCounter = 0;

			if ( yo%2 == 1)
				{
					retour[ pntCounter] = new Point( xo - radius , yo );
					pntCounter ++;

					int i = 0;
					for ( int x = xo - radius + 1; x <= xo; x++ )
					{
						retour[ pntCounter] = new Point( x, yo - i*2 - 2);
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 + 2);
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 + 1 );
						pntCounter ++;
						i++;
					}
				
					i = radius;
					for ( int x = xo + 1; x <= xo + radius - 1; x++ )
					{ // bo
						retour[ pntCounter] = new Point( x, yo - i*2 + 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 + 2 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 2 );
						pntCounter ++;
						i--;
					}

					retour[ pntCounter] = new Point( xo + radius, yo );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo + radius, yo-1 );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo + radius, yo+1 );
					pntCounter ++;
				}
				else
				{
					retour[ pntCounter] = new Point( xo + radius , yo );
					pntCounter ++;

					int i = 0;
					for ( int x = xo + radius - 1; x >= xo; x-- )
					{
						retour[ pntCounter] = new Point( x, yo - i*2 - 2);
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 + 2);
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 + 1 );
						pntCounter ++;
						i++;
					}
				
					i = radius;
					for ( int x = xo - 1; x >= xo - radius + 1; x-- )
					{
						retour[ pntCounter] = new Point( x, yo - i*2 + 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo - i*2 + 2 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 1 );
						pntCounter ++;
						retour[ pntCounter] = new Point( x, yo + i*2 - 2 );
						pntCounter ++;
						i--;
					}

					retour[ pntCounter] = new Point( xo - radius, yo );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo - radius, yo-1 );
					pntCounter ++;
					retour[ pntCounter] = new Point( xo - radius, yo+1 );
					pntCounter ++;

				}

		/*	// vérification des limites
			for ( int j = 0; j < retour.Length; j++)
			{
				if ( retour[ j ].X < 0)
					retour[ j ].X += game.width;
				else if ( retour[ j ].X >= game.width)
					retour[ j ].X -= game.width;

				if ( retour[ j ].Y >= game.height)
				{
					retour[ j ].Y = - 1;
					retour[ j ].X = - 1;
				}
				else if ( retour[ j ].Y < 0)
				{
					retour[ j ].Y = - 1;	
					retour[ j ].X = - 1;
				}
			}*/

			/*Point[] pntBuffer = new Point[ retour.Length ];
			int j = 0;
			for ( int i = 0; i < retour.Length; i ++ )
			{
				if ( retour[ i ] != new Point( -1, -1 ) )
				{
					pntBuffer[ j ] = retour[ i ];
					j ++;
				}
			}

			Point[] retour1 = new Point[ j ];
			for ( int i = 0; i < retour1.Length; i ++ )
			{
				retour1[ i ] = pntBuffer[ i ];
			}*/

			Point[] retour2 = verifyPointFound( verifyLimits( retour ) );
			return retour2;

		}
		
		#endregion 

#region is next to water
		public  bool isNextToWater( int x, int y )
		{
			return game.grid[ x, y ].isNextToWater;

			/*Point[] bob5 = returnEmptySquare( x, y, 1);

			for ( int i = 0; i < bob5.Length; i++ )
				if ( Statistics.terrains[ game.grid[ bob5[ i ].X, bob5[ i ].Y ].type ].ew == 0 )
					return true;

			return false;*/
		}

		/*public  bool isNextToWater( int x, int y )
		{
			Point[] bob5 = returnEmptySquare( x, y, 1);

			for ( int i = 0; i < bob5.Length; i++ )
				if ( Statistics.terrains[ game.grid[ bob5[ i ].X, bob5[ i ].Y ].type ].ew == 0 )
					return true;
					
			return false;
		}*/

		public  void setIsNextToWater()
		{
			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					if ( Statistics.terrains[ game.grid[ x, y ].type ].ew == 1 )
					{
						if ( game.grid[ x, y ].river )
						{
							game.grid[ x, y ].isNextToWater = true; 
						}
						else
						{
							Point[] bob5 = returnEmptySquare( x, y, 1 );

							for ( int i = 0; i < bob5.Length; i++ )
								if ( 
									Statistics.terrains[ game.grid[ bob5[ i ].X, bob5[ i ].Y ].type ].ew == 0 ||
									game.grid[ bob5[ i ].X, bob5[ i ].Y ].river
									)
								{
									game.grid[ x, y ].isNextToWater = true; 
									break;
								}
						}
					}
					else if ( game.grid[ x, y ].type == (byte)enums.terrainType.coast )
					{
						Point[] bob5 = returnEmptySquare( x, y, 1 );

						for ( int i = 0; i < bob5.Length; i++ )
							if ( Statistics.terrains[ game.grid[ bob5[ i ].X, bob5[ i ].Y ].type ].ew == 1 )
							{
								game.grid[ x, y ].isNextToWater = true;
								break;
							}
					}
		}
		#endregion

#region is next to irrigation
		public bool isNextToIrrigation( int x, int y ) 
		{ 
			if ( Statistics.terrains[ game.grid[ x, y ].type ].ew == 0 ) 
				return false; 
			else if ( game.grid[ x, y ].river )
				return true;
			
			Point[] bob5 = returnEmptySquare( x, y, 1);
			for ( int i = 0; i < bob5.Length; i++ )
				if ( 
					Statistics.terrains[ game.grid[ bob5[ i ].X, bob5[ i ].Y ].type ].ew == 0 || 
					game.grid[ bob5[ i ].X, bob5[ i ].Y ].civicImprovement == 1 ||
					game.grid[ x, y ].river
					)
					return true;

			return false;
		}
		#endregion

#region is next to land
		public  bool isNextToLand( int x, int y )
		{
			Point[] bob5 = returnEmptySquare( x, y, 1);
			for ( int i = 0; i < bob5.Length; i++ )
			{
				if ( Statistics.terrains[ game.grid[ bob5[ i ].X, bob5[ i ].Y ].type ].ew == 1 )
					return true;
			}
			return false;
		}
#endregion

#region is in square
		public  bool isInSquare( Point ori, Point dest, int radius )
		{
			return isInSquare( ori.X, ori.Y, radius, dest.X, dest.Y ); 
		}

		public  bool isInSquare( int xo, int yo, int radius, int x, int y )
		{
			Point[] pnts = returnSquare( xo, yo, radius);

			for ( int i = 0; i < pnts.Length; i++ )
				if ( 
					pnts[ i ].X == x && 
					pnts[ i ].Y == y 
					)
					return true;

			return false;
		}
		#endregion

#region find nearest case to
		public  structures.pntWithDist[] findNearestCaseTo( int xo, int yo, int xDest, int yDest, byte terrain, byte player, bool tet, bool attacking )
		{
			Point dest = new Point( xDest, yDest );
			Point[] sqr = returnEmptySquare( xo, yo, 1 );
			bool[] valid = new bool[ sqr.Length ];
			int[] dist = new int[ sqr.Length ];
			byte[] order = new byte[ sqr.Length ];

			for ( int j = 0; j < sqr.Length; j ++ )
			{
				if ( sqr[ j ].X == xDest && sqr[ j ].Y == yDest )
				{
					structures.pntWithDist[] pwd = new xycv_ppc.structures.pntWithDist[ 1 ];
					pwd[ 0 ].X = sqr[ j ].X;
					pwd[ 0 ].Y = sqr[ j ].Y;
					pwd[ 0 ].dist = 0;

					return pwd;
				}

				if ( 
						( 
							Statistics.terrains[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].type ].ew == terrain || 
							(
								game.grid[ sqr[ j ].X, sqr[ j ].Y ].city > 0 &&
								(
									game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == player ||
									game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
									game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
									game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
								)
							) ||
							game.grid[ sqr[ j ].X, sqr[ j ].Y ].river
						) && 
						( 
							game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == player || 
							game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory == 0 || 
							game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector ||
							(
								game.playerList[ player ].foreignRelation[ game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war && 
								tet
							) ||
							(
								game.grid[ xo, yo ].territory - 1 != player &&
								(
									game.grid[ sqr[ j ].X, sqr[ j ].Y ].city == 0 || 
									attacking
								)
							)
						)
					)
				{ // valid case
					if ( 
						!game.radius.caseOccupiedByRelationType( sqr[ j ].X, sqr[ j ].Y, player, true, true, true, false, false, false )
						||
						(
							attacking &&
							!game.radius.caseOccupiedByRelationType( sqr[ j ].X, sqr[ j ].Y, player, false, true, true, false, false, false )
						)
						)
					{
						valid[ j ] = true;
						dist[ j ] = getDistWith( sqr[ j ], dest );
					}
				}
				else
				{ // invalid case
				}
			}

			byte validCases = 0;
			for ( int j = 0; j < sqr.Length; j ++ )
				if ( valid[ j ] )
					validCases ++;

			structures.pntWithDist[] retour = new structures.pntWithDist[ validCases ];
			for ( int j = 0, k = 0; j < sqr.Length; j ++ )
				if ( valid[ j ] )
				{
					retour[ k ].X = sqr[ j ].X;
					retour[ k ].Y = sqr[ j ].Y;
					retour[ k ].dist = dist[ j ];
					k ++;
				}

			bool mod = true;
			structures.pntWithDist buffer;

			while ( mod )
			{
				mod = false;

				for ( int j = 1; j < retour.Length; j ++ )
					if ( retour[ j ].dist < retour[ j - 1 ].dist )
					{
						buffer = retour[ j ];
						retour[ j ] = retour[ j - 1 ];
						retour[ j - 1 ] = buffer;
						mod = true;
					}
			}

			return retour;
		}
#endregion
		
#region findWayTo 1
	/*	public  System.Drawing.Point[] findWayTo1( byte player, Point ori, Point dest, byte terrain, bool attack )
		{
			if ( ori.X == -1 || ori.Y == -1 || dest.X == -1 || dest.Y == -1 )
				return new Point[ 1 ] { new Point( -1, -1 ) };

			Point[] pnt1 = new Point[ game.width * game.height ]; 
			int pos = 0; 
			int indOri = 0, indDest = 0; 

			byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 

			if ( terrain == 1 ) 
			{ 
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
					{
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( dest.X == x && dest.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indDest = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == game.grid[ xo, yo ].continent &&
							(
							game.grid[ x, y ].territory - 1 == player ||
							game.grid[ x, y ].territory == 0 ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
							) &&
							!caseOccupiedByRelationType( x, y, player, pols )
							)
						{
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						}
					}
			}
			else
			{
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( dest.X == x && dest.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indDest = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == 0 || 
							(
							game.grid[ x, y ].city > 0 &&
							(
							game.grid[ x, y ].territory - 1 == player ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
							)
							) &&
							!caseOccupiedByRelationType( x, y, player, pols )
							)
						{
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						}
			}

			Point[] pnt = new Point[ pos ]; 
			int[] order = new int[ pos ]; 
			int[] cost = new int[ pos ]; 
			int[] totalCost = new int[ pos ]; 
			bool[,] inRange = new bool[ pos, pos ];

			for ( int i = 0; i < pos; i ++ )
			{
				order[ i ] = 10000; 

				for ( int j = i + 1; j < pos; j ++ )
					if ( isNextTo( pnt[ i ], pnt[ j ] ) )
					{
						inRange[ i, j ] = true;
						inRange[ j, i ] = true;
					}
			}

			order[ indOri ] = 0;
			cost[ indOri ] = 0;

			Point[] way;
			bool allOverBestFound = false;
			int bestCost = 30000;

			bool mod = true;
			for ( int rad = 1; mod; rad ++ )
			{
				mod = false; 
				allOverBestFound = true; 

				for ( int i = 0; i < pos; i ++ )
					if ( order[ i ] == rad - 1 && i != indDest )
						for ( int j = 0; j < pos; j ++ )
							if ( inRange[ i, j ] && order[ j ] == 1000 )
							{
								order[ j ] = rad; 
								prevPnt[ j ] = i; 
								mod = true;

								cost[ j ] = cost[ i ] + 1;

								if ( j == indDest && cost[ j ] < bestCost ) 
								{ // yeah 
									way = new Point[ rad ]; 
									int[] wayInt = new int[ rad ]; 
									way[ way.Length - 1 ] = dest; 
									wayInt[ way.Length - 1 ] = j; 
									bestCost = cost[ j ];

									for ( int c = way.Length - 1; c > 0; c -- ) 
									{
										wayInt[ c - 1 ] = prevPnt[ wayInt[ c ] - 1 ]; 
										way[ c - 1 ] = new Point( 
											game.playerList[ player ].cityList[ prevPnt[ wayInt[ c ] - 1 ] ].X, 
											game.playerList[ player ].cityList[ prevPnt[ wayInt[ c ] - 1 ] ].Y 
											); 
									}

								} 

								if ( cost[ j ] < bestCost )
									allOverBestFound = false;
							}

				if ( !allOverBestFound )
					return bestWay;

			}

			return new Point[] { new Point( -1, -1 ) };	
		}*/
		#endregion

#region findWayTo 2

		public  System.Drawing.Point[] findWayTo2( int xo, int yo, int xDest, int yDest, byte terrain, byte player, bool attack/*, bool tet*/ )
		{
			if ( xo == -1 || yo == -1 || xDest == -1 || yDest == -1 )
				return new Point[ 1 ] { new Point( -1, -1 ) };

			/*
			if ( game.grid[ xo, yo ].continent != game.grid[ xDest, yDest ].continent )
				return new Point[ 1 ] { new Point( -1, -1 ) };
			*/
			
			if ( 
				game.grid[ xo, yo ].continent != game.grid[ xDest, yDest ].continent && 
				game.grid[ xo, yo ].continent != 0 &&
				game.grid[ xDest, yDest ].continent != 0 
				)
				return new Point[ 1 ] { new Point( -1, -1 ) };

			if ( xo == xDest && yo == yDest )
				return new Point[ 1 ] { new Point( -2, -2 ) };

			Point[] wayPnts = new Point[ 100 ];
		//	Radius radius = new Radius();
			int complexity = 0;
			int[,] order = new int[ game.width, game.height ];
			structures.pntWithDist[][][] pwd = new structures.pntWithDist[ game.width ][][];

			for ( int i = 0; i < game.width; i ++ )
				pwd[ i ] = new structures.pntWithDist[ game.height ][];
			
			wayPnts[ 0 ] = new Point( xo, yo );
			
			pwd[ xo ][ yo ] = findNearestCaseTo(		
				xo,		
				yo,		
				xDest,		
				yDest,		
				terrain,
				player,
				attack, //tet
				attack
				);

			if ( pwd[ xo ][ yo ].Length > 0 )
			{
				if ( pwd[ xo ][ yo ][ 0 ].dist == 0 )
				{
					Point[] wayPnts1 = new Point[ 1 ];
					wayPnts1[ 0 ] = new Point( xDest, yDest );
					return wayPnts1;
				}
			}
			else
				return new Point[ 1 ] { new Point( -1, -1 ) };

			for ( int i = 1; i < 100; i++ )
			{
				if ( pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ] == null )
				{
					pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ] = findNearestCaseTo(		
						wayPnts[ i - 1 ].X,		
						wayPnts[ i - 1 ].Y,		
						xDest,		
						yDest,		
						terrain,
						player,
						true, //tet
						attack
						);

					for ( int k = 0; k < pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ].Length; k ++ )
						if ( pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ][ k ].dist == 0 )
						{
							Point[] wayPnts1 = new Point[ i ]; 

							for ( int j = 0; j < wayPnts1.Length - 1 ; j ++ ) 
								wayPnts1[ j ] = wayPnts[ j + 1 ]; 

							wayPnts1[ wayPnts1.Length - 1 ] = new Point( xDest, yDest ); 
							return wayPnts1; 
						}
				}

				if ( pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ].Length <= order[ wayPnts[ i - 1 ].X, wayPnts[ i - 1 ].Y ] )
					return new Point[] { new Point( -1, -1 ) };	

				wayPnts[ i ].X = pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ][ order[ wayPnts[ i - 1 ].X, wayPnts[ i - 1 ].Y ] ].X;
				wayPnts[ i ].Y = pwd[ wayPnts[ i - 1 ].X ][ wayPnts[ i - 1 ].Y ][ order[ wayPnts[ i - 1 ].X, wayPnts[ i - 1 ].Y ] ].Y;

				for ( int k = 0; k < i; k ++ )
					if ( wayPnts[ i ] == wayPnts[ k ] )
					{
						if ( i - 2 != k )
						{
							order[ wayPnts[ i - 1 ].X, wayPnts[ i - 1 ].Y ] ++;
							i = k;
						}

						bool found = false; 
						while ( !found ) 
						{ 
							i --;
							if ( i < 0 )
								return new Point[] { new Point( -1, -1 ) };	

							order[ wayPnts[ i ].X, wayPnts[ i ].Y ] ++; // i + 1 - 1
							if ( order[ wayPnts[ i ].X, wayPnts[ i ].Y ] < pwd[ wayPnts[ i ].X ][ wayPnts[ i ].Y ].Length )		
								found = true; // ne dépasse pas la limite
						} 
						break; 
					} 

				complexity ++;
				if ( complexity > 2000 )
					return new Point[] { new Point( -1, -1 ) };
			}			
				
			return new Point[] { new Point( -1, -1 ) };		
		}		
#endregion
		
#region findWayTo
		public  System.Drawing.Point[] findWayTo( int xo, int yo, int xDest, int yDest, byte terrain, byte player, bool attack/*, bool tet*/ )
		{
			return findWayTo( new Point( xo, yo ), new Point( xDest, yDest ), terrain, player, attack/*, tet*/ );
		}

		public  System.Drawing.Point[] findWayTo( Point ori, Point dest, byte terrain, byte player, bool attack )
		{
			if ( terrain == 0 )
				return findWayToOnWater( ori, dest, player, attack );
			else 
				return findWayToOnLand( ori, dest, player, attack );
		}
		#endregion

	#region findWayToOnWater

		public  System.Drawing.Point[] findWayToOnWater( int xo, int yo, int xDest, int yDest, byte player, bool attack/*, bool tet*/ )
		{
			return findWayToOnWater( new Point( xo, yo ), new Point( xDest, yDest ), player, attack/*, tet*/ );
		}

		public  System.Drawing.Point[] findWayToOnWater( Point ori, Point dest, byte player, bool attack )
		{
			if ( ori.X == -1 || ori.Y == -1 || dest.X == -1 || dest.Y == -1 )
				return new Point[ 1 ] { new Point( -1, -1 ) };
			else if ( 
				game.grid[ ori.X, ori.Y ].continent != game.grid[ dest.X, dest.Y ].continent && 
				game.grid[ ori.X, ori.Y ].continent != 0 &&
				game.grid[ dest.X, dest.Y ].continent != 0 
				)
				return new Point[ 1 ] { new Point( -1, -1 ) };
			else if ( ori.X == dest.X && ori.Y == dest.Y )
				return new Point[ 1 ] { new Point( -2, -2 ) };
			else
			{
				byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 
				bool onNeutralTerritory = false;

				if ( 
					game.grid[ ori.X, ori.Y ].territory - 1 != player &&
					game.grid[ ori.X, ori.Y ].territory != 0 &&
					(
					game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
					game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace  ||
					game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
					)
					)
					onNeutralTerritory = true;
			
				uint[,] order = new uint[ game.width, game.height ];
				structures.Pnt[,] prevPnt = new structures.Pnt[ game.width, game.height ];
				bool[,] alreadyWorked = new bool[ game.width, game.height ];

				order[ ori.X, ori.Y ] = 1; 

				bool mod = true, modInRad = false; 

				for ( int rad = 1; mod && rad < 3 * game.width; rad ++ ) 
				{ 
					mod = false; 
					modInRad = true; 

					while ( modInRad )
					{
						modInRad = false; 

						for ( int x1 = 0; x1 < game.width; x1 ++ ) 
							for ( int y1 = 0; y1 < game.height; y1 ++ ) 
								if ( order[ x1, y1 ] == rad ) 
								{
									if ( !alreadyWorked[ x1, y1 ] )
									{ 
										alreadyWorked[ x1, y1 ] = true;
										structures.Pnt[] inRange = new structures.Pnt[ 8 ];
										Point[] sqr = game.radius.returnEmptySquare( x1, y1, 1 );
										int pos = 0;

										for ( int k = 0; k < sqr.Length; k ++ )
											if ( ori.X == sqr[ k ].X && ori.Y == sqr[ k ].Y )
											{
												inRange[ pos ].X = (int)sqr[ k ].X; 
												inRange[ pos ].Y = (int)sqr[ k ].Y; 
												pos ++;
											}
											else if ( dest.X == sqr[ k ].X && dest.Y == sqr[ k ].Y )
											{
												inRange[ pos ].X = (int)sqr[ k ].X; 
												inRange[ pos ].Y = (int)sqr[ k ].Y; 
												pos ++; 
											}
											else if ( 
												Statistics.terrains[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == 0 || 
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].river ||
												( 
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 && 
												( 
												onNeutralTerritory || 
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player || 
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance || 
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected || 
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector || 
												( 
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war && 
												attack 
												) 
												) 
												) && 
												!caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, pols ) 
												) 
											{ 
												inRange[ pos ].X = (int)sqr[ k ].X; 
												inRange[ pos ].Y = (int)sqr[ k ].Y; 
												pos ++; 
											}

										for ( int k = pos; k < inRange.Length; k ++ )
											inRange[ k ].X = -1;

										for ( int i = 0; i < inRange.Length; i ++ ) 
											if ( 
												inRange[ i ].X != -1 &&
												(
													order[ inRange[ i ].X, inRange[ i ].Y ] == 0  || 
													( 
														order[ inRange[ i ].X, inRange[ i ].Y ] > rad && 
														order[ inRange[ i ].X, inRange[ i ].Y ] > rad + costBetweenOnWater( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) 
													) 
												)
												) 
											{ 
												order[ inRange[ i ].X, inRange[ i ].Y ] = (uint)( rad + costBetweenOnWater( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) ); 
												prevPnt[ inRange[ i ].X, inRange[ i ].Y ].X = x1; 
												prevPnt[ inRange[ i ].X, inRange[ i ].Y ].Y = y1; 
												mod = true; 
												modInRad = true; 

												if ( inRange[ i ].X == dest.X && inRange[ i ].Y == dest.Y ) 
												{ 
													Point[] way = new Point[ order[ inRange[ i ].X, inRange[ i ].Y ] ]; 
													structures.Pnt[] wayInt = new structures.Pnt[ order[ inRange[ i ].X, inRange[ i ].Y ] ]; 
													way[ 0 ] = dest; 
													wayInt[ 0 ] = inRange[ i ];

													for ( int c = 0; c < order[ inRange[ i ].X, inRange[ i ].Y ] - 1; c ++ ) 
													{ 
														if ( 
															prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].X == ori.X && 
															prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].Y == ori.Y
															)
														{
															Point[] finalWay = new Point[ c + 1 ];

															for ( int f = 0; f < finalWay.Length; f ++ ) 
																finalWay[ f ] = way[ finalWay.Length - f - 1 ]; 

															return finalWay;
														}

														wayInt[ c + 1 ] = prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ]; 
														way[ c + 1 ] = new Point( prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].X,  prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].Y ); 
													} 

													return new Point[] { new Point( -1, -1 ) };	
													//	return way; 
												} 
											} 
									} 
								}
								else if ( order[ x1, y1 ] > rad ) 
									mod = true; 
					} 
				} 

				return new Point[] { new Point( -1, -1 ) };	
			}
		}
		#endregion

	#region findWayToOnLand

		public  System.Drawing.Point[] findWayToOnLand( int xo, int yo, int xDest, int yDest, byte player, bool attack/*, bool tet*/ )
		{
			return findWayToOnLand( new Point( xo, yo ), new Point( xDest, yDest ), player, attack/*, tet*/ );
		}

		public  System.Drawing.Point[] findWayToOnLand( Point ori, Point dest, byte player, bool attack )
		{
			if ( ori.X == -1 || ori.Y == -1 || dest.X == -1 || dest.Y == -1 )
				return new Point[ 1 ] { new Point( -1, -1 ) };
			else if ( 
				game.grid[ ori.X, ori.Y ].continent != game.grid[ dest.X, dest.Y ].continent && 
				game.grid[ ori.X, ori.Y ].continent != 0 &&
				game.grid[ dest.X, dest.Y ].continent != 0 
				)
				return new Point[ 1 ] { new Point( -1, -1 ) };
			else if ( ori.X == dest.X && ori.Y == dest.Y )
				return new Point[ 1 ] { new Point( -2, -2 ) };
			else
			{
				byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 
				bool onNeutralTerritory = false;

				if ( 
					game.grid[ ori.X, ori.Y ].territory - 1 != player &&
					game.grid[ ori.X, ori.Y ].territory != 0 &&
					(
					game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
					game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace ||
					game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
					)
					)
					onNeutralTerritory = true;
			
				uint[,] order = new uint[ game.width, game.height ];
				structures.Pnt[,] prevPnt = new structures.Pnt[ game.width, game.height ];
				bool[,] alreadyWorked = new bool[ game.width, game.height ];

				order[ ori.X, ori.Y ] = 1; 

				bool mod = true, modInRad = false; 

				for ( int rad = 1; mod && rad < 3 * game.width; rad ++ ) 
				{ 
					mod = false; 
					modInRad = true; 

					while ( modInRad )
					{
						modInRad = false; 

						for ( int x1 = 0; x1 < game.width; x1 ++ ) 
							for ( int y1 = 0; y1 < game.height; y1 ++ ) 
								if ( order[ x1, y1 ] == rad ) 
								{
									if ( !alreadyWorked[ x1, y1 ] )
									{ 
										alreadyWorked[ x1, y1 ] = true;
										structures.Pnt[] inRange = new structures.Pnt[ 8 ];
										Point[] sqr = game.radius.returnEmptySquare( x1, y1, 1 );
										int pos = 0;
										
										for ( int k = 0; k < sqr.Length; k ++ ) 
											if ( ori.X == sqr[ k ].X && ori.Y == sqr[ k ].Y ) 
											{ 
												inRange[ pos ].X = (int)sqr[ k ].X; 
												inRange[ pos ].Y = (int)sqr[ k ].Y; 
												pos ++; 
											} 
											else if ( dest.X == sqr[ k ].X && dest.Y == sqr[ k ].Y ) 
											{ 
												inRange[ pos ].X = (int)sqr[ k ].X; 
												inRange[ pos ].Y = (int)sqr[ k ].Y; 
												pos ++; 
											} 
											else if ( 
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == game.grid[ ori.X, ori.Y ].continent &&
												(
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player || 
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory == 0 || 
												(
												onNeutralTerritory && 
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].city == 0 
												) ||
												(
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector 
												) || 
												(
												attack &&
												game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
												)
												) &&
												!caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, pols )
												)
											{
												inRange[ pos ].X = (int)sqr[ k ].X; 
												inRange[ pos ].Y = (int)sqr[ k ].Y; 
												pos ++;
											}

										for ( int k = pos; k < inRange.Length; k ++ )
											inRange[ k ].X = -1;

										for ( int i = 0; i < inRange.Length; i ++ ) 
											if ( 
												inRange[ i ].X != -1 &&
												(
												order[ inRange[ i ].X, inRange[ i ].Y ] == 0  || 
												( 
												order[ inRange[ i ].X, inRange[ i ].Y ] > rad && 
												order[ inRange[ i ].X, inRange[ i ].Y ] > rad + costBetweenOnLand( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) 
												) 
												)
												) 
											{ 
												order[ inRange[ i ].X, inRange[ i ].Y ] = (uint)( rad + costBetweenOnLand( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) ); 
												prevPnt[ inRange[ i ].X, inRange[ i ].Y ].X = x1; 
												prevPnt[ inRange[ i ].X, inRange[ i ].Y ].Y = y1; 
												mod = true; 
												modInRad = true; 

												if ( inRange[ i ].X == dest.X && inRange[ i ].Y == dest.Y ) 
												{ 
													Point[] way = new Point[ order[ inRange[ i ].X, inRange[ i ].Y ] ]; 
													structures.Pnt[] wayInt = new structures.Pnt[ order[ inRange[ i ].X, inRange[ i ].Y ] ]; 
													way[ 0 ] = dest; 
													wayInt[ 0 ] = inRange[ i ];

													for ( int c = 0; c < order[ inRange[ i ].X, inRange[ i ].Y ] - 1; c ++ ) 
													{ 
														if ( 
															prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].X == ori.X && 
															prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].Y == ori.Y
															)
														{
															Point[] finalWay = new Point[ c + 1 ];

															for ( int f = 0; f < finalWay.Length; f ++ ) 
																finalWay[ f ] = way[ finalWay.Length - f - 1 ]; 

															return finalWay;
														}

														wayInt[ c + 1 ] = prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ]; 
														way[ c + 1 ] = new Point( prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].X,  prevPnt[ wayInt[ c ].X, wayInt[ c ].Y ].Y ); 
													} 
												} 
											} 
									} 
								}
								else if ( order[ x1, y1 ] > rad ) 
									mod = true; 
					} 
				} 

				return new Point[] { new Point( -1, -1 ) };	
			}
		}
		#endregion

#region findWayTo 4
		public  System.Drawing.Point[] findWayTo4( Point ori, Point dest, byte terrain, byte player, bool attack )
		{

			if ( ori.X == -1 || ori.Y == -1 || dest.X == -1 || dest.Y == -1 )
				return new Point[ 1 ] { new Point( -1, -1 ) };
			
			if ( 
				game.grid[ ori.X, ori.Y ].continent != game.grid[ dest.X, dest.Y ].continent && 
				game.grid[ ori.X, ori.Y ].continent != 0 &&
				game.grid[ dest.X, dest.Y ].continent != 0 
				)
				return new Point[ 1 ] { new Point( -1, -1 ) };

			if ( ori.X == dest.X && ori.Y == dest.Y )
				return new Point[ 1 ] { new Point( -2, -2 ) };

			Point[] pnt1 = new Point[ game.width * game.height ]; 
			int pos = 0; 
			int indOri = 0, indDest = 0; 

			byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 

			if ( terrain == 1 ) 
			{ 
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
					{
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( dest.X == x && dest.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indDest = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == game.grid[ ori.X, ori.Y ].continent &&
							(
							game.grid[ x, y ].territory - 1 == player ||
							game.grid[ x, y ].territory == 0 ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
							) &&
							!caseOccupiedByRelationType( x, y, player, pols )
							)
						{
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						}
					}
			}
			else
			{ // water
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( dest.X == x && dest.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indDest = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == 0 || 
							(
							game.grid[ x, y ].city > 0 &&
							(
							game.grid[ x, y ].territory - 1 == player ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
							)
							) &&
							!caseOccupiedByRelationType( x, y, player, pols )
							)
						{
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						}
			}
			
			Point[] pnt = new Point[ pos ]; 
			int[] cost = new int[ pos ], prevPnt = new int[ pos ], order = new int[ pos ]; 
			Int16[][] inRange =  new Int16[ pos ][];

			for ( int i = 0; i < pos; i ++ ) 
			{
				pnt[ i ] = pnt1[ i ]; 
				order[ i ] = -1; 
			}

			System.Collections.BitArray alreadyWorked = new System.Collections.BitArray( pos, false );

			order[ indOri ] = 0; 
			cost[ indOri ] = 0; 
			Point[] way; 

			bool mod = true, modInRad = false; 

			for ( int rad = 0; mod; rad ++ ) 
			{ 
				mod = false; 
				modInRad = true; 

				while ( modInRad )
				{
					modInRad = false; 

					for ( int i = 0; i < pos; i ++ ) 
						if ( order[ i ] == rad ) 
						{
							if ( !alreadyWorked.Get( i ) ) 
							{ 
								Int16[] temp = new Int16[ pos ];
								int tot = 0;

								for ( int j = 0; j < pos; j ++ ) 
									if ( isNextTo( pnt[ i ], pnt[ j ] ) ) 
									{ 
										temp[ tot ] = (Int16)j;
										tot ++;
									} 

								inRange[ i ] = new Int16[ tot ];
								
								for ( int j = 0; j < tot; j ++ ) 
									inRange[ i ][ j ] = temp[ j ];

								alreadyWorked.Set( i, true ); 
							} 

							for ( int j = 0; j < inRange[ i ].Length; j ++ ) 
								if ( 
									order[ inRange[ i ][ j ] ] == -1 || 
									( 
									order[ inRange[ i ][ j ] ] > rad && 
									order[ inRange[ i ][ j ] ] > rad + costBetween( pnt[ i ], pnt[ inRange[ i ][ j ] ] ) 
									) 
									) 
								{ 
									order[ i ] --;
									order[ inRange[ i ][ j ] ] = rad + costBetween( pnt[ i ], pnt[ inRange[ i ][ j ] ] ); 
									prevPnt[ inRange[ i ][ j ] ] = i; 
									mod = true; 
									modInRad = true; 

									if ( inRange[ i ][ j ] == indDest ) 
									{ 
										way = new Point[ pos ]; 
										int[] wayInt = new int[ pos ]; 
										way[ 0 ] = dest; 
										wayInt[ 0 ] = indDest; //inRange[ i ][ j ]; 

										for ( int c = 0; c < pos - 1; c ++ ) 
										{ 
											if ( prevPnt[ wayInt[ c ] ] == indOri )
											{
												Point[] finalWay = new Point[ c + 1 ];

												for ( int f = 0; f < finalWay.Length; f ++ ) 
													finalWay[ f ] = way[ finalWay.Length - f - 1 ]; 

												return finalWay;
											}

											wayInt[ c + 1 ] = prevPnt[ wayInt[ c ] ]; 
											way[ c + 1 ] = pnt[ prevPnt[ wayInt[ c ] ] ]; 
										} 

										return way; 
									} 
								} 
						} 
						else if ( order[ i ] < rad ) 
						{ 
							mod = true; 
						} 
				} 
			} 

			return new Point[] { new Point( -1, -1 ) };	
		}
		#endregion

#region findCostTo
		public  int findCostTo( int xo, int yo, int xDest, int yDest, byte terrain, byte player, bool attack/*, bool tet*/ )
		{
			return findCostTo( new Point( xo, yo ), new Point( xDest, yDest ), terrain, player, attack/*, tet*/ );
		}

		public  int findCostTo( Point ori, Point dest, byte terrain, byte player, bool attack )
		{
			if ( ori.X == dest.X && ori.Y == dest.Y )
				return 0;

			if ( ori.X == -1 || ori.Y == -1 || dest.X == -1 || dest.Y == -1 )
				return 10000;
			
			if ( 
				game.grid[ ori.X, ori.Y ].continent != game.grid[ dest.X, dest.Y ].continent && 
				game.grid[ ori.X, ori.Y ].continent != 0 &&
				game.grid[ dest.X, dest.Y ].continent != 0 
				)
				return 10000;

			Point[] pnt1 = new Point[ game.width * game.height ]; 
			int pos = 0; 
			int indOri = 0, indDest = 0; 

			byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 
			bool onNeutralTerritory = false;

			if ( 
				game.grid[ ori.X, ori.Y ].territory - 1 != player &&
				game.grid[ ori.X, ori.Y ].territory != 0 &&
				(
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace  ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
				)
				)
				onNeutralTerritory = true;

			if ( terrain == 1 ) 
			{ 
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
					{
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( dest.X == x && dest.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indDest = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == game.grid[ ori.X, ori.Y ].continent &&
							(
							game.grid[ x, y ].territory - 1 == player || 
							game.grid[ x, y ].territory == 0 || 
							(
							onNeutralTerritory && 
							game.grid[ x, y ].city == 0 
							) ||
							(
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector 
							) || 
							(
							attack &&
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
							)
							) &&
							!caseOccupiedByRelationType( x, y, player, pols )
							)
						{
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						}
					}
			}
			else
			{ // water
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( dest.X == x && dest.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indDest = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == 0 || 
							( 
							game.grid[ x, y ].city > 0 && 
							( 
							onNeutralTerritory ||
							game.grid[ x, y ].territory - 1 == player || 
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector || 
							(
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war && 
							attack
							)
							) 
							) && 
							!caseOccupiedByRelationType( x, y, player, pols ) 
							) 
						{ 
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						} 
			}
			
			Point[] pnt = new Point[ pos ]; 
			int[] cost = new int[ pos ], prevPnt = new int[ pos ], order = new int[ pos ]; 
			System.Collections.BitArray[] inRange = new System.Collections.BitArray[ pos ]; 

			for ( int i = 0; i < pos; i ++ ) 
			{
				pnt[ i ] = pnt1[ i ]; 
				order[ i ] = -1; 

				inRange[ i ] = new System.Collections.BitArray( i );
			}

			System.Collections.BitArray alreadyWorked = new System.Collections.BitArray( pos, false );
			order[ indOri ] = 0; 
			cost[ indOri ] = 0; 

			bool mod = true, modInRad = false; 

			for ( int rad = 0; mod && rad < 3 * game.width; rad ++ ) 
			{ 
				mod = false; 
				modInRad = true; 

				while ( modInRad )
				{
					modInRad = false; 

					for ( int i = 0; i < pos; i ++ ) 
						if ( order[ i ] == rad ) 
						{
							if ( !alreadyWorked.Get( i ) ) 
							{ 
								for ( int j = 0; j < pos; j ++ ) 
									if ( isNextTo( pnt[ i ], pnt[ j ] ) ) 
									{ 
										if ( i < j ) 
											inRange[ j ].Set( i, true ); 
										else if ( i > j ) 
											inRange[ i ].Set(j, true ); 
									} 

								alreadyWorked.Set( i, true ); 
							} 

							for ( int j = 0; j < pos; j ++ ) 
								if ( 
									(
									( 
									i < j &&
									inRange[ j ].Get( i )
									) ||
									( 
									i > j &&
									inRange[ i ].Get( j )
									)
									)
									&& 
									( 
									order[ j ] == -1 || 
									( 
									order[ j ] > rad && 
									order[ j ] > rad + costBetween( pnt[ i ], pnt[ j ] ) 
									) 
									) 
									) 
								{ 
									order[ i ] --;
									order[ j ] = rad + costBetween( pnt[ i ], pnt[ j ] ); 

									prevPnt[ j ] = i; 
									mod = true; 
									modInRad = true; 

									if ( j == indDest ) 
										return order[ j ];
								} 
						} 
						else if ( order[ i ] > rad ) 
						{ 
							mod = true; 
						} 
				} 
			} 

			return 10000;
		}
#endregion

#region gridCost
		public  int[,] gridCost( Point ori, byte terrain, byte player, bool attack, int radius )
		{
			if ( terrain == 0 )
				return gridCostOnWater( ori, player, attack, radius );
			else
				return gridCostOnLand( ori, player, attack, radius );

		/*	Point[] pnt1 = new Point[ game.width * game.height ]; 
			int pos = 0, indOri = 0; 
			byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 
			bool onNeutralTerritory = false; 

			if ( 
				game.grid[ ori.X, ori.Y ].territory - 1 != player &&
				game.grid[ ori.X, ori.Y ].territory != 0 &&
				(
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace  ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
				)
				)
				onNeutralTerritory = true;

			if ( terrain == 1 ) 
			{ 
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == game.grid[ ori.X, ori.Y ].continent &&
							(
							game.grid[ x, y ].territory - 1 == player || 
							game.grid[ x, y ].territory == 0 || 
							(
							onNeutralTerritory && 
							game.grid[ x, y ].city == 0 
							) ||
							(
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector 
							) || 
							(
							attack &&
							game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
							)
							) &&
							!caseOccupiedByRelationType( x, y, player, pols )
							)
						{
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						}
			}
			else
			{ // water
				for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						if ( ori.X == x && ori.Y == y )
						{
							pnt1[ pos ] = new Point( x, y ); 
							indOri = pos; 
							pos ++; 
						}
						else if ( 
							game.grid[ x, y ].continent == 0 || 
							( 
								game.grid[ x, y ].city > 0 && 
								( 
									onNeutralTerritory ||
									game.grid[ x, y ].territory - 1 == player || 
									game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
									game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
									game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector || 
									(
									game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war && 
									attack
									)
								) 
							) && 
							!caseOccupiedByRelationType( x, y, player, pols ) 
							) 
						{ 
							pnt1[ pos ] = new Point( x, y ); 
							pos ++; 
						} 
			}
			
			Point[] pnt = new Point[ pos ]; 
			int[] order = new int[ pos ]; 
			System.Collections.BitArray[] inRange = new System.Collections.BitArray[ pos ]; 

			for ( int i = 0; i < pos; i ++ ) 
			{
				pnt[ i ] = pnt1[ i ]; 
				order[ i ] = -1; 
				inRange[ i ] = new System.Collections.BitArray( i );
			}

			System.Collections.BitArray alreadyWorked = new System.Collections.BitArray( pos, false );
			order[ indOri ] = 0; 
			bool mod = true, modInRad = false; 

			for ( int rad = 0; mod && rad < radius * 3 /*3 * game.width/; rad ++ ) 
			{ 
				mod = false; 
				modInRad = true; 

				while ( modInRad ) 
				{ 
					modInRad = false; 

					for ( int i = 0; i < pos; i ++ ) 
						if ( order[ i ] == rad ) 
						{ 
							if ( !alreadyWorked.Get( i ) ) 
							{ 
								for ( int j = 0; j < pos; j ++ ) 
									if ( isNextTo( pnt[ i ], pnt[ j ] ) ) 
									{ 
										if ( i < j ) 
											inRange[ j ].Set( i, true ); 
										else if ( i > j ) 
											inRange[ i ].Set(j, true ); 
									} 

								alreadyWorked.Set( i, true ); 
							} 

							for ( int j = 0; j < pos; j ++ ) 
								if ( 
									(
										( 
										i < j &&
										inRange[ j ].Get( i )
										) ||
										( 
											i > j &&
											inRange[ i ].Get( j )
										)
									)
									&& 
									( 
										order[ j ] == -1 || 
										( 
											order[ j ] > rad && 
											order[ j ] > rad + costBetween( pnt[ i ], pnt[ j ] ) 
										) 
									) 
									) 
								{ 
									order[ j ] = rad + costBetween( pnt[ i ], pnt[ j ] ); 
									mod = true; 
									modInRad = true; 
								} 
						} 
						else if ( order[ i ] > rad ) 
							mod = true; 
				} 
			} 
			int[,] costs = new int[ game.width, game.height ]; 

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.width; y ++ )
					costs[ x, y ] = -1; 

			for ( int i = 0; i < pos; i ++ ) 
				if ( order[ i ] != -1 ) 
					costs[ pnt[ i ].X, pnt[ i ].Y ] = order[ i ]; 

			return costs;*/
		}
#endregion

	#region gridCostOnLand
		public  int[,] gridCostOnLand( Point ori, byte player, bool attack, int radius )
		{
			byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 
			bool onNeutralTerritory = false;

			if ( 
				game.grid[ ori.X, ori.Y ].territory - 1 != player &&
				game.grid[ ori.X, ori.Y ].territory != 0 &&
				(
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace  ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
				)
				)
				onNeutralTerritory = true;
			
			int[,] order = new int[ game.width, game.height ];
			bool[,] alreadyWorked = new bool[ game.width, game.height ];

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					order[ x, y ] = -1; 

			order[ ori.X, ori.Y ] = 0; 

			bool mod = true, modInRad = false; 
			
			int pos = 0;

			for ( int rad = 0; mod && rad < 3 * game.width; rad ++ ) 
			{ 
				mod = false; 
				modInRad = true; 

				while ( modInRad )
				{
					modInRad = false; 

					for ( int x1 = 0; x1 < game.width; x1 ++ ) 
						for ( int y1 = 0; y1 < game.height; y1 ++ ) 
							if ( order[ x1, y1 ] == rad ) 
							{
								if ( !alreadyWorked[ x1, y1 ] )
								{ 
									alreadyWorked[ x1, y1 ] = true;
									structures.Pnt[] inRange = new structures.Pnt[ 8 ];
									Point[] sqr = game.radius.returnEmptySquare( x1, y1, 1 );
									pos = 0;
										
									for ( int k = 0; k < sqr.Length; k ++ ) 
										if ( ori.X == sqr[ k ].X && ori.Y == sqr[ k ].Y ) 
										{ 
											inRange[ pos ].X = (int)sqr[ k ].X; 
											inRange[ pos ].Y = (int)sqr[ k ].Y; 
											pos ++; 
										} 
										else if ( 
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == game.grid[ ori.X, ori.Y ].continent &&
											(
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player || 
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory == 0 || 
											(
											onNeutralTerritory && 
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].city == 0 
											) ||
											(
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector 
											) || 
											(
											attack &&
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
											)
											) &&
											!caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, pols )
											)
										{
											inRange[ pos ].X = (int)sqr[ k ].X; 
											inRange[ pos ].Y = (int)sqr[ k ].Y; 
											pos ++;
										}

									for ( int k = pos; k < inRange.Length; k ++ )
										inRange[ k ].X = -1;

									for ( int i = 0; i < inRange.Length; i ++ ) 
										if ( 
											inRange[ i ].X != -1 &&
											(
											order[ inRange[ i ].X, inRange[ i ].Y ] == -1  || 
											( 
											order[ inRange[ i ].X, inRange[ i ].Y ] > rad && 
											order[ inRange[ i ].X, inRange[ i ].Y ] > rad + costBetweenOnLand( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) 
											) 
											)
											) 
										{ 
											order[ inRange[ i ].X, inRange[ i ].Y ] = (int)( rad + costBetweenOnLand( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) ); 
											mod = true; 
											modInRad = true; 
										} 
								} 
							}
							else if ( order[ x1, y1 ] > rad ) 
								mod = true; 
				} 
			} 

	/*		for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.width; y ++ )
					order[ x, y ] --; */

			return order;
		}
		#endregion

	#region gridCostOnWater
		public  int[,] gridCostOnWater( Point ori, byte player, bool attack, int radius )
		{
			byte[] pols = returnRelationTypeList( !attack, true, true, false, false, false ); 
			bool onNeutralTerritory = false;

			if ( 
				game.grid[ ori.X, ori.Y ].territory - 1 != player &&
				game.grid[ ori.X, ori.Y ].territory != 0 &&
				(
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace  ||
				game.playerList[ player ].foreignRelation[ game.grid[ ori.X, ori.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
				)
				)
				onNeutralTerritory = true;
			
			int[,] order = new int[ game.width, game.height ];
			bool[,] alreadyWorked = new bool[ game.width, game.height ];

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					order[ x, y ] = -1; 

			order[ ori.X, ori.Y ] = 0; 

			bool mod = true, modInRad = false; 
			
			int pos = 0;

			for ( int rad = 0; mod && rad < 3 * game.width; rad ++ ) 
			{ 
				mod = false; 
				modInRad = true; 

				while ( modInRad )
				{
					modInRad = false; 

					for ( int x1 = 0; x1 < game.width; x1 ++ ) 
						for ( int y1 = 0; y1 < game.height; y1 ++ ) 
							if ( order[ x1, y1 ] == rad ) 
							{
								if ( !alreadyWorked[ x1, y1 ] )
								{ 
									alreadyWorked[ x1, y1 ] = true;
									structures.Pnt[] inRange = new structures.Pnt[ 8 ];
									Point[] sqr = game.radius.returnEmptySquare( x1, y1, 1 );
									pos = 0;
										
									for ( int k = 0; k < sqr.Length; k ++ ) 
										if ( ori.X == sqr[ k ].X && ori.Y == sqr[ k ].Y )
										{
											inRange[ pos ].X = (int)sqr[ k ].X; 
											inRange[ pos ].Y = (int)sqr[ k ].Y; 
											pos ++;
										}
										else if ( 
											Statistics.terrains[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == 0 || 
											( 
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 && 
											( 
											onNeutralTerritory || 
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player || 
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance || 
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected || 
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector || 
											( 
											game.playerList[ player ].foreignRelation[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war && 
											attack 
											) 
											) 
											) && 
											!caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, pols ) 
											) 
										{ 
											inRange[ pos ].X = (int)sqr[ k ].X; 
											inRange[ pos ].Y = (int)sqr[ k ].Y; 
											pos ++; 
										}

									for ( int k = pos; k < inRange.Length; k ++ )
										inRange[ k ].X = -1;

									for ( int i = 0; i < inRange.Length; i ++ ) 
										if ( 
											inRange[ i ].X != -1 &&
											(
											order[ inRange[ i ].X, inRange[ i ].Y ] == -1  || 
											( 
											order[ inRange[ i ].X, inRange[ i ].Y ] > rad && 
											order[ inRange[ i ].X, inRange[ i ].Y ] > rad + costBetweenOnWater( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) 
											) 
											)
											) 
										{ 
											order[ inRange[ i ].X, inRange[ i ].Y ] = (int)( rad + costBetweenOnWater( new Point( x1, y1 ), new Point( inRange[ i ].X, inRange[ i ].Y ) ) ); 
											mod = true; 
											modInRad = true; 
										} 
								} 
							}
							else if ( order[ x1, y1 ] > rad ) 
								mod = true; 
				} 
			} 

			return order;
		}
		#endregion

#region costs between
		public  int costBetween( Point ori, Point dest )
		{
			if ( game.grid[ ori.X, ori.Y ].type == (byte)enums.terrainType.coast && game.grid[ dest.X, dest.Y ].type == (byte)enums.terrainType.coast )
				return 1;
			else if ( game.grid[ ori.X, ori.Y ].type == (byte)enums.terrainType.sea || game.grid[ dest.X, dest.Y ].type == (byte)enums.terrainType.sea )
				return 3;
			else if ( game.grid[ ori.X, ori.Y ].roadLevel == 2 && game.grid[ dest.X, dest.Y ].roadLevel == 2 )
				return 0;
			else if ( game.grid[ ori.X, ori.Y ].roadLevel > 0 && game.grid[ dest.X, dest.Y ].roadLevel > 0 )
				return 1;
			else
				return Statistics.terrains[ game.grid[ dest.X, dest.Y ].type ].move * 3;
		}

		public  int costBetweenOnLand( Point ori, Point dest )
		{
			if ( game.grid[ ori.X, ori.Y ].roadLevel == 2 && game.grid[ dest.X, dest.Y ].roadLevel == 2 )
				return 0;
			else if ( game.grid[ ori.X, ori.Y ].roadLevel > 0 && game.grid[ dest.X, dest.Y ].roadLevel > 0 )
				return 1;
			else
				return Statistics.terrains[ game.grid[ dest.X, dest.Y ].type ].move * 3;
		}

		public  int costBetweenOnWater( Point ori, Point dest )
		{
			if ( game.grid[ ori.X, ori.Y ].type == (byte)enums.terrainType.coast && game.grid[ dest.X, dest.Y ].type == (byte)enums.terrainType.coast )
				return 1;
			else //if ( game.grid[ ori.X, ori.Y ].type == (byte)enums.terrainType.sea || game.grid[ dest.X, dest.Y ].type == (byte)enums.terrainType.sea )
				return 3;
		}
#endregion

#region case is ok
		public bool caseIsOk( int x, int y, Point[] pntToAvoid )
		{
			for ( int i = 0; i < pntToAvoid.Length; i ++ )
			{
				if ( x == pntToAvoid[ i ].X && y == pntToAvoid[ i ].Y )
				{
					return false;
				}
			}
			return true;
		}
		#endregion

#region find direction to
		public Point findDirectionTo( int xo, int yo, int xDest, int yDest )
		{ // Point
			Point retour = new Point( -1, -1 );

			int xDiff = xDest - xo;
			int yDiff = yDest - yo;

			if ( xDiff > game.width / 2 )
			{
				xDiff -= game.width;
			}
			else if ( xDiff < game.width / - 2 )
			{
				xDiff += game.width;
			}

			if ( isInSquare( xo, yo, 1, xDest, yDest ) )
			{
				return new Point( xDest, yDest );
			}

			int xDiff1 = xDiff;
			if ( xDiff < 0 )
			{
				xDiff1 = - xDiff;
			}

			int yDiff1 = yDiff;
			if ( yDiff < 0 )
			{
				yDiff1 = - yDiff;
			}

			if ( xDiff1 > yDiff1 / 2 )
			{
				if ( xDiff > 0 )
				{
					retour = new Point( xo + 1, yo );
				}
				else
				{
					retour = new Point( xo - 1, yo );
				}
			}
			else
			{
				if ( yDiff > 0 )
				{
					retour = new Point( xo, yo + 2 );
				}
				else
				{
					retour = new Point( xo, yo - 2 );
				}
			}

			if ( retour.X < 0)
				retour.X += game.width;
			else if ( retour.X >= game.width)
				retour.X -= game.width;

			return retour;
		}
#endregion

#region is near an ennemy unit
		public bool isNearAnEnnemyUnit( int xo, int yo, byte owner )
		{
		//	Radius radius = new Radius();

			Point[] sqr = game.radius.returnSquare( xo, yo, 2 );

			for ( int i = 0; i < sqr.Length; i ++ )
			{
				if ( game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack.Length > 0 )
				{
					for ( int j = 1; j <= game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack.Length; j ++ )
					{
						if ( game.playerList[ owner ].foreignRelation[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].player.player ].politic == (byte)Form1.relationPolType.war )
						{
							return true;
						}
					}
				}
				else if ( 
					game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 && 
					game.playerList[ owner ].foreignRelation[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war )
				{
					return true;
				}
			}

			return false;
		}
#endregion

#region nearest city
		/*public Point findNearestSafeSpot( int xo, int yo, byte owner )
		{
		}*/
#endregion

#region goesThroughtEnnemyTerritory
		public bool goesThroughtEnnemyTerritory ( int xo, int yo, Point dest, byte player )
		{
			int xDiff = dest.X - xo;
			int yDiff = dest.Y - yo;

			if ( xDiff > game.width / 2 )
			{
				xDiff -= game.width;
			}

			int x1 = xDiff * 1 / 3 + xo;
			int x2 = xDiff * 2 / 3 + xo;

			if ( x1 >= game.width )
				x1 -= game.width;
			else if ( x1 < 0 )
				x1 += game.width;

			if ( x2 >= game.width )
				x2 -= game.width;
			else if ( x2 < 0 )
				x2 += game.width;


			if ( 
				game.grid[ x1, yDiff * 1 / 3 + yo ].territory != 0 && 
				game.playerList[ player ].foreignRelation[ game.grid[ x1, yDiff * 1 / 3 + yo ].territory - 1 ].politic != (byte)Form1.relationPolType.alliance &&
				game.grid[ x1, yDiff * 1 / 3 + yo ].territory - 1 != player 
				)
			{
				return true;
			}
			if ( 
				game.grid[ x2, yDiff * 2 / 3 + yo ].territory != 0 && 
				game.playerList[ player ].foreignRelation[ game.grid[ x2, yDiff * 2 / 3 + yo ].territory - 1 ].politic != (byte)Form1.relationPolType.alliance &&
				game.grid[ x2, yDiff * 2 / 3 + yo ].territory - 1 != player 
				)
			{
				return true;
			}
			return false;
		}
		#endregion

#region getFrontWith
		private Point getFrontWith( Point ori, byte player, byte playerDest )
		{
			Point[] possiblePnts = new Point[ 0 ];

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
				{
					if ( 
						game.grid[ x, y ].territory - 1 == player && 
						game.grid[ x, y ].continent == game.grid[ ori.X, ori.Y ].continent
						)
					{
						Point[] sqr = returnEmptySquare( ori.X, ori.Y, 1 );

						for ( int i = 0; i < sqr.Length; i ++ )
						{
							if ( game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 == playerDest )
							{
								Point[] buffer = possiblePnts;
								possiblePnts = new Point[ buffer.Length + 1 ];

								for ( int j = 0; j < buffer.Length; j ++ )
								{
									possiblePnts[ j ] = buffer[ j ];
								}

								possiblePnts[ buffer.Length + 1 ] = new Point( x, y );
								break;
							}
						}
					}
				}

			Random r = new Random();
			return possiblePnts[ r.Next( possiblePnts.Length ) ];
		}
		#endregion

#region isOkForExpand
		public  bool isOkForExpand( Point pos, byte player, int continent )
		{
			if ( game.grid[ pos.X, pos.Y ].continent == continent )
			{
				return true;
			}

		//	Radius radius = new Radius();
			Point[] sqr = game.radius.returnEmptySquare( pos.X, pos.Y, 1 );
			for ( int i = 0; i < sqr.Length; i ++ )
			{
				if ( game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 == player )
				{
					return true;
				}
			}
			return false;
		}
		#endregion

#region isNextTo
		public  bool isNextTo( Point ori, Point dest )
		{
			if ( ori.X == dest.X && ori.Y == dest.Y )
				return false;

			int xDiff = dest.X - ori.X, 
				yDiff = dest.Y - ori.Y;

			if ( xDiff < 0 )
				xDiff *= -1;

			if ( xDiff > game.width / 2 )
			{
				xDiff -= game.width;
			//	ori.X += game.width;
			}

			if ( xDiff < 0 )
				xDiff *= -1;

			if ( xDiff > 1 || xDiff < -1 )
				return false;

			if ( yDiff > 2 || yDiff < -2 )
				return false;

			if ( yDiff < 0 )
				yDiff *= -1;

			int Dest;
			if ( ori.Y % 2 == 1 )
			{
				if ( dest.X > ori.X || ( dest.X == 0 && ori.X == game.width - 1 ) )
					Dest = xDiff + yDiff / 2;
				else 
					Dest = xDiff + ( yDiff + 1 ) / 2;
			}
			else
			{
				if ( dest.X < ori.X || ( dest.X == game.width - 1 && ori.X == 0 ) )
					Dest = xDiff + yDiff / 2;
				else 
					Dest = xDiff + ( yDiff + 1 ) / 2;
			}

			if ( Dest == 1 /*|| Dest == -1*/ )
				return true;

			return false;
		}
#endregion

#region getDistWith
		public  int getDistWith( Point ori, Point dest )
		{
			if ( ori.X == dest.X && ori.Y == dest.Y )
				return 0;

			int xDiff = dest.X - ori.X, yDiff = dest.Y - ori.Y;

			if ( xDiff < 0 )
				xDiff *= -1;

			if ( yDiff < 0 )
				yDiff *= -1;

			if ( xDiff > game.width / 2 )
				xDiff -= game.width;
			
			if ( xDiff < 0 )
				xDiff *= -1;

			if ( ori.Y % 2 == 1 )
			{
				if ( dest.X > ori.X )
					return xDiff + yDiff / 2;
				else 
					return xDiff + ( yDiff + 1 ) / 2;
			}
			else
			{
				if ( dest.X < ori.X )
					return xDiff + yDiff / 2;
				else 
					return xDiff + ( yDiff + 1 ) / 2;
			}
		}
		
		public int getDistWith( Case ori, Case dest )
		{
			if ( ori.X == dest.X && ori.Y == dest.Y )
				return 0;

			int xDiff = dest.X - ori.X, yDiff = dest.Y - ori.Y;

			if ( xDiff < 0 )
				xDiff *= -1;

			if ( yDiff < 0 )
				yDiff *= -1;

			if ( xDiff > game.width / 2 )
				xDiff -= game.width;
			
			if ( xDiff < 0 )
				xDiff *= -1;

			if ( ori.Y % 2 == 1 )
			{
				if ( dest.X > ori.X )
					return xDiff + yDiff / 2;
				else 
					return xDiff + ( yDiff + 1 ) / 2;
			}
			else
			{
				if ( dest.X < ori.X )
					return xDiff + yDiff / 2;
				else 
					return xDiff + ( yDiff + 1 ) / 2;
			}
		}
		#endregion

#region regionInvalidForCity
		public  Point[] regionInvalidForCity( Point pos )
		{
			Point[] pnts = new Point[ 50 ]; 
			int pos1 = 0;

			for ( int i = 0; i <= 2; i ++ )
			{
				Point[] sqr = returnEmptySquare( pos.X, pos.Y, i );

				for ( int j = 0; j < sqr.Length; j ++ )
				{
					pnts[ pos1 ] = sqr[ j ];
					pos1 ++;
				}
			}

		//	Point[] sqr1 = returnEmptySquare( pos.X, pos.Y, 3 );

			if ( pos.Y % 2 == 0 )
			{
				pnts[ pos1 ] = new Point( pos.X + 1, pos.Y + 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 1, pos.Y + 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 2, pos.Y + 2 );
				pos1 ++;
				
				pnts[ pos1 ] = new Point( pos.X + 1, pos.Y - 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 1, pos.Y - 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 2, pos.Y - 2 );
				pos1 ++;

				pnts[ pos1 ] = new Point( pos.X - 1, pos.Y + 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 2, pos.Y + 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 2, pos.Y + 2 );
				pos1 ++;
				
				pnts[ pos1 ] = new Point( pos.X - 1, pos.Y - 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 2, pos.Y - 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 2, pos.Y - 2 );
				pos1 ++;
			}
			else
			{
				pnts[ pos1 ] = new Point( pos.X - 1, pos.Y + 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 1, pos.Y + 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 2, pos.Y + 2 );
				pos1 ++;
				
				pnts[ pos1 ] = new Point( pos.X - 1, pos.Y - 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 1, pos.Y - 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X - 2, pos.Y - 2 );
				pos1 ++;

				pnts[ pos1 ] = new Point( pos.X + 1, pos.Y + 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 2, pos.Y + 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 2, pos.Y + 2 );
				pos1 ++;
				
				pnts[ pos1 ] = new Point( pos.X + 1, pos.Y - 4 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 2, pos.Y - 3 );
				pos1 ++;
				pnts[ pos1 ] = new Point( pos.X + 2, pos.Y - 2 );
				pos1 ++;
			}

			/*
				if ( 
					!( sqr1[ j ].X == pos.X &&		sqr1[ j ].Y > pos.Y + 4 ) && 
					!( sqr1[ j ].X == pos.X &&		sqr1[ j ].Y < pos.Y - 4 ) && 
					!( sqr1[ j ].X > pos.X + 2 &&	sqr1[ j ].Y == pos.Y ) && 
					!( sqr1[ j ].X < pos.X + 2 &&	sqr1[ j ].Y == pos.Y )
					)
			*/

			for ( int j = pos1; j < pnts.Length; j ++ )
			{
				pnts[ j ] = new Point( -1, -1 );
			}

			return verifyPointFound( verifyLimits( pnts ) );
		}
		#endregion

#region caseOccupiedByRelationType
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="player"></param>
		/// <param name="relationType"> relation type to look for, relation in the player's memory</param>
		/// <returns></returns>
		public  bool caseOccupiedByRelationType( int x, int y, byte player, byte[] relationType )
		{
			for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++)
				if ( game.grid[ x, y ].stack[ i - 1 ].player.player != player )
					for ( int j = 0; j < relationType.Length; j ++ )
						if ( game.playerList[ player ].foreignRelation[ game.grid[ x, y ].stack[ i - 1 ].player.player ].politic == relationType[ j ] )
							return true;

			return false;
		}

		public  byte[] relationTypeListAllies = new byte[] { 
			(byte)Form1.relationPolType.alliance,
			(byte)Form1.relationPolType.Protected, 
			(byte)Form1.relationPolType.Protector
		};
		public  byte[] relationTypeListEnnemies = new byte[] { 
			(byte)Form1.relationPolType.war
		};
		public  byte[] relationTypeListNonAllies = new byte[] { 
			(byte)Form1.relationPolType.ceaseFire, 
			(byte)Form1.relationPolType.peace,
			(byte)Form1.relationPolType.war
		};
		public  byte[] relationTypeListAll = new byte[] {
			(byte)Form1.relationPolType.alliance, 
			(byte)Form1.relationPolType.ceaseFire, 
			(byte)Form1.relationPolType.peace,
			(byte)Form1.relationPolType.Protector,
			(byte)Form1.relationPolType.Protected, 
			(byte)Form1.relationPolType.war
		};
		public  byte[] relationTypeListWarAndCF = new byte[] {
			(byte)Form1.relationPolType.ceaseFire, 
			(byte)Form1.relationPolType.war
		};

		public  byte[] returnRelationTypeList( bool war, bool peace, bool ceaseFire, bool alliance, bool Protector, bool Protected )
		{
			byte[] n = new byte[ 10 ];
			int pos = 0;

			if ( war )
			{
				n[ pos ] = (byte)Form1.relationPolType.war;
				pos ++;
			}

			if ( peace )
			{
				n[ pos ] = (byte)Form1.relationPolType.peace;
				pos ++;
			}

			if ( ceaseFire )
			{
				n[ pos ] = (byte)Form1.relationPolType.ceaseFire;
				pos ++;
			}

			if ( alliance )
			{
				n[ pos ] = (byte)Form1.relationPolType.alliance;
				pos ++;
			}

			if ( Protector ) 
			{ 
				n[ pos ] = (byte)Form1.relationPolType.Protector; 
				pos ++; 
			} 

			if ( Protected ) 
			{ 
				n[ pos ] = (byte)Form1.relationPolType.Protected; 
				pos ++; 
			} 

			byte[] relationType = new byte[ pos ]; 
			for ( int i = 0; i < relationType.Length; i ++ ) 
				relationType[ i ] = n[ i ]; 

			return relationType; 
		}

		public  bool caseOccupiedByRelationType( int x, int y, byte player, bool war, bool peace, bool ceaseFire , bool alliance, bool Protector, bool Protected )
		{
			byte[] relationType = returnRelationTypeList( war, peace, ceaseFire , alliance, Protector, Protected );
			return caseOccupiedByRelationType( x, y, player, relationType );
		}
		#endregion

#region caseToMeetBoat
		public  Point caseToMeetBoat( byte player, Point unitOri, Point transOri )
		{
			int[] distList = new int[ game.height * game.width ];
			Point[] caseList = new Point[ game.height * game.width ];
			int pos = 0;

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					if ( 
						game.grid[ x, y ].continent == game.grid[ unitOri.X, unitOri.Y ].continent &&
						(
						game.grid[ x, y ].territory == 0 ||
						game.grid[ x, y ].territory - 1 == player ||
						game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
						game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
						game.playerList[ player ].foreignRelation[ game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war ||
						game.grid[ unitOri.X, unitOri.Y ].territory - 1 != player
						) &&
						isNextToWater( x, y )
						)
					{
						caseList[ pos ] = new Point( x, y );
						distList[ pos ] = getDistWith( new Point( x, y ), unitOri ) * 3 + getDistWith( new Point( x, y ), transOri );

						if ( game.grid[ x, y ].city > 0 )

						pos ++;
					}

			int[] distList1 = new int[ pos ];

			for ( int i = 0; i < distList1.Length; i ++ )
				distList1[ i ] = distList[ i ];

			int[] order = count.ascOrder( distList1 );

			for ( int i = 0; i < order.Length - 1; i ++ ) 
			{ 
				if ( 
					findWayTo( 
					unitOri, 
					caseList[ order[ i ] ], 
					1, 
					player, 
					false 
					)[ 0 ].X != - 1 
					&& 
					findWayTo( 
					transOri, 
					caseList[ order[ i ] ], 
					1, 
					player, 
					false 
					)[ 0 ].X != - 1 
					) 
				{ 
					return caseList[ order[ i ] ];
				} 
			}

			return new Point( -1, -1 );
		}
		#endregion

#region isNextToCity
		public bool isNextToCity( Point ori )
		{
			Point[] sqr = returnEmptySquare( ori, 1 );
			for ( int k = 0; k < sqr.Length; k ++ )
				if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 )
					return true;

			return false;
		}
#endregion
	}
}
