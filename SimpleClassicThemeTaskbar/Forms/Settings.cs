using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class Settings : Form
    {
        System.ComponentModel.ComponentResourceManager Resources = new(typeof(Settings));

        public Settings()
        {
            InitializeComponent();

            labelCopyrightSCT.Location = new Point(tabAbout.Width - labelCopyrightSCT.Width, labelCopyrightSCT.Location.Y);
        }

        public void Save()
        {
            Config.EnableSystemTrayHover = enableSysTrayHover.Checked;
            Config.EnableSystemTrayColorChange = enableSysTrayColorChange.Checked;
            Config.ShowTaskbarOnAllDesktops = showTaskbarOnAllDesktops.Checked;
            Config.EnableQuickLaunch = enableQuickLaunch.Checked;
            Config.TaskbarProgramWidth = (int)taskbarProgramWidth.Value;
            Config.SpaceBetweenTrayIcons = (int)spaceBetweenTrayIcons.Value;
            Config.SpaceBetweenTaskbarIcons = (int)spaceBetweenTaskbarIcons.Value;
            Config.SpaceBetweenQuickLaunchIcons = (int)spaceBetweenQuickLaunchIcons.Value;
            Config.StartButtonImage = textStartLocation.Text;
            Config.StartButtonCustomIcon = radioStartIcon.Checked;
            Config.StartButtonCustomButton = radioStartButton.Checked;
            Config.ProgramGroupCheck = (ProgramGroupCheck)comboBoxGroupingMethod.SelectedIndex;
            string taskbarFilter = "";
            foreach (object f in listBox1.Items)
            {
                string filter = f.ToString();
                taskbarFilter += filter + "*";
            }
            Config.TaskbarProgramFilter = taskbarFilter;
            Config.Language = (string)comboBox2.SelectedItem;

            if ((string)comboBox1.SelectedItem == Resources.GetString("comboBox1.Items"))
                Config.RendererPath = "Internal/Classic";
            else if ((string)comboBox1.SelectedItem == Resources.GetString("comboBox1.Items1"))
                Config.RendererPath = "Internal/Luna";
            else
                Config.RendererPath = (string)comboBox1.SelectedItem;

            Config.ConfigChanged = true;
            Config.SaveToRegistry();
            ApplicationEntryPoint.NewTaskbars();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Compatible image files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image temp;
                try
                {
                    temp = Image.FromFile(openFileDialog1.FileName);
                }
                catch
                {
                    _ = MessageBox.Show(this, "Invalid image!");
                    return;
                }
                if (radioStartIcon.Checked && (temp.Width != 16 || temp.Height != 16))
                {
                    _ = MessageBox.Show(this, "Image is not 16x16!");
                    return;
                }
                if (radioStartButton.Checked && (temp.Height != 66))
                {
                    _ = MessageBox.Show(this, "Image is not 66px high! (3 * 22px)");
                    return;
                }
                textStartLocation.Text = openFileDialog1.FileName;
                temp.Dispose();
                startButton1.DummySettings(textStartLocation.Text, radioStartIcon.Checked, radioStartButton.Checked);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SettingsAddProgramFilter dialog = new();
            _ = dialog.ShowDialog(this);
            _ = listBox1.Items.Add(dialog.result);
            dialog.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            Save();
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

        private void label3_Click(object sender, EventArgs e)
        {
            _ = Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Internet Explorer\\Quick Launch\\");
        }

        private void RadioStart_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
                startButton1.DummySettings(textStartLocation.Text, radioStartIcon.Checked, radioStartButton.Checked);
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            labelCopyrightSCTT.Text = labelCopyrightSCTT.Text.Replace("{sctt_ver}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (ApplicationEntryPoint.SCTCompatMode)
            {
                labelCopyrightSCT.Show();
                labelCopyrightSCT.Text = labelCopyrightSCT.Text.Replace("{sct_ver}", Assembly.LoadFrom("C:\\SCT\\SCT.exe").GetName().Version.ToString());
            }

            comboBoxGroupingMethod.SelectedIndex = (int)Config.ProgramGroupCheck;

            enableSysTrayHover.Checked = Config.EnableSystemTrayHover;
            enableSysTrayColorChange.Checked = Config.EnableSystemTrayColorChange;
            showTaskbarOnAllDesktops.Checked = Config.ShowTaskbarOnAllDesktops;
            enableQuickLaunch.Checked = Config.EnableQuickLaunch;
            radioStartIcon.Checked = Config.StartButtonCustomIcon;
            radioStartButton.Checked = Config.StartButtonCustomButton;
            radioStartDefault.Checked = !Config.StartButtonCustomIcon && !Config.StartButtonCustomButton;
            textStartLocation.Text = Config.StartButtonImage;

            radioStartIcon.CheckedChanged += RadioStart_CheckedChanged;
            radioStartButton.CheckedChanged += RadioStart_CheckedChanged;
            radioStartDefault.CheckedChanged += RadioStart_CheckedChanged;

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
            comboBox2.SelectedItem = Config.Language;

            string taskbarFilter = Config.TaskbarProgramFilter;
            foreach (string filter in taskbarFilter.Split('*'))
                if (filter != "")
                    _ = listBox1.Items.Add(filter);

            // Detect renderer correctly
            switch (Config.RendererPath)
            {
                case "Internal/Classic":
                    comboBox1.SelectedItem = "Classic";
                    break;
                case "Internal/Luna":
                    comboBox1.SelectedItem = "Luna";
                    break;
                default:
                    comboBox1.Items.Add(Config.RendererPath);
                    comboBox1.SelectedItem = Config.RendererPath;
                    break;
            }

            Image original = ApplicationEntryPoint.SCTCompatMode ? Properties.Resources.logo_sct_t : Properties.Resources.logo_sctt;
            Bitmap newImage = new(original);
            //for (int x = 0; x < newImage.Width; x++)
            //    for (int y = 0; y < newImage.Height; y++)
            //    {
            //        Color c = newImage.GetPixel(x, y);
            //        if (c.R == 0x00 && c.G == 0x00 && c.B == 0x00 && c.A != 0x00)
            //            newImage.SetPixel(x, y, SystemColors.ControlText);
            //    }
            pictureBox1.Image = newImage;
            if (ApplicationEntryPoint.SCTCompatMode)
            {
                pictureBox1.Height = 106;
                pictureBox3.Height = 121;
                pictureBox2.Location = new Point(0, 124);
            }

            Color A = SystemColors.ActiveCaption;
            Color B = SystemColors.GradientActiveCaption;
            Bitmap bitmap = new(696, 5);
            for (int i = 0; i < 348; i++)
            {
                int r = A.R + ((B.R - A.R) * i / 348);
                int g = A.G + ((B.G - A.G) * i / 348);
                int b = A.B + ((B.B - A.B) * i / 348);

                for (int y = 0; y < 5; y++)
                    bitmap.SetPixel(i, y, Color.FromArgb(r, g, b));

                for (int y = 0; y < 5; y++)
                    bitmap.SetPixel(695 - i, y, Color.FromArgb(r, g, b));
            }
            if (IntPtr.Size == 8)
                pictureBox2.Location = new Point(pictureBox2.Location.X + 3, pictureBox2.Location.Y);
            pictureBox2.Image = bitmap;

            _ = UXTheme.SetWindowTheme(Handle, " ", " ");
        }

        private void startButton1_SizeChanged(object sender, EventArgs e)
        {
            startButton1.Location = new Point(groupBox1.Width - 7 - startButton1.Width, startButton1.Location.Y);
        }

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
            if ((string)comboBox1.SelectedItem == Resources.GetString("comboBox1.Items2"))
			{
                FolderBrowserDialog fbd = new();
                if (fbd.ShowDialog() == DialogResult.OK)
				{
                    string[] files = { "settings.txt", "startbutton.png", "systemtrayborder.png", "systemtraytexture.png", "taskbartexture.png", "taskbuttongroupwindowborder.png", "taskbuttonnormal.png", "taskbuttonnormalhover.png", "taskbuttonpressed.png", "taskbuttonpressedhover.png" };
                    string[] filesInDirectory = Directory.GetFiles(fbd.SelectedPath);
                    if (files.SequenceEqual(filesInDirectory))
                    {
                        comboBox1.Items.Clear();
                        comboBox1.Items.Add(Resources.GetString("comboBox1.Items"));
                        comboBox1.Items.Add(Resources.GetString("comboBox1.Items1"));
                        comboBox1.Items.Add(Resources.GetString("comboBox1.Items2"));
                        comboBox1.Items.Add(fbd.SelectedPath);
                        comboBox1.SelectedItem = fbd.SelectedPath;
                        return;
                    }
                }
                // Detect renderer correctly
                switch (Config.RendererPath)
                {
                    case "Internal/Classic":
                        comboBox1.SelectedItem = "Classic";
                        break;
                    case "Internal/Luna":
                        comboBox1.SelectedItem = "Luna";
                        break;
                    default:
                        comboBox1.Items.Add(Config.RendererPath);
                        comboBox1.SelectedItem = Config.RendererPath;
                        break;
                }
            }
        }
	}
}