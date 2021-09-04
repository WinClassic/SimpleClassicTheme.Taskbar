using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
    /// <summary>
    /// Represents Win32's <c>windef.h</c>.
    /// </summary>
    public static class WinDef
    {
        internal const int ERROR_INSUFFICIENT_BUFFER = 0x0000007a;
        internal const int ERROR_SUCCESS = 0x00000000;
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator Point(POINT p) => new(p.X, p.Y);

            public static implicit operator POINT(Point p) => new(p.X, p.Y);
        }

        [Serializable]
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

            public static implicit operator Rectangle(RECT r) => Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);

            public static implicit operator RECT(Rectangle r) => new(r.Left, r.Top, r.Right, r.Bottom);
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx, cy;

            public SIZE(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }

            public static implicit operator Size(SIZE s) => new(s.cx, s.cy);

            public static implicit operator SIZE(Size s) => new(s.Width, s.Height);
        }
    }
}