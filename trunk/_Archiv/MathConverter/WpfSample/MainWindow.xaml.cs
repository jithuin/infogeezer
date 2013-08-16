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
using System.Threading;
using System.Windows.Threading;

namespace WpfSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RealTime_Click(object sender, RoutedEventArgs e)
        {
            if (RealTime.IsChecked == true)
            {
                if (_timer != null) 
                {
                    _timer.Dispose();
                }

                _timer = new Timer(OnTimer, null, 0, 1000);
            }
            else
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        void OnTimer(object state)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<object>(OnTimer), DispatcherPriority.Input, state);
                return;
            }

            var time = DateTime.Now.TimeOfDay;
            Hours.Text = time.Hours.ToString();
            Minutes.Text = time.Minutes.ToString();
            Seconds.Text = time.Seconds.ToString();
        }
    }
}
