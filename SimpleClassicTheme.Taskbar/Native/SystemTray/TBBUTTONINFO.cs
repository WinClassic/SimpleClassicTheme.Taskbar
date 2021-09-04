using System;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    internal struct TBBUTTONINFO
	{
		public IntPtr hwnd;
		public ulong pid;
		public string toolTip;
		public bool visible;
		public IntPtr icon;
		public uint callbackMessage;
		public uint id;
	}
}
