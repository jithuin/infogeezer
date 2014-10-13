using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Forms = System.Windows.Forms;

namespace HotKey
{
    [Serializable]
    public class HotkeyProxy
    {
        [NonSerialized]
        [XmlIgnore]
        protected ManagedWinapi.Hotkey _hotkey;

        public HotkeyProxy()
        {
            _hotkey = new ManagedWinapi.Hotkey();
        }

        #region Properties

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

        #endregion Properties

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

        internal void ModifyHotKey(FKeyModifiers mod_in, Forms.Keys key_in)
        {
            Enabled = false;
            Alt = (mod_in & FKeyModifiers.Alt) != 0;
            Ctrl = (mod_in & FKeyModifiers.Ctrl) != 0;
            Shift = (mod_in & FKeyModifiers.Shift) != 0;
            WindowsKey = (mod_in & FKeyModifiers.Win) != 0;
            KeyCode = key_in;
            Enabled = true;
        }
    }
}