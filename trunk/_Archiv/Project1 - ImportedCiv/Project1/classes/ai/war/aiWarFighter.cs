using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiWarFighter.
	/// </summary>
	public class aiWarFighter
	{
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

			structures.memory[] mem = Form1.game.playerList[ player ].memory.findAllSince( (byte)enums.playerMemory.beenBombed, 2 );

			for ( int j = 0; j < mem.Length; j ++ )
				for ( int i = 0; i < costs.Length; i ++ ) 
					if ( range >= Form1.game.radius.getDistWith( new Point( mem[ j ].ind[ 0 ], mem[ j ].ind[ 1 ] ), new Point( costs[ i ].X, costs[ i ].Y ) ) ) 
						values[ i ] += 1000; 

		/*	for ( int x = 0; x < Form1.game.width; x ++ ) 
				for ( int y = 0; y < Form1.game.height; y ++ ) 
					if ( Form1.game.playerList[ player ].discovered[ x, y ] ) 
					{ 
						int caseValue = aiWarBomber.caseValueToBombard( player, x, y ); 

						if ( caseValue > 0 ) 
							for ( int i = 0; i < costs.Length; i ++ ) 
								if ( range >= game.radius.getDistWith( new Point( x, y ), new Point( costs[ i ].X, costs[ i ].Y ) ) ) 
									values[ i ] += caseValue; 
					}*/

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
