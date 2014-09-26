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

        private static List<DataObjectBase> SubClasses
        {
            get
            {
                #region Lazy init

                if (_subClasses == null)
                    _subClasses = new List<DataObjectBase>(
                        typeof(DataObjectBase)
                                .Assembly
                                .GetTypes()
                                .Where(type =>
                                    type.IsSubclassOf(typeof(DataObjectBase))
                                    && !type.IsAbstract
                                    && type.GetConstructor(new Type[] { typeof(IDataObject) }) != null)
                                .Select(type =>
                                    (DataObjectBase)type
                                        .GetConstructor(new Type[] { typeof(IDataObject) })
                                        .Invoke(new object[] { null })));

                #endregion Lazy init

                return _subClasses;
            }
        }

        static DataObjectBase()
        {
        }

        protected IDataObject _dataObject;

        public DataObjectBase(IDataObject dataObj_in)
        {
            _dataObject = dataObj_in;
        }

        public abstract string DataString { get; }

        public virtual bool IsDataObjectCompatible(IDataObject object_in)
        {
            return true;
        }

        public static DataObjectBase GetDataObjectWrapper(IDataObject object_in)
        {
            DataObjectBase ret = SubClasses.FirstOrDefault(dataObj => dataObj.IsDataObjectCompatible(object_in));
            if (ret != null)
                return (DataObjectBase)ret.GetType()
                                          .GetConstructor(new Type[] { typeof(IDataObject) })
                                          .Invoke(new object[] { object_in });
            else
                return null;
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