using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static SimpleClassicTheme.Taskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicTheme.Taskbar.Helpers.NativeMethods
{
    internal static partial class User32
    {
        internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        internal delegate bool EnumWindowsCallback(IntPtr hWnd, int lParam);

        internal delegate IntPtr WindowsHookProcedure(ShellEvents nCode, Integer32 wParam, Integer32 lParam);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern bool AppendMenu(IntPtr hMenu, uint uFlags, int uIDNewItem, string lpNewItem);

        [DllImport(nameof(User32), CharSet = CharSet.Auto)]
        internal static extern bool AppendMenu(IntPtr hMenu, uint uFlags, int uIDNewItem, IntPtr lpNewItem);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

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

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport(nameof(User32))]
        internal static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

        [DllImport(nameof(User32))]
        internal static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string a, string b);

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
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport(nameof(User32), EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowText(IntPtr hwnd, System.Text.StringBuilder lpString, int cch);

        [DllImport(nameof(User32), EntryPoint = "GetWindowTextLengthW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport(nameof(User32), SetLastError = false)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(Integer hWnd);

        [DllImport(nameof(User32))]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(nameof(User32))]
        internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport(nameof(User32), CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadBitmapA(IntPtr hModule, string lpBitmapName);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool RegisterShellHookWindow(IntPtr hwnd);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport(nameof(User32), CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport(nameof(User32))]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(nameof(User32), SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport(nameof(User32), CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(ShellHookId idHook, WindowsHookProcedure lpfn, IntPtr hmod, uint dwThreadId);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport(nameof(User32))]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport(nameof(User32))]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

        [DllImport(nameof(User32))]
        internal static extern ushort TileWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport(nameof(User32))]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [DllImport(nameof(User32), SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadImageW(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);
    }
}