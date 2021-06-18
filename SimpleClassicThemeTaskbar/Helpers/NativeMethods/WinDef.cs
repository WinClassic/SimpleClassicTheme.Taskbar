using System;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    /// <summary>
    /// This header is used by Windows GDI.
    /// </summary>
    internal static class WinDef
	{
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		internal struct POINT
		{
			public int X;
			public int Y;
		}
	}
}
