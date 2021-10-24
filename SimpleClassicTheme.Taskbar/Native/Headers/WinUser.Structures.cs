using System;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinDef;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{

    internal partial class WinUser
    {
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;
        }

        /// <summary>
        /// Contains the window class attributes that are registered by the <see cref="RegisterClass"/> function.
        /// 
        /// This structure has been superseded by the WNDCLASSEX structure used with the RegisterClassEx function. You can still use <see cref="WNDCLASS"/> and <see cref="RegisterClass"/> if you do not need to set the small icon associated with the window class.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WNDCLASS
        {
            public int style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszClassName;
        }

        [Serializable]
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
            internal int xHotspot;

            /// <summary>
            /// The y-coordinate of a cursor's hot spot
            /// </summary>
            internal int yHotspot;

            /// <summary>
            /// The icon bitmask bitmap
            /// </summary>
            internal IntPtr hbmMask;

            /// <summary>
            /// A handle to the icon color bitmap.
            /// </summary>
            internal IntPtr hbmColor;
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
                _ = GetWindowInfo(handle, out WINDOWINFO windowInfo);
                return windowInfo;
            }
        }
    }
}