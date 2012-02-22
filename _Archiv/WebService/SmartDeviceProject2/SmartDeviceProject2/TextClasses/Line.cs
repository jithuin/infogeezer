using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceProject2.TextClasses
{
	class Line
	{
		//VARIABLES
		public List<Word> words;
		public int LocationY;
		public Paragraph parent;
		//CONSTRUCTORS
		Line(Paragraph pparent)
		{
			this.words = new List<Word>();
			this.parent = pparent;
		}
		//FUNCTIONS
	}
}
