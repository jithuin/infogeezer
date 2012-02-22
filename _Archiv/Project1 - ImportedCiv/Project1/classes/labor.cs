using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for labor.
	/// </summary>
	public class labor
	{
		public static bool autoAddLabor( byte player, int city )
		{
			Point[] pos = findBestCase( player, city );
			if ( pos.Length > 0 )
			{
				addLaborAt( player, city, pos[ 0 ] );
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool autoRemoveLabor( byte player, int city, bool changeLabor )
		{
			int ind = findWorstLaboredCase( player, city );
			if ( ind != -1 )
			{
				removeLaborAt( player, city, Form1.game.playerList[ player ].cityList[ city ].laborPos[ ind ], changeLabor );
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void removeLaborAt( byte player, int city, Point pos, bool changeLabor )
		{
			int order = 0;
			for ( int i = 0; i < Form1.game.playerList[ player ].cityList[ city ].laborPos.Length; i ++ )
				if ( Form1.game.playerList[ player ].cityList[ city ].laborPos[ i ] == pos )
				{
					order = i;
					break;
				}

			Form1.game.playerList[ player ].cityList[ city ].laborOnField --;
				
			Form1.game.grid[ 
				Form1.game.playerList[ player ].cityList[ city ].laborPos[ order ].X,
				Form1.game.playerList[ player ].cityList[ city ].laborPos[ order ].Y 
				].laborOwner = 0;
			Form1.game.grid[ 
				Form1.game.playerList[ player ].cityList[ city ].laborPos[ order ].X, 
				Form1.game.playerList[ player ].cityList[ city ].laborPos[ order ].Y 
				].laborCity = 0;

			Point[] buffer = Form1.game.playerList[ player ].cityList[ city ].laborPos;
			Form1.game.playerList[ player ].cityList[ city ].laborPos = new Point[ buffer.Length - 1 ];

			for ( int i = 0, j = 0; i < buffer.Length; i ++ )
				if ( i != order )
				{
					Form1.game.playerList[ player ].cityList[ city ].laborPos[ j ] = buffer[ i ];
					j ++;
				}

			if ( changeLabor )
				laborChange( player, city );
		//	Form1.game.playerList[ player ].cityList[ city ].invalidateLastTrade();
		}

		public static void addLaborAt( byte player, int city, Point pos )
		{
			Point[] buffer = Form1.game.playerList[ player ].cityList[ city ].laborPos;
			Form1.game.playerList[ player ].cityList[ city ].laborPos = new Point[ buffer.Length + 1 ];

			for ( int i = 0; i < buffer.Length; i ++ )
				Form1.game.playerList[ player ].cityList[ city ].laborPos[ i ] = buffer[ i ];

			Form1.game.playerList[ player ].cityList[ city ].laborOnField ++;
			Form1.game.playerList[ player ].cityList[ city ].laborPos[ Form1.game.playerList[ player ].cityList[ city ].laborOnField - 1 ] = pos;
			
			Form1.game.grid[ pos.X, pos.Y ].laborOwner = player;
			Form1.game.grid[ pos.X, pos.Y ].laborCity = city;

			laborChange( player, city );
		/*	Form1.game.playerList[ player ].cityList[ city ].invalidateLastTrade();
			Form1.game.playerList[ player ].cityList[ city ].setHasDirectAccessToRessource();	*/
		}

		public static bool tryToAddLaborAt( byte player, int city, Point pos )
		{
			if( 
				( Form1.game.grid[ pos.X,  pos.Y ].territory - 1 == player ||
				Form1.game.grid[ pos.X,  pos.Y ].type == (byte)enums.terrainType.sea ) && 
				Form1.game.grid[ pos.X,  pos.Y ].city == 0 && 
				Form1.game.grid[ pos.X,  pos.Y ].laborCity == 0 
				)
			{
				addLaborAt( player, city, pos );
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void removeAllLabor( byte player, int city )
		{
			for ( int i = 0; i < Form1.game.playerList[ player ].cityList[ city ].laborPos.Length; i ++ )
			{
				Form1.game.grid[ 
					Form1.game.playerList[ player ].cityList[ city ].laborPos[ i ].X,
					Form1.game.playerList[ player ].cityList[ city ].laborPos[ i ].Y 
					].laborOwner = 0;
				Form1.game.grid[ 
					Form1.game.playerList[ player ].cityList[ city ].laborPos[ i ].X, 
					Form1.game.playerList[ player ].cityList[ city ].laborPos[ i ].Y 
					].laborCity = 0;
			}

			Form1.game.playerList[ player ].cityList[ city ].laborOnField = 0;

			Form1.game.playerList[ player ].cityList[ city ].laborPos = new Point[ 0 ];

			laborChange( player, city );
		}

		/// <summary>
		/// Add all labor possible
		/// </summary>
		/// <param name="player">City owner</param>
		/// <param name="city">City</param>
		public static void addAllLabor( byte player, int city )
		{
			removeAllLabor( player, city );
			
			Point[] pos = findBestCase( player, city );

			for ( int i = 0; i < Form1.game.playerList[ player ].cityList[ city ].population; i ++ )
			{
				if ( pos.Length > i )
					addLaborAt( player, city, pos[ i ] );
				else
					break;
			}
		}

	#region find best cases
		public static Point[] findBestCase( byte owner, int city )
		{
			int x1 = Form1.game.playerList[ owner ].cityList[ city ].X;
			int y1 = Form1.game.playerList[ owner ].cityList[ city ].Y;
			
//		//	Radius radius = new Radius();
			Point[] pntCovered = Form1.game.radius.returnCityRadius(
				Form1.game.playerList[ owner ].cityList[ city ].X, 
				Form1.game.playerList[ owner ].cityList[ city ].Y 
				);

			int[] caseValue = new int[ pntCovered.Length ];
			int nbrValidCases = 0;

			for ( int i = 0; i < pntCovered.Length ; i ++ )
				if( 
					(
					Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].territory - 1 == owner ||
					Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].type == (byte)enums.terrainType.sea 
					) && 
					Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].city == 0 && 
					Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].laborCity == 0 &&
					Form1.game.playerList[ owner ].discovered[ pntCovered[ i ].X,  pntCovered[ i ].Y ]
					)
				{
					caseValue[ i ] = 
						/*Form1.game.playerList[ owner ].preferences.laborProd **/ (int)((getPFT.getCaseProd( pntCovered[ i ].X, pntCovered[ i ].Y ) + 
					/*Form1.game.playerList[ owner ].preferences.laborFood **/ getPFT.getCaseFood( pntCovered[ i ].X, pntCovered[ i ].Y ) )* 1.5 + 
						/*Form1.game.playerList[ owner ].preferences.laborTrade **/ getPFT.getCaseTrade( pntCovered[ i ].X, pntCovered[ i ].Y ));

					nbrValidCases ++;
				}
				else
				{
					caseValue[ i ] = -1;
				}

			int[] order = count.descOrder( caseValue ); 
			Point[] pntCovered2 = new Point[ nbrValidCases ]; 
			for ( int i = 0; i < pntCovered2.Length; i ++ ) 
				pntCovered2[ i ] = pntCovered[ order[ i ] ]; 

			return pntCovered2;
		}
		#endregion

	#region find case to abandon
		public static int findWorstLaboredCase( byte owner, int city )
		{
			getPFT gpft = new getPFT();

			int x1 = Form1.game.playerList[ owner ].cityList[ city ].X;
			int y1 = Form1.game.playerList[ owner ].cityList[ city ].Y;
			
	//	//	Radius radius = new Radius();
			Point[] pntCovered = new Point[ Form1.game.playerList[ owner ].cityList[ city ].laborOnField ];

			if ( pntCovered.Length != 0 )
			{
				for ( int i = 0; i < pntCovered.Length; i ++ )
				{
					pntCovered[ i ] = Form1.game.playerList[ owner ].cityList[ city ].laborPos[ i ];
				}

				double[] caseValue = new double[ pntCovered.Length ];
			
				for ( int i = 0; i < pntCovered.Length ; i ++ )
				{
					/*	if( 
							( Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].territory - 1 == owner ||
							Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].type == (byte)enums.terrainType.sea ) && 
							Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].city == 0 && 
							Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].laborCity == 0 
							)
						{*/
					caseValue[ i ] = 
						Form1.game.playerList[ owner ].preferences.laborProd * getPFT.getCaseProd( pntCovered[ i ].X, pntCovered[ i ].Y ) + 
						Form1.game.playerList[ owner ].preferences.laborFood * getPFT.getCaseFood( pntCovered[ i ].X, pntCovered[ i ].Y ) + 
						Form1.game.playerList[ owner ].preferences.laborTrade * getPFT.getCaseTrade( pntCovered[ i ].X, pntCovered[ i ].Y );

					if ( Form1.game.radius.isNextToIrrigation( pntCovered[ i ].X,  pntCovered[ i ].Y ) )
						caseValue[ i ] += 3;
					/*	}
						else
							caseValue[ i ] = -1;*/
				}

				int caseChoose = 0;

				for ( int i = 1; i < pntCovered.Length; i++ )
				{
					if ( caseValue[ caseChoose ] > caseValue[ i ] )
						caseChoose = i ;
				}

				if ( caseValue[ caseChoose ] >= 0 )
					return caseChoose;
				else
					return -1;
			}
			return -1;
		}
		#endregion

		private static void laborChange( byte player, int city )
		{
			int nbr = Form1.game.playerList[ player ].cityList[ city ].population - Form1.game.playerList[ player ].cityList[ city ].laborOnField - Form1.game.playerList[ player ].cityList[ city ].nonLabor.total;
			if ( nbr > 0 )
				Form1.game.playerList[ player ].cityList[ city ].nonLabor.add( nbr );
			else if ( nbr < 0 )
			{
				// remove all from non labor and some from labor
				nbr *= -1;
				int toRemoveFromLabor = 0;

				if ( nbr > Form1.game.playerList[ player ].cityList[ city ].nonLabor.total )
				{
					toRemoveFromLabor = nbr - Form1.game.playerList[ player ].cityList[ city ].nonLabor.total;
					nbr = Form1.game.playerList[ player ].cityList[ city ].nonLabor.total;

					// remove from labor
					for ( int i = 0; i < toRemoveFromLabor; i ++ )
						autoRemoveLabor( player, city, false );
				}

				Form1.game.playerList[ player ].cityList[ city ].nonLabor.remove( nbr );
			}

			Form1.game.playerList[ player ].cityList[ city ].invalidateLastTrade();
			Form1.game.playerList[ player ].cityList[ city ].setHasDirectAccessToRessource();
		}

	}
}
