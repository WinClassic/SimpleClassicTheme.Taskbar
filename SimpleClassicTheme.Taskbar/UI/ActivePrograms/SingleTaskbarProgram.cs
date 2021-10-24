using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Forms;
using SimpleClassicTheme.Taskbar.Helpers;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar
{
    public enum TaskbarProgramClickAction
    {
        None,
        ShowHide,
        Close,
        ContextMenu,
        NewInstance,
    }

    public class SingleTaskbarProgram : BaseTaskbarProgram
    {
        private Process process;
        private Window window;

        public SingleTaskbarProgram()
        {
            Constructor();
        }

        public SingleTaskbarProgram(Window window) : this()
        {
            Window = window;
            Process = Process.GetProcessById(window.ProcessId);
        }

        public override int MinimumWidth => Config.Default.Renderer.TaskButtonMinimalWidth;
        public override Process Process { get => process; set => process = value; }
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
            $"Window HWND: {Window.Handle:X8} {(IsWindow(Window.Handle) ? "Valid" : "Invalid")}\n" +
            $"Icon HWND: {Icon.Handle:X8} ({(IsWindow(Icon.Handle) ? "Valid" : "Invalid")})";

        public override void OnClick(object sender, MouseEventArgs e)
        {
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "handling mouse click";

            if (IsMoving)
                IsMoving = false;
            else if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt) && e.Button == MouseButtons.Right)
                new IconTest(Window).Show();
            else if (e.Button == MouseButtons.Left)
                PerformClickAction(Config.Default.Tweaks.TaskbarProgramLeftClickAction, e);
            else if (e.Button == MouseButtons.Middle)
                PerformClickAction(Config.Default.Tweaks.TaskbarProgramMiddleClickAction, e);
            else
                PerformClickAction(Config.Default.Tweaks.TaskbarProgramRightClickAction, e);
        }

        public override void OnDoubleClick(object sender, MouseEventArgs e)
        {
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "handling mouse double click";

            if (IsMoving)
                IsMoving = false;
            else if (e.Button == MouseButtons.Left)
                PerformClickAction(Config.Default.Tweaks.TaskbarProgramLeftDoubleClickAction, e);
            else if (e.Button == MouseButtons.Middle)
                PerformClickAction(Config.Default.Tweaks.TaskbarProgramMiddleDoubleClickAction, e);
            else
                PerformClickAction(Config.Default.Tweaks.TaskbarProgramRightDoubleClickAction, e);
        }

        public void PerformClickAction(TaskbarProgramClickAction action, MouseEventArgs e)
        {
            switch (action)
            {
                case TaskbarProgramClickAction.ShowHide:
                    if (ActiveWindow)
                    {
                        _ = ShowWindow(Window.Handle, SW_MINIMIZE);
                    }
                    else
                    {
                        if ((Window.WindowInfo.dwStyle & WS_MINIMIZE) != 0)
                            _ = ShowWindow(Window.Handle, SW_RESTORE);

                        _ = SetForegroundWindow(Window.Handle);

                        if (Parent is Taskbar)
                        {
                            foreach (Control control in Parent.Controls)
                            {
                                if (control is BaseTaskbarProgram program)
                                {
                                    program.ActiveWindow = false;
                                }
                            }
                        }

                        ActiveWindow = true;
                    }
                    break;

                case TaskbarProgramClickAction.Close:
                    SendMessage(Window.Handle, WM_CLOSE, 0, 0);
                    break;

                case TaskbarProgramClickAction.ContextMenu:
                    var systemMenu = GetSystemMenu(Window.Handle, false);

                    if (systemMenu == IntPtr.Zero)
                    {
                        Logger.Instance.Log(LoggerVerbosity.Verbose, "SingleTaskbarProgram", $"Got an empty system menu ({systemMenu:X8})");
                        return;
                    }

                    // Inserting menu items is bugged sadly.
                    // AppendMenu(systemMenu, MF_SEPARATOR, 0x0000, IntPtr.Zero);
                    // AppendMenu(systemMenu, MF_STRING, 0x1337, "Icon Test");

                    // HACK: preserving the pressed down state when the menu is open
                    ActiveWindow = true;

                    _ = ShowWindow(Window.Handle, SW_SHOW);
                    _ = SetForegroundWindow(Window.Handle);

                    var screenLocation = PointToScreen(e.Location);
                    var cmd = TrackPopupMenuEx(systemMenu, TPM_RETURNCMD, screenLocation.X, screenLocation.Y, this.Handle, IntPtr.Zero);

                    ActiveWindow = false;

                    SendMessage(Window.Handle, WM_SYSCOMMAND, cmd, 0);
                    break;

                case TaskbarProgramClickAction.NewInstance:
                    Process.Start(Process.MainModule.FileName);
                    break;
            }
        }

        public override string ToString() => $"Handle: {Window.Handle}, Title: {Title}";

        protected override bool CancelMouseDown(MouseEventArgs e) => false;
    }
}