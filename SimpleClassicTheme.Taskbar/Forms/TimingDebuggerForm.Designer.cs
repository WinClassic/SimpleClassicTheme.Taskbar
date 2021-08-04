
namespace SimpleClassicTheme.Taskbar.Forms
{
    partial class TimingDebuggerForm
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.timelinePanel = new System.Windows.Forms.Panel();
            this.listView = new System.Windows.Forms.ListView();
            this.timestampColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.durationColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.percentageColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.labelColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.togglesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.percentageCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.togglesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(8, 8);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer.Panel1.Controls.Add(this.timelinePanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listView);
            this.splitContainer.Size = new System.Drawing.Size(640, 394);
            this.splitContainer.SplitterDistance = 30;
            this.splitContainer.TabIndex = 0;
            // 
            // timelinePanel
            // 
            this.timelinePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timelinePanel.Location = new System.Drawing.Point(0, 0);
            this.timelinePanel.Name = "timelinePanel";
            this.timelinePanel.Size = new System.Drawing.Size(636, 26);
            this.timelinePanel.TabIndex = 0;
            this.timelinePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.TimelinePanel_Paint);
            this.timelinePanel.Resize += new System.EventHandler(this.TimelinePanel_Resize);
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.timestampColumnHeader,
            this.durationColumnHeader,
            this.percentageColumnHeader,
            this.labelColumnHeader});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(636, 356);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListView_RetrieveVirtualItem);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.listView.Resize += new System.EventHandler(this.ListView_Resize);
            // 
            // timestampColumnHeader
            // 
            this.timestampColumnHeader.Text = "Timestamp";
            // 
            // durationColumnHeader
            // 
            this.durationColumnHeader.Text = "Duration";
            // 
            // percentageColumnHeader
            // 
            this.percentageColumnHeader.Text = "Percentage";
            this.percentageColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelColumnHeader
            // 
            this.labelColumnHeader.Text = "Label";
            // 
            // togglesPanel
            // 
            this.togglesPanel.AutoSize = true;
            this.togglesPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.togglesPanel.Controls.Add(this.percentageCheckBox);
            this.togglesPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.togglesPanel.Location = new System.Drawing.Point(8, 402);
            this.togglesPanel.Margin = new System.Windows.Forms.Padding(0);
            this.togglesPanel.Name = "togglesPanel";
            this.togglesPanel.Size = new System.Drawing.Size(640, 23);
            this.togglesPanel.TabIndex = 1;
            // 
            // percentageCheckBox
            // 
            this.percentageCheckBox.AutoSize = true;
            this.percentageCheckBox.Checked = true;
            this.percentageCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.percentageCheckBox.Location = new System.Drawing.Point(3, 3);
            this.percentageCheckBox.Name = "percentageCheckBox";
            this.percentageCheckBox.Size = new System.Drawing.Size(110, 17);
            this.percentageCheckBox.TabIndex = 0;
            this.percentageCheckBox.Text = "Show &percentage";
            this.percentageCheckBox.UseVisualStyleBackColor = true;
            this.percentageCheckBox.CheckedChanged += new System.EventHandler(this.PercentageCheckBox_CheckedChanged);
            // 
            // TimingDebuggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 433);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.togglesPanel);
            this.Name = "TimingDebuggerForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Text = "TimingDebuggerForm";
            this.Load += new System.EventHandler(this.TimingDebuggerForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.togglesPanel.ResumeLayout(false);
            this.togglesPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel timelinePanel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader timestampColumnHeader;
        private System.Windows.Forms.ColumnHeader durationColumnHeader;
        private System.Windows.Forms.ColumnHeader labelColumnHeader;
        private System.Windows.Forms.FlowLayoutPanel togglesPanel;
        private System.Windows.Forms.CheckBox percentageCheckBox;
        private System.Windows.Forms.ColumnHeader percentageColumnHeader;
    }
}