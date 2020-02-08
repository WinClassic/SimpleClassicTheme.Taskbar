using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

//Really don't know why I chose to do this
using BOOL = System.Boolean;
using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    

    public partial class Taskbar : Form
    {
        //Constructor
        public Taskbar()
        {
            InitializeComponent();
            TopLevel = true;
        }

        //Constants that will later be options
        private const int tb_height = 30;

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

        public Icon GetAppIcon(IntPtr hwnd)
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

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

        [DllImport("user32.dll")]
        static extern HWND GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern BOOL IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("user32.dll")]
        private static extern BOOL ShowWindow(HWND hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern HWND FindWindowW(string a, string b);

        #endregion

        //Function to determine which windows to add to the window list
        private static BOOL IsAltTabWindow(HWND hwnd)
        {
            //If window isn't visible it can't possibly be on the taskbar
            if (!IsWindowVisible(hwnd))
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
            //Get the monitor on which the current taskbar is located
            Screen taskbarScreen = Screen.FromHandle(FindWindowW("Shell_TrayWnd", ""));

            //Align the window on that monitor
            Width = taskbarScreen.Bounds.Width;
            Height = tb_height;
            int X = taskbarScreen.WorkingArea.Left;
            int Y = taskbarScreen.WorkingArea.Bottom;
            Location = new Point(X, Y);
            line.Width = Width;

            //Hide taskbar
            ShowWindow(FindWindowW("Shell_TrayWnd", ""), 0);
        }

        private void Taskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
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
                    MenuItem exit = new MenuItem { Text = "&Exit" };
                    exit.Click += delegate { Close(); };

                    //Add all menu items
                    d.MenuItems.Add(exit);
                }
                //Show the context menu
                d.Show(this, e.Location);
            }
        }

        private List<TaskbarProgram> icons = new List<TaskbarProgram>();

        private HWND lastOpenWindow;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Get the foreground window (Used later)
            HWND ForegroundWindow = GetForegroundWindow();
            if (ForegroundWindow != Handle)
                lastOpenWindow = ForegroundWindow;

            //Re-check the window handle list
            windows.Clear();
            EnumWindowsCallback d = EnumWind;
            EnumWindows(d, 0);

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

            //Re-display all windows
            int x = startButton1.Location.X + startButton1.Width + 2;
            foreach (TaskbarProgram icon in newIcons)
            {
                icon.ActiveWindow = icon.WindowHandle == ForegroundWindow;
                icon.Location = new Point(x, 0);
                icon.Title = icon.Window.Title;
                icon.Icon = GetAppIcon(icon.WindowHandle);
                if (!Controls.Contains(icon))
                    Controls.Add(icon);
                x += 162;
            }
        }
    }
}

