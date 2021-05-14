using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    public class SystemTrayIcon : PictureBox
    {
        public new IntPtr Handle;

        public bool IsMoving = false;

        public uint PID;

        public CodeBridge.TBUTTONINFO TBUTTONINFO_Struct;

        public new string Text;

        public Misc.BetterToolTip toolTip;

        public SystemTrayIcon(CodeBridge.TBUTTONINFO button)
        {
            Handle = button.hwnd;
            Text = button.toolTip;
            SizeMode = PictureBoxSizeMode.StretchImage;
            PID = button.pid;
            TBUTTONINFO_Struct = button;

            //Icon size
            Size = new Size(16, 16);

            //Idk why, I just felt like it was needed
            if (Config.EnableSystemTrayHover)
            {
                MouseEnter += delegate { BackColor = Color.FromArgb(128, 128, 128, 128); };
                MouseLeave += delegate { BackColor = Color.Transparent; };
            }

            toolTip = new Misc.BetterToolTip
            {
                ShowAlways = true
            };

            _ = User32.GetWindowThreadProcessId(Handle, out PID);
            string Pname = Process.GetProcessById(checked((int)PID)).ProcessName;

            string tempText = Text.Replace("\r\n", "\n");
            string text = tempText.Contains("\n") ? tempText[(tempText.IndexOf('\n') + 1)..] : "";
            string tooltip = $"{text}";
            //string tooltip = $"{text}\r\n\r\n{Pname} - {PID}\r\n{Handle}";
            toolTip.SetToolTip(this, tooltip);
            toolTip.ToolTipTitle = tempText.Contains("\n") ? tempText.Substring(0, tempText.IndexOf("\n")) : tempText;

            MouseClick += SystemTrayIcon_MouseClick;
            UpdateTrayIcon(TBUTTONINFO_Struct, true);
        }

        public IntPtr BaseHandle
        {
            get => base.Handle;
        }

        public Icon Icon
        {
            get => Icon.FromHandle(((Bitmap)Image).GetHicon());
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

        public override string ToString()
        {
            return Text;
        }

        public unsafe void UpdateTrayIcon(CodeBridge.TBUTTONINFO button, bool firstTime = false)
        {
            if (TBUTTONINFO_Struct.icon != button.icon || firstTime)
            {
                //Update icon
                Image oldImage = Image;
                try
                {
                    string pName = Process.GetProcessById((int)PID).ProcessName.ToLower();
                    //20 lines of code just to get the explorer tray icons looking right
                    if (Config.EnableSystemTrayColorChange &&
                        Environment.OSVersion.Version.Major == 10 &&
                        (pName == "explorer" ||
                        pName == "onedrive") &&
                        (button.callbackMessage == 1120 || //Volume icon callback
                        button.callbackMessage == 49488 || //Network icon callback
                        button.callbackMessage == 32868))  //OneDrive icon callback
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
                        if (oldImage != null) oldImage.Dispose();
                    }
                    else
                    {
                        Image = Icon.FromHandle(button.icon).ToBitmap();
                        if (oldImage != null) oldImage.Dispose();
                    }
                }
                catch { }
                Invalidate();
                //Refresh();
                //Update();
            }

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

        private void SystemTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            const uint WM_CONTEXTMENU = 0x007B;
            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_LBUTTONUP = 0x0202;
            const uint WM_RBUTTONDOWN = 0x0204;
            const uint WM_RBUTTONUP = 0x0205;
            const uint NIN_SELECT = 0x0400;

            if (e.Button == MouseButtons.Right)
            {
                _ = MessageBox.Show($"{Process.GetProcessById(checked((int)PID)).ProcessName}\n" + TBUTTONINFO_Struct.callbackMessage.ToString());
                return;
            }
            if (IsMoving)
            {
                IsMoving = false;
                return;
            }

            IntPtr Shell_TrayWnd = User32.FindWindowW("Shell_TrayWnd", "");
            _ = User32.GetWindowThreadProcessId(Shell_TrayWnd, out uint pidExplorer);

            uint mouse = ((uint)Cursor.Position.Y << 16) | (uint)Cursor.Position.X;

            _ = User32.SendNotifyMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, mouse,
                (e.Button == MouseButtons.Left ? WM_LBUTTONDOWN : WM_RBUTTONDOWN) | (TBUTTONINFO_Struct.id << 16));
            _ = User32.SendNotifyMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, mouse,
                (e.Button == MouseButtons.Left ? WM_LBUTTONUP : WM_RBUTTONUP) | (TBUTTONINFO_Struct.id << 16));
            _ = User32.SendNotifyMessage(TBUTTONINFO_Struct.hwnd, TBUTTONINFO_Struct.callbackMessage, mouse,
                (e.Button == MouseButtons.Left ? NIN_SELECT : WM_CONTEXTMENU) | (TBUTTONINFO_Struct.id << 16));

            return;
        }
    }
}