using System;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    internal static class Keyboard
    {
        public static void KeyDown(IntPtr hWnd, Keys vKey)
        {
            _ = PostMessage(hWnd, WM_KEYDOWN, (uint)vKey, 0);
        }

        public static void KeyDown(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
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

        public static void KeyPress(IntPtr hWnd, params Keys[] keys)
        {
            // Push down keys in order
            for (int i = 0; i < keys.Length; i++)
                KeyDown(hWnd, keys[i]);

            // Release keys backwards
            for (int i = keys.Length - 1; i >= 0; i--)
                KeyUp(hWnd, keys[i]);
        }

        public static void KeyUp(IntPtr hWnd, Keys vKey)
        {
            _ = PostMessage(hWnd, WM_KEYUP, (uint)vKey, 0);
        }

        public static void KeyUp(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}