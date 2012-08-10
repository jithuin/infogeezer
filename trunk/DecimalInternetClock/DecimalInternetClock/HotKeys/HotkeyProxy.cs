using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedWinapi;

namespace DecimalInternetClock.HotKeys
{
    [Serializable]
    public class HotkeyProxy : IHotkey
    {
        [NonSerialized]
        protected Hotkey _hotkey;

        public HotkeyProxy()
        {
            _hotkey = new Hotkey();
        }

        #region IHotkey Members

        public bool Alt
        {
            get
            {
                return _hotkey.Alt;
            }
            set
            {
                _hotkey.Alt = value;
            }
        }

        public bool Ctrl
        {
            get
            {
                return _hotkey.Ctrl;
            }
            set
            {
                _hotkey.Ctrl = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _hotkey.Enabled;
            }
            set
            {
                _hotkey.Enabled = value;
            }
        }

        public event EventHandler HotkeyPressed
        {
            add
            {
                _hotkey.HotkeyPressed += value;
            }
            remove
            {
                _hotkey.HotkeyPressed -= value;
            }
        }

        public System.Windows.Forms.Keys KeyCode
        {
            get
            {
                return _hotkey.KeyCode;
            }
            set
            {
                _hotkey.KeyCode = value;
            }
        }

        public bool Shift
        {
            get
            {
                return _hotkey.Shift;
            }
            set
            {
                _hotkey.Shift = value;
            }
        }

        public bool WindowsKey
        {
            get
            {
                return _hotkey.WindowsKey;
            }
            set
            {
                _hotkey.WindowsKey = value;
            }
        }

        #endregion IHotkey Members
    }
}