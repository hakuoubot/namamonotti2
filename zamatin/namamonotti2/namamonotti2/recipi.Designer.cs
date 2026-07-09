namespace namamonotti2
{
    partial class recipi
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
            statusLabel.Font = new Font("Yu Gothic UI", 11F);
            statusLabel.ForeColor = Color.FromArgb(177, 138, 150);
            statusLabel.Location = new Point(0, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Padding = new Padding(4, 12, 4, 4);
            statusLabel.Size = new Size(200, 37);
            statusLabel.TabIndex = 0;
            statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
            //
            // contentArea
            //
            contentArea.AutoScroll = true;
            contentArea.Dock = DockStyle.Fill;
            contentArea.FlowDirection = FlowDirection.TopDown;
            contentArea.Location = new Point(0, 37);
            contentArea.Name = "contentArea";
            contentArea.Padding = new Padding(4);
            contentArea.Size = new Size(880, 523);
            contentArea.TabIndex = 1;
            contentArea.WrapContents = false;
            //
            // recipi
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 245, 248);
            Controls.Add(contentArea);
            Controls.Add(statusLabel);
            Name = "recipi";
            Size = new Size(880, 560);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label statusLabel;
        private FlowLayoutPanel contentArea;
    }
}
