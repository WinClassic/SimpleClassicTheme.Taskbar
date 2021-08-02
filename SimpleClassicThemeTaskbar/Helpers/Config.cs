﻿
using SimpleClassicTheme.Common.Configuration;
using SimpleClassicTheme.Common.Serialization;

using SimpleClassicThemeTaskbar.ThemeEngine;
using SimpleClassicThemeTaskbar.ThemeEngine.VisualStyles;

using System.ComponentModel;
using System.Resources;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public class Config : ConfigBase<Config>
    {
        private string rendererPath = "Internal/Classic";

        public Config() : base("Taskbar", ConfigType.Taskbar)
        {
        }

        [RegistryIgnore]
        public bool ConfigChanged { get; set; } = true;

        public bool EnableActiveTaskbar { get; internal set; } = true;

        public bool EnableQuickLaunch { get; set; } = true;

        public bool EnableSystemTrayColorChange { get; set; } = true;

        public bool EnableSystemTrayHover { get; set; } = true;

        public bool EnablePassiveTray { get; set; } = false;

        public string QuickLaunchOrder { get; set; } = string.Empty;

        [RegistryIgnore]        
        public BaseRenderer Renderer { get; set; } = new ClassicRenderer();

        // VisualStyleRenderer settings
        public string VisualStylePath { get; set; } = string.Empty;
        
        public string VisualStyleSize { get; set; } = string.Empty;
        
        public string VisualStyleColor { get; set; } = string.Empty;
        
        public ExitMenuItemCondition ExitMenuItemCondition { get; internal set; }
        
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
        
        public StartButtonAppearance StartButtonAppearance { get; set; } = StartButtonAppearance.Default;
        
        public string StartButtonIconImage { get; set; } = string.Empty;
        
        public string StartButtonImage { get; set; } = string.Empty;
        
        public string TaskbarProgramFilter { get; set; } = string.Empty;

        public bool ShowTaskbarOnAllDesktops { get; set; } = true;
        
        public string Language { get; set; }

        public static string ImageThemePath { get; set; } = string.Empty;

        public Tweaks Tweaks { get; set; } = new();
    }

    public class Tweaks
    {
        [DisplayName("Enable debugging options")]
        public bool EnableDebugging { get; set; } = true;

        public ProgramGroupCheck ProgramGroupCheck { get; set; } = ProgramGroupCheck.FileNameAndPath;

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
    }
}