using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using DecimalInternetClock.Helpers;
using ManagedWinapi;
using ManagedWinapi.Windows;

namespace DecimalInternetClock.HotKeys
{
    public class ResizerHotKey : HotKeyFeatureExtension
    {
        public ResizerHotKey() : base() { }

        public ResizerHotKey(FKeyModifiers mod_in, Keys key_in)

            : base(mod_in, key_in)
        { }

        public List<ResizerHotkeyState> ResizeStates;
        protected int _statePointer = 0;
        protected SystemWindow _currentWindow = null;

        protected System.Windows.Forms.FormWindowState _curWindowFormerState;

        protected override void HotKeyFeatureExtension_HotkeyPressed(object sender, EventArgs e)
        {
            if (ResizeStates == null || ResizeStates.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("There are no window state connected to this hotkey!");
                return;
            }
            ChangeCurrentWindow();
            if (SystemWindow.ForegroundWindow.WindowState != System.Windows.Forms.FormWindowState.Normal)
                SystemWindow.ForegroundWindow.WindowState = System.Windows.Forms.FormWindowState.Normal;

            // window setting
            Screen screen = Screen.FromPoint(_currentWindow.Location);
            SystemWindow.ForegroundWindow.Location = CalculateLocation(screen.WorkingArea, ResizeStates[_statePointer].Location);
            SystemWindow.ForegroundWindow.Size = CalculateSize(screen.WorkingArea.Size, ResizeStates[_statePointer].Size);

            // state iteration
            if (_statePointer + 1 + 1 > ResizeStates.Count)
            {
                _statePointer = 0;
                _currentWindow.WindowState = _curWindowFormerState;
            }
            else
                _statePointer++;
        }

        private System.Drawing.Point CalculateLocation(System.Drawing.Rectangle workingArea_in, Vector scaleVector_in)
        {
            Matrix scale = new Matrix();
            scale.SetIdentity();
            scale.Scale(scaleVector_in.X, scaleVector_in.Y);

            return (scale.Transform(workingArea_in.Size.ToVector()) + workingArea_in.Location.ToVector()).ToPoint();
        }

        private Vector ScaleVectorFromLocation(System.Drawing.Rectangle workingArea_in, System.Drawing.Point AbsoluteLocation_in)
        {
            Matrix scale = new Matrix();
            scale.SetIdentity();
            scale.Scale(1.0 / workingArea_in.Size.Width, 1.0 / workingArea_in.Size.Height);

            return scale.Transform(AbsoluteLocation_in.ToVector() - workingArea_in.Location.ToVector());
        }

        private System.Drawing.Size CalculateSize(System.Drawing.Size screenSize_in, Vector scaleVector_in)
        {
            Matrix scale = new Matrix();
            scale.SetIdentity();
            scale.Scale(scaleVector_in.X, scaleVector_in.Y);

            return scale.Transform(screenSize_in.ToVector()).ToSize();
        }

        private Vector ScaleVectorFromSize(System.Drawing.Size screenSize_in, System.Drawing.Size absoluteSize_in)
        {
            Matrix scale = new Matrix();
            scale.SetIdentity();
            scale.Scale(1.0 / screenSize_in.Width, 1.0 / screenSize_in.Height);

            return scale.Transform(absoluteSize_in.ToVector());
        }

        public void ChangeCurrentWindow()
        {
            if (_currentWindow != SystemWindow.ForegroundWindow)
            {
                if (_currentWindow != null)
                {
                    ResizeStates.RemoveAt(ResizeStates.Count - 1);// remove last (resize state of the former window )
                }
                _currentWindow = SystemWindow.ForegroundWindow;
                Screen screen = Screen.FromPoint(_currentWindow.Location);
                ResizeStates.Add(new ResizerHotkeyState(
                    ScaleVectorFromLocation(screen.WorkingArea, _currentWindow.Location),
                    ScaleVectorFromSize(screen.WorkingArea.Size, _currentWindow.Size)));//add resize state of the current window
                _curWindowFormerState = _currentWindow.WindowState;
                _statePointer = 0;
                OnCurrentWindowChanged();
            }
        }

        public event EventHandler CurrentWindowChanged;

        protected void OnCurrentWindowChanged()
        {
            if (CurrentWindowChanged != null)
            {
                CurrentWindowChanged.Invoke(this, new EventArgs());
            }
        }
    }
}