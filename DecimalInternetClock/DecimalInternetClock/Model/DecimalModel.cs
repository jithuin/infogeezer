﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DecimalInternetClock
{
    public class DecimalModel : IDecimalClock
    {
        #region IDecimalClock Members
        private double _decimalTime = 0;

        public DateTime Now
        {
            get
            {
                DateTime _date = DateTime.Now;
                return new DateTime(_date.Year, _date.Month, _date.Day, Hour, Minute, Second, MilliSecond);
                
            }
            set
            {
                double decimalValue = (double)value.Millisecond / (24.0 * 60.0 * 60.0 * 1000.0);
                decimalValue += (double)value.Second / (24.0 * 60.0 * 60.0);
                decimalValue += (double)value.Minute / (24.0 * 60.0);
                decimalValue += (double)value.Hour / (24.0);
                if (_decimalTime != decimalValue)
                {
                    _decimalTime = decimalValue;
                    OnBasePropertyChanged();
                }
            }
        }
        private int Hour
        {
            get
            {
                return (int)(_decimalTime * 24.0) % 24;
            }
        }
        private int Minute
        {
            get
            {
                return (int)(_decimalTime * 24.0 * 60.0) % 60;
            }
        }
        private int Second
        {
            get
            {
                return (int)(_decimalTime * 24.0 * 60.0 * 60.0) % 60;
            }
        }
        private int MilliSecond
        {
            get
            {
                return (int)(_decimalTime * 24.0 * 60.0 * 60.0 * 1000.0) % 1000;
            }
        }

        public double DecimalTime
        {
            get
            {
                return _decimalTime;
            }
            set
            {
                if (_decimalTime != value)
                {
                    _decimalTime = value;
                    OnBasePropertyChanged();
                }
            }
        }
        public int DecimalHour
        {
            get
            {
                return (int)(_decimalTime * 10.0) % 10;
            }
            private set
            {
                throw new NotImplementedException("DecimalHour private set");
            }
        }
        public int DecimalMin
        {
            get
            {
                return (int)(_decimalTime * 1000.0) % 100;
            }
            private set
            {
                throw new NotImplementedException("DecimalMin private set");
            }
        }
        public int DecimalSecond
        {
            get
            {
                return (int)(_decimalTime * 100000.0) % 100;
            }
            private set
            {
                throw new NotImplementedException("DecimalSecond private set");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        void OnBasePropertyChanged()
        {
            OnPropertyChanged("Now");
            OnPropertyChanged("DecimalTime");
            OnPropertyChanged("DecimalHour");
            OnPropertyChanged("DecimalMin");
            OnPropertyChanged("DecimalSecond");
        }


        void OnPropertyChanged(String propName_in)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName_in));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    /// <typeparam name="I">indexer type</typeparam>
    public class IndexMe<T, I>
    {
        public IndexMe()
        {

        }
        public IndexMe(Func<I,IndexMe<T, I>,T> myFunc, T baseData_in)
        {
            myGetfunc = myFunc;
            BaseData = baseData_in;
        }

        public T BaseData {get; set;}
        Func<I,IndexMe<T, I>,T>  myGetfunc;

        public T this[I index]
        {
            get { return myGetfunc(index, this); }
            set {  }
        }
    }

    public class Father
    {
        IndexMe<byte, int> Son = new IndexMe<byte, int>(new Func<int, IndexMe<byte, int>, byte>((i, Son) => { return (byte)((Son.BaseData >> i) & 0x1); }), 6);

        public byte this[int index]
        {
            get { return Son[index]; }
            set { /* set the specified index to value here */ }
        }
    }
}
