using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SmartDeviceProject2.TextClasses
{
	class Document
	{
		//VARIABLES
		public List<Paragraph> paragraphs;
		public String text;

		public TextPosition RenderedTextPos;
		public TextPosition DisplayedTextPos;
		public List<TextPosition> SelectedTextPos;
		//CONSTRUCTORS
		Document()
		{
			this.paragraphs = new List<Paragraph>();
			this.RenderedTextPos = new TextPosition();
			this.DisplayedTextPos = new TextPosition();
			this.SelectedTextPos = new List<TextPosition>();
		}

		

		//FUNCTIONS
		void AddParagraph(TextPosition tp)
		{
			paragraphs.Add(new Paragraph(this));
			throw new NotImplementedException("only partially implemented");
		}
		public void DrawMe(Graphics g)
		{
			PointF p = new PointF(g.ClipBounds.Left, g.ClipBounds.Top);
			if (this.paragraphs.Count != 0)
				foreach (Paragraph p in paragraphs)
				{
					p.DrawMe(g);
				}
		}

		internal void InitGraphic(Graphics graphics)
		{

			// itt kellene a kezdő tagolásokat elkezdeni

			if (this.paragraphs.Count != 0)
				foreach (Paragraph p in this.paragraphs)
					p.Rendered = false;

		}
	}
}
