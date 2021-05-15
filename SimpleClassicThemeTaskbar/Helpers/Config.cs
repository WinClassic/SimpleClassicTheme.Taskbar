using Microsoft.Win32;

using SimpleClassicThemeTaskbar.ThemeEngine;

using System.Globalization;
using System.Resources;

namespace SimpleClassicThemeTaskbar.Helpers
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

        public static string Language;
        public static ProgramGroupCheck ProgramGroupCheck = ProgramGroupCheck.FileNameAndPath;

        public static string QuickLaunchOrder = "";

        public static BaseRenderer Renderer = new ClassicRenderer(); // new ImageRenderer("D:\\Classic Theme\\Resources");

        public static string RendererPath = "Internal/Classic";
        public static bool ShowTaskbarOnAllDesktops = true;
        public static int SpaceBetweenQuickLaunchIcons = 2;
        public static int SpaceBetweenTaskbarIcons = 2;
        public static int SpaceBetweenTrayIcons = 2;
        public static StartButtonAppearance StartButtonAppearance = StartButtonAppearance.Default;
        public static string StartButtonIconImage = "";
        public static string StartButtonImage = "";
        public static string TaskbarProgramFilter = "";
        public static int TaskbarProgramWidth = 160;

        public static void LoadFromRegistry()
        {
            Language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (Language != "en-US" && Language != "nl-NL")
                Language = "en-US";

            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                // Boolean values
                _ = bool.TryParse((string)scttSubKey.GetValue("EnableSystemTrayHover", "True"), out EnableSystemTrayHover);
                _ = bool.TryParse((string)scttSubKey.GetValue("EnableSystemTrayColorChange", "True"), out EnableSystemTrayColorChange);
                _ = bool.TryParse((string)scttSubKey.GetValue("ShowTaskbarOnAllDesktops", "True"), out ShowTaskbarOnAllDesktops);
                _ = bool.TryParse((string)scttSubKey.GetValue("EnableQuickLaunch", "True"), out EnableQuickLaunch);

                // Integer values
                TaskbarProgramWidth = (int)scttSubKey.GetValue("TaskbarProgramWidth", 160);
                SpaceBetweenTrayIcons = (int)scttSubKey.GetValue("SpaceBetweenTrayIcons", 2);
                SpaceBetweenTaskbarIcons = (int)scttSubKey.GetValue("SpaceBetweenTaskbarIcons", 2);
                SpaceBetweenQuickLaunchIcons = (int)scttSubKey.GetValue("SpaceBetweenQuickLaunchIcons", 2);

                // String values
                StartButtonImage = (string)scttSubKey.GetValue("StartButtonImage", "");
                StartButtonIconImage = (string)scttSubKey.GetValue("StartButtonIconImage", "");
                QuickLaunchOrder = (string)scttSubKey.GetValue("QuickLaunchOrder", "");
                TaskbarProgramFilter = (string)scttSubKey.GetValue("TaskbarProgramFilter", "");
                RendererPath = (string)scttSubKey.GetValue("RendererPath", "Internal/Classic");

                // Enum values
                ProgramGroupCheck = (ProgramGroupCheck)scttSubKey.GetValue("ProgramGroupCheck", ProgramGroupCheck.FileNameAndPath);
                StartButtonAppearance = (StartButtonAppearance)scttSubKey.GetValue("StartButtonAppearance", StartButtonAppearance.Default);
            }

            // Setup renderer
            switch (RendererPath)
            {
                case "Internal/Classic":
                    Renderer = new ClassicRenderer();
                    break;

                case "Internal/Luna":
                    Renderer = new ImageRenderer(new ResourceManager("SimpleClassicThemeTaskbar.ThemeEngine.Themes.Luna", typeof(Config).Assembly));
                    break;

                default:
                    Renderer = new ImageRenderer(RendererPath);
                    break;
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
        }

        public static void SaveToRegistry()
        {
            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                // Boolean values
                scttSubKey.SetValue("EnableSystemTrayHover", EnableSystemTrayHover.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("EnableSystemTrayColorChange", EnableSystemTrayColorChange.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("ShowTaskbarOnAllDesktops", ShowTaskbarOnAllDesktops.ToString(), RegistryValueKind.String);
                scttSubKey.SetValue("EnableQuickLaunch", EnableQuickLaunch.ToString(), RegistryValueKind.String);

                // Integer values
                scttSubKey.SetValue("TaskbarProgramWidth", TaskbarProgramWidth, RegistryValueKind.DWord);
                scttSubKey.SetValue("SpaceBetweenTrayIcons", SpaceBetweenTrayIcons, RegistryValueKind.DWord);
                scttSubKey.SetValue("SpaceBetweenTaskbarIcons", SpaceBetweenTaskbarIcons, RegistryValueKind.DWord);
                scttSubKey.SetValue("SpaceBetweenQuickLaunchIcons", SpaceBetweenQuickLaunchIcons, RegistryValueKind.DWord);

                // Enum values
                scttSubKey.SetValue("ProgramGroupCheck", ProgramGroupCheck, RegistryValueKind.DWord);
                scttSubKey.SetValue("StartButtonAppearance", StartButtonAppearance, RegistryValueKind.DWord);

                // String values
                scttSubKey.SetValue("StartButtonImage", StartButtonImage);
                scttSubKey.SetValue("StartButtonIconImage", StartButtonIconImage);
                scttSubKey.SetValue("QuickLaunchOrder", QuickLaunchOrder);
                scttSubKey.SetValue("TaskbarProgramFilter", TaskbarProgramFilter);
                scttSubKey.SetValue("RendererPath", RendererPath);
            }
        }
    }
}