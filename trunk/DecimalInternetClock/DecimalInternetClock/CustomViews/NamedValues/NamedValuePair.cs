using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.NamedValues
{
    public class KeyValuePairClass<N, V> : INotifyPropertyChanged
    {
        #region Fields and Properties

        protected N _name;
        protected V _value;
        protected bool _isReadonly = false;

        public N Name
        {
            get { return _name; }
            set
            {
                if (!value.Equals(_name))
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public V Value
        {
            get { return _value; }
            set
            {
                if (!IsReadonly)
                {
                    if (value == null)
                    {
                        if (_value == null)
                            return;
                    }
                    else
                    {
                        if (value.Equals(_value))
                            return;
                    }

                    _value = value;
                    OnPropertyChanged("Value");
                }
                else
                    new AccessViolationException(String.Format("The '{0}' named value is readonly thus cannot be modified", Name));
            }
        }

        public bool IsReadonly
        {
            get { return _isReadonly; }
            set { _isReadonly = value; }
        }

        #endregion Fields and Properties

        #region Constructors

        public KeyValuePairClass()
        { }

        public KeyValuePairClass(N name_in, V value_in)
        {
            Name = name_in;
            Value = value_in;
        }

        public KeyValuePairClass(N name_in, V value_in, bool isReadonly_in)
            : this(name_in, value_in)
        {
            IsReadonly = isReadonly_in;
        }

        #endregion Constructors

        #region Interface implementations

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        #endregion Interface implementations

        #region Overrides

        public override string ToString()
        {
            return String.Format("{0}: {1}", Name, Value);
        }

        #endregion Overrides
    }

    public class NamedValuePair : KeyValuePairClass<string, object>
    {
        #region Constructors

        public NamedValuePair() : base() { ;}

        public NamedValuePair(string name_in, object value_in) : base(name_in, value_in) { ;}

        public NamedValuePair(string name_in, object value_in, bool isReadonly_in) : base(name_in, value_in, isReadonly_in) { ;}

        #endregion Constructors
    }
}