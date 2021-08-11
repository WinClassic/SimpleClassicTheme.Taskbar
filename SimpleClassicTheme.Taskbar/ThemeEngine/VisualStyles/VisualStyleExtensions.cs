using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Drawing;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.VisualStyles
{
    public static class VisualStyleExtensions
    {
        public static void DebugDrawPadding(this Graphics graphics, VisualStyleElement element, Rectangle bounds)
        {
            if (element.ContentMargins is Padding contentMargin)
            {
                DebugDrawPadding(graphics, bounds, contentMargin, Color.Lime);
            }
        }
        public static void DebugDrawPadding(this Graphics graphics, Rectangle bounds, Padding padding, Color color)
        {
            var backColor = Color.FromArgb(96, color);
            using (var brush = new SolidBrush(backColor))
            {
                graphics.FillRectangle(brush, 0, 0, bounds.Width, padding.Top);
                graphics.FillRectangle(brush, 0, bounds.Height - padding.Bottom, bounds.Width, padding.Bottom);
                graphics.FillRectangle(brush, 0, padding.Top, padding.Left, bounds.Height - padding.Vertical);
                graphics.FillRectangle(brush, bounds.Width - padding.Right, padding.Top, padding.Right, bounds.Height - padding.Vertical);
            }
        }

        public static void DrawElement(this Graphics graphics, VisualStyleElement element, Rectangle bounds, int? imageIndex = null)
        {
            var bitmap = element.Image;

            var srcRect = new Rectangle(Point.Empty, bitmap.Size);

            if (imageIndex.HasValue)
            {
                switch (element.ImageLayout)
                {
                    case Orientation.Horizontal:
                        throw new NotImplementedException();

                    case Orientation.Vertical:
                        var actualHeight = bitmap.Height / element.ImageCount;
                        var top = actualHeight * imageIndex.Value;
                        srcRect = new Rectangle(0, top, bitmap.Width, actualHeight);
                        break;
                }
            }

            var geometry = new NinePatchGeometry(element.SizingMargins.Value, srcRect);
            graphics.DrawNinePatch(bitmap, geometry, bounds, element.SizingType == SizingType.Tile);
        }

        public static int Get0BGR(Color rgbColor)
        {
            // Return a zero-alpha 24-bit BGR color integer
            return (0 << 24) + (rgbColor.B << 16) + (rgbColor.G << 8) + rgbColor.R;
        }

        public static void DrawString(this Graphics graphics, VisualStyleElement element, string s, int x, int y, int w, int h)
        {
            if (element.TextShadowType == TextShadowType.Continuous)
            {
                var textColor = (uint)Get0BGR(element.TextColor.Value);
                var shadowColor = (uint)Get0BGR(element.TextShadowColor.Value);

                var rect = new RECT(x, y, x + w, y + h);
                var hdc = graphics.GetHdc();
                var hFont = element.Font.ToHfont();
                
                Gdi32.SelectObject(hdc, hFont);
                Gdi32.SetTextColor(hdc, textColor);
                ComCtl32.DrawShadowText(hdc, s, (uint)s.Length, rect, 0, textColor, shadowColor, element.TextShadowOffset.X, element.TextShadowOffset.Y);
                
                Gdi32.DeleteObject(hFont);
                graphics.ReleaseHdc(hdc);
            }
            else
            {
                using var textBrush = new SolidBrush(element.TextColor.Value);
                graphics.DrawString(s, element.Font, textBrush, x, y);
            }
        }
    }
}
