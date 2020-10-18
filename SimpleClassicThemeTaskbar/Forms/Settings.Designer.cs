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
			this.enableSysTrayHover = new System.Windows.Forms.CheckBox();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.settingsTabs = new System.Windows.Forms.TabControl();
			this.tabAppearance = new System.Windows.Forms.TabPage();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.spaceBetweenQuickLaunchIcons = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.spaceBetweenTaskbarIcons = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.enableQuickLaunch = new System.Windows.Forms.CheckBox();
			this.showTaskbarOnAllDesktops = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.spaceBetweenTrayIcons = new System.Windows.Forms.NumericUpDown();
			this.taskbarProgramWidth = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.labelCopyrightWindows = new System.Windows.Forms.Label();
			this.labelAlpha2 = new System.Windows.Forms.Label();
			this.labelCopyrightSCT = new System.Windows.Forms.Label();
			this.labelCopyrightSCTT = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.settingsTabs.SuspendLayout();
			this.tabAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenQuickLaunchIcons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTaskbarIcons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTrayIcons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).BeginInit();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// enableSysTrayHover
			// 
			resources.ApplyResources(this.enableSysTrayHover, "enableSysTrayHover");
			this.enableSysTrayHover.Name = "enableSysTrayHover";
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
			resources.ApplyResources(this.settingsTabs, "settingsTabs");
			this.settingsTabs.Controls.Add(this.tabAppearance);
			this.settingsTabs.Controls.Add(this.tabPage1);
			this.settingsTabs.Name = "settingsTabs";
			this.settingsTabs.SelectedIndex = 0;
			// 
			// tabAppearance
			// 
			resources.ApplyResources(this.tabAppearance, "tabAppearance");
			this.tabAppearance.Controls.Add(this.label8);
			this.tabAppearance.Controls.Add(this.label6);
			this.tabAppearance.Controls.Add(this.spaceBetweenQuickLaunchIcons);
			this.tabAppearance.Controls.Add(this.label5);
			this.tabAppearance.Controls.Add(this.spaceBetweenTaskbarIcons);
			this.tabAppearance.Controls.Add(this.label3);
			this.tabAppearance.Controls.Add(this.enableQuickLaunch);
			this.tabAppearance.Controls.Add(this.showTaskbarOnAllDesktops);
			this.tabAppearance.Controls.Add(this.label2);
			this.tabAppearance.Controls.Add(this.spaceBetweenTrayIcons);
			this.tabAppearance.Controls.Add(this.taskbarProgramWidth);
			this.tabAppearance.Controls.Add(this.label1);
			this.tabAppearance.Controls.Add(this.enableSysTrayHover);
			this.tabAppearance.Name = "tabAppearance";
			this.tabAppearance.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
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
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label3.Name = "label3";
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// enableQuickLaunch
			// 
			resources.ApplyResources(this.enableQuickLaunch, "enableQuickLaunch");
			this.enableQuickLaunch.Name = "enableQuickLaunch";
			this.enableQuickLaunch.UseVisualStyleBackColor = true;
			// 
			// showTaskbarOnAllDesktops
			// 
			resources.ApplyResources(this.showTaskbarOnAllDesktops, "showTaskbarOnAllDesktops");
			this.showTaskbarOnAllDesktops.Name = "showTaskbarOnAllDesktops";
			this.showTaskbarOnAllDesktops.UseVisualStyleBackColor = true;
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
			// tabPage1
			// 
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Controls.Add(this.labelCopyrightWindows);
			this.tabPage1.Controls.Add(this.labelAlpha2);
			this.tabPage1.Controls.Add(this.labelCopyrightSCT);
			this.tabPage1.Controls.Add(this.labelCopyrightSCTT);
			this.tabPage1.Controls.Add(this.pictureBox1);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
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
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::SimpleClassicThemeTaskbar.Properties.Resources.win98scttbanner;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// Settings
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.settingsTabs);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonApply);
			this.Name = "Settings";
			this.Load += new System.EventHandler(this.Settings_Load);
			this.settingsTabs.ResumeLayout(false);
			this.tabAppearance.ResumeLayout(false);
			this.tabAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenQuickLaunchIcons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTaskbarIcons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spaceBetweenTrayIcons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.taskbarProgramWidth)).EndInit();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox enableSysTrayHover;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TabControl settingsTabs;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.NumericUpDown spaceBetweenTrayIcons;
        private System.Windows.Forms.NumericUpDown taskbarProgramWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox showTaskbarOnAllDesktops;
        private System.Windows.Forms.CheckBox enableQuickLaunch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown spaceBetweenQuickLaunchIcons;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown spaceBetweenTaskbarIcons;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label labelAlpha2;
		private System.Windows.Forms.Label labelCopyrightSCT;
		private System.Windows.Forms.Label labelCopyrightSCTT;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label labelCopyrightWindows;
	}
}