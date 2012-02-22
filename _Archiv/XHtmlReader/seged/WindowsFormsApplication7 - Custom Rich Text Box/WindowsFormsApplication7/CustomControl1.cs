using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{
    public partial class CustomControl1 : Control
    {
        //Constructor
        public CustomControl1()
        {
            //Init Default properties
            InitializeComponent();

            //Init vScrollBar1
            this.vScrollBar1 = new VScrollBar();
            this.vScrollBar1.Minimum = 0;
            this.vScrollBar1.Maximum = 100;
            this.vScrollBar1.Location = new Point(this.ClientRectangle.Right - this.vScrollBar1.Width, this.ClientRectangle.Top);
            //this.vScrollBar1.Location = new Point(this.ClientRectangle.Right - this.vScrollBar1.Width, 0);
            //this.vScrollBar1.Location = new Point(0, this.ClientRectangle.Top);
            //this.vScrollBar1.Location = new Point(0, 0);
            //this.vScrollBar1.Height = this.ClientRectangle.Height;
            this.Controls.Add(vScrollBar1);
            this.vScrollBar1.ValueChanged += new EventHandler(vScrollBar1_ValueChanged);
            //Init Text property
            //this.Text = "Hello World!";

            //Init textview property
            tv= new TextView();
            
            //Init clientRectResize 
            clientRectResized = true;
        }


        //Member variables
        private VScrollBar vScrollBar1;
        private String text;
        public override String Text { get { return text; } set { textChanged = true; text = value; } }
        private bool textChanged;
        TextView tv;
        private bool clientRectResized;
        int startLine = 0;

        //Member functions


        //Event handlers
        protected override void OnResize(EventArgs e)
        {
            clientRectResized = true;
            this.vScrollBar1.Location = new Point(this.ClientRectangle.Right - this.vScrollBar1.Width, this.ClientRectangle.Top);
            this.vScrollBar1.Height = this.ClientRectangle.Height;
            base.OnResize(e);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            if (textChanged)
            {
                tv.text = this.Text;
                tv.MakeWords();
                textChanged= false;
                clientRectResized = true;
            }
            if (clientRectResized)
            {
                tv.MakeLines(g, pe.ClipRectangle);
                this.vScrollBar1.Maximum = tv.lines.Count();
                clientRectResized = false;
                //ide még illene a nagy soremelést is állítani
            }
            float i = -startLine;
            float lineLocation;
            foreach (String line in tv.lines)
            {
                lineLocation = 10f + (Fonts.normal.Height + 2) * i;
                if (0 <= lineLocation && lineLocation < pe.ClipRectangle.Height)
                //if (i<1)
                {
                    g.DrawString(line, Fonts.normal, Brushes.Black, 10f, lineLocation);
                    
                }
                i++;
            }
            base.OnPaint(pe);
        }
        void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            startLine = this.vScrollBar1.Value;
            this.Invalidate();
        }

    }
}
