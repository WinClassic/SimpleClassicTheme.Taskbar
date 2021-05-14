
using System.Drawing;

namespace SimpleClassicThemeTaskbar
{
    partial class Taskbar
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
			this.quickLaunch1 = new SimpleClassicThemeTaskbar.UIElements.QuickLaunch.QuickLaunch();
			this.verticalDivider3 = new SimpleClassicThemeTaskbar.UIElements.Misc.VerticalDivider();
			this.systemTray1 = new SimpleClassicThemeTaskbar.UIElements.SystemTray.SystemTray();
			this.startButton1 = new SimpleClassicThemeTaskbar.UIElements.StartButton.StartButton();
			this.startButtonPanel = new System.Windows.Forms.Panel();
			this.timerUpdateUI = new System.Windows.Forms.Timer(this.components);
			this.timerUpdateInformation = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// quickLaunch1
			// 
			this.quickLaunch1.AllowDrop = true;
			this.quickLaunch1.BackColor = System.Drawing.Color.Transparent;
			this.quickLaunch1.Location = new System.Drawing.Point(59, 0);
			this.quickLaunch1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.quickLaunch1.Name = "quickLaunch1";
			this.quickLaunch1.Size = new System.Drawing.Size(150, 28);
			this.quickLaunch1.TabIndex = 13;
			// 
			// verticalDivider3
			// 
			this.verticalDivider3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.verticalDivider3.BackColor = System.Drawing.SystemColors.Control;
			this.verticalDivider3.Location = new System.Drawing.Point(668, 4);
			this.verticalDivider3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.verticalDivider3.Name = "verticalDivider3";
			this.verticalDivider3.Size = new System.Drawing.Size(2, 22);
			this.verticalDivider3.TabIndex = 12;
			this.verticalDivider3.Visible = false;
			this.verticalDivider3.Wide = false;
			// 
			// systemTray1
			// 
			this.systemTray1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.systemTray1.BackColor = System.Drawing.Color.Transparent;
			this.systemTray1.Location = new System.Drawing.Point(677, 0);
			this.systemTray1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.systemTray1.Name = "systemTray1";
			this.systemTray1.Size = new System.Drawing.Size(62, 28);
			this.systemTray1.TabIndex = 11;
			// 
			// startButton1
			// 
			this.startButton1.BackColor = System.Drawing.Color.Transparent;
			this.startButton1.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
			this.startButton1.Dummy = false;
			this.startButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.startButton1.Location = new System.Drawing.Point(0, 0);
			this.startButton1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.startButton1.Name = "startButton1";
			this.startButton1.Pressed = false;
			this.startButton1.Size = new System.Drawing.Size(57, 28);
			this.startButton1.TabIndex = 4;
			// 
			// startButtonPanel
			// 
			this.startButtonPanel.BackColor = System.Drawing.Color.Transparent;
			this.startButtonPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.startButtonPanel.Location = new System.Drawing.Point(0, 0);
			this.startButtonPanel.Name = "startButtonPanel";
			this.startButtonPanel.Size = new System.Drawing.Size(57, 28);
			this.startButtonPanel.TabIndex = 14;
			// 
			// timerUpdateUI
			// 
			this.timerUpdateUI.Interval = 15;
			this.timerUpdateUI.Tick += new System.EventHandler(this.timerUpdateUI_Tick);
			// 
			// timerUpdateInformation
			// 
			this.timerUpdateInformation.Interval = 75;
			this.timerUpdateInformation.Tick += new System.EventHandler(this.timerUpdateInformation_Tick);
			// 
			// Taskbar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 28);
			this.Controls.Add(this.quickLaunch1);
			this.Controls.Add(this.verticalDivider3);
			this.Controls.Add(this.systemTray1);
			this.Controls.Add(this.startButton1);
			this.Controls.Add(this.startButtonPanel);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "Taskbar";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Taskbar_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Taskbar_MouseClick);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel startButtonPanel;
        private UIElements.StartButton.StartButton startButton1;
        private UIElements.SystemTray.SystemTray systemTray1;
        private UIElements.Misc.VerticalDivider verticalDivider3;
        private UIElements.QuickLaunch.QuickLaunch quickLaunch1;
        private System.Windows.Forms.Timer timerUpdateUI;
        private System.Windows.Forms.Timer timerUpdateInformation;
    }
}

