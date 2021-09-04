
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.CommCtrl;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>shellapi.h</c>.
    /// </summary>
    internal static class ShellApi
    {
        private const string _dllName = "shell32.dll";

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        internal const int SHGFI_LARGEICON = 0x0;
        internal const int SHGFI_SMALLICON = 0x1;
        internal const int SHIL_EXTRALARGE = 0x2;
        internal const int SHIL_JUMBO = 0x4;

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetFileInfo(string pszPath, Integer32 dwFileAttributes, ref SHFILEINFO psfi, Integer32 cbFileInfo, Integer32 uFlags);

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        [DllImport(_dllName)]
        internal static extern int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);
    }
}
