using SimpleClassicTheme.Taskbar.Native.SystemTray;

using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{

    internal partial class SystemTrayNotificationService
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct TRAYNOTIFYDATA
		{
			public uint dwSignature;
			public uint dwMessage;
			public SystemTrayNotificationData nid;
		}
	}
}
