using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

//Really don't know why I chose to do this
using BOOL = System.Boolean;
using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    public partial class Taskbar : Form
    {
        public bool isPrimaryTaskbar = true;
        public bool NeverShow = false;

        bool temp = false;
        HWND wParam = new HWND(0x5354); //ST
        HWND lParam = new HWND(0x4F50); //OP
        protected override void WndProc(ref Message m)
        { 
            if (m.Msg == WM_EXITTASKBAR && !temp)
            {
                temp = true;
                if (m.Msg == WM_EXITTASKBAR && m.WParam == wParam && m.LParam == lParam)
                {
                    selfClose = true; Close(); Application.Exit();
                }
            }
            base.WndProc(ref m);
        }

        //Constructor
        public Taskbar(bool isPrimary)
        {
            isPrimaryTaskbar = isPrimary;
            
            //TEMP Set the display language
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en_US");
            //TEMP Load config
            Config.LoadFromRegistry();

            //Initialize thingies
            InitializeComponent();
            MouseMove += Taskbar_MouseMove;
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

        #region Win32

        public const int GCL_HICONSM = -34;
        public const int GCL_HICON = -14;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;

        public const int WM_GETICON = 0x7F;
        public const int WM_EXITTASKBAR = 0x0420;

        public delegate bool EnumWindowsCallback(IntPtr hWnd, int lParam);

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }


        [DllImport("User32.dll")]
        static extern uint GetWindowThreadProcessId(HWND hWnd, out uint lpdwProcessId);

        public Icon GetAppIcon(HWND hwnd)
        {
            //Try different ways of getting the icon
            IntPtr iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
            if (iconHandle == IntPtr.Zero)
                iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);
            if (iconHandle == IntPtr.Zero)
                return null;
            return Icon.FromHandle(iconHandle);
        }

        static HWND GetLastActivePopupOfWindow(HWND root)
        {
            HWND lastPopup = GetLastActivePopup(root);
            if (IsWindowVisible(lastPopup))
                return lastPopup;
            if (lastPopup == root)
                return HWND.Zero;
            return GetLastActivePopupOfWindow(lastPopup);
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out int pvAttribute, int cbAttribute);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HWND hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

        [DllImport("user32.dll")]
        static extern HWND GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern BOOL IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint gwFlags);

        [DllImport("user32.dll")]
        private static extern BOOL ShowWindow(HWND hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowW(string a, string b);

        #endregion

        private static Cpp.CLI.Interop cppCode = new Cpp.CLI.Interop();

        //Function that displays the taskbar on the specified screen
        public void ShowOnScreen(Screen screen)
        {
            StartPosition = FormStartPosition.Manual;
            Location = new Point(screen.WorkingArea.Left, screen.WorkingArea.Bottom);
            Size = new Size(screen.Bounds.Width, 28);
            Show();
        }

        //Function to determine which windows to add to the window list
        private BOOL IsAltTabWindow(HWND hwnd)
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
            HWND root = GetAncestor(hwnd, 3);

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

        //Window handle list
        private readonly List<Window> windows = new List<Window>();
        private bool LookingForTray = false;
        private bool EnumWind(IntPtr hWnd, int lParam)
        {
            if (LookingForTray)
            {
                //If looking for the taskbar, check if it is Shell_TrayWnd and if it is in the bound of the current desktop
                Window wi = new Window(hWnd);
                if (wi.ClassName == "Shell_TrayWnd" || wi.ClassName == "Shell_SecondaryTrayWnd")
                    if (Screen.FromHandle(hWnd).Bounds == Screen.FromControl(this).Bounds)
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
            if (isPrimaryTaskbar && Path.GetFileName(Assembly.GetExecutingAssembly().Location).ToLower() == "sct_taskbar.exe")
                File.WriteAllText("C:\\SCT\\Taskbar\\MainWindow.txt", Handle.ToString());

            //TODO: Add an option to registry tweak classic alt+tab
            line.Width = Width + 2;
            line.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            windows.Clear();
            quickLaunch1.Disabled = (!isPrimaryTaskbar) || (!Config.EnableQuickLaunch);
            quickLaunch1.UpdateIcons();
        }

        public bool selfClose = false;
        private void Taskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!selfClose)
            {
                PostMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYDOWN, (int)WIN32.VK_MENU, 0);
                PostMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYDOWN, (int)WIN32.VK_F4, 0);
                SendMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYUP, (int)WIN32.VK_F4, 0);
                SendMessage(FindWindowW("Shell_TrayWnd", ""), (int)WIN32.WM_KEYUP, (int)WIN32.VK_MENU, 0);
                e.Cancel = true;
            }
            else
            {
                //cppCode.DeInitCom();
                //Make taskbar show up again
                ShowWindow(FindWindowW("Shell_TrayWnd", ""), 5);
            }
        }

        private ContextMenu d;
        private void Taskbar_MouseClick(object sender, MouseEventArgs e)
        {
            //Open context menu
            if (e.Button == MouseButtons.Right)
            {
                //Check if the context menu is initialized
                if (d == null)
                {
                    //Create the context menu
                    d = new ContextMenu();

                    //Exit menu item
                    MenuItem taskmanager = new MenuItem { Text = "Open Task &Manager" };
                    taskmanager.Click += delegate { Process.Start("taskmgr"); };
                    MenuItem settings = new MenuItem { Text = "&Configure SCT Taskbar" };
                    settings.Click += delegate
                    {
                        Settings form = new Settings();
                        form.Show();
                    };
                    MenuItem exit = new MenuItem { Text = "&Exit" };
                    exit.Click += delegate { selfClose = true; Close(); Application.Exit(); };

                    //Add all menu items
                    d.MenuItems.Add(taskmanager);
                    d.MenuItems.Add(settings);
                    d.MenuItems.Add(exit);
                }
                //Show the context menu
                d.Show(this, e.Location);
            }
        }

        //TODO: Make TaskbarProgams moveable
        private List<TaskbarProgram> icons = new List<TaskbarProgram>();

        public TaskbarProgram heldDown;
        public bool held = false;

        public static bool CanInvoke = false;
        public static bool waitBeforeShow = false;
        public static HWND lastOpenWindow;
        public bool busy = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //TODO: Add quick-launch icons
            systemTray1.UpdateTime();
            verticalDivider3.Location = new Point(systemTray1.Location.X - 4, verticalDivider3.Location.Y);
            if (!busy && !held)
            {
                busy = true;
                //Get the foreground window (Used later)
                HWND ForegroundWindow = GetForegroundWindow();
                if (ForegroundWindow != Handle)
                {
                    lastOpenWindow = ForegroundWindow;
                }

                //Check if it was the start menu
                Window wnd = new Window(ForegroundWindow);
                startButton1.Pressed = wnd.ClassName == "OpenShell.CMenuContainer" ||
                                       wnd.ClassName == "Windows.UI.Core.CoreWindow";

                if (waitBeforeShow)
                {
                    if (wnd.ClassName != "Shell_TrayWnd" && wnd.ClassName != "Shell_SecondaryTrayWnd")
                    {
                        waitBeforeShow = false;
                    }
                }
                else
                {
                    //Hide taskbar
                    windows.Clear();
                    LookingForTray = true;
                    EnumWindowsCallback callback = EnumWind;
                    EnumWindows(callback, 0);
                    LookingForTray = false;

                    Screen scrr = Screen.FromControl(this);
                    Rectangle rct = scrr.Bounds;
                    cppCode.SetWorkingArea(rct.Left, rct.Right, rct.Top, rct.Bottom - 28);

                    if (NeverShow)
                    {
                        Screen screen = Screen.FromControl(this);
                        Rectangle rect = screen.Bounds;
                        cppCode.SetWorkingArea(rect.Left, rect.Right, rect.Top, rect.Bottom);
                    }

                    foreach (Window w in windows)
                        if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                            ShowWindow(w.Handle, 0);

                    //Check if it's a fullscreen window. If so: hide
                    Screen scr = Screen.FromHandle(wnd.Handle);
                    int xy = cppCode.GetSize(wnd.Handle);
                    int width = xy >> 16, height = xy & 0x0000FFFF;
                    bool full = width >= scr.Bounds.Width && height >= scr.Bounds.Height;
                    //The window should only be visible if the active window is not fullscreen (with the exception of the desktop window)
                    if (NeverShow)
                        Visible = false;
                    else
                        Visible = !(full && wnd.ClassName != "Progman" && wnd.ClassName != "WorkerW");
                }

                if (!Visible)
                {
                    busy = false;
                    return; 
                }

                //Re-check the window handle list
                windows.Clear();
                EnumWindowsCallback d = EnumWind;
                EnumWindows(d, 0);

                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

                //Check if any new window exists, if so: add it
                foreach (Window z in windows)
                {
                    //Get a handle
                    HWND hWnd = z.Handle;
                    //Check if it already exists
                    bool exists = false;
                    foreach (TaskbarProgram icon in icons)
                        if (icon.WindowHandle == hWnd)
                            exists = true;
                    if (!exists)
                    {
                        //Create the button
                        TaskbarProgram button = new TaskbarProgram();
                        button.Window = z;
                        button.WindowHandle = z.Handle;
                        button.Icon = GetAppIcon(z.Handle);
                        button.Title = z.Title;
                        button.MouseDown += delegate
                        {
                            heldDown = button;
                            held = true;
                        };
                        button.MouseUp += delegate
                        {
                            held = false;
                        };
                        //Add it to the list
                        icons.Add(button);
                    }
                }

                //The new list of icons
                List<TaskbarProgram> newIcons = new List<TaskbarProgram>();

                //Create a new list with only the windows that are still open
                foreach (TaskbarProgram icon in icons)
                {
                    bool contains = false;
                    foreach (Window z in windows)
                        if (z.Handle == icon.WindowHandle)
                        {
                            contains = true;
                            icon.Window = z;
                        }

                    if (contains)
                        newIcons.Add(icon);
                }

                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

                //Remove controls of the windows that were removed from the list
                foreach (Control dd in Controls)
                {
                    if (dd is TaskbarProgram)
                    {
                        TaskbarProgram icon = dd as TaskbarProgram;
                        if (!newIcons.Contains(icon))
                        {
                            icons.Remove(icon);
                            Controls.Remove(icon);
                            icon.Dispose();
                        }
                    }
                }

                //Update config related graphics
                if (Config.configChanged)
                {
                    systemTray1.firstTime = true;
                    systemTray1.ClearButtons();

                    Config.configChanged = false;
                }

                //Update systray
                if (isPrimaryTaskbar)
                    systemTray1.UpdateButtons();

                //Update quick-launch
                if (isPrimaryTaskbar)
                    quickLaunch1.UpdateIcons();

                //Make sure the ForegroundWindow variable is updated
                ForegroundWindow = GetForegroundWindow();

                int startX = quickLaunch1.Location.X + quickLaunch1.Width + 4;

                int programWidth = isPrimaryTaskbar ? Config.TaskbarProgramWidth + Config.SpaceBetweenTaskbarIcons : 24;
                //Calculate availabe space in taskbar and then divide that space over all programs
                int availableSpace = verticalDivider3.Location.X - startX - 6;
                availableSpace += Config.SpaceBetweenTaskbarIcons;
                if (newIcons.Count > 0 && availableSpace / newIcons.Count > programWidth)
                    availableSpace = newIcons.Count * programWidth;

                //Re-display all windows
                int x = startX;
                int iconWidth = newIcons.Count > 0 ? (int) Math.Floor((double)availableSpace / newIcons.Count) - Config.SpaceBetweenTaskbarIcons : 01;
                foreach (TaskbarProgram icon in newIcons)
                {
                    icon.Width = iconWidth;
                    icon.ActiveWindow = icon.WindowHandle == ForegroundWindow;
                    icon.Location = new Point(x, 0);
                    icon.Title = icon.Window.Title;
                    icon.Icon = GetAppIcon(icon.WindowHandle);
                    if (!Controls.Contains(icon))
                    {
                        Controls.Add(icon);
                        icon.Width = iconWidth;
                        verticalDivider3.BringToFront();
                        line.BringToFront();
                    }
                    x += icon.Width + Config.SpaceBetweenTaskbarIcons;
                }

                CanInvoke = true;

                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();
                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();
                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

                CanInvoke = false;

                busy = false;
            }
        }

        private void Taskbar_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}

