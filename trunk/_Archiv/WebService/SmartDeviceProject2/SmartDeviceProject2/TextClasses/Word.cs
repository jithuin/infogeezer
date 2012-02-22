using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceProject2.TextClasses
{
	class Word
	{
		//VARIABLES
		public List<Morpheme> morphemes;
		public Line parent;
		//CONSTRUCTORS
		Word(Line pparent)
		{
			this.parent = pparent;
			this.morphemes = new List<Character>();
		}
		//FUNCTIONS
	}
}
