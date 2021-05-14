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
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("Indicates if the control is to be used for example only"), Category("Behavior")]
        public bool Dummy { get { return dummy; } set { dummy = value; } }
        private bool dummy = false;
        public bool WasPressed = false;
        public bool pressed = false;

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

        [DllImport("user32.dll")]
        public static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

        public const uint DFC_BUTTON = 4;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_PUSHED = 512;

        public bool Do3DBorder = true;
        public bool isButton = true;

        private string imageFile = "";
        private bool CustomIcon = false;
        private bool CustomButton = false;
        private bool OpeningStartMenu = false;

        private DateTime LastPress = DateTime.Now;

        public void DummySettings(string image, bool customIcon, bool customButton)
        {
            imageFile = image;
            CustomIcon = customIcon;
            CustomButton = customButton;

            Invalidate();
        }

        public StartButton()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseClick += OnMouseClick;

            Do3DBorder = true;

            imageFile = Config.StartButtonImage;
            CustomIcon = Config.StartButtonCustomIcon;
            CustomButton = Config.StartButtonCustomButton;
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

        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (dummy)
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
            HWND wnd = Taskbar.FindWindowW("Shell_TrayWnd", "");
            if (e.Button == MouseButtons.Right)
            {
                Keyboard.KeyDown(Keys.LWin);
                Keyboard.KeyDown(Keys.X);
                Keyboard.KeyUp(Keys.X);
                Keyboard.KeyUp(Keys.LWin);
                //SystemTrayIcon.PostMessage(wnd, WIN32.WM_RBUTTONDOWN, 0x0002, 0);
                //SystemTrayIcon.SendMessage(wnd, WIN32.WM_RBUTTONUP, 0x0000, 0);
            }
            else
            {
                Window d = new Window(Taskbar.lastOpenWindow);
                if (d.ClassName == "OpenShell.CMenuContainer" || d.ClassName == "Windows.UI.Core.CoreWindow")
                {
                    Taskbar.lastOpenWindow = Parent.Handle;
                    Pressed = false;
                }
                else
                {
                    OpeningStartMenu = true;
                    Pressed = true;
                    Keyboard.KeyDown(Keys.LWin);
                    Keyboard.KeyUp(Keys.LWin);
                }
            }
        }

        public Border3DStyle style = Border3DStyle.Raised;

        [Description("Defines the style to be used when drawing ")]
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

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Config.Renderer.DrawStartButton(this, e.Graphics);
            return;

            e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            Image temp = null;
            if (!CustomIcon && !CustomButton)
            {
                temp = Properties.Resources.startIcon95;
            }
            else if (CustomIcon)
			{
                try
                {
                    temp = Image.FromFile(imageFile);
                }
                catch
                {
                    e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
                    return;
                }
                if (temp.Width != 16 || temp.Height != 16)
				{
                    temp.Dispose();
                    temp = Properties.Resources.startIcon95;
                }
            }
            else if (CustomButton)
			{
                try
				{
                    temp = Image.FromFile(imageFile);
				}
				catch
				{
                    e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
                    return;
				}
                if (temp.Height != 66)
				{
                    e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
                    temp.Dispose();
                    return;
                }
                if (Width != temp.Width)
                {
                    Width = temp.Width;
                    Invalidate();
                }
                e.Graphics.DrawImage(temp, new Rectangle(0, 0, temp.Width, 22), new Rectangle(0, pressed ? 44 : 0, temp.Width, 22), GraphicsUnit.Pixel);
                return;
			}

            if (Width != 55)
			{
                Width = 55;
                Invalidate();
			}

            if (Do3DBorder)
            {
                if (isButton)
                {
                    RECT rect = new RECT(ClientRectangle);
                    uint buttonStyle = style == Border3DStyle.Raised ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
                    DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
                    e.Graphics.ReleaseHdc();
                }
                else
                {
                    ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, style);
                }
            }
            else
            {
                ButtonBorderStyle bStyle =
                    style == Border3DStyle.Raised ? ButtonBorderStyle.Outset : ButtonBorderStyle.Inset;
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                    SystemColors.Control, 1, bStyle,
                    SystemColors.Control, 1, bStyle,
                    SystemColors.Control, 2, bStyle,
                    SystemColors.Control, 2, bStyle);
            }
            if (BackgroundImage != null)
            {
                using (TextureBrush brush = new TextureBrush(BackgroundImage, WrapMode.Tile))
                {
                    e.Graphics.FillRectangle(brush, Rectangle.Inflate(ClientRectangle, -2, -2));
                }
            }

            bool mouseIsDown = ClientRectangle.Contains(PointToClient(MousePosition)) && (MouseButtons & MouseButtons.Left) != 0;
            e.Graphics.DrawImage(temp != null ? temp : Properties.Resources.startIcon95, mouseIsDown ? new Point(5, 4) : new Point(4, 3));
            e.Graphics.DrawString("Start", new Font("Tahoma", 8F, FontStyle.Bold), SystemBrushes.ControlText, mouseIsDown ? new PointF(21F, 5F) : new PointF(20F, 4F));
           
            temp.Dispose();
        }

		private void StartButton_Load(object sender, EventArgs e)
		{

		}

		private void StartButton_Click(object sender, EventArgs e)
		{

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
	}
}
