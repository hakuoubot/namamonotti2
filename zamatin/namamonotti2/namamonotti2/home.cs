using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace namamonotti2
{
    // 食材1件ぶんの表示用データ。DB連携ができたら StockItem から組み立てて渡す想定。
    public record StockCardData(string Name, string CategoryEmoji, string CategoryName, CharacterState State, int DaysLeft);

    // 食材の状態（賞味期限までの残り日数で決まる）
    // Fresh=元気, Warning=心配, Danger=危険, Zombie=期限切れ
    public enum CharacterState { Fresh, Warning, Danger, Zombie }

    public partial class home : UserControl
    {
        readonly MainForm? _main;

        public home()
        {
            InitializeComponent();
        }

        // MainForm(シェル)から生成されるときに呼ばれるコンストラクタ。
        // 画面が表示された(Load)タイミングでカード一覧を作る。
        public home(MainForm main) : this()
        {
            _main = main;
            Load += (s, e) => LoadCards();
        }

        // ホーム画面のメイン処理：在庫食材をキャラカードとして並べる
        void LoadCards()
        {
            contentArea.Controls.Clear();

            List<StockCardData> items;
            try
            {
                items = GetStockItemsFromDb();
            }
            catch (Exception ex)
            {
                contentArea.Controls.Add(MakeHint($"DB接続エラー: {ex.Message}"));
                return;
            }

            // 食材が1件もない場合は案内文だけ出して終了
            if (items.Count == 0)
            {
                contentArea.Controls.Add(MakeHint("まだ食材が登録されていません。「とうろく」から追加してね！"));
                return;
            }

            // 状態によって「危険・ゾンビ（急いで使うべき）」と「元気・心配（まだ余裕あり）」の
            // 2グループに分けて、危険なものを上に表示する
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

        // DB(food_Table)から在庫食材を取得する
        static List<StockCardData> GetStockItemsFromDb()
        {
            var items = new List<StockCardData>();

            using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
            // SITUATIONが入っている行は「成仏」済み（図鑑にカウント済み）なので在庫一覧には出さない
            using var cmd = new SqlCommand("SELECT FOODNAME, DATELANE, GENRU FROM dbo.food_Table WHERE SITUATION IS NULL ORDER BY DATELANE ASC", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string name = reader["FOODNAME"].ToString() ?? "";
                DateTime expiry = Convert.ToDateTime(reader["DATELANE"]);
                string category = reader["GENRU"].ToString() ?? "その他";
                int daysLeft = (expiry.Date - DateTime.Today).Days;

                items.Add(new StockCardData(name, EmojiFor(category), category, StateFor(daysLeft), daysLeft));
            }
            return items;
        }

        // カテゴリ名(GENRU)に対応する絵文字を割り当てる
        static string EmojiFor(string category) => category switch
        {
            "肉" => "🥩",
            "魚" => "🐟",
            "野菜" => "🥬",
            "乳製品" => "🥛",
            "卵" => "🥚",
            _ => "🍱",
        };

        // 残り日数から状態(State)を判定（zaikoの状態バッジと同じ基準）
        static CharacterState StateFor(int daysLeft) => daysLeft switch
        {
            < 0 => CharacterState.Zombie,
            <= 1 => CharacterState.Danger,
            <= 3 => CharacterState.Warning,
            _ => CharacterState.Fresh,
        };

        // 食材カードを横並び・折り返しで並べる入れ物を作る
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

        // 食材1件ぶんのカードを組み立てる
        // 状態(State)に応じて、背景色・バッジの色と文字・残り日数の表示を切り替える
        Panel MakeCharCard(StockCardData item)
        {
            var (bg, badgeColor, badgeText, days) = item.State switch
            {
                CharacterState.Danger => (Color.FromArgb(255, 235, 235), Color.FromArgb(240, 106, 106), "危険", $"残{item.DaysLeft}日"),
                CharacterState.Zombie => (Color.FromArgb(230, 245, 230), Color.FromArgb(139, 191, 106), "ゾンビ", $"{item.DaysLeft}日"),
                CharacterState.Warning => (Color.FromArgb(255, 250, 220), Color.FromArgb(240, 179, 74), "心配", $"残{item.DaysLeft}日"),
                _ => (Color.FromArgb(255, 243, 214), Color.FromArgb(127, 212, 184), "元気", $"残{item.DaysLeft}日")
            };

            // カード本体（角丸っぽく見せるため、枠線は自前で描画している）
            var card = new Panel { Width = 150, Height = 160, BackColor = bg, Margin = new Padding(6), Cursor = Cursors.Hand };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(255, 208, 224), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
            };

            // カードの中身：ドット絵キャラ・商品名・残り日数・状態バッジの4パーツ
            // 絵文字だった部分を、自作のドット絵キャラ画像に置き換え（1マス1px＝64px四方）
            // カテゴリごとに3パターンある中から、表示のたびにランダムで1つ選ぶ
            var charBmp = PixelArt.Render(PixelArt.GetRandomPattern(item.CategoryName), 1, Color.Black, Color.White);
            var charImage = new PictureBox
            {
                Image = charBmp,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Width = 142,
                Height = 60,
                Top = 10,
                Left = 4,
                BackColor = Color.Transparent
            };
            var name = new Label { Text = item.Name, Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Width = 142, Height = 24, Top = 74, Left = 4, ForeColor = Color.FromArgb(107, 74, 85) };
            var daysLabel = new Label { Text = days, Font = new Font("Yu Gothic UI", 9F), TextAlign = ContentAlignment.MiddleCenter, Width = 142, Height = 20, Top = 98, Left = 4, ForeColor = badgeColor };
            var badge = new Label { Text = badgeText, Font = new Font("Yu Gothic UI", 8F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Width = 80, Height = 22, Top = 122, Left = 35, BackColor = badgeColor, ForeColor = Color.White };

            card.Controls.AddRange([charImage, name, daysLabel, badge]);

            // カードをクリックすると「ざいこ」画面に飛ぶ
            card.Click += (s, e) => _main?.NavigateTo("stock");
            return card;
        }

        // 「⚠️早く使ってあげて」のような見出しラベルを作る共通処理
        static Label MakeGroupLabel(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(239, 110, 152),
            AutoSize = true,
            Margin = new Padding(2, 12, 2, 4)
        };

        // 在庫が0件のときに出す案内文を作る共通処理
        static Label MakeHint(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F),
            ForeColor = Color.FromArgb(177, 138, 150),
            AutoSize = true,
            Margin = new Padding(2, 6, 2, 6)
        };

        // 下部の「この食材で料理を提案する」ボタン：押すと「りょうり」画面（recipi）に遷移
        void proposeBtn_Click(object sender, EventArgs e)
        {
            _main?.NavigateTo("recipe");
        }

        private void hintLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
