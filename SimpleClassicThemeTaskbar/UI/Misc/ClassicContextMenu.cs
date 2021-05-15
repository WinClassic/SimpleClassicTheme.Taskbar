using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
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
                var systemItem = GetSystemContextMenuItem(item);
                systemContextMenu.AddItem(systemItem);
            }
            return systemContextMenu;
        }

        public void AddItem(SystemContextMenuItem item)
        {
            Items.Add(item);
            item.ID = nextItemId;

            if ((item.Flags & User32.MF_BITMAP) == User32.MF_BITMAP)
                _ = User32.AppendMenu(MenuHandle, item.Flags, nextItemId++, item.Image.GetHbitmap());
            else
                _ = User32.AppendMenu(MenuHandle, item.Flags, nextItemId++, item.Text);
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
            int id = User32.TrackPopupMenuEx(MenuHandle, User32.TPM_RETURNCMD, x, y, windowHandle, IntPtr.Zero);
            PerformAction(id);
        }

        private static SystemContextMenuItem GetSystemContextMenuItem(ToolStripItem item)
        {
            uint flags = 0;

            switch (item)
            {
                case ToolStripMenuItem menuItem:
                    if (menuItem.Checked)
                    {
                        flags |= User32.MF_CHECKED;
                    }

                    if (!menuItem.Enabled)
                    {
                        flags |= User32.MF_DISABLED;
                        flags |= User32.MF_GRAYED;
                    }

                    if (menuItem.Image != null)
                    {
                        flags |= User32.MF_BITMAP;
                        return new(menuItem.Text, menuItem.PerformClick, flags, menuItem.Image);
                    }
                    else
                    {
                        return new(menuItem.Text, menuItem.PerformClick, flags);
                    }

                case ToolStripSeparator seperator:
                    flags |= User32.MF_SEPARATOR;
                    return new SystemContextMenuItem(string.Empty, new Action(() => { }), flags);

                default:
                    throw new ArgumentException($"Unsupported ToolStripItem type {item.GetType()}", nameof(item));
            }
        }
    }

    public class SystemContextMenuItem
    {
        public uint Flags;
        public int ID;
        public Bitmap Image;
        public Action OnClick;
        public string Text;

        public SystemContextMenuItem(string text, Action onClick, uint flags)
        {
            Text = text;
            OnClick = onClick;
            Flags = flags;
        }

        public SystemContextMenuItem(string text, Action onClick, uint flags, Image image)
        {
            Flags = flags | User32.MF_BITMAP;
            Text = text;
            OnClick = onClick;
            Image = new Bitmap(image);
        }
    }
}