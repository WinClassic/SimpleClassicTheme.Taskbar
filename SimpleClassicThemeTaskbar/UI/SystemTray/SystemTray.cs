using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    public partial class SystemTray : UserControlEx
    {
        public SystemTrayIcon heldDownIcon = null;
        public int heldDownOriginalX = 0;
        public List<SystemTrayIcon> icons = new();
        public int mouseOriginalX = 0;
        public List<(string, TimeSpan)> times = new();
        public bool watchTray = true;
        private readonly Stopwatch sw = new();
        private object culprit;

        public SystemTray()
        {
            InitializeComponent();
            Point p = Config.Renderer.SystemTrayTimeLocation;
            labelTime.Location = new Point(Width + p.X, p.Y);
            labelTime.Font = Config.Renderer.SystemTrayTimeFont;
            labelTime.ForeColor = Config.Renderer.SystemTrayTimeColor;
        }

        public void ClearButtons()
        {
            Control[] controls = new Control[Controls.Count];
            Controls.CopyTo(controls, 0);
            foreach (Control d in controls)
            {
                if (d is SystemTrayIcon)
                {
                    Controls.Remove(d);
                    d.Dispose();
                }
            }
        }

        public override string GetErrorString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(GetBaseErrorString());
            sb.AppendLine("Unexpected error occured while " + controlState);

            var buttonInfo = GetCulpritButtonInfo();
            var process = GetProcessFromIcon(buttonInfo).MainModule.ModuleName;

            var windowHwnd = buttonInfo.hwnd;
            var isWindowHwndValid = User32.IsWindow(windowHwnd);

            var iconHwnd = buttonInfo.icon;
            var isIconHwndValid = User32.IsWindow(iconHwnd);

            sb.AppendLine($"Process: {process} ({buttonInfo.pid})");
            sb.AppendLine($"Corresponding Window HWND: {windowHwnd} ({(isWindowHwndValid ? "Valid" : "Invalid")})");
            sb.AppendLine($"Icon HWND: {iconHwnd} ({(isIconHwndValid ? "Valid" : "Invalid")})");
            sb.AppendLine($"Icon Caption: {buttonInfo.toolTip}");
            sb.AppendLine($"Icon ID: {buttonInfo.id}");

            return sb.ToString();
        }

        public void UpdateIcons()
        {
            if (!watchTray)
            {
                sw.Reset();
                sw.Start();
            }

            //Lists that will receive all button information
            List<UnmanagedCodeMigration.TBBUTTONINFO> existingButtons = new();
            List<IntPtr> existingHWNDs = new();
            IntPtr tray = GetSystemTrayHandle();

            //Loop through all buttons
            UnmanagedCodeMigration.TBBUTTONINFO[] bInfos = UnmanagedCodeMigration.GetTrayButtons(tray);
            foreach (UnmanagedCodeMigration.TBBUTTONINFO bInfo in bInfos)
            {
                //Save it
                if (bInfo.visible)
                {
                    existingButtons.Add(bInfo);
                    existingHWNDs.Add(bInfo.hwnd);
                }
            }

            if (!watchTray)
            {
                times.Add(("Get all TBUTTONINFO's", sw.Elapsed));
                sw.Reset();
                sw.Start();
            }

            //Remove any icons that are now invalid or hidden
            List<SystemTrayIcon> newIconList = new();
            SystemTrayIcon[] enumerator = new SystemTrayIcon[icons.Count];
            icons.CopyTo(enumerator);
            foreach (SystemTrayIcon existingIco in enumerator)
                if (existingIco is SystemTrayIcon existingIcon)
                    if (existingHWNDs.Contains(existingIcon.Handle))
                        newIconList.Add(existingIcon);
                    else
                    {
                        _ = icons.Remove(existingIcon);
                        Controls.Remove(existingIcon);
                        existingIcon.Dispose();
                    }

            if (!watchTray)
            {
                times.Add(("Remove icons that are invalid", sw.Elapsed));
                sw.Reset();
                sw.Start();
            }

            //Add icons that didn't display before
            foreach (UnmanagedCodeMigration.TBBUTTONINFO info in existingButtons)
            {
                //By default we say the icon doesn't exist
                bool exists = false;
                SystemTrayIcon existingIcon = null;

                //Then we loop through each control to see if it actually does exist
                foreach (SystemTrayIcon icon in newIconList)
                    if (icon.Handle == info.hwnd)
                    {
                        existingIcon = icon;
                        exists = true;
                    }

                controlState = "updating existing tray icon";
                culprit = existingIcon;
                //If it does we just update it, else we create it
                if (exists)
                    existingIcon.UpdateTrayIcon(info);
                else
                {
                    controlState = "creating new tray icon";
                    culprit = info;
                    SystemTrayIcon trayIcon = new(info);
                    trayIcon.MouseDown += SystemTray_IconDown;
                    newIconList.Add(trayIcon);
                }
            }

            if (!watchTray)
            {
                times.Add(("Add icons that didn't display", sw.Elapsed));
                sw.Reset();
                sw.Start();
            }

            //De-dupe all controls
            List<SystemTrayIcon> finalIconList = new();
            List<IntPtr> pointers = new();
            foreach (SystemTrayIcon icon in newIconList)
            {
                if (!pointers.Contains(icon.Handle))
                    finalIconList.Add(icon);
                else
                    icon.Dispose();
                pointers.Add(icon.Handle);
            }

            //Icons are drawn from right to left so we reverse it so we can draw left to right (probably as easy)
            finalIconList.Reverse();

            if (!watchTray)
            {
                times.Add(("De-dupe and display everything", sw.Elapsed));
                sw.Reset();
                sw.Start();
            }

            //Display all controls
            int virtualWidth = 16 + Config.SpaceBetweenTrayIcons;

            Invalidate();
            //betterBorderPanel1.Refresh();
            //betterBorderPanel1.Update();

            int startX = 3;
            int iconWidth = 16;
            int iconSpacing = Config.SpaceBetweenTrayIcons;

            //See if we're moving, if so calculate new position, if we finished calculate new position and finalize position values
            if (heldDownIcon != null)
            {
                if (Math.Abs(mouseOriginalX - Cursor.Position.X) > 3)
                    heldDownIcon.IsMoving = true;
                if ((MouseButtons & MouseButtons.Left) != 0)
                {
                    Point p = new(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownIcon.Location.Y);
                    int newIndex = (startX - p.X - ((iconWidth + iconSpacing) / 2)) / (iconWidth + iconSpacing) * -1;
                    if (newIndex < 0) newIndex = 0;
                    _ = finalIconList.Remove(heldDownIcon);
                    finalIconList.Insert(Math.Min(finalIconList.Count, newIndex), heldDownIcon);
                }
                else
                {
                    Point p = new(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownIcon.Location.Y);
                    int newIndex = (startX - p.X - ((iconWidth + iconSpacing) / 2)) / (iconWidth + iconSpacing) * -1;
                    if (newIndex < 0) newIndex = 0;
                    _ = finalIconList.Remove(heldDownIcon);
                    finalIconList.Insert(Math.Min(finalIconList.Count, newIndex), heldDownIcon);
                    _ = icons.Remove(heldDownIcon);
                    icons.Insert(Math.Max(Math.Min(icons.Count, finalIconList.Count - newIndex - 1), 0), heldDownIcon);
                    heldDownIcon = null;
                }
            }

            if (!watchTray)
            {
                times.Add(("Check moving", sw.Elapsed));
                sw.Reset();
                sw.Start();
            }

            Width = Config.Renderer.GetSystemTrayWidth(finalIconList.Count);

            foreach (SystemTrayIcon icon in finalIconList)
            {
                //Add the control
                if (icon.Parent != this)
                {
                    icons.Add(icon);
                    Controls.Add(icon);
                }

                //Put the control at the correct position
                icon.Location = Config.Renderer.GetSystemTrayIconLocation(finalIconList.IndexOf(icon));
            }

            if (!watchTray)
            {
                watchTray = true;
                sw.Stop();
                string f = "Watch SystemTray: \n";
                foreach ((string, TimeSpan) ts in times)
                    f += $"{ts.Item2}\t{ts.Item1}\n";
                f += $"{sw.Elapsed}\tFinal bit";
                _ = MessageBox.Show(f);
            }

            //Move to the left
            int X = Parent.Size.Width - Width;
            int Y = Location.Y;

            Location = new Point(X, Y);
        }

        public void UpdateTime()
        {
            labelTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
        }

        private static Process GetProcessFromIcon(UnmanagedCodeMigration.TBBUTTONINFO TBUTTONINFO_Struct)
        {
            _ = User32.GetWindowThreadProcessId(TBUTTONINFO_Struct.hwnd, out uint pid);
            return Process.GetProcessById((int)pid);
        }

        private static IntPtr GetSystemTrayHandle()
        {
            IntPtr hWndTray = User32.FindWindowW("Shell_TrayWnd", null);
            if (hWndTray != IntPtr.Zero)
            {
                hWndTray = User32.FindWindowExW(hWndTray, IntPtr.Zero, "TrayNotifyWnd", null);
                if (hWndTray != IntPtr.Zero)
                {
                    hWndTray = User32.FindWindowExW(hWndTray, IntPtr.Zero, "SysPager", null);
                    if (hWndTray != IntPtr.Zero)
                    {
                        hWndTray = User32.FindWindowExW(hWndTray, IntPtr.Zero, "ToolbarWindow32", null);
                        return hWndTray;
                    }
                }
            }
            return IntPtr.Zero;
        }

        private UnmanagedCodeMigration.TBBUTTONINFO GetCulpritButtonInfo()
        {
            if (culprit is SystemTrayIcon trayIcon)
            {
                return trayIcon.TBUTTONINFO_Struct;
            }
            else if (culprit is UnmanagedCodeMigration.TBBUTTONINFO @struct)
            {
                return @struct;
            }
            else
            {
                return new();
            }
        }

        private void labelTime_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(labelTime, DateTime.Now.ToShortDateString());
        }

        private void SystemTray_IconDown(object sender, MouseEventArgs e)
        {
            heldDownIcon = (SystemTrayIcon)sender;
            heldDownOriginalX = heldDownIcon.Location.X;
            mouseOriginalX = Cursor.Position.X;
        }

        private void SystemTray_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Config.Renderer.DrawSystemTray(this, e.Graphics);
        }
    }
}