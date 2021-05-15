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

        public static void KeyUp(Keys vKey)
        {
            User32.keybd_event((byte)vKey, 0, User32.KEYEVENTF_EXTENDEDKEY | User32.KEYEVENTF_KEYUP, 0);
        }
    }
}