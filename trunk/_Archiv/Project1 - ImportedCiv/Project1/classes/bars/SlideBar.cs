using System;
using System.Windows.Forms;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for SlideBar.
	/// </summary>
	public class SlideBar
	{
		public PictureBox picBox;
		public int red, yellow, green, ValueMax, Value, min, max, defaultValue, ValueMin;
		public bool enabled, showMax;
		static Pen blackPen = new Pen( Color.Black );
		static Pen grayPen = new Pen( Color.Gray );
		public string Text, Text2, maxInfo;
		public Color backColor;
		private bool mouseTapped;

		Graphics g;
		Bitmap aff;

		public SlideBar( int x, int y, int width, int height, int minV, int maxV )
		{
			backColor = Color.White;
			picBox = new PictureBox();
			picBox.Left = x;
			picBox.Top = y;
			picBox.Width = width;
			picBox.Height = height;
			ValueMin = minV;
			ValueMax = maxV;
			max = -1; 
			min = -1;

			red = ValueMin;
			yellow = ValueMin;
			green = ValueMin;

			picBox.MouseMove += new MouseEventHandler( picBox_MouseMove );
			picBox.MouseDown +=new MouseEventHandler(picBox_MouseDown);
			picBox.MouseUp += new MouseEventHandler(picBox_MouseUp);

			aff = new Bitmap( picBox.Width, picBox.Height );
			g = Graphics.FromImage( aff );

			//
			// TODO: Add constructor logic here
			//
		}

	#region drawSlider
		public void drawSlider()
		{
			int top = aff.Height / 3, height = aff.Height * 1 / 3;
			g.Clear( backColor ); //.FillRectangle( new SolidBrush( Color.White ), 0, 0, aff.Width, aff.Height );

		#region color bars
			if ( red > ValueMin )
			{
				Rectangle rRed = new Rectangle(
					8,
					top,
					( ( red - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ),
					height
					);

				g.FillRectangle( 
					new SolidBrush( Color.Red ), 
					rRed 
					);
			}
			if ( yellow > ValueMin )
			{
				int left;
				if ( red <= ValueMin )
					left = 0;
				else
					left = ( ( red - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ) + 8;

				Rectangle rYellow = new Rectangle(
					left, // ( ( red - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 8,
					top,
					( ( yellow - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ) - ( ( red - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ),
					height
					);

				g.FillRectangle( 
					new SolidBrush( Color.Yellow ), 
					rYellow 
					);
			}
			if ( green > ValueMin )
			{
				if ( yellow > 0 )
				{
					Rectangle rGreen = new Rectangle(
						( ( yellow - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ) + 8,
						top,
						( ( green - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ) - ( ( yellow - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ),
						height
						);

					g.FillRectangle( 
						new SolidBrush( Color.Green ), 
						rGreen 
						);
					}
				else
				{
					Rectangle rGreen = new Rectangle(
						( ( red - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ) + 8,
						top,
						( ( green - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ) - ( ( red - ValueMin ) * ( aff.Width - 16 ) ) / ( ( ValueMax - ValueMin ) != 0 ? ( ValueMax - ValueMin ) : 1 ),
						height
						);

					g.FillRectangle( 
						new SolidBrush( Color.Green ), 
						rGreen 
						);
				}
			}
			#endregion

		#region main lines
			g.DrawLine( 
				grayPen,
				8,
				1,
				8,
				aff.Height - 1
				);
			g.DrawLine( 
				grayPen, 
				8, 
				aff.Height / 2, 
				aff.Width - 8, 
				aff.Height / 2 
				);
			g.DrawLine( 
				grayPen,
				aff.Width - 7,
				1,
				aff.Width - 7,
				aff.Height - 1
				);

			Pen tempBlackPen;
			/*if ( enabled )*/
				tempBlackPen = blackPen;
		/*	else
				tempBlackPen = new Pen( Color.DarkGray );*/

			g.DrawLine( 
				tempBlackPen,
				7,
				0,
				7,
				aff.Height - 2
				);
			g.DrawLine( 
				tempBlackPen, 
				8 - 1, 
				aff.Height / 2 - 1, 
				aff.Width - 8 - 1, 
				aff.Height / 2 - 1 
				);
			g.DrawLine( 
				tempBlackPen,
				aff.Width - 8,
				0,
				aff.Width - 8,
				aff.Height - 2
				);
			#endregion

		#region line at 0
			if ( ValueMin != 0 )
			{
				// 0 - 2
				g.DrawLine( 
					blackPen,
					( ( 0 - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 8,
					0,
					( ( 0 - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 8,
					aff.Height - 2
					);
				g.DrawLine( 
					grayPen,
					( ( 0 - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					1,
					( ( 0 - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height / 2 - 2
					);
				g.DrawLine( 
					grayPen,
					( ( 0 - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height / 2,
					( ( 0 - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height - 1
					);
			}
			#endregion

		#region default value
			if ( defaultValue != 0 )
			{
				g.DrawLine( 
					blackPen,
					( ( defaultValue - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 8,
					aff.Height / 4 - 1,
					( ( defaultValue - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 8,
					aff.Height * 3 / 4 - 1
					);
				g.DrawLine( 
					grayPen,
					( ( defaultValue - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height / 4 + 1 - 1,
					( ( defaultValue - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height / 2 - 2
					);
				g.DrawLine( 
					grayPen,
					( ( defaultValue - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height / 2,
					( ( defaultValue - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 9,
					aff.Height * 3 / 4 + 1 - 1
					);
			}
			#endregion

		#region text
			if ( Text != "" )
			{
				Font fntText = new Font( "Tahoma", 10, FontStyle.Regular );
				g.DrawString(
					Text,
					fntText,
					new SolidBrush( Color.White ),
					11,
					-3
					);
				g.DrawString(
					Text,
					fntText,
					new SolidBrush( Color.DarkGray ),
					13,
					-3
					);
				g.DrawString(
					Text,
					fntText,
					new SolidBrush( Color.Black ),
					12,
					-4
					);
			}
			#endregion

		#region text2
			if ( Text2 != "" )
			{
				Font fntText = new Font( "Tahoma", 8, FontStyle.Regular );
				SizeF sizeText = g.MeasureString( Text2, fntText );

				g.DrawString(
					Text2,
					fntText,
					new SolidBrush( Color.White ),
					aff.Width - sizeText.Width - 10,
					aff.Height - sizeText.Height + 1
					);
				g.DrawString(
					Text2,
					fntText,
					new SolidBrush( Color.DarkGray ),
					aff.Width - sizeText.Width - 8,
					aff.Height - sizeText.Height + 1
					);
				g.DrawString(
					Text2,
					fntText,
					new SolidBrush( Color.Black ),
					aff.Width - sizeText.Width - 9,
					aff.Height - sizeText.Height
					);
			}
			#endregion

		#region showMax
			if ( showMax )
			{
				Font fntText = new Font( "Tahoma", 7, FontStyle.Regular );
				SizeF sizeText = g.MeasureString( ValueMax.ToString() + maxInfo, fntText );

				g.DrawString(
					ValueMax.ToString() + maxInfo, 
					fntText, 
					new SolidBrush( Color.White ), 
					aff.Width - sizeText.Width - 10, 
					-3 + 2
					);
				g.DrawString(
					ValueMax.ToString() + maxInfo,
					fntText,
					new SolidBrush( Color.DarkGray ),
					aff.Width - sizeText.Width - 8,
					-3 + 2
					);
				g.DrawString(
					ValueMax.ToString() + maxInfo,
					fntText,
					new SolidBrush( Color.Black ),
					aff.Width - sizeText.Width - 9,
					-4 + 2
					);
				
			//	fntText = new Font( "Tahoma", 10, FontStyle.Regular );
				sizeText = g.MeasureString( ValueMin.ToString() + maxInfo, fntText );

				g.DrawString(
					ValueMin.ToString() + maxInfo,
					fntText,
					new SolidBrush( Color.White ),
					11,
					aff.Height - sizeText.Height + 1
					);
				g.DrawString(
					ValueMin.ToString() + maxInfo,
					fntText,
					new SolidBrush( Color.DarkGray ),
					13,
					aff.Height - sizeText.Height + 1
					);
				g.DrawString(
					ValueMin.ToString() + maxInfo,
					fntText,
					new SolidBrush( Color.Black ),
					12,
					aff.Height - sizeText.Height
					);
			}
		#endregion

		#region ellipse

			int sliderWidth = 6 * platformSpec.resolution.mod;

			Rectangle r = new Rectangle(
				( ( Value - ValueMin ) * ( aff.Width - 16 ) ) / ( ValueMax - ValueMin ) + 8 - sliderWidth / 2,
				0,
				sliderWidth,
				aff.Height - 1
				);

			if ( enabled )
			{
				if ( Value <= red && red > ValueMin )
				{
					g.FillEllipse( 
						new SolidBrush( Color.Red ), 
						r 
						);
				}
				else if ( Value <= yellow && yellow > ValueMin )
				{
					g.FillEllipse( 
						new SolidBrush( Color.Yellow ), 
						r 
						);
				}
				else if ( Value <= green && green > ValueMin )
				{
					g.FillEllipse( 
						new SolidBrush( Color.Green ), 
						r 
						);
				}
				else
				{
					g.FillEllipse( 
						new SolidBrush( Color.White ), 
						r 
						);
				}
			}
			else
			{
				g.FillEllipse( 
					new SolidBrush( Color.DimGray ), 
					r 
					);
			}

			g.DrawEllipse( 
				new Pen( Color.Black ), 
				r 
				);

			#endregion

			picBox.Image = aff;
		}
	#endregion

	#region picBox_MouseMove
		private void picBox_MouseMove(object sender, MouseEventArgs e)
		{
			if ( enabled && mouseTapped )
			{
				Point pntClick = picBox.PointToClient( Form1.MousePosition );

				int result = ( pntClick.X - 8 ) * ValueMax / ( aff.Width - 16 );

				if ( result > 0 || result < ValueMax )
				{
					if ( result > Value )
						while ( Value < result )
						{
							if ( Value < ValueMax && 
								( Value < max || max == -1 ) //
								)
								Value ++;

							else break;
						}
					else
						while ( Value > result )
						{
							if ( Value > 0 && Value > min )
								Value --;

							else break;
						}
				}
				drawSlider();
			}
		}
#endregion

		private void picBox_MouseDown(object sender, MouseEventArgs e)
		{
			mouseTapped = true;
		}

		private void picBox_MouseUp(object sender, MouseEventArgs e)
		{
			mouseTapped = false;
		}
	}
}
