using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleClassicThemeTaskbar
{
    public partial class TaskbarProgram : UserControl
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int DrawFrameControl(IntPtr hdc, ref RECT lpRect, uint un1, uint un2);

        public const uint DFC_BUTTON = 4;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_PUSHED = 512;

        public Process Process;
        public Window Window;
        public Icon Icon
        {
            get { return iconBackup; }
            set { try { icon = new Icon(value, 16, 16).ToBitmap(); iconBackup = value; Invalidate(); } catch { } }
        }
        public string Title
        {
            get { return text; }
            set { text = value; Invalidate(); }
        }

        public bool IsMoving = false;
        public new MouseEventHandler MouseDown;
        public new MouseEventHandler MouseUp;
        public new MouseEventHandler MouseMove;

        private Icon iconBackup;
        private Image icon;
        private string text = "";

        private Font textFont;
        private bool textIndent = false;
        private bool drawBackground = false;
        private Border3DStyle style;

        private Bitmap d;
        private bool activeWindow = false;
        public bool ActiveWindow
        {
            get
            {
                return activeWindow;
            }
            set 
            { 
                activeWindow = value;
                
                if (ClientRectangle.Contains(PointToClient(MousePosition)))
                {
                    if ((MouseButtons & MouseButtons.Left) != 0)
                    {
                        //Don't update if the mouse is held down on this control
                        return;
                    }
                }

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

        public TaskbarProgram()
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

            MouseDown += delegate {
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
            Click += OnClick;
        }

        public void OnClick(object sender, EventArgs e)
        {
            if (IsMoving)
            {
                IsMoving = false;
            }
            else if (ActiveWindow)
            {
                ShowWindow(Window.Handle, 6);
            }
            else
            {
                if ((Window.WindowInfo.dwStyle & 0x20000000) > 0)
                    ShowWindow(Window.Handle, 9);
                SetForegroundWindow(Window.Handle);

                foreach (Control d in Parent.Controls)
                {
                    if (d is TaskbarProgram b)
                    {
                        b.ActiveWindow = false;
                    }
                }

                ActiveWindow = true;
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            Rectangle newRect = ClientRectangle;
            newRect.Y += 4;
            newRect.Height -= 6;
            //ControlPaint.DrawBorder3D(e.Graphics, newRect, style);
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
            if (icon != null)
            {

                e.Graphics.DrawImage(icon, textIndent ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16));
                //e.Graphics.DrawString(Title, textFont, SystemBrushes.ControlText, textIndent ? new PointF(20F, 8F) : new PointF(19F, 8F)/*, new StringFormat() { Trimming = StringTrimming.EllipsisCharacter }*/);
                //e.Graphics.DrawString("Start", new Font("Tahoma", 8F, FontStyle.Bold), SystemBrushes.ControlText, mouseIsDown ? new PointF(21F, 5F) : new PointF(20F, 4F));
                //e.Graphics.FillRectangle(Brushes.Red, textIndent ? new Rectangle(21, 10, Width - 21 - 4, 11) : new Rectangle(20, 10, Width - 20 - 4, 11));
                e.Graphics.DrawString(Title, textFont, SystemBrushes.ControlText, textIndent ? new Rectangle(21, 11, Width - 21 - 4, 10) : new Rectangle(20, 10, Width - 20 - 4, 11), format);
            }
            else
            {
                e.Graphics.DrawString(Title, textFont, SystemBrushes.ControlText, textIndent ? new Rectangle(5, 11, Width - 5 - 4, 10) : new Rectangle(4, 10, Width - 4 - 4, 11), format);
            }
        }

        private void TaskbarProgram_Load(object sender, EventArgs e)
        {
            line.Width = Width;
        }
    }
}
