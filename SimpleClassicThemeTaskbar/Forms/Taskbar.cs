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
        private readonly Stopwatch sw = new();
        private readonly List<(string, TimeSpan)> times = new();

        //Window handle list
        private readonly List<Window> windows = new();

        //private Thread BackgroundThread;
        private IntPtr CrossThreadHandle;

        //TODO: Clean this shitty mess like wth
        private ContextMenuStrip d;
        private ToolStripMenuItem exitContextMenuItem;

        private bool dummy;
        private List<BaseTaskbarProgram> icons = new();
        private bool LookingForTray = false;
        private Range taskArea;
        private int taskIconWidth;
        
        private TimingDebugger timingDebugger = new();
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
            foreach (string filter in Config.TaskbarProgramFilter.Split('*'))
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
            Config.LoadFromRegistry();

            //Initialize thingies
            InitializeComponent();
            HandleCreated += delegate { CrossThreadHandle = Handle; };
            TopLevel = true;

            //Fix height according to renderers preferences
            Height = Config.Renderer.TaskbarHeight;
            startButton1.Height = Height;
            systemTray1.Height = Height;
            quickLaunch1.Height = Height;

            //Make sure programs are aware of the new work area
            if (isPrimary)
            {
                windows.Clear();
                _ = User32.EnumWindows(EnumWind, 0);
                foreach (Window w in windows)
                {
                    //WM_WININICHANGE - SPI_SETWORKAREA
                    _ = User32.PostMessage(w.Handle, 0x001A, 0x002F, 0);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e) => Config.Renderer.DrawTaskBar(this, e.Graphics);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
			{
                case User32.WM_ENDSESSION:
                    ApplicationEntryPoint.ExitSCTT();
                    break;
                case User32.WM_QUERYENDSESSION:
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
			}
            if (m.Msg == WM_SHELLHOOKMESSAGE)
			{
                HookProcedure((User32.ShellEvents)m.WParam, m.LParam, IntPtr.Zero);
			}
            base.WndProc(ref m);
        }

        //Initialize stuff
        private void Form1_Load(object sender, EventArgs e)
        {
            //TODO: Add an option to registry tweak classic alt+tab
            windows.Clear();
            quickLaunch1.Disabled = (!Primary) || (!Config.EnableQuickLaunch);
            quickLaunch1.UpdateIcons();

            // Create shell hook
            if (Config.EnablePassiveTaskbar)
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
                _ = User32.PostMessage(User32.FindWindowW("Shell_TrayWnd", ""), (int)User32.WM_KEYDOWN, (int)User32.VK_MENU, 0);
                _ = User32.PostMessage(User32.FindWindowW("Shell_TrayWnd", ""), (int)User32.WM_KEYDOWN, (int)User32.VK_F4, 0);
                _ = User32.SendMessage(User32.FindWindowW("Shell_TrayWnd", ""), (int)User32.WM_KEYUP, (int)User32.VK_F4, 0);
                _ = User32.SendMessage(User32.FindWindowW("Shell_TrayWnd", ""), (int)User32.WM_KEYUP, (int)User32.VK_MENU, 0);
                e.Cancel = true;
            }
            else
            {
                //Show taskbar
                _ = User32.ShowWindow(User32.FindWindowW("Shell_TrayWnd", ""), 5);
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
                int newIndex = (taskArea.Start.Value - heldDownButton.Location.X - ((taskIconWidth + Config.SpaceBetweenTaskbarIcons) / 2)) / (taskIconWidth + Config.SpaceBetweenTaskbarIcons) * -1;
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
                        x += icon.Width + Config.SpaceBetweenTaskbarIcons;
                        continue;
                    }
                    icon.Location = new Point(x, 0);
                    x += icon.Width + Config.SpaceBetweenTaskbarIcons;
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
                if (d == null)
                {
                    d = ConstructTaskbarContextMenu();
                }
                ////Show the context menu
                //d.Show(this, e.Location);
                exitContextMenuItem.Available = ShouldShowExit();
                SystemContextMenu menu = SystemContextMenu.FromToolStripItems(d.Items);
                Point location = PointToScreen(e.Location);
                menu.Show(Handle, location.X, location.Y);
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
                times.Clear();
                watchUI = !watchUI;
            }));

            debuggingItem.DropDownItems.Add(new ToolStripMenuItem("Watch Logic", null, (_, __) =>
            {
                times.Clear();
                watchLogic = !watchLogic;
            }));

            debuggingItem.DropDownItems.Add(new ToolStripMenuItem("Watch Tray", null, (_, __) =>
            {
                times.Clear();
                systemTray1.times.Clear();
                systemTray1.watchTray = !systemTray1.watchTray;
            }));

            return debuggingItem;
        }

        private IntPtr HookProcedure(User32.ShellEvents nCode, IntPtr wParam, IntPtr lParam)
        {
            Logger.Log(LoggerVerbosity.Verbose, "Taskbar/HookProcedure", $"Call parameters: {nCode}, {wParam:X8}, {lParam:X8}");

            if (nCode < 0)
                return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

            switch (nCode)
            {
                // Create a button for the new window
                case User32.ShellEvents.HSHELL_WINDOWCREATED:
                    HandleWindowCreated(lParam);
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

            return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private void HandleWindowDestroyed(IntPtr wParam)
        {
            List<BaseTaskbarProgram> referenceList = new List<BaseTaskbarProgram>(icons);
            foreach (BaseTaskbarProgram taskbarProgram in referenceList)
            {
                if (taskbarProgram is SingleTaskbarProgram singleProgram)
                {
                    if (singleProgram.Window.Handle == wParam)
                    {
                        Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Deleting window {singleProgram.Title}.");
                        Controls.Remove(singleProgram);
                        icons.Remove(singleProgram);
                    }
                }
                else if (taskbarProgram is GroupedTaskbarProgram groupedProgram)
                {
                    if (!groupedProgram.RemoveWindow(wParam))
                    {
                        if (groupedProgram.ProgramWindows.Count == 1)
                        {
                            BaseTaskbarProgram seperatedProgram = new SingleTaskbarProgram
                            {
                                Window = groupedProgram.ProgramWindows[0].Window
                            };
                            seperatedProgram.MouseDown += Taskbar_IconDown;
                            seperatedProgram.MouseMove += Taskbar_IconMove;
                            seperatedProgram.MouseUp += Taskbar_IconUp;
                            seperatedProgram.Height = Height;

                            User32.GetWindowThreadProcessId(seperatedProgram.Window.Handle, out uint processId);
                            Process process = Process.GetProcessById((int)processId);
                            seperatedProgram.Process = process;

                            icons.Insert(icons.IndexOf(groupedProgram), seperatedProgram);
                        }
                        icons.Remove(groupedProgram);
                        Controls.Remove(groupedProgram);
                        groupedProgram.Dispose();
                    }
                }
            }

            UpdateUI();
            UpdateUI();
        }

        private void HandleWindowActivated(IntPtr wParam)
        {
            foreach (BaseTaskbarProgram taskbarProgram in icons)
                taskbarProgram.ActiveWindow = taskbarProgram.Window.Handle == wParam;
        }

        private void HandleWindowCreated(IntPtr wParam)
        {

            Window window = new(wParam);
            SingleTaskbarProgram button = new()
            {
                Window = window
            };
            button.MouseDown += Taskbar_IconDown;
            button.MouseMove += Taskbar_IconMove;
            button.MouseUp += Taskbar_IconUp;
            button.Height = Height;

            User32.GetWindowThreadProcessId(button.Window.Handle, out uint pid);
            Process p = Process.GetProcessById((int)pid);
            button.Process = p;

            //Check if not blacklisted
            if (BlacklistedClassNames.Contains(window.ClassName) ||
                BlacklistedWindowNames.Contains(window.Title) ||
                BlacklistedProcessNames.Contains(p.ProcessName) ||
                (Environment.OSVersion.Version.Major == 10 && !UnmanagedCodeMigration.IsWindowOnCurrentVirtualDesktop(wParam)))
                button.Dispose();
            else
            {
                icons.Add(button);
                Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Adding window {button.Title}.");
            }

            UpdateUI();
            UpdateUI();
        }

        internal void EnumerateWindows()
		{
            // Check if the foreground window was the start menu
            IntPtr ForegroundWindow = User32.GetForegroundWindow();
            startButton1.UpdateState(new Window(ForegroundWindow));

            //Put left side controls in the correct place
            quickLaunch1.Location = new Point(startButton1.Location.X + startButton1.Width + 2, 1);

            if (!Dummy)
			{
                // Hide explorer's taskbar(s)
                waitBeforeShow = false;
                windows.Clear();
                LookingForTray = true;
                User32.EnumWindows(EnumWind, 0);
                LookingForTray = false;

                foreach (Window w in windows)
                    if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                        User32.ShowWindow(w.Handle, 0);
            }

            // Obtain task list
            windows.Clear();
            User32.EnumWindows(EnumWind, 0);

            if (!Dummy)
            {
                // Resize work area
                ApplyWorkArea();
            }

            // Clear icon list as this is a full enumeration
            icons.Clear();

            //Check if any new window exists, if so: add it
            foreach (Window z in windows)
            {
                //Create the button
                BaseTaskbarProgram button = new SingleTaskbarProgram
                {
                    Window = z
                };
                button.MouseDown += Taskbar_IconDown;
                button.MouseMove += Taskbar_IconMove;
                button.MouseUp += Taskbar_IconUp;
                button.Height = Height;

                User32.GetWindowThreadProcessId(button.Window.Handle, out uint pid);
                Process p = Process.GetProcessById((int)pid);
                button.Process = p;

                //Check if not blacklisted
                if (BlacklistedClassNames.Contains(z.ClassName) || BlacklistedWindowNames.Contains(z.Title) || BlacklistedProcessNames.Contains(p.ProcessName))
                    button.Dispose();
                else
                    icons.Add(button);

                Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Adding window {button.Title}.");
            }

            //The new list of icons
            List<BaseTaskbarProgram> newIcons = new(icons);

            // Remove all currently displayed controls
            Control[] controls = new Control[Controls.Count];
            Controls.CopyTo(controls, 0);
            foreach (Control control in controls)
            {
                if (control is BaseTaskbarProgram taskbarProgram)
                {
                    Logger.Log(LoggerVerbosity.Verbose, "Taskbar/EnumerateWindows", $"Deleting window {taskbarProgram.Title}.");
                    icons.Remove(taskbarProgram);
                    Controls.Remove(taskbarProgram);
                    taskbarProgram.Dispose();
                }
            }

            //Create new list for finalized values
            List<BaseTaskbarProgram> programs = new();
            if (Config.ProgramGroupCheck != ProgramGroupCheck.None)
            {

                if (!watchLogic)
                    timingDebugger.FinishRegion("Create controls for all tasks");

                //Check for grouping and finalize position values
                foreach (BaseTaskbarProgram taskbarProgram in newIcons)
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

                            User32.GetWindowThreadProcessId(button.Window.Handle, out uint pid);
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
                            if (programBase is SingleTaskbarProgram program && IsGroupConditionMet(group, program))
                            {
                                programs.Remove(program);
                                group.ProgramWindows.Add(program);
                            }
                        }
                        programs.Add(taskbarProgram);
                    }
                    else if (taskbarProgram is SingleTaskbarProgram icon)
                    {
                        BaseTaskbarProgram sameThing = programs.FirstOrDefault((p) => IsGroupConditionMet(p, icon));

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
                            newGroup.MouseDown += Taskbar_IconDown;
                            newGroup.ProgramWindows.Add(sameThing as SingleTaskbarProgram);
                            newGroup.ProgramWindows.Add(icon);
                            icon.IsMoving = false;
                            programs.Add(newGroup);
                        }
                    }
                }

                icons = programs;
            }
            else
            {
                icons = newIcons;
            }

            //Update systray
            if (Primary)
                systemTray1.UpdateIcons();

            //Update clock
            systemTray1.UpdateTime();

            //Update quick-launch
            if (Primary)
                quickLaunch1.UpdateIcons();

            //Put divider in correct place
            verticalDivider3.Location = new Point(systemTray1.Location.X - 9, verticalDivider3.Location.Y);
            UpdateUI();
            UpdateUI();
        }

        private static bool IsGroupConditionMet(BaseTaskbarProgram a, BaseTaskbarProgram b)
        {
            try
            {
                return Config.ProgramGroupCheck switch
                {
                    ProgramGroupCheck.Process => a.Process.Id == b.Process.Id,
                    ProgramGroupCheck.FileNameAndPath => a.Process.MainModule.FileName == b.Process.MainModule.FileName,
                    ProgramGroupCheck.ModuleName => a.Process.MainModule.ModuleName == b.Process.MainModule.ModuleName,
                    _ => false,
                };
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
            {
                // If we got an access denied exception we catch it and return false.
                // Otherwise we re-throw the exception
                Logger.Log(LoggerVerbosity.Basic, "Taskbar/Groups", $"Failed to compare taskbar programs: {ex}");
                return false;
            }
        }

        private void UpdateUI()
        {
            // Get the foreground window to set the ActiveWindow property on the taskbar items
            IntPtr ForegroundWindow = User32.GetForegroundWindow();

            // Calculate availabe space in taskbar and then divide that space over all programs
            int startX = quickLaunch1.Location.X + quickLaunch1.Width + 4;
            int programWidth = Primary ? Config.TaskbarProgramWidth + Config.SpaceBetweenTaskbarIcons : 24;

            int availableSpace = verticalDivider3.Location.X - startX - 6;
            availableSpace += Config.SpaceBetweenTaskbarIcons;
            if (icons.Count > 0 && availableSpace / icons.Count > programWidth)
                availableSpace = icons.Count * programWidth;
            int x = startX;
            int iconWidth = icons.Count > 0 ? (int)Math.Floor((double)availableSpace / icons.Count) - Config.SpaceBetweenTaskbarIcons : 01;
            int maxX = verticalDivider3.Location.X - iconWidth;

            // Re-display all windows (except heldDownButton)
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

            Invalidate();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
		{
            // Get the foreground window to set the ActiveWindow property on the taskbar items
            IntPtr ForegroundWindow = User32.GetForegroundWindow();
            foreach (BaseTaskbarProgram taskbarProgram in icons)
                taskbarProgram.ActiveWindow = taskbarProgram.Window.Handle == ForegroundWindow;

            // Check if the foreground window was the start menu
            startButton1.UpdateState(new Window(ForegroundWindow));

            //Put left side controls in the correct place
            quickLaunch1.Location = new Point(startButton1.Location.X + startButton1.Width + 2, 1);
        }

        private void ApplyWorkArea()
        {
            Screen screen = Screen.FromHandle(CrossThreadHandle);
            Rectangle rct = screen.Bounds;
            rct.Height -= Height;
            Point desiredLocation = new(rct.Left, rct.Bottom);

            if (screen.WorkingArea.ToString() != rct.ToString())
            {
                var windowHandles = windows.Select(a => a.Handle).ToArray();
                UnmanagedCodeMigration.SetWorkingArea(new RECT(rct), Environment.OSVersion.Version.Major < 10, windowHandles);
            }

            if (Location.ToString() != desiredLocation.ToString())
                Location = desiredLocation;

            if (!watchLogic)
                timingDebugger.FinishRegion("Resize work area");
        }

        private ContextMenuStrip ConstructTaskbarContextMenu()
        {
            ContextMenuStrip c = new();

            ToolStripMenuItem cascadeWindows;
            ToolStripMenuItem showWindowsStacked;
            ToolStripMenuItem showWindowsSideBySide;
            ToolStripMenuItem taskManager;
            ToolStripMenuItem settings;
            ToolStripMenuItem showDesktop;

            var items = new ToolStripItem[]
            {
                new ToolStripMenuItem("&Toolbars") { Enabled = false },
                new ToolStripSeparator(),
                cascadeWindows = new("Ca&scade Windows"),
                showWindowsStacked = new("Tile Windows &Horizontally"),
                showWindowsSideBySide = new("Tile Windows V&ertically"),
                showDesktop = new("&Show the Desktop", null, (_, __) => Keyboard.KeyPress(Keys.LWin, Keys.D)),
                new ToolStripSeparator(),
                taskManager = new("Tas&k Manager", null, (_, __) => Process.Start("taskmgr")),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Lock the Taskbar") { Enabled = false },
                settings = new("P&roperties"),
                exitContextMenuItem = new("&Exit SCT Taskbar", null, (_, __) => ApplicationEntryPoint.ExitSCTT())
            };

            cascadeWindows.Click += delegate { _ = User32.CascadeWindows(IntPtr.Zero, User32.MDITILE_ZORDER, IntPtr.Zero, 0, IntPtr.Zero); };
            showWindowsStacked.Click += delegate { _ = User32.TileWindows(IntPtr.Zero, User32.MDITILE_HORIZONTAL, IntPtr.Zero, 0, IntPtr.Zero); };
            showWindowsSideBySide.Click += delegate { _ = User32.TileWindows(IntPtr.Zero, User32.MDITILE_VERTICAL, IntPtr.Zero, 0, IntPtr.Zero); };
            settings.Click += delegate { new Settings().Show(); };

            if (Config.EnableDebugging)
            {
                c.Items.Add(ConstructDebuggingMenu());
            }

            c.Items.AddRange(items);

            return c;
        }

        private bool ShouldShowExit()
        {
            if (Config.ExitMenuItemCondition == ExitMenuItemCondition.Always)
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
            Location = new Point(screen.WorkingArea.Left, screen.Bounds.Bottom - Config.Renderer.TaskbarHeight);
            Size = new Size(screen.Bounds.Width, Config.Renderer.TaskbarHeight);
            Show();
        }
    }
}