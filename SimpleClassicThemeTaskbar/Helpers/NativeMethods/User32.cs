using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class User32
    {
        internal const int DFC_BUTTON = 4;
        internal const int DFCS_BUTTONPUSH = 0x10;
        internal const int DFCS_PUSHED = 512;
        internal const int GCL_HICON = -14;
        internal const int GCL_HICONSM = -34;
        internal const int GW_HWNDFIRST = 0;
        internal const int GWL_EXSTYLE = -20;
        internal const int ICON_BIG = 1;
        internal const int ICON_SMALL = 0;
        internal const int ICON_SMALL2 = 2;
        internal const int KEYEVENTF_EXTENDEDKEY = 1;
        internal const int KEYEVENTF_KEYUP = 2;
        internal const int MDITILE_HORIZONTAL = 0x0001;
        internal const int MDITILE_VERTICAL = 0x0000;
        internal const int MDITILE_ZORDER = 0x0004;
        internal const int MF_BITMAP = 0x00000004;
        internal const int MF_CHECKED = 0x00000008;
        internal const int MF_DISABLED = 0x00000002;
        internal const int MF_ENABLED = 0x00000000;
        internal const int MF_GRAYED = 0x00000001;
        internal const int MF_MENUBARBREAK = 0x00000020;
        internal const int MF_MENUBREAK = 0x00000040;
        internal const int MF_OWNERDRAW = 0x00000100;
        internal const int MF_POPUP = 0x00000010;
        internal const int MF_SEPARATOR = 0x00000800;
        internal const int MF_STRING = 0x00000000;
        internal const int MF_UNCHECKED = 0x00000000;
        internal const int SW_SHOW = 5;
        internal const int TBSTATE_HIDDEN = 0x0008;
        internal const int TB_BUTTONCOUNT = 0x0418;
        internal const int TB_GETBUTTON = 0x0417;
        internal const int TPM_BOTTOMALIGN = 0x0020;
        internal const int TPM_CENTERALIGN = 0x0004;
        internal const int TPM_HORNEGANIMATION = 0x0800;
        internal const int TPM_HORPOSANIMATION = 0x0400;
        internal const int TPM_LEFTALIGN = 0x0000;
        internal const int TPM_LEFTBUTTON = 0x0000;
        internal const int TPM_NOANIMATION = 0x4000;
        internal const int TPM_NONOTIFY = 0x0080;
        internal const int TPM_RETURNCMD = 0x0100;
        internal const int TPM_RIGHTALIGN = 0x0008;
        internal const int TPM_RIGHTBUTTON = 0x0002;
        internal const int TPM_TOPALIGN = 0x0000;
        internal const int TPM_VCENTERALIGN = 0x0010;
        internal const int TPM_VERNEGANIMATION = 0x2000;
        internal const int TPM_VERPOSANIMATION = 0x2000;
        internal const int VK_F4 = 0x73;
        internal const int VK_MENU = 0x12;
        internal const int WM_CLOSE = 0x0010;
        internal const int WM_COPYDATA = 0x004a;
        internal const int WM_ENDSESSION = 0x0016;
        internal const int WM_GETICON = 0x007F;
        internal const int WM_KEYDOWN = 0x0100;
        internal const int WM_KEYUP = 0x0101;
        internal const int WM_LBUTTONDOWN = 0x0201;
        internal const int WM_LBUTTONUP = 0x0202;
        internal const int WM_PAINT = 0x000F;
        internal const int WM_QUERYENDSESSION = 0x0011;
        internal const int WM_RBUTTONDOWN = 0x0204;
        internal const int WM_RBUTTONUP = 0x0205;
        internal const int WM_SYSCOMMAND = 0x0112;
        internal const int WM_WINDOWPOSCHANGED = 0x0047;

        internal delegate bool EnumThreadDelegate(Integer hWnd, Integer32 lParam);
        internal delegate bool EnumWindowsCallback(Integer hWnd, Integer32 lParam);

        internal delegate IntPtr WindowsHookProcedure(ShellEvents nCode, Integer32 wParam, Integer32 lParam);

        internal enum ShellHookId : int
        {
            WH_SHELL = 0xa,
            WH_CBT = 0x5
        }

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

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool AppendMenu(Integer hMenu, Integer32 uFlags, Integer32 uIDNewItem, Integer lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool BringWindowToTop(Integer hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(Integer hhk, Integer32 nCode, Integer32 wParam, Integer32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern short CascadeWindows(Integer hwndParent, Integer32 wHow, Integer lpRect, Integer32 cKids, Integer lpKids);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer CreatePopupMenu();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DeleteMenu(Integer hMenu, Integer32 uPosition, Integer32 uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DestroyMenu(Integer hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DestroyWindow(Integer hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int DrawFrameControl(Integer hdc, ref RECT lpRect, Integer32 un1, Integer32 un2);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EnumThreadWindows(Integer32 dwThreadId, EnumThreadDelegate lpfn, Integer32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int EnumWindows(EnumWindowsCallback callPtr, Integer32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer FindWindowEx(Integer hWndParent, Integer hWndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer FindWindow(string a, string b);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer GetAncestor(Integer hWnd, Integer32 gaFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetClassLong")]
        internal static extern Integer32 GetClassLongPtr32(Integer32 hWnd, Integer32 nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetClassLongPtr")]
        internal static extern Integer GetClassLongPtr64(Integer hWnd, Integer32 nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer32 GetClassName(Integer hWnd, StringBuilder lpClassName, Integer32 nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetIconInfo(Integer hIcon, out ICONINFO piconinfo);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer GetLastActivePopup(Integer hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer GetSystemMenu(Integer hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer GetWindow(Integer hWnd, Integer32 gwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetWindowInfo(Integer hWnd, out WINDOWINFO pwi);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetWindowLongW")]
        internal static extern Integer32 GetWindowLongPtr32(Integer32 hWnd, Integer32 nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetWindowLongPtrW")]
        internal static extern Integer64 GetWindowLongPtr64(Integer64 hWnd, Integer32 nIndex);

        internal static Integer GetWindowLongPtr(Integer hWnd, Integer32 nIndex)
            => IntPtr.Size == 8 ?
                GetWindowLongPtr64((ulong)hWnd, nIndex) :
                GetWindowLongPtr32((uint)hWnd, nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetWindowRect(Integer hwnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetWindowTextW")]
        internal static extern Integer32 GetWindowText(Integer hwnd, StringBuilder lpString, Integer32 nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetWindowTextLengthW")]
        internal static extern Integer32 GetWindowTextLength(Integer hwnd);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer32 GetWindowThreadProcessId(Integer hWnd, out Integer32 lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(Integer hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool IsWindowVisible(Integer hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void keybd_event(byte bVk, byte bScan, Integer32 dwFlags, Integer32 dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool PostMessage(Integer hWnd, Integer32 Msg, Integer32 wParam, Integer32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool RegisterShellHookWindow(Integer hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer SendMessage(Integer hWnd, Integer32 Msg, Integer32 wParam, Integer32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SendNotifyMessage(Integer hWnd, Integer32 Msg, Integer32 wParam, Integer32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetForegroundWindow(Integer hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetTaskmanWindow(Integer hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetWindowPos(Integer hWnd, Integer hWndInsertAfter, Integer32 X, Integer32 Y, Integer32 cx, Integer32 cy, Integer32 uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(Integer32 idHook, Integer lpfn, Integer hmod, Integer32 dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "SetWindowLongW")]
        internal static extern Integer32 SetWindowLongPtr32(Integer32 hWnd, Integer32 nIndex, Integer32 dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "SetWindowLongPtrW")]
        internal static extern Integer64 SetWindowLongPtr64(Integer64 hWnd, Integer32 nIndex, Integer32 dwNewLong);

        internal static Integer SetWindowLongPtr(Integer hWnd, Integer32 nIndex, Integer32 dwNewLong)
            => IntPtr.Size == 8 ?
                SetWindowLongPtr64((ulong)hWnd, nIndex, dwNewLong) :
                SetWindowLongPtr32((uint)hWnd, nIndex, dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetWindowText(Integer hWnd, string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool ShowScrollBar(Integer hWnd, Integer32 wBar, bool bShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool ShowWindow(Integer hWnd, Integer32 nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern ushort TileWindows(Integer hwndParent, Integer32 wHow, Integer lpRect, Integer32 cKids, Integer lpKids);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Integer32 TrackPopupMenuEx(Integer hmenu, Integer32 fuFlags, Integer32 x, Integer32 y, Integer hwnd, Integer lptpm);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool UnhookWindowsHookEx(Integer hhk);

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
            internal Integer32 xHotspot;

            /// <summary>
            /// The y-coordinate of a cursor's hot spot
            /// </summary>
            internal Integer32 yHotspot;

            /// <summary>
            /// The icon bitmask bitmap
            /// </summary>
            internal Integer hbmMask;

            /// <summary>
            /// A handle to the icon color bitmap.
            /// </summary>
            internal Integer hbmColor;
        }
    }
}