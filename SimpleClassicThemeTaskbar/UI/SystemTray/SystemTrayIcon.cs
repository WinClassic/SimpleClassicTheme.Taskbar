using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    public class SystemTrayIcon : PictureBox
    {
        internal SystemTrayNotificationData NotificationData;

        private readonly Misc.BetterToolTip toolTip;

        public uint CallbackMessage => NotificationData.CallbackMessage;
        public IntPtr Identifier => NotificationData.Identifier;
        public new IntPtr Handle => NotificationData.WindowHandle;
        public new string Text => NotificationData.ToolTip;
        public Guid Guid => NotificationData.Guid;
        public uint Id => NotificationData.Id;
        public bool IsMoving = false;

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

        internal SystemTrayIcon(SystemTrayNotificationData data)
        {
            NotificationData = data;
            Icon = Icon.FromHandle(data.IconHandle);

            SuspendLayout();
            SizeMode = PictureBoxSizeMode.StretchImage;
            Size = new Size(16, 16);
            ResumeLayout();

            if (Config.EnableSystemTrayHover)
            {
                MouseEnter += delegate { BackColor = Color.FromArgb(128, 128, 128, 128); };
                MouseLeave += delegate { BackColor = Color.Transparent; };
            }

            toolTip = new Misc.BetterToolTip()
            {
                ShowAlways = true
            };

            string tempText = Text.Replace("\r\n", "\n");
            string text = tempText.Contains("\n") ? tempText[(tempText.IndexOf('\n') + 1)..] : " ";
            string tooltip = $"{text}";
            toolTip.SetToolTip(this, tooltip);
            toolTip.ToolTipTitle = tempText.Contains("\n") ? tempText.Substring(0, tempText.IndexOf("\n")) : tempText;

            MouseClick += SystemTrayIcon_MouseClick;
            VisibleChanged += delegate { Size = new Size(16, 16); };
		}

        internal void Update(SystemTrayNotificationData data)
		{
            NotificationData.Update(data);
            if (data.Flags.HasFlag(SystemTrayNotificationFlags.ToolTipValid))
            {
                string tempText = Text.Replace("\r\n", "\n");
                string text = tempText.Contains("\n") ? tempText[(tempText.IndexOf('\n') + 1)..] : "";
                string tooltip = $"{text}";
                toolTip.SetToolTip(this, tooltip);
                toolTip.ToolTipTitle = tempText.Contains("\n") ? tempText.Substring(0, tempText.IndexOf("\n")) : tempText;
            }
            if (data.Flags.HasFlag(SystemTrayNotificationFlags.IconHandleValid))
                Icon = Icon.FromHandle(NotificationData.IconHandle);
		}

        private void SystemTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            const uint WM_CONTEXTMENU = 0x007B;
            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_LBUTTONUP = 0x0202;
            const uint WM_RBUTTONDOWN = 0x0204;
            const uint WM_RBUTTONUP = 0x0205;
            const uint NIN_SELECT = 0x0400;

            if (IsMoving)
            {
                IsMoving = false;
                return;
            }

            uint mouse = ((uint)Cursor.Position.Y << 16) | (uint)Cursor.Position.X;

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
            {
                _ = User32.SendNotifyMessage(Handle, CallbackMessage, mouse,
                    (e.Button == MouseButtons.Left ? WM_LBUTTONDOWN : WM_RBUTTONDOWN) | (Id << 16));
                _ = User32.SendNotifyMessage(Handle, CallbackMessage, mouse,
                    (e.Button == MouseButtons.Left ? WM_LBUTTONUP : WM_RBUTTONUP) | (Id << 16));
                _ = User32.SendNotifyMessage(Handle, CallbackMessage, mouse,
                    (e.Button == MouseButtons.Left ? NIN_SELECT : WM_CONTEXTMENU) | (Id << 16));
            }
            return;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}