﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clocks.Model;

namespace Clocks.Tester
{
    public class ClockTester
    {
        public ClockTester()
        {
            DecimalClockModel dcm = new DecimalClockModel();
            OrdinaryClockModel ocm = new OrdinaryClockModel();
            HexaDecimalClockModel hdcm = new HexaDecimalClockModel();

            dcm.Now = DateTime.Now;
            ocm.Now = DateTime.Now;
            hdcm.Now = DateTime.Now;

            long res = hdcm[HexaDecimalClockModel.EUnits.Hour];
            res = dcm[DecimalClockModel.EUnits.Hour];
            res = ocm[OrdinaryClockModel.EUnits.Hour];
        }
    }
}