using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Helpers.NativeMethods;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public class SystemContextMenu : IDisposable
    {
        public List<SystemContextMenuItem> Items = new();
        public IntPtr MenuHandle;

        public SystemContextMenu(int startId = 1)
        {
            NextItemId = startId;
            MenuHandle = User32.CreatePopupMenu();
        }

        internal int NextItemId { get; private set; }

        public static SystemContextMenu FromToolStripItems(ToolStripItemCollection items, int startId = 1)
        {
            SystemContextMenu systemContextMenu = new(startId);

            foreach (ToolStripItem item in items)
            {
                if (!item.Available)
                {
                    continue;
                }

                var systemItem = systemContextMenu.GetSystemContextMenuItem(item);
                systemContextMenu.AddItem(systemItem);
            }

            return systemContextMenu;
        }

        public void AddItem(SystemContextMenuItem item)
        {
            Items.Add(item);
            item.ID = NextItemId;

            int id;
            IntPtr pointer = IntPtr.Zero;

            if ((item.Flags & User32.MF_POPUP) == User32.MF_POPUP)
            {
                id = item.SubMenu.MenuHandle.ToInt32();
            }
            else
            {
                id = NextItemId++;
            }

            if ((item.Flags & User32.MF_BITMAP) == User32.MF_BITMAP)
            {
                pointer = item.Image.GetHbitmap();
            }
            else if ((item.Flags & User32.MF_STRING) == User32.MF_STRING)
            {
                pointer = Marshal.StringToHGlobalUni(item.Text);
            }

            _ = User32.AppendMenu(MenuHandle, item.Flags, id, pointer);
        }

        public void Dispose()
        {
            User32.DestroyMenu(MenuHandle);
        }

        public void PerformAction(int itemId)
        {
            var item = SearchForId(Items, itemId);

            if (item == null)
            {
                Logger.Instance.Log(LoggerVerbosity.Basic, "ClassicContextMenu", $"Tried to perform action for item with ID {itemId}, but couldn't find it.");
            }
            else
            {
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

            // If the user cancels the menu without making a selection, or if an error occurs, the return value is zero.
            if (id == 0)
            {
                return;
            }

            PerformAction(id);
        }

        private static SystemContextMenuItem SearchForId(IEnumerable<SystemContextMenuItem> items, int itemId)
        {
            foreach (var item in items)
            {
                if (item.ID == itemId)
                {
                    return item;
                }
                else if (item.SubMenu?.Items != null)
                {
                    var foundItems = SearchForId(item.SubMenu.Items, itemId);
                    if (foundItems != null)
                        return foundItems;
                }
            }

            return null;
        }

        private SystemContextMenuItem GetSystemContextMenuItem(ToolStripItem item)
        {
            uint flags = 0;

            switch (item)
            {
                case ToolStripMenuItem menuItem:
                    if (menuItem.Checked)
                    {
                        flags |= User32.MF_CHECKED;
                    }

                    SystemContextMenu subMenu = null;
                    if (menuItem.HasDropDownItems)
                    {
                        flags |= User32.MF_POPUP;
                        subMenu = SystemContextMenu.FromToolStripItems(menuItem.DropDownItems, NextItemId);
                        NextItemId = subMenu.NextItemId;
                    }

                    if (!menuItem.Enabled)
                    {
                        flags |= User32.MF_DISABLED;
                        flags |= User32.MF_GRAYED;
                    }

                    return new(menuItem.Text, menuItem.PerformClick, flags, menuItem.Image, subMenu);

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
        public SystemContextMenu SubMenu;
        public string Text;

        public SystemContextMenuItem(string text, Action onClick, uint flags, Image image = null, SystemContextMenu contextMenu = null)
        {
            Text = text;
            OnClick = onClick;
            Flags = flags;

            if (image != null)
            {
                Image = new Bitmap(image);
                Flags |= User32.MF_BITMAP;
            }

            if (contextMenu != null)
            {
                SubMenu = contextMenu;
                Flags |= User32.MF_POPUP;
            }
        }
    }
}