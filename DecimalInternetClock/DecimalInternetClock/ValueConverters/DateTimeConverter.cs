using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DecimalInternetClock.ValueConverters
{
    [ValueConversion(typeof(DateTime), typeof(bool))]
    public class AlarmDateTimeConverter : IMultiValueConverter, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is DateTime) && (value != null) && DateTime.Now.CompareTo(value) > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (values[0] is DateTime) && (values[0] != null) && DateTime.Now.CompareTo(values[0]) > 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }
}