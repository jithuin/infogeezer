using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;

namespace DecimalInternetClock.Clocks
{
    public class DecimalTimer : INotifyPropertyChanged
    {
        #region Fields and Properties
        Timer _timer = new Timer(50);

        public double TimerInterval
        {
            get
            {
                return _timer.Interval;
            }
            set
            {
                if (_timer.Interval != value)
                {
                    _timer.Stop();
                    _timer.Interval = value;
                    _timer.Start();
                }
            }
        }

        private double _time;

        public double Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        #endregion Fields and Properties

        #region Constructor and Initials

        public DecimalTimer()
        {
            InitTimer();
        }

        private void InitTimer()
        {
            //_timer.AutoReset = true;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }

        #endregion Constructor and Initials

        #region Methods

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
            this.Time = ret;
            //System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(new Action(() => { this.Time = ret; }));
        }

        #endregion Methods

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