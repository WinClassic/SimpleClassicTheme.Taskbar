﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using static SimpleClassicThemeTaskbar.Taskbar;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class User32
    {
        internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool AppendMenu(IntPtr hMenu, SystemContextMenuItemFlags uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool AppendMenu(IntPtr hMenu, SystemContextMenuItemFlags uFlags, int uIDNewItem, IntPtr lpNewItem);

        [DllImport("user32.dll")]
        internal static extern ushort CascadeWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport("user32.dll")]
        internal static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll")]
        internal static extern bool DeleteMenu(IntPtr hMenu, int uPosition, uint uFlags);

        [DllImport("user32.dll")]
        internal static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

        [DllImport("user32.dll")]
        internal static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowExW(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowW(string a, string b);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        internal static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        internal static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint gwFlags);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern ushort TileWindows(IntPtr hwndParent, uint wHow, IntPtr lpRect, uint cKids, IntPtr lpKids);

        [DllImport("user32.dll")]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, SystemContextMenuTrackPopupMenuFlags fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

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