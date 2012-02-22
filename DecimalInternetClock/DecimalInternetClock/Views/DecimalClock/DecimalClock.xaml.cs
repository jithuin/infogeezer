using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for DecimalClock.xaml
    /// </summary>
    public partial class DecimalClock : UserControl
    {
        public DecimalClock()
        {
            InitializeComponent();
        }
        public double DecimalTime
        {
            get { return (double)GetValue(DecimalTimeProperty); }
            set { SetValue(DecimalTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecimalTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalTimeProperty =
            DependencyProperty.Register("DecimalTime", typeof(double), typeof(DecimalClock), new UIPropertyMetadata(0.0));

        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Color), typeof(DecimalClock), new UIPropertyMetadata(Colors.Transparent));

        public byte Transparency
        {
            get { return (byte)GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transparency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransparencyProperty =
            DependencyProperty.Register("Transparency", typeof(byte), typeof(DecimalClock), new UIPropertyMetadata((byte)0));
    }
    [ValueConversion(typeof(double), typeof(String))]
    public class DoubleValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                double dval = (double)value;
                int decHour = (int)(dval / 100.0) % 10;
                int decMin = (int)(dval ) % 100;
                int decSec = (int)(dval * 100) % 100;
                return String.Format("{0}:{1:00}.{2:00}", decHour, decMin, decSec);
            }
            else
            {
                return "-";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    [ValueConversion(typeof(byte), typeof(double))]
    public class TransparencyToOpacityConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte? tr = value as byte?;
            if (tr != null)
            {
                return (double)(255 - tr) / 255.0;
            }
            else
                return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
