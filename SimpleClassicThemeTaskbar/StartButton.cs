using System;
using System.Windows.Forms;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    public partial class StartButton : UserControl
    {

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
                panel1.Do3DBorder = false;
                panel1.style = pressed ? Border3DStyle.Sunken : Border3DStyle.Raised;
                panel1.Invalidate();
            }
        }

        public StartButton()
        {
            InitializeComponent();
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
            //Add event handlers
            pictureBox1.MouseClick += OnMouseClick;
            label1.MouseClick += OnMouseClick;
            MouseClick += OnMouseClick;

            pictureBox1.MouseDown += OnMouseDown;
            label1.MouseDown += OnMouseDown;
            MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            WasPressed = Pressed;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
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
                SystemTrayIcon.PostMessage(wnd, WIN32.WM_RBUTTONDOWN, 0x0002, 0);
                SystemTrayIcon.SendMessage(wnd, WIN32.WM_RBUTTONUP, 0x0000, 0);
            }
            else
            {
                Window d = new Window(Taskbar.lastOpenWindow);
                if (pressed)
                {
                    Taskbar.lastOpenWindow = Parent.Handle;
                    Pressed = false;
                }
                else if (d.ClassName != "OpenShell.CMenuContainer" && d.ClassName != "Windows.UI.Core.CoreWindow")
                {
                    Keyboard.KeyDown(Keys.LWin);
                    Keyboard.KeyUp(Keys.LWin);
                }
            }
        }
    }
}
