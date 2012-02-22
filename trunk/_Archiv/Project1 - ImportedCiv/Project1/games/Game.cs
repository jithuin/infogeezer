using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Game.
	/// </summary>
	public class Game
	{
		public structures.caseImprovement[] caseImps;

		public PlayerList[] playerList;
		public Case[,] grid;
		public int width, height, curTurn;
		public byte	difficulty;

	//  public PlayerList humanPlayer;
		public string savePath;

		// classes / tools
		public Radius radius;
		public Plans plans;
		public Frontier frontier;
		public Rvtbc rvtbc;
		// end tools

		public byte curPlayerInd = 0;
		public PlayerList curPlayer
		{
			get
			{
				return playerList[ curPlayerInd ];
			}
		}

		public State state = State.ok;

		public Game()
		{
			radius = new Radius( this );
			plans = new Plans( this );
			frontier = new Frontier( this );
		}

		public Game( int width, int height )
		{
			radius = new Radius( this );
			plans = new Plans( this );
			frontier = new Frontier( this );

			this.width = width;
			this.height = height;

			grid = new Case[ width, height ];

			for ( int x = 0; x < width; x ++ )
				for ( int y = 0; y < height; y ++ )
					grid[ x, y ] = new Case( x, y );
		}

		public void initGrid( int width, int height )
		{
			this.width = width;
			this.height = height;

			grid = new Case[ width, height ];

			for ( int x = 0; x < width; x ++ )
				for ( int y = 0; y < height; y ++ )
					grid[ x, y ] = new Case( x, y );
		}
		
#region new game
	/*	public static Game newGame(int width, int height, byte percWater, byte nbrPlayer1, string playerName, byte civ, byte difficulty1, byte contSize, byte age, bool isInCenter, bool niceSite, bool tut )
		{ 
			return newGame( 
				width, height, percWater, nbrPlayer1, playerName, civ, difficulty1, contSize, age, isInCenter, niceSite, tut, 
				false, "", "", 0, Color.Empty, null 
				);
		}
		public static Game newGame( int width, int height, byte percWater, byte nbrPlayer1, string playerName, byte civ, byte difficulty1, byte contSize, byte age, bool isInCenter, bool niceSite, bool tut, bool customNation, string customNationName, string customNationDesc, byte customNationCityList, Color customNationColor, byte[] aiCivs )
		{
			Game game = new Game();

			showProgress.showProgressInd = true;
			showProgress.lblInfo = "Loading statistics and bitmaps...";
			showProgress.progressBar = 1;

			wC.show = true; 

			if ( customNation )
			{
				Stat.Civilization[] buffer = Statistics.civilizations;
				Statistics.civilizations = new Stat.Civilization[ buffer.Length + 1 ];

				for ( int i = 0; i < buffer.Length; i ++ )
					Statistics.civilizations[ i ] = buffer[ i ];
				
				Statistics.civilizations[ buffer.Length ].name = customNationName;
				Statistics.civilizations[ buffer.Length ].description = customNationDesc;
				Statistics.civilizations[ buffer.Length ].cityNames = Statistics.civilizations[ customNationCityList ].cityNames;
				Statistics.civilizations[ buffer.Length ].color = customNationColor;

				civ = (byte)buffer.Length;
			}

			Tutorial.init( tut );
			//	tutorialMode = tutorial; 
			game.difficulty = difficulty1; 
			game.savePath = ""; 

			game.curTurn = 0;

			//	cfgOut cfgO = new cfgOut(); 
			//	structures.cfgFile cfgFile = cfgO.readCfgFile(); 
			Form1.options.showBatteryStatus = cfgOut.getValues.batteryStatus; 

			Form1.options.showOnScreenDPad = cfgOut.getValues.dPad;
			Form1.options.miniMapType = 1;
			Form1.options.frontierType = 1;

			Form1.options.showGrid = cfgOut.getValues.showGrid;

			Form1.options.hideUndiscovered = true;

			//		game.playerList.Length = nbrPlayer1;


		/*	if ( !Statistics.variableInitialized )
			{
				showProgress.lblInfo = "Loading graphics...";
				Form1.InitGraphics();

				Statistics.initAllBut();
			}*

			showProgress.lblInfo = "Generating map...";
			game.width = width;
			game.height = height;
			game.grid = new structures.sGrid[ game.width, game.height ];
			
			game.caseImps = new structures.caseImprovement[ 0 ];
			
			showProgress.progressBar = 10;

			mapGenerator mapGen = new mapGenerator( game );
			if ( mapGen.generateMap( width, height, percWater, contSize, age ) )
			{
				showProgress.progressBar = 50;
				showProgress.lblInfo = "Locating continents...";

				mapGen.findContinents( width, height );
				int[] contSize1 = mapGen.getContSize();

				showProgress.progressBar = 57;
				showProgress.lblInfo = "Set specials...";
				mapGen.setSpecials();

				showProgress.progressBar = 59;
				showProgress.lblInfo = "Set resources...";
				mapGen.setResources( width * 7 * ( 100 - percWater ) / ( 40 * 100 ) );

				showProgress.progressBar = 62;
				showProgress.lblInfo = "Set rivers...";
				mapGen.setRivers( 10 * ( width / 40 ) ^ 2 * ( 100 - percWater ) / 100 );

				showProgress.progressBar = 65;
				showProgress.lblInfo = "Finished generating map"; // Map generated"; // "";

				for (int x = 0; x < game.width; x ++ )
					for (int y = 0; y < game.height; y ++ )
						game.grid[ x, y ].stack = new UnitList[ 0 ];
			
				Random R = new Random();
				game.radius.setIsNextToWater(); 

				#region generer les case cotiere possible
				Point[] possibleCases = new Point[ width * 8 * height / 10 ];
				int d = 0;

				for ( int x = 0; x < width; x ++)
					for ( int y = height / 7; y < 6 * height / 7; y ++)
					{
						if ( 
							game.grid[ x, y ].type == (byte)enums.terrainType.plain && 
							game.radius.isNextToWater( x, y ) &&
							game.grid[ x, y ].resources == 0
							)
						{
							possibleCases[ d ] = new Point( x, y );
							d ++;
						}
					}

				Point[] possibleCases1 = new Point[ d ];

				for ( int i = 0; i < possibleCases1.Length; i ++ )
				{
					possibleCases1[ i ] = possibleCases[ i ];
				}
				#endregion
			
				showProgress.progressBar = 70;
				//	showProgress.lblInfo = 

				game.playerList = new PlayerList[ nbrPlayer1 ]; // game.playerList.Length ];

				//						nbr		civ		name			food	prod	trade	Sci		happy	wealth
	//			game.enterNewPlayer(	0,		civ,	playerName,		1,		1,		1,		50,		1,		50		);

				#region players
				bool foundStart = false;
				for ( int j = 0; !foundStart; j ++ )
				{
					Point startPos = possibleCases1[ R.Next( possibleCases1.Length ) ];

					if ( 
						!isInCenter || 
						( startPos.X > game.width / 5 && startPos.X < game.width * 4 / 5 )
						)
					{
						if ( niceSite )
						{
							if ( contSize1[ game.grid[ startPos.X, startPos.Y ].continent ] > 100 )
							{
								Point[] sqr = game.radius.returnSquare( startPos.X, startPos.Y, 4 );
								int tot = 0;

								for ( int k = 0; k < sqr.Length; k ++ )
									if ( 
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent == game.grid[ startPos.X, startPos.Y ].continent && 
										(
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.glacier ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.jungle ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.mountain ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.swamp ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.tundra ||
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.desert
										)
										)
									{
										if ( k > 8 )
											tot ++; 
										else
										{
											tot += 2000;
											break;
										}

									}

								if ( tot < 5 + j * 2 )
								{
					//				unitCreate( game, startPos.X, startPos.Y, 0, (byte)Form1.unitType.colon );
									foundStart = true;
								}
							}
						}
						else
						{
							if ( contSize1[ game.grid[ startPos.X, startPos.Y ].continent ] > 30 )
							{
					//			unitCreate( game, startPos.X, startPos.Y, 0, (byte)Form1.unitType.colon );
								foundStart = true;
							}
						}
					}

					if ( j > possibleCases1.Length * 2 ) // 3
						return null;
					//	MessageBox.Show( "Failed finding" );
				}
				#endregion

				showProgress.progressBar = 70 + 5;
				showProgress.lblInfo = "Player info...";

				for ( byte i = 1; i < game.playerList.Length; i ++ )
				{
					#region ai players

					int civType = 0;
					if ( aiCivs == null || aiCivs.Length == 0 ) // )
					{
						bool found = false;
						while ( !found )
						{
							civType = (byte)R.Next( Statistics.normalCivilizationNumber );
							found = true;
							for ( int k = 0; k < i; k++ )
							{
								if ( civType == game.playerList[ k ].civType )
								{
									found = false;
									break;
								}
							}
						}
					}
					else
					{
						civType = aiCivs[ i - 1 ];
					}

					Random r = new Random();
					game.enterNewPlayer( i, (byte)civType, Statistics.civilizations[ civType ].leaderNames[ r.Next( Statistics.civilizations[ civType ].leaderNames.Length ) ], 1, 1, 1, 50, 0, 50 );

					foundStart = false;
					for ( int j = 0; !foundStart; j ++ )
					{
						if ( j > 1000 )
						{
							int aiOld = game.playerList.Length - 1;
							int aiNew = i - 1;
							//			game.playerList.Length = (byte)( i );

							PlayerList[] buffer = game.playerList;
							game.playerList = new PlayerList[ game.playerList.Length + 1 ];

							for ( int k = 0; k < game.playerList.Length; k ++ )
								game.playerList[ k ] = buffer[ k ];

							MessageBox.Show(
								"Due to the specification of the terrain the number of AIs has been reduced from " + aiOld.ToString() + " to " + aiNew.ToString() + ".", 
								"Map generation exception"
								); 
 
							break; 
						}

						Point startPos = possibleCases1[ R.Next( possibleCases1.Length ) ];

						if ( game.grid[ startPos.X, startPos.Y ].stack.Length == 0 )
						{
							if ( contSize1[ game.grid[ startPos.X, startPos.Y ].continent ] > 30 )
							{
								Point[] sqr = game.radius.returnSquare( startPos.X, startPos.Y, 7 );
								bool foundUnit = false;

								for ( int k = 0; k < sqr.Length; k ++ )
									if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length > 0 )
									{
										foundUnit = true;
										break;
									}

								if ( !foundUnit )
								{
									game.unitCreate( startPos.X, startPos.Y, i, (byte)Form1.unitType.colon );
									foundStart = true;
								}
							}
						}
					
					}
					#endregion
				
					showProgress.progressBar = 70 + 5 + i * ( 20 / game.playerList.Length );
					showProgress.lblInfo = "AI info generated";
				}

				label.initList( 0 );

				rvtbc.init(); 
				plans.init();
				Tutorial.init( tut );

				showProgress.progressBar = 95; 
				showProgress.lblInfo = "Finalizing"; 

				return game; 
			}
			else
			{
				showProgress.showProgressInd = false;
				wC.show = false;
				MessageBox.Show( "Generation failed, try more or less water." );
				return null;
			}
		}

		#region  enter new player
		
		public void enterNewPlayer( byte playerNbr, byte civType, string playerName, sbyte prefFood, sbyte prefProd, sbyte prefTrade, sbyte prefScience, sbyte prefHapiness, sbyte prefWealth )
		{
			this.playerList[ playerNbr ] = new PlayerList( playerNbr ); 
			this.playerList[ playerNbr ].lastSeen = new structures.lastSeen[ this.width, this.height ];

			this.playerList[ playerNbr ].civType = civType;
			this.playerList[ playerNbr ].playerName = playerName;
			this.playerList[ playerNbr ].money = 0;
			this.playerList[ playerNbr ].currentResearch = 0;

			this.playerList[ playerNbr ].unitList = new UnitList[ 10 ];
			this.playerList[ playerNbr ].cityList = new CityList[ 5 ];
			this.playerList[ playerNbr ].foreignRelation = new structures.sForeignRelation[ this.playerList.Length ];

			for ( int i = 0; i < this.playerList.Length; i ++ )
			{
				this.playerList[ playerNbr ].foreignRelation[ i ].quality = 100;
				this.playerList[ playerNbr ].foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
				
				for ( int j = 0; j < 4; j ++ )
				{
					this.playerList[ playerNbr ].foreignRelation[ i ].spies[ j ] = new xycv_ppc.structures.sSpies();
					this.playerList[ playerNbr ].foreignRelation[ i ].spies[ j ].nbr = 0;
				}
			}
	
			this.playerList[ playerNbr ].technos = new xycv_ppc.structures.technoList[ Statistics.technologies.Length ];

			this.playerList[ playerNbr ].unitNumber = 0;
			this.playerList[ playerNbr ].cityNumber = 0;
			this.playerList[ playerNbr ].technos[ 0 ].pntDiscovered = 0;
			
			this.playerList[ playerNbr ].preferences.laborFood = prefFood;
			this.playerList[ playerNbr ].preferences.laborProd = prefProd;
			this.playerList[ playerNbr ].preferences.laborTrade = prefTrade;
			this.playerList[ playerNbr ].preferences.science = prefScience;
			this.playerList[ playerNbr ].preferences.reserve = prefWealth;
			this.playerList[ playerNbr ].discovered = new bool[ this.width, this.height ];
			this.playerList[ playerNbr ].see = new bool[ this.width, this.height ];
			//	this.playerList[ playerNbr ].unitList = new UnitList[ 50 ];
			//	this.playerList[ playerNbr ].foreignRelation = new structures.sForeignRelation[ Form1.this.playerList.Length ];
			
			this.playerList[ playerNbr ].setResourcesAccess();
			this.playerList[ playerNbr ].memory = new Memory( playerNbr, 0 );
		}

		#endregion
		
		#region unit Creation
		public void unitCreate( int x, int y, byte player, byte unitType )
		{
			if ( Statistics.units[ unitType ].speciality != enums.speciality.builder )
			{
				int mf = count.militaryFunding( player );
				int un = Form1.game.playerList[ player ].totMilitaryUnits; //count.militaryUnits( player );
				sbyte nextMFp = 0;
				bool mod = true;

		//		getPFT gp = new getPFT(); 

				if ( un == 0 ) 
				{ 
					nextMFp = (sbyte)( 3 * 100 / Form1.game.playerList[ player ].totalTrade + 1 ); 
				}
				else // game.playerList[ player ].preferences.military 
				{ 
					if ( un <= mf / 3 ) // over yellow 
						nextMFp = (sbyte)( 3 * 100 * ( un + 1 ) / Form1.game.playerList[ player ].totalTrade + 1 ); 
					else if ( un <= mf * 6 / ( 3 * 5 ) ) // over red 
						nextMFp = (sbyte)( 3 * 100 * 4 * ( un + 1 ) / ( 5 * Form1.game.playerList[ player ].totalTrade ) + 1 ); 
					else 
						mod = false;
				}

				if ( mod )
					aiPref.setMilitary( player, nextMFp );
			}

			this.playerList[ player ].unitNumber ++;

			if ( this.playerList[ player ].unitNumber >= this.playerList[ player ].unitList.Length )
			{
				UnitList[] unitListBuffer = this.playerList[ player ].unitList;
				this.playerList[ player ].unitList = new UnitList[ unitListBuffer.Length + 10 ];

				for ( int i = 0; i < unitListBuffer.Length; i ++ )
					this.playerList[ player ].unitList[ i ] = unitListBuffer[ i ];
			}

			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ] = new UnitList( player );

			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].X = x;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].Y = y;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].type = unitType;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].moveLeft = (sbyte)Statistics.units[ unitType].move;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].moveLeftFraction = 0;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].state = (byte)Form1.unitState.idle;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].health = 3;
			this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].level = 1; 

			// transport
			if ( Statistics.units[ unitType ].transport > 0 )
			{
				this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].transport = new int[ Statistics.units[ unitType ].transport ];
				this.playerList[ player ].unitList[ this.playerList[ player ].unitNumber ].transported = 0;
			}		

			move.moveUnitToCase( x, y, player, this.playerList[ player ].unitNumber );

		//	discoverRadius( x, y, Statistics.units[ unitType ].sight, player );

			#region special resources
			if ( this.grid[ x, y ].resources > 0 && this.grid[ x, y ].resources < 100 )
			{
				if ( this.grid[  x, y ].resources == (byte)enums.speciaResources.horses )
				{
					if ( !this.playerList[ player ].technos[ (byte)Form1.technoList.horseBreed ].researched )
					{
						this.playerList[ player ].technos[ (byte)Form1.technoList.horseBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered horses.", "Rare resources" );
						}
					}
				}
				else if ( this.grid[  x, y ].resources == (byte)enums.speciaResources.elephant )
				{
					if ( !this.playerList[ player ].technos[ (byte)Form1.technoList.elephantBreed ].researched )
					{
						this.playerList[ player ].technos[ (byte)Form1.technoList.elephantBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							DrawMap();
							MessageBox.Show( "You just discovered elephants.", "Rare resources" );
						}
					}
				}
				else if ( this.grid[  x, y ].resources == (byte)enums.speciaResources.camel )
				{
					if ( !this.playerList[ player ].technos[ (byte)Form1.technoList.camelBreed ].researched )
					{
						this.playerList[ player ].technos[ (byte)Form1.technoList.camelBreed ].researched = true;

						if ( Form1.game.curPlayerInd == player )
						{
							Form1.DrawMap();
							MessageBox.Show( "You just discovered camels.", "Rare resources" );
						}
					}
				}

				this.grid[  x, y ].resources = 0;
			}
			#endregion

		}*/
		#endregion

#region load game
		public static Game load( string sp )
		{
			Game game = null; // = new Game();
			FileStream file = null;
			BinaryReader binReader = null;
			bool success = false;

			try
			{	
			/*	if ( true )
				{	*/
				
					Form1.selected = new structures.sSelected();

					file = new FileStream( sp, FileMode.Open, FileAccess.Read ); 
					binReader = new BinaryReader( file ); 

					int buildingNbr,
						tempTechnoNbr,
						tutorialLength,
						nbrPlayer,
						version = binReader.ReadInt32();

					/* removed:
							 * 083
							 * 0831
							 * 0840
							 * 0842
							 * 0857
							 * 0862
							 * 0863
							 * 0864
							 * 0865
							 */

				/*	if ( got < 871 )
					{
						wC.show = false;

						MessageBox.Show( 
							String.Format( language.getAString( language.order.errorUnrecongnisedSaveVersion ), (double)got / 1000 ), 
							language.getAString( language.order.errorTitle ) 
							);
						
						return null;
					}
					else */if ( version == 871 )
					{
						#region case 0871

						Form1.sliHor = binReader.ReadInt32();
						Form1.sliVer = binReader.ReadInt32();
						int width = binReader.ReadInt32(),
							height = binReader.ReadInt32();

						game = new Game( width, height );

						game.curTurn = binReader.ReadInt32();
						Form1.options.frontierType = (Options.FrontierTypes)binReader.ReadByte(); // option
						Form1.options.miniMapType = (Options.MiniMapTypes)binReader.ReadByte(); // option
						buildingNbr = binReader.ReadInt32();
						tempTechnoNbr = binReader.ReadInt32();
						Form1.options.hideUndiscovered = binReader.ReadBoolean(); // option
						Form1.options.showGrid = binReader.ReadBoolean(); // option
						Form1.options.showOnScreenDPad = binReader.ReadBoolean(); // option
						Form1.options.showBatteryStatus = binReader.ReadBoolean(); // option
						Form1.options.showLabels = binReader.ReadBoolean(); // option
						Form1.options.showCommonSpyDialogs = binReader.ReadBoolean(); // option
						Form1.options.autosave = binReader.ReadBoolean();  // option

						// temp
						structures.sStack[][][] tempStacks = new xycv_ppc.structures.sStack[ game.width ][][];

						for ( int x = 0; x < game.width; x ++ )
							tempStacks[ x ] = new xycv_ppc.structures.sStack[ game.height ][];

						#region game.grid
						//	game.grid = new structures.sGrid[ game.width, game.height ];
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
							{
								game.grid[ x, y ].type = binReader.ReadByte();
								if ( game.grid[ x, y ].type != (byte)enums.terrainType.sea && game.grid[ x, y ].type != (byte)enums.terrainType.coast )
								{
									if ( binReader.ReadBoolean() )
										game.grid[ x, y ].city = binReader.ReadInt32();
									else
										game.grid[ x, y ].city = 0;

									game.grid[ x, y ].civicImprovement = binReader.ReadByte();
									game.grid[ x, y ].militaryImprovement = binReader.ReadByte();
									game.grid[ x, y ].roadLevel = binReader.ReadByte();
									game.grid[ x, y ].continent = binReader.ReadByte();
									if ( binReader.ReadBoolean() )
										game.grid[ x, y ].resources = binReader.ReadByte();

									game.grid[ x, y ].river = binReader.ReadBoolean(); // 863 
									if ( game.grid[ x, y ].river )
									{
										game.grid[ x, y ].riversDir = new bool[ 8 ];
										for ( int v = 0; v < game.grid[ x, y ].riversDir.Length; v++ )
											game.grid[ x, y ].riversDir[ v ] = binReader.ReadBoolean();
									}
								}

								if ( binReader.ReadBoolean() )
									game.grid[ x, y ].laborCity = binReader.ReadInt32();
								else
									game.grid[ x, y ].laborCity = 0;

								if ( binReader.ReadBoolean() )
								{
									
									tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ binReader.ReadInt32() ];

									game.grid[ x, y ].stack = new UnitList[ tempStacks[ x ][ y ].Length ];
									game.grid[ x, y ].stackPos = binReader.ReadInt32();
								}
								else
								{
									
									tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ 0 ];

									game.grid[ x, y ].stack = new UnitList[ 0 ];
									game.grid[ x, y ].stackPos = 0;
								}
							}
						#endregion

						#region stack
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
								{
									tempStacks[ x ][ y ][ i - 1 ].owner = binReader.ReadByte();
									tempStacks[ x ][ y ][ i - 1 ].unit = binReader.ReadInt32();

									/*	game.grid[ x, y ].stack[ i - 1 ].player.player = binReader.ReadByte();
										game.grid[ x, y ].stack[ i - 1 ].unit = binReader.ReadInt32();*/
								}
						#endregion

						game.difficulty = binReader.ReadByte();
						nbrPlayer = binReader.ReadByte();
						game.playerList = new PlayerList[ nbrPlayer ]; 

						#region players # # #
						for ( int j = 0; j < nbrPlayer; j++ )
						{
							game.playerList[ j ] = new PlayerList( game, j );
							game.playerList[ j ].playerName = binReader.ReadString();
							game.playerList[ j ].civType = binReader.ReadByte();
							game.playerList[ j ].cityNumber = binReader.ReadInt32();
							game.playerList[ j ].unitNumber = binReader.ReadInt32();
							game.playerList[ j ].dead = binReader.ReadBoolean();
							if ( game.playerList[ j ].dead )
							{
								game.playerList[ j ].cityNumber = 0;
								game.playerList[ j ].unitNumber = 0;
							}

							if ( !game.playerList[ j ].dead )
							{
								game.playerList[ j ].currentResearch = binReader.ReadByte();
								game.playerList[ j ].money = binReader.ReadInt64();
								game.playerList[ j ].preferences.laborFood = binReader.ReadSByte();
								game.playerList[ j ].preferences.laborProd = binReader.ReadSByte();
								game.playerList[ j ].preferences.laborTrade = binReader.ReadSByte();
								game.playerList[ j ].preferences.science = binReader.ReadSByte();
								game.playerList[ j ].preferences.reserve = binReader.ReadSByte();
								game.playerList[ j ].preferences.buildings = binReader.ReadSByte();
								game.playerList[ j ].preferences.culture = binReader.ReadSByte();
								game.playerList[ j ].preferences.intelligence = binReader.ReadSByte();
								game.playerList[ j ].preferences.military = binReader.ReadSByte();
								game.playerList[ j ].preferences.space = binReader.ReadSByte();
								game.playerList[ j ].counterIntNbr = binReader.ReadInt32();
								game.playerList[ j ].govType = Statistics.governements[ binReader.ReadByte() ];
								game.playerList[ j ].economyType = binReader.ReadByte();
								game.playerList[ j ].posInCityFile = binReader.ReadByte();
								game.playerList[ j ].technos = new structures.technoList[ Statistics.technologies.Length ];
								game.playerList[ j ].discovered = new bool[ game.width, game.height ];
								game.playerList[ j ].see = new bool[ game.width, game.height ];
								game.playerList[ j ].unitList = new UnitList[ game.playerList[ j ].unitNumber + 10 ];
								game.playerList[ j ].cityList = new CityList[ game.playerList[ j ].cityNumber + 10 ];
								game.playerList[ j ].foreignRelation = new structures.sForeignRelation[ nbrPlayer ]; 
					
								#region foreignRelations
								for ( int i = 0; i < nbrPlayer; i++ )
								{
									game.playerList[ j ].foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
									for ( int k = 0; k < 4; k++ )
										game.playerList[ j ].foreignRelation[ i ].spies[ k ] = new xycv_ppc.structures.sSpies();

									game.playerList[ j ].foreignRelation[ i ].madeContact = binReader.ReadBoolean();
									if ( game.playerList[ j ].foreignRelation[ i ].madeContact )
									{
										game.playerList[ j ].foreignRelation[ i ].quality = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].politic = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].embargo = binReader.ReadBoolean();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].efficiency = binReader.ReadByte();
									}
								}
								#endregion

								#region see & discovered
								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										game.playerList[ j ].see[ x, y ] = binReader.ReadBoolean();

								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										if ( !game.playerList[ j ].see[ x, y ] )
											game.playerList[ j ].discovered[ x, y ] = binReader.ReadBoolean();
										else
											game.playerList[ j ].discovered[ x, y ] = true;
								#endregion
							
								#region last seen game.grid
							
								game.playerList[ j ].lastSeen = new structures.lastSeen[ game.width, game.height ];
								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										if ( !game.playerList[ j ].see[ x, y ] && game.playerList[ j ].discovered[ x, y ] )
										{
											if ( Statistics.terrains[ game.grid[ x, y ].type ].ew == 1 )
											{
												game.playerList[ j ].lastSeen[ x, y ].cityPop = binReader.ReadByte();
												if ( game.playerList[ j ].lastSeen[ x, y ].cityPop > 0 )
													game.playerList[ j ].lastSeen[ x, y ].city = binReader.ReadInt32();

												game.playerList[ j ].lastSeen[ x, y ].civicImp = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].militaryImp = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].road = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].territory = binReader.ReadByte();
											}
											else if ( game.grid[ x, y ].type == (byte)enums.terrainType.coast )
											{
												game.playerList[ j ].lastSeen[ x, y ].territory = binReader.ReadByte();
											}

											game.playerList[ j ].lastSeen[ x, y ].turn = binReader.ReadInt32();
										}
								#endregion

								#region techno
								for ( int i = 0; i < tempTechnoNbr; i++ )
								{
									byte num = binReader.ReadByte();

									if ( i < Statistics.technologies.Length )
										switch ( num )
										{
											case 1 :
												if ( i < game.playerList[ j ].technos.Length )
													game.playerList[ j ].technos[ i ].researched = true;

												break;

											case 2 :
												if ( i < game.playerList[ j ].technos.Length )
												{
													game.playerList[ j ].technos[ i ].researched = false;
													game.playerList[ j ].technos[ i ].pntDiscovered = binReader.ReadInt32();
												}
												else
												{
													binReader.ReadInt32();
												}

												break;

											default :
												if ( i < game.playerList[ j ].technos.Length )
													game.playerList[ j ].technos[ i ].researched = false;

												break;
										}
								}
								#endregion

								#region units
								for ( int i = 1; i <= game.playerList[ j ].unitNumber; i++ )
								{
									game.playerList[ j ].unitList[ i ] = new UnitList(  game.playerList[ j ], i );
									game.playerList[ j ].unitList[ i ].state = binReader.ReadByte();
									if ( game.playerList[ j ].unitList[ i ].state != (byte)Form1.unitState.dead )
									{
										game.playerList[ j ].unitList[ i ].type = binReader.ReadByte();
										game.playerList[ j ].unitList[ i ].health = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].level = binReader.ReadByte();
										game.playerList[ j ].unitList[ i ].moveLeft = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].moveLeftFraction = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].transported = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].X = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].Y = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].dest.X = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].dest.Y = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].transport = new int[ Statistics.units[ game.playerList[ j ].unitList[ i ].type ].transport ];
										for ( int k = 0; k < game.playerList[ j ].unitList[ i ].transported; k++ )
											game.playerList[ j ].unitList[ i ].transport[ k ] = binReader.ReadInt32();

										game.playerList[ j ].unitList[ i ].automated = binReader.ReadBoolean(); //842
									}
								}
								#endregion

								#region cities
								for ( int i = 1; i <= game.playerList[ j ].cityNumber; i++ )
								{
									game.playerList[ j ].cityList[ i ] = new CityList( game.playerList[ j ], i );
									game.playerList[ j ].cityList[ i ].state = binReader.ReadByte();
									game.playerList[ j ].cityList[ i ].name = binReader.ReadString();
									if ( game.playerList[ j ].cityList[ i ].state != (byte)enums.cityState.dead )
									{
										game.playerList[ j ].cityList[ i ].construction.points = binReader.ReadInt32();

										// 0862
										for ( int k = 0; k < 10; k++ )
										{
											int cType =  binReader.ReadInt32();
											byte type = binReader.ReadByte();

											if ( cType != -1 )
												switch ( cType )
												{
													case 0:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
														break;

													case Stat.Unit.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
														break;
												
													case Stat.Building.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.buildings[ type ];
														break;
												
													case Stat.Wealth.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
														break;
												
											/*		case Stat.SmallWonder.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.smallWonders[ type ];
														break;
												
													case Stat.Wonder.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.wonders[ type ];
														break;*/

													default:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
														break;
												}
										//	game.playerList[ j ].cityList[ i ].construction.list[ k ].ind = binReader.ReadInt32();
										//	game.playerList[ j ].cityList[ i ].construction.list[ k ].type = binReader.ReadByte();	
										/*	game.playerList[ j ].cityList[ i ].construction.list[ k ].ind = binReader.ReadInt32();
											game.playerList[ j ].cityList[ i ].construction.list[ k ].type = binReader.ReadByte();	*/
										}

										// end 0862
										game.playerList[ j ].cityList[ i ].foodReserve = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].laborOnField = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].originalOwner = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].population = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].X = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].Y = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].laborPos = new Point[ game.playerList[ j ].cityList[ i ].laborOnField ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].laborOnField; k++ )
										{
											game.playerList[ j ].cityList[ i ].laborPos[ k ].X = binReader.ReadInt32();
											game.playerList[ j ].cityList[ i ].laborPos[ k ].Y = binReader.ReadInt32();
										}

										game.playerList[ j ].cityList[ i ].buildingList = new bool[ Statistics.buildings.Length ];
										for ( int k = 0; k < buildingNbr; k++ )
											if ( k < game.playerList[ j ].cityList[ i ].buildingList.Length )
												game.playerList[ j ].cityList[ i ].buildingList[ k ] = binReader.ReadBoolean();
											else
												binReader.ReadBoolean();

										bool cap = binReader.ReadBoolean();

										game.playerList[ j ].cityList[ i ].isCapitale = cap;

										game.playerList[ j ].cityList[ i ].setHasDirectAccessToRessource();
									}
								}
								#endregion

								#region memories 0840

								game.playerList[ j ].memory = new Memory( game.playerList[ j ], binReader.ReadInt32() );
								for ( int i = 0; i < game.playerList[ j ].memory.Lenght; i++ )
								{
									game.playerList[ j ].memory.list[ i ].type = binReader.ReadByte();
									game.playerList[ j ].memory.list[ i ].turn = binReader.ReadInt32();
									game.playerList[ j ].memory.list[ i ].ind = new int[ binReader.ReadInt32() ];
									for ( int k = 0; k < game.playerList[ j ].memory.list[ i ].ind.Length; k++ )
										game.playerList[ j ].memory.list[ i ].ind[ k ] = binReader.ReadInt32();
								}

								#endregion

							}
						}
						#endregion

						// temp stack
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
									game.grid[ x, y ].stack[ i - 1 ] = game.playerList[ tempStacks[ x ][ y ][ i - 1 ].owner ].unitList[ tempStacks[ x ][ y ][ i - 1 ].unit ];

						#region caseImp
						game.caseImps = new structures.caseImprovement[ binReader.ReadInt32() ];
						for ( int i = 0; i < game.caseImps.Length; i++ )
						{
							game.caseImps[ i ].construction = binReader.ReadByte();
							game.caseImps[ i ].constructionPntLeft = binReader.ReadInt32();
							game.caseImps[ i ].owner = binReader.ReadByte();
							game.caseImps[ i ].pos.X = binReader.ReadInt32();
							game.caseImps[ i ].pos.Y = binReader.ReadInt32();
							game.caseImps[ i ].type = binReader.ReadByte();
							game.caseImps[ i ].units = new UnitList[ binReader.ReadInt32() ];
							for ( int j = 0; j < game.caseImps[ i ].units.Length; j++ )
							{
							/*	game.caseImps[ i ].units[ j ].owner = binReader.ReadByte();
								game.caseImps[ i ].units[ j ].unit = binReader.ReadInt32();*/

								game.caseImps[ i ].units[ j ] = game.playerList[ binReader.ReadByte() ].unitList[ binReader.ReadInt32() ];
							}
						}
						#endregion

						#region label
						label.initList( binReader.ReadInt32() );
						for ( int i = 0; i < label.list.Length; i++ )
						{
							label.list[ i ].X = binReader.ReadInt32();
							label.list[ i ].Y = binReader.ReadInt32();
							label.list[ i ].text = binReader.ReadString();
						}
						#endregion

						#region tutorial 857

						/*tutorial.init( binReader.ReadBoolean() );
								tutorialLength =  binReader.ReadInt32();

								if ( Tutorial.mode )
									for ( int i = 0; i < tutorialLength; i ++ )
										if ( i < Tutorial.alreadySeen.Length )
											tutorial.alreadySeen[ i ] =  binReader.ReadBoolean();*/

						#endregion

						#region custom nation 872
						if ( binReader.ReadBoolean() ) // game.playerList[ Form1.game.curPlayerInd ].civType == Statistics.normalCivilizationNumber )
						{
							Stat.Civilization[] buffer = Statistics.civilizations;

							Statistics.civilizations = new Stat.Civilization[ buffer.Length + 1 ];
							for ( int i = 0; i < buffer.Length; i++ )
								Statistics.civilizations[ i ] = buffer[ i ];

							Statistics.civilizations[ buffer.Length ].name = binReader.ReadString();
							Statistics.civilizations[ buffer.Length ].color = Color.FromArgb( 
								binReader.ReadByte(), 
								binReader.ReadByte(),
								binReader.ReadByte() 
								);
							Statistics.civilizations[ buffer.Length ].description = binReader.ReadString();
							Statistics.civilizations[ buffer.Length ].cityNames = Statistics.civilizations[ binReader.ReadByte() ].cityNames;
						}
						#endregion
						

						//	bool testing = binReader.ReadBoolean();
						/*	
								options.showLabels = true;
								options.showCommonSpyDialogs = true;
								*/
						Tutorial.init( false );
			
						#endregion
					}
					else if ( version == 877 )
					{
						#region case 0877

						Form1.sliHor = binReader.ReadInt32();
						Form1.sliVer = binReader.ReadInt32();
						int width = binReader.ReadInt32(),
							height = binReader.ReadInt32();

						game = new Game( width, height );

						game.curTurn = binReader.ReadInt32();
						Form1.options.frontierType = (Options.FrontierTypes)binReader.ReadByte();
						Form1.options.miniMapType = (Options.MiniMapTypes)binReader.ReadByte();
						buildingNbr = binReader.ReadInt32();
						tempTechnoNbr = binReader.ReadInt32();
						Form1.options.hideUndiscovered = binReader.ReadBoolean();
						Form1.options.showGrid = binReader.ReadBoolean();
						Form1.options.showOnScreenDPad = binReader.ReadBoolean();
						Form1.options.showBatteryStatus = binReader.ReadBoolean();
						Form1.options.showLabels = binReader.ReadBoolean();
						Form1.options.showCommonSpyDialogs = binReader.ReadBoolean();
						Form1.options.autosave = binReader.ReadBoolean(); 

						// temp
						structures.sStack[][][] tempStacks = new xycv_ppc.structures.sStack[ game.width ][][];

						for ( int x = 0; x < game.width; x ++ )
							tempStacks[ x ] = new xycv_ppc.structures.sStack[ game.height ][];
					
				/*		for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack		*/

						// end temp

						#region game.grid
						//		game.grid = new structures.sGrid[ game.width, game.height ];
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
							{
								game.grid[ x, y ].type = binReader.ReadByte();
								if ( game.grid[ x, y ].type != (byte)enums.terrainType.sea && game.grid[ x, y ].type != (byte)enums.terrainType.coast )
								{
									if ( binReader.ReadBoolean() )
										game.grid[ x, y ].city = binReader.ReadInt32();
									else
										game.grid[ x, y ].city = 0;

									game.grid[ x, y ].civicImprovement = binReader.ReadByte();
									game.grid[ x, y ].militaryImprovement = binReader.ReadByte();
									game.grid[ x, y ].roadLevel = binReader.ReadByte();
									game.grid[ x, y ].continent = binReader.ReadByte();
									if ( binReader.ReadBoolean() )
										game.grid[ x, y ].resources = binReader.ReadByte();

									game.grid[ x, y ].river = binReader.ReadBoolean(); // 863 
									if ( game.grid[ x, y ].river )
									{
										game.grid[ x, y ].riversDir = new bool[ 8 ];
										for ( int v = 0; v < game.grid[ x, y ].riversDir.Length; v++ )
											game.grid[ x, y ].riversDir[ v ] = binReader.ReadBoolean();
									}
								}

								if ( binReader.ReadBoolean() )
									game.grid[ x, y ].laborCity = binReader.ReadInt32();
								else
									game.grid[ x, y ].laborCity = 0;

								if ( binReader.ReadBoolean() )
								{
									
									tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ binReader.ReadInt32() ];

									game.grid[ x, y ].stack = new UnitList[ tempStacks[ x ][ y ].Length ];
									game.grid[ x, y ].stackPos = binReader.ReadInt32();
								}
								else
								{
									
									tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ 0 ];

									game.grid[ x, y ].stack = new UnitList[ 0 ];
									game.grid[ x, y ].stackPos = 0;
								}
							}
						#endregion

						#region stack
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
								{
									tempStacks[ x ][ y ][ i - 1 ].owner = binReader.ReadByte();
									tempStacks[ x ][ y ][ i - 1 ].unit = binReader.ReadInt32();

								/*	game.grid[ x, y ].stack[ i - 1 ].player.player = binReader.ReadByte();
									game.grid[ x, y ].stack[ i - 1 ].unit = binReader.ReadInt32();*/
								}

					/*	
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
								{
									game.grid[ x, y ].stack[ i - 1 ].player.player = binReader.ReadByte();
									game.grid[ x, y ].stack[ i - 1 ].unit = binReader.ReadInt32();
								}
					*/
						#endregion

						game.difficulty = binReader.ReadByte();
						nbrPlayer = binReader.ReadByte();
						game.playerList = new PlayerList[ nbrPlayer ]; 

						#region players # # #
						for ( int j = 0; j < nbrPlayer; j++ )
						{
							game.playerList[ j ] = new PlayerList( game, j );
							game.playerList[ j ].playerName = binReader.ReadString();
							game.playerList[ j ].civType = binReader.ReadByte();
							game.playerList[ j ].cityNumber = binReader.ReadInt32();
							game.playerList[ j ].unitNumber = binReader.ReadInt32();
							game.playerList[ j ].dead = binReader.ReadBoolean();
							if ( game.playerList[ j ].dead )
							{
								game.playerList[ j ].cityNumber = 0;
								game.playerList[ j ].unitNumber = 0;
							}

							if ( !game.playerList[ j ].dead )
							{
								game.playerList[ j ].currentResearch = binReader.ReadByte();
								game.playerList[ j ].money = binReader.ReadInt64();
								game.playerList[ j ].preferences.laborFood = binReader.ReadSByte();
								game.playerList[ j ].preferences.laborProd = binReader.ReadSByte();
								game.playerList[ j ].preferences.laborTrade = binReader.ReadSByte();
								game.playerList[ j ].preferences.science = binReader.ReadSByte();
								game.playerList[ j ].preferences.reserve = binReader.ReadSByte();
								game.playerList[ j ].preferences.buildings = binReader.ReadSByte();
								game.playerList[ j ].preferences.culture = binReader.ReadSByte();
								game.playerList[ j ].preferences.intelligence = binReader.ReadSByte();
								game.playerList[ j ].preferences.military = binReader.ReadSByte();
								game.playerList[ j ].preferences.space = binReader.ReadSByte();
								game.playerList[ j ].counterIntNbr = binReader.ReadInt32();
								game.playerList[ j ].govType = Statistics.governements[ binReader.ReadByte() ];
								game.playerList[ j ].economyType = binReader.ReadByte();
								game.playerList[ j ].posInCityFile = binReader.ReadByte();
								game.playerList[ j ].technos = new structures.technoList[ Statistics.technologies.Length ];
								game.playerList[ j ].discovered = new bool[ game.width, game.height ];
								game.playerList[ j ].see = new bool[ game.width, game.height ];
								game.playerList[ j ].unitList = new UnitList[ game.playerList[ j ].unitNumber + 10 ];
								game.playerList[ j ].cityList = new CityList[ game.playerList[ j ].cityNumber + 10 ];
								game.playerList[ j ].foreignRelation = new structures.sForeignRelation[ nbrPlayer ]; 
					
								#region foreignRelations
								for ( int i = 0; i < nbrPlayer; i++ )
								{
									game.playerList[ j ].foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
									for ( int k = 0; k < 4; k++ )
										game.playerList[ j ].foreignRelation[ i ].spies[ k ] = new xycv_ppc.structures.sSpies();

									game.playerList[ j ].foreignRelation[ i ].madeContact = binReader.ReadBoolean();
									if ( game.playerList[ j ].foreignRelation[ i ].madeContact )
									{
										game.playerList[ j ].foreignRelation[ i ].quality = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].politic = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].embargo = binReader.ReadBoolean();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].efficiency = binReader.ReadByte();
									}
								}
								#endregion

								#region see & discovered
								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										game.playerList[ j ].see[ x, y ] = binReader.ReadBoolean();

								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										if ( !game.playerList[ j ].see[ x, y ] )
											game.playerList[ j ].discovered[ x, y ] = binReader.ReadBoolean();
										else
											game.playerList[ j ].discovered[ x, y ] = true;
								#endregion
							
								#region last seen game.grid
							
								game.playerList[ j ].lastSeen = new structures.lastSeen[ game.width, game.height ];
								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										if ( !game.playerList[ j ].see[ x, y ] && game.playerList[ j ].discovered[ x, y ] )
										{
											if ( Statistics.terrains[ game.grid[ x, y ].type ].ew == 1 )
											{
												game.playerList[ j ].lastSeen[ x, y ].cityPop = binReader.ReadByte();
												if ( game.playerList[ j ].lastSeen[ x, y ].cityPop > 0 )
													game.playerList[ j ].lastSeen[ x, y ].city = binReader.ReadInt32();

												game.playerList[ j ].lastSeen[ x, y ].civicImp = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].militaryImp = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].road = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].territory = binReader.ReadByte();
											}
											else if ( game.grid[ x, y ].type == (byte)enums.terrainType.coast )
											{
												game.playerList[ j ].lastSeen[ x, y ].territory = binReader.ReadByte();
											}

											game.playerList[ j ].lastSeen[ x, y ].turn = binReader.ReadInt32();
										}
								#endregion

								#region techno
								for ( int i = 0; i < tempTechnoNbr; i++ )
								{
									byte num = binReader.ReadByte();

									if ( i < Statistics.technologies.Length )
										switch ( num )
										{
											case 1 :
												if ( i < game.playerList[ j ].technos.Length )
													game.playerList[ j ].technos[ i ].researched = true;

												break;

											case 2 :
												if ( i < game.playerList[ j ].technos.Length )
												{
													game.playerList[ j ].technos[ i ].researched = false;
													game.playerList[ j ].technos[ i ].pntDiscovered = binReader.ReadInt32();
												}
												else
												{
													binReader.ReadInt32();
												}

												break;

											default :
												if ( i < game.playerList[ j ].technos.Length )
													game.playerList[ j ].technos[ i ].researched = false;

												break;
										}
								}
								#endregion

								#region units
								for ( int i = 1; i <= game.playerList[ j ].unitNumber; i++ )
								{
									game.playerList[ j ].unitList[ i ] = new UnitList( game.playerList[ j ], i );
									game.playerList[ j ].unitList[ i ].state = binReader.ReadByte();
									if ( game.playerList[ j ].unitList[ i ].state != (byte)Form1.unitState.dead )
									{
										game.playerList[ j ].unitList[ i ].type = binReader.ReadByte();
										game.playerList[ j ].unitList[ i ].health = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].level = binReader.ReadByte();
										game.playerList[ j ].unitList[ i ].moveLeft = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].moveLeftFraction = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].transported = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].X = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].Y = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].dest.X = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].dest.Y = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].transport = new int[ Statistics.units[ game.playerList[ j ].unitList[ i ].type ].transport ];
										for ( int k = 0; k < game.playerList[ j ].unitList[ i ].transported; k++ )
											game.playerList[ j ].unitList[ i ].transport[ k ] = binReader.ReadInt32();

										game.playerList[ j ].unitList[ i ].automated = binReader.ReadBoolean(); //842
									}
								}
								#endregion

								#region cities
								for ( int i = 1; i <= game.playerList[ j ].cityNumber; i++ )
								{
									game.playerList[ j ].cityList[ i ] = new CityList( game.playerList[ j ], i );
									game.playerList[ j ].cityList[ i ].state = binReader.ReadByte();
									game.playerList[ j ].cityList[ i ].name = binReader.ReadString();
									if ( game.playerList[ j ].cityList[ i ].state != (byte)enums.cityState.dead )
									{
										game.playerList[ j ].cityList[ i ].construction.points = binReader.ReadInt32();

										// 0862
										for ( int k = 0; k < 10; k++ )
										{
											int cType =  binReader.ReadInt32();
											byte type = binReader.ReadByte();

											if ( cType != -1 )
												switch ( cType )
												{
													case 0:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
														break;

													case Stat.Unit.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
														break;
												
													case Stat.Building.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.buildings[ type ];
														break;
												
													case Stat.Wealth.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
														break;
												
														/*		case Stat.SmallWonder.constructionType:
																	game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.smallWonders[ type ];
																	break;
												
																case Stat.Wonder.constructionType:
																	game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.wonders[ type ];
																	break;*/

													default:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
														break;
												}
										//	game.playerList[ j ].cityList[ i ].construction.list[ k ].ind = binReader.ReadInt32();
										//	game.playerList[ j ].cityList[ i ].construction.list[ k ].type = binReader.ReadByte();
										}

										// end 0862
										game.playerList[ j ].cityList[ i ].foodReserve = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].laborOnField = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].originalOwner = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].population = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].X = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].Y = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].laborPos = new Point[ game.playerList[ j ].cityList[ i ].laborOnField ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].laborOnField; k++ )
										{
											game.playerList[ j ].cityList[ i ].laborPos[ k ].X = binReader.ReadInt32();
											game.playerList[ j ].cityList[ i ].laborPos[ k ].Y = binReader.ReadInt32();
										}

										game.playerList[ j ].cityList[ i ].buildingList = new bool[ Statistics.buildings.Length ];
										for ( int k = 0; k < buildingNbr; k++ )
											if ( k < game.playerList[ j ].cityList[ i ].buildingList.Length )
												game.playerList[ j ].cityList[ i ].buildingList[ k ] = binReader.ReadBoolean();
											else
												binReader.ReadBoolean();

										bool cap = binReader.ReadBoolean();

										game.playerList[ j ].cityList[ i ].isCapitale = cap;

										//0877
										game.playerList[ j ].cityList[ i ].nonLabor.list = new byte[ binReader.ReadInt32() ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].nonLabor.list.Length; k++ )
											game.playerList[ j ].cityList[ i ].nonLabor.list[ k ] = binReader.ReadByte();

										game.playerList[ j ].cityList[ i ].slaves.list = new byte[ binReader.ReadInt32() ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].slaves.list.Length; k++ )
											game.playerList[ j ].cityList[ i ].slaves.list[ k ] = binReader.ReadByte();
										//end of 0877

										game.playerList[ j ].cityList[ i ].setHasDirectAccessToRessource();
									}
								}
								#endregion

								#region memories 0840

								game.playerList[ j ].memory = new Memory( game.playerList[ j ], binReader.ReadInt32() );
								for ( int i = 0; i < game.playerList[ j ].memory.Lenght; i++ )
								{
									game.playerList[ j ].memory.list[ i ].type = binReader.ReadByte();
									game.playerList[ j ].memory.list[ i ].turn = binReader.ReadInt32();
									game.playerList[ j ].memory.list[ i ].ind = new int[ binReader.ReadInt32() ];
									for ( int k = 0; k < game.playerList[ j ].memory.list[ i ].ind.Length; k++ )
										game.playerList[ j ].memory.list[ i ].ind[ k ] = binReader.ReadInt32();
								}

								#endregion

								#region slaves 0878
								game.playerList[ j ].slaves.transferts = new playerSlavery.transfertList[ binReader.ReadInt32() ];
								for ( int i = 0; i < game.playerList[ j ].slaves.transferts.Length; i++ )
								{
									game.playerList[ j ].slaves.transferts[ i ].dest = binReader.ReadInt32();
									game.playerList[ j ].slaves.transferts[ i ].ori = binReader.ReadInt32();
									game.playerList[ j ].slaves.transferts[ i ].nbr = binReader.ReadInt32();
									game.playerList[ j ].slaves.transferts[ i ].eta = binReader.ReadInt32();
								}
								#endregion

							}
						}
						#endregion

						// temp stack
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
									game.grid[ x, y ].stack[ i - 1 ] = game.playerList[ tempStacks[ x ][ y ][ i - 1 ].owner ].unitList[ tempStacks[ x ][ y ][ i - 1 ].unit ];

						#region caseImp
						game.caseImps = new structures.caseImprovement[ binReader.ReadInt32() ];
						for ( int i = 0; i < game.caseImps.Length; i++ )
						{
							game.caseImps[ i ].construction = binReader.ReadByte();
							game.caseImps[ i ].constructionPntLeft = binReader.ReadInt32();
							game.caseImps[ i ].owner = binReader.ReadByte();
							game.caseImps[ i ].pos.X = binReader.ReadInt32();
							game.caseImps[ i ].pos.Y = binReader.ReadInt32();
							game.caseImps[ i ].type = binReader.ReadByte();
							game.caseImps[ i ].units = new UnitList[ binReader.ReadInt32() ];
							for ( int j = 0; j < game.caseImps[ i ].units.Length; j++ )
							{
								/*	game.caseImps[ i ].units[ j ].owner = binReader.ReadByte();
									game.caseImps[ i ].units[ j ].unit = binReader.ReadInt32();*/

								game.caseImps[ i ].units[ j ] = game.playerList[ binReader.ReadByte() ].unitList[ binReader.ReadInt32() ];
							}
						}
						#endregion

						#region label
						label.initList( binReader.ReadInt32() );
						for ( int i = 0; i < label.list.Length; i++ )
						{
							label.list[ i ].X = binReader.ReadInt32();
							label.list[ i ].Y = binReader.ReadInt32();
							label.list[ i ].text = binReader.ReadString();
						}
						#endregion

						#region tutorial 857

						/*tutorial.init( binReader.ReadBoolean() );
								tutorialLength =  binReader.ReadInt32();

								if ( Tutorial.mode )
									for ( int i = 0; i < tutorialLength; i ++ )
										if ( i < Tutorial.alreadySeen.Length )
											tutorial.alreadySeen[ i ] =  binReader.ReadBoolean();*/

						#endregion

						#region custom nation 872
						if ( binReader.ReadBoolean() ) // game.playerList[ Form1.game.curPlayerInd ].civType == Statistics.normalCivilizationNumber )
						{
							Stat.Civilization[] buffer = Statistics.civilizations;

							Statistics.civilizations = new Stat.Civilization[ buffer.Length + 1 ];
							for ( int i = 0; i < buffer.Length; i++ )
								Statistics.civilizations[ i ] = buffer[ i ];

							Statistics.civilizations[ buffer.Length ].name = binReader.ReadString();
							Statistics.civilizations[ buffer.Length ].color = Color.FromArgb( binReader.ReadByte(), binReader.ReadByte(), binReader.ReadByte() );
							Statistics.civilizations[ buffer.Length ].description = binReader.ReadString();
							Statistics.civilizations[ buffer.Length ].cityNames = Statistics.civilizations[ binReader.ReadByte() ].cityNames;
						}
						#endregion
						

						//	bool testing = binReader.ReadBoolean();
						/*	
							options.showLabels = true;
							options.showCommonSpyDialogs = true;
							*/
						Tutorial.init( false );
			
						#endregion
					}
					else if ( version == 885 )
					{
						#region case 0885

						Form1.sliHor = binReader.ReadInt32();
						Form1.sliVer = binReader.ReadInt32();
						int width = binReader.ReadInt32(),
							height = binReader.ReadInt32();

						game = new Game( width, height );

						game.curTurn = binReader.ReadInt32();
						Form1.options.frontierType = (Options.FrontierTypes)binReader.ReadByte();
						Form1.options.miniMapType = (Options.MiniMapTypes)binReader.ReadByte();
						buildingNbr = binReader.ReadInt32();
						tempTechnoNbr = binReader.ReadInt32();
						Form1.options.hideUndiscovered = binReader.ReadBoolean();
						Form1.options.showGrid = binReader.ReadBoolean();
						Form1.options.showOnScreenDPad = binReader.ReadBoolean();
						Form1.options.showBatteryStatus = binReader.ReadBoolean();
						Form1.options.showLabels = binReader.ReadBoolean();
						Form1.options.showCommonSpyDialogs = binReader.ReadBoolean();
						Form1.options.autosave = binReader.ReadBoolean(); 

						// temp
						structures.sStack[][][] tempStacks = new xycv_ppc.structures.sStack[ game.width ][][];

						for ( int x = 0; x < game.width; x ++ )
							tempStacks[ x ] = new xycv_ppc.structures.sStack[ game.height ][];

						#region game.grid
						//		game.grid = new structures.sGrid[ game.width, game.height ];
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
							{
								game.grid[ x, y ].type = binReader.ReadByte();
								if ( game.grid[ x, y ].type != (byte)enums.terrainType.sea && game.grid[ x, y ].type != (byte)enums.terrainType.coast )
								{
									if ( binReader.ReadBoolean() )
										game.grid[ x, y ].city = binReader.ReadInt32();
									else
										game.grid[ x, y ].city = 0;

									game.grid[ x, y ].civicImprovement = binReader.ReadByte();
									game.grid[ x, y ].militaryImprovement = binReader.ReadByte();
									game.grid[ x, y ].roadLevel = binReader.ReadByte();
									game.grid[ x, y ].continent = binReader.ReadByte();

									if ( binReader.ReadBoolean() )
										game.grid[ x, y ].resources = binReader.ReadByte();

									game.grid[ x, y ].river = binReader.ReadBoolean(); // 863 

									if ( game.grid[ x, y ].river )
									{
										game.grid[ x, y ].riversDir = new bool[ 8 ];
										for ( int v = 0; v < game.grid[ x, y ].riversDir.Length; v++ )
											game.grid[ x, y ].riversDir[ v ] = binReader.ReadBoolean();
									}
								}

								if ( binReader.ReadBoolean() )
									game.grid[ x, y ].laborCity = binReader.ReadInt32();
								else
									game.grid[ x, y ].laborCity = 0;


								if ( binReader.ReadBoolean() )
								{
									
									tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ binReader.ReadInt32() ];

									game.grid[ x, y ].stack = new UnitList[ tempStacks[ x ][ y ].Length ];
									game.grid[ x, y ].stackPos = binReader.ReadInt32();
								}
								else
								{
									
									tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ 0 ];

									game.grid[ x, y ].stack = new UnitList[ 0 ];
									game.grid[ x, y ].stackPos = 0;
								}
							}
						#endregion

						#region stack
						for ( int x = 0; x < game.width; x++ )
							for ( int y = 0; y < game.height; y++ )
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
								{
									tempStacks[ x ][ y ][ i - 1 ].owner = binReader.ReadByte();
									tempStacks[ x ][ y ][ i - 1 ].unit = binReader.ReadInt32();

									/*	game.grid[ x, y ].stack[ i - 1 ].player.player = binReader.ReadByte();
										game.grid[ x, y ].stack[ i - 1 ].unit = binReader.ReadInt32();*/
								}
						#endregion

						game.difficulty = binReader.ReadByte(); 
						nbrPlayer = binReader.ReadByte(); // binReader.ReadInt32(); 
						game.playerList = new PlayerList[ nbrPlayer ]; 

						#region players # # #
						for ( int j = 0; j < nbrPlayer; j++ )
						{
							game.playerList[ j ] = new PlayerList( game, j );
							game.playerList[ j ].playerName = binReader.ReadString();
							game.playerList[ j ].civType = binReader.ReadByte();
							game.playerList[ j ].cityNumber = binReader.ReadInt32();
							game.playerList[ j ].unitNumber = binReader.ReadInt32();
							game.playerList[ j ].dead = binReader.ReadBoolean();
							if ( game.playerList[ j ].dead )
							{
								game.playerList[ j ].cityNumber = 0;
								game.playerList[ j ].unitNumber = 0;
							}

							if ( !game.playerList[ j ].dead )
							{
								game.playerList[ j ].currentResearch = binReader.ReadByte();
								game.playerList[ j ].money = binReader.ReadInt64();
								game.playerList[ j ].preferences.laborFood = binReader.ReadSByte();
								game.playerList[ j ].preferences.laborProd = binReader.ReadSByte();
								game.playerList[ j ].preferences.laborTrade = binReader.ReadSByte();
								game.playerList[ j ].preferences.science = binReader.ReadSByte();
								game.playerList[ j ].preferences.reserve = binReader.ReadSByte();
								game.playerList[ j ].preferences.buildings = binReader.ReadSByte();
								game.playerList[ j ].preferences.culture = binReader.ReadSByte();
								game.playerList[ j ].preferences.intelligence = binReader.ReadSByte();
								game.playerList[ j ].preferences.military = binReader.ReadSByte();
								game.playerList[ j ].preferences.space = binReader.ReadSByte();
								game.playerList[ j ].counterIntNbr = binReader.ReadInt32();
								game.playerList[ j ].govType = Statistics.governements[ binReader.ReadByte() ];
								game.playerList[ j ].economyType = binReader.ReadByte();
								game.playerList[ j ].posInCityFile = binReader.ReadByte();
								game.playerList[ j ].technos = new structures.technoList[ Statistics.technologies.Length ];
								game.playerList[ j ].discovered = new bool[ game.width, game.height ];
								game.playerList[ j ].see = new bool[ game.width, game.height ];
								game.playerList[ j ].unitList = new UnitList[ game.playerList[ j ].unitNumber + 10 ];
								game.playerList[ j ].cityList = new CityList[ game.playerList[ j ].cityNumber + 10 ];
								game.playerList[ j ].foreignRelation = new structures.sForeignRelation[ nbrPlayer ]; 
					
								#region foreignRelations
								for ( int i = 0; i < nbrPlayer; i++ )
								{
									game.playerList[ j ].foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
									for ( int k = 0; k < 4; k++ )
										game.playerList[ j ].foreignRelation[ i ].spies[ k ] = new xycv_ppc.structures.sSpies();

									game.playerList[ j ].foreignRelation[ i ].madeContact = binReader.ReadBoolean();
									if ( game.playerList[ j ].foreignRelation[ i ].madeContact )
									{
										game.playerList[ j ].foreignRelation[ i ].quality = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].politic = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].embargo = binReader.ReadBoolean();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].efficiency = binReader.ReadByte();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr = binReader.ReadInt32();
										game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].efficiency = binReader.ReadByte();
									}
								}
								#endregion

								#region see & discovered
								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										game.playerList[ j ].see[ x, y ] = binReader.ReadBoolean();

								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										if ( !game.playerList[ j ].see[ x, y ] )
											game.playerList[ j ].discovered[ x, y ] = binReader.ReadBoolean();
										else
											game.playerList[ j ].discovered[ x, y ] = true;
								#endregion
							
								#region last seen game.grid
							
								game.playerList[ j ].lastSeen = new structures.lastSeen[ game.width, game.height ];
								for ( int x = 0; x < game.width; x++ )
									for ( int y = 0; y < game.height; y++ )
										if ( !game.playerList[ j ].see[ x, y ] && game.playerList[ j ].discovered[ x, y ] )
										{
											if ( Statistics.terrains[ game.grid[ x, y ].type ].ew == 1 )
											{
												game.playerList[ j ].lastSeen[ x, y ].cityPop = binReader.ReadByte();
												if ( game.playerList[ j ].lastSeen[ x, y ].cityPop > 0 )
													game.playerList[ j ].lastSeen[ x, y ].city = binReader.ReadInt32();

												game.playerList[ j ].lastSeen[ x, y ].civicImp = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].militaryImp = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].road = binReader.ReadByte();
												game.playerList[ j ].lastSeen[ x, y ].territory = binReader.ReadByte();
											}
											else if ( game.grid[ x, y ].type == (byte)enums.terrainType.coast )
											{
												game.playerList[ j ].lastSeen[ x, y ].territory = binReader.ReadByte();
											}

											game.playerList[ j ].lastSeen[ x, y ].turn = binReader.ReadInt32();
										}
								#endregion

								#region techno
								for ( int i = 0; i < tempTechnoNbr; i++ )
								{
									byte num = binReader.ReadByte();

									if ( i < Statistics.technologies.Length )
										switch ( num )
										{
											case 1 :
												if ( i < game.playerList[ j ].technos.Length )
													game.playerList[ j ].technos[ i ].researched = true;

												break;

											case 2 :
												if ( i < game.playerList[ j ].technos.Length )
												{
													game.playerList[ j ].technos[ i ].researched = false;
													game.playerList[ j ].technos[ i ].pntDiscovered = binReader.ReadInt32();
												}
												else
												{
													binReader.ReadInt32();
												}

												break;

											default :
												if ( i < game.playerList[ j ].technos.Length )
													game.playerList[ j ].technos[ i ].researched = false;

												break;
										}
								}
								#endregion

								#region units
								for ( int i = 1; i <= game.playerList[ j ].unitNumber; i++ )
								{
									game.playerList[ j ].unitList[ i ] = new UnitList( game.playerList[ j ], i );
									game.playerList[ j ].unitList[ i ].state = binReader.ReadByte();
									if ( game.playerList[ j ].unitList[ i ].state != (byte)Form1.unitState.dead )
									{
										game.playerList[ j ].unitList[ i ].type = binReader.ReadByte();
										game.playerList[ j ].unitList[ i ].health = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].level = binReader.ReadByte();
										game.playerList[ j ].unitList[ i ].moveLeft = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].moveLeftFraction = binReader.ReadSByte();
										game.playerList[ j ].unitList[ i ].transported = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].X = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].Y = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].dest.X = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].dest.Y = binReader.ReadInt32();
										game.playerList[ j ].unitList[ i ].transport = new int[ Statistics.units[ game.playerList[ j ].unitList[ i ].type ].transport ];
										for ( int k = 0; k < game.playerList[ j ].unitList[ i ].transported; k++ )
											game.playerList[ j ].unitList[ i ].transport[ k ] = binReader.ReadInt32();

										game.playerList[ j ].unitList[ i ].automated = binReader.ReadBoolean(); //842
									}
								}
								#endregion

								#region cities
								for ( int i = 1; i <= game.playerList[ j ].cityNumber; i++ )
								{
									game.playerList[ j ].cityList[ i ] = new CityList( game.playerList[ j ], i );
									game.playerList[ j ].cityList[ i ].state = binReader.ReadByte();
									game.playerList[ j ].cityList[ i ].name = binReader.ReadString();
									if ( game.playerList[ j ].cityList[ i ].state != (byte)enums.cityState.dead )
									{
										game.playerList[ j ].cityList[ i ].construction.points = binReader.ReadInt32();

										// 0862
										for ( int k = 0; k < 10; k++ )
										{
											int cType =  binReader.ReadInt32();
											byte type = binReader.ReadByte();

											if ( cType != -1 )
												switch ( cType )
												{
													case 0:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
														break;

													case Stat.Unit.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
														break;
												
													case Stat.Building.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.buildings[ type ];
														break;
												
													case Stat.Wealth.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
														break;
												
											/*		case Stat.SmallWonder.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.smallWonders[ type ];
														break;
												
													case Stat.Wonder.constructionType:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.wonders[ type ];
														break;*/

													default:
														game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
														break;
												}

									//		game.playerList[ j ].cityList[ i ].construction.list[ k ].ind = binReader.ReadInt32();
									//		game.playerList[ j ].cityList[ i ].construction.list[ k ].type = binReader.ReadByte();
										}

										// end 0862
										game.playerList[ j ].cityList[ i ].foodReserve = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].laborOnField = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].originalOwner = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].population = binReader.ReadByte();
										game.playerList[ j ].cityList[ i ].X = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].Y = binReader.ReadInt32();
										game.playerList[ j ].cityList[ i ].laborPos = new Point[ game.playerList[ j ].cityList[ i ].laborOnField ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].laborOnField; k++ )
										{
											game.playerList[ j ].cityList[ i ].laborPos[ k ].X = binReader.ReadInt32();
											game.playerList[ j ].cityList[ i ].laborPos[ k ].Y = binReader.ReadInt32();
										}

										game.playerList[ j ].cityList[ i ].buildingList = new bool[ Statistics.buildings.Length ];
										for ( int k = 0; k < buildingNbr; k++ )
											if ( k < game.playerList[ j ].cityList[ i ].buildingList.Length )
												game.playerList[ j ].cityList[ i ].buildingList[ k ] = binReader.ReadBoolean();
											else
												binReader.ReadBoolean();

										bool cap = binReader.ReadBoolean();

										game.playerList[ j ].cityList[ i ].isCapitale = cap;

										//0877
										game.playerList[ j ].cityList[ i ].nonLabor.list = new byte[ binReader.ReadInt32() ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].nonLabor.list.Length; k++ )
											game.playerList[ j ].cityList[ i ].nonLabor.list[ k ] = binReader.ReadByte();

										game.playerList[ j ].cityList[ i ].slaves.list = new byte[ binReader.ReadInt32() ];
										for ( int k = 0; k < game.playerList[ j ].cityList[ i ].slaves.list.Length; k++ )
											game.playerList[ j ].cityList[ i ].slaves.list[ k ] = binReader.ReadByte();
										//end of 0877

										game.playerList[ j ].cityList[ i ].setHasDirectAccessToRessource();
									}
								}
								#endregion

								#region memories 0840

								game.playerList[ j ].memory = new Memory( game.playerList[ j ], binReader.ReadInt32() );
								for ( int i = 0; i < game.playerList[ j ].memory.Lenght; i++ )
								{
									game.playerList[ j ].memory.list[ i ].type = binReader.ReadByte();
									game.playerList[ j ].memory.list[ i ].turn = binReader.ReadInt32();
									game.playerList[ j ].memory.list[ i ].ind = new int[ binReader.ReadInt32() ];
									for ( int k = 0; k < game.playerList[ j ].memory.list[ i ].ind.Length; k++ )
										game.playerList[ j ].memory.list[ i ].ind[ k ] = binReader.ReadInt32();
								}

								#endregion

								#region slaves 0878
								game.playerList[ j ].slaves.transferts = new playerSlavery.transfertList[ binReader.ReadInt32() ];
								for ( int i = 0; i < game.playerList[ j ].slaves.transferts.Length; i++ )
								{
									game.playerList[ j ].slaves.transferts[ i ].dest = binReader.ReadInt32();
									game.playerList[ j ].slaves.transferts[ i ].ori = binReader.ReadInt32();
									game.playerList[ j ].slaves.transferts[ i ].nbr = binReader.ReadInt32();
									game.playerList[ j ].slaves.transferts[ i ].eta = binReader.ReadInt32();
								}
								#endregion

							}
						}
						#endregion

						// temp stack 
						for ( int x = 0; x < game.width; x++ ) 
							for ( int y = 0; y < game.height; y++ ) 
								for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ ) 
									game.grid[ x, y ].stack[ i - 1 ] = game.playerList[ tempStacks[ x ][ y ][ i - 1 ].owner ].unitList[ tempStacks[ x ][ y ][ i - 1 ].unit ]; 

						#region caseImp

						game.caseImps = new structures.caseImprovement[ binReader.ReadInt32() ];

						// correct bug pre 887
						bool[] invalidCaseImps = new bool[ game.caseImps.Length ];
						int totalInvalidCaseImps = 0;
						//

						for ( int i = 0; i < game.caseImps.Length; i++ )
						{
							game.caseImps[ i ].construction = binReader.ReadByte();
							game.caseImps[ i ].constructionPntLeft = binReader.ReadInt32();
							game.caseImps[ i ].owner = binReader.ReadByte();
							game.caseImps[ i ].pos.X = binReader.ReadInt32();
							game.caseImps[ i ].pos.Y = binReader.ReadInt32();
							game.caseImps[ i ].type = binReader.ReadByte();
							game.caseImps[ i ].units = new UnitList[ binReader.ReadInt32() ];

							for ( int j = 0; j < game.caseImps[ i ].units.Length; j++ )
							{
								// debuger
								byte p0 = binReader.ReadByte();
								int u0 = binReader.ReadInt32();

								if ( !game.playerList[ p0 ].dead )
									game.caseImps[ i ].units[ j ] = game.playerList[ p0 ].unitList[ u0 ];
								else
								{
									game.caseImps[ i ].units[ j ] = game.playerList[ 0 ].unitList[ 0 ];

									if ( !invalidCaseImps[ i ] )
									{
										invalidCaseImps[ i ] = true;
										totalInvalidCaseImps ++;
									}
								}

							//	normal, to be restored...
							//	game.caseImps[ i ].units[ j ] = game.playerList[ binReader.ReadByte() ].unitList[ binReader.ReadInt32() ];
							}
						}
						#endregion

						// suite debugger
							   if ( totalInvalidCaseImps > 0 )
							   {
									structures.caseImprovement[] ciBuffer = game.caseImps;
									game.caseImps = new structures.caseImprovement[ ciBuffer.Length - totalInvalidCaseImps ];

									for ( int i = 0, j = 0; i < ciBuffer.Length; i ++ )
										if ( !invalidCaseImps[ i ] )
										{
											game.caseImps[ j ] = ciBuffer[ i ];
											j ++;
										}
							   }

						#region label
						label.initList( binReader.ReadInt32() );
						for ( int i = 0; i < label.list.Length; i++ )
						{
							label.list[ i ].X = binReader.ReadInt32();
							label.list[ i ].Y = binReader.ReadInt32();
							label.list[ i ].text = binReader.ReadString();
						}
						#endregion

						#region tutorial 857 returned 885

						Tutorial.init( binReader.ReadBoolean() );
						if ( Tutorial.mode )
						{
							tutorialLength =  binReader.ReadInt32();

							for ( int i = 0; i < tutorialLength; i ++ )
								if ( i < Tutorial.alreadySeen.Length )
									Tutorial.alreadySeen[ i ] = binReader.ReadBoolean();
								else
									binReader.ReadBoolean();
						}

						#endregion

						#region custom nation 872
						if ( binReader.ReadBoolean() ) // game.playerList[ Form1.game.curPlayerInd ].civType == Statistics.normalCivilizationNumber )
						{
							Stat.Civilization[] buffer = Statistics.civilizations;

							Statistics.civilizations = new Stat.Civilization[ buffer.Length + 1 ];
							for ( int i = 0; i < buffer.Length; i++ )
								Statistics.civilizations[ i ] = buffer[ i ];

							Statistics.civilizations[ buffer.Length ].name = binReader.ReadString();
							Statistics.civilizations[ buffer.Length ].color = Color.FromArgb( binReader.ReadByte(), binReader.ReadByte(), binReader.ReadByte() );
							Statistics.civilizations[ buffer.Length ].description = binReader.ReadString();
							Statistics.civilizations[ buffer.Length ].cityNames = Statistics.civilizations[ binReader.ReadByte() ].cityNames;
						}
						#endregion

						/*
							bool testing = binReader.ReadBoolean();

							options.showLabels = true;
							options.showCommonSpyDialogs = true;
						*/
			
						#endregion
					}
					else if ( version == 887 )
					{
						#region case 0887

						FileHeader header = FileHeader.getFromStream( new FileHeader(), sp, version, binReader );

						if ( header.type == FileHeader.Type.playedGame )
						{
							game = new Game();
						}
						else  if ( header.type == FileHeader.Type.playedScenario )
						{
							game = new Scenario();

							((Scenario)game).name = header.name;
							((Scenario)game).description = header.description;
							((Scenario)game).goalType = header.goalType;
							((Scenario)game).goalInd = header.goalInd;
						}

					/*	byte gameType = binReader.ReadByte();
						if ( gameType == 1 )
						{
							game = new Scenario();
							((Scenario)game).name = binReader.ReadString();
							((Scenario)game).description = binReader.ReadString();
							((Scenario)game).goalType = (Scenario.GoalType)binReader.ReadByte();
						}
						else
						{
							game = new Game();
							binReader.ReadString();
							binReader.ReadString();
						}*/

					//	FileHeader.loadMap( null, binReader );

						#region options
						Form1.options.frontierType = (Options.FrontierTypes)binReader.ReadByte();
						Form1.options.miniMapType = (Options.MiniMapTypes)binReader.ReadByte();
						Form1.options.hideUndiscovered = binReader.ReadBoolean();
						Form1.options.showGrid = binReader.ReadBoolean();
						Form1.options.showOnScreenDPad = binReader.ReadBoolean();
						Form1.options.showBatteryStatus = binReader.ReadBoolean();
						Form1.options.showLabels = binReader.ReadBoolean();
						Form1.options.showCommonSpyDialogs = binReader.ReadBoolean();
						Form1.options.autosave = binReader.ReadBoolean(); 
						#endregion

						Form1.sliHor = binReader.ReadInt32();
						Form1.sliVer = binReader.ReadInt32();

						loadCore( game, binReader );
	
						#endregion
					}
					else
					{
						wC.show = false;

						MessageBox.Show( 
							String.Format( language.getAString( language.order.errorUnrecongnisedSaveVersion ), (double)version / 1000 ), 
							language.getAString( language.order.errorTitle ) 
							);
						
						return null;
					}

					if ( System.IO.Path.GetFileName( System.IO.Path.GetDirectoryName( sp ) ) != "auto" )
						game.savePath = sp; 
					else
						game.savePath = "";

					roads.setAll( game ); // display

					for ( int p = 0; p < game.playerList.Length; p ++ )
						if ( !game.playerList[ p ].dead )
						{
							// for pre 0877
							for ( int c = 1; c <= game.playerList[ p ].cityNumber; c++ )
								if ( !game.playerList[ p ].cityList[ c ].dead )
								{
									if ( game.playerList[ p ].cityList[ c ].population - game.playerList[ p ].cityList[ c ].laborOnField > 0 )
										game.playerList[ p ].cityList[ c ].nonLabor.add( game.playerList[ p ].cityList[ c ].population - game.playerList[ p ].cityList[ c ].laborOnField );
									else if ( game.playerList[ p ].cityList[ c ].population - game.playerList[ p ].cityList[ c ].laborOnField < 0 )
									{
										int i = 0;
									}
								}
							// end of pre 0877

							for ( int u = 1; u <= game.playerList[ p ].unitNumber; u++ )
								if ( game.playerList[ p ].unitList[ u ].state == (byte)Form1.unitState.turnCompleted )
								{
									if ( game.playerList[ p ].unitList[ u ].X == -1 )
										game.playerList[ p ].unitList[ u ].state = (byte)Form1.unitState.inTransport;
									else
										game.playerList[ p ].unitList[ u ].state = (byte)Form1.unitState.idle;
								}
						}

				
					for ( int p = 0; p < game.playerList.Length; p++ )
						if ( !game.playerList[ p ].dead )
							game.playerList[ p ].setResourcesAccess();

					game.plans.init();

					game.frontier.setFrontiers(); 

					game.radius.setIsNextToWater();
					game.rvtbc = new Rvtbc( game ); //.init(); 
					game.rvtbc.doit();
			
					if ( game.wonderList == null ) // pre 887
					{
						game.wonderList = new WonderList( Statistics.wonders.Length );
						game.wonderList.initialize();

						foreach ( PlayerList player in game.playerList )
							if ( player.smallWonderList == null ) // pre 887
							{
								player.smallWonderList = new WonderList( Statistics.smallWonders.Length );
								player.smallWonderList.initialize();
							}
					}

					/////// temp
					foreach ( PlayerList p in game.playerList )
						if ( !p.dead )
							for ( int u = 1; u <= p.unitNumber; u ++ )
								if ( 
									p.unitList[ u ].state == (byte)Form1.unitState.waitingForBoat && 
									p.unitList[ u ].X == 0 && 
									p.unitList[ u ].Y == 0 
									)
									p.unitList[ u ].state = (byte)Form1.unitState.dead;

					/*					
										for ( int x = 0; x < game.width; x++ )
											for ( int y = 0; y < game.height; y++ )
												if ( game.grid[ x, y ].militaryImprovement == (byte)enums.militaryImprovement.airport ) 
													game.grid[ x, y ].militaryImprovement = 0;
					*/

					/*
										Form1.selected.state = 0;
										Form1.selected.unit = 0;
										Form1.pictureBox3.Visible = false;
										Form1.pictureBox4.Visible = false;

										Form1.drawMiniMap(); 
										Form1.pictureBox1.Enabled = true; 
										Form1.pictureBox1.Visible = true; 
										Form1.menuItem18.Enabled = true; 

										//   platformSpec.keys.Set( this );//keyIn = new keyInPut(); 
										// timer1.Enabled = true;
										unitOrder.setOrder( Form1.game.curPlayerInd );
			
										Form1.oldSliVer = -5; 

										if ( !Form1.showNextUnitNow( 0, 0 ) ) 
											Form1.DrawMap(); 

										Form1.guiEnabled = true; 
										wC.show = false; 
					*/
			//	}

				success = true;
			}
			catch ( System.IO.FileNotFoundException )
			{
				string pr = System.IO.Path.GetDirectoryName( sp ); 
				string txt; 

				if ( pr == @"\SD Card" || pr == @"\SD Card\My Documents" ) 
					txt = " Check the SD card..."; 
				else if ( pr == @"\CF Card" || pr == @"\CF Card\My Documents" ) 
					txt = " Check the CF card..."; 
				else 
					txt = ""; 

				MessageBox.Show( "File not found." + txt + ", Sorry", "Error" ); 

				success = false;
			}
			catch ( System.Exception e )
			{
				MessageBox.Show( "Error loading from file, sorry.", e.ToString() ); 

				success = false;
			}
			finally
			{
				if ( file != null )
					file.Close(); 

				if ( binReader != null )
					binReader.Close(); 
			}

			if ( success )
				return game;
			else
				return null;
		}
		#endregion

#region saveAsGetPath
		public bool saveAsGetPath()
		{
			string path = FrmSaveGame.getNow( Form1.options.savesDirectoryFullPath, savePath );
			bool success;

			if ( path == "" ) 
			{ 
				path = userTextInput.getNow(
					language.getAString( language.order.filesNewFileDialogTitle ),
					language.getAString( language.order.filesNewFileDialogText ),
					possibleSaveName,
					language.getAString( language.order.ok ),
					language.getAString( language.order.cancel )
					);

				if ( path.Length > 0 && path != null )
					path = Form1.options.savesDirectoryFullPath + path + ".phs";

			} 

			if ( path != null && path.Length > 0 )
			{
			//	wC.show = true;

		//		savePath = path; // sfd.FileName;
				success = save( path, true );
				Form1.options.save();

		//		wC.show = false;
				success = true;
			}
			else success = false;

			return success;
		}
#endregion
		
#region autosave
		public void autosave()
		{
			save( platformSpec.main.appPath + @"\saves\auto\" + possibleSaveName + ".phs", false ); //playerList[ Form1.game.curPlayerInd ].playerName + " - " + Statistics.civilizations[ playerList[ Form1.game.curPlayerInd ].civType ].name + ".phs" 
		}
#endregion
		
#region save game
		public bool save()
		{
			if ( savePath == "" )
				return saveAsGetPath();
			else 
				return save( savePath, true );
		}

		private bool save( string sp, bool keepPath )
		{
			wC.show = true;

			if ( keepPath )
				savePath = sp;

			FileStream file = null;
			BinaryWriter txtOut = null;
			bool success = false;

			try
			{		
				file = new FileStream( sp, FileMode.Create, FileAccess.Write ); 
				txtOut = new BinaryWriter( file ); 

				txtOut.Write( (Int32)887 ); 

				if ( this is Scenario )
				{
					txtOut.Write( (byte)FileHeader.Type.playedScenario ); 
					txtOut.Write( ((Scenario)this).name ); 
					txtOut.Write( ((Scenario)this).description );
					txtOut.Write( this.curTurn ); 

					saveImage( txtOut );

					txtOut.Write( (byte)((Scenario)this).goalType );
					txtOut.Write( ((Scenario)this).goalInd );
					
				}
				else
				{
					txtOut.Write( (byte)FileHeader.Type.playedGame ); 

					txtOut.Write( playerList[ curPlayerInd ].playerName ); // name
					txtOut.Write( Statistics.civilizations[ playerList[ Form1.game.curPlayerInd ].civType ].name ); // civ
					txtOut.Write( this.curTurn ); // turn

					saveImage( txtOut );
				}

				#region options

				txtOut.Write( (byte)Form1.options.frontierType ); 
				txtOut.Write( (byte)Form1.options.miniMapType ); 
				txtOut.Write( Form1.options.hideUndiscovered ); 
				txtOut.Write( Form1.options.showGrid ); 
				txtOut.Write( Form1.options.showOnScreenDPad ); 
				txtOut.Write( Form1.options.showBatteryStatus ); 
				txtOut.Write( Form1.options.showLabels ); 
				txtOut.Write( Form1.options.showCommonSpyDialogs ); 
				txtOut.Write( Form1.options.autosave ); 

				#endregion

				txtOut.Write( Form1.sliHor ); 
				txtOut.Write( Form1.sliVer ); 

				saveCore( txtOut );

				success = true;
			}
			catch ( System.IO.DirectoryNotFoundException e )
			{
				MessageBox.Show( "Unable to save, directory not found.", e.Message );
				return false;
			}
			catch ( System.IO.IOException e )
			{
				MessageBox.Show( "Undefined exception, possible cause:\nOut of memory on selected storage", e.Message );
				return false;
			}
			catch ( Exception e )
			{
				MessageBox.Show( "Error", e.Message );
				return false;
			}
			finally
			{		
				txtOut.Close();
				file.Close();
			}		

			wC.show = false;
			return success;
		}
		#endregion

#region saveAsScenario
		public bool saveAsScenario( string name, string description, string path, Scenario.GoalType goal, int goalInd )
		{
			bool success;
			BinaryWriter writer = null;
			FileStream file = null;

			try
			{
				file = new FileStream( path, FileMode.Create, FileAccess.Write );
				writer = new BinaryWriter( file );

				writer.Write( (int)887 ); 
				writer.Write( (byte)FileHeader.Type.scenario ); 
				writer.Write( name ); 
				writer.Write( description ); 
				writer.Write( curTurn ); 

				saveImage( writer );

				writer.Write( (byte)goal ); 
				writer.Write( goalInd ); 

				saveCore( writer );
				success = true;
			}
			catch ( Exception /*e*/ )
			{
				success = false;
			}
			finally
			{
				writer.Flush();
				writer.Close();
			}

			return success;
		}
#endregion

#region saveCore
		public void saveCore( BinaryWriter writer )
		{
			writer.Write( this.width ); 
			writer.Write( this.height ); 
			writer.Write( this.curTurn ); 
			
			writer.Write( Statistics.buildings.Length ); 
			writer.Write( Statistics.technologies.Length ); 

			#region this.grid
			for ( int x = 0; x < this.width; x++ )
				for ( int y = 0; y < this.height; y ++ )
				{
					writer.Write( this.grid[ x, y ].type );
					if ( 
						this.grid[ x, y ].type != (byte)enums.terrainType.sea && 
						this.grid[ x, y ].type != (byte)enums.terrainType.coast 
						)
					{
						if ( this.grid[ x, y ].city > 0 )
						{
							writer.Write( true );
							writer.Write( this.grid[ x, y ].city );
						}
						else
							writer.Write( false );

						//	writer.Write( this.grid[ x, y ].territory - 1 );
						writer.Write( this.grid[ x, y ].civicImprovement );
						writer.Write( this.grid[ x, y ].militaryImprovement );
						writer.Write( this.grid[ x, y ].roadLevel );
						writer.Write( this.grid[ x, y ].continent );

						if ( this.grid[ x, y ].resources > 0 )
						{
							writer.Write( true );
							writer.Write( this.grid[ x, y ].resources );
						}
						else
							writer.Write( false );

						writer.Write( this.grid[ x, y ].river );

						if ( this.grid[ x, y ].river )
							for ( int v = 0; v < this.grid[ x, y ].riversDir.Length; v ++ )
								writer.Write( this.grid[ x, y ].riversDir[ v ] );
					}

					if ( this.grid[ x, y ].laborCity > 0 )
					{
						writer.Write( true );
						writer.Write( this.grid[ x, y ].laborCity );
					}
					else
						writer.Write( false );
					
					if ( this.grid[ x, y ].stack.Length > 0 )
					{
						writer.Write( true );
						writer.Write( this.grid[ x, y ].stack.Length );
						writer.Write( this.grid[ x, y ].stackPos );
					}
					else
						writer.Write( false );
				}
			#endregion

			#region stack
			for ( int x = 0; x < this.width; x++ )
				for ( int y = 0; y < this.height; y ++ )
					for ( int i = 1; i <= this.grid[ x, y ].stack.Length; i ++ )
					{
						writer.Write( this.grid[ x, y ].stack[ i - 1 ].player.player );
						writer.Write( this.grid[ x, y ].stack[ i - 1 ].ind );
					}
			#endregion

			writer.Write( this.difficulty );
			writer.Write( this.playerList.Length );

			#region players ###
			for ( int j = 0; j < this.playerList.Length; j ++ )
			{
				writer.Write( this.playerList[ j ].playerName );
				writer.Write( this.playerList[ j ].civType );

				writer.Write( this.playerList[ j ].cityNumber );
				writer.Write( this.playerList[ j ].unitNumber );

				writer.Write( this.playerList[ j ].dead );

				if ( !this.playerList[ j ].dead )
				{
					writer.Write( this.playerList[ j ].currentResearch );
					writer.Write( this.playerList[ j ].money );

					writer.Write( this.playerList[ j ].preferences.laborFood );
					writer.Write( this.playerList[ j ].preferences.laborProd );
					writer.Write( this.playerList[ j ].preferences.laborTrade );

					writer.Write( this.playerList[ j ].preferences.science );
					writer.Write( this.playerList[ j ].preferences.reserve );
					writer.Write( this.playerList[ j ].preferences.buildings );
					writer.Write( this.playerList[ j ].preferences.culture );
					writer.Write( this.playerList[ j ].preferences.intelligence );
					writer.Write( this.playerList[ j ].preferences.military );
					writer.Write( this.playerList[ j ].preferences.space );
					
					writer.Write( this.playerList[ j ].counterIntNbr );
					
					writer.Write( this.playerList[ j ].govType.type );
					writer.Write( this.playerList[ j ].economyType );
					writer.Write( this.playerList[ j ].posInCityFile );  //863

					#region foreign relation
					for ( int i = 0; i < this.playerList.Length; i ++ )
					{
						if ( 
							this.playerList[ j ].foreignRelation[ i ].madeContact == true && i != j
							)
						{
							writer.Write( true );
							writer.Write( (byte)this.playerList[ j ].foreignRelation[ i ].quality );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].politic );

							writer.Write( this.playerList[ j ].foreignRelation[ i ].embargo );

							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].nbr );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].efficiency );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].efficiency );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].efficiency );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr );
							writer.Write( this.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].efficiency );
						}
						else
							writer.Write( false );
					}
					#endregion

					#region see and discover
					for ( int x = 0; x < this.width; x ++ )
						for ( int y = 0; y < this.height; y ++ )
							writer.Write( this.playerList[ j ].see[ x, y ] );

					for ( int x = 0; x < this.width; x ++ )
						for ( int y = 0; y < this.height; y ++ )
							if ( !this.playerList[ j ].see[ x, y ] )
								writer.Write( this.playerList[ j ].discovered[ x, y ] );
					#endregion

					#region last seen this.grid 
					for ( int x = 0; x < this.width; x ++ )
						for ( int y = 0; y < this.height; y ++ )
							if ( !this.playerList[ j ].see[ x, y ] && this.playerList[ j ].discovered[ x, y ] )
							{
								if ( Statistics.terrains[ this.grid[ x, y ].type ].ew == 1 )
								{
									writer.Write( this.playerList[ j ].lastSeen[ x, y ].cityPop );

									if ( this.playerList[ j ].lastSeen[ x, y ].cityPop > 0 )
										writer.Write( this.playerList[ j ].lastSeen[ x, y ].city );

									writer.Write( this.playerList[ j ].lastSeen[ x, y ].civicImp );
									writer.Write( this.playerList[ j ].lastSeen[ x, y ].militaryImp );
									writer.Write( this.playerList[ j ].lastSeen[ x, y ].road );
							
									writer.Write( this.playerList[ j ].lastSeen[ x, y ].territory );
								}
								else if ( this.grid[ x, y ].type == (byte)enums.terrainType.coast )
								{
									writer.Write( this.playerList[ j ].lastSeen[ x, y ].territory );
								}

								writer.Write( this.playerList[ j ].lastSeen[ x, y ].turn );
							}
					#endregion

					#region techno
					for ( int i = 0; i < Statistics.technologies.Length; i ++ )
					{
						if ( this.playerList[ j ].technos[ i ].researched == true )
						{
							writer.Write( (byte)1 );
						}
						else if ( this.playerList[ j ].technos[ i ].pntDiscovered > 0 )
						{
							writer.Write( (byte)2 );
							writer.Write( this.playerList[ j ].technos[ i ].pntDiscovered );
						}
						else
						{
							writer.Write( (byte)0 );
						}
					}
					#endregion

					#region units
					for ( int i = 1; i <= this.playerList[ j ].unitNumber; i ++ )
					{
						writer.Write( this.playerList[ j ].unitList[ i ].state );

						if ( this.playerList[ j ].unitList[ i ].state != (byte)Form1.unitState.dead )
						{
							writer.Write( this.playerList[ j ].unitList[ i ].type ); 
							writer.Write( this.playerList[ j ].unitList[ i ].health ); 
							writer.Write( this.playerList[ j ].unitList[ i ].level ); 
							writer.Write( this.playerList[ j ].unitList[ i ].moveLeft ); 
							writer.Write( this.playerList[ j ].unitList[ i ].moveLeftFraction ); 
							writer.Write( this.playerList[ j ].unitList[ i ].transported ); 
							writer.Write( this.playerList[ j ].unitList[ i ].X ); 
							writer.Write( this.playerList[ j ].unitList[ i ].Y ); 
							writer.Write( this.playerList[ j ].unitList[ i ].dest.X ); 
							writer.Write( this.playerList[ j ].unitList[ i ].dest.Y ); 

							for ( int k = 0; k < this.playerList[ j ].unitList[ i ].transported; k ++ ) 
								writer.Write( this.playerList[ j ].unitList[ i ].transport[ k ] ); 
							
							writer.Write( this.playerList[ j ].unitList[ i ].automated ); 
						}
					}
					#endregion

					#region cities
					for ( int i = 1; i <= this.playerList[ j ].cityNumber; i ++ )
					{
						writer.Write( this.playerList[ j ].cityList[ i ].state );
						writer.Write( this.playerList[ j ].cityList[ i ].name );

						if ( this.playerList[ j ].cityList[ i ].state != (byte)enums.cityState.dead )
						{
							writer.Write( this.playerList[ j ].cityList[ i ].construction.points );

							// 0862 
							for ( int k = 0; k < 10; k ++ ) 
							{ 
								if ( this.playerList[ j ].cityList[ i ].construction.list[ k ] == null )
									writer.Write( false );
								else
								{
									writer.Write( true );
									writer.Write( this.playerList[ j ].cityList[ i ].construction.list[ k ].getConstructionType() ); 
									writer.Write( this.playerList[ j ].cityList[ i ].construction.list[ k ].type ); 
								}
							} 
							// end 0862 

							writer.Write( this.playerList[ j ].cityList[ i ].foodReserve );
							writer.Write( this.playerList[ j ].cityList[ i ].laborOnField );
							writer.Write( this.playerList[ j ].cityList[ i ].originalOwner );
							writer.Write( this.playerList[ j ].cityList[ i ].population );
							writer.Write( this.playerList[ j ].cityList[ i ].X );
							writer.Write( this.playerList[ j ].cityList[ i ].Y );

							for ( int k = 0; k < this.playerList[ j ].cityList[ i ].laborOnField; k ++ )
							{
								writer.Write( this.playerList[ j ].cityList[ i ].laborPos[ k ].X );
								writer.Write( this.playerList[ j ].cityList[ i ].laborPos[ k ].Y );
							}

							for ( int k = 0; k < this.playerList[ j ].cityList[ i ].buildingList.Length ; k ++ )
								writer.Write( this.playerList[ j ].cityList[ i ].buildingList[ k ] );

							writer.Write( this.playerList[ j ].cityList[ i ].isCapitale );

							//0877
							writer.Write( this.playerList[ j ].cityList[ i ].nonLabor.list.Length );
							for ( int k = 0; k < this.playerList[ j ].cityList[ i ].nonLabor.list.Length; k++ )
								writer.Write( this.playerList[ j ].cityList[ i ].nonLabor.list[ k ] );

							writer.Write( this.playerList[ j ].cityList[ i ].slaves.list.Length );
							for ( int k = 0; k < this.playerList[ j ].cityList[ i ].slaves.list.Length; k++ )
								writer.Write( this.playerList[ j ].cityList[ i ].slaves.list[ k ] );
							//end of 0877
						}
					}
					#endregion

					#region memories 0840

					writer.Write( this.playerList[ j ].memory.list.Length );
					for ( int i = 0; i < this.playerList[ j ].memory.Lenght; i ++ )
					{
						writer.Write( this.playerList[ j ].memory.list[ i ].type );
						writer.Write( this.playerList[ j ].memory.list[ i ].turn );
						writer.Write( this.playerList[ j ].memory.list[ i ].ind.Length );

						for ( int k = 0; k < this.playerList[ j ].memory.list[ i ].ind.Length; k ++ )
							writer.Write( this.playerList[ j ].memory.list[ i ].ind[ k ] );
					}

					#endregion

					#region sats 0871

					//	this.playerList[ j ].Sats.

					#endregion

					#region slaves 0878
					writer.Write( this.playerList[ j ].slaves.transferts.Length ); 
					for ( int i = 0; i < this.playerList[ j ].slaves.transferts.Length; i++ )
					{
						writer.Write( this.playerList[ j ].slaves.transferts[ i ].dest );
						writer.Write( this.playerList[ j ].slaves.transferts[ i ].ori );
						writer.Write( this.playerList[ j ].slaves.transferts[ i ].nbr );
						writer.Write( this.playerList[ j ].slaves.transferts[ i ].eta );
					}
					#endregion
				}
			}
			#endregion

			#region caseImp
			writer.Write( this.caseImps.Length );
			for ( int i = 0; i < this.caseImps.Length; i ++ )
			{
				writer.Write( this.caseImps[ i ].construction );
				writer.Write( this.caseImps[ i ].constructionPntLeft );
				writer.Write( this.caseImps[ i ].owner );
				writer.Write( this.caseImps[ i ].pos.X );
				writer.Write( this.caseImps[ i ].pos.Y );
				writer.Write( this.caseImps[ i ].type );
				writer.Write( this.caseImps[ i ].units.Length );

				for ( int j = 0; j < this.caseImps[ i ].units.Length; j ++ )
				{
					writer.Write( this.caseImps[ i ].units[ j ].player.player );
					writer.Write( this.caseImps[ i ].units[ j ].ind );
				}

			}
			#endregion

			#region labels
			writer.Write( label.list.Length );
			for ( int j = 0; j < label.list.Length; j ++ )
			{
				writer.Write( label.list[ j ].X );
				writer.Write( label.list[ j ].Y );
				writer.Write( label.list[ j ].text );
			}
			#endregion

			#region tutorial 857 returned 885

			writer.Write( Tutorial.mode );
			if ( Tutorial.mode )
			{
				writer.Write( Tutorial.alreadySeen.Length );

				if ( Tutorial.mode )
					for ( int i = 0; i < Tutorial.alreadySeen.Length; i ++ )
						writer.Write( Tutorial.alreadySeen[ i ] );
			}

			#endregion

			#region custom nation 871 mod at 887
			int customNations = Statistics.civilizations.Length - Statistics.normalCivilizationNumber;
			writer.Write( customNations );

			if ( customNations > 0 )//this.playerList[ Form1.game.curPlayerInd ].civType == Statistics.normalCivilizationNumber )
			{
			//	writer.Write( customNations );//true );

				for ( int n = Statistics.normalCivilizationNumber; n < Statistics.civilizations.Length; n ++ )
				{
					writer.Write( Statistics.civilizations[ n ].name );
					writer.Write( Statistics.civilizations[ n ].color.R );
					writer.Write( Statistics.civilizations[ n ].color.G );
					writer.Write( Statistics.civilizations[ n ].color.B );
					writer.Write( Statistics.civilizations[ n ].description );

					byte citiesFrom = 0;
					for ( byte i = 0; i <  Statistics.normalCivilizationNumber; i ++ )
						if ( Statistics.civilizations[ i ].cityNames[ 0 ] == Statistics.civilizations[ n ].cityNames[ 0 ] )
						{
							citiesFrom = i;
							break;
						}

					writer.Write( citiesFrom );
				}
			}
		/*	else
				writer.Write( (int)0 ); // false );*/

			#endregion
		}
#endregion

#region loadCore
		public static void loadCore( Game game, BinaryReader reader )
		{
			int width = reader.ReadInt32(),
				height = reader.ReadInt32();
			game.curTurn = reader.ReadInt32();

			game.initGrid( width, height );


			int buildingNbr = reader.ReadInt32(),
				tempTechnoNbr = reader.ReadInt32();

			// temp
			structures.sStack[][][] tempStacks = new xycv_ppc.structures.sStack[ game.width ][][];

			for ( int x = 0; x < game.width; x ++ )
				tempStacks[ x ] = new xycv_ppc.structures.sStack[ game.height ][];

			#region game.grid
			//		game.grid = new structures.sGrid[ game.width, game.height ];
			for ( int x = 0; x < game.width; x++ )
				for ( int y = 0; y < game.height; y++ )
				{
					game.grid[ x, y ].type = reader.ReadByte();
					if ( game.grid[ x, y ].type != (byte)enums.terrainType.sea && game.grid[ x, y ].type != (byte)enums.terrainType.coast )
					{
						if ( reader.ReadBoolean() )
							game.grid[ x, y ].city = reader.ReadInt32();
						else
							game.grid[ x, y ].city = 0;

						game.grid[ x, y ].civicImprovement = reader.ReadByte();
						game.grid[ x, y ].militaryImprovement = reader.ReadByte();
						game.grid[ x, y ].roadLevel = reader.ReadByte();
						game.grid[ x, y ].continent = reader.ReadByte();

						if ( reader.ReadBoolean() )
							game.grid[ x, y ].resources = reader.ReadByte();

						game.grid[ x, y ].river = reader.ReadBoolean(); // 863 

						if ( game.grid[ x, y ].river )
						{
							game.grid[ x, y ].riversDir = new bool[ 8 ];
							for ( int v = 0; v < game.grid[ x, y ].riversDir.Length; v++ )
								game.grid[ x, y ].riversDir[ v ] = reader.ReadBoolean();
						}
					}

					if ( reader.ReadBoolean() )
						game.grid[ x, y ].laborCity = reader.ReadInt32();
					else
						game.grid[ x, y ].laborCity = 0;


					if ( reader.ReadBoolean() )
					{
							
						tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ reader.ReadInt32() ];

						game.grid[ x, y ].stack = new UnitList[ tempStacks[ x ][ y ].Length ];
						game.grid[ x, y ].stackPos = reader.ReadInt32();
					}
					else
					{
							
						tempStacks[ x ][ y ] = new xycv_ppc.structures.sStack[ 0 ];

						game.grid[ x, y ].stack = new UnitList[ 0 ];
						game.grid[ x, y ].stackPos = 0;
					}
				}
			#endregion

			#region stack
			for ( int x = 0; x < game.width; x++ )
				for ( int y = 0; y < game.height; y++ )
					for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
					{
						tempStacks[ x ][ y ][ i - 1 ].owner = reader.ReadByte();
						tempStacks[ x ][ y ][ i - 1 ].unit = reader.ReadInt32();

						/*	game.grid[ x, y ].stack[ i - 1 ].player.player = reader.ReadByte();
										game.grid[ x, y ].stack[ i - 1 ].unit = reader.ReadInt32();*/
					}
			#endregion

			game.difficulty = reader.ReadByte();
			int nbrPlayer = reader.ReadInt32();
			game.playerList = new PlayerList[ nbrPlayer ]; 

			#region players # # #
			for ( int j = 0; j < nbrPlayer; j++ )
			{
				game.playerList[ j ] = new PlayerList( game, j );
				game.playerList[ j ].playerName = reader.ReadString();
				game.playerList[ j ].civType = reader.ReadByte();
				game.playerList[ j ].cityNumber = reader.ReadInt32();
				game.playerList[ j ].unitNumber = reader.ReadInt32();
				game.playerList[ j ].dead = reader.ReadBoolean();
				if ( game.playerList[ j ].dead )
				{
					game.playerList[ j ].cityNumber = 0;
					game.playerList[ j ].unitNumber = 0;
				}

				if ( !game.playerList[ j ].dead )
				{
					game.playerList[ j ].currentResearch = reader.ReadByte();
					game.playerList[ j ].money = reader.ReadInt64();
					game.playerList[ j ].preferences.laborFood = reader.ReadSByte();
					game.playerList[ j ].preferences.laborProd = reader.ReadSByte();
					game.playerList[ j ].preferences.laborTrade = reader.ReadSByte();
					game.playerList[ j ].preferences.science = reader.ReadSByte();
					game.playerList[ j ].preferences.reserve = reader.ReadSByte();
					game.playerList[ j ].preferences.buildings = reader.ReadSByte();
					game.playerList[ j ].preferences.culture = reader.ReadSByte();
					game.playerList[ j ].preferences.intelligence = reader.ReadSByte();
					game.playerList[ j ].preferences.military = reader.ReadSByte();
					game.playerList[ j ].preferences.space = reader.ReadSByte();
					game.playerList[ j ].counterIntNbr = reader.ReadInt32();
					game.playerList[ j ].govType = Statistics.governements[ reader.ReadByte() ];
					game.playerList[ j ].economyType = reader.ReadByte();
					game.playerList[ j ].posInCityFile = reader.ReadByte();
					game.playerList[ j ].technos = new structures.technoList[ Statistics.technologies.Length ];
					game.playerList[ j ].discovered = new bool[ game.width, game.height ];
					game.playerList[ j ].see = new bool[ game.width, game.height ];
					game.playerList[ j ].unitList = new UnitList[ game.playerList[ j ].unitNumber + 10 ];
					game.playerList[ j ].cityList = new CityList[ game.playerList[ j ].cityNumber + 10 ];
					game.playerList[ j ].foreignRelation = new structures.sForeignRelation[ nbrPlayer ]; 
			
					#region foreignRelations
					for ( int i = 0; i < nbrPlayer; i++ )
					{
						game.playerList[ j ].foreignRelation[ i ].spies = new xycv_ppc.structures.sSpies[ 4 ];
						for ( int k = 0; k < 4; k++ )
							game.playerList[ j ].foreignRelation[ i ].spies[ k ] = new xycv_ppc.structures.sSpies();

						game.playerList[ j ].foreignRelation[ i ].madeContact = reader.ReadBoolean();
						if ( game.playerList[ j ].foreignRelation[ i ].madeContact )
						{
							game.playerList[ j ].foreignRelation[ i ].quality = reader.ReadByte();
							game.playerList[ j ].foreignRelation[ i ].politic = reader.ReadByte();
							game.playerList[ j ].foreignRelation[ i ].embargo = reader.ReadBoolean();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].nbr = reader.ReadInt32();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.people ].efficiency = reader.ReadByte();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].nbr = reader.ReadInt32();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.gov ].efficiency = reader.ReadByte();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].nbr = reader.ReadInt32();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.military ].efficiency = reader.ReadByte();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].nbr = reader.ReadInt32();
							game.playerList[ j ].foreignRelation[ i ].spies[ (byte)enums.spyType.science ].efficiency = reader.ReadByte();
						}
					}
					#endregion

					#region see & discovered
					for ( int x = 0; x < game.width; x++ )
						for ( int y = 0; y < game.height; y++ )
							game.playerList[ j ].see[ x, y ] = reader.ReadBoolean();

					for ( int x = 0; x < game.width; x++ )
						for ( int y = 0; y < game.height; y++ )
							if ( !game.playerList[ j ].see[ x, y ] )
								game.playerList[ j ].discovered[ x, y ] = reader.ReadBoolean();
							else
								game.playerList[ j ].discovered[ x, y ] = true;
					#endregion
					
					#region last seen game.grid
					
					game.playerList[ j ].lastSeen = new structures.lastSeen[ game.width, game.height ];
					for ( int x = 0; x < game.width; x++ )
						for ( int y = 0; y < game.height; y++ )
							if ( !game.playerList[ j ].see[ x, y ] && game.playerList[ j ].discovered[ x, y ] )
							{
								if ( Statistics.terrains[ game.grid[ x, y ].type ].ew == 1 )
								{
									game.playerList[ j ].lastSeen[ x, y ].cityPop = reader.ReadByte();
									if ( game.playerList[ j ].lastSeen[ x, y ].cityPop > 0 )
										game.playerList[ j ].lastSeen[ x, y ].city = reader.ReadInt32();

									game.playerList[ j ].lastSeen[ x, y ].civicImp = reader.ReadByte();
									game.playerList[ j ].lastSeen[ x, y ].militaryImp = reader.ReadByte();
									game.playerList[ j ].lastSeen[ x, y ].road = reader.ReadByte();
									game.playerList[ j ].lastSeen[ x, y ].territory = reader.ReadByte();
								}
								else if ( game.grid[ x, y ].type == (byte)enums.terrainType.coast )
								{
									game.playerList[ j ].lastSeen[ x, y ].territory = reader.ReadByte();
								}

								game.playerList[ j ].lastSeen[ x, y ].turn = reader.ReadInt32();
							}
					#endregion

					#region techno
					for ( int i = 0; i < tempTechnoNbr; i++ )
					{
						byte num = reader.ReadByte();

						if ( i < Statistics.technologies.Length )
							switch ( num )
							{
								case 1 :
									if ( i < game.playerList[ j ].technos.Length )
										game.playerList[ j ].technos[ i ].researched = true;

									break;

								case 2 :
									if ( i < game.playerList[ j ].technos.Length )
									{
										game.playerList[ j ].technos[ i ].researched = false;
										game.playerList[ j ].technos[ i ].pntDiscovered = reader.ReadInt32();
									}
									else
									{
										reader.ReadInt32();
									}

									break;

								default :
									if ( i < game.playerList[ j ].technos.Length )
										game.playerList[ j ].technos[ i ].researched = false;

									break;
							}
					}
					#endregion

					#region units
					for ( int i = 1; i <= game.playerList[ j ].unitNumber; i++ )
					{
						game.playerList[ j ].unitList[ i ] = new UnitList( game.playerList[ j ], i );
						game.playerList[ j ].unitList[ i ].state = reader.ReadByte();
						if ( game.playerList[ j ].unitList[ i ].state != (byte)Form1.unitState.dead )
						{
							game.playerList[ j ].unitList[ i ].type = reader.ReadByte();
							game.playerList[ j ].unitList[ i ].health = reader.ReadSByte();
							game.playerList[ j ].unitList[ i ].level = reader.ReadByte();
							game.playerList[ j ].unitList[ i ].moveLeft = reader.ReadSByte();
							game.playerList[ j ].unitList[ i ].moveLeftFraction = reader.ReadSByte();
							game.playerList[ j ].unitList[ i ].transported = reader.ReadInt32();
							game.playerList[ j ].unitList[ i ].X = reader.ReadInt32();
							game.playerList[ j ].unitList[ i ].Y = reader.ReadInt32();
							game.playerList[ j ].unitList[ i ].dest.X = reader.ReadInt32();
							game.playerList[ j ].unitList[ i ].dest.Y = reader.ReadInt32();
							game.playerList[ j ].unitList[ i ].transport = new int[ Statistics.units[ game.playerList[ j ].unitList[ i ].type ].transport ];
							for ( int k = 0; k < game.playerList[ j ].unitList[ i ].transported; k++ )
								game.playerList[ j ].unitList[ i ].transport[ k ] = reader.ReadInt32();

							game.playerList[ j ].unitList[ i ].automated = reader.ReadBoolean(); //842
						}
					}
					#endregion

					#region cities
					for ( int i = 1; i <= game.playerList[ j ].cityNumber; i++ )
					{
						game.playerList[ j ].cityList[ i ] = new CityList( game.playerList[ j ], i );
						game.playerList[ j ].cityList[ i ].state = reader.ReadByte();
						game.playerList[ j ].cityList[ i ].name = reader.ReadString();
						if ( game.playerList[ j ].cityList[ i ].state != (byte)enums.cityState.dead )
						{
							game.playerList[ j ].cityList[ i ].construction.points = reader.ReadInt32();

							// 0862
							for ( int k = 0; k < 10; k++ )
							{
								if ( reader.ReadBoolean() )
								{
									byte cType = reader.ReadByte(),
										type = reader.ReadByte();

									switch ( cType )
									{
										case Stat.Unit.constructionType:
											game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.units[ type ];
											break;
											
										case Stat.Building.constructionType:
											game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.buildings[ type ];
											break;
											
										case Stat.Wealth.constructionType:
											game.playerList[ j ].cityList[ i ].construction.list[ k ] = new Stat.Wealth();
											break;
											
										case Stat.SmallWonder.constructionType:
											game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.smallWonders[ type ];
											break;
											
										case Stat.Wonder.constructionType:
											game.playerList[ j ].cityList[ i ].construction.list[ k ] = Statistics.wonders[ type ];
											break;
									}

								//	game.playerList[ j ].cityList[ i ].construction.list[ k ].ind = reader.ReadInt32();
								//	game.playerList[ j ].cityList[ i ].construction.list[ k ].type = reader.ReadByte();
								}
							}

							// end 0862
							game.playerList[ j ].cityList[ i ].foodReserve = reader.ReadInt32();
							game.playerList[ j ].cityList[ i ].laborOnField = reader.ReadByte();
							game.playerList[ j ].cityList[ i ].originalOwner = reader.ReadByte();
							game.playerList[ j ].cityList[ i ].population = reader.ReadByte();
							game.playerList[ j ].cityList[ i ].X = reader.ReadInt32();
							game.playerList[ j ].cityList[ i ].Y = reader.ReadInt32();
							game.playerList[ j ].cityList[ i ].laborPos = new Point[ game.playerList[ j ].cityList[ i ].laborOnField ];
							for ( int k = 0; k < game.playerList[ j ].cityList[ i ].laborOnField; k++ )
							{
								game.playerList[ j ].cityList[ i ].laborPos[ k ].X = reader.ReadInt32();
								game.playerList[ j ].cityList[ i ].laborPos[ k ].Y = reader.ReadInt32();
							}

							game.playerList[ j ].cityList[ i ].buildingList = new bool[ Statistics.buildings.Length ];
							for ( int k = 0; k < buildingNbr; k++ )
								if ( k < game.playerList[ j ].cityList[ i ].buildingList.Length )
									game.playerList[ j ].cityList[ i ].buildingList[ k ] = reader.ReadBoolean();
								else
									reader.ReadBoolean();

							bool cap = reader.ReadBoolean();

							game.playerList[ j ].cityList[ i ].isCapitale = cap;

							//0877
							game.playerList[ j ].cityList[ i ].nonLabor.list = new byte[ reader.ReadInt32() ];
							for ( int k = 0; k < game.playerList[ j ].cityList[ i ].nonLabor.list.Length; k++ )
								game.playerList[ j ].cityList[ i ].nonLabor.list[ k ] = reader.ReadByte();

							game.playerList[ j ].cityList[ i ].slaves.list = new byte[ reader.ReadInt32() ];
							for ( int k = 0; k < game.playerList[ j ].cityList[ i ].slaves.list.Length; k++ )
								game.playerList[ j ].cityList[ i ].slaves.list[ k ] = reader.ReadByte();
							//end of 0877

							game.playerList[ j ].cityList[ i ].setHasDirectAccessToRessource();
						}
					}
					#endregion

					#region memories 0840

					game.playerList[ j ].memory = new Memory( game.playerList[ j ], reader.ReadInt32() );
					for ( int i = 0; i < game.playerList[ j ].memory.Lenght; i++ )
					{
						game.playerList[ j ].memory.list[ i ].type = reader.ReadByte();
						game.playerList[ j ].memory.list[ i ].turn = reader.ReadInt32();
						game.playerList[ j ].memory.list[ i ].ind = new int[ reader.ReadInt32() ];
						for ( int k = 0; k < game.playerList[ j ].memory.list[ i ].ind.Length; k++ )
							game.playerList[ j ].memory.list[ i ].ind[ k ] = reader.ReadInt32();
					}

					#endregion

					#region slaves 0878
					game.playerList[ j ].slaves.transferts = new playerSlavery.transfertList[ reader.ReadInt32() ];
					for ( int i = 0; i < game.playerList[ j ].slaves.transferts.Length; i++ )
					{
						game.playerList[ j ].slaves.transferts[ i ].dest = reader.ReadInt32();
						game.playerList[ j ].slaves.transferts[ i ].ori = reader.ReadInt32();
						game.playerList[ j ].slaves.transferts[ i ].nbr = reader.ReadInt32();
						game.playerList[ j ].slaves.transferts[ i ].eta = reader.ReadInt32();
					}
					#endregion

				}
			}
			#endregion

			// temp stack 
			for ( int x = 0; x < game.width; x++ )
				for ( int y = 0; y < game.height; y++ )
					for ( int i = 1; i <= game.grid[ x, y ].stack.Length; i++ )
						game.grid[ x, y ].stack[ i - 1 ] = game.playerList[ tempStacks[ x ][ y ][ i - 1 ].owner ].unitList[ tempStacks[ x ][ y ][ i - 1 ].unit ];


			#region caseImp
			game.caseImps = new structures.caseImprovement[ reader.ReadInt32() ];
			for ( int i = 0; i < game.caseImps.Length; i++ )
			{
				game.caseImps[ i ].construction = reader.ReadByte();
				game.caseImps[ i ].constructionPntLeft = reader.ReadInt32();
				game.caseImps[ i ].owner = reader.ReadByte();
				game.caseImps[ i ].pos.X = reader.ReadInt32();
				game.caseImps[ i ].pos.Y = reader.ReadInt32();
				game.caseImps[ i ].type = reader.ReadByte();
				game.caseImps[ i ].units = new UnitList[ reader.ReadInt32() ];
				for ( int j = 0; j < game.caseImps[ i ].units.Length; j++ )
				{
					/*	game.caseImps[ i ].units[ j ].owner = reader.ReadByte();
									game.caseImps[ i ].units[ j ].unit = reader.ReadInt32();*/

					game.caseImps[ i ].units[ j ] = game.playerList[ reader.ReadByte() ].unitList[ reader.ReadInt32() ];
				}
			}
			#endregion

			#region label
			label.initList( reader.ReadInt32() );
			for ( int i = 0; i < label.list.Length; i++ )
			{
				label.list[ i ].X = reader.ReadInt32();
				label.list[ i ].Y = reader.ReadInt32();
				label.list[ i ].text = reader.ReadString();
			}
			#endregion

			#region tutorial 857 returned 885

			Tutorial.init( reader.ReadBoolean() );
			if ( Tutorial.mode )
			{
				int tutorialLength =  reader.ReadInt32();

				for ( int i = 0; i < tutorialLength; i ++ )
					if ( i < Tutorial.alreadySeen.Length )
						Tutorial.alreadySeen[ i ] = reader.ReadBoolean();
					else
						reader.ReadBoolean();
			}

			#endregion

			#region custom nation 872 mod 887
			int customNations = reader.ReadInt32();
			if ( customNations > 0 )//reader.ReadBoolean() ) // game.playerList[ Form1.game.curPlayerInd ].civType == Statistics.normalCivilizationNumber )
			{
				if ( Statistics.civilizations.Length > Statistics.normalCivilizationNumber )
					Statistics.initCivilizations();

				Stat.Civilization[] buffer = Statistics.civilizations;

				Statistics.civilizations = new Stat.Civilization[ buffer.Length + customNations ];

				for ( int i = 0; i < buffer.Length; i++ )
					Statistics.civilizations[ i ] = buffer[ i ];

				for ( int n = Statistics.normalCivilizationNumber; n < Statistics.civilizations.Length; n ++ )
				{
					Statistics.civilizations[ n ].name = reader.ReadString();
					Statistics.civilizations[ n ].color = Color.FromArgb( reader.ReadByte(), reader.ReadByte(), reader.ReadByte() );
					Statistics.civilizations[ n ].description = reader.ReadString();
					Statistics.civilizations[ n ].cityNames = Statistics.civilizations[ reader.ReadByte() ].cityNames;
				}
			}
			#endregion

			game.wonderList = new WonderList( Statistics.wonders.Length );
			game.wonderList.initialize();

			foreach ( PlayerList player in game.playerList )
				player.smallWonderList.initialize();
		}
#endregion

		private void saveImage( /*Game game,*/ BinaryWriter writer )
		{
			writer.Write( FileHeader.BmpWidth );
			writer.Write( FileHeader.BmpWidth * 3/2 );

			int bmpWidth = FileHeader.BmpWidth,
				bmpHeight = FileHeader.BmpWidth * 3/2;
			int[,] bmp = new int[ bmpWidth, bmpHeight ];

			for ( int x = 0; x < bmpWidth; x ++ )
				for ( int y = 0; y < bmpHeight; y ++ )
					if ( !playerList[ Form1.game.curPlayerInd ].discovered[ x * width / bmpWidth, y * height / bmpHeight ] )
						bmp[ x, y ] = Color.Black.ToArgb();
					else if ( grid[ x * width / bmpWidth, y * height / bmpHeight ].territory > 0 )
						bmp[ x, y ] = Statistics.civilizations[ playerList[ grid[ x * width / bmpWidth, y * height / bmpHeight ].territory - 1 ].civType ].color.ToArgb();
					else
						bmp[ x, y ] = Statistics.terrains[ grid[ x * width / bmpWidth, y * height / bmpHeight ].type ].color.ToArgb();

			int[] colors = new int[ playerList.Length + Statistics.terrains.Length ]; 
			int pos = 0; 
			for ( int x = 0; x < bmpWidth; x ++ ) 
				for ( int y = 0; y < bmpHeight; y ++ ) 
				{ 
					bool found = false;
					for ( int i = 0; i < pos; i ++ )
						if ( colors[ i ] == bmp[ x, y ] )
						{
							found = true;
							bmp[ x, y ] = i;
							break;
						}

					if ( !found )
					{
						colors[ pos ] = bmp[ x, y ];
						bmp[ x, y ] = pos;
						pos ++;
					}
				} 

			writer.Write( pos );
			for ( int i = 0; i < pos; i ++ )
				writer.Write( colors[ i ] );

			for ( int x = 0; x < bmpWidth; x ++ ) 
				for ( int y = 0; y < bmpHeight; y ++ ) 
					writer.Write( (byte)bmp[ x, y ] );
		}

	/*	public static void loadMap( FileHeader intro, BinaryReader reader )
		{
			intro.bmp = new Bitmap( reader.ReadInt32(), reader.ReadInt32() );
			Color[] colors = new Color[ reader.ReadInt32() ];

			for ( int c = 0; c < colors.Length; c ++ )
				colors[ c ] = Color.FromArgb( reader.ReadInt32() );

			for ( int x = 0; x < intro.bmp.Width; x ++ )
				for ( int y = 0; y < intro.bmp.Height; y ++ )
					intro.bmp.SetPixel( x, y, colors[ reader.ReadByte() ] );  // Color.FromArgb( reader.ReadInt32() ) );
		}	*/

		public string possibleSaveName
		{
			get
			{
				return playerList[ Form1.game.curPlayerInd ].playerName + " - " + playerList[ Form1.game.curPlayerInd ].civ.name;
			}
		}

		#region year s  interpreter
		public int year
		{
			get
			{
				return -4000 + curTurn * 20;
			}
		}

		public string yearString()
		{
			return yearString( year );
		}
		public static string yearString( int year )
		{
			if ( year < 0 ) 
				return String.Format( 
					language.getAString( language.order.mainYearBC ), 
					year * -1 
					); 
			else 
				return String.Format( 
					language.getAString( language.order.mainYearAD ), 
					year
					);
		}
		#endregion

		public enum State
		{
			ok,
			playerWon,
			playerLost
		}

		public void verifyState()
		{
			if ( curPlayer.dead )
				state = State.playerLost;
			else
			{
				bool oneCpuLeft = false;

				for ( int p = 0; p < playerList.Length; p ++ ) 
					if (
						p != curPlayerInd && 
						!playerList[ p ].dead
						)
					{
						oneCpuLeft = true;
						break;
					}

				if ( !oneCpuLeft )
					state = State.playerWon;
			}
		}

		public WonderList wonderList;

	/*	privatebool[] wondersBuilt;
	
		public bool canBuildWonder( Stat.Wonder w )
		{
			return !wondersBuilt[ w.type ];
		}
	
		public void buildWonder( Stat.Wonder w )
		{
			wondersBuilt[ w.type ] = true;
		}
	
		public void initializeWonderList()
		{
			wondersBuilt = new bool[ Statistics.wonders.Length ];

		/*	foreach ( PlayerList player in playerList )
			{
				for ( int c = 1; c <= player.cityNumber; c ++ )
					for (  )
			}*
		}*/
	}
}
