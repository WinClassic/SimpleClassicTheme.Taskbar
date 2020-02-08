using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct WINDOWINFO
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);

        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public static WINDOWINFO FromHWND(IntPtr handle)
        {
            WINDOWINFO d;
            GetWindowInfo(handle, out d);
            return d;
        }
    }

    struct Window
    {
        [DllImport("user32.dll")]
        private static extern bool GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public WINDOWINFO WindowInfo;
        public string ClassName;
        public string Title;
        public IntPtr Handle;

        public Window(IntPtr handle)
            : this()
        {
            Handle = handle;
            WindowInfo = WINDOWINFO.FromHWND(handle);
            StringBuilder cn = new StringBuilder(100);
            GetClassName(handle, cn, cn.Capacity - 1);
            ClassName = cn.ToString();
            StringBuilder title = new StringBuilder(100);
            GetWindowTextW(handle, title, 100);
            Title = title.ToString();
        }
    }
}
