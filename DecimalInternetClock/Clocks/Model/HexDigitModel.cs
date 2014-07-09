using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Model
{
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

        internal new long Now
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

        public HexDigitModel(long time_in)
            : base(time_in)
        {
            ;
        }

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
}