using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DecimalInternetClock.Helpers;

namespace DecimalInternetClock.Clocks.ViewModel
{
    public class BinaryHexDigitClockViewModel : INotifyPropertyChanged
    {
        #region Fields

        private HexDigitClockModel _clock;
        private Dictionary<HexDigitClockModel.EUnits, BinaryHexDigitViewModel> _subViewModels;

        #endregion Fields

        #region Public Properties

        public bool IsReadonly { get; set; }

        public DateTime Now
        {
            set
            {
                _clock.Now = value;
                foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
                {
                    if (_subViewModels[unit].Now != _clock[unit])
                    {
                        _subViewModels[unit].Now = _clock[unit];
                        OnPropertyChanged(unit);
                    }
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

        public string ForegroundPropertyName = "Foreground";
        protected Brush _foreground = Brushes.Black;

        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    foreach (HexDigitClockModel.EUnits unit in Enum.GetValues(typeof(HexDigitClockModel.EUnits)))
                        _subViewModels[unit].Foreground = _foreground;
                    OnPropertyChanged(ForegroundPropertyName);
                }
            }
        }

        public string StrokeThicknessPropertyName = "StrokeThickness";
        protected double _strokeThickness = 1;

        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness != value)
                {
                    _strokeThickness = value;
                    foreach (HexDigitClockModel.EUnits unit in Enum.GetValues(typeof(HexDigitClockModel.EUnits)))
                    {
                        _subViewModels[unit].StrokeThickness = _strokeThickness;
                        _subViewModels[unit].Margin = new Thickness(_strokeThickness);
                    }
                    OnPropertyChanged(StrokeThicknessPropertyName);
                }
            }
        }

        #endregion Public Properties

        #region Constructor

        public BinaryHexDigitClockViewModel()
        {
            _clock = new HexDigitClockModel();
            IsReadonly = true;
            //Init Sub-View-Models
            _subViewModels = new Dictionary<HexDigitClockModel.EUnits, BinaryHexDigitViewModel>();
            foreach (HexDigitClockModel.EUnits unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
            {
                _subViewModels.Add(unit, new BinaryHexDigitViewModel());

                _subViewModels[unit].PropertyChanged += new PropertyChangedEventHandler(
                    (sender, e) =>
                    {
                        //foreach (HexDigitClockModel.EUnits _unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
                        if (!IsReadonly && sender != this && _subViewModels[unit].Now != _clock[unit])
                        {
                            _clock[unit] = _subViewModels[unit].Now;
                            OnPropertyChanged(sender, unit);
                        }
                    }
                    );
            }
        }

        #endregion Constructor

        #region Methods

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

        public void OnPropertyChanged(object sender, String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propName));
            }
        }

        public void OnPropertyChanged(object sender, HexDigitClockModel.EUnits unit_in)
        {
            OnPropertyChanged(sender, unit_in.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}