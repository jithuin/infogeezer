using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for pref.
	/// </summary>
	public class aiPref
	{
		public static void setMilitary( byte player, sbyte mod )
		{
			int diff = mod - Form1.game.playerList[ player ].preferences.military;

			if ( Form1.game.playerList[ player ].preferences.reserve - mod >= -100 )
			{
				Form1.game.playerList[ player ].preferences.military = mod;

				setReserve( player );
			}
		}
		
		public static void setReserve( byte player )
		{
			int tot = 
				Form1.game.playerList[ player ].preferences.buildings + 
				Form1.game.playerList[ player ].preferences.culture + 
				Form1.game.playerList[ player ].preferences.intelligence + 
				Form1.game.playerList[ player ].preferences.military + 
				Form1.game.playerList[ player ].preferences.space + 
				Form1.game.playerList[ player ].preferences.science + 
				Form1.game.playerList[ player ].preferences.exchanges;

			int left = 200 - tot;

			while ( left < 0 )
			{
				if ( Form1.game.playerList[ player ].preferences.science > 0 )
					Form1.game.playerList[ player ].preferences.science --;
				else if ( Form1.game.playerList[ player ].preferences.culture > 0 )
					Form1.game.playerList[ player ].preferences.culture --;
				else if ( Form1.game.playerList[ player ].preferences.military > 0 )
					Form1.game.playerList[ player ].preferences.military --;
				else if ( Form1.game.playerList[ player ].preferences.buildings > 0 )
					Form1.game.playerList[ player ].preferences.buildings --;

				left ++;
				tot --;
			}

			Form1.game.playerList[ player ].preferences.reserve = (sbyte)( 100 - tot );
		}

		public static void setPrefs( byte player, int nationTrade )
		{
			bool isInWar = ai.isInWar( player ); 
			if ( count.technoAdvancement( player ) > 10 )
			{
				int technoLevelPosition = count.technoPosition( player ); 

				int nextR = Form1.game.playerList[ player ].preferences.reserve;
				int reserveMin = (int)(- 1 * ( Form1.game.playerList[ player ].money * 100 ) / ( ( nationTrade != 0 ? nationTrade : 1 ) * 10 ));

				if ( reserveMin < -100 )
					reserveMin = -100;
				else if ( reserveMin > 100 )
					reserveMin = 100;

				int maxSci = 
					Form1.game.playerList[ player ].preferences.science + 
					Form1.game.playerList[ player ].preferences.reserve +
					100 - reserveMin;
				int minSci = 0;

				if ( maxSci > 100 ) 
					maxSci = 100; 

				Random r = new Random(); 
				int newSci = r.Next( maxSci - minSci ) + minSci;

				if ( newSci > maxSci ) 
					newSci = maxSci; 
 
				Form1.game.playerList[ player ].preferences.science = (sbyte)newSci; 
				setReserve( player ); 
			}
		}

		public static void setBuildingUpkeep( byte player ) 
		{ 
		/*	getPFT gp = new getPFT(); 
			int nationTrade = gp.getNationTrade( player ); */

			int cost = count.upkeepCost( player ); 
			Form1.game.playerList[ player ].preferences.buildings = (sbyte)( cost * 100 / ( Form1.game.playerList[ player ].totalTrade != 0 ? Form1.game.playerList[ player ].totalTrade : 1 ) ); 

			setReserve( player ); 
		} 
	}
}
