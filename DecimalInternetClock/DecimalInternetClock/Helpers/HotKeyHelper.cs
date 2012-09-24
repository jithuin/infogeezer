using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DecimalInternetClock.HotKeys;
using ManagedWinapi;

namespace DecimalInternetClock.Helpers
{
    public static class HotKeyHelper
    {
        public static void ModifyHotKey(this Hotkey hotkey_in, FKeyModifiers mod_in, Keys key_in)
        {
            hotkey_in.Enabled = false;
            hotkey_in.SetKeyModifiers(mod_in);
            hotkey_in.KeyCode = key_in;
            hotkey_in.Enabled = true;
        }

        private static void SetKeyModifiers(this Hotkey hotkey_in, FKeyModifiers mod_in)
        {
            hotkey_in.Alt = (mod_in & FKeyModifiers.Alt) != 0;
            hotkey_in.Ctrl = (mod_in & FKeyModifiers.Ctrl) != 0;
            hotkey_in.Shift = (mod_in & FKeyModifiers.Shift) != 0;
            hotkey_in.WindowsKey = (mod_in & FKeyModifiers.Win) != 0;
        }
    }
}