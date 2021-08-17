using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Helpers.NativeMethods.Kernel32;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    internal static class HelperFunctions
    {
        public static bool IsWindows10OrHigher => Environment.OSVersion.Version.Major >= 10;

        public static bool CanEnableVirtualDesktops => IsWindows10OrHigher && IsExplorerRunning;

        public static bool ShouldEnableVirtualDesktops => CanEnableVirtualDesktops && Config.Default.Tweaks.EnableVirtualDesktops;

        public static bool ShouldUseVirtualDesktops => ShouldEnableVirtualDesktops && VirtualDesktops.IsInitialized;

        public static bool IsExplorerRunning => Process.GetProcessesByName("explorer").Length > 0;

        public static void Crawl(this Control control, Action<Control> action)
        {
            foreach (Control childControl in control.Controls)
            {
                Crawl(childControl, action);
            }

            action.Invoke(control);
        }

        delegate IntPtr MsgHookProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void OpenQuickLaunchFolder()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var quickLaunchPath = Path.Combine(appDataPath, @"Microsoft\Internet Explorer\Quick Launch");

            _ = Process.Start(new ProcessStartInfo
            {
                FileName = quickLaunchPath,
                UseShellExecute = true,
            });
        }

        public static void SetFlatStyle(this Form form, FlatStyle style)
        {
            form.Crawl((control) =>
            {
                switch (control)
                {
                    case ButtonBase buttonBase:
                        buttonBase.FlatStyle = style;
                        break;

                    case Label label:
                        label.FlatStyle = style;
                        break;

                    case ComboBox comboBox:
                        comboBox.FlatStyle = style;
                        break;

                    case GroupBox groupBox:
                        groupBox.FlatStyle = style;
                        break;
                }
            });
        }

        public static Bitmap RemoveTransparencyKey(this Bitmap bitmap, Color? transparencyKey = null)
        {
            if (!transparencyKey.HasValue)
            {
                transparencyKey = Color.Magenta;
            }

            var rect = new Rectangle(Point.Empty, bitmap.Size);
            bitmap = bitmap.Clone(rect, PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var bitsPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat);
            var size = bitmapData.Stride * bitmapData.Height;
            var data = new byte[size];

            Marshal.Copy(bitmapData.Scan0, data, 0, size);

            for (int i = 0; i < size; i += bitsPerPixel / 8)
            {
                // var magnitude = 1 / 3d * (data[i] + data[i + 1] + data[i + 2]);

                //data[i] is the first of 3 bytes of color
                if (transparencyKey.Value.B == data[i] &&
                    transparencyKey.Value.G == data[i + 1] &&
                    transparencyKey.Value.R == data[i + 2])
                {
                    data[i + 3] = 0;
                }
            }

            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static void DrawImage(this Graphics graphics, Image image, Rectangle destRect, Rectangle srcRect)
        {
            graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
        }

        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            var bufferLength = (uint)fileNameBuilder.Capacity + 1;

            var handle = OpenProcess(0x0400, false, process.Id);

            if (handle == IntPtr.Zero)
            {
                return null;
                //throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (QueryFullProcessImageNameW(handle, 0, fileNameBuilder, ref bufferLength))
            {
                return fileNameBuilder.ToString();
            }
            else
            {
                //throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return null;
        }

        /// <summary>
        /// Replaces Windows line endings (\r\n) to Unix line endings (\n)
        /// </summary>
        public static string CrLfToLf(this string value)
        {
            return value.Replace("\r\n", "\n");
        }

        public static (string Title, string Content) GetToolTipContent(string rawToolTip)
        {
            string raw = rawToolTip.CrLfToLf();

            int newLineIndex = raw.IndexOf("\n");

            string title = raw;
            string text = string.Empty;

            if (newLineIndex != -1)
            {
                title = raw.Substring(0, newLineIndex);
                text = raw[(newLineIndex + 1)..];
            }

            return (title, text);
        }

        /// <summary>
        /// Encapsulates SystemTrayIcon's code for adjusting color of a tray icon.
        /// </summary>
        public static void RecolorTrayIcon(Bitmap bitmap)
        {
            Rectangle rect = new(new Point(0, 0), bitmap.Size);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            int length = Math.Abs(data.Stride) * bitmap.Height;

            byte[] pixelValues = new byte[length];
            Marshal.Copy(data.Scan0, pixelValues, 0, length);

            (float factorR, float factorG, float factorB, float factorA) = SystemColors.ControlText.GetFactors();

            for (int i = 0; i < pixelValues.Length; i += 4)
            {
                byte a = pixelValues[i + 3];
                byte r = pixelValues[i + 2];
                byte g = pixelValues[i + 1];
                byte b = pixelValues[i + 0];

                if (((r << 16) + (g << 8) + (b << 0)) > 0)
                {
                    pixelValues[i + 3] = (byte)(a * factorA);
                    pixelValues[i + 2] = (byte)(r * factorR);
                    pixelValues[i + 1] = (byte)(g * factorG);
                    pixelValues[i + 0] = (byte)(b * factorB);
                }
            }

            Marshal.Copy(pixelValues, 0, data.Scan0, length);

            bitmap.UnlockBits(data);
        }

        public static (float R, float G, float B, float A) GetFactors(this Color color)
        {
            return (color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static Bitmap ChangePixelFormat(Bitmap bitmap, PixelFormat pixelFormat)
        {
            Bitmap result = new(bitmap.Width, bitmap.Height, pixelFormat);
            Rectangle bmpBounds = new(Point.Empty, bitmap.Size);
            BitmapData srcData = bitmap.LockBits(bmpBounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            BitmapData resData = result.LockBits(bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat);

            long srcScan0 = srcData.Scan0.ToInt64();
            long resScan0 = resData.Scan0.ToInt64();
            int srcStride = srcData.Stride;
            int resStride = resData.Stride;
            int rowLength = Math.Abs(srcData.Stride);
            try
            {
                byte[] buffer = new byte[rowLength];
                for (int y = 0; y < srcData.Height; y++)
                {
                    IntPtr sourcePtr = new(srcScan0 + (y * srcStride));
                    Marshal.Copy(sourcePtr, buffer, 0, rowLength);

                    IntPtr resPtr = new(resScan0 + (y * resStride));
                    Marshal.Copy(buffer, 0, resPtr, rowLength);
                }
            }
            finally
            {
                bitmap.UnlockBits(srcData);
                result.UnlockBits(resData);
            }

            return result;
        }
    }
}