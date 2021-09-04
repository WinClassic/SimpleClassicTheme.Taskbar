using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>psapi.h</c>.
    /// </summary>
    internal class PSAPI
    {
        private const string _dllName = "psapi.dll";

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetModuleBaseNameW(IntPtr hProcess, IntPtr hModule, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBaseName, int nSize);

        public static string GetProcessModuleName(IntPtr hProcess)
        {
            int capacity = 2000;
            StringBuilder builder = new(capacity);

            if (GetModuleBaseNameW(hProcess, IntPtr.Zero, builder, capacity) == 0)
            {
                return string.Empty;
            }

            return builder.ToString();
        }
    }
}
