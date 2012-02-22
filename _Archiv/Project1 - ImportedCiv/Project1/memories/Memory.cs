using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for playerMemory.
	/// </summary>
	public class Memory
	{
	//	byte player;
		PlayerList player;

		public Memory( PlayerList olayer, int length ) 
		{ 
			this.player = olayer;
			list = new structures.memory[ length ]; 
		} 

		public structures.memory[] list; 

		public int Lenght
		{
			get
			{
				return list.Length;
			}
		}

#region adds
		public void add( byte type, int[] ind )
		{
			add( type, ind, player.game.curTurn );
		}
		public void add(byte type, int ind1 )
		{
			add( type, new int[] { ind1 }, player.game.curTurn );
		}
		public void add( byte type, int ind1, int ind2 )
		{
			add( type, new int[] { ind1, ind2 }, player.game.curTurn );
		}
		public void add( byte type, int ind1, int ind2, int turn )
		{
			add( type, new int[] { ind1, ind2 }, turn );
		}
		public void add( byte type, int[] ind, int turn ) 
		{ 
			structures.memory[] buffer = list; 
			list = new structures.memory[ buffer.Length + 1 ]; 

			for ( int i = 0; i < buffer.Length; i ++ ) 
				list[ i ] = buffer[ i ]; 

			list[ list.Length - 1 ].type = type; 
			list[ list.Length - 1 ].ind = ind;
			list[ list.Length - 1 ].turn = turn; 
		} 
		#endregion 

#region finds
		public structures.memory findLatest(byte type)
		{
			for ( int i = list.Length - 1; i >= 0; i-- )
				if ( list[ i ].type == type )
					return list[ i ];

			structures.memory retour = new xycv_ppc.structures.memory();

			retour.ind = new int[ 0 ];
			return retour;
		} 
		
		public structures.memory[] findAllSince(byte type, int turns)
		{
			structures.memory[] retour = new xycv_ppc.structures.memory[ 0 ];

			for ( int i = list.Length - 1; i >= 0; i-- )
				if ( list[ i ].turn < player.game.curTurn - turns )
					break;
				else if ( list[ i ].type == type )
				{
					structures.memory[] buffer = retour;

					retour = new xycv_ppc.structures.memory[ buffer.Length + 1 ]; 

					for ( int j = 0; j < buffer.Length; j++ )
						retour[ j ] = buffer[ j ];

					retour[ buffer.Length ] = list[ i ];
				}

			return retour;
		} 

		public structures.memory findInTheLast(byte type, int turn)
		{
			for ( int i = list.Length - 1; i >= 0; i-- )
				if ( list[ i ].type == type )
					return list[ i ];
				else if ( list[ i ].turn < player.game.curTurn - turn )
					break;

			structures.memory retour = new xycv_ppc.structures.memory();

			retour.ind = new int[ 0 ];
			return retour;
		} 

		public structures.memory[] findAll(byte type)
		{
			structures.memory[] retour = new xycv_ppc.structures.memory[ list.Length ];
			int pos = 0;

			for ( int i = 0; i < list.Length; i++ )
				if ( list[ i ].type == type )
				{
					retour[ pos ] = list[ i ];
					pos++;
				}

			structures.memory[] retour1 = new xycv_ppc.structures.memory[ pos ];

			for ( int i = 0; i < retour1.Length; i++ )
				retour1[ i ] = retour[ i ];

			return retour1;
		}

		public structures.memory findAndDeleteLatest(byte type)
		{
			int foundAt = -1;

			for ( int i = list.Length - 1; i >= 0; i-- )
				if ( list[ i ].type == type )
				{
					foundAt = i;
					break;
				}

			structures.memory retour = new xycv_ppc.structures.memory();

			if ( foundAt != -1 )
			{
				structures.memory[] buffer = new structures.memory[ list.Length ];

				list = new structures.memory[ buffer.Length - 1 ];
				for ( int i = foundAt; i < list.Length; i++ )
					list[ i ] = buffer[ i + 1 ];

				for ( int i = 0; i < foundAt; i++ )
					list[ i ] = buffer[ i ];
			}
			else
			{
				retour = new xycv_ppc.structures.memory();
				retour.ind = new int[ 0 ];
			}

			return retour;
		} 

#endregion

		public structures.memory[] findAllSinceAndDeletePrevious(byte type, int turns )
		{
			structures.memory[] retour = new xycv_ppc.structures.memory[ 0 ];
			bool[] toBeRemoved = new bool[ list.Length ];
			int totToBeRemoved = 0;

			for ( int i = list.Length - 1; i >= 0; i-- )
				if ( list[ i ].turn < player.game.curTurn - turns )
				{
					if ( list[ i ].type == type )
					{
						totToBeRemoved ++;
						toBeRemoved[ i ] = true;
					}
				}
				else if ( list[ i ].type == type )
				{
					structures.memory[] buffer = retour;

					retour = new xycv_ppc.structures.memory[ buffer.Length + 1 ];
					for ( int j = 0; j < buffer.Length; j++ )
						retour[ j ] = buffer[ j ];

					retour[ buffer.Length ] = list[ i ];
				}

			if ( totToBeRemoved > 0 )
			{
				structures.memory[] buffer = list;
				list = new structures.memory[ buffer.Length - totToBeRemoved ];

				for ( int i = 0, j = 0; i < buffer.Length; i++ )
					if ( !toBeRemoved[ i ] )
					{
						list[ j ] = buffer[ i ];
						j++;
					}
			}

			return retour;
		} 

#region delete
		public void deleteTypeBefore(byte type, int turn)
		{
			bool[] toErease = new bool[ list.Length ];
			int tot = 0;
			for ( int i = 0; i < list.Length; i++ )
				if ( list[ i ].turn < turn )
				{
					if ( list[ i ].type == type )
					{
						toErease[ i ] = true;
						tot++;
					}
				}
				else
				{
					break;
				}

			structures.memory[] buffer = list;
			list = new structures.memory[ buffer.Length - tot ];
			for ( int i = 0, j = 0; i < buffer.Length; i ++ )
				if ( !toErease[ i ] )
				{
					list[ j ] = buffer[ i ];
					j ++;
				}
		} 
		#endregion

#region exchanges

/************************************
*
*	Trade format					
*	 type =		exchange			
*	 turn =		player.game.curTurn		
*	 ind[ 0 ] =	other 		
*	 ind[ 1 ] =	giver			0	
*	 ind[ 2 ] =	exchangeType	0	
*	 ind[ 3 ] =	exchangeInfo	0	
*	 ind[ 4 ] =	giver			1	
*	 ind[ 5 ] =	exchangeType	1	
*	 ind[ 6 ] =	exchangeInfo	1	
*
*	exchangeType:
*	 money per turn
*	 politic
*	 economic
*	 resources
*
************************************/

		public void addAttemptedNego(byte other)
		{
			add( (byte)enums.playerMemory.attemptedNego, other );
		}

		public bool attemptedNegoRecently(byte other)
		{
			int turns = 5;
			structures.memory[] mem = findAllSince( (byte)enums.playerMemory.attemptedNego, turns );

			for ( int i = 0; i < mem.Length; i ++ )
				if ( mem[ i ].ind[ 0 ] == other )
					return true;

			return false;
		}

	#region get infos
		/// <summary></summary>
		/// <returns>
		///int[ players, g + r1 + r2 ...], neg num means player give</returns>
		public int[,] getCurrentExchanges()
		{
			structures.memory[] mems = findAll( (byte)enums.playerMemory.exchangeWith );

			if ( mems.Length > 0 )
			{
				int[,] exchanges = new int[ player.game.playerList.Length, Statistics.resources.Length + 1 ];

				for ( int i = 0; i < mems.Length; i++ )
					if ( mems[ i ].turn + nego.turnsForExchanges >= player.game.curTurn )
					{
						byte other = (byte)mems[ i ].ind[ 0 ];

						for ( int k = 1; k < mems[ i ].ind.Length; )
						{
							int localType = mems[ i ].ind[ k ];
							k++;
							switch ( localType )
							{
								case (int)nego.infoType.giveResource:

									int localPlayer = mems[ i ].ind[ k ];
									k++;
									int localInfo = mems[ i ].ind[ k ];
									k++;

									if ( localPlayer == player.player )
									{
										exchanges[ other, localInfo + 1 ]--;
									}
									else
									{
										exchanges[ other, localInfo + 1 ]++;
									}

									break;

								case (int)nego.infoType.giveMoneyPerTurn :

									int localPlayer1 = mems[ i ].ind[ k ];
									k++;
									int localInfo1 = mems[ i ].ind[ k ];
									k++;

									if ( localPlayer1 == player.player )
									{
										exchanges[ other, 0 ] -= localInfo1;
									}
									else
									{
										exchanges[ other, 0 ] += localInfo1;
									}

									break;
							}
						}
					}

				return exchanges;
			}
			else
				return new int[ player.game.playerList.Length, Statistics.resources.Length + 1 ];
		}
#endregion
		
		public void cancelExchangesWith(byte other)
		{
			bool[] toErease = new bool[ list.Length ];
			int tot = 0;

			for ( int i = 0; i < list.Length; i++ )
				if ( list[ i ].type == (byte)enums.playerMemory.exchangeWith )
				{
					if ( list[ i ].ind[ 0 ] == other )
					{
						toErease[ i ] = true;
						tot++;
					}
				}

			structures.memory[] buffer = list;

			list = new structures.memory[ buffer.Length - tot ];
			for ( int i = 0, j = 0; i < buffer.Length; i++ )
				if ( !toErease[ i ] )
				{
					list[ j ] = buffer[ i ];
					j++;
				}
		}

		public void addExchange( negoList list )
		{
			byte other;
			if ( list.players[ 0 ] == player.player )
				other = list.players[ 1 ];
			else
				other = list.players[ 0 ];

			int tot = 0;


			int[] inds0 = new int[ 100 ];

			for ( int p = 0; p < 2; p ++ )
				if ( list.moneyPerTurn[ p ] > 0 )
				{
					inds0[ tot ] = (int)nego.infoType.giveMoneyPerTurn;
					tot++;
					inds0[ tot ] = list.players[ p ];
					tot++;
					inds0[ tot ] = (int)list.moneyPerTurn[ p ];
					tot++;
				}

			for ( int i = 0; i < list.Length; i++ )
			{
				if ( list.list[ i ].type == (int)nego.infoType.giveResource )
				{
					inds0[ tot ] = list.list[ i ].type;
					tot++;
					inds0[ tot ] = list.players[ list.list[ i ].player ];
					tot++;
					inds0[ tot ] = list.list[ i ].info;
					tot++;
				}
			}

			if ( tot > 0 )
			{
				int[] inds1 = new int[ tot + 1 ];
				inds1[ 0 ] = other;

				for ( int i = 0; i < tot; i ++ )
					inds1[ i + 1 ] = inds0[ i ];

				add( (byte)enums.playerMemory.exchangeWith, inds1, player.game.curTurn );
			}
		}
#endregion

#region city site
		public void addCitySiteNotFoundOnCont( /*int cont,*/ Point pos)
		{
			int cont = player.game.grid[ pos.X, pos.Y ].continent;
			int[] inds0 = new int[]
			{
				cont,
				pos.X,
				pos.Y
			};
			add( (byte)enums.playerMemory.cannotFindCitySiteOnCont, inds0, player.game.curTurn ); // !!!
		}

		public void addCitySiteNotFoundWW()//int player)
		{
			int[] inds0 = new int[ 0 ];
			add( (byte)enums.playerMemory.cannotFindCitySiteWW, inds0, player.game.curTurn ); // !!!
		}

		public void addFoundCitySiteWW( Point dest )
		{
			int[] inds0 = new int[]
			{
				dest.X,
				dest.Y/*,
				1*/
			};
			add( (byte)enums.playerMemory.foundCitySiteWW, inds0, player.game.curTurn ); // !!!
		}

		public bool mayBeAbleToFindCitySiteOnCont(/*int cont,*/ Point pos)
		{
			int cont = player.game.grid[ pos.X, pos.Y ].continent;
			structures.memory[] mems = findAllSinceAndDeletePrevious( (byte)enums.playerMemory.cannotFindCitySiteOnCont, 1 );
			return mems.Length == 0;
		}

		public bool mayBeAbleToFindCitySiteWW(/*int cont,*/ Point pos)
		{
			int cont = player.game.grid[ pos.X, pos.Y ].continent;
			structures.memory[] mems = findAllSinceAndDeletePrevious( (byte)enums.playerMemory.cannotFindCitySiteWW, 1 );
			return mems.Length == 0;
		}

		public Point lastCitySiteWW(/*int cont,*/ Point pos)
		{
			int cont = player.game.grid[ pos.X, pos.Y ].continent;
			structures.memory[] mems = findAllSinceAndDeletePrevious( (byte)enums.playerMemory.cannotFindCitySiteWW, 1 );
			
			if ( mems.Length > 0 )
				return new Point( mems[ 0 ].ind[ 0 ], mems[ 0 ].ind[ 1 ] );
			else
				return new Point( -1, -1 );
		}
#endregion

#region join city

		public void addSentToJoinedCity( int cont )
		{
			int pos = -1;
			for ( int i = list.Length - 1; i >= 0; i -- )
				if ( list[ i ].turn == player.game.curTurn )
				{
					if ( list[ i ].type == (byte)enums.playerMemory.settlerSentToJoinCityOnConts )
					{
						pos = i;
						break;
					}
				}
				else
					break;

			if ( pos == -1 )
			{
				add( (byte)enums.playerMemory.settlerSentToJoinCityOnConts, cont );
			}
			else
			{
				bool alreadyThere = false;
				for ( int i = 0; i < list[ pos ].ind.Length; i ++ )
					if ( list[ pos ].ind[ i ] == cont )
						alreadyThere = true;

				if ( !alreadyThere )
				{
					int[] buffer = list[ pos ].ind;

					list[ pos ].ind = new int[ buffer.Length + 1 ];
					buffer.CopyTo( list[ pos ].ind, 0 );
					list[ pos ].ind[ buffer.Length ] = cont;
				}
			}
		}
		public bool settlersStillAcceptedOnCont(int cont)
		{
		/*	structures.memory[] mems = findAllSinceAndDeletePrevious( (byte)enums.playerMemory.cannotFindCitySiteOnCont, 4 );
			for ( int i = 0; i < mems.Length; i++ )
				for ( int j = 0; i < mems[ i ].ind.Length; j++ )
					if ( mems[ i ].ind[ j ] == cont )
					{
					}*/

			structures.memory[] mems = findAllSinceAndDeletePrevious( (byte)enums.playerMemory.settlerSentToJoinCityOnConts, 4 );
			for ( int i = 0; i < mems.Length; i ++ )
				for ( int j = 0; i < mems[ i ].ind.Length; j++ )
					if ( mems[ i ].ind[ j ] == cont )
						return false;

			return true;
		}

#endregion

	}
}

