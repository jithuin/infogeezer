using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DecimalInternetClock.HotKeys
{
    public class ResizerHotkeyState
    {
        public ResizerHotkeyState()
        {
            Location = new Vector();
            Size = new Vector();
        }

        public ResizerHotkeyState(Vector location_in, Vector size_in)
        {
            Location = location_in;
            Size = size_in;
        }

        public Vector Location { get; set; }

        public Vector Size { get; set; }
    }
}