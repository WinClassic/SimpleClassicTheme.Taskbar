using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class Settings : Form
    {
        [DllImport("uxtheme.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        public Settings()
        {
            InitializeComponent();

            labelCopyrightSCT.Location = new Point(tabAbout.Width - labelCopyrightSCT.Width, labelCopyrightSCT.Location.Y);
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

            string taskbarFilter = Config.TaskbarProgramFilter;
            foreach (string filter in taskbarFilter.Split('*'))
                if (filter != "")
                    listBox1.Items.Add(filter);

            Image original = ApplicationEntryPoint.SCTCompatMode ? Properties.Resources.logo_sct_t : Properties.Resources.logo_sctt;
            Bitmap newImage = new Bitmap(original);
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
            Bitmap bitmap = new Bitmap(696, 5);
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

            SetWindowTheme(Handle, " ", " ");
        }

		private void RadioStart_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
                    startButton1.DummySettings(textStartLocation.Text, radioStartIcon.Checked, radioStartButton.Checked);
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

            Config.configChanged = true;
            Config.SaveToRegistry();
            ApplicationEntryPoint.NewTaskbars();
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
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Internet Explorer\\Quick Launch\\");
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
                    MessageBox.Show(this, "Invalid image!");
                    return;
				}
                if (radioStartIcon.Checked && (temp.Width != 16 || temp.Height != 16))
				{
                    MessageBox.Show(this, "Image is not 16x16!");
                    return;
				}
                if (radioStartButton.Checked && (temp.Height != 66))
                {
                    MessageBox.Show(this, "Image is not 66px high! (3 * 22px)");
                    return;
                }
                textStartLocation.Text = openFileDialog1.FileName;
                temp.Dispose();
                startButton1.DummySettings(textStartLocation.Text, radioStartIcon.Checked, radioStartButton.Checked);
            }
		}

		private void startButton1_SizeChanged(object sender, EventArgs e)
		{
            startButton1.Location = new Point(groupBox1.Width - 7 - startButton1.Width, startButton1.Location.Y);
		}

        private void button2_Click(object sender, EventArgs e)
        {
            SettingsAddProgramFilter dialog = new SettingsAddProgramFilter();
            dialog.ShowDialog(this);
            listBox1.Items.Add(dialog.result);
            dialog.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }
    }
}
