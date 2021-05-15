e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

Image temp = null;

switch (Appearance)
{
    case StartButtonAppearance.Default:
        temp = Properties.Resources.startIcon95;
        break;

    case StartButtonAppearance.CustomIcon:
        try
        {
            temp = Image.FromFile(iconImageFile);
        }
        catch
        {
            e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
            return;
        }
        if (temp.Width != 16 || temp.Height != 16)
        {
            temp.Dispose();
            temp = Properties.Resources.startIcon95;
        }
        break;

    case StartButtonAppearance.CustomButton:
        try
        {
            temp = Image.FromFile(imageFile);
        }
        catch
        {
            e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
            return;
        }
        if (temp.Height != 66)
        {
            e.Graphics.DrawString("ERROR", new Font("Tahoma", 8F, FontStyle.Bold), Brushes.Black, new PointF(0F, 0F));
            temp.Dispose();
            return;
        }
        if (Width != temp.Width)
        {
            Width = temp.Width;
            Invalidate();
        }
        e.Graphics.DrawImage(temp, new Rectangle(0, 0, temp.Width, 22), new Rectangle(0, pressed ? 44 : 0, temp.Width, 22), GraphicsUnit.Pixel);
        return;
}

if (Width != 55)
{
    Width = 55;
    Invalidate();
}

if (Do3DBorder)
{
    if (isButton)
    {
        RECT rect = new(ClientRectangle);
        uint buttonStyle = style == Border3DStyle.Raised ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
        _ = User32.DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
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

bool mouseIsDown = ClientRectangle.Contains(PointToClient(MousePosition)) && (MouseButtons & MouseButtons.Left) != 0;
e.Graphics.DrawImage(temp ?? Properties.Resources.startIcon95, mouseIsDown ? new Point(5, 4) : new Point(4, 3));
e.Graphics.DrawString("Start", new Font("Tahoma", 8F, FontStyle.Bold), SystemBrushes.ControlText, mouseIsDown ? new PointF(21F, 5F) : new PointF(20F, 4F));

temp.Dispose();