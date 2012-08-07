using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManagedWinapi;
using ManagedWinapi.Windows;

namespace DecimalInternetClock.HotKeys
{
    public class Vector2D
    {
        #region Properties

        public double X { get; set; }

        public double Y { get; set; }

        #endregion Properties

        #region Constructors

        public Vector2D() : this(0.0, 0.0) { ;}

        public Vector2D(double x_in, double y_in)
        {
            X = x_in;
            Y = y_in;
        }

        #endregion Constructors

        #region Operators

        #region System.Windows.Size

        public static explicit operator Vector2D(System.Windows.Size size_in)
        {
            return new Vector2D(size_in.Width / SystemWindow.DesktopWindow.Size.Width,
                                size_in.Height / SystemWindow.DesktopWindow.Size.Height);
        }

        public static explicit operator System.Windows.Size(Vector2D vector_in)
        {
            return new System.Windows.Size(vector_in.X * SystemWindow.DesktopWindow.Size.Width,
                                           vector_in.Y * SystemWindow.DesktopWindow.Size.Height);
        }

        #endregion System.Windows.Size

        #region System.Drawing.Size

        public static explicit operator Vector2D(System.Drawing.Size size_in)
        {
            return new Vector2D((double) size_in.Width / SystemWindow.DesktopWindow.Size.Width,
                                (double) size_in.Height / SystemWindow.DesktopWindow.Size.Height);
        }

        public static explicit operator System.Drawing.Size(Vector2D vector_in)
        {
            return new System.Drawing.Size((int)(vector_in.X * SystemWindow.DesktopWindow.Size.Width),
                                           (int)(vector_in.Y * SystemWindow.DesktopWindow.Size.Height));
        }

        #endregion System.Drawing.Size

        #region System.Windows.Point

        public static explicit operator Vector2D(System.Windows.Point point_in)
        {
            return new Vector2D(point_in.X / SystemWindow.DesktopWindow.Size.Width,
                                point_in.Y / SystemWindow.DesktopWindow.Size.Height);
        }

        public static explicit operator System.Windows.Point(Vector2D vector_in)
        {
            return new System.Windows.Point(SystemWindow.DesktopWindow.Size.Width * vector_in.X,
                                            SystemWindow.DesktopWindow.Size.Height * vector_in.Y);
        }

        #endregion System.Windows.Point

        #region System.Drawing.Point

        public static explicit operator Vector2D(System.Drawing.Point point_in)
        {
            return new Vector2D((double)point_in.X / SystemWindow.DesktopWindow.Size.Width,
                                (double)point_in.Y / SystemWindow.DesktopWindow.Size.Height);
        }

        public static explicit operator System.Drawing.Point(Vector2D vector_in)
        {
            return new System.Drawing.Point((int)(SystemWindow.DesktopWindow.Size.Width * vector_in.X),
                                            (int)(SystemWindow.DesktopWindow.Size.Height * vector_in.Y));
        }

        #endregion System.Drawing.Point

        #endregion Operators
    }
}