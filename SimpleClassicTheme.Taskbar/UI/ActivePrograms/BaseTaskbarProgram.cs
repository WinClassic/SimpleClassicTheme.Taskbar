using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Diagnostics;
using System.Drawing;
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
        private Icon icon;

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
                }
                else
                {
                    style = Border3DStyle.Raised;
                    IsPressed = false;
                }

                Logger.Instance.Log(LoggerVerbosity.Verbose, "BaseTaskbarProgram", $"Tried to set ActiveWindow to {value}: {new StackTrace()}");

                Invalidate();
            }
        }

        public virtual Icon Icon
        {
            get => icon;
            set
            {
                icon = value;

                IconImage = null;

                try
                {
                    if (icon != null)
                    {
                        IconImage = new Icon(icon, 16, 16).ToBitmap();
                    }
                }
                catch
                {
                    Logger.Instance.Log(LoggerVerbosity.Verbose, "TaskbarProgram/Icon", "Failed to set icon");
                }
            }
        }

        public virtual Image IconImage { get; protected set; }

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

            base.MouseDown += delegate (object sender, MouseEventArgs e) { _ = (MouseDown?.DynamicInvoke(this, e)); };
            base.MouseUp += delegate (object sender, MouseEventArgs e) { _ = (MouseUp?.DynamicInvoke(this, e)); };
            base.MouseMove += delegate (object sender, MouseEventArgs e) { _ = (MouseMove?.DynamicInvoke(this, e)); };

            MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (CancelMouseDown(e))
                    return;

                style = Border3DStyle.Sunken;
                IsPressed = true;
                Invalidate();
            };
            MouseUp += delegate
            {
                Invalidate();
            };
            MouseClick += OnClick;
            MouseDoubleClick += OnDoubleClick;

            ClientSizeChanged += delegate { if (Height == 24) Logger.Instance.Log(LoggerVerbosity.Verbose, "BaseTaskbarProgram", $"Size changed to {ClientRectangle}"); };
        }

        public abstract void FinishOnPaint(PaintEventArgs e);
        public abstract void OnClick(object sender, MouseEventArgs e);
        public abstract void OnDoubleClick(object sender, MouseEventArgs e);
        protected abstract bool CancelMouseDown(MouseEventArgs e);

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

        public virtual bool IsWindow(IntPtr hWnd)
        {
            return Window.Handle.Equals(hWnd);
        }


        private void BaseTaskbarProgram_Load(object sender, EventArgs e)
        {
        }
    }
}