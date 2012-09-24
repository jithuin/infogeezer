using System;

namespace ManagedWinapi
{
    public interface IHotkey
    {
        bool Alt { get; set; }

        bool Ctrl { get; set; }

        bool Enabled { get; set; }

        event EventHandler HotkeyPressed;

        System.Windows.Forms.Keys KeyCode { get; set; }

        bool Shift { get; set; }

        bool WindowsKey { get; set; }
    }
}