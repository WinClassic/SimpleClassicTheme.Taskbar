using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>appmodel.h</c>.
    /// </summary>
    internal class AppModel
    {
        public const int APPMODEL_ERROR_NO_APPLICATION = 0x00003D57;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetApplicationUserModelId(IntPtr hProcess, ref uint applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);
    }
}
