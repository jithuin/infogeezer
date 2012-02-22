using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for sellUnit.
	/// </summary>
	public class sell
	{
		public static int unitReturn( byte player, int unit )
		{
			return Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].cost / 3;
		}
	}
}
