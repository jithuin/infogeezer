using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for NewFileHeader.
	/// </summary>
	public class NewFileHeader : FileHeader
	{
		public NewFileHeader()
		{
			path = "";
		}

		public override void draw( Graphics g, Rectangle dest )
		{
			Font txtFont = new Font(
				"Tahoma",
				9,
				FontStyle.Regular
				);
			Brush blackBrush = new SolidBrush( Color.Black );
			int border = 4, 
				textHeight = (int)g.MeasureString( "a", txtFont ).Height - 2;

			g.FillRectangle( 
				new SolidBrush( Form1.defaultBackColor ), 
				dest 
				);

		/*	g.DrawRectangle( 
				blackBrush, 
				new Rectangle( 
				dest.Left + border, 
				dest.Top + dest.Height / 2 - BmpWidth / 3, 
				BmpWidth, 
				BmpWidth * 2/3 
				) 
				);*/
		/*	if ( bmp != null )
				g.DrawImage(
					bmp,
					dest.Left + border,
					dest.Top + dest.Height / 2 - BmpWidth / 3,
					new Rectangle( 0, 0, BmpWidth, BmpWidth * 2/3 ),
					GraphicsUnit.Pixel
					);
			else
				g.FillRectangle( 
					blackBrush, 
					new Rectangle( 
					dest.Left + border, 
					dest.Top + dest.Height / 2 - BmpWidth / 3, 
					BmpWidth, 
					BmpWidth * 2/3 
					) 
					);*/

			g.DrawString( 
				language.getAString( language.order.filesNewFile ),
				txtFont,
				blackBrush,
				dest.Left + BmpWidth + 2*border,
				dest.Top + textHeight // + border
				);
		}
	}
}
