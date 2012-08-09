using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.HotKeys
{
    public class ResizeHotkeyList : IList<ResizerHotKey>
    {
        protected List<ResizerHotKey> _rhkList = null;

        protected static ResizeHotkeyList _instance = null;

        public static ResizeHotkeyList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ResizeHotkeyList();
                return _instance;
            }
        }

        protected ResizeHotkeyList()
        {
            _rhkList = new List<ResizerHotKey>();
        }

        private void hk_CurrentWindowChanged(object sender, EventArgs e)
        {
            foreach (ResizerHotKey rhk in this)
                rhk.ChangeCurrentWindow();
        }

        #region IList<ResizerHotKey> Members

        public int IndexOf(ResizerHotKey item)
        {
            return _rhkList.IndexOf(item);
        }

        public void Insert(int index, ResizerHotKey item)
        {
            item.CurrentWindowChanged += new EventHandler(hk_CurrentWindowChanged);
            _rhkList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _rhkList[index].CurrentWindowChanged += new EventHandler(hk_CurrentWindowChanged);
            _rhkList.RemoveAt(index);
        }

        public ResizerHotKey this[int index]
        {
            get
            {
                return _rhkList[index];
            }
            set
            {
                if (_rhkList[index] != value)
                {
                    _rhkList[index].CurrentWindowChanged -= new EventHandler(hk_CurrentWindowChanged);
                    _rhkList[index] = value;
                    _rhkList[index].CurrentWindowChanged += new EventHandler(hk_CurrentWindowChanged);
                }
            }
        }

        #endregion IList<ResizerHotKey> Members

        #region ICollection<ResizerHotKey> Members

        public new void Add(ResizerHotKey item)
        {
            item.CurrentWindowChanged += new EventHandler(hk_CurrentWindowChanged);
            _rhkList.Add(item);
        }

        public void Clear()
        {
            foreach (ResizerHotKey rhk in _rhkList)
                rhk.CurrentWindowChanged -= new EventHandler(hk_CurrentWindowChanged);
            _rhkList.Clear();
        }

        public bool Contains(ResizerHotKey item)
        {
            return _rhkList.Contains(item);
        }

        public void CopyTo(ResizerHotKey[] array, int arrayIndex)
        {
            _rhkList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _rhkList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ResizerHotKey item)
        {
            if (_rhkList.Remove(item))
            {
                item.CurrentWindowChanged -= new EventHandler(hk_CurrentWindowChanged);
                return true;
            }
            else
                return false;
        }

        #endregion ICollection<ResizerHotKey> Members

        #region IEnumerable<ResizerHotKey> Members

        public IEnumerator<ResizerHotKey> GetEnumerator()
        {
            return _rhkList.GetEnumerator();
        }

        #endregion IEnumerable<ResizerHotKey> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_rhkList).GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}