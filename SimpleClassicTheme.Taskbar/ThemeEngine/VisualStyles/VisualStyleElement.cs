using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.VisualStyles
{
    public class VisualStyleElement : IDisposable
    {
        private Bitmap _image;

        private Font font;

        internal VisualStyleElement(VisualStyleColorScheme colorScheme)
        {
            ColorScheme = colorScheme;
        }

        public Color? AccentColorHint { get; init; }

        public BackgroundType BackgroundType { get; init; }

        public Color? BorderColorHint { get; init; }

        public VisualStyleColorScheme ColorScheme { get; }

        public Padding? ContentMargins { get; init; }

        public Color? FillColorHint { get; init; }

        public string FontName { get; init; }

        public Font Font
        {
            get
            {
                if (font == null)
                {
                    var split = FontName.Split(", ", StringSplitOptions.RemoveEmptyEntries);
                    
                    var fontFamily = split[0];
                    var fontSize = float.Parse(split[1]);
                    var fontStyle = FontStyle.Regular;

                    if (split.Length >= 3)
                    {
                        fontStyle = Enum.Parse<FontStyle>(split[2], true);
                    }


                    font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Point);
                }
                return font;
            }
        }

        public Bitmap Image
        {
            get
            {
                if (_image == null)
                {
                    var resourceName = VisualStyle.GetResourceName(ImageFile);
                    var bitmap = ColorScheme.VisualStyle.LoadBitmap(resourceName);

                    if (Transparent)
                    {
                        bitmap = bitmap.RemoveTransparencyKey(TransparentColor);
                    }

                    _image = bitmap;
                }

                return _image;
            }
            private set => _image = value;
        }

        public int ImageCount { get; init; }

        public string ImageFile { get; init; }

        public Orientation ImageLayout { get; init; }

        public Point Offset { get; init; }

        public OffsetType OffsetType { get; init; }

        public Padding? SizingMargins { get; init; }

        public SizingType SizingType { get; init; }

        public Color? TextColor { get; init; }

        public Color? TextShadowColor { get; init; }

        public Point TextShadowOffset { get; init; }

        public TextShadowType TextShadowType { get; init; }

        public bool Transparent { get; init; }

        public Color? TransparentColor { get; init; }

        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }
           
            GC.SuppressFinalize(this);
        }

        public Bitmap[] GetBitmaps()
        {
            var fullBitmap = (Bitmap)Image.Clone();

            if (Transparent)
            {
                fullBitmap = fullBitmap.RemoveTransparencyKey(TransparentColor);
            }

            var bitmapArray = new Bitmap[ImageCount];
            var actualHeight = fullBitmap.Height / ImageCount;

            for (int i = 0; i < ImageCount; i++)
            {
                switch (ImageLayout)
                {
                    case Orientation.Horizontal:
                        throw new NotImplementedException();

                    case Orientation.Vertical:
                        var top = actualHeight * i;
                        var rect = new Rectangle(0, top, fullBitmap.Width, actualHeight);
                        bitmapArray[i] = fullBitmap.Clone(rect, fullBitmap.PixelFormat);
                        break;
                }
            }

            fullBitmap?.Dispose();

            return bitmapArray;
        }

    }
}