namespace SimpleClassicThemeTaskbar.UIElements.OldSystemTray
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
			this.labelTime = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
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
            this.Controls.Add(labelTime);
            this.DoubleBuffered = true;
			this.Name = "SystemTray";
			this.Paint += SystemTray_Paint;
			this.Size = new System.Drawing.Size(98, 22);
			this.ResumeLayout(false);

        }

		#endregion

        private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
