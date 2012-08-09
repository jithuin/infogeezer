using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DecimalInternetClock.Helpers;
using DecimalInternetClock.HotKeys;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ResizeHotkeyList.Instance.SetToDefault();
        }
    }
}