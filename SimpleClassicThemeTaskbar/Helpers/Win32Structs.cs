using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using static SimpleClassicThemeTaskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicThemeTaskbar
{

    public struct Window
    {
        public IntPtr Handle;
        public WINDOWINFO WindowInfo;
        public string GroupingKey;
        public Process Process;

        public Window(IntPtr handle) : this()
        {
            Handle = handle;
            WindowInfo = WINDOWINFO.FromHWND(handle);
            _ = User32.GetWindowThreadProcessId(Handle, out uint pid);
            Process = Process.GetProcessById((int)pid);
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

        public string Title
        {
            get
            {
                var sb = new StringBuilder(100);
                if (User32.GetWindowText(Handle, sb, sb.Capacity) != 0)
                    return sb.ToString();
                else
                    return "";
            }
            set
            {
                User32.SetWindowText(Handle, value);
            }
        }
    }

    // http://en.verysource.com/code/281806_1/tray.c.html
    [StructLayout(LayoutKind.Sequential)]
    public struct _TVSD
    {
        public uint dwSize;
        public int lSignature;        // signature (must be negative)
        public TvsdFlags dwFlags;          // TVSD_ flags
        public uint uStuckPlace;      // current stuck edge
        public SIZE sStuckWidths;      // widths of stuck rects (BUGBUG: in tbd units)
        public RECT rcLastStuck;       // last stuck position in pixels
    }

    [Flags]
    public enum TvsdFlags : uint
    {
        None = 0,
        AutoHide = 1,
        TopMost = 2,
        SmallIcons = 4,
        HideClock = 8,
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