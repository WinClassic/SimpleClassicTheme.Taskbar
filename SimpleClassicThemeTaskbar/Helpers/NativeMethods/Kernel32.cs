using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class Kernel32
    {
        internal const int APPMODEL_ERROR_NO_APPLICATION = 0x00003D57;
        internal const int ERROR_INSUFFICIENT_BUFFER = 0x0000007a;
        internal const int ERROR_SUCCESS = 0x00000000;
        internal const int MEM_COMMIT = 0x00001000;
        internal const int MEM_RELEASE = 0x00008000;
        internal const int PAGE_READWRITE = 0x00000004;
        internal const int PROCESS_ALL_ACCESS = 0x001FFFFF;
        internal const int PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000;
        internal const int PROCESS_VM_READ = 0x10;

        [DllImport(nameof(Kernel32))]
        internal static extern bool CloseHandle(IntPtr hHandle);

        [DllImport(nameof(Kernel32), CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindResourceW(IntPtr hModule, string lpName, string lpType);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref UInt32 applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);

        [DllImport(nameof(Kernel32))]
        internal static extern uint GetCurrentThreadId();

        [DllImport(nameof(Kernel32), CharSet = CharSet.Unicode)]
        internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport(nameof(Kernel32), CharSet = CharSet.Auto)]
        internal static extern int GetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string path, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder shortPath, int shortPathLength);

        [DllImport(nameof(Kernel32), SetLastError = true, ExactSpelling = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadLibraryExA(string lpLibFileName, IntPtr hFile, uint dwFlags);

        [DllImport(nameof(Kernel32))]
        internal static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport(nameof(Kernel32))]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport(nameof(Kernel32), SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int flAllocationType, int flProtect);

        [DllImport(nameof(Kernel32), SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int dwFreeType);

        [DllImport(nameof(Kernel32), CharSet = CharSet.Unicode)]
        internal static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport(nameof(Kernel32), CharSet = CharSet.Ansi, ExactSpelling = true)]
        internal static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport(nameof(Kernel32), ExactSpelling = true)]
        internal static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport(nameof(Kernel32), SetLastError = true)]
        internal static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        [DllImport(nameof(Kernel32), CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBaseName, int nSize);

        internal static string GetProcessFileName(int pid)
        {
            int capacity = 2000;
            StringBuilder builder = new(capacity);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
            if (!QueryFullProcessImageName(hProcess, 0, builder, ref capacity))
            {
                CloseHandle(hProcess);
                return String.Empty;
            }
            CloseHandle(hProcess);
            return builder.ToString();
        }

        internal static string GetProcessModuleName(int pid)
        {
            int capacity = 2000;
            StringBuilder builder = new(capacity);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION | PROCESS_VM_READ, false, pid);
            if (!GetModuleBaseName(hProcess, IntPtr.Zero, builder, capacity))
            {
                CloseHandle(hProcess);
                return String.Empty;
			}
            CloseHandle(hProcess);
            return builder.ToString();
        }
    }
}