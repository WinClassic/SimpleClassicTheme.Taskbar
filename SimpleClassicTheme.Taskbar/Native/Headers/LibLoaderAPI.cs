using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>libloaderapi.h</c>.
    /// </summary>
    internal class LibLoaderAPI
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindResourceW(IntPtr hModule, string lpName, string lpType);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryExW(string lpLibFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);
    }
}
