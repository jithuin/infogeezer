﻿using System;
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
using DecimalInternetClock.Clocks.ViewModel;
using DecimalInternetClock.Helpers;
using DecimalInternetClock.Properties;
using DecimalInternetClock.Touch;
using ManagedWinapi;
using ManagedWinapi.Windows;
using Windows7.Multitouch;
using Windows7.Multitouch.WPF;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainView : Window, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Timer interval for clocks on the UI
        /// </summary>
        /// <value>50 ms</value>
        protected int TimerInterval = 50;

        public DecimalTimer DecimalTime { get; set; }

        private Windows7.Multitouch.GestureHandler _gestureHandler;

        protected IRotateable _currentRotateableVisual;

        public BinaryHexDigitClockViewModel HexClock { get; set; }

        #endregion Properties

        #region Constructor

        public MainView()
        {
            DecimalTime = new DecimalTimer();
            HexClock = new BinaryHexDigitClockViewModel();
            InitializeComponent();
            //SetLocalBinding(this, ForegroundProperty, HexClock, HexClock.ForegroundPropertyName);
            HexClock.Foreground = Settings.Default.Foreground; // TODO: It should be bound instead
            HexClock.StrokeThickness = 10; // TODO: it should be a designer property
            InitHexTimer();
            LoadSettings();
            InitGesture();
            //NameScope.SetNameScope(contextMenu, this);
        }

        private void InitHexTimer()
        {
            Timer hexTimer = new Timer(TimerInterval);
            hexTimer.AutoReset = true;
            hexTimer.Elapsed += new ElapsedEventHandler(hexTimer_Elapsed);

            hexTimer.Start();
        }

        private void hexTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            HexClock.UpdateNow();
        }

        private void InitGesture()
        {
            if (Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
            {
                _gestureHandler = Factory.CreateGestureHandler(this);

                _gestureHandler.Pan += ProcessPan;
                _gestureHandler.PanBegin += ProcessPan;
                _gestureHandler.PanEnd += ProcessPan;

                _gestureHandler.Rotate += ProcessRotate;
                _gestureHandler.RotateBegin += ProcessRotateBegin;
                _gestureHandler.RotateEnd += ProcessRotate;

                _gestureHandler.PressAndTap += ProcessRollOver;

                //_gestureHandler.TwoFingerTap += ProcessTwoFingerTap;

                //_gestureHandler.Zoom += ProcessZoom;
                //_gestureHandler.ZoomBegin += ProcessZoom;
                //_gestureHandler.ZoomEnd += ProcessZoom;
            }
        }

        #endregion Constructor

        #region Settings

        private void LoadSettings()
        {
            SetValue(ForegroundProperty, Settings.Default.Foreground);
            SetValue(BackgroundProperty, Settings.Default.Background);
            SetValue(WidthProperty, Settings.Default.WindowSize.Width);
            SetValue(HeightProperty, Settings.Default.WindowSize.Height);
            SetValue(LeftProperty, Settings.Default.WindowPosition.X);
            SetValue(TopProperty, Settings.Default.WindowPosition.Y);
            this.textBox.Text = Settings.Default.Text;
        }

        private void SaveSettings()
        {
            Settings.Default.Foreground = (SolidColorBrush)this.Foreground;
            Settings.Default.Background = (SolidColorBrush)this.Background;

            Settings.Default.WindowSize = new Size(this.ActualWidth, this.ActualHeight);
            Settings.Default.WindowPosition = new Point(this.Left, this.Top);
            Settings.Default.Text = this.textBox.Text;
        }

        #endregion Settings

        #region Event Handlers

        private void ProcessPan(object sender, GestureEventArgs args)
        {
            //_translate.X += args.PanTranslation.Width;
            _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - args.PanTranslation.Height);
            //_translate.Y += args.PanTranslation.Height;
        }

        private void ProcessRotateBegin(object sender, GestureEventArgs args)
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
                new PointHitTestParameters((Point)args.Center.ToVector()));
            ProcessRotate(sender, args);
        }

        private void ProcessRotate(object sender, GestureEventArgs args)
        {
            if (_currentRotateableVisual != null)
                _currentRotateableVisual.RotateAngle -= args.RotateAngle * 180 / Math.PI;
        }

        private void ProcessRollOver(object sender, GestureEventArgs args)
        {
            MouseHelper.RightClick(((Vector)this.PointToScreen((Point)args.Location.ToVector())).ToPoint());
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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
                this.Foreground = new SolidColorBrush(cpd.SelectedColor);
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

        //#region INameScope Members

        //private Dictionary<string, object> items = new Dictionary<string, object>();

        //object INameScope.FindName(string name)
        //{
        //    return items[name];
        //}

        //void INameScope.RegisterName(string name, object scopedElement)
        //{
        //    items.Add(name, scopedElement);
        //}

        //void INameScope.UnregisterName(string name)
        //{
        //    items.Remove(name);
        //}

        //#endregion INameScope Members

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}