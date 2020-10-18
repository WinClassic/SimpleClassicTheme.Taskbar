using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            labelCopyrightSCTT.Text = labelCopyrightSCTT.Text.Replace("{sctt_ver}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (Program.SCTCompatMode)
			{
                labelCopyrightSCT.Show();
                labelCopyrightSCT.Text = labelCopyrightSCT.Text.Replace("{sct_ver}", Assembly.LoadFrom("C:\\SCT\\SCT.exe").GetName().Version.ToString());
			}

            enableSysTrayHover.Checked = Config.EnableSystemTrayHover;
            showTaskbarOnAllDesktops.Checked = Config.ShowTaskbarOnAllDesktops;
            enableQuickLaunch.Checked = Config.EnableQuickLaunch;

            if (Config.TaskbarProgramWidth <= taskbarProgramWidth.Maximum)
                taskbarProgramWidth.Value = Config.TaskbarProgramWidth;
            else
                taskbarProgramWidth.Value = taskbarProgramWidth.Maximum;

            if (Config.SpaceBetweenTrayIcons <= spaceBetweenTrayIcons.Maximum)
                spaceBetweenTrayIcons.Value = Config.SpaceBetweenTrayIcons;
            else
                spaceBetweenTrayIcons.Value = spaceBetweenTrayIcons.Maximum;

            if (Config.SpaceBetweenTaskbarIcons <= spaceBetweenTaskbarIcons.Maximum)
                spaceBetweenTaskbarIcons.Value = Config.SpaceBetweenTaskbarIcons;
            else
                spaceBetweenTaskbarIcons.Value = spaceBetweenTaskbarIcons.Maximum;

            if (Config.SpaceBetweenQuickLaunchIcons <= spaceBetweenQuickLaunchIcons.Maximum)
                spaceBetweenQuickLaunchIcons.Value = Config.SpaceBetweenQuickLaunchIcons;
            else
                spaceBetweenQuickLaunchIcons.Value = spaceBetweenQuickLaunchIcons.Maximum;

            taskbarProgramWidth.Maximum = Screen.PrimaryScreen.Bounds.Width;
        }

        public void Save()
        {
            Config.EnableSystemTrayHover = enableSysTrayHover.Checked;
            Config.ShowTaskbarOnAllDesktops = showTaskbarOnAllDesktops.Checked;
            Config.EnableQuickLaunch = enableQuickLaunch.Checked;
            Config.TaskbarProgramWidth = (int)taskbarProgramWidth.Value;
            Config.SpaceBetweenTrayIcons = (int)spaceBetweenTrayIcons.Value;
            Config.SpaceBetweenTaskbarIcons = (int)spaceBetweenTaskbarIcons.Value;
            Config.SpaceBetweenQuickLaunchIcons = (int)spaceBetweenQuickLaunchIcons.Value;

            Config.configChanged = true;
            Config.SaveToRegistry();
            Program.NewTaskbars();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Links\\");
        }
    }
}
