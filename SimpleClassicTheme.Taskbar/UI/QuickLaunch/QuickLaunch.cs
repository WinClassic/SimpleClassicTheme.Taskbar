using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SimpleClassicTheme.Taskbar.Helpers;

namespace SimpleClassicTheme.Taskbar.UIElements.QuickLaunch
{
    public partial class QuickLaunch : UserControl
    {
        public bool Disabled = false;

        public bool FirstUpdate = true;

        public QuickLaunchIcon heldDownIcon;

        public int heldDownOriginalX = 0;

        public List<QuickLaunchIcon> icons = new();

        public string LastOrder = "";

        public int mouseOriginalX = 0;

        public QuickLaunch()
        {
            InitializeComponent();
            AllowDrop = true;

            DragEnter += QuickLaunch_DragEnter;
            DragDrop += QuickLaunch_DragDrop;
        }

        public void UpdateIcons()
        {

            const int iconSize = 16 + (3 * 2);

            //If it is disabled, don't show
            if (Disabled)
            {
                Width = 0;
                return;
            }

            //Get all shortcuts in the links directory
            List<string> newFiles = new();
            foreach (string file in Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Internet Explorer\\Quick Launch\\", "*.lnk"))
            {
                newFiles.Add(file);
                bool contains = icons.Any(icon => icon.FileName == file);
                if (!contains)
                {
                    QuickLaunchIcon icon = new();
                    icon.FileName = file;
                    icon.Size = new Size(iconSize, 28);
                    icon.MouseDown += QuickLaunch_IconDown;
                    icon.Click += delegate
                    {
                        _ = Process.Start(new ProcessStartInfo(icon.FileName) { UseShellExecute = true });
                    };
                    IntPtr iconHandle = Win32Icon.GetIconFromPath(icon.FileName, Win32Icon.IconSizeEnum.SmallIcon16);
                    icon.IconHandle = iconHandle;
                    icons.Add(icon);
                }
            }

            //Remove all deleted elements
            foreach (QuickLaunchIcon icon in icons.ToArray())
                if (!newFiles.Contains(icon.FileName))
                {
                    _ = icons.Remove(icon);
                    icon.Dispose();
                }

            //Get the icons in the correct order
            if (FirstUpdate && Config.Default.QuickLaunchOrder != "")
            {
                string[] shortcutOrder = Config.Default.QuickLaunchOrder.Split('|');
                List<QuickLaunchIcon> newOrder = new();
                foreach (string shortcut in shortcutOrder)
                {
                    foreach (QuickLaunchIcon icon in icons)
                    {
                        if (icon.FileName.EndsWith(shortcut))
                        {
                            newOrder.Add(icon);
                        }
                    }
                }
                foreach (QuickLaunchIcon icon in icons)
                {
                    if (!newOrder.Contains(icon))
                        newOrder.Add(icon);
                }
                icons.Clear();
                icons.AddRange(newOrder);
                FirstUpdate = false;
            }

            //If there aren't any elements just don't render
            if (icons.Count < 1)
            {
                Width = 0;
                return;
            }

            //Display everything
            int iconSpacing = Config.Default.Tweaks.SpaceBetweenQuickLaunchIcons;
            int qlPadding = Config.Default.Renderer.QuickLaunchPadding;
            
            int emptySpace = iconSpacing * (icons.Count - 1);
            Width = (qlPadding * 2) + (icons.Count * iconSize) + emptySpace;
            int x = qlPadding;

            int startX = x;

            //See if we're moving, if so calculate new position, if we finished calculate new position and finalize position values
            if (heldDownIcon != null)
            {
                if (Math.Abs(mouseOriginalX - Cursor.Position.X) > 3)
                    heldDownIcon.IsMoving = true;

                Point p = new(heldDownOriginalX + (Cursor.Position.X - mouseOriginalX), heldDownIcon.Location.Y);
                int newIndex = (startX - p.X - ((iconSize + iconSpacing) / 2)) / (iconSize + iconSpacing) * -1;

                if (newIndex < 0)
                {
                    newIndex = 0;
                }

                _ = icons.Remove(heldDownIcon);
                icons.Insert(Math.Min(icons.Count, newIndex), heldDownIcon);

                if ((MouseButtons & MouseButtons.Left) == 0)
                {
                    heldDownIcon = null;
                }
            }

            //Save correct order
            string correctOrder = "";
            foreach (QuickLaunchIcon icon in icons)
                correctOrder += Path.GetFileName(icon.FileName) + "|";
            correctOrder = correctOrder.Trim('|');
            Config.Default.QuickLaunchOrder = correctOrder;
            if (LastOrder != correctOrder)
                Config.Default.WriteToRegistry();
            LastOrder = correctOrder;

            foreach (QuickLaunchIcon icon in icons)
            {
                icon.Location = new Point(x, 0);
                
                if (!Controls.Contains(icon))
                    Controls.Add(icon);

                x += iconSize + Config.Default.Tweaks.SpaceBetweenQuickLaunchIcons;
            }

            if (heldDownIcon != null)
                heldDownIcon.BringToFront();

            //Point verticalDividerLocation = new Point(Width - verticalDivider2.Width, verticalDivider2.Location.Y);
            //Point verticalDividerLocation = new Point(icons.Count > 0 ? icons[icons.Count - 1].Location.X + 16 + Config.Default.SpaceBetweenQuickLaunchIcons : 3, verticalDivider2.Location.Y);
            //if (verticalDivider2.Location != verticalDividerLocation)
            //    verticalDivider2.Location = verticalDividerLocation;

            Invalidate();
        }

        private void QuickLaunch_DragDrop(object sender, DragEventArgs e)
        {
            // TODO: Finish following code
            // string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // foreach (string file in files)
            // {
            // }
        }

        private void QuickLaunch_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void QuickLaunch_IconDown(object sender, MouseEventArgs e)
        {
            heldDownIcon = (QuickLaunchIcon)sender;
            heldDownOriginalX = heldDownIcon.Location.X;
            mouseOriginalX = Cursor.Position.X;
        }

        private void QuickLaunch_Load(object sender, EventArgs e)
        {
        }
    }
}