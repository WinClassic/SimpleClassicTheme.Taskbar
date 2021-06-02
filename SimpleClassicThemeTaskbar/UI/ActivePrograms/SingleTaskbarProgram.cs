using SimpleClassicThemeTaskbar.Forms;
using SimpleClassicThemeTaskbar.Helpers;
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
        public override int MinimumWidth => Helpers.Config.Instance.Renderer.TaskButtonMinimalWidth;
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
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "handling mouse click";

            if (e.Button == MouseButtons.Right)
            {
                if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                {
                    new IconTest(Window).Show();
                }
                else
                {
                    var systemMenu = User32.GetSystemMenu(Window.Handle, false);

                    if (systemMenu == IntPtr.Zero)
                    {
                        Logger.Log(LoggerVerbosity.Verbose, "SingleTaskbarProgram", $"Got an empty system menu ({systemMenu.ToInt64():X8})");
                        return;
                    }

                    // Inserting menu items is bugged sadly.
                    // User32.AppendMenu(systemMenu, User32.MF_SEPARATOR, 0x0000, IntPtr.Zero);
                    // User32.AppendMenu(systemMenu, User32.MF_STRING, 0x1337, "Icon Test");

                    // HACK: preserving the pressed down state when the menu is open
                    ActiveWindow = true;

                    _ = User32.ShowWindow(Window.Handle, User32.SW_SHOW);
                    _ = User32.SetForegroundWindow(Window.Handle);

                    var screenLocation = PointToScreen(e.Location);
                    var cmd = User32.TrackPopupMenuEx(systemMenu, User32.TPM_RETURNCMD, screenLocation.X, screenLocation.Y, this.Handle, IntPtr.Zero);

                    ActiveWindow = false;

                    User32.SendMessage(Window.Handle, User32.WM_SYSCOMMAND, cmd, 0);
                }
            }
            else
            {
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

                    if (Parent is Taskbar)
                    {
                        foreach (Control control in Parent.Controls)
                        {
                            if (control is TaskbarProgram program)
                            {
                                program.ActiveWindow = false;
                            }
                        }
                    }

                    ActiveWindow = true;
                }
            }
        }

        public override string ToString() => $"Handle: {Window.Handle}, Title: {Title}";

        protected override bool CancelMouseDown(MouseEventArgs e) => false;
    }
}