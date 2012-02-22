using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SmartDeviceProject2.TextClasses
{
	class Character
	{
		//MEMBERS
		public Morpheme parent;
		public Char character;
		public Font font;
		public int LocationX;
		public int LocationY
		{
			get
			{
				return this.parent.parent.LocationY;
			}
		}
		//CONSTRUCTORS
		Character(Morpheme pparent)
		{
			this.parent = pparent;
			this.character = ' ';
			this.font = new Font("Tahoma", 8f, FontStyle.Regular);
			LocationX = 0;
		}
		//FUNCTIONS
		public SizeF MeasureString(Graphics g)
		{
			return g.MeasureString(this.character, this.font);
		}
		//ENUMERATIONS

	}
}
