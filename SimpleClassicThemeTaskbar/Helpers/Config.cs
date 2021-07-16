using Microsoft.Win32;

using SimpleClassicThemeTaskbar.ThemeEngine;
using SimpleClassicThemeTaskbar.ThemeEngine.VisualStyles;

using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public class Config
    {
        private static readonly object instanceLock = new();
        private static Config instance;
        private string rendererPath = "Internal/Classic";

        public static string ImageThemePath { get; set; } = string.Empty;

        public static Config Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new Config();
                    }

                    return instance;
                }
            }
        }

        public bool ConfigChanged { get; set; } = true;
        public bool EnableGrouping { get; set; } = true;
        public bool EnablePassiveTaskbar { get; internal set; } = false;
        public bool EnableQuickLaunch { get; set; } = true;
        public bool EnableSystemTrayColorChange { get; set; } = true;
        public bool EnableSystemTrayHover { get; set; } = true;
        public ExitMenuItemCondition ExitMenuItemCondition { get; internal set; }
        public string Language { get; set; }
        public string QuickLaunchOrder { get; set; } = string.Empty;
        public BaseRenderer Renderer { get; set; } = new ClassicRenderer();

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

        public bool ShowTaskbarOnAllDesktops { get; set; } = true;
        public StartButtonAppearance StartButtonAppearance { get; set; } = StartButtonAppearance.Default;
        public string StartButtonIconImage { get; set; } = string.Empty;
        public string StartButtonImage { get; set; } = string.Empty;
        public string TaskbarProgramFilter { get; set; } = string.Empty;
        public TweaksConfig Tweaks { get; set; } = new();
        public bool UseExplorerTaskbarPosition { get; internal set; }
        public string VisualStyleColor { get; set; } = string.Empty;
        public string VisualStylePath { get; set; } = string.Empty;
        public string VisualStyleSize { get; set; } = string.Empty;

        public void LoadFromRegistry()
        {
            Language = Thread.CurrentThread.CurrentUICulture.Name;
            if (Language != "en-US" && Language != "nl-NL")
                Language = "en-US";

            Logger.Log(LoggerVerbosity.Verbose, "Config", "Loading from registry");

            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                RegistrySerializer.DeserializeFromRegistry(scttSubKey, this);
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
        }

        public void SaveToRegistry()
        {
            Logger.Log(LoggerVerbosity.Verbose, "Config", "Saving to registry");
            using (var scttSubKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\1337ftw\SimpleClassicThemeTaskbar"))
            {
                RegistrySerializer.SerializeToRegistry(scttSubKey, this);
            }
        }
    }

    public class TweaksConfig
    {
        [DisplayName("Enable debugging options")]
        public bool EnableDebugging { get; set; } = true;

        [Category("Task View")]
        [DisplayName("Grouping method")]
        public ProgramGroupCheck ProgramGroupCheck { get; set; } = ProgramGroupCheck.FileNameAndPath;

        [Category("Spacing between items")]
        [DisplayName("Spacing between Quick Launch icons")]
        public int SpaceBetweenQuickLaunchIcons { get; set; } = 2;

        [Category("Spacing between items")]
        [DisplayName("Spacing between taskband buttons")]
        public int SpaceBetweenTaskbarIcons { get; set; } = 2;

        [Category("Spacing between items")]
        [DisplayName("Spacing between tray icons")]
        public int SpaceBetweenTrayIcons { get; set; } = 2;

        [Category("Task View")]
        [DisplayName("Program width")]
        public int TaskbarProgramWidth { get; set; } = 160;
    }
}