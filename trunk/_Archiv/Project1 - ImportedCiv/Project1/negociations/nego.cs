using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for nego.
	/// </summary>
	public class nego
	{
		public static int turnsForExchanges = 20;
		public enum infoType: byte
		{
			politicTreaty,
			economicTreaty,
			breakAllianceWith,
			embargoOn,
			warOn,
			votes,

			giveCity,
			giveRegion,
			giveTechno,
			giveContactWith,
			giveMoney,
			giveMoneyPerTurn,
			givePercBrut,
			giveMap,	//	0 = world, 1 = territory, 2 = sea
			giveResource,
			giveSlaves,
			
		//	shareDiscoveries, 
		//	shareSight,
		//	shareTechnoResearch

			threat,
		}

	/*	public struct List
		{
			bool accepted;
			byte type;
			int info;
			int[] conflict;
		//	int[] gain;
			byte player;
		}*/

		/*public enum uniType: byte
		{
			City,
			Region,
			Techno,
			ContactWith,
			Map, 
			Money,
			MoneyPerTurn,
			PercBrut,
			threat,
		}

		public enum biType: byte
		{
			politicTreaty,
			economicTreaty,
			breakAllianceWith,
			embargoOn,
			warOn,
			votes
		}*/

#region giveWorldMap
		public static void giveWorldMap(byte giver, byte given)
		{
			for ( int x = 0; x < Form1.game.width; x++ )
				for ( int y = 0; y < Form1.game.height; y++ )
					if ( Form1.game.grid[ x, y ].territory - 1 == giver || Form1.game.playerList[ giver ].discovered[ x, y ] )
					{
						Form1.game.playerList[ given ].discovered[ x, y ] = true;

						if ( Form1.game.playerList[ giver ].see[ x, y ] )
							sight.lastSee( x, y, given );
						else
							sight.transfertLastSeen( x, y, giver, given );
					}
		}
#endregion

#region giveTerritoryMap
		public static void giveTerritoryMap(byte giver, byte given)
		{
			for ( int x = 0; x < Form1.game.width; x++ )
				for ( int y = 0; y < Form1.game.height; y++ )
					if ( Form1.game.grid[ x, y ].territory - 1 == giver )
					{
						Form1.game.playerList[ given ].discovered[ x, y ] = true;
						sight.lastSee( x, y, given );
					}
		}	
#endregion

#region give city
		public static void giveCity( byte receiver, byte giver, int city )
		{
	//		Form1.giveCity( receiver, giver, city );
			int x = Form1.game.playerList[ giver ].cityList[ city ].X,
				y = Form1.game.playerList[ giver ].cityList[ city ].Y;

			#region move units


		/*	if ( Form1.game.grid[ x, y ].stack.Length > 0 )
			{
*/
				for ( int u = Form1.game.grid[ x, y ].stack.Length - 1; u >= 0; u -- )
					if ( Form1.game.grid[ x, y ].stack[ u ].player.player != receiver )
					{
						UnitList unitToMove = Form1.game.grid[ x, y ].stack[ u ];

						System.Drawing.Point dest = unitToMove.player.capitalCity.pos;
						move.moveUnitFromCase( x, y, unitToMove.player.player, unitToMove.ind );
						move.moveUnitToCase( dest.X, dest.Y, unitToMove.player.player, unitToMove.ind );
						
						/*	
						System.Drawing.Point dest = Form1.game.grid[ x, y ].stack[ u ].player.capitalCity.pos;
						move.moveUnitFromCase( x, y, Form1.game.grid[ x, y ].stack[ u ].player.player, Form1.game.grid[ x, y ].stack[ u ].ind );
						move.moveUnitToCase( dest.X, dest.Y, Form1.game.grid[ x, y ].stack[ u ].player.player, Form1.game.grid[ x, y ].stack[ u ].ind );
					*/
					}
		//	}

	#endregion


			Form1.game.playerList[ receiver ].cityNumber ++;
			
			if ( Form1.game.playerList[ receiver ].cityNumber >= Form1.game.playerList[ receiver ].cityList.Length )
			{
				CityList[] cityListBuffer = Form1.game.playerList[ receiver ].cityList;
				Form1.game.playerList[ receiver ].cityList = new CityList[ cityListBuffer.Length + 5 ];

				for ( int i = 0; i < cityListBuffer.Length; i ++ )
					Form1.game.playerList[ receiver ].cityList[ i ] = cityListBuffer[ i ];
			}

			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ] = new CityList( Form1.game.playerList[ receiver ], Form1.game.playerList[ receiver ].cityNumber );

			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].buildingList = 
				Form1.game.playerList[ giver ].cityList[ city ].buildingList;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].foodReserve = 
				Form1.game.playerList[ giver ].cityList[ city ].foodReserve;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].laborOnField = 
				Form1.game.playerList[ giver ].cityList[ city ].laborOnField;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].laborPos = 
				Form1.game.playerList[ giver ].cityList[ city ].laborPos;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].name = 
				Form1.game.playerList[ giver ].cityList[ city ].name;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].population = 
				Form1.game.playerList[ giver ].cityList[ city ].population;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].X = 
				Form1.game.playerList[ giver ].cityList[ city ].X;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].Y = 
				Form1.game.playerList[ giver ].cityList[ city ].Y;
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].originalOwner =
				Form1.game.playerList[ giver ].cityList[ city ].originalOwner;

			Form1.game.playerList[ giver ].cityList[ city ].state = (byte)enums.cityState.dead;

		//	Form1.game.grid[ x, y ].territory - 1 = receiver;
			Form1.game.grid[ x, y ].city = Form1.game.playerList[ receiver ].cityNumber;

			for ( int u = Form1.game.grid[ Form1.game.playerList[ giver ].cityList[ city ].X, Form1.game.playerList[ giver ].cityList[ city ].Y ].stack.Length - 1; u >= 0; u-- )
			{
				// to do: move unit or delete them
			}

			Form1.game.frontier.setFrontiers();
			sight.setTerritorySight( receiver );

			//	game.playerList[ owner ].cityList[ game.playerList[ owner ].cityNumber ].laborPos = new Point[ 0 ];
			labor.removeAllLabor( receiver, Form1.game.playerList[ receiver ].cityNumber );
			labor.addAllLabor( receiver, Form1.game.playerList[ receiver ].cityNumber );
			Form1.game.playerList[ receiver ].cityList[ Form1.game.playerList[ receiver ].cityNumber ].setHasDirectAccessToRessource();

			if ( receiver == Form1.game.curPlayerInd )
				uiWrap.choseConstruction( receiver, Form1.game.playerList[ receiver ].cityNumber, true );
			else
				ai.chooseConstruction( receiver, Form1.game.playerList[ receiver ].cityNumber );
		}
		#endregion

#region aquireTechno
	/*	public static void aquireTechno(byte player, byte techno)
		{
			Form1.game.playerList[ player ].technos[ techno ].researched = true;
		//	Form1.game.playerList[ player ].technos[ Form1.game.playerList[ player ].currentResearch ].researched = true;

			if ( Form1.game.playerList[ player ].currentResearch == techno )
			{
				string technoName = Statistics.technologies[ Form1.game.playerList[ player ].currentResearch ].name;
				ai Ai = new ai();
				byte[] technos = Ai.returnDisponibleTechnologies( player );

				if ( player == Form1.game.curPlayerInd )
				{
					if ( technos.Length > 0 )
					{
						byte nextTechno = Ai.randomTechnology( player );
						string[] choices = uiWrap.technoListStrings( player, technos );
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
							Form1.game.playerList[ player ].currentResearch = (byte)nextTechno;

							sciTree sciTree1 = new sciTree();

							sciTree1.ShowDialog();
						}
						else
						{ // accept
							Form1.game.playerList[ player ].currentResearch = (byte)technos[ res ];
						}
					}
					else
					{
						if ( Form1.game.playerList[ player ].currentResearch != 0 )
							System.Windows.Forms.MessageBox.Show( 
							String.Format( language.getAString( language.order.uiYouJustDiscoveredEverything ), Statistics.technologies[ Form1.game.playerList[ player ].currentResearch ].name ), 
							language.getAString( language.order.uiNewTechnology ) 
							);

						Form1.game.playerList[ player ].currentResearch = 0;
						Form1.game.playerList[ player ].technos[ Form1.game.playerList[ player ].currentResearch ].pntDiscovered = 0;
					}
				}
				else
				{
					if ( technos.Length > 0 )
					{
						byte nextTechnos = Ai.randomTechnology( player );

						Form1.game.playerList[ player ].currentResearch = nextTechnos;
					}
					else
					{
						Form1.game.playerList[ player ].currentResearch = 0;
						Form1.game.playerList[ player ].technos[ Form1.game.playerList[ player ].currentResearch ].pntDiscovered = 0;
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

			}*
		}*/
#endregion

#region establishContact
		public static void establishContact(byte player, byte other)
		{
			if ( 
			!Form1.game.playerList[ player ].foreignRelation[ other ].madeContact ||
			!Form1.game.playerList[ other ].foreignRelation[ player ].madeContact 
			)
			{
				Form1.game.playerList[ player ].foreignRelation[ other ].madeContact = true;
				Form1.game.playerList[ other ].foreignRelation[ player ].madeContact = true;

				if ( player == Form1.game.curPlayerInd )
					System.Windows.Forms.MessageBox.Show( 
					"You just made contact with " + Form1.game.playerList[ other ].playerName + " of " + Statistics.civilizations[ Form1.game.playerList[ other ].civType ].name + ".",
					"First contact"
					);
				else if ( other == Form1.game.curPlayerInd )
					System.Windows.Forms.MessageBox.Show( 
					"You just made contact with " + Form1.game.playerList[ player ].playerName + " of " + Statistics.civilizations[ Form1.game.playerList[ player ].civType ].name + ".",
					"First contact"
					);
			}

		}

#endregion

		public enum threats : byte
		{
			toDeclareWar,
			toBreakAlliance,
		}

		/// <summary>
		/// pos in lng file
		/// </summary>
		public static language.order[] threatsName = new language.order[]
		{
			language.order.negoGiveThreatWar, 
			language.order.negoGiveThreatBreakAlliance,
		};

		/*public static void executeExchange( negoList list )
		{
			for ( int i = 0; i < list.Length; i ++ )
				if ( list.list[ i ].accepted )
				{
					if ( list.list[ i ].type == (byte)nego.infoType.breakAllianceWith )
					{
						for ( int p = 0; p < list.players.Length; p ++ )
							if ( 
							Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.list[ i ].info ].politic == (byte)Form1.relationPolType.alliance ||
							Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.list[ i ].info ].politic == (byte)Form1.relationPolType.Protected ||
							Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.list[ i ].info ].politic == (byte)Form1.relationPolType.Protector
								)
							{
								Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.list[ i ].info ].politic = (byte)Form1.relationPolType.peace;
								Form1.game.playerList[ list.list[ i ].info ].foreignRelation[ list.players[ p ] ].politic = (byte)Form1.relationPolType.peace;
							}
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.warOn )
					{
						for ( int p = 0; p < list.players.Length; p++ )
							if ( Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.list[ i ].info ].politic != (byte)Form1.relationPolType.war )
								aiPolitics.declareWar( list.players[ p ], (byte)list.list[ i ].info );
							//	{
								/*
							Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.list[ i ].info ].politic = (byte)Form1.relationPolType.war;
							Form1.game.playerList[ list.list[ i ].info ].foreignRelation[ list.players[ p ] ].politic = (byte)Form1.relationPolType.war;*
					//	}
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.embargoOn )
					{
					/*	for ( int p = 0; p < list.players.Length; p++ )
							Form1.game.playerList[ players[ p ] ].foreignRelation[ players[ ( p + 1 ) % 2 ] ].politic = Form1.relationPolType.war;	*
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.politicTreaty )
					{
						for ( int p = 0; p < list.players.Length; p++ )
							Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.players[ ( p + 1 ) % 2 ] ].politic = (byte)list.list[ i ].info;
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.economicTreaty )
					{
						for ( int p = 0; p < list.players.Length; p++ )
							Form1.game.playerList[ list.players[ p ] ].foreignRelation[ list.players[ ( p + 1 ) % 2 ] ].economic = (byte)list.list[ i ].info;
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.votes )
					{
					/*	for ( int p = 0; p < list.players.Length; p++ )
							Form1.game.playerList[ players[ p ] ].foreignRelation[ players[ ( p + 1 ) % 2 ] ].politic = (byte)list.list[ i ].info;*
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveCity )
					{
						giveCity( list.players[ ( list.list[ i ].player + 1 ) % 2 ], list.players[ list.list[ i ].player ], list.list[ i ].info );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveContactWith )
					{
						establishContact( list.players[ (list.list[ i ].player+1)%2 ], (byte)list.list[ i ].info );
						/*for ( int p = 0; p < list.players.Length; p++ )
						{
							Form1.game.playerList[ players[ p ] ].foreignRelation[ list.list[ i ].info ].madeContact = true;
							Form1.game.playerList[ list.list[ i ].info ].foreignRelation[ players[ p ] ].madeContact = true;
						}*
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveMap )
					{
						if ( list.list[ i ].info == 0 ) // world map
							giveWorldMap( list.players[ list.list[ i ].player ], list.players[ ( list.list[ i ].player + 1 ) % 2 ] );
						else // territory map
							giveTerritoryMap( list.players[ list.list[ i ].player ], list.players[ ( list.list[ i ].player + 1 ) % 2 ] );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveTechno )
					{
						aquireTechno( list.players[ ( list.list[ i ].player + 1 ) % 2 ], (byte)list.list[ i ].info );
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveResource )
					{
					}
					else if ( list.list[ i ].type == (byte)nego.infoType.giveRegion )
					{
						for ( int c = 0; c < list.Length; c ++ )
							if ( 
								c != i &&
								list.list[ c ].type == (byte)nego.infoType.giveCity &&
								list.list[ c ].conflict != null && 
								list.list[ c ].conflict[ 0 ] == i 
								)
								giveCity( list.players[ ( list.list[ i ].player + 1 ) % 2 ], list.players[ list.list[ i ].player ], list.list[ c ].info );
					}
				}
		}*/
	}
}
