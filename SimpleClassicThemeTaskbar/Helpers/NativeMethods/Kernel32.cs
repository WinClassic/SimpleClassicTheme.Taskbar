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

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool CloseHandle(Integer hHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern Integer32 GetApplicationUserModelId(IntPtr hProcess, ref Integer32 applicationUserModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern Integer32 GetCurrentThreadId();

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern Integer32 GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, Integer32 size, string filePath);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern Integer32 GetShortPathName([MarshalAs(UnmanagedType.LPWStr)] string path, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder shortPath, Integer32 shortPathLength);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern Integer32 OpenProcess(Integer32 dwDesiredAccess, bool bInheritHandle, Integer32 dwProcessId);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool ReadProcessMemory(Integer hProcess, Integer lpBaseAddress, [Out] byte[] buffer, Integer32 dwSize, out Integer32 lpNumberOfBytesRead);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern Integer VirtualAllocEx(Integer hProcess, Integer lpAddress, Integer32 dwSize, Integer32 flAllocationType, Integer32 flProtect);
		
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool VirtualFreeEx(Integer hProcess, Integer lpAddress, Integer32 dwSize, Integer32 dwFreeType);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
	}
}