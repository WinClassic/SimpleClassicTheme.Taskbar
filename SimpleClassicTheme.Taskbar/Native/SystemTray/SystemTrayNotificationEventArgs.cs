using SimpleClassicTheme.Taskbar.Native.SystemTray;

using System;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray;

internal class SystemTrayNotificationEventArgs : EventArgs
{
    public SystemTrayNotificationType Type { get; private set; }
    public SystemTrayNotificationData Data { get; private set; }
    public IntPtr ReturnValue { get; private set; }

    public SystemTrayNotificationEventArgs(SystemTrayNotificationType type, SystemTrayNotificationData data)
    {
        Type = type;
        Data = data;
    }
}
