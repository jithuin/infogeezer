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
    /// Interaction logic for BinaryRing.xaml
    /// </summary>
    public partial class BinaryRing : UserControl
    {
        public BinaryRing()
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
            DependencyProperty.Register("DecimalTime", typeof(double), typeof(BinaryRing), new UIPropertyMetadata(0.0));
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

        #endregion
    }

}
