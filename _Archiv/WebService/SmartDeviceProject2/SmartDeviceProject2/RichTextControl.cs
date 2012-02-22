using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartDeviceProject2
{
	public partial class RichTextControl : Control
	{
		//public string Text { get; set; }
		TextClasses.Document document;
		Bitmap bmBuffer;
		Graphics grBuffer;
		Graphics graphics;
		bool GraphicInitDone = false;

		public RichTextControl()
		{
			InitializeComponent();

			//Init document
			document = new SmartDeviceProject2.TextClasses.Document();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			if (!GraphicInitDone)
			{				
				InitGraphic(pe.Graphics);	// Initial formatting 
				GraphicInitDone = true;
			}
			this.document.DrawMe(pe.Graphics);



			// Calling the base class OnPaint
			base.OnPaint(pe);
		}
		protected override void OnResize(EventArgs e)
		{
			this.GraphicInitDone = false; // Reformat the text
			base.OnResize(e);
		}

		public SizeF MeasureString(String s, Font font)
		{
			return graphics.MeasureString(s, font);
		}
		public RectangleF ClipBounds()
		{
			return graphics.ClipBounds;
		}

		private void InitGraphic(Graphics graphics)
		{
			this.graphics = graphics;
			this.document.InitGraphic(graphics);
		}

		
	}
}
