using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Helpers.NativeMethods
{
    internal static class UXTheme
    {
        [DllImport(nameof(UXTheme), SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }
}