using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for playerSlavery.
	/// </summary>
	public class playerSlavery
	{
		PlayerList player;
		public struct transfertList
		{
			public int ori, dest, nbr, eta;
		}
		public transfertList[] transferts;

		public playerSlavery( PlayerList player )
		{
			transferts = new transfertList[ 0 ];
			this.player = player;
		}

	/*	
		public int tot
		{
			get
			{
				int tot = 
				return 0;
			}
		}	
	*/
		public int availableInMarket
		{
			get
			{
				return player.cityList[ player.capital ].slaves.total;
			}
		}

		public int moveSlave( int cityOri, int cityDest, int nbr )
		{
			player.cityList[ cityOri ].slaves.remove( nbr );
			transfertList[] buffer = transferts;
			transferts = new transfertList[ buffer.Length + 1 ];

			for ( int i = 0; i < buffer.Length; i++ )
			{
				transferts[ i ] = buffer[ i ];
			}

			transferts[ buffer.Length ].ori = cityOri;
			transferts[ buffer.Length ].dest = cityDest;
			transferts[ buffer.Length ].nbr = nbr;
			transferts[ buffer.Length ].eta = Form1.game.curTurn + Form1.game.radius.getDistWith( player.cityList[ cityOri ].pos, player.cityList[ cityDest ].pos ) / 4;
			return Form1.game.radius.getDistWith( player.cityList[ cityOri ].pos, player.cityList[ cityDest ].pos ) / 4;
		}

		public void removeSlave( int nbr )
		{
			/*if ( player.cityList[ player.capital ].slaves.total < nbr )
			{
				int whatsLeft = nbr - player.cityList[ player.capital ].slaves.total;
				player.cityList[ player.capital ].slaves.removeAll();

				while ( whatsLeft
			}
			else
			{*/
				player.cityList[ player.capital ].slaves.remove( nbr );
			//}

		}

		public void addSlave( int nbr )
		{
			player.cityList[ player.capital ].slaves.add( nbr );
		}

		public void endOfTurn()
		{
			for ( int c = 1; c <= player.cityNumber; c ++ )
				if ( !player.cityList[ c ].dead )
				{
					if ( player.cityList[ c ].nonLabor.totalOfOneType( (byte)peopleNonLabor.types.slaver ) > 0 )
					{
						player.cityList[ c ].slaves.convertingToSlave += 200 * player.cityList[ c ].nonLabor.totalOfOneType( (byte)peopleNonLabor.types.slaver );

						if ( player.cityList[ c ].slaves.convertingToSlave > 1000 )
						{
							int toBeSlave = player.cityList[ c ].slaves.convertingToSlave / 1000;
							if ( toBeSlave >= player.cityList[ c ].population )
								toBeSlave = player.cityList[ c ].population - 1;

							player.cityList[ c ].slaves.add( toBeSlave );
							player.cityList[ c ].population -= (byte)toBeSlave;
							player.cityList[ c ].slaves.convertingToSlave -= 1000;
						}
					}
					else
					{
						player.cityList[ c ].slaves.convertingToSlave = 0;
					}

				}

			if ( transferts.Length > 0 )
			{
				bool[] toBeRemoved = new bool[ transferts.Length ];
				int tot = 0;

				for ( int i = 0; i < transferts.Length; i++ )
					if ( Form1.game.curTurn <= transferts[ i ].eta )
					{
						player.cityList[ transferts[ i ].dest ].slaves.add( transferts[ i ].nbr );
						toBeRemoved[ i ] = true;
						tot++;
					}

				if ( tot > 0 )
				{
					transfertList[] buffer = transferts;
					transferts = new transfertList[ buffer.Length - tot ];

					for ( int i = 0, j = 0; i < buffer.Length; i++ )
						if ( !toBeRemoved[ i ] )
						{
							transferts[ j ] = buffer[ i ];
							j ++;
						}
				}
			}
		}
	}
}
