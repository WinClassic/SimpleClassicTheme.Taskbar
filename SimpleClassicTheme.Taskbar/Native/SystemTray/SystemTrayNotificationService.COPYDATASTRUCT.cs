using System;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{

    internal partial class SystemTrayNotificationService
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct COPYDATASTRUCT
		{
			public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
			public int cbData;       // The count of bytes in the message.
			public IntPtr lpData;    // The address of the message.
		}
	}
}
