using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DecimalInternetClock
{
    public class BinaryModel : IDecimalClock, INotifyPropertyChanged
    {
        #region IDecimalClock Members

        public DateTime Now
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion IDecimalClock Members

        #region INotifyPropertyChanged Members

        private void OnBasePropertyChanged()
        {
            //UNDONE
        }

        private void OnPropertyChanged(String propName_in)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName_in));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}