using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SmartDeviceProject2.TextClasses
{
	class Paragraph
	{
		//VARIABLES
		public List<Line> lines;
		public PointF Location;
		public SizeF Size;
		public Document parent;
		public bool Rendered;
		//CONSTRUCTORS
		Paragraph(Document pparent)
		{
			this.lines = new List<Line>();
			this.parent = pparent;
		}
		//FUNCTIONS

		internal void InitGraphic(Graphics graphics)
		{
			throw new NotImplementedException();
		}
	}
}
