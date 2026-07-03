namespace namamonotti2
{
    partial class home
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            hintLabel = new Label();
            contentArea = new FlowLayoutPanel();
            proposeBtn = new Button();
            SuspendLayout();
            // 
            // hintLabel
            // 
            hintLabel.AutoSize = true;
            hintLabel.Dock = DockStyle.Top;
            hintLabel.Font = new Font("Yu Gothic UI", 10F);
            hintLabel.ForeColor = Color.FromArgb(177, 138, 150);
            hintLabel.Location = new Point(0, 0);
            hintLabel.Margin = new Padding(4, 0, 4, 0);
            hintLabel.Name = "hintLabel";
            hintLabel.Padding = new Padding(6, 13, 6, 13);
            hintLabel.Size = new Size(550, 54);
            hintLabel.TabIndex = 0;
            hintLabel.Text = "きょうも食材を救おう！ 期限が近い子が「使って！」と訴えています。";
            hintLabel.Click += hintLabel_Click;
            // 
            // contentArea
            // 
            contentArea.AutoScroll = true;
            contentArea.Dock = DockStyle.Fill;
            contentArea.FlowDirection = FlowDirection.TopDown;
            contentArea.Location = new Point(0, 54);
            contentArea.Margin = new Padding(4, 5, 4, 5);
            contentArea.Name = "contentArea";
            contentArea.Padding = new Padding(6, 7, 6, 7);
            contentArea.Size = new Size(1257, 772);
            contentArea.TabIndex = 1;
            contentArea.WrapContents = false;
            // 
            // proposeBtn
            // 
            proposeBtn.BackColor = Color.FromArgb(255, 143, 177);
            proposeBtn.Cursor = Cursors.Hand;
            proposeBtn.Dock = DockStyle.Bottom;
            proposeBtn.FlatAppearance.BorderSize = 0;
            proposeBtn.FlatStyle = FlatStyle.Flat;
            proposeBtn.Font = new Font("Yu Gothic UI", 11F, FontStyle.Bold);
            proposeBtn.ForeColor = Color.White;
            proposeBtn.Location = new Point(0, 826);
            proposeBtn.Margin = new Padding(4, 5, 4, 5);
            proposeBtn.Name = "proposeBtn";
            proposeBtn.Size = new Size(1257, 107);
            proposeBtn.TabIndex = 2;
            proposeBtn.Text = "🍳 この食材で料理を提案する";
            proposeBtn.UseVisualStyleBackColor = false;
            proposeBtn.Click += proposeBtn_Click;
            // 
            // home
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 245, 248);
            Controls.Add(contentArea);
            Controls.Add(proposeBtn);
            Controls.Add(hintLabel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "home";
            Size = new Size(1257, 933);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label hintLabel;
        private FlowLayoutPanel contentArea;
        private Button proposeBtn;
    }
}
