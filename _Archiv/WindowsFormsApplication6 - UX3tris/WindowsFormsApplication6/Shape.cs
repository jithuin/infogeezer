using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication6
{
    class Shape
    {
        public enum Shapes { T, L, rL, O, N, I, rN }
        public enum Orientations {Left, Up, Right, Down }
        public enum Directions { Left, Right, Down, Turn, Else };

        private List<Square> m_squares;
        private Shapes m_shape;
        private Orientations m_orientation;
        private Point m_location;
        private Size m_blockSize;

        public Shape()
        {
            m_shape = Shapes.I;
            m_orientation = Orientations.Up;
            m_squares = new List<Square>(4);
            m_location = new Point(100, 100);
            m_blockSize = new Size(10, 10);

            for (int i = 0; i < 4; i++)
                m_squares.Add(
                    new Square(
                        new Point(m_location.X, m_location.Y - (i + 1) * m_blockSize.Height),
                        new Size(m_blockSize.Width - 2, m_blockSize.Height - 2)
                    )
                );

        } 

        private void turnCounterClockwise()
        {
            //switch the orientation
            if (m_orientation == Orientations.Left)
                m_orientation = Orientations.Down;
            else
                m_orientation -= 1;
            //actualise drawing
            updateSquares();
        }
        private void shiftLeftOrRight(Directions dir)
        {
            switch (dir)
            {
                case Directions.Left:
                    {
                        m_location.X -= m_blockSize.Width;
                        updateSquares();
                        break;
                    }
                case Directions.Right:
                    {
                        m_location.X += m_blockSize.Width;
                        updateSquares();
                        break;
                    }
                default:
                    break;
            }
        }
        private void updateSquares()
        {
            switch (m_orientation)
            {
                case Orientations.Down:
                case Orientations.Up:
                    {
                        for (int i = 0; i < 4; i++)
                            m_squares[i].Position = new Point(m_location.X, m_location.Y - (i + 1) * m_blockSize.Height);
                        break;
                    }
                case Orientations.Left:
                case Orientations.Right:
                    {
                        for (int i = 0; i < 4; i++)
                            m_squares[i].Position = new Point(m_location.X+i*m_blockSize.Width, m_location.Y - m_blockSize.Height);
                        break;
                    }
            }
        }

        public void TransformShape(Directions dir)
        {
            switch (dir)
            {
                case Directions.Turn:
                    {
                        turnCounterClockwise();
                        break;
                    }
                case Directions.Left:
                case Directions.Right:
                    {
                        shiftLeftOrRight(dir);
                        break;
                    }
                default:
                    break;
            }
        }
        public void DrawMe(Graphics g)
        {
            foreach (Square s in m_squares)
                g.DrawRectangle(Pens.Black, s.Rectangle);
        }

    }
}
