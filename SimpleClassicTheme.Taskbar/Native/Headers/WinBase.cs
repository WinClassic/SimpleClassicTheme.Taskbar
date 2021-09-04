using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>winbase.h</c>.
    /// </summary>
    internal class WinBase
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        internal static string GetProcessFileName(IntPtr hProcess)
        {
            int capacity = 2000;
            StringBuilder builder = new(capacity);

            if (!QueryFullProcessImageNameW(hProcess, 0, builder, ref capacity))
            {
                return string.Empty;
            }

            return builder.ToString();
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        internal static extern bool QueryFullProcessImageNameW([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool QueryFullProcessImageNameW([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);
    }
}
