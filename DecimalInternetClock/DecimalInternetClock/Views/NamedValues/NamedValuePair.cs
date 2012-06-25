using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.NamedValues
{
    public class NamedValuePair<N, V>
    {
        #region Fields and Properties

        protected N _name;
        protected V _value;
        protected bool _isReadonly = false;

        public N Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public V Value
        {
            get { return _value; }
            set
            {
                if (!IsReadonly)
                    _value = value;
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

        public NamedValuePair()
        { }

        public NamedValuePair(N name_in, V value_in)
        {
            Name = name_in;
            Value = value_in;
        }

        #endregion Constructors
    }
}