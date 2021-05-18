using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }
    }

    public struct Window
    {
        public IntPtr Handle;
        public WINDOWINFO WindowInfo;

        public Window(IntPtr handle) : this()
        {
            Handle = handle;
            WindowInfo = WINDOWINFO.FromHWND(handle);
        }

        public string ClassName
        {
            get
            {
                var sb = new StringBuilder(100);
                _ = User32.GetClassName(Handle, sb, sb.Capacity - 1);
                return sb.ToString();
            }
        }

        public Process Process
        {
            get
            {
                _ = User32.GetWindowThreadProcessId(Handle, out uint pid);
                return Process.GetProcessById((int)pid);
            }
        }

        public string Title
        {
            get
            {
                var sb = new StringBuilder(100);
                _ = User32.GetWindowTextW(Handle, sb, 100);
                return sb.ToString();
            }
            set
            {
                User32.SetWindowTextW(Handle, value);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO
    {
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
            _ = User32.GetWindowInfo(handle, out WINDOWINFO d);
            return d;
        }
    }
}