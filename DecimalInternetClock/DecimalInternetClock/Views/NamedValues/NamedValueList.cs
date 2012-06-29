using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.NamedValues
{
    public class NamedValueList : ObservableCollection<NamedValuePair>
    {
        public NamedValueList() : base() { ;}

        public void Add(string name_in, object value_in)
        {
            this.Add(new NamedValuePair(name_in, value_in));
        }

        public void AddRange(IEnumerable<NamedValuePair> other)
        {
            foreach (NamedValuePair nvp in other)
                this.Add(nvp);
        }

        public void RemoveAnyByName(string name_in)
        {
            List<NamedValuePair> toBeRemoved = new List<NamedValuePair>();
            foreach (NamedValuePair nvp in this)
            {
                if (name_in == nvp.Name)
                    toBeRemoved.Add(nvp);
            }

            foreach (NamedValuePair nvp in toBeRemoved)
            {
                this.Remove(nvp);
            }
        }

        public void ModifyRange(IList<object> other)
        {
            if (other.Count() != this.Count)
                throw new ArgumentException("Only exactly the same number of arguments are acceptable in calling this function");

            for (int i = 0; i < this.Count; i++)
            {
                NamedValuePair nvp = this[i];
                bool isReadonly = nvp.IsReadonly; //HACK: the invoke result must be written back into the list
                nvp.IsReadonly = false;
                nvp.Value = other[i];
                nvp.IsReadonly = isReadonly;
            }
        }

        public object this[string index]
        {
            get
            {
                foreach (NamedValuePair item in this)
                {
                    if (item.Name == index)
                        return item.Value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                foreach (NamedValuePair item in this)
                {
                    if (item.Name == index)
                        item.Value = value;
                }
                throw new KeyNotFoundException();
            }
        }
    }
}