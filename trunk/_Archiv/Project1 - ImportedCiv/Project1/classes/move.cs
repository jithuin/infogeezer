using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for move.
	/// </summary>
	public class move
	{
		public static UnitList[] stackBuffer;

#region pay
		public static void payThis( byte player, int unit, int movementCost )
		{
			payThis( player, unit, movementCost, 0 );
		}

		public static void payThis( byte player, int unit, int movementCost, int movementCostFraction )
		{
			if ( 
				Form1.game.playerList[ player ].unitList[ unit ].moveLeft == 0 && 
				Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction > 0 
				) 
				Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction = 0; 
			else 
				Form1.game.playerList[ player ].unitList[ unit ].moveLeft -= (sbyte)movementCost; 

			if ( Form1.game.playerList[ player ].unitList[ unit ].moveLeft < 0 ) 
				Form1.game.playerList[ player ].unitList[ unit ].moveLeft = 0; 

			if ( Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction < 0 ) 
				Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction = 0; 
		}
		
		public static void payThrough( byte player, int unit, Point ori, Point dest ) 
		{ 
			if ( 
				Form1.game.grid[ ori.X, ori.Y ].roadLevel == 2 && 
				Form1.game.grid[ dest.X, dest.Y ].roadLevel == 2 
				)
			{
			}
			else if ( 
				(
					Form1.game.grid[ ori.X, ori.Y ].roadLevel > 0 &&
					Form1.game.grid[ dest.X, dest.Y ].roadLevel > 0 
				) ||
				(
					Form1.game.grid[ ori.X, ori.Y ].river &&
					Form1.game.grid[ dest.X, dest.Y ].river
				)
				)
			{
				if ( Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction == 0 )
				{
					Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction = 3;//(sbyte)Form1.moveFraction;
					Form1.game.playerList[ player ].unitList[ unit ].moveLeft --;
				}
				
				Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction -= 1;
			}
			else
			{
				payAt( player, unit, dest );
			}
		}
		
		public static void payAt( byte player, int unit, Point dest ) 
		{ 
			payThis( 
				player, 
				unit, 
				Statistics.terrains[ Form1.game.grid[ dest.X, dest.Y ].type ].move 
				); 
		} 
#endregion

#region moveUnitToCase
		public static void moveUnitToCase(int xDest, int yDest, byte owner, int unit)
		{	// ajouter l unite à la case cible
			int un = Form1.game.grid[ xDest, yDest ].stack.Length + 1;

			stackBuffer = Form1.game.grid[ xDest, yDest ].stack;
			Form1.game.grid[ xDest, yDest ].stack = new UnitList[ un ];
			for ( int i = 0; i < stackBuffer.Length; i++ )
				Form1.game.grid[ xDest, yDest ].stack[ i ] = stackBuffer[ i ];

			Form1.game.grid[ xDest, yDest ].stack[ un - 1 ] = Form1.game.playerList[ owner ].unitList[ unit ];
		/*	Form1.game.grid[ xDest, yDest ].stack[ un - 1 ].player.player = owner;
			Form1.game.grid[ xDest, yDest ].stack[ un - 1 ].unit = unit;	*/
			Form1.game.grid[ xDest, yDest ].stackPos = un;
			relationQuality.makeAStep( owner, new Point( xDest, yDest ) );
		}
		#endregion

#region moveUnitFromCase
		public static void moveUnitFromCase(int xo, int yo, byte owner, int unit) 
		{
			int un = Form1.game.grid[ xo, yo ].stack.Length - 1;

			for ( int i = 1; i <= Form1.game.grid[ xo, yo ].stack.Length; i++ )
				if ( 
					Form1.game.grid[ xo, yo ].stack[ i - 1 ] == Form1.game.playerList[ owner ].unitList[ unit ] /* && 
					Form1.game.grid[ xo, yo ].stack[ i - 1 ].player.player == owner */
					)
				{
					Form1.game.grid[ xo, yo ].stackPos = i;
					break;
				}

			stackBuffer = Form1.game.grid[ xo, yo ].stack;
			Form1.game.grid[ xo, yo ].stack = new UnitList[ un ];

			int j = 0;

			if ( un > 0 )
				for ( int i = 0; i < Form1.game.grid[ xo, yo ].stackPos - 1; i++ )
				{
					Form1.game.grid[ xo, yo ].stack[ j ] = stackBuffer[ i ];
					j++;
				}

			for ( int i = Form1.game.grid[ xo, yo ].stackPos; i < stackBuffer.Length; i++ )
			{
				Form1.game.grid[ xo, yo ].stack[ j ] = stackBuffer[ i ];
				j++;
			}

			Form1.game.grid[ xo, yo ].stackPos = un;
		}
		#endregion
		
#region disembark unit from transport
		public static void disembarkUnit(byte player, int unit, int ship, Point dest)
		{
			Form1.game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.idle;
			if ( 
				Form1.game.playerList[ player ].unitList[ ship ].X != dest.X || 
				Form1.game.playerList[ player ].unitList[ ship ].Y != dest.Y 
				)
				move.payThis( player, unit, 1 );

			Form1.game.playerList[ player ].unitList[ unit ].X = dest.X;
			Form1.game.playerList[ player ].unitList[ unit ].Y = dest.Y;
			moveUnitToCase( dest.X, dest.Y, player, unit );
			transport.removeUnitFromTransport( player, unit, ship );
			sight.setUnitSight( player, unit );
		}
#endregion 

#region unit movement to transport
		public static void UnitMove2Transport(byte player, int unit, int transport)
		{
			// mouvement
			if ( Form1.game.playerList[ player ].unitList[ unit ].X != Form1.game.playerList[ player ].unitList[ transport ].X || Form1.game.playerList[ player ].unitList[ unit ].Y != Form1.game.playerList[ player ].unitList[ transport ].Y )
				payThis( player, unit, 1 );

			// ajouter l'unité au transport 
			Form1.game.playerList[ player ].unitList[ transport ].transported++;
			Form1.game.playerList[ player ].unitList[ transport ].transport[ Form1.game.playerList[ player ].unitList[ transport ].transported - 1 ] = unit;

			// effacer l unite de la derniere case
			move.moveUnitFromCase( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y, player, unit );
			Form1.game.playerList[ player ].unitList[ unit ].X = -1;
			Form1.game.playerList[ player ].unitList[ unit ].Y = -1;
			Form1.game.playerList[ player ].unitList[ unit ].state = (byte)Form1.unitState.inTransport;
		}
	#endregion

	} 
} 
