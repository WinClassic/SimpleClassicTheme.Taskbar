
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray;

[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "This shouldn't be a class, this needs to be refactored into a struct.")]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal class SystemTrayNotificationData
{
    public int Size/* = Marshal.SizeOf(typeof(SystemTrayNotificationData))*/;
    private int windowHandle;
    public uint Id;
    public SystemTrayNotificationFlags Flags;
    public uint CallbackMessage;
    private int iconHandle;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
    public string ToolTip;
    public SystemTrayIconState State;
    public SystemTrayIconState StateMask;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
    public string InfoBalloonText;
    private int TimeoutOrVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
    public string InfoBalloonTitle;
    public SystemTrayIconInfoBalloonFlags InfoBalloonFlags;
    public Guid Guid;
    private int infoBalloonIcon;

    public IntPtr WindowHandle => new(windowHandle);
    public IntPtr IconHandle => new(iconHandle);
    public IntPtr InfoBalloonIcon => new(infoBalloonIcon);

    public int Timeout { get => TimeoutOrVersion; }
    public int Version { get => TimeoutOrVersion; }
    public IntPtr Identifier => WindowHandle + (int)Id;

    public void Update(SystemTrayNotificationData newData)
    {
        if (newData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
            CallbackMessage = newData.CallbackMessage;
        if (newData.Flags.HasFlag(SystemTrayNotificationFlags.IconHandleValid))
            iconHandle = newData.iconHandle;
        if (newData.Flags.HasFlag(SystemTrayNotificationFlags.ToolTipValid))
            ToolTip = newData.ToolTip;
        if (newData.Flags.HasFlag(SystemTrayNotificationFlags.StateValid))
        {
            State = newData.State & newData.StateMask;
            StateMask = newData.StateMask;
        }
    }
}
