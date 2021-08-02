namespace SimpleClassicThemeTaskbar
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.exitItemLabel = new System.Windows.Forms.Label();
            this.exitItemComboBox = new System.Windows.Forms.ComboBox();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.tabThemes = new System.Windows.Forms.TabPage();
            this.visualStyleGroupBox = new System.Windows.Forms.GroupBox();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.colorSchemeComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.visualStyleComboBox = new System.Windows.Forms.ComboBox();
            this.manageStylesButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.themeLabel = new System.Windows.Forms.Label();
            this.tabStartButton = new System.Windows.Forms.TabPage();
            this.customIconBrowseButton = new System.Windows.Forms.Button();
            this.customIconTextBox = new System.Windows.Forms.TextBox();
            this.customButtonBrowseButton = new System.Windows.Forms.Button();
            this.customButtonTextBox = new System.Windows.Forms.TextBox();
            this.radioStartDefault = new System.Windows.Forms.RadioButton();
            this.customButtonRadioButton = new System.Windows.Forms.RadioButton();
            this.customIconRadioButton = new System.Windows.Forms.RadioButton();
            this.tabQuickLaunch = new System.Windows.Forms.TabPage();
            this.quickLaunchLinkLabel = new System.Windows.Forms.Label();
            this.enableQuickLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.tabTaskView = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addElementButton = new System.Windows.Forms.Button();
            this.taskbarFilterListBox = new System.Windows.Forms.ListBox();
            this.removeElementButton = new System.Windows.Forms.Button();
            this.comboBoxGroupingMethod = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.showTaskbarOnAllDesktops = new System.Windows.Forms.CheckBox();
            this.taskbarProgramWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSystemTray = new System.Windows.Forms.TabPage();
            this.enableSysTrayColorChange = new System.Windows.Forms.CheckBox();
            this.enableSysTrayHover = new System.Windows.Forms.CheckBox();
            this.tabTweaks = new System.Windows.Forms.TabPage();
            this.tweaksPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.copyrightTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelCopyrightSCTT = new System.Windows.Forms.Label();
            this.labelCopyrightWindows = new System.Windows.Forms.Label();
            this.labelCopyrightSCT = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.bannerPictureBox = new System.Windows.Forms.PictureBox();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.enableActiveTaskbarCheckBox = new System.Windows.Forms.CheckBox();
            this.customButtonFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.aboutLabel = new System.Windows.Forms.LinkLabel();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.customIconFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contentSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabThemes.SuspendLayout();
            this.visualStyleGroupBox.SuspendLayout();
            this.tabStartButton.SuspendLayout();
            this.tabQuickLaunch.SuspendLayout();
            this.tabTaskView.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).BeginInit();
            this.tabSystemTray.SuspendLayout();
            this.tabTweaks.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.copyrightTablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).BeginInit();
            this.tabDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentSplitContainer)).BeginInit();
            this.contentSplitContainer.Panel1.SuspendLayout();
            this.contentSplitContainer.Panel2.SuspendLayout();
            this.contentSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonApply
            // 
            resources.ApplyResources(this.buttonApply, "buttonApply");
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.ButtonApply_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabThemes);
            this.tabControl.Controls.Add(this.tabStartButton);
            this.tabControl.Controls.Add(this.tabQuickLaunch);
            this.tabControl.Controls.Add(this.tabTaskView);
            this.tabControl.Controls.Add(this.tabSystemTray);
            this.tabControl.Controls.Add(this.tabTweaks);
            this.tabControl.Controls.Add(this.tabAbout);
            this.tabControl.Controls.Add(this.tabDebug);
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.SettingsTabs_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.exitItemLabel);
            this.tabGeneral.Controls.Add(this.exitItemComboBox);
            this.tabGeneral.Controls.Add(this.languageComboBox);
            this.tabGeneral.Controls.Add(this.languageLabel);
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // exitItemLabel
            // 
            resources.ApplyResources(this.exitItemLabel, "exitItemLabel");
            this.exitItemLabel.Name = "exitItemLabel";
            // 
            // exitItemComboBox
            // 
            this.exitItemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exitItemComboBox.FormattingEnabled = true;
            this.exitItemComboBox.Items.AddRange(new object[] {
            resources.GetString("exitItemComboBox.Items"),
            resources.GetString("exitItemComboBox.Items1")});
            resources.ApplyResources(this.exitItemComboBox, "exitItemComboBox");
            this.exitItemComboBox.Name = "exitItemComboBox";
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            resources.GetString("languageComboBox.Items"),
            resources.GetString("languageComboBox.Items1")});
            resources.ApplyResources(this.languageComboBox, "languageComboBox");
            this.languageComboBox.Name = "languageComboBox";
            // 
            // languageLabel
            // 
            resources.ApplyResources(this.languageLabel, "languageLabel");
            this.languageLabel.Name = "languageLabel";
            // 
            // tabThemes
            // 
            this.tabThemes.Controls.Add(this.visualStyleGroupBox);
            this.tabThemes.Controls.Add(this.themeComboBox);
            this.tabThemes.Controls.Add(this.themeLabel);
            resources.ApplyResources(this.tabThemes, "tabThemes");
            this.tabThemes.Name = "tabThemes";
            this.tabThemes.UseVisualStyleBackColor = true;
            // 
            // visualStyleGroupBox
            // 
            this.visualStyleGroupBox.Controls.Add(this.sizeComboBox);
            this.visualStyleGroupBox.Controls.Add(this.label6);
            this.visualStyleGroupBox.Controls.Add(this.colorSchemeComboBox);
            this.visualStyleGroupBox.Controls.Add(this.label4);
            this.visualStyleGroupBox.Controls.Add(this.visualStyleComboBox);
            this.visualStyleGroupBox.Controls.Add(this.manageStylesButton);
            this.visualStyleGroupBox.Controls.Add(this.label3);
            resources.ApplyResources(this.visualStyleGroupBox, "visualStyleGroupBox");
            this.visualStyleGroupBox.Name = "visualStyleGroupBox";
            this.visualStyleGroupBox.TabStop = false;
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.sizeComboBox, "sizeComboBox");
            this.sizeComboBox.Name = "sizeComboBox";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // colorSchemeComboBox
            // 
            this.colorSchemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorSchemeComboBox.FormattingEnabled = true;
            this.colorSchemeComboBox.Items.AddRange(new object[] {
            resources.GetString("colorSchemeComboBox.Items"),
            resources.GetString("colorSchemeComboBox.Items1"),
            resources.GetString("colorSchemeComboBox.Items2")});
            resources.ApplyResources(this.colorSchemeComboBox, "colorSchemeComboBox");
            this.colorSchemeComboBox.Name = "colorSchemeComboBox";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // visualStyleComboBox
            // 
            this.visualStyleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.visualStyleComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.visualStyleComboBox, "visualStyleComboBox");
            this.visualStyleComboBox.Name = "visualStyleComboBox";
            this.visualStyleComboBox.SelectedIndexChanged += new System.EventHandler(this.visualStyleComboBox_SelectedIndexChanged);
            // 
            // manageStylesButton
            // 
            resources.ApplyResources(this.manageStylesButton, "manageStylesButton");
            this.manageStylesButton.Name = "manageStylesButton";
            this.manageStylesButton.UseVisualStyleBackColor = true;
            this.manageStylesButton.Click += new System.EventHandler(this.manageStylesButton_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Items.AddRange(new object[] {
            resources.GetString("themeComboBox.Items"),
            resources.GetString("themeComboBox.Items1"),
            resources.GetString("themeComboBox.Items2"),
            resources.GetString("themeComboBox.Items3")});
            resources.ApplyResources(this.themeComboBox, "themeComboBox");
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.themeComboBox_SelectedIndexChanged);
            // 
            // themeLabel
            // 
            resources.ApplyResources(this.themeLabel, "themeLabel");
            this.themeLabel.Name = "themeLabel";
            // 
            // tabStartButton
            // 
            this.tabStartButton.Controls.Add(this.customIconBrowseButton);
            this.tabStartButton.Controls.Add(this.customIconTextBox);
            this.tabStartButton.Controls.Add(this.customButtonBrowseButton);
            this.tabStartButton.Controls.Add(this.customButtonTextBox);
            this.tabStartButton.Controls.Add(this.radioStartDefault);
            this.tabStartButton.Controls.Add(this.customButtonRadioButton);
            this.tabStartButton.Controls.Add(this.customIconRadioButton);
            resources.ApplyResources(this.tabStartButton, "tabStartButton");
            this.tabStartButton.Name = "tabStartButton";
            this.tabStartButton.UseVisualStyleBackColor = true;
            // 
            // customIconBrowseButton
            // 
            resources.ApplyResources(this.customIconBrowseButton, "customIconBrowseButton");
            this.customIconBrowseButton.Name = "customIconBrowseButton";
            this.customIconBrowseButton.UseVisualStyleBackColor = true;
            this.customIconBrowseButton.Click += new System.EventHandler(this.CustomIconBrowseButton_Click);
            // 
            // customIconTextBox
            // 
            resources.ApplyResources(this.customIconTextBox, "customIconTextBox");
            this.customIconTextBox.Name = "customIconTextBox";
            this.customIconTextBox.ReadOnly = true;
            // 
            // customButtonBrowseButton
            // 
            resources.ApplyResources(this.customButtonBrowseButton, "customButtonBrowseButton");
            this.customButtonBrowseButton.Name = "customButtonBrowseButton";
            this.customButtonBrowseButton.UseVisualStyleBackColor = true;
            this.customButtonBrowseButton.Click += new System.EventHandler(this.CustomButtonBrowseButton_Click);
            // 
            // customButtonTextBox
            // 
            resources.ApplyResources(this.customButtonTextBox, "customButtonTextBox");
            this.customButtonTextBox.Name = "customButtonTextBox";
            this.customButtonTextBox.ReadOnly = true;
            // 
            // radioStartDefault
            // 
            resources.ApplyResources(this.radioStartDefault, "radioStartDefault");
            this.radioStartDefault.Name = "radioStartDefault";
            this.radioStartDefault.UseVisualStyleBackColor = true;
            this.radioStartDefault.CheckedChanged += new System.EventHandler(this.StartButtonRadioButton_CheckedChanged);
            // 
            // customButtonRadioButton
            // 
            resources.ApplyResources(this.customButtonRadioButton, "customButtonRadioButton");
            this.customButtonRadioButton.Name = "customButtonRadioButton";
            this.customButtonRadioButton.UseVisualStyleBackColor = true;
            this.customButtonRadioButton.CheckedChanged += new System.EventHandler(this.StartButtonRadioButton_CheckedChanged);
            // 
            // customIconRadioButton
            // 
            resources.ApplyResources(this.customIconRadioButton, "customIconRadioButton");
            this.customIconRadioButton.Name = "customIconRadioButton";
            this.customIconRadioButton.UseVisualStyleBackColor = true;
            this.customIconRadioButton.CheckedChanged += new System.EventHandler(this.StartButtonRadioButton_CheckedChanged);
            // 
            // tabQuickLaunch
            // 
            this.tabQuickLaunch.Controls.Add(this.quickLaunchLinkLabel);
            this.tabQuickLaunch.Controls.Add(this.enableQuickLaunchCheckBox);
            resources.ApplyResources(this.tabQuickLaunch, "tabQuickLaunch");
            this.tabQuickLaunch.Name = "tabQuickLaunch";
            this.tabQuickLaunch.UseVisualStyleBackColor = true;
            // 
            // quickLaunchLinkLabel
            // 
            resources.ApplyResources(this.quickLaunchLinkLabel, "quickLaunchLinkLabel");
            this.quickLaunchLinkLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.quickLaunchLinkLabel.Name = "quickLaunchLinkLabel";
            this.quickLaunchLinkLabel.Click += new System.EventHandler(this.QuickLaunchLinkLabel_Click);
            // 
            // enableQuickLaunchCheckBox
            // 
            resources.ApplyResources(this.enableQuickLaunchCheckBox, "enableQuickLaunchCheckBox");
            this.enableQuickLaunchCheckBox.Name = "enableQuickLaunchCheckBox";
            this.enableQuickLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabTaskView
            // 
            this.tabTaskView.Controls.Add(this.groupBox1);
            this.tabTaskView.Controls.Add(this.comboBoxGroupingMethod);
            this.tabTaskView.Controls.Add(this.label9);
            this.tabTaskView.Controls.Add(this.showTaskbarOnAllDesktops);
            this.tabTaskView.Controls.Add(this.taskbarProgramWidth);
            this.tabTaskView.Controls.Add(this.label1);
            resources.ApplyResources(this.tabTaskView, "tabTaskView");
            this.tabTaskView.Name = "tabTaskView";
            this.tabTaskView.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addElementButton);
            this.groupBox1.Controls.Add(this.taskbarFilterListBox);
            this.groupBox1.Controls.Add(this.removeElementButton);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // addElementButton
            // 
            resources.ApplyResources(this.addElementButton, "addElementButton");
            this.addElementButton.Name = "addElementButton";
            this.addElementButton.UseVisualStyleBackColor = true;
            this.addElementButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // taskbarFilterListBox
            // 
            this.taskbarFilterListBox.FormattingEnabled = true;
            resources.ApplyResources(this.taskbarFilterListBox, "taskbarFilterListBox");
            this.taskbarFilterListBox.Name = "taskbarFilterListBox";
            // 
            // removeElementButton
            // 
            resources.ApplyResources(this.removeElementButton, "removeElementButton");
            this.removeElementButton.Name = "removeElementButton";
            this.removeElementButton.UseVisualStyleBackColor = true;
            this.removeElementButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBoxGroupingMethod
            // 
            this.comboBoxGroupingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGroupingMethod.FormattingEnabled = true;
            this.comboBoxGroupingMethod.Items.AddRange(new object[] {
            resources.GetString("comboBoxGroupingMethod.Items"),
            resources.GetString("comboBoxGroupingMethod.Items1"),
            resources.GetString("comboBoxGroupingMethod.Items2"),
            resources.GetString("comboBoxGroupingMethod.Items3")});
            resources.ApplyResources(this.comboBoxGroupingMethod, "comboBoxGroupingMethod");
            this.comboBoxGroupingMethod.Name = "comboBoxGroupingMethod";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // showTaskbarOnAllDesktops
            // 
            resources.ApplyResources(this.showTaskbarOnAllDesktops, "showTaskbarOnAllDesktops");
            this.showTaskbarOnAllDesktops.Name = "showTaskbarOnAllDesktops";
            this.showTaskbarOnAllDesktops.UseVisualStyleBackColor = false;
            // 
            // taskbarProgramWidth
            // 
            resources.ApplyResources(this.taskbarProgramWidth, "taskbarProgramWidth");
            this.taskbarProgramWidth.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.taskbarProgramWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.taskbarProgramWidth.Name = "taskbarProgramWidth";
            this.taskbarProgramWidth.Value = new decimal(new int[] {
            143,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabSystemTray
            // 
            this.tabSystemTray.Controls.Add(this.enableSysTrayColorChange);
            this.tabSystemTray.Controls.Add(this.enableSysTrayHover);
            resources.ApplyResources(this.tabSystemTray, "tabSystemTray");
            this.tabSystemTray.Name = "tabSystemTray";
            this.tabSystemTray.UseVisualStyleBackColor = true;
            // 
            // enableSysTrayColorChange
            // 
            resources.ApplyResources(this.enableSysTrayColorChange, "enableSysTrayColorChange");
            this.enableSysTrayColorChange.Name = "enableSysTrayColorChange";
            this.enableSysTrayColorChange.UseVisualStyleBackColor = true;
            // 
            // enableSysTrayHover
            // 
            resources.ApplyResources(this.enableSysTrayHover, "enableSysTrayHover");
            this.enableSysTrayHover.Name = "enableSysTrayHover";
            // 
            // tabTweaks
            // 
            this.tabTweaks.Controls.Add(this.tweaksPropertyGrid);
            resources.ApplyResources(this.tabTweaks, "tabTweaks");
            this.tabTweaks.Name = "tabTweaks";
            this.tabTweaks.UseVisualStyleBackColor = true;
            // 
            // tweaksPropertyGrid
            // 
            resources.ApplyResources(this.tweaksPropertyGrid, "tweaksPropertyGrid");
            this.tweaksPropertyGrid.Name = "tweaksPropertyGrid";
            this.tweaksPropertyGrid.ToolbarVisible = false;
            this.tweaksPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.tweaksPropertyGrid_PropertyValueChanged);
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.copyrightTablePanel);
            this.tabAbout.Controls.Add(this.bannerPictureBox);
            resources.ApplyResources(this.tabAbout, "tabAbout");
            this.tabAbout.Name = "tabAbout";
            // 
            // copyrightTablePanel
            // 
            resources.ApplyResources(this.copyrightTablePanel, "copyrightTablePanel");
            this.copyrightTablePanel.Controls.Add(this.labelCopyrightSCTT, 0, 0);
            this.copyrightTablePanel.Controls.Add(this.labelCopyrightWindows, 0, 1);
            this.copyrightTablePanel.Controls.Add(this.labelCopyrightSCT, 1, 0);
            this.copyrightTablePanel.Controls.Add(this.label7, 1, 1);
            this.copyrightTablePanel.Name = "copyrightTablePanel";
            // 
            // labelCopyrightSCTT
            // 
            resources.ApplyResources(this.labelCopyrightSCTT, "labelCopyrightSCTT");
            this.labelCopyrightSCTT.Name = "labelCopyrightSCTT";
            // 
            // labelCopyrightWindows
            // 
            resources.ApplyResources(this.labelCopyrightWindows, "labelCopyrightWindows");
            this.labelCopyrightWindows.Name = "labelCopyrightWindows";
            // 
            // labelCopyrightSCT
            // 
            resources.ApplyResources(this.labelCopyrightSCT, "labelCopyrightSCT");
            this.labelCopyrightSCT.Name = "labelCopyrightSCT";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // bannerPictureBox
            // 
            this.bannerPictureBox.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.bannerPictureBox, "bannerPictureBox");
            this.bannerPictureBox.Image = global::SimpleClassicThemeTaskbar.Properties.Resources.win98scttbanner;
            this.bannerPictureBox.Name = "bannerPictureBox";
            this.bannerPictureBox.TabStop = false;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.enableActiveTaskbarCheckBox);
            resources.ApplyResources(this.tabDebug, "tabDebug");
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // enableActiveTaskbarCheckBox
            // 
            resources.ApplyResources(this.enableActiveTaskbarCheckBox, "enableActiveTaskbarCheckBox");
            this.enableActiveTaskbarCheckBox.Name = "enableActiveTaskbarCheckBox";
            this.enableActiveTaskbarCheckBox.UseVisualStyleBackColor = true;
            // 
            // customButtonFileDialog
            // 
            resources.ApplyResources(this.customButtonFileDialog, "customButtonFileDialog");
            this.customButtonFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.CustomButtonFileDialog_FileOk);
            // 
            // aboutLabel
            // 
            resources.ApplyResources(this.aboutLabel, "aboutLabel");
            this.aboutLabel.LinkColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.TabStop = true;
            // 
            // panelPreview
            // 
            this.panelPreview.BackColor = System.Drawing.SystemColors.Desktop;
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.Name = "panelPreview";
            // 
            // notifyIcon1
            // 
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            // 
            // customIconFileDialog
            // 
            resources.ApplyResources(this.customIconFileDialog, "customIconFileDialog");
            this.customIconFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.CustomIconFileDialog_FileOk);
            // 
            // contentSplitContainer
            // 
            this.contentSplitContainer.Cursor = System.Windows.Forms.Cursors.HSplit;
            resources.ApplyResources(this.contentSplitContainer, "contentSplitContainer");
            this.contentSplitContainer.Name = "contentSplitContainer";
            // 
            // contentSplitContainer.Panel1
            // 
            this.contentSplitContainer.Panel1.Controls.Add(this.panelPreview);
            // 
            // contentSplitContainer.Panel2
            // 
            this.contentSplitContainer.Panel2.Controls.Add(this.tabControl);
            // 
            // Settings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.contentSplitContainer);
            this.Controls.Add(this.aboutLabel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabThemes.ResumeLayout(false);
            this.visualStyleGroupBox.ResumeLayout(false);
            this.tabStartButton.ResumeLayout(false);
            this.tabStartButton.PerformLayout();
            this.tabQuickLaunch.ResumeLayout(false);
            this.tabQuickLaunch.PerformLayout();
            this.tabTaskView.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).EndInit();
            this.tabSystemTray.ResumeLayout(false);
            this.tabSystemTray.PerformLayout();
            this.tabTweaks.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.copyrightTablePanel.ResumeLayout(false);
            this.copyrightTablePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).EndInit();
            this.tabDebug.ResumeLayout(false);
            this.contentSplitContainer.Panel1.ResumeLayout(false);
            this.contentSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.contentSplitContainer)).EndInit();
            this.contentSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabTaskView;
        private System.Windows.Forms.NumericUpDown taskbarProgramWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox showTaskbarOnAllDesktops;
		private System.Windows.Forms.TabPage tabAbout;
		private System.Windows.Forms.Label labelCopyrightSCT;
		private System.Windows.Forms.Label labelCopyrightSCTT;
		private System.Windows.Forms.PictureBox bannerPictureBox;
		private System.Windows.Forms.Label labelCopyrightWindows;
		private System.Windows.Forms.OpenFileDialog customButtonFileDialog;
		private System.Windows.Forms.TabPage tabStartButton;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button customButtonBrowseButton;
		private System.Windows.Forms.TextBox customButtonTextBox;
		private System.Windows.Forms.RadioButton customButtonRadioButton;
		private System.Windows.Forms.RadioButton customIconRadioButton;
		private System.Windows.Forms.RadioButton radioStartDefault;
		private System.Windows.Forms.TabPage tabSystemTray;
		private System.Windows.Forms.CheckBox enableSysTrayColorChange;
		private System.Windows.Forms.CheckBox enableSysTrayHover;
		private System.Windows.Forms.ComboBox comboBoxGroupingMethod;
		private System.Windows.Forms.Button removeElementButton;
		private System.Windows.Forms.Button addElementButton;
		private System.Windows.Forms.ListBox taskbarFilterListBox;
        private System.Windows.Forms.TabPage tabQuickLaunch;
        private System.Windows.Forms.Label quickLaunchLinkLabel;
        private System.Windows.Forms.CheckBox enableQuickLaunchCheckBox;
        private System.Windows.Forms.LinkLabel aboutLabel;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.Button customIconBrowseButton;
        private System.Windows.Forms.TextBox customIconTextBox;
        private System.Windows.Forms.TableLayoutPanel copyrightTablePanel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.OpenFileDialog customIconFileDialog;
        private System.Windows.Forms.Label bel;
        private System.Windows.Forms.Label exitItemLabel;
        private System.Windows.Forms.ComboBox exitItemComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TabPage tabDebug;
		private System.Windows.Forms.CheckBox enableActiveTaskbarCheckBox;
        private System.Windows.Forms.TabPage tabThemes;
        private System.Windows.Forms.GroupBox visualStyleGroupBox;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox colorSchemeComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox visualStyleComboBox;
        private System.Windows.Forms.Button manageStylesButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.Label themeLabel;
        private System.Windows.Forms.TabPage tabTweaks;
        private System.Windows.Forms.PropertyGrid tweaksPropertyGrid;
        private System.Windows.Forms.SplitContainer contentSplitContainer;
    }
}