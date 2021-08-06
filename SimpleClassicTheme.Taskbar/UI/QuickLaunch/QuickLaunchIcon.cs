using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using static System.Windows.Forms.AxHost;

namespace SimpleClassicTheme.Taskbar.UIElements.QuickLaunch
{
    public partial class QuickLaunchIcon : Control, IHasMouseState
    {
        private IntPtr _iconHandle;

        public string FileName { get; set; } = string.Empty;

        public IntPtr IconHandle
        {
            get => _iconHandle;
            set
            {
                _iconHandle = value;
                Image = Icon.FromHandle(value).ToBitmap();
            }
        }

        public bool IsMoving = false;
        public bool IsPressed { get; set; }
        public bool IsHovered { get; set; }

        public Image Image { get; set; }

        public new MouseEventHandler MouseDown;
        public new MouseEventHandler MouseUp;
        public new MouseEventHandler MouseMove;

        public QuickLaunchIcon()
        {
            InitializeComponent();

            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;

            MouseDown += delegate (object sender, MouseEventArgs e) { _ = (MouseDown?.DynamicInvoke(this, e)); };
            MouseUp += delegate (object sender, MouseEventArgs e) { _ = (MouseUp?.DynamicInvoke(this, e)); };
            MouseMove += delegate (object sender, MouseEventArgs e) { _ = (MouseMove?.DynamicInvoke(this, e)); };

            DragEnter += QuickLaunchIcon_DragEnter;
            DragDrop += QuickLaunchIcon_DragDrop;

            AllowDrop = true;

            MouseHandler.SubscribeToMouseEvents(this);
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

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            var mouse = (IHasMouseState)this;

            if (mouse.MouseState != MouseState.Normal)
            {
                // var rectangle = new Rectangle(Point.Empty, Size);
                Config.Default.Renderer.DrawQuickLaunchIcon(this, pevent.Graphics, mouse.MouseState);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int iconSize = 16;

            Rectangle rectangle = new(
                (Width / 2) - (iconSize / 2),
                (Height / 2) - (iconSize / 2),
                iconSize, iconSize );

            e.Graphics.DrawImage(Image, rectangle);
        }
    }
}
