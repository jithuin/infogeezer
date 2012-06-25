using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.NamedValues
{
    public class NamedValueList : ObservableCollection<NamedValuePair<string, object>>
    {
        public NamedValueList() : base() { ;}

        public void Add(string name_in, object value_in)
        {
            this.Add(new NamedValuePair<string, object>(name_in, value_in));
        }

        public void AddRange(IEnumerable<NamedValuePair<string, object>> other)
        {
            foreach (NamedValuePair<string, object> nvp in other)
                this.Add(nvp);
        }

        public void ModifyRange(IList<object> other)
        {
            if (other.Count() != this.Count)
                throw new ArgumentException("Only exactly the same number of arguments are acceptable in calling this function");

            for (int i = 0; i < this.Count; i++)
            {
                NamedValuePair<string, object> nvp = this[i];
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
                foreach (NamedValuePair<string, object> item in this)
                {
                    if (item.Name == index)
                        return item.Value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                foreach (NamedValuePair<string, object> item in this)
                {
                    if (item.Name == index)
                        item.Value = value;
                }
                throw new KeyNotFoundException();
            }
        }
    }
}