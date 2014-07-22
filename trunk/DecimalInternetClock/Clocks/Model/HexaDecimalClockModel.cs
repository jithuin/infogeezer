using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Model
{
    public class HexaDecimalClockModel : ClockBase<HexaDecimalClockModel.EUnits>
    {
        public enum EUnits
        {
            Hour,
            MinuteHi,
            MinuteLow,
            Second
        }

        public HexaDecimalClockModel()
        {
        }

        protected override void InitClockUnits()
        {
            AddBase(EUnits.Hour, 16);
            AddBase(EUnits.MinuteHi, 16);
            AddBase(EUnits.MinuteLow, 16);
            AddBase(EUnits.Second, 16);
        }
    }
}