using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WindowsFormsApplication5
{
    public class Entry
    {
        [XmlElement("id")]
        public string Id
        {
            get
            ;
            set
            ;
        }

        [XmlElement("title")]
        public string Title
        {
            get
            ;
            set
            ;
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            //ret.AppendFormat("  Feed: \r\n");
            ret.AppendFormat("{0}: {1}{2}\r\n", Title, Id.Substring(0, 1).ToLower(), Id.Substring(1));

            return ret.ToString();
        }

        public string ToRtfString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append('{');
            String s = String.Format("\\rtf1\\ansi\\deff0\\trowd\\cellx1000\\cellx2000\\intbl {0}\\cell\\intbl {1}\\cell\\row", Title, Id);

            ret.Append(s);
            ret.Append('}');
            return ret.ToString();
        }
    }
}