using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class Gdi32
    {
        [DllImport(nameof(Gdi32))]
        internal static extern void SetTextColor(IntPtr hdc, uint color);

        [DllImport(nameof(Gdi32))]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
    }
}
