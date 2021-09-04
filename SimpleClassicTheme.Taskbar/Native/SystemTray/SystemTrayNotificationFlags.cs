
using System;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    [Flags]
    internal enum SystemTrayNotificationFlags : int
    {
        None = 0,
        CallbackMessageValid = 1,
        IconHandleValid = 2,
        ToolTipValid = 4,
        StateValid = 8,
        InfoBalloonValid = 16,
        GuidValid = 32,
        Realtime = 64,
        ShowToolTip = 128
    }
}
