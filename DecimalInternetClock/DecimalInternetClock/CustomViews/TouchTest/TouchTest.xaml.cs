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
using System.Windows.Shapes;

namespace DecimalInternetClock
{
    /// <summary>
    /// Interaction logic for TouchTest.xaml
    /// </summary>
    public partial class TouchTest : Window
    {
        public TouchTest()
        {
            InitializeComponent();
        }

        protected bool wasPreviousStylusMove = false;
        protected bool wasPreviousMouseMove = false;

        private void FeedBack(String text)
        {
            tbFeedBack.Text += String.Format("{0}\r\n", text);
            wasPreviousStylusMove = false;
            wasPreviousMouseMove = false;
        }

        #region Stylus Operations

        #region Stylus.

        private void cProbe_StylusButtonDown(object sender, StylusButtonEventArgs e)
        {
            FeedBack("StylusButtonDown" + e.ToText());
        }

        private void cProbe_StylusButtonUp(object sender, StylusButtonEventArgs e)
        {
            FeedBack("StylusButtonUp" + e.ToText());
        }

        private void cProbe_StylusDown(object sender, StylusDownEventArgs e)
        {
            FeedBack("StylusDown" + e.ToText());
        }

        private void cProbe_StylusEnter(object sender, StylusEventArgs e)
        {
            FeedBack("StylusEnter" + e.ToText());
        }

        private void cProbe_StylusInAirMove(object sender, StylusEventArgs e)
        {
            FeedBack("StylusInAirMove" + e.ToText());
        }

        private void cProbe_StylusInRange(object sender, StylusEventArgs e)
        {
            FeedBack("StylusInRange" + e.ToText());
        }

        private void cProbe_StylusLeave(object sender, StylusEventArgs e)
        {
            FeedBack("StylusLeave" + e.ToText());
        }

        private void cProbe_StylusMove(object sender, StylusEventArgs e)
        {
            if (!wasPreviousStylusMove)
            {
                tbFeedBack.Text += "StylusMove: " + e.ToText() + "\r\n";
                wasPreviousStylusMove = true;
            }
        }

        private void cProbe_StylusOutOfRange(object sender, StylusEventArgs e)
        {
            FeedBack("StylusOutOfRange" + e.ToText());
        }

        private void cProbe_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            FeedBack("StylusSystemGesture" + e.ToText());
        }

        private void cProbe_StylusUp(object sender, StylusEventArgs e)
        {
            FeedBack("StylusUp" + e.ToText());
        }

        #endregion Stylus.

        #endregion Stylus Operations

        #region Mouse Operations

        private void cProbe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FeedBack("MouseDown: " + e.ToText());
        }

        private void cProbe_MouseEnter(object sender, MouseEventArgs e)
        {
            FeedBack("MouseEnter: " + e.ToText());
        }

        private void cProbe_MouseLeave(object sender, MouseEventArgs e)
        {
            FeedBack("MouseLeave: " + e.ToText());
        }

        private void cProbe_MouseMove(object sender, MouseEventArgs e)
        {
            if (!wasPreviousMouseMove)
            {
                tbFeedBack.Text += "MouseMove: " + e.ToText() + "\r\n";
                wasPreviousMouseMove = true;
            }
        }

        private void cProbe_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FeedBack("MouseUp: " + e.ToText());
        }

        private void cProbe_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            FeedBack("MouseWheel: " + e.ToText());
        }

        #endregion Mouse Operations

        #region Scroll

        private void cProbe_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            tbFeedBack.Text += "Scroll\r\n";
        }

        private void cProbe_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            tbFeedBack.Text += "ScrollChanged\r\n";
        }

        #endregion Scroll

        #region Touch

        private void cProbe_TouchDown(object sender, TouchEventArgs e)
        {
            tbFeedBack.Text += "TouchDown\r\n";
        }

        private void cProbe_TouchEnter(object sender, TouchEventArgs e)
        {
            tbFeedBack.Text += "TouchEnter\r\n";
        }

        private void cProbe_TouchLeave(object sender, TouchEventArgs e)
        {
            tbFeedBack.Text += "TouchLeave\r\n";
        }

        private void cProbe_TouchMove(object sender, TouchEventArgs e)
        {
            tbFeedBack.Text += "TouchMove\r\n";
        }

        private void cProbe_TouchUp(object sender, TouchEventArgs e)
        {
            tbFeedBack.Text += "TouchUp\r\n";
        }

        #endregion Touch

        #region Manipulation

        private void cProbe_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            tbFeedBack.Text += "ManipulationCompleted\r\n";
        }

        private void cProbe_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            tbFeedBack.Text += "ManipulationDelta\r\n";
        }

        private void cProbe_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            tbFeedBack.Text += "ManipulationInertiaStarting\r\n";
        }

        private void cProbe_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            tbFeedBack.Text += "ManipulationStarted\r\n";
        }

        private void cProbe_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            tbFeedBack.Text += "ManipulationStarting\r\n";
        }

        #endregion Manipulation
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