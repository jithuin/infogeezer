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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LedUserControl : UserControl
    {
        public LedUserControl()
        {
            InitializeComponent();
        }


        public bool LedState
        {
            get { return (bool)GetValue(LedStateProperty); }
            set { SetValue(LedStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LedState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LedStateProperty =
            DependencyProperty.Register("LedState", typeof(bool), typeof(LedUserControl), new UIPropertyMetadata(true));


        public byte Transparency
        {
            get { return (byte)GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transparency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransparencyProperty =
            DependencyProperty.Register("Transparency", typeof(byte), typeof(LedUserControl), new UIPropertyMetadata((byte)0));


    }
    //[ValueConversion(typeof(bool), typeof(Color))]
    //public class BoolColorConverter : IMultiValueConverter
    //{
    //    #region IMultiValueConverter Members

    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        byte alpha;
    //        if (values.Count() < 3) return Colors.Transparent;
    //        bool? isOn = values[0] as bool?;
    //        Color? color = values[1] as Color?;
    //        byte? transparency = values[2] as byte?;

    //        if (color != null && isOn != null && isOn == true && byte.TryParse((string)parameter, out alpha) && transparency != null)
    //        {
    //            Color c = new Color();
    //            c.A = (byte)(alpha * color.Value.A / 255);
    //            c.R = color.Value.R;
    //            c.G = color.Value.G;
    //            c.B = color.Value.B;
    //            return c;
    //        }
    //        else
    //            return Colors.Transparent;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion

    //}
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

        #endregion
    }



}
