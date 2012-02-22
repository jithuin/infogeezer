using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for PlayerList.
	/// </summary>
	public class PlayerList
	{
		public PlayerList( Game game, int player )
		{
			this.game = game;
			this.player = (byte)player;

			memory = new Memory( this, 0 );
			
			curCapital = -1; 
			for ( int i = 1; i <= cityNumber; i ++ ) 
				if ( 
					cityList[ i ].state != (byte)enums.cityState.dead && 
					cityList[ i ].isCapitale 
					) 
				{
					curCapital = i;
					break;
				}

			if ( curCapital == -1 )
				for ( int i = 1; i <= cityNumber; i ++ ) 
					if ( cityList[ i ].state != (byte)enums.cityState.dead )
					{
						capital = i; 
						break; 
					}

			slaves = new playerSlavery( this );
			govType = Statistics.governements[ (byte)enums.governements.despotism ];
			invalidateTrade();

			smallWonderList = new WonderList( Statistics.smallWonders.Length );
		}

	
		public PlayerList( Game game, byte player, byte civType, string playerName, sbyte prefFood, sbyte prefProd, sbyte prefTrade, sbyte prefScience, sbyte prefHapiness, sbyte prefWealth )
		{
			#region short
			
			this.game = game;
			this.player = (byte)player;

			memory = new Memory( this, 0 );
			
			curCapital = -1; 
			for ( int i = 1; i <= cityNumber; i ++ ) 
				if ( 
					cityList[ i ].state != (byte)enums.cityState.dead && 
					cityList[ i ].isCapitale 
					) 
				{
					curCapital = i;
					break;
				}

			if ( curCapital == -1 )
				for ( int i = 1; i <= cityNumber; i ++ ) 
					if ( cityList[ i ].state != (byte)enums.cityState.dead )
					{
						capital = i; 
						break; 
					}

			slaves = new playerSlavery( this );
			govType = Statistics.governements[ (byte)enums.governements.despotism ];
			invalidateTrade();

			#endregion

			lastSeen = new structures.lastSeen[ game.width, game.height ];

			this.civType = civType;
			this.playerName = playerName;
			money = 0;
			currentResearch = 0;

			unitList = new UnitList[ 10 ];
			cityList = new CityList[ 5 ];
			foreignRelation = new structures.sForeignRelation[ game.playerList.Length ];

			for ( int i = 0; i < game.playerList.Length; i ++ )
			{
				foreignRelation[ i ].quality = 100;
				foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
				
				for ( int j = 0; j < 4; j ++ )
				{
					foreignRelation[ i ].spies[ j ] = new xycv_ppc.structures.sSpies();
					foreignRelation[ i ].spies[ j ].nbr = 0;
				}
			}
	
			technos = new xycv_ppc.structures.technoList[ Statistics.technologies.Length ];

			unitNumber = 0;
			cityNumber = 0;
			technos[ 0 ].pntDiscovered = 0;
			
			preferences.laborFood = prefFood;
			preferences.laborProd = prefProd;
			preferences.laborTrade = prefTrade;
			preferences.science = prefScience;
			preferences.reserve = prefWealth;
			discovered = new bool[ game.width, game.height ];
			see = new bool[ game.width, game.height ];
			//	p.unitList = new UnitList[ 50 ];
			//	p.foreignRelation = new structures.sForeignRelation[ game.playerList.Length ];
			
			setResourcesAccess();
			//		p.memory = new Memory( this, 0 );

			smallWonderList = new WonderList( Statistics.smallWonders.Length );
		}

		public Game game;

		public playerSlavery slaves;
		public byte player;
		public byte civType;
		public string playerName;
		public byte currentResearch;
		public long money;

		public int unitNumber;
		public int cityNumber;
		public int cityNameUsed;

		public bool dead;
		public bool[,] discovered;
		public bool[,] see;
		//	public Form1.sUnitList[] unitList;
		public UnitList[] unitList;
		public CityList[] cityList;
		public structures.sForeignRelation[] foreignRelation;
		public byte peopleLove;

		public structures.technoList[] technos;
		public structures.preferences preferences;

		public WonderList smallWonderList;

		public Stat.Civilization civ
		{
			get
			{
				return Statistics.civilizations[ civType ];
			}
		}

#region gov
		public Stat.Governement govType;

		public bool isInRevolution
		{
			get
			{
				return govType.type == (byte)enums.governements.anarchy;
			}
		}
		public int turnsLeftToRevolution;

		public void initiateRevolution( Stat.Governement nextGov )
		{
			int turns = timeNeededToChangeTo( nextGov );
		}

		public int timeNeededToChangeTo( Stat.Governement nextGov)
		{
			int minTurns = 1, maxTurns = 5, turns = 1;

			switch ( nextGov.thisSupport )
			{
				case Stat.Governement.supportType.military :
					// many units
					float unitsPerCity = (float)totMilitaryUnits / (float)totCities;
					turns = maxTurns - (int)( Math.Pow( unitsPerCity, 2 ) / 3 ); // unitsPerCity = 0 -> 5, 1 -> 4, 2 -> 3, 3 -> 3, 0 -> 4, 16 -> 0
					break;

				case Stat.Governement.supportType.people :
					// small unified pop
					turns = 3;
					break;

				case Stat.Governement.supportType.religion :
					// high culture
					turns = 3;
					break;

				case Stat.Governement.supportType.wealth :
					// + slavery, wealthy/power

					int[] citiesWealth = new int[ cityNumber ];
					int totWealth = 0;
					for ( int c = 1; c <= cityNumber; c ++ )
						if ( !cityList[ c ].dead )
						{
							citiesWealth[ c ] = cityList[ c ].realTrade + cityList[ c ].realProduction;
							totWealth += cityList[ c ].realTrade + cityList[ c ].realProduction;
						}
						else
							citiesWealth[ c ] = -1;

					int[] order = count.descOrder( citiesWealth );
					int totWealthiest = 0;
					double percWealthiest = 0.25;

					for ( int i = 0; i < order.Length && i < cityNumber * percWealthiest && citiesWealth[ order[ i ] ] > 0; i++ )
						totWealthiest += citiesWealth[ order[ i ] ];

				//	if ( totWealthiest > totWealth / 3 ) // 1th of the nation should have 1 half of the wealth

					turns = minTurns + (int)(( maxTurns - minTurns ) * ( ( (double)totWealthiest / (double)totWealth ) - percWealthiest *(1-percWealthiest)));

					if ( economyType == (byte)enums.economies.slaveryRacial || economyType == (byte)enums.economies.slaverySocial )
						turns /= 2;

					break;
			}

			if ( turns > maxTurns )
				turns = maxTurns;

			if ( turns < minTurns )
				turns = minTurns;

			return turns;
		}

		/*public byte curGovType;
		public byte govType
		{
			get
			{
				return curGovType;
			}
			set
			{
				curGovType = value;
			}
		}*/
#endregion

#region eco
		public byte curEconomyType;
		public byte economyType
		{
			get
			{
				return curEconomyType;
			}
			set
			{
				if ( Statistics.economies[ curEconomyType ].supportSlavery && !Statistics.economies[ value ].supportSlavery )
					for ( int c = 1; c <= cityNumber; c ++ )
						if ( !cityList[ c ].dead )
							cityList[ c ].slaves.removeAll();

				curEconomyType = value;
			}
		}
#endregion

		public float foodModifier
		{
			get
			{
				return (float)govType.foodPerc * (float)Statistics.economies[ economyType ].food / ( 100 * 100 );
			}
		}
		public float prodModifier
		{
			get
			{
				return (float)govType.productionPerc * (float)Statistics.economies[ economyType ].prod / ( 100 * 100 );
			}
		}
		public float tradeModifier
		{
			get
			{
				return (float)govType.tradePerc * (float)Statistics.economies[ economyType ].trade / ( 100 * 100 );
			}
		}

		public int counterIntNbr;
		public int culture;

		public int globalHappiness;

		public structures.lastSeen[,] lastSeen;

#region resources
		public int[] resourcesDirectAcces;
		public int[] resourcesFromOtherNation;

		public bool hasAccessToResources( int resourceType )
		{
			return resourcesDirectAcces[ resourceType ] + resourcesFromOtherNation[ resourceType ] > 0;
		
			/*if ( resourcesDirectAcces[ resourceType ] + resourcesFromOtherNation[ resourceType ] > 0 ) 
				return true;
			else
				return false;*/
		}

		public bool canGiveResources(int resourceType)
		{
			return resourcesDirectAcces[ resourceType ] + resourcesFromOtherNation[ resourceType ] > 1;
		}

		public void setResourcesAccess()
		{
			resourcesDirectAcces = new int[ Statistics.resources.Length ];
			resourcesFromOtherNation = new int[ Statistics.resources.Length ];

			for ( int c = 1; c <= cityNumber; c ++ )
				if ( !cityList[ c ].dead )
					for ( int r = 0; r < Statistics.resources.Length; r++ )
						resourcesDirectAcces[ r ] += cityList[ c ].hdatr[ r ];

			int[,] exchanges = memory.getCurrentExchanges();

			for ( int r = 0; r < Statistics.resources.Length; r ++ )
				for ( int p = 0; p < game.playerList.Length; p ++ )
					resourcesFromOtherNation[ r ] += exchanges[ p, r + 1 ];

		/*	int goldPerTurn = 0;
			for ( int p = 0; p < game.playerList.Length; p ++ )
				goldPerTurn += exchanges[ p, 0 ];

			preferences.exchanges = (sbyte)((goldPerTurn * 100 / totalTrade));*/
		}
#endregion

		public Memory memory;
		public byte posInCityFile;
		int curCapital;

		public int capital 
		{ 
			get 
			{ 
				if ( curCapital == -1 ) 
				{ 
					for ( int i = 1; i <= cityNumber; i ++ ) 
						if ( 
							cityList[ i ].state != (byte)enums.cityState.dead && 
							cityList[ i ].isCapitale 
							) 
						{ 
							curCapital = i; 
							break;
						} 
					
					if ( curCapital == -1 ) 
					{
						System.Windows.Forms.MessageBox.Show( "No capital found", "Error" );

						for ( int i = 1; i <= cityNumber; i ++ ) 
							if ( cityList[ i ].state != (byte)enums.cityState.dead ) 
							{ 
								capital = i; 
								break;
							} 
					}
				}

				return curCapital;
			}
			set
			{
				for ( int i = 1; i <= cityNumber; i ++ ) 
					if ( cityList[ i ].state != (byte)enums.cityState.dead )
					{
						cityList[ i ].isCapitale = false;
					}

				cityList[ value ].isCapitale = true;
			}
		}

		public CityList capitalCity
		{
			get
			{
				return cityList[ capital ];
			}
		}
		
		
		public bool canBuildTransport
		{
			get
			{
				if ( technos[ (byte)Form1.technoList.coastalNavigation ].researched )
					return true;
				else
					return false;
			}
		}

#region food, prod and trade

		private int lastTotalTrade;
		private int lastTotalProd;
		private int lastTotalFood;
		private int lastTotalTradeTurn;
		private int lastTotalProdTurn;
		private int lastTotalFoodTurn;

		public void invalidateTrade()
		{
			lastTotalTradeTurn = -1;
			lastTotalProdTurn = -1;
			lastTotalFoodTurn = -1;
		}

		public int totalTrade
		{
			get
			{
				if ( lastTotalTradeTurn == game.curTurn )
				{
					return lastTotalTrade;
				}
				else
				{
					int tot = 1;

					for ( int c = 1; c <= cityNumber; c ++ )
						if ( cityList[ c ].state != (byte)enums.cityState.dead )
							tot += cityList[ c ].realTrade;

					int[,] exs = memory.getCurrentExchanges();
					int totExGoldPerTurn = 0;
					for ( int p = 0; p < game.playerList.Length; p ++ )
						totExGoldPerTurn += exs[ p, 0 ];

					lastTotalTrade = tot + totExGoldPerTurn;
					lastTotalTradeTurn = game.curTurn;

					preferences.exchanges = (sbyte)( totExGoldPerTurn * 100 / lastTotalTrade );
					aiPref.setReserve( player );

					return lastTotalTrade;
				}
			}
		}

		public int totalProduction
		{
			get
			{
				if ( lastTotalProdTurn == game.curTurn )
				{
					return lastTotalProd;
				}
				else
				{
					int tot = 1;

					for ( int c = 1; c <= cityNumber; c ++ )
						if ( cityList[ c ].state != (byte)enums.cityState.dead )
							tot += cityList[ c ].realProduction;

					lastTotalProd = tot;
					lastTotalProdTurn = game.curTurn;
					return tot;
				}
			}
		}

		public int totalFood
		{
			get
			{
				if ( lastTotalFoodTurn == game.curTurn )
				{
					return lastTotalFood;
				}
				else
				{
					int tot = 1;

					for ( int c = 1; c <= cityNumber; c ++ )
						if ( cityList[ c ].state != (byte)enums.cityState.dead )
							tot += cityList[ c ].realFood;

					lastTotalFood = tot;
					lastTotalFoodTurn = game.curTurn;
					return tot;
				}
			}
		}
#endregion

		public int currentCityCount
		{
			get
			{
				int tot = 0;
				for ( int c = 1; c <= cityNumber; c ++ )
					if ( cityList[ c ].state != (byte)enums.cityState.dead )
						tot ++;

				return tot;
			}
		}

		bool[] canBuildCityOnCont, canImproveOnCont;

#region sats
		public sats Sats;
#endregion

		public string civName
		{
			get
			{
				return Statistics.civilizations[ civType ].name;
			}
		}

		public int cityType 
		{ 
			get
			{
				if ( dead )
				{
					if ( technos[ (byte)Form1.technoList.steamPower ].researched )
						return 1;
					else
						return 0;
				}
				else if ( technos[ (byte)Form1.technoList.steamPower ].researched )
					return 1;
				else
					return 0;
			}
		} 
		
		public bool isInWar
		{
			get
			{
				for ( byte p = 0; p < game.playerList.Length; p++ )
					if ( 
						p != player && 
						foreignRelation[ p ].politic == (byte)Form1.relationPolType.war 
						)
						return true;

				return false;
			}
		}

		public bool usesSlavery
		{
			get
			{
				return ( economyType == (byte)enums.economies.slaveryRacial || economyType == (byte)enums.economies.slaverySocial );
			}
		}

		public bool canSwithToGov( enums.governements ind )
		{
			if ( ind == enums.governements.anarchy )
				return false;
			else
				return ( technos[ Statistics.governements[ (byte)ind ].neededTechno ].researched );
		}
		public bool canSwithToEco(int ind)
		{
		/*	if ( ind == (byte)enums.economies.natural )
				return false;
			else*/
				return ( technos[ Statistics.economies[ ind ].techno ].researched );
		}
		
		public int totMilitaryUnits
		{
			get
			{
				int un = 0;

				for ( int i = 1; i <= unitNumber; i++ )
					if ( 
						unitList[ i ].state != (byte)Form1.unitState.dead && 
						Statistics.units[ unitList[ i ].type ].speciality != enums.speciality.builder 
						)
						un++;

				return un;
			}
		}

		public int totCities
		{
			get
			{
				int un = 0;

				for ( int i = 1; i <= cityNumber; i++ )
					if ( !cityList[ i ].dead )
						un++;

				return un;
			}
		}

		#region wide settleing
/*
		bool[] caseImprovementPossibleOnEachCont,
			citiesPossibleOnEachCont;

		public void setWideSetteling()
		{
			caseImprovementPossibleOnEachCont = new bool[ 100 ];
			citiesPossibleOnEachCont = new bool[ 100 ];

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
				{
					if ( !game.grid[ x, y ].water )
						if ( 
							!citiesPossibleOnEachCont[ game.grid[ x, y ].continent - 1 ] &&
							game.grid[ x, y ].canBuildCity 
							)
						{
						}
						else if ( 
							!caseImprovementPossibleOnEachCont[ game.grid[ x, y ].continent - 1 ] //&&
						//	game.grid[ x, y ].canBuildCity 
							)
						{
						}
				}
		}
*/
		#endregion

		public bool isAtWarWith( PlayerList other )
		{
			return foreignRelation[ other.player ].politic == (byte)Form1.relationPolType.war;
		}

		public bool isAtPeaceWith( PlayerList other )
		{
			return !isAtWarWith( other ); //foreignRelation[ other.player ].politic == Form1.relationPolType.war;
		}

		public override bool Equals(object obj)
		{
			return player == ((PlayerList)obj).player;
		}

		public void kill( PlayerList killer )
		{
			dead = true;

			for ( int i = 1; i <= unitNumber; i ++ )
				if ( !unitList[ i ].dead )
					unitList[ i ].kill(); //unitDelete( ancientOwner, i );

			// safeguard
			for ( int i = 0; i < game.caseImps.Length; i ++ )
				if ( game.caseImps[ i ].owner == player )
					caseImprovement.destroyCaseImps( i );

			if ( killer.player == Form1.game.curPlayerInd )
				System.Windows.Forms.MessageBox.Show( 
					String.Format( language.getAString( language.order.killYouKilled ), new object[] { 
																										 playerName,
																										 Statistics.civilizations[ civType ].name
																									 }
					)
					//"You have destroyed " + playerName + " of " + Statistics.civilizations[ civType ].name + ".",  
					//"You won the war"
					);
			else if ( this.isCurPlayer )
				System.Windows.Forms.MessageBox.Show( 
					String.Format( language.getAString( language.order.killYouHaveBeenKilled ), new object[] { 
																										 killer.playerName,
																										 Statistics.civilizations[ killer.civType ].name
																									 }
					)
				//	playerName + " of " + Statistics.civilizations[ civType ].name + " has been destroyed...",  
				//	killer.playerName + " won the war"
					);
			else if ( 
				game.curPlayer.foreignRelation[ player ].madeContact || 
				game.curPlayer.foreignRelation[ killer.player ].madeContact 
				)
				System.Windows.Forms.MessageBox.Show( 
					String.Format( language.getAString( language.order.killHasBeenKilledBy ), new object[] { 
																										 playerName,
																										 Statistics.civilizations[ civType ].name,
																										 killer.playerName,
																										 Statistics.civilizations[ killer.civType ].name
																									 }
					)
				//	playerName + " of " + Statistics.civilizations[ civType ].name + " has been destroyed...",  
				//	killer.playerName + " won the war"
					);
		}
		
		public void aquireTechno()
		{
			aquireTechno( currentResearch );
		}
		public void aquireTechno( byte techno )
		{
			technos[ techno ].researched = true;
			//	Form1.game.playerList[ player ].technos[ Form1.game.playerList[ player ].currentResearch ].researched = true;

			if ( currentResearch == techno )
			{
			//	ai Ai = new ai();
				byte[] possibleTechnos = ai.returnDisponibleTechnologies( player );

				if ( player == Form1.game.curPlayerInd )
				{
					string technoName = Statistics.technologies[ currentResearch ].name;

					if ( possibleTechnos.Length > 0 )
					{
						byte nextTechno = ai.randomTechnology( player );
						string[] choices = uiWrap.technoListStrings( player, possibleTechnos );

						userChoice ui = new userChoice(
							language.getAString( language.order.uiNewTechnology ), 
							String.Format( language.getAString( language.order.uiYouJustDiscovered ), 
							technoName ), 
							choices,
							0,
							language.getAString( language.order.ok ), 
							language.getAString( language.order.uiOpenTechnoTree ) 
							);

						ui.ShowDialog();

						int res = ui.result;

						if ( res == -1 )
						{ // science tree
							currentResearch = nextTechno;

							sciTree sciTree1 = new sciTree();

							sciTree1.ShowDialog();
						}
						else
						{ // accept
							currentResearch = (byte)possibleTechnos[ res ];
						}
					}
					else
					{
						if ( currentResearch != 0 )
							System.Windows.Forms.MessageBox.Show( 
								String.Format( language.getAString( language.order.uiYouJustDiscoveredEverything ), Statistics.technologies[ Form1.game.playerList[ player ].currentResearch ].name ), 
								language.getAString( language.order.uiNewTechnology ) 
								);

						currentResearch = 0;
						technos[ currentResearch ].pntDiscovered = 0;
					}
				}
				else // cpu
				{
					if ( possibleTechnos.Length > 0 )
					{
						byte nextTechnos = ai.randomTechnology( player );

						currentResearch = nextTechnos;
					}
					else
					{
						currentResearch = 0;
						technos[ Form1.game.playerList[ player ].currentResearch ].pntDiscovered = 0;
					}
				}
			}
			/*	Form1.game.playerList[ player ].technos[ techno ].researched = true;

			if ( Form1.game.playerList[ player ].currentResearch == techno )
			{
				if ( player == Form1.game.curPlayerInd )
				{
					uiWrap.chooseNextTechno( 
						player, 
						"You just aquired " + Statistics.technologies[ Form1.game.playerList[ player ].currentResearch ].name.ToLower() + ".  Please choose your next research.", 
						"Technology aquired" 
						);
				}
				else
				{
					ai Ai = new ai();
					Form1.game.playerList[ player ].currentResearch = Ai.randomTechnology( player );
				}

			}*/
		}

		public CityList addCity()
		{
			return addCity( new CityList( this, cityNumber + 1 ) );
		}

		public CityList addCity( CityList city )
		{
			cityNumber ++;

			if ( cityNumber >= cityList.Length )
			{
				CityList[] cityListBuffer = cityList;
				cityList = new CityList[ cityListBuffer.Length + 5 ];

				for ( int i = 0; i < cityListBuffer.Length; i ++ )
					cityList[ i ] = cityListBuffer[ i ];
			}

			cityList[ cityNumber ] = city;

			return cityList[ cityNumber ];
		}

		public bool isCurPlayer
		{
			get
			{
				return game.curPlayerInd == player;
			}
		}

		public void createUnit( int x, int y, byte unitType )
		{
			if ( Statistics.units[ unitType ].speciality != enums.speciality.builder )
			{
				int mf = count.militaryFunding( player );
				int un = totMilitaryUnits; //count.militaryUnits( player );
				sbyte nextMFp = 0;
				bool mod = true;

				getPFT gp = new getPFT(); 

				if ( un == 0 ) 
				{ 
					nextMFp = (sbyte)( 3 * 100 / totalTrade + 1 ); 
				} 
				else // game.playerList[ player ].preferences.military 
				{ 
					if ( un <= mf / 3 ) // over yellow 
						nextMFp = (sbyte)( 3 * 100 * ( un + 1 ) / totalTrade + 1 ); 
					else if ( un <= mf * 6 / ( 3 * 5 ) ) // over red 
						nextMFp = (sbyte)( 3 * 100 * 4 * ( un + 1 ) / ( 5 * totalTrade ) + 1 ); 
					else 
						mod = false;
				}

				if ( mod )
					aiPref.setMilitary( player, nextMFp );
			}

			unitNumber ++;

			if ( unitNumber >= unitList.Length )
			{
				UnitList[] unitListBuffer = unitList;
				unitList = new UnitList[ unitListBuffer.Length + 10 ];

				for ( int i = 0; i < unitListBuffer.Length; i ++ )
					unitList[ i ] = unitListBuffer[ i ];
			}

			unitList[ unitNumber ] = new UnitList( game.playerList[ player ], unitNumber );

			unitList[ unitNumber ].X = x;
			unitList[ unitNumber ].Y = y;
			unitList[ unitNumber ].type = unitType;
			unitList[ unitNumber ].moveLeft = (sbyte)Statistics.units[ unitType].move;
			unitList[ unitNumber ].moveLeftFraction = 0;
			unitList[ unitNumber ].state = (byte)Form1.unitState.idle;
			unitList[ unitNumber ].health = 3;
			unitList[ unitNumber ].level = 1;

			// transport
			if ( Statistics.units[ unitType ].transport > 0 )
			{
				unitList[ unitNumber ].transport = new int[ Statistics.units[ unitType ].transport ];
				unitList[ unitNumber ].transported = 0;
			}

			move.moveUnitToCase( x, y, player, unitNumber );

			sight.discoverRadius( x, y, Statistics.units[ unitType ].sight, player );

			#region special resources
	/*		if ( game.grid[ x, y ].resources > 0 && game.grid[ x, y ].resources < 100 )
			{
				if ( game.grid[  x, y ].resources == (byte)enums.speciaResources.horses )
				{
					if ( !technos[ (byte)Form1.technoList.horseBreed ].researched )
					{
						technos[ (byte)Form1.technoList.horseBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered horses.", "Rare resources" );
						}
					}
				}
				else if ( game.grid[  x, y ].resources == (byte)enums.speciaResources.elephant )
				{
					if ( !technos[ (byte)Form1.technoList.elephantBreed ].researched )
					{
						technos[ (byte)Form1.technoList.elephantBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered elephants.", "Rare resources" );
						}
					}
				}
				else if ( game.grid[  x, y ].resources == (byte)enums.speciaResources.camel )
				{
					if ( !technos[ (byte)Form1.technoList.camelBreed ].researched )
					{
						technos[ (byte)Form1.technoList.camelBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered camels.", "Rare resources" );
						}
					}
				}

				game.grid[  x, y ].resources = 0;
			}*/
			#endregion

		}
	}
}
