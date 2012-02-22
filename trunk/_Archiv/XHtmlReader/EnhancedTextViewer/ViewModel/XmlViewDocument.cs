using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;

namespace XML_processor
{
    public class XmlViewDocument
    {
        public XmlDocument Document { get; set; }
        protected Graphics _graphics { get; set; }
        public List<Paragraph> Paragraphs { get; private set; }
        XmlViewDocument() 
        {
            this.Paragraphs = new List<Paragraph>();

        }
        public XmlViewDocument(Graphics g) : this() { _graphics = g; }
        internal void Paint(PaintEventArgs pe)
        {
            //TODO: implement
        }
    }
}
