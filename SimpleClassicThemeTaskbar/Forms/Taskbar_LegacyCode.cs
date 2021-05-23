/*
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        private bool EnumWind(IntPtr hWnd, int lParam)
        {
            if (LookingForTray)
            {
                //return false;
                //If looking for the taskbar, check if it is Shell_TrayWnd and if it is in the bound of the current desktop
                Window wi = new(hWnd);
                if (wi.ClassName == "Shell_TrayWnd" || wi.ClassName == "Shell_SecondaryTrayWnd")
                    if (wi.Title != "SCT Shell Window" && Screen.FromHandle(hWnd).Bounds == Screen.FromHandle(CrossThreadHandle).Bounds)
                    {
                        windows.Add(wi);
                        return false;
                    }
            }
            else
            {
                //If you would show the window in alt+tab, show it on the custom taskbar
                if (IsAltTabWindow(hWnd))
                    windows.Add(new Window(hWnd));
            }
            return true;
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
            windows.Clear();
            LookingForTray = true;
            User32.EnumWindowsCallback callback = EnumWind;
            User32.EnumWindows(callback, 0);
            LookingForTray = false;

            foreach (Window w in windows)
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
            windows.Clear();
            User32.EnumWindowsCallback d = EnumWind;
            User32.EnumWindows(d, 0);

            if (!watchLogic)
                timingDebugger.FinishRegion("Get the task list");

            //Resize work area

            if (!Dummy)
            {
                ApplyWorkArea();
            }

            List<BaseTaskbarProgram> oldList = new();
            oldList.AddRange(icons);

            //Check if any new window exists, if so: add it
            foreach (Window z in windows)
            {
                //Get a handle
                IntPtr hWnd = z.Handle;
                //Check if it already exists
                bool exists = false;
                foreach (BaseTaskbarProgram icon in icons)
                    if (icon is SingleTaskbarProgram)
                        if (icon.Window.Handle == hWnd)
                            exists = true;
                        else { }
                    else if (icon is GroupedTaskbarProgram group)
                        if (group.ContainsWindow(hWnd))
                            exists = true;
                if (!exists)
                {
                    //Create the button
                    BaseTaskbarProgram button = new SingleTaskbarProgram
                    {
                        Window = z
                    };
                    button.MouseDown += Taskbar_IconDown;
                    button.MouseMove += Taskbar_IconMove;
                    button.MouseUp += Taskbar_IconUp;

                    User32.GetWindowThreadProcessId(button.Window.Handle, out Integer32 pid);
                    Process p = Process.GetProcessById((int)pid);
                    button.Process = p;

                    //Check if not blacklisted
                    if (BlacklistedClassNames.Contains(z.ClassName) || BlacklistedWindowNames.Contains(z.Title) || BlacklistedProcessNames.Contains(p.ProcessName))
                        button.Dispose();
                    else
                        icons.Add(button);
                }
            }

            //The new list of icons
            List<BaseTaskbarProgram> newIcons = new();

            //Create a new list with only the windows that are still open
            foreach (BaseTaskbarProgram baseIcon in icons)
            {
                if (baseIcon is SingleTaskbarProgram singleIcon)
                {
                    bool contains = false;
                    foreach (Window z in windows)
                        if (z.Handle == singleIcon.Window.Handle)
                        {
                            contains = true;
                            singleIcon.Window = z;
                        }

                    if (contains)
                        newIcons.Add(singleIcon);
                }
                if (baseIcon is GroupedTaskbarProgram groupedIcon)
                {
                    if (groupedIcon.UpdateWindowList(windows))
                    {
                        newIcons.Add(groupedIcon);
                    }
                }
            }

            if (oldList.SequenceEqual(newIcons))
                goto displayWindows;

            //Remove controls of the windows that were removed from the list
            foreach (Control dd in Controls)
            {
                if (dd is BaseTaskbarProgram)
                {
                    BaseTaskbarProgram icon = dd as BaseTaskbarProgram;
                    if (!newIcons.Contains(icon))
                    {
                        icons.Remove(icon);
                        Controls.Remove(icon);
                        icon.Dispose();
                    }
                }
            }

            //Create new list for finalized values
            List<BaseTaskbarProgram> programs = new();
            if (Config.ProgramGroupCheck == ProgramGroupCheck.None)
                goto addAllWindows;

            if (!watchLogic)
                timingDebugger.FinishRegion("Create controls for all tasks");

            //Check for grouping and finalize position values
            foreach (BaseTaskbarProgram taskbarProgram in newIcons)
            {
                try
                {
                    if (taskbarProgram is GroupedTaskbarProgram)
                    {
                        GroupedTaskbarProgram group = taskbarProgram as GroupedTaskbarProgram;
                        if (group.ProgramWindows.Count == 1)
                        {
                            //Create the button
                            BaseTaskbarProgram button = new SingleTaskbarProgram
                            {
                                Window = group.ProgramWindows[0].Window
                            };
                            button.MouseDown += Taskbar_IconDown;
                            button.MouseMove += Taskbar_IconMove;
                            button.MouseUp += Taskbar_IconUp;
                            button.Height = Height;

                            User32.GetWindowThreadProcessId(button.Window.Handle, out Integer32 pid);
                            Process p = Process.GetProcessById((int)pid);
                            button.Process = p;

                            //Add it to the list
                            programs.Add(button);

                            group.Dispose();
                            continue;
                        }
                        if (group.ProgramWindows.Count == 0)
                        {
                            group.Dispose();
                            continue;
                        }
                        BaseTaskbarProgram[] pr = new BaseTaskbarProgram[programs.Count];
                        programs.CopyTo(pr);
                        foreach (BaseTaskbarProgram programBase in pr)
                        {
                            if (programBase is SingleTaskbarProgram program)
                            {
                                bool sameProcessExists = false;
                                bool sameExecutableExists = false;
                                bool sameModuleNameExists = false;
                                if (group.Process.Id == program.Process.Id)
                                {
                                    sameProcessExists = true;
                                }
                                if (group.Process.MainModule.FileName == program.Process.MainModule.FileName)
                                {
                                    sameExecutableExists = true;
                                }
                                if (group.Process.MainModule.ModuleName == program.Process.MainModule.ModuleName)
                                {
                                    sameModuleNameExists = true;
                                }
                                if ((Config.ProgramGroupCheck == ProgramGroupCheck.Process && sameProcessExists) ||
                                    (Config.ProgramGroupCheck == ProgramGroupCheck.FileNameAndPath && sameExecutableExists) ||
                                    (Config.ProgramGroupCheck == ProgramGroupCheck.ModuleName && sameModuleNameExists))
                                {
                                    programs.Remove(program);
                                    group.ProgramWindows.Add(program);
                                }
                            }
                        }
                        programs.Add(taskbarProgram);
                    }
                    else if (taskbarProgram is SingleTaskbarProgram)
                    {
                        SingleTaskbarProgram icon = taskbarProgram as SingleTaskbarProgram;
                        bool sameProcessExists = false;
                        bool sameExecutableExists = false;
                        bool sameModuleNameExists = false;
                        BaseTaskbarProgram sameThing = null;
                        foreach (BaseTaskbarProgram program in programs)
                        {
                            if (icon.Process.Id == program.Process.Id)
                            {
                                sameProcessExists = true;
                                sameThing = program;
                            }
                            if (icon.Process.MainModule.FileName == program.Process.MainModule.FileName)
                            {
                                sameExecutableExists = true;
                                sameThing = program;
                            }
                            if (icon.Process.MainModule.ModuleName == program.Process.MainModule.ModuleName)
                            {
                                sameModuleNameExists = true;
                                sameThing = program;
                            }
                        }

                        if ((Config.ProgramGroupCheck == ProgramGroupCheck.Process && sameProcessExists) ||
                            (Config.ProgramGroupCheck == ProgramGroupCheck.FileNameAndPath && sameExecutableExists) ||
                            (Config.ProgramGroupCheck == ProgramGroupCheck.ModuleName && sameModuleNameExists))
                        {
                            if (sameThing is GroupedTaskbarProgram)
                            {
                                GroupedTaskbarProgram group = sameThing as GroupedTaskbarProgram;
                                if (!group.ProgramWindows.Contains(icon))
                                {
                                    icon.IsMoving = false;
                                    group.ProgramWindows.Add(icon);
                                }
                            }
                            else
                            {
                                GroupedTaskbarProgram group = new();
                                Controls.Remove(sameThing);
                                programs.Remove(sameThing);
                                group.MouseDown += Taskbar_IconDown;
                                group.ProgramWindows.Add(sameThing as SingleTaskbarProgram);
                                group.ProgramWindows.Add(icon);
                                icon.IsMoving = false;
                                programs.Add(group);
                            }
                            //foreach (TaskbarProgram p in programs)
                            //    Console.Write($"- {p.Process.ProcessName} ({programs.IndexOf(p)})");
                            //Console.WriteLine($" / {index} -> {programs.IndexOf(icon)}");
                            //Console.WriteLine($"{index} -> {programs.IndexOf(icon)}");
                        }
                        else
                        {
                            programs.Add(icon);
                        }
                    }
                }
                catch (Exception ex)
                {
                    programs.Clear();
#if DEBUG
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
#endif
                    goto addAllWindows;
                }
            }

            if (!watchLogic)
                timingDebugger.FinishRegion("Do grouping");

            icons = programs;
            goto displayWindows;

        addAllWindows:
            icons.Clear();
            foreach (BaseTaskbarProgram taskbarProgram in newIcons)
            {
                icons.Add(taskbarProgram);
            }

        //Re-display all windows (except heldDownButton)
        displayWindows:

            if (!watchLogic)
                timingDebugger.FinishRegion("Systray");

            //Update quick-launch
            if (Primary)
                quickLaunch1.UpdateIcons();

            if (!watchLogic)
                timingDebugger.FinishRegion("Quick Launch");

            //Put divider in correct place
            verticalDivider3.Location = new Point(systemTray1.Location.X - 9, verticalDivider3.Location.Y);

            //Update clock
            systemTray1.UpdateTime();

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
            //Get the forground window to check some stuff
            IntPtr ForegroundWindow = User32.GetForegroundWindow();
            Window wnd = new(ForegroundWindow);

            //Check if the foreground window was the start menu
            startButton1.UpdateState(wnd);

            //Put left side controls in the correct place
            quickLaunch1.Location = new Point(startButton1.Location.X + startButton1.Width + 2, 1);

            //Calculate availabe space in taskbar and then divide that space over all programs
            int startX = quickLaunch1.Location.X + quickLaunch1.Width + 4;
            int programWidth = Primary ? Config.TaskbarProgramWidth + Config.SpaceBetweenTaskbarIcons : 24;

            int availableSpace = verticalDivider3.Location.X - startX - 6;
            availableSpace += Config.SpaceBetweenTaskbarIcons;
            if (icons.Count > 0 && availableSpace / icons.Count > programWidth)
                availableSpace = icons.Count * programWidth;
            int x = startX;
            int iconWidth = icons.Count > 0 ? (int)Math.Floor((double)availableSpace / icons.Count) - Config.SpaceBetweenTaskbarIcons : 01;
            int maxX = verticalDivider3.Location.X - iconWidth;

            if (sender is Boolean a && a == true)
                goto displayWindows;
            else

                if (!watchUI)
            {
                watchUI = true;
                sw.Stop();
                _ = MessageBox.Show($"Watch UI: {sw.Elapsed}");
            }
            return;

        //Re-display all windows (except heldDownButton)
        displayWindows:
            foreach (BaseTaskbarProgram icon in icons)
            {
                icon.Width = Math.Max(icon.MinimumWidth, iconWidth);
                if (icon == heldDownButton)
                {
                    x += icon.Width + Config.SpaceBetweenTaskbarIcons;
                    continue;
                }
                _ = icon.IsActiveWindow(ForegroundWindow);
                icon.Location = new Point(x, 0);
                icon.Icon = GetAppIcon(icon.Window);
                if (!Controls.Contains(icon))
                {
                    Controls.Add(icon);
                    icon.Width = iconWidth;
                    icon.Height = Height;
                    verticalDivider3.BringToFront();
                }
                x += icon.Width + Config.SpaceBetweenTaskbarIcons;
                icon.Visible = true;
            }
            if (heldDownButton != null)
            {
                heldDownButton.BringToFront();
                verticalDivider3.BringToFront();
            }

            taskArea = new Range(new Index(startX), new Index(maxX));
            taskIconWidth = iconWidth;
            if (!watchUI)
            {
                watchUI = true;
                sw.Stop();
                _ = MessageBox.Show($"Watch UI+: {sw.Elapsed}");
            }
        }
    }
}
