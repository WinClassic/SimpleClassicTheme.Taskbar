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
using System.Diagnostics;

namespace SimpleClassicThemeTaskbar.UIElements.QuickLaunch
{
    public partial class QuickLaunchIcon : UserControl
    {
        public string FileName = "";
        public Image Image { get { return pictureBox1.Image; } set { pictureBox1.Image = value; } }

        public bool IsMoving = false;
        public new MouseEventHandler MouseDown;
        public new MouseEventHandler MouseUp;
        public new MouseEventHandler MouseMove;

        public QuickLaunchIcon()
        {
            InitializeComponent();

            base.MouseDown += delegate (object sender, MouseEventArgs e) { _ = (MouseDown?.DynamicInvoke(this, e)); };
            pictureBox1.MouseDown += delegate (object sender, MouseEventArgs e) { _ = (MouseDown?.DynamicInvoke(this, e)); };
            base.MouseUp += delegate (object sender, MouseEventArgs e) { _ = (MouseUp?.DynamicInvoke(this, e)); };
            pictureBox1.MouseUp += delegate (object sender, MouseEventArgs e) { _ = (MouseUp?.DynamicInvoke(this, e)); };
            base.MouseMove += delegate (object sender, MouseEventArgs e) { _ = (MouseMove?.DynamicInvoke(this, e)); };
            pictureBox1.MouseMove += delegate (object sender, MouseEventArgs e) { _ = (MouseMove?.DynamicInvoke(this, e)); };

            DragEnter += QuickLaunchIcon_DragEnter;
            pictureBox1.DragEnter += QuickLaunchIcon_DragEnter;
            DragDrop += QuickLaunchIcon_DragDrop;
            pictureBox1.DragDrop += QuickLaunchIcon_DragDrop;

            AllowDrop = true;
        }

        private void QuickLaunchIcon_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        private void QuickLaunchIcon_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) _ = Process.Start(FileName, $"\"{file}\"");
        }

        private void QuickLaunchIcon_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (IsMoving)
                IsMoving = false;
            else
                OnClick(new EventArgs());
        }
    }
}
