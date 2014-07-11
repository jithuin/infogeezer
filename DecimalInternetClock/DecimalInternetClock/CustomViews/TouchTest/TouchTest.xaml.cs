using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for TouchTest.xaml
    /// </summary>
    public partial class TouchTest : Window, INotifyPropertyChanged
    {
        public TouchTest()
        {
            InitializeComponent();
        }

        protected bool wasPreviousStylusMove = false;
        protected bool wasPreviousMouseMove = false;
        protected bool wasPreviousManipulationDelta = false;

        //private void FeedBack(String text)
        //{
        //    tbFeedBack.Text += String.Format("{0}\r\n", text);
        //    wasPreviousStylusMove = false;
        //    wasPreviousMouseMove = false;
        //    wasPreviousManipulationDelta = false;
        //}

        public enum EEventSourceTypes
        {
            None = 0,
            Stylus,
            Mouse,
            Scroll,
            Touch,
            Manipulation,
        }

        public enum EEventActionTypes
        {
            None = 0,
            Move,
            Delta,
        }

        protected Dictionary<EEventSourceTypes, bool> _isEventSourceTraceEnabledDictionary = new Dictionary<EEventSourceTypes, bool>()
            {
                {EEventSourceTypes.Manipulation, false},
                {EEventSourceTypes.Mouse, true},
                {EEventSourceTypes.Scroll, false},
                {EEventSourceTypes.Stylus, false},
                {EEventSourceTypes.Touch, false}
            };

        public bool IsStylusTraceEnabled
        {
            get
            {
                return _isEventSourceTraceEnabledDictionary[EEventSourceTypes.Stylus];
            }
            set
            {
                _isEventSourceTraceEnabledDictionary[EEventSourceTypes.Stylus] = value;
            }
        }

        public bool IsMouseTraceEnabled { get; set; } //TODO: the field behind should be the IsEventSourceTraceEnabled field

        public bool IsScrollTraceEnabled { get; set; }

        public bool IsTouchTraceEnabled { get; set; }

        public bool IsManipulationTraceEnabled { get; set; }

        public bool IsDetailedTraceEnabled { get; set; }

        protected StringBuilder _textField = new StringBuilder();

        protected void DisplayEvent(string eventName_in)
        {
            DisplayEvent(eventName_in, null);
        }

        /// <summary>
        /// Displays the event with details
        /// </summary>
        /// <param name="eventName_in"></param>
        /// <param name="detailes_in"></param>
        protected void DisplayEvent(string eventName_in, string detailes_in)
        {
            // determining the current Source and Action type
            EEventSourceTypes currentSourceType = EEventSourceTypes.None;
            EEventActionTypes currentActionType = EEventActionTypes.None;
            foreach (EEventSourceTypes sourceType in Enum.GetValues(typeof(EEventSourceTypes)))
                if (eventName_in.Contains(sourceType.ToString()))
                    currentSourceType = sourceType;
            foreach (EEventActionTypes actionType in Enum.GetValues(typeof(EEventActionTypes)))
                if (eventName_in.Contains(actionType.ToString()))
                    currentActionType = actionType;

            // display the text
            if (ShouldDisplayEvent(currentSourceType, currentActionType))
            {
                _textField.Append(eventName_in);

                if (IsDetailedTraceEnabled && detailes_in != null && detailes_in != String.Empty)
                    _textField.Append(detailes_in);

                _textField.AppendLine();

                RaisePropertyChanged(DisplayTextPropertyName);
            }

            SetPreviousState(currentSourceType, currentActionType);
        }

        private void SetPreviousState(EEventSourceTypes currentSourceType_in, EEventActionTypes currentActionType_in)
        {
            if (currentActionType_in == EEventActionTypes.Move
                && currentSourceType_in == EEventSourceTypes.Stylus
                )
                wasPreviousStylusMove = true;
            else if (currentActionType_in == EEventActionTypes.Move
                && currentSourceType_in == EEventSourceTypes.Mouse
                )
                wasPreviousMouseMove = true;
            else if (currentActionType_in == EEventActionTypes.Delta
                && currentSourceType_in == EEventSourceTypes.Manipulation
                )
                wasPreviousManipulationDelta = true;
            else
            {
                wasPreviousStylusMove = false;
                wasPreviousMouseMove = false;
                wasPreviousManipulationDelta = false;
            }
        }

        /// <summary>
        /// determining if the event should be displayed or not
        /// </summary>
        /// <param name="currentSourceType_in"></param>
        /// <param name="currentActionType_in"></param>
        /// <returns></returns>
        protected bool ShouldDisplayEvent(EEventSourceTypes currentSourceType_in, EEventActionTypes currentActionType_in)
        {
            bool shouldDisplayEvent = _isEventSourceTraceEnabledDictionary[currentSourceType_in];
            switch (currentActionType_in)
            {
                case EEventActionTypes.None:
                    shouldDisplayEvent &= true;
                    break;

                case EEventActionTypes.Move:
                    shouldDisplayEvent &= (currentSourceType_in == EEventSourceTypes.Stylus && !wasPreviousStylusMove)
                                        || (currentSourceType_in == EEventSourceTypes.Mouse && !wasPreviousMouseMove);
                    break;

                case EEventActionTypes.Delta:
                    shouldDisplayEvent &= currentSourceType_in == EEventSourceTypes.Manipulation && !wasPreviousManipulationDelta;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return shouldDisplayEvent;
        }

        protected const string DisplayTextPropertyName = "DisplayText";

        public string DisplayText
        {
            get
            {
                return _textField.ToString();
            }
        }

        #region Stylus Operations

        #region Stylus.

        private void cProbe_StylusButtonDown(object sender, StylusButtonEventArgs e)
        {
            DisplayEvent("StylusButtonDown", e.ToText());
        }

        private void cProbe_StylusButtonUp(object sender, StylusButtonEventArgs e)
        {
            DisplayEvent("StylusButtonUp", e.ToText());
        }

        private void cProbe_StylusDown(object sender, StylusDownEventArgs e)
        {
            DisplayEvent("StylusDown", e.ToText());
        }

        private void cProbe_StylusEnter(object sender, StylusEventArgs e)
        {
            DisplayEvent("StylusEnter", e.ToText());
        }

        private void cProbe_StylusInAirMove(object sender, StylusEventArgs e)
        {
            DisplayEvent("StylusInAirMove", e.ToText());
        }

        private void cProbe_StylusInRange(object sender, StylusEventArgs e)
        {
            DisplayEvent("StylusInRange", e.ToText());
        }

        private void cProbe_StylusLeave(object sender, StylusEventArgs e)
        {
            DisplayEvent("StylusLeave", e.ToText());
        }

        private void cProbe_StylusMove(object sender, StylusEventArgs e)
        {
            //if (!wasPreviousStylusMove)
            //{
            //    tbFeedBack.Text += "StylusMove: " + e.ToText() + "\r\n";
            //    wasPreviousStylusMove = true;
            //}
            DisplayEvent("StylusMove", e.ToText());
        }

        private void cProbe_StylusOutOfRange(object sender, StylusEventArgs e)
        {
            DisplayEvent("StylusOutOfRange", e.ToText());
        }

        private void cProbe_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            DisplayEvent("StylusSystemGesture", e.ToText());
        }

        private void cProbe_StylusUp(object sender, StylusEventArgs e)
        {
            DisplayEvent("StylusUp", e.ToText());
        }

        #endregion Stylus.

        #endregion Stylus Operations

        #region Mouse Operations

        private void cProbe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayEvent("MouseDown: ", e.ToText());
        }

        private void cProbe_MouseEnter(object sender, MouseEventArgs e)
        {
            DisplayEvent("MouseEnter: ", e.ToText());
        }

        private void cProbe_MouseLeave(object sender, MouseEventArgs e)
        {
            DisplayEvent("MouseLeave: ", e.ToText());
        }

        private void cProbe_MouseMove(object sender, MouseEventArgs e)
        {
            //if (!wasPreviousMouseMove)
            //{
            //    tbFeedBack.Text += "MouseMove: " + e.ToText() + "\r\n";
            //    wasPreviousMouseMove = true;
            //}
            DisplayEvent("MouseMove: ", e.ToText());
        }

        private void cProbe_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DisplayEvent("MouseUp: ", e.ToText());
        }

        private void cProbe_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            DisplayEvent("MouseWheel: ", e.ToText());
        }

        #endregion Mouse Operations

        #region Scroll

        private void cProbe_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            //tbFeedBack.Text += "Scroll\r\n";
            DisplayEvent("Scroll");
        }

        private void cProbe_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //tbFeedBack.Text += "ScrollChanged\r\n";
            DisplayEvent("ScrollChanged");
        }

        #endregion Scroll

        #region Touch

        private void cProbe_TouchDown(object sender, TouchEventArgs e)
        {
            //tbFeedBack.Text += "TouchDown\r\n";
            DisplayEvent("TouchDown");
        }

        private void cProbe_TouchEnter(object sender, TouchEventArgs e)
        {
            //tbFeedBack.Text += "TouchEnter\r\n";
            DisplayEvent("TouchEnter");
        }

        private void cProbe_TouchLeave(object sender, TouchEventArgs e)
        {
            //tbFeedBack.Text += "TouchLeave\r\n";
            DisplayEvent("TouchLeave");
        }

        private void cProbe_TouchMove(object sender, TouchEventArgs e)
        {
            //tbFeedBack.Text += "TouchMove\r\n";
            DisplayEvent("TouchMove");
        }

        private void cProbe_TouchUp(object sender, TouchEventArgs e)
        {
            //tbFeedBack.Text += "TouchUp\r\n";
            DisplayEvent("TouchUp");
        }

        #endregion Touch

        #region Manipulation

        private void cProbe_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DisplayEvent("ManipulationCompleted");
            //tbFeedBack.Text += "ManipulationCompleted\r\n";
        }

        private void cProbe_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            DisplayEvent("ManipulationDelta");
            //tbFeedBack.Text += "ManipulationDelta\r\n";
        }

        private void cProbe_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            DisplayEvent("ManipulationInertiaStarting");
            //tbFeedBack.Text += "ManipulationInertiaStarting\r\n";
        }

        private void cProbe_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            DisplayEvent("ManipulationStarted");
            //tbFeedBack.Text += "ManipulationStarted\r\n";
        }

        private void cProbe_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            DisplayEvent("ManipulationStarting");
            //tbFeedBack.Text += "ManipulationStarting\r\n";
        }

        #endregion Manipulation

        public void RaisePropertyChanged(string PropName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _textField.Clear();
            RaisePropertyChanged(DisplayTextPropertyName);
        }

        private void TraceEnabled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            foreach (EEventSourceTypes source in Enum.GetValues(typeof(EEventSourceTypes)))
            {
                if(mi.Name.Contains(source.ToString()))
                    _isEventSourceTraceEnabledDictionary[source]=mi.IsChecked;
            }
        }
    }

    public static class StylusHelpers
    {
        public static string ToText(this StylusEventArgs e)
        {
            return e.StylusDevice.ToText();
        }

        public static string ToText(this StylusDeviceCollection ds)
        {
            StringBuilder sb = new StringBuilder();
            foreach (StylusDevice d in ds)
            {
                sb.Append(" Id: " + d.Id.ToString()
                    + " Position: " + d.GetPosition(d.Target).ToText()
                    + " InAir: " + d.InAir.ToString()
                    + " InRange: " + d.InRange.ToString()
                    + " Inverted: " + d.Inverted.ToString()
                    + " Name: " + d.Name
                    + " Buttons: " + d.StylusButtons.ToText());
            }
            return sb.ToString();
        }

        public static string ToText(this StylusDevice d)
        {
            if (d != null)
            {
                return " Id: " + d.Id.ToString()
                    + " Position: " + d.GetPosition(d.Target).ToText()
                    + " InAir: " + d.InAir.ToString()
                    + " InRange: " + d.InRange.ToString()
                    + " Inverted: " + d.Inverted.ToString()
                    + " Name: " + d.Name
                    + " Buttons: " + d.StylusButtons.ToText()
                    + " StylusDeviceCount: " + d.TabletDevice.StylusDevices.Count
                    + " " + d.TabletDevice.StylusDevices.ToText();
            }
            else
            {
                return " StylusDevice: null ";
            }
        }

        public static string ToText(this StylusButtonCollection bts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (StylusButton bt in bts)
            {
                sb.AppendFormat(" {0}: {1} ", bt.Name, bt.StylusButtonState.ToString());
            }
            return sb.ToString();
        }

        public static string ToText(this StylusButtonEventArgs e)
        {
            return e.StylusButton.Name + e.StylusButton.StylusButtonState.ToString() + e.StylusDevice.ToText();
        }

        public static string ToText(this StylusDownEventArgs e)
        {
            return "TapCount: " + e.TapCount + " " + e.StylusDevice.ToText();
        }

        public static string ToText(this StylusSystemGestureEventArgs e)
        {
            return "System Gesture: " + e.SystemGesture.ToString() + " " + e.StylusDevice.ToText();
        }
    }

    public static class MouseHelpers
    {
        public static string ToText(this MouseEventArgs e)
        {
            return " Position: " + e.GetPosition((IInputElement)e.Source).ToText()
                + " " + e.MouseDevice.ToText()
                + " " + e.StylusDevice.ToText();
        }

        public static string ToText(this MouseDevice d)
        {
            if (d != null)
            {
                return "Position: " + d.GetPosition(d.Target).ToText()
                    + " Left: " + d.LeftButton.ToString()
                    + " Middle: " + d.MiddleButton.ToString()
                    + " Right: " + d.RightButton.ToString();
            }
            else
            {
                return "MouseDevice: null";
            }
        }

        public static string ToText(this Point p)
        {
            return String.Format("X: {0} Y: {1}", p.X, p.Y);
        }

        public static string ToText(this MouseButtonEventArgs e)
        {
            return e.ChangedButton.ToString() + ": " + e.ButtonState.ToString()
                + " Click Count: " + e.ClickCount
                + " " + e.MouseDevice.ToText()
                + " " + e.StylusDevice.ToText();
        }

        public static string ToText(this MouseWheelEventArgs e)
        {
            return " Delta: " + e.Delta.ToString()
                + " " + e.MouseDevice.ToText()
                + " " + e.StylusDevice.ToText();
        }
    }
}