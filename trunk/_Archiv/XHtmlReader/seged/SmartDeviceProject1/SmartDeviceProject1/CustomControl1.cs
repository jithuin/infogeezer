using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartDeviceProject1
{
    public partial class FlowDocument : Control
    {
        public FlowDocument()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            pe.Graphics.DrawString("this", new Font(FontFamily.GenericMonospace, 10f, FontStyle.Bold),new SolidBrush(Color.Black) ,new RectangleF(10,10,100,100));
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
    }
}
