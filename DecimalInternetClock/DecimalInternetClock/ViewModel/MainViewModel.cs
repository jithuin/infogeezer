using System.Configuration;
using System.Timers;
using Clocks.ViewModel;
using DecimalInternetClock.Properties;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace DecimalInternetClock.ViewModel
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
            Timer hexTimer = new Timer(TimerInterval);
            hexTimer.AutoReset = true;
            hexTimer.Elapsed += new ElapsedEventHandler((o, e) => HexClock.UpdateNow());

            hexTimer.Start();

            #endregion HexClockInit
        }

        public ApplicationSettingsBase Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        public BinaryHexDigitClockViewModel HexClock
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BinaryHexDigitClockViewModel>();
            }
        }
    }
}