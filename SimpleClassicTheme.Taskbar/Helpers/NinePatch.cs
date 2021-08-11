using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public static class NinePatchExtensions
    {
        public static void DrawNinePatch(this Graphics graphics, Image image, NinePatchGeometry geometry, Rectangle destinationRectangle, bool tile = false)
        {
            var srcRects = geometry.GetRectangles();

            var dstGeometry = new NinePatchGeometry(geometry.Padding, destinationRectangle);
            var dstRects = dstGeometry.GetRectangles();

            for (int i = 0; i < 9; i++)
            {
                var srcRect = srcRects[i];
                var dstRect = dstRects[i];

                if (srcRect.Width == 0 || srcRect.Height == 0)
                {
                    continue;
                }

                if (tile)
                {
                    var cut = ((Bitmap)image).Clone(srcRect, image.PixelFormat);
                    using (var textureBrush = new TextureBrush(cut, WrapMode.Tile))
                    {
                        // We use transforms so the results are identical:
                        //
                        // If we would run this method without transforms
                        // on two different positions, the start of the
                        // bitmap would look different. This is because
                        // each paint function is acting like a stencil
                        // for a Brush (in this case TextureBrush).

                        graphics.TranslateTransform(dstRect.X, dstRect.Y);
                        graphics.FillRectangle(textureBrush, new (Point.Empty, dstRect.Size));
                        graphics.ResetTransform();
                    }
                }
                else
                {
                    graphics.DrawImage(image, dstRect, srcRect);
                }
            }
        }
    }

    /// <summary>
    /// Contains numeric data for a nine-patch and properties for calculating these values.
    /// </summary>
    public struct NinePatchGeometry
    {
        public Padding Padding;
        public Rectangle Rectangle;

        public int[] Horizontals => new int[]
        {
            Rectangle.X,
            Rectangle.Left + Padding.Left,
            Rectangle.Right - Padding.Right,
            Rectangle.Right
        };

        public int[] Verticals => new int[]
        {
            Rectangle.Y,
            Rectangle.Top + Padding.Top,
            Rectangle.Bottom - Padding.Bottom,
            Rectangle.Bottom
        };

        public NinePatchGeometry(Padding padding, Rectangle rectangle)
        {
            Padding = padding;
            Rectangle = rectangle;
        }

        public Rectangle[] GetRectangles()
        {
            var rectangles = new Rectangle[9];
            var h = Horizontals;
            var v = Verticals;

            var i = 0;
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                {
                    rectangles[i] = Rectangle.FromLTRB(Math.Max(0, h[x]), Math.Max(0, v[y]), Math.Max(0, h[x + 1]), Math.Max(0, v[y + 1]));
                    i++;
                }

            return rectangles;
        }
    }
}
