using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>handleapi.h</c>.
    /// </summary>
    internal class HandleAPI
    {
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hHandle);
    }
}
