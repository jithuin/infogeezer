using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DecimalInternetClock.ValueConverters
{
    public class RangeValueConverter : MarkupExtension, IValueConverter, INotifyPropertyChanged
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return value.ToString();
            else
                return "-- (nem áll rendelkezésre adat)";
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion INotifyPropertyChanged Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class IntRangeValueConverter : RangeValueConverter
    {
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ret;
            if (int.TryParse((String)value, out ret))
                return ret;
            else
                throw new ArgumentException();
        }
    }

    public class FloatRangeValueConverter : RangeValueConverter
    {
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float ret;
            if (float.TryParse((String)value, out ret))
                return ret;
            else
                throw new ArgumentException();
        }
    }

    public class RemissionDivisionConverter : DependencyObject, IValueConverter
    {
        #region Properties
        //private int _divider = 1;
        //public int Divider
        //{
        //    get { return _divider; }
        //    set
        //    {
        //        if (_divider != value)
        //        {
        //            _divider = value;
        //            OnPropertyChanged("Divider");
        //        }
        //    }
        //}

        public int Divider
        {
            get { return (int)GetValue(DividerProperty); }
            set { SetValue(DividerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Divider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DividerProperty =
            DependencyProperty.Register("Divider", typeof(int), typeof(RemissionDivisionConverter), new UIPropertyMetadata(1));

        #endregion Properties

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value) / Divider;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int.Parse((string)value) * Divider);
        }

        #endregion IValueConverter Members
    }
}