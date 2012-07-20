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
    /// Interaction logic for BinaryClock.xaml
    /// </summary>
    public partial class BinaryClock : UserControl
    {
        public BinaryClock()
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
            DependencyProperty.Register("DecimalTime", typeof(double), typeof(BinaryClock), new UIPropertyMetadata(0.0));

        [Obsolete]
        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Color), typeof(BinaryClock), new UIPropertyMetadata(Colors.Transparent));

        [Obsolete]
        public byte Transparency
        {
            get { return (byte)GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transparency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransparencyProperty =
            DependencyProperty.Register("Transparency", typeof(byte), typeof(BinaryClock), new UIPropertyMetadata((byte)0));
    }
    
   
}
