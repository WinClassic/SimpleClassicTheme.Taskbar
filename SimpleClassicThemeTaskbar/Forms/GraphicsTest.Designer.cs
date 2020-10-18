namespace SimpleClassicThemeTaskbar.Forms
{
    partial class GraphicsTest
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
            this.betterBorderPanel1 = new SimpleClassicThemeTaskbar.UIElements.Misc.BetterBorderPanel();
            this.SuspendLayout();
            // 
            // betterBorderPanel1
            // 
            this.betterBorderPanel1.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
            this.betterBorderPanel1.IsButton = true;
            this.betterBorderPanel1.Location = new System.Drawing.Point(193, 120);
            this.betterBorderPanel1.Name = "betterBorderPanel1";
            this.betterBorderPanel1.Size = new System.Drawing.Size(200, 100);
            this.betterBorderPanel1.TabIndex = 0;
            // 
            // GraphicsTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.betterBorderPanel1);
            this.Name = "GraphicsTest";
            this.Text = "GraphicsTest";
            this.ResumeLayout(false);

        }

        #endregion

        private UIElements.Misc.BetterBorderPanel betterBorderPanel1;
    }
}