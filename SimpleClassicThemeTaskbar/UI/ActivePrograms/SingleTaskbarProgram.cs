using SimpleClassicThemeTaskbar.Forms;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public class SingleTaskbarProgram : BaseTaskbarProgram
    {
        private Icon icon;

        private Bitmap iconImage;

        private Process process;

        private Window window;

        public SingleTaskbarProgram()
        {
            Constructor();
        }

        public override Icon Icon
        {
            get => icon;
            set
            {
                icon = value;
                if (icon != null)
                    iconImage = new Icon(icon, 16, 16).ToBitmap();
                else
                    iconImage = null;
                /*if (icon == null)
                    return;
                ICONINFO ii;
                GetIconInfo(icon.Handle, out ii);
                Bitmap bmpIcon = Bitmap.FromHbitmap(ii.hbmColor);
                Rectangle rectBounds = new Rectangle(0, 0, bmpIcon.Width, bmpIcon.Height );
                BitmapData bmData = new BitmapData();
                bmpIcon.LockBits(rectBounds, ImageLockMode.ReadOnly, bmpIcon.PixelFormat, bmData);
                Bitmap bmpAlpha = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);
                bmpIcon.UnlockBits(bmData);
                iconImage = bmpAlpha;*/
            }
        }

        public override Image IconImage { get => iconImage;/* { try { return new Icon(Icon, 16, 16).ToBitmap(); } catch { return null; } } */}
        public override int MinimumWidth => Helpers.Config.Renderer.TaskButtonMinimalWidth;
        public override Process Process { get => process; set { process = value; /*MessageBox.Show(ApplicationEntryPoint.d.GetAppUserModelId(Process.Id));*/ } }
        public override string Title { get => window.Title; set => throw new NotImplementedException(); }
        public override Window Window { get => window; set => window = value; }

        public override void FinishOnPaint(PaintEventArgs e)
        {
        }

        public override string GetErrorString()
                    => GetBaseErrorString() +
            $"Process: {Process.MainModule.ModuleName} ({Process.Id})\n" +
            $"Window title: {Title}\n" +
            $"Window class: {Window.ClassName}\n" +
            $"Window HWND: {Window.Handle:X8} {(User32.IsWindow(Window.Handle) ? "Valid" : "Invalid")}\n" +
            $"Icon HWND: {Icon.Handle:X8} ({(User32.IsWindow(Icon.Handle) ? "Valid" : "Invalid")})";

        public override bool IsActiveWindow(IntPtr activeWindow)
        {
            bool result = activeWindow == Window.Handle;
            ActiveWindow = result;
            return result;
        }

        public override void OnClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                new IconTest(Window).Show();
                return;
            }

            ApplicationEntryPoint.ErrorSource = this;
            controlState = "handling mouse click";
            if (IsMoving)
            {
                IsMoving = false;
            }
            else if (ActiveWindow)
            {
                _ = User32.ShowWindow(Window.Handle, 6);
            }
            else
            {
                if ((Window.WindowInfo.dwStyle & 0x20000000) > 0)
                    _ = User32.ShowWindow(Window.Handle, 9);

                _ = User32.SetForegroundWindow(Window.Handle);

                if (Parent != null && Parent is Taskbar)
                {
                    foreach (Control d in Parent.Controls)
                    {
                        if (d is TaskbarProgram b)
                        {
                            b.ActiveWindow = false;
                        }
                    }
                }

                ActiveWindow = true;
            }
        }

        public override string ToString() => $"Handle: {Window.Handle}, Title: {Title}";

        protected override bool CancelMouseDown(MouseEventArgs e) => false;
    }
}