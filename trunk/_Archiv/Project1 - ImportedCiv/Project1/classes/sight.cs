using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for sight.
	/// </summary>
	public class sight
	{

		#region set all sight
		public static void setAllSight( byte player )
		{
			setTerritorySight( player );

			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber; i ++ )
				if ( 
					Form1.game.playerList[ player ].unitList[ i ].state != (byte)Form1.unitState.dead 
					)
				{
					setUnitSight( player, i );
				}
		}
		#endregion
		
		public static void clearSight( byte player )
		{
			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					Form1.game.playerList[ player ].see[ x, y ] = false;
		}	
		
		#region set territory sight
		public static void setTerritorySight( byte player )
		{
			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( Form1.game.grid[ x, y ].territory - 1 == player )
					{
						Form1.game.playerList[ player ].see[ x, y ] = true;
						Form1.game.playerList[ player ].discovered[ x, y ] = true;
						
						if ( Statistics.terrains[ Form1.game.grid[ x, y ].type ].ew == 0 )
						{
							Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );
							for ( int h = 0; h < sqr.Length; h ++ )
							{
								Form1.game.playerList[ player ].see[ sqr[ h ].X, sqr[ h ].Y ] = true;
								Form1.game.playerList[ player ].discovered[ sqr[ h ].X, sqr[ h ].Y ] = true;
							}
						}
					}
					else if ( Form1.game.playerList[ player ].see[ x, y ] )
					{
						lastSee( x, y, player );
						Form1.game.playerList[ player ].see[ x, y ] = false;
					}
					else
					{
						Form1.game.playerList[ player ].see[ x, y ] = false;
					}
			
			for ( int x = 0; x < Form1.game.width; x ++ )
				for ( int y = 0; y < Form1.game.height; y ++ )
					if ( 
						Form1.game.grid[ x, y ].territory - 1 == player && 
						Statistics.terrains[ Form1.game.grid[ x, y ].type ].ew == 0 
						)
					{
						Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );
						for ( int h = 0; h < sqr.Length; h ++ )
						{
							Form1.game.playerList[ player ].see[ sqr[ h ].X, sqr[ h ].Y ] = true;
							Form1.game.playerList[ player ].discovered[ sqr[ h ].X, sqr[ h ].Y ] = true;
						}
					}
		}
		#endregion
		
		#region set unit sight
		public static void setUnitSight( byte player, int unit )
		{
			if ( Form1.game.playerList[ player ].unitList[ unit ].X != -1 && Form1.game.playerList[ player ].unitList[ unit ].Y != -1 ) // Form1.game.playerList[ player ].unitList[ unit ].state != (byte)Form1.unitState.inTransport )
			{
			//	int bob = (byte)Form1.unitState.turnCompleted;
				Form1.oldSliVer = -1;
			
				byte sight = Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].sight;
					
				if ( Form1.game.grid[ Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ].type == (byte)enums.terrainType.mountain || Form1.game.grid[ Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y ].type == (byte)enums.terrainType.hill )
					sight ++;

				for ( int j = 0; j <= sight; j ++ )
				{
					Point[] sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ player ].unitList[ unit ].X, Form1.game.playerList[ player ].unitList[ unit ].Y, j );

					for ( int k = 0; k < sqr.Length; k ++ )
					{
						Form1.game.playerList[ player ].see[ sqr[k ].X, sqr[ k ].Y ] = true;
						Form1.game.playerList[ player ].discovered[ sqr[k ].X, sqr[ k ].Y ] = true;

						if ( 
							Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 && 
							Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 != player && 
							!Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].madeContact
							)// si il y a une ville autre que la sienne
						{
							nego.establishContact( player, (byte)(Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1) );
						}
						else
							for ( int h = 0; h < Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack.Length; h ++ )
								if ( 
									Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ h ].player.player != player && 
									!Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ h ].player.player ].madeContact
									)
									nego.establishContact( player, (byte)(Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].stack[ h ].player.player) );
						
						if ( 
							Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.sea || 
							Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.coast 
							)
						{
							Point[] sqr2 = Form1.game.radius.returnEmptySquare( sqr[ k ].X, sqr[ k ].Y, 1 );

							for ( int h = 0; h < sqr2.Length; h ++ )
							{
								Form1.game.playerList[ player ].see[ sqr2[ h ].X, sqr2[ h ].Y ] = true;
								Form1.game.playerList[ player ].discovered[ sqr2[ h ].X, sqr2[ h ].Y ] = true;
							}
						}
					}
				}
			}
		}
		#endregion

		public static void lastSee( Point pos, byte player )
		{
			lastSee( pos.X, pos.Y, player );
		}

		public static void lastSee( int x, int y, byte player )
		{
			Form1.game.playerList[ player ].lastSeen[ x, y ].territory = Form1.game.grid[ x, y ].territory;
			Form1.game.playerList[ player ].lastSeen[ x, y ].militaryImp = Form1.game.grid[ x, y ].militaryImprovement;
			Form1.game.playerList[ player ].lastSeen[ x, y ].civicImp = Form1.game.grid[ x, y ].civicImprovement;
			Form1.game.playerList[ player ].lastSeen[ x, y ].road = Form1.game.grid[ x, y ].roadLevel;
			Form1.game.playerList[ player ].lastSeen[ x, y ].turn = Form1.game.curTurn;

			Form1.game.playerList[ player ].lastSeen[ x, y ].city = Form1.game.grid[ x, y ].city;

			if ( 
				Form1.game.grid[ x, y ].territory != 0 &&
				Form1.game.playerList[ player ].lastSeen[ x, y ].city > 0
				)
				Form1.game.playerList[ player ].lastSeen[ x, y ].cityPop = Form1.game.playerList[ Form1.game.grid[ x, y ].territory - 1 ].cityList[ Form1.game.grid[ x, y ].city ].population;
		}

		public static void transfertLastSeen( int x, int y, byte giver, byte receiver )
		{
			Form1.game.playerList[ receiver ].lastSeen[ x, y ].territory		=	Form1.game.playerList[ giver ].lastSeen[ x, y ].territory;
			Form1.game.playerList[ receiver ].lastSeen[ x, y ].militaryImp	=	Form1.game.playerList[ giver ].lastSeen[ x, y ].militaryImp;
			Form1.game.playerList[ receiver ].lastSeen[ x, y ].civicImp		=	Form1.game.playerList[ giver ].lastSeen[ x, y ].civicImp;
			Form1.game.playerList[ receiver ].lastSeen[ x, y ].road			=	Form1.game.playerList[ giver ].lastSeen[ x, y ].road;
			Form1.game.playerList[ receiver ].lastSeen[ x, y ].turn			=	Form1.game.playerList[ giver ].lastSeen[ x, y ].turn;

			Form1.game.playerList[ receiver ].lastSeen[ x, y ].city			=	Form1.game.playerList[ giver ].lastSeen[ x, y ].city;
			Form1.game.playerList[ receiver ].lastSeen[ x, y ].cityPop		=	Form1.game.playerList[ giver ].lastSeen[ x, y ].cityPop;
		}

		public static void seeByPlaneAt( byte player, int unit, Point dest )
		{
			for ( int r = 0; r <= Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].sight; r ++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( dest, r );

				for ( int k = 0; k < sqr.Length; k ++ )
				{
					Form1.game.playerList[ player ].discovered[ sqr[ k ].X, sqr[ k ].Y ] = true;
					Form1.game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
				}
			}
		}

		#region set spy sat sight
		public static void setSpySatSight(byte player, Point pos, int sight )
		{
			for ( int j = 0; j <= sight; j++ )
			{
				Point[] sqr = Form1.game.radius.returnEmptySquare( pos, j );

				for ( int k = 0; k < sqr.Length; k++ )
				{
					Form1.game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
					Form1.game.playerList[ player ].discovered[ sqr[ k ].X, sqr[ k ].Y ] = true;

				}
			}
		}
		#endregion

		#region discover radius
		public static void discoverRadius( int x, int y, int radius1, byte player )
		{
			Point[] sqr;
			if ( 
				Form1.game.grid[ x, y ].type == (byte)enums.terrainType.mountain || 
				Form1.game.grid[ x, y ].type == (byte)enums.terrainType.hill 
				)
				sqr = Form1.game.radius.returnSquare( x, y, radius1 + 1 );
			else
				sqr = Form1.game.radius.returnSquare( x, y, radius1 );

			Form1.game.playerList[ player ].discovered[ x , y ] = true;

			for ( int i = 0; i < sqr.Length; i ++ )
			{
				Form1.game.playerList[ player ].discovered[ sqr[ i ].X , sqr[ i ].Y ] = true;

				if ( 
					Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].city > 0 && 
					Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 != player && 
					!Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 ].madeContact
					)// si il y a une ville autre que la sienne
				{
					Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 ].madeContact = true;
					Form1.game.playerList[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].territory - 1 ].foreignRelation[ player ].madeContact = true;
				}

				if (
					Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack.Length > 0
					)
				{
					for ( int j = 1; j <= Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack.Length; j ++ )
					{
						if (
							Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player != player &&
							( !Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].madeContact || 
							!Form1.game.playerList[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].foreignRelation[ player ].madeContact )
							)
						{
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].madeContact = true;
							Form1.game.playerList[ Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].stack[ j - 1 ].player.player ].foreignRelation[ player ].madeContact = true;
						}
					}
				}

				if ( Form1.game.grid[ sqr[ i ].X , sqr[ i ].Y ].type == (byte)enums.terrainType.sea || Form1.game.grid[  sqr[ i ].X , sqr[ i ].Y ].type == (byte)enums.terrainType.coast )
				{
					Point[] sqr2 = Form1.game.radius.returnEmptySquare( sqr[ i ].X , sqr[ i ].Y, 1 );
					for ( int j = 0; j < sqr2.Length; j ++ )
					{
						Form1.game.playerList[ player ].discovered[ sqr2[ j ].X , sqr2[ j ].Y ] = true;
					}
				}
			}
		}
		#endregion
	}
}
