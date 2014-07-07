using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using DecimalInternetClock.HotKeys;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private String probeFileName = ".\\Options\\probe.bin";

        public App()
        {
        }
    }
}