using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class ShObjIDL_Core
    {
        [ComImport]
        [Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IVirtualDesktopManager
        {
            bool IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow);

            Guid GetWindowDesktopId(IntPtr topLevelWindow);

            void MoveWindowToDesktop(IntPtr topLevelWindow, ref Guid desktopId);
        }


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

        [ComImport]
        [Guid("0cd45e71-d927-4f15-8b0a-8fef525337bf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IVirtualDesktopNotificationService
        {
            int Register(IVirtualDesktopNotification pNotification, out IntPtr dwCookie);

            int Unregister(IntPtr dwCookie);
        }
    }
}