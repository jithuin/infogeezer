using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DecimalInternetClock.Helpers;

namespace DecimalInternetClock
{
    public class BinaryHexDigitClockViewModel : INotifyPropertyChanged
    {
        HexDigitClockModel _clock;

        public DateTime Now
        {
            set
            {
                _clock.Now = value;
                UpdateSubViewModels();
            }
        }

        public BinaryHexDigitClockViewModel()
        {
            _clock = new HexDigitClockModel();
            InitSubViewModels();
        }

        private void InitSubViewModels()
        {
            _subViewModels = new Dictionary<HexDigitClockModel.EUnits, BinaryHexDigitViewModel>();
            foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
            {
                _subViewModels.Add(unit, new BinaryHexDigitViewModel());
                _subViewModels[unit].PropertyChanged += new PropertyChangedEventHandler(BinaryHexDigitClockViewModel_PropertyChanged);
            }
        }

        private void BinaryHexDigitClockViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateFromSubViewModels();
        }

        private void UpdateFromSubViewModels()
        {
            foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
                if (_subViewModels[unit].Now != _clock[unit])
                {
                    _clock[unit] = _subViewModels[unit].Now;
                    OnPropertyChanged(unit);
                }
        }

        private void UpdateSubViewModels()
        {
            foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
            {
                _subViewModels[unit].Now = _clock[unit];
                OnPropertyChanged(unit);
            }
        }

        public void UpdateNow()
        {
            Now = DateTime.Now;
        }

        Dictionary<HexDigitClockModel.EUnits, BinaryHexDigitViewModel> _subViewModels;

        public BinaryHexDigitViewModel Hour
        {
            get
            {
                return _subViewModels[HexDigitClockModel.EUnits.Hour];
            }
        }

        public BinaryHexDigitViewModel MinuteHi
        {
            get
            {
                return _subViewModels[HexDigitClockModel.EUnits.MinuteHi];
            }
        }

        public BinaryHexDigitViewModel MinuteLow
        {
            get
            {
                return _subViewModels[HexDigitClockModel.EUnits.MinuteLow];
            }
        }

        public BinaryHexDigitViewModel Second
        {
            get
            {
                return _subViewModels[HexDigitClockModel.EUnits.Second];
            }
        }

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(HexDigitClockModel.EUnits unit_in)
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