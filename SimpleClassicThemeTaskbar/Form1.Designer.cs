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
            this.line = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.startButton1 = new SimpleClassicThemeTaskbar.StartButton();
            this.vertSep2 = new VertSep();
            this.vertSep1 = new VertSep();
            this.SuspendLayout();
            // 
            // line
            // 
            this.line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.line.Location = new System.Drawing.Point(0, -1);
            this.line.Name = "line";
            this.line.Size = new System.Drawing.Size(676, 3);
            this.line.TabIndex = 0;
            this.line.Text = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // startButton1
            // 
            this.startButton1.Location = new System.Drawing.Point(3, 4);
            this.startButton1.Name = "startButton1";
            this.startButton1.Pressed = false;
            this.startButton1.Size = new System.Drawing.Size(52, 23);
            this.startButton1.TabIndex = 4;
            // 
            // vertSep2
            // 
            this.vertSep2.LineColor = System.Drawing.SystemColors.ControlText;
            this.vertSep2.Location = new System.Drawing.Point(67, 8);
            this.vertSep2.Name = "vertSep2";
            this.vertSep2.Size = new System.Drawing.Size(1, 14);
            this.vertSep2.TabIndex = 3;
            this.vertSep2.Text = "vertSep2";
            // 
            // vertSep1
            // 
            this.vertSep1.LineColor = System.Drawing.SystemColors.ControlText;
            this.vertSep1.Location = new System.Drawing.Point(64, 8);
            this.vertSep1.Name = "vertSep1";
            this.vertSep1.Size = new System.Drawing.Size(1, 14);
            this.vertSep1.TabIndex = 2;
            this.vertSep1.Text = "vertSep1";
            // 
            // Taskbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 30);
            this.Controls.Add(this.startButton1);
            this.Controls.Add(this.vertSep2);
            this.Controls.Add(this.vertSep1);
            this.Controls.Add(this.line);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
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

        private System.Windows.Forms.Label line;
        private VertSep vertSep1;
        private VertSep vertSep2;
        private System.Windows.Forms.Timer timer1;
        private StartButton startButton1;
    }
}

