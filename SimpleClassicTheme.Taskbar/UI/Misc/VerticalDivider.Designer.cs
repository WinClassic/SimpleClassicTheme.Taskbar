namespace SimpleClassicThemeTaskbar.UIElements.Misc
{
    partial class VerticalDivider
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
            this.betterBorderPanel1 = new SimpleClassicThemeTaskbar.UIElements.Misc.BetterBorderPanel();
            this.betterBorderPanel2 = new SimpleClassicThemeTaskbar.UIElements.Misc.BetterBorderPanel();
            this.betterBorderPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // betterBorderPanel1
            // 
            this.betterBorderPanel1.Location = new System.Drawing.Point(0, -2);
            this.betterBorderPanel1.Name = "betterBorderPanel1";
            this.betterBorderPanel1.Size = new System.Drawing.Size(2, 26);
            this.betterBorderPanel1.TabIndex = 0;
            // 
            // betterBorderPanel2
            // 
            this.betterBorderPanel2.Location = new System.Drawing.Point(4, 2);
            this.betterBorderPanel2.Name = "betterBorderPanel2";
            this.betterBorderPanel2.Size = new System.Drawing.Size(3, 18);
            this.betterBorderPanel2.TabIndex = 0;
            // 
            // VerticalDivider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.betterBorderPanel1);
            this.Controls.Add(this.betterBorderPanel2);
            this.Name = "VerticalDivider";
            this.Size = new System.Drawing.Size(7, 22);
            this.Load += new System.EventHandler(this.VerticalDivider_Load);
            this.betterBorderPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BetterBorderPanel betterBorderPanel1;
        private BetterBorderPanel betterBorderPanel2;
    }
}
