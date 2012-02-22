using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        List<Class1> points = new List<Class1>();
        System.Timers.Timer m_timer = new System.Timers.Timer();
        Boolean pointAddEnabled;
        Random rnd = new Random();
        delegate void ButtonCounter();

        private void Init()
        {
            points.Add(new Class1(new Point(200, 100),new PointF(2.0f,0.5f)));
            this.Paint += new PaintEventHandler(points[0].DrawMe);
            //foreach (Class1 p in points)
            //    this.Paint += new PaintEventHandler(p.DrawMe);
            m_timer.Interval = 20;
            m_timer.AutoReset = true;
            //m_timer.Enabled = true;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_timer_Elapsed);
            m_timer.Start();
            pointAddEnabled = false;
        }

        void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timer.Stop();
            if (pointAddEnabled)
            {
                this.Add();
                Invoke(new ButtonCounter(button_Counter));
            }
            Class1 p;
            for (int i = 0; i< points.Count; i++)
            {
                p = points[i];
                if (p.Step(20)) this.Invalidate(p.InvalidRect);
                if (!this.ClientRectangle.Contains(p.Position))
                {
                    this.Paint -= new PaintEventHandler(p.DrawMe);
                    points.RemoveAt(i);
                    i--;
                    Invoke(new ButtonCounter(button_Counter));
                }
            }
            m_timer.Start();
        }

        private void Add(Point p)
        {
            points.Add(new Class1(p));
            this.Paint += new PaintEventHandler(points[points.Count - 1].DrawMe);
            this.Invalidate(points[points.Count - 1].InvalidRect);
        }
        private void Add()
        {
            points.Add(new Class1(new Point(rnd.Next(200), rnd.Next(200)),new PointF((float)rnd.NextDouble(),(float)rnd.NextDouble())));
            this.Paint += new PaintEventHandler(points[points.Count - 1].DrawMe);
            this.Invalidate(points[points.Count - 1].InvalidRect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            pointAddEnabled = !pointAddEnabled;
        }
        private void button_Counter()
        {
            this.button1.Text = this.points.Count.ToString();
            this.button1.Invalidate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
            this.Update();
        }
    }
}
