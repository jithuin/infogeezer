using System;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for pollution.
	/// </summary>
	public class pollution
	{
		/// <summary>
		/// Per player
		/// </summary>
		/// <param name="player"></param>
		public static void endTurn( byte player )
		{
			int tn = count.technoNumber( player );

			for ( int city = 1; city <= Form1.game.playerList[ player ].cityNumber; city ++ )
				if ( Form1.game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
				{
					int cal = Form1.game.playerList[ player ].cityList[ city ].population * tn - 10 * 20;

					if ( cal > 0 )
						Form1.globalPollution += (uint)cal;
				}

			verifyState();
		}
		
		public static void verifyState()
		{
			if ( Form1.globalPollution > 60000)
			{
			}
			else if ( Form1.globalPollution > 50000)
			{
				MessageBox.Show( "The pollution level in the atmosphere is reaching a dangerous high.", "Pollution alert" );
			}
		}
		
		public static void nuke( byte player, byte type )
		{
			verifyState();
		}

		public static void warming()
		{
		}

		public static void winter()
		{
		}
	}
}
