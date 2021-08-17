using IniParser.Model;
using IniParser.Model.Configuration;
using IniParser.Parser;

using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleClassicTheme.Taskbar.ThemeEngine.VisualStyles
{
    /// <summary>
    /// Represents a Windows visual style
    /// </summary>
    public class VisualStyle : IDisposable
    {
        private readonly IntPtr hModule;
        private bool disposedValue;
        private readonly IniData themeIni;
        private static readonly IniParserConfiguration iniParserConfig = new()
        {
            CaseInsensitive = true,
            AllowDuplicateKeys = true,
            AllowDuplicateSections = true,
            OverrideDuplicateKeys = true,
        };

        public VisualStyle(string vsPath)
        {
            hModule = NativeLibrary.Load(vsPath);

            if (hModule == IntPtr.Zero)
            {
                throw new ArgumentException("Couldn't load the specified visual style.", nameof(vsPath));
            }

            var buffer = LoadResource("THEMES_INI", "TEXTFILE");
            var text = Encoding.Unicode.GetString(buffer);
            text = FilterComments(text);
            themeIni = new IniDataParser(iniParserConfig).Parse(text);
            var docsSection = themeIni["documentation"];
            DisplayName = docsSection["DisplayName"];
            ToolTip = docsSection["ToolTip"];
            Author = docsSection["Author"];
            Company = docsSection["Company"];
            Copyright = docsSection["Copyright"];
            Description = docsSection["Description"];
            ThemeName = docsSection["ThemeName"];
            WmpSkinName = docsSection["WmpSkinName"];

            // Special parsing
            // LastUpdated = DateTime.Parse(docsSection["LastUpdated"]);
            // URL = new Uri(docsSection["URL"]);
        }

        ~VisualStyle()
        {
            Dispose(disposing: false);
        }

        private byte[] LoadResource(string resourceName, string resourceType)
        {
            var hResInfo = Kernel32.FindResourceW(hModule, resourceName, resourceType);
            var hResData = Kernel32.LoadResource(hModule, hResInfo);
            var resSize = Kernel32.SizeofResource(hModule, hResInfo);

            var buffer = new byte[resSize];
            Marshal.Copy(hResData, buffer, 0, buffer.Length);

            return buffer;
        }

        public string[] ColorNames
        {
            get
            {
                var buffer = LoadResource("#1", "COLORNAMES");
                var text = Encoding.Unicode.GetString(buffer);
                var colorNames = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);
                return colorNames;
            }
        }

        public string[] SizeNames
        {
            get
            {
                var buffer = LoadResource("#1", "SIZENAMES");
                var text = Encoding.Unicode.GetString(buffer);
                var colorNames = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);
                return colorNames;
            }

        }


        public VisualStyleColorScheme[] GetColorSchemes()
        {
            var colorSchemes = new VisualStyleColorScheme[ColorNames.Length * SizeNames.Length];
            var i = 0;
            foreach (var colorName in ColorNames)
            {
                foreach (var sizeName in SizeNames)
                {
                    colorSchemes[i] = GetColorScheme(colorName, sizeName);
                    i++;
                }
            }
            return colorSchemes;
        }

        private string GetIniName(string colorName, string sizeName)
        {
            var colorIndex = Array.IndexOf(ColorNames, colorName);
            var sizeIndex = Array.IndexOf(SizeNames, sizeName);
            var fileResIndex = sizeIndex + (colorIndex * SizeNames.Length);

            var buffer = LoadResource("#1", "FILERESNAMES");
            var text = Encoding.Unicode.GetString(buffer);
            var fileResNames = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);

            return fileResNames[fileResIndex];
        }



        public string Author { get; private set; }
        public string Company { get; private set; }
        public string Copyright { get; private set; }
        public string Description { get; private set; }
        public string DisplayName { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public string ThemeName { get; private set; }
        public string ToolTip { get; private set; }
        public Uri URL { get; private set; }

        /// <summary>
        /// Name of the accompanying Windows Media Player skin.
        /// </summary>
        public string WmpSkinName { get; private set; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                NativeLibrary.Free(hModule);

                disposedValue = true;
            }
        }

        /// <summary>
        /// Converts a visual style file path into a resource name.
        /// </summary>
        /// <param name="filePath">A file path (like "Blue\button.bmp")</param>
        /// <returns>The translated resource name (like "BLUE_BUTTON_BMP")</returns>
        public static string GetResourceName(string filePath)
        {
            var invalidChars = Path.GetInvalidFileNameChars().Append('.');
            var filteredFilePathChars = filePath.Select((c) => invalidChars.Contains(c) ? '_' : c);
            var filteredFilePath = new string(filteredFilePathChars.ToArray());
            return filteredFilePath.ToUpperInvariant();
        }



        public Bitmap LoadBitmap(string filePath)
        {
            string resourceName = GetResourceName(filePath);
            //IntPtr hBitmap = User32.LoadImageW(hModule, resourceName, 0, 0, 0, 0x00002000 | 0x00008000 | 0x00000040 | 0x00000080);
            //IntPtr hBitmap = User32.LoadBitmapW(hModule, resourceName);
            //Bitmap bitmap = Bitmap.FromHbitmap(hBitmap);
            Bitmap bitmap = Bitmap.FromResource(hModule, resourceName);
            return bitmap;
        }

        public VisualStyleColorScheme GetColorScheme(string iniFileName)
        {
            var buffer = LoadResource(iniFileName, "TEXTFILE");
            var text = Encoding.Unicode.GetString(buffer);

            text = FilterComments(text);

            var colorSchemeIni = new IniDataParser(iniParserConfig).Parse(text);

            return new(this, colorSchemeIni);
        }

        public VisualStyleColorScheme GetColorScheme(string colorName, string sizeName)
        {
            var fileResName = GetIniName(colorName, sizeName);
            return GetColorScheme(fileResName);
        }

        // HACK: fuck ini parsers not handling comments correctly
        private static string FilterComments(string value)
        {
            return Regex.Replace(value, @";(.*)", string.Empty);
        }

        public (string DisplayName, string ToolTip) GetSizeDisplay(string sizeName)
        {
            var section = themeIni["Size." + sizeName];
            return (section["DisplayName"], section["ToolTip"]);
        }
        public (string DisplayName, string ToolTip) GetColorDisplay(string colorName)
        {
            var section = themeIni["ColorScheme." + colorName];
            return (section["DisplayName"], section["ToolTip"]);
        }
    }
}