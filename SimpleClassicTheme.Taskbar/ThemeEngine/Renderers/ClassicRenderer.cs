using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;
using SimpleClassicTheme.Taskbar.UIElements.QuickLaunch;
using SimpleClassicTheme.Taskbar.UIElements.StartButton;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.Renderers
{
    internal class ClassicRenderer : BaseRenderer
    {
        private readonly Bitmap checkerboardPattern;

        public ClassicRenderer()
        {
            checkerboardPattern = new Bitmap(2, 2);
            checkerboardPattern.SetPixel(0, 0, SystemColors.Control);
            checkerboardPattern.SetPixel(0, 1, SystemColors.ControlLightLight);
            checkerboardPattern.SetPixel(1, 1, SystemColors.Control);
            checkerboardPattern.SetPixel(1, 0, SystemColors.ControlLightLight);
        }

        public override int StartButtonWidth
        {
            get
            {
                if (Config.Default.StartButtonAppearance == StartButtonAppearance.CustomButton)
                    try
                    {
                        return Image.FromFile(Config.Default.StartButtonImage).Width;
                    }
                    catch { }
                return 55;
            }
        }

        public override Color SystemTrayTimeColor => SystemColors.ControlText;
        public override Font SystemTrayTimeFont => SystemFonts.DefaultFont;
        public override Point SystemTrayTimeLocation => new(-52, 9);
        public override int TaskbarHeight => 28;
        public override int TaskButtonMinimalWidth => 24;

        public override Padding QuickLaunchPadding
        {
            get
            {
                int left = 1;
                int right = 0;

                if (!Config.Default.IsLocked)
                {
                    left += 11;
                    right += 11;
                }

                return new(left, 0, right, 0);
            }
        }

        public override void DrawQuickLaunch(QuickLaunch systemTray, Graphics g)
        {
            if (Config.Default.IsLocked)
            {
                return;
            }

            DrawGripper(1, -1, systemTray.Height, g);
            DrawGripper(systemTray.Width - 7, -1, systemTray.Height, g);
        }

        public void DrawGripper(int x, int y, int height, Graphics g)
        {
            const int topPad = 4;
            const int bottomPad = 2;

            Rectangle dentRect = new(x, y + topPad, 2, height - topPad - bottomPad);
            ControlPaint.DrawBorder(g, dentRect,
                SystemColors.ControlDark, 1, ButtonBorderStyle.Solid,
                Color.Transparent, 0, ButtonBorderStyle.Solid,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Solid,
                Color.Transparent, 0, ButtonBorderStyle.Solid);

            Rectangle notchRect = new(x + 4, dentRect.Y + 2, 3, dentRect.Height - 4);
            ControlPaint.DrawBorder3D(g, notchRect, Border3DStyle.RaisedInner);
        }
        public override void DrawStartButton(StartButton startButton, Graphics g)
        {
            try
            {
                Image icon = Properties.Resources.startIcon95;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                if (Config.Default.StartButtonAppearance == StartButtonAppearance.CustomButton)
                {
                    Image image = Image.FromFile(Config.Default.StartButtonImage);
                    if (image.Height != 66)
                        throw new InvalidOperationException();
                    if (startButton.Width != image.Width)
                        startButton.Width = image.Width;
                    g.DrawImage(image, new Rectangle(2, 4, image.Width, 22), new Rectangle(0, startButton.Pressed ? 44 : startButton.ClientRectangle.Contains(startButton.PointToClient(Control.MousePosition)) ? 22 : 0, image.Width, 22), GraphicsUnit.Pixel);
                    return;
                }
                else if (Config.Default.StartButtonAppearance == StartButtonAppearance.CustomIcon)
                {
                    icon = Image.FromFile(Config.Default.StartButtonIconImage);
                }
                if (startButton.Width != 57)
                    startButton.Width = 57;
                RECT rect = startButton.ClientRectangle;
                rect.Left += 2;
                rect.Top += 4;
                rect.Bottom -= 2;
                uint buttonStyle = User32.DFCS_BUTTONPUSH;

                if (startButton.Pressed)
                {
                    buttonStyle |= User32.DFCS_PUSHED;
                }

                _ = User32.DrawFrameControl(g.GetHdc(), ref rect, User32.DFC_BUTTON, buttonStyle);
                g.ReleaseHdc();
                g.ResetTransform();
                bool mouseIsDown = startButton.ClientRectangle.Contains(startButton.PointToClient(Control.MousePosition)) && (Control.MouseButtons & MouseButtons.Left) != 0;
                g.DrawImage(icon ?? Properties.Resources.startIcon95, mouseIsDown ? new Point(7, 8) : new Point(6, 7));
                g.DrawString("Start", new Font("Tahoma", 8F, FontStyle.Bold), SystemBrushes.ControlText, mouseIsDown ? new PointF(23F, 9F) : new PointF(22F, 8F));
                icon.Dispose();
            }
            catch
            {
                // Fill background
                g.FillRectangle(new SolidBrush(SystemColors.Control), startButton.ClientRectangle);
                g.DrawString("ERROR", SystemFonts.DefaultFont, Brushes.Black, new PointF(0F, 0F));
            }
        }

        public override void DrawSystemTray(Control systemTray, Graphics g)
        {
            Rectangle rect = systemTray.ClientRectangle;
            rect.Location = new Point(0, 4);
            rect.Height = 22;
            ControlPaint.DrawBorder3D(g, rect, Border3DStyle.SunkenOuter);
        }

        public override void DrawTaskBar(Taskbar taskbar, Graphics g)
        {
            // Fill background
            Rectangle rect = taskbar.ClientRectangle;
            g.FillRectangle(new SolidBrush(SystemColors.Control), taskbar.ClientRectangle);

            // Draw 1px line at the top
            g.DrawLine(new Pen(SystemColors.ControlLightLight), new Point(0, 1), new Point(rect.Width - 1, 1));
        }

        public override void DrawTaskButton(BaseTaskbarProgram taskbarProgram, Graphics g)
        {
            bool isPushed = taskbarProgram.IsPushed;
            bool isPressed = taskbarProgram.IsPressed;

            // Draw button frame
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            Rectangle newRect = taskbarProgram.ClientRectangle;
            newRect.Y += 4;
            newRect.Height -= 6;
            RECT rect = newRect;
            uint buttonStyle = User32.DFCS_BUTTONPUSH;

            if (taskbarProgram.IsPushed)
            {
                buttonStyle |= User32.DFCS_PUSHED;
            }

            _ = User32.DrawFrameControl(g.GetHdc(), ref rect, User32.DFC_BUTTON, buttonStyle);
            g.ReleaseHdc();
            g.ResetTransform();

            // Draw checkerboard pattern if the button is being held down
            if (isPressed && checkerboardPattern != null)
            {
                using (TextureBrush brush = new(checkerboardPattern, WrapMode.Tile))
                {
                    g.FillRectangle(brush, Rectangle.Inflate(newRect, -2, -2));
                }
            }

            // Generate font
            Font font = SystemFonts.DefaultFont;
            if (isPushed)
                font = new Font(font, FontStyle.Bold);

            // Draw text and icon
            StringFormat format = new();
            format.HotkeyPrefix = HotkeyPrefix.None;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;
            if (taskbarProgram.IconImage != null)
            {
                g.DrawImage(taskbarProgram.IconImage, isPushed ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16));

                if (taskbarProgram.Width >= 60)
                    g.DrawString(taskbarProgram.Title, font, SystemBrushes.ControlText, isPushed ? new Rectangle(21, 9, taskbarProgram.Width - 21 - 3 - taskbarProgram.SpaceNeededNextToText, 14) : new Rectangle(20, 8, taskbarProgram.Width - 20 - 3 - taskbarProgram.SpaceNeededNextToText, 15), format);
            }
            else
            {
                g.DrawString(taskbarProgram.Title, font, SystemBrushes.ControlText, isPushed ? new Rectangle(5, 9, taskbarProgram.Width - 5 - 3 - taskbarProgram.SpaceNeededNextToText, 14) : new Rectangle(4, 8, taskbarProgram.Width - 4 - 3 - taskbarProgram.SpaceNeededNextToText, 15), format);
            }
        }

        public override void DrawTaskButtonGroupButton(GroupedTaskbarProgram taskbarProgram, Graphics g)
        {
            // Draw button frame
            Rectangle newRect = taskbarProgram.ClientRectangle;
            newRect.X += taskbarProgram.Width - 19;
            newRect.Width -= taskbarProgram.Width - 19 + 3;
            newRect.Y += 7;
            newRect.Height -= 12;
            RECT rect = newRect;

            uint buttonStyle = User32.DFCS_BUTTONPUSH;

            if (taskbarProgram.GroupWindow.Visible)
            {
                buttonStyle |= User32.DFCS_PUSHED;
            }

            _ = User32.DrawFrameControl(g.GetHdc(), ref rect, User32.DFC_BUTTON, buttonStyle);
            g.ReleaseHdc();
            g.ResetTransform();

            // Generate font
            Font font = SystemFonts.DefaultFont;

            // Draw number
            StringFormat format = new();
            format.HotkeyPrefix = HotkeyPrefix.None;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;
            newRect.Inflate(new Size(-2, -2));
            //g.DrawString(taskbarProgram.ProgramWindows.Count.ToString(), font, SystemBrushes.ControlText, taskbarProgram.GroupWindow.Visible ? new Rectangle(taskbarProgram.Width - 15, 9, 7, 14) : new Rectangle(taskbarProgram.Width - 16, 8, 7, 15), format);
            g.DrawString(taskbarProgram.ProgramWindows.Count.ToString(), font, SystemBrushes.ControlText, newRect, format);
        }

        public override void DrawTaskButtonGroupWindow(PopupTaskbarGroup taskbarGroup, Graphics g)
        {
            // Define constants
            const uint DFC_BUTTON = 4;
            const uint DFCS_BUTTONPUSH = 0x10;

            Rectangle newRect = taskbarGroup.ClientRectangle;
            RECT rect = newRect;
            uint buttonStyle = DFCS_BUTTONPUSH;
            _ = User32.DrawFrameControl(g.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
            g.ReleaseHdc();
            g.ResetTransform();
        }

        public override void DrawQuickLaunchIcon(QuickLaunchIcon icon, Graphics g, MouseState state)
        {
            Rectangle rectangle = new(0, (icon.Height / 2) - (icon.Width / 2), icon.Width, icon.Width);

            if (state == MouseState.Pressed)
            {
                ControlPaint.DrawBorder3D(g, rectangle, Border3DStyle.SunkenOuter);
            }
            else if (state == MouseState.Hover)
            {
                ControlPaint.DrawBorder3D(g, rectangle, Border3DStyle.RaisedInner);
            }
        }

        public override Point GetQuickLaunchIconLocation(int index) => new(16 + (index * (16 + Config.Default.Tweaks.SpaceBetweenQuickLaunchIcons)), 7);

        public override int GetQuickLaunchWidth(int iconCount) => (iconCount * 16) + (Config.Default.Tweaks.SpaceBetweenQuickLaunchIcons * (iconCount - 1));

        public override Point GetSystemTrayIconLocation(int index) => new(3 + (index * (16 + Config.Default.Tweaks.SpaceBetweenTrayIcons)), 7);

        public override int GetSystemTrayWidth(int iconCount) => 63 + (iconCount * (16 + Config.Default.Tweaks.SpaceBetweenTrayIcons));

        public override Point GetTaskButtonGroupWindowButtonLocation(int index) => new(4, (index - 1) * 24);

        public override Size GetTaskButtonGroupWindowSize(int buttonCount) => new(Config.Default.Tweaks.TaskbarProgramWidth + 8, ((buttonCount - 1) * 24) + 6);
    }
}