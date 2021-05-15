using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
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

        public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }
    }

    public struct Window
    {
        public IntPtr Handle;
        public WINDOWINFO WindowInfo;
        private string className;
        private string title;

        public Window(IntPtr handle) : this()
        {
            Handle = handle;
            WindowInfo = WINDOWINFO.FromHWND(handle);
        }

        public string ClassName
        {
            get
            {
                if (className == null)
                {
                    var sb = new StringBuilder(100);
                    _ = User32.GetClassName(Handle, sb, sb.Capacity - 1);
                    className = sb.ToString();
                }

                return className;
            }
        }

        public string Title
        {
            get
            {
                if (title == null)
                {
                    var sb = new StringBuilder(100);
                    _ = User32.GetWindowTextW(Handle, sb, 100);
                    title = sb.ToString();
                }

                return title;
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

    public static class WIN32
    {
        public const uint VK_F4 = 0x73;
        public const uint VK_MENU = 0x12;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;
    }
}