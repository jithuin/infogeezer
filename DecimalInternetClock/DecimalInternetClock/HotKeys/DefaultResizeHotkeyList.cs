using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace DecimalInternetClock.HotKeys
{
    public class DefaultResizeHotkeyList : List<ResizerHotKey>
    {
        protected enum EWinPosVert
        {
            Top,
            Centre,
            Bottom,
            Total,
        }

        protected enum EWinPosHoriz
        {
            Left,
            Middle,
            Right,
            Total,
        }

        protected class WinPos
        {
            public WinPos() { }

            public WinPos(EWinPosVert vpos_in, EWinPosHoriz hpos_in)
            {
                HPos = hpos_in;
                VPos = vpos_in;
            }

            public WinPos(EWinPosHoriz hpos_in, EWinPosVert vpos_in)
                : this(vpos_in, hpos_in)
            { }

            public EWinPosHoriz HPos;

            public EWinPosVert VPos;
        }

        protected static DefaultResizeHotkeyList _instance = null;

        public static DefaultResizeHotkeyList Instance
        {
            get
            {
                if (_instance == null)
                    Init();
                return _instance;
            }
        }

        public static void Init()
        {
            _instance = new DefaultResizeHotkeyList();
        }

        List<double> _portions = new List<double>() { 0.25, 1.0 / 3, 0.5, 0.75 };

        FKeyModifiers _defaultModifiers = FKeyModifiers.Alt | FKeyModifiers.Ctrl;

        Dictionary<WinPos, List<Keys>> _posCommandKey = new Dictionary<WinPos, List<Keys>>()
        {
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Total), new List<Keys>(){Keys.Left}},
            //{new WinPos(EWinPosHoriz.Middle, EWinPosVert.Total), new List<Keys>(){Keys.Left, Keys.NumPad4}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Total), new List<Keys>(){Keys.Right}},
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Top), new List<Keys>(){Keys.NumPad7}},
            {new WinPos(EWinPosHoriz.Middle, EWinPosVert.Top), new List<Keys>(){Keys.NumPad8}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Top), new List<Keys>(){Keys.NumPad9}},
            {new WinPos(EWinPosHoriz.Total, EWinPosVert.Top), new List<Keys>(){Keys.Up}},
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Bottom), new List<Keys>(){Keys.NumPad1}},
            {new WinPos(EWinPosHoriz.Middle, EWinPosVert.Bottom), new List<Keys>(){Keys.NumPad2}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Bottom), new List<Keys>(){Keys.NumPad3}},
            {new WinPos(EWinPosHoriz.Total, EWinPosVert.Bottom), new List<Keys>(){Keys.Down}},
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Centre), new List<Keys>(){Keys.NumPad4}},
            {new WinPos(EWinPosHoriz.Middle, EWinPosVert.Centre), new List<Keys>(){Keys.NumPad5}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Centre), new List<Keys>(){Keys.NumPad6}},
            //{new WinPos(EWinPosHoriz.Total, EWinPosVert.Centre), new List<Keys>(){Keys.Left, Keys.NumPad4}},
        };

        protected DefaultResizeHotkeyList()
        {
            foreach (WinPos wp in _posCommandKey.Keys)
            {
                ResizerHotKey hk = new ResizerHotKey();
                foreach (Keys key in _posCommandKey[wp])
                    hk.Add(_defaultModifiers, key);

                hk.ResizeStates = new List<ResizerState>();
                foreach (double portion in _portions)
                    hk.ResizeStates.Add(CreateResizeState(wp, portion));
                hk.CurrentWindowChanged += new EventHandler(hk_CurrentWindowChanged);
                this.Add(hk);
            }
        }

        private ResizerState CreateResizeState(WinPos wp, double portion)
        {
            Vector location = new Vector();
            Vector size = new Vector();

            #region Size calculation
            if (wp.HPos == EWinPosHoriz.Total)
                size.X = 1;
            else
                size.X = portion;
            if (wp.VPos == EWinPosVert.Total)
                size.Y = 1;
            else
                size.Y = portion;
            #endregion Size calculation

            #region Location calculation
            switch (wp.HPos)
            {
                case EWinPosHoriz.Middle:
                    {
                        location.X = (1 - portion) / 2.0;

                        break;
                    }
                case EWinPosHoriz.Right:
                    {
                        location.X = 1 - portion;

                        break;
                    }
                case EWinPosHoriz.Left:
                case EWinPosHoriz.Total:
                default:
                    {
                        location.X = 0;

                        break;
                    }
            }
            switch (wp.VPos)
            {
                case EWinPosVert.Centre:
                    {
                        location.Y = (1 - portion) / 2.0;

                        break;
                    }
                case EWinPosVert.Bottom:
                    {
                        location.Y = 1 - portion;

                        break;
                    }
                case EWinPosVert.Top:
                case EWinPosVert.Total:
                default:
                    {
                        location.Y = 0;

                        break;
                    }
            }
            #endregion Location calculation

            return new ResizerState(location, size);
        }

        private void hk_CurrentWindowChanged(object sender, EventArgs e)
        {
            foreach (ResizerHotKey rhk in this)
                rhk.ChangeCurrentWindow();
        }
    }
}