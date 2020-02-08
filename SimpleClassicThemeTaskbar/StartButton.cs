using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            pictureBox1.Click += delegate { OnMouseClick(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)); };
            label1.Click += delegate { OnMouseClick(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)); };

            //Absolutely terribly way to do it.
            //TODO: Check if start menu is open or not
            MouseClick += delegate(object senderr, MouseEventArgs ee)
            {
                if (ee.Button == MouseButtons.Right)
                    return;
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
            };
        }
    }
}
