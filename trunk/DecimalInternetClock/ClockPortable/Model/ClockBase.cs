using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelpersPortable.Exceptions;

namespace Clocks.Model
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

        internal virtual DateTime Now
        {
            set
            {
                _time = OrdinaryClockModel.GetTime(value);
            }
        }

        public void UpdateNow()
        {
            Now = DateTime.Now;
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

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="E">The E type must be some kind of enum type</typeparam>
    public abstract class ClockBase<E> : ClockBase
        where E : struct, IComparable, IFormattable
    {
        public ClockBase()
        {
            //if (!typeof(E).IsEnum)
            //    throw new XArchitectureException();
            TestGenericTypeEnum();
        }

        public ClockBase(double time_in)
            : base(time_in)
        {
            //if (!typeof(E).IsEnum)
            //    throw new XArchitectureException();
            TestGenericTypeEnum();
        }

        private static void TestGenericTypeEnum()
        {
            Enum.GetValues(typeof(E)).Cast<E>(); // HACK: Testing the E generic type if it is an Enum type
        }



        protected void AddBase(E lowerUnit_in, long base_in)
        {
            _listOfBases.Add(Convert.ToInt32(lowerUnit_in), base_in);
        }

        public long this[E index]
        {
            get { return base[Convert.ToInt32(index)]; }
            set { base[Convert.ToInt32(index)] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (E index in Enum.GetValues(typeof(E)).Cast<E>())
            {
                sb.AppendFormat("{0}:{1}, ", index, this[index]);
            }
            return sb.ToString();
        }
    }
}