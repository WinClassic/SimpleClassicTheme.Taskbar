using Craftplacer.Windows.VisualStyles;

using SimpleClassicTheme.Taskbar.Forms;
using SimpleClassicTheme.Taskbar.Helpers;
using SimpleClassicTheme.Taskbar.Localization;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

using static SimpleClassicTheme.Taskbar.Native.Headers.UxTheme;

namespace SimpleClassicTheme.Taskbar
{
    public partial class Settings : Form
    {
        private Taskbar previewTaskbar;
        private string[] visualStylePaths;
        private VisualStyle[] visualStyles;

        public Settings()
        {
            InitializeComponent();

            labelCopyrightSCT.Location = new Point(tabAbout.Width - labelCopyrightSCT.Width, labelCopyrightSCT.Location.Y);

            this.SetFlatStyle(FlatStyle.System);
        }

        private StartButtonAppearance CurrentStartButtonAppearance
        {
            get
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
        }

        public static IEnumerable<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> list = new();
            ResourceManager rm = new(typeof(WindowsStrings));
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture))
                    {
                        continue;
                    }

                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        list.Add(culture);
                    }
                }
                catch (CultureNotFoundException)
                {
                    //NOP
                }
            }
            return list;
        }

        public void SaveSettings()
        {
            Config.Default.EnableActiveTaskbar = enableActiveTaskbarCheckBox.Checked;
            Config.Default.EnableGrouping = enableGroupingCheckBox.Checked;
            Config.Default.EnableQuickLaunch = enableQuickLaunchCheckBox.Checked;
            Config.Default.EnableSystemTrayColorChange = enableSysTrayColorChange.Checked;
            Config.Default.EnableSystemTrayHover = enableSysTrayHover.Checked;
            Config.Default.ExitMenuItemCondition = (ExitMenuItemCondition)exitItemComboBox.SelectedIndex;
            Config.Default.ShowTaskbarOnAllDesktops = showTaskbarOnAllDesktops.Checked;
            Config.Default.StartButtonAppearance = CurrentStartButtonAppearance;
            Config.Default.StartButtonIconImage = customIconTextBox.Text;
            Config.Default.StartButtonImage = customButtonTextBox.Text;
            Config.Default.GroupAppearance = (GroupAppearance)groupAppearanceComboBox.SelectedIndex;

            if (languageComboBox.SelectedItem is CultureInfo cultureInfo)
            {
                Config.Default.Language = cultureInfo.ToString();
            }

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
                    if (visualStyles == null || visualStyles.Length == 0)
                    {
                        MessageBox.Show(
                            this,
                            "You must have at least one visual style installed and selected to be able to use visual styles.",
                            "No visual styles installed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        ColorScheme colorScheme = visualStyleSelector.SelectedItem as ColorScheme;
                        Config.Default.VisualStyleColor = colorScheme.ColorName;
                        Config.Default.VisualStyleSize = colorScheme.VisualStyle.SizeNames[sizeComboBox.SelectedIndex];
                        Config.Default.VisualStylePath = colorScheme.VisualStyle.Path;
                        Config.Default.RendererPath = "Internal/VisualStyle";
                    }
                    break;

                default:
                    Config.Default.RendererPath = (string)themeComboBox.SelectedItem;
                    break;
            }

            Config.Default.ConfigChanged = true;
            Config.Default.WriteToRegistry();
            TaskbarManager.GenerateNewTaskbars();

            previewTaskbar.Height = Config.Default.Renderer.TaskbarHeight;
            // previewTaskbar.EnumerateWindows();
        }

        private static void UpdateStartButton()
        {
            // var appearance = GetCurrentStartButtonAppearance();
            // startButton.DummySettings(customButtonTextBox.Text, customIconTextBox.Text, appearance);
        }

        /// <summary>
        /// Checks the provided <paramref name="path"/> against a predefined list of files
        /// </summary>
        /// <param name="path">Path of custom theme</param>
        /// <returns>Returns <see cref="true"/>, if <paramref name="path"/> has all required files.</returns>
        private static bool VerifyCustomTheme(string path)
        {
            string[] requiredFiles = {
                "settings.txt",
                "startbutton.png",
                "systemtrayborder.png",
                "systemtraytexture.png",
                "taskbartexture.png",
                "taskbuttongroupwindowborder.png",
                "taskbuttonnormal.png",
                "taskbuttonnormalhover.png",
                "taskbuttonpressed.png",
                "taskbuttonpressedhover.png"
            };

            IEnumerable<string> files = Directory.GetFiles(path).Select(fp => Path.GetFileName(fp).ToLower());
            return requiredFiles.SequenceEqual(files);
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

        private void EnableGroupingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            groupAppearanceComboBox.Enabled = enableGroupingCheckBox.Checked;
        }

        private IEnumerable<VisualStyle> GetVisualStyles()
        {
            List<VisualStyle> visualStyles = new(visualStylePaths.Length);

            foreach (var path in visualStylePaths)
            {
                try
                {
                    visualStyles.Add(new(path));
                }
                catch
                {
                    MessageBox.Show(
                        this,
                        $"{Path.GetFileNameWithoutExtension(path)} has failed to load. Verify that it is a valid Windows XP visual style.",
                        string.Empty,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }

            return visualStyles;
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

            const int borderSize = 1;
            const int borderHorizontal = borderSize * 2;
            int visibleWidth = panelPreview.Width - borderHorizontal;
            int offsetX;

            previewTaskbar.Top = panelPreview.Height - previewTaskbar.Height - borderSize;

            if (showRightSide)
            {
                offsetX = panelPreview.Width - previewTaskbar.Width - borderSize;
                previewTaskbar.Left = offsetX;
                previewTaskbar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            }
            else
            {
                offsetX = 0;
                previewTaskbar.Left = 1;
                previewTaskbar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            }

            Rectangle regionRect = new(offsetX *= -1, 0, visibleWidth, previewTaskbar.Height);
            previewTaskbar.Region = new(regionRect);
        }

        private void LoadSettings()
        {
            customButtonRadioButton.Checked = Config.Default.StartButtonAppearance == StartButtonAppearance.CustomButton;
            customButtonTextBox.Text = Config.Default.StartButtonImage;
            customIconRadioButton.Checked = Config.Default.StartButtonAppearance == StartButtonAppearance.CustomIcon;
            customIconTextBox.Text = Config.Default.StartButtonIconImage;
            enableActiveTaskbarCheckBox.Checked = Config.Default.EnableActiveTaskbar;
            enableGroupingCheckBox.Checked = Config.Default.EnableGrouping;
            enableQuickLaunchCheckBox.Checked = Config.Default.EnableQuickLaunch;
            enableSysTrayColorChange.Checked = Config.Default.EnableSystemTrayColorChange;
            enableSysTrayHover.Checked = Config.Default.EnableSystemTrayHover;
            exitItemComboBox.SelectedIndex = (int)Config.Default.ExitMenuItemCondition;
            radioStartDefault.Checked = Config.Default.StartButtonAppearance == StartButtonAppearance.Default;
            showTaskbarOnAllDesktops.Checked = Config.Default.ShowTaskbarOnAllDesktops;
            groupAppearanceComboBox.SelectedIndex = (int)Config.Default.GroupAppearance;

            if (!Config.Default.Tweaks.EnableDebugging)
            {
                tabControl.TabPages.Remove(tabDebug);
            }

            UpdateSelectedRenderer();
            PopulateVisualStyles();

            PopulateAvailableLanguages();
            languageComboBox.SelectedItem = new CultureInfo(Config.Default.Language);

            tweaksPropertyGrid.SelectedObject = Config.Default.Tweaks;
        }

        private void ManageHiddenElementsButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new ProgramFilterSettings())
            {
                dialog.ShowDialog();
            }
        }

        private void ManageStylesButton_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(Constants.VisualStyleDirectory)
            {
                UseShellExecute = true
            });
        }

        private void PanelPreview_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new(Point.Empty, panelPreview.Size);
            ControlPaint.DrawBorder3D(e.Graphics, rect, Border3DStyle.SunkenOuter);
        }

        private void PanelPreview_Resize(object sender, EventArgs e)
        {
            panelPreview.Invalidate();
        }

        private void PopulateAvailableLanguages()
        {
            var defaultCulture = new CultureInfo("en-US");
            var cultures = GetAvailableCultures().Prepend(defaultCulture);
            languageComboBox.Items.Clear();
            languageComboBox.Items.AddRange(cultures.ToArray());
        }

        private void PopulateVisualStyles()
        {
            visualStylePaths = Directory.GetFiles(Constants.VisualStyleDirectory, "*.msstyles");

            if (visualStylePaths.Length != 0)
            {
                visualStyles = GetVisualStyles().ToArray();

                visualStyleSelector.Items.Clear();

                foreach (VisualStyle vs in visualStyles)
                {
                    foreach (string colorName in vs.ColorNames)
                    {
                        var colorScheme = vs.GetColorScheme(colorName, vs.SizeNames.First());
                        var newI = visualStyleSelector.Items.Add(colorScheme);

                        if (colorScheme.ColorName == Config.Default.VisualStyleColor && colorScheme.VisualStyle.Path == Config.Default.VisualStylePath)
                        {
                            visualStyleSelector.SelectedIndex = newI;
                        }
                    }
                }
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

            _ = SetWindowTheme(Handle, " ", " ");

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

            contentSplitContainer.Panel1Collapsed = aboutSelected;
            panelPreview.Visible = !aboutSelected;
        }

        private void ShowDescriptionButton_CheckedChanged(object sender, EventArgs e)
        {
            tweaksPropertyGrid.HelpVisible = showDescriptionButton.Checked;
        }

        private void StartButtonRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            customButtonTextBox.Enabled = customButtonBrowseButton.Enabled = customButtonRadioButton.Checked;
            customIconTextBox.Enabled = customIconBrowseButton.Enabled = customIconRadioButton.Checked;

            UpdateStartButton();
        }

        private void ThemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (themeComboBox.SelectedItem is not string label)
            {
                return;
            }

            visualStyleTab.Enabled = label == "Visual Style";

            if (label == "Custom...")
            {
                DialogResult dialogResult = customThemeFolderBrowserDialog.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    UpdateSelectedRenderer();
                    return;
                }

                string path = customThemeFolderBrowserDialog.SelectedPath;
                if (VerifyCustomTheme(path))
                {
                    // Commented these lines because it makes no sense
                    // to always reset the list

                    // themeComboBox.Items.Clear();
                    // themeComboBox.Items.Add("Classic");
                    // themeComboBox.Items.Add("Visual Style");
                    // themeComboBox.Items.Add("Custom...");

                    if (themeComboBox.Items.Contains(path))
                    {
                        themeComboBox.Items.Add(path);
                        themeComboBox.SelectedItem = path;
                    }

                    return;
                }

                UpdateSelectedRenderer();
            }
        }

        private void TweakResetButton_Click(object sender, EventArgs e)
        {
            var obj = tweaksPropertyGrid.SelectedObject;
            tweaksPropertyGrid.SelectedGridItem.PropertyDescriptor.ResetValue(obj);
        }

        private void TweaksPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
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

        private void TweaksPropertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            var descriptor = tweaksPropertyGrid.SelectedGridItem.PropertyDescriptor;
            tweakResetButton.Enabled = descriptor?.CanResetValue(null) == true;
        }

        private void UpdateSelectedRenderer()
        {
            // Detect renderer correctly
            switch (Config.Default.RendererPath)
            {
                case "Internal/Classic":
                    themeComboBox.SelectedItem = "Classic";
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

        private void VisualStyleSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            sizeComboBox.Items.Clear();

            try
            {
                ColorScheme colorScheme = visualStyleSelector.SelectedItem as ColorScheme;
                VisualStyle visualStyle = colorScheme.VisualStyle;
                var sizeNames = visualStyle.SizeNames.Select(sn => visualStyle.GetSizeDisplay(sn).DisplayName).ToArray();

                sizeComboBox.Items.AddRange(sizeNames);
                sizeComboBox.SelectedIndex = 0;
                sizeComboBox.Enabled = true;
            }
            catch
            {
                sizeComboBox.Enabled = false;

                MessageBox.Show(
                    this,
                    "Failed to get color schemes/sizes, please verify that this visual style is valid.",
                    string.Empty,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }
    }
}