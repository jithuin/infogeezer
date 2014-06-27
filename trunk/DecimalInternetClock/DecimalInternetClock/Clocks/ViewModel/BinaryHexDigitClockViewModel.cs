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
        #region Fields

        private HexDigitClockModel _clock;
        private Dictionary<HexDigitClockModel.EUnits, BinaryHexDigitViewModel> _subViewModels;

        #endregion Fields

        #region Public Properties

        public DateTime Now
        {
            set
            {
                _clock.Now = value;
                foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
                {
                    _subViewModels[unit].Now = _clock[unit];
                    OnPropertyChanged(unit);
                }
            }
        }

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

        #endregion Public Properties

        #region Constructor

        public BinaryHexDigitClockViewModel()
        {
            _clock = new HexDigitClockModel();
            //Init Sub-View-Models
            _subViewModels = new Dictionary<HexDigitClockModel.EUnits, BinaryHexDigitViewModel>();
            foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
            {
                _subViewModels.Add(unit, new BinaryHexDigitViewModel());
                _subViewModels[unit].PropertyChanged += new PropertyChangedEventHandler(
                    (sender, e) =>
                    {
                        //foreach (HexDigitClockModel.EUnits _unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
                        if (_subViewModels[unit].Now != _clock[unit])
                        {
                            _clock[unit] = _subViewModels[unit].Now;
                            OnPropertyChanged(unit);
                        }
                    }
                    );
            }
        }

        #endregion Constructor

        #region Methods

        //private void BinaryHexDigitClockViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
        //        if (_subViewModels[unit].Now != _clock[unit])
        //        {
        //            _clock[unit] = _subViewModels[unit].Now;
        //            OnPropertyChanged(unit);
        //        }
        //}

        public void UpdateNow()
        {
            Now = DateTime.Now;
        }

        #endregion Methods

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