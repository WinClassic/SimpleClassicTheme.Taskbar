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

namespace SimpleClassicTheme.Taskbar.UIElements.QuickLaunch
{
    public partial class QuickLaunchIcon : PictureBox
    {
        private IntPtr iconHandle { get; set; }

        public string FileName = "";
        public IntPtr IconHandle { get { return iconHandle; } set { iconHandle = value; Image = Icon.FromHandle(value).ToBitmap(); } }

        public bool IsMoving = false;
        public new MouseEventHandler MouseDown;
        public new MouseEventHandler MouseUp;
        public new MouseEventHandler MouseMove;

        public QuickLaunchIcon()
        {
            InitializeComponent();

            SizeMode = PictureBoxSizeMode.CenterImage;

            MouseDown += delegate (object sender, MouseEventArgs e) { _ = (MouseDown?.DynamicInvoke(this, e)); };
            MouseUp += delegate (object sender, MouseEventArgs e) { _ = (MouseUp?.DynamicInvoke(this, e)); };
            MouseMove += delegate (object sender, MouseEventArgs e) { _ = (MouseMove?.DynamicInvoke(this, e)); };

            DragEnter += QuickLaunchIcon_DragEnter;
            DragDrop += QuickLaunchIcon_DragDrop;

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
    }
}
