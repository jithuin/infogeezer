using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using WindowsFormsApplication3.Properties;

namespace WindowsFormsApplication3
{
    class Box : Object
    {
        bool bColor;
        public Box() { bColor = false; }
        public Point Location { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public void ClickOnMe(Point click)
        {
            Rectangle r = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            if (r.Contains(click)) this.bColor = !this.bColor;
        }
        public void OnPaint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Bottom > this.Location.Y)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black), this.Location.X, this.Location.Y, this.Width, this.Height);
                if (this.bColor)
                    e.Graphics.FillRectangle(Brushes.Red, this.Location.X + 1, this.Location.Y + 1, this.Width - 1, this.Height - 1);
                else
                    e.Graphics.FillRectangle(Brushes.Gray, this.Location.X + 1, this.Location.Y + 1, this.Width - 1, this.Height - 1);
            }
        }

    }
    class Boxes
    {
        List<Box> boxes;
        Point location;
        public Point Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                if (boxes != null && boxes.Count > 0)
                {
                    boxes[0].Location = this.Location;
                    for (int i = 1; i < boxes.Count; i++)
                    {
                        boxes[i].Location = new Point(this.Location.X, boxes[i - 1].Location.Y + boxes[i - 1].Height);
                    }
                }
            }
        }
        public Boxes() 
        {
            boxes = new List<Box>(10);
            for (int i = 0; i < 10; i++)
            {
                Box b = new Box();
                b.Height = 50;
                b.Width = 320;
                //b.Location = new Point(10, i * 50);
                boxes.Add(b);
            }
            Location = new Point(0, 0);
        }
        public void OnPaint(object sender, PaintEventArgs e)
        {
            foreach (Box b in this.boxes)
            {
                b.OnPaint(sender, e);
            }
        }
        public void ClickOnMe(Point click)
        {
            foreach (Box b in this.boxes)
            {
                b.ClickOnMe(click);
            }
        }
        public int Height
        {
            get
            {
                int ret = 0;
                foreach (Box b in boxes)
                    ret += b.Height;
                return ret;
            }
        }
        public int Width
        {
            get
            {
                int ret = 0;
                foreach (Box b in boxes)
                    if (ret < b.Width) ret = b.Width;
                return ret;
            }
            set
            {
                foreach (Box b in boxes)
                    b.Width = value;
            }
        }
    }
}
