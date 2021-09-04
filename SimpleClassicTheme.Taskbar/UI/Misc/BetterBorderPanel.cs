using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinDef;
using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar.UIElements.Misc
{
    public partial class BetterBorderPanel : Panel
    {
        public const uint DFC_BUTTON = 4;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_PUSHED = 512;

        public bool Do3DBorder = true;
        public bool isButton = false;

        public Border3DStyle style = Border3DStyle.Raised;

        public BetterBorderPanel()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += OnPaint;
        }

        [Description("Defines the style to be used when drawing ")]
        [Category("Appearance")]
        public new Border3DStyle BorderStyle
        {
            get => style;
            set
            {
                style = value;
                Invalidate();
            }
        }

        [Description("Specifies if the control should be rendered like a button")]
        [Category("Appearance")]
        public bool IsButton
        {
            get => isButton;
            set
            {
                isButton = value;
                Invalidate();
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (Do3DBorder)
            {
                if (isButton)
                {
                    RECT rect = ClientRectangle;
                    uint buttonStyle = style == Border3DStyle.Raised ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
                    _ = DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
                    e.Graphics.ReleaseHdc();
                }
                else
                {
                    ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, style);
                }
            }
            else
            {
                ButtonBorderStyle bStyle =
                    style == Border3DStyle.Raised ? ButtonBorderStyle.Outset : ButtonBorderStyle.Inset;
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                    SystemColors.Control, 1, bStyle,
                    SystemColors.Control, 1, bStyle,
                    SystemColors.Control, 2, bStyle,
                    SystemColors.Control, 2, bStyle);
            }
            if (BackgroundImage != null)
            {
                using (TextureBrush brush = new(BackgroundImage, WrapMode.Tile))
                {
                    e.Graphics.FillRectangle(brush, Rectangle.Inflate(ClientRectangle, -2, -2));
                }
            }
        }
    }
}