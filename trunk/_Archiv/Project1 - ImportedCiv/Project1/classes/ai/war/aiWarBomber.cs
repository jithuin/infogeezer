using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiWarBomber.
	/// </summary>
	public class aiWarBomber
	{
		#region findCaseToBombard
		public static Point findCaseToBombard( byte player, int unit )
		{
			return findCaseToBombard( 
				player,
				new Point(
				Form1.game.playerList[ player ].unitList[ unit ].X, 
				Form1.game.playerList[ player ].unitList[ unit ].Y ), 
				Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].range
				);
		}

		public static Point findCaseToBombard( byte player, Point pos, int range ) 
		{ 
			Point[] sqr = Form1.game.radius.returnSquare( pos, range ); 
 
			int[] caseValues = new int[ sqr.Length ]; 
 /*
			byte[] ennemies = game.radius.relationTypeListEnnemies;//returnRelationTypeList(	true,	false,	false,	false,	false,	false	); 
			byte[] allies = game.radius.relationTypeListAllies; //.returnRelationTypeList(	false,	false,	false,	false,	false,	false	); 
 */
			for ( int i = 0; i < sqr.Length; i ++ ) 
			{ 
				caseValues[ i ] = caseValueToBombard( player, sqr[ i ].X, sqr[ i ].Y );
				/*if ( Form1.game.playerList[ player ].discovered[ sqr[ i ].X, sqr[ i ].Y ] )
				{
					if ( Form1.game.playerList[ player ].see[ sqr[ i ].X, sqr[ i ].Y ] ) 
						if ( game.radius.caseOccupiedByRelationType( sqr[ i ].X, sqr[ i ].Y, player, ennemy ) ) 
							caseValues[ i ] += 10; 
							
					if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].continent == 0 )
					{
					}
					else if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 == player )
					{
						if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].civicImprovement > 0 )
							caseValues[ i ] -= 3;

						if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].militaryImprovement > 0 )
							caseValues[ i ] -= 3;
						
						caseValues[ i ] += 3;
					}
					else if ( 
						Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory > 0 &&
						Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
						)
					{
						if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].civicImprovement > 0 )
							caseValues[ i ] += 3;

						if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].militaryImprovement > 0 )
							caseValues[ i ] += 3;
					}
					else
						caseValues[ i ] -= 2;

					if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].city > 0 ) 
					{ 
						if ( Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war )
							caseValues[ i ] += 8;
						else
							caseValues[ i ] -= 8;
					} 
				}
				else
				{
					caseValues[ i ] = -1;
				}*/
			} 

			int[] ordered = count.descOrder( caseValues ); 

			if ( ordered.Length > 0 && caseValues[ ordered[ 0 ] ] > 0 ) 
				return sqr[ ordered[ 0 ] ]; 
			else 
				return new Point( -1, -1 ); 
		} 
		#endregion
 
		#region caseValueToBombard
		public static int caseValueToBombard( byte player, int x, int y ) 
		{
			int caseValue = 0;
			if ( Form1.game.playerList[ player ].discovered[ x, y ] )
			{
				if ( Form1.game.playerList[ player ].see[ x, y ] ) 
					for ( int unit = 0; unit < Form1.game.grid[ x, y ].stack.Length; unit ++ )
						if ( Form1.game.grid[ x, y ].stack[ unit ].player.player == player )
							caseValue -= 20; 
						else if ( Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].stack[ unit ].player.player ].politic == (byte)Form1.relationPolType.war )
							caseValue += 10; 
						else if ( 
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].stack[ unit ].player.player ].politic == (byte)Form1.relationPolType.alliance ||
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].stack[ unit ].player.player ].politic == (byte)Form1.relationPolType.Protected ||
							Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].stack[ unit ].player.player ].politic == (byte)Form1.relationPolType.Protector
							) 
							caseValue -= 10; 
						else // peace, ceasefire 
							caseValue -= 5; 


					//if ( game.radius.caseOccupiedByRelationType( x, y, player, ennemies ) ) 
						//caseValue += 5; 

				if ( Form1.game.grid[ x, y ].continent == 0 )
				{
				}
				else if ( Form1.game.grid[ x, y ].territory - 1 == player )
				{
					if ( Form1.game.grid[ x, y ].civicImprovement > 0 )
						caseValue -= 3;

					if ( Form1.game.grid[ x, y ].militaryImprovement > 0 )
						caseValue -= 3;
						
					//caseValue += 3;
				}
				else if ( 
					Form1.game.grid[ x, y ].territory > 0 &&
					Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war 
					)
				{
					if ( Form1.game.grid[ x, y ].civicImprovement > 0 )
						caseValue += 3;

					if ( Form1.game.grid[ x, y ].militaryImprovement > 0 )
						caseValue += 3;
				}
				else
					caseValue -= 2;

				if ( Form1.game.grid[ x, y ].city > 0 ) 
				{ 
					if ( Form1.game.playerList[ player ].foreignRelation[ Form1.game.grid[ x, y ].territory - 1 ].politic == (byte)Form1.relationPolType.war )
						caseValue += 8;
					else
						caseValue -= 8;
				} 
			}
			else
				caseValue = -1;

			return caseValue;
		}
		#endregion
 
		#region findWhereToRebase
		public static Point findWhereToRebase( byte player, int unit ) 
		{
			return findWhereToRebase( player, Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].range, Form1.game.playerList[ player ].unitList[ unit ].pos );
		}
 
		public static Point findWhereToRebase( byte player, int range, Point ori ) 
		{ 
			structures.pntWithDist[] costs = aiPlanes.costList( player, range, ori ); 
			int[] values = new int[ costs.Length ];
			byte[] ennemies = Form1.game.radius.relationTypeListEnnemies;//.returnRelationTypeList(	true,	false,	false,	false,	false,	false	); 

			for ( int x = 0; x < Form1.game.width; x ++ ) 
				for ( int y = 0; y < Form1.game.height; y ++ ) 
					if ( Form1.game.playerList[ player ].discovered[ x, y ] ) 
					{ 
						int caseValue = caseValueToBombard( player, x, y ); 

						if ( caseValue > 0 ) 
							for ( int i = 0; i < costs.Length; i ++ ) 
								if ( range >= Form1.game.radius.getDistWith( new Point( x, y ), new Point( costs[ i ].X, costs[ i ].Y ) ) ) 
									values[ i ] += caseValue; 
					}

			for ( int i = 0; i < costs.Length; i ++ ) 
				if ( values[ i ] > 0 ) 
					values[ i ] += 100 - costs[ i ].dist; 

			int[] order = count.descOrder( values ); 

			if ( values[ order[ 0 ] ] > 0 ) 
				return new Point( costs[ order[ 0 ] ].X, costs[ order[ 0 ] ].Y ); 
			else 
				return new Point( -1, -1 ); 
		} 
		#endregion
	} 
} 
 