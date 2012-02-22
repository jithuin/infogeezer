using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;


namespace XML_processor
{
    

    public class ViewChangedEventArgs : EventArgs
    {
        public ViewChangedEventArgs() { }
    }
    public delegate void ViewChangedDelegate(object sender, ViewChangedEventArgs e);
    public interface IViewElement
    {
        /// <summary>
        /// Absolute position in screen coordinates
        /// </summary>
        PointF Position { get; set; }
        /// <summary>
        /// Relative position to its parent container
        /// </summary>
        PointF OffsetPosition { get; set; }
        SizeF Size { get; set; }
        
        IViewElement Parent{ get; set; }
        List<IViewElement> Children{ get; }

        /// <summary>
        /// Gives back whether the position is inside the element
        /// </summary>
        /// <param name="Position">Queried Position in screen coordinates (logical/world coordinates)</param>
        /// <returns></returns>
        bool HitTest(PointF Position);

        void Measure(Graphics g);
        void Paint(PaintEventArgs pe);
        void OnViewChanged(ViewChangedEventArgs e);
        event ViewChangedDelegate ViewChanged;
    }

    
}
