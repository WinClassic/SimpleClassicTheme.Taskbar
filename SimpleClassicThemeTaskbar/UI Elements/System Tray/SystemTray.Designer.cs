namespace SimpleClassicThemeTaskbar.UIElements.SystemTray
{
    partial class SystemTray
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.betterBorderPanel1 = new SimpleClassicThemeTaskbar.UIElements.Misc.BetterBorderPanel();
			this.labelTime = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.betterBorderPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// betterBorderPanel1
			// 
			this.betterBorderPanel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.betterBorderPanel1.Controls.Add(this.labelTime);
			this.betterBorderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.betterBorderPanel1.IsButton = false;
			this.betterBorderPanel1.Location = new System.Drawing.Point(0, 0);
			this.betterBorderPanel1.Name = "betterBorderPanel1";
			this.betterBorderPanel1.Size = new System.Drawing.Size(98, 22);
			this.betterBorderPanel1.TabIndex = 0;
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.AutoSize = true;
			this.labelTime.Location = new System.Drawing.Point(44, 5);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(47, 13);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "5:03 PM";
			this.labelTime.MouseHover += new System.EventHandler(this.labelTime_MouseHover);
			// 
			// SystemTray
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.betterBorderPanel1);
			this.Name = "SystemTray";
			this.Size = new System.Drawing.Size(98, 22);
			this.betterBorderPanel1.ResumeLayout(false);
			this.betterBorderPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private SimpleClassicThemeTaskbar.UIElements.Misc.BetterBorderPanel betterBorderPanel1;
        private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
