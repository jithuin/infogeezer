using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DecimalInternetClock.ValueConverters
{
    public class BrushColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte alpha;
            SolidColorBrush brush = value as SolidColorBrush;
            if (brush != null && byte.TryParse((string)parameter, out alpha))
            {
                Color c = new Color();
                c.A = (byte)(alpha * brush.Color.A / 255);
                c.R = brush.Color.R;
                c.G = brush.Color.G;
                c.B = brush.Color.B;
                return c;
            }
            else
            {
                return Colors.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}