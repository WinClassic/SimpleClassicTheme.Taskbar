using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class UXTheme
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SetWindowTheme(Integer hWnd, string pszSubAppName, string pszSubIdList);
    }
}