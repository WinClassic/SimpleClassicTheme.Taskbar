using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    [Serializable]
	[StructLayout(LayoutKind.Sequential)]
	struct TRAYDATA
	{
		public IntPtr hwnd;

		public uint uID;

		public uint uCallbackMessage;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Reserved;

		public IntPtr hIcon;
	}
}
