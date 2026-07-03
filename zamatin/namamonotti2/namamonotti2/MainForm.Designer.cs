namespace namamonotti2
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private Panel MakeDexCard(
    string emoji,
    string name,
    string countText,
    string dateText)
        {
            // カード本体
            Panel card = new Panel();
            card.Width = 120;
            card.Height = 150;
            card.BackColor = Color.Beige;
            card.Margin = new Padding(10);

            // 絵文字
            Label emojiLabel = new Label();
            emojiLabel.Text = emoji;
            emojiLabel.Font = new Font("Segoe UI Emoji", 24);
            emojiLabel.TextAlign = ContentAlignment.MiddleCenter;
            emojiLabel.SetBounds(0, 10, 120, 40);

            // 名前
            Label nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            nameLabel.SetBounds(0, 60, 120, 20);

            // 回数
            Label countLabel = new Label();
            countLabel.Text = countText;
            countLabel.TextAlign = ContentAlignment.MiddleCenter;
            countLabel.ForeColor = Color.Green;
            countLabel.SetBounds(0, 85, 120, 20);

            // 日付
            Label dateLabel = new Label();
            dateLabel.Text = dateText;
            dateLabel.TextAlign = ContentAlignment.MiddleCenter;
            dateLabel.SetBounds(0, 110, 120, 20);

            card.Controls.Add(emojiLabel);
            card.Controls.Add(nameLabel);
            card.Controls.Add(countLabel);
            card.Controls.Add(dateLabel);

            return card;
        }

        private void InitializeComponent()
        {
            sideNav = new Panel();
            navPanel = new FlowLayoutPanel();
            btnDex = new Button();
            btnRecipe = new Button();
            btnStock = new Button();
            btnAdd = new Button();
            btnHome = new Button();
            brandLabel = new Label();
            rightArea = new Panel();
            mainArea = new Panel();
            topBar = new Panel();
            titleLabel = new Label();
            sideNav.SuspendLayout();
            navPanel.SuspendLayout();
            rightArea.SuspendLayout();
            topBar.SuspendLayout();
            SuspendLayout();
            //
            // sideNav
            //
            sideNav.BackColor = Color.FromArgb(255, 143, 177);
            sideNav.Controls.Add(navPanel);
            sideNav.Controls.Add(brandLabel);
            sideNav.Dock = DockStyle.Left;
            sideNav.Location = new Point(0, 0);
            sideNav.Name = "sideNav";
            sideNav.Padding = new Padding(10);
            sideNav.Size = new Size(180, 631);
            sideNav.TabIndex = 0;
            //
            // navPanel
            //
            navPanel.Controls.Add(btnDex);
            navPanel.Controls.Add(btnRecipe);
            navPanel.Controls.Add(btnStock);
            navPanel.Controls.Add(btnAdd);
            navPanel.Controls.Add(btnHome);
            navPanel.Dock = DockStyle.Fill;
            navPanel.FlowDirection = FlowDirection.TopDown;
            navPanel.Location = new Point(10, 50);
            navPanel.Name = "navPanel";
            navPanel.Padding = new Padding(0, 4, 0, 0);
            navPanel.Size = new Size(160, 571);
            navPanel.TabIndex = 1;
            navPanel.WrapContents = false;
            //
            // btnHome
            //
            btnHome.BackColor = Color.White;
            btnHome.Cursor = Cursors.Hand;
            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.FlatStyle = FlatStyle.Flat;
            btnHome.Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold);
            btnHome.ForeColor = Color.FromArgb(239, 110, 152);
            btnHome.Location = new Point(0, 7);
            btnHome.Margin = new Padding(0, 3, 0, 0);
            btnHome.Name = "btnHome";
            btnHome.Padding = new Padding(8, 0, 0, 0);
            btnHome.Size = new Size(158, 42);
            btnHome.TabIndex = 0;
            btnHome.Text = "🏠 ホーム";
            btnHome.TextAlign = ContentAlignment.MiddleLeft;
            btnHome.UseVisualStyleBackColor = false;
            //
            // btnAdd
            //
            btnAdd.BackColor = Color.FromArgb(30, 255, 255, 255);
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Yu Gothic UI", 10F);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(0, 52);
            btnAdd.Margin = new Padding(0, 3, 0, 0);
            btnAdd.Name = "btnAdd";
            btnAdd.Padding = new Padding(8, 0, 0, 0);
            btnAdd.Size = new Size(158, 42);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "➕ とうろく";
            btnAdd.TextAlign = ContentAlignment.MiddleLeft;
            btnAdd.UseVisualStyleBackColor = false;
            //
            // btnStock
            //
            btnStock.BackColor = Color.FromArgb(30, 255, 255, 255);
            btnStock.Cursor = Cursors.Hand;
            btnStock.FlatAppearance.BorderSize = 0;
            btnStock.FlatStyle = FlatStyle.Flat;
            btnStock.Font = new Font("Yu Gothic UI", 10F);
            btnStock.ForeColor = Color.White;
            btnStock.Location = new Point(0, 97);
            btnStock.Margin = new Padding(0, 3, 0, 0);
            btnStock.Name = "btnStock";
            btnStock.Padding = new Padding(8, 0, 0, 0);
            btnStock.Size = new Size(158, 42);
            btnStock.TabIndex = 2;
            btnStock.Text = "📋 ざいこ";
            btnStock.TextAlign = ContentAlignment.MiddleLeft;
            btnStock.UseVisualStyleBackColor = false;
            //
            // btnRecipe
            //
            btnRecipe.BackColor = Color.FromArgb(30, 255, 255, 255);
            btnRecipe.Cursor = Cursors.Hand;
            btnRecipe.FlatAppearance.BorderSize = 0;
            btnRecipe.FlatStyle = FlatStyle.Flat;
            btnRecipe.Font = new Font("Yu Gothic UI", 10F);
            btnRecipe.ForeColor = Color.White;
            btnRecipe.Location = new Point(0, 142);
            btnRecipe.Margin = new Padding(0, 3, 0, 0);
            btnRecipe.Name = "btnRecipe";
            btnRecipe.Padding = new Padding(8, 0, 0, 0);
            btnRecipe.Size = new Size(158, 42);
            btnRecipe.TabIndex = 3;
            btnRecipe.Text = "🍳 りょうり";
            btnRecipe.TextAlign = ContentAlignment.MiddleLeft;
            btnRecipe.UseVisualStyleBackColor = false;
            //
            // btnDex
            //
            btnDex.BackColor = Color.FromArgb(30, 255, 255, 255);
            btnDex.Cursor = Cursors.Hand;
            btnDex.FlatAppearance.BorderSize = 0;
            btnDex.FlatStyle = FlatStyle.Flat;
            btnDex.Font = new Font("Yu Gothic UI", 10F);
            btnDex.ForeColor = Color.White;
            btnDex.Location = new Point(0, 187);
            btnDex.Margin = new Padding(0, 3, 0, 0);
            btnDex.Name = "btnDex";
            btnDex.Padding = new Padding(8, 0, 0, 0);
            btnDex.Size = new Size(158, 42);
            btnDex.TabIndex = 4;
            btnDex.Text = "🏆 ずかん";
            btnDex.TextAlign = ContentAlignment.MiddleLeft;
            btnDex.UseVisualStyleBackColor = false;
            //
            // brandLabel
            //
            brandLabel.Dock = DockStyle.Top;
            brandLabel.Font = new Font("Yu Gothic UI", 13F, FontStyle.Bold);
            brandLabel.ForeColor = Color.White;
            brandLabel.Location = new Point(10, 10);
            brandLabel.Name = "brandLabel";
            brandLabel.Padding = new Padding(4, 0, 0, 0);
            brandLabel.Size = new Size(160, 40);
            brandLabel.TabIndex = 0;
            brandLabel.Text = "🧊 なまものっち";
            brandLabel.TextAlign = ContentAlignment.MiddleLeft;
            //
            // rightArea
            //
            rightArea.Controls.Add(mainArea);
            rightArea.Controls.Add(topBar);
            rightArea.Dock = DockStyle.Fill;
            rightArea.Location = new Point(180, 0);
            rightArea.Name = "rightArea";
            rightArea.Size = new Size(904, 631);
            rightArea.TabIndex = 1;
            //
            // mainArea
            //
            mainArea.AutoScroll = true;
            mainArea.BackColor = Color.FromArgb(255, 245, 248);
            mainArea.Dock = DockStyle.Fill;
            mainArea.Location = new Point(0, 52);
            mainArea.Name = "mainArea";
            mainArea.Padding = new Padding(16);
            mainArea.Size = new Size(904, 579);
            mainArea.TabIndex = 1;
            //
            // topBar
            //
            topBar.BackColor = Color.White;
            topBar.Controls.Add(titleLabel);
            topBar.Dock = DockStyle.Top;
            topBar.Location = new Point(0, 0);
            topBar.Name = "topBar";
            topBar.Padding = new Padding(16, 0, 16, 0);
            topBar.Size = new Size(904, 52);
            topBar.TabIndex = 0;
            //
            // titleLabel
            //
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font("Yu Gothic UI", 13F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(107, 74, 85);
            titleLabel.Location = new Point(16, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Padding = new Padding(4, 0, 0, 0);
            titleLabel.Size = new Size(872, 52);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "ホーム（キャラ部屋）";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 245, 248);
            ClientSize = new Size(1084, 631);
            Controls.Add(rightArea);
            Controls.Add(sideNav);
            Font = new Font("Yu Gothic UI", 10F);
            MinimumSize = new Size(900, 560);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "🧊 なまものっち";
            sideNav.ResumeLayout(false);
            navPanel.ResumeLayout(false);
            rightArea.ResumeLayout(false);
            topBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel sideNav;
        private FlowLayoutPanel navPanel;
        private Button btnHome;
        private Button btnAdd;
        private Button btnStock;
        private Button btnRecipe;
        private Button btnDex;
        private Label brandLabel;
        private Panel rightArea;
        private Panel mainArea;
        private Panel topBar;
        private Label titleLabel;
    }
}
