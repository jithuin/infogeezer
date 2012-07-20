using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using ManagedWinapi;

namespace DecimalInternetClock.HotKeys
{
    [Flags()]
    public enum KeyModifiers
    {
        Alt = 0x0001,
        Ctrl = 0x0010,
        Shift = 0x0100,
        Win = 0x1000,
    }

    public class ActivatorHotkey : HotKeyFeatureExtension
    {
        public ActivatorHotkey() { }

        public ActivatorHotkey(Window window_in, KeyModifiers mod_in, Keys key_in)
            : base(window_in, mod_in, key_in)
        {
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