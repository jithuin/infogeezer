using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for ia.
	/// </summary>
	public class ai
	{
		public ai()
		{
			//
			// TODO: Add constructor logic here
			//
		}

#region choose construction
		public static void chooseConstruction( byte player, int city )
		{
			Random R = new Random();
			int nbrOfSettler = 0;
			bool ssaoc = Form1.game.playerList[ player ].memory.settlersStillAcceptedOnCont( Form1.game.grid[ Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ].continent );

			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber && i < 10; i ++ )
				if ( 
					Statistics.units[ Form1.game.playerList[ player ].unitList[ i ].type ].speciality == enums.speciality.builder &&
					Form1.game.playerList[ player ].unitList[ i ].state != (byte)Form1.unitState.dead
					)
					nbrOfSettler ++;

			if ( 
				ssaoc &&
				!ai.isInWar( player ) && 
				Form1.game.playerList[ player ].cityNumber < 4 && 
				nbrOfSettler < 3 
				) 
			{ 
				buildSettler( player, city ); 
			} 
			else if ( isInWar( player ) ) 
			{ 
				int generalWin = 0; 
				for ( int i = 0; i < Form1.game.playerList.Length; i ++ ) 
					if ( !Form1.game.playerList[ i ].dead && Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
						generalWin += 20 - whoIsWinning( player, (byte)i ); 

				int ran = R.Next( 3 + generalWin / 2 ); 

				if ( ran == 0 ) 
					buildRandomBuilding( player, city ); 
				else if ( ssaoc && ran == 1 && nbrOfSettler < Form1.game.playerList[ player ].cityNumber * 2.5 )
					buildSettler( player, city ); 
				else 
					buildRandomUnit( player, city ); 
			}
			else
			{
				int ran = R.Next( 3 );

				if ( ran == 0 )
					buildRandomBuilding( player, city );
				else if ( ssaoc && ran == 1 && nbrOfSettler < Form1.game.playerList[ player ].cityNumber * 2.5 )
					buildSettler( player, city );
				else
					buildRandomUnit( player, city );
			}
		}
	
		private static void buildSettler( byte player, int city )
		{
	//		Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type = 1;
	//		Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind = (byte)Form1.unitType.colon;
			Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] = Statistics.units[ (byte)Form1.unitType.colon ];
			Form1.game.playerList[ player ].cityList[ city ].construction.points = 0;
		}

		public static void buildRandomTransport( byte player, int city )
		{
			Random R = new Random();
			byte[] unitPossible2 = new byte[ Statistics.units.Length ]; 
			int q = 0;
			for ( byte i = 1; i < Form1.nbrUnit ; i ++ ) // units
				if ( 
					Form1.game.playerList[ player ].technos[ Statistics.units[i ].disponibility ].researched && 
					Statistics.units[ i ].terrain == 0 && 
					( 
						Statistics.units[ i ].obselete == 0 || 
						!Form1.game.playerList[ player ].technos[ Statistics.units[ Statistics.units[ i ].obselete ].disponibility ].researched 
					) 
					) 
				{ 
					unitPossible2[ q ] = i; 
					q++; 
				} 

		//	Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type = 1; 
		//	Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind = unitPossible2[ R.Next( q ) ]; 
			Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] = Statistics.units[ unitPossible2[ R.Next( q ) ] ];
			Form1.game.playerList[ player ].cityList[ city ].construction.points = 0; 
		}
		
/*		public static bool canBuildTransport( byte player )
		{
			if ( Form1.game.playerList[ player ].technos[ (byte)Form1.technoList.coastalNavigation ].researched )
				return true;
			else
				return false;
		}*/

		private static void buildRandomUnit( byte player, int city )
		{
			Random R = new Random();
			byte[] unitPossible2 = new byte[ 30 ]; // 30 can be changed
			int q = 0;
			for ( byte i = 1; i < Statistics.units.Length ; i ++ ) // units
				if ( 
					Form1.game.playerList[ player ].technos[ Statistics.units[i ].disponibility ].researched && 
					( 
						Statistics.units[ i ].obselete == 0 || 
						!Form1.game.playerList[ player ].technos[ Statistics.units[ Statistics.units[ i ].obselete ].disponibility ].researched 
					) &&
					( 
						Statistics.units[ i ].terrain > 0 ||  
						Form1.game.radius.isNextToWater( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y )
					)
					)
				{
					unitPossible2[ q ] = i;
					q++;
				}

		//	Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type = 1;
		//	Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind = unitPossible2[ R.Next( q - 1 ) ];
			Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] = Statistics.units[ unitPossible2[ R.Next( q ) ] ];
			Form1.game.playerList[ player ].cityList[ city ].construction.points = 0;
		}

		private static void buildRandomBuilding( byte player, int city )
		{
			Random R = new Random();
		//	Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type = 2;
			bool foundBuilding = false;
			for ( int k = 0; k <= Form1.totalBuildings; k ++) // totalBuildings
				if ( Form1.game.playerList[ player ].cityList[ city ].buildingList[ k ] == false )
				{
					Form1.game.playerList[ player ].cityList[ city ].construction.setFirstAndOnly( Statistics.buildings[ k ] );
			//		Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind = k;
					Form1.game.playerList[ player ].cityList[ city ].construction.points = 0; //Statistics.buildings[ k ].cost;
					foundBuilding = true;
					break;
				}

			if ( !foundBuilding ) // units
				buildRandomUnit( player, city );
		}
#endregion

#region is in war
		public static bool isInWar( byte player )
		{
			for ( byte i = 0; i < Form1.game.playerList.Length; i++ )
			{
				if ( Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
					return true;
			}
			return false;
		}
		#endregion

#region find City Site
		public static Point findCitySite( int xo, int yo, byte player )
		{
			return findCitySite( xo, yo, player, Form1.game.grid[ xo, yo ].continent );
		}

		public static Point findCitySite( int xo, int yo, byte player, int cont )
		{
			int startRadius = 0;

			int[] caseValue = new int[ 40 ];
			Point[] pntList = new Point[ 40 ];
			int pos = 0;

			if ( Form1.game.grid[ xo, yo ].city > 0 )
				startRadius = 3;

			int[,] costs = Form1.game.radius.gridCost( new Point( xo, yo ), 1, player, false, 8 );
			byte[] pols = Form1.game.radius.relationTypeListNonAllies; // .relationTypeListEnnemies;

			for ( int i = startRadius; i < 8; i ++ )
			{ // rayon principal
				Point[] sqr0 = Form1.game.radius.returnEmptySquare( xo, yo, i );

				for ( int j = 0; j < sqr0.Length; j ++ ) // cases possible de ville 
				{  // -1; 
					if ( 
						Form1.regionValidToBuildCity[ sqr0[ j ].X ].Get( sqr0[ j ].Y ) &&
						costs[ sqr0[ j ].X, sqr0[ j ].Y ] != -1 && 
						Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].continent == cont &&
						( 
						Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].territory == 0 || 
						Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].territory - 1 == player 
						) &&
						!Form1.game.radius.caseOccupiedByRelationType( sqr0[ j ].X, sqr0[ j ].Y, player, pols ) &&
						Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type != (byte)enums.terrainType.mountain &&
						Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type != (byte)enums.terrainType.glacier
						)
					{
						bool stillValid = true;

						if ( ai.isInWar( player ) )
							{
								Point[] sqr2 = Form1.game.radius.returnEmptySquare( sqr0[ j ], 1 );
								for ( int g = 0; g < sqr2.Length && stillValid; g ++ ) // rayon de la case verifié					
									if ( 
										Form1.game.radius.caseOccupiedByRelationType( sqr2[ g ].X, sqr2[ g ].Y, player, pols ) ||
										(
										Form1.game.grid[ sqr2[ g ].X, sqr2[ g ].Y ].territory != 0 &&
										Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr2[ g ].X, sqr2[ g ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war
										)
										)
									{
										stillValid = false;
										break;
									}
							}

						if ( stillValid )
						{
							caseValue[ pos ] = 100;

							for ( int k = 4; k < 6; k ++ )
							{
								Point[] sqr1 = Form1.game.radius.returnEmptySquare( sqr0[ j ].X, sqr0[ j ].Y, k );
								
								for ( int g = 0; g < sqr1.Length; g ++ )
									if ( 
										Form1.game.grid[ sqr1[ g ].X, sqr1[ g ].Y ].city > 0 && 
										Form1.game.grid[ sqr1[ g ].X, sqr1[ g ].Y ].territory - 1 == player &&
										Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr1[ g ].X, sqr1[ g ].Y ].territory - 1 ].madeContact
										)
										caseValue[ pos ] += 70;
							}

							if ( Form1.game.radius.isNextToWater( sqr0[ j ].X, sqr0[ j ].Y ) )
								caseValue[ pos ] += 50;

							if ( 
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.plain ||
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.forest ||
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.jungle
								)
								caseValue[ pos ] += 15;
							else if (
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.prairie
								)
								caseValue[ pos ] += 13;
							else if (
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.hill ||
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.tundra ||
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.swamp ||
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.desert
								)
								caseValue[ pos ] -= 5;
							else if (
								Form1.game.grid[ sqr0[ j ].X, sqr0[ j ].Y ].type == (byte)enums.terrainType.glacier
								)
								caseValue[ pos ] -= 10;

							caseValue[ pos ] -= costs[ sqr0[ j ].X, sqr0[ j ].Y ] * 2;

							pntList[ pos ] = sqr0[ j ]; 
							pos ++;

							if ( pos >= pntList.Length ) 
							{ 
								int[] bestCases = count.descOrder( caseValue );
								return pntList[ bestCases[ 0 ] ];
							} 
						}
					}
				}
			}

			if ( pos > 0 ) 
			{ 
				int[] caseValue1 = new int[ pos ]; 
				for ( int i = 0; i < pos; i ++ ) 
					caseValue1[ i ] = caseValue[ i ]; 
 
				int[] bestCases = count.descOrder( caseValue1 ); 
				return pntList[ bestCases[ 0 ] ]; 
			} 
 
			return new Point( -1, -1 ); 
		} 
#endregion

#region site Is Valid For City
		public static bool siteIsValidForCity( int xo, int yo, byte player ) 
		{ 
		////	Radius radius = new Radius(); 
			//getPFT getPFT1 = new getPFT(); 

			if ( Form1.game.grid[ xo, yo ].city > 0 || ai.strongestEnnemy( xo, yo, player ) > 0 ) 
				return false; 

			Point[] sqr = Form1.game.radius.regionInvalidForCity( new Point( xo, yo ) ); 
			for ( int i = 0; i < sqr.Length; i ++ ) 
				if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 ) 
					return false; 

			/*Point[] cityRadius = radius.returnCityRadius( xo, yo );
			for ( int i = 0; i < cityRadius.Length; i ++ )
			{
			}*/

			return true; 
		} 
		#endregion

#region find Case To Improve
		public static Point findCaseToImprove( int xo, int yo, byte player )
		{
			if ( 
				Form1.game.grid[ xo, yo ].laborCity > 0 && 
				Form1.game.grid[ xo, yo ].laborOwner == player && 
				( 
					Form1.game.grid[ xo, yo ].civicImprovement == 0 || 
					Form1.game.grid[ xo, yo ].roadLevel == 0 
				) 
				) 
				return new Point( xo, yo ); 

		////	Radius radius = new Radius(); 
			int pos = 0; 
			int[] caseValue = new int[ 25 ]; 
			Point[] cases = new Point[ 25 ]; 
			byte[] rtl = Form1.game.radius.relationTypeListNonAllies;

			int[,] costs = Form1.game.radius.gridCost( new Point( xo, yo ), 1, player, false, 8 );

			for ( int i = 0; i < 10; i ++ ) 
			{ 
				Point[] sqr = Form1.game.radius.returnEmptySquare( xo, yo, i ); 

				for ( int j = 0; j < sqr.Length; j ++ ) 
				{ 
					caseValue[ pos ] = 0; 

					if ( 
						costs[ sqr[ j ].X, sqr[ j ].Y ] != -1 &&
						Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].continent == Form1.game.grid[ xo, yo ].continent && 
						Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].territory - 1 == player && 
						Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].city == 0 && 
						!Form1.game.radius.caseOccupiedByRelationType( sqr[ j ].X, sqr[ j ].Y, player, rtl )
						) 
					{ // si y a rien de construit 
						bool stillValid = false;

						if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].civicImprovement == 0 ) 
						{ 
							caseValue[ pos ] += 10; 
							stillValid = true; 
						} 
						else if ( 
							Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].roadLevel == 0 || 
							( 
							Form1.game.playerList[ player ].technos[ (byte)Form1.technoList.steamPower ].researched && 
							Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].roadLevel == 1 
							) 
							) 
						{ 
							caseValue[ pos ] += 5; 
							stillValid = true; 
						} 

						if ( stillValid ) 
						{ 
							int totSettler = 0; 
							for ( int h = 0; h < Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].stack.Length; h ++ ) 
								if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].stack[ h ].typeClass.speciality == enums.speciality.builder ) 
								{
									totSettler ++; 
									if ( totSettler > 2 ) 
										stillValid = false; 
								} 
						} 

						if ( stillValid )
						{ 
							stillValid = false;
							if ( 
								Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].laborCity > 0 && 
								Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].laborOwner == player 
								)
							{ 
								caseValue[ pos ] += 10; 
								stillValid = true; 
							} 
							else if ( Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].laborCity == 0 ) 
							{ 
								Point[] cityRadius = Form1.game.radius.returnCityRadius(  sqr[ j ].X, sqr[ j ].Y ); 

								for ( int k = 0; k < cityRadius.Length; k ++ )
									if ( 
										Form1.game.grid[ cityRadius[ k ].X, cityRadius[ k ].Y ].city > 0 && 
										Form1.game.grid[ cityRadius[ k ].X, cityRadius[ k ].Y ].territory - 1 == player
										) 
									{ 
										caseValue[ pos ] += 5; 
										stillValid = true; 
										break; 
									} 
							} 
						}

						if ( stillValid )
						{
							caseValue[ pos ] -= 2 * costs[ sqr[ j ].X, sqr[ j ].Y ]; //game.radius.findCostTo( xo, yo, sqr[ j ].X, sqr[ j ].Y, 1, player, false ); 
							
							if ( 
								Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].type == (byte)enums.terrainType.desert ||
								Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].type == (byte)enums.terrainType.tundra ||
								Form1.game.grid[ sqr[ j ].X, sqr[ j ].Y ].type == (byte)enums.terrainType.swamp
								) 
								caseValue[ pos ] -= 5; 
 
							if ( Form1.game.radius.isNextToIrrigation( sqr[ j ].X, sqr[ j ].Y ) ) 
								caseValue[ pos ] += 10; 
 
							cases[ pos ] = sqr[ j ]; 
							pos ++; 

							if ( pos >= cases.Length ) 
							{ 
								int bestCase = 0; 
								for ( int h = 1; h < pos; h ++ ) 
									if ( caseValue[ h ] > caseValue[ bestCase ] ) 
										bestCase = h; 

								return cases[ bestCase ]; 
							} 
						} 
					} 
				} 
			}

			//	if i > 10
			if ( pos > 0 ) 
			{ 
				int bestCase = 0; 
				for ( int h = 1; h < pos; h ++ ) 
					if ( caseValue[ h ] > caseValue[ bestCase ] ) 
						bestCase = h; 

				return cases[ bestCase ]; 
			} 
			else
				return new Point( -1, -1 );
		}
		#endregion

#region choose Improvement
		public static sbyte chooseImprovement( int xo, int yo, byte player )
		{ // 0 = road, 1 = irr, 2 = mine
			Random R = new Random();
	//	//	Radius radius = new Radius();

			if ( Form1.game.grid[ xo, yo ].civicImprovement == 0 && Form1.game.grid[ xo, yo ].city == 0 )
			{
				if ( 
					Form1.game.grid[ xo, yo ].type == (byte)enums.terrainType.plain ||
					Form1.game.grid[ xo, yo ].type == (byte)enums.terrainType.prairie 
					)
				{
					if ( Form1.game.radius.isNextToIrrigation( xo, yo ) )
					{
						if ( R.Next( 10 ) < 7 )
							return 1;
						else
							return 2;
					}
					else // non-next to irrigation
						return 2;
				}
				else if (
					Form1.game.grid[ xo, yo ].type == (byte)enums.terrainType.hill ||
					Form1.game.grid[ xo, yo ].type == (byte)enums.terrainType.mountain
					)
				{
					return 2;
				}
				else if ( Form1.game.grid[ xo, yo ].type == (byte)enums.terrainType.desert )
				{
					if ( Form1.game.radius.isNextToIrrigation( xo, yo ) )
						return 1;
					else if ( Form1.game.grid[ xo, yo ].roadLevel == 0 )
						return 0;
					else
						return 2;
				}
				else
					return 2;
			} //	else 

			if ( 
				Form1.game.grid[ xo, yo ].city == 0 &&
				(
					Form1.game.grid[ xo, yo ].roadLevel == 0 || 
				(
					Form1.game.grid[ xo, yo ].roadLevel == 1 &&
					Form1.game.playerList[ player ].technos[ (int)Form1.technoList.steamPower ].researched
				)
				)
				)
				return 0;
			else
				return -1;

			//return -1;
		}
		#endregion

#region return disponible technos
		public static byte[] returnDisponibleTechnologies( byte player )
		{
			int j = 0;
			byte[] retour = new byte[ Statistics.technologies.Length ];
			
			for ( byte i = 1; i < Statistics.technologies.Length; i++ )
				if ( 
					!Form1.game.playerList[ player ].technos[ i ].researched && 
					( 
					Form1.game.playerList[ player ].technos[ Statistics.technologies[ i ].needs[ 0 ] ].researched &&    
					Form1.game.playerList[ player ].technos[ Statistics.technologies[ i ].needs[ 1 ] ].researched &&    
					Form1.game.playerList[ player ].technos[ Statistics.technologies[ i ].needs[ 2 ] ].researched 
					) &&
					Statistics.technologies[ i ].canBeResearched
					)
				{
					retour[ j ] = i;
					j ++;
				}


			byte[] retour1 = new byte[ j ];
			for ( int i = 0; i < retour1.Length ; i ++ )
				retour1[ i ] = retour[ i ];

	//		if ( retour1.Length > 0 )
			return retour1;
	/*		else
				return new byte[]{ Form1.technoList.initial };			*/

		}
		#endregion

#region random technos
		public static byte randomTechnology( byte player )
		{
			Random r = new Random();
			byte[] dispTechnos = returnDisponibleTechnologies( player );

			return dispTechnos[ r.Next( dispTechnos.Length) ];
			
		}
#endregion

#region Mine - transport mine, unitmine, strongest ennemy
		public int unitMine( int x, int y)
		{
			//int unitStuck = 0;
			for ( int i=1; i <= Form1.game.grid[ x, y ].stack.Length; i++ )
			{
				if ( Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == Form1.game.curPlayerInd ) //  && unitList[ Form1.game.curPlayerInd, game.grid[ x, y ].stack [ i - 1 ].unit ].moveLeft
				{
					return i;//selected.owner = (byte);
				}
			}
			return 0;
		}

		public int transportMine( int x, int y)
		{
			//int unitStuck = 0;
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++ )
			{
				if ( Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == Form1.game.curPlayerInd &&  Statistics.units[ Form1.game.grid[ x, y ].stack[ i - 1 ].type ].transport > 0 )
				{
					return i;//selected.owner = (byte);
				}
			}
			return 0;
		}

		/// <summary>
		/// return unit pos in stack
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="attacker"></param>
		/// <returns></returns>
		public static int strongestEnnemy( int x, int y, byte attacker )
		{
			int retour = 0;
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++)
			{
				if ( Form1.game.grid[ x, y ].stack[ i - 1 ].player.foreignRelation[ attacker ].politic == (byte)Form1.relationPolType.war)
					if ( retour == 0)
					{
						retour = i;
					}
					else if ( Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.defense * 
						Form1.game.grid[ x, y ].stack[ i - 1 ].health >
						Form1.game.grid[ x, y ].stack[ retour - 1 ].typeClass.defense * 
						Form1.game.grid[ x, y ].stack[ retour - 1 ].health
						)
					{
						retour = i;
					}
			}

			return retour;
		}

		public static bool caseOccupied( int x, int y, byte attacker )
		{
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++)
			{
				if ( Form1.game.grid[ x, y ].stack[ i - 1 ].player.player != attacker )
					if ( 
						Form1.game.playerList[ attacker ].foreignRelation[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].politic == (byte)Form1.relationPolType.war || 
						Form1.game.playerList[ attacker ].foreignRelation[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].politic == (byte)Form1.relationPolType.Protector || 
						Form1.game.playerList[ attacker ].foreignRelation[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].politic == (byte)Form1.relationPolType.ceaseFire || 
						Form1.game.playerList[ attacker ].foreignRelation[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].politic == (byte)Form1.relationPolType.peace 
						)
						return true;
			}

			return false;
		}

		public int intruderUnit( int x, int y, byte player )
		{
			int retour = 0;

			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++)
				if ( Form1.game.playerList[ Form1.game.grid[ x, y ].stack[ i - 1 ].player.player ].foreignRelation[ player ].politic == (byte)Form1.relationPolType.war )
				{
					if ( retour == 0)
						retour = i;
					else if ( Statistics.units[ Form1.game.grid[ x, y ].stack[ i - 1 ].type ].defense * 
						Form1.game.grid[ x, y ].stack[ i - 1 ].health >
						Statistics.units[ Form1.game.grid[ x, y ].stack[ i - 1 ].type ].defense * 
						Form1.game.grid[ x, y ].stack[ i - 1 ].health
						)
						retour = i;
				}

			return retour;

		}
		#endregion

#region hasAdvUnit
		private bool hasAdvUnit( int x, int y, byte player )
		{
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i ++ )
			{
				if ( 
					Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].stack[ i - 1].player.player ].politic != (byte)Form1.relationPolType.alliance &&
					Form1.game.grid[ x, y ].stack[ i - 1].player.player != player
					)
				{
					return true;
				}
			}

			return false;
		}
#endregion

#region fortEnnemyAtillery
		public UnitList[] fortEnnemyAtillery( Point pos, byte player )
		{ // shall i put the verification isInWar here?
			UnitList[] ennemyArt = new xycv_ppc.UnitList[ 0 ];
			UnitList[] buffer = new xycv_ppc.UnitList[ 0 ];
	//	//	Radius radius = new Radius();

			for ( int k = 1; k < 4; k ++ ) // 4 may have to be changed if there is greater range canons
			{
				Point[] sqr = Form1.game.radius.returnSquare( pos.X, pos.Y, k ); 

				for ( int i = 0; i < sqr.Length; i ++ )
					for ( int j = 1; j <= Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack.Length; j ++ )
						if ( 
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].player.player ].politic == (byte)Form1.relationPolType.war &&
							Statistics.units[ Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].type ].speciality == enums.speciality.bombard &&
							Statistics.units[ Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].type ].range <= k  &&
							Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ].anyMoveLeft
							)
						{
							buffer = ennemyArt;
							ennemyArt = new xycv_ppc.UnitList[ buffer.Length + 1 ];

							for ( int h = 0; h < buffer.Length; h ++ )
								ennemyArt[ h ] = buffer[ h ];

							ennemyArt[ ennemyArt.Length - 1 ] = Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].stack[ j - 1 ];
						}
			}

			return ennemyArt;
		}
#endregion

#region whoIsWinning
		/// <summary>
		///Player:
		///0 - losing,		
		///10 - equal,		
		///20 - winning
		/// </summary>
		public static byte whoIsWinning( byte player, byte ennemy )
		{
			int playerAdv = 0;
			int ennemyAdv = 0;
			int unitMod = 5;
			int cityMod = 3;
			int otherCityMod = 6;
			int technoMod = 1;

			#region units
			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber; i ++ )
				if (
					Form1.game.playerList[ player ].unitList[ i ].state != (byte)Form1.unitState.dead &&
					Statistics.units[ Form1.game.playerList[ player ].unitList[ i ].type ].speciality != enums.speciality.builder
					)
					playerAdv += unitMod;
			
			for ( int i = 1; i <= Form1.game.playerList[ ennemy ].unitNumber; i ++ )
				if ( 
					Form1.game.playerList[ ennemy ].unitList[ i ].state != (byte)Form1.unitState.dead &&
					Statistics.units[ Form1.game.playerList[ ennemy ].unitList[ i ].type ].speciality != enums.speciality.builder
					)
					ennemyAdv += unitMod;
			#endregion

			#region city
			for ( int i = 1; i <= Form1.game.playerList[ player ].cityNumber; i ++ )
				if ( Form1.game.playerList[ player ].cityList[ i ].state != (byte)enums.cityState.dead )
				{
					if ( Form1.game.playerList[ player ].cityList[ i ].originalOwner == ennemy )
						playerAdv += Form1.game.playerList[ player ].cityList[ i ].population * otherCityMod;
					else
						playerAdv += Form1.game.playerList[ player ].cityList[ i ].population * cityMod;
				}

			for ( int i = 1; i <= Form1.game.playerList[ ennemy ].cityNumber; i ++ )
				if ( Form1.game.playerList[ ennemy ].cityList[  i ].state != (byte)enums.cityState.dead )
				{
					if ( Form1.game.playerList[ ennemy ].cityList[  i ].originalOwner == player )
						ennemyAdv += Form1.game.playerList[ ennemy ].cityList[  i ].population * otherCityMod;
					else
						ennemyAdv += Form1.game.playerList[ ennemy ].cityList[  i ].population * cityMod;
				}
			#endregion

			#region technos
			for ( int i = 0; i < Statistics.technologies.Length; i ++ )
			{
				if ( Form1.game.playerList[ player ].technos[ i ].researched && !Form1.game.playerList[ ennemy ].technos[ i ].researched )
				{
					playerAdv += Statistics.technologies[ i ].cost * technoMod;
				}
				else if ( Form1.game.playerList[ ennemy ].technos[ i ].researched && !Form1.game.playerList[ player ].technos[ i ].researched )
				{
					ennemyAdv += Statistics.technologies[ i ].cost * technoMod;
				}
			}
			#endregion

			#region nbr fronts
			int playerEnnemies = 1;
			int ennemyEnnemies = 1;

			for ( int i = 0; i < Form1.game.playerList.Length; i ++  )
			{
				if ( !Form1.game.playerList[ i ].dead )
				{
					if ( Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
					{
						playerEnnemies ++;
					}
					if ( Form1.game.playerList[ ennemy ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
					{
						ennemyEnnemies ++;
					}
				}
			}

			playerAdv /= playerEnnemies;
			ennemyAdv /= ennemyEnnemies;

			#endregion

			#region cur player wealth
			if ( Form1.game.playerList[ player ].money > Form1.game.playerList[ ennemy ].money * 10 )
			{
				playerAdv *= 3 / 2;
			}
			else if ( Form1.game.playerList[ ennemy ].money > Form1.game.playerList[ player ].money * 10 )
			{
				ennemyAdv *= 3 / 2;
			}
			#endregion

			int diff;
			if ( ennemyAdv > 0 )
				diff = 10 * playerAdv / ennemyAdv;
			else
				diff = 10 * playerAdv / 1;

		//	System.Windows.Forms.MessageBox.Show( "player: " + playerAdv.ToString() + "\nEnnemy: " + ennemyAdv.ToString() + "\nDiff: " + diff.ToString(), "Who is winning" );

			if ( diff > 20 )
				return 20;
			else if ( diff < 0 ) // even if it s impossible
				return 0;
			else
				return (byte)diff;

		}
#endregion

#region whatCanBeBuilt
		public static Stat.Construction[] whatCanBeBuilt( byte player, int city )
		{
		//	structures.intByte[] ib = new xycv_ppc.structures.intByte[ 200 ];
			Stat.Construction[] ib = new Stat.Construction[ Statistics.units.Length + Statistics.buildings.Length + Statistics.smallWonders.Length + Statistics.wonders.Length + 1 ];
			int curr = 0;

			for ( byte i = 0; i < Statistics.units.Length; i ++ ) //units
				if ( 
						Form1.game.playerList[ player ].technos[ Statistics.units[i ].disponibility ].researched && 
						( 
							Statistics.units[ i ].obselete == 0 || 
							!Form1.game.playerList[ player ].technos[ Statistics.units[ Statistics.units[ i ].obselete ].disponibility ].researched 
						) &&
						( 
							Statistics.units[ i ].terrain == 1 || 
							Form1.game.radius.isNextToWater( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ) 
						)
					)
				{
					ib[ curr ] = Statistics.units[ i ];
					curr ++;
				}

			if ( !Form1.game.playerList[ player ].technos[ 0 ].researched )
			{
				ib[ curr ] = Statistics.units[ 0 ];
				curr ++;
			}

			for ( byte i = 0; i < Statistics.buildings.Length; i ++ ) //buildings
				if ( 
					Form1.game.playerList[ player ].technos[ Statistics.buildings[ i ].disponibility ].researched &&
					!Form1.game.playerList[ player ].cityList[ city ].buildingList[ i ]
					)
				{
					ib[ curr ] = Statistics.buildings[ i ];
					curr ++;
				}

			for ( byte i = 0; i < Statistics.smallWonders.Length; i ++ ) //smallWonders
				if ( 
					Form1.game.playerList[ player ].technos[ Statistics.smallWonders[ i ].disponibility ].researched &&
					Form1.game.playerList[ player ].smallWonderList.canBuildWonder( i )
					)
				{
					ib[ curr ] = Statistics.smallWonders[ i ];
					curr ++;
				}

			for ( byte i = 0; i < Statistics.wonders.Length; i ++ ) //Wonders
				if ( 
					Form1.game.playerList[ player ].technos[ Statistics.wonders[ i ].disponibility ].researched &&
					Form1.game.wonderList.canBuildWonder( i )
					)
				{
					ib[ curr ] = Statistics.wonders[ i ];
					curr ++;
				}
			
			ib[ curr ] = new Stat.Wealth();
			curr ++;

			Stat.Construction[] retour = new Stat.Construction[ curr ];

			for ( int i = 0; i < retour.Length; i ++ )
				retour[ i ] = ib[ i ];

			return retour;
		}
		#endregion

#region findWhereToExplore
		public static Point findWhereToExplore( byte player, int unit )
		{
			return findWhereToExplore( player, new Point( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ), Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].terrain );
		}

		public static Point findWhereToExplore( byte player, Point ori, byte terrain )
		{
			int maxRange = 5;
			int[,] cost = Form1.game.radius.gridCost( ori, terrain, player, false, maxRange );

			for ( int r = 1; r < maxRange; r ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( ori, r );
				int[] caseValue = new int[ sqr.Length ];

				for ( int k = 0; k < sqr.Length; k ++ )
					if ( 
						( // continent 
						Statistics.terrains[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == terrain &&
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == Form1.game.grid[ ori.X, ori.Y ].continent || 
						( 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.coast && 
						Form1.game.radius.isNextToLand( sqr[ k ].X, sqr[ k ].Y ) 
						) 
						) && 
						( // territory
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory == 0 || 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == player || 
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance || 
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protected || 
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.Protector 
						) 
						) 
					{ 
						if ( !Form1.game.playerList[ player ].discovered[ sqr[ k ].X, sqr[ k ].Y ] )
						{
							if ( 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.mountain || 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.hill
								)
								caseValue[ k ] = 2;
							else
								caseValue[ k ] = 1;
						}
						else if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].resources > 0 && Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].resources < 100 )
						{
							if ( cost[ sqr[ k ].X, sqr[ k ].Y ] != -1 )
								return sqr[ k ];
							else
								caseValue[ k ] = -1;
						}
						else
							caseValue[ k ] = -1;
					} 
					else
					{
						caseValue[ k ] = -1;
					}

				int[] order = count.descOrder( caseValue );
				for ( int i = 0; i < order.Length; i ++ )
					if ( caseValue[ order[ i ] ] != -1 )
					{
						Point[] sqr0 = Form1.game.radius.returnEmptySquare( sqr[ order[ i ] ], 1 );
						int[] caseValue0 = new int[ sqr0.Length ];
						for ( int h = 0; h < sqr0.Length; h ++ )
							caseValue0[ h ] = cost[ sqr0[ h ].X, sqr0[ h ].Y ];

						int[] order0 = count.ascOrder( caseValue0 );
						for ( int h = 0; h < order0.Length; h ++ )
							if ( caseValue0[ order0[ h ] ] != -1 )
								return sqr0[ order0[ h ] ];
					}
					else
						break;
			}

			return new Point( - 1, -1 ); 
		}
#endregion

#region findCitySiteWorldWide
		public static Point findCitySiteWorldWide( Point ori, byte player )
		{
			int xo = ori.X, yo = ori.Y; 
		//	int startRadius = 0; 

			int[] caseValue = new int[ Form1.game.width * Form1.game.height / 5 ]; 
			Point[] pntList = new Point[ caseValue.Length ]; 
			int pos = 0; 

		/*	if ( Form1.game.grid[ xo, yo ].city > 0 ) 
				startRadius = 3; */

			byte[] pols = Form1.game.radius.relationTypeListAll;//.returnRelationTypeList( true, true, true, true, true, true ); 

			for ( int x = 0; x < Form1.game.width; x ++ ) 
				for ( int y = 0; y < Form1.game.height; y ++ ) 
					if ( Form1.regionValidToBuildCity[ x ].Get( y ) ) 
						if ( Statistics.terrains[ Form1.game.grid[ x, y ].type ].ew == 1 ) 
							if ( Form1.game.radius.isNextToWater( x, y ) ) 
								if ( !Form1.game.radius.caseOccupiedByRelationType( x, y, player, pols ) ) 
									if ( Form1.game.grid[ x, y ].type != (byte)enums.terrainType.mountain ) 
										if ( Form1.game.grid[ x, y ].type != (byte)enums.terrainType.glacier ) 
											if (  
												Form1.game.grid[ x, y ].territory == 0 || 
												Form1.game.grid[ x, y ].territory - 1 == player 
						
												)
												/*if ( 
													Form1.regionValidToBuildCity[ x ].Get( y ) &&
													Statistics.terrains[ Form1.game.grid[ x, y ].type ].ew == 1 &&
													game.radius.isNextToWater( x, y )  &&
													!game.radius.caseOccupiedByRelationType( x, y, player, pols ) &&
													Form1.game.grid[ x, y ].type != (byte)enums.terrainType.mountain &&
													Form1.game.grid[ x, y ].type != (byte)enums.terrainType.glacier&&
													( 
														Form1.game.grid[ x, y ].territory == 0 || 
														Form1.game.grid[ x, y ].territory - 1 == player 
													)
													)*/
											{
												caseValue[ pos ] = 1000;

												if ( 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.plain || 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.forest || 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.jungle 
													) 
													caseValue[ pos ] += 15; 
												else if ( 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.prairie 
													) 
													caseValue[ pos ] += 13; 
												else if ( 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.hill || 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.tundra || 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.swamp || 
													Form1.game.grid[ x, y ].type == (byte)enums.terrainType.desert 
													) 
													caseValue[ pos ] -= 5; 
						
												caseValue[ pos ] -= 
													2 * Form1.game.radius.getDistWith( 
													ori, 
													new Point( x, y ) 
													) + 
													3 * Form1.game.radius.getDistWith( 
													ori, 
													new Point( 
													Form1.game.playerList[ player ].cityList[ Form1.game.playerList[ player ].capital ].X, 
													Form1.game.playerList[ player ].cityList[ Form1.game.playerList[ player ].capital ].Y 
													) 
													); 

												pntList[ pos ] = new Point( x, y ); 
												pos ++;

												if ( pos >= pntList.Length ) 
												{ 
													int[] bestCases = count.descOrder( caseValue );
													return pntList[ bestCases[ 0 ] ];
												} 
											}

			if ( pos > 0 ) 
			{ 
				int[] caseValue1 = new int[ pos ]; 
				for ( int i = 0; i < pos; i ++ ) 
					caseValue1[ i ] = caseValue[ i ]; 
 
				int[] bestCases = count.descOrder( caseValue1 ); 
				return pntList[ bestCases[ 0 ] ]; 
			} 
 
			return new Point( -1, -1 ); 
		} 
		#endregion 

#region findNearestPlayerCity
		public static Point findNearestPlayerCity( byte player, int unit )
		{
			return findNearestPlayerCity( player, Form1.game.playerList[ player ].unitList[ unit ].pos );
		}
		public static Point findNearestPlayerCity(byte player, Point pos )
		{
			int[] dists = new int[ Form1.game.playerList[ player ].cityNumber + 1 ];
			bool foundSomething = false;
			dists[ 0 ] = 30000;
			for ( int c = 1; c<= Form1.game.playerList[ player ].cityNumber; c ++ )
				if ( 
					Form1.game.playerList[ player ].cityList[ c ].state == (byte)enums.cityState.ok &&
					Form1.game.grid[ Form1.game.playerList[ player ].cityList[ c ].X, Form1.game.playerList[ player ].cityList[ c ].Y ].continent == Form1.game.grid[ pos.X,pos.Y ].continent
					)
				{
					dists[ c ] = Form1.game.radius.getDistWith( pos, Form1.game.playerList[ player ].cityList[ c ].pos );
					foundSomething = true;
				}
				else
				{
					dists[ c ] = 30000;
				}

			if ( foundSomething )
			{
				int[] order = count.ascOrder( dists );
				return Form1.game.playerList[ player ].cityList[ order[ 0 ] ].pos;
			}
			else
				return new Point( -1, -1 );

		}
#endregion

	} 
} 