using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XML_processor
{
    public class Interval
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class DataChangedEventArgs : EventArgs
    {
        public DataChangedEventArgs() { }
    }
    public delegate void DataChangedDelegate(object sender, DataChangedEventArgs e);
    public interface IDataElement
    {
        XmlNode Node { get; set; }
        Interval XmlInterval { get; set; }
        void OnDataChanged(DataChangedEventArgs e);
        event DataChangedDelegate DataChanged;
    }
}
