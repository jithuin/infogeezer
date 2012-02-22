using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for singleSattelite.
	/// </summary>
	public class singleSateliteGroup
	{
		public singleSateliteGroup( int Length, int bottom, int top )
		{
			list = new structures.singleSattelite[ Length ];

			for ( int s = 0; s < Length; s ++ )
				list[ s ].path = new byte[ Form1.game.width ];

			Top = top;
			Bottom = bottom;
		}

		public structures.singleSattelite[] list;
		public int Top, Bottom; 

#region endTurn
		public void endTurn()
		{
			for ( int s = 0; s < list.Length; s++ )
			{
				double move = 10;

				while ( move > 0 )
				{
					if ( list[ s ].pos >= Form1.game.width - 1 )
					{
						list[ s ].lastY = list[ s ].path[ Form1.game.width - 1 ];

						byte y2 = list[ s ].path[ list[ s ].pos ];

						list[ s ].pos = 0;
						list[ s ].xStart += 2 * Math.PI * Form1.game.width / ( Bottom - Top ) / 2;
						list[ s ].xStart = list[ s ].xStart; // % (2 * Math.PI);

						if ( list[ s ].specialPath && list[ s ].nextPath != null )
						{
							list[ s ].path = list[ s ].nextPath;
							list[ s ].nextPath = null;
							list[ s ].specialPath = true;
						}
						else
							setNormalPath( s );

						byte y1 = list[ s ].path[ list[ s ].pos ];

						move -= Math.Sqrt( 1 + Math.Pow( y2 - y1, 2 ) );
					}
					else
					{
						move -= Math.Sqrt( 1 + Math.Pow( list[ s ].path[ list[ s ].pos + 1 ] - list[ s ].path[ list[ s ].pos ], 2 ) );
						list[ s ].pos++;
					}
				}

				
			}
		}
			
#endregion
		
#region set normal path
		public void setNormalPath( int s )
		{
			int amp = ( Bottom - Top ) / 2, 
			oriY = ( Bottom + Top ) / 2, 
			etendu = amp * 3 / 2;

			/*for ( int s = 0; s < list.Length; s++ )
			{*/
				list[ s ].specialPath = false;

				for ( int x = 0; x < Form1.game.width; x++ )
					list[ s ].path[ x ] = (byte)sinEquation( x, etendu, list[ s ].xStart, amp, oriY );
			//}
		}
			
		#endregion

#region change path
		public void changePath( int newTop, int newBottom )
		{
			int amp = ( Bottom - Top ) / 2, oriY = ( Bottom + Top ) / 2, etendu = amp * 3 / 2;
			int amp1 = ( newBottom - newTop ) / 2, oriY1 = ( newBottom + newTop ) / 2, etendu1 = amp1 * 3 / 2;
			int orbitChangeDist = 8;

			double angle0;

			for ( int s = 0; s < list.Length; s++ )
			{
				if ( list[ s ].pos == 0 )
					angle0 = Math.Atan( list[ s ].path[ list[ s ].pos ] - list[ s ].lastY );//sinEquation( list[ s ].pos, etendu, list[ s ].xStart, amp, oriY ) );
					else
					angle0 = Math.Atan( list[ s ].path[ list[ s ].pos ] - list[ s ].path[ list[ s ].pos - 1 ] );

				//	angle0 = 0;
				double searchedOriX = 0;

				if ( angle0 > 0 )
					if ( sinEquation( list[ s ].pos, etendu, list[ s ].xStart, amp, oriY ) > oriY1 )
						searchedOriX = Math.PI;
					else
						searchedOriX = 0;
				else if ( sinEquation( list[ s ].pos, etendu, list[ s ].xStart, amp, oriY ) > oriY1 )
					searchedOriX = Math.PI;
				else
					searchedOriX = 0;

				double startX1 = searchedOriX;
				double angle1 = Math.Atan( ( sinEquation( 0, etendu1, startX1, amp1, oriY1 ) - sinEquation( -1, etendu1, startX1, amp1, oriY1 ) ) / 1 );
				double angleDiff = angle1 - angle0;

				if ( angleDiff < 0 )
					angleDiff *= -1;

				if ( angleDiff > Math.PI )
					angleDiff = 2 * Math.PI - angleDiff;

				startX1 = searchedOriX - ( ( list[ s ].pos + orbitChangeDist ) * Math.PI * 2 ) / etendu1;

				// 3/3
				if ( list[ s ].pos + orbitChangeDist >= list[ s ].path.Length )
				{
					list[ s ].nextPath = new byte[ Form1.game.width ];
					for ( int x = list[ s ].pos + orbitChangeDist; x < list[ s ].path.Length + Form1.game.width; x++ )
						list[ s ].nextPath[ x - Form1.game.width ] = (byte)sinEquation( x, etendu1, startX1, amp1, oriY1 );
				}
				else
				{
					for ( int x = list[ s ].pos + orbitChangeDist; x < list[ s ].path.Length; x++ )
						list[ s ].path[ x ] = (byte)sinEquation( x, etendu1, startX1, amp1, oriY1 );
				}

				int handle0 = 12, handle1 = 12;
				System.Drawing.Point A = new System.Drawing.Point( list[ s ].pos, list[ s ].path[ list[ s ].pos ] ),//(int)sinEquation( list[ s ].pos, etendu, list[ s ].xStart, amp, oriY ) ), 
					B = new System.Drawing.Point( A.X + (int)( Math.Cos( angle0 ) * handle0 ), A.Y + (int)( Math.Sin( angle0 ) * handle0 ) ), C = new System.Drawing.Point( list[ s ].pos + orbitChangeDist - (int)( Math.Cos( angle1 ) * handle1 ), (byte)sinEquation( list[ s ].pos + orbitChangeDist, etendu1, startX1, amp1, oriY1 ) - (int)( Math.Sin( angle1 ) * handle1 ) ), D = new System.Drawing.Point( list[ s ].pos + orbitChangeDist, (int)sinEquation( list[ s ].pos + orbitChangeDist, etendu1, startX1, amp1, oriY1 ) );
				double t = 0.01;

				/// 2/3
				for ( int x = list[ s ].pos + 1; x < list[ s ].pos + orbitChangeDist; )
				{
					double xt = A.X * Math.Pow( 1 - t, 3 ) + 3 * B.X * t * Math.Pow( 1 - t, 2 ) + 3 * C.X * Math.Pow( t, 2 ) * ( 1 - t ) + D.X * Math.Pow( t, 3 );

					if ( xt > x )
					{
						if ( x >= list[ s ].path.Length )
						{//list[ s ].nextPath[ x - Form1.game.width ]
							int res = (byte)( A.Y * Math.Pow( 1 - t, 3 ) + 3 * B.Y * t * Math.Pow( 1 - t, 2 ) + 3 * C.Y * Math.Pow( t, 2 ) * ( 1 - t ) + D.Y * Math.Pow( t, 3 ) );
						
							if ( res < 0 )
								list[ s ].nextPath[ x - Form1.game.width ] = 0;
							else if ( res > Form1.game.height )
								list[ s ].nextPath[ x - Form1.game.width ] = (byte)Form1.game.height;
							else
								list[ s ].nextPath[ x - Form1.game.width ] = (byte)res;
						}
						else
						{//list[ s ].path[ x ]
							int res = (byte)( A.Y * Math.Pow( 1 - t, 3 ) + 3 * B.Y * t * Math.Pow( 1 - t, 2 ) + 3 * C.Y * Math.Pow( t, 2 ) * ( 1 - t ) + D.Y * Math.Pow( t, 3 ) );
						
							if ( res < 0 )
								list[ s ].path[ x ] = 0;
							else if ( res > Form1.game.height )
								list[ s ].path[ x ] = (byte)Form1.game.height;
							else
								list[ s ].path[ x ] = (byte)res;
						}

						x++;
					}
					else
						t += 0.01;
				}

				list[ s ].specialPath = true;
			}
		}
			
		#endregion

#region addSat
		public void addSat( int Pos )
		{
			/*int nbr;
			if ( list )*/
			structures.singleSattelite[] buffer = list;

			list = new structures.singleSattelite[ buffer.Length + 1 ];

			for ( int i = 0; i < buffer.Length; i++ )
				list[ i ] = buffer[ i ];

			Random r = new Random();

			list[ buffer.Length ].xStart = r.NextDouble() * 2 * Math.PI;
			list[ buffer.Length ].path = new byte[ Form1.game.width ];
			setNormalPath( buffer.Length );

			int amp = ( Bottom - Top ) / 2, 
			oriY = ( Bottom + Top ) / 2, 
			etendu = amp * 3 / 2;

			list[ buffer.Length ].lastY = (byte)sinEquation( -1, etendu, list[ buffer.Length ].xStart, amp, oriY );
			list[ buffer.Length ].pos = Pos;
		}
			
		#endregion

#region sin equation
		/// <summary>
		/// Sin( ( x * 6.283 ) / etendu + startX ) * amp + oriY
		/// </summary>
		/// <param name="x">Sin( ( ? * 6.283 ) / etendu + startX ) * amp + oriY</param>
		/// <param name="etendu">Sin( ( x * 6.283 ) / ? + startX ) * amp + oriY</param>
		/// <param name="startX">Sin( ( x * 6.283 ) / etendu + ? ) * amp + oriY</param>
		/// <param name="amp">Sin( ( x * 6.283 ) / etendu + startX ) * ? + oriY</param>
		/// <param name="oriY">Sin( ( x * 6.283 ) / etendu + startX ) * amp + ?</param>
		/// <returns></returns>
		private double sinEquation(int x, int etendu, double startX, int amp, int oriY)
		{
			double result = Math.Sin( ( x * Math.PI * 2 ) / etendu + startX ) * amp + oriY;

			if ( result >= Form1.game.height )
				return Form1.game.width;
			else if ( result < 0 )
				return 0;
			else
				return result;
		}
#endregion

	}
}
