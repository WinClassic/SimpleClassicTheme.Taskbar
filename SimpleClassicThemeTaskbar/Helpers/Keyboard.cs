using System;
using System.Windows.Forms;

using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

namespace SimpleClassicThemeTaskbar.Helpers
{
    internal static class Keyboard
    {
        public static void KeyDown(Keys vKey)
        {
            User32.keybd_event((byte)vKey, 0, User32.KEYEVENTF_EXTENDEDKEY, 0);
        }

        public static void KeyPress(params Keys[] keys)
        {
            // Push down keys in order
            for (int i = 0; i < keys.Length; i++)
                KeyDown(keys[i]);

            // Release keys backwards
            for (int i = keys.Length - 1; i >= 0; i--)
                KeyUp(keys[i]);
        }

        public static void KeyUp(Keys vKey)
        {
            User32.keybd_event((byte)vKey, 0, User32.KEYEVENTF_EXTENDEDKEY | User32.KEYEVENTF_KEYUP, 0);
        }
    }
}