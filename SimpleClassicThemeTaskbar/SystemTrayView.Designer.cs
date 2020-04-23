namespace SimpleClassicThemeTaskbar
{
    partial class SystemTrayView
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Icon = new System.Windows.Forms.DataGridViewImageColumn();
            this.Handle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WindowHandle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tooltip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Icon,
            this.Handle,
            this.WindowHandle,
            this.Tooltip});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(800, 450);
            this.dataGridView1.TabIndex = 0;
            // 
            // Icon
            // 
            this.Icon.HeaderText = "Icon";
            this.Icon.Name = "Icon";
            // 
            // Handle
            // 
            this.Handle.HeaderText = "Handle";
            this.Handle.Name = "Handle";
            this.Handle.ReadOnly = true;
            // 
            // WindowHandle
            // 
            this.WindowHandle.HeaderText = "WindowHandle";
            this.WindowHandle.Name = "WindowHandle";
            this.WindowHandle.ReadOnly = true;
            // 
            // Tooltip
            // 
            this.Tooltip.HeaderText = "Tooltip";
            this.Tooltip.Name = "Tooltip";
            this.Tooltip.ReadOnly = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(725, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // SystemTrayView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "SystemTrayView";
            this.Text = "SystemTrayView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewImageColumn Icon;
        private System.Windows.Forms.DataGridViewTextBoxColumn Handle;
        private System.Windows.Forms.DataGridViewTextBoxColumn WindowHandle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tooltip;
        private System.Windows.Forms.Button button1;
    }
}