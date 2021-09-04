using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>processthreadsapi.h</c>.
    /// </summary>
    internal class ProcessThreadsAPI
    {
        public const int PROCESS_ALL_ACCESS = 0x001FFFFF;
        public const int PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000;
        public const int PROCESS_VM_READ = 0x10;
        public const uint STILL_ACTIVE = 259;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
    }
}
