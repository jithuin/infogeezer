using System;
using System.Drawing;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for ImageButton.
	/// </summary>
	public class ImageButton : Button
	{
		Bitmap image;
		Rectangle srcRect;
		bool mouseOver, mouseDown;

		public ImageButton( Bitmap image )
		{
			this.image = image;
			this.Size = image.Size;

			srcRect = new Rectangle(
				0,
				0,
				image.Width,
				image.Height
				);
		}

		private Rectangle thisRect
		{
			get
			{
				return new Rectangle( Left, Top, Width, Height );
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			mouseDown = true;
			base.OnMouseDown (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseDown = false;
			base.OnMouseUp (e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}


		protected override void OnPaint(PaintEventArgs e)
		{
		//	base.OnPaint (e);
			if ( Visible )
				if ( mouseDown )
					e.Graphics.FillRectangle(
						new SolidBrush( Color.Black ),
						thisRect
						);
				else
					e.Graphics.DrawImage(
						image,
						Left,
						Top,
						srcRect,
						GraphicsUnit.Pixel
						);
		}

	}
}
