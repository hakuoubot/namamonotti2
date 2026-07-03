namespace namamonotti2
{
    partial class zukan
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
            statusLabel = new Label();
            contentArea = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Dock = DockStyle.Top;
            statusLabel.Location = new Point(0, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(59, 25);
            statusLabel.TabIndex = 0;
            statusLabel.Text = "label1";
            // 
            // contentArea
            // 
            contentArea.AutoScroll = true;
            contentArea.Dock = DockStyle.Fill;
            contentArea.Location = new Point(0, 25);
            contentArea.Name = "contentArea";
            contentArea.Size = new Size(880, 535);
            contentArea.TabIndex = 1;
            // 
            // zukan
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(contentArea);
            Controls.Add(statusLabel);
            Name = "zukan";
            Size = new Size(880, 560);
            Load += zukan_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label statusLabel;
        private FlowLayoutPanel contentArea;
    }
}
