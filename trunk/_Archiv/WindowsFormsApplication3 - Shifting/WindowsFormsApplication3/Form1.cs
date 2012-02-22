using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }
        Point Start, Delta;
        long sTime;
        int i;
        bool bScrolling = false;
        System.Timers.Timer timer;
        Boxes boxes;
        int[] szamok;
        int prevVscroll;
        private void Init()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.OptimizedDoubleBuffer, true);
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
            // ControlStyles.AllPaintingInWmPaint |
            // ControlStyles.UserPaint |
            // ControlStyles.Opaque, true);
            boxes = new Boxes();
            boxes.Location = new Point(100, 100);
            this.Paint += new PaintEventHandler(boxes.OnPaint);
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            CheckForIllegalCrossThreadCalls = false;
            //prevVscroll = vScrollBar1.Value;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            boxes.ClickOnMe(e.Location);
            vScrollBar1.Minimum = 0;
            vScrollBar1.LargeChange = this.ClientRectangle.Height;
            if (boxes.Location.Y > 0)
            {
                vScrollBar1.Maximum = (boxes.Height + boxes.Location.Y);
                vScrollBar1.LargeChange = this.ClientRectangle.Height;
                vScrollBar1.Value = 0;
            }
            else
            {
                vScrollBar1.Maximum = (this.ClientRectangle.Height - boxes.Location.Y);
                vScrollBar1.LargeChange = this.ClientRectangle.Height;
                vScrollBar1.Value = vScrollBar1.Maximum;
            }
            Start.X = e.X;
            Start.Y = e.Y;
            Delta.X = 0;
            Delta.Y = 0;
            bScrolling = true;
            sTime = DateTime.Now.ToBinary();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (bScrolling)
            {
                Delta.X = e.X - Start.X;
                Delta.Y = e.Y - Start.Y;
                //foreach (Control c in this.Controls)
                //{
                //    c.Location = new Point(c.Location.X + Delta.X, c.Location.Y + Delta.Y);
                //}
                boxes.Location = new Point(boxes.Location.X + Delta.X, boxes.Location.Y + Delta.Y);
                if (boxes.Location.Y > 0)
                {
                    vScrollBar1.Maximum = (boxes.Height + boxes.Location.Y);
                    vScrollBar1.LargeChange = this.ClientRectangle.Height;
                    //prevVscroll = 0;
                    vScrollBar1.Value = 0;
                }
                else
                {
                    vScrollBar1.Maximum = (this.ClientRectangle.Height - boxes.Location.Y);
                    vScrollBar1.LargeChange = this.ClientRectangle.Height;
                    //prevVscroll = vScrollBar1.Maximum;
                    vScrollBar1.Value = vScrollBar1.Maximum;
                }
                Start.X = e.X;
                Start.Y = e.Y;
                this.Invalidate();
                //sTime = DateTime.Now.Ticks;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            bScrolling = false;
            //Delta.X = e.X - Start.X;
            //Delta.Y = e.Y - Start.Y;
            //foreach (Control c in this.Controls)
            //{
            //    c.Location = new Point(c.Location.X + Delta.X, c.Location.Y + Delta.Y);
            //}
            //Start.X = e.X;
            //Start.Y = e.Y;
            //this.Invalidate();

            int interval = DateTime.FromBinary(DateTime.Now.ToBinary() - sTime).Millisecond / 20;
            if (interval != 0)
            {
                timer.Interval = interval;
                i = 0;
                timer.Start();
            }
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            i++;
            if (i < 40)
            {
                //foreach (Control c in this.Controls)
                //{
                //    c.Location = new Point(c.Location.X + Delta.X * (40 - i) / 40, c.Location.Y + Delta.Y * (40 - i) / 40);
                //}
                boxes.Location = new Point(boxes.Location.X + Delta.X * (40 - i) / 40, boxes.Location.Y + Delta.Y * (40 - i) / 40);
                if (boxes.Location.Y > 0)
                {
                    vScrollBar1.Maximum = (boxes.Height + boxes.Location.Y);
                    vScrollBar1.LargeChange = this.ClientRectangle.Height;
                    vScrollBar1.Value = 0;
                }
                else
                {
                    vScrollBar1.Maximum = (this.ClientRectangle.Height - boxes.Location.Y);
                    vScrollBar1.LargeChange = this.ClientRectangle.Height;
                    //prevVscroll = vScrollBar1.Maximum;
                    vScrollBar1.Value = vScrollBar1.Maximum;
                }
                this.Invalidate();
                timer.Start();
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            lbStatus.Text = "Horizontal: " + hScrollBar1.Value.ToString();
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            //int y = (prevVscroll - vScrollBar1.Value) * vScrollBar1.Maximum / vScrollBar1.LargeChange;
            //prevVscroll = vScrollBar1.Value;
            //boxes.Location = new Point(boxes.Location.X, boxes.Location.Y + y);
            //this.Invalidate();
            ////if (boxes.Location.Y > 0)
            ////{
            ////    vScrollBar1.Maximum = boxes.Height + boxes.Location.Y;
            ////    //vScrollBar1.Value = vScrollBar1.Maximum;
            ////}
            ////else
            ////{
            ////    vScrollBar1.Maximum = (this.ClientRectangle.Height - boxes.Location.Y);
            ////    //vScrollBar1.Value = 0;
            ////}
            //lbStatus.Text = "Vertical: " + vScrollBar1.Value.ToString();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int y = (e.OldValue - e.NewValue)*vScrollBar1.Maximum / vScrollBar1.LargeChange;
            boxes.Location = new Point(boxes.Location.X, boxes.Location.Y + y);
            this.Invalidate();
            lbStatus.Text = "Vertical: " + vScrollBar1.Value.ToString();
        }


    }
}
