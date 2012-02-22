using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication6
{
    class Square
    {
        #region CONSTRUCTORS
        public Square()
        {
            Random rnd = new Random();
            m_position.X = (float)rnd.NextDouble() * 240;
            m_position.Y = (float)rnd.NextDouble() * 320;
            temp_prevpos = Position;
        }
        public Square(Point position)
        {
            temp_prevpos = Position = position;
            Size = new Size(10, 10);
        }
        public Square(Point position, Size size)
        {
            temp_prevpos = Position = position;
            Size = size;
        }

        #endregion

        //------------------------------------------------
        #region PRIVATE MEMBERS
        //------------------------------------------------
        PointF m_position;
        Point temp_prevpos;
        Point prevpos;
        int speedUp = 10;
        Rectangle m_rectangle;
        #endregion

        //------------------------------------------------
        #region PUBLIC MEMBERS
        //------------------------------------------------
        
        public Point Position
        {
            get { return m_rectangle.Location; }
            set
            {
                m_position = (PointF)value;
                m_rectangle.Location = value;
            }
        }
        public Size Size
        {
            get { return m_rectangle.Size; }
            set { m_rectangle.Size = value; }
        }
        public int Height
        {
            get { return m_rectangle.Height; }
            set { m_rectangle.Height = value; }
        }
        public int Width
        {
            get { return m_rectangle.Width; }
            set { m_rectangle.Width = value; }
        }
        public Rectangle Rectangle
        {
            get { return m_rectangle; }
            set { m_rectangle = value; }
        }
        #endregion

        //------------------------------------------------
        #region PUBLIC FUNCTIONS
        //------------------------------------------------
        public void DrawMe(Graphics g)
        {
            //g.DrawRectangle(Pens.Black, m_position.X, m_position.Y, 10f, 10f);
            g.DrawRectangle(Pens.Black, m_rectangle);
        }
        //public void DrawMe(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.DrawRectangle(Pens.Black, m_position.X, m_position.Y, 0.9f , 0.9f);
        //}
        //public Rectangle InvalidRect
        //{
        //    get
        //    {
        //        //return new Rectangle(prevpos.X, prevpos.Y, Position.X - prevpos.X + 2, Position.Y - prevpos.Y + 2);
        //        return new Rectangle(Position.X, Position.Y, 2, 2);
        //    }
        //}
        //public Boolean Step(int milisec)
        //{
        //    prevpos = Position;
        //    m_position.X += m_velocity.X * 20 / 1000.0f*speedUp;
        //    m_position.Y += m_velocity.Y * 20 / 1000.0f*speedUp;
        //    if (temp_prevpos != Position)
        //    {
        //        temp_prevpos = Position;
        //        return true;
        //    }
        //    else
        //        return false;
        //}
        public void Step(Shape.Directions direction)
        {
            switch (direction)
            {
                case Shape.Directions.Left:
                    {
                        m_position.X -= m_rectangle.Width;
                        m_rectangle.X -= m_rectangle.Width;
                        break;
                    }
                case Shape.Directions.Right:
                    {
                        m_position.X += m_rectangle.Width;
                        m_rectangle.X += m_rectangle.Width;
                        break;
                    }
                case Shape.Directions.Down:
                    {
                        m_position.Y += m_rectangle.Height;
                        m_rectangle.Y += m_rectangle.Height;
                        break;
                    }
                case Shape.Directions.Turn:
                    {
                        m_position.Y -= m_rectangle.Height;
                        m_rectangle.Y -= m_rectangle.Height;
                        break;
                    }
            }
        }

        #endregion


    }
}
