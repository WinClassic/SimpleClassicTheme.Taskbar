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