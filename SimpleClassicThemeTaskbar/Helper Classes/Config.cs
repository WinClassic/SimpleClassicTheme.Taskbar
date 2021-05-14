using Microsoft.Win32;

using SimpleClassicThemeTaskbar.Theme_Engine;

namespace SimpleClassicThemeTaskbar
{
    //Config class
    //TODO: Move more configurable settings to here
    //TODO: Make config menu
    public static class Config
    {
        public static bool ConfigChanged = true;

        public static bool EnableQuickLaunch = true;

        public static bool EnableSystemTrayColorChange = true;

        public static bool EnableSystemTrayHover = true;

        public static ProgramGroupCheck ProgramGroupCheck = ProgramGroupCheck.FileNameAndPath;

        public static string QuickLaunchOrder = "";

        public static BaseRenderer Renderer = new ClassicRenderer(); // new ImageRenderer("D:\\Classic Theme\\Resources");

        public static bool ShowTaskbarOnAllDesktops = true;
        public static int SpaceBetweenQuickLaunchIcons = 2;
        public static int SpaceBetweenTaskbarIcons = 2;
        public static int SpaceBetweenTrayIcons = 2;
        public static bool StartButtonCustomButton = false;
        public static bool StartButtonCustomIcon = false;
        public static string StartButtonImage = "";
        public static string TaskbarProgramFilter = "";
        public static int TaskbarProgramWidth = 160;

        public static void LoadFromRegistry()
        {
            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                // Boolean values
                _ = bool.TryParse((string)scttSubKey.GetValue("EnableSystemTrayHover", "True"), out EnableSystemTrayHover);
                _ = bool.TryParse((string)scttSubKey.GetValue("EnableSystemTrayColorChange", "True"), out EnableSystemTrayColorChange);
                _ = bool.TryParse((string)scttSubKey.GetValue("ShowTaskbarOnAllDesktops", "True"), out ShowTaskbarOnAllDesktops);
                _ = bool.TryParse((string)scttSubKey.GetValue("EnableQuickLaunch", "True"), out EnableQuickLaunch);
                _ = bool.TryParse((string)scttSubKey.GetValue("StartButtonCustomIcon", "False"), out StartButtonCustomIcon);
                _ = bool.TryParse((string)scttSubKey.GetValue("StartButtonCustomButton", "False"), out StartButtonCustomButton);

                // Integer values
                TaskbarProgramWidth = (int)scttSubKey.GetValue("TaskbarProgramWidth", 160);
                SpaceBetweenTrayIcons = (int)scttSubKey.GetValue("SpaceBetweenTrayIcons", 2);
                SpaceBetweenTaskbarIcons = (int)scttSubKey.GetValue("SpaceBetweenTaskbarIcons", 2);
                SpaceBetweenQuickLaunchIcons = (int)scttSubKey.GetValue("SpaceBetweenQuickLaunchIcons", 2);

                // String values
                StartButtonImage = (string)scttSubKey.GetValue("StartButtonImage", "");
                QuickLaunchOrder = (string)scttSubKey.GetValue("QuickLaunchOrder", "");
                TaskbarProgramFilter = (string)scttSubKey.GetValue("TaskbarProgramFilter", "");

                // Enum values
                ProgramGroupCheck = (ProgramGroupCheck)scttSubKey.GetValue("ProgramGroupCheck", ProgramGroupCheck.FileNameAndPath);
            }
        }

        public static void SaveToRegistry()
        {
            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                scttSubKey.SetValue("EnableSystemTrayHover", Config.EnableSystemTrayHover.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("EnableSystemTrayColorChange", Config.EnableSystemTrayColorChange.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("ShowTaskbarOnAllDesktops", Config.ShowTaskbarOnAllDesktops.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("EnableQuickLaunch", Config.EnableQuickLaunch.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("StartButtonCustomIcon", Config.StartButtonCustomIcon.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("StartButtonCustomButton", Config.StartButtonCustomButton.ToString(), RegistryValueKind.String);

                scttSubKey.SetValue("TaskbarProgramWidth", Config.TaskbarProgramWidth, RegistryValueKind.DWord);
                scttSubKey.SetValue("SpaceBetweenTrayIcons", Config.SpaceBetweenTrayIcons, RegistryValueKind.DWord);
                scttSubKey.SetValue("SpaceBetweenTaskbarIcons", Config.SpaceBetweenTaskbarIcons, RegistryValueKind.DWord);
                scttSubKey.SetValue("SpaceBetweenQuickLaunchIcons", Config.SpaceBetweenQuickLaunchIcons, RegistryValueKind.DWord);

                scttSubKey.SetValue("ProgramGroupCheck", Config.ProgramGroupCheck, RegistryValueKind.DWord);

                scttSubKey.SetValue("StartButtonImage", StartButtonImage);
                scttSubKey.SetValue("QuickLaunchOrder", QuickLaunchOrder);
                scttSubKey.SetValue("TaskbarProgramFilter", TaskbarProgramFilter);
            }
        }
    }
}