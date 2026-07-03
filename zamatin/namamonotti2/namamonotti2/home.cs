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
    // 食材1件ぶんの表示用データ。DB連携ができたら StockItem から組み立てて渡す想定。
    public record StockCardData(string Name, string CategoryEmoji, string CategoryName, CharacterState State, int DaysLeft);

    public enum CharacterState { Fresh, Warning, Danger, Zombie }

    public partial class home : UserControl
    {
        readonly MainForm? _main;

        public home()
        {
            InitializeComponent();
        }

        public home(MainForm main) : this()
        {
            _main = main;
            Load += (s, e) => LoadCards();
        }

        void LoadCards()
        {
            // DB接続は未実装（データ層チームの完成待ち）。仮データで見た目だけ確認する。
            var items = GetMockStockItems();

            contentArea.Controls.Clear();

            if (items.Count == 0)
            {
                contentArea.Controls.Add(MakeHint("まだ食材が登録されていません。「とうろく」から追加してね！"));
                return;
            }

            var urgent = items.Where(i => i.State is CharacterState.Danger or CharacterState.Zombie).ToList();
            var normal = items.Where(i => i.State is CharacterState.Fresh or CharacterState.Warning).ToList();

            if (urgent.Count > 0)
            {
                contentArea.Controls.Add(MakeGroupLabel("⚠️ 早く使ってあげて"));
                contentArea.Controls.Add(MakeCharGrid(urgent));
            }
            if (normal.Count > 0)
            {
                contentArea.Controls.Add(MakeGroupLabel("😊 元気な食材"));
                contentArea.Controls.Add(MakeCharGrid(normal));
            }
        }

        static List<StockCardData> GetMockStockItems() =>
        [
            new("鶏むね肉", "🍗", "肉", CharacterState.Danger, 0),
            new("さば", "🐟", "魚", CharacterState.Zombie, -1),
            new("キャベツ", "🥬", "野菜", CharacterState.Warning, 2),
            new("牛乳", "🥛", "乳製品", CharacterState.Fresh, 5),
            new("たまご", "🥚", "卵", CharacterState.Fresh, 10),
        ];

        FlowLayoutPanel MakeCharGrid(List<StockCardData> items)
        {
            var grid = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Width = 840
            };
            foreach (var item in items)
                grid.Controls.Add(MakeCharCard(item));
            return grid;
        }

        Panel MakeCharCard(StockCardData item)
        {
            var (bg, badgeColor, badgeText, days) = item.State switch
            {
                CharacterState.Danger => (Color.FromArgb(255, 235, 235), Color.FromArgb(240, 106, 106), "危険", $"残{item.DaysLeft}日"),
                CharacterState.Zombie => (Color.FromArgb(230, 245, 230), Color.FromArgb(139, 191, 106), "ゾンビ", $"{item.DaysLeft}日"),
                CharacterState.Warning => (Color.FromArgb(255, 250, 220), Color.FromArgb(240, 179, 74), "心配", $"残{item.DaysLeft}日"),
                _ => (Color.FromArgb(255, 243, 214), Color.FromArgb(127, 212, 184), "元気", $"残{item.DaysLeft}日")
            };

            var card = new Panel { Width = 130, Height = 140, BackColor = bg, Margin = new Padding(6), Cursor = Cursors.Hand };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(255, 208, 224), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
            };

            var emoji = new Label { Text = item.CategoryEmoji, Font = new Font("Segoe UI Emoji", 28F), TextAlign = ContentAlignment.MiddleCenter, Width = 130, Height = 55, Top = 10, Left = 0 };
            var name = new Label { Text = item.Name, Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Width = 130, Height = 22, Top = 65, Left = 0, ForeColor = Color.FromArgb(107, 74, 85) };
            var daysLabel = new Label { Text = days, Font = new Font("Yu Gothic UI", 9F), TextAlign = ContentAlignment.MiddleCenter, Width = 130, Height = 18, Top = 85, Left = 0, ForeColor = badgeColor };
            var badge = new Label { Text = badgeText, Font = new Font("Yu Gothic UI", 8F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Width = 70, Height = 20, Top = 108, Left = 30, BackColor = badgeColor, ForeColor = Color.White };

            card.Controls.AddRange([emoji, name, daysLabel, badge]);
            card.Click += (s, e) => _main?.NavigateTo("stock");
            return card;
        }

        static Label MakeGroupLabel(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(239, 110, 152),
            AutoSize = true,
            Margin = new Padding(2, 12, 2, 4)
        };

        static Label MakeHint(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F),
            ForeColor = Color.FromArgb(177, 138, 150),
            AutoSize = true,
            Margin = new Padding(2, 6, 2, 6)
        };

        void proposeBtn_Click(object sender, EventArgs e)
        {
            _main?.NavigateTo("recipe");
        }

        private void hintLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
