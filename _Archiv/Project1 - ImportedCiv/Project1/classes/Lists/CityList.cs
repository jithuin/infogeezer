using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for CityList.
	/// </summary>
	public class CityList
	{
		public CityList( PlayerList player, int city )
		{
			this.player = player;
			this.city = city;

			construction = new Construction();
			slaves = new citySlavery( player, city );
			nonLabor = new peopleNonLabor( player, city );
			hatrLastCalcul = -1;
		}

		public PlayerList player;

		public citySlavery slaves;
		public peopleNonLabor nonLabor;
		public int X, Y;
		public int city;
		
		public System.Drawing.Point pos
		{
			get 
			{
				return new System.Drawing.Point( X, Y );
			}
		}

		public Case onCase
		{
			get
			{
				return player.game.grid[ X, Y ];
			}
		}

//		public byte player;
		public string name;
		public byte originalOwner;
		public byte population;
		public int foodReserve;

		/// <summary>
		/// 0 = rien, 1 = units, 2 = buildings, 3 = wonders, 4 = wealth
		/// </summary>
		/*	public byte currConstType;
			public int constPoints;
			public int currConst;*/

		public bool[] buildingList;	
		public byte laborOnField;
		public System.Drawing.Point[] laborPos;

		public byte state; // 0 = ok, 1 = capturé, 2 = evacué
		public bool isCapitale;
		public bool rioting;

		public int happiness;
		public uint pollution;

#region has access to ressources

		public int[] hdatr; 
		private int hdatrLastCalcul; 
		private int hatrLastCalcul; 

		public void setHasDirectAccessToRessource()
		{
			hdatr = new int[ Statistics.resources.Length ];
			for ( int l = 0; l < laborOnField; l ++ )
				if ( player.game.grid[ laborPos[ l ].X, laborPos[ l ].Y ].resources >= 100 )
					hdatr[ player.game.grid[ laborPos[ l ].X, laborPos[ l ].Y ].resources - 100 ] ++;
		}

		public bool hasAccessToRessource( int ind ) 
		{ 
			if ( hdatr[ ind ] > 0 )
				return true;
		/*	else if ( player.hasAccessToResources( ind ) )
				return true;*/
			else
			{
				for ( int c = 1; c <= player.cityNumber; c ++ )
					if ( 
						!player.cityList[ c ].dead &&
						player.cityList[ c ].hdatr[ ind ] > 0 &&
						player.game.grid[ player.cityList[ c ].pos.X, player.cityList[ c ].pos.Y ].continent == player.game.grid[ pos.X, pos.Y ].continent 
						)
						return true;

				int cityReceivingExchanges = player.capital;
				if ( 
					player.resourcesFromOtherNation[ ind ] > 0 && 
					player.game.grid[ player.cityList[ cityReceivingExchanges ].pos.X, player.cityList[ cityReceivingExchanges ].pos.Y ].continent == player.game.grid[ pos.X, pos.Y ].continent 
					)
					return true;

				return false;
			}
			/*if ( hatrLastCalcul == player.game.curTurn ) 
			{ 
				return hatr[ ind ]; 
			} 
			else 
			{ 
				hatrLocally = new bool[ Statistics.resources.Length ];
				hatr = new bool[ Statistics.resources.Length ];
				Point[] sqr = game.radius.returnCityRadius( X, Y ); 

				for ( int i = 0; i < sqr.Length; i ++ ) 
					if ( player.game.grid[ sqr[ i ].X, sqr[ i ].Y ].resources >= 100 ) 
						if ( player.game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 == player )
						{ 
							hatrLocally[ player.game.grid[ sqr[ i ].X, sqr[ i ].Y ].resources - 100 ] = true;
							hatr[ player.game.grid[ sqr[ i ].X, sqr[ i ].Y ].resources - 100 ] = true;
						} 

				hatrLastCalcul = player.game.curTurn; 
				return hatr[ ind ]; 
			} */
	//		return true;
		} 
#endregion
		/*	
			return new bool[ 0 ];*/


		public Construction construction;

		/*	structures.constructionList[] constructionList = new xycv_ppc.structures.constructionList[ 10 ];
			/// <summary>
			/// 0 = rien, 1 = units, 2 = buildings, 3 = wonders, 4 = wealth
			/// </summary>
			public byte currConstType; 
			public int constPoints; 
			public int currConst; */
		
#region trade, prod and food

		private int lastTrade;
		private int lastProd;
		private int lastFood;
		private int lastTradeTurn;
		private int lastProdTurn;
		private int lastFoodTurn;

		public void invalidateLastTrade()
		{
			lastTradeTurn = -1;
			lastProdTurn = -1;
			lastFoodTurn = -1;
			player.invalidateTrade();
		}

		public int trade
		{
			get
			{
				if ( Statistics.economies[ player.economyType ].communism )
					return player.totalTrade / player.currentCityCount;
				else
					return realTrade;
			}
		}

		public int production
		{
			get
			{
				if ( Statistics.economies[ player.economyType ].communism )
					return player.totalProduction / player.currentCityCount;
				else
					return realProduction;
			}
		}

		public int food
		{
			get
			{
				if ( Statistics.economies[ player.economyType ].communism )
					return player.totalFood / player.currentCityCount;
				else
					return realFood;
			}
	}
		
		public int realProduction
		{
			get
			{
				if ( lastProdTurn == player.game.curTurn )
					return lastProd;
				else
				{
					int tot = getPFT.getCaseProd( pos.X, pos.Y ); // + population
			
					for ( byte i = 0; i < laborPos.Length; i ++ )
						tot +=	getPFT.getCaseProd( laborPos[ i ].X, laborPos[ i ].Y );

					lastProd = (int)( tot * ( 1 + 0.33 * slaves.totalOfOneType( (byte)citySlavery.types.prod ) ) * player.prodModifier );

					if ( lastProd < 1 )
						lastProd = 1;

					lastProdTurn = player.game.curTurn;
					return lastProd;
				}
			}
		}
		
		public int realFood
		{
			get
			{
				if ( lastFoodTurn == player.game.curTurn )
					return lastFood;
				else
				{
					int tot = getPFT.getCaseFood( pos.X, pos.Y );
			
					for ( byte i = 0; i < laborPos.Length; i ++ )
						tot +=	getPFT.getCaseFood( laborPos[ i ].X, laborPos[ i ].Y );

					lastFood = (int)(tot * ( 1 + 0.33F * slaves.totalOfOneType( (byte)citySlavery.types.food ) ) * player.foodModifier );

					if ( lastFood < 1 )
						lastFood = 1;

					lastFoodTurn = player.game.curTurn;
					return lastFood;
				}
			}
		}
		
		public int realTrade
		{
			get
			{
				if ( lastTradeTurn == player.game.curTurn )
					return lastTrade;
				else
				{
					int tot = getPFT.getCaseTrade( pos.X, pos.Y );// + population + nonLabor.totalOfOneType( (byte)peopleNonLabor.types.trader ) * 2;
			
					for ( byte i = 0; i < laborPos.Length; i ++ )
						tot +=	getPFT.getCaseTrade( laborPos[ i ].X, laborPos[ i ].Y );

					lastTrade = (int)(tot * player.tradeModifier);

					if ( lastTrade < 1 )
						lastTrade = 1;

					lastTradeTurn = player.game.curTurn;
					return lastTrade;
				}
			}
		}
#endregion

		public int buildingCount
		{
			get
			{
				int tot = 0;

				for ( int i = 0; i < buildingList.Length; i++ )
					if ( buildingList[ i ] )
						tot++;

				return tot;
			}
		}

		public int citySize
		{
			get
			{
				if ( population < 4 )
					return 0;
				else if ( population < 10 )
					return 1;
				else
					return 2;
			}
		}

		public bool dead
		{
			get
			{
				return state == (byte)enums.cityState.dead;
			}
		}

		public void transfertToPlayer( PlayerList nextOwner )
		{
	//		state = (byte)enums.cityState.dead;

	/*		nextOwner.cityNumber ++;
				
			if ( nextOwner.cityNumber >= nextOwner.cityList.Length )
			{
				CityList[] cityListBuffer = nextOwner.cityList;
				nextOwner.cityList = new CityList[ cityListBuffer.Length + 5 ];

				for ( int i = 0; i < cityListBuffer.Length; i ++ )
					nextOwner.cityList[ i ] = cityListBuffer[ i ];
			}

			newCity = new CityList( nextOwner, nextOwner.cityNumber );*/

			CityList newCity = nextOwner.addCity();

			//		getTreasure( nextOwner.player, player, game.grid[ xDest, yDest ].city );

			#region get treasure

				byte[] posTechno = new byte[ Statistics.technologies.Length ];
				byte nbr = 0;
				for ( int i = 1; i < Statistics.technologies.Length; i ++ )
					if ( 
						nextOwner.technos[ i ].researched &&
						!player.technos[ i ].researched
						)
					{
						posTechno[ nbr ] = (byte)i;
						nbr ++;
					}

				if ( nbr > 0 )
				{
					Random r = new Random();

					byte technoChose = posTechno[ r.Next( nbr - 1 ) + 1 ];

					nextOwner.technos[ technoChose ].researched = true;
				}

				if ( 
					( 
						player.cityList[ city ].population > 6 || 
						player.cityList[ city ].isCapitale 
					) &&
					player.technos[ (byte)Form1.technoList.mapMaking ].researched 
					)
				{
					for ( int x = 0; x < Form1.game.width; x ++ )
						for ( int y = 0; y < Form1.game.height; y ++ )
							if ( Form1.game.grid[ x, y ].territory - 1 == player.player || player.discovered[ x, y ] )
								nextOwner.discovered[ x, y ] = true;
				}
				else if ( 
					player.cityList[ city ].population > 3 &&
					player.technos[ (byte)Form1.technoList.mapMaking ].researched  )
				{
					for ( int x = 0; x < Form1.game.width; x ++ )
						for ( int y = 0; y < Form1.game.height; y ++ )
							if ( Form1.game.grid[ x, y ].territory - 1 == player.player )
								nextOwner.discovered[ x, y ] = true;
				}
		#endregion

			labor.removeAllLabor( player.player, city );

			newCity.buildingList = this.buildingList;
			newCity.foodReserve = this.foodReserve;
			newCity.name = this.name;
			newCity.population = this.population;
			newCity.X = this.X;
			newCity.Y = this.Y;
			newCity.originalOwner = this.originalOwner;

			state = (byte)enums.cityState.dead;

			//	game.grid[ xDest, yDest ].territory - 1 = owner;
			player.game.grid[ X, Y ].city = newCity.city;

			if ( 
				nextOwner.economyType == (byte)enums.economies.slaveryRacial && 
				newCity.population > 0 
				)
			{
				int slavesToTransfert = newCity.population / 2;
				nextOwner.slaves.addSlave( slavesToTransfert );
				newCity.population -= (byte)slavesToTransfert;
				System.Windows.Forms.MessageBox.Show( 
					String.Format( language.getAString( language.order.slaveryAquiredFromCity ), slavesToTransfert, newCity.name ), 
					language.getAString( language.order.slaveryTitle ) 
					);
			}

			ai.chooseConstruction( nextOwner.player, nextOwner.cityNumber );

			player.game.frontier.setFrontiers();

			newCity.laborPos = new Point[ 0 ];
			labor.addAllLabor( nextOwner.player, nextOwner.cityNumber );
			newCity.setHasDirectAccessToRessource();

			bool oneCityLeft = false;
			for ( int i = 1; i <= player.cityNumber; i ++ )
				if ( player.cityList[ i ].state != (byte)enums.cityState.dead )
				{
					oneCityLeft = true;
					break;
				}

			if ( !oneCityLeft )
			{
				player.kill( nextOwner );
			}
			else if ( this.isCapitale )
			{
				bool found = false;
				for ( int c = 1; c <= player.cityNumber && !found; c ++ )
					if ( player.cityList[ c ].state != (byte)enums.cityState.dead )
					{
						found = true; 
						player.cityList[ c ].isCapitale = true; 
					}

				if ( !found )
					System.Windows.Forms.MessageBox.Show( "error in finding a capital" );
			}

			if ( player.player == Form1.game.curPlayerInd )
				open.cityFrm( newCity );	//owner, city, this );

		//		openCityFrm( owner, nextOwner.cityNumber );

		}

		public void buildConstruction( Form1 form1 ) // Building( Stat.Building building )
		{
			bool keepGoing = true;
			if ( 
				population == 1 && 
				construction.list[ 0 ] == Statistics.units[ (byte)Form1.unitType.colon ]
				)
			{
				if ( player.player == player.game.curPlayerInd )
				{
					Stat.Construction[] ib = ai.whatCanBeBuilt(
						player.player, 
						city 
						);
					string[] choices = uiWrap.buildingListStrings( player.player, city, ib ); 
											
					form1.zoomOnCity( player.player, city );

					userChoice ui = new userChoice(
						language.getAString( language.order.uiNewConstrution ),
						String.Format( language.getAString( language.order.uiNewConstrution ), name ),
						choices,
						0,
						language.getAString( language.order.ok ),
						language.getAString( language.order.uiGoToCity )
						);
					ui.ShowDialog();
					int res = ui.result;

					if ( res == -1 )
					{
						form1.openCityFrm( player.player, city );
						keepGoing = false;
					}
					else if ( res == 0 )
					{
						keepGoing = true;
						/*		construction.points -= Statistics.units[ construction.list[ 0 ].ind ].cost;
								Form1.game.curPlayer.createUnit ( X, Y, (byte)construction.list[ 0 ].ind );
								Form1.game.curPlayerInd.cityList[ i ].kill();
								continue;*/
					}
					else
					{ // si non
						construction.list[0] = ib[res];
						keepGoing = false;
						/*		Form1.construction.list[ 0 ].ind = ib[ res ].info;
								Form1.construction.list[ 0 ].type = ib[ res ].type;*/
					}
				}
				else
					ai.chooseConstruction( player.player, city );
			}
		//	else

			if ( keepGoing )
			{
				construction.points -= construction.list[ 0 ].cost; //Statistics.units[ construction.list[ 0 ].ind ].cost;
												
				if ( construction.list[ 0 ] is Stat.Unit ) // construction.list[ 0 ].type == 1 )
				{ // units
					player.createUnit( X, Y, (byte)construction.list[ 0 ].type );
												
					if ( construction.list[ 0 ] == Statistics.units[ (byte)Form1.unitType.colon ] ) //construction.list[ 0 ].ind == (byte)Form1.unitType.colon )
					{
						if ( population > 1 )
						{
							labor.autoRemoveLabor( player.player, city, true );
							population --;
						}
						else
						{
							kill();
						}
					}
				}
				else if ( construction.list[ 0 ] is Stat.Building )
				{ // buildings
					buildingList[ construction.list[ 0 ].type ] = true;
					aiPref.setBuildingUpkeep( player.player );

					#region special buildings
					#endregion
				}
				else if ( construction.list[ 0 ] is Stat.SmallWonder )
				{ // buildings
				//	buildingList[ construction.list[ 0 ].type ] = true;
				///	aiPref.setBuildingUpkeep( player.player );
				
					player.smallWonderList.buildWonder( construction.list[ 0 ] );

					#region special
					#endregion
				}
				else if ( construction.list[ 0 ] is Stat.Wonder )
				{ // buildings
				//	buildingList[ construction.list[ 0 ].type ] = true;
				//	aiPref.setBuildingUpkeep( player.player );
					
					player.game.wonderList.buildWonder( construction.list[ 0 ] );

					#region special
					switch ( construction.list[ 0 ].type )
					{
						case (int)Stat.Wonder.list.chinaWall:
							break;
					}
					#endregion
				}

				if ( !dead )
				{
					if ( player.isCurPlayer )
					{
						if ( construction.list[ 1 ] != null )
						{
							construction.removeFirst();
						}
						else
						{
							form1.zoomOnCity( player.player, city );
							uiWrap.choseConstruction( player.player, city, false ); 
						}
					}
					else
					{
						ai.chooseConstruction( player.player, city );
					}
				}
			}
		}

		public void kill()
		{
			state = (byte)enums.cityState.dead;
			player.game.grid[ X, Y ].city = 0;
			//	game.grid[ game.playerList[ owner ].cityList[ city ].X, game.playerList[ owner ].cityList[ city ].Y ].territory - 1 = 0;

			labor.removeAllLabor( player.player, city );
			
			player.game.rvtbc.doit();
		}

	}
}
