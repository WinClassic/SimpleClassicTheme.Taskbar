using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SimpleClassicThemeTaskbar
{
    //Config class
    //TODO: Move more configurable settings to here
    //TODO: Make config menu
    public static class Config
    {
        public static bool EnableSystemTrayHover = true;
        public static bool ShowTaskbarOnAllDesktops = true;
        public static bool EnableQuickLaunch = true;
        public static int TaskbarProgramWidth = 160;
        public static int SpaceBetweenTrayIcons = 2;
        public static int SpaceBetweenTaskbarIcons = 2;
        public static int SpaceBetweenQuickLaunchIcons = 2;

        public static bool configChanged = true;

        public static void SaveToRegistry()
        {
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("EnableSystemTrayHover", Config.EnableSystemTrayHover.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("ShowTaskbarOnAllDesktops", Config.ShowTaskbarOnAllDesktops.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("EnableQuickLaunch", Config.EnableQuickLaunch.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("TaskbarProgramWidth", Config.TaskbarProgramWidth, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("SpaceBetweenTrayIcons", Config.SpaceBetweenTrayIcons, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("SpaceBetweenTaskbarIcons", Config.SpaceBetweenTaskbarIcons, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("SpaceBetweenQuickLaunchIcons", Config.SpaceBetweenQuickLaunchIcons, RegistryValueKind.DWord);
        }

        public static void LoadFromRegistry()
        {
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("EnableSystemTrayHover", "True"), out EnableSystemTrayHover);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("ShowTaskbarOnAllDesktops", "True"), out ShowTaskbarOnAllDesktops);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("EnableQuickLaunch", "True"), out EnableQuickLaunch);
            TaskbarProgramWidth = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("TaskbarProgramWidth", 160);
            SpaceBetweenTrayIcons = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("SpaceBetweenTrayIcons", 2);
            SpaceBetweenTaskbarIcons = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("SpaceBetweenTaskbarIcons", 2);
            SpaceBetweenQuickLaunchIcons = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("SpaceBetweenQuickLaunchIcons", 2);
        }
    }
}
