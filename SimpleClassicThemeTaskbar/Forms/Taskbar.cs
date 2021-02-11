using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class Taskbar : Form
    {
        //TODO: Clean this shitty mess like wth
        private ContextMenuStrip d;

        //Window handle list
        private readonly List<Window> windows = new List<Window>();
        private bool LookingForTray = false;
        public bool selfClose = false;

        private List<BaseTaskbarProgram> icons = new List<BaseTaskbarProgram>();
        //private Thread BackgroundThread;
        private IntPtr CrossThreadHandle;

        public BaseTaskbarProgram heldDownButton;
        public int heldDownOriginalX = 0;
        public int mouseOriginalX = 0;

        public bool CanInvoke = false;
        public static bool waitBeforeShow = false;
        public static IntPtr lastOpenWindow;

        List<string> BlacklistedClassNames = new List<string>();
        List<string> BlacklistedWindowNames = new List<string>();
        List<string> BlacklistedProcessNames = new List<string>();

        public bool busy = false;
        public bool Primary = true;
        public bool NeverShow = false;

        protected override void WndProc(ref Message m)
        { 
            if (m.Msg == WM_ENDSESSION)
            {
                selfClose = true; 
                Config.SaveToRegistry();
                Environment.Exit(0);
            }
            if (m.Msg == WM_QUERYENDSESSION)
            {
                m.Result = new IntPtr(1);
            }
            if (m.Msg == WM_SCT)
            {
                switch (m.WParam.ToInt32())
                {
                    case SCTWP_EXIT:
                        if (ApplicationEntryPoint.SCTCompatMode || m.LParam.ToInt32() == SCTLP_FORCE)
                        {
                            selfClose = true;
                            Close();
                            Application.Exit();
                        }
                        break;
                    case SCTWP_ISMANAGED:
                        m.Result = new IntPtr(1);
                        return;
                    case SCTWP_ISSCT:
                        m.Result = new IntPtr(ApplicationEntryPoint.SCTCompatMode ? 1 : 0);
                        return;
                }
            }
            if (m.Msg == 0x15/*WM_SYSCOLORCHANGE*/)
            {
                //SuspendLayout();
                //Font = SystemFonts.MessageBoxFont;
                //ResumeLayout();
            }
            if (m.Msg == 0x1a/*WM_SETTINGCHANGE*/)
			{
                //Rectangle screen = Screen.FromControl(this).WorkingArea;
                //Console.WriteLine($"{screen.X},{screen.Y} {screen.Width}x{screen.Height}");
			}
            base.WndProc(ref m);
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
            MouseMove += Taskbar_MouseMove;
            HandleCreated += delegate { CrossThreadHandle = Handle; };
            TopLevel = true;
            cppCode.InitCom();

            //Make sure programs are aware of the new work area
            if (isPrimary)
            {
                windows.Clear();
                EnumWindowsCallback callback = EnumWind;
                EnumWindows(callback, 0);
                foreach (Window w in windows)
                {
                    //WM_WININICHANGE - SPI_SETWORKAREA
                    PostMessage(w.Handle, 0x001A, 0x002F, 0);
                }
            }
        }

        //Make sure form doesnt show in alt tab and that it shows up on all virtual desktops
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

        private static CodeBridge cppCode = new CodeBridge();

        //Function that displays the taskbar on the specified screen
        public void ShowOnScreen(Screen screen)
        {
            StartPosition = FormStartPosition.Manual;
            Location = new Point(screen.WorkingArea.Left, screen.Bounds.Bottom - 28);
            Size = new Size(screen.Bounds.Width, 28);
			Show();
        }

        //Function to determine which windows to add to the window list
        private bool IsAltTabWindow(IntPtr hwnd)
        {
            //If window isn't visible it can't possibly be on the taskbar
            if (!IsWindowVisible(hwnd))
                return false;

            //Check if the OS is Windows 10
            if (Environment.OSVersion.Version.Major == 10)
                //Check if the window is on the current Desktop
                if (!cppCode.WindowIsOnCurrentDesktop(hwnd))
                    return false;

            //Get the root owner of the window
            IntPtr root = GetAncestor(hwnd, 3);

            //If the last active popup of the root owner is NOT this window: don't show it
            //This method is described by Raymond Chen in this blogpost:
            //https://devblogs.microsoft.com/oldnewthing/20071008-00/?p=24863
            if (GetLastActivePopupOfWindow(root) != hwnd)
                return false;

            //Create a Window object
            Window wi = new Window(hwnd);

            //If it's a tool window: don't show it
            if ((wi.WindowInfo.dwExStyle & 0x00000080L) > 0)
                return false;

            //If it's any of these odd cases: don't show it
            if (wi.ClassName == "Shell_TrayWnd" ||                          //Windows taskbar
                wi.ClassName == "WorkerW" ||                                //Random Windows thing
                wi.ClassName == "Progman" ||                                //The program manager
                wi.ClassName == "ThumbnailDeviceHelperWnd" ||               //UWP
                wi.ClassName == "Windows.UI.Core.CoreWindow" ||             //Empty UWP apps
                wi.ClassName == "DV2ControlHost" ||                         //Windows startmenu, if open
                (wi.ClassName == "Button" && wi.Title == "Start") ||        //Windows startmenu-button.
                wi.ClassName == "MsgrIMEWindowClass" ||                     //Live messenger's notifybox i think
                wi.ClassName == "SysShadow" ||                              //Live messenger's shadow-hack
                wi.ClassName.StartsWith("WMP9MediaBarFlyout") ||            //WMP's "now playing" taskbar-toolbar
                wi.Title.Length == 0)                                       //Window without a name
                return false;

            //UWP app
            if (wi.ClassName == "ApplicationFrameWindow")
            {
                //Do an API call to see if app isn't cloaked
                int d;
                DwmGetWindowAttribute(wi.Handle, DWMWINDOWATTRIBUTE.Cloaked, out d, Marshal.SizeOf(0));

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
                //If looking for the taskbar, check if it is Shell_TrayWnd and if it is in the bound of the current desktop
                Window wi = new Window(hWnd);
                if (wi.ClassName == "Shell_TrayWnd" || wi.ClassName == "Shell_SecondaryTrayWnd")
                    if (Screen.FromHandle(hWnd).Bounds == Screen.FromHandle(CrossThreadHandle).Bounds)
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

        //Initialize stuff
        private void Form1_Load(object sender, EventArgs e)
        {
            //if (Primary && Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToLower() == "sct_taskbar.exe")
            //    File.WriteAllText("C:\\SCT\\Taskbar\\MainWindow.txt", Handle.ToString());

            //TODO: Add an option to registry tweak classic alt+tab
            line.Width = Width + 2;
            line.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            windows.Clear();
            quickLaunch1.Disabled = (!Primary) || (!Config.EnableQuickLaunch);
            quickLaunch1.UpdateIcons();

            //BackgroundThread = new Thread(BackgroundThreadFunction);
            //BackgroundThread.Start();
            timerUpdateInformation.Start();
            timerUpdateUI.Start();
        }

        
        private void Taskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!selfClose)
            {
                //If we don't close ourselves, show the shutdown dialog
                PostMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYDOWN, (int)WIN32.VK_MENU, 0);
                PostMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYDOWN, (int)WIN32.VK_F4, 0);
                SendMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYUP, (int)WIN32.VK_F4, 0);
                SendMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYUP, (int)WIN32.VK_MENU, 0);
                e.Cancel = true;
            }
            else
            {
                //Kill background thread and show taskbar
                ShowWindow(FindWindowW("Shell_TrayWnd", ""), 5);
            }
        }

        private void Taskbar_MouseClick(object sender, MouseEventArgs e)
        {
            //Open context menu
            if (e.Button == MouseButtons.Right)
            {
                //Check if the context menu is initialized
                if (d == null)
                {
                    //Create the context menu
                    d = new ContextMenuStrip();

                    //Exit menu item
                    ToolStripMenuItem toolbars = new ToolStripMenuItem("(NIY) &Toolbars");
                    ToolStripSeparator seperator1 = new ToolStripSeparator();
                    ToolStripMenuItem cascadeWindows = new ToolStripMenuItem("Casca&de windows");
                    ToolStripMenuItem showWindowsStacked = new ToolStripMenuItem("Show windows stack&ed");
                    ToolStripMenuItem showWindowsSideBySide = new ToolStripMenuItem("Show windows s&ide by side");
                    ToolStripMenuItem showDesktop = new ToolStripMenuItem("&Show the desktop");
                    ToolStripSeparator seperator2 = new ToolStripSeparator();
                    ToolStripMenuItem taskManager = new ToolStripMenuItem("Tas&k Manager");
                    ToolStripSeparator seperator3 = new ToolStripSeparator();
                    ToolStripMenuItem lockTaskbar = new ToolStripMenuItem("(NIY) &Lock the taskbar");
                    ToolStripMenuItem settings = new ToolStripMenuItem("Configu&re SCT Taskbar");
                    ToolStripMenuItem exit = new ToolStripMenuItem("&Exit SCT Taskbar");

                    cascadeWindows.Click += delegate { CascadeWindows(IntPtr.Zero, MDITILE_ZORDER, IntPtr.Zero, 0, IntPtr.Zero); };
                    showWindowsStacked.Click += delegate { TileWindows(IntPtr.Zero, MDITILE_HORIZONTAL, IntPtr.Zero, 0, IntPtr.Zero); };
                    showWindowsSideBySide.Click += delegate { TileWindows(IntPtr.Zero, MDITILE_VERTICAL, IntPtr.Zero, 0, IntPtr.Zero); };
                    taskManager.Click += delegate { Process.Start("taskmgr"); };
                    settings.Click += delegate { new Settings().Show(); };
                    showDesktop.Click += delegate { Keyboard.KeyDown(Keys.LWin); Keyboard.KeyDown(Keys.D); Keyboard.KeyUp(Keys.D); Keyboard.KeyUp(Keys.LWin); };
                    exit.Click += delegate { selfClose = true; Close(); Application.Exit(); };

                    //Add all menu items
                    d.Items.Add(toolbars);
                    d.Items.Add(seperator1);
                    d.Items.Add(cascadeWindows);
                    d.Items.Add(showWindowsStacked);
                    d.Items.Add(showWindowsSideBySide);
                    d.Items.Add(showDesktop);
                    d.Items.Add(seperator2);
                    d.Items.Add(taskManager);
                    d.Items.Add(seperator3);
                    d.Items.Add(lockTaskbar);
                    d.Items.Add(settings);
                    d.Items.Add(exit);

                    d.RenderMode = ToolStripRenderMode.Professional;
                }
                ////Show the context menu
                //d.Show(this, e.Location);
                SystemContextMenu menu = SystemContextMenu.FromContextMenuStrip(d);
                Point location = PointToScreen(e.Location);
                menu.Show(Handle, location.X, location.Y);
            }
            else if (e.Button == MouseButtons.Left && e.Location.X == Width - 1)
            {
                Keyboard.KeyDown(Keys.LWin); Keyboard.KeyDown(Keys.D); Keyboard.KeyUp(Keys.D); Keyboard.KeyUp(Keys.LWin);
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

        private void Taskbar_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void StartButtonPanel_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            startButton1.OnMouseClick(startButton1, e);
        }

        private void StartButtonPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            startButton1.OnMouseDown(startButton1, e);
        }

        private void StartButtonPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            startButton1.OnMouseUp(startButton1, e);
        }

		private void timerUpdateUI_Tick(object sender, EventArgs e)
		{
            //Get the forground window to check some stuff
            IntPtr ForegroundWindow = GetForegroundWindow();
            Window wnd = new Window(ForegroundWindow);

            //Check if the foreground window was the start menu
            startButton1.UpdateState(wnd);

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

            //See if we're moving, if so calculate new position, if we finished calculate new position
            if (heldDownButton != null)
            {
                if (Math.Abs(mouseOriginalX - Cursor.Position.X) > 5)
                    heldDownButton.IsMoving = true;
                if ((MouseButtons & MouseButtons.Left) != 0)
                {
                    Point p = new Point(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownButton.Location.Y);
                    heldDownButton.Location = new Point(Math.Max(startX, Math.Min(p.X, maxX)), p.Y);
                    int newIndex = (startX - heldDownButton.Location.X - ((iconWidth + Config.SpaceBetweenTaskbarIcons) / 2)) / (iconWidth + Config.SpaceBetweenTaskbarIcons) * -1;
                    if (newIndex < 0) newIndex = 0;
                    icons.Remove(heldDownButton);
                    icons.Insert(Math.Min(icons.Count, newIndex), heldDownButton);
                    icons.Remove(heldDownButton);
                    icons.Insert(Math.Min(icons.Count, newIndex), heldDownButton);
                }
                else
                {
                    Point p = new Point(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownButton.Location.Y);
                    heldDownButton.Location = new Point(Math.Max(startX, Math.Min(p.X, maxX)), p.Y);
                    int newIndex = (startX - heldDownButton.Location.X - ((iconWidth + Config.SpaceBetweenTaskbarIcons) / 2)) / (iconWidth + Config.SpaceBetweenTaskbarIcons) * -1;
                    if (newIndex < 0) newIndex = 0;
                    icons.Remove(heldDownButton);
                    icons.Insert(Math.Min(icons.Count, newIndex), heldDownButton);
                    icons.Remove(heldDownButton);
                    icons.Insert(Math.Min(icons.Count, newIndex), heldDownButton);
                    heldDownButton = null;
                }
            }

            //Re-display all windows (except heldDownButton)
            foreach (BaseTaskbarProgram icon in icons)
            {
                icon.Width = iconWidth;
                if (icon == heldDownButton)
                {
                    x += icon.Width + Config.SpaceBetweenTaskbarIcons;
                    continue;
                }
                icon.IsActiveWindow(ForegroundWindow);
                icon.Location = new Point(x, 0);
                icon.Title = icon.Window.Title;
                icon.Icon = GetAppIcon(icon.Window);
                if (!Controls.Contains(icon))
                {
                    Controls.Add(icon);
                    icon.Width = iconWidth;
                    verticalDivider3.BringToFront();
                    line.BringToFront();
                }
                x += icon.Width + Config.SpaceBetweenTaskbarIcons;
                icon.Visible = true;

                //Application.DoEvents();
            }
            if (heldDownButton != null)
            {
                heldDownButton.BringToFront();
                verticalDivider3.BringToFront();
                line.BringToFront();
            }
        }

		private void timerUpdateInformation_Tick(object sender, EventArgs e)
		{
            //Get the forground window to check some stuff
            IntPtr ForegroundWindow = GetForegroundWindow();
            Window wnd = new Window(ForegroundWindow);

            //Hide explorer's taskbar
            waitBeforeShow = false;
            windows.Clear();
            LookingForTray = true;
            EnumWindowsCallback callback = EnumWind;
            EnumWindows(callback, 0);
            LookingForTray = false;

            foreach (Window w in windows)
                if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                    ShowWindow(w.Handle, 0);

            //The window should only be visible if the active window is not fullscreen (with the exception of the desktop window)
            Screen scr = Screen.FromHandle(wnd.Handle);
            int xy = cppCode.GetSize(wnd.Handle);
            int width = xy >> 16, height = xy & 0x0000FFFF;
            bool full = width >= scr.Bounds.Width && height >= scr.Bounds.Height;
            if (NeverShow)
                Visible = false;
            else
                Visible = !(full && wnd.ClassName != "Progman" && wnd.ClassName != "WorkerW");

            //If we're not visible, why do anything?
            if (!Visible)
                return;

            //Obtain task list
            windows.Clear();
            EnumWindowsCallback d = EnumWind;
            EnumWindows(d, 0);

            //Resize work area
            Screen screen = Screen.FromHandle(CrossThreadHandle);
            Rectangle rct = screen.Bounds;
            rct.Height -= Height;
            Point desiredLocation = new Point(rct.Left, rct.Bottom);
            if (screen.WorkingArea.ToString() != rct.ToString())
                cppCode.SetWorkingArea(rct.Left, rct.Right, rct.Top, rct.Bottom, Environment.OSVersion.Version.Major < 10, windows.Select(a => a.Handle).ToArray());
            if (Location.ToString() != desiredLocation.ToString())
                Location = desiredLocation;

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
                    BaseTaskbarProgram button = new SingleTaskbarProgram();
                    button.Window = z;
                    button.Title = z.Title;
                    button.MouseDown += Taskbar_IconDown;

                    uint pid;
                    GetWindowThreadProcessId(button.Window.Handle, out pid);
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
            List<BaseTaskbarProgram> newIcons = new List<BaseTaskbarProgram>();

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
            List<BaseTaskbarProgram> programs = new List<BaseTaskbarProgram>();
            if (Config.ProgramGroupCheck == ProgramGroupCheck.None)
                goto addAllWindows;

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
                            BaseTaskbarProgram button = new SingleTaskbarProgram();
                            button.Window = group.ProgramWindows[0].Window;
                            button.MouseDown += Taskbar_IconDown;

                            uint pid;
                            GetWindowThreadProcessId(button.Window.Handle, out pid);
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
                                GroupedTaskbarProgram group = new GroupedTaskbarProgram();
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
            goto displayWindows;

            addAllWindows:
            foreach (BaseTaskbarProgram taskbarProgram in newIcons)
                programs.Add(taskbarProgram);

            //Re-display all windows (except heldDownButton)
            displayWindows:
            icons = programs;

            //Update systray
            if (Primary)
                systemTray1.UpdateIcons();

            //Update quick-launch
            if (Primary)
                quickLaunch1.UpdateIcons();

            //Update clock
            systemTray1.UpdateTime();

            //Update UI
            timerUpdateUI_Tick(null, null);
        }
	}
}

