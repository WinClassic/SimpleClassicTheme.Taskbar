
using System;
using System.Drawing;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Taskbar : Form
    {
        #region Win32

        public static Icon GetAppIcon(Window wnd)
        {
            IntPtr hwnd = wnd.Handle;

            // Try different ways of getting the icon
            IntPtr iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);

            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);

            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);

            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);

            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICON);

            if (iconHandle == IntPtr.Zero)
                return null;

            Icon icon = Icon.FromHandle(iconHandle);
            return icon;
        }

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        private static IntPtr GetLastActivePopupOfWindow(IntPtr root)
        {
            IntPtr lastPopup = GetLastActivePopup(root);
            if (IsWindowVisible(lastPopup))
                return lastPopup;
            if (lastPopup == root)
                return IntPtr.Zero;
            return GetLastActivePopupOfWindow(lastPopup);
        }

        #endregion Win32
    }
}