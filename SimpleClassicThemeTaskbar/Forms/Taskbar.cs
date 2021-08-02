using Microsoft.Win32;

using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class Taskbar : Form
    {
        public static IntPtr lastOpenWindow;
        public static bool waitBeforeShow = false;
        public bool busy = false;
        public bool CanInvoke = false;
        public BaseTaskbarProgram heldDownButton;
        public int heldDownOriginalX = 0;
        public int mouseOriginalX = 0;
        public bool NeverShow = false;
        public bool Primary = true;
        public bool selfClose = false;
        private readonly List<string> BlacklistedClassNames = new();
        private readonly List<string> BlacklistedProcessNames = new();
        private readonly List<string> BlacklistedWindowNames = new();

        //private Thread BackgroundThread;
        private IntPtr CrossThreadHandle;
        private bool dummy;
        private List<BaseTaskbarProgram> icons = new();
        private Range taskArea;
        private int taskIconWidth;
        
        private readonly TimingDebugger uiTiming = new();
        private readonly TimingDebugger logicTiming = new();
        private bool watchLogic = true;
        private bool watchUI = true;
        
        //private User32.WindowsHookProcedure hookProcedure;
        private int WM_SHELLHOOKMESSAGE = -1;

        /// <summary>
        /// Sets whether this taskbar should behave a "decoration piece", this disables some logic.
        /// </summary>
        public bool Dummy
        {
            get => dummy;
            set
            {
                dummy = value;

                if (dummy)
                {
                    Enabled = false;
                }
            }
        }

        public IEnumerable<BaseTaskbarProgram> Programs
        {
            get
            {
                foreach (var control in Controls)
                {
                    if (control is BaseTaskbarProgram taskbarProgram)
                    {
                        yield return taskbarProgram;
                    }
                }
            }
        }

        /// <summary>
        /// Make sure form doesnt show in alt tab and that it shows up on all virtual desktops
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        //Constructor
        public Taskbar(bool isPrimary)
        {
            //Load filters
            foreach (string filter in Config.Default.TaskbarProgramFilter.Split('*'))
            {
                if (filter == "")
                    continue;
                string[] filterParts = filter.Split('|');
                if (filterParts[1] == "ClassName")
                {
                    BlacklistedClassNames.Add(filterParts[0]);
                }
                else if (filterParts[1] == "WindowName")
                {
                    BlacklistedWindowNames.Add(filterParts[0]);
                }
                else if (filterParts[1] == "ProcessName")
                {
                    BlacklistedProcessNames.Add(filterParts[0]);
                }
            }

            //Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            Primary = isPrimary;
            
            //Initialize thingies
            InitializeComponent();
            HandleCreated += delegate { CrossThreadHandle = Handle; };
            systemTray1.SizeChanged += delegate { UpdateUI(); UpdateUI(); };
            TopLevel = true;

            //Fix height according to renderers preferences
            Height = Config.Default.Renderer.TaskbarHeight;
            startButton1.Height = Height;
            systemTray1.Height = Height;
            quickLaunch1.Height = Height;

            //Make sure programs are aware of the new work area
            if (isPrimary)
            {
                var windows = GetTaskbarWindows();
                foreach (Window w in windows)
                {
                    //WM_WININICHANGE - SPI_SETWORKAREA
                    _ = User32.PostMessage(w.Handle, 0x001A, 0x002F, 0);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e) => Config.Default.Renderer.DrawTaskBar(this, e.Graphics);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
			{
                case (int)User32.WM_ENDSESSION:
                    ApplicationEntryPoint.ExitSCTT();
                    break;

                case (int)User32.WM_QUERYENDSESSION:
                    m.Result = new IntPtr(1);
                    break;

                case Constants.WM_SCT:
                    switch (m.WParam.ToInt32())
                    {
                        case Constants.SCTWP_EXIT:
                            if (ApplicationEntryPoint.SCTCompatMode || m.LParam.ToInt32() == Constants.SCTLP_FORCE)
                            {
                                ApplicationEntryPoint.ExitSCTT();
                            }
                            break;

                        case Constants.SCTWP_ISMANAGED:
                            m.Result = new IntPtr(1);
                            return;

                        case Constants.SCTWP_ISSCT:
                            m.Result = new IntPtr(ApplicationEntryPoint.SCTCompatMode ? 1 : 0);
                            return;
                    }
                    break;

                default:
                    if (m.Msg == (int)WM_SHELLHOOKMESSAGE)
                    {
                        HookProcedure((User32.ShellEvents)m.WParam, m.LParam, IntPtr.Zero);
                    }
                    break;

            }

            base.WndProc(ref m);
        }

        //Initialize stuff
        private void Taskbar_Load(object sender, EventArgs e)
        {
            //TODO: Add an option to registry tweak classic alt+tab
            quickLaunch1.Disabled = (!Primary) || (!Config.Default.EnableQuickLaunch);
            quickLaunch1.UpdateIcons();

            // Create shell hook
            if (!Config.Default.EnableActiveTaskbar)
            {
                if (!User32.RegisterShellHookWindow(Handle))
                //hookProcedure = new User32.WindowsHookProcedure(HookProcedure);
                //if (User32.SetWindowsHookEx(User32.ShellHookId.WH_SHELL, hookProcedure, Marshal.GetHINSTANCE(typeof(Taskbar).Module), /*Kernel32.GetCurrentThreadId()*/0) == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Logger.Log(LoggerVerbosity.Basic, "Taskbar/Constructor", $"Failed to create Shell Hook. ({errorCode:X8})");
                    throw new Win32Exception(errorCode);
                }
                WM_SHELLHOOKMESSAGE = User32.RegisterWindowMessage("SHELLHOOK");
                if (WM_SHELLHOOKMESSAGE == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Logger.Log(LoggerVerbosity.Basic, "Taskbar/Constructor", $"Failed to register Shell Hook message. ({errorCode:X8})");
                    throw new Win32Exception(errorCode);
                }

                EnumerateWindows();
                timerUpdate.Start();
            }
            else
                timerUpdateInformation.Start();
        }

        private void Taskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!selfClose)
            {
                //If we don't close ourselves, show the shutdown dialog
                var hWnd = User32.FindWindow("Shell_TrayWnd", "");
                Keyboard.KeyPress(hWnd, Keys.Menu, Keys.F4);
                e.Cancel = true;
            }
            else
            {
                //Show taskbar
                _ = User32.ShowWindow(User32.FindWindow("Shell_TrayWnd", ""), 5);
            }
        }

        private void Taskbar_IconDown(object sender, MouseEventArgs e)
        {
            if (((Control)sender).Parent == this)
            {
                heldDownButton = (BaseTaskbarProgram)sender;
                heldDownOriginalX = heldDownButton.Location.X;
                mouseOriginalX = Cursor.Position.X;
            }
        }

        private void Taskbar_IconMove(object sender, MouseEventArgs e)
        {
            //See if we're moving, if so calculate new position, if we finished calculate new position
            if (heldDownButton != null)
            {
                if (Math.Abs(mouseOriginalX - Cursor.Position.X) > 5)
                    heldDownButton.IsMoving = true;

                Point p = new(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownButton.Location.Y);
                heldDownButton.Location = new Point(Math.Max(taskArea.Start.Value, Math.Min(p.X, taskArea.End.Value)), p.Y);
                int newIndex = (taskArea.Start.Value - heldDownButton.Location.X - ((taskIconWidth + Config.Default.Tweaks.SpaceBetweenTaskbarIcons) / 2)) / (taskIconWidth + Config.Default.Tweaks.SpaceBetweenTaskbarIcons) * -1;
                if (newIndex < 0) newIndex = 0;
                if (newIndex != icons.IndexOf(heldDownButton))
                {
                    _ = icons.Remove(heldDownButton);
                    icons.Insert(Math.Min(icons.Count, newIndex), heldDownButton);
                }

                heldDownButton.BringToFront();

                if ((MouseButtons & MouseButtons.Left) == 0)
                    heldDownButton = null;

                int x = taskArea.Start.Value;
                foreach (BaseTaskbarProgram icon in icons)
                {
                    if (icon == heldDownButton)
                    {
                        x += icon.Width + Config.Default.Tweaks.SpaceBetweenTaskbarIcons;
                        continue;
                    }
                    icon.Location = new Point(x, 0);
                    x += icon.Width + Config.Default.Tweaks.SpaceBetweenTaskbarIcons;
                }
            }
        }

        private void Taskbar_IconUp(object sender, MouseEventArgs e)
        {
            heldDownButton = null;
            UpdateUI();
            UpdateUI();
        }

        private void Taskbar_MouseClick(object sender, MouseEventArgs e)
        {
            //Open context menu
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = ConstructTaskbarContextMenu();

                SystemContextMenu menu = SystemContextMenu.FromToolStripItems(contextMenu.Items);

                Point location = PointToScreen(e.Location);
                menu.Show(Handle, location.X, location.Y);

                User32.DestroyMenu(contextMenu.Handle);
                contextMenu.Dispose();
            }
            else if (e.Button == MouseButtons.Left && e.Location.X == Width - 1)
            {
                Keyboard.KeyPress(Keys.LWin, Keys.D);
            }
        }

        private ToolStripMenuItem ConstructDebuggingMenu()
        {
            var debuggingItem = new ToolStripMenuItem("&Debugging");

            debuggingItem.DropDownItems.Add(new ToolStripMenuItem("Watch UI", null, (_, __) =>
            {
                uiTiming.Reset();
                watchUI = !watchUI;
            }));

            debuggingItem.DropDownItems.Add(new ToolStripMenuItem("Watch Logic", null, (_, __) =>
            {
                logicTiming.Reset();
                watchLogic = !watchLogic;
            }));

            return debuggingItem;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            // Get the foreground window to set the ActiveWindow property on the taskbar items
            IntPtr ForegroundWindow = User32.GetForegroundWindow();
            foreach (BaseTaskbarProgram taskbarProgram in icons)
                taskbarProgram.ActiveWindow = taskbarProgram.Window.Handle == ForegroundWindow;

            // Check if the foreground window was the start menu
            startButton1.UpdateState(new Window(ForegroundWindow));

            if (Primary)
            {
                systemTray1.UpdateTime();

                using (uiTiming.StartRegion("Updating System Tray icons"))
                {
                    systemTray1.UpdateIcons();
                }

                quickLaunch1.Location = new Point(startButton1.Location.X + startButton1.Width + 2, 1);

                using (uiTiming.StartRegion("Update Quick Launch icons"))
                {
                    quickLaunch1.UpdateIcons();
                }
            }

            //Put left side controls in the correct place
            quickLaunch1.Location = new Point(startButton1.Location.X + startButton1.Width + 2, 1);
        }

        private IntPtr HookProcedure(User32.ShellEvents nCode, IntPtr wParam, IntPtr lParam)
        {
            Logger.Log(LoggerVerbosity.Verbose, "Taskbar/HookProcedure", $"Call parameters: {nCode}, {wParam:X8}, {lParam:X8}");

            if (nCode < 0)
                return User32.CallNextHookEx(IntPtr.Zero, (User32.ShellEvents)nCode, wParam, lParam);

            switch (nCode)
            {
                // Create a button for the new window
                case User32.ShellEvents.HSHELL_WINDOWCREATED:
                    HandleWindowCreated(wParam);
                    break;
                
                case User32.ShellEvents.HSHELL_WINDOWACTIVATED:
                    HandleWindowActivated(wParam);
                    break;
                
                case User32.ShellEvents.HSHELL_WINDOWDESTROYED:
                    HandleWindowDestroyed(wParam);
                    break;

                default:
                    Logger.Log(LoggerVerbosity.Verbose, "Taskbar/HookProcedure", $"Cannot handle {(int)nCode} (W:{wParam:X8}, L:{lParam:X8})");
                    break;
            }

            return User32.CallNextHookEx(IntPtr.Zero, (User32.ShellEvents)nCode, wParam, lParam);
        }

        private void HandleWindowDestroyed(IntPtr wParam)
        {
            RemoveTaskbandButton(wParam);
            
            UpdateUI();
            UpdateUI();
        }

        private void HandleWindowActivated(IntPtr wParam)
        {
            UpdateActiveWindow(wParam);
        }

        /// <summary>
        /// Highlights only taskband buttons that match the provided window handle.
        /// </summary>
        private void UpdateActiveWindow(IntPtr hWnd)
        {
            foreach (BaseTaskbarProgram taskbarProgram in icons)
            {
                if (taskbarProgram is GroupedTaskbarProgram programGroup)
                    programGroup.IsActiveWindow(hWnd);
                else
                    taskbarProgram.ActiveWindow = taskbarProgram.Window.Handle == hWnd;
            }
        }

        private bool IsBlacklisted(Window window)
        {
            if (BlacklistedClassNames.Contains(window.ClassName))
            {
                return true;
            }

            if (BlacklistedWindowNames.Contains(window.Title))
            {
                return true;
            }

            if (BlacklistedProcessNames.Contains(window.Process.ProcessName))
            {
                return true;
            }

            // UWP app
            if (window.ClassName == "ApplicationFrameWindow")
            {
                //Do an API call to see if app isn't cloaked
                _ = DwmApi.DwmGetWindowAttribute(window.Handle, DwmApi.DWMWINDOWATTRIBUTE.Cloaked, out var d, Marshal.SizeOf(0));

                //If returned value is not 0, the window is cloaked
                if (d > 0)
                {
                    return false;
                }
            }

            return false;
        }

        private void HandleWindowCreated(IntPtr wParam)
        {
            if ((Environment.OSVersion.Version.Major >= 10 && !UnmanagedCodeMigration.IsWindowOnCurrentVirtualDesktop(wParam)))
            {
                return;
            }

            var window = new Window(wParam);
            if (IsBlacklisted(window))
            {
                return;
            }

            var icon = CreateTaskbandButton(window);
            Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Adding window {icon.Title}.");
            BaseTaskbarProgram sameThing = icons.Where((p) => IsGroupConditionMet(p, icon)).FirstOrDefault();

            // No group
            if (sameThing == null)
            {
                icons.Add(icon);
            }
            else if (sameThing is GroupedTaskbarProgram group)
            {
                if (!group.ProgramWindows.Contains(icon))
                {
                    icon.IsMoving = false;
                    group.ProgramWindows.Add(icon);
                }
            }
            else
            {
                GroupedTaskbarProgram newGroup = new();
                Controls.Remove(sameThing);
                icons.Remove(sameThing);
                newGroup.MouseMove += Taskbar_IconMove;
                newGroup.MouseDown += Taskbar_IconDown;
                newGroup.MouseUp += Taskbar_IconUp;
                newGroup.ProgramWindows.Add(sameThing as SingleTaskbarProgram);
                newGroup.ProgramWindows.Add(icon);
                icon.IsMoving = false;
                icons.Add(newGroup);
            }

            UpdateUI();
            UpdateUI();
        }

        private static IEnumerable<BaseTaskbarProgram> GetValidWindows(IEnumerable<BaseTaskbarProgram> icons, IEnumerable<Window> windows)
        {
            foreach (BaseTaskbarProgram baseIcon in icons)
            {
                if (baseIcon is SingleTaskbarProgram singleIcon)
                {
                    foreach (Window window in windows)
                    {
                        if (window.Handle != singleIcon.Window.Handle)
                            continue;

                        singleIcon.Window = window;
                        yield return singleIcon;

                        break;
                    }
                }
                else if (baseIcon is GroupedTaskbarProgram groupedIcon)
                {
                    if (!groupedIcon.UpdateWindowList(windows))
                        continue;

                    yield return groupedIcon;
                }
            }
        }

        internal void EnumerateWindows()
		{
            // Obtain task list
            var windows = GetTaskbarWindows();

            if (!Dummy)
			{
                // Hide explorer's taskbar(s)
                waitBeforeShow = false;

                var enumTrayWindows = GetTrayWindows();

                foreach (Window w in enumTrayWindows)
                    if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                        User32.ShowWindow(w.Handle, 0);

                // Resize work area
                ApplyWorkArea(windows);
            }


            // Clear icon list as this is a full enumeration
            icons.Clear();

            //Check if any new window exists, if so: add it
            foreach (Window z in windows)
            {
                //Check if not blacklisted
                if (IsBlacklisted(z))
                    continue;

                //Create the button
                BaseTaskbarProgram button = CreateTaskbandButton(z);
                icons.Add(button);
                Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Adding window {button.Title}.");
            }

            //The new list of icons
            List<BaseTaskbarProgram> newIcons = new(icons);

            // Remove all currently displayed controls
            Clear();

            logicTiming.FinishRegion("Create controls for all tasks");

            //Create new list for finalized values
            List<BaseTaskbarProgram> programs = new();

            UpdateTaskbarButtons(newIcons, programs);

            UpdateUI();
            UpdateUI();
        }

        // HACK: This is ref based for now since I have no idea what can go away or how it should be changed.
        private void UpdateTaskbarButtons(IEnumerable<BaseTaskbarProgram> newIcons, List<BaseTaskbarProgram> programs)
        {
            if (Config.Default.Tweaks.ProgramGroupCheck == ProgramGroupCheck.None)
            {
                icons = newIcons.ToList();
                return;
            }
            
            //Check for grouping and finalize position values
            foreach (BaseTaskbarProgram taskbarProgram in newIcons)
            {
                if (taskbarProgram is GroupedTaskbarProgram)
                {
                    var group = taskbarProgram as GroupedTaskbarProgram;
                    programs = UpdateTaskbarGroup(programs, taskbarProgram, group);
                }
                else if (taskbarProgram is SingleTaskbarProgram icon)
                {
                    BaseTaskbarProgram sameThing = programs.Where((p) => IsGroupConditionMet(p, icon)).FirstOrDefault();

                    // No group
                    if (sameThing == null)
                    {
                        programs.Add(icon);
                        continue;
                    }

                    if (sameThing is GroupedTaskbarProgram group)
                    {
                        if (!group.ProgramWindows.Contains(icon))
                        {
                            icon.IsMoving = false;
                            group.ProgramWindows.Add(icon);
                        }
                    }
                    else
                    {
                        GroupedTaskbarProgram newGroup = new();
                        Controls.Remove(sameThing);
                        programs.Remove(sameThing);
                        newGroup.MouseMove += Taskbar_IconMove;
                        newGroup.MouseDown += Taskbar_IconDown;
                        newGroup.MouseUp += Taskbar_IconUp;
                        newGroup.ProgramWindows.Add(sameThing as SingleTaskbarProgram);
                        newGroup.ProgramWindows.Add(icon);
                        icon.IsMoving = false;
                        programs.Add(newGroup);
                    }
                }
            }

            icons = programs;
        }

        private List<BaseTaskbarProgram> UpdateTaskbarGroup(List<BaseTaskbarProgram> programs, BaseTaskbarProgram taskbarProgram, GroupedTaskbarProgram group)
        {
            if (group.ProgramWindows.Count < 2)
            {
                DissolveGroupButton(ref programs, group);
            }
            else
            {
                BaseTaskbarProgram[] pr = new BaseTaskbarProgram[programs.Count];
                programs.CopyTo(pr);
                foreach (BaseTaskbarProgram programBase in pr)
                {
                    if (programBase is SingleTaskbarProgram program && IsGroupConditionMet(group, program))
                    {
                        programs.Remove(program);
                        group.ProgramWindows.Add(program);
                    }
                }
                programs.Add(taskbarProgram);
            }

            return programs;
        }

        private void RemoveRemainingPrograms(IEnumerable<BaseTaskbarProgram> programs)
        {
            foreach (var program in Programs)
            {
                if (!programs.Contains(program))
                {
                    DisposeTaskbarProgram(program);
                }
            }
        }

        private bool HasWindow(IntPtr hWnd)
        {
            foreach (BaseTaskbarProgram icon in icons)
            {
                if (icon is SingleTaskbarProgram && icon.Window.Handle == hWnd)
                {
                    return true;
                }
                else if (icon is GroupedTaskbarProgram group && group.ContainsWindow(hWnd))
                {
                    return true;
                }
            }

            return false;
        }

        private void DissolveGroupButton(ref List<BaseTaskbarProgram> programs, GroupedTaskbarProgram group)
        {
            if (group.ProgramWindows.Count == 1)
            {
                var button = CreateTaskbandButton(group.ProgramWindows[0].Window);

                //Add it to the list
                programs.Add(button);
            }

            group.Dispose();
        }

        private void Clear()
        {
            Control[] controls = new Control[Controls.Count];
            Controls.CopyTo(controls, 0);
            foreach (Control control in controls)
            {
                if (control is BaseTaskbarProgram taskbarProgram)
                {
                    DisposeTaskbarProgram(taskbarProgram);
                }
            }
        }

        private SingleTaskbarProgram CreateTaskbandButton(Window window)
        {
            SingleTaskbarProgram button = new()
            {
                Window = window,
                Process = window.Process,
            };

            button.MouseDown += Taskbar_IconDown;
            button.MouseMove += Taskbar_IconMove;
            button.MouseUp += Taskbar_IconUp;
            button.Height = Height;

            return button;
        }

        /// <summary>
        /// Removes the provided <paramref name="hWnd"/> from the taskbar.
        /// </summary>
        /// <param name="hWnd">The window handle to look for.</param>
        private void RemoveTaskbandButton(IntPtr hWnd)
        {
            List<BaseTaskbarProgram> referenceList = new(icons);
            foreach (BaseTaskbarProgram taskbarProgram in referenceList)
            {
                bool remove = false;

                if (taskbarProgram is SingleTaskbarProgram singleProgram)
                {
                    remove = singleProgram.Window.Handle == hWnd;
                }
                else if (taskbarProgram is GroupedTaskbarProgram groupedProgram)
                {
                    if (remove = !groupedProgram.RemoveWindow(hWnd))
                    {
                        if (groupedProgram.ProgramWindows.Count == 1)
                        {
                            BaseTaskbarProgram seperatedProgram = CreateTaskbandButton(groupedProgram.ProgramWindows[0].Window);

                            icons.Insert(icons.IndexOf(groupedProgram), seperatedProgram);
                        }
                    }
                }

                if (remove)
                {
                    DisposeTaskbarProgram(taskbarProgram);
                }
            }
        }

        private void DisposeTaskbarProgram(BaseTaskbarProgram program)
        {
            Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Deleting window {program.Title}.");
            Controls.Remove(program);
            icons.Remove(program);
            program.Dispose();
        }

        private static bool IsGroupConditionMet(BaseTaskbarProgram a, BaseTaskbarProgram b)
        {
            var aKey = GetGroupKey(a.Window);
            var bKey = GetGroupKey(b.Window);

            if (aKey == null || bKey == null)
            {
                return false;
            }

            return aKey == bKey;
        }

        private static string GetGroupKey(Window window)
        {
            try
            {
                if (window.Process.HasExited)
                {
                    return null;
                }

                return Config.Default.Tweaks.ProgramGroupCheck switch
                {
                    ProgramGroupCheck.Process => window.Process.Id.ToString(),
                    ProgramGroupCheck.FileNameAndPath => Kernel32.GetProcessFileName(window.Process.Id),
                    ProgramGroupCheck.ModuleName => Kernel32.GetProcessModuleName(window.Process.Id),
                };
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
            {
                // If we got an access denied exception we catch it and return false.
                // Otherwise we re-throw the exception
                Logger.Log(LoggerVerbosity.Basic, "Taskbar/Groups", $"Failed to compare taskbar programs: {ex}");
                return null;
            }
        }

        private void LayoutTaskbandButtons()
        {
            // Calculate availabe space in taskbar and then divide that space over all programs
            int startX = quickLaunch1.Location.X + quickLaunch1.Width + 4;
            int programWidth = Primary ? Config.Default.Tweaks.TaskbarProgramWidth + Config.Default.Tweaks.SpaceBetweenTaskbarIcons : 24;

            int availableSpace = verticalDivider3.Location.X - startX - 6;
            availableSpace += Config.Default.Tweaks.SpaceBetweenTaskbarIcons;

            if (icons.Count > 0 && availableSpace / icons.Count > programWidth)
                availableSpace = icons.Count * programWidth;

            int x = startX;
            int iconWidth = icons.Any() ? (int)Math.Floor((double)availableSpace / icons.Count) - Config.Default.Tweaks.SpaceBetweenTaskbarIcons : 1;
            int maxX = verticalDivider3.Location.X - iconWidth;

            // Re-display all windows (except heldDownButton)
            foreach (BaseTaskbarProgram icon in icons)
            {
                icon.Width = Math.Max(icon.MinimumWidth, iconWidth);
                if (icon == heldDownButton)
                {
                    x += icon.Width + Config.Default.Tweaks.SpaceBetweenTaskbarIcons;
                    continue;
                }

                icon.Location = new Point(x, 0);
                icon.Icon = GetAppIcon(icon.Window);
                icon.Width = iconWidth;
                icon.Height = Height;
                icon.Visible = true;

                verticalDivider3.BringToFront();

                x += icon.Width + Config.Default.Tweaks.SpaceBetweenTaskbarIcons;

                if (!Controls.Contains(icon))
                {
                    Controls.Add(icon);
                }

                icon.Invalidate();
            }

            if (heldDownButton != null)
            {
                heldDownButton.BringToFront();
                verticalDivider3.BringToFront();
            }

            taskArea = new Range(new Index(startX), new Index(maxX));
            taskIconWidth = iconWidth;

            Invalidate();
        }

        private void UpdateUI()
        {
            startButton1.UpdateState(new Window(User32.GetForegroundWindow()));

            if (Primary)
            {
                systemTray1.UpdateTime();

                using (uiTiming.StartRegion("Updating System Tray icons"))
                {
                    systemTray1.UpdateIcons();
                }

                quickLaunch1.Location = new Point(startButton1.Location.X + startButton1.Width + 2, 1);

                using (uiTiming.StartRegion("Update Quick Launch icons"))
                {
                    quickLaunch1.UpdateIcons();
                }
            }

            verticalDivider3.Location = new Point(systemTray1.Location.X - 9, verticalDivider3.Location.Y);

            var foregroundWindow = User32.GetForegroundWindow();
            UpdateActiveWindow(foregroundWindow);

            LayoutTaskbandButtons();

            Invalidate();
        }

        private void ApplyWorkArea(IEnumerable<Window> windows)
        {
            using var _ = logicTiming.StartRegion("Resize work area");

            Screen screen = Screen.FromHandle(CrossThreadHandle);
            Rectangle rct = screen.Bounds;
            rct.Height -= Height;
            Point desiredLocation = new(rct.Left, rct.Bottom);

            if (!screen.WorkingArea.Equals(rct))
            {
                var windowHandles = windows.Select(a => a.Handle).ToArray();
                UnmanagedCodeMigration.SetWorkingArea(rct, Environment.OSVersion.Version.Major < 10, windowHandles);
            }

            if (!Location.Equals(desiredLocation))
                Location = desiredLocation;
        }

        private ContextMenuStrip ConstructTaskbarContextMenu()
        {
            ContextMenuStrip contextMenu = new();

            if (Config.Default.Tweaks.EnableDebugging)
            {
                contextMenu.Items.Add(ConstructDebuggingMenu());
            }

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&Toolbars") { Enabled = false },
                new ToolStripSeparator(),
                new ToolStripMenuItem("Ca&scade Windows", null, (_, __) => {
                    User32.CascadeWindows(IntPtr.Zero, User32.MDITILE_ZORDER, IntPtr.Zero, 0, IntPtr.Zero);
                }),
                new ToolStripMenuItem("Tile Windows &Horizontally", null, (_, __) => {
                    User32.TileWindows(IntPtr.Zero, User32.MDITILE_HORIZONTAL, IntPtr.Zero, 0, IntPtr.Zero);
                }),
                new ToolStripMenuItem("Tile Windows V&ertically", null, (_, __) => {
                    User32.TileWindows(IntPtr.Zero, User32.MDITILE_VERTICAL, IntPtr.Zero, 0, IntPtr.Zero);
                }),
                new ToolStripMenuItem("&Show the Desktop", null, (_, __) => Keyboard.KeyPress(Keys.LWin, Keys.D)),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Tas&k Manager", null, (_, __) => Process.Start(new ProcessStartInfo("taskmgr") { UseShellExecute = true })),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Lock the Taskbar") { Enabled = false },
                new ToolStripMenuItem("P&roperties", null, (_, __) => {
                    new Settings().Show();
                }),
                new ToolStripMenuItem("&Exit SCT Taskbar", null, (_, __) => ApplicationEntryPoint.ExitSCTT()) { Available = ShouldShowExit() }
            });

            return contextMenu;
        }

        private bool ShouldShowExit()
        {
            if (Config.Default.ExitMenuItemCondition == ExitMenuItemCondition.Always)
            {
                return true;
            }

            // ExitMenuItemCondition.RequireShortcut
            return ModifierKeys.HasFlag(Keys.Control | Keys.Shift);
        }

        //Function that displays the taskbar on the specified screen
        public void ShowOnScreen(Screen screen)
        {
            StartPosition = FormStartPosition.Manual;
            Location = new Point(screen.WorkingArea.Left, screen.Bounds.Bottom - Config.Default.Renderer.TaskbarHeight);
            Size = new Size(screen.Bounds.Width, Config.Default.Renderer.TaskbarHeight);
            Show();
        }
    }
}