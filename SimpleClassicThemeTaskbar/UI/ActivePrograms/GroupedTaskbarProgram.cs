using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public class GroupedTaskbarProgram : BaseTaskbarProgram
    {
        public PopupTaskbarGroup GroupWindow;
        public List<SingleTaskbarProgram> ProgramWindows = new();
        private const int MAX_PATH = 255;
        private readonly bool line = false;

        public GroupedTaskbarProgram()
        {
            Constructor();
            GroupWindow = new PopupTaskbarGroup(this);
            SpaceNeededNextToText = line ? 19 : 16;
        }

        public bool CanBeSingleWindow => ProgramWindows.Count < 2;

        private SingleTaskbarProgram PrimaryWindow => ProgramWindows.FirstOrDefault();
        public override Icon Icon
        {
            get => PrimaryWindow?.Icon;
            set => PrimaryWindow.Icon = value;
        }

        public override Image IconImage
        {
            get
            {
                if (Icon != null)
                {
                    try
                    {
                        return new Icon(Icon, 16, 16).ToBitmap();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Ignore
                    }
                }

                return null;
            }
        }

        public override int MinimumWidth => Config.Default.Renderer.TaskButtonMinimalWidth + 18;

        public override Process Process
        {
            get => PrimaryWindow?.Process;
            set => PrimaryWindow.Process = value;
        }

        public override string Title
        {
            get => PrimaryWindow?.Title;
            set => PrimaryWindow.Title = value;
        }

        public override Window Window
        {
            get => PrimaryWindow.Window;
            set => PrimaryWindow.Window = value;
        }

        public bool ContainsWindow(IntPtr hwnd)
        {
            foreach (SingleTaskbarProgram program in ProgramWindows)
                if (program.Window.Handle == hwnd)
                    return true;
            return false;
        }

        public override void FinishOnPaint(PaintEventArgs e)
        {
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "painting grouped window extension";

            Config.Default.Renderer.DrawTaskButtonGroupButton(this, e.Graphics);
        }

        public override string GetErrorString()
            => GetBaseErrorString() +
            $"Process: {Process.MainModule.ModuleName} ({Process.Id})\n" +
            $"Window title: {Title}\n" +
            $"Window class: {Window.ClassName}\n" +
            $"Window HWND: {string.Join(", ", ProgramWindows.Select(o => o.Window.Handle.ToString("X8") + (User32.IsWindow(o.Window.Handle) ? "Valid" : "Invalid")).ToArray())}\n" +
            $"Icon HWND: {string.Join(", ", ProgramWindows.Select(o => o.Icon.Handle.ToString("X8") + (User32.IsWindow(o.Icon.Handle) ? "Valid" : "Invalid")).ToArray())}";

        public override bool IsActiveWindow(IntPtr activeWindow)
        {
            ApplicationEntryPoint.ErrorSource = this;
            controlState = "reordering group";
            foreach (SingleTaskbarProgram window in ProgramWindows)
            {
                window.Icon = Taskbar.GetAppIcon(window.Window);
                if (window.Window.Handle == activeWindow)
                {
                    ActiveWindow = true;
                    window.ActiveWindow = true;

                    _ = ProgramWindows.Remove(window);
                    ProgramWindows.Insert(0, window);
                    return true;
                }
                else
                {
                    window.ActiveWindow = false;
                }
            }
            ActiveWindow = false;
            return false;
        }

		public override void OnDoubleClick(object sender, MouseEventArgs e)
		{
            if (e.X > Width - 19 &&
                e.X < Width - 3 &&
                e.Y > 7 &&
                e.Y < 24)
            {
                GroupWindow.Show(PointToScreen(new Point(0, 0)));
            }
            else if (IsMoving)
            {
                IsMoving = false;
            }
            else
            {
                PrimaryWindow.OnDoubleClick(sender, e);
                ActiveWindow = PrimaryWindow.ActiveWindow;
            }
        }

		public override void OnClick(object sender, MouseEventArgs e)
        {
            if (e.X > Width - 19 &&
                e.X < Width - 3 &&
                e.Y > 7 &&
                e.Y < 24)
            {
                GroupWindow.Show(PointToScreen(new Point(0, 0)));
            }
            else if (IsMoving)
            {
                IsMoving = false;
            }
            else
            {
                PrimaryWindow.OnClick(sender, e);
                ActiveWindow = PrimaryWindow.ActiveWindow;
            }
        }

        public override string ToString()
        {
            if (Config.Default.Tweaks.ProgramGroupCheck == ProgramGroupCheck.Process)
            {
                return $"Process - ID: {Process.Id}, Name: {Process.ProcessName}";
            }
            else if (Config.Default.Tweaks.ProgramGroupCheck == ProgramGroupCheck.FileNameAndPath)
            {
                return $"Filepath - {GetShortPath(Process.MainModule.FileName)}";
            }
            else if (Config.Default.Tweaks.ProgramGroupCheck == ProgramGroupCheck.ModuleName)
            {
                return $"Filename - {Process.MainModule.ModuleName}";
            }
            return "None - I shouldn't be here!";
        }

        public bool RemoveWindow(IntPtr window)
		{
            List<SingleTaskbarProgram> referenceList = new(ProgramWindows);
            foreach (SingleTaskbarProgram program in referenceList)
			{
                if (program.Window.Handle == window)
				{
                    ProgramWindows.Remove(program);
                    program.Dispose();
				}
			}

            return ProgramWindows.Count > 1;
		}

        public bool UpdateWindowList(IEnumerable<Window> windows)
        {
            //The new list of icons
            List<SingleTaskbarProgram> newIcons = new();

            //Create a new list with only the windows that are still open
            foreach (SingleTaskbarProgram baseIcon in ProgramWindows)
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
            }

            //Remove controls of the windows that were removed from the list
            SingleTaskbarProgram[] enumerator = new SingleTaskbarProgram[ProgramWindows.Count];
            ProgramWindows.CopyTo(enumerator);
            foreach (SingleTaskbarProgram dd in enumerator)
            {
                if (dd is SingleTaskbarProgram)
                {
                    SingleTaskbarProgram icon = dd as SingleTaskbarProgram;
                    if (!newIcons.Contains(icon))
                    {
                        _ = ProgramWindows.Remove(icon);
                        icon.Dispose();
                    }
                }
            }

            return ProgramWindows.Count > 0;
        }

        protected override bool CancelMouseDown(MouseEventArgs e)
        {
            if (e.X > Width - 19 &&
                e.X < Width - 3 &&
                e.Y > 7 &&
                e.Y < 24)
            {
                return true;
            }
            return false;
        }

        private static string GetShortPath(string path)
        {
            var shortPath = new StringBuilder(MAX_PATH);
            _ = Kernel32.GetShortPathName(path, shortPath, MAX_PATH);
            return shortPath.ToString();
        }
    }
}