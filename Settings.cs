for (int x = 0; x < newImage.Width; x++)
    for (int y = 0; y < newImage.Height; y++)
    {
        Color c = newImage.GetPixel(x, y);
        if (c.R == 0x00 && c.G == 0x00 && c.B == 0x00 && c.A != 0x00)
            newImage.SetPixel(x, y, SystemColors.ControlText);
    }
if (ApplicationEntryPoint.SCTCompatMode)
{
    pictureBox1.Height = 106;
    pictureBox3.Height = 121;
    pictureBox2.Location = new Point(0, 124);
}

Color A = SystemColors.ActiveCaption;
Color B = SystemColors.GradientActiveCaption;
Bitmap bitmap = new(696, 5);
for (int i = 0; i < 348; i++)
{
    int r = A.R + ((B.R - A.R) * i / 348);
    int g = A.G + ((B.G - A.G) * i / 348);
    int b = A.B + ((B.B - A.B) * i / 348);

    for (int y = 0; y < 5; y++)
        bitmap.SetPixel(i, y, Color.FromArgb(r, g, b));

    for (int y = 0; y < 5; y++)
        bitmap.SetPixel(695 - i, y, Color.FromArgb(r, g, b));
}
if (IntPtr.Size == 8)
    pictureBox2.Location = new Point(pictureBox2.Location.X + 3, pictureBox2.Location.Y);
pictureBox2.Image = bitmap;