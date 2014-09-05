using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DragDrop.Model
{
    /// <summary>
    /// Standard wrapper for DataObject to allow subclassing
    /// </summary>
    public abstract class DataObjectBase : IDataObject
    {
        protected IDataObject _dataObject;

        public DataObjectBase(IDataObject dataObj_in)
        {
            _dataObject = dataObj_in;
        }

        public object GetData(string format, bool autoConvert)
        {
            return _dataObject.GetData(format, autoConvert);
        }

        public object GetData(Type format)
        {
            return _dataObject.GetData(format);
        }

        public object GetData(string format)
        {
            return _dataObject.GetData(format);
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            return _dataObject.GetDataPresent(format, autoConvert);
        }

        public bool GetDataPresent(Type format)
        {
            return _dataObject.GetDataPresent(format);
        }

        public bool GetDataPresent(string format)
        {
            return _dataObject.GetDataPresent(format);
        }

        public string[] GetFormats(bool autoConvert)
        {
            return _dataObject.GetFormats(autoConvert);
        }

        public string[] GetFormats()
        {
            return _dataObject.GetFormats();
        }

        public void SetData(string format, object data, bool autoConvert)
        {
            _dataObject.SetData(format, data, autoConvert);
        }

        public void SetData(Type format, object data)
        {
            _dataObject.SetData(format, data);
        }

        public void SetData(string format, object data)
        {
            _dataObject.SetData(format, data);
        }

        public void SetData(object data)
        {
            _dataObject.SetData(data);
        }
    }
}