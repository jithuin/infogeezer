using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ManagedWinapi
{
    public class MouseHelper
    {
        /// <summary>
        /// Inject a mouse event into the event loop, as if the user performed
        /// it with his mouse.
        /// </summary>
        public static void InjectMouseEvent(uint flags, uint dx, uint dy, uint data, UIntPtr extraInfo)
        {
            mouse_event(flags, dx, dy, data, extraInfo);
        }

        public static void InjectMouseEvent(MouseEventFlagValues flags, uint dx, uint dy, uint data, UIntPtr extraInfo)
        {
            mouse_event((uint)flags, dx, dy, data, extraInfo);
        }

        public static void Click()
        {
            InjectMouseEvent(MouseEventFlagValues.LEFTDOWN);
            InjectMouseEvent(MouseEventFlagValues.LEFTUP);
            Thread.Sleep(100);
        }

        public static void InjectMouseEvent(MouseEventFlagValues flags)
        {
            MouseHelper.InjectMouseEvent(flags, (uint)0, (uint)0, (uint)0, UIntPtr.Zero);
        }

        public static void InjectMouseEvent(MouseEventFlagValues flags, System.Drawing.Point relativePoint_in)
        {
            MouseHelper.InjectMouseEvent(flags, (uint)relativePoint_in.X, (uint)relativePoint_in.Y, (uint)0, UIntPtr.Zero);
        }

        public static void RightClick()
        {
            InjectMouseEvent(MouseEventFlagValues.RIGHTDOWN);
            InjectMouseEvent(MouseEventFlagValues.RIGHTUP);
            Thread.Sleep(100);
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData,
           UIntPtr dwExtraInfo);

        public static void RightClick(System.Drawing.Point relativePoint_in)
        {
            InjectMouseEvent(MouseEventFlagValues.RIGHTDOWN, relativePoint_in);
            InjectMouseEvent(MouseEventFlagValues.RIGHTUP);
            Thread.Sleep(100);
        }
    }

    [Flags]
    public enum MouseEventFlagValues
    {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010,
        WHEEL = 0x00000800,
        HWHEEL = 0x00001000
    }
}