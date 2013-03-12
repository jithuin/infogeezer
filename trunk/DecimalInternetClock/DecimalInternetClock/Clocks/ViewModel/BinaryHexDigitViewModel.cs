using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DecimalInternetClock.Helpers;

namespace DecimalInternetClock
{
    public class BinaryHexDigitViewModel : INotifyPropertyChanged
    {
        HexDigitModel _clock;

        public BinaryHexDigitViewModel()
        {
            _clock = new HexDigitModel();
        }

        public long Now
        {
            get
            {
                return _clock.Now;
            }
            set
            {
                _clock.Now = value;
                UpdateAllSubProperties();
            }
        }

        private void UpdateAllSubProperties()
        {
            foreach (HexDigitModel.EUnits unit in EnumHelper.GetValues<HexDigitModel.EUnits>())
                OnPropertyChanged(unit);
        }

        public bool First
        {
            get
            {
                return _clock[HexDigitModel.EUnits.First];
            }
            set
            {
                if (_clock[HexDigitModel.EUnits.First] != value)
                {
                    _clock[HexDigitModel.EUnits.First] = value;
                    OnPropertyChanged(HexDigitModel.EUnits.First);
                }
            }
        }

        public bool Second
        {
            get
            {
                return _clock[HexDigitModel.EUnits.Second];
            }
            set
            {
                if (_clock[HexDigitModel.EUnits.Second] != value)
                {
                    _clock[HexDigitModel.EUnits.Second] = value;
                    OnPropertyChanged(HexDigitModel.EUnits.Second);
                }
            }
        }

        public bool Third
        {
            get
            {
                return _clock[HexDigitModel.EUnits.Third];
            }
            set
            {
                if (_clock[HexDigitModel.EUnits.Third] != value)
                {
                    _clock[HexDigitModel.EUnits.Third] = value;
                    OnPropertyChanged(HexDigitModel.EUnits.Third);
                }
            }
        }

        public bool Fourth
        {
            get
            {
                return _clock[HexDigitModel.EUnits.Fourth];
            }
            set
            {
                if (_clock[HexDigitModel.EUnits.Fourth] != value)
                {
                    _clock[HexDigitModel.EUnits.Fourth] = value;
                    OnPropertyChanged(HexDigitModel.EUnits.Fourth);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(HexDigitModel.EUnits unit_in)
        {
            OnPropertyChanged(unit_in.ToString());
        }

        public void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}