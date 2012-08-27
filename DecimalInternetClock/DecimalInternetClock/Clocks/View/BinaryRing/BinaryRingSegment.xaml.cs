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
    /// Interaction logic for BinaryRingSegment.xaml
    /// </summary>
    public partial class BinaryRingSegment : UserControl, IRotateable
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

        public double RotateAngle
        {
            get { return (double)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotateAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotateAngleProperty =
            DependencyProperty.Register("RotateAngle", typeof(double), typeof(BinaryRingSegment), new UIPropertyMetadata(0.0));

        private void this_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isRotationEnabled = true;
                _rotationStartPoint = this.PointToScreen(e.GetPosition(this));
                _rotationStartAngle = this.RotateAngle;
                _transformControl = this;
                e.MouseDevice.Capture(this, CaptureMode.SubTree);
                e.Handled = true;
            }
            else
            {
                _isRotationEnabled = false;
                if (_transformControl == this)
                {
                    _transformControl = null;
                    e.MouseDevice.Capture(this, CaptureMode.None);
                    e.Handled = true;
                }
            }
        }

        UserControl _transformControl = null;
        bool _isRotationEnabled = false;
        double _rotationStartAngle = 0.0;
        Point _rotationStartPoint;
        Point _rotationCurrentPoint;
        Point _centerPoint;

        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRotationEnabled && _transformControl == this)
            {
                _rotationCurrentPoint = this.PointToScreen(e.GetPosition(this));

                _centerPoint = this.PointToScreen(new Point(this.ActualWidth / 2, this.ActualHeight / 2));
                double angle = Vector.AngleBetween(_rotationStartPoint - _centerPoint, _rotationCurrentPoint - _centerPoint);
                this.RotateAngle = angle + _rotationStartAngle;
                e.Handled = true;
            }
        }
    }
}