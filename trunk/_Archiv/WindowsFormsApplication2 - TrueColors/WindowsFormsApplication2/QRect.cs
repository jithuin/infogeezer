using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    class QRect
    {
        public enum Quarter { First = 0, Second = 1, Third = 3, Forth = 2, Lower = 2, Right = 1, None = -1 };
        Rectangle[] QRects;
        Quarter previousQuarter, presentQuarter;

        public QRect()
        {
            InitQrects();
        }
        public QRect(Size ClientSize)
        {
            InitQrects();
            ResizeQrects(ClientSize);
            previousQuarter = Quarter.First;
        }

        void InitQrects()
        {
            QRects = new Rectangle[4];
            previousQuarter = Quarter.First;
        }
        public void ResizeQrects(Size ClientSize)
        {
            Size s = new Size(ClientSize.Width / 2, ClientSize.Height / 2);
            QRects[(int)Quarter.First] = new Rectangle(new Point(0, 0), s);
            QRects[(int)Quarter.Second] = new Rectangle(new Point(s.Width + 1, 0), s);
            QRects[(int)Quarter.Third] = new Rectangle(new Point(s.Width + 1, s.Height + 1), s);
            QRects[(int)Quarter.Forth] = new Rectangle(new Point(0, s.Height + 1), s);

            previousQuarter = Quarter.None;
        }

        private Quarter GetQfromPoint(Point p, Size s)
        {
            Quarter q = 0;
            if (p.X > s.Width / 2) q |= Quarter.Right;
            if (p.Y > s.Height / 2) q |= Quarter.Lower;
            return q;
        }
        private Color GetColorFromQ(Quarter q)
        {
            switch (q)
            {
                case Quarter.First:
                    return Color.Red;
                case Quarter.Second:
                    return Color.Yellow;
                case Quarter.Third:
                    return Color.Green;
                case Quarter.Forth:
                    return Color.Blue;
                default:
                    return Color.Gray;
            }
        }
        private Color GetBackColorFromQ(Quarter q)
        {
            switch (q)
            {
                case Quarter.First:
                    return Color.Pink;
                case Quarter.Second:
                    return Color.LightYellow;
                case Quarter.Third:
                    return Color.LightGreen;
                case Quarter.Forth:
                    return Color.LightBlue;
                default:
                    return Color.Gray;
            }
        }

        private Brush GetBrushFromQ(Quarter q)
        {
            switch (q)
            {
                case Quarter.First:
                    return Brushes.Red;
                case Quarter.Second:
                    return Brushes.Yellow;
                case Quarter.Third:
                    return Brushes.Green;
                case Quarter.Forth:
                    return Brushes.Blue;
                default:
                    return Brushes.Gray;
            }
        }
        public Color GetColor(Point mousePosition, Size ClientSize)
        {
            return GetColorFromQ(GetQfromPoint(mousePosition, ClientSize));
        }
        internal Color GetBackColor(Point mousePosition, Size ClientSize)
        {
            return GetBackColorFromQ(GetQfromPoint(mousePosition, ClientSize));
        }

        public void PaintRectangles(PaintEventArgs e, Point mousePosition, Size ClientSize)
        {
            Quarter q = GetQfromPoint(mousePosition, ClientSize);
            e.Graphics.FillRectangle(GetBrushFromQ(q), QRects[(int)q]);
        }
        public void PaintBorderLines(PaintEventArgs e, Size ClientSize)
        {
            Pen p = new Pen(Color.Black);
            p.Width = 1;
            Size s = ClientSize;
            e.Graphics.DrawLine(p, s.Width / 2, 0, s.Width / 2, s.Height);
            e.Graphics.DrawLine(p, 0, s.Height / 2, s.Width, s.Height / 2);
        }
        internal int min(int a, int b)
        {
            return a < b ? a : b;
        }
        internal int abs(int a)
        {
            return a > 0 ? a : (-a);
        }
        
        //internal Rectangle InvalidRect(Point mousePosition, Size size)
        //{
        //    Rectangle a, b;
        //    int x,y,width,height;
        //    presentQuarter = GetQfromPoint(mousePosition,size);
        //    if (previousQuarter != presentQuarter && previousQuarter != Quarter.None)
        //    {
        //        a = QRects[(int)previousQuarter];
        //        b = QRects[(int)presentQuarter];
        //        previousQuarter = presentQuarter;
        //        x = min(a.X, b.X);
        //        y = min(a.Y, b.Y);
        //        width = abs(a.X - b.X) + a.Width;
        //        height = abs(a.Y - b.Y) + a.Height;
        //        return new Rectangle(x, y, width, height);
        //    }
        //    else
        //        return new Rectangle();
        //}

        internal Region InvalidRegion(Point mousePosition, Size size)
        {
            presentQuarter = GetQfromPoint(mousePosition, size);
            if (previousQuarter != presentQuarter)
            {
                Region r = new Region(QRects[(int)presentQuarter]);
                if (previousQuarter != Quarter.None)   
                    r.Union(QRects[(int)previousQuarter]); 
                previousQuarter = presentQuarter;
                return r;
            }
            else
                return new Region(new Rectangle(0, 0, 0, 0));
            
        }

        
    }
}
