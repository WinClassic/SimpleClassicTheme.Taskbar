using System;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinDef;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
	/// <summary>
	/// Represents Win32's <c>commctrl.h</c>.
	/// </summary>
	internal static class CommCtrl
	{
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct TBBUTTON
		{
			public int iBitmap;
			public int idCommand;
			public byte fsState;
			public byte fsStyle;

#if ARCH_X64
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public byte[] bReserved;
#else
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public byte[] bReserved;
#endif
			public IntPtr dwData;
			public IntPtr iString;
		}

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
	}
}
