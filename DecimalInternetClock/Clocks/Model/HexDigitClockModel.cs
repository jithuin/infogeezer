using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Model
{
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
}