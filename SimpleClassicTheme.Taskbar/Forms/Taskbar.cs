﻿using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Common.Native.Controls;
using SimpleClassicTheme.Common.Performance;
using SimpleClassicTheme.Common.Providers;
using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;
using SimpleClassicTheme.Taskbar.Localization;
using SimpleClassicTheme.Taskbar.Native;
using SimpleClassicTheme.Taskbar.Providers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.ProcessThreadsAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.PSAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.WinBase;
using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Taskbar : Form
    {
        public bool busy = false;
        public bool CanInvoke = false;
        public BaseTaskbarProgram heldDownButton;
        public int heldDownOriginalX = 0;
        public int mouseOriginalX = 0;
        public bool NeverShow = false;
        public bool Primary = true;
        public bool selfClose = false;

        private static BaseTaskbarProgram lastActiveButton;

        //private Thread BackgroundThread;
        private readonly Screen _screen;

        private readonly List<string> BlacklistedClassNames = new();
        private readonly List<string> BlacklistedProcessNames = new();
        private readonly List<string> BlacklistedWindowNames = new();
        private readonly Dictionary<int, string> groupKeys = new();
        private readonly TimingDebugger logicTiming = new();
        private readonly TimingDebugger uiTiming = new();
        private ShellHook _shellHook;
        private bool dummy;
        private Range taskArea;
        private bool watchLogic = true;
        private bool watchUI = true;

        //Constructor
        public Taskbar(bool isPrimary)
        {
            LoadTaskbarFilters();

            //Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
            Primary = isPrimary;

            //Initialize thingies
            InitializeComponent();
            systemTray.SizeChanged += delegate { UpdateUI(); UpdateUI(); };
            TopLevel = true;

            UpdateHeight();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Taskbar"/> class from the specified <see cref="Screen"/>.
        /// </summary>
        /// <param name="screen">The <see cref="Screen"/> class from which to initialize this <see cref="Taskbar"/> class</param>
        public Taskbar(Screen screen) : this(screen.Primary)
        {
            _screen = screen;

            StartPosition = FormStartPosition.Manual;
            Location = GetLocation(_screen.Bounds);
            Size = new Size(_screen.Bounds.Width, Config.Default.Renderer.TaskbarHeight);
        }

        public static BaseTaskbarProgram LastActiveButton
        {
            get => lastActiveButton;
            set
            {
                if (lastActiveButton != null)
                {
                    lastActiveButton.ActiveWindow = false;
                }

                lastActiveButton = value;
                lastActiveButton.ActiveWindow = true;
            }
        }

        public static IntPtr LastActiveWindow { get; set; }
        public static IntPtr LastOpenWindow { get; set; }

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

        public Provider<Window> Provider { get; private set; }

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

        /// <summary>
        /// Gets the appropiate location for this taskbar based on the user's config.
        /// </summary>
        private static Point GetLocation(Rectangle bounds)
        {
            return Config.Default.Position switch
            {
                DockStyle.Top => new Point(bounds.Left, bounds.Top),
                DockStyle.Bottom => new Point(bounds.Left, bounds.Bottom - Config.Default.Renderer.TaskbarHeight),
                DockStyle.None or DockStyle.Fill => throw new ArgumentOutOfRangeException(),
                _ => throw new NotImplementedException(),
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Config.Default.Renderer.DrawTaskBar(this, e.Graphics);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)WM_ENDSESSION:
                    ApplicationEntryPoint.ExitSCTT();
                    break;

                case (int)WM_QUERYENDSESSION:
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
                    if (_shellHook != null && m.Msg == _shellHook.WindowMessage)
                    {
                        _shellHook.HandleWindowMessage(m.WParam, m.LParam);
                    }
                    break;
            }

            base.WndProc(ref m);
        }


        /// <summary>
        /// Adds the taskbar button while respecting grouping.
        /// </summary>
        /// <param name="newButton">The new button.</param>
        private void AddTaskbarButton(SingleTaskbarProgram newButton)
        {
            if (!Config.Default.EnableGrouping)
            {
                Controls.Add(newButton);
                return;
            }

            BaseTaskbarProgram sameThing = Programs.FirstOrDefault(p =>
            {
                return IsGroupConditionMet(p, newButton);
            });

            switch (sameThing)
            {
                case GroupedTaskbarProgram group:
                    if (!group.ProgramWindows.Contains(newButton))
                    {
                        newButton.IsMoving = false;
                        group.ProgramWindows.Add(newButton);
                    }
                    return;

                case SingleTaskbarProgram existingButton:
                    {
                        GroupedTaskbarProgram newGroup = ConstructGroupButton(existingButton, newButton);
                        Controls.Add(newGroup);
                        Controls.Remove(existingButton);
                        break;
                    }

                default:
                    Controls.Add(newButton);
                    break;
            }
        }

        private void ApplyWorkArea(IEnumerable<Window> windows)
        {
            if (_screen == null)
            {
                Logger.Instance.Log(LoggerVerbosity.Detailed, "Taskbar", "Failed to apply work area because no screen was provided when taskbar was created.");
                return;
            }

            using var _ = logicTiming.StartRegion("Resize work area");

            Rectangle workArea = _screen.Bounds;
            Point desiredLocation = GetLocation(workArea);

            switch (Config.Default.Position)
            {
                case DockStyle.Top:
                    workArea.Y += Height;
                    workArea.Height -= Height;
                    break;
                
                case DockStyle.Bottom:
                    workArea.Height -= Height;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (!_screen.WorkingArea.Equals(workArea))
            {
                var windowHandles = windows.Select(a => a.Handle).ToArray();
                var isBelowWin10 = Environment.OSVersion.Version.Major < 10;
                UnmanagedCodeMigration.SetWorkingArea(workArea, isBelowWin10, windowHandles);
            }

            if (!Location.Equals(desiredLocation))
                Location = desiredLocation;
        }

        private GroupedTaskbarProgram ConstructGroupButton(params SingleTaskbarProgram[] programs)
        {
            GroupedTaskbarProgram group = new() { IsMoving = false };

            group.MouseMove += Taskbar_IconMove;
            group.MouseDown += Taskbar_IconDown;
            group.MouseUp += Taskbar_IconUp;
            group.ProgramWindows.AddRange(programs);

            return group;
        }

        private SingleTaskbarProgram CreateTaskbandButton(Window window)
        {
            SingleTaskbarProgram button = new(window)
            {
                Height = Height,
                ActiveWindow = false,
            };

            button.MouseDown += Taskbar_IconDown;
            button.MouseMove += Taskbar_IconMove;
            button.MouseUp += Taskbar_IconUp;

            return button;
        }

        private void DisposeTaskbarProgram(BaseTaskbarProgram program)
        {
            Logger.Instance.Log(LoggerVerbosity.Verbose, "Taskbar", $"Deleting window {program.Title}.");
            Controls.Remove(program);
            //Programs.Remove(program);
            program.Dispose();
        }

        private string GetGroupKey(Window window)
        {
            var handle = window.Handle.ToInt32();

            if (groupKeys.ContainsKey(handle))
            {
                return groupKeys[handle];
            }

            try
            {
                if (GetExitCodeProcess(window.ProcessHandle, out var exitCode) && exitCode != STILL_ACTIVE)
                {
                    return null;
                }

                return Config.Default.Tweaks.ProgramGroupCheck switch
                {
                    ProgramGroupCheck.Process => window.ProcessId.ToString(),
                    ProgramGroupCheck.FileNameAndPath => GetProcessFileName(window.ProcessHandle),
                    ProgramGroupCheck.ModuleName => GetProcessModuleName(window.ProcessHandle),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
            {
                // If we got an access denied exception we catch it and return false.
                // Otherwise we re-throw the exception
                Logger.Instance.Log(LoggerVerbosity.Basic, "Taskbar/Groups", $"Failed to compare taskbar programs: {ex}");
                return null;
            }
        }

        private void HideExplorerTaskbars()
        {
            var enumTrayWindows = GetTrayWindows();

            foreach (Window w in enumTrayWindows)
                if ((w.WindowInfo.dwStyle & 0x10000000L) > 0)
                    ShowWindow(w.Handle, 0);
        }

        private bool IsGroupConditionMet(BaseTaskbarProgram a, BaseTaskbarProgram b)
        {
            var aKey = GetGroupKey(a.Window);
            var bKey = GetGroupKey(b.Window);

            if (aKey == null || bKey == null)
            {
                return false;
            }

            return aKey == bKey;
        }

        private void LayoutTaskbandButtons()
        {
            var icons = Programs.ToArray();

            // Calculate availabe space in taskbar and then divide that space over all programs
            int startX = quickLaunch.Location.X + quickLaunch.Width + 4;
            int programWidth = Primary ? Config.Default.Tweaks.TaskbarProgramWidth + Config.Default.Tweaks.SpaceBetweenTaskbarIcons : 24;

            int availableSpace = verticalDivider.Location.X - startX - 6;
            availableSpace += Config.Default.Tweaks.SpaceBetweenTaskbarIcons;

            if (icons.Length > 0 && availableSpace / icons.Length > programWidth)
                availableSpace = icons.Length * programWidth;

            int x = startX;
            int iconWidth = Enumerable.Any(icons) ? (int)Math.Floor((double)availableSpace / icons.Length) - Config.Default.Tweaks.SpaceBetweenTaskbarIcons : 1;
            int maxX = verticalDivider.Location.X - iconWidth;

            if (maxX < 0)
            {
                Logger.Instance.Log(LoggerVerbosity.Basic, "Taskbar", "maxX is below 0, aborting layout!");
                return;
            }

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
                // icon.Visible = true;

                verticalDivider.BringToFront();

                x += icon.Width + Config.Default.Tweaks.SpaceBetweenTaskbarIcons;

                if (!Controls.Contains(icon))
                {
                    Controls.Add(icon);
                }

                // icon.Invalidate();
            }

            if (heldDownButton != null)
            {
                heldDownButton.BringToFront();
                verticalDivider.BringToFront();
            }

            taskArea = new Range(new Index(startX), new Index(maxX));

            Invalidate(true);
        }

        public void LayoutUI()
        {
            quickLaunch.Location = new Point(startButton.Location.X + startButton.Width + 2, 1);
            LayoutTaskbandButtons();
            verticalDivider.Location = new Point(systemTray.Location.X - 9, verticalDivider.Location.Y);
        }

        private void LoadTaskbarFilters()
        {
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
        }

        private void Provider_ItemAdded(object sender, ProviderEventArgs<Window> e)
        {
            if (ShouldIgnoreWindow(e.Item))
            {
                return;
            }

            SingleTaskbarProgram newButton = CreateTaskbandButton(e.Item);
            AddTaskbarButton(newButton);
        }

        private void Provider_ItemRemoved(object sender, ProviderEventArgs<Window> e)
        {
            var button = Programs.FirstOrDefault(btp =>
            {
                return btp.Window.Handle.Equals(e.Item.Handle);
            });

            if (button == null)
            {
                return;
            }

            if (button is GroupedTaskbarProgram group)
            {
                RemoveFromGroupButton(group, e.Item);
            }
            else
            {
                DisposeTaskbarProgram(button);
            }
        }

        private void RemoveFromGroupButton(GroupedTaskbarProgram group, Window window)
        {
            var count = group.ProgramWindows.Count;
            var singleButton = group.ProgramWindows.FirstOrDefault(stp => stp.Window == window);

            if (count > 2)
            {
                group.ProgramWindows.Remove(singleButton);
            }
            else
            {
                if (count == 1)
                {
                    Controls.Add(singleButton);
                    group.ProgramWindows.Remove(singleButton);
                }

                group.Dispose();
                Controls.Remove(group);
            }
        }

        private void Taskbar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!selfClose)
            {
                //If we don't close ourselves, show the shutdown dialog
                var hWnd = FindWindow("Shell_TrayWnd", "");
                Keyboard.KeyPress(hWnd, Keys.Menu, Keys.F4);
                e.Cancel = true;
            }
            else
            {
                //Show taskbar
                _ = ShowWindow(FindWindow("Shell_TrayWnd", ""), 5);
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
                heldDownButton.BringToFront();

                if ((MouseButtons & MouseButtons.Left) == 0)
                    heldDownButton = null;

                int x = taskArea.Start.Value;
                foreach (BaseTaskbarProgram icon in Programs)
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

        //Initialize stuff
        private void Taskbar_Load(object sender, EventArgs e)
        {
            //Make sure programs are aware of the new work area
            if (Primary)
            {
                var windows = GetTaskbarWindows();
                foreach (Window w in windows)
                {
                    //WM_WININICHANGE - SPI_SETWORKAREA
                    _ = PostMessage(w.Handle, 0x001A, 0x002F, 0);
                }
            }

            //TODO: Add an option to registry tweak classic alt+tab
            quickLaunch.Disabled = (!Primary) || (!Config.Default.EnableQuickLaunch);
            quickLaunch.UpdateIcons();

            if (Config.Default.EnableActiveTaskbar)
            {
                Provider = new ActiveWindowProvider();
                timerUpdateInformation.Start();
            }
            else
            {
                _shellHook = new ShellHook(Handle);
                _shellHook.WindowActivated += (_, wParam) =>
                {
                    if (ShouldIgnoreWindow(new(wParam)))
                    {
                        return;
                    }

                    UpdateActiveWindow(wParam);
                };

                Provider = new ShellHookWindowProvider(_shellHook);

                var initialWindows = GetTaskbarWindows().Where(w => !ShouldIgnoreWindow(w)).ToArray();
                foreach (var window in initialWindows)
                {
                    if (ShouldIgnoreWindow(window))
                    {
                        continue;
                    }

                    AddTaskbarButton(CreateTaskbandButton(window));
                }

                HideExplorerTaskbars();
                ApplyWorkArea(Programs.Select(p => p.Window));

                timerUpdate.Start();
            }

            Provider.ItemAdded += Provider_ItemAdded;
            Provider.ItemRemoved += Provider_ItemRemoved;
        }

        private void Taskbar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                using (TaskbarContextMenu contextMenu = new(this))
                {
                    Point location = PointToScreen(e.Location);
                    contextMenu.Show(this, location.X, location.Y);
                }
            }
            else if (e.Button == MouseButtons.Left && e.Location.X == Width - 1)
            {
                Keyboard.KeyPress(Keys.LWin, Keys.D);
            }
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            startButton.UpdateState(new Window(foregroundWindow));

            if (_shellHook == null)
            {
                UpdateActiveWindow(foregroundWindow);
            }

            if (Primary)
            {
                systemTray.UpdateTime();

                using (uiTiming.StartRegion("Updating System Tray icons"))
                {
                    systemTray.UpdateIcons();
                }

                using (uiTiming.StartRegion("Update Quick Launch icons"))
                {
                    quickLaunch.UpdateIcons();
                }
            }

            //Put left side controls in the correct place
            LayoutUI();
        }

        /// <summary>
        /// Highlights only taskband buttons that match the provided window handle.
        /// </summary>
        private void UpdateActiveWindow(IntPtr hWnd)
        {
            if (LastActiveButton?.Window.Handle == hWnd)
            {
                return;
            }

            LastActiveWindow = hWnd;
            var button = Programs.FirstOrDefault(prg => prg.IsWindow(LastActiveWindow));

            // No button found that could be our window, creating new one
            if (button == null)
            {
                var newButton = CreateTaskbandButton(new(hWnd));
                AddTaskbarButton(newButton);
                button = newButton;
            }

            LastActiveButton = button;
        }

        private void UpdateHeight()
        {
            //Fix height according to renderers preferences
            Height = Config.Default.Renderer.TaskbarHeight;
            startButton.Height = Height;
            systemTray.Height = Height;
            quickLaunch.Height = Height;
        }

        private void UpdateUI()
        {
            var foregroundWindow = GetForegroundWindow();
            startButton.UpdateState(new Window(foregroundWindow));

            if (Primary)
            {
                systemTray.UpdateTime();

                using (uiTiming.StartRegion("Updating System Tray icons"))
                {
                    systemTray.UpdateIcons();
                }

                using (uiTiming.StartRegion("Update Quick Launch icons"))
                {
                    quickLaunch.UpdateIcons();
                }
            }

            if (_shellHook == null)
            {
                UpdateActiveWindow(foregroundWindow);
            }

            LayoutUI();

            Invalidate();
        }
    }

    internal class TaskbarContextMenu : SystemPopupMenu
    {
        private readonly Taskbar _taskbar;

        public TaskbarContextMenu(Taskbar taskbar)
        {
            _taskbar = taskbar;
            
            Add(new SystemPopupMenuItem() { Text = WindowsStrings.Toolbars, Enabled = false });
            Add(new SystemPopupMenuSeparator());
            Add(new SystemPopupMenuItem(WindowsStrings.CascadeWindows, CascadeWindows_Click));
            Add(new SystemPopupMenuItem(WindowsStrings.TileWindowsHorizontally, TileWindowsHorizontally_Click));
            Add(new SystemPopupMenuItem(WindowsStrings.TileWindowsVertically, TileWindowsVertically_Click));
            Add(new SystemPopupMenuItem(WindowsStrings.ShowDesktop, ShowDesktop_Click));
            Add(new SystemPopupMenuSeparator());
            Add(new SystemPopupMenuItem(WindowsStrings.TaskManager, OpenTaskManager_Click));
            Add(new SystemPopupMenuSeparator());
            Add(new SystemPopupMenuItem(WindowsStrings.LockTaskbar, LockTaskbar_Click) { Checked = Config.Default.IsLocked });
            Add(new SystemPopupMenuItem(WindowsStrings.Properties, OpenProperties_Click));
            Add(new SystemPopupMenuItem("&Exit SCT Taskbar", ExitSCTT_Click) { Visible = ShouldShowExit});
        }

        private void ExitSCTT_Click(object sender, EventArgs e)
        {
            ApplicationEntryPoint.ExitSCTT();
        }

        private void OpenProperties_Click(object sender, EventArgs e)
        {
            new Settings().Show();
        }

        private void OpenTaskManager_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("taskmgr") { UseShellExecute = true });
        }

        private void CascadeWindows_Click(object sender, EventArgs e)
        {
            CascadeWindows(
                IntPtr.Zero,
                MDITILE_ZORDER,
                IntPtr.Zero,
                0,
                IntPtr.Zero);
        }

        private void TileWindowsHorizontally_Click(object sender, EventArgs e)
        {
            TileWindows(
                IntPtr.Zero,
                MDITILE_HORIZONTAL,
                IntPtr.Zero,
                0,
                IntPtr.Zero);
        }

        private void TileWindowsVertically_Click(object sender, EventArgs e)
        {
            TileWindows(
                IntPtr.Zero,
                MDITILE_VERTICAL,
                IntPtr.Zero,
                0,
                IntPtr.Zero);
        }

        private void LockTaskbar_Click(object sender, EventArgs e)
        {
            Config.Default.IsLocked = !Config.Default.IsLocked;
            Config.Default.WriteToRegistry();

            _taskbar.LayoutUI();
            _taskbar.Invalidate();
        }

        private void ShowDesktop_Click(object sender, EventArgs e)
        {
            Keyboard.KeyPress(Keys.LWin, Keys.D);
        }

        private static bool ShouldShowExit
        {
            get
            {
                if (Config.Default.ExitMenuItemCondition == ExitMenuItemCondition.Always)
                {
                    return true;
                }

                return Control.ModifierKeys.HasFlag(Keys.Control | Keys.Shift);
            }
        }
    }
}