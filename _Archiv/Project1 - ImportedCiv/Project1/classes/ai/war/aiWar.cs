using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiWar.
	/// </summary>
	public class aiWar
	{

#region hasCommonFrontier
		public static bool hasCommonFrontier( byte player, byte other, int continent )
		{
		//	Radius radius = new Radius();

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( Form1.game.grid[ x, y ].territory - 1 == player && ( Form1.game.grid[ x, y ].continent == continent || continent == -1 ) )
					{
						Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );

						for ( int j = 0; j < sqr.Length; j ++ )
							if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == other )
							{
								return true;
							}
					}

			return false;
		}
		#endregion

#region returnDestToAttack
		public static Point returnDestToAttack( byte player, byte ennemy, int unit )
		{
			Point pos = new Point(
				Form1.game.playerList[ player ].unitList[ unit ].X, 
				Form1.game.playerList[ player ].unitList[ unit ].Y 
				);
			byte continent = Form1.game.grid[ pos.X, pos.Y ].continent;

			Point[] tempList = new Point[ Form1.game.width * Form1.game.height ];
			int k = 0;
		//	Radius radius = new Radius();

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( Form1.game.grid[ x, y ].continent == continent )
						if ( Form1.game.grid[ x, y ].territory - 1 == player )
						{
							Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );

							for ( int j = 0; j < sqr.Length; j ++ )
								if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == ennemy )
								{
									tempList[ k ] = new Point( x, y );
									k ++;
									break;
								}
						}

			Random r = new Random();
			return tempList[ r.Next( k ) ];
		}
		#endregion

#region returnLandFrontierWith
		public static Point[] returnLandFrontierWith( byte player, byte other, int continent )
		{
			Point[] tempList = new Point[ Form1.game.width * Form1.game.height ];
			int k = 0;
		//	Radius radius = new Radius();

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( Form1.game.grid[ x, y ].territory - 1 == player && ( Form1.game.grid[ x, y ].continent == continent || continent == -1 ) )
					{
						Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );

						for ( int j = 0; j < sqr.Length; j ++ )
							if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == other )
							{
								tempList[ k ] = new Point( x, y );
								k ++;
								break;
							}
					}

			Point[] retour = new Point[ k ];

			for ( int i = 0; i < retour.Length; i ++ )
			{
				retour[ i ] = tempList[ i ];
			}

			return retour;
		}
		#endregion

#region returnCoastWith
		public static Point[] returnCoastWith( byte player, byte other )
		{
			Point[] tempList = new Point[ Form1.game.width * Form1.game.height ];
			int k = 0;
		//	Radius radius = new Radius();

			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( Form1.game.grid[ x, y ].territory - 1 == player && Form1.game.grid[ x, y ].continent > 0 )
					{
						Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );

						for ( int j = 0; j < sqr.Length; j ++ )
							if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == other )
							{
								tempList[ k ] = new Point( x, y );
								k ++;
								break;
							}
					}

			Point[] retour = new Point[ k ];

			for ( int i = 0; i < retour.Length; i ++ )
			{
				retour[ i ] = tempList[ i ];
			}

			return retour;
		}
		#endregion

#region returnLandFrontierToAttack
		public static Point returnLandFrontierToAttack( byte player, byte other, Point pos ) 
		{ 
			Point[] frontier = returnLandFrontierWith( player, other, Form1.game.grid[ pos.X, pos.Y ].continent ); 
 
			if ( frontier.Length > 0 )
			{
				Random r = new Random();

				return frontier[ r.Next( frontier.Length ) ];
			}
			else
			{
				return new Point( -1, -1 );
			}
		} 
		
	/*	public static Point returnLandFrontierToAttack( byte player, Point pos ) 
		{ 
			Point[][] fronts = new Point[ Form1.game.playerList.Length ][];

			for ( int i = 0; i < fronts.Length; i ++ )
				if ( 
					player != i &&
					Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war
					)
					fronts[ i ] = returnLandFrontierWith( player, i, Form1.game.grid[ pos.X, pos.Y ].continent );
			

			Point[] frontier = returnLandFrontierWith( player, other, Form1.game.grid[ pos.X, pos.Y ].continent ); 
 
			Random r = new Random(); 
			return frontier[ r.Next( frontier.Length ) ]; 
		} */
		#endregion

#region whichEnnemyToAttack
		public static byte whichEnnemyToAttack( byte player, Point pos ) 
		{
			int[] dist = new int[ Form1.game.playerList.Length ];
			for ( int i = 0; i < Form1.game.playerList.Length; i ++ )
				if ( 
					Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war && 
					i != player 
					) 
				{ 
					dist[ i ] = -1;

					for ( int j = 1; j <= Form1.game.playerList[ player ].cityNumber; j ++ )
					{
						int temp = Form1.game.radius.getDistWith( pos, new Point( Form1.game.playerList[ player ].cityList[ j ].X, Form1.game.playerList[ player ].cityList[ j ].Y ) );

						if ( temp < dist[ i ] || dist[ i ] == -1 )
							dist[ i ] = temp;
					}
				} 
				else 
				{ 
					dist[ i ] = -1;
				} 

			byte nearestPlayer = 0;

			for ( byte i = 1; i < Form1.game.playerList.Length; i ++ )
				if ( 
					dist[ i ] < dist[ nearestPlayer ] && 
					dist[ i ] != -1 
					)
					nearestPlayer = i;

			if ( dist[ nearestPlayer ] != -1 )
				return nearestPlayer;
			else
				return player;
		}
		#endregion

#region findTransportShip
		public static int findTransportShip( byte player, Point pos )
		{
			int[] transportDist = new int[ Form1.game.playerList[ player ].unitNumber ];
			int[] transportNbr = new int[ Form1.game.playerList[ player ].unitNumber ];
			int arrPos = 0;

			for ( int unit = 1; unit <= Form1.game.playerList[ player ].unitNumber; unit ++ )
				if ( 
					Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].transport > 0 &&
					Form1.game.playerList[ player ].unitList[ unit ].state != (byte)Form1.unitState.dead 
					)
				{
					transportDist[ arrPos ] = Form1.game.radius.getDistWith( 
						pos,
						new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y )
						);
					transportNbr[ arrPos ] = unit;
					arrPos ++;
				}

			bool mod = true;
			while ( mod )
			{
				mod = false;

				for ( int i = 1; i < arrPos; i ++ )
					if ( transportDist[ i - 1 ] > transportDist[ i ] )
					{
						int buffer = transportDist[ i - 1 ];
						transportDist[ i - 1 ] = transportDist[ i ];
						transportDist[ i ] = buffer;

						buffer = transportNbr[ i - 1 ];
						transportNbr[ i - 1 ] = transportNbr[ i ];
						transportNbr[ i ] = buffer;
						mod = true;
					}
			}

			if ( transportDist[ 0 ] != -1 && transportDist[ 0 ] < Form1.game.width / 2 )
			{
				return transportNbr[ 0 ];
			}
			else
			{
			}

			return -1;
		}
		#endregion

#region removeAllUnitFromTerritory
		public static void removeAllUnitFromTerritory( byte territoryOwner, byte intruder )
		{
			bool found = true;
			for ( int unit = 1; unit <= Form1.game.playerList[ intruder ].unitNumber; unit ++ )
				if ( 
					Form1.game.playerList[ intruder ].unitList[ unit ].state != (byte)Form1.unitState.dead && 
					Form1.game.grid[ Form1.game.playerList[ intruder ].unitList[ unit ].X, Form1.game.playerList[ intruder ].unitList[ unit ].Y ].territory - 1 == territoryOwner
					)
				{
					found = false;
					for ( int rad = 1; rad < 4 && !found; rad ++ )
					{
						Point[] sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ intruder ].unitList[ unit ].X, Form1.game.playerList[ intruder ].unitList[ unit ].Y, rad );

						for ( int k = 0; k < sqr.Length; k ++ )
							if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == intruder )
							{
								found = true;
								break;
							}
							else if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory > 0 )
								if (
									Form1.game.playerList[ intruder ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance ||
									Form1.game.playerList[ intruder ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected
									)
								{
									found = true;
									break;
								}
					}

					if ( !found )
					{
					//	st5ru nc = locateNearestAlliedCity( intruder, new Point( Form1.game.playerList[ intruder ].unitList[ unit ].X, Form1.game.playerList[ intruder ].unitList[ unit ].Y ) );
						
					}
				}
		
		}
		#endregion

#region locateNearestAlliedCity
		public static structures.intByte locateNearestAlliedCity( byte owner, Point pos )
		{
			structures.intByte retour = new structures.intByte();
			int lastDist = -1;

			for ( int player = 0; player < Form1.game.playerList.Length; player ++ )
				if ( 
					player == owner || 
					Form1.game.playerList[ owner ].foreignRelation[ player ].politic == (byte)Form1.relationPolType.alliance || 
					Form1.game.playerList[ owner ].foreignRelation[ player ].politic == (byte)Form1.relationPolType.Protected
					)
					for ( int city = 1; city <= Form1.game.playerList[ player ].cityNumber; city ++ )
						if ( Form1.game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
						{
							int dist = Form1.game.radius.getDistWith( pos, new Point( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ) );
							if ( lastDist == -1 || dist < lastDist )
							{
								retour.type = (byte)player;
								retour.info = city;
							}
						}

			return retour;
		}
		#endregion

#region strongestEnnemy
		public static int strongestEnnemy( Point pos, byte attacker )
		{
			int x = pos.X, y = pos.Y;
			int retour = 0;
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++)
			{
				if (Form1.game.playerList[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].foreignRelation[ attacker ].politic == (byte)Form1.relationPolType.war)
					if ( retour == 0)
					{
						retour = i;
					}
					else if ( 
						Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.defense * 
						Form1.game.grid[ x, y ].stack[ i - 1 ].health >
						Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.defense * 
						Form1.game.grid[ x, y ].stack[ i - 1 ].health
						)
					{
						retour = i;
					}
			}

			return retour;
		}
#endregion

#region caseValidToAttack
		public static bool caseValidToAttack( Point pos, byte player )
		{
			int x = pos.X, y = pos.Y; 
			if ( 
				Form1.game.grid[ x, y ].city > 0 &&
				Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
				)
				return true;

			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i ++ ) 
				if ( Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].politic == (byte)Form1.relationPolType.war )
					return true;

			return false;
		}
		#endregion

#region findEnnemyToAttack

	/*	public static Point findEnnemyToAttack( byte player, Point pos )
		{
			for ( int rad = 1; rad < 15; rad ++ )
			{
				Point[] sqr = game.radius.returnEmptySquare( pos.X, pos.Y, rad );

				for ( int k = 0; k < sqr.Length; k ++ )
					if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == cont )
						if ( caseValidToAttack( sqr[ k ], player ) )
							return sqr[ k ];
			}

			for ( int rad = 1; rad < 15; rad ++ )
			{
				Point[] sqr = game.radius.returnEmptySquare( pos.X, pos.Y, rad );

				for ( int k = 0; k < sqr.Length; k ++ )
					if ( 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent > 0 && 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent != cont 
						)
						if ( caseValidToAttack( sqr[ k ], player ) )
							return sqr[ k ];
			}

			return returnLandFrontierToAttack( 
				player, 
				whichEnnemyToAttack( 
					player, 
					new Point( 
					Form1.game.playerList[ player ].unitList[ unit ].X, 
					Form1.game.playerList[ player ].unitList[ unit ].Y 
					) 
					), 
				new Point( 
				Form1.game.playerList[ player ].unitList[ unit ].X, 
				Form1.game.playerList[ player ].unitList[ unit ].Y 
				) 
				); 
		}*/

		public static Point findEnnemyToAttack( byte player, int unit )
		{
			return findEnnemyToAttack( player, unit, Form1.game.grid[ Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ].continent );
		}

		public static Point findEnnemyToAttack( byte player, int unit, int cont )
		{
			Point pos = new Point( 
				Form1.game.playerList[ player ].unitList[ unit ].X, 
				Form1.game.playerList[ player ].unitList[ unit ].Y 
				);

			for ( int rad = 1; rad < Form1.game.width; rad ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( pos.X, pos.Y, rad );

				for ( int k = 0; k < sqr.Length; k ++ )
					if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == cont )
						if ( caseValidToAttack( sqr[ k ], player ) )
							return sqr[ k ];
			}

			if ( Statistics.terrains[ Form1.game.grid[ pos.X, pos.Y ].type ].ew == 1 )
			{
				if ( Form1.game.playerList[ player ].technos[ (byte)Form1.technoList.coastalNavigation ].researched )
					for ( int rad = 1; rad < Form1.game.width; rad ++ )
					{
						Point[] sqr = Form1.game.radius.returnEmptySquare( pos.X, pos.Y, rad );

						for ( int k = 0; k < sqr.Length; k ++ )
							if ( 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent > 0 && 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent != cont 
								)
								if ( caseValidToAttack( sqr[ k ], player ) )
									return sqr[ k ];
					}

				return returnLandFrontierToAttack( 
					player, 
					whichEnnemyToAttack( 
					player, new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ) 
					), 
					new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ) 
					); 
			}
			else
			{
				return new Point( -1, -1 );
			}
		}
		#endregion
		
#region wayToSiteToDefend
		public static Point[] wayToSiteToDefend( byte player, Point pos )
		{
		/*	int pos = 0;
			int[] caseValues = new int[ 25 ];
			Point[] cases = new Point[ 25 ];*/
			byte[] rtl = Form1.game.radius.relationTypeListWarAndCF;

			for ( int radius = 0; radius < 8; radius ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( pos, radius );
				for ( int sqrPos = 0; sqrPos < sqr.Length; sqrPos ++ )
				{
					if ( 
						Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].city > 0 &&
						(
						Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 == player ||
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector
						)
						)
					{ // city
						int totDUOC = 0;
						for ( int h = 0; h < Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].stack.Length; h ++ )
							if ( 
								Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].stack[ h ].player.player == player &&
								Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].stack[ h ].typeClass.attack > Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].stack[ h ].typeClass.defense
								)
								totDUOC ++;

						int totEUIR = 0; 
			
						for ( int r = 1; r < 3; r ++ )
						{
							Point[] sqr1 = Form1.game.radius.returnEmptySquare( sqr[ sqrPos ].X, sqr[ sqrPos ].Y, r );
							for ( int k = 0; k < sqr1.Length; k ++ ) 
								if ( Form1.game.radius.caseOccupiedByRelationType( sqr1[ k ].X, sqr1[ k ].Y, player, rtl ) )
									totEUIR ++;
						}

						if ( totDUOC < 3 || totDUOC < totEUIR )
						{
							Point[] way = Form1.game.radius.findWayTo( pos, sqr[ sqrPos ], 1, player, false ); 

							if ( way[ 0 ] != new Point( -1, -1 ) )
								return way;
						}
					}
					else if ( Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].militaryImprovement == (byte)enums.militaryImprovement.fort )
					{ // fort
						if (
							(
							Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 == player ||
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector ||
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war
							) &&
							!ai.caseOccupied( sqr[ sqrPos ].X, sqr[ sqrPos ].Y, player ) 
							)
						{
							Point[] way = Form1.game.radius.findWayTo( pos, sqr[ sqrPos ], 1, player, false ); 

							if ( way[ 0 ] != new Point( -1, -1 ) )
								return way;
						}
					}
					else if (
						Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].type == (byte)enums.terrainType.mountain ||
						Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].type == (byte)enums.terrainType.hill
						)
					{// mountains
						if (
							(
							Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 == player ||
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector ||
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war
							) &&
							!ai.caseOccupied( sqr[ sqrPos ].X, sqr[ sqrPos ].Y, player ) 
							)
							{ 
								Point[] way = Form1.game.radius.findWayTo( pos, sqr[ sqrPos ], 1, player, false ); 

								if ( way[ 0 ] != new Point( -1, -1 ) )
									return way;
							}
					}
				}
			}

			return new Point[ 1 ] { new Point( -1, -1 ) };
		}
		#endregion

#region hasEnnemyNear
		public static bool hasEnnemyNear( Point pos, byte player )
		{
			Point[] sqr = Form1.game.radius.returnEmptySquare( pos, 3 );

			for ( int i = 0; i < sqr.Length; i ++ )
				if ( ai.strongestEnnemy( sqr[ i ].X, sqr[ i ].Y, player ) > 0 )
					return true;

			return false;
		}
		#endregion

		public static byte buildTransportShip( byte player, Point pos )
		{
			return player;
		}

		public static byte goToTransport( byte player, int unit, Point dest )
		{
			return player;
		}

		public static Point[] wayToSiteToDefend( byte player, int unit )
		{
			return wayToSiteToDefend( player, new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ) );
		}

		public static int totDefensesAt( byte player, Point pos )
		{
			int totDef = 0;
			for ( int u = 0; u < Form1.game.grid[ pos.X, pos.Y ].stack.Length; u ++ )
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].stack[ u ].player.player == player &&
					Form1.game.grid[ pos.X, pos.Y ].stack[ u ].typeClass.speciality == enums.speciality.none
					)
					totDef += Form1.game.grid[ pos.X, pos.Y ].stack[ u ].typeClass.defense;

			return totDef;
		}
	}
}
