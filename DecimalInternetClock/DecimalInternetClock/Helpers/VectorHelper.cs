using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DecimalInternetClock.Helpers
{
    public static class VectorHelper
    {
        public static System.Drawing.Point ToPoint(this Vector vector_in)
        {
            return new System.Drawing.Point((int)vector_in.X, (int)vector_in.Y);
        }

        public static System.Drawing.Size ToSize(this Vector vector_in)
        {
            return new System.Drawing.Size((int)vector_in.X, (int)vector_in.Y);
        }

        public static Vector ToVector(this System.Drawing.Point point_in)
        {
            return new Vector((double)point_in.X, (double)point_in.Y);
        }

        public static Vector ToVector(this System.Drawing.Size size_in)
        {
            return new Vector((double)size_in.Width, (double)size_in.Height);
        }

        public static Matrix ToScaleMatrix(this System.Drawing.Size screenSize_in)
        {
            Matrix scale = new Matrix();
            scale.SetIdentity();
            scale.Scale(screenSize_in.Width, screenSize_in.Height);
            return scale;
        }

        public static Matrix ToInverted(this Matrix matrix_in)
        {
            matrix_in.Invert();
            return matrix_in;
        }
    }
}