using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceProject2
{
	class PointF : Object
	{
		public float X { get; set; }
		public float Y { get; set; }
		PointF()
		{
			this.X = 0;
			this.Y = 0;
		}
		PointF(float xp, float yp)
		{
			this.X = xp;
			this.Y = yp;
		}

		public override string ToString()
		{
			return "PointF: X:" + this.X.ToString() + " Y:" + this.Y.ToString();
		}
	}
}
