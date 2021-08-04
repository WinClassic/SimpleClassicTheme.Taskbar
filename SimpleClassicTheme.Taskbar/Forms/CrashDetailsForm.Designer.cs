
namespace SimpleClassicTheme.Taskbar.Forms
{
    partial class CrashDetailsForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.exceptionTabPage = new System.Windows.Forms.TabPage();
            this.exceptionTextBox = new System.Windows.Forms.TextBox();
            this.logTabPage = new System.Windows.Forms.TabPage();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.exceptionTabPage.SuspendLayout();
            this.logTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.exceptionTabPage);
            this.tabControl.Controls.Add(this.logTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(8, 8);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(478, 259);
            this.tabControl.TabIndex = 0;
            // 
            // exceptionTabPage
            // 
            this.exceptionTabPage.Controls.Add(this.exceptionTextBox);
            this.exceptionTabPage.Location = new System.Drawing.Point(4, 22);
            this.exceptionTabPage.Name = "exceptionTabPage";
            this.exceptionTabPage.Padding = new System.Windows.Forms.Padding(8);
            this.exceptionTabPage.Size = new System.Drawing.Size(470, 233);
            this.exceptionTabPage.TabIndex = 0;
            this.exceptionTabPage.Text = "Exception";
            this.exceptionTabPage.UseVisualStyleBackColor = true;
            // 
            // exceptionTextBox
            // 
            this.exceptionTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.exceptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exceptionTextBox.Location = new System.Drawing.Point(8, 8);
            this.exceptionTextBox.Multiline = true;
            this.exceptionTextBox.Name = "exceptionTextBox";
            this.exceptionTextBox.ReadOnly = true;
            this.exceptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.exceptionTextBox.Size = new System.Drawing.Size(454, 217);
            this.exceptionTextBox.TabIndex = 1;
            this.exceptionTextBox.WordWrap = false;
            // 
            // logTabPage
            // 
            this.logTabPage.Controls.Add(this.logTextBox);
            this.logTabPage.Location = new System.Drawing.Point(4, 22);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Padding = new System.Windows.Forms.Padding(8);
            this.logTabPage.Size = new System.Drawing.Size(470, 233);
            this.logTabPage.TabIndex = 1;
            this.logTabPage.Text = "Log file";
            this.logTabPage.UseVisualStyleBackColor = true;
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Location = new System.Drawing.Point(8, 8);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTextBox.Size = new System.Drawing.Size(454, 217);
            this.logTextBox.TabIndex = 0;
            this.logTextBox.WordWrap = false;
            // 
            // CrashDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 275);
            this.Controls.Add(this.tabControl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashDetailsForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Crash details";
            this.Load += new System.EventHandler(this.CrashDetailsForm_Load);
            this.tabControl.ResumeLayout(false);
            this.exceptionTabPage.ResumeLayout(false);
            this.exceptionTabPage.PerformLayout();
            this.logTabPage.ResumeLayout(false);
            this.logTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage exceptionTabPage;
        private System.Windows.Forms.TextBox exceptionTextBox;
        private System.Windows.Forms.TabPage logTabPage;
        private System.Windows.Forms.TextBox logTextBox;
    }
}