using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DragDrop.Model.Notes
{
    [XmlRoot("feed")]
    public class Feed
    {
        [XmlElement("entry")]
        public List<Entry> Entries
        {
            get
            ;
            set
            ;
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            //ret.AppendFormat("Entry: \r\n");
            foreach (Entry e in Entries)
                ret.Append(e);
            return ret.ToString();
        }

        //public string ToRtfString()
        //{
        //    return Entry.ToRtfString();
        //}
    }
}