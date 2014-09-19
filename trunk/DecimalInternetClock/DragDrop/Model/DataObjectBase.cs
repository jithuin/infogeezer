using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace DragDrop.Model
{
    /// <summary>
    /// Standard wrapper for DataObject to allow subclassing
    /// </summary>
    public abstract class DataObjectBase : IDataObject
    {
        private static List<DataObjectBase> _subClasses;

        static DataObjectBase()
        {
        }

        private static void GenerateDerivedObjects()
        {
            _subClasses = new List<DataObjectBase>();

            _subClasses.AddRange(
                typeof(DataObjectBase)
                    .Assembly
                    .GetTypes()
                    .Where(type =>
                        type.IsSubclassOf(typeof(DataObjectBase))
                        && type.GetConstructor(new Type[] { typeof(IDataObject) }) != null)
                    .Select(type =>
                        (DataObjectBase)type
                            .GetConstructor(new Type[] { typeof(IDataObject) })
                            .Invoke(new object[] { null })));
        }

        protected IDataObject _dataObject;

        public DataObjectBase(IDataObject dataObj_in)
        {
            _dataObject = dataObj_in;
        }

        public virtual bool IsDataObjectCompatible(IDataObject object_in)
        {
            return true;
        }

        public static DataObjectBase GetDataObjectWrapper(IDataObject object_in)
        {
            if (_subClasses == null)
                GenerateDerivedObjects();
            return _subClasses.Single(dataObj => dataObj.IsDataObjectCompatible(object_in));
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