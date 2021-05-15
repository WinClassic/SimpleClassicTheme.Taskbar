namespace SimpleClassicThemeTaskbar.Helpers
{
    public static class Constants
    {
        public static readonly string[] HiddenClassNames = new string[]
        {
            "Shell_TrayWnd",                // Windows taskbar
            "WorkerW",                      // Random Windows thing
            "Progman",                      // The program manager
            "ThumbnailDeviceHelperWnd",     // UWP
            "Windows.UI.Core.CoreWindow",   // Empty UWP apps
            "DV2ControlHost",               // Windows start menu, if open
            "MsgrIMEWindowClass",           // Live messenger's notifybox
            "SysShadow",                    // Live messenger's shadow-hack
        };
    }
}