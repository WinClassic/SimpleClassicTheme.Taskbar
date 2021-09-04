using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.Headers
{
	/// <summary>
	/// Represents Win32's <c>servprov.h</c>.
	/// </summary>
	public static class ServProv
    {
		[ComImport]
		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IServiceProvider
		{
			[PreserveSig]
			[return: MarshalAs(UnmanagedType.I4)]
			int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		}
	}
}
