using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Clocks.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Clocks.ViewModel
{
    public class HexaDecimalClockViewModel : ClockViewModelBase<HexaDecimalClockModel, HexaDecimalClockModel.EUnits>
    {
        #region Fields

        private Dictionary<HexaDecimalClockModel.EUnits, HexaDecimalDigitViewModel> _subViewModels;

        #endregion Fields

        #region Public Properties

        public bool IsReadonly { get; set; }

        public override string TimeString
        {
            get
            {
                return base.TimeString;
            }
        }

        public DateTime Now
        {
            set
            {
                _clock.Now = value;
                OnPropertyChanged(TimeStringPropertyName);
                foreach (HexaDecimalClockModel.EUnits unit in Enum.GetValues(typeof(HexaDecimalClockModel.EUnits)))
                {
                    if (_subViewModels[unit].Now != _clock[unit])
                    {
                        _subViewModels[unit].Now = _clock[unit];
                        OnPropertyChanged(unit);
                    }
                }
                
            }
        }

        public HexaDecimalDigitViewModel Hour
        {
            get
            {
                return _subViewModels[HexaDecimalClockModel.EUnits.Hour];
            }
        }

        public HexaDecimalDigitViewModel MinuteHi
        {
            get
            {
                return _subViewModels[HexaDecimalClockModel.EUnits.MinuteHi];
            }
        }

        public HexaDecimalDigitViewModel MinuteLow
        {
            get
            {
                return _subViewModels[HexaDecimalClockModel.EUnits.MinuteLow];
            }
        }

        public HexaDecimalDigitViewModel Second
        {
            get
            {
                return _subViewModels[HexaDecimalClockModel.EUnits.Second];
            }
        }

        /// <summary>
        /// The <see cref="TextMargin" /> property's name.
        /// </summary>
        public const string TextMarginPropertyName = "TextMargin";


        /// <summary>
        /// Sets and gets the TextMargin property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Thickness TextMargin
        {
            get
            {
                return new Thickness(_strokeThickness * (int)HexaDecimalClockModel.NumberOfUnits + _strokeThickness / 2);
            }
        }

        

        public string ForegroundPropertyName = "Foreground";
        protected Brush _foreground;

        public Brush Foreground
        {
            get 
            { 
                if (_foreground == null)
                {
                    Windows.UI.Color color  = new Windows.UI.Color();
                    color.A = 0xff;
                    color.B = 0x00;
                    color.G = 0x00;
                    color.R = 0x00;
                    _foreground = new SolidColorBrush(color);
                }
                return _foreground; 
            }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    foreach (HexaDecimalClockModel.EUnits unit in Enum.GetValues(typeof(HexaDecimalClockModel.EUnits)))
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
                    foreach (HexaDecimalClockModel.EUnits unit in Enum.GetValues(typeof(HexaDecimalClockModel.EUnits)))
                    {
                        _subViewModels[unit].StrokeThickness = _strokeThickness;
                        _subViewModels[unit].Margin = new Thickness(_strokeThickness * (int)unit + _strokeThickness / 2);
                        // plus half strokethickness is needed because the path somehow draws the
                        // thick (strokethickness > 1) line half out half in the specified path.
                        // Thus half outer part would disappear if I do not add a margin
                        // that equals 0.5*strokethickness. And If it is added to the most outer path
                        // than it should be added to all.
                    }
                    OnPropertyChanged(StrokeThicknessPropertyName);
                    OnPropertyChanged(MinWidthPropertyName);
                    OnPropertyChanged(TextMarginPropertyName);
                }
            }
        }

        #region MinWidth

        /// <summary>
        /// The <see cref="MinWidth" /> property's name.
        /// </summary>
        public const string MinWidthPropertyName = "MinWidth";

        /// <summary>
        /// Sets and gets the MinWidth property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double MinWidth
        {
            get { return _strokeThickness * 8.5; } //4*2 line + 0.5 margin = 8.5 (see StrokeThickness set accessor for details)
        }

        #endregion MinWidth

        #endregion Public Properties

        #region Constructor

        public HexaDecimalClockViewModel()
            : base()
        {
            IsReadonly = true;
            TimeStringFormat = "{0:X}:{1:X}.{2:X}..{3:X}";
            //Init Sub-View-Models
            _subViewModels = new Dictionary<HexaDecimalClockModel.EUnits, HexaDecimalDigitViewModel>();
            foreach (HexaDecimalClockModel.EUnits unit in Enum.GetValues(typeof(HexaDecimalClockModel.EUnits)))
            {
                _subViewModels.Add(unit, new HexaDecimalDigitViewModel());

                _subViewModels[unit].PropertyChanged += new PropertyChangedEventHandler(
                    (sender, e) =>
                    {
                        //foreach (HexDigitClockModel.EUnits _unit in EnumHelper.GetValues<HexDigitClockModel.EUnits>())
                        if (!IsReadonly && sender != this && _subViewModels[unit].Now != _clock[unit])
                        {
                            _clock[unit] = _subViewModels[unit].Now;
                            OnPropertyChanged(sender, unit);
                            OnPropertyChanged(TimeStringPropertyName);
                        }
                    }
                    );
            }
        }

        #endregion Constructor

        #region Methods

        public override void UpdateNow()
        {
            Now = DateTime.Now;
        }

        #endregion Methods

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(HexaDecimalClockModel.EUnits unit_in)
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

        public void OnPropertyChanged(object sender, HexaDecimalClockModel.EUnits unit_in)
        {
            OnPropertyChanged(sender, unit_in.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}