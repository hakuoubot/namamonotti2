using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace namamonotti2
{
    public record DexCategoryData(string Emoji, string Name, bool Unlocked, int Count, string LastDate);

    public partial class zukan : UserControl
    {
        readonly MainForm? _main;

        public zukan()
        {
            InitializeComponent();
        }

        public zukan(MainForm main) : this()
        {
            _main = main;
        }

        private void zukan_Load(object sender, EventArgs e)
        {
            // DB接続は未実装（データ層チームの完成待ち）。仮データで見た目だけ確認する。
            var categories = GetMockCategories();
            int unlockedCount = categories.Count(c => c.Unlocked);

            statusLabel.Text = $"コレクション {unlockedCount}/{categories.Count} ／ 救済率 82%";

            contentArea.Controls.Clear();
            foreach (var cat in categories)
                contentArea.Controls.Add(MakeDexCard(cat));
        }

        static List<DexCategoryData> GetMockCategories() =>
        [
            new("🍗", "肉", true, 12, "07/01"),
            new("🐟", "魚", true, 7, "06/28"),
            new("🥬", "野菜", true, 20, "07/02"),
            new("🥛", "乳製品", false, 0, ""),
            new("🥚", "卵", false, 0, ""),
            new("🍱", "その他", false, 0, ""),
        ];

        Panel MakeDexCard(DexCategoryData cat)
        {
            bool unlocked = cat.Unlocked;

            var card = new Panel
            {
                Width = 120,
                Height = 150,
                BackColor = unlocked ? Color.FromArgb(255, 243, 214) : Color.FromArgb(230, 230, 230),
                Margin = new Padding(10)
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(255, 208, 224), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
            };

            var emojiLabel = new Label
            {
                Text = unlocked ? cat.Emoji : "？",
                Font = new Font("Segoe UI Emoji", 24F),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.Black : Color.FromArgb(180, 180, 180)
            };
            emojiLabel.SetBounds(0, 10, 120, 40);

            var nameLabel = new Label
            {
                Text = cat.Name,
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.FromArgb(107, 74, 85) : Color.FromArgb(180, 180, 180)
            };
            nameLabel.SetBounds(0, 60, 120, 20);

            var countLabel = new Label
            {
                Text = unlocked ? $"×{cat.Count}" : "未成仏",
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.FromArgb(63, 158, 126) : Color.FromArgb(177, 138, 150)
            };
            countLabel.SetBounds(0, 85, 120, 20);

            var dateLabel = new Label
            {
                Text = unlocked ? cat.LastDate : "",
                Font = new Font("Yu Gothic UI", 8F),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(177, 138, 150)
            };
            dateLabel.SetBounds(0, 110, 120, 20);

            card.Controls.AddRange([emojiLabel, nameLabel, countLabel, dateLabel]);
            return card;
        }
    }
}
