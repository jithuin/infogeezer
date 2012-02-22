using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XML_processor
{
    public class ElementBase : IViewElement, IDataElement
    {
        #region Private Fields
        private System.Xml.XmlNode _node;
        private Interval _xmlInterval;
        private System.Drawing.PointF _position;
        private System.Drawing.SizeF _size;
        private IViewElement _parent;
        private List<IViewElement> _children;
        private bool _test;
        private bool _isMeasureNeeded;
        #endregion

        #region IDataElement Members

        public System.Xml.XmlNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

        public Interval XmlInterval
        {
            get { return _xmlInterval; }
            set { _xmlInterval = value; }
        }
        public virtual void OnDataChanged(DataChangedEventArgs e)
        {
            _isMeasureNeeded = true;
            if (DataChanged != null)
            {
                DataChanged(this, e);
            }
        }

        public event DataChangedDelegate DataChanged;
        #endregion

        #region IViewElement Members

        public System.Drawing.PointF Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public System.Drawing.PointF OffsetPosition
        {
            get
            {
                if (this.Parent == null)
                    return Position;
                else
                    return this.Parent.Position.Sub(this.Position);
            }
            set
            {
                if (this.Parent == null)
                    Position = value;
                else
                    Position = this.Parent.Position.Add(value);
            }
        }

        public System.Drawing.SizeF Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public IViewElement Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<IViewElement> Children
        {
            get { return _children; }
            protected set { _children = value; }
        }


        public bool HitTest(PointF Position_in)
        {
            RectangleF rect = new RectangleF(this.Position, this.Size);
            return rect.Contains(Position_in);
        }

        public virtual void Measure(Graphics g)
        {
            foreach (IViewElement elem in Children)
            {
                elem.Measure(g);
            }
        }

        public virtual void Paint(PaintEventArgs pe)
        {
            if (_isMeasureNeeded)
                Measure(pe.Graphics);
            foreach (IViewElement elem in Children)
            {
                elem.Paint(pe);
            }
        }

        public event ViewChangedDelegate ViewChanged;
        public virtual void OnViewChanged(ViewChangedEventArgs e)
        {
            if (ViewChanged != null)
                ViewChanged(this, e);
        }

        #endregion
    }
}
