using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        QRect qrect;
        public Form1()
        {
            InitializeComponent();
            this.qrect = new QRect(this.ClientSize);   
        }
        

        private void Form1_Click(object sender, EventArgs e)
        {
            this.BackColor = qrect.GetBackColor(mousePosition, this.ClientSize);
        }
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            label1.Text = "x: " + e.X.ToString() + ", y: " + e.Y.ToString() + " Button: " + e.Button.ToString();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            qrect.PaintBorderLines(e, this.ClientSize);
            if (!bResizing) qrect.PaintRectangles(e, mousePosition, this.ClientSize);
        }

        Point mousePosition;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = "x: " + e.X.ToString() + ", y: " + e.Y.ToString() + " Button: " + e.Button.ToString();
            mousePosition = e.Location;
            //MouseOverRect(e);
            this.Invalidate(qrect.InvalidRegion(mousePosition, this.ClientSize));
            
        }

        

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.BackColor = Color.WhiteSmoke;
            this.Invalidate();
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            bResizing = false;
            qrect.ResizeQrects(this.ClientSize);
            //this.Invalidate();
        }
        bool bResizing = false;
        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            bResizing = true;
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            qrect.ResizeQrects(this.ClientSize);
        }

        


    }
}
