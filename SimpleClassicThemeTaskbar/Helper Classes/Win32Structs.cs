using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleClassicThemeTaskbar
{
    public static class WIN32
    {
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_RBUTTONUP = 0x0205;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint VK_MENU = 0x12;
        public const uint VK_F4 = 0x73;
    }
    enum DWMWINDOWATTRIBUTE : uint
    {
        NCRenderingEnabled = 1,
        NCRenderingPolicy = 2,
        TransitionsForceDisabled = 3,
        AllowNCPaint = 4,
        CaptionButtonBounds = 5,
        NonClientRtlLayout = 6,
        ForceIconicRepresentation = 7,
        Flip3DPolicy = 8,
        ExtendedFrameBounds = 9, 
        HasIconicBitmap = 10,
        DisallowPeek = 11,
        ExcludedFromPeek = 12,
        Cloak = 13,
        Cloaked = 14,
        FreezeRepresentation = 15
    }

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

        public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }
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
