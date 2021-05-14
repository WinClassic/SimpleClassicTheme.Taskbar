using SimpleClassicThemeTaskbar.Helpers;

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

namespace SimpleClassicThemeTaskbar
{
    public abstract partial class BaseTaskbarProgram : UserControlEx
	{
		public const uint DFC_BUTTON = 4;
		public const uint DFCS_BUTTONPUSH = 0x10;
		public const uint DFCS_PUSHED = 512;

        //Drawing information
        private Font textFont;
        private bool textIndent = false;
        private bool drawBackground = false;
        private Border3DStyle style;
        private bool activeWindow = false;

        //Movemement and events
        public bool IsMoving = false;
        public new MouseEventHandler MouseDown;
        public new MouseEventHandler MouseUp;
        public new MouseEventHandler MouseMove;

        //Abstract functions
        public abstract bool IsActiveWindow(IntPtr activeWindow);
        public abstract void OnClick(object sender, MouseEventArgs e);
        public abstract void FinishOnPaint(PaintEventArgs e);
        protected abstract bool CancelMouseDown(MouseEventArgs e);

        //Abstract properties
        public abstract Process Process { get; set; }
        public abstract Window Window { get; set; }
        public abstract Icon Icon { get; set; }
        public abstract Image IconImage { get; }
        public abstract string Title { get; set; }
        public abstract int MinimumWidth { get; }

        /// <summary>
        /// This value is true if the task is selected
        /// </summary>
        public bool IsPushed { get => style != Border3DStyle.Raised; }
        /// <summary>
        /// This value is true if the user is holding down the mouse on the button
        /// </summary>
        public bool IsPressed { get => drawBackground; }
        /// <summary>
        /// This value indicates how much space in needed for extra drawing
        /// </summary>
        public int SpaceNeededNextToText { get; protected internal set; }

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
                    drawBackground = true;
                    textFont = new Font(textFont.FontFamily, textFont.Size, FontStyle.Bold, GraphicsUnit.Point);
                }
                else
                {
                    style = Border3DStyle.Raised;
                    drawBackground = false;
                    textFont = new Font(textFont.FontFamily, textFont.Size, FontStyle.Regular, GraphicsUnit.Point);
                }

                Invalidate();
            }
        }

        public void Constructor()
		{
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;

            BackColor = Color.Transparent;
            textFont = new Font("Tahoma", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);

            base.MouseDown += delegate (object sender, MouseEventArgs e) { MouseDown?.DynamicInvoke(this, e); };
            base.MouseUp += delegate (object sender, MouseEventArgs e) { MouseUp?.DynamicInvoke(this, e); };
            base.MouseMove += delegate (object sender, MouseEventArgs e) { MouseMove?.DynamicInvoke(this, e); };

            MouseDown += delegate (object sender, MouseEventArgs e) {
                if (CancelMouseDown(e))
                    return;
                style = Border3DStyle.Sunken;
                textFont = new Font(textFont.FontFamily, textFont.Size, FontStyle.Bold, GraphicsUnit.Point);
                drawBackground = true;
                textIndent = true;
                Invalidate();
            };
            MouseUp += delegate
            {
                textIndent = false;
                Invalidate();
            };
            MouseClick += OnClick;
        }

        public Font GetFont => textFont;

		private void BaseTaskbarProgram_Load(object sender, EventArgs e)
		{
			
		}

        public void OnPaint(object sender, PaintEventArgs e)
        {
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "painting base window";
            if (Erroring)
			{
                DrawError(e.Graphics);
                return;
            }

            Config.Renderer.DrawTaskButton(this, e.Graphics);

            FinishOnPaint(e);
        }
    }
}
