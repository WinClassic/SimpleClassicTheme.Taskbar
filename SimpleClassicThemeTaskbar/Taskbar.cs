using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//Really don't know why I chose to do this
using BOOL = System.Boolean;
using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    public partial class Taskbar : Form
    {
        //Constants that will later be options
        private const int tb_height = 28;

        //Constructor
        public Taskbar()
        {
            InitializeComponent();
            TopLevel = true;
            cppCode.InitCom();
        }

        //Make sure form doesnt show in alt tab
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

        //Function to determine which windows to add to the window list
        private static BOOL IsAltTabWindow(HWND hwnd)
        {
            //If window isn't visible it can't possibly be on the taskbar
            if (!IsWindowVisible(hwnd))
                return false;

            //TODO: Only do certain checks on certain operating systems to retain the best compatibility
            bool result = cppCode.WindowIsOnCurrentDesktop(hwnd);

            if (!result)
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
        private static readonly List<Window> windows = new List<Window>();
        private static bool EnumWind(IntPtr hWnd, int lParam)
        {
            //If you would show the window in alt+tab, show it on the custom taskbar
            if (IsAltTabWindow(hWnd)) 
                windows.Add(new Window(hWnd));
            return true;
        }

        //Initialize stuff
        private void Form1_Load(object sender, EventArgs e)
        {
            //TODO: Make sure the working area of the screen is the total height - height of taskbar
            //TODO: Add an option to registry tweak classic alt+tab

            //Get the monitor on which the current taskbar is located
            Screen taskbarScreen = Screen.FromHandle(FindWindowW("Shell_TrayWnd", ""));

            //Align the window on that monitor
            Width = taskbarScreen.Bounds.Width;
            Height = tb_height;
            int X = taskbarScreen.WorkingArea.Left;
            int Y = taskbarScreen.Bounds.Bottom - Height;
            Location = new Point(X, Y);
            line.Width = Width;

            //Hide taskbar
            Window w = new Window(FindWindowW("Shell_TrayWnd", ""));
            if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                ShowWindow(w.Handle, 0);
        }

        private void Taskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            cppCode.DeInitCom();

            //Make taskbar show up again
            ShowWindow(FindWindowW("Shell_TrayWnd", ""), 5);
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
                    MenuItem exit = new MenuItem { Text = "&Exit" };
                    exit.Click += delegate { Close(); };

                    //Add all menu items
                    d.MenuItems.Add(taskmanager);
                    d.MenuItems.Add(exit);
                }
                //Show the context menu
                d.Show(this, e.Location);
            }
        }

        //TODO: Make TaskbarProgams moveable
        private List<TaskbarProgram> icons = new List<TaskbarProgram>();

        public static bool CanInvoke = false;
        public static bool waitBeforeShow = false;
        public static HWND lastOpenWindow;
        public bool busy = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //TODO: Optimize update loop
            //TODO: Add quick-launch icons
            systemTray1.UpdateTime();
            verticalDivider3.Location = new Point(systemTray1.Location.X - 4, verticalDivider3.Location.Y);
            if (!busy)
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
                    if (wnd.ClassName != "Shell_TrayWnd")
                        waitBeforeShow = false;
                }
                else
                {
                    //Hide taskbar
                    Window w = new Window(FindWindowW("Shell_TrayWnd", ""));
                    if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                        ShowWindow(w.Handle, 0);
                    //Check if it's a fullscreen window. If so: hide
                    Screen scr = Screen.FromHandle(wnd.Handle);
                    int xy = cppCode.GetSize(wnd.Handle);
                    int width = xy >> 16, height = xy & 0x0000FFFF;
                    bool full = width >= scr.Bounds.Width && height >= scr.Bounds.Height;
                    //The window should only be visible if the active window is not fullscreen (with the exception of the desktop window)
                    Visible = !(full && wnd.ClassName != "Progman" && wnd.ClassName != "WorkerW");
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

                //Update systray
                systemTray1.UpdateButtons();

                //Make sure the ForegroundWindow variable is updated
                ForegroundWindow = GetForegroundWindow();

                //Calculate availabe space in taskbar and then divide that space over all programs
                int availableSpace = verticalDivider3.Location.X - panel1.Location.X - 4;
                if (newIcons.Count > 0 && availableSpace / newIcons.Count > Config.TaskbarProgramWidth)
                    availableSpace = newIcons.Count * Config.TaskbarProgramWidth;

                //Re-display all windows
                int x = panel1.Location.X;
                int iconWidth = newIcons.Count > 0 ? (int) Math.Floor((double)availableSpace / newIcons.Count) : 01;
                foreach (TaskbarProgram icon in newIcons)
                {
                    icon.Width = iconWidth;
                    icon.ActiveWindow = icon.WindowHandle == ForegroundWindow;
                    icon.Location = new Point(x, 0);
                    icon.Title = icon.Window.Title;
                    icon.Icon = GetAppIcon(icon.WindowHandle);
                    if (!Controls.Contains(icon))
                        Controls.Add(icon);
                    x += icon.Width + 1;
                }

                CanInvoke = true;

                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();
                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();
                Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

                CanInvoke = false;

                busy = false;
            }
        }
    }
}

