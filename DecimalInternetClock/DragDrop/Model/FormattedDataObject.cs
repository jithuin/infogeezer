using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DragDrop.Model
{
    public abstract class FormattedDataObject<T> : DataObjectBase
    {
        public FormattedDataObject(IDataObject object_in, string format_in)
            : base(object_in)
        {
            Format = format_in;
        }

        public FormattedDataObject(IDataObject object_in)
            : base(object_in)
        {
            Format = DefaultFormat;
        }

        private bool CheckDataValidity(IDataObject object_in, string format_in)
        {
            return object_in.GetFormats().Contains(format_in) && object_in.GetDataPresent(format_in);
        }

        public override bool IsDataObjectCompatible(IDataObject object_in)
        {
            return CheckDataValidity(object_in, Format);
        }

        public abstract string DefaultFormat
        {
            get;
        }

        private string _format;

        public string Format
        {
            get
            {
                return _format;
            }
            private set
            {
                if (_format != value)
                    _format = value;
            }
        }

        public T Data
        {
            get
            {
                return (T)_dataObject.GetData(Format);
            }
        }
    }
}