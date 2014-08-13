using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ColorPicker;
using DecimalInternetClock.Clocks;
using DecimalInternetClock.Helpers;
using DecimalInternetClock.Properties;
using DecimalInternetClock.Touch;
using ManagedWinapi;
using ManagedWinapi.Windows;

//using Windows7.Multitouch;
//using Windows7.Multitouch.WPF;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainView : Window
    {
        #region Properties

        public DecimalTimer DecimalTime { get; set; }

        protected IRotateable _currentRotateableVisual;

        #endregion Properties

        #region Constructor

        public MainView()
        {
            DecimalTime = new DecimalTimer();

            InitializeComponent();

            LoadSettings();
        }

        #endregion Constructor

        #region Settings

        private void LoadSettings()
        {
            //SetValue(ForegroundProperty, Settings.Default.Foreground);
            //SetValue(BackgroundProperty, Settings.Default.Background);
            SetValue(WidthProperty, Settings.Default.WindowSize.Width);
            SetValue(HeightProperty, Settings.Default.WindowSize.Height);
            SetValue(LeftProperty, Settings.Default.WindowPosition.X);
            SetValue(TopProperty, Settings.Default.WindowPosition.Y);
            this.textBox.Text = Settings.Default.Text;
        }

        private void SaveSettings()
        {
            //Settings.Default.Foreground = (SolidColorBrush)this.Foreground;
            //Settings.Default.Background = (SolidColorBrush)this.Background;

            Settings.Default.WindowSize = new Size(this.ActualWidth, this.ActualHeight);
            Settings.Default.WindowPosition = new Point(this.Left, this.Top);
            Settings.Default.Text = this.textBox.Text;
        }

        #endregion Settings

        #region Event Handlers

        private void this_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            VisualTreeHelper.HitTest(this,
                //null
                (hittestDepObj) =>
                {
                    _currentRotateableVisual = hittestDepObj as IRotateable;
                    return _currentRotateableVisual != null ? HitTestFilterBehavior.Stop : HitTestFilterBehavior.ContinueSkipSelf;
                }
                ,
                (r) => { return HitTestResultBehavior.Continue; }
                //(result) =>
                //{ _currentRotateableVisual = result.VisualHit as IRotateable;
                //    return _currentRotateableVisual!= null? HitTestResultBehavior.Stop: HitTestResultBehavior.Continue;}
                ,
                new PointHitTestParameters(e.ManipulationOrigin));
        }

        private void this_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - e.DeltaManipulation.Translation.Y);
            if (_currentRotateableVisual != null)
                _currentRotateableVisual.RotateAngle -= e.DeltaManipulation.Rotation * 180 / Math.PI;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Released)
                DragMove();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void ForeColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(((SolidColorBrush)Foreground).Color);
            if ((bool)cpd.ShowDialog())
            {
                Settings.Default.Foreground = new SolidColorBrush(cpd.SelectedColor);
                //this.Foreground = new SolidColorBrush(cpd.SelectedColor);
            }
        }

        private void BackColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(((SolidColorBrush)Background).Color);
            if ((bool)cpd.ShowDialog())
            {
                this.Background = new SolidColorBrush(cpd.SelectedColor);
            }
        }

        private void miOption_Click(object sender, RoutedEventArgs e)
        {
            Options.Options ow = new Options.Options();
            ow.Show();
        }

        private void miTouchTest_Click(object sender, RoutedEventArgs e)
        {
            TouchTest tt = new TouchTest();
            tt.Show();
        }

        #endregion Event Handlers
    }
}