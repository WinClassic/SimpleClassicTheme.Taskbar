using System;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.CommCtrl;
using static SimpleClassicTheme.Taskbar.Native.Headers.ShellApi;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public class Win32Icon
    {
        public enum IconSizeEnum
        {
            SmallIcon16 = SHGFI_SMALLICON,
            MediumIcon32 = SHGFI_LARGEICON,
            LargeIcon48 = SHIL_EXTRALARGE,
            ExtraLargeIcon = SHIL_JUMBO
        }

        public static IntPtr GetIconFromPath(string path, IconSizeEnum size)
		{
            const uint SHGFI_SYSICONINDEX = 0x4000;
            const int ILD_TRANSPARENT = 1;

            SHFILEINFO fileInfo = new();
            _ = SHGetFileInfo(path, 0, ref fileInfo, Marshal.SizeOf(fileInfo), SHGFI_SYSICONINDEX);
            var imageListGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            SHGetImageList((int)size, ref imageListGuid, out IImageList imageList);
            var iconIndex = fileInfo.iIcon;
            IntPtr iconHandle = IntPtr.Zero;
            imageList.GetIcon(iconIndex, ILD_TRANSPARENT, ref iconHandle);
            return iconHandle;
        }
    }
}