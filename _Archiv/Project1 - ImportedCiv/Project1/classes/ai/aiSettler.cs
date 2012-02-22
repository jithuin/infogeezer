using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiSettler.
	/// </summary>
	public class aiSettler
	{
		public static int citySiteValue( byte player, System.Drawing.Point pos )
		{
			if ( 
				Form1.game.grid[ pos.X, pos.Y ].water ||
				Form1.game.grid[ pos.X, pos.Y ].territory - 1 != player ||
				Form1.game.grid[ pos.X, pos.Y ].city != 0 ||
				Form1.game.grid[ pos.X, pos.Y ].laborCity != 0 ||
				Form1.game.radius.caseOccupiedByRelationType( pos.X, pos.Y, player, Form1.game.radius.relationTypeListNonAllies )
				)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}
	}
}
