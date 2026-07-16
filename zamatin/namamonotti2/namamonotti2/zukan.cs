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
    // 図鑑カード1件ぶんのデータ。
    // Unlocked=そのカテゴリを1回でも「成仏」させたことがあるか、Count=成仏させた回数
    public record DexCategoryData(string Emoji, string Name, bool Unlocked, int Count);

    public partial class zukan : UserControl
    {
        readonly MainForm? _main;

        // 図鑑に並べる固定の6カテゴリ（とうろく画面のカテゴリ一覧と揃えている）
        static readonly (string Emoji, string Name)[] AllCategories =
        [
            ("🍗", "肉"),
            ("🐟", "魚"),
            ("🥬", "野菜"),
            ("🥛", "乳製品"),
            ("🥚", "卵"),
            ("🍱", "その他"),
        ];

        // ゾンビ枠：成仏できずに廃棄してしまった食材をまとめて記録する特別枠（カテゴリ横断）
        const string ZombieName = "ゾンビ";
        const string ZombieEmoji = "🧟";

        public zukan()
        {
            InitializeComponent();
        }

        // MainForm(シェル)から生成されるときに呼ばれるコンストラクタ
        public zukan(MainForm main) : this()
        {
            _main = main;
        }

        // 画面表示時のメイン処理：カテゴリごとの収集カードを並べる
        private void zukan_Load(object sender, EventArgs e)
        {
            contentArea.Controls.Clear();

            List<DexCategoryData> categories;
            int totalSeibutsu, totalHaiki;
            try
            {
                (categories, totalSeibutsu, totalHaiki) = GetCategoriesFromDb();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "";
                contentArea.Controls.Add(new Label
                {
                    Text = $"DB接続エラー: {ex.Message}",
                    Font = new Font("Yu Gothic UI", 9F),
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Margin = new Padding(10)
                });
                return;
            }

            // 「成仏済み」のカテゴリ数と、救済率（成仏÷(成仏+廃棄)）を上部の集計表示に使う
            int unlockedCount = categories.Count(c => c.Unlocked);
            int totalEvents = totalSeibutsu + totalHaiki;
            string rescueText = totalEvents > 0
                ? $" ／ 救済率 {(int)Math.Round(totalSeibutsu * 100.0 / totalEvents)}%"
                : "";
            statusLabel.Text = $"コレクション {unlockedCount}/{categories.Count}{rescueText}";

            foreach (var cat in categories)
                contentArea.Controls.Add(MakeDexCard(cat));
        }

        // DB(food_Table)からカテゴリごとの成仏数・廃棄数を集計する
        // SITUATION='成仏' → 図鑑にカウント、SITUATION='廃棄' → 救済率の分母にのみ使う
        static (List<DexCategoryData> categories, int totalSeibutsu, int totalHaiki) GetCategoriesFromDb()
        {
            var seibutsuCounts = new Dictionary<string, int>();
            int totalSeibutsu = 0, totalHaiki = 0;

            using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT GENRU, SITUATION, COUNT(*) AS Cnt FROM dbo.food_Table WHERE SITUATION IS NOT NULL GROUP BY GENRU, SITUATION",
                conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string category = reader["GENRU"].ToString() ?? "";
                string situation = reader["SITUATION"].ToString() ?? "";
                int cnt = Convert.ToInt32(reader["Cnt"]);

                if (situation == "成仏")
                {
                    seibutsuCounts[category] = seibutsuCounts.GetValueOrDefault(category) + cnt;
                    totalSeibutsu += cnt;
                }
                else if (situation == "廃棄")
                {
                    totalHaiki += cnt;
                }
            }

            var categories = AllCategories
                .Select(c =>
                {
                    int count = seibutsuCounts.GetValueOrDefault(c.Name);
                    return new DexCategoryData(c.Emoji, c.Name, count > 0, count);
                })
                .ToList();

            // ゾンビ枠：カテゴリ問わず「廃棄」した回数がそのままカウントになる（1件でも廃棄したら解放）
            categories.Add(new DexCategoryData(ZombieEmoji, ZombieName, totalHaiki > 0, totalHaiki));

            return (categories, totalSeibutsu, totalHaiki);
        }

        // カテゴリ1件ぶんのカードを組み立てる
        // 成仏済み(unlocked)かどうかで、色・絵文字・表示文言を切り替える
        // 未成仏の場合：絵文字→「？」、名前・回数の文字色→グレー、回数→「未成仏」
        // カテゴリ1件ぶんのカードを組み立てる
        Panel MakeDexCard(DexCategoryData cat)
        {
            bool unlocked = cat.Unlocked;
            bool isZombie = cat.Name == ZombieName;

            // カード本体（成仏済みはクリーム色、未成仏はグレーの背景）
            var card = new Panel
            {
                Width = 140,
                Height = 145,
                BackColor = unlocked ? Color.FromArgb(255, 243, 214) : Color.FromArgb(230, 230, 230),
                Margin = new Padding(10)
            };

            // カードの描画処理（枠線と、未成仏時の「？」マークをここに描きます）
            card.Paint += (s, e) =>
            {
                // 枠線を描く
                using var pen = new Pen(Color.FromArgb(255, 208, 224), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);

                // 未成仏の場合はドット絵の代わりに「？」を表示する
                if (!unlocked)
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    using var markFont = new Font("Yu Gothic UI", 24F, FontStyle.Bold);
                    var markRect = new Rectangle(0, 14, 140, 56);
                    TextRenderer.DrawText(e.Graphics, "？", markFont, markRect, Color.FromArgb(180, 180, 180), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            };

            // 成仏済みのカテゴリは、homeと同じドット絵キャラを表示する。
            // homeと違い、ここは毎回同じ絵（パターン1固定）にする。
            PictureBox? charImage = null;
            if (unlocked)
            {
                var charBmp = PixelArt.Render(PixelArt.GetFirstPattern(cat.Name), 1, Color.Black, Color.White);
                charImage = new PictureBox
                {
                    Image = charBmp,
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    Width = 132,
                    Height = 56,
                    Top = 14,
                    Left = 4,
                    BackColor = Color.Transparent
                };
            }

            // カテゴリ名（肉・魚・野菜など）
            var nameLabel = new Label
            {
                Text = cat.Name,
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.FromArgb(107, 74, 85) : Color.FromArgb(180, 180, 180)
            };
            nameLabel.SetBounds(4, 76, 132, 24);

            // 成仏回数（未成仏なら「未成仏」と表示）
            var countLabel = new Label
            {
                Text = unlocked ? $"×{cat.Count}" : (isZombie ? "未発見" : "未成仏"),
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.FromArgb(63, 158, 126) : Color.FromArgb(177, 138, 150)
            };
            countLabel.SetBounds(4, 104, 132, 24);

            // ラベルたちをカードに追加（未成仏時はドット絵キャラなし）
            card.Controls.AddRange([nameLabel, countLabel]);
            if (charImage != null)
                card.Controls.Add(charImage);

            return card;
        }
        

           

        private void contentArea_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
