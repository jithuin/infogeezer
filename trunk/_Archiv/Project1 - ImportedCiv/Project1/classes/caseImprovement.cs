using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for caseImprovement.
	/// </summary>
	public class caseImprovement
	{

#region addCaseImps
		/// <summary>
		/// Add unit, improvement to shared current imp list
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="type">0 = none, 1 = civic, 2 = military, 3 = remove</param>
		/// <param name="construction"></param>
		/// <param name="owner"></param>
		/// <param name="unit"></param>
		public static void addCaseImps( Point pos, byte type, byte construction, byte owner, int unit )
		{

			#region Already Exist
			bool alreadyExist = false;
			for ( int i = 0; i < Form1.game.caseImps.Length; i ++ )
				if ( 
					Form1.game.caseImps[ i ].pos == pos && 
					Form1.game.caseImps[ i ].type == type &&
					Form1.game.caseImps[ i ].owner == owner
					)
				{
					UnitList[] buffer = Form1.game.caseImps[ i ].units;
					Form1.game.caseImps[ i ].units = new UnitList[ buffer.Length + 1 ];

					for ( int j = 0; j < buffer.Length; j ++ )
						Form1.game.caseImps[ i ].units[ j ] = buffer[ j ];

					Form1.game.caseImps[ i ].units[ buffer.Length ] = Form1.game.playerList[ owner ].unitList[ unit ];
						
						/*.unit = unit;
					Form1.game.caseImps[ i ].units[ buffer.Length ].owner = owner;*/

					alreadyExist = true;
					break;
				}
			#endregion

			#region create new 
			if ( !alreadyExist )
			{
				structures.caseImprovement[] buffer = Form1.game.caseImps;
				Form1.game.caseImps = new structures.caseImprovement[ buffer.Length + 1 ];

				for ( int i = 0; i < buffer.Length; i ++ )
					Form1.game.caseImps[ i ] = buffer[ i ];

				Form1.game.caseImps[ buffer.Length ].construction = construction;
				Form1.game.caseImps[ buffer.Length ].constructionPntLeft = Statistics.terrains[ Form1.game.grid[ pos.X, pos.Y ].type ].difficulty * 3;
				Form1.game.caseImps[ buffer.Length ].pos = pos;
				Form1.game.caseImps[ buffer.Length ].type = type;
				Form1.game.caseImps[ buffer.Length ].owner = owner;
				
				Form1.game.caseImps[ buffer.Length ].units = new UnitList[ 1 ];
			/*	Form1.game.caseImps[ buffer.Length ].units[ 0 ].owner = owner;
				Form1.game.caseImps[ buffer.Length ].units[ 0 ].unit = unit;	*/

				Form1.game.caseImps[ buffer.Length ].units[ 0 ] = Form1.game.playerList[ owner ].unitList[ unit ];
			}
			#endregion

		}
		#endregion

#region destroyCaseImps
		public static void destroyCaseImps( int nbr ) // ( Point pos, byte type )
		{
			for ( int i = 0; i < Form1.game.caseImps[ nbr ].units.Length; i ++ )
			{
				if ( !Form1.game.caseImps[ nbr ].units[ i ].dead )
					Form1.game.caseImps[ nbr ].units[ i ].state = (byte)Form1.unitState.idle;
			}

			structures.caseImprovement[] buffer = Form1.game.caseImps;
			Form1.game.caseImps = new structures.caseImprovement[ buffer.Length - 1 ];

			int j = 0;
			for ( int i = 0; i < nbr; i ++ )
			{
				Form1.game.caseImps[ j ] = buffer[ i ];
				j ++;
			}
			for ( int i = nbr + 1; i < buffer.Length; i ++ )
			{
				Form1.game.caseImps[ j ] = buffer[ i ];
				j ++;
			}
		}
		#endregion

#region removeUnitFromCaseImps
		public static void removeUnitFromCaseImps( byte owner, int unit ) // ( Point pos, byte type, int unit )
		{
			bool found = false;
			int nbr = 10000, order = 10000;
			for ( int i = 0; i < Form1.game.caseImps.Length && !found; i ++ )
				if ( Form1.game.caseImps[ i ].owner == owner )
					for ( int j = 0; j < Form1.game.caseImps[ i ].units.Length && !found; j ++ )
						if ( Form1.game.caseImps[ i ].units[ j ] == Form1.game.playerList[ owner ].unitList[ unit ] )
						{
							nbr = i;
							order = j;
							found = true;
					//		break;
						}

			// test 
	//		Game game = Form1.game;

			if ( found )
			{
				UnitList[] buffer = Form1.game.caseImps[ nbr ].units;
				Form1.game.caseImps[ nbr ].units = new UnitList[ buffer.Length - 1 ];

				if ( Form1.game.playerList[ owner ].unitList[ unit ].moveLeft > 0 || Form1.game.playerList[ owner ].unitList[ unit ].moveLeftFraction > 0 )
					Form1.game.playerList[ owner ].unitList[ unit ].state = (byte)Form1.unitState.idle;

				if ( Form1.game.caseImps[ nbr ].units.Length < 1 )
				{
					destroyCaseImps( nbr );
				}
				else
				{
					int j = 0;
					for ( int i = 0; i < order; i ++ )
					{
						Form1.game.caseImps[ nbr ].units[ j ] = buffer[ i ];
						j ++;
					}
					for ( int i = order + 1; i < buffer.Length; i ++ )
					{
						Form1.game.caseImps[ nbr ].units[ j ] = buffer[ i ];
						j ++;
					}
				}
			}

		}
		#endregion

#region getTurnLeft
		public static int getTurnLeft( int ind, bool plusOne, byte player, int unit )
		{
			int moveLeft = 0;
			for ( int j = 0; j < Form1.game.caseImps[ ind ].units.Length; j ++ )
			{
				moveLeft += 
					Form1.game.caseImps[ ind ].units[ j ].moveLeft * 3 + 
					Form1.game.caseImps[ ind ].units[ j ].moveLeftFraction;
			}

			if ( plusOne )
			{
				moveLeft += 
					Form1.game.playerList[ player ].unitList[ unit ].moveLeft * 3 + 
					Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction;
			}

			if ( 
			//	moveLeft != 0 && // not sure
				Form1.game.caseImps[ ind ].constructionPntLeft / moveLeft > 1 
				)
			{
				int unitNbr = Form1.game.caseImps[ ind ].units.Length;

				if ( plusOne )
					unitNbr ++;

				return ( Form1.game.caseImps[ ind ].constructionPntLeft - moveLeft / ( ( unitNbr + 1 ) * 3 ) );
			}
			else
			{
				return Form1.game.caseImps[ ind ].constructionPntLeft / moveLeft;//Form1.game.caseImps[ ind ].units.Length + 1 );
			}
		}
#endregion

#region getTurnToBuild
		/// <summary>
		/// getTurnToBuild
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="type">0 = none, 1 = civic, 2 = military</param>
		/// <param name="construction"></param>
		public static int getTurnToBuild( Point pos, byte type, int construction, byte owner, int unit )
		{
			for ( int i = 0; i < Form1.game.caseImps.Length; i ++ )
				if ( Form1.game.caseImps[ i ].pos == pos && Form1.game.caseImps[ i ].type == type)
				{
					return getTurnLeft( i, true, owner, unit );
					//break;
				}

			int moveLeft = 
					Form1.game.playerList[ owner ].unitList[ unit ].moveLeft * 3 + 
					Form1.game.playerList[ owner ].unitList[ unit ].moveLeftFraction;
			

			if ( Statistics.terrains[ Form1.game.grid[ pos.X, pos.Y ].type ].difficulty * 3 / moveLeft > 1 )
			{
				//int unitNbr = Form1.game.caseImps[ ind ].units.Length;

				/*if ( plusOne )
					unitNbr ++;*/

				return ( ( Statistics.terrains[ Form1.game.grid[ pos.X, pos.Y ].type ].difficulty * 3 - moveLeft ) / 3 ) + 1;
			}
			else
			{
				return Statistics.terrains[ Form1.game.grid[ pos.X, pos.Y ].type ].difficulty * 3 / moveLeft;//Form1.game.caseImps[ ind ].units.Length + 1 );
			}
		}
#endregion

#region advImprovement
		public static void advImprovement( byte owner, int unit )
		{
			advImprovement( Form1.game.playerList[ owner ].unitList[ unit ] );
		}
		public static void advImprovement( UnitList unit )
		{
			for ( int i = 0; i < Form1.game.caseImps.Length; i ++ )
				if ( Form1.game.caseImps[ i ].pos == unit.pos )
					for ( int j = 0; j < Form1.game.caseImps[ i ].units.Length; j ++ )
						if ( Form1.game.caseImps[ i ].units[ j ] == unit )
						{
							Form1.game.caseImps[ i ].constructionPntLeft -= unit.moveLeft * 3 + unit.moveLeftFraction;
							if ( Form1.game.caseImps[ i ].constructionPntLeft <= 0 )
							{
								switch ( Form1.game.caseImps[ i ].type )
								{
									case 0:
										Form1.game.grid[ Form1.game.caseImps[ i ].pos.X, Form1.game.caseImps[ i ].pos.Y ].roadLevel =  Form1.game.caseImps[ i ].construction;
										roads.setAround( Form1.game, Form1.game.caseImps[ i ].pos );
										break;

									case 1:
										Form1.game.grid[ Form1.game.caseImps[ i ].pos.X, Form1.game.caseImps[ i ].pos.Y ].civicImprovement =  Form1.game.caseImps[ i ].construction;
										break;

									case 2:
										Form1.game.grid[ Form1.game.caseImps[ i ].pos.X, Form1.game.caseImps[ i ].pos.Y ].militaryImprovement =  Form1.game.caseImps[ i ].construction;
										break;
								}
								
								destroyCaseImps( i );
							}
							break;
						}
		}
#endregion

#region getTurnLeftStringPar
		public static string getTurnLeftStringPar( Point pos, byte type, int construction, byte owner, int unit )
		{
			int turnsLeft = getTurnToBuild( new Point( pos.X, pos.Y ), 0, 0, owner, unit );
			string turn;
			if ( turnsLeft > 1 )
			{
				turn = " turns";
			}
			else
			{
				turn = " turn";
			}

			return "( " + Convert.ToString( turnsLeft , 10 ) + turn + " )";
		}
		#endregion
		
#region getTurnLeftString
		public static string getTurnLeftString( byte owner, int unit )
		{
			return getTurnLeftString( Form1.game.playerList[ owner ].unitList[ unit ] );
		}

		public static string getTurnLeftString( UnitList unit )
		{
			bool ok = true;
			int ind = 1000;

			for ( int i = 0; i < Form1.game.caseImps.Length && ok; i ++ )
				if ( Form1.game.caseImps[ i ].owner == unit.player.player )
					for ( int j = 0; j < Form1.game.caseImps[ i ].units.Length; j ++ )
						if ( Form1.game.caseImps[ i ].units[ j ] == unit )
						{
							ind = i;
							ok = false;
							break;
						}
			
			int turnsLeft = getTurnLeft( ind, false, 100, 0 );
			string turn;
			if ( turnsLeft > 1 )
			{
				turn = " turns";
			}
			else
			{
				turn = " turn";
			}
			

			return Convert.ToString( turnsLeft , 10 ) + turn;
		}
#endregion

#region canBuildImprovement
		public static bool canBuildImprovement ( Point pos, byte type, int construction, byte player )
		{
			for ( int i = 0; i < Form1.game.caseImps.Length; i ++ )
				if ( Form1.game.caseImps[ i ].pos == pos )
					if ( Form1.game.caseImps[ i ].type == type )
					{ 
						if ( Form1.game.caseImps[ i ].construction != construction || Form1.game.caseImps[ i ].owner != player )
							return false;
						else	// assuming there s no invalid entry in game.caseImps
							return true;
					}

			return true;
		}
#endregion

#region killAllImprovementsAt
		public static void killAllImprovementsAt( Point pos )
		{
			for ( int i = 0; i < Form1.game.caseImps.Length; i ++ )
				if ( Form1.game.caseImps[ i ].pos == pos )
				{
					destroyCaseImps( i );
				}
		}
#endregion

	}
}
