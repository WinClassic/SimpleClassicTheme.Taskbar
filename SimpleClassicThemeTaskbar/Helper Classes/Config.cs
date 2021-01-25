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
        public static bool EnableSystemTrayColorChange = true;
        public static bool ShowTaskbarOnAllDesktops = true;
        public static bool EnableQuickLaunch = true;
        public static bool StartButtonCustomIcon = false;
        public static bool StartButtonCustomButton = false;
        
        public static int TaskbarProgramWidth = 160;
        public static int SpaceBetweenTrayIcons = 2;
        public static int SpaceBetweenTaskbarIcons = 2;
        public static int SpaceBetweenQuickLaunchIcons = 2;

        public static ProgramGroupCheck ProgramGroupCheck = ProgramGroupCheck.FileNameAndPath;

        public static string StartButtonImage = "";
        public static string QuickLaunchOrder = "";
        public static string TaskbarProgramFilter = "";

        public static bool configChanged = true;

        public static void SaveToRegistry()
        {
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("EnableSystemTrayHover", Config.EnableSystemTrayHover.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("EnableSystemTrayColorChange", Config.EnableSystemTrayColorChange.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("ShowTaskbarOnAllDesktops", Config.ShowTaskbarOnAllDesktops.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("EnableQuickLaunch", Config.EnableQuickLaunch.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("StartButtonCustomIcon", Config.StartButtonCustomIcon.ToString(), RegistryValueKind.String);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("StartButtonCustomButton", Config.StartButtonCustomButton.ToString(), RegistryValueKind.String);

            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("TaskbarProgramWidth", Config.TaskbarProgramWidth, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("SpaceBetweenTrayIcons", Config.SpaceBetweenTrayIcons, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("SpaceBetweenTaskbarIcons", Config.SpaceBetweenTaskbarIcons, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("SpaceBetweenQuickLaunchIcons", Config.SpaceBetweenQuickLaunchIcons, RegistryValueKind.DWord);

            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("ProgramGroupCheck", Config.ProgramGroupCheck, RegistryValueKind.DWord);

            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("StartButtonImage", StartButtonImage);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("QuickLaunchOrder", QuickLaunchOrder);
            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").SetValue("TaskbarProgramFilter", TaskbarProgramFilter);
        }

        public static void LoadFromRegistry()
        {
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("EnableSystemTrayHover", "True"), out EnableSystemTrayHover);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("EnableSystemTrayColorChange", "True"), out EnableSystemTrayColorChange);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("ShowTaskbarOnAllDesktops", "True"), out ShowTaskbarOnAllDesktops);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("EnableQuickLaunch", "True"), out EnableQuickLaunch);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("StartButtonCustomIcon", "False"), out StartButtonCustomIcon);
            Boolean.TryParse((string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("StartButtonCustomButton", "False"), out StartButtonCustomButton);

            TaskbarProgramWidth = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("TaskbarProgramWidth", 160);
            SpaceBetweenTrayIcons = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("SpaceBetweenTrayIcons", 2);
            SpaceBetweenTaskbarIcons = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("SpaceBetweenTaskbarIcons", 2);
            SpaceBetweenQuickLaunchIcons = (int)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("SpaceBetweenQuickLaunchIcons", 2);

            ProgramGroupCheck = (ProgramGroupCheck)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("ProgramGroupCheck", ProgramGroupCheck.FileNameAndPath);

            StartButtonImage = (string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("StartButtonImage", "");
            QuickLaunchOrder = (string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("QuickLaunchOrder", "");
            TaskbarProgramFilter = (string)Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("1337ftw").CreateSubKey("SimpleClassicThemeTaskbar").GetValue("TaskbarProgramFilter", "");
        }
    }
}
