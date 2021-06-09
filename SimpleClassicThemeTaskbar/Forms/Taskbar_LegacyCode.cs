﻿/*
 * 
 * This file contains the current stable code for 
 * haandling with taskbar windows (pulling data).
 * The idea is that we add new code to Taskbar.cs 
 * and keep this in place as a backup for when
 * things go wrong with the new system (getting 
 * pushed data).
 * 
 */

using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class Taskbar : Form
    {
        //Function to determine which windows to add to the window list
        private static bool IsAltTabWindow(IntPtr hwnd)
        {
            //If window isn't visible it can't possibly be on the taskbar
            if (!User32.IsWindowVisible(hwnd))
                return false;

            //Check if the OS is Windows 10
            if (Environment.OSVersion.Version.Major == 10)
                //Check if the window is on the current Desktop
                if (!UnmanagedCodeMigration.IsWindowOnCurrentVirtualDesktop(hwnd))
                    return false;

            //Get the root owner of the window
            IntPtr root = User32.GetAncestor(hwnd, 3);

            //If the last active popup of the root owner is NOT this window: don't show it
            //This method is described by Raymond Chen in this blogpost:
            //https://devblogs.microsoft.com/oldnewthing/20071008-00/?p=24863
            if (GetLastActivePopupOfWindow(root) != hwnd)
                return false;

            //Create a Window object
            Window wi = new(hwnd);

            //If it's a tool window: don't show it
            if ((wi.WindowInfo.dwExStyle & 0x00000080L) > 0)
                return false;

            //If it's any of these odd cases: don't show it
            if (string.IsNullOrWhiteSpace(wi.Title) ||                  // Window without a name
                Constants.HiddenClassNames.Contains(wi.ClassName) ||    // Other windows
                (wi.ClassName == "Button" && wi.Title == "Start") ||    // Windows startmenu-button.
                wi.ClassName.StartsWith("WMP9MediaBarFlyout"))          // WMP's "now playing" taskbar-toolbar
                return false;

            //UWP app
            if (wi.ClassName == "ApplicationFrameWindow")
            {
                //Do an API call to see if app isn't cloaked
                _ = DwmApi.DwmGetWindowAttribute(wi.Handle, DwmApi.DWMWINDOWATTRIBUTE.Cloaked, out int d, Marshal.SizeOf(0));

                //If returned value is not 0, the window is cloaked
                if (d > 0)
                {
                    return false;
                }
            }

            //If none of those things failed: Yay, we have a window we should display!
            return true;
        }

        private static IEnumerable<Window> GetTaskbarWindows()
        {
            var trayWindows = UnmanagedHelpers.FilterWindows(IsAltTabWindow);
            return trayWindows.Select(hWnd => new Window(hWnd));
        }

        private IEnumerable<Window> GetTrayWindows()
        {
            var trayWindows = UnmanagedHelpers.FilterWindows((hWnd) =>
            {
                var window = new Window(hWnd);
                var isTrayWindow = window.ClassName == "Shell_TrayWnd" || window.ClassName == "Shell_SecondaryTrayWnd";
                return isTrayWindow && Screen.FromHandle(hWnd).Bounds == Screen.FromHandle(CrossThreadHandle).Bounds;
            });

            return trayWindows.Select(hWnd => new Window(hWnd));
        }

        private void timerUpdateInformation_Tick(object sender, EventArgs e)
        {
            if (!watchLogic)
                timingDebugger.Start();

            //Get the forground window to check some stuff
            IntPtr ForegroundWindow = User32.GetForegroundWindow();
            Window wnd = new(ForegroundWindow);

            //Hide explorer's taskbar(s)
            waitBeforeShow = false;

            var enumTrayWindows = GetTrayWindows();

            foreach (Window w in enumTrayWindows)
                if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                    User32.ShowWindow(w.Handle, 0);

            if (!watchLogic)
                timingDebugger.FinishRegion("Hide Shell_TrayWnd and Shell_SecondaryTrayWnd");

            //The window should only be visible if the active window is not fullscreen (with the exception of the desktop window)
            Screen scr = Screen.FromHandle(wnd.Handle);
            User32.GetWindowRect(wnd.Handle, out RECT rect);
            int width = rect.Right - rect.Left, height = rect.Bottom - rect.Top;
            bool full = width >= scr.Bounds.Width && height >= scr.Bounds.Height;
            if (NeverShow)
                Visible = false;
            else
                Visible = !(full && wnd.ClassName != "Progman" && wnd.ClassName != "WorkerW");

            //If we're not visible, why do anything?
            if (!Visible)
                return;

            if (!watchLogic)
                timingDebugger.FinishRegion("Hide if ForegroundWindow is fullscreen");

            //Obtain task list
            var windows = GetTaskbarWindows();

            if (!watchLogic)
                timingDebugger.FinishRegion("Get the task list");

            //Resize work area
            if (!Dummy) ApplyWorkArea(windows);

            //Make a backup of the current icons
            List<BaseTaskbarProgram> oldList = new(icons);

            //Check if any new window exists, if so: add it
            foreach (Window window in windows)
            {
                if (HasWindow(window.Handle)) continue;
                if (IsBlacklisted(window)) continue;

                var button = CreateTaskbandButton(window);
                icons.Add(button);
            }

            // Create a new list with only the windows that are still open
            var newIcons = GetValidWindows(icons, windows);

            if (!oldList.SequenceEqual(newIcons))
            {
                //Remove controls of the windows that were removed from the list
                RemoveRemainingPrograms(newIcons);

                if (!watchLogic)
                    timingDebugger.FinishRegion("Create controls for all tasks");

                //Create new list for finalized values
                List<BaseTaskbarProgram> programs = new();
                UpdateTaskbarButtons(newIcons, ref programs);

                if (!watchLogic)
                    timingDebugger.FinishRegion("Do grouping");

                icons = programs;
            }

            // Reset taskbar buttons
            // icons.Clear();
            // foreach (BaseTaskbarProgram taskbarProgram in newIcons)
            // {
            //     icons.Add(taskbarProgram);
            // }

            if (!watchLogic)
            {
                watchLogic = true;
                timingDebugger.Stop();
                MessageBox.Show(timingDebugger.ToString());
            }

            //Update UI
            timerUpdateUI_Tick(true, null);
        }

        private void timerUpdateUI_Tick(object sender, EventArgs e)
        {
            if (!watchUI)
            {
                sw.Reset();
                sw.Start();
            }

            UpdateUI();

            if (!watchUI)
            {
                watchUI = true;
                sw.Stop();
                _ = MessageBox.Show($"Watch UI: {sw.Elapsed}");
            }
        }
    }
}