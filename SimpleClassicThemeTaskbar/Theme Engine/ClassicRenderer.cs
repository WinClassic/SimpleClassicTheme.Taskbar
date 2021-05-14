using SimpleClassicThemeTaskbar.UIElements.StartButton;
using SimpleClassicThemeTaskbar.UIElements.SystemTray;
using SimpleClassicThemeTaskbar.UIElements.QuickLaunch;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

namespace SimpleClassicThemeTaskbar.Theme_Engine
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
                if (Config.StartButtonCustomButton)
                    try
                    {
                        return Image.FromFile(Config.StartButtonImage).Width;
                    }
                    catch { }
                return 55;
            }
        }

        public override Color SystemTrayTimeColor => SystemColors.ControlText;
        public override Font SystemTrayTimeFont => SystemFonts.DefaultFont;
        public override Point SystemTrayTimeLocation => new(-52, 9);
        public override int TaskbarHeight => 28;

        public override void DrawQuickLaunch(QuickLaunch systemTray, Graphics g)
        {
        }

        public override void DrawStartButton(StartButton startButton, Graphics g)
        {
            // Define constants
            const uint DFC_BUTTON = 4;
            const uint DFCS_BUTTONPUSH = 0x10;
            const uint DFCS_PUSHED = 512;

            try
            {
                Image icon = Properties.Resources.startIcon95;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                if (Config.StartButtonCustomButton)
                {
                    Image image = Image.FromFile(Config.StartButtonImage);
                    if (image.Height != 66)
                        throw new InvalidOperationException();
                    if (startButton.Width != image.Width)
                        startButton.Width = image.Width;
                    g.DrawImage(image, new Rectangle(2, 4, image.Width, 22), new Rectangle(0, startButton.Pressed ? 44 : startButton.ClientRectangle.Contains(startButton.PointToClient(Control.MousePosition)) ? 22 : 0, image.Width, 22), GraphicsUnit.Pixel);
                    return;
                }
                else if (Config.StartButtonCustomIcon)
                {
                    icon = Image.FromFile(Config.StartButtonImage);
                }
                if (startButton.Width != 57)
                    startButton.Width = 57;
                RECT rect = new(startButton.ClientRectangle);
                rect.Left += 2;
                rect.Top += 4;
                rect.Bottom -= 2;
                uint buttonStyle = !startButton.Pressed ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
                _ = User32.DrawFrameControl(g.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
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

        public override void DrawSystemTray(SystemTray systemTray, Graphics g)
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
            // Define constants

            const uint DFC_BUTTON = 4;
            const uint DFCS_BUTTONPUSH = 0x10;
            const uint DFCS_PUSHED = 512;
            bool isPushed = taskbarProgram.IsPushed;
            bool isPressed = taskbarProgram.IsPressed;

            // Draw button frame
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            Rectangle newRect = taskbarProgram.ClientRectangle;
            newRect.Y += 4;
            newRect.Height -= 6;
            RECT rect = new(newRect);
            uint buttonStyle = taskbarProgram.IsPushed ? DFCS_BUTTONPUSH | DFCS_PUSHED : DFCS_BUTTONPUSH;
            _ = User32.DrawFrameControl(g.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
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
                    g.DrawString(taskbarProgram.Title, font, SystemBrushes.ControlText, isPushed ? new Rectangle(21, 11, taskbarProgram.Width - 21 - 3 - taskbarProgram.SpaceNeededNextToText, 10) : new Rectangle(20, 10, taskbarProgram.Width - 20 - 3 - taskbarProgram.SpaceNeededNextToText, 11), format);
            }
            else
            {
                g.DrawString(taskbarProgram.Title, font, SystemBrushes.ControlText, isPushed ? new Rectangle(5, 11, taskbarProgram.Width - 5 - 3 - taskbarProgram.SpaceNeededNextToText, 10) : new Rectangle(4, 10, taskbarProgram.Width - 4 - 3 - taskbarProgram.SpaceNeededNextToText, 11), format);
            }
        }

        public override void DrawTaskButtonGroupButton(GroupedTaskbarProgram taskbarProgram, Graphics g)
        {
            // Define constants
            const uint DFC_BUTTON = 4;
            const uint DFCS_BUTTONPUSH = 0x10;
            const uint DFCS_PUSHED = 512;

            // Draw button frame
            Rectangle newRect = taskbarProgram.ClientRectangle;
            newRect.X += taskbarProgram.Width - 19;
            newRect.Width -= taskbarProgram.Width - 19 + 3;
            newRect.Y += 7;
            newRect.Height -= 12;
            RECT rect = new(newRect);
            uint buttonStyle = !taskbarProgram.GroupWindow.Visible ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
            _ = Shell32.DrawFrameControl(g.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
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
            g.DrawString(taskbarProgram.ProgramWindows.Count.ToString(), font, SystemBrushes.ControlText, taskbarProgram.GroupWindow.Visible ? new Rectangle(taskbarProgram.Width - 15, 11, 7, 10) : new Rectangle(taskbarProgram.Width - 16, 10, 7, 11), format);
        }

        public override void DrawTaskButtonGroupWindow(PopupTaskbarGroup taskbarGroup, Graphics g)
        {
            // Define constants
            const uint DFC_BUTTON = 4;
            const uint DFCS_BUTTONPUSH = 0x10;

            Rectangle newRect = taskbarGroup.ClientRectangle;
            RECT rect = new(newRect);
            uint buttonStyle = DFCS_BUTTONPUSH;
            _ = User32.DrawFrameControl(g.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
            g.ReleaseHdc();
            g.ResetTransform();
        }

        public override Point GetQuickLaunchIconLocation(int index) => new(16 + (index * (16 + Config.SpaceBetweenQuickLaunchIcons)), 7);

        public override int GetQuickLaunchWidth(int iconCount) => (iconCount * 16) + (Config.SpaceBetweenQuickLaunchIcons * (iconCount - 1));

        public override Point GetSystemTrayIconLocation(int index) => new(3 + (index * (16 + Config.SpaceBetweenTrayIcons)), 7);

        public override int GetSystemTrayWidth(int iconCount) => 63 + (iconCount * (16 + Config.SpaceBetweenTrayIcons));

        public override Point GetTaskButtonGroupWindowButtonLocation(int index) => new(4, (index - 1) * 24);

        public override Size GetTaskButtonGroupWindowSize(int buttonCount) => new(Config.TaskbarProgramWidth + 8, ((buttonCount - 1) * 24) + 6);
    }
}