using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DecimalInternetClock
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class DoubleBinaryValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? normaledValue = value as int?;
            int shift;
            if (normaledValue != null && int.TryParse((String)parameter, out shift))
            {
                for (int i = 0; i < shift; i++)
                {
                    normaledValue /= 2;
                }
                int ret = (int)normaledValue;
                return ret % 2 == 1;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(int), typeof(Color))]
    public class IntToColorValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? normaledValue = value as int?;
            int shift;
            if (normaledValue != null && int.TryParse((String)parameter, out shift))
            {
                for (int i = 0; i < shift; i++)
                {
                    normaledValue /= 2;
                }

                if (normaledValue.Value % 2 == 1)
                    return Colors.Black;
                else
                    return Colors.Transparent;
            }
            else
                return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(int), typeof(String))]
    public class HexStringValueConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int i = (int)value;
                String s = i.ToString("X");
                return s;
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    [ValueConversion(typeof(double), typeof(int))]
    public class DoubleHexValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double? normaledValue = value as double?;
            int shift;
            if (normaledValue != null && int.TryParse((String)parameter, out shift))
            {
                normaledValue /= 1000;
                for (int i = 0; i < shift + 1; i++)
                {
                    normaledValue *= 16;
                }
                int ret = (int)normaledValue;
                return ret % 16;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
