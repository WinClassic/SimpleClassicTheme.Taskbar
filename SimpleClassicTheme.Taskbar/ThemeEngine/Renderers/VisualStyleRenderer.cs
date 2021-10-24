﻿using Craftplacer.Windows.VisualStyles;

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
    public class VisualStyleRenderer : BaseRenderer
    {
        private readonly Bitmap _flagBitmap;
        private readonly ColorScheme colorScheme;
        private readonly ClassicRenderer r = new();

        public VisualStyleRenderer(ColorScheme colorScheme)
        {
            this.colorScheme = colorScheme;

            _flagBitmap = HelperFunctions.ChangePixelFormat(Properties.Resources.FlagXP, PixelFormat.Format32bppArgb);
        }

        public override WfPadding QuickLaunchPadding
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

        public override int StartButtonWidth
        {
            get
            {
                var startButtonElement = colorScheme["Start::Button"];
                if (Config.Default.Tweaks.Experiments.NewVsStartCalc)
                {
                    Padding margins = startButtonElement.ContentMargins ?? Padding.Empty;

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

        public override Color SystemTrayTimeColor => colorScheme["TrayNotify::Clock"].TextColor.Value;
        public override Font SystemTrayTimeFont => colorScheme["TrayNotify::Clock"].Font;
        public override Point SystemTrayTimeLocation => r.SystemTrayTimeLocation;
        public override int TaskbarHeight => Math.Max(colorScheme["TaskBar.BackgroundBottom"].Image.Height, 30);

        public override int TaskButtonMinimalWidth => r.TaskButtonMinimalWidth;

        public void DrawGripper(int x, int y, int height, Graphics g)
        {
            var element = colorScheme["TaskBar::Rebar.Gripper"];
            var gripperWidth = element.Image.Width;
            g.DrawElement(element, new Rectangle(x, y, gripperWidth, height));
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

        public override void DrawQuickLaunchIcon(QuickLaunchIcon icon, Graphics g, MouseState state)
        {
            // HACK: hard coded offset to match screenshots
            Rectangle rectangle = new(0, -3, icon.Width, icon.Height + 4);

            var element = colorScheme["TaskBar::Toolbar.Button"];
            g.DrawElement(element, rectangle, state == MouseState.Pressed ? 2 : 1);
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

            if (IsActive && toolbarElementPressed != null && toolbarElementPressed.TextColor.HasValue)
            {
                textColor = toolbarElementPressed.TextColor.Value;
            }

            using var textBrush = new SolidBrush(textColor);
            if (!(taskbarProgram is SingleTaskbarProgram single && single.IsInsidePopup == true))
            {
                Rectangle bounds = new(Point.Empty, taskbarProgram.Size);
                g.DrawElement(buttonElement, bounds, index);
            }

            // Draw text and icon
            StringFormat stringFormat = new()
            {
                HotkeyPrefix = HotkeyPrefix.None,
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
            };

            Padding margins = buttonElement.ContentMargins ?? Padding.Empty;
            int x = 10;
            int y = margins.Top + 3;
            int equalHeight = taskbarProgram.Height - y - y;

            if (taskbarProgram.IconImage != null)
            {
                const int iconSize = 16;

                g.DrawImage(
                    taskbarProgram.IconImage,
                    x,
                    (taskbarProgram.Height / 2) - (iconSize / 2),
                    iconSize,
                    iconSize);

                x += iconSize + 2;
            }

            if (taskbarProgram is GroupedTaskbarProgram group)
            {
                using (var groupCountTextBrush = new SolidBrush(groupCount.TextColor ?? textColor))
                {
                    var groupCountText = group.ProgramWindows.Count.ToString();
                    Rectangle groupCountTextRect = new(x, y, taskbarProgram.Width - x, equalHeight);

                    g.DrawString(
                        groupCountText,
                        groupCount.Font,
                        groupCountTextBrush,
                        groupCountTextRect,
                        stringFormat);

                    x += (int)g.MeasureString(groupCountText, groupCount.Font).Width;
                }
            }

            Rectangle layoutRectangle = new(x, y, taskbarProgram.Width - x, equalHeight);
            g.DrawString(
                taskbarProgram.Title,
                toolbarElement.Font,
                textBrush,
                layoutRectangle,
                stringFormat);
        }

        public override void DrawTaskButtonGroupButton(GroupedTaskbarProgram taskbarProgram, Graphics g)
        {
            if (Config.Default.GroupAppearance == GroupAppearance.Default)
            {
                var element = colorScheme["Combobox.DropDownButton"];
                var rectangle = new Rectangle(taskbarProgram.Width - 13, 4, 13, 13);
                g.DrawElement(element, rectangle, 0);
            }
        }

        public override void DrawTaskButtonGroupWindow(PopupTaskbarGroup taskbarGroup, Graphics g)
        {
            var element = colorScheme["TaskBandGroupMenu::Toolbar"];
            g.DrawElement(element, new(Point.Empty, taskbarGroup.Size));
        }

        public override Point GetQuickLaunchIconLocation(int index)
        {
            return r.GetQuickLaunchIconLocation(index);
        }

        public override int GetQuickLaunchWidth(int iconCount)
        {
            return r.GetQuickLaunchWidth(iconCount);
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
    }
}