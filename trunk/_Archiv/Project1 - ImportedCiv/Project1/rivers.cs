using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for plans.
	/// </summary>
	public class rivers
	{
		/// <summary> 
		/// 
		/// </summary> 
		/// <param name="g"></param> 
		/// <param name="ori"> x, y</param> 
		/// <param name="r">std r</param> 
		public static void drawCase( Graphics g, Point ori, Rectangle r ) 
		{ 
			int seen;
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ ori.X, ori.Y ] )
				seen = 0;
			else
				seen = 1;

			for ( int i = 0; i < Form1.game.grid[ ori.X, ori.Y ].riversDir.Length; i ++ )
				if ( Form1.game.grid[ ori.X, ori.Y ].riversDir[ i ] ) 
					g.DrawImage( 
						Form1.riverBmp[ i ][ seen ], 
						r, 
						0, 
						0, 
						Form1.riverBmp[ i ][ seen ].Width, 
						Form1.riverBmp[ i ][ seen ].Height, 
						GraphicsUnit.Pixel, 
						Form1.ia
						);
		} 
	}
}
