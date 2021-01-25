using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    public partial class SystemTray : UserControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowW(string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowExW(HWND hWndParent, HWND hWndChildAfter, string lpszClass, string lpszWindow);

        public List<SystemTrayIcon> icons = new List<SystemTrayIcon>();
        private CodeBridge d = new CodeBridge();
        public SystemTrayIcon heldDownIcon = null;
        public int heldDownOriginalX = 0;
        public int mouseOriginalX = 0;

        private void SystemTray_IconDown(object sender, MouseEventArgs e)
        {
            heldDownIcon = (SystemTrayIcon)sender;
            heldDownOriginalX = heldDownIcon.Location.X;
            mouseOriginalX = Cursor.Position.X;
        }

        static HWND GetSystemTrayHandle()
        {
            HWND hWndTray = FindWindowW("Shell_TrayWnd", null);
            if (hWndTray != HWND.Zero)
            {
                hWndTray = FindWindowExW(hWndTray, HWND.Zero, "TrayNotifyWnd", null);
                if (hWndTray != HWND.Zero)
                {
                    hWndTray = FindWindowExW(hWndTray, HWND.Zero, "SysPager", null);
                    if (hWndTray != HWND.Zero)
                    {
                        hWndTray = FindWindowExW(hWndTray, HWND.Zero, "ToolbarWindow32", null);
                        return hWndTray;
                    }
                }
            }
            return HWND.Zero;
        }

        public void ClearButtons()
        {
            Control[] controls = new Control[betterBorderPanel1.Controls.Count];
            betterBorderPanel1.Controls.CopyTo(controls, 0);
            foreach (Control d in controls)
            {
                if (d is SystemTrayIcon)
                {
                    betterBorderPanel1.Controls.Remove(d);
                    d.Dispose();
                }
            }
        }

        //TODO: Make tray icons moveable
        public void UpdateIcons()
        {
            //Get button count
            int count = d.GetTrayButtonCount(GetSystemTrayHandle());

            //Lists that will receive all button information
            List<CodeBridge.TBUTTONINFO> existingButtons = new List<CodeBridge.TBUTTONINFO>();
            List<HWND> existingHWNDs = new List<HWND>();

            //Loop through all buttons
            for (int i = 0; i < count; i++)
            {
                //Get the button info
                CodeBridge.TBUTTONINFO bInfo = new CodeBridge.TBUTTONINFO();
                d.GetTrayButton(GetSystemTrayHandle(), i, ref bInfo);

                //Save it
                if (bInfo.visible)
                {
                    existingButtons.Add(bInfo);
                    existingHWNDs.Add(bInfo.hwnd);
                }
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
                        betterBorderPanel1.Controls.Remove(existingIcon);
                        existingIcon.Dispose();
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

                //If it does we just update it, else we create it
                if (exists)
                    existingIcon.UpdateTrayIcon(info);
                else
                {
                    SystemTrayIcon trayIcon = new SystemTrayIcon(info);
                    trayIcon.MouseDown += SystemTray_IconDown;
                    newIconList.Add(trayIcon);
                }
            }

            //De-dupe all controls
            List<SystemTrayIcon> finalIconList = new List<SystemTrayIcon>();
            List<HWND> pointers = new List<HWND>();
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

            //Display all controls
            int virtualWidth = 16 + Config.SpaceBetweenTrayIcons;
            int x = 3;
            Width = 63 + (finalIconList.Count * virtualWidth);

            int startX = x;
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
                    icons.Insert(Math.Min(icons.Count, finalIconList.Count - newIndex - 1), heldDownIcon);
                    heldDownIcon = null;
                }
            }

            
            foreach (SystemTrayIcon icon in finalIconList)
            {
                //Add the control
                if (icon.Parent != betterBorderPanel1)
                {
                    icons.Add(icon);
                    betterBorderPanel1.Controls.Add(icon);
                }

                //Put the control at the correct position
                icon.Location = new Point(x, 3);
                x += virtualWidth; 
            }

            //Move to the left
            int X = Parent.Size.Width - Width - 2;
            int Y = Location.Y;

            Location = new Point(X, Y);
        }

        public SystemTray()
        {
            InitializeComponent();
        }

        public void UpdateTime()
        {
            labelTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
        }

        private void labelTime_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(labelTime, DateTime.Now.ToShortDateString());
        }
    }
}
