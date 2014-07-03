using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DecimalInternetClock.Clocks.View;
using DecimalInternetClock.Helpers;
using GalaSoft.MvvmLight.Command;

namespace DecimalInternetClock.Clocks.ViewModel
{
    public class BinaryHexDigitViewModel : INotifyPropertyChanged
    {
        private HexDigitModel _clock;

        public BinaryHexDigitViewModel()
        {
            _clock = new HexDigitModel();
            _clock.Now = 0xf;
            DebugCommand = new RelayCommand(() =>
        {
            if (this._clock != null)
                _clock.UpdateNow();
        });
        }

        #region Properties

        public BinaryHexDigitView view { get; set; }

        public RelayCommand DebugCommand { get; private set; }

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
                    foreach (HexDigitModel.EUnits unit in EnumHelper.GetValues<HexDigitModel.EUnits>())
                        OnPropertyChanged(unit);
                }
            }
        }

        #region Height

        /// <summary>
        /// The <see cref="Height" /> property's name.
        /// </summary>
        public const string HeightPropertyName = "Height";

        /// <summary>
        /// Sets and gets the ActualHeight property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double Height
        {
            get { return _actualWidth; }
        }

        #endregion Height

        #region ActualWidth

        /// <summary>
        /// The <see cref="ActualWidth" /> property's name.
        /// </summary>
        public const string ActualWidthPropertyName = "ActualWidth";

        private double _actualWidth = 100;

        /// <summary>
        /// Sets and gets the ActualWidth property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                //if (DesignerProperties.GetIsInDesignMode(this))
                //    return 100.0;
                //else
                return _actualWidth;
            }
            set
            {
                if (_actualWidth != value)
                {
                    _actualWidth = value;
                    OnPropertyChanged(ActualWidthPropertyName);
                    OnPropertyChanged(HeightPropertyName);
                }
            }
        }

        #endregion ActualWidth

        #region PathData

        /// <summary>
        /// The <see cref="PathData" /> property's name.
        /// </summary>
        public const string PathDataPropertyName = "PathData";

        protected const string _pathDataFormatString = "M 0,{0} A {0},{0} 0 0 1 {0},0";

        /// <summary>
        /// Sets and gets the PathData property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string PathData
        {
            get { return String.Format(_pathDataFormatString, (_actualWidth / 2).ToString(CultureInfo.InvariantCulture)); }
        }

        #endregion PathData

        #region First

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

        #endregion First

        #region Second

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

        #endregion Second

        #region Third

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

        #endregion Third

        #region Fourth

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

        #endregion Fourth

        #region Foreground

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

        #endregion Foreground

        #region StrokeThickness

        public string StrokeThicknessPropertyName = "StrokeThickness";
        protected double _strokeThickness = 10;

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
                    OnPropertyChanged(PathDataPropertyName);
                }
            }
        }

        #endregion StrokeThickness

        #region Margin

        public const string MarginPropertyName = "Margin";
        protected Thickness _margin = new Thickness(0);

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

        #endregion Margin

        #region InternalMargin

        public const string InternalMarginPropertyName = "InternalMargin";

        public Thickness InternalMargin
        {
            get
            {
                return new Thickness(0);
            }
        }

        #endregion InternalMargin

        #endregion Properties

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