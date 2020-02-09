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
            this.panel1 = new System.Windows.Forms.Panel();
            this.verticalDivider2 = new SimpleClassicThemeTaskbar.VerticalDivider();
            this.verticalDivider1 = new SimpleClassicThemeTaskbar.VerticalDivider();
            this.startButton1 = new SimpleClassicThemeTaskbar.StartButton();
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
            this.timer1.Interval = 150;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(174, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 7;
            // 
            // verticalDivider2
            // 
            this.verticalDivider2.BackColor = System.Drawing.SystemColors.Control;
            this.verticalDivider2.Location = new System.Drawing.Point(165, 4);
            this.verticalDivider2.Name = "verticalDivider2";
            this.verticalDivider2.Size = new System.Drawing.Size(7, 22);
            this.verticalDivider2.TabIndex = 9;
            // 
            // verticalDivider1
            // 
            this.verticalDivider1.BackColor = System.Drawing.SystemColors.Control;
            this.verticalDivider1.Location = new System.Drawing.Point(58, 4);
            this.verticalDivider1.Name = "verticalDivider1";
            this.verticalDivider1.Size = new System.Drawing.Size(7, 22);
            this.verticalDivider1.TabIndex = 8;
            // 
            // startButton1
            // 
            this.startButton1.Location = new System.Drawing.Point(2, 4);
            this.startButton1.Name = "startButton1";
            this.startButton1.Pressed = false;
            this.startButton1.Size = new System.Drawing.Size(54, 22);
            this.startButton1.TabIndex = 4;
            // 
            // Taskbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 28);
            this.Controls.Add(this.verticalDivider2);
            this.Controls.Add(this.verticalDivider1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.startButton1);
            this.Controls.Add(this.line);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
        private System.Windows.Forms.Timer timer1;
        private StartButton startButton1;
        private System.Windows.Forms.Panel panel1;
        private VerticalDivider verticalDivider1;
        private VerticalDivider verticalDivider2;
    }
}

