using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.Helpers.NativeMethods;
using SimpleClassicThemeTaskbar.ThemeEngine.VisualStyles;

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
        private Taskbar previewTaskbar;
        private System.ComponentModel.ComponentResourceManager Resources = new(typeof(Settings));
        private VisualStyle[] visualStyles;
        private string[] visualStylePaths;

        public Settings()
        {
            InitializeComponent();

            labelCopyrightSCT.Location = new Point(tabAbout.Width - labelCopyrightSCT.Width, labelCopyrightSCT.Location.Y);

            this.SetFlatStyle(FlatStyle.System);
        }

        public void SaveSettings()
        {
            Config.Default.EnableSystemTrayHover = enableSysTrayHover.Checked;
            Config.Default.EnableSystemTrayColorChange = enableSysTrayColorChange.Checked;
            Config.Default.ShowTaskbarOnAllDesktops = showTaskbarOnAllDesktops.Checked;
            Config.Default.EnableQuickLaunch = enableQuickLaunchCheckBox.Checked;
            Config.Default.Tweaks.TaskbarProgramWidth = (int)taskbarProgramWidth.Value;
            Config.Default.StartButtonImage = customButtonTextBox.Text;
            Config.Default.StartButtonIconImage = customIconTextBox.Text;
            Config.Default.StartButtonAppearance = GetCurrentStartButtonAppearance();
            Config.Default.Language = (string)languageComboBox.SelectedItem;
            Config.Default.Tweaks.ProgramGroupCheck = (ProgramGroupCheck)comboBoxGroupingMethod.SelectedIndex;
            Config.Default.ExitMenuItemCondition = (ExitMenuItemCondition)exitItemComboBox.SelectedIndex;
            Config.Default.EnableActiveTaskbar = enableActiveTaskbarCheckBox.Checked;

            // Save taskbar filter
            string taskbarFilter = "";
            foreach (object f in taskbarFilterListBox.Items)
            {
                string filter = f.ToString();
                taskbarFilter += filter + "*";
            }
            Config.Default.TaskbarProgramFilter = taskbarFilter;

            // Save renderer path
            switch (themeComboBox.SelectedItem)
            {
                case "Classic":
                    Config.Default.RendererPath = "Internal/Classic";
                    break;

                case "Luna":
                    Config.Default.RendererPath = "Internal/Luna";
                    break;

                case "Visual Style":
                    var visualStyle = visualStyles[visualStyleComboBox.SelectedIndex];
                    Config.Default.VisualStyleColor = visualStyle.ColorNames[colorSchemeComboBox.SelectedIndex];
                    Config.Default.VisualStyleSize = visualStyle.SizeNames[sizeComboBox.SelectedIndex];
                    Config.Default.VisualStylePath = visualStylePaths[visualStyleComboBox.SelectedIndex];

                    Config.Default.RendererPath = "Internal/VisualStyle";
                    break;

                default:
                    Config.Default.RendererPath = (string)themeComboBox.SelectedItem;
                    break;
            }

            Config.Default.ConfigChanged = true;
            Config.Default.WriteToRegistry();
            ApplicationEntryPoint.NewTaskbars();

            previewTaskbar.Height = Config.Default.Renderer.TaskbarHeight;
            previewTaskbar.EnumerateWindows();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SettingsAddProgramFilter dialog = new();
            _ = dialog.ShowDialog(this);
            _ = taskbarFilterListBox.Items.Add(dialog.result);
            dialog.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (taskbarFilterListBox.SelectedIndex != -1)
            {
                taskbarFilterListBox.Items.RemoveAt(taskbarFilterListBox.SelectedIndex);
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)themeComboBox.SelectedItem == "Custom...")
            {
                FolderBrowserDialog fbd = new();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string[] files = { "settings.txt", "startbutton.png", "systemtrayborder.png", "systemtraytexture.png", "taskbartexture.png", "taskbuttongroupwindowborder.png", "taskbuttonnormal.png", "taskbuttonnormalhover.png", "taskbuttonpressed.png", "taskbuttonpressedhover.png" };
                    string[] filesInDirectory = Directory.GetFiles(fbd.SelectedPath);
                    string[] filesInDirectoryLower = new string[filesInDirectory.Length];
                    for (int i = 0; i < filesInDirectory.Length; i++)
                        filesInDirectoryLower[i] = Path.GetFileName(filesInDirectory[i]).ToLower();
                    if (files.SequenceEqual(filesInDirectoryLower))
                    {
                        themeComboBox.Items.Clear();
                        themeComboBox.Items.Add("Classic");
                        themeComboBox.Items.Add("Luna");
                        themeComboBox.Items.Add("Visual Style");
                        themeComboBox.Items.Add("Custom...");
                        themeComboBox.Items.Add(fbd.SelectedPath);
                        themeComboBox.SelectedItem = fbd.SelectedPath;
                        return;
                    }
                }

                UpdateSelectedRenderer();
            }
        }

        private void CustomButtonBrowseButton_Click(object sender, EventArgs e)
        {
            customButtonFileDialog.ShowDialog();
        }

        private void CustomButtonFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Image temp;

            try
            {
                temp = Image.FromFile(customButtonFileDialog.FileName);
            }
            catch
            {
                _ = MessageBox.Show(this, "The image you selected is invalid.", "SimpleClassicThemeTaskbar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (temp.Height != 66)
            {
                _ = MessageBox.Show(this, $"The image must be 66 pixels in height. The current height is {temp.Height} pixels. (3 * 22px)", "SimpleClassicThemeTaskbar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            customButtonTextBox.Text = customButtonFileDialog.FileName;
            temp.Dispose();

            UpdateStartButton();
        }

        private void CustomIconBrowseButton_Click(object sender, EventArgs e)
        {
            customIconFileDialog.ShowDialog();
        }

        private void CustomIconFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Image temp;
            try
            {
                temp = Image.FromFile(customIconFileDialog.FileName);
            }
            catch
            {
                _ = MessageBox.Show(this, "The image you selected is invalid.", "SimpleClassicThemeTaskbar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (temp.Width != 16 || temp.Height != 16)
            {
                _ = MessageBox.Show(this, $"The image you specified must be 16x16 pixels in size.", "SimpleClassicThemeTaskbar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            customIconTextBox.Text = customIconFileDialog.FileName;
            temp.Dispose();

            UpdateStartButton();
        }

        private StartButtonAppearance GetCurrentStartButtonAppearance()
        {
            if (radioStartDefault.Checked)
            {
                return StartButtonAppearance.Default;
            }
            else if (customIconRadioButton.Checked)
            {
                return StartButtonAppearance.CustomIcon;
            }
            else if (customButtonRadioButton.Checked)
            {
                return StartButtonAppearance.CustomButton;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void LayoutTaskbarPreview(bool showRightSide = false)
        {
            if (previewTaskbar == null)
            {
                previewTaskbar = new Taskbar(true)
                {
                    TopLevel = false,
                    Width = 1024, // 1024x768 is a pretty normal resolution
                    Dummy = true,
                };

                panelPreview.Controls.Add(previewTaskbar);

                previewTaskbar.Show();
            }

            const int weirdOffset = -4;
            previewTaskbar.Top = panelPreview.Height - previewTaskbar.Height + weirdOffset;

            if (showRightSide)
            {
                previewTaskbar.Left = panelPreview.Width - previewTaskbar.Width;
                previewTaskbar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            }
            else
            {
                previewTaskbar.Left = 0;
                previewTaskbar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            }
        }

        private void LoadSettings()
        {
            comboBoxGroupingMethod.SelectedIndex = (int)Config.Default.Tweaks.ProgramGroupCheck;
            exitItemComboBox.SelectedIndex = (int)Config.Default.ExitMenuItemCondition;

            enableActiveTaskbarCheckBox.Checked = Config.Default.EnableActiveTaskbar;
            enableSysTrayHover.Checked = Config.Default.EnableSystemTrayHover;
            enableSysTrayColorChange.Checked = Config.Default.EnableSystemTrayColorChange;
            showTaskbarOnAllDesktops.Checked = Config.Default.ShowTaskbarOnAllDesktops;
            enableQuickLaunchCheckBox.Checked = Config.Default.EnableQuickLaunch;

            // Start button
            customIconRadioButton.Checked = Config.Default.StartButtonAppearance == StartButtonAppearance.CustomIcon;
            customButtonRadioButton.Checked = Config.Default.StartButtonAppearance == StartButtonAppearance.CustomButton;
            radioStartDefault.Checked = Config.Default.StartButtonAppearance == StartButtonAppearance.Default;
            customButtonTextBox.Text = Config.Default.StartButtonImage;
            customIconTextBox.Text = Config.Default.StartButtonIconImage;

            taskbarProgramWidth.Value = Math.Min(Config.Default.Tweaks.TaskbarProgramWidth, taskbarProgramWidth.Maximum);

            taskbarProgramWidth.Maximum = Screen.PrimaryScreen.Bounds.Width;
            languageComboBox.SelectedItem = Config.Default.Language;

            if (!Config.Default.Tweaks.EnableDebugging)
            {
                tabControl.TabPages.Remove(tabDebug);
            }

            // Load taskbar filter
            string taskbarFilter = Config.Default.TaskbarProgramFilter;
            foreach (string filter in taskbarFilter.Split('*'))
                if (filter != "")
                    _ = taskbarFilterListBox.Items.Add(filter);

            UpdateSelectedRenderer();
            PopulateVisualStyles();

            tweaksPropertyGrid.SelectedObject = Config.Default.Tweaks;
        }

        private void UpdateSelectedRenderer()
        {
            // Detect renderer correctly
            switch (Config.Default.RendererPath)
            {
                case "Internal/Classic":
                    themeComboBox.SelectedItem = "Classic";
                    break;

                case "Internal/Luna":
                    themeComboBox.SelectedItem = "Luna";
                    break;

                case "Internal/VisualStyle":
                    themeComboBox.SelectedItem = "Visual Style";
                    break;

                default:
                    themeComboBox.Items.Add(Config.Default.RendererPath);
                    themeComboBox.SelectedItem = Config.Default.RendererPath;
                    break;
            }
        }

        private void QuickLaunchLinkLabel_Click(object sender, EventArgs e)
        {
            HelperFunctions.OpenQuickLaunchFolder();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            labelCopyrightSCTT.Text = labelCopyrightSCTT.Text.Replace("{sctt_ver}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (ApplicationEntryPoint.SCTCompatMode)
            {
                labelCopyrightSCT.Show();
                labelCopyrightSCT.Text = labelCopyrightSCT.Text.Replace("{sct_ver}", Assembly.LoadFrom("C:\\SCT\\SCT.exe").GetName().Version.ToString());
            }

            LoadSettings();

            if (ApplicationEntryPoint.SCTCompatMode)
            {
                bannerPictureBox.Image = Properties.Resources.logo_sct_t;
            }
            else
            {
                bannerPictureBox.Image = Properties.Resources.logo_sctt;
            }

            _ = UXTheme.SetWindowTheme(Handle, " ", " ");

            LayoutTaskbarPreview();
        }

        private void SettingsTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var aboutIndex = tabControl.TabPages.IndexOf(tabAbout);
            var systemTrayIndex = tabControl.TabPages.IndexOf(tabSystemTray);

            var aboutSelected = tabControl.SelectedIndex == aboutIndex;
            var systemTraySelected = tabControl.SelectedIndex == systemTrayIndex;

            if (!aboutSelected)
            {
                LayoutTaskbarPreview(systemTraySelected);
            }

            panelPreview.Visible = !aboutSelected;
        }

        private void StartButtonRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            customButtonTextBox.Enabled = customButtonBrowseButton.Enabled = customButtonRadioButton.Checked;
            customIconTextBox.Enabled = customIconBrowseButton.Enabled = customIconRadioButton.Checked;

            UpdateStartButton();
        }

        private void UpdateStartButton()
        {
            // var appearance = GetCurrentStartButtonAppearance();
            // startButton.DummySettings(customButtonTextBox.Text, customIconTextBox.Text, appearance);
        }

        private void manageStylesButton_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(Constants.VisualStyleDirectory)
            {
                UseShellExecute = true
            });
        }

        private void themeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            visualStyleGroupBox.Enabled = themeComboBox.SelectedItem is string label && label == "Visual Style";
        }

        private void PopulateVisualStyles()
        {
            visualStylePaths = Directory.GetFiles(Constants.VisualStyleDirectory, "*.msstyles");

            if (visualStylePaths.Length != 0)
            {

                visualStyles = visualStylePaths
                    .Select((path) => new VisualStyle(path))
                    .ToArray();

                visualStyleComboBox.Items.Clear();

                var i = 0;
                var selectedIndex = 0;
                foreach (var vs in visualStyles)
                {
                    visualStyleComboBox.Items.Add(vs.DisplayName);

                    if (Config.Default.VisualStylePath == visualStylePaths[i])
                    {
                        selectedIndex = i;
                    }

                    i++;
                }

                visualStyleComboBox.SelectedIndex = selectedIndex;

                PopulateVisualStyleCombos();
            }
        }

        private void PopulateVisualStyleCombos()
        {
            var visualStyle = visualStyles[visualStyleComboBox.SelectedIndex];

            var colorNames = visualStyle.ColorNames.Select((cn) => visualStyle.GetColorDisplay(cn).DisplayName).ToArray();
            colorSchemeComboBox.Items.Clear();
            colorSchemeComboBox.Items.AddRange(colorNames);

            var sizeNames = visualStyle.SizeNames.Select((sn) => visualStyle.GetSizeDisplay(sn).DisplayName).ToArray();
            sizeComboBox.Items.Clear();
            sizeComboBox.Items.AddRange(sizeNames);

            colorSchemeComboBox.SelectedIndex = Math.Max(0, Array.IndexOf(visualStyle.ColorNames, Config.Default.VisualStyleColor));
            sizeComboBox.SelectedIndex = Math.Max(0, Array.IndexOf(visualStyle.SizeNames, Config.Default.VisualStyleSize));
        }

        private void visualStyleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateVisualStyleCombos();
        }

        private void tweaksPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == nameof(Config.Default.Tweaks.EnableDebugging))
            {
                if (Config.Default.Tweaks.EnableDebugging)
                {
                    if (!tabControl.TabPages.Contains(tabDebug))
                        tabControl.TabPages.Add(tabDebug);
                }
                else
                {
                    if (tabControl.TabPages.Contains(tabDebug))
                        tabControl.TabPages.Remove(tabDebug);
                }
            }
        }
    }
}