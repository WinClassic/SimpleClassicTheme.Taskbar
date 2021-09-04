using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    [Serializable]
	[StructLayout(LayoutKind.Sequential)]
	struct TBBUTTON64
	{
		public int iBitmap;

		public int idCommand;

		public byte fsState;

		public byte fsStyle;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public byte[] bReserved;

		public IntPtr dwData;

		public IntPtr iString;
	}
}
