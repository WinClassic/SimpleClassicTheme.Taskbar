using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Helpers.NativeMethods
{
    internal static class Gdi32
    {
        [DllImport(nameof(Gdi32))]
        internal static extern void SetTextColor(IntPtr hdc, uint color);

        [DllImport(nameof(Gdi32), SetLastError = false)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
    }
}
