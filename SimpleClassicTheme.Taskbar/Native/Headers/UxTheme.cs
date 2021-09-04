using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    internal static class UxTheme
    {
        [DllImport(nameof(UxTheme), SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }
}