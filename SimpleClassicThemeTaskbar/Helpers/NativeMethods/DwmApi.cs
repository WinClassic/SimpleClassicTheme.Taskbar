using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    internal static class DwmApi
    {
        internal enum DWMWINDOWATTRIBUTE : uint
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

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out int pvAttribute, int cbAttribute);
    }
}