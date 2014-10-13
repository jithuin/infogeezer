using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using ManagedWinapi;

namespace HotKey
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
    [XmlInclude(typeof(HotKeyFeatureExtension))]
    public abstract class HotKeyFeatureExtension
    {
        #region Properties and Fields

        protected List<HotkeyProxy> _hotkeyList;

        [Obsolete("This property is only for xml purposes it should be deleted")]
        [XmlArray("hotkeyList")]
        public List<HotkeyProxy> HotkeyList
        {
            get
            {
                return _hotkeyList;
            }
            set
            {
                _hotkeyList = value;
            }
        }

        #endregion Properties and Fields

        #region Constructors

        public HotKeyFeatureExtension()
        {
            _hotkeyList = new List<HotkeyProxy>();
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
            HotkeyProxy hotkey = new HotkeyProxy();
            hotkey.ModifyHotKey(mod_in, key_in);
            this.Add(hotkey);
        }

        #endregion Methods

        #region Interface implementations

        #region IList<Hotkey> Members

        public int IndexOf(HotkeyProxy item)
        {
            return _hotkeyList.IndexOf(item);
        }

        public void Insert(int index, HotkeyProxy item)
        {
            item.HotkeyPressed += new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
            _hotkeyList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _hotkeyList[index].HotkeyPressed -= new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
            _hotkeyList.RemoveAt(index);
        }

        public HotkeyProxy this[int index]
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

        public void Add(HotkeyProxy item)
        {
            item.HotkeyPressed += new EventHandler(HotKeyFeatureExtension_HotkeyPressed);
            _hotkeyList.Add(item);
        }

        public void Clear()
        {
            foreach (HotkeyProxy hk in _hotkeyList)
                hk.HotkeyPressed -= new EventHandler(HotKeyFeatureExtension_HotkeyPressed);

            _hotkeyList.Clear();
        }

        public bool Contains(HotkeyProxy item)
        {
            return _hotkeyList.Contains(item);
        }

        public void CopyTo(HotkeyProxy[] array, int arrayIndex)
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

        public bool Remove(HotkeyProxy item)
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

        public IEnumerator<HotkeyProxy> GetEnumerator()
        {
            return _hotkeyList.GetEnumerator();
        }

        #endregion IEnumerable<Hotkey> Members

        #region IList Members

        public int Add(object value)
        {
            this.Add((HotkeyProxy)value);
            return this.Count - 1;
        }

        public bool Contains(object value)
        {
            return this.Contains((HotkeyProxy)value);
        }

        public int IndexOf(object value)
        {
            return this.IndexOf((HotkeyProxy)value);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, (HotkeyProxy)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.Remove((HotkeyProxy)value);
        }

        #endregion IList Members

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            this.CopyTo(array.Cast<ManagedWinapi.Hotkey>().ToArray(), index);
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

        #endregion Interface implementations
    }
}