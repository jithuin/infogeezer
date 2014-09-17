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
            CheckDataValidity(object_in, Format);
        }

        public FormattedDataObject(IDataObject object_in)
            : base(object_in)
        {
            Format = DefaultFormat;
            CheckDataValidity(object_in, Format);
        }

        private void CheckDataValidity(IDataObject object_in, string format_in)
        {
            object_in.GetFormats().Contains(format_in);
            object_in.GetDataPresent(format_in);
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
            protected set
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