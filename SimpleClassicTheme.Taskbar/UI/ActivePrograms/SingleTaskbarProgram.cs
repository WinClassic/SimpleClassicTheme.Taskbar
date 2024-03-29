﻿using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Forms;
using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

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

        public override int MinimumWidth => Config.Default.Renderer.TaskButtonMinimalWidth;
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

        public void PerformClickAction(TaskbarProgramClickAction action, MouseEventArgs e)
		{
            switch (action)
			{
                case TaskbarProgramClickAction.ShowHide:
                    if (ActiveWindow)
                    {
                        _ = User32.ShowWindow(Window.Handle, User32.SW_MINIMIZE);
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
                    User32.SendMessage(Window.Handle, User32.WM_CLOSE, 0, 0);
                    break;
                case TaskbarProgramClickAction.ContextMenu:
                    var systemMenu = User32.GetSystemMenu(Window.Handle, false);

                    if (systemMenu == IntPtr.Zero)
                    {
                        Logger.Instance.Log(LoggerVerbosity.Verbose, "SingleTaskbarProgram", $"Got an empty system menu ({systemMenu:X8})");
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
                    break;
                case TaskbarProgramClickAction.NewInstance:
                    Process.Start(Window.Process.MainModule.FileName);
                    break;
			}
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

        public override string ToString() => $"Handle: {Window.Handle}, Title: {Title}";

        protected override bool CancelMouseDown(MouseEventArgs e) => false;
    }
}