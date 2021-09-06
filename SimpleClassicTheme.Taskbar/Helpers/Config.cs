using SimpleClassicTheme.Common.Configuration;
using SimpleClassicTheme.Common.Serialization;
using SimpleClassicTheme.Taskbar.ThemeEngine.Renderers;
using Craftplacer.Windows.VisualStyles;

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public class Config : ConfigBase<Config>
    {
        private string rendererPath = "Internal/Classic";
        private string _language = "en-US";

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
        public string Language
        {
            get => _language;
            set
            {
                var cultureInfo = new CultureInfo(_language = value);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
        }
        public string QuickLaunchOrder { get; set; } = string.Empty;
        public string StartButtonIconImage { get; set; } = string.Empty;
        public string StartButtonImage { get; set; } = string.Empty;
        public string TaskbarProgramFilter { get; set; } = string.Empty;
        public string VisualStyleColor { get; set; } = string.Empty;
        public string VisualStylePath { get; set; } = string.Empty;
        public string VisualStyleSize { get; set; } = string.Empty;
        public bool IsLocked { get; set; } = false;
        public GroupAppearance GroupAppearance { get; set; } = GroupAppearance.Default;
        public Tweaks Tweaks { get; set; } = new();
        public DockStyle Position { get; set; } = DockStyle.Bottom;

        public string RendererPath
        {
            get => rendererPath;
            set
            {
                switch (rendererPath = value)
                {
                    case "Internal/ImageRenderer":
                        Renderer = new ImageRenderer(ImageThemePath);
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

        [DisplayName("Enable virtual desktops")]
        [Description("Defines whether integration for virtual desktops should be enabled. (Windows 10 and up)")]
        public bool EnableVirtualDesktops { get; set; } = HelperFunctions.CanEnableVirtualDesktops;

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

        [Description("Contains experimental changes that aren't ready yet")]
        public Experiments Experiments { get; set; } = new();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Experiments
    {
        [DisplayName("New calculation of start button size for visual styles")]
        [DefaultValue(false)]
        public bool NewVsStartCalc { get; set; }

        public override string ToString()
        {
            return typeof(Experiments).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length + " experiment(s)";
        }
    }
}