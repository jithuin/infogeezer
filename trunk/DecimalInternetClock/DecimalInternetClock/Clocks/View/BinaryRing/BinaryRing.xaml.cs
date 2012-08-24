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
using DecimalInternetClock.Touch;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for BinaryRing.xaml
    /// </summary>
    public partial class BinaryRing : UserControl, IRotateable
    {
        public BinaryRing()
        {
            InitializeComponent();
        }



        public double RotateAngle
        {
            get { return (double)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotateAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotateAngleProperty =
            DependencyProperty.Register("RotateAngle", typeof(double), typeof(BinaryRing), new UIPropertyMetadata(0.0));



        public double DecimalTime
        {
            get { return (double)GetValue(DecimalTimeProperty); }
            set { SetValue(DecimalTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecimalTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalTimeProperty =
            DependencyProperty.Register("DecimalTime", typeof(double), typeof(BinaryRing), new UIPropertyMetadata(0.0));
    }
}