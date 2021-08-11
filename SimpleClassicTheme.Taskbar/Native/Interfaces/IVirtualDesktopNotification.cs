using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Interfaces
{
    [ComImport]
	[Guid("c179334c-4295-40d3-bea1-c654d965605a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopNotification
	{
		void VirtualDesktopCreated(IntPtr pDesktop);
		void VirtualDesktopDestroyBegin(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
		void VirtualDesktopDestroyFailed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
		void VirtualDesktopDestroyed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
		void ViewVirtualDesktopChanged(IntPtr pView);
		void CurrentVirtualDesktopChanged(IntPtr pDesktopOld, IntPtr pDesktopNew);
	}
}
