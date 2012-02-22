using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for cityHappiness.
	/// </summary>
	public class cityHappiness
	{
		public cityHappiness()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void endTurn( byte player, int city )
		{
			// modérer
			Form1.game.playerList[ player ].cityList[ city ].happiness = Form1.game.playerList[ player ].cityList[ city ].happiness * 80 / 100;

		#region ennemy units

			int unitInRange = countUnitInRange( player, city );
			if ( unitInRange > 0 )
			{
				Form1.game.playerList[ player ].cityList[ city ].happiness -= 100 * unitInRange;
			}
			else
			{
				Form1.game.playerList[ player ].cityList[ city ].happiness += 100;
			}

		#endregion

		#region city pop

			Form1.game.playerList[ player ].cityList[ city ].happiness -= ( Form1.game.playerList[ player ].cityList[ city ].population - 6 ) * 5;

		#endregion

		#region building
			for ( int i = 0; i < Form1.game.playerList[ player ].cityList[ city ].buildingList.Length; i ++ )
				if ( Form1.game.playerList[ player ].cityList[ city ].buildingList[ i ] )
				{
					Form1.game.playerList[ player ].cityList[ city ].happiness += 5;
				}
		#endregion

		#region wonder
		#endregion

		#region unit in the city

			int muoc = militaryUnitOnCase( 
				Form1.game.playerList[ player ].cityList[ city ].X, 
				Form1.game.playerList[ player ].cityList[ city ].Y, 
				player );

			if ( Form1.game.playerList[ player ].govType.oppresive )
			{ 
				int idealNbr = 2;
				if ( muoc > idealNbr * 3 )
				{
					Form1.game.playerList[ player ].cityList[ city ].happiness -= 100;
				}
				else if ( muoc > idealNbr )
				{
					Form1.game.playerList[ player ].cityList[ city ].happiness += ( muoc - 2 ) * 15;
				}
				else
				{
					Form1.game.playerList[ player ].cityList[ city ].happiness -= 100;
				}
			}
			else // free
			{ // 2 being the max number
				Form1.game.playerList[ player ].cityList[ city ].happiness -= ( muoc - 2 ) * 15;
			}
		#endregion

		#region neighbours culture


		#endregion

			if ( Form1.game.playerList[ player ].cityList[ city ].happiness < 0 )
				Form1.game.playerList[ player ].cityList[ city ].rioting = true;
			else
				Form1.game.playerList[ player ].cityList[ city ].rioting = false;
		}

		public static bool[] nationsToRebel( byte player )
		{
			bool[] list = citiesToRebel( player );

			if ( list.Length > 0 )
			{
				bool[] listNation = new bool[ Form1.game.playerList.Length ];
				int[] totPerNation = new int[ Form1.game.playerList.Length ];
				int[] revoltingPerNation = new int[ Form1.game.playerList.Length ];

				for ( int i = 1; i <= Form1.game.playerList[ player ].cityNumber; i ++ )
					if ( 
						Form1.game.playerList[ player ].cityList[ i ].state != (byte)enums.cityState.dead &&
						Form1.game.playerList[ player ].cityList[ i ].originalOwner != player 
						)
					{
						totPerNation[ Form1.game.playerList[ player ].cityList[ i ].originalOwner ] ++;
						if ( list[ i - 1 ] )
							revoltingPerNation[ Form1.game.playerList[ player ].cityList[ i ].originalOwner ] ++;
					}

				for ( int i = 0; i <= totPerNation.Length; i ++ )
					if ( totPerNation[ i ] != 0 )
						if ( revoltingPerNation[ i ] * 100 / totPerNation[ i ] >= 75 )
							listNation[ i ] = true;

				return listNation;
			}
			else
			{
				return new bool[ 0 ];
			}
		}

		public static bool[] continentsToRebel( byte player )
		{
			return new bool[ 0 ];
		}

		/// <summary>
		/// return bool list, city - 1
		/// </summary>
		/// <param name="player"></param>
		/// <param name="city"></param>
		/// <returns></returns>
		public static bool[] citiesToRebel( byte player )
		{
			bool[] list = new bool[ Form1.game.playerList[ player ].cityNumber ];
			bool mod = false;

			for ( int i = 1; i <= Form1.game.playerList[ player ].cityNumber; i ++ )
				if ( Form1.game.playerList[ player ].cityList[ i ].state != (byte)enums.cityState.dead )
					if ( Form1.game.playerList[ player ].cityList[ i ].happiness < -1000 )
					{
						list[ i - 1 ] = true;
						mod = true;
					}

			if ( !mod )
				return new bool[ 0 ];
			else
				return list;
		}

		public static int countUnitInRange( byte player, int city )
		{
			Point[] region = Form1.game.radius.returnCityRadius( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y );
			int tot = 0;
			byte[] rt = new byte[ 1 ];
			rt[ 0 ] = (byte)Form1.relationPolType.war;

			for ( int i = 0; i < region.Length; i ++ )
				if ( 
					Form1.game.radius.caseOccupiedByRelationType( 
					region[ i ].X, 
					region[ i ].Y,
					player,
					rt
					) 
					)
					tot ++;

			return 0;
		}
		
		public static int militaryUnitOnCase( int x, int y, byte player )
		{
			int tot = 0;
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++)
				if ( Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == player )
					if ( Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.speciality == enums.speciality.none )
						tot ++;

			return tot;
		}
	}
}
