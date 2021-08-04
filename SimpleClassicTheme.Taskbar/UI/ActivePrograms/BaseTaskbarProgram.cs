using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar
{
    public abstract partial class BaseTaskbarProgram : UserControlEx
    {
        public const uint DFC_BUTTON = 4;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_PUSHED = 512;

        //Movemement and events
        public bool IsMoving = false;

        public new MouseEventHandler MouseDown;

        public new MouseEventHandler MouseMove;

        public new MouseEventHandler MouseUp;

        private bool activeWindow = false;
        private Border3DStyle style;
        private bool textIndent = false;

        public bool ActiveWindow
        {
            get
            {
                return activeWindow;
            }
            set
            {
                activeWindow = value;

                try
                {
                    if (Parent != null && Parent.Visible && ClientRectangle.Contains(PointToClient(MousePosition)))
                    {
                        if ((MouseButtons & MouseButtons.Left) != 0)
                        {
                            //Don't update if the mouse is held down on this control
                            return;
                        }
                    }
                }
                catch { }

                if (activeWindow)
                {
                    style = Border3DStyle.Sunken;
                    IsPressed = true;
                    GetFont = new Font(GetFont.FontFamily, GetFont.Size, FontStyle.Bold, GraphicsUnit.Point);
                }
                else
                {
                    style = Border3DStyle.Raised;
                    IsPressed = false;
                    GetFont = new Font(GetFont.FontFamily, GetFont.Size, FontStyle.Regular, GraphicsUnit.Point);
                }

                Invalidate();
            }
        }

        public Font GetFont { get; private set; }

        public abstract Icon Icon { get; set; }

        public abstract Image IconImage { get; }

        /// <summary>
        /// This value is true if the user is holding down the mouse on the button
        /// </summary>
        public bool IsPressed { get; private set; } = false;

        /// <summary>
        /// This value is true if the task is selected
        /// </summary>
        public bool IsPushed { get => style != Border3DStyle.Raised; }

        public abstract int MinimumWidth { get; }

        //Abstract properties
        public abstract Process Process { get; set; }

        /// <summary>
        /// This value indicates how much space in needed for extra drawing
        /// </summary>
        public int SpaceNeededNextToText { get; protected internal set; }

        public abstract string Title { get; set; }

        public abstract Window Window { get; set; }

        public void Constructor()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;

            BackColor = Color.Transparent;
            GetFont = new Font("Tahoma", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);

            base.MouseDown += delegate (object sender, MouseEventArgs e) { _ = (MouseDown?.DynamicInvoke(this, e)); };
            base.MouseUp += delegate (object sender, MouseEventArgs e) { _ = (MouseUp?.DynamicInvoke(this, e)); };
            base.MouseMove += delegate (object sender, MouseEventArgs e) { _ = (MouseMove?.DynamicInvoke(this, e)); };

            MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (CancelMouseDown(e))
                    return;
                style = Border3DStyle.Sunken;
                GetFont = new Font(GetFont.FontFamily, GetFont.Size, FontStyle.Bold, GraphicsUnit.Point);
                IsPressed = true;
                textIndent = true;
                Invalidate();
            };
            MouseUp += delegate
            {
                textIndent = false;
                Invalidate();
            };
            MouseClick += OnClick;
            MouseDoubleClick += OnDoubleClick;

            ClientSizeChanged += delegate { if (Height == 24) Logger.Log(LoggerVerbosity.Verbose, "BaseTaskbarProgram", $"Size changed to {ClientRectangle}"); };
        }

        public abstract void FinishOnPaint(PaintEventArgs e);

        //Abstract functions
        public abstract bool IsActiveWindow(IntPtr activeWindow);

        public abstract void OnClick(object sender, MouseEventArgs e);
        public abstract void OnDoubleClick(object sender, MouseEventArgs e);

        public void OnPaint(object sender, PaintEventArgs e)
        {
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "painting base window";
            if (Erroring)
            {
                DrawError(e.Graphics);
                return;
            }

            Config.Default.Renderer.DrawTaskButton(this, e.Graphics);

            FinishOnPaint(e);
        }

        protected abstract bool CancelMouseDown(MouseEventArgs e);

        private void BaseTaskbarProgram_Load(object sender, EventArgs e)
        {
        }
    }
}