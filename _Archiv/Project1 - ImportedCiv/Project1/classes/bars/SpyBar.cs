using System;
using System.Windows.Forms;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for SlideBar.
	/// </summary>
	public class SpyBar
	{
		public PictureBox picBox;
		public int spyNbr, maxSpies/*, x, y*/, selected, lastSelected;
		public bool enabled, showMax;
		public Color backColor;
		public int Height;
		string empty;

		Bitmap spiesBack, aff;

		public SpyBar( int X, int Y, int width ) // , int height 
		{ 
			picBox = new PictureBox(); 
			picBox.Left = X; 
			picBox.Top = Y;
			picBox.Width = width; 
			picBox.Height = Form1.spyBmp.Height;
			Height = Form1.spyBmp.Height; 

			maxSpies = 50; 
			backColor = Color.White; 

			picBox.MouseMove += new MouseEventHandler(picBox_MouseMove); 
			picBox.MouseDown += new MouseEventHandler(picBox_MouseDown);

			empty = "empty";
 
			spiesBack = new Bitmap( picBox.Width, picBox.Height ); 
			aff = new Bitmap( picBox.Width, picBox.Height ); 
/* 
			drawSpies(); 
			drawAff(); 
*/ 
			// 
			// TODO: Add constructor logic here 
			// 
		} 

		public int x
		{
			get
			{
				return picBox.Left;
			}
			set
			{
				picBox.Left = value;
			}
		}
		public int y
		{
			get
			{
				return picBox.Top;
			}
			set
			{
				picBox.Top = value;
			}
		}

#region drawSpies
		public void drawSpies() 
		{ 
			Graphics g = Graphics.FromImage( spiesBack ); 
			g.Clear( backColor );

			if ( spyNbr <= maxSpies && spyNbr > 0 ) 
			{ 
				float diff1 = diff; 
				for ( int i = spyNbr - 1; i >= 0  ; i -- ) 
				{ 
					g.DrawImage( 
						Form1.spyBmp, 
						new Rectangle( 
						(int)( 8 + diff1 * i ),
						0, 
						Form1.spyBmp.Width, 
						Form1.spyBmp.Height 
						), 
						0, 
						0, 
						Form1.spyBmp.Width, 
						Form1.spyBmp.Height, 
						GraphicsUnit.Pixel, 
						Form1.ia 
						); 
				} 
			} 
			else if ( spyNbr <= 0 )
			{
				spyNbr = 0;

				SizeF sEmpty = g.MeasureString(
					empty,
					new Font( "Tahoma", 10, FontStyle.Regular )
					);

				g.DrawString( empty, new Font( "Tahoma", 10, FontStyle.Regular ), new SolidBrush( Color.Black ), spiesBack.Width / 2 - sEmpty.Width / 2, spiesBack.Height / 2 - sEmpty.Height / 2 );
			}
			else
			{
				int diff1 = ( picBox.Width - Form1.spyBmp.Width ) / ( 50 - 1 );
				for ( int i = 50 - 1; i >= 0  ; i -- ) 
				{ 
					g.DrawImage( 
						Form1.spyBmp, 
						new Rectangle( 
						8 + diff1 * i, 
						0, 
						Form1.spyBmp.Width, 
						Form1.spyBmp.Height 
						), 
						0, 
						0, 
						Form1.spyBmp.Width, 
						Form1.spyBmp.Height, 
						GraphicsUnit.Pixel, 
						Form1.ia 
						); 
				} 
			}
		} 
	#endregion

#region drawAff
		public void drawAff()
		{
			Graphics g = Graphics.FromImage( aff ); 
			g.DrawImage( spiesBack, 0, 0 );

			if ( selected > 0 )
			{
				//	int diff = ( picBox.Width - Form1.spyBmp.Width ) / ( spyNbr - 1 ); 

				g.DrawRectangle( 
					new Pen( Color.Black ),
					new Rectangle(
					8 + 0,
					0,
					(int)(diff * ( selected - 1 ) + Form1.spyBmp.Width - 1),//( selected - 1 ) * ( picBox.Width - Form1.spyBmp.Width ) / spyNbr + Form1.spyBmp.Width,
					aff.Height - 1
					)
					);

				g.DrawString( 
					selected.ToString(), 
					new Font( "Tahoma", 12, FontStyle.Regular ), 
					new SolidBrush( Color.White ), 
					2 + 1, 
					4 + 1
					);
			
				g.DrawString( 
					selected.ToString(), 
					new Font( "Tahoma", 12, FontStyle.Regular ), 
					new SolidBrush( Color.DarkCyan ), 
					2, 
					4 
					);
			}

			picBox.Image = aff;
		}
	#endregion

#region picBox_MouseMove
		private void picBox_MouseMove(object sender, MouseEventArgs e)
		{
			mouse();
		}

		private void picBox_MouseDown(object sender, MouseEventArgs e)
		{
			mouse();
		}

		private void mouse()
		{
			if ( enabled && spyNbr > 0 )
			{
				Point pntClick = picBox.PointToClient( Form1.MousePosition );
				pntClick.X -= 8; 

				if ( pntClick.X < 1 )
				{
					selected = 0;
				}
				else
				{
					selected = 0;

					for ( int k = spyNbr; k >= 0 ; k -- )
						if ( pntClick.X > diff * k )
						{
							selected = k + 1;
							break;
						}

					if ( selected > spyNbr )
					{
						selected = spyNbr;
					} 
				}
				
				if ( lastSelected != selected )
				{
					drawAff();
					lastSelected = selected;
				}
			}
		}
	#endregion

		private float diff
		{
			get
			{
				if ( spyNbr > 0 )
					return ( (float)picBox.Width - (float)Form1.spyBmp.Width ) / (float)( spyNbr != 0 ? spyNbr : 1 );
				else
					return 1;
			}
		}

	}
}
