using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightSample
{
    public partial class MainPage : UserControl
    {
        Timer _timer;

        public MainPage()
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
                Dispatcher.BeginInvoke(new Action<object>(OnTimer), state);
                return;
            }

            var time = DateTime.Now.TimeOfDay;
            Hours.Text = time.Hours.ToString();
            Minutes.Text = time.Minutes.ToString();
            Seconds.Text = time.Seconds.ToString();
        }

    }
}
