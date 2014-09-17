using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DragDrop.Model.Notes
{
    public class NotesDataObject : FormattedDataObject<MemoryStream>
    {
        public NotesDataObject(IDataObject object_in)
            : base(object_in)
        { ;}

        public override string DefaultFormat
        {
            get
            {
                return "xml";
            }
        }

        public string DataContent
        {
            get
            {
                return Data.XmlDeserialize<Feed>().ToString();
            }
        }
    }
}