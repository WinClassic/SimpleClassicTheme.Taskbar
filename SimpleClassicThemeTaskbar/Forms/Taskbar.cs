using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static readonly CodeBridge cppCode = new();
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

        private bool dummy;
        private List<BaseTaskbarProgram> icons = new();
        private bool LookingForTray = false;
        private Range taskArea;

        private int taskIconWidth;
        private TimingDebugger timingDebugger = new();
        private bool watchLogic = true;
        private bool watchUI = true;
        private bool experimentGetDataPushed = false;
        private User32.WindowsHookProcedure hookProcedure;

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
            cppCode.InitCom();

            //Fix height according to renderers preferences
            Height = Config.Renderer.TaskbarHeight;
            startButton1.Height = Height;
            systemTray1.Height = Height;
            quickLaunch1.Height = Height;

            // Create shell hook
            if (File.Exists("egdp.txt")) 
                experimentGetDataPushed = true;
            if (experimentGetDataPushed)
            {
                hookProcedure = new User32.WindowsHookProcedure(HookProcedure);
                if (User32.SetWindowsHookEx(User32.WH_SHELL, hookProcedure, IntPtr.Zero, Kernel32.GetCurrentThreadId()) == IntPtr.Zero)
				{
                    int errorCode = Marshal.GetLastWin32Error();
                    Logger.Log(LoggerVerbosity.Basic, "Taskbar/Constructor", $"Failed to create Windows Hook. ({errorCode:X8})");
                    throw new Win32Exception(errorCode);
				}
            }

            //Make sure programs are aware of the new work area
            if (isPrimary)
            {
                windows.Clear();
                User32.EnumWindowsCallback callback = EnumWind;
                _ = User32.EnumWindows(callback, 0);
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
            if (m.Msg == User32.WM_ENDSESSION)
            {
                ApplicationEntryPoint.ExitSCTT();
            }

            if (m.Msg == User32.WM_QUERYENDSESSION)
            {
                m.Result = new IntPtr(1);
            }

            if (m.Msg == Constants.WM_SCT)
            {
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

            if (experimentGetDataPushed)
                timerUpdate.Start();
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
                //Kill background thread and show taskbar
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
        }

        private void Taskbar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                times.Clear();
                systemTray1.times.Clear();
                switch (Microsoft.VisualBasic.Interaction.InputBox(""))
                {
                    case "1":
                        watchUI = false;
                        break;

                    case "2":
                        watchLogic = false;
                        break;

                    case "3":
                        systemTray1.watchTray = false;
                        break;

                    default:
                        break;
                }
                return;
            }

            //Open context menu
            if (e.Button == MouseButtons.Right)
            {
                if (d == null)
                {
                    d = ConstructTaskbarContextMenu();
                }
                ////Show the context menu
                //d.Show(this, e.Location);
                SystemContextMenu menu = SystemContextMenu.FromContextMenuStrip(d);
                Point location = PointToScreen(e.Location);
                menu.Show(Handle, location.X, location.Y);
            }
            else if (e.Button == MouseButtons.Left && e.Location.X == Width - 1)
            {
                Keyboard.KeyPress(Keys.LWin, Keys.D);
            }
        }

        private IntPtr HookProcedure(User32.ShellEvents nCode, IntPtr wParam, IntPtr lParam)
        {
            Logger.Log(LoggerVerbosity.Verbose, "Taskbar/HookProcedure", $"Call parameters: {nCode}, {wParam:X8}, {lParam:X8}");

            if (nCode < 0)
                return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

            switch (nCode)
            {
                case User32.ShellEvents.HSHELL_WINDOWCREATED:

                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
		{

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
                cppCode.SetWorkingArea(rct.Left, rct.Right, rct.Top, rct.Bottom, Environment.OSVersion.Version.Major < 10, windowHandles);
            }

            if (Location.ToString() != desiredLocation.ToString())
                Location = desiredLocation;

            if (!watchLogic)
                timingDebugger.FinishRegion("Resize work area");
        }

        private static ContextMenuStrip ConstructTaskbarContextMenu()
        {
            ToolStripMenuItem cascadeWindows;
            ToolStripMenuItem showWindowsStacked;
            ToolStripMenuItem showWindowsSideBySide;
            ToolStripMenuItem taskManager;
            ToolStripMenuItem settings;
            ToolStripMenuItem showDesktop;
            ToolStripMenuItem exit;

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
                // exit = new("&Exit SCT Taskbar", null, (_, __) => ApplicationEntryPoint.ExitSCTT()),
            };

            cascadeWindows.Click += delegate { _ = User32.CascadeWindows(IntPtr.Zero, User32.MDITILE_ZORDER, IntPtr.Zero, 0, IntPtr.Zero); };
            showWindowsStacked.Click += delegate { _ = User32.TileWindows(IntPtr.Zero, User32.MDITILE_HORIZONTAL, IntPtr.Zero, 0, IntPtr.Zero); };
            showWindowsSideBySide.Click += delegate { _ = User32.TileWindows(IntPtr.Zero, User32.MDITILE_VERTICAL, IntPtr.Zero, 0, IntPtr.Zero); };
            settings.Click += delegate { new Settings().Show(); };

            ContextMenuStrip contextMenu = new();
            contextMenu.Items.AddRange(items);

            return contextMenu;
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