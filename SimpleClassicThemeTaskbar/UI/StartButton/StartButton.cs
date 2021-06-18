using SimpleClassicThemeTaskbar.Helpers;

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.StartButton
{
    public partial class StartButton : UserControl
    {
        public const uint DFC_BUTTON = 4;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_PUSHED = 512;

        private bool OpeningStartMenu = false;
        private bool Do3DBorder = true;
        private bool isButton = true;
        private bool pressed = false;
        private bool WasPressed = false;

        public Border3DStyle style = Border3DStyle.Raised;

        public StartButton()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseClick += OnMouseClick;

            Do3DBorder = true;

            Appearance = Config.Instance.StartButtonAppearance;
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

        public MouseState MouseState
        {
            get
            {
                if (Pressed)
                {
                    return MouseState.Pressed;
                }

                if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
                {
                    return MouseState.Hover;
                }

                return MouseState.Normal;
            }
        }

        public bool Pressed
        {
            get
            {
                return pressed;
            }
            set
            {
                if (value && ApplicationEntryPoint.TrayNotificationService != null)
                {
                    ApplicationEntryPoint.TrayNotificationService.RegainTrayPriority();
                }
                pressed = value;
                style = pressed ? Border3DStyle.Sunken : Border3DStyle.Raised;
                Invalidate();
            }
        }

        public void DummySettings(string buttonImagePath, string iconImagePath, StartButtonAppearance mode)
        {
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
                if (fgWnd.ClassName == "OpenShell.CMenuContainer" || fgWnd.ClassName == "Windows.UI.Core.CoreWindow")
                    OpeningStartMenu = false;
                else if (fgWnd.ClassName != "OpenShell.CMenuContainer" && fgWnd.ClassName != "Windows.UI.Core.CoreWindow" && fgWnd.Handle != Parent.Handle)
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
            Config.Instance.Renderer.DrawStartButton(this, e.Graphics);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
        }
    }
}