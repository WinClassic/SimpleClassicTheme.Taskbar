using Craftplacer.Windows.VisualStyles;

using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.UIElements.QuickLaunch;
using SimpleClassicTheme.Taskbar.UIElements.StartButton;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

using Padding = Craftplacer.Windows.VisualStyles.Padding;
using WfPadding = System.Windows.Forms.Padding;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.Renderers
{
    class VisualStyleRenderer : BaseRenderer
    {
        private readonly ColorScheme colorScheme;
        private readonly ClassicRenderer r = new();
        private Bitmap _flagBitmap;

        public VisualStyleRenderer(ColorScheme colorScheme)
        {
            this.colorScheme = colorScheme;

            _flagBitmap = HelperFunctions.ChangePixelFormat(Properties.Resources.FlagXP, PixelFormat.Format32bppArgb);
        }

        public override Point SystemTrayTimeLocation => r.SystemTrayTimeLocation;
        public override Font SystemTrayTimeFont => colorScheme["TrayNotify::Clock"].Font;
        public override Color SystemTrayTimeColor => colorScheme["TrayNotify::Clock"].TextColor.Value;

        public override int StartButtonWidth
        {
            get
            {
                var startButtonElement = colorScheme["Start::Button"];
                if (Config.Default.Tweaks.Experiments.NewVsStartCalc)
                {
                    Padding margins = startButtonElement.ContentMargins.HasValue
                   ? startButtonElement.ContentMargins.Value
                   : Padding.Empty;

                    int textWidth = 0;
                    using (var graphics = Graphics.FromImage(startButtonElement.Image))
                    {
                        var textSize = graphics.MeasureString("start", startButtonElement.Font);
                        textWidth = (int)textSize.Width;
                    }

                    // Last constant value is the missing icon width, the other one is just a hacky offset
                    return margins.Horizontal + 25 + textWidth - 4;
                }
                else
                {
                    return startButtonElement.Image.Width;
                }
            }
        }

        public override int TaskbarHeight => Math.Max(colorScheme["TaskBar.BackgroundBottom"].Image.Height, 30);

        public override int TaskButtonMinimalWidth => r.TaskButtonMinimalWidth;

        public override Padding QuickLaunchPadding
        {
            get
            {
                if (Config.Default.IsLocked)
                {
                    return new(3);
                }
                else
                {
                    return new(15, 0, 12, 0);
                }
            }
        }

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
            Element toolbarElement = colorScheme["TaskBand::Toolbar"];
            Element toolbarElementPressed = colorScheme["TaskBand::Toolbar(pressed)"];
            Element buttonElement = colorScheme["TaskBand::Toolbar.Button"];
            Element groupCount = colorScheme["TaskBand.GroupCount"];

            var textColor = toolbarElement.TextColor.Value;

            if (IsActive && toolbarElementPressed.TextColor.HasValue)
            {
                textColor = toolbarElementPressed.TextColor.Value;
            }

            using var textBrush = new SolidBrush(textColor);

            Rectangle bounds = new Rectangle(Point.Empty, taskbarProgram.Size);
            g.DrawElement(buttonElement, bounds, index);

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
            int startButtonHeight;
            if (Config.Default.Tweaks.Experiments.NewVsStartCalc)
            {
                startButtonHeight = startButtonElement.Image.Height / startButtonElement.ImageCount;
            }
            else
            {
                startButtonHeight = TaskbarHeight + 2;
            }

            var expectedSize = new Size(StartButtonWidth, startButtonHeight);

            if (startButton.Size != expectedSize)
            {
                startButton.Size = expectedSize;
            }

            var contentMargin = startButtonElement.ContentMargins ?? Padding.Empty;
            using var textBrush = new SolidBrush(startButtonElement.TextColor.Value);
            Rectangle bounds = new(0, 0, startButton.Width, startButton.Height);
            g.DrawElement(startButtonElement, bounds, (int)startButton.MouseState);
            g.DrawImageUnscaled(_flagBitmap, contentMargin.Left - 1, contentMargin.Top + 3);
            g.DrawString(startButtonElement, "start", 35, 3, startButton.Width - 35, startButton.Height - 3);
        }

        public override void DrawSystemTray(Control systemTray, Graphics g)
        {
            var element = colorScheme["TrayNotifyHoriz::TrayNotify.Background"];
            g.DrawElement(element, new Rectangle(Point.Empty, systemTray.Size));
        }

        public override void DrawQuickLaunch(QuickLaunch systemTray, Graphics g)
        {
            if (Config.Default.IsLocked)
            {
                return;
            }

            DrawGripper(5, -1, systemTray.Height, g);
            DrawGripper(systemTray.Width - 6, -1, systemTray.Height, g);
        }

        public void DrawGripper(int x, int y, int height, Graphics g)
        {
            var element = colorScheme["TaskBar::Rebar.Gripper"];
            var gripperWidth = element.Image.Width;
            g.DrawElement(element, new Rectangle(x, y, gripperWidth, height));
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
