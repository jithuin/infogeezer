using System;
using System.Drawing;

namespace xycv_ppc//.draw
{
	/// <summary>
	/// Summary description for map.
	/// </summary>
	public class drawMap
	{
		public static Bitmap[] getBmps( int Width, int Height )
		{
			Bitmap[] bmp = new Bitmap[ 2 ]; 

			for ( byte i = 0; i < bmp.Length; i ++ )
				drawPoints( ref bmp[ i ], i, Width, Height ); 

			return bmp; 
		}

		public static Bitmap getBmp(byte miniMapType, int Width, int Height)
		{
			Bitmap bmp = new Bitmap( 1, 1 );
			drawPoints( ref bmp, miniMapType, Width, Height );
			return bmp;
		}

		public static void drawPoints(ref Bitmap miniMap, byte miniMapType, int Width, int Height)
		{
			int mmw = Width - 2;
			int mmh = Height - 2;
			miniMap = new Bitmap( Width, Height );

			if ( miniMapType == 0 )
			{ // terrain type
				for ( int y = 0; y < mmh; y++ )
					for ( int x = 0; x < mmw; x++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ] )
							miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].color );

				for ( int p = 0; p < Form1.game.playerList.Length; p++ )
					for ( int c = 1; c <= Form1.game.playerList[ p ].cityNumber; c++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ Form1.game.playerList[ p ].cityList[ c ].X, Form1.game.playerList[ p ].cityList[ c ].Y ] )
						{
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Statistics.civilizations[ Form1.game.playerList[ p ].civType ].color );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
						}
			}
			else
			{ // borders
			//	miniMap = new Bitmap( pictureBox2.Width, pictureBox2.Height );
				for ( int y = 0; y < mmh; y++ )
					for ( int x = 0; x < mmw; x++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ] )
						{
							if ( Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].ew == 0 || Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].territory == 0 ) // si il y a de l eau
								miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].color );
							else
								miniMap.SetPixel( x + 1, y + 1, Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].territory - 1 ].civType ].color );
						}

				for ( int p = 0; p < Form1.game.playerList.Length; p++ )
					for ( int c = 1; c <= Form1.game.playerList[ p ].cityNumber; c++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ Form1.game.playerList[ p ].cityList[ c ].X, Form1.game.playerList[ p ].cityList[ c ].Y ] )
						{
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.White );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
						}
			}

			double miniMapAdjX = (double)Form1.game.width / (double)Width;
			double miniMapAdjY = (double)Form1.game.height / (double)Height;
		}

		public static void drawLine( ref Bitmap miniMap, byte miniMapType, int Width, int Height)
		{
			int mmw = Width - 2;
			int mmh = Height - 2;

			miniMap = new Bitmap( Width, Height );
			Graphics g = Graphics.FromImage( miniMap );
			Double diff = (double)mmw / (double)Form1.game.width;

			if ( miniMapType == 0 )
			{ // terrain type
				
				for ( int y = 0; y < Form1.game.width; y++ )
					for ( int x = 0; x < Form1.game.height; x++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ x, y ] )
							g.FillRectangle(
								new SolidBrush( Statistics.terrains[ Form1.game.grid[ x, y ].type ].color ), 
								(int)(diff * x), 
								(int)(diff * y),
								(int)(diff * ( x + 1 )), 
								(int)(diff * ( y + 1 ))
								);
				//miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].color );

			/*	
				for ( int y = 0; y < mmh; y++ )
					for ( int x = 0; x < mmw; x++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ] )
							g.DrawLine( new Pen( Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].color ), x + 1, y + 1, x + 1, y + 1 );//miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].color );
*/
			/*	for ( int p = 0; p < Form1.game.playerList.Length; p++ )
					for ( int c = 1; c <= Form1.game.playerList[ p ].cityNumber; c++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ Form1.game.playerList[ p ].cityList[ c ].X, Form1.game.playerList[ p ].cityList[ c ].Y ] )
						{
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Statistics.civilizations[ Form1.game.playerList[ p ].civType ].color );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
						}*/
			}
			else
			{ // borders
				//	miniMap = new Bitmap( pictureBox2.Width, pictureBox2.Height );
				for ( int y = 0; y < mmh; y++ )
					for ( int x = 0; x < mmw; x++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ] )
						{
							if ( Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].ew == 0 || Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].territory == 0 ) // si il y a de l eau
								miniMap.SetPixel( x + 1, y + 1, Statistics.terrains[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].type ].color );
							else
								miniMap.SetPixel( x + 1, y + 1, Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ x * Form1.game.width / mmw, y * Form1.game.height / mmh ].territory - 1 ].civType ].color );
						}

				for ( int p = 0; p < Form1.game.playerList.Length; p++ )
					for ( int c = 1; c <= Form1.game.playerList[ p ].cityNumber; c++ )
						if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ Form1.game.playerList[ p ].cityList[ c ].X, Form1.game.playerList[ p ].cityList[ c ].Y ] )
						{
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.White );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 - 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1 - 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
							miniMap.SetPixel( Form1.game.playerList[ p ].cityList[ c ].X * mmw / Form1.game.width + 1, Form1.game.playerList[ p ].cityList[ c ].Y * mmh / Form1.game.height + 1 + 1, Color.Black );
						}
			}

			double miniMapAdjX = (double)Form1.game.width / (double)Width;
			double miniMapAdjY = (double)Form1.game.height / (double)Height;
		}
	}
}
