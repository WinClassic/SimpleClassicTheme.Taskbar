using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.UIElements.SystemTray
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
                catch
                {
                }
                value.Dispose();
            }
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

        internal SystemTrayIcon(SystemTrayNotificationData data)
        {
            NotificationData = data;
            Icon = Icon.FromHandle(data.IconHandle);

            SuspendLayout();
            SizeMode = PictureBoxSizeMode.StretchImage;
            Size = new Size(16, 16);
            ResumeLayout();

            if (Config.Default.EnableSystemTrayHover)
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

			MouseEnter += SystemTrayIcon_MouseEnter;
			MouseLeave += SystemTrayIcon_MouseLeave;
			MouseMove += SystemTrayIcon_MouseMove;
			MouseDown += SystemTrayIcon_MouseDown;
			MouseUp += SystemTrayIcon_MouseUp;
            MouseClick += SystemTrayIcon_MouseClick;
			MouseDoubleClick += SystemTrayIcon_MouseDoubleClick;
            VisibleChanged += delegate { Size = new Size(16, 16); };
		}

		private void SystemTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
            const uint WM_LBUTTONDBLCLICK = 0x0203;
            const uint WM_RBUTTONDBLCLICK = 0x0206;

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
                SendNotifyMessage(Handle, CallbackMessage, Id, (e.Button == MouseButtons.Left) ? WM_LBUTTONDBLCLICK : WM_RBUTTONDBLCLICK, NotificationData.Version);
        }

        private void SystemTrayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (!User32.IsWindow(NotificationData.WindowHandle))
                (Parent as SystemTray).TrayNotified(this, new SystemTrayNotificationEventArgs(SystemTrayNotificationType.IconDeleted, NotificationData));
        }

        private void SystemTrayIcon_MouseLeave(object sender, EventArgs e)
		{
            const uint WM_MOUSELEAVE = 0x0200;

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
                SendNotifyMessage(Handle, CallbackMessage, Id, WM_MOUSELEAVE, NotificationData.Version);
        }

		private void SystemTrayIcon_MouseEnter(object sender, EventArgs e)
		{
            const uint WM_MOUSEMOVE = 0x0200;

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
                SendNotifyMessage(Handle, CallbackMessage, Id, WM_MOUSEMOVE, NotificationData.Version);
        }

		private void SystemTrayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            const uint WM_LBUTTONUP = 0x0202;
            const uint WM_RBUTTONUP = 0x0205;

            if (IsMoving)
            {
                IsMoving = false;
                return;
            }

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
                SendNotifyMessage(Handle, CallbackMessage, Id, (e.Button == MouseButtons.Left) ? WM_LBUTTONUP : WM_RBUTTONUP, NotificationData.Version);
        }

		private void SystemTrayIcon_MouseDown(object sender, MouseEventArgs e)
        {
            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_RBUTTONDOWN = 0x0204;

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
                SendNotifyMessage(Handle, CallbackMessage, Id, (e.Button == MouseButtons.Left) ? WM_LBUTTONDOWN : WM_RBUTTONDOWN, NotificationData.Version);
        }

        private void SystemTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            const uint NIN_SELECT = 0x0400;
            const uint WM_CONTEXTMENU = 0x007B;

            if (NotificationData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid) && NotificationData.Version >= 3)
                SendNotifyMessage(Handle, CallbackMessage, Id, (e.Button == MouseButtons.Left) ? NIN_SELECT : WM_CONTEXTMENU, NotificationData.Version);
        }

        public static void SendNotifyMessage(IntPtr hWnd, uint uCallbackMessage, uint uID, uint eventMessage, int version)
		{
            uint mouse = ((uint)Cursor.Position.Y << 16) | (uint)Cursor.Position.X;
            uint wParam = version > 3 ? mouse : uID;
            uint lParamH = version > 3 ? uID : 0;
            uint lParamL = eventMessage;
            uint lParam = (lParamH << 16) | lParamL;

            _ = User32.SendNotifyMessage(hWnd, uCallbackMessage, wParam, lParam);
		}

        public override string ToString()
        {
            return Text;
        }
    }
}