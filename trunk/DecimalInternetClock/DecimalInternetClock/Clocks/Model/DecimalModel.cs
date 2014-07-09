using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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

        #endregion IDecimalClock Members

        #region INotifyPropertyChanged Members

        private void OnBasePropertyChanged()
        {
            OnPropertyChanged("Now");
            OnPropertyChanged("DecimalTime");
            OnPropertyChanged("DecimalHour");
            OnPropertyChanged("DecimalMin");
            OnPropertyChanged("DecimalSecond");
        }

        private void OnPropertyChanged(String propName_in)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName_in));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="D">data type</typeparam>
    /// <typeparam name="I">indexer type</typeparam>
    /// <typeparam name="R">return type</typeparam>
    public class IndexMe<D, I, R>
    {
        public IndexMe()
        {
            BaseData = (D)(object)0;
            myGetfunc = new Func<I, IndexMe<D, I, R>, R>((index, data) => { return (R)Convert.ChangeType(data.BaseData, typeof(R)); });
        }

        public IndexMe(Func<I, IndexMe<D, I, R>, R> myFunc, D baseData_in)
        {
            myGetfunc = myFunc;
            BaseData = baseData_in;
        }

        public D BaseData { get; set; }

        private Func<I, IndexMe<D, I, R>, R> myGetfunc;

        public R this[I index]
        {
            get { return myGetfunc(index, this); }
            set { }
        }
    }

    public class IndexMeUnitTester
    {
        protected static int _LengthOfByteInBits = 8;
        protected static int _ByteMask = 0xFF;
        protected static int _BitMask = 0x1;

        public IndexMeUnitTester()
        {
            Func<int, IndexMe<byte, int, bool>, bool> flagFunc = new Func<int, IndexMe<byte, int, bool>, bool>((i, Son) => { return ((Son.BaseData >> i) & _BitMask) != 0; });
            IndexMe<byte, int, bool> SonFlag = new IndexMe<byte, int, bool>(flagFunc, 6);
            Func<int, IndexMe<byte, int, bool>, bool> lesserFunc = new Func<int, IndexMe<byte, int, bool>, bool>((i, son) => { return son.BaseData <= i; });
            IndexMe<byte, int, bool> SonLesser = new IndexMe<byte, int, bool>(lesserFunc, 2);

            IndexMe<int, int, byte> SonPart = new IndexMe<int, int, byte>(new Func<int, IndexMe<int, int, byte>, byte>((index, item) => { return (byte)((item.BaseData >> (index * _LengthOfByteInBits)) & _ByteMask); }), 4);
            IndexMe<int, int, IndexMe<byte, int, bool>> SonSandwich =
                new IndexMe<int, int, IndexMe<byte, int, bool>>(
                    new Func<int, IndexMe<int, int, IndexMe<byte, int, bool>>, IndexMe<byte, int, bool>>(
                        (index, item) =>
                        {
                            return new IndexMe<byte, int, bool>(flagFunc, (byte)((item.BaseData >> (index * _LengthOfByteInBits)) & _ByteMask));
                        }), 7);
        }
    }
}