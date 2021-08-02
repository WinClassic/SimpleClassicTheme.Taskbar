using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static SimpleClassicThemeTaskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicThemeTaskbar
{
    public partial class PopupTaskbarGroup : Form
    {
        public const uint DFC_BUTTON = 4;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_PUSHED = 512;

        public const uint WA_INACTIVE = 0x0000;
        public const uint WM_ACTIVATE = 0x0006;
        public GroupedTaskbarProgram parent;

        public PopupTaskbarGroup(GroupedTaskbarProgram origin)
        {
            InitializeComponent();
            parent = origin;
        }

        //Make sure form doesnt show in alt tab and that it shows up on all virtual desktops
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        public void Show(Point buttonScreenCoordinates)
        {
            if (parent.ProgramWindows.Count < 2)
                return;
            Controls.Clear();
            StartPosition = FormStartPosition.Manual;
            //Size = new Size(Config.Default.TaskbarProgramWidth + 8, ((parent.ProgramWindows.Count - 1) * 24) + 6);
            Size = Config.Default.Renderer.GetTaskButtonGroupWindowSize(parent.ProgramWindows.Count);
            Location = new Point(buttonScreenCoordinates.X + (parent.Width / 2) - (Width / 2), buttonScreenCoordinates.Y - Height);
            for (int i = 1; i < parent.ProgramWindows.Count; i++)
            {
                SingleTaskbarProgram program = parent.ProgramWindows[i];
                program.Parent = this;
                program.Location = Config.Default.Renderer.GetTaskButtonGroupWindowButtonLocation(i);
                program.Width = Config.Default.TaskbarProgramWidth;
                program.Visible = true;
                //program.MouseClick += delegate (object sender, MouseEventArgs e) {  };
            }
            Show();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ACTIVATE && m.WParam == new IntPtr(WA_INACTIVE))
            {
                Hide();
            }
            base.WndProc(ref m);
        }

        private void PopupTaskbarGroup_Paint(object sender, PaintEventArgs e)
        {
            Config.Default.Renderer.DrawTaskButtonGroupWindow(this, e.Graphics);
            return;

            Rectangle newRect = ClientRectangle;
            RECT rect = newRect;
            uint buttonStyle = DFCS_BUTTONPUSH;
            _ = User32.DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
            e.Graphics.ReleaseHdc();
            e.Graphics.ResetTransform();
        }
    }
}