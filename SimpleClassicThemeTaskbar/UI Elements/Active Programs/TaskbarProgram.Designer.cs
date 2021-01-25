namespace SimpleClassicThemeTaskbar
{
    partial class TaskbarProgram
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
            this.line = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // line
            // 
            this.line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.line.Location = new System.Drawing.Point(0, -1);
            this.line.Name = "line";
            this.line.Size = new System.Drawing.Size(676, 3);
            this.line.TabIndex = 1;
            this.line.Text = "label1";
            // 
            // TaskbarProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.line);
            this.Name = "TaskbarProgram";
            this.Size = new System.Drawing.Size(143, 28);
            this.Load += new System.EventHandler(this.TaskbarProgram_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label line;
    }
}
