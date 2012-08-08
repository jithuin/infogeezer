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
            ResizerHotKey hk = new ResizerHotKey();
            hk.Add(FKeyModifiers.Alt | FKeyModifiers.Ctrl, System.Windows.Forms.Keys.Left);
            hk.Add(FKeyModifiers.Alt | FKeyModifiers.Ctrl, System.Windows.Forms.Keys.NumPad4);

            List<ResizerState> _resizeStates = new List<ResizerState>();
            _resizeStates.Add(new ResizerState(new Vector(0.0, 0.0), new Vector(0.5, 1)));
            _resizeStates.Add(new ResizerState(new Vector(0.0, 0.0), new Vector(0.3, 1)));
            _resizeStates.Add(new ResizerState(new Vector(0.0, 0.0), new Vector(0.2, 1)));
            hk.ResizeStates = _resizeStates;
        }
    }
}