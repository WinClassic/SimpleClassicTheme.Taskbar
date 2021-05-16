using Microsoft.Win32;

using SimpleClassicThemeTaskbar.ThemeEngine;

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace SimpleClassicThemeTaskbar.Helpers
{
    //Config class
    //TODO: Move more configurable settings to here
    //TODO: Make config menu
    public static class Config
    {
        private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
        private static string rendererPath = "Internal/Classic";
        public static bool ConfigChanged { get; set; } = true;
        public static bool EnableQuickLaunch { get; set; } = true;
        public static bool EnableSystemTrayColorChange { get; set; } = true;
        public static bool EnableSystemTrayHover { get; set; } = true;
        public static string Language { get; set; }
        public static ProgramGroupCheck ProgramGroupCheck { get; set; } = ProgramGroupCheck.FileNameAndPath;
        public static string QuickLaunchOrder { get; set; } = string.Empty;
        public static BaseRenderer Renderer { get; set; } = new ClassicRenderer();

        public static string RendererPath
        {
            get => rendererPath;
            set
            {
                switch (rendererPath = value)
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
            }
        }

        public static bool ShowTaskbarOnAllDesktops { get; set; } = true;
        public static int SpaceBetweenQuickLaunchIcons { get; set; } = 2;
        public static int SpaceBetweenTaskbarIcons { get; set; } = 2;
        public static int SpaceBetweenTrayIcons { get; set; } = 2;
        public static StartButtonAppearance StartButtonAppearance { get; set; } = StartButtonAppearance.Default;
        public static string StartButtonIconImage { get; set; } = string.Empty;
        public static string StartButtonImage { get; set; } = string.Empty;
        public static string TaskbarProgramFilter { get; set; } = string.Empty;
        public static int TaskbarProgramWidth { get; set; } = 160;

        public static void LoadFromRegistry()
        {
            Language = Thread.CurrentThread.CurrentUICulture.Name;
            if (Language != "en-US" && Language != "nl-NL")
                Language = "en-US";

            Logger.Log(LoggerVerbosity.Verbose, "Config", "Loading from registry");

            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                foreach (var property in typeof(Config).GetProperties(bindingFlags))
                {
                    object value = property.GetValue(null);
                    var registryValue = scttSubKey.GetValue(property.Name, value);

                    // string → bool
                    if (registryValue is string boolString && property.PropertyType == typeof(bool))
                    {
                        registryValue = bool.Parse(boolString);
                    }

                    Logger.Log(LoggerVerbosity.Verbose, "Config", $"Setting property: {property.Name} → {value} → {registryValue}");
                    try
                    {
                        property.SetValue(null, registryValue);
                    }
                    catch
                    {
                    }
                }
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
        }

        public static void SaveToRegistry()
        {
            Logger.Log(LoggerVerbosity.Verbose, "Config", "Saving to registry");
            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                foreach (var property in typeof(Config).GetProperties(bindingFlags))
                {
                    RegistryValueKind valueKind = RegistryValueKind.Unknown;
                    object value = property.GetValue(null);
                    switch (value)
                    {
                        case bool boolValue:
                            value = boolValue.ToString();
                            valueKind = RegistryValueKind.String;
                            break;

                        case int intValue:
                        case Enum enumValue:
                            valueKind = RegistryValueKind.DWord;
                            break;

                        case String stringValue:
                            valueKind = RegistryValueKind.String;
                            break;

                        default:
                            Logger.Log(LoggerVerbosity.Basic, "Config", $"Ignoring property {property.Name} because {value.GetType()} is an unknown type");
                            break;
                    }

                    Logger.Log(LoggerVerbosity.Verbose, "Config", $"Setting registry key: {property.Name} ({valueKind}) → {value}");
                    scttSubKey.SetValue(property.Name, value, valueKind);
                }
            }
        }
    }
}