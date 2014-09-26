using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DragDrop.Model
{
    public class BrowserLinkDataObject : FormattedDataObject<MemoryStream>
    {
        public BrowserLinkDataObject(IDataObject object_in)
            : base(object_in)
        { ;}

        public override string DefaultFormat
        {
            get { return "UniformResourceLocator"; }
        }

        public override bool IsDataObjectCompatible(IDataObject object_in)
        {
            if (base.IsDataObjectCompatible(object_in))
            {
                BrowserLinkDataObject obj = new BrowserLinkDataObject(object_in);
                if (obj.Data.Position != 0)
                    obj.Data.Seek(0, SeekOrigin.Begin);

                return Uri.IsWellFormedUriString(new StreamReader(obj.Data).ReadToEnd(), UriKind.RelativeOrAbsolute);
            }

            return false;
        }

        public override string DataString
        {
            get
            {
                if (Data.Position != 0)
                    Data.Seek(0, SeekOrigin.Begin);
                return new StreamReader(Data).ReadToEnd();
            }
        }
    }
}