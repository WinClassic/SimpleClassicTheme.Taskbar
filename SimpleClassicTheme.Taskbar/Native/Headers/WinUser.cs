using System;
using System.Runtime.InteropServices;
using System.Text;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinDef;
using static SimpleClassicTheme.Taskbar.Native.SystemTray.SystemTrayNotificationService;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>winuser.h</c>.
    /// </summary>
    internal static partial class WinUser
    {
        private const string _dllName = "user32.dll";

        internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        internal delegate bool EnumWindowsCallback(IntPtr hWnd, int lParam);

        internal delegate IntPtr WindowsHookProcedure(ShellEvents nCode, Integer32 wParam, Integer32 lParam);

        [DllImport(_dllName, CharSet = CharSet.Unicode)]
        internal static extern bool AppendMenu(IntPtr hMenu, uint uFlags, int uIDNewItem, string lpNewItem);

        [DllImport(_dllName, CharSet = CharSet.Auto)]
        internal static extern bool AppendMenu(IntPtr hMenu, uint uFlags, int uIDNewItem, IntPtr lpNewItem);

        [DllImport(_dllName, SetLastError = true)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport(_dllName)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, ShellEvents nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(_dllName)]
        internal static extern ushort CascadeWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport(_dllName)]
        internal static extern IntPtr CreatePopupMenu();

        [DllImport(_dllName)]
        internal static extern bool DeleteMenu(IntPtr hMenu, int uPosition, uint uFlags);

        /// <summary>
        /// Destroys the specified menu and frees any memory that the menu occupies.
        /// </summary>
        /// <param name="hMenu">A handle to the menu to be destroyed.</param>
        [DllImport(_dllName, SetLastError = true)]
        internal static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport(_dllName, SetLastError = true)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport(_dllName)]
        internal static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

        [DllImport(_dllName)]
        internal static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport(_dllName, SetLastError = true)]
        internal static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

        [DllImport(_dllName, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        [DllImport(_dllName, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string a, string b);

        [DllImport(_dllName)]
        internal static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport(_dllName, EntryPoint = "GetClassLong")]
        internal static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport(_dllName, EntryPoint = "GetClassLongPtr")]
        internal static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport(_dllName, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport(_dllName)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport(_dllName)]
        internal static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport(_dllName)]
        internal static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport(_dllName)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport(_dllName)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint gwFlags);

        [DllImport(_dllName)]
        internal static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);

        [DllImport(_dllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport(_dllName, SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport(_dllName, EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowText(IntPtr hwnd, System.Text.StringBuilder lpString, int cch);

        [DllImport(_dllName, EntryPoint = "GetWindowTextLengthW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport(_dllName, SetLastError = false)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(_dllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(Integer hWnd);

        [DllImport(_dllName)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(_dllName)]
        internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport(_dllName, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadBitmapA(IntPtr hModule, string lpBitmapName);

        [DllImport(_dllName, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(_dllName, SetLastError = true)]
        internal static extern bool RegisterShellHookWindow(IntPtr hwnd);

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport(_dllName, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(_dllName, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(_dllName)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(_dllName, SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport(_dllName, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(ShellHookId idHook, WindowsHookProcedure lpfn, IntPtr hmod, uint dwThreadId);

        [DllImport(_dllName, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport(_dllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport(_dllName)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(_dllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

        [DllImport(_dllName)]
        internal static extern ushort TileWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport(_dllName)]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport(_dllName, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadImageW(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);
        
        [DllImport(_dllName, CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle, UInt16 lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport(_dllName, CharSet = CharSet.Unicode)]
        internal static extern ushort RegisterClass([In] ref WNDCLASS lpWndClass);

        [DllImport(_dllName)]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(_dllName)]
        internal static extern bool SendNotifyMessage(int hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    }
}