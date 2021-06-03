using System;
using System.Runtime.InteropServices;

using static SimpleClassicThemeTaskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static partial class User32
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
    }
}
