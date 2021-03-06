using Clocks.ViewModel;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Threading;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace Clocks.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Timer interval for clocks on the UI
        /// </summary>
        /// <value>50 ms</value>
        protected int TimerInterval = 50;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            #region HexClockInit

            HexClock.StrokeThickness = 10; // TODO: it should be a designer property
            bool updating = false;
            DispatcherTimer hexTimer = new DispatcherTimer();
            hexTimer.Tick += (sender, e) =>
                    {
                        //HexClock.UpdateNow();
                        if (Monitor.TryEnter(HexClock))
                        {
                            try
                            {
                                HexClock.UpdateNow();
                                DecClock.UpdateNow();
                            }
                            finally
                            {
                                Monitor.Exit(HexClock);
                            }
                        }
                    };
            hexTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            hexTimer.Start();

            #endregion HexClockInit
        }

        public DecimalClockViewModel DecClock
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DecimalClockViewModel>();
            }
        }

        public HexaDecimalClockViewModel HexClock
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HexaDecimalClockViewModel>();
            }
        }
    }
}