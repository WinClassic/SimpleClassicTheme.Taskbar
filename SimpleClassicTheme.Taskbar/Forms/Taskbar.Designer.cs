
using System.Drawing;

namespace SimpleClassicTheme.Taskbar
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
			this.quickLaunch = new SimpleClassicTheme.Taskbar.UIElements.QuickLaunch.QuickLaunch();
			this.verticalDivider = new SimpleClassicTheme.Taskbar.UIElements.Misc.VerticalDivider();
			this.systemTray = SimpleClassicTheme.Taskbar.Helpers.Config.Default.EnablePassiveTray ? new UIElements.SystemTray.SystemTray() : new UIElements.OldSystemTray.SystemTray();
			this.startButton = new SimpleClassicTheme.Taskbar.UIElements.StartButton.StartButton();
			this.timerUpdateUI = new System.Windows.Forms.Timer(this.components);
			this.timerUpdateInformation = new System.Windows.Forms.Timer(this.components);
			this.timerUpdate = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// quickLaunch1
			// 
			this.quickLaunch.AllowDrop = true;
			this.quickLaunch.BackColor = System.Drawing.Color.Transparent;
			this.quickLaunch.Location = new System.Drawing.Point(59, 0);
			this.quickLaunch.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.quickLaunch.Name = "quickLaunch1";
			this.quickLaunch.Size = new System.Drawing.Size(150, 28);
			this.quickLaunch.TabIndex = 13;
			// 
			// verticalDivider3
			// 
			this.verticalDivider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.verticalDivider.BackColor = System.Drawing.SystemColors.Control;
			this.verticalDivider.Location = new System.Drawing.Point(668, 4);
			this.verticalDivider.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.verticalDivider.Name = "verticalDivider3";
			this.verticalDivider.Size = new System.Drawing.Size(2, 22);
			this.verticalDivider.TabIndex = 12;
			this.verticalDivider.Visible = false;
			this.verticalDivider.Wide = false;
			// 
			// systemTray1
			// 
			this.systemTray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.systemTray.BackColor = System.Drawing.Color.Transparent;
			this.systemTray.Location = new System.Drawing.Point(677, 0);
			this.systemTray.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.systemTray.Name = "systemTray1";
			this.systemTray.Size = new System.Drawing.Size(62, 28);
			this.systemTray.TabIndex = 11;
			// 
			// startButton1
			// 
			this.startButton.BackColor = System.Drawing.Color.Transparent;
			this.startButton.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
			this.startButton.Dummy = false;
			this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.startButton.Location = new System.Drawing.Point(0, 0);
			this.startButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.startButton.Name = "startButton1";
			this.startButton.Pressed = false;
			this.startButton.Size = new System.Drawing.Size(57, 28);
			this.startButton.TabIndex = 4;
			// 
			// timerUpdateUI
			// 
			this.timerUpdateUI.Interval = 15;
			this.timerUpdateUI.Tick += new System.EventHandler(this.TimerUpdateUI_Tick);
			// 
			// timerUpdateInformation
			// 
			this.timerUpdateInformation.Interval = 75;
			this.timerUpdateInformation.Tick += new System.EventHandler(this.TimerUpdateInformation_Tick);
			// 
			// timerUpdate
			// 
			this.timerUpdate.Tick += new System.EventHandler(this.TimerUpdate_Tick);
			// 
			// Taskbar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 28);
			this.Controls.Add(this.quickLaunch);
			this.Controls.Add(this.verticalDivider);
			this.Controls.Add(this.systemTray);
			this.Controls.Add(this.startButton);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "Taskbar";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Taskbar_FormClosing);
			this.Load += new System.EventHandler(this.Taskbar_Load);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Taskbar_MouseClick);
			this.ResumeLayout(false);

        }

        #endregion

        private UIElements.StartButton.StartButton startButton;
        private UI.Misc.SystemTrayBase systemTray;
        private UIElements.Misc.VerticalDivider verticalDivider;
        private UIElements.QuickLaunch.QuickLaunch quickLaunch;
        private System.Windows.Forms.Timer timerUpdateUI;
        private System.Windows.Forms.Timer timerUpdateInformation;
		private System.Windows.Forms.Timer timerUpdate;
	}
}

