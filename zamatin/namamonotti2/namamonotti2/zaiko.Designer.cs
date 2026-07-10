namespace namamonotti2
{
    partial class zaiko
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
            hintLabel1 = new Label();
            contentArea = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // hintLabel1
            // 
            hintLabel1.AutoSize = true;
            hintLabel1.Dock = DockStyle.Top;
            hintLabel1.Location = new Point(0, 0);
            hintLabel1.Name = "hintLabel1";
            hintLabel1.Size = new Size(327, 25);
            hintLabel1.TabIndex = 0;
            hintLabel1.Text = "並び: 残日数が近い順（危険・ゾンビが上）";
            // 
            // contentArea
            // 
            contentArea.AutoScroll = true;
            contentArea.Dock = DockStyle.Fill;
            contentArea.FlowDirection = FlowDirection.TopDown;
            contentArea.Location = new Point(0, 25);
            contentArea.Name = "contentArea";
            contentArea.Size = new Size(880, 535);
            contentArea.TabIndex = 1;
            // 
            // zaiko
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(contentArea);
            Controls.Add(hintLabel1);
            Name = "zaiko";
            Size = new Size(880, 560);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label hintLabel1;
        private FlowLayoutPanel contentArea;
    }
}
