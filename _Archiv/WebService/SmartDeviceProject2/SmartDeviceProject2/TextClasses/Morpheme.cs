using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceProject2.TextClasses
{
	class Morpheme
	{
		//VARIABLES
		public List<Character> characters;
		public Word parent;
		//CONSTRUCTORS
		Morpheme(Word pparent)
		{
			this.parent = pparent;
			this.characters = new List<Character>();
		}
		//FUNCTIONS
	}
}
