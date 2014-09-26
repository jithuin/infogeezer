using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DragDrop.Model
{
    public class FileLinkDataObject : FormattedDataObject<String[]>
    {
        public FileLinkDataObject(IDataObject object_in)
            : base(object_in)
        { ;}

        public override string DefaultFormat
        {
            get { return "FileDrop"; }
        }

        public override bool IsDataObjectCompatible(IDataObject object_in)
        {
            return base.IsDataObjectCompatible(object_in) &&
                new FileLinkDataObject(object_in).Data.All((s) => new FileInfo(s).Exists);
        }

        public override string DataString
        {
            get
            {
                return Data.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString(); //aggregate all file links in new lines.
            }
        }
    }
}