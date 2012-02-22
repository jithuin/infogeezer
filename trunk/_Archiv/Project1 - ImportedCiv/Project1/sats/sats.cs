using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for sats.
	/// </summary>
	public class sats
	{
		public sats( int spyLenght )
		{
			spy = new singleSateliteGroup( spyLenght, Form1.game.width, 0 );
		}

		public singleSateliteGroup spy;

	}
}
