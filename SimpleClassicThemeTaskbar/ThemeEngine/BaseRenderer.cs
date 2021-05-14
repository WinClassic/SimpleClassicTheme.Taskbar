using SimpleClassicThemeTaskbar.UIElements.StartButton;
using SimpleClassicThemeTaskbar.UIElements.SystemTray;
using SimpleClassicThemeTaskbar.UIElements.QuickLaunch;
using System.Drawing;

namespace SimpleClassicThemeTaskbar.ThemeEngine
{
    public abstract class BaseRenderer
    {
        public abstract int StartButtonWidth { get; }
        public abstract Color SystemTrayTimeColor { get; }
        public abstract Font SystemTrayTimeFont { get; }
        public abstract Point SystemTrayTimeLocation { get; }
        public abstract int TaskbarHeight { get; }
        public abstract int TaskButtonMinimalWidth { get; }

        public abstract void DrawQuickLaunch(QuickLaunch quickLaunch, Graphics g);

        public abstract void DrawStartButton(StartButton startButton, Graphics g);

        public abstract void DrawSystemTray(SystemTray systemTray, Graphics g);

        public abstract void DrawTaskBar(Taskbar taskbar, Graphics g);

        public abstract void DrawTaskButton(BaseTaskbarProgram taskbarProgram, Graphics g);

        public abstract void DrawTaskButtonGroupButton(GroupedTaskbarProgram taskbarProgram, Graphics g);

        public abstract void DrawTaskButtonGroupWindow(PopupTaskbarGroup taskbarGroup, Graphics g);

        public abstract Point GetQuickLaunchIconLocation(int index);

        public abstract int GetQuickLaunchWidth(int iconCount);

        public abstract Point GetSystemTrayIconLocation(int index);

        public abstract int GetSystemTrayWidth(int iconCount);

        public abstract Point GetTaskButtonGroupWindowButtonLocation(int index);

        public abstract Size GetTaskButtonGroupWindowSize(int buttonCount);
    }
}