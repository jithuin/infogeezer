using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XML_processor
{
    public static class PointHelper
    {
        public static PointF Sub(this PointF A, PointF B)
        {
            return new PointF(A.X - B.X, A.Y - B.Y);
        }
        public static PointF Add(this PointF A, PointF B)
        {
            return new PointF(A.X + B.X, A.Y + B.Y);
        }
    }
}
