using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class DwmApi
    {
        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out int pvAttribute, int cbAttribute);
    }
}