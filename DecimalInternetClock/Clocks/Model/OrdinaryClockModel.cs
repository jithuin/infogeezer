using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Model
{
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
}