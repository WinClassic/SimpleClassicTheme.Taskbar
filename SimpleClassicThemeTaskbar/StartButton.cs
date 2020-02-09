using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    public partial class StartButton : UserControl
    {
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
        }

        //Absolutely terribly way to do it.
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                HWND wnd = Taskbar.FindWindowW("Progman", "Program Manager");
                Taskbar.SendMessage(wnd, 0x0204, 0x0002, (-1 << 15) + -1);
                Taskbar.SendMessage(wnd, 0x0205, 0x0000, (-1 << 15) + -1);
            }
            else
            {
                Window wnd = new Window(Taskbar.lastOpenWindow);
                if (wnd.ClassName == "OpenShell.CMenuContainer" || wnd.ClassName == "Windows.UI.Core.CoreWindow")
                {
                    Taskbar.lastOpenWindow = Parent.Handle;
                    Pressed = false;
                }
                else
                {
                    SendKeys.Send("^{ESC}");
                }
            }
        }
    }
}
