using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SimpleClassicThemeTaskbar.Cpp.CLI;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    public partial class SystemTray : UserControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowW(string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowExW(HWND hWndParent, HWND hWndChildAfter, string lpszClass, string lpszWindow);

        private Interop d = new Cpp.CLI.Interop();
        public bool firstTime = true;
        
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

        //TODO: Make tray icons moveable
        public void UpdateButtons()
        {
            //Get button count
            int count = d.GetTrayButtonCount(GetSystemTrayHandle());

            //Lists that will receive all button information
            List<TBUTTONINFO> existingButtons = new List<TBUTTONINFO>();
            List<HWND> existingHWNDs = new List<HWND>();

            //Loop through all buttons
            for (int i = 0; i < count; i++)
            {
                //Get the button info
                TBUTTONINFO bInfo = new TBUTTONINFO();
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
            foreach (Control existingIco in betterBorderPanel1.Controls)
                if (existingIco is SystemTrayIcon existingIcon)
                    if (existingHWNDs.Contains(existingIcon.Handle))
                        newIconList.Add(existingIcon);
                    else
                    {
                        betterBorderPanel1.Controls.Remove(existingIcon);
                        existingIcon.Dispose();
                    }

            //Add icons that didn't display before
            foreach (TBUTTONINFO info in existingButtons)
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
                    newIconList.Add(new SystemTrayIcon(info));
            }

            //Icons are drawn from right to left so we reverse it so we can draw left to right (probably as easy)
            if (firstTime) { 
                newIconList.Reverse();
            }

            //Display all controls
            int virtualWidth = 16 + Config.SpaceBetweenTrayIcons;
            int x = 3;
            Width = 63 + (newIconList.Count * virtualWidth);
            foreach (SystemTrayIcon icon in newIconList)
            {
                //Add the control
                if (icon.Parent != betterBorderPanel1)
                    betterBorderPanel1.Controls.Add(icon);

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
            Load += delegate { UpdateButtons(); };
        }

        public void UpdateTime()
        {
            labelTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
        }
    }
}
