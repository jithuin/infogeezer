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
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class BinaryClockSegment : UserControl
    {


        public int HexTime
        {
            get { return (int)GetValue(HexTimeProperty); }
            set { SetValue(HexTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HexTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HexTimeProperty =
            DependencyProperty.Register("HexTime", typeof(int), typeof(BinaryClockSegment), new UIPropertyMetadata(0));



        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Color), typeof(BinaryClockSegment), new UIPropertyMetadata(Colors.Transparent));

        public byte Transparency
        {
            get { return (byte)GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transparency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransparencyProperty =
            DependencyProperty.Register("Transparency", typeof(byte), typeof(BinaryClockSegment), new UIPropertyMetadata((byte)0));


        public BinaryClockSegment()
        {
            InitializeComponent();
        }
    }
    
}
