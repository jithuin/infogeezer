using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DecimalInternetClock.Helpers;

namespace DecimalInternetClock
{
    public class BinaryHexDigitViewModel : INotifyPropertyChanged
    {
        private HexDigitModel _clock;

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
                if (_clock.Now != value)
                {
                    _clock.Now = value;
                    UpdateAllSubProperties();
                }
            }
        }

        private void UpdateAllSubProperties()
        {
            foreach (HexDigitModel.EUnits unit in EnumHelper.GetValues<HexDigitModel.EUnits>())
                OnPropertyChanged(unit);
        }

        public Visibility FirstVisibility
        {
            get
            {
                return First ? Visibility.Visible : Visibility.Hidden;
            }
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

        public Visibility SecondVisibility
        {
            get
            {
                return Second ? Visibility.Visible : Visibility.Hidden;
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

        public Visibility ThirdVisibility
        {
            get
            {
                return Third ? Visibility.Visible : Visibility.Hidden;
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

        public Visibility FourthVisibility
        {
            get
            {
                return Fourth ? Visibility.Visible : Visibility.Hidden;
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

        public string ForegroundPropertyName = "Foreground";
        protected Brush _foreground;

        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    OnPropertyChanged(ForegroundPropertyName);
                }
            }
        }

        public string StrokeThicknessPropertyName = "StrokeThickness";
        protected double _strokeThickness;

        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness != value)
                {
                    _strokeThickness = value;
                    OnPropertyChanged(StrokeThicknessPropertyName);
                    OnPropertyChanged(InternalMarginPropertyName);
                }
            }
        }

        public const string MarginPropertyName = "Margin";
        protected Thickness _margin = new Thickness();

        public Thickness Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                if (_margin != value)
                {
                    _margin = value;
                    OnPropertyChanged(MarginPropertyName);
                }
            }
        }

        public const string InternalMarginPropertyName = "InternalMargin";

        public Thickness InternalMargin
        {
            get
            {
                return new Thickness(_strokeThickness);
            }
        }

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(HexDigitModel.EUnits unit_in)
        {
            OnPropertyChanged(unit_in.ToString());
            OnPropertyChanged(String.Format("{0}Visibility", unit_in.ToString()));
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