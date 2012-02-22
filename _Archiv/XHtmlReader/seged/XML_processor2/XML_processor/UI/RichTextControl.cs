using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Windows.Media;


namespace XML_processor
{
    public partial class RichTextControl : Control
    {
        public RichTextControl()
        {
            InitializeComponent();
        }

        public XmlViewDocument ViewDocument { get; set; }
        public XmlDocument Document { get; set; }

        bool _isFirstDraw = true;
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_isFirstDraw)
            {
                _isFirstDraw = false;
                InitGraphics(pe);
            }
            // UNDONE:
            //ViewDocument.Paint(pe);
            pe.Graphics.DrawString(pe.ClipRectangle.ToString(), new Font(FontFamily.GenericSansSerif, 10f), new SolidBrush(Colors.Black));
            pe.Graphics.DrawString(pe.Graphics.ClipBounds.ToString(), new Font(FontFamily.GenericSansSerif, 10f), new SolidBrush(Colors.Black));
            base.OnPaint(pe);
        }

        private void InitGraphics(PaintEventArgs pe)
        {
            ViewDocument = new XmlViewDocument(pe.Graphics, Document);
            
        }
        
    }
}
