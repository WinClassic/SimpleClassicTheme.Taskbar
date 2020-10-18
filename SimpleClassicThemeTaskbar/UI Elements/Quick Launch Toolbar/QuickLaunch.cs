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

namespace SimpleClassicThemeTaskbar.UIElements.QuickLaunch
{
    public partial class QuickLaunch : UserControl
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        
        [DllImport("Comctl32.dll")]
        public static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);
        
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public QuickLaunch()
        {
            InitializeComponent();
        }

        public List<QuickLaunchIcon> icons = new List<QuickLaunchIcon>();
        public bool Disabled = false;

        private void QuickLaunch_Load(object sender, EventArgs e)
        {

        }

        public void UpdateIcons()
        {
            //If it is disabled, don't show
            if (Disabled)
            {
                Width = 0;
                return;
            }

            //Get all shortcuts in the links directory
            List<string> newFiles = new List<string>();
            foreach (string file in Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Internet Explorer\\Quick Launch\\", "*.lnk"))
            {
                newFiles.Add(file);
                bool contains = false;
                foreach (QuickLaunchIcon icon in icons)
                    if (icon.FileName == file)
                        contains = true;
                if (!contains)
                {
                    QuickLaunchIcon icon = new QuickLaunchIcon();
                    icon.FileName = file;
                    icon.Click += delegate
                    {
                        Process.Start(icon.FileName);
                    };
                    SHFILEINFO shFileInfo = new SHFILEINFO();
                    IntPtr list = SHGetFileInfo(file, 0, ref shFileInfo, (uint)Marshal.SizeOf(shFileInfo), 0x4000);
                    var iconHandle = ImageList_GetIcon(list, shFileInfo.iIcon, 0);
                    Icon icn = Icon.FromHandle(iconHandle);
                    icon.Image = icn.ToBitmap();
                    icons.Add(icon);
                }
            }

            //Remove all deleted elements
            foreach (QuickLaunchIcon icon in icons.ToArray())
                if (!newFiles.Contains(icon.FileName))
                {
                    icons.Remove(icon);
                    icon.Dispose();
                }

            //If there aren't any elements just don't render
            if (icons.Count < 1)
            {
                Width = 0; 
                return;
            }

            //Display everything
            Width = 22 + ((icons.Count * 16) + (Config.SpaceBetweenQuickLaunchIcons * (icons.Count - 1)));
            int x = 12;
            foreach (QuickLaunchIcon icon in icons)
            {
                icon.Location = new Point(x, 0);
                if (!Controls.Contains(icon))
                    Controls.Add(icon);
                x += 16 + Config.SpaceBetweenQuickLaunchIcons;
            }
        }
    }
}
