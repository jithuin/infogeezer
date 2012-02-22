using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace XML_processor
{
    public partial class RichTextControl : Control
    {
        public RichTextControl()
        {
            InitializeComponent();
        }

        public XmlViewDocument Document { get; set; }


        bool _isFirstDraw = true;
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_isFirstDraw)
            {
                _isFirstDraw = false;
                InitGraphics(pe);
            }
            Document.Paint(pe);
            base.OnPaint(pe);
        }

        private void InitGraphics(PaintEventArgs pe)
        {
            Document = new XmlViewDocument(pe.Graphics);
        }
        
    }
}
