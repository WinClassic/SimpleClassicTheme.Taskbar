using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace SimpleClassicThemeTaskbar.UIElements.QuickLaunch
{
    public partial class QuickLaunchIcon : UserControl
    {
        public string FileName = "";
        public Image Image { get { return pictureBox1.Image; } set { pictureBox1.Image = value; } }

        public QuickLaunchIcon()
        {
            InitializeComponent();
        }

        private void QuickLaunchIcon_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OnClick(new EventArgs());
        }
    }
}
