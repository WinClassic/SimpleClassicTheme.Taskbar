using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using HWND = System.IntPtr;

namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    public class SystemTrayIcon : PictureBox
    {
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        //public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendMessage(HWND hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HWND hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("User32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern HWND FindWindowW(string a, string b);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(HWND hWnd, int nCmdShow);

        public HWND BaseHandle
        {
            get => base.Handle;
        }
        public CodeBridge.TBUTTONINFO TBUTTONINFO_Struct;
        public new HWND Handle;
        public new string Text;
        public uint PID;
        public Misc.BetterToolTip toolTip;
        public bool IsMoving = false;
        public Icon Icon
        {
            get => Icon.FromHandle(((Bitmap) Image).GetHicon());
            set
            {
                try
                {
                    Image = value.ToBitmap();
                }
                catch { }    
                value.Dispose();
            }
        }
  
        public SystemTrayIcon(CodeBridge.TBUTTONINFO button)
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

           
            toolTip = new Misc.BetterToolTip();
            toolTip.ShowAlways = true;

            GetWindowThreadProcessId(Handle, out PID);
            string Pname = Process.GetProcessById(checked((int)PID)).ProcessName;

            string tempText = Text.Replace("\r\n", "\n");
            string text = tempText.Contains("\n") ? tempText.Substring(tempText.IndexOf('\n') + 1) : "";
            string tooltip = $"{text}";
            //string tooltip = $"{text}\r\n\r\n{Pname} - {PID}\r\n{Handle}";
            toolTip.SetToolTip(this, tooltip);
            toolTip.ToolTipTitle = tempText.Contains("\n") ? tempText.Substring(0, tempText.IndexOf("\n")) : tempText;

            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_LBUTTONUP = 0x0202;
            const uint WM_RBUTTONDOWN = 0x0204;
            const uint WM_RBUTTONUP = 0x0205;
            MouseClick += delegate(object sender, MouseEventArgs e)
            {
                if (IsMoving)
                {
                    IsMoving = false;
                    return;
                }
                HWND Shell_TrayWnd = FindWindowW("Shell_TrayWnd", "");
                uint pidExplorer;
                GetWindowThreadProcessId(Shell_TrayWnd, out pidExplorer);
                if (Environment.OSVersion.Version.Major == 10 && PID == pidExplorer)
                {
                    //TODO: Actually fix the issue instead of using this workaround
                    Regex volume = new Regex(@"^[A-z]+\:{1}");
                    Regex battery = new Regex(@"^[0-9]+");
                    if (volume.IsMatch(Text))
                        Process.Start("sndvol");
                    else if (battery.IsMatch(Text))
                    {
                        SetWindowPos(FindWindowW("Shell_TrayWnd", ""), new IntPtr(0), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
                        FindForm().Hide();
                        ShowWindow(FindWindowW("Shell_TrayWnd", ""), 5);
                        PostMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, TBUTTONINFO_Struct.id, e.Button == MouseButtons.Left ? WM_LBUTTONDOWN : WM_RBUTTONDOWN);
                        SendMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, TBUTTONINFO_Struct.id, e.Button == MouseButtons.Left ? WM_LBUTTONUP : WM_RBUTTONUP);
                        SetWindowPos(FindWindowW("Shell_TrayWnd", ""), new IntPtr(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
                        FindForm().Hide();
                        Taskbar.waitBeforeShow = true;
                        
                    }
                    else
                    {
                        Process.Start("ms-availablenetworks:");
                    }


                    //TODO: Fix issue with explorer icons that require Shell_TrayWnd to be the active window
                    //MessageBox.Show("The clicked icon is from explorer.exe\r\nThis means that the icon currently only works when clicked from Shell_TrayWnd", "Not implemented");

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

        public unsafe void UpdateTrayIcon(CodeBridge.TBUTTONINFO button)
        {
            //Update icon
            Image oldImage = Image;
            try
            {
                //20 lines of code just to get the explorer tray icons looking right
                if (Config.EnableSystemTrayColorChange && Environment.OSVersion.Version.Major == 10 && Process.GetProcessById((int)PID).ProcessName == "explorer")
                {
                    Bitmap temp = Icon.FromHandle(button.icon).ToBitmap();
                    BitmapData data = temp.LockBits(new Rectangle(new Point(0, 0), temp.Size), System.Drawing.Imaging.ImageLockMode.ReadWrite, temp.PixelFormat);
                    IntPtr ptr = data.Scan0;
                    int bytes = Math.Abs(data.Stride) * temp.Height;
                    byte[] pixelValues = new byte[bytes];
                    Marshal.Copy(ptr, pixelValues, 0, bytes);
                    int pixWidth = temp.PixelFormat == PixelFormat.Format24bppRgb ? 3 : temp.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 4;

                    Color clr = SystemColors.ControlText;
                    float factorA = clr.A / 255F;
                    float factorR = clr.R / 255F;
                    float factorG = clr.G / 255F;
                    float factorB = clr.B / 255F;

                    for (int i = 0; i < pixelValues.Length; i += 4)
                    {
                        byte a = pixelValues[i + 3];
                        byte r = pixelValues[i + 2];
                        byte g = pixelValues[i + 1];
                        byte b = pixelValues[i + 0];
                        if (((r << 16) + (g << 8) + (b << 0)) > 0)
                        {
                            pixelValues[i + 3] = (byte)(a * factorA);
                            pixelValues[i + 2] = (byte)(r * factorR);
                            pixelValues[i + 1] = (byte)(g * factorG);
                            pixelValues[i + 0] = (byte)(b * factorB);
                        }   
                    }
                    Marshal.Copy(pixelValues, 0, ptr, bytes);
                    temp.UnlockBits(data);
                    Image = temp;
                    oldImage.Dispose();
                }
                else
                {
                    Image = Icon.FromHandle(button.icon).ToBitmap();
                    oldImage.Dispose();
                }
            }
            catch { }
            Invalidate();
            Refresh();
            Update();

            //Update tooltip
            if (TBUTTONINFO_Struct.toolTip != button.toolTip)
            {
                Text = button.toolTip;

                string tooltip = $"{Text}";
                //string Pname = Process.GetProcessById(checked((int)PID)).ProcessName;
                //string tooltip = $"{Text}\r\n\r\n{Pname} - {PID}\r\n{Handle}";
                toolTip.SetToolTip(this, tooltip);
            }

            //Save new TBUTTONINFO into current object
            TBUTTONINFO_Struct = button;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
