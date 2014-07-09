using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Model
{
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
}