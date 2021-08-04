using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    /// <summary>
    /// This header is used by Windows Controls.
    /// </summary>
    internal static class CommCtrl
	{
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		internal struct TBBUTTON
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
	}
}
