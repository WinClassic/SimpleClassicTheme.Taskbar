using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
	public partial class Taskbar : Form
	{

        #region Win32

        public const int GCL_HICONSM = -34;
        public const int GCL_HICON = -14;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;

        public const int WM_GETICON = 0x007F;
        public const int WM_ENDSESSION = 0x0016;
        public const int WM_QUERYENDSESSION = 0x0011;

        public const int WM_SCT = 0x0420;
        public const int SCTWP_EXIT = 0x0001;
        public const int SCTWP_ISMANAGED = 0x0002;
        public const int SCTWP_ISSCT = 0x0003;
        public const int SCTLP_FORCE = 0x0001;

        public const int MDITILE_ZORDER = 0x0004;
        public const int MDITILE_VERTICAL = 0x0000;
        public const int MDITILE_HORIZONTAL = 0x0001;

        public delegate bool EnumWindowsCallback(IntPtr hWnd, int lParam);

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return User32.GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(User32.GetClassLongPtr32(hWnd, nIndex));
        }


        public static Icon GetAppIcon(Window wnd)
        {
            IntPtr hwnd = wnd.Handle;
            //Try different ways of getting the icon
            IntPtr iconHandle = User32.SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);
            if (iconHandle == IntPtr.Zero)
                iconHandle = User32.SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = User32.SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
            if (iconHandle == IntPtr.Zero)
                return null;
            Icon icon = Icon.FromHandle(iconHandle);
            return icon;
        }

        static IntPtr GetLastActivePopupOfWindow(IntPtr root)
        {
            IntPtr lastPopup = User32.GetLastActivePopup(root);
            if (User32.IsWindowVisible(lastPopup))
                return lastPopup;
            if (lastPopup == root)
                return IntPtr.Zero;
            return GetLastActivePopupOfWindow(lastPopup);
        }


      

        #endregion
    }
}
