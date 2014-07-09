using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DecimalInternetClock.Properties;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace DecimalInternetClock.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        internal Settings Settings
        {
            get
            {
                return Settings.Default;
            }
        }
    }
}