using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for relationQuality.
	/// </summary>
	public class relationQuality
	{

		public relationQuality()
		{
			//
			// TODO: Add constructor logic here
			//
		}

#region makeAStep
		public static void makeAStep( byte player, Point pos )
		{
			if ( 
				Form1.game.grid[ pos.X, pos.Y ].territory - 1 != player &&
				Form1.game.grid[ pos.X, pos.Y ].territory != 0 )
			{
				stepOnTerritory( player, (byte)(Form1.game.grid[ pos.X, pos.Y ].territory - 1) );
			}
		}
		#endregion

#region stepOnTerritory
		public static void stepOnTerritory( byte offender, byte offended )
		{
			if ( Form1.game.playerList[ offender ].foreignRelation[ offended ].politic == (byte)Form1.relationPolType.peace )
			{
				Form1.game.playerList[ offender ].foreignRelation[ offended ].quality --;
			}
			else if ( Form1.game.playerList[ offender ].foreignRelation[ offended ].politic == (byte)Form1.relationPolType.ceaseFire )
			{
				Form1.game.playerList[ offender ].foreignRelation[ offended ].quality -= 5;
			}
		}
		#endregion

#region endTurn
		public static void endTurn( byte player )
		{
			for ( int i = 0; i < Form1.game.playerList.Length; i ++ )
				if ( 
					i != player && 
					!Form1.game.playerList[ i ].dead &&
					Form1.game.playerList[ player ].foreignRelation[ i ].madeContact
					)
				{
					if ( 
						Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.peace ||
						Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.Protected
						)
					{
						Form1.game.playerList[ player ].foreignRelation[ i ].quality ++;
					}
					else if ( Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.ceaseFire )
					{
					}
					else if ( Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.alliance )
					{
					}
					else if ( Form1.game.playerList[ player ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
					{
					}
				}
		}
		#endregion

	}
}
