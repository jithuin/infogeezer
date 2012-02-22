using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for rvtbc.
	/// </summary>
	public class Rvtbc
	{
		Game game;

		public Rvtbc( Game game )
		{
			this.game = game;
			
			Form1.regionValidToBuildCity = new System.Collections.BitArray[ game.width ];
			for ( int x = 0; x < game.width; x ++ )
				Form1.regionValidToBuildCity[ x ] = new System.Collections.BitArray( game.height, true );
		}

	/*	public static void init()
		{
			Form1.regionValidToBuildCity = new System.Collections.BitArray[ game.width ];
			for ( int x = 0; x < game.width; x ++ )
				Form1.regionValidToBuildCity[ x ] = new System.Collections.BitArray( game.height, true );
		}*/
		
		public void doit()
		{
			for ( int x = 0; x < game.width; x ++ )
				Form1.regionValidToBuildCity[ x ].SetAll( true );

			for ( int player = 0; player < game.playerList.Length; player ++ )
				if ( !game.playerList[ player ].dead )
					for ( int city = 1; city <= game.playerList[ player ].cityNumber; city ++ )
						if ( game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
						{
							Point[] rifc = game.radius.regionInvalidForCity( new Point( game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y ) );

							for ( int k = 0; k < rifc.Length; k ++ )
								Form1.regionValidToBuildCity[ rifc[ k ].X ].Set( rifc[ k ].Y, false );
						}
			/*	for ( int y = 0; y < game.height; y ++ )
				{
				}*/
		}
	}
}
