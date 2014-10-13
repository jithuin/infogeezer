using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DecimalInternetClock.Helpers;
using HotKey;

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
            this.Exit += new ExitEventHandler(App_Exit);
            ResizerHotkeyList.Instance.Init();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            ResizerHotkeyList.Instance.Save();
        }
    }
}