using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Clocks.Model;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Clocks.ViewModel
{
    public class DecimalClockViewModel : ClockViewModelBase<DecimalClockModel, DecimalClockModel.EUnits>
    {
        /// <summary>
        ///
        /// </summary>
        [PreferredConstructor]
        public DecimalClockViewModel()
            : base()
        {
            TimeStringFormat = "{0}:{1:00}.{2:00}";
        }

        public DecimalClockViewModel(DecimalClockModel clock_in)
            : base(clock_in)
        {
            TimeStringFormat = "{0}:{1:00}.{2:00}";
        }
    }
}