using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static SimpleClassicTheme.Taskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    [ComImport()]
    [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImageList
    {
        [PreserveSig]
        int Add(
            IntPtr hbmImage,
            IntPtr hbmMask,
            ref int pi);

        [PreserveSig]
        int ReplaceIcon(
            int i,
            IntPtr hicon,
            ref int pi);

        [PreserveSig]
        int SetOverlayImage(
            int iImage,
            int iOverlay);

        [PreserveSig]
        int Replace(
            int i,
            IntPtr hbmImage,
            IntPtr hbmMask);

        [PreserveSig]
        int AddMasked(
            IntPtr hbmImage,
            int crMask,
            ref int pi);

        [PreserveSig]
        int Draw(
            ref IMAGELISTDRAWPARAMS pimldp);

        [PreserveSig]
        int Remove(
            int i);

        [PreserveSig]
        int GetIcon(
            int i,
            int flags,
            ref IntPtr picon);
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }

    public struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public int crEffect;
        public int cx;
        public int cy;
        public int dwRop;
        public int Frame;
        public int fState;
        public int fStyle;
        public IntPtr hdcDst;
        public IntPtr himl;
        public int i;
        public int rgbBk;
        public int rgbFg;
        public int x;
        public int xBitmap;
        public int y;
        public int yBitmap;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    public class Win32Icon
    {
        public enum IconSizeEnum
        {
            SmallIcon16 = Shell32.SHGFI_SMALLICON,
            MediumIcon32 = Shell32.SHGFI_LARGEICON,
            LargeIcon48 = Shell32.SHIL_EXTRALARGE,
            ExtraLargeIcon = Shell32.SHIL_JUMBO
        }

        public static IntPtr GetIconFromPath(string path, IconSizeEnum size)
		{
            const uint SHGFI_SYSICONINDEX = 0x4000;
            const int ILD_TRANSPARENT = 1;

            SHFILEINFO fileInfo = new();
            _ = Shell32.SHGetFileInfo(path, 0, ref fileInfo, Marshal.SizeOf(fileInfo), SHGFI_SYSICONINDEX);
            var imageListGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            Shell32.SHGetImageList((int)size, ref imageListGuid, out IImageList imageList);
            var iconIndex = fileInfo.iIcon;
            IntPtr iconHandle = IntPtr.Zero;
            imageList.GetIcon(iconIndex, ILD_TRANSPARENT, ref iconHandle);
            return iconHandle;
        }
    }
}