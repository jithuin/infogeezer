using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using ManagedWinapi;

namespace DecimalInternetClock.HotKeys
{
    public class ActivatorHotkey : HotKeyFeatureExtension
    {
        protected Window _win;

        public ActivatorHotkey() { }

        public ActivatorHotkey(Window window_in, FKeyModifiers mod_in, Keys key_in)
            : base(mod_in, key_in)
        {
            _win = window_in;
            _win.Deactivated += new EventHandler(Window1_Deactivated);
        }

        private void Window1_Deactivated(object sender, EventArgs e)
        {
            _win.Hide();
        }

        protected override void HotKeyFeatureExtension_HotkeyPressed(object sender, EventArgs e)
        {
            _win.Show();
            _win.Activate();
        }
    }
}