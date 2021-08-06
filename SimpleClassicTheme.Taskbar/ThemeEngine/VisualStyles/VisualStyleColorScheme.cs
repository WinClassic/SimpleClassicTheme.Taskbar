using IniParser.Model;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.VisualStyles
{
    public class VisualStyleColorScheme
    {
        private readonly IniData ini;
        private readonly Dictionary<string, VisualStyleElement> sectionCache = new();

        public VisualStyle VisualStyle { get; }

        internal VisualStyleColorScheme(VisualStyle visualStyle, IniData ini)
        {
            VisualStyle = visualStyle;
            this.ini = ini ?? throw new ArgumentNullException(nameof(ini));
        }

        public VisualStyleElement this[string section]
        {
            get
            {
                section = section.ToLowerInvariant();

                if (!sectionCache.ContainsKey(section))
                {
                    sectionCache[section] = CreateElement(ini[section]);
                }

                return sectionCache[section];
            }
        }

        private VisualStyleElement CreateElement(KeyDataCollection data)
        {
            static Padding? ParsePadding(string value)
            {
                if (value == null)
                {
                    return null;
                }

                var values = value
                    .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select((v) => int.Parse(v))
                    .ToArray();

                return new(values[0], values[2], values[1], values[3]);
            }

            static Color? ParseColor(string value)
            {
                if (value == null)
                {
                    return null;
                }

                var values = value
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select((v) => byte.Parse(v))
                    .ToArray();

                return Color.FromArgb(values[0], values[1], values[2]);
            }

            static bool ParseBool(string value, bool @default)
            {
                if (bool.TryParse(value, out var result))
                {
                    return result;
                }

                return @default;
            }

            static int ParseInt(string value, int @default)
            {
                if (int.TryParse(value, out var result))
                {
                    return result;
                }

                return @default;
            }

            static Point ParseOffset(string value)
            {
                if (value == null)
                {
                    return Point.Empty;
                }

                var values = value
                   .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (int.TryParse(values[0], out var x) && int.TryParse(values[1], out var y))
                {
                    return new Point(x, y);
                }

                return Point.Empty;
            }

            return new(this)
            {
                BackgroundType = Enum.Parse<BackgroundType>(data["BgType"] ?? "None", true),
                SizingMargins = ParsePadding(data["SizingMargins"]),
                ContentMargins = ParsePadding(data["ContentMargins"]),
                FillColorHint = ParseColor(data["FillColorHint"]),
                TextColor = ParseColor(data["TextColor"]),
                BorderColorHint = ParseColor(data["BorderColorHint"]),
                ImageFile = data["ImageFile"],
                FontName = data["Font"],
                SizingType = Enum.Parse<SizingType>(data["SizingType"] ?? "Stretch", true),
                Transparent = ParseBool(data["Transparent"], false),
                TransparentColor = ParseColor(data["TransparentColor"]),
                ImageLayout = Enum.Parse<Orientation>(data["ImageLayout"] ?? "Horizontal", true),
                ImageCount = ParseInt(data["ImageCount"], 1),
                TextShadowType = Enum.Parse<TextShadowType>(data["TextShadowType"] ?? "Single", true),
                TextShadowColor = ParseColor(data["TextShadowColor"]),
                TextShadowOffset = ParseOffset(data["TextShadowOffset"]),
            };
        }
    }
}
