using System;
using System.Collections.Generic;
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
using DecimalInternetClock.Properties;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window, INameScope
    {
        public MainWindow()
        {
            SetValue(ForegroundProperty, Settings.Default.Foreground);
            SetValue(BackgroundProperty, Settings.Default.Background);
            SetValue(WidthProperty, Settings.Default.WindowSize.Width);
            SetValue(HeightProperty, Settings.Default.WindowSize.Height);
            SetValue(LeftProperty, Settings.Default.WindowPosition.X);
            SetValue(TopProperty, Settings.Default.WindowPosition.Y);

            InitializeComponent();

            NameScope.SetNameScope(contextMenu, this);

            _timer.AutoReset = true;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            double ret = 0;
            ret += (double)dt.Millisecond / 1000.0;
            ret += (double)dt.Second;
            ret += (double)dt.Minute * 60.0;
            ret += (double)dt.Hour * 3600.0;
            ret /= 86400; //60*60*24
            ret *= 1000;
            this.Dispatcher.Invoke(new Action(() => { this.DecimalTime = ret; }));
        }

        Timer _timer = new Timer(50);

        public double DecimalTime
        {
            get { return (double)GetValue(DecimalTimeProperty); }
            set { SetValue(DecimalTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecimalTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalTimeProperty =
            DependencyProperty.Register("DecimalTime", typeof(double), typeof(MainWindow), new UIPropertyMetadata(0.0));

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Foreground = (SolidColorBrush)this.Foreground;
            Settings.Default.Background = (SolidColorBrush)this.Background;

            Settings.Default.WindowSize = new Size(this.ActualWidth, this.ActualHeight);
            Settings.Default.WindowPosition = new Point(this.Left, this.Top);
            Settings.Default.Text = this.textBox.Text;
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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            this.ExitMenuItem_Click(sender, e);
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        #region INameScope Members

        Dictionary<string, object> items = new Dictionary<string, object>();

        object INameScope.FindName(string name)
        {
            return items[name];
        }

        void INameScope.RegisterName(string name, object scopedElement)
        {
            items.Add(name, scopedElement);
        }

        void INameScope.UnregisterName(string name)
        {
            items.Remove(name);
        }

        #endregion INameScope Members

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
    }
}