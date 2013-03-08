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
        String probeFileName = ".\\Options\\probe.bin";

        public App()
        {
            new ClockTester();
            ResizerHotkeyList.Instance.SetToDefault();
            //ResizerHotkeyList.Instance.BinSerializeThisTo(probeFileName);
            //ResizerHotkeyList.Instance.Clear();
            //ResizerHotkeyList.Instance.BinDeserializeThisFrom(probeFileName);
            //ResizerHotkeyList.Instance.SerializeThisTo(probeFileName);
            //ResizerHotkeyList.Instance.Clear();
            //ResizerHotkeyList.Instance.DeserializeThisFrom(probeFileName);
        }
    }
}