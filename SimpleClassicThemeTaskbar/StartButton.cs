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
        private bool pressed = false;

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

                if (value)
                    SendKeys.Send("^{ESC}");
            }
        }

        public StartButton()
        {
            InitializeComponent();
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
            //Add event handlers
            pictureBox1.Click += delegate { OnClick(new EventArgs()); };
            label1.Click += delegate { OnClick(new EventArgs()); };

            //Absolutely terribly way to do it.
            //TODO: Check if start menu is open or not
            Click += delegate { Pressed = !Pressed; };
        }
    }
}
