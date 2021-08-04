using Microsoft.VisualBasic;

using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;
using SimpleClassicTheme.Taskbar.UI.Misc;
using SimpleClassicThemeTaskbar;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.UIElements.SystemTray
{
    public partial class SystemTray : SystemTrayBase
    {
        private SystemTrayIcon heldDownIcon = null;
        private int heldDownOriginalX = 0;
        private int mouseOriginalX = 0;

        private Dictionary<IntPtr, SystemTrayIcon> lookupId = new();
        private Dictionary<Guid, SystemTrayIcon> lookupGuid = new();

        public SystemTray()
        {
            InitializeComponent();

            Point p = Config.Default.Renderer.SystemTrayTimeLocation;
            labelTime.Location = new Point(Width + p.X, p.Y);
            labelTime.Font = Config.Default.Renderer.SystemTrayTimeFont;
            labelTime.ForeColor = Config.Default.Renderer.SystemTrayTimeColor;

            TaskbarManager.TrayNotificationService.RegisterNotificationEvent(TrayNotified);
            Disposed += delegate { TaskbarManager.TrayNotificationService.UnregisterNotificationEvent(TrayNotified); };
            ParentChanged += delegate { RepositionIcons(); };
            ControlAdded += delegate { RepositionIcons(); };
            ControlRemoved += delegate { RepositionIcons(); };
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

            ////Display all controls
            //int virtualWidth = 16 + Config.Instance.Tweaks.SpaceBetweenTrayIcons;

            //Invalidate();
            ////betterBorderPanel1.Refresh();
            ////betterBorderPanel1.Update();

            //int startX = 3;
            //int iconWidth = 16;
            //int iconSpacing = Config.Instance.Tweaks.SpaceBetweenTrayIcons;

            // Create a new tray icon and add it if it isn't hidden
            SystemTrayIcon trayIcon = new(e.Data);
            trayIcon.MouseDown += SystemTray_IconDown;
            if (e.Data.StateMask.HasFlag(SystemTrayIconState.Hidden))
                if (!e.Data.State.HasFlag(SystemTrayIconState.Hidden))
                    Controls.Add(trayIcon);
            if (!e.Data.StateMask.HasFlag(SystemTrayIconState.Hidden))
                Controls.Add(trayIcon);

            // Save icon for reference
            if (e.Data.Guid != Guid.Empty)
                lookupGuid.Add(e.Data.Guid, trayIcon);
            else
                lookupId.Add(e.Data.Identifier, trayIcon);
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

            // If the icon is hidden, remove it (as long as it's not already removed)
            // If the icon is not hidden, add it (as long as it's not already added)
            if (e.Data.StateMask.HasFlag(SystemTrayIconState.Hidden))
                if (e.Data.State.HasFlag(SystemTrayIconState.Hidden) && Controls.Contains(icon))
                    Controls.Remove(icon);
                else if (!e.Data.State.HasFlag(SystemTrayIconState.Hidden) && !Controls.Contains(icon))
                    Controls.Add(icon);

            // Call the update function
            icon.Update(e.Data);
		}

        internal void RemoveIcon(SystemTrayNotificationEventArgs e)
        {
            controlState = "removing icon";

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
            controlState = "respositioning icons";

            // Get an array so we can obtain the index
            Control[] icons = new Control[Controls.Count];
            Controls.CopyTo(icons, 0);

            // Set the width and position of the tray
            Width = Config.Default.Renderer.GetSystemTrayWidth(icons.Length - 1);
            if (Parent != null)
                Location = new Point(Parent.Width - Width, 0);
            foreach (Control control in icons)
            {
                // Put the control at the correct position
                if (control is SystemTrayIcon icon)
                    icon.Location = Config.Default.Renderer.GetSystemTrayIconLocation(Array.IndexOf(icons, icon) - 1);
            }

            // Fix tray clock
            Point p = Config.Default.Renderer.SystemTrayTimeLocation;
            labelTime.Location = new Point(Width + p.X, p.Y);
            labelTime.Font = Config.Default.Renderer.SystemTrayTimeFont;
            labelTime.ForeColor = Config.Default.Renderer.SystemTrayTimeColor;
        }

        public override void UpdateIcons()
        {
            
        }

		public override void UpdateTime()
        {
            labelTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
        }

		public override string GetErrorString()
		{
            var sb = new StringBuilder();

            sb.AppendLine(GetBaseErrorString());
            sb.AppendLine("Unexpected error occured while " + controlState);
            sb.AppendLine("If you're seeing this message tell Leet that he should remove this scuffed message.");

            return sb.ToString();
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
            Config.Default.Renderer.DrawSystemTray(this, e.Graphics);
        }
    }
}