using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Taskbar : Form
    {
        #region Win32

        public static Icon GetAppIcon(Window wnd)
        {
            IntPtr hwnd = wnd.Handle;

            // Try different ways of getting the icon
            IntPtr iconHandle = User32.SendMessage(hwnd, User32.WM_GETICON, User32.ICON_SMALL2, 0);

            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, User32.GCL_HICONSM);

            if (iconHandle == IntPtr.Zero)
                iconHandle = User32.SendMessage(hwnd, User32.WM_GETICON, User32.ICON_SMALL, 0);

            if (iconHandle == IntPtr.Zero)
                iconHandle = User32.SendMessage(hwnd, User32.WM_GETICON, User32.ICON_BIG, 0);

            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, User32.GCL_HICON);

            if (iconHandle == IntPtr.Zero)
                return null;

            Icon icon = Icon.FromHandle(iconHandle);
            return icon;
        }

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return User32.GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(User32.GetClassLongPtr32(hWnd, nIndex));
        }

        private static IntPtr GetLastActivePopupOfWindow(IntPtr root)
        {
            IntPtr lastPopup = User32.GetLastActivePopup(root);
            if (User32.IsWindowVisible(lastPopup))
                return lastPopup;
            if (lastPopup == root)
                return IntPtr.Zero;
            return GetLastActivePopupOfWindow(lastPopup);
        }

        #endregion Win32
    }
}