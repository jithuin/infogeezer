using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication4
{
    class Class1
    {
        #region private members (variables)
        PointF m_position;
        PointF m_velocity;
        Point temp_prevpos;
        Point prevpos;
        int speedUp = 10;
        #endregion

        #region public members (properties)
        public Point Position
        {
            get
            {
                return new Point((int)m_position.X, (int)m_position.Y);
            }
            set
            {
                m_position.X = value.X;
                m_position.Y = value.Y;
            }
        }
        public void DrawMe(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, m_position.X, m_position.Y, 0.9f , 0.9f);
        }
        public Rectangle InvalidRect
        {
            get
            {
                return new Rectangle(prevpos.X, prevpos.Y, Position.X - prevpos.X + 2, Position.Y - prevpos.Y + 2);
                //return new Rectangle(Position.X, Position.Y, 2, 2);
            }
        }
        public Boolean Step(int milisec)
        {
            prevpos = Position;
            m_position.X += m_velocity.X * 20 / 1000.0f*speedUp;
            m_position.Y += m_velocity.Y * 20 / 1000.0f*speedUp;
            if (temp_prevpos != Position)
            {
                temp_prevpos = Position;
                return true;
            }
            else
                return false;
        }
        #endregion

        #region constructors
        public Class1()
        {
            Random rnd = new Random();
            m_position.X = (float)rnd.NextDouble()*240;
            m_position.Y = (float)rnd.NextDouble()*320;
            temp_prevpos = Position;
            m_velocity.X = (float)rnd.NextDouble();
            m_velocity.Y = (float)rnd.NextDouble();
        }
        public Class1(Point position)
        {
            temp_prevpos = Position = position;
            m_velocity.X = 0;
            m_velocity.Y = 0;
        }
        public Class1(Point position, PointF velocity)
        {
            temp_prevpos = Position = position;
            m_velocity.X = velocity.X;
            m_velocity.Y = velocity.Y;
        }

        #endregion
    }
}
