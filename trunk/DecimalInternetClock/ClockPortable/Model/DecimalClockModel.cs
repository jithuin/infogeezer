using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Model
{
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
}