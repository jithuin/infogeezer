using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using ManagedWinapi;

namespace DecimalInternetClock.HotKeys
{
    public abstract class HotKeyFeatureExtension : Hotkey
    {
        protected Window _win;

        public HotKeyFeatureExtension()
        {
            this.HotkeyPressed += new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
        }

        public HotKeyFeatureExtension(Window window_in, KeyModifiers mod_in, Keys key_in)
            : this()
        {
            _win = window_in;
            ModifyHotKey(mod_in, key_in);
        }

        public void ModifyHotKey(KeyModifiers mod_in, Keys key_in)
        {
            this.Enabled = false;
            SetKeyModifiers(mod_in);
            this.KeyCode = key_in;
            this.Enabled = true;
        }

        private void SetKeyModifiers(KeyModifiers mod_in)
        {
            Alt = (mod_in & KeyModifiers.Alt) != 0;
            Ctrl = (mod_in & KeyModifiers.Ctrl) != 0;
            Shift = (mod_in & KeyModifiers.Shift) != 0;
            WindowsKey = (mod_in & KeyModifiers.Win) != 0;
        }

        protected abstract void HotKeyFeatureExtension_HotkeyPressed(object sender, EventArgs e);
    }
}