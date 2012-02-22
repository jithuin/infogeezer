using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary> 
	/// Summary description for aiTransport. 
	/// </summary> 
	public class aiTransport 
	{ 
		
#region whereToDisembarkMilitaryUnits

		public static Point whereToDisembarkMilitaryUnits( byte player, int unit )
		{
			return whereToDisembarkMilitaryUnits( player, new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ) );
		}
		
		/// <summary> 
		/// Return case on land 
		/// </summary> 
		/// <param name="player"></param> 
		/// <param name="pos"></param> 
		/// <returns></returns> 
		public static Point whereToDisembarkMilitaryUnits( byte player, Point pos ) 
		{ 
			int posInt = 0;
			int[] values = new int[ 20 ];
			Point[] cases = new Point[ 20 ];

			for ( int rad = 1; rad < Form1.game.width * 2 /*&&*/ /*pos < 20*/; rad ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( pos, rad );

				for ( int k = 0; k < sqr.Length; k ++ ) 
					if ( 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent > 0 && 
						!( // faux 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory > 0 &&
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 && 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player && 
						( 
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire
						) 
						) && 
						Form1.game.radius.isNextToWater( sqr[ k ].X, sqr[ k ].Y ) && 
						!Form1.game.radius.caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, true, true, true, false, false, false )
						) 
					{ 
						cases[ posInt ] = sqr[ k ];
					//	values[ posInt ] = 100;

						if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory > 0 )
							if ( 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 != player && 
								Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
								) 
							{ 
								Point[] smallSqr = Form1.game.radius.returnEmptySquare(  sqr[ k ], 1 ); 
								bool isNextToCity = false; 
								for ( int h = 0; h < smallSqr.Length; h ++ ) 
									if ( Form1.game.grid[ smallSqr[ h ].X, smallSqr[ h ].Y ].city > 0 ) 
									{ 
										isNextToCity = true; 
										break; 
									} 

								if ( isNextToCity ) 
									values[ posInt ] += 20; 
								else 
								{ 
									smallSqr = Form1.game.radius.returnEmptySquare( sqr[ k ], 2 ); 
								
									for ( int h = 0; h < smallSqr.Length; h ++ ) 
										if ( Form1.game.grid[ smallSqr[ h ].X, smallSqr[ h ].Y ].city > 0 ) 
										{ 
											isNextToCity = true; 
											break; 
										} 
								
									if ( isNextToCity ) 
										values[ posInt ] += 12; 
								} 
							}
							else if ( 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player || 
								Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance  || 
								Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected  || 
								Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector 
								) 
								if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 ) 
									values[ posInt ] += 10; 

						values[ posInt ] -= rad; 
						posInt ++; 

						if ( posInt == 20 ) 
						{ 
							int[] order = count.descOrder( values ); 

							for ( int i = 0; i < order.Length; i ++ ) 
								if ( Form1.game.radius.findWayTo( pos, cases[ order[ i ] ], 0, player, false )[ 0 ].X != -1 ) 
									return cases[ order[ i ] ]; 

							posInt = 0; 
						} 
					} 
			}

			int[] values1 = new int[ posInt ]; 
			for ( int i = 0; i < values1.Length; i ++ )
				values1[ i ] = values[ i ];

			int[] order1 = count.descOrder( values1 ); 

			for ( int i = 0; i < order1.Length; i ++ ) 
				if ( Form1.game.radius.findWayTo( pos, cases[ order1[ i ] ], 0, player, false )[ 0 ].X != -1 ) 
					return cases[ order1[ i ] ]; 

			return new Point( -1, -1 );
		}
#endregion
	
#region whereToDisembarkSettler
		public static Point whereToDisembarkSettler( byte player, int unit )
		{
			return whereToDisembarkSettler( player, new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ) );
		}

		public static Point whereToDisembarkSettler( byte player, Point pos )
		{
			return pos;
		}
#endregion

	} 
} 
 