using System;
using System.IO;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public static class Constants
    {
        public const int SCTLP_FORCE = 0x0001;

        public const int SCTWP_EXIT = 0x0001;

        public const int SCTWP_ISMANAGED = 0x0002;

        public const int SCTWP_ISSCT = 0x0003;

        /// <summary>
        /// The only true window message™
        /// </summary>
        public const int WM_SCT = 0x0420;

        public static string VisualStyleDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "1337ftw", "SimpleClassicThemeTaskbar", "VisualStyles");

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