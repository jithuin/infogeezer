using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Clocks.Model;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Clocks.ViewModel
{
    public class HexaDecimalDigitViewModel : INotifyPropertyChanged
    {
        private HexaDecimalDigitModel _clock;

        public HexaDecimalDigitViewModel()
        {
            _clock = new HexaDecimalDigitModel();
            _clock.Now = 0xf;
            DebugCommand = new RelayCommand(() =>
            {
                if (this._clock != null)
                    _clock.UpdateNow();
            });
        }

        #region Properties

        public RelayCommand DebugCommand { get; private set; }

        #region TimeProperties

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
                    foreach (HexaDecimalDigitModel.EUnits unit in Enum.GetValues(typeof(HexaDecimalDigitModel.EUnits)))
                        OnPropertyChanged(unit);
                    OnPropertyChanged(NowStringPropertyName);
                }
            }
        }

        #region NowString

        /// <summary>
        /// The <see cref="NowString" /> property's name.
        /// </summary>
        public const string NowStringPropertyName = "NowString";

        /// <summary>
        /// Sets and gets the NowString property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string NowString
        {
            get { return String.Format("{0:X}", Now); }
        }

        #endregion NowString

        #region First

        public Visibility FirstVisibility
        {
            get
            {
                return BooleanToVisibility(First);
            }
        }

        private Visibility BooleanToVisibility(bool boolean_in)
        {
            return boolean_in ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool First
        {
            get
            {
                return _clock[HexaDecimalDigitModel.EUnits.First];
            }
            set
            {
                if (_clock[HexaDecimalDigitModel.EUnits.First] != value)
                {
                    _clock[HexaDecimalDigitModel.EUnits.First] = value;
                    OnPropertyChanged(HexaDecimalDigitModel.EUnits.First);
                }
            }
        }

        #endregion First

        #region Second

        public Visibility SecondVisibility
        {
            get
            {
                return BooleanToVisibility(Second); 
            }
        }

        public bool Second
        {
            get
            {
                return _clock[HexaDecimalDigitModel.EUnits.Second];
            }
            set
            {
                if (_clock[HexaDecimalDigitModel.EUnits.Second] != value)
                {
                    _clock[HexaDecimalDigitModel.EUnits.Second] = value;
                    OnPropertyChanged(HexaDecimalDigitModel.EUnits.Second);
                }
            }
        }

        #endregion Second

        #region Third

        public Visibility ThirdVisibility
        {
            get
            {
                return BooleanToVisibility(Third);
            }
        }

        public bool Third
        {
            get
            {
                return _clock[HexaDecimalDigitModel.EUnits.Third];
            }
            set
            {
                if (_clock[HexaDecimalDigitModel.EUnits.Third] != value)
                {
                    _clock[HexaDecimalDigitModel.EUnits.Third] = value;
                    OnPropertyChanged(HexaDecimalDigitModel.EUnits.Third);
                }
            }
        }

        #endregion Third

        #region Fourth

        public Visibility FourthVisibility
        {
            get
            {
                return BooleanToVisibility(Fourth);
            }
        }

        public bool Fourth
        {
            get
            {
                return _clock[HexaDecimalDigitModel.EUnits.Fourth];
            }
            set
            {
                if (_clock[HexaDecimalDigitModel.EUnits.Fourth] != value)
                {
                    _clock[HexaDecimalDigitModel.EUnits.Fourth] = value;
                    OnPropertyChanged(HexaDecimalDigitModel.EUnits.Fourth);
                }
            }
        }

        #endregion Fourth

        #endregion TimeProperties

        #region ViewProperties

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
                return _actualWidth;
            }
            set
            {
                if (_actualWidth != value)
                {
                    _actualWidth = value;
                    OnPropertyChanged(ActualWidthPropertyName);
                    OnPropertyChanged(PathDataPropertyName);
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
                    OnPropertyChanged(PathDataPropertyName);
                }
            }
        }

        #endregion StrokeThickness

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

        #endregion ViewProperties

        #endregion Properties

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(HexaDecimalDigitModel.EUnits unit_in)
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