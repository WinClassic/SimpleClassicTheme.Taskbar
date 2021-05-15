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
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.themeLabel = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.tabStartButton = new System.Windows.Forms.TabPage();
            this.customIconBrowseButton = new System.Windows.Forms.Button();
            this.customIconTextBox = new System.Windows.Forms.TextBox();
            this.customButtonBrowseButton = new System.Windows.Forms.Button();
            this.customButtonTextBox = new System.Windows.Forms.TextBox();
            this.radioStartDefault = new System.Windows.Forms.RadioButton();
            this.customButtonRadioButton = new System.Windows.Forms.RadioButton();
            this.customIconRadioButton = new System.Windows.Forms.RadioButton();
            this.tabQuickLaunch = new System.Windows.Forms.TabPage();
            this.quickLaunchOptionsPanel = new System.Windows.Forms.Panel();
            this.quickLaunchSpacingLabel = new System.Windows.Forms.Label();
            this.quickLaunchSpacingNumBox = new System.Windows.Forms.NumericUpDown();
            this.quickLaunchLinkLabel = new System.Windows.Forms.Label();
            this.enableQuickLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.tabTaskView = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.comboBoxGroupingMethod = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.spaceBetweenTaskbarIcons = new System.Windows.Forms.NumericUpDown();
            this.showTaskbarOnAllDesktops = new System.Windows.Forms.CheckBox();
            this.taskbarProgramWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSystemTray = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.spaceBetweenTrayIcons = new System.Windows.Forms.NumericUpDown();
            this.enableSysTrayColorChange = new System.Windows.Forms.CheckBox();
            this.enableSysTrayHover = new System.Windows.Forms.CheckBox();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCopyrightSCTT = new System.Windows.Forms.Label();
            this.labelCopyrightWindows = new System.Windows.Forms.Label();
            this.labelCopyrightSCT = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.bannerPictureBox = new System.Windows.Forms.PictureBox();
            this.customButtonFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.aboutLabel = new System.Windows.Forms.LinkLabel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.startButton = new SimpleClassicThemeTaskbar.UIElements.StartButton.StartButton();
            this.label4 = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.customIconFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabStartButton.SuspendLayout();
            this.tabQuickLaunch.SuspendLayout();
            this.quickLaunchOptionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quickLaunchSpacingNumBox)).BeginInit();
            this.tabTaskView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTaskbarIcons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).BeginInit();
            this.tabSystemTray.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTrayIcons)).BeginInit();
            this.tabAbout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).BeginInit();
            this.panelContent.SuspendLayout();
            this.panelPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonApply
            // 
            resources.ApplyResources(this.buttonApply, "buttonApply");
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabStartButton);
            this.tabControl.Controls.Add(this.tabQuickLaunch);
            this.tabControl.Controls.Add(this.tabTaskView);
            this.tabControl.Controls.Add(this.tabSystemTray);
            this.tabControl.Controls.Add(this.tabAbout);
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.settingsTabs_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.themeComboBox);
            this.tabGeneral.Controls.Add(this.themeLabel);
            this.tabGeneral.Controls.Add(this.languageComboBox);
            this.tabGeneral.Controls.Add(this.languageLabel);
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Items.AddRange(new object[] {
            resources.GetString("themeComboBox.Items"),
            resources.GetString("themeComboBox.Items1"),
            resources.GetString("themeComboBox.Items2")});
            resources.ApplyResources(this.themeComboBox, "themeComboBox");
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // themeLabel
            // 
            resources.ApplyResources(this.themeLabel, "themeLabel");
            this.themeLabel.Name = "themeLabel";
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
            this.customButtonBrowseButton.Click += new System.EventHandler(this.button1_Click);
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
            this.tabQuickLaunch.Controls.Add(this.quickLaunchOptionsPanel);
            this.tabQuickLaunch.Controls.Add(this.quickLaunchLinkLabel);
            this.tabQuickLaunch.Controls.Add(this.enableQuickLaunchCheckBox);
            resources.ApplyResources(this.tabQuickLaunch, "tabQuickLaunch");
            this.tabQuickLaunch.Name = "tabQuickLaunch";
            this.tabQuickLaunch.UseVisualStyleBackColor = true;
            // 
            // quickLaunchOptionsPanel
            // 
            this.quickLaunchOptionsPanel.Controls.Add(this.quickLaunchSpacingLabel);
            this.quickLaunchOptionsPanel.Controls.Add(this.quickLaunchSpacingNumBox);
            resources.ApplyResources(this.quickLaunchOptionsPanel, "quickLaunchOptionsPanel");
            this.quickLaunchOptionsPanel.Name = "quickLaunchOptionsPanel";
            // 
            // quickLaunchSpacingLabel
            // 
            resources.ApplyResources(this.quickLaunchSpacingLabel, "quickLaunchSpacingLabel");
            this.quickLaunchSpacingLabel.Name = "quickLaunchSpacingLabel";
            // 
            // quickLaunchSpacingNumBox
            // 
            resources.ApplyResources(this.quickLaunchSpacingNumBox, "quickLaunchSpacingNumBox");
            this.quickLaunchSpacingNumBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.quickLaunchSpacingNumBox.Name = "quickLaunchSpacingNumBox";
            this.quickLaunchSpacingNumBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            this.enableQuickLaunchCheckBox.CheckedChanged += new System.EventHandler(this.enableQuickLaunchCheckBox_CheckedChanged);
            // 
            // tabTaskView
            // 
            this.tabTaskView.Controls.Add(this.button3);
            this.tabTaskView.Controls.Add(this.button2);
            this.tabTaskView.Controls.Add(this.label11);
            this.tabTaskView.Controls.Add(this.listBox1);
            this.tabTaskView.Controls.Add(this.comboBoxGroupingMethod);
            this.tabTaskView.Controls.Add(this.label9);
            this.tabTaskView.Controls.Add(this.label5);
            this.tabTaskView.Controls.Add(this.spaceBetweenTaskbarIcons);
            this.tabTaskView.Controls.Add(this.showTaskbarOnAllDesktops);
            this.tabTaskView.Controls.Add(this.taskbarProgramWidth);
            this.tabTaskView.Controls.Add(this.label1);
            resources.ApplyResources(this.tabTaskView, "tabTaskView");
            this.tabTaskView.Name = "tabTaskView";
            this.tabTaskView.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.Name = "listBox1";
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // spaceBetweenTaskbarIcons
            // 
            resources.ApplyResources(this.spaceBetweenTaskbarIcons, "spaceBetweenTaskbarIcons");
            this.spaceBetweenTaskbarIcons.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spaceBetweenTaskbarIcons.Name = "spaceBetweenTaskbarIcons";
            this.spaceBetweenTaskbarIcons.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // showTaskbarOnAllDesktops
            // 
            resources.ApplyResources(this.showTaskbarOnAllDesktops, "showTaskbarOnAllDesktops");
            this.showTaskbarOnAllDesktops.Name = "showTaskbarOnAllDesktops";
            this.showTaskbarOnAllDesktops.UseVisualStyleBackColor = true;
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
            this.tabSystemTray.Controls.Add(this.label2);
            this.tabSystemTray.Controls.Add(this.spaceBetweenTrayIcons);
            this.tabSystemTray.Controls.Add(this.enableSysTrayColorChange);
            this.tabSystemTray.Controls.Add(this.enableSysTrayHover);
            resources.ApplyResources(this.tabSystemTray, "tabSystemTray");
            this.tabSystemTray.Name = "tabSystemTray";
            this.tabSystemTray.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // spaceBetweenTrayIcons
            // 
            resources.ApplyResources(this.spaceBetweenTrayIcons, "spaceBetweenTrayIcons");
            this.spaceBetweenTrayIcons.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spaceBetweenTrayIcons.Name = "spaceBetweenTrayIcons";
            this.spaceBetweenTrayIcons.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.tableLayoutPanel1);
            this.tabAbout.Controls.Add(this.bannerPictureBox);
            resources.ApplyResources(this.tabAbout, "tabAbout");
            this.tabAbout.Name = "tabAbout";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelCopyrightSCTT, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCopyrightWindows, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCopyrightSCT, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
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
            // panelContent
            // 
            this.panelContent.Controls.Add(this.tabControl);
            this.panelContent.Controls.Add(this.panelPreview);
            resources.ApplyResources(this.panelContent, "panelContent");
            this.panelContent.Name = "panelContent";
            // 
            // panelPreview
            // 
            this.panelPreview.BackColor = System.Drawing.SystemColors.Desktop;
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelPreview.Controls.Add(this.startButton);
            this.panelPreview.Controls.Add(this.label4);
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.Name = "panelPreview";
            // 
            // startButton
            // 
            this.startButton.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
            this.startButton.Dummy = true;
            resources.ApplyResources(this.startButton, "startButton");
            this.startButton.Name = "startButton";
            this.startButton.Pressed = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // notifyIcon1
            // 
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            // 
            // customIconFileDialog
            // 
            resources.ApplyResources(this.customIconFileDialog, "customIconFileDialog");
            this.customIconFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.customIconFileDialog_FileOk);
            // 
            // Settings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
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
            this.tabGeneral.PerformLayout();
            this.tabStartButton.ResumeLayout(false);
            this.tabStartButton.PerformLayout();
            this.tabQuickLaunch.ResumeLayout(false);
            this.tabQuickLaunch.PerformLayout();
            this.quickLaunchOptionsPanel.ResumeLayout(false);
            this.quickLaunchOptionsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quickLaunchSpacingNumBox)).EndInit();
            this.tabTaskView.ResumeLayout(false);
            this.tabTaskView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTaskbarIcons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).EndInit();
            this.tabSystemTray.ResumeLayout(false);
            this.tabSystemTray.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTrayIcons)).EndInit();
            this.tabAbout.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).EndInit();
            this.panelContent.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown spaceBetweenTaskbarIcons;
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown spaceBetweenTrayIcons;
		private System.Windows.Forms.CheckBox enableSysTrayColorChange;
		private System.Windows.Forms.CheckBox enableSysTrayHover;
		private System.Windows.Forms.ComboBox comboBoxGroupingMethod;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabPage tabQuickLaunch;
        private System.Windows.Forms.Label quickLaunchLinkLabel;
        private System.Windows.Forms.NumericUpDown quickLaunchSpacingNumBox;
        private System.Windows.Forms.Label quickLaunchSpacingLabel;
        private System.Windows.Forms.CheckBox enableQuickLaunchCheckBox;
        private System.Windows.Forms.LinkLabel aboutLabel;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.Label label4;
        private UIElements.StartButton.StartButton startButton;
        private System.Windows.Forms.Button customIconBrowseButton;
        private System.Windows.Forms.TextBox customIconTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.Label themeLabel;
        private System.Windows.Forms.Panel quickLaunchOptionsPanel;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.OpenFileDialog customIconFileDialog;
    }
}