using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.ThemeEngine.VisualStyles;
using SimpleClassicTheme.Taskbar.UIElements.QuickLaunch;
using SimpleClassicTheme.Taskbar.UIElements.StartButton;

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.Renderers
{
    class VisualStyleRenderer : BaseRenderer
    {
        private readonly VisualStyleColorScheme colorScheme;

        readonly ClassicRenderer r = new();

        public VisualStyleRenderer(VisualStyleColorScheme colorScheme)
        {
            this.colorScheme = colorScheme;
        }

        public override Point SystemTrayTimeLocation => r.SystemTrayTimeLocation;
        public override Font SystemTrayTimeFont => colorScheme["TrayNotify::Clock"].Font;
        public override Color SystemTrayTimeColor => colorScheme["TrayNotify::Clock"].TextColor.Value;

        public override int StartButtonWidth => r.StartButtonWidth;

        public override int TaskbarHeight => Math.Max(colorScheme["TaskBar.BackgroundBottom"].Image.Height, 30);

        public override int TaskButtonMinimalWidth => r.TaskButtonMinimalWidth;

        public override int QuickLaunchPadding => 3;

        public override Point GetQuickLaunchIconLocation(int index)
        {
            return r.GetQuickLaunchIconLocation(index);
        }
        public override int GetQuickLaunchWidth(int iconCount)
        {
            return r.GetQuickLaunchWidth(iconCount);
        }

        public override void DrawTaskBar(Taskbar taskbar, Graphics g)
        {
            var background = colorScheme["TaskBar.BackgroundBottom"];
            g.DrawElement(background, new Rectangle(Point.Empty, taskbar.Size));
            // using (TextureBrush brush = new(taskbarTexture, WrapMode.Tile))
            // 	g.FillRectangle(brush, taskbar.ClientRectangle);
        }

        public override void DrawTaskButton(BaseTaskbarProgram taskbarProgram, Graphics g)
        {
            bool IsActive = taskbarProgram.IsPushed;
            bool IsHover = taskbarProgram.ClientRectangle.Contains(taskbarProgram.PointToClient(Control.MousePosition));
            int index = (IsHover ? 1 : 0) + (IsActive ? 2 : 0);

            //r.DrawTaskButtonGroupWindow(taskbarGroup, g);
            var toolbarElement = colorScheme["TaskBand::Toolbar"];
            var toolbarElementPressed = colorScheme["TaskBand::Toolbar(pressed)"];
            var buttonElement = colorScheme["TaskBand::Toolbar.Button"];

            var textColor = toolbarElement.TextColor.Value;

            if (IsActive && toolbarElementPressed.TextColor.HasValue)
            {
                textColor = toolbarElementPressed.TextColor.Value;
            }

            using var textBrush = new SolidBrush(textColor);

            g.DrawElement(buttonElement, new Rectangle(Point.Empty, taskbarProgram.Size), index);

            // Draw text and icon
            StringFormat format = new();
            format.HotkeyPrefix = HotkeyPrefix.None;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;
            if (taskbarProgram.IconImage != null)
            {
                Rectangle iconRect = IsActive
                    ? new Rectangle(5, 8, 16, 16)
                    : new Rectangle(4, 7, 16, 16);

                g.DrawImage(taskbarProgram.IconImage, iconRect);

                if (taskbarProgram.Width >= 60)
                    g.DrawString(taskbarProgram.Title, toolbarElement.Font, textBrush, IsActive ? new Rectangle(21, 10, taskbarProgram.Width - 21 - 3 - taskbarProgram.SpaceNeededNextToText, 13) : new Rectangle(20, 9, taskbarProgram.Width - 20 - 3 - taskbarProgram.SpaceNeededNextToText, 14), format);
            }
            else
            {
                g.DrawString(taskbarProgram.Title, toolbarElement.Font, textBrush, IsActive ? new Rectangle(5, 10, taskbarProgram.Width - 5 - 3 - taskbarProgram.SpaceNeededNextToText, 13) : new Rectangle(4, 9, taskbarProgram.Width - 4 - 3 - taskbarProgram.SpaceNeededNextToText, 14), format);
            }
        }

        public override void DrawTaskButtonGroupButton(GroupedTaskbarProgram taskbarProgram, Graphics g)
        {

            // var sizingMargins = new Padding(8, 3, 18, 8);
            // var rectangle = new Rectangle(0, 28, 13, 28);
            // var geometry = new NinePatchGeometry(sizingMargins, rectangle);
            // g.DrawNinePatch(taskBandButtonBitmap, geometry, new Rectangle(Point.Empty, taskbarProgram.Size));
        }

        public override void DrawTaskButtonGroupWindow(PopupTaskbarGroup taskbarGroup, Graphics g)
        {

        }

        public override void DrawStartButton(StartButton startButton, Graphics g)
        {
            var startButtonElement = colorScheme["Start::Button"];

            if (startButton.Width != startButtonElement.Image.Width)
                startButton.Width = startButtonElement.Image.Width;

            if (startButton.Height != TaskbarHeight)
            {
                startButton.Height = TaskbarHeight;
            }

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.TextContrast = 12;

            using var textBrush = new SolidBrush(startButtonElement.TextColor.Value);
            g.DrawElement(startButtonElement, new Rectangle(0, 0, startButton.Width, startButton.Height + 2), (int)startButton.MouseState);
            g.DrawString(startButtonElement, "start", 35, 3, startButton.Width - 35, startButton.Height - 3);
        }

        public override void DrawSystemTray(Control systemTray, Graphics g)
        {
            var element = colorScheme["TrayNotifyHoriz::TrayNotify.Background"];

            g.DrawElement(element, new Rectangle(Point.Empty, systemTray.Size));


            //using (TextureBrush brush = new(systemTrayTexture, WrapMode.Tile))
            //	g.FillRectangle(brush, systemTray.ClientRectangle);
            //g.DrawImageUnscaled(systemTrayBorder, new Point(0, 0));

            //r.DrawSystemTray(systemTray, g);
        }

        public override void DrawQuickLaunch(QuickLaunch quickLaunch, Graphics g)
        {
            r.DrawQuickLaunch(quickLaunch, g);
        }

        public override Point GetSystemTrayIconLocation(int index)
        {
            var element = colorScheme["TrayNotifyHoriz::TrayNotify.Background"];
            var point = r.GetSystemTrayIconLocation(index);
            point.X += element.ContentMargins.Value.Left;
            return point;
        }

        public override int GetSystemTrayWidth(int iconCount)
        {
            var element = colorScheme["TrayNotifyHoriz::TrayNotify.Background"];
            return r.GetSystemTrayWidth(iconCount) + element.ContentMargins.Value.Horizontal;
        }

        public override Point GetTaskButtonGroupWindowButtonLocation(int index)
        {
            return r.GetTaskButtonGroupWindowButtonLocation(index);
        }

        public override Size GetTaskButtonGroupWindowSize(int buttonCount)
        {
            return r.GetTaskButtonGroupWindowSize(buttonCount);
        }

        public override void DrawQuickLaunchIcon(QuickLaunchIcon icon, Graphics g, MouseState state)
        {
            // HACK: hard coded offset to match screenshots
            Rectangle rectangle = new(0, -3, icon.Width, icon.Height + 4);

            var element = colorScheme["TaskBar::Toolbar.Button"];
            g.DrawElement(element, rectangle, state == MouseState.Pressed ? 2 : 1);
        }
    }
}
