using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Ethnicity.
	/// </summary>
	public class Ethnicity
	{
		int city;
		byte player;
		int[] percOrigins;

		public Ethnicity( byte p, int c )
		{
			player = p;
			city = c;
			percOrigins = new int[ Form1.game.playerList.Length ];
			percOrigins[ player ] = 100;
		}

		public Ethnicity(  byte p, int c, int[] percs )
		{
			player = p;
			city = c;
			percOrigins = percs;
		}

	/*	public int[] getFinalNumber
		{
			get
			{
				int[] nbrs = new int[ Form1.game.playerList.Length ];
				int tot = 0;

				for ( int p = 0; p < nbrs.Length; p++ )
				{
					nbrs[ p ] = percOrigins[ p ] * Form1.game.playerList[ player ].cityList[ city ].population;
					tot += nbrs[ p ];
				}

				while ( tot < Form1.game.playerList[ player ].cityList[ city ].population )
				{
					nbrs[ player ]++;
					tot++;
				}

				return nbrs;
			}
		}	*/
	}
}
