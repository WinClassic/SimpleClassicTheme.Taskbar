using SimpleClassicTheme.Common.Configuration;
using SimpleClassicTheme.Common.Serialization;

using SimpleClassicTheme.Taskbar.ThemeEngine;
using SimpleClassicTheme.Taskbar.ThemeEngine.Renderers;
using SimpleClassicTheme.Taskbar.ThemeEngine.VisualStyles;

using System.ComponentModel;
using System.Diagnostics;
using System.Resources;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public class Config : ConfigBase<Config>
    {

        private string rendererPath = "Internal/Classic";

        public Config() : base(@"Simple Classic Theme\Taskbar", ConfigType.Taskbar)
        {
        }

        [RegistryIgnore]
        public bool ConfigChanged { get; set; } = true;

        [RegistryIgnore]
        public BaseRenderer Renderer { get; set; } = new ClassicRenderer();

        // VisualStyleRenderer settings
        public bool EnableActiveTaskbar { get; internal set; } = false;
        public bool EnableGrouping { get; set; } = true;
        public bool EnablePassiveTaskbar { get; internal set; } = false;
        public bool EnablePassiveTray { get; set; } = false;
        public bool EnableQuickLaunch { get; set; } = true;
        public bool EnableSystemTrayColorChange { get; set; } = true;
        public bool EnableSystemTrayHover { get; set; } = true;
        public bool ShowTaskbarOnAllDesktops { get; set; } = true;
        public bool UseExplorerTaskbarPosition { get; internal set; }
        public ExitMenuItemCondition ExitMenuItemCondition { get; internal set; }
        public StartButtonAppearance StartButtonAppearance { get; set; } = StartButtonAppearance.Default;
        public static string ImageThemePath { get; set; } = string.Empty;
        public string Language { get; set; }
        public string QuickLaunchOrder { get; set; } = string.Empty;
        public string StartButtonIconImage { get; set; } = string.Empty;
        public string StartButtonImage { get; set; } = string.Empty;
        public string TaskbarProgramFilter { get; set; } = string.Empty;
        public string VisualStyleColor { get; set; } = string.Empty;
        public string VisualStylePath { get; set; } = string.Empty;
        public string VisualStyleSize { get; set; } = string.Empty;

        public bool IsLocked { get; set; } = false;
        public Tweaks Tweaks { get; set; } = new();

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
    }

    public class Tweaks
    {
        [DisplayName("Enable debugging options")]
        public bool EnableDebugging { get; set; } = Debugger.IsAttached;

        [Category("Task view")]
        [DisplayName("Grouping method")]
        public ProgramGroupCheck ProgramGroupCheck { get; set; } = ProgramGroupCheck.FileNameAndPath;

        [Category("Task view click actions")]
        [DisplayName("Left click")]
        [DefaultValue(TaskbarProgramClickAction.ShowHide)]
        public TaskbarProgramClickAction TaskbarProgramLeftClickAction { get; set; } = TaskbarProgramClickAction.ShowHide;

        [Category("Task view click actions")]
        [DisplayName("Right click")]
        [DefaultValue(TaskbarProgramClickAction.ContextMenu)]
        public TaskbarProgramClickAction TaskbarProgramRightClickAction { get; set; } = TaskbarProgramClickAction.ContextMenu;

        [Category("Task view click actions")]
        [DisplayName("Middle click")]
        [DefaultValue(TaskbarProgramClickAction.NewInstance)]
        public TaskbarProgramClickAction TaskbarProgramMiddleClickAction { get; set; } = TaskbarProgramClickAction.NewInstance;

        [Category("Task view click actions")]
        [DisplayName("Left double click")]
        [DefaultValue(TaskbarProgramClickAction.None)]
        public TaskbarProgramClickAction TaskbarProgramLeftDoubleClickAction { get; set; } = TaskbarProgramClickAction.None;

        [Category("Task view click actions")]
        [DisplayName("Right double click")]
        [DefaultValue(TaskbarProgramClickAction.Close)]
        public TaskbarProgramClickAction TaskbarProgramRightDoubleClickAction { get; set; } = TaskbarProgramClickAction.Close;

        [Category("Task view click actions")]
        [DisplayName("Middle double click")]
        [DefaultValue(TaskbarProgramClickAction.None)]
        public TaskbarProgramClickAction TaskbarProgramMiddleDoubleClickAction { get; set; } = TaskbarProgramClickAction.None;

        [Category("Spacing between items")]
        [DisplayName("Quick Launch icons")]
        [Description("Defines the space between quick launch icons in pixels.")]
        [DefaultValue(0)]
        public int SpaceBetweenQuickLaunchIcons { get; set; } = 0;

        [Category("Spacing between items")]
        [DisplayName("Task View items")]
        [Description("Defines the space between task view items in pixels.")]
        [DefaultValue(2)]
        public int SpaceBetweenTaskbarIcons { get; set; } = 2;

        [Category("Spacing between items")]
        [DisplayName("Tray icons")]
        [Description("Defines the space between tray icons in pixels.")]
        [DefaultValue(2)]
        public int SpaceBetweenTrayIcons { get; set; } = 2;

        [Category("Task view")]
        [DisplayName("Program width")]
        [Description("Defines the width of programs in pixels.")]
        public int TaskbarProgramWidth { get; set; } = 160;
    }
}