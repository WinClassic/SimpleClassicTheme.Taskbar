using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
	internal static class Kernel32
	{
		internal const int APPMODEL_ERROR_NO_APPLICATION = 0x00003D57;
		internal const int PROCESS_ALL_ACCESS = 0x001FFFFF;
		internal const int PROCESS_QUERY_LIMITED_INFORMATION = 0x00001000;
		internal const int ERROR_INSUFFICIENT_BUFFER = 0x0000007a;
		internal const int ERROR_SUCCESS = 0x00000000;
		internal const int MEM_COMMIT = 0x00001000;
		internal const int MEM_RELEASE = 0x00008000;
		internal const int PAGE_READWRITE = 0x00000004;

		[DllImport("kernel32.dll")]
		internal static extern bool CloseHandle(IntPtr hHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern Int32 GetApplicationUserModelId(IntPtr hProcess, ref UInt32 applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);

		[DllImport("kernel32")]
		internal static extern uint GetCurrentThreadId();

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		internal static extern int GetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string path, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder shortPath, int shortPathLength);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, int dwSize, out int lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int flAllocationType, int flProtect);
		
		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int dwFreeType);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		internal static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
	}
}