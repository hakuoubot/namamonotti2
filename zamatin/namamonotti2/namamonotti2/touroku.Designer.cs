namespace namamonotti2
{
    partial class touroku
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            titleLabel = new Label();
            card = new Panel();
            photoScanBtn = new Button();
            saveBtn = new Button();
            datePicker = new DateTimePicker();
            lblExpiry = new Label();
            unitBox = new ComboBox();
            qtyBox = new NumericUpDown();
            lblQty = new Label();
            categoryBox = new ComboBox();
            lblCategory = new Label();
            nameBox = new TextBox();
            lblName = new Label();
            card.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)qtyBox).BeginInit();
            SuspendLayout();
            //
            // titleLabel
            //
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Yu Gothic UI", 13F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(107, 74, 85);
            titleLabel.Location = new Point(20, 16);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(150, 25);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "在庫とうろく";
            //
            // card
            //
            card.BackColor = Color.White;
            card.Controls.Add(photoScanBtn);
            card.Controls.Add(saveBtn);
            card.Controls.Add(datePicker);
            card.Controls.Add(lblExpiry);
            card.Controls.Add(unitBox);
            card.Controls.Add(qtyBox);
            card.Controls.Add(lblQty);
            card.Controls.Add(categoryBox);
            card.Controls.Add(lblCategory);
            card.Controls.Add(nameBox);
            card.Controls.Add(lblName);
            card.Location = new Point(20, 56);
            card.Name = "card";
            card.Padding = new Padding(16);
            card.Size = new Size(420, 410);
            card.TabIndex = 1;
            //
            // photoScanBtn
            //
            photoScanBtn.BackColor = Color.FromArgb(255, 235, 240);
            photoScanBtn.Cursor = Cursors.Hand;
            photoScanBtn.FlatAppearance.BorderSize = 0;
            photoScanBtn.FlatStyle = FlatStyle.Flat;
            photoScanBtn.Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold);
            photoScanBtn.ForeColor = Color.FromArgb(107, 74, 85);
            photoScanBtn.Location = new Point(20, 16);
            photoScanBtn.Name = "photoScanBtn";
            photoScanBtn.Size = new Size(360, 34);
            photoScanBtn.TabIndex = 0;
            photoScanBtn.Text = "📷 バーコードでスキャン";
            photoScanBtn.UseVisualStyleBackColor = false;
            photoScanBtn.Click += photoScanBtn_Click;
            //
            // lblName
            //
            lblName.AutoSize = true;
            lblName.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold);
            lblName.ForeColor = Color.FromArgb(107, 74, 85);
            lblName.Location = new Point(20, 60);
            lblName.Name = "lblName";
            lblName.Size = new Size(60, 17);
            lblName.TabIndex = 1;
            lblName.Text = "商品名 *";
            //
            // nameBox
            //
            nameBox.Font = new Font("Yu Gothic UI", 10F);
            nameBox.Location = new Point(20, 80);
            nameBox.Name = "nameBox";
            nameBox.Size = new Size(360, 25);
            nameBox.TabIndex = 2;
            //
            // lblCategory
            //
            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold);
            lblCategory.ForeColor = Color.FromArgb(107, 74, 85);
            lblCategory.Location = new Point(20, 116);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(70, 17);
            lblCategory.TabIndex = 3;
            lblCategory.Text = "カテゴリ *";
            //
            // categoryBox
            //
            categoryBox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryBox.Font = new Font("Yu Gothic UI", 10F);
            categoryBox.Items.AddRange(new object[] { "肉", "魚", "野菜", "乳製品", "卵", "その他" });
            categoryBox.Location = new Point(20, 136);
            categoryBox.Name = "categoryBox";
            categoryBox.Size = new Size(360, 25);
            categoryBox.TabIndex = 4;
            //
            // lblQty
            //
            lblQty.AutoSize = true;
            lblQty.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold);
            lblQty.ForeColor = Color.FromArgb(107, 74, 85);
            lblQty.Location = new Point(20, 172);
            lblQty.Name = "lblQty";
            lblQty.Size = new Size(40, 17);
            lblQty.TabIndex = 5;
            lblQty.Text = "数量";
            //
            // qtyBox
            //
            qtyBox.Font = new Font("Yu Gothic UI", 10F);
            qtyBox.Location = new Point(20, 192);
            qtyBox.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            qtyBox.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            qtyBox.Name = "qtyBox";
            qtyBox.Size = new Size(100, 25);
            qtyBox.TabIndex = 6;
            qtyBox.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // unitBox
            //
            unitBox.DropDownStyle = ComboBoxStyle.DropDownList;
            unitBox.Font = new Font("Yu Gothic UI", 10F);
            unitBox.Items.AddRange(new object[] { "個", "本", "g", "ml", "袋", "枚", "匹" });
            unitBox.Location = new Point(130, 192);
            unitBox.Name = "unitBox";
            unitBox.Size = new Size(120, 25);
            unitBox.TabIndex = 7;
            //
            // lblExpiry
            //
            lblExpiry.AutoSize = true;
            lblExpiry.Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold);
            lblExpiry.ForeColor = Color.FromArgb(107, 74, 85);
            lblExpiry.Location = new Point(20, 228);
            lblExpiry.Name = "lblExpiry";
            lblExpiry.Size = new Size(70, 17);
            lblExpiry.TabIndex = 8;
            lblExpiry.Text = "賞味期限";
            //
            // datePicker
            //
            datePicker.Font = new Font("Yu Gothic UI", 10F);
            datePicker.Format = DateTimePickerFormat.Short;
            datePicker.Location = new Point(20, 248);
            datePicker.Name = "datePicker";
            datePicker.Size = new Size(360, 25);
            datePicker.TabIndex = 9;
            //
            // saveBtn
            //
            saveBtn.BackColor = Color.FromArgb(255, 143, 177);
            saveBtn.Cursor = Cursors.Hand;
            saveBtn.FlatAppearance.BorderSize = 0;
            saveBtn.FlatStyle = FlatStyle.Flat;
            saveBtn.Font = new Font("Yu Gothic UI", 11F, FontStyle.Bold);
            saveBtn.ForeColor = Color.White;
            saveBtn.Location = new Point(20, 294);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new Size(360, 44);
            saveBtn.TabIndex = 10;
            saveBtn.Text = "登録する";
            saveBtn.UseVisualStyleBackColor = false;
            saveBtn.Click += saveBtn_Click;
            //
            // touroku
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 245, 248);
            Size = new Size(480, 500);
            Controls.Add(card);
            Controls.Add(titleLabel);
            Font = new Font("Yu Gothic UI", 9F);
            Name = "touroku";
            card.ResumeLayout(false);
            card.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)qtyBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label titleLabel;
        private Panel card;
        private Button photoScanBtn;
        private Label lblName;
        private TextBox nameBox;
        private Label lblCategory;
        private ComboBox categoryBox;
        private Label lblQty;
        private NumericUpDown qtyBox;
        private ComboBox unitBox;
        private Label lblExpiry;
        private DateTimePicker datePicker;
        private Button saveBtn;
    }
}
