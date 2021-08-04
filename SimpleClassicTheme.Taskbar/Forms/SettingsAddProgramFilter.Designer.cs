namespace SimpleClassicThemeTaskbar
{
	partial class SettingsAddProgramFilter
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
			this.button2 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.betterBorderPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// betterBorderPanel1
			// 
			this.betterBorderPanel1.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
			this.betterBorderPanel1.Controls.Add(this.button2);
			this.betterBorderPanel1.Controls.Add(this.label2);
			this.betterBorderPanel1.Controls.Add(this.label1);
			this.betterBorderPanel1.Controls.Add(this.comboBox1);
			this.betterBorderPanel1.Controls.Add(this.textBox1);
			this.betterBorderPanel1.Controls.Add(this.button1);
			this.betterBorderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.betterBorderPanel1.IsButton = false;
			this.betterBorderPanel1.Location = new System.Drawing.Point(0, 0);
			this.betterBorderPanel1.Name = "betterBorderPanel1";
			this.betterBorderPanel1.Size = new System.Drawing.Size(223, 102);
			this.betterBorderPanel1.TabIndex = 0;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(55, 65);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Filter value";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Filter type";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "ClassName",
            "WindowName",
            "ProcessName"});
			this.comboBox1.Location = new System.Drawing.Point(90, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 2;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(90, 39);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(121, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(136, 65);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Add";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// SettingsAddProgramFilter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(223, 102);
			this.Controls.Add(this.betterBorderPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SettingsAddProgramFilter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SettingsAddProgramFilter";
			this.betterBorderPanel1.ResumeLayout(false);
			this.betterBorderPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UIElements.Misc.BetterBorderPanel betterBorderPanel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}