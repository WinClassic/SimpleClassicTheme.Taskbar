using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class UXTheme
    {
        [DllImport(nameof(UXTheme), SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }
}