using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;
using SimpleClassicThemeTaskbar.UIElements.Misc;
using SimpleClassicThemeTaskbar.UIElements.SystemTray;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar.UIElements.StartButton
{
    public partial class StartButton : UserControl
    {
        public const uint DFC_BUTTON = 4;

        public const uint DFCS_BUTTONPUSH = 0x10;

        public const uint DFCS_PUSHED = 512;

        public bool Do3DBorder = true;

        public bool isButton = true;

        public bool pressed = false;

        public Border3DStyle style = Border3DStyle.Raised;

        public bool WasPressed = false;

        private string buttonImageFile = "";

        // private readonly DateTime LastPress = DateTime.Now;
        private bool CustomButton = false;

        private bool CustomIcon = false;
        private string iconImageFile = "";
        private bool OpeningStartMenu = false;

        public StartButton()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseClick += OnMouseClick;

            Do3DBorder = true;

            buttonImageFile = Config.StartButtonImage;
            iconImageFile = Config.StartButtonIconImage;
            Appearance = Config.StartButtonAppearance;
        }

        [Category("Appearance")]
        [DefaultValue(StartButtonAppearance.Default)]
        public StartButtonAppearance Appearance { get; set; } = StartButtonAppearance.Default;

        [Description("Defines the style to be used when drawing")]
        [Category("Appearance")]
        public new Border3DStyle BorderStyle
        {
            get => style;
            set
            {
                style = value;
                Invalidate();
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("Indicates if the control is to be used for example only"), Category("Behavior")]
        public bool Dummy { get; set; } = false;

        public bool Pressed
        {
            get
            {
                return pressed;
            }
            set
            {
                pressed = value;
                style = pressed ? Border3DStyle.Sunken : Border3DStyle.Raised;
                Invalidate();
            }
        }

        public void DummySettings(string buttonImagePath, string iconImagePath, StartButtonAppearance mode)
        {
            buttonImageFile = buttonImagePath;
            iconImageFile = iconImagePath;
            Appearance = mode;

            Invalidate();
        }

        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (Dummy)
            {
                pressed = !pressed;
                style = pressed ? Border3DStyle.Sunken : Border3DStyle.Raised;
                Invalidate();
                return;
            }

            //Gets the state of the button when the mouse went down
            //This ensures the menu doesnt re-open if the button was held down longer then a milisecond
            if (WasPressed)
            {
                WasPressed = false;
                return;
            }

            // HWND wnd = User32.FindWindowW("Shell_TrayWnd", "");
            if (e.Button == MouseButtons.Right)
            {
                Keyboard.KeyPress(Keys.LWin, Keys.X);
                //SystemTrayIcon.PostMessage(wnd, WIN32.WM_RBUTTONDOWN, 0x0002, 0);
                //SystemTrayIcon.SendMessage(wnd, WIN32.WM_RBUTTONUP, 0x0000, 0);
            }
            else
            {
                Window d = new(Taskbar.lastOpenWindow);
                if (d.ClassName == "OpenShell.CMenuContainer" || d.ClassName == "Windows.UI.Core.CoreWindow")
                {
                    Taskbar.lastOpenWindow = Parent.Handle;
                    Pressed = false;
                }
                else
                {
                    OpeningStartMenu = true;
                    Pressed = true;

                    Keyboard.KeyPress(Keys.LWin);
                }
            }
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            WasPressed = Pressed;
            Pressed = true;
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            WasPressed = Pressed;
        }

        public void UpdateState(Window fgWnd)
        {
            bool mouseIsDown = ClientRectangle.Contains(PointToClient(MousePosition)) && (MouseButtons & MouseButtons.Left) != 0;
            if (mouseIsDown)
                return;
            if (OpeningStartMenu)
            {
                if (fgWnd.ClassName != "OpenShell.CMenuContainer" && fgWnd.ClassName != "Windows.UI.Core.CoreWindow" && fgWnd.Handle != Parent.Handle)
                {
                    OpeningStartMenu = false;
                }
                return;
            }
            else
            {
                Pressed = fgWnd.ClassName == "OpenShell.CMenuContainer" ||
                          fgWnd.ClassName == "Windows.UI.Core.CoreWindow";
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Config.Renderer.DrawStartButton(this, e.Graphics);
            return;

            // e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            //
            // Image temp = null;
            //
            // switch (Appearance)
            // {
            //     case StartButtonAppearance.Default:
            //         temp = Properties.Resources.startIcon95;
            //         break;
            //
            //     case StartButtonAppearance.CustomIcon:
            //         try
            //         {
            //             temp = Image.FromFile(iconImageFile);
            //         }
            //         catch
            //         {
            //             e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
            //             return;
            //         }
            //         if (temp.Width != 16 || temp.Height != 16)
            //         {
            //             temp.Dispose();
            //             temp = Properties.Resources.startIcon95;
            //         }
            //         break;
            //
            //     case StartButtonAppearance.CustomButton:
            //         try
            //         {
            //             temp = Image.FromFile(imageFile);
            //         }
            //         catch
            //         {
            //             e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
            //             return;
            //         }
            //         if (temp.Height != 66)
            //         {
            //             e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
            //             temp.Dispose();
            //             return;
            //         }
            //         if (Width != temp.Width)
            //         {
            //             Width = temp.Width;
            //             Invalidate();
            //         }
            //         e.Graphics.DrawImage(temp, new Rectangle(0, 0, temp.Width, 22), new Rectangle(0, pressed ? 44 : 0, temp.Width, 22),  GraphicsUnit.Pixel);
            //         return;
            // }
            //
            // if (Width != 55)
            // {
            //     Width = 55;
            //     Invalidate();
            // }
            //
            // if (Do3DBorder)
            // {
            //     if (isButton)
            //     {
            //         RECT rect = new(ClientRectangle);
            //         uint buttonStyle = style == Border3DStyle.Raised ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
            //         _ = User32.DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
            //         e.Graphics.ReleaseHdc();
            //     }
            //     else
            //     {
            //         ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, style);
            //     }
            // }
            // else
            // {
            //     ButtonBorderStyle bStyle =
            //         style == Border3DStyle.Raised ? ButtonBorderStyle.Outset : ButtonBorderStyle.Inset;
            //     ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
            //         SystemColors.Control, 1, bStyle,
            //         SystemColors.Control, 1, bStyle,
            //         SystemColors.Control, 2, bStyle,
            //         SystemColors.Control, 2, bStyle);
            // }
            // if (BackgroundImage != null)
            // {
            //     using (TextureBrush brush = new(BackgroundImage, WrapMode.Tile))
            //     {
            //         e.Graphics.FillRectangle(brush, Rectangle.Inflate(ClientRectangle, -2, -2));
            //     }
            // }
            //
            // bool mouseIsDown = ClientRectangle.Contains(PointToClient(MousePosition)) && (MouseButtons & MouseButtons.Left) != 0;
            // e.Graphics.DrawImage(temp ?? Properties.Resources.startIcon95, mouseIsDown ? new Point(5, 4) : new Point(4, 3));
            // e.Graphics.DrawString("Start", new Font("Tahoma", 8F, FontStyle.Bold), SystemBrushes.ControlText, mouseIsDown ? new PointF(21F, 5F) :  new PointF(20F, 4F));
            //
            // temp.Dispose();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
        }
    }
}