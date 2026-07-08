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
    // 図鑑カード1件ぶんのデータ。DB連携ができたらCollection/Categoryから組み立てて渡す想定。
    // Unlocked=そのカテゴリを1回でも「成仏」させたことがあるか
    public record DexCategoryData(string Emoji, string Name, bool Unlocked, int Count, string LastDate);

    public partial class zukan : UserControl
    {
        readonly MainForm? _main;

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
            // DB接続は未実装（データ層チームの完成待ち）。仮データで見た目だけ確認する。
            var categories = GetMockCategories();

            // 「成仏済み」のカテゴリ数を数えて、上部の集計表示に使う
            int unlockedCount = categories.Count(c => c.Unlocked);
            statusLabel.Text = $"コレクション {unlockedCount}/{categories.Count} ／ 救済率 82%";

            // 前回表示分をクリアしてから、カテゴリ1つにつき1枚カードを作って並べる
            contentArea.Controls.Clear();
            foreach (var cat in categories)
                contentArea.Controls.Add(MakeDexCard(cat));
        }

        // 動作確認用の仮データ（DB接続ができたら本物のCollectionデータに差し替える）
        // 肉・魚・野菜は成仏済み、乳製品・卵・その他はまだ未成仏、という想定
        static List<DexCategoryData> GetMockCategories() =>
        [
            new("🍗", "肉", true, 12, "07/01"),
            new("🐟", "魚", true, 7, "06/28"),
            new("🥬", "野菜", true, 20, "07/02"),
            new("🥛", "乳製品", false, 0, ""),
            new("🥚", "卵", false, 0, ""),
            new("🍱", "その他", false, 0, ""),
        ];

        // カテゴリ1件ぶんのカードを組み立てる
        // 成仏済み(unlocked)かどうかで、色・絵文字・表示文言を切り替える
        // 未成仏の場合：絵文字→「？」、名前・回数の文字色→グレー、回数→「未成仏」
        // カテゴリ1件ぶんのカードを組み立てる
        Panel MakeDexCard(DexCategoryData cat)
        {
            bool unlocked = cat.Unlocked;

            // カード本体（成仏済みはクリーム色、未成仏はグレーの背景）
            var card = new Panel
            {
                Width = 120,
                Height = 150,
                BackColor = unlocked ? Color.FromArgb(255, 243, 214) : Color.FromArgb(230, 230, 230),
                Margin = new Padding(10)
            };

            // カードの描画処理（枠線と、カラー絵文字を直接ここに描きます）
            // カードの描画処理（枠線と、カラー絵文字を直接ここに描きます）
            card.Paint += (s, e) =>
            {
                // ① 枠線を描く（元からのコード）
                using var pen = new Pen(Color.FromArgb(255, 208, 224), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);

                // ★ここが超重要！カラー絵文字の色データを有効化する設定
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // ② カラー絵文字を直接描画する
                string emojiText = unlocked ? cat.Emoji : "？";
                using var emojiFont = new Font("Segoe UI Emoji", 24F);
                var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                var emojiRect = new Rectangle(0, 12, 120, 52);

                // カラー絵文字を描画するときは、Brushes.Black ではなく「SystemBrushes.WindowText」を使うか、
                // Windowsの描画エンジン（TextRenderer）に任せると確実に色が出ます
                TextRenderer.DrawText(e.Graphics, emojiText, emojiFont, emojiRect, Color. Black,TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            // カテゴリ名（肉・魚・野菜など）
            var nameLabel = new Label
            {
                Text = cat.Name,
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.FromArgb(107, 74, 85) : Color.FromArgb(180, 180, 180)
            };
            nameLabel.SetBounds(0, 68, 120, 20);

            // 成仏回数（未成仏なら「未成仏」と表示）
            var countLabel = new Label
            {
                Text = unlocked ? $"×{cat.Count}" : "未成仏",
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = unlocked ? Color.FromArgb(63, 158, 126) : Color.FromArgb(177, 138, 150)
            };
            countLabel.SetBounds(0, 93, 120, 20);

            // 最後に成仏させた日付（未成仏なら空欄）
            var dateLabel = new Label
            {
                Text = unlocked ? cat.LastDate : "",
                Font = new Font("Yu Gothic UI", 8F),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(177, 138, 150)
            };
            dateLabel.SetBounds(0, 118, 120, 20);

            // ラベルたちをカードに追加（emojiLabelは無いので除外しています）
            card.Controls.AddRange([nameLabel, countLabel, dateLabel]);

            return card;
        }
        

           

        private void contentArea_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
