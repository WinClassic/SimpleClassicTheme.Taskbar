using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    public partial class SystemTray : UserControlEx
    {
        public SystemTrayIcon heldDownIcon = null;
        public int heldDownOriginalX = 0;
        public List<SystemTrayIcon> icons = new List<SystemTrayIcon>();
        public int mouseOriginalX = 0;
        public List<(string, TimeSpan)> times = new();
        public bool watchTray = true;
        private object culprit;
        private CodeBridge d = new CodeBridge();
        private Stopwatch sw = new();

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
            => GetBaseErrorString() +
            $"Unexpected error occured while {controlState}\n" +
            (culprit is SystemTrayIcon trayIcon ?
                $"Process: {GetProcessFromIcon(trayIcon.TBUTTONINFO_Struct).MainModule.ModuleName} ({trayIcon.TBUTTONINFO_Struct.pid})\n" +
                $"Corresponding Window HWND: {trayIcon.TBUTTONINFO_Struct.hwnd} (Valid: {User32.IsWindow(trayIcon.TBUTTONINFO_Struct.hwnd)})\n" +
                $"Icon HWND: {trayIcon.TBUTTONINFO_Struct.icon} (Valid: {User32.IsWindow(trayIcon.TBUTTONINFO_Struct.icon)})\n" +
                $"Icon caption: \n({trayIcon.TBUTTONINFO_Struct.toolTip})\n" +
                $"Icon ID: {trayIcon.TBUTTONINFO_Struct.id}\n" : String.Empty) +
            (culprit is CodeBridge.TBUTTONINFO TBUTTONINFO_Struct ?
                $"Process: {GetProcessFromIcon(TBUTTONINFO_Struct).MainModule.ModuleName} ({TBUTTONINFO_Struct.pid})\n" +
                $"Corresponding Window HWND: {TBUTTONINFO_Struct.hwnd} (Valid: {User32.IsWindow(TBUTTONINFO_Struct.hwnd)})\n" +
                $"Icon HWND: {TBUTTONINFO_Struct.icon} (Valid: {User32.IsWindow(TBUTTONINFO_Struct.icon)})\n" +
                $"Icon caption: \n({TBUTTONINFO_Struct.toolTip})\n" +
                $"Icon ID: {TBUTTONINFO_Struct.id}\n" : String.Empty);

        public void UpdateIcons()
        {
            if (!watchTray)
            {
                sw.Reset();
                sw.Start();
            }

            //Get button count
            int count = d.GetTrayButtonCount(GetSystemTrayHandle());

            //Lists that will receive all button information
            List<CodeBridge.TBUTTONINFO> existingButtons = new List<CodeBridge.TBUTTONINFO>();
            List<IntPtr> existingHWNDs = new List<IntPtr>();
            IntPtr tray = GetSystemTrayHandle();

            //Loop through all buttons
            CodeBridge.TBUTTONINFO[] bInfos = d.GetTrayButtons(tray, count);
            foreach (CodeBridge.TBUTTONINFO bInfo in bInfos)
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
            List<SystemTrayIcon> newIconList = new List<SystemTrayIcon>();
            SystemTrayIcon[] enumerator = new SystemTrayIcon[icons.Count];
            icons.CopyTo(enumerator);
            foreach (SystemTrayIcon existingIco in enumerator)
                if (existingIco is SystemTrayIcon existingIcon)
                    if (existingHWNDs.Contains(existingIcon.Handle))
                        newIconList.Add(existingIcon);
                    else
                    {
                        icons.Remove(existingIcon);
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
            foreach (CodeBridge.TBUTTONINFO info in existingButtons)
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
                    SystemTrayIcon trayIcon = new SystemTrayIcon(info);
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
            List<SystemTrayIcon> finalIconList = new List<SystemTrayIcon>();
            List<IntPtr> pointers = new List<IntPtr>();
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
                    Point p = new Point(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownIcon.Location.Y);
                    int newIndex = (startX - p.X - ((iconWidth + iconSpacing) / 2)) / (iconWidth + iconSpacing) * -1;
                    if (newIndex < 0) newIndex = 0;
                    finalIconList.Remove(heldDownIcon);
                    finalIconList.Insert(Math.Min(finalIconList.Count, newIndex), heldDownIcon);
                }
                else
                {
                    Point p = new Point(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownIcon.Location.Y);
                    int newIndex = (startX - p.X - ((iconWidth + iconSpacing) / 2)) / (iconWidth + iconSpacing) * -1;
                    if (newIndex < 0) newIndex = 0;
                    finalIconList.Remove(heldDownIcon);
                    finalIconList.Insert(Math.Min(finalIconList.Count, newIndex), heldDownIcon);
                    icons.Remove(heldDownIcon);
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
                MessageBox.Show(f);
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

        private Process GetProcessFromIcon(CodeBridge.TBUTTONINFO TBUTTONINFO_Struct)
        {
            User32.GetWindowThreadProcessId(TBUTTONINFO_Struct.hwnd, out uint pid);
            return Process.GetProcessById((int)pid);
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