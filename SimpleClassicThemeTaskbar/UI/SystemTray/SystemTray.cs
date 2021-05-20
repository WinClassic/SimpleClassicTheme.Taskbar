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
    public partial class SystemTray : UserControl
    {
        private SystemTrayIcon heldDownIcon = null;
        private int heldDownOriginalX = 0;
        private int mouseOriginalX = 0;

        private Dictionary<IntPtr, SystemTrayIcon> lookupId = new();
        private Dictionary<Guid, SystemTrayIcon> lookupGuid = new();

        public SystemTray()
        {
            InitializeComponent();
            Point p = Config.Renderer.SystemTrayTimeLocation;
            labelTime.Location = new Point(Width + p.X, p.Y);
            labelTime.Font = Config.Renderer.SystemTrayTimeFont;
            labelTime.ForeColor = Config.Renderer.SystemTrayTimeColor;

            ApplicationEntryPoint.TrayNotificationService.RegisterNotificationEvent(TrayNotified);
            Disposed += delegate { ApplicationEntryPoint.TrayNotificationService.UnregisterNotificationEvent(TrayNotified); };
            ParentChanged += delegate { RepositionIcons(); };
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

        internal void TrayNotified(object sender, SystemTrayNotificationEventArgs e)
		{
            switch (e.Type)
			{
                case SystemTrayNotificationType.IconAdded:
                    AddIcon(e);
                    break;
                case SystemTrayNotificationType.IconModified:
                    ModifyIcon(e);
                    break;
                case SystemTrayNotificationType.IconDeleted:
                    RemoveIcon(e);
                    break;
                case SystemTrayNotificationType.FocusTray:
                    Focus();
                    break;
                case SystemTrayNotificationType.SetVersion:
                    // Not needed.
                    // We will always assume that we're running on version
                    // 5 as .NET 5.0 only supports Windows 7 or up
                    break;
			}
		}

        internal void AddIcon(SystemTrayNotificationEventArgs e)
		{
            // If it already exists, update it
            if (lookupGuid.ContainsKey(e.Data.Guid) ||
                lookupId.ContainsKey(e.Data.Identifier))
            {
                ModifyIcon(e);
                return;
            }

            // Create a new tray icon and add it if it isn't hidden
            SystemTrayIcon trayIcon = new(e.Data);
            trayIcon.MouseDown += SystemTray_IconDown;
            if (!e.Data.State.HasFlag(SystemTrayIconState.Hidden))
                Controls.Add(trayIcon);

            // Save icon for reference
            if (e.Data.Guid != Guid.Empty)
                lookupGuid.Add(e.Data.Guid, trayIcon);
            else
                lookupId.Add(e.Data.Identifier, trayIcon);

            // Position icons
            RepositionIcons();
        }

        internal void ModifyIcon(SystemTrayNotificationEventArgs e)
		{
            // Look for the icon
            SystemTrayIcon icon;
            if (lookupGuid.ContainsKey(e.Data.Guid))
                icon = lookupGuid[e.Data.Guid];
            else if (lookupId.ContainsKey(e.Data.Identifier))
                icon = lookupId[e.Data.Identifier];
            else
                return;

            // Call the update function
            icon.Update(e.Data);
		}

        internal void RemoveIcon(SystemTrayNotificationEventArgs e)
        {
            // Look for the icon and remove it from the reference list
            SystemTrayIcon icon;
            if (e.Data.Guid != Guid.Empty && lookupGuid.ContainsKey(e.Data.Guid))
                lookupGuid.Remove(e.Data.Guid, out icon);
            else if (lookupId.ContainsKey(e.Data.Identifier))
                lookupId.Remove(e.Data.Identifier, out icon);
            else
                return;

            // Remove the icon from the system tray, dispose it and position icons
            Controls.Remove(icon);
            icon.Dispose();
            RepositionIcons();
        }

        public void RepositionIcons()
		{
            // Get an array so we can obtain the index
            Control[] icons = new Control[Controls.Count];
            Controls.CopyTo(icons, 0);

            // Set the width and position of the tray
            Width = Config.Renderer.GetSystemTrayWidth(icons.Length);
            if (Parent != null)
                Location = new Point(Parent.Width - Width, 0);
            foreach (Control control in icons)
            {
                // Put the control at the correct position
                if (control is SystemTrayIcon icon)
                    icon.Location = Config.Renderer.GetSystemTrayIconLocation(Array.IndexOf(icons, icon));
            }
        }

        public void UpdateTime()
        {
            labelTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
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