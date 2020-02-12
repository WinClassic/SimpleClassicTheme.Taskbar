using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        private bool firstTime = true;

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

        List<SystemTrayIcon> buttons = new List<SystemTrayIcon>();
        public void UpdateButtons()
        {
            int count = d.GetTrayButtonCount(GetSystemTrayHandle());

            List<HWND> existingButtons = new List<HWND>();

            //Loop through all buttons
            for (int i = 0; i < count; i++)
            {
                //Get the button info
                TBUTTONINFO bInfo = new TBUTTONINFO();
                d.GetTrayButton(GetSystemTrayHandle(), i, ref bInfo);
                existingButtons.Add(bInfo.hwnd);

                //Check if the button already exists
                SystemTrayIcon icon = null;
                bool exists = false;
                foreach (SystemTrayIcon existingIcon in buttons)
                    if (existingIcon.Handle == bInfo.hwnd)
                    {
                        exists = true;
                        icon = existingIcon;
                    }

                //If not, create it
                if (!exists)
                    icon = new SystemTrayIcon(bInfo);

                //If it does: update info
                icon.UpdateTrayIcon(bInfo);

                //Add the button at the left side of the tray
                buttons.Insert(0, icon);
            }

            //Remove any icons that are now invalid or hidden
            List<SystemTrayIcon> newIconList = new List<SystemTrayIcon>();
            foreach (SystemTrayIcon existingIcon in buttons)
                if (existingButtons.Contains(existingIcon.Handle) && existingIcon.TBUTTONINFO_Struct.visible)
                    newIconList.Add(existingIcon);
                else
                {
                    betterBorderPanel1.Controls.Remove(existingIcon);
                    existingIcon.Dispose();
                }

            if (firstTime)
            {
                firstTime = false;
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
