using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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

        #endregion

        #region INotifyPropertyChanged Members

        void OnBasePropertyChanged()
        {
            //UNDONE
        }


        void OnPropertyChanged(String propName_in)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName_in));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        
    }
}
