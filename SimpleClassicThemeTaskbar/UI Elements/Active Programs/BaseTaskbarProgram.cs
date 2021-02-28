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
        //Win32
		[DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

		public const uint DFC_BUTTON = 4;
		public const uint DFCS_BUTTONPUSH = 0x10;
		public const uint DFCS_PUSHED = 512;

        //Drawing information
        private Font textFont;
        private bool textIndent = false;
        private bool drawBackground = false;
        private Border3DStyle style;
        private Bitmap d;
        private bool activeWindow = false;
        protected internal int SpaceNeededNextToText = 0;

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

            d = new Bitmap(2, 2);
            d.SetPixel(0, 0, SystemColors.Control);
            d.SetPixel(0, 1, SystemColors.ControlLightLight);
            d.SetPixel(1, 1, SystemColors.Control);
            d.SetPixel(1, 0, SystemColors.ControlLightLight);

            BackColor = Color.Transparent;
            textFont = new Font("Tahoma", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);

            base.MouseDown += delegate (object sender, MouseEventArgs e) { MouseDown?.DynamicInvoke(this, e); };
            base.MouseUp += delegate (object sender, MouseEventArgs e) { MouseUp?.DynamicInvoke(this, e); };
            base.MouseMove += delegate (object sender, MouseEventArgs e) { MouseMove?.DynamicInvoke(this, e); };

            MouseDown += delegate (object sender, MouseEventArgs e) {
                if (CancelMouseDown(e))
                    return;
                style = Border3DStyle.Sunken;
                d.SetPixel(0, 0, SystemColors.Control);
                d.SetPixel(0, 1, SystemColors.ControlLightLight);
                d.SetPixel(1, 1, SystemColors.Control);
                d.SetPixel(1, 0, SystemColors.ControlLightLight);
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
        public Label GetLine => line;

		private void BaseTaskbarProgram_Load(object sender, EventArgs e)
		{
			line.Width = Width;
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

            e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            Rectangle newRect = ClientRectangle;
            newRect.Y += 4;
            newRect.Height -= 6;
            RECT rect = new RECT(newRect);
            uint buttonStyle = style == Border3DStyle.Raised ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
            DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
            e.Graphics.ReleaseHdc();
            e.Graphics.ResetTransform();

            if (drawBackground && d != null)
            {
                using (TextureBrush brush = new TextureBrush(d, WrapMode.Tile))
                {
                    e.Graphics.FillRectangle(brush, Rectangle.Inflate(newRect, -2, -2));
                }
            }

            StringFormat format = new StringFormat();
            format.HotkeyPrefix = HotkeyPrefix.None;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;
            if (IconImage != null)
            {
                //const int DI_NORMAL = 0x0003;
                //const int DI_IMAGE = 0x0002;
                //const int DI_NOMIRROR = 0x0010;
                //[DllImport("user32.dll", SetLastError = true)]
                //static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, int hbrFlickerFreeDraw, int diFlags);
                
                //Rectangle dest = textIndent ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16);
                //DrawIconEx(e.Graphics.GetHdc(), dest.X, dest.Y, Icon.Handle, dest.Width, dest.Height, 0, 0, DI_NORMAL);
                //MessageBox.Show(GetLastError().ToString());
                //e.Graphics.ReleaseHdc();

                //e.Graphics.DrawImage(IconImage, textIndent ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16));
                //e.Graphics.DrawImage(Bitmap.FromHicon(Icon.Handle), textIndent ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16));

                e.Graphics.DrawImage(IconImage, textIndent ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16));

                if (Width >= 60)
                    e.Graphics.DrawString(Title, textFont, SystemBrushes.ControlText, textIndent ? new Rectangle(21, 11, Width - 21 - 3 - SpaceNeededNextToText, 10) : new Rectangle(20, 10, Width - 20 - 3 - SpaceNeededNextToText, 11), format);
            }
            else
            {
                e.Graphics.DrawString(Title, textFont, SystemBrushes.ControlText, textIndent ? new Rectangle(5, 11, Width - 5 - 3 - SpaceNeededNextToText, 10) : new Rectangle(4, 10, Width - 4 - 3 - SpaceNeededNextToText, 11), format);
            }

            FinishOnPaint(e);
        }
    }
}
