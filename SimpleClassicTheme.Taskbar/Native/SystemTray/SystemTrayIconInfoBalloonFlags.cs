using System;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray;

[Flags]
internal enum SystemTrayIconInfoBalloonFlags : int
{
    NoIcon = 0,
    InfoIcon = 1,
    WarningIcon = 2,
    ErrorIcon = 3,
    UserIcon = 4,
    NoSound = 0x10,
    LargeIcon = 0x20,
    RespectQuietTime = 0x80
}
