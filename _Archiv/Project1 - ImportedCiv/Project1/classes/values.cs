using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for values.
	/// </summary>
	public class values
	{
		public static int unitSupported( byte player )
		{
			getPFT gp = new getPFT();
			return ( Form1.game.playerList[ player ].totalTrade * Form1.game.playerList[ player ].preferences.military / 100 ) / 3; /// Statistics.governements
		}
		
		public static int unitSupported( byte player, sbyte pref, int nationTrade )
		{
			return ( nationTrade * pref / 100 ) / 3; /// Statistics.governements
		}
	}
}
