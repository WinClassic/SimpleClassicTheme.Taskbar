using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SimpleClassicThemeTaskbar.Cpp.CLI;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar
{
    //Temporary config class
    public static class Config
    {
        public static bool EnableSystemTrayHover = true;
        public static int SpaceBetweenTrayIcons = 1;
    }

    public class SystemTrayIcon : PictureBox
    {
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        //public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendMessage(HWND hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(HWND hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("User32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowW(string a, string b);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(HWND hWnd, int nCmdShow);

        public TBUTTONINFO TBUTTONINFO_Struct;
        public HWND Handle;
        public new string Text;
        public uint PID;
        public Icon Icon
        {
            get => Icon.FromHandle(((Bitmap) Image).GetHicon());
            set { Image = value.ToBitmap(); }
        }
  
        public SystemTrayIcon(TBUTTONINFO button)
        {
            Handle = button.hwnd;
            Text = button.toolTip;
            Icon = Icon.FromHandle(button.icon);
            SizeMode = PictureBoxSizeMode.StretchImage;
            PID = button.pid;
            TBUTTONINFO_Struct = button;

            //Icon size
            Size = new Size(16, 16);

            //Idk why, I just felt like it was needed
            if (Config.EnableSystemTrayHover)
            {
                MouseEnter += delegate { BackColor = SystemColors.ControlDark; };
                MouseLeave += delegate { BackColor = SystemColors.Control; };
            }

            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_LBUTTONUP = 0x0202;
            const uint WM_RBUTTONDOWN = 0x0204;
            const uint WM_RBUTTONUP = 0x0205;
            MouseClick += delegate(object sender, MouseEventArgs e)
            {
                HWND Shell_TrayWnd = FindWindowW("Shell_TrayWnd", "");
                uint pidExplorer;
                GetWindowThreadProcessId(Shell_TrayWnd, out pidExplorer);
                uint pid;
                GetWindowThreadProcessId(Handle, out pid);
                //pid = 0;
                if (pid == pidExplorer)
                {
                    //TODO: Fix bug with explorer icons that require Shell_TrayWnd to be the active window
                    MessageBox.Show("The clicked icon is from explorer.exe\r\nThis means that the icon currently only works when clicked from Shell_TrayWnd", "Not implemented");
                    //SetWindowPos(FindWindowW("Shell_TrayWnd", ""), new IntPtr(0), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
                    //FindForm().Hide();
                    //ShowWindow(FindWindowW("Shell_TrayWnd", ""), 5);
                    //SetWindowPos(FindWindowW("Shell_TrayWnd", ""), new IntPtr(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
                    //FindForm().Hide();
                    //Taskbar.waitBeforeShow = true;
                }
                else
                {
                    PostMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, TBUTTONINFO_Struct.id,
                        e.Button == MouseButtons.Left ? WM_LBUTTONDOWN : WM_RBUTTONDOWN);
                    SendMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, TBUTTONINFO_Struct.id,
                        e.Button == MouseButtons.Left ? WM_LBUTTONUP : WM_RBUTTONUP);
                }
            };
        }

        public void UpdateTrayIcon(TBUTTONINFO button)
        {
            //Update icon
            if (TBUTTONINFO_Struct.icon != button.icon)
                Icon = Icon.FromHandle(button.icon);

            //Update tooltip
            if (TBUTTONINFO_Struct.toolTip != button.toolTip)
                Text = button.toolTip;

            //Save new TBUTTONINFO into current object
            TBUTTONINFO_Struct = button;
        }

        public static bool operator ==(SystemTrayIcon left, SystemTrayIcon right) => left.Handle == right.Handle;
        public static bool operator !=(SystemTrayIcon left, SystemTrayIcon right) => left.Handle != right.Handle;
    }
}
