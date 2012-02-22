using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for negoList.
	/// </summary>
	public class negoList
	{
		public List[] list;
		public byte[] players;
		int[] wiw;
		public long[] canGiveMoney, canGivePerTurn, giveMoney, moneyPerTurn;

		public long getValuePerTurn( long amountPerTurn )
		{
			long tot = 0;
			for ( long t = 0; t < nego.turnsForExchanges; t++ )
				tot += (long)( amountPerTurn * ( 100 - 15 * Math.Pow( t, 0.4 ) ) / 100 );
			return tot;
		}

		public int Length
		{
			get
			{
				return list.Length;
			}
		}

		public struct List
		{
			public bool accepted;
			public byte type;
			public int info;
			public int[] conflict;
			public byte player;
			public bool cantBeRemoved;
		}

		private int pos;

		private void addInfoToList(byte type, int info, int player)
		{
			if ( pos >= list.Length )
			{
				List[] list1 = list;

				list = new List[ list1.Length + 50 ];
				for ( int i = 0; i < list1.Length; i++ )
					list[ i ] = list1[ i ];
			}

			list[ pos ].type = type;
			list[ pos ].info = info;
			list[ pos ].player = (byte)player;
			pos++;
		}

#region negoList - init
		public negoList( byte[] Players )
		{
			players = Players;

			wiw = new int[ 2 ];
			for ( int p = 0; p < 2; p ++ )
				wiw[ p ] = ai.whoIsWinning( players[ p ], players[ ( p + 1 ) % 2 ] );

			list = new List[ 100 ]; 
			pos = 0; 

	#region bi

			#region Politic treaty

			int pb = 2;
			bool piw = false;

			if ( wiw[ 0 ] > 9 )
				piw = true;

			if ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.peace )
			{
				addInfoToList( (byte)nego.infoType.politicTreaty, (byte)Form1.relationPolType.alliance, pb );
				if ( !piw )
					addInfoToList( (byte)nego.infoType.politicTreaty, (byte)Form1.relationPolType.Protected, pb );
				else
					addInfoToList( (byte)nego.infoType.politicTreaty, (byte)Form1.relationPolType.Protector, pb );
			}
			else if ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.ceaseFire )
			{
				addInfoToList( (byte)nego.infoType.politicTreaty, (byte)Form1.relationPolType.peace, pb );
			}
			else if ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.war )
			{  // cease fire est nécessaire pour négocier des ententes d alliances
				addInfoToList( (byte)nego.infoType.politicTreaty, (byte)Form1.relationPolType.ceaseFire, pb );
				list[ pos - 1 ].cantBeRemoved = true;
				list[ pos - 1 ].accepted = true;
			}
			else if ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.alliance )
			{
			}
			else if ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.Protector )
			{
			}
			else if ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ players[ 1 ] ].politic == (byte)Form1.relationPolType.Protected )
			{
			}

		#endregion

			// break alliance
			for ( int p = 0; p < Form1.game.playerList.Length; p++ )
				if ( 
					p != players[ 0 ] &&
					p != players[ 1 ] &&
					!Form1.game.playerList[ p ].dead &&
					Form1.game.playerList[ players[ 0 ] ].foreignRelation[ p ].madeContact && 
						Form1.game.playerList[ players[ 1 ] ].foreignRelation[ p ].madeContact && 
						( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ p ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ 1 ] ].foreignRelation[ p ].politic == (byte)Form1.relationPolType.alliance ) )
					addInfoToList( (byte)nego.infoType.breakAllianceWith, p, pb );

			// war on
			for ( int p = 0; p < Form1.game.playerList.Length; p++ )
				if ( 
					p != players[ 0 ] &&
					p != players[ 1 ] &&
					!Form1.game.playerList[ p ].dead &&
					Form1.game.playerList[ players[ 0 ] ].foreignRelation[ p ].madeContact && Form1.game.playerList[ players[ 1 ] ].foreignRelation[ p ].madeContact && ( Form1.game.playerList[ players[ 0 ] ].foreignRelation[ p ].politic != (byte)Form1.relationPolType.war || Form1.game.playerList[ players[ 1 ] ].foreignRelation[ p ].politic != (byte)Form1.relationPolType.war ) )
					addInfoToList( (byte)nego.infoType.warOn, p, pb );

			// embargo on

			// vote

#endregion

	#region uni

			for ( pb = 0; pb < 2; pb++ )
			{
				int player = players[ pb ];
				bool[] region = new bool[ Form1.game.playerList.Length ];

				// cities
				for ( int c = 1; c <= Form1.game.playerList[ players[ pb ] ].cityNumber; c++ )
					if ( Form1.game.playerList[ players[ pb ] ].cityList[ c ].state != (byte)enums.cityState.dead && !Form1.game.playerList[ players[ pb ] ].cityList[ c ].isCapitale )
					{
						addInfoToList( (byte)nego.infoType.giveCity, c, pb );

						if ( Form1.game.playerList[ players[ pb ] ].cityList[ c ].originalOwner != player )
							region[ Form1.game.playerList[ players[ pb ] ].cityList[ c ].originalOwner ] = true;
					}

				// regions
				for ( int r = 0; r < Form1.game.playerList.Length; r++ )
					if ( region[ r ] )
						addInfoToList( (byte)nego.infoType.giveRegion, r, pb );

				// technos
				for ( int t = 0; t < Form1.game.playerList[ players[ pb ] ].technos.Length; t++ )
					if ( Form1.game.playerList[ players[ pb ] ].technos[ t ].researched && !Form1.game.playerList[ players[ (pb+1)%2 ] ].technos[ t ].researched )
						addInfoToList( (byte)nego.infoType.giveTechno, t, pb );

				// contact with
				for ( int p = 0; p < Form1.game.playerList.Length; p++ )
					if ( 
						p != players[ 0 ] &&
						p != players[ 1 ] &&
						!Form1.game.playerList[ p ].dead &&
						Form1.game.playerList[ players[ pb ] ].foreignRelation[ p ].madeContact && 
						!Form1.game.playerList[ players[ (pb+1)%2 ] ].foreignRelation[ p ].madeContact 
						)
						addInfoToList( (byte)nego.infoType.giveContactWith, p, pb );

				// maps
				if ( Form1.game.playerList[ players[ pb ] ].technos[ (byte)Form1.technoList.mapMaking ].researched )
				{
					addInfoToList( (byte)nego.infoType.giveMap, 0, pb );
					addInfoToList( (byte)nego.infoType.giveMap, 1, pb );
				}

				// resources
				for ( int r = 0; r < Statistics.resources.Length; r ++ )
					if ( 
						Form1.game.playerList[ players[ pb ] ].hasAccessToResources( r ) && !Form1.game.playerList[ players[ ( pb + 1 ) % 2 ] ].hasAccessToResources( r ) && 
						Form1.game.playerList[ players[ pb ] ].technos[ Statistics.resources[ r ].techno ].researched && Form1.game.playerList[ players[ ( pb + 1 ) % 2 ] ].technos[ Statistics.resources[ r ].techno ].researched
						)
						addInfoToList( (byte)nego.infoType.giveResource, r, pb ); 
/*
				// money
				if ( Form1.game.playerList[ players[ 0 ] ].money > 0 )
					addInfoToList( (byte)nego.infoType.giveMoney, (int)Form1.game.playerList[ players[ 0 ] ].money, pb );

				// money per turn
				if ( Form1.game.playerList[ players[ 0 ] ].totalTrade * Form1.game.playerList[ players[ 0 ] ].preferences.reserve / 100 > 0 )
					addInfoToList( (byte)nego.infoType.giveMoneyPerTurn, Form1.game.playerList[ players[ 0 ] ].totalTrade * Form1.game.playerList[ players[ 0 ] ].preferences.reserve / 100, pb );

				// % per turn
				if ( Form1.game.playerList[ players[ 0 ] ].preferences.reserve > 0 )
					addInfoToList( (byte)nego.infoType.givePercBrut, Form1.game.playerList[ players[ 0 ] ].preferences.reserve, pb );
*/
				// threats
			//	addInfoToList( (byte)nego.infoType.threat, Form1.game.playerList[ players[ pb ] ].preferences.reserve, pb );
			}

	#endregion

			List[] list1 = list;
			list = new List[ pos ];

			for ( int i = 0; i < list.Length; i ++ )
				list[ i ] = list1[ i ];

	#region set conflicts
			for ( int i = 0; i < list.Length; i ++ )
			{
				if ( list[ i ].type == (byte)nego.infoType.giveMap )
				{
					list[ i ].conflict = new int[ 1 ];
					for ( int j = 0; j < list.Length; j ++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.giveMap && list[ i ].player == list[ j ].player )
							list[ i ].conflict[ 0 ] = j;
				}
				else if ( list[ i ].type == (byte)nego.infoType.politicTreaty )
				{
					int tot = 0;

					for ( int j = 0; j < list.Length; j ++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.politicTreaty )
							tot++;

					list[ i ].conflict = new int[ tot ];
					for ( int j = 0, k = 0; j < list.Length; j ++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.politicTreaty )
							list[ i ].conflict[ k ] = j;
				}
				else if ( list[ i ].type == (byte)nego.infoType.economicTreaty )
				{
					int tot = 0;

					for ( int j = 0; j < list.Length; j++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.economicTreaty )
							tot++;

					list[ i ].conflict = new int[ tot ];
					for ( int j = 0, k = 0; j < list.Length; j++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.economicTreaty )
						{
							list[ i ].conflict[ k ] = j;
							k++;
						}
				}
				else if ( list[ i ].type == (byte)nego.infoType.giveRegion )
				{
					int tot = 0;
					for ( int j = 0; j < list.Length; j++ )
						if (
							i != j && 
							list[ j ].type == (byte)nego.infoType.giveCity && 
							list[ i ].player == list[ j ].player &&
							Form1.game.playerList[ players[ list[ i ].player ] ].cityList[ list[ j ].info ].originalOwner == list[ i ].info
							)
							tot++;

					list[ i ].conflict = new int[ tot ];
					for ( int j = 0, k = 0; j < list.Length; j++ )
						if (
							i != j && 
							list[ j ].type == (byte)nego.infoType.giveCity && 
							list[ i ].player == list[ j ].player &&
							Form1.game.playerList[ players[ list[ i ].player ] ].cityList[ list[ j ].info ].originalOwner == list[ i ].info
							)
						{
							list[ i ].conflict[ k ] = j;
							k++;

							list[ j ].conflict = new int[ 1 ];
							list[ j ].conflict[ 0 ] = i;
						}
				}
				else if ( 
					list[ i ].type == (byte)nego.infoType.warOn || 
					list[ i ].type == (byte)nego.infoType.breakAllianceWith || 
					list[ i ].type == (byte)nego.infoType.embargoOn 
					)
				{
					int tot = 0;

					for ( int j = 0; j < list.Length; j++ )
						if ( 
							i != j &&
							(
								list[ j ].type == (byte)nego.infoType.warOn || 
								list[ j ].type == (byte)nego.infoType.breakAllianceWith || 
								list[ j ].type == (byte)nego.infoType.embargoOn 
							) &&
							list[ j ].info == list[ i ].info
							)
							tot++;

					list[ i ].conflict = new int[ tot ];
					for ( int j = 0, k = 0; j < list.Length; j++ )
						if ( 
							i != j &&
							(
								list[ j ].type == (byte)nego.infoType.warOn || 
								list[ j ].type == (byte)nego.infoType.breakAllianceWith || 
								list[ j ].type == (byte)nego.infoType.embargoOn 
							) &&
							list[ j ].info == list[ i ].info
							)
							list[ i ].conflict[ k ] = j;
				}
				else if ( list[ i ].type == (byte)nego.infoType.threat )
				{
					int tot = 0;
					for ( int j = 0; j < list.Length; j++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.threat && list[ i ].player == list[ j ].player )
							tot++;

					list[ i ].conflict = new int[ tot ];
					for ( int j = 0, k = 0; j < list.Length; j++ )
						if ( i != j && list[ j ].type == (byte)nego.infoType.threat && list[ i ].player == list[ j ].player )
						{
							list[ i ].conflict[ k ] = j;
							k++;
						}
				}
			}
	#endregion

			canGiveMoney = new long[ 2 ];
			canGivePerTurn = new long[ 2 ]; 
			giveMoney = new long[ 2 ];
			moneyPerTurn = new long[ 2 ];

			for ( int p = 0; p < 2; p++ )
			{
				canGiveMoney[ p ] = (int)Form1.game.playerList[ players[ p ] ].money;
				canGivePerTurn[ p ] = Form1.game.playerList[ players[ p ] ].totalTrade * Form1.game.playerList[ players[ p ] ].preferences.reserve / 100;
			}
		}
#endregion

		public void add(int ind)
		{
			list[ ind ].accepted = true;

			if ( list[ ind ].conflict != null )
				for ( int i = 0 ; i < list[ ind ].conflict.Length; i ++ )
					list[ list[ ind ].conflict[ i ] ].accepted = false;
		}

		public void remove(int ind)
		{
			list[ ind ].accepted = false;
		}


		public long[] getTotalGain()
		{
			long[] gain = new long[ 2 ];

			for ( int p = 0; p < 2; p++ )
			{
				gain[ p ] += giveMoney[ ( p + 1 ) % 2 ];
				gain[ p ] -= giveMoney[ p ];
				gain[ p ] += getValuePerTurn( moneyPerTurn[ ( p + 1 ) % 2 ] );
				gain[ p ] -= getValuePerTurn( moneyPerTurn[ p ] );
			}

			for ( int i = 0 ; i < list.Length ;  i ++ )
				if ( list[ i ].accepted )
				{
					long[] tempGain = getGainOne( i );

					for ( int p = 0; p < gain.Length; p ++ )
						gain[ p ] += tempGain[ p ];
				}

			return gain;
		}

	#region get gain
		public long[] getPossibleGain(int ind)
		{
			long[] gain = getGainOne( ind );

			if ( list[ ind ].conflict != null )
				for ( int i = 0; i < list[ ind ].conflict.Length; i++ )
					if ( list[ i ].accepted )
					{
						long[] loss = getGainOne( i );
						for ( int j = 0; j < gain.Length; j ++ )
							gain[ j ] -= loss[ j ];
					}

			return gain;
		}

		private long[] getGainOne(int ind)
		{
			List ib = list[ ind ];

			long[] gain = new long[ 2 ];
			int basicCityValue = 100,
			cityValuePerPop = 50,
			cityValuePerBuilding = 20, 
			contact = 30, 
			techno = 2, 
			mapPerCase = 5;

			if ( ib.player == 2 )
			{
				for ( int p = 0; p < 2; p++ )
				{
					if ( ib.type == (byte)nego.infoType.breakAllianceWith )
					{
						if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protector )
							gain[ p ] -= 150; // difficulty
							else if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.war )
							gain[ p ] += 200;
						else
							gain[ p ] -= 50; // difficulty
					}
					else if ( ib.type == (byte)nego.infoType.warOn )
					{
						if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protector )
							gain[ p ] -= 350; // difficulty
							else if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.war )
							gain[ p ] += 300;
						else
							gain[ p ] -= 200; // difficulty
					}
					else if ( ib.type == (byte)nego.infoType.embargoOn )
					{
						//	int wiw = ai.whoIsWinning( players[ p ], players[ ( p + 1 ) % 2 ] ) * -1 - 10;
						if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protector )
							gain[ p ] -= 350; // difficulty
							else if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.war )
							gain[ p ] += 300;
						else
							gain[ p ] -= 200; // difficulty
					}
					else if ( ib.type == (byte)nego.infoType.politicTreaty )
					{
						if ( ib.info == (byte)Form1.relationPolType.ceaseFire )
							gain[ p ] += wiw[ p ] * -40;
						else if ( ib.info == (byte)Form1.relationPolType.peace )
							gain[ p ] += wiw[ p ] * -10;
						else if ( ib.info == (byte)Form1.relationPolType.alliance || ib.info == (byte)Form1.relationPolType.Protected || ib.info == (byte)Form1.relationPolType.Protector )
							gain[ p ] += wiw[ p ] * -10;
					}
					else if ( ib.type == (byte)nego.infoType.economicTreaty )
					{
						gain[ p ] += 10;
					}
					else if ( ib.type == (byte)nego.infoType.votes )
					{
						gain[ p ] += 10;
					}
				}
			}
			else
			{
				int giver = ib.player, receiver = (ib.player+1)%2;
			//	int p = ib.player; // giver
				
				if ( ib.type == (byte)nego.infoType.giveCity )
				{
					int gainForReceiver = // basicCityValue + 
					cityValuePerPop * Form1.game.playerList[ players[ giver ] ].cityList[ ib.info ].population + 
					cityValuePerBuilding * Form1.game.playerList[ players[ giver ] ].cityList[ ib.info ].buildingCount;
					int lossForGiver = basicCityValue + 
					cityValuePerPop * Form1.game.playerList[ players[ giver ] ].cityList[ ib.info ].population + 
					cityValuePerBuilding * Form1.game.playerList[ players[ giver ] ].cityList[ ib.info ].buildingCount;

					if ( Form1.game.playerList[ players[ giver ] ].cityList[ ib.info ].originalOwner == players[ giver ] )
						lossForGiver *= 3;
					else if ( Form1.game.playerList[ players[ giver ] ].cityList[ ib.info ].originalOwner == players[ receiver ] )
						gainForReceiver *= 3;

					gain[ receiver ] += gainForReceiver;
					gain[ giver ] -= lossForGiver;
				}
				else if ( ib.type == (byte)nego.infoType.giveContactWith )
				{
					gain[ receiver ] += contact;
					gain[ giver ] -= contact / 3;
				}
				else if ( ib.type == (byte)nego.infoType.giveMap )
				{
					if ( ib.info == 0 )
					{// world map
						int tot = 0;

						for ( int x = 0; x < Form1.game.width; x++ )
							for ( int y = 0; y < Form1.game.height; y++ )
								if ( !Form1.game.playerList[ players[ receiver ] ].discovered[ x, y ] && Form1.game.playerList[ players[ giver ] ].discovered[ x, y ] )
									tot++;

						gain[ receiver ] += tot * mapPerCase;
						gain[ giver ] -= tot * mapPerCase / 3;
					}
					else
					{// territory map
						int tot = 0;

						for ( int x = 0; x < Form1.game.width; x++ )
							for ( int y = 0; y < Form1.game.height; y++ )
								if ( !Form1.game.playerList[ players[ receiver ] ].discovered[ x, y ] && Form1.game.grid[ x, y ].territory - 1 == players[ giver ] )
									tot++;

						gain[ receiver ] += tot * mapPerCase;
						gain[ giver ] -= tot * mapPerCase / 3;
					}
				}
				else if ( ib.type == (byte)nego.infoType.giveTechno )
				{
					gain[ receiver ] += Statistics.technologies[ ib.info ].cost * techno;
					gain[ giver ] -= Statistics.technologies[ ib.info ].cost * techno / 3;
				}
				else if ( ib.type == (byte)nego.infoType.threat )
				{
					if ( ib.info == 0 )
						gain[ receiver ] += wiw[ giver ] * 50; // war
					else if ( ib.info == 1 )
						gain[ receiver ] += wiw[ giver ] * 10; // var with exchanges // embargo
						else
						gain[ receiver ] += wiw[ giver ] * 20; // alliance
				}
				else if ( ib.type == (byte)nego.infoType.giveRegion )
				{
					for ( int c = 1; c <= Form1.game.playerList[ players[ giver ] ].cityNumber; c++ )
						if ( 
							Form1.game.playerList[ players[ giver ] ].cityList[ c ].state != (byte)enums.cityState.dead && 
							Form1.game.playerList[ players[ giver ] ].cityList[ c ].originalOwner == ib.info 
							)
						{
							int gainForReceiver = // basicCityValue + 
								cityValuePerPop * Form1.game.playerList[ players[ giver ] ].cityList[ c ].population + 
								cityValuePerBuilding * Form1.game.playerList[ players[ giver ] ].cityList[ c ].buildingCount;
							int lossForGiver = basicCityValue + 
								cityValuePerPop * Form1.game.playerList[ players[ giver ] ].cityList[ c ].population + 
								cityValuePerBuilding * Form1.game.playerList[ players[ giver ] ].cityList[ c ].buildingCount;

							if ( Form1.game.playerList[ players[ giver ] ].cityList[ c ].originalOwner == players[ giver ] )
								lossForGiver *= 3;
							else if ( Form1.game.playerList[ players[ giver ] ].cityList[ c ].originalOwner == players[ receiver ] )
								gainForReceiver *= 3;

							gain[ receiver ] += gainForReceiver;
							gain[ giver ] -= lossForGiver;
						}
				}
			}

			return gain;
		}
#endregion

		public bool conflicted(int ind)
		{
			if ( list[ ind ].conflict != null )
				for ( int i = 0; i < list[ ind ].conflict.Length; i ++ )
					if ( list[ list[ ind ].conflict[ i ] ].accepted )
						return true;

			return false;
		}

#region addTo moneys
		public void addToGiveMoney( int player, long inc )
		{
			while ( giveMoney[ (player+1)%2 ] > 0 && inc > 0 )
			{
				giveMoney[ ( player + 1 ) % 2 ]--;
				inc--;
			}

			while ( giveMoney[ player ] < canGiveMoney[ player ] && inc > 0 )
			{
				giveMoney[ player ]++;
				inc--;
			}
		}
		public void addToMoneyPerTurn(int player, long inc)
		{
			while ( moneyPerTurn[ ( player + 1 ) % 2 ] > 0 && inc > 0 )
			{
				moneyPerTurn[ ( player + 1 ) % 2 ]--;
				inc--;
			}

			while ( moneyPerTurn[ player ] < canGivePerTurn[ player ] && inc > 0 )
			{
				moneyPerTurn[ player ]++;
				inc--;
			}
		}
#endregion

#region executeExchange
		public void executeExchange()
		{
			for ( int i = 0; i < list.Length; i++ )
				if ( list[ i ].accepted )
				{
					if ( list[ i ].type == (byte)nego.infoType.breakAllianceWith )
					{
						for ( int p = 0; p < players.Length; p++ )
							if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic == (byte)Form1.relationPolType.Protector )
							{
								Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic = (byte)Form1.relationPolType.peace;
								Form1.game.playerList[ list[ i ].info ].foreignRelation[ players[ p ] ].politic = (byte)Form1.relationPolType.peace;
							}
					}
					else if ( list[ i ].type == (byte)nego.infoType.warOn )
					{
						for ( int p = 0; p < players.Length; p++ )
							if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic != (byte)Form1.relationPolType.war )
								aiPolitics.declareWar( players[ p ], (byte)list[ i ].info );
					/*	{
							Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic = (byte)Form1.relationPolType.war;
							Form1.game.playerList[ list[ i ].info ].foreignRelation[ players[ p ] ].politic = (byte)Form1.relationPolType.war;
						}	*/
					}
					else if ( list[ i ].type == (byte)nego.infoType.embargoOn )
					{
						/*	for ( int p = 0; p < players.Length; p++ )
							Form1.game.playerList[ players[ p ] ].foreignRelation[ players[ ( p + 1 ) % 2 ] ].politic = Form1.relationPolType.war;	*/
					}
					else if ( list[ i ].type == (byte)nego.infoType.politicTreaty )
					{
						for ( int p = 0; p < players.Length; p++ )
							Form1.game.playerList[ players[ p ] ].foreignRelation[ players[ ( p + 1 ) % 2 ] ].politic = (byte)list[ i ].info;
					}
					else if ( list[ i ].type == (byte)nego.infoType.economicTreaty )
					{
						for ( int p = 0; p < players.Length; p++ )
							Form1.game.playerList[ players[ p ] ].foreignRelation[ players[ ( p + 1 ) % 2 ] ].economic = (byte)list[ i ].info;
					}
					else if ( list[ i ].type == (byte)nego.infoType.votes )
					{
						/*	for ( int p = 0; p < players.Length; p++ )
							Form1.game.playerList[ players[ p ] ].foreignRelation[ players[ ( p + 1 ) % 2 ] ].politic = (byte)list[ i ].info;*/
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveCity )
					{
						nego.giveCity( players[ ( list[ i ].player + 1 ) % 2 ], players[ list[ i ].player ], list[ i ].info );
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveContactWith )
					{
						nego.establishContact( players[ ( list[ i ].player + 1 ) % 2 ], (byte)list[ i ].info );
						/*for ( int p = 0; p < players.Length; p++ )
						{
							Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].madeContact = true;
							Form1.game.playerList[ list[ i ].info ].foreignRelation[ players[ p ] ].madeContact = true;
						}*/
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveMap )
					{
						if ( list[ i ].info == 0 ) // world map
							nego.giveWorldMap( players[ list[ i ].player ], players[ ( list[ i ].player + 1 ) % 2 ] );
						else // territory map
							nego.giveTerritoryMap( players[ list[ i ].player ], players[ ( list[ i ].player + 1 ) % 2 ] );
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveTechno )
					{
						Form1.game.playerList[ players[ ( list[ i ].player + 1 ) % 2 ] ].aquireTechno( (byte)list[ i ].info );
					//	nego.aquireTechno( players[ ( list[ i ].player + 1 ) % 2 ], (byte)list[ i ].info );
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveResource )
					{
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveRegion )
					{
						for ( int c = 0; c < list.Length; c++ )
							if ( c != i && list[ c ].type == (byte)nego.infoType.giveCity && list[ c ].conflict != null && list[ c ].conflict[ 0 ] == i )
								nego.giveCity( players[ ( list[ i ].player + 1 ) % 2 ], players[ list[ i ].player ], list[ c ].info );
					}
				}
			
			for ( int p = 0; p < 2; p ++ )
			{
				Form1.game.playerList[ players[ p ] ].money += this.giveMoney[ (p+1)%2 ];
				Form1.game.playerList[ players[ p ] ].money -= this.giveMoney[ p ];

				Form1.game.playerList[ players[ p ] ].setResourcesAccess();
				Form1.game.playerList[ players[ p ] ].memory.addExchange( this );

				if ( this.moneyPerTurn[ (p+1)%2 ] > 0 )
					Form1.game.playerList[ players[ p ] ].invalidateTrade();
			}

		}
#endregion

#region whatHeWants
		public int[] whatHeWants( int p )
		{
			int[] values = new int[ list.Length ];

			for ( int i = 0; i < list.Length; i++ )
			{
				if ( list[ i ].type == (byte)nego.infoType.breakAllianceWith )
				{
					if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic == (byte)Form1.relationPolType.war )
						values[ i ] = ( 20 - wiw[ p ] ) * 5;
				}
				else if ( list[ i ].type == (byte)nego.infoType.economicTreaty )
				{
				}
				else if ( list[ i ].type == (byte)nego.infoType.embargoOn )
				{
				}
				else if ( list[ i ].type == (byte)nego.infoType.politicTreaty )
				{
				}
				else if ( list[ i ].type == (byte)nego.infoType.votes )
				{
				}
				else if ( list[ i ].type == (byte)nego.infoType.warOn )
				{
					if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ list[ i ].info ].politic == (byte)Form1.relationPolType.war )
						values[ i ] = (20-wiw[ p ]) * 3;
				}
				else if ( list[ i ].player == (p+1)%2 ) // autre joueur
				{
					if ( list[ i ].type == (byte)nego.infoType.giveCity )
					{
						if ( Form1.game.playerList[ players[ (p+1)%2 ] ].cityList[ list[ i ].info ].originalOwner == players[ p ] )
						{
							System.Drawing.Point[] sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ players[ (p+1)%2 ] ].cityList[ list[ i ].info ].pos, 3 );

							for ( int k = 0; k < sqr.Length; k ++ )
								if ( Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory == players[ p ] )
								{
									values[ i ] = (20-wiw[ p ]) * 3;
									break;
								}
						}
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveResource )
					{
						if ( Form1.game.playerList[ players[ p ] ].hasAccessToResources( list[ i ].info ) )
							values[ i ] = ( 20 - wiw[ p ] ) * 3;
					}
					else if ( list[ i ].type == (byte)nego.infoType.giveTechno )
					{
						if ( Form1.game.playerList[ players[ p ] ].isInWar ) 
							values[ i ] = Statistics.technologies[ list[ i ].info ].militaryValue * 5; 
						else
							values[ i ] = ( 20 - Statistics.technologies[ list[ i ].info ].militaryValue ) * 5; 
					}
				}
			}

			return values;
		}
#endregion

		public negoList getClone()
		{ 
			/*= (negoList)this.MemberwiseClone();
			list.*/

			negoList list = new negoList( players );

			list.list = new List[ this.list.Length ]; //  this.list;
			this.list.CopyTo( list.list, 0 );

			list.wiw = new int[ this.wiw.Length ];
			this.wiw.CopyTo( list.wiw, 0 );

			list.canGiveMoney = new long[ this.canGiveMoney.Length ];//this.canGiveMoney;
			this.canGiveMoney.CopyTo( list.canGiveMoney, 0 );

			list.canGivePerTurn = new long[ this.canGivePerTurn.Length ];//this.canGivePerTurn;
			this.canGivePerTurn.CopyTo( list.canGivePerTurn, 0 );

			list.giveMoney = new long[ this.giveMoney.Length ];//this.giveMoney;
			this.giveMoney.CopyTo( list.giveMoney, 0 );

			list.moneyPerTurn = new long[ this.moneyPerTurn.Length ];//this.moneyPerTurn;
			this.moneyPerTurn.CopyTo( list.moneyPerTurn, 0 );

			return list;
		}
	}
}
