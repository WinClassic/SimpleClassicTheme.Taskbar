using System;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    [Flags]
    internal enum SystemTrayIconState : int
    {
        None = 0,
        Hidden = 1,
        IconHandleShared = 2
    }
}