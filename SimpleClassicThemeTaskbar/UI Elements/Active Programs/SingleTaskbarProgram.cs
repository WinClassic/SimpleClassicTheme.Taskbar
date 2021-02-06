using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
	public class SingleTaskbarProgram : BaseTaskbarProgram
	{
		public SingleTaskbarProgram()
		{
            Constructor();
        }

        private Process process;
        private Window window;
        private Icon icon;

        public override Process Process { get => process; set { process = value; /*MessageBox.Show(ApplicationEntryPoint.d.GetAppUserModelId(Process.Id));*/ } }
		public override Window Window { get => window; set => window = value; }
        public override string Title { get => window.Title; set => window.Title = value; }
        public override Icon Icon { get => icon; set => icon = value; }
        public override Image IconImage { get { try { return new Icon(Icon, 16, 16).ToBitmap(); } catch { return null; } } }

        public override string GetErrorString()
            => GetBaseErrorString() +
            $"Process: {Process.MainModule.ModuleName} ({Process.Id})\n" +
            $"Window title: {Title}\n" +
            $"Window class: {Window.ClassName}\n" +
            $"Window HWND: {Window.Handle:X8} {(IsWindow(Window.Handle) ? "Valid" : "Invalid")}\n" +
            $"Icon HWND: {Icon.Handle:X8} ({(IsWindow(Icon.Handle) ? "Valid" : "Invalid")})";

		public override bool IsActiveWindow(IntPtr activeWindow)
        {
            bool result = activeWindow == Window.Handle;
            ActiveWindow = result;
            return result;
        }

        public override void FinishOnPaint(PaintEventArgs e) { }

        public override void OnClick(object sender, MouseEventArgs e)
		{
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "handling mouse click";
            if (IsMoving)
            {
                IsMoving = false;
            }
            else if (ActiveWindow)
            {
                ShowWindow(Window.Handle, 6);
            }
            else
            {
                if ((Window.WindowInfo.dwStyle & 0x20000000) > 0)
                    ShowWindow(Window.Handle, 9);
                SetForegroundWindow(Window.Handle);

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

        protected override bool CancelMouseDown(MouseEventArgs e) => false;
        public override string ToString() => $"Handle: {Window.Handle}, Title: {Title}";
    }
}
