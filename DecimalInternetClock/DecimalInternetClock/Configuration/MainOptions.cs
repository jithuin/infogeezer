using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace DecimalInternetClock.Configuration
{
    public class MainOptions : ObservableObject
    {
        #region MainFontFamily

        /// <summary>
        /// The <see cref="MainFontFamily" /> property's name.
        /// </summary>
        public const string MainFontFamilyPropertyName = "MainFontFamily";

        private FontFamily _mainFontFamily = new FontFamily("Aurabesh");

        /// <summary>
        /// Sets and gets the MainFontFamily property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FontFamily MainFontFamily
        {
            get
            {
                return _mainFontFamily;
            }

            set
            {
                if (_mainFontFamily == value)
                {
                    return;
                }

                RaisePropertyChanging(MainFontFamilyPropertyName);
                _mainFontFamily = value;
                RaisePropertyChanged(MainFontFamilyPropertyName);
            }
        }

        #endregion MainFontFamily
    }
}