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
    /// Interaction logic for BinaryRingSegment.xaml
    /// </summary>
    public partial class BinaryRingSegment : UserControl
    {
        public BinaryRingSegment()
        {
            InitializeComponent();
        }
        public int HexTime
        {
            get { return (int)GetValue(HexTimeProperty); }
            set { SetValue(HexTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HexTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HexTimeProperty =
            DependencyProperty.Register("HexTime", typeof(int), typeof(BinaryRingSegment), new UIPropertyMetadata(0));
    }
}
