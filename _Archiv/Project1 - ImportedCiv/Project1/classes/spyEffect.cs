using System;
using System.Drawing;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for spyEffect.
	/// </summary>
	public class spyEffect
	{
		static Control parent;

		public static bool counterIntResist( byte player, byte def, byte type ) 
		{ 
			Random r = new Random(); 

			if ( r.Next( Form1.game.playerList[ player ].foreignRelation[ def ].spies[ type ].efficiency ) > Form1.game.playerList[ def ].counterIntNbr ) 
				return true; 
			else 
				return false; 
		} 
 
		public static bool counterIntGetInfo( byte player, byte def, byte type ) 
		{ 
			Random r = new Random(); 

			if ( 
				Form1.game.playerList[ def ].counterIntNbr * 2 > Form1.game.playerList[ player ].foreignRelation[ def ].spies[ type ].nbr && 
				r.Next( Form1.game.playerList[ player ].foreignRelation[ def ].spies[ type ].efficiency ) < 25 
				) 
				return true; 
			else 
				return false; 
		} 

#region civic
		public static void civicSeeCity( byte player, byte adv )
		{
			int city = selectRandomCityFromPop( adv );

			for ( int radius = 0; radius <= Form1.game.playerList[ adv ].cityList[ city ].population / 3 + 1; radius ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ adv ].cityList[ city ].X, Form1.game.playerList[ adv ].cityList[ city ].Y, radius );
				for ( int k = 0; k < sqr.Length; k ++ )
					if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == adv )
					{
						Form1.game.playerList[ player ].discovered[ sqr[ k ].X, sqr[ k ].Y ] = true;
						Form1.game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
					}
			}

			if ( player == Form1.game.curPlayerInd && Form1.options.autosave )
				System.Windows.Forms.MessageBox.Show( "One of your spies sent information about " + Form1.game.playerList[ adv ].cityList[ city ].name + ".", "Intelligence" );
		}

		public static void civicSeeSettler( byte player, byte adv )
		{
			int unit = selectRandomSettler( adv );
			seeUnit( player, adv, unit );
		}

		public static void civicStartRiot( byte player, byte adv )
		{
			int city = selectRandomCityFromPop( adv );

			Form1.game.playerList[ player ].cityList[ city ].rioting = true;

			if ( player == Form1.game.curPlayerInd && Form1.options.autosave )
				System.Windows.Forms.MessageBox.Show( "One of your spies managed to start a riot in " +Form1.game.playerList[ adv ].cityList[ city ].name + ".", "Intelligence" );
		}

		public static void civicSabotageConstruction( byte player, byte adv )
		{
			int city = selectRandomCity( adv );

			Random r = new Random();
			Form1.game.playerList[ player ].cityList[ city ].construction.points *= ( r.Next( 60 ) + 20 ) / 100 ;

			if ( player == Form1.game.curPlayerInd && Form1.options.autosave )
				System.Windows.Forms.MessageBox.Show( "One of your spies succesfully sabotaged the construction in " + Form1.game.playerList[ adv ].cityList[ city ].name + ".", "Intelligence" );
		}
#endregion

#region select random city and settler
		public static int selectRandomCityFromPop( byte adv )
		{
			int totalPop = 0;
			for ( int i = 1; i <= Form1.game.playerList[ adv ].cityNumber; i ++ )
				if ( Form1.game.playerList[ adv ].cityList[ i ].state != (byte)enums.cityState.dead )
					totalPop += Form1.game.playerList[ adv ].cityList[ i ].population;

			Random r = new Random();
			int ran = r.Next( totalPop );
			
			for ( int i = 1; i <= Form1.game.playerList[ adv ].cityNumber; i ++ )
				if ( Form1.game.playerList[ adv ].cityList[ i ].state != (byte)enums.cityState.dead )
				{
					if ( ran < Form1.game.playerList[ adv ].cityList[ i ].population )
					{
						return i;
					}
					else
					{
						ran -= Form1.game.playerList[ adv ].cityList[ i ].population;
					}
				}
			return -1;
		}

		public static int selectRandomCity( byte adv )
		{
			Random r = new Random();
			int watch = 0;

			while ( true )
			{
				int city = r.Next( Form1.game.playerList[ adv ].cityNumber ) + 1;

				if ( Form1.game.playerList[ adv ].cityList[ city ].state != (byte)enums.cityState.dead )
					return city;

				watch ++;
				if ( watch > 5000 )
					break;
			}

			return -1;
		}

		public static int selectRandomSettler( byte adv )
		{
			Random r = new Random();
			int tot = 0;

			for ( int unit = 1; unit <= Form1.game.playerList[ adv ].unitNumber; unit ++ )
				if ( 
					Form1.game.playerList[ adv ].unitList[ unit ].state != (byte)Form1.unitState.dead &&  
					Statistics.units[ Form1.game.playerList[ adv ].unitList[ unit ].type ].speciality == enums.speciality.builder
					)
					tot ++;

			int chose = r.Next( tot );
			
			for ( int unit = 1; unit <= Form1.game.playerList[ adv ].unitNumber; unit ++ )
				if ( 
					Form1.game.playerList[ adv ].unitList[ unit ].state != (byte)Form1.unitState.dead &&  
					Statistics.units[ Form1.game.playerList[ adv ].unitList[ unit ].type ].speciality == enums.speciality.builder
					)
				{
					if ( chose == 0 )
					{
						return unit;
					}
					else
						chose --;
				}
			return -1;
		}
#endregion
		
#region military
		public static void militarySeeUnit( byte player, byte adv )
		{
			int unit = selectRandomMilitaryUnit( adv );
			seeUnit( player, adv, unit );
		}

		public static void militaryGetUnitPlan( byte player, byte adv )
		{
			int unit = selectRandomMilitaryUnit( adv );
			seeUnit( player, adv, unit );
		}

		public static void militarySabotageUnit( byte player, byte adv )
		{
			int unit = selectRandomMilitaryUnit( adv );
			seeUnit( player, adv, unit );
			
			if ( player == Form1.game.curPlayerInd && Form1.options.autosave )
				System.Windows.Forms.MessageBox.Show( "One of your spies sabotaged a " + Statistics.units[ Form1.game.playerList[ adv ].unitList[ unit ].type ].name.ToLower() + " from the armies of " + Form1.game.playerList[ adv ].playerName + ".", "Intelligence" );
		}
		
		public static int selectRandomMilitaryUnit( byte adv )
		{
			Random r = new Random();
			int tot = 0;

			for ( int unit = 1; unit <= Form1.game.playerList[ adv ].unitNumber; unit ++ )
				if ( 
					Form1.game.playerList[ adv ].unitList[ unit ].state != (byte)Form1.unitState.dead &&  
					Statistics.units[ Form1.game.playerList[ adv ].unitList[ unit ].type ].speciality != enums.speciality.builder
					)
					tot ++;

			int chose = r.Next( tot );
			
			for ( int unit = 1; unit <= Form1.game.playerList[ adv ].unitNumber; unit ++ )
				if ( 
					Form1.game.playerList[ adv ].unitList[ unit ].state != (byte)Form1.unitState.dead &&  
					Statistics.units[ Form1.game.playerList[ adv ].unitList[ unit ].type ].speciality != enums.speciality.builder
					)
				{
					if ( chose == 0 )
					{
						return unit;
					}
					else
						chose --;
				}
			return -1;
		}
#endregion

#region science
		public static int selectRandomUndiscoveredTechno( byte player, byte adv )
		{
			Random r = new Random();
			int tot = 0;

			for ( int unit = 0; unit < Form1.game.playerList[ adv ].technos.Length; unit ++ )
				if ( 
					!Form1.game.playerList[ player ].technos[ unit ].researched &&
					Form1.game.playerList[ adv ].technos[ unit ].researched
					)
					tot ++;

			int chose = r.Next( tot );
			
			for ( int unit = 0; unit < Form1.game.playerList[ adv ].technos.Length; unit ++ )
				if ( 
					!Form1.game.playerList[ player ].technos[ unit ].researched &&
					Form1.game.playerList[ adv ].technos[ unit ].researched
					)
				{
					if ( chose == 0 )
						return unit;
					else
						chose --;
				}
			return -1;
		}
		
		public static int selectRandomHalfResearchedTechno( byte adv )
		{
			Random r = new Random();
			int tot = 0;

			for ( int unit = 0; unit < Form1.game.playerList[ adv ].technos.Length; unit ++ )
				if ( 
					Form1.game.playerList[ adv ].technos[ unit ].pntDiscovered > 0 &&
					!Form1.game.playerList[ adv ].technos[ unit ].researched
					)
					tot ++;

			int chose = r.Next( tot );
			
			for ( int unit = 0; unit < Form1.game.playerList[ adv ].technos.Length; unit ++ )
				if ( 
					Form1.game.playerList[ adv ].technos[ unit ].pntDiscovered > 0 &&
					!Form1.game.playerList[ adv ].technos[ unit ].researched
					)
				{
					if ( chose == 0 )
						return unit;
					else
						chose --;
				}
			return -1;
		}
		
		public static void stealTechno( byte player, byte adv )
		{
			int techno = selectRandomUndiscoveredTechno( player, adv );

			if ( techno != -1 )
			{
				Form1.game.playerList[ player ].technos[ techno ].researched = true;

				if ( Form1.game.playerList[ player ].currentResearch == techno )
				{
					if ( player == Form1.game.curPlayerInd )
					{
						uiWrap.chooseNextTechno( 
							player, 
							"Your spies succesfully stole " + Statistics.technologies[ Form1.game.playerList[ player ].currentResearch ].name.ToLower() + " from " + Statistics.civilizations[ Form1.game.playerList[ adv ].civType ].name + ".  Please chose your next research.", 
							"Intelligence" 
							);
					}
					else
					{
						ai Ai = new ai();
						Form1.game.playerList[ player ].currentResearch = ai.randomTechnology( player );
					}
				}

			}
		}
		
		public static void sabotageResearch( byte player, byte adv )
		{
			int techno = selectRandomHalfResearchedTechno( adv );
			
			if ( techno != -1 )
			{
				Random r = new Random();
				Form1.game.playerList[ adv ].technos[ techno ].pntDiscovered *= ( r.Next( 40 ) + 50 ) / 100;
			}
		}

#endregion

#region gov
		public static void stealMoney( byte player, byte adv ) 
		{ // agressive
			Random r = new Random();
			long amount = (Form1.game.playerList[ adv ].money * ( r.Next( 3 ) + 1 ) * Form1.game.playerList[ player ].foreignRelation[ adv ].spies[ (byte)enums.spyType.gov ].efficiency / ( 200 * 4 ));
			
			Form1.game.playerList[ adv ].money -= amount;
			Form1.game.playerList[ player ].money += amount;

			if ( player == Form1.game.curPlayerInd && Form1.options.autosave )
				MessageBox.Show( "Your spies have stolen " + amount.ToString() + " gold from " + Statistics.civilizations[ Form1.game.playerList[ adv ].civType ].name + ".", "Intelligence" );
			else if ( adv == Form1.game.curPlayerInd && Form1.options.autosave )
				MessageBox.Show( "An unknown spy has stolen " + amount.ToString() + " gold for you treasury.", "Intelligence" );
		} 
		
		public static void modOurRelation( byte player, byte adv )
		{ // passive
			Random r = new Random();
			aiRelations.modifyRelationQuatlityWith( adv, player, r.Next( 150 ) );
		}
		
		public static void modOtherRelations( byte player, byte adv )
		{ // agressive
			Random r = new Random();

			for ( byte p = 0; p < Form1.game.playerList.Length; p ++ )
				if ( Form1.game.playerList[ player ].foreignRelation[ p ].politic == (byte)Form1.relationPolType.war )
					aiRelations.modifyRelationQuatlityWith( adv, p, - 1 * r.Next( 150 ) );
				else if (
					Form1.game.playerList[ player ].foreignRelation[ p ].politic == (byte)Form1.relationPolType.alliance ||
					Form1.game.playerList[ player ].foreignRelation[ p ].politic == (byte)Form1.relationPolType.Protected ||
					Form1.game.playerList[ player ].foreignRelation[ p ].politic == (byte)Form1.relationPolType.Protector
					)
					aiRelations.modifyRelationQuatlityWith( adv, p, r.Next( 150 ) );
		}
#endregion

#region seeUnit
		public static void seeUnit( byte player, byte adv, int unit )
		{
			byte sight = Statistics.units[ Form1.game.playerList[ adv ].unitList[ unit ].type ].sight;
					
			if ( 
				Form1.game.grid[ Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ].type == (byte)enums.terrainType.mountain || 
				Form1.game.grid[ Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ].type == (byte)enums.terrainType.hill 
				)
				sight ++;

			for ( int j = 0; j <= sight; j ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( 
					Form1.game.playerList[ adv ].unitList[ unit ].X, 
					Form1.game.playerList[ adv ].unitList[ unit ].Y, 
					j 
					);

				for ( int k = 0; k < sqr.Length; k ++ )
				{
					Form1.game.playerList[ player ].see[ sqr[k ].X, sqr[ k ].Y ] = true;
					Form1.game.playerList[ player ].discovered[ sqr[k ].X, sqr[ k ].Y ] = true;

					if ( 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.sea || 
						Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.coast 
						)
					{
						Point[] sqr2 = Form1.game.radius.returnEmptySquare( sqr[ k ].X, sqr[ k ].Y, 1 );
						for ( int h = 0; h < sqr2.Length; h ++ )
						{
							Form1.game.playerList[ player ].see[ sqr2[ h ].X, sqr2[ h ].Y ] = true;
							Form1.game.playerList[ player ].discovered[ sqr2[ h ].X, sqr2[ h ].Y ] = true;
						}
					}
				}
			}
		}
		#endregion
		
#region go
		public static void go( byte player, Control parent1 )
		{
			parent = parent1;
			Random r = new Random();

			for ( byte i = 0; i < Form1.game.playerList.Length; i ++ )
				if ( 
					i != player && 
					!Form1.game.playerList[ i ].dead && 
					Form1.game.playerList[ player ].foreignRelation[ i ].madeContact 
					)
				{
					for ( int h = 0; h < Form1.game.playerList[ player ].foreignRelation[ i ].spies.Length; h ++ )
					{
						if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].nbr > 0 && Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency < 200 )
						{
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].state == 0 )
							{
								Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency ++;

								if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency < 200 )
									Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency ++;

								if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency < 200 )
									Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency ++;
							}
							else
								Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency ++;
						}
						else if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency > 0 )
						{
							Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency --;
							
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency > 0 )
								Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency --;

							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency > 0 )
								Form1.game.playerList[ player ].foreignRelation[ i ].spies[ h ].efficiency --;
						}
					}

				#region people
					byte j = (int)enums.spyType.people;

					if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].state == 0 )
					{ // passive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 500 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 50 )
									{
										civicSeeCity( player, i );
									}
									else if ( random < 100 )
									{
										civicSeeSettler( player, i );
									}
								}
					}
					else
					{ // agressive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 300 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 25 )
									{
										civicSeeCity( player, i );
									}
									else if ( random < 50 )
									{
										civicSeeSettler( player, i );
									}
									else if ( random < 75 )
									{
										civicStartRiot( player, i );
									}
									else if ( random < 100 )
									{
										civicSabotageConstruction( player, i );
									}
								}
					}
				#endregion

				#region military
					j = (byte)enums.spyType.military;

					if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].state == 0 )
					{ // passive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 500 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 100 )
									{
										militarySeeUnit( player, i );
									}
								}
					}
					else
					{ // agressive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 300 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 25 )
									{
										militarySabotageUnit( player, i );
									}
									if ( random < 100 )
									{
										militarySeeUnit( player, i );
									}
								}
					}
					#endregion

				#region gov
					j = (byte)enums.spyType.gov;

					if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].state == 0 )
					{ // passive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 5000 ) )
								if ( !counterIntResist( player, i, j ) )
								{
								/*	int random = r.Next( 100 );

									if ( random < 50 )
									{*/
										modOurRelation( player, i );
								/*	}
									else if ( random < 100 )
									{
										civicSeeSettler( player, i );
									}*/
								}
					}
					else
					{ // agressive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 3000 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 35 )
										modOurRelation( player, i );
									else if ( random < 75 )
										stealMoney( player, i );
									else if ( random < 100 )
										modOtherRelations( player, i );
								}
					}
				#endregion

				#region science
					j = (byte)enums.spyType.science;

					if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].state == 0 )
					{ // passive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 500 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 100 )
									{
										stealTechno( player, i );
									}
								}
					}
					else
					{ // agressive
						for ( int k = 0; k < Form1.game.playerList[ player ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr; k ++ )
							if ( Form1.game.playerList[ player ].foreignRelation[ i ].spies[ j ].efficiency + 50 > r.Next( 300 ) )
								if ( !counterIntResist( player, i, j ) )
								{
									int random = r.Next( 100 );

									if ( random < 75 )
									{
										stealTechno( player, i );
									}
									else if ( random < 100 )
									{
										sabotageResearch( player, i );
									}
								}
					}
				#endregion

				}
		}
	#endregion

	}
}
