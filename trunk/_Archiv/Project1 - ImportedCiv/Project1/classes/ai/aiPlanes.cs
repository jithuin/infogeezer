using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiPlanes.
	/// </summary>
	public class aiPlanes
	{

#region findWayToBase
		public static Point[] findWayToBase( byte player, int range, int cityOrigine, int cityDest ) 
		{ 
			return findWayToBase( 
				player, 
				range, 
				new Point( Form1.game.playerList[ player ].cityList[ cityOrigine ].X, Form1.game.playerList[ player ].cityList[ cityOrigine ].Y ),
				new Point( Form1.game.playerList[ player ].cityList[ cityDest ].X, Form1.game.playerList[ player ].cityList[ cityDest ].Y )
				);
		}

		public static Point[] findWayToBase( byte player, int range, Point ori, Point dest ) 
		{ 
			if ( Form1.game.radius.getDistWith( ori, dest ) <= range )
				return new Point[] { dest };

			structures.Pnt[] tempPntList = new xycv_ppc.structures.Pnt[ Form1.game.width * Form1.game.height ];
			int tot = 0;
			byte[] pols = Form1.game.radius.relationTypeListNonAllies;//.returnRelationTypeList( true, true, true, false, false, false ); 
			int destInd = 0, oriInd = 0;

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( x == ori.X && y == ori.Y )
					{
						oriInd = tot;
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
					else if ( x == dest.X && y == dest.Y )
					{
						destInd = tot;
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
					else if ( 
						Form1.game.playerList[ player ].discovered[ x, y ] && 
						( 
						Form1.game.grid[ x, y ].militaryImprovement == (byte)enums.militaryImprovement.airport ||
						Form1.game.grid[ x, y ].city > 0
						) &&
						( 
						Form1.game.grid[ x, y ].territory > 0 &&
						( 
						Form1.game.grid[ x, y ].territory - 1 == player ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
						)&& 
						!Form1.game.radius.caseOccupiedByRelationType( x, y, player, pols )
						) 
						) 
					{
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
			
			structures.Pnt[] pnts = new xycv_ppc.structures.Pnt[ tot ];
			int[] prevPnt = new int[ tot ];
			int[] order = new int[ tot ];

			for ( int i = 0; i < tot; i ++ )
			{
				pnts[ i ] = tempPntList[ i ];
				order[ i ] = -1;
			}

			order[ oriInd ] = 0; 
			bool mod = true;

			for ( int rad = 1; mod; rad ++ ) 
			{
				mod = false;

				for ( int i = 0; i < tot; i ++ ) 
					if ( order[ i ] == rad - 1 ) // used 
						for( int j = 0; j < tot; j ++ ) 
							if ( i != j && order[ j ] == -1 ) // &&
								/*	range >= game.radius.getDistWith( 
									new Point( pnts[ i ].X, pnts[ i ].Y ), 
									new Point( pnts[ j ].X, pnts[ j ].Y ) 
									)
								) */
								// different, unused & in range
								{
									int dw = Form1.game.radius.getDistWith( new Point( pnts[ i ].X, pnts[ i ].Y ), new Point( pnts[ j ].X, pnts[ j ].Y ) );

									if ( range >= dw )
									{ 
										order[ j ] = rad; 
										prevPnt[ j ] = i; 
										mod = true;

										if ( j == destInd ) 
										{ // yeah 
											Point[] way = new Point[ rad ]; 
											int[] wayInt = new int[ rad ]; 
											way[ way.Length - 1 ] = dest; 
											wayInt[ way.Length - 1 ] = destInd; 

											for ( int c = way.Length - 1; c > 0; c -- ) 
											{
												wayInt[ c - 1 ] = prevPnt[ wayInt[ c ] /*- 1*/ ];
												way[ c - 1 ] = new Point( 
													pnts[ prevPnt[ wayInt[ c ] ] ].X, 
													pnts[ prevPnt[ wayInt[ c ] ] ].Y 
													);
											}

											return way;
										} 
									}
								}
			}
		
			return new Point[] { new Point( -1, -1 ) };					
		}
		#endregion

#region costGrid
		public static int[,] costGrid( byte player, int range, Point ori ) 
		{ 
			structures.Pnt[] tempPntList = new xycv_ppc.structures.Pnt[ Form1.game.width * Form1.game.height ];
			int tot = 0;
			byte[] pols = Form1.game.radius.relationTypeListNonAllies; 
			int oriInd = 0;

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.width; y ++ )
					if ( x == ori.X && y == ori.Y )
					{
						oriInd = tot;
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
					else if ( 
						Form1.game.playerList[ player ].discovered[ x, y ] &&
						( 
						Form1.game.grid[ x, y ].territory > 0 &&
						( 
						Form1.game.grid[ x, y ].territory - 1 == player ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
						) && 
						( 
						Form1.game.grid[ x, y ].militaryImprovement == (byte)enums.militaryImprovement.airport ||
						Form1.game.grid[ x, y ].city > 0
						) && 
						!Form1.game.radius.caseOccupiedByRelationType( x, y, player, pols )
						) 
						) 
					{
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
			
			structures.Pnt[] pnts = new xycv_ppc.structures.Pnt[ tot ];
			int[] prevPnt = new int[ tot ];
			int[] order = new int[ tot ];

			for ( int i = 0; i < tot; i ++ )
			{
				pnts[ i ] = tempPntList[ i ];
				order[ i ] = -1;
			}

			order[ oriInd ] = 0; 
			bool mod = true;

			for ( int rad = 1; mod; rad ++ ) 
			{
				mod = false;

				for ( int i = 0; i < tot; i ++ ) 
					if ( order[ i ] == rad - 1 ) // used 
						for( int j = 0; j < tot; j ++ ) 
							if ( 
								i != j &&
								order[ j ] == -1 &&
								range >= Form1.game.radius.getDistWith( new Point( pnts[ i ].X, pnts[ i ].Y ), new Point( pnts[ j ].X, pnts[ j ].Y ) )
								) // different, unused & in range
							{ 
								order[ j ] = rad; 
								prevPnt[ j ] = i; 
								mod = true;
							}
			}

            int[,] costs = new int[ Form1.game.width, Form1.game.height ];

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.width; y ++ )
					costs[ x, y ] = -1;
		
			for ( int i = 0; i < tot; i ++ )
				costs[ pnts[ i ].X, pnts[ i ].Y ] = order [ i ];

			return costs;
		}
		#endregion

#region costList
		public static structures.pntWithDist[] costList( byte player, int range, Point ori ) 
		{ 
			structures.Pnt[] tempPntList = new xycv_ppc.structures.Pnt[ Form1.game.width * Form1.game.height ];
			int tot = 0;
			byte[] pols = Form1.game.radius.relationTypeListNonAllies; 
			int oriInd = 0;

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.width; y ++ )
					if ( x == ori.X && y == ori.Y )
					{
						oriInd = tot;
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
					else if ( 
						Form1.game.playerList[ player ].discovered[ x, y ] &&
						( 
						Form1.game.grid[ x, y ].territory > 0 &&
						( 
						Form1.game.grid[ x, y ].territory - 1 == player ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
						) && 
						( 
						Form1.game.grid[ x, y ].militaryImprovement == (byte)enums.militaryImprovement.airport ||
						Form1.game.grid[ x, y ].city > 0
						) && 
						!Form1.game.radius.caseOccupiedByRelationType( x, y, player, pols )
						) 
						) 
					{
						tempPntList[ tot ].X = x;
						tempPntList[ tot ].Y = y;
						tot ++;
					}
			
			structures.Pnt[] pnts = new xycv_ppc.structures.Pnt[ tot ];
			int[] prevPnt = new int[ tot ];
			int[] order = new int[ tot ];

			for ( int i = 0; i < tot; i ++ )
			{
				pnts[ i ] = tempPntList[ i ];
				order[ i ] = -1;
			}

			order[ oriInd ] = 0; 
			bool mod = true;

			for ( int rad = 1; mod; rad ++ ) 
			{
				mod = false;

				for ( int i = 0; i < tot; i ++ ) 
					if ( order[ i ] == rad - 1 ) // used 
						for( int j = 0; j < tot; j ++ ) 
							if ( 
								i != j &&
								order[ j ] == -1 &&
								range >= Form1.game.radius.getDistWith( new Point( pnts[ i ].X, pnts[ i ].Y ), new Point( pnts[ j ].X, pnts[ j ].Y ) )
								) // different, unused & in range
							{ 
								order[ j ] = rad; 
								prevPnt[ j ] = i; 
								mod = true;
							}
			}

			bool[] valid = new bool[ tot ];
			int tot1 = 0;
			
			for ( int i = 0; i < tot; i ++ )
				if ( order[ i ] != -1 )
				{
					valid[ i ] = true;
					tot1 ++;
				}
		
			structures.pntWithDist[] costs = new xycv_ppc.structures.pntWithDist[ tot1 ];

			int pos = 0; 
			for ( int i = 0; i < tot; i ++ ) 
				if ( valid[ i ] ) 
				{ 
					costs[ pos ].dist = order[ i ]; 
					costs[ pos ].X = pnts[ i ].X; 
					costs[ pos ].Y = pnts[ i ].Y; 
					pos ++; 
				} 

			return costs;
		}
		#endregion

	}
}
