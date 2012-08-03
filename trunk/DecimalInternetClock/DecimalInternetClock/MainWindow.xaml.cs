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
using DecimalInternetClock.Properties;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window, INameScope, INotifyPropertyChanged
    {
        #region Properties

        public DecimalTimer DecimalTime { get; set; }

        #endregion Properties

        #region Constructor

        public MainWindow()
        {
            DecimalTime = new DecimalTimer();
            InitializeComponent();
            LoadSettings();
            NameScope.SetNameScope(contextMenu, this);
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