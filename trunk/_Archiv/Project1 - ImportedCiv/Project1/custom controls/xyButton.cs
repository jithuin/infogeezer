using System;
using System.Windows.Forms;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for xyButton.
	/// </summary>
	public class xyButton : System.Windows.Forms.Control
	{
		static SolidBrush disabledBrush,
			normalBrush,
			overBrush,
			downBrush,
			textBrush;

		static Pen blackPen;

		bool over, mouseDown;

		public xyButton()
		{
			if ( disabledBrush == null )
			{
				disabledBrush = new SolidBrush( Color.Gray );
				normalBrush = new SolidBrush( Color.Wheat );
				overBrush = new SolidBrush( Color.DarkGoldenrod );
				downBrush = new SolidBrush( Color.Brown );
				textBrush = new SolidBrush( Color.Black );
				
				/*
				disabledBrush = new SolidBrush( Color.Gray );
				normalBrush = new SolidBrush( Color.LightGreen );
				overBrush = new SolidBrush( Color.Green );
				downBrush = new SolidBrush( Color.DarkGreen );
				textBrush = new SolidBrush( Color.Black );
				*/

				blackPen = new Pen( Color.Black );
			}

			this.Height = 20;
		}
		
	/*	protected override void OnPaintBackground(PaintEventArgs e)
		{
			if ( !this.Enabled )
				e.Graphics.FillRectangle( disabledBrush, this.ClientRectangle );
			else if ( this.Focused )
				e.Graphics.FillRectangle( overBrush, this.ClientRectangle );
			else
				e.Graphics.FillRectangle( normalBrush, this.ClientRectangle );

			e.Graphics.DrawRectangle( blackPen, this.ClientRectangle );
		}	*/

		protected override void OnPaint(PaintEventArgs e)
		{
#if CF
	/*		if ( !this.Enabled )
				e.Graphics.FillRectangle( disabledBrush, this.drawingRect );
			else if ( over && mouseDown )
				e.Graphics.FillRectangle( overBrush, this.drawingRect );
			else
				e.Graphics.FillRectangle( normalBrush, this.drawingRect );*/

			if ( !this.Enabled )
				e.Graphics.FillEllipse( disabledBrush, this.drawingRect );
			else if ( over && mouseDown )
				e.Graphics.FillEllipse( overBrush, this.drawingRect );
			else
				e.Graphics.FillEllipse( normalBrush, this.drawingRect );
#else
			if ( !this.Enabled )
				e.Graphics.FillRectangle( disabledBrush, this.drawingRect );
			else if ( over )
				if ( mouseDown )
					e.Graphics.FillRectangle( downBrush, this.drawingRect );
				else
					e.Graphics.FillRectangle( overBrush, this.drawingRect );
			else
				e.Graphics.FillRectangle( normalBrush, this.drawingRect );

#endif
			
		/*	else if ( this.Focused )
				e.Graphics.FillRectangle( overBrush, this.drawingRect );
			else
				e.Graphics.FillRectangle( normalBrush, this.drawingRect );*/

			if ( this.Focused )
				e.Graphics.DrawRectangle( 
					blackPen, 
					new Rectangle( 
						this.drawingRect.Left + this.drawingRect.Width / 6, 
						this.drawingRect.Top + this.drawingRect.Height / 6, 
						this.drawingRect.Right - this.drawingRect.Width / 6, 
						this.drawingRect.Bottom - this.drawingRect.Height / 6
					)
					);
			
	//		e.Graphics.DrawRectangle( blackPen, drawingRect );
			e.Graphics.DrawEllipse( blackPen, drawingRect );

			SizeF s = e.Graphics.MeasureString( Text, Font );

			e.Graphics.DrawString( 
				this.Text, this.Font, textBrush, 
				this.Width / 2 - (int)s.Width / 2,
				this.Height / 2 - (int)s.Height / 2
				);
		}

		Rectangle drawingRect
		{
			get
			{
				return new Rectangle( this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1 );
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			bool oldMouseDown = mouseDown,
				oldOver = over;

			mouseDown = true;
			over = this.ClientRectangle.Contains( e.X, e.Y );

			if ( oldMouseDown != mouseDown || oldOver != over )
				this.Invalidate();

			base.OnMouseDown (e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			bool oldOver = over;

			over = this.ClientRectangle.Contains( e.X, e.Y );

			if ( oldOver != over )
				this.Invalidate();

			base.OnMouseMove (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			bool oldMouseDown = mouseDown;

			mouseDown = false;

			if ( oldMouseDown != mouseDown )
				this.Invalidate();

			base.OnMouseUp (e);
		}



	}
}
