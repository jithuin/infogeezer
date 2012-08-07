using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using ManagedWinapi;
using ManagedWinapi.Windows;

namespace DecimalInternetClock.HotKeys
{
    public class ResizerHotKey : HotKeyFeatureExtension
    {
        public ResizerHotKey() : base() { }

        public ResizerHotKey(KeyModifiers mod_in, System.Windows.Forms.Keys key_in)
        {
            base.ModifyHotKey(mod_in, key_in);
        }

        public List<ResizeState> ResizeStates;
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
            if (_currentWindow != SystemWindow.ForegroundWindow)
                ChangeCurrentWindow();
            if (SystemWindow.ForegroundWindow.WindowState != System.Windows.Forms.FormWindowState.Normal)
                SystemWindow.ForegroundWindow.WindowState = System.Windows.Forms.FormWindowState.Normal;

            SystemWindow.ForegroundWindow.Location = (System.Drawing.Point)ResizeStates[_statePointer].Location;
            SystemWindow.ForegroundWindow.Size = (System.Drawing.Size)ResizeStates[_statePointer].Size;

            if (_statePointer + 1 + 1 > ResizeStates.Count)
            {
                _statePointer = 0;
                _currentWindow.WindowState = _curWindowFormerState;
            }
            else
                _statePointer++;
        }

        private void ChangeCurrentWindow()
        {
            if (_currentWindow != null)
            {
                ResizeStates.RemoveAt(ResizeStates.Count - 1);// remove last
            }
            _currentWindow = SystemWindow.ForegroundWindow;
            ResizeStates.Add(new ResizeState(
                (Vector2D)_currentWindow.Location, (Vector2D)_currentWindow.Size));//add current window's
            _curWindowFormerState = _currentWindow.WindowState;
            _statePointer = 0;
        }
    }

    public class ResizeState
    {
        public ResizeState()
        {
            Location = new Vector2D();
            Size = new Vector2D();
        }

        public ResizeState(Vector2D location_in, Vector2D size_in)
        {
            Location = location_in;
            Size = size_in;
        }

        public Vector2D Location { get; set; }

        public Vector2D Size { get; set; }
    }

    //public class PercentSize
    //{
    //    public PercentSize() : this(0.0, 0.0) { ;}

    //    public PercentSize(double width_in, double height_in)
    //    {
    //        Width = width_in;
    //        Height = height_in;
    //    }

    //    public PercentSize(Size size_in)
    //    {
    //        SystemSize = size_in;
    //    }

    //    public Size SystemSize
    //    {
    //        get
    //        {
    //        }
    //        set;
    //    }

    //    public double Width { get; set; }

    //    public double Height { get; set; }
    //}
}