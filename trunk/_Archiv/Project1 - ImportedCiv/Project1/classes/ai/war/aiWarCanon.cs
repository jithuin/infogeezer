using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiWarCanon.
	/// </summary>
	public class aiWarCanon
	{
		public static Point whereToMoveCanon( byte player, int unit )
		{
			return whereToMoveCanon( 
				player, 
				Form1.game.playerList[ player ].unitList[ unit ].pos, 
				Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].range 
				);
		}
		public static Point whereToMoveCanon( byte player, Point ori, int range )
		{
			int pos = 0;
			int[] values = new int[ 10 ];
			Point[] attackSpot = new Point[ values.Length ];

			int[,] costs = Form1.game.radius.gridCost( ori, 1, player, true, 8 );

			for ( int r = 1; r < 10 && pos < values.Length; r ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( ori, r );
				for ( int k = 0; k < sqr.Length && pos < values.Length; k ++ )
				{
					int caseValue = aiWarBomber.caseValueToBombard( player, sqr[ k ].X, sqr[ k ].Y );
					if ( caseValue > 0 )
					{
						Point[] sqr0 = Form1.game.radius.returnSquare( sqr[ k ], range );
						int[] caseValues = new int[ sqr0.Length ];

						for ( int k0 = 0; k0 < sqr0.Length; k0 ++ )
							if ( 
								costs[ sqr0[ k0 ].X, sqr0[ k0 ].Y ] >= 0 &&
								!(
								sqr0[ k0 ].X == sqr[ k ].X &&
								sqr0[ k0 ].Y == sqr[ k ].Y
								)
								)
								caseValues[ k0 ] = costs[ sqr0[ k0 ].X, sqr0[ k0 ].Y ];
							else
								caseValues[ k0 ] = 10000;

						int[] order = count.ascOrder( caseValues );
						if ( caseValues[ order[ 0 ] ] < 10000 )
						{
							int whereInd = pos;
							pos ++;
							for ( int i = 0; i < pos; i ++ ) 
								if ( 
									attackSpot[ i ].X == sqr0[ order[ 0 ] ].X && 
									attackSpot[ i ].Y == sqr0[ order[ 0 ] ].Y
									)
								{
									whereInd = i;
									pos --;
									break;
								}

							if ( values[ whereInd ] > 3 )
								values[ whereInd ] += caseValue;
							else
								values[ whereInd ] = caseValue - 3 * caseValues[ order[ 0 ] ] + aiWar.totDefensesAt( player, sqr0[ order[ 0 ] ] );

							attackSpot[ whereInd ] = sqr0[ order[ 0 ] ];
							//	pos ++;
						}
					}
				}
			}

			if ( pos > 0 )
			{
				int[] order1 = count.descOrder( values );

				return attackSpot[ order1[ 0 ] ];
			}
			else
				return new Point( -1, -1 );
		}
	}
}
