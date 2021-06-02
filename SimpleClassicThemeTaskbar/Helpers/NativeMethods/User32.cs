using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class User32
    {
        internal const uint DFC_BUTTON = 4;
        internal const uint DFCS_BUTTONPUSH = 0x10;
        internal const uint DFCS_PUSHED = 512;
        internal const int GCL_HICON = -14;
        internal const int GCL_HICONSM = -34;
        internal const int ICON_BIG = 1;
        internal const int ICON_SMALL = 0;
        internal const int ICON_SMALL2 = 2;
        internal const int KEYEVENTF_EXTENDEDKEY = 1;
        internal const int KEYEVENTF_KEYUP = 2;
        internal const int MDITILE_HORIZONTAL = 0x0001;
        internal const int MDITILE_VERTICAL = 0x0000;
        internal const int MDITILE_ZORDER = 0x0004;
        internal const uint MF_BITMAP = 0x00000004;
        internal const uint MF_CHECKED = 0x00000008;
        internal const uint MF_DISABLED = 0x00000002;
        internal const uint MF_ENABLED = 0x00000000;
        internal const uint MF_GRAYED = 0x00000001;
        internal const uint MF_MENUBARBREAK = 0x00000020;

        internal const uint MF_MENUBREAK = 0x00000040;

        internal const uint MF_OWNERDRAW = 0x00000100;

        internal const uint MF_POPUP = 0x00000010;

        internal const uint MF_SEPARATOR = 0x00000800;

        internal const uint MF_STRING = 0x00000000;

        internal const uint MF_UNCHECKED = 0x00000000;

        internal const int SW_SHOW = 5;

        internal const int TB_BUTTONCOUNT = 0x0418;

        internal const int TB_GETBUTTON = 0x0417;

        internal const int TBSTATE_HIDDEN = 0x0008;

        internal const uint TPM_BOTTOMALIGN = 0x0020;

        internal const uint TPM_CENTERALIGN = 0x0004;

        internal const uint TPM_HORNEGANIMATION = 0x0800;

        internal const uint TPM_HORPOSANIMATION = 0x0400;

        internal const uint TPM_LEFTALIGN = 0x0000;

        internal const uint TPM_LEFTBUTTON = 0x0000;

        internal const uint TPM_NOANIMATION = 0x4000;

        internal const uint TPM_NONOTIFY = 0x0080;

        internal const uint TPM_RETURNCMD = 0x0100;

        internal const uint TPM_RIGHTALIGN = 0x0008;

        internal const uint TPM_RIGHTBUTTON = 0x0002;

        internal const uint TPM_TOPALIGN = 0x0000;

        internal const uint TPM_VCENTERALIGN = 0x0010;

        internal const uint TPM_VERNEGANIMATION = 0x2000;

        internal const uint TPM_VERPOSANIMATION = 0x2000;

        internal const uint VK_F4 = 0x73;

        internal const uint VK_MENU = 0x12;

        internal const uint WM_CLOSE = 0x0010;

        internal const int WM_ENDSESSION = 0x0016;

        internal const int WM_GETICON = 0x007F;

        internal const uint WM_KEYDOWN = 0x0100;

        internal const uint WM_KEYUP = 0x0101;

        internal const uint WM_LBUTTONDOWN = 0x0201;

        internal const uint WM_LBUTTONUP = 0x0202;

        internal const int WM_QUERYENDSESSION = 0x0011;

        internal const uint WM_RBUTTONDOWN = 0x0204;

        internal const uint WM_RBUTTONUP = 0x0205;

        internal const int WM_SYSCOMMAND = 0x0112;

        internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        internal delegate bool EnumWindowsCallback(IntPtr hWnd, int lParam);

        internal delegate IntPtr WindowsHookProcedure(ShellEvents nCode, IntPtr wParam, IntPtr lParam);

        internal enum ShellEvents : int
        {
            HSHELL_WINDOWCREATED = 1,
            HSHELL_WINDOWDESTROYED = 2,
            HSHELL_ACTIVATESHELLWINDOW = 3,
            HSHELL_WINDOWACTIVATED = 4,
            HSHELL_GETMINRECT = 5,
            HSHELL_REDRAW = 6,
            HSHELL_TASKMAN = 7,
            HSHELL_LANGUAGE = 8,
            HSHELL_ACCESSIBILITYSTATE = 11,
            HSHELL_APPCOMMAND = 12
        }

        internal enum ShellHookId : int
        {
            WH_SHELL = 0xa,
            WH_CBT = 0x5
        }

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern bool AppendMenu(IntPtr hMenu, uint uFlags, int uIDNewItem, string lpNewItem);

        [DllImport(nameof(User32), CharSet = CharSet.Auto)]
        internal static extern bool AppendMenu(IntPtr hMenu, uint uFlags, int uIDNewItem, IntPtr lpNewItem);

        [DllImport(nameof(User32))]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, ShellEvents nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(nameof(User32))]
        internal static extern ushort CascadeWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport(nameof(User32))]
        internal static extern IntPtr CreatePopupMenu();

        [DllImport(nameof(User32))]
        internal static extern bool DeleteMenu(IntPtr hMenu, int uPosition, uint uFlags);

        /// <summary>
        /// Destroys the specified menu and frees any memory that the menu occupies.
        /// </summary>
        /// <param name="hMenu">A handle to the menu to be destroyed.</param>
        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport(nameof(User32))]
        internal static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

        [DllImport(nameof(User32))]
        internal static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport(nameof(User32))]
        internal static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowExW(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowW(string a, string b);

        [DllImport(nameof(User32))]
        internal static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport(nameof(User32), EntryPoint = "GetClassLong")]
        internal static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport(nameof(User32), EntryPoint = "GetClassLongPtr")]
        internal static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport(nameof(User32))]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport(nameof(User32))]
        internal static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport(nameof(User32))]
        internal static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport(nameof(User32))]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport(nameof(User32))]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint gwFlags);

        [DllImport(nameof(User32))]
        internal static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport(nameof(User32), EntryPoint = "GetWindowTextA", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowText(IntPtr hwnd, System.Text.StringBuilder lpString, int cch);

        [DllImport(nameof(User32), EntryPoint = "GetWindowTextLengthA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern bool GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(nameof(User32))]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport(nameof(User32))]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(nameof(User32))]
        internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport(nameof(User32))]
        internal static extern IntPtr LoadBitmapA(IntPtr hModule, string lpBitmapName);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool RegisterShellHookWindow(IntPtr hwnd);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport(nameof(User32), CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(nameof(User32), CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(nameof(User32))]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport(nameof(User32), CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(ShellHookId idHook, WindowsHookProcedure lpfn, IntPtr hmod, uint dwThreadId);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool SetWindowTextW(IntPtr hWnd, string lpString);

        [DllImport(nameof(User32))]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport(nameof(User32))]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(nameof(User32))]
        internal static extern ushort TileWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport(nameof(User32))]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [StructLayout(LayoutKind.Sequential)]
        internal struct ICONINFO
        {
            /// <summary>
            /// Specifies whether this structure defines an icon or a cursor.
            /// A value of TRUE specifies an icon; FALSE specifies a cursor
            /// </summary>
            internal bool fIcon;

            /// <summary>
            /// The x-coordinate of a cursor's hot spot
            /// </summary>
            internal Int32 xHotspot;

            /// <summary>
            /// The y-coordinate of a cursor's hot spot
            /// </summary>
            internal Int32 yHotspot;

            /// <summary>
            /// The icon bitmask bitmap
            /// </summary>
            internal IntPtr hbmMask;

            /// <summary>
            /// A handle to the icon color bitmap.
            /// </summary>
            internal IntPtr hbmColor;
        }
    }
}