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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.settingsTabs = new System.Windows.Forms.TabControl();
			this.tabTaskviewAppearance = new System.Windows.Forms.TabPage();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.comboBoxGroupingMethod = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.spaceBetweenTaskbarIcons = new System.Windows.Forms.NumericUpDown();
			this.showTaskbarOnAllDesktops = new System.Windows.Forms.CheckBox();
			this.taskbarProgramWidth = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.tabStartButtonAppearance = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.spaceBetweenQuickLaunchIcons = new System.Windows.Forms.NumericUpDown();
			this.enableQuickLaunch = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.startButton1 = new SimpleClassicThemeTaskbar.UIElements.StartButton.StartButton();
			this.button1 = new System.Windows.Forms.Button();
			this.textStartLocation = new System.Windows.Forms.TextBox();
			this.radioStartButton = new System.Windows.Forms.RadioButton();
			this.radioStartIcon = new System.Windows.Forms.RadioButton();
			this.radioStartDefault = new System.Windows.Forms.RadioButton();
			this.tabSysTrayAppearance = new System.Windows.Forms.TabPage();
			this.label10 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.spaceBetweenTrayIcons = new System.Windows.Forms.NumericUpDown();
			this.enableSysTrayColorChange = new System.Windows.Forms.CheckBox();
			this.enableSysTrayHover = new System.Windows.Forms.CheckBox();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.labelCopyrightWindows = new System.Windows.Forms.Label();
			this.labelAlpha2 = new System.Windows.Forms.Label();
			this.labelCopyrightSCT = new System.Windows.Forms.Label();
			this.labelCopyrightSCTT = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.settingsTabs.SuspendLayout();
			this.tabTaskviewAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTaskbarIcons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).BeginInit();
			this.tabStartButtonAppearance.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenQuickLaunchIcons)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tabSysTrayAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTrayIcons)).BeginInit();
			this.tabAbout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
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
			// settingsTabs
			// 
			this.settingsTabs.Controls.Add(this.tabTaskviewAppearance);
			this.settingsTabs.Controls.Add(this.tabStartButtonAppearance);
			this.settingsTabs.Controls.Add(this.tabSysTrayAppearance);
			this.settingsTabs.Controls.Add(this.tabAbout);
			resources.ApplyResources(this.settingsTabs, "settingsTabs");
			this.settingsTabs.Name = "settingsTabs";
			this.settingsTabs.SelectedIndex = 0;
			// 
			// tabTaskviewAppearance
			// 
			this.tabTaskviewAppearance.Controls.Add(this.button3);
			this.tabTaskviewAppearance.Controls.Add(this.button2);
			this.tabTaskviewAppearance.Controls.Add(this.label11);
			this.tabTaskviewAppearance.Controls.Add(this.listBox1);
			this.tabTaskviewAppearance.Controls.Add(this.comboBoxGroupingMethod);
			this.tabTaskviewAppearance.Controls.Add(this.label9);
			this.tabTaskviewAppearance.Controls.Add(this.label8);
			this.tabTaskviewAppearance.Controls.Add(this.label5);
			this.tabTaskviewAppearance.Controls.Add(this.spaceBetweenTaskbarIcons);
			this.tabTaskviewAppearance.Controls.Add(this.showTaskbarOnAllDesktops);
			this.tabTaskviewAppearance.Controls.Add(this.taskbarProgramWidth);
			this.tabTaskviewAppearance.Controls.Add(this.label1);
			resources.ApplyResources(this.tabTaskviewAppearance, "tabTaskviewAppearance");
			this.tabTaskviewAppearance.Name = "tabTaskviewAppearance";
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
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
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
			// tabStartButtonAppearance
			// 
			this.tabStartButtonAppearance.Controls.Add(this.groupBox2);
			this.tabStartButtonAppearance.Controls.Add(this.label7);
			this.tabStartButtonAppearance.Controls.Add(this.groupBox1);
			resources.ApplyResources(this.tabStartButtonAppearance, "tabStartButtonAppearance");
			this.tabStartButtonAppearance.Name = "tabStartButtonAppearance";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.spaceBetweenQuickLaunchIcons);
			this.groupBox2.Controls.Add(this.enableQuickLaunch);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label3.Name = "label3";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// spaceBetweenQuickLaunchIcons
			// 
			resources.ApplyResources(this.spaceBetweenQuickLaunchIcons, "spaceBetweenQuickLaunchIcons");
			this.spaceBetweenQuickLaunchIcons.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spaceBetweenQuickLaunchIcons.Name = "spaceBetweenQuickLaunchIcons";
			this.spaceBetweenQuickLaunchIcons.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// enableQuickLaunch
			// 
			resources.ApplyResources(this.enableQuickLaunch, "enableQuickLaunch");
			this.enableQuickLaunch.Name = "enableQuickLaunch";
			this.enableQuickLaunch.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.startButton1);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.textStartLocation);
			this.groupBox1.Controls.Add(this.radioStartButton);
			this.groupBox1.Controls.Add(this.radioStartIcon);
			this.groupBox1.Controls.Add(this.radioStartDefault);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// startButton1
			// 
			this.startButton1.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
			this.startButton1.Dummy = true;
			resources.ApplyResources(this.startButton1, "startButton1");
			this.startButton1.Name = "startButton1";
			this.startButton1.Pressed = false;
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textStartLocation
			// 
			resources.ApplyResources(this.textStartLocation, "textStartLocation");
			this.textStartLocation.Name = "textStartLocation";
			this.textStartLocation.ReadOnly = true;
			// 
			// radioStartButton
			// 
			resources.ApplyResources(this.radioStartButton, "radioStartButton");
			this.radioStartButton.Name = "radioStartButton";
			this.radioStartButton.UseVisualStyleBackColor = true;
			// 
			// radioStartIcon
			// 
			resources.ApplyResources(this.radioStartIcon, "radioStartIcon");
			this.radioStartIcon.Name = "radioStartIcon";
			this.radioStartIcon.UseVisualStyleBackColor = true;
			// 
			// radioStartDefault
			// 
			resources.ApplyResources(this.radioStartDefault, "radioStartDefault");
			this.radioStartDefault.Name = "radioStartDefault";
			this.radioStartDefault.UseVisualStyleBackColor = true;
			// 
			// tabSysTrayAppearance
			// 
			this.tabSysTrayAppearance.Controls.Add(this.label10);
			this.tabSysTrayAppearance.Controls.Add(this.label2);
			this.tabSysTrayAppearance.Controls.Add(this.spaceBetweenTrayIcons);
			this.tabSysTrayAppearance.Controls.Add(this.enableSysTrayColorChange);
			this.tabSysTrayAppearance.Controls.Add(this.enableSysTrayHover);
			resources.ApplyResources(this.tabSysTrayAppearance, "tabSysTrayAppearance");
			this.tabSysTrayAppearance.Name = "tabSysTrayAppearance";
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
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
			this.tabAbout.Controls.Add(this.pictureBox2);
			this.tabAbout.Controls.Add(this.pictureBox1);
			this.tabAbout.Controls.Add(this.pictureBox3);
			this.tabAbout.Controls.Add(this.labelCopyrightWindows);
			this.tabAbout.Controls.Add(this.labelAlpha2);
			this.tabAbout.Controls.Add(this.labelCopyrightSCT);
			this.tabAbout.Controls.Add(this.labelCopyrightSCTT);
			resources.ApplyResources(this.tabAbout, "tabAbout");
			this.tabAbout.Name = "tabAbout";
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.White;
			this.pictureBox1.Image = SimpleClassicThemeTaskbar.Properties.Resources.win98scttbanner;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox3
			// 
			this.pictureBox3.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.pictureBox3, "pictureBox3");
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.TabStop = false;
			// 
			// labelCopyrightWindows
			// 
			resources.ApplyResources(this.labelCopyrightWindows, "labelCopyrightWindows");
			this.labelCopyrightWindows.Name = "labelCopyrightWindows";
			// 
			// labelAlpha2
			// 
			resources.ApplyResources(this.labelAlpha2, "labelAlpha2");
			this.labelAlpha2.Name = "labelAlpha2";
			// 
			// labelCopyrightSCT
			// 
			resources.ApplyResources(this.labelCopyrightSCT, "labelCopyrightSCT");
			this.labelCopyrightSCT.Name = "labelCopyrightSCT";
			// 
			// labelCopyrightSCTT
			// 
			resources.ApplyResources(this.labelCopyrightSCTT, "labelCopyrightSCTT");
			this.labelCopyrightSCTT.Name = "labelCopyrightSCTT";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// Settings
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.settingsTabs);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonApply);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Settings";
			this.Load += new System.EventHandler(this.Settings_Load);
			this.settingsTabs.ResumeLayout(false);
			this.tabTaskviewAppearance.ResumeLayout(false);
			this.tabTaskviewAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTaskbarIcons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).EndInit();
			this.tabStartButtonAppearance.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenQuickLaunchIcons)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabSysTrayAppearance.ResumeLayout(false);
			this.tabSysTrayAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTrayIcons)).EndInit();
			this.tabAbout.ResumeLayout(false);
			this.tabAbout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TabControl settingsTabs;
        private System.Windows.Forms.TabPage tabTaskviewAppearance;
        private System.Windows.Forms.NumericUpDown taskbarProgramWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox showTaskbarOnAllDesktops;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown spaceBetweenTaskbarIcons;
		private System.Windows.Forms.TabPage tabAbout;
		private System.Windows.Forms.Label labelAlpha2;
		private System.Windows.Forms.Label labelCopyrightSCT;
		private System.Windows.Forms.Label labelCopyrightSCTT;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label labelCopyrightWindows;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TabPage tabStartButtonAppearance;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private UIElements.StartButton.StartButton startButton1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textStartLocation;
		private System.Windows.Forms.RadioButton radioStartButton;
		private System.Windows.Forms.RadioButton radioStartIcon;
		private System.Windows.Forms.RadioButton radioStartDefault;
		private System.Windows.Forms.TabPage tabSysTrayAppearance;
		private System.Windows.Forms.CheckBox enableQuickLaunch;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown spaceBetweenQuickLaunchIcons;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown spaceBetweenTrayIcons;
		private System.Windows.Forms.CheckBox enableSysTrayColorChange;
		private System.Windows.Forms.CheckBox enableSysTrayHover;
		private System.Windows.Forms.ComboBox comboBoxGroupingMethod;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ListBox listBox1;
	}
}