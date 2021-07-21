using Microsoft.Win32;

using SimpleClassicThemeTaskbar.ThemeEngine;
using SimpleClassicThemeTaskbar.ThemeEngine.VisualStyles;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public class Config
    {
        private static Config instance;
        private static object instanceLock = new();
        public static Config Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                        instance.LoadFromRegistry();
                    }

                    return instance;
                }
            }
        }

        private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
        private string rendererPath = "Internal/Classic";

        #region Non-browsable properties
        [Browsable(false)]
        public bool ConfigChanged { get; set; } = true;

        [Browsable(false)]
        public bool EnableQuickLaunch { get; set; } = true;


        [Browsable(false)]
        public string QuickLaunchOrder { get; set; } = string.Empty;

        [Browsable(false)]
        public BaseRenderer Renderer { get; set; } = new ClassicRenderer();

        // VisualStyleRenderer settings
        [Browsable(false)]
        public string VisualStylePath { get; set; } = string.Empty;
        [Browsable(false)]
        public string VisualStyleSize { get; set; } = string.Empty;
        [Browsable(false)]
        public string VisualStyleColor { get; set; } = string.Empty;
        [Browsable(false)]
        public ExitMenuItemCondition ExitMenuItemCondition { get; internal set; }
        [Browsable(false)]
        public string RendererPath
        {
            get => rendererPath;
            set
            {
                switch (rendererPath = value)
                {

                    case "Internal/ImageRenderer":
                        Renderer = ImageThemePath switch
                        {
                            "Internal/ImageRenderer/Luna" => new ImageRenderer(new ResourceManager("SimpleClassicThemeTaskbar.ThemeEngine.Themes.Luna", typeof(Config).Assembly)),
                            _ => new ImageRenderer(ImageThemePath),
                        };
                        break;

                    case "Internal/VisualStyle":
                        var visualStyle = new VisualStyle(VisualStylePath);
                        var colorScheme = visualStyle.GetColorScheme(VisualStyleColor, VisualStyleSize);
                        Renderer = new VisualStyleRenderer(colorScheme);
                        break;

                    case "Internal/Classic":
                    default:
                        Renderer = new ClassicRenderer();
                        break;
                }
            }
        }

        [Browsable(false)]
        public StartButtonAppearance StartButtonAppearance { get; set; } = StartButtonAppearance.Default;

        [Browsable(false)]
        public string StartButtonIconImage { get; set; } = string.Empty;

        [Browsable(false)]
        public string StartButtonImage { get; set; } = string.Empty;

        [Browsable(false)]
        public string TaskbarProgramFilter { get; set; } = string.Empty;

        [Browsable(false)]
        public bool ShowTaskbarOnAllDesktops { get; set; } = true;

        [Browsable(false)]
        public string Language { get; set; }
        #endregion

        [Category("(Misc)")]
        [DisplayName("Enable debugging options")]
        public bool EnableDebugging { get; set; } = true;

        public bool EnableActiveTaskbar { get; internal set; } = true;
        public bool EnablePassiveTray { get; set; } = false;

        public bool EnableSystemTrayColorChange { get; set; } = true;

        public bool EnableSystemTrayHover { get; set; } = true;

        public ProgramGroupCheck ProgramGroupCheck { get; set; } = ProgramGroupCheck.FileNameAndPath;

        // ImageRenderer settings
        public static string ImageThemePath { get; set; } = string.Empty;

        [Category("Task view click actions")]
        [DisplayName("Left click")]
        public TaskbarProgramClickAction TaskbarProgramLeftClickAction { get; set; } = TaskbarProgramClickAction.ShowHide;

        [Category("Task view click actions")]
        [DisplayName("Right click")]
        public TaskbarProgramClickAction TaskbarProgramRightClickAction { get; set; } = TaskbarProgramClickAction.ContextMenu;

        [Category("Task view click actions")]
        [DisplayName("Middle click")]
        public TaskbarProgramClickAction TaskbarProgramMiddleClickAction { get; set; } = TaskbarProgramClickAction.NewInstance;

        [Category("Task view click actions")]
        [DisplayName("Left double click")]
        public TaskbarProgramClickAction TaskbarProgramLeftDoubleClickAction { get; set; } = TaskbarProgramClickAction.None;

        [Category("Task view click actions")]
        [DisplayName("Right double click")]
        public TaskbarProgramClickAction TaskbarProgramRightDoubleClickAction { get; set; } = TaskbarProgramClickAction.Close;

        [Category("Task view click actions")]
        [DisplayName("Middle double click")]
        public TaskbarProgramClickAction TaskbarProgramMiddleDoubleClickAction { get; set; } = TaskbarProgramClickAction.None;

        [Category("Spacing between items")]
        [DisplayName("Spacing between Quick Launch icons")]
        public int SpaceBetweenQuickLaunchIcons { get; set; } = 2;

        [Category("Spacing between items")]
        [DisplayName("Spacing between task view items")]
        public int SpaceBetweenTaskbarIcons { get; set; } = 2;

        [Category("Spacing between items")]
        [DisplayName("Spacing between tray icons")]
        public int SpaceBetweenTrayIcons { get; set; } = 2;

        public int TaskbarProgramWidth { get; set; } = 160;

        public void LoadFromRegistry()
        {
            Language = Thread.CurrentThread.CurrentUICulture.Name;
            if (Language != "en-US" && Language != "nl-NL")
                Language = "en-US";

            Logger.Log(LoggerVerbosity.Verbose, "Config", "Loading from registry");

            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                foreach (var property in typeof(Config).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!property.CanWrite)
                    {
                        continue;
                    }

                    object value = property.GetValue(this);
                    var registryValue = scttSubKey.GetValue(property.Name, value);

                    // string → bool
                    if (registryValue is string boolString && property.PropertyType == typeof(bool))
                    {
                        registryValue = bool.Parse(boolString);
                    }

                    // Logger.Log(LoggerVerbosity.Verbose, "Config", $"Setting property: {property.Name} → {value} → {registryValue}");
                    try
                    {
                        property.SetValue(this, registryValue);
                    }
                    catch
                    {
                    }
                }
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
        }

        public void SaveToRegistry()
        {
            Logger.Log(LoggerVerbosity.Verbose, "Config", "Saving to registry");
            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                foreach (var property in typeof(Config).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    RegistryValueKind valueKind = RegistryValueKind.Unknown;
                    object value = property.GetValue(this);
                    switch (value)
                    {
                        case bool boolValue:
                            value = boolValue.ToString();
                            valueKind = RegistryValueKind.String;
                            break;

                        case int:
                        case Enum:
                            valueKind = RegistryValueKind.DWord;
                            break;

                        case string:
                            valueKind = RegistryValueKind.String;
                            break;

                        default:
                            Logger.Log(LoggerVerbosity.Basic, "Config", $"Ignoring property {property.Name} because {value?.GetType()} is an unknown type");
                            break;
                    }

                    // Logger.Log(LoggerVerbosity.Verbose, "Config", $"Setting registry key: {property.Name} ({valueKind}) → {value}");
                    scttSubKey.SetValue(property.Name, value, valueKind);
                }
            }
        }
    }
}