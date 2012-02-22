using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for unitOrder.
	/// </summary>
	public class unitOrder
	{
		public static int[] list;
		public static void setOrder( byte player )
		{
			int unitNbr = 0, invalid = 10000;

			Point ori;
			if ( Form1.game.playerList[ player ].cityNumber > 0 )
				ori = Form1.game.playerList[ player ].cityList[ Form1.game.playerList[ player ].capital ].pos;
			else
				ori = new Point( Form1.game.width / 2, Form1.game.height / 2 );

			for ( int u = 1; u <= Form1.game.playerList[ player ].unitNumber; u ++ )
			/*	if (
					Form1.game.playerList[ player ].unitList[ u ].state == (byte)Form1.unitState.idle && 
					( 
					Form1.game.playerList[ player ].unitList[ u ].moveLeft > 0 || 
					Form1.game.playerList[ player ].unitList[ u ].moveLeftFraction > 0 
					) &&
					!Form1.game.playerList[ player ].unitList[ u ].automated
					)*/
				if ( !Form1.game.playerList[ player ].unitList[ u ].dead )
					unitNbr ++;

			list = new int[ unitNbr ];

			if ( unitNbr > 0 )
			{
				bool[] valid = new bool[ Form1.game.playerList[ player ].unitNumber ];
				int[] distFomOri = new int[ Form1.game.playerList[ player ].unitNumber ]; // unitNbr ];

				for ( int u0 = 1, u1 = 0; u0 <= Form1.game.playerList[ player ].unitNumber; u0 ++, u1 ++ )
				/*	if (
						Form1.game.playerList[ player ].unitList[ u0 ].state == (byte)Form1.unitState.idle && 
						( 
						Form1.game.playerList[ player ].unitList[ u0 ].moveLeft > 0 || 
						Form1.game.playerList[ player ].unitList[ u0 ].moveLeftFraction > 0 
						) &&
						!Form1.game.playerList[ player ].unitList[ u0 ].automated
						)*/
					if ( !Form1.game.playerList[ player ].unitList[ u0 ].dead ) // validUnit( player, u0 ) )//!Form1.game.playerList[ player ].unitList[ u0 ].dead )
					{
						distFomOri[ u1 ] = Form1.game.radius.getDistWith( Form1.game.playerList[ player ].unitList[ u0 ].pos, ori );
						valid[ u1 ] = true;
				//		u1 ++;
					}
					else
					{
						distFomOri[ u0 - 1 ] = invalid;
						valid[ u0 - 1 ] = false;
					}

				int[] order = count.ascOrder( distFomOri );
				list[ 0 ] = order[ 0 ];
				valid[ order[ 0 ] ] = false;

				for ( int un = 1; un < list.Length; un++ )
				{
					int[] distFomOtherUnit = new int[ Form1.game.playerList[ player ].unitNumber ];
					for ( int u0 = 1; u0 <= Form1.game.playerList[ player ].unitNumber; u0++ )
						if ( valid[ u0 - 1 ] ) 
							distFomOtherUnit[ u0 - 1 ] = Form1.game.radius.getDistWith( Form1.game.playerList[ player ].unitList[ u0 ].pos, Form1.game.playerList[ player ].unitList[ list[ un - 1 ] + 1 ].pos );
						else
							distFomOtherUnit[ u0 - 1 ] = invalid;

					order = count.ascOrder( distFomOtherUnit );

					list[ un ] = order[ 0 ];
					valid[ order[ 0 ] ] = false;
				}
			}
		} 
		public static int nextUnit( byte player, int unit )
		{
			if ( list.Length > 0 )
			{
				int pos = list.Length;

				for ( int i = 0; i < list.Length; i ++ )
					if ( list[ i ] + 1 == unit )
					{
						pos = i;
						break;
					}

				int nextPos = getNextPos( pos );
				while ( !validInList( player, nextPos ) )
				{
					nextPos = getNextPos( nextPos );

					if ( nextPos == getNextPos( pos ) || ( pos == list.Length && nextPos == list.Length - 1 ) )
						return -1;
				}

				return list[ nextPos ] + 1;
			}
			else
				return -1;
		} 
		public static int prevUnit(byte player, int unit)
		{
			if ( list.Length > 0 )
			{
				int pos = list.Length;

				for ( int i = 0; i < list.Length; i ++ )
					if ( list[ i ] + 1 == unit )
					{
						pos = i;
						break;
					}

				int prevPos = getPrevPos( pos );
				while ( !validInList( player, prevPos ) )
				{
					prevPos = getPrevPos( prevPos );

					if ( prevPos == getPrevPos( pos ) || ( pos == list.Length && prevPos == 0 ) )
						return -1;
				}

				return list[ prevPos ] + 1;
			}
			else
				return -1;
		}

		private static int getNextPos(int from)
		{
			if ( from < list.Length - 1 )
				return from + 1;
			else
				return 0;
		}

		private static int getPrevPos(int from)
		{
			if ( from > 0 )
				return from - 1;
			else
				return list.Length - 1;
		}

		private static bool validInList(byte player, int pos)
		{ 
			return validUnit( player, list[ pos ] + 1 );

		/*	if (
			//	!Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].dead &&
				(
					Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].moveLeft > 0 ||
					Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].moveLeftFraction > 0
				) &&
				Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].state == (byte)Form1.unitState.idle 
			/* &&
				!Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].automated &&
				Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].state != (byte)Form1.unitState.inTransport &&
				Form1.plaList[ player ].unitList[ list[ pos ] + 1 ].state != (byte)Form1.unitState.sleep &&
				Form1.game.playerList[ player ].unitList[ list[ pos ] + 1 ].state != (byte)Form1.unitState.fortified*
				)
				return true;
			else
				return false;*/
		}

		private static bool validUnit(byte player, int unit)
		{ 
			if (
				Form1.game.playerList[ player ].unitList[ unit ].state == (byte)Form1.unitState.idle &&
				!Form1.game.playerList[ player ].unitList[ unit ].automated &&
				(
				Form1.game.playerList[ player ].unitList[ unit ].moveLeft > 0 ||
				Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction > 0
				)
				)
				return true;
			else
				return false;
		}	
	}
}
