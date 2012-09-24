using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using DecimalInternetClock.Helpers;
using ManagedWinapi;

namespace DecimalInternetClock.HotKeys
{
    [Flags()]
    public enum FKeyModifiers
    {
        Alt = 0x0001,
        Ctrl = 0x0010,
        Shift = 0x0100,
        Win = 0x1000,
    }

    [Serializable]
    public abstract class HotKeyFeatureExtension : IList<Hotkey>, IList
    {
        #region Properties and Fields

        protected List<Hotkey> _hotkeyList;

        #endregion Properties and Fields

        #region Constructors

        public HotKeyFeatureExtension()
        {
            _hotkeyList = new List<Hotkey>();
        }

        public HotKeyFeatureExtension(FKeyModifiers mod_in, Keys key_in)
            : this()
        {
            this.Add(mod_in, key_in);
        }

        #endregion Constructors

        #region Methods

        protected abstract void HotKeyFeatureExtension_HotkeyPressed(object sender, EventArgs e);

        public void Add(FKeyModifiers mod_in, Keys key_in)
        {
            Hotkey hotkey = new Hotkey();
            hotkey.ModifyHotKey(mod_in, key_in);
            this.Add(hotkey);
        }

        #endregion Methods

        #region Interface implementations

        #region IList<Hotkey> Members

        public int IndexOf(Hotkey item)
        {
            return _hotkeyList.IndexOf(item);
        }

        public void Insert(int index, Hotkey item)
        {
            item.HotkeyPressed += new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
            _hotkeyList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _hotkeyList[index].HotkeyPressed -= new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
            _hotkeyList.RemoveAt(index);
        }

        public Hotkey this[int index]
        {
            get
            {
                return _hotkeyList[index];
            }
            set
            {
                if (value != _hotkeyList[index])
                {
                    _hotkeyList[index].HotkeyPressed -= new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
                    _hotkeyList[index] = value;
                    _hotkeyList[index].HotkeyPressed += new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
                }
            }
        }

        #endregion IList<Hotkey> Members

        #region ICollection<Hotkey> Members

        public void Add(Hotkey item)
        {
            item.HotkeyPressed += new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
            _hotkeyList.Add(item);
        }

        public void Clear()
        {
            foreach (Hotkey hk in _hotkeyList)
                hk.HotkeyPressed -= new EventHandler(HotKeyFeatureExtension_HotkeyPressed);

            _hotkeyList.Clear();
        }

        public bool Contains(Hotkey item)
        {
            return _hotkeyList.Contains(item);
        }

        public void CopyTo(Hotkey[] array, int arrayIndex)
        {
            _hotkeyList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _hotkeyList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Hotkey item)
        {
            if (_hotkeyList.Remove(item))
            {
                item.HotkeyPressed -= new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
                return true;
            }
            else
                return false;
        }

        #endregion ICollection<Hotkey> Members

        #region IEnumerable<Hotkey> Members

        public IEnumerator<Hotkey> GetEnumerator()
        {
            return _hotkeyList.GetEnumerator();
        }

        #endregion IEnumerable<Hotkey> Members

        #region IList Members

        public int Add(object value)
        {
            this.Add((Hotkey)value);
            return this.Count - 1;
        }

        public bool Contains(object value)
        {
            return this.Contains((Hotkey)value);
        }

        public int IndexOf(object value)
        {
            return this.IndexOf((Hotkey)value);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, (Hotkey)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.Remove((Hotkey)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (Hotkey)value;
            }
        }

        #endregion IList Members

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            this.CopyTo(array.Cast<Hotkey>().ToArray(), index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion ICollection Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_hotkeyList).GetEnumerator();
        }

        #endregion IEnumerable Members

        #endregion Interface implementations
    }

    public class HotKeyStore : List<Hotkey>
    {
        protected HotKeyStore() { }

        protected HotKeyStore _instance;

        public HotKeyStore Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HotKeyStore();
                return _instance;
            }
        }
    }
}