using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Interfaces
{
    [ComImport]
	[Guid("0cd45e71-d927-4f15-8b0a-8fef525337bf")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopNotificationService
	{
		int Register(IVirtualDesktopNotification pNotification, out uint dwCookie);
		int Unregister(uint dwCookie);
	}
}
