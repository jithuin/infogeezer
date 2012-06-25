using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DecimalInternetClock.ValueConverters
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

        #endregion IValueConverter Members
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

        #endregion IValueConverter Members
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

        #endregion IValueConverter Members
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

        #endregion IValueConverter Members
    }

    public class HexaStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double? _decimalTime = value as double?;
            if (_decimalTime != null)
            {
                double temp = _decimalTime.Value / 1000.0;
                List<int> _Time = new List<int>(4);
                for (int i = 0; i < 4; i++)
                {
                    temp *= 16;
                    _Time.Add((int)temp % 16);
                }
                return String.Format("{0:X}:{1:X}{2:X}.{3:X}", _Time[0], _Time[1], _Time[2], _Time[3]);
            }
            else
                return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}