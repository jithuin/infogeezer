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
        public App()
        {
            InitWinSplitFeatures();
        }

        private void InitWinSplitFeatures()
        {
            ResizerHotKey hk = new ResizerHotKey(KeyModifiers.Alt | KeyModifiers.Ctrl, System.Windows.Forms.Keys.Left);
            List<ResizeState> _resizeStates = new List<ResizeState>();
            _resizeStates.Add(new ResizeState(new Vector2D(0.0, 0.0), new Vector2D(0.5, 0.5)));
            hk.ResizeStates = _resizeStates;
        }
    }
}