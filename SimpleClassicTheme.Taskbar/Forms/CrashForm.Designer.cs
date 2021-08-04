
namespace SimpleClassicTheme.Taskbar.Forms
{
    partial class CrashForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.dontSendButton = new System.Windows.Forms.Button();
            this.dontAskAgainCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.descriptionLinkLabel = new System.Windows.Forms.LinkLabel();
            this.viewDetailsButton = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.submitLogCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SimpleClassicTheme.Taskbar.Properties.Resources.CrashIcon;
            this.pictureBox1.Location = new System.Drawing.Point(11, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(324, 13);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "It seems like SimpleClassicTheme Taskbar has crashed.";
            // 
            // sendButton
            // 
            this.sendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sendButton.Location = new System.Drawing.Point(304, 122);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // dontSendButton
            // 
            this.dontSendButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.dontSendButton.Location = new System.Drawing.Point(223, 122);
            this.dontSendButton.Name = "dontSendButton";
            this.dontSendButton.Size = new System.Drawing.Size(75, 23);
            this.dontSendButton.TabIndex = 1;
            this.dontSendButton.Text = "Don\'t send";
            this.dontSendButton.UseVisualStyleBackColor = true;
            // 
            // dontAskAgainCheckBox
            // 
            this.dontAskAgainCheckBox.AutoSize = true;
            this.dontAskAgainCheckBox.Enabled = false;
            this.dontAskAgainCheckBox.Location = new System.Drawing.Point(0, 88);
            this.dontAskAgainCheckBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.dontAskAgainCheckBox.Name = "dontAskAgainCheckBox";
            this.dontAskAgainCheckBox.Size = new System.Drawing.Size(100, 17);
            this.dontAskAgainCheckBox.TabIndex = 0;
            this.dontAskAgainCheckBox.Text = "Don\'t ask again";
            this.dontAskAgainCheckBox.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.titleLabel);
            this.flowLayoutPanel1.Controls.Add(this.descriptionLinkLabel);
            this.flowLayoutPanel1.Controls.Add(this.submitLogCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.dontAskAgainCheckBox);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(50, 11);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(329, 105);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // descriptionLinkLabel
            // 
            this.descriptionLinkLabel.AutoSize = true;
            this.descriptionLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(151, 10);
            this.descriptionLinkLabel.Location = new System.Drawing.Point(0, 21);
            this.descriptionLinkLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.descriptionLinkLabel.Name = "descriptionLinkLabel";
            this.descriptionLinkLabel.Size = new System.Drawing.Size(327, 42);
            this.descriptionLinkLabel.TabIndex = 3;
            this.descriptionLinkLabel.TabStop = true;
            this.descriptionLinkLabel.Text = "SimpleClassicTheme exited due to an error. If you\'d like click on Send, to send u" +
    "s an error report over Sentry. To learn more about the data collected click here" +
    ".";
            this.descriptionLinkLabel.UseCompatibleTextRendering = true;
            this.descriptionLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DescriptionLinkLabel_LinkClicked);
            // 
            // viewDetailsButton
            // 
            this.viewDetailsButton.Location = new System.Drawing.Point(11, 122);
            this.viewDetailsButton.Name = "viewDetailsButton";
            this.viewDetailsButton.Size = new System.Drawing.Size(75, 23);
            this.viewDetailsButton.TabIndex = 3;
            this.viewDetailsButton.Text = "View Details";
            this.viewDetailsButton.UseVisualStyleBackColor = true;
            this.viewDetailsButton.Click += new System.EventHandler(this.viewDetailsButton_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
            // 
            // submitLogCheckBox
            // 
            this.submitLogCheckBox.AutoSize = true;
            this.submitLogCheckBox.Checked = true;
            this.submitLogCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.submitLogCheckBox.Location = new System.Drawing.Point(0, 71);
            this.submitLogCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.submitLogCheckBox.Name = "submitLogCheckBox";
            this.submitLogCheckBox.Size = new System.Drawing.Size(167, 17);
            this.submitLogCheckBox.TabIndex = 4;
            this.submitLogCheckBox.Text = "Submit log file with error report";
            this.submitLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // CrashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 156);
            this.Controls.Add(this.viewDetailsButton);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.dontSendButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SimpleClassicTheme Taskbar crashed";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CrashForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button dontSendButton;
        private System.Windows.Forms.CheckBox dontAskAgainCheckBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button viewDetailsButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.LinkLabel descriptionLinkLabel;
        private System.Windows.Forms.CheckBox submitLogCheckBox;
    }
}