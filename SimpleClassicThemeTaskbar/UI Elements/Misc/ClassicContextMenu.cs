using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [Flags]
    public enum SystemContextMenuItemFlags : uint
    {
        Default = 0x0000,
        GrayedAndDisabled = 0x0001,
        Disabled = 0x0002,
        Bitmap = 0x0004,
        Checked = 0x0008,
        OwnerDraw = 0x0100,
        IsSeperator = 0x0800
    }

    [Flags]
    public enum SystemContextMenuTrackPopupMenuFlags : uint
    {
        Default = 0x0000,

        LeftAlign = 0x0000,
        HorizontalCenterAlign = 0x0004,
        RightAlign = 0x0008,

        TopAlign = 0x0000,
        VerticalCenterAlign = 0x0010,
        BottomAlign = 0x0020,

        TrackLeftButton = 0x0000,
        TrackRightButton = 0x0002,

        LeftToRightAnimation = 0x0400,
        RightToLeftAnimation = 0x0800,
        TopToBottomAnimation = 0x1000,
        BottomToTopAnimation = 0x2000,
        NoAnimation = 0x4000,

        ReturnResult = 0x0100
    }

    public class SystemContextMenu
    {
        public List<SystemContextMenuItem> Items = new();
        public IntPtr MenuHandle;
        private int nextItemId = 0;

        public SystemContextMenu()
        {
            MenuHandle = User32.CreatePopupMenu();
        }

        public static SystemContextMenu FromContextMenuStrip(ContextMenuStrip contextMenuStrip)
        {
            SystemContextMenu systemContextMenu = new();
            foreach (ToolStripItem item in contextMenuStrip.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    SystemContextMenuItemFlags flags = SystemContextMenuItemFlags.Default;
                    if (menuItem.Checked)
                        flags |= SystemContextMenuItemFlags.Checked;
                    if (!menuItem.Enabled)
                        flags |= SystemContextMenuItemFlags.Disabled;

                    if (menuItem.Image != null)
                    {
                        flags |= SystemContextMenuItemFlags.Bitmap;
                        SystemContextMenuItem systemContextMenuItem = new(menuItem.Text, menuItem.PerformClick, flags, menuItem.Image);
                        systemContextMenu.AddItem(systemContextMenuItem);
                    }
                    else
                    {
                        SystemContextMenuItem systemContextMenuItem = new(menuItem.Text, menuItem.PerformClick, flags);
                        systemContextMenu.AddItem(systemContextMenuItem);
                    }
                }
                else if (item is ToolStripSeparator seperator)
                    systemContextMenu.AddItem(new SystemContextMenuItem("", new Action(() => { }), SystemContextMenuItemFlags.IsSeperator));
            }
            return systemContextMenu;
        }

        public void AddItem(SystemContextMenuItem item)
        {
            Items.Add(item);
            item.ID = nextItemId;
            if (item.MenuItemFlags.HasFlag(SystemContextMenuItemFlags.Bitmap))
                _ = User32.AppendMenu(MenuHandle, item.MenuItemFlags, nextItemId++, item.Image.GetHbitmap());
            else
                _ = User32.AppendMenu(MenuHandle, item.MenuItemFlags, nextItemId++, item.Text);
        }

        public void PerformAction(int itemId)
        {
            IEnumerable<SystemContextMenuItem> items = Items.Where(a => a.ID == itemId);
            if (items.Any())
            {
                SystemContextMenuItem item = items.First();
                item.OnClick();
            }
        }

        public void RemoveItem(SystemContextMenuItem item)
        {
            if (Items.Contains(item))
            {
                _ = Items.Remove(item);
                _ = User32.DeleteMenu(MenuHandle, item.ID, 0x0000);
            }
        }

        public void Show(IntPtr windowHandle, int x, int y)
        {
            int id = User32.TrackPopupMenuEx(MenuHandle, SystemContextMenuTrackPopupMenuFlags.ReturnResult, x, y, windowHandle, IntPtr.Zero);
            PerformAction(id);
        }
    }

    public class SystemContextMenuItem
    {
        public int ID;
        public Bitmap Image;
        public SystemContextMenuItemFlags MenuItemFlags;
        public Action OnClick;
        public string Text;

        public SystemContextMenuItem(string text, Action onClick, SystemContextMenuItemFlags flags)
        {
            Text = text;
            OnClick = onClick;
            MenuItemFlags = flags;
        }

        public SystemContextMenuItem(string text, Action onClick, SystemContextMenuItemFlags flags, Image image)
        {
            if (!flags.HasFlag(SystemContextMenuItemFlags.Bitmap))
                flags |= SystemContextMenuItemFlags.Bitmap;

            Text = text;
            OnClick = onClick;
            MenuItemFlags = flags;
            Image = new Bitmap(image);
        }
    }
}