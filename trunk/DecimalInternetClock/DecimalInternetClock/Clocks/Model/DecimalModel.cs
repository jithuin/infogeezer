using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DecimalInternetClock
{
    public abstract class ClockBase
    {
        protected Dictionary<int, double> _listOfBases = new Dictionary<int, double>();
        protected double _time = 0;

        //public double Time
        //{
        //    get
        //    {
        //        return _time;
        //    }
        //    set
        //    {
        //        _time = value;
        //    }
        //}

        public virtual DateTime Now
        {
            set
            {
                _time = OrdinaryClockModel.GetTime(value);
            }
        }

        public ClockBase()
        {
            InitClockUnits();
        }

        public ClockBase(double time_in)
        {
            _time = time_in;
        }

        protected double GetGeneratorAtIndex(int generatorIndex_in)
        {
            return _listOfBases.Aggregate((double)1, new Func<double, KeyValuePair<int, double>, double>((seed, kvp) => kvp.Key <= generatorIndex_in ? seed * kvp.Value : seed));
        }

        protected abstract void InitClockUnits();

        public long this[int index]
        {
            get
            {
                double ret = _time;
                ret *= GetGeneratorAtIndex(index);
                ret %= _listOfBases[index];

                return (long)ret;
            }
            set
            {
                _time = _time + (value - this[index]) / GetGeneratorAtIndex(index);// TODO: Update
            }
        }
    }

    public abstract class ClockBase<E> : ClockBase
        where E : struct, IConvertible, IComparable
    {
        public ClockBase()
        {
        }

        public ClockBase(double time_in)
            : base(time_in)
        {
        }

        protected void AddBase(E lowerUnit_in, long base_in)
        {
            _listOfBases.Add(lowerUnit_in.ToInt32(null), base_in);
        }

        public long this[E index]
        {
            get { return base[index.ToInt32(null)]; }
            set { base[index.ToInt32(null)] = value; }
        }
    }

    public class DecimalClockModel : ClockBase<DecimalClockModel.EUnits>
    {
        public enum EUnits
        {
            Hour,
            Minute,
            Second,
            MilliSecond
        }

        public DecimalClockModel(long time_in)
            : base(time_in)
        {
        }

        public DecimalClockModel()
        {
        }

        protected override void InitClockUnits()
        {
            AddBase(EUnits.Hour, 10);
            AddBase(EUnits.Minute, 100);
            AddBase(EUnits.Second, 100);
            AddBase(EUnits.MilliSecond, 1000);
        }
    }

    public class OrdinaryClockModel : ClockBase<OrdinaryClockModel.EUnits>
    {
        public OrdinaryClockModel(DateTime dateTime_in)
            : this()
        {
            _time = GetTime(dateTime_in);
        }

        private static OrdinaryClockModel _convertClock = new OrdinaryClockModel(); // for performance issue

        public enum EUnits
        {
            Hour,
            Minute,
            Second,
            MilliSecond
        }

        public OrdinaryClockModel()
        {
        }

        protected override void InitClockUnits()
        {
            AddBase(EUnits.Hour, 24);
            AddBase(EUnits.Minute, 60);
            AddBase(EUnits.Second, 60);
            AddBase(EUnits.MilliSecond, 1000);
        }

        internal static double GetTime(DateTime dateTime_in)
        {
            _convertClock[EUnits.Hour] = dateTime_in.Hour;
            _convertClock[EUnits.Minute] = dateTime_in.Minute;
            _convertClock[EUnits.Second] = dateTime_in.Second;
            _convertClock[EUnits.MilliSecond] = dateTime_in.Millisecond;
            return _convertClock._time;
        }
    }

    public class HexDigitModel : ClockBase<HexDigitModel.EUnits>
    {
        public enum EUnits
        {
            First,
            Second,
            Third,
            Fourth
        }

        public const int cMaxValue = 16;

        public new long Now
        {
            get
            {
                return (long)(_time * cMaxValue);
            }
            set
            {
                _time = ((value % cMaxValue) / (double)cMaxValue);
            }
        }

        public HexDigitModel(long time_in) : base(time_in) { ;}

        public HexDigitModel()
        {
        }

        protected override void InitClockUnits()
        {
            AddBase(EUnits.First, 2);
            AddBase(EUnits.Second, 2);
            AddBase(EUnits.Third, 2);
            AddBase(EUnits.Fourth, 2);
        }

        public new bool this[HexDigitModel.EUnits index]
        {
            get { return base[index] == 1; }

            set { base[index] = value ? 1 : 0; }
        }
    }

    public class HexTextClockModel : ClockBase<HexTextClockModel.EUnits>
    {
        public enum EUnits
        {
            Hour,
            Minute,
            Second
        }

        public HexTextClockModel()
        {
        }

        protected override void InitClockUnits()
        {
            AddBase(EUnits.Hour, 16);
            AddBase(EUnits.Minute, 256);
            AddBase(EUnits.Second, 16);
        }
    }

    public class HexDigitClockModel : ClockBase<HexDigitClockModel.EUnits>
    {
        public enum EUnits
        {
            Hour,
            MinuteHi,
            MinuteLow,
            Second
        }

        public HexDigitClockModel()
        {
        }

        //public new HexDigitModel this[EUnits index]
        //{
        //    get { return new HexDigitModel(base[index]); }
        //    set { base[index] = value.DecimalTime; }
        //}

        protected override void InitClockUnits()
        {
            AddBase(EUnits.Hour, 16);
            AddBase(EUnits.MinuteHi, 16);
            AddBase(EUnits.MinuteLow, 16);
            AddBase(EUnits.Second, 16);
        }
    }

    public class ClockTester
    {
        public ClockTester()
        {
            DecimalClockModel dcm = new DecimalClockModel();
            OrdinaryClockModel ocm = new OrdinaryClockModel();
            HexDigitClockModel hdcm = new HexDigitClockModel();

            dcm.Now = DateTime.Now;
            ocm.Now = DateTime.Now;
            hdcm.Now = DateTime.Now;

            long res = hdcm[HexDigitClockModel.EUnits.Hour];
            res = dcm[DecimalClockModel.EUnits.Hour];
            res = ocm[OrdinaryClockModel.EUnits.Hour];
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------

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

        Func<I, IndexMe<D, I, R>, R> myGetfunc;

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