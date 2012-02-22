using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiTown.
	/// </summary>
	public class aiTown
	{
		public static bool shouldBuyDefenseMilitaryUnit( byte player, int city ) 
		{ 
			int totMU = 0; 
			for ( int i = 0; i < Form1.game.grid[ Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ].stack.Length; i ++ )
				if ( 
					(
					Form1.game.grid[ Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ].stack[ i ].player.player == player || 
					Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ].stack[ i ].player.player ].politic == (byte)Form1.relationPolType.alliance || 
					Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ].stack[ i ].player.player ].politic == (byte)Form1.relationPolType.Protector
					) && 
					Form1.game.grid[ Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y ].stack[ i ].typeClass.speciality == enums.speciality.none
					)
					totMU ++;

			if ( totMU == 0 ) 
			{ 
				int totEUIR = 0; 
				byte[] rtl = Form1.game.radius.relationTypeListWarAndCF;
			
				Point[] sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y, 1 );
				for ( int k = 0; k < sqr.Length; k ++ ) 
					if ( Form1.game.radius.caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, rtl ) )
						totEUIR ++;

				if ( totEUIR == 0 )
					for ( int r = 2; r < 4; r ++ ) 
					{ 
						sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y, r );
						for ( int k = 0; k < sqr.Length; k ++ ) 
							if ( Form1.game.radius.caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, rtl ) )
								totEUIR ++;

						if ( totEUIR > 0 )
							return true;
					} 
				else return false;
			} 
			else if ( totMU < 3 ) 
			{ 
				int totEUIR = 0; 
				byte[] rtl = Form1.game.radius.relationTypeListWarAndCF;
 
				for ( int r = 1; r < 4; r ++ ) 
				{ 
					Point[] sqr = Form1.game.radius.returnEmptySquare( Form1.game.playerList[ player ].cityList[ city ].X, Form1.game.playerList[ player ].cityList[ city ].Y, r );
					for ( int k = 0; k < sqr.Length; k ++ ) 
						if ( Form1.game.radius.caseOccupiedByRelationType( sqr[ k ].X, sqr[ k ].Y, player, rtl ) )
							totEUIR ++; 
 
					if ( totEUIR > totMU ) 
						return true; 
				} 
			} 

			return false;
		}

		public static void buyContruction( byte player, int city )
		{
			int cost = Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
			Form1.game.playerList[ player ].money -= cost;
			Form1.game.playerList[ player ].cityList[ city ].construction.points = Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].cost;
			
			/*if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.constructionType.unit )
			{
				int cost = Statistics.units[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
				Form1.game.playerList[ player ].money -= cost;
				Form1.game.playerList[ player ].cityList[ city ].construction.points = Statistics.units[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost;
			}
			else if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.constructionType.building )
			{
				int cost = Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
				Form1.game.playerList[ player ].money -= cost;
				Form1.game.playerList[ player ].cityList[ city ].construction.points = Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost;
			}*/
		}

		public static bool canBuyContruction( byte player, int city )
		{
			int cost = -1;

			if ( 
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Wealth ||
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.Wonder ||
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] is Stat.SmallWonder 
				)
				return false;
			else
				return Form1.game.playerList[ player ].money + 
					Form1.game.playerList[ player ].cityList[ city ].construction.points - 
					Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].cost >= 0;

		/*	if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.constructionType.unit )
				cost = Statistics.units[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
			else if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.constructionType.building )
				cost = Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points;
			else if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.constructionType.wealth )
				return false;
			else if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.constructionType.wonder )
				return false;
			else
				return false;

			if ( Form1.game.playerList[ player ].money - cost >= 0 )
				return true;

			return false;*/
		}

		/*public static void chooseConstruction( byte player, int city, byte type, int ind )
		{
		}*/

		public static void buildRandomDefensiveUnit( byte player, int city )
		{
			Random R = new Random();
			byte[] unitPossible2 = new byte[ Statistics.units.Length ]; 
			int q = 0;
			for ( byte i = 1; i < Statistics.units.Length; i ++ ) // units
				if ( 
					Form1.game.playerList[ player ].technos[ Statistics.units[ i ].disponibility ].researched && 
					Statistics.units[ i ].terrain == 1 && 
					( 
					Statistics.units[ i ].obselete == 0 || 
					!Form1.game.playerList[ player ].technos[ Statistics.units[ Statistics.units[ i ].obselete ].disponibility ].researched 
					) &&
					Statistics.units[ i ].defense > Statistics.units[ i ].attack
					) 
				{ 
					unitPossible2[ q ] = i; 
					q++; 
				} 

	//		Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type = 1; 
	//		Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind = unitPossible2[ R.Next( q ) ]; 
			Form1.game.playerList[ player ].cityList[ city ].construction.setFirstAndOnly( Statistics.units[ unitPossible2[ R.Next( q ) ] ] );
			Form1.game.playerList[ player ].cityList[ city ].construction.points = 0; 
		}
	}
}
