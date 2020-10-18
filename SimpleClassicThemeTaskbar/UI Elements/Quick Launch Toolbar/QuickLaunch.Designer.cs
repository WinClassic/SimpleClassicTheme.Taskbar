using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.QuickLaunch
{
    partial class QuickLaunch
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
            this.verticalDivider1 = new SimpleClassicThemeTaskbar.UIElements.Misc.VerticalDivider();
            this.verticalDivider2 = new SimpleClassicThemeTaskbar.UIElements.Misc.VerticalDivider();
            this.SuspendLayout();
            // 
            // verticalDivider1
            // 
            this.verticalDivider1.BackColor = System.Drawing.SystemColors.Control;
            this.verticalDivider1.Location = new System.Drawing.Point(1, 3);
            this.verticalDivider1.Name = "verticalDivider1";
            this.verticalDivider1.Size = new System.Drawing.Size(7, 22);
            this.verticalDivider1.TabIndex = 0;
            this.verticalDivider1.Wide = true;
            // 
            // verticalDivider2
            // 
            this.verticalDivider2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.verticalDivider2.BackColor = System.Drawing.SystemColors.Control;
            this.verticalDivider2.Location = new System.Drawing.Point(142, 3);
            this.verticalDivider2.Name = "verticalDivider2";
            this.verticalDivider2.Size = new System.Drawing.Size(7, 22);
            this.verticalDivider2.TabIndex = 1;
            this.verticalDivider2.Wide = true;
            // 
            // QuickLaunch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.verticalDivider2);
            this.Controls.Add(this.verticalDivider1);
            this.Name = "QuickLaunch";
            this.Size = new System.Drawing.Size(150, 27);
            this.Load += new System.EventHandler(this.QuickLaunch_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UIElements.Misc.VerticalDivider verticalDivider1;
        private UIElements.Misc.VerticalDivider verticalDivider2;
    }
}
