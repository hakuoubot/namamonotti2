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
    // 料理候補1件ぶんのデータ。AI連携ができたらClaudeの提案結果から組み立てる想定。
    public record RecipeCandidateData(string Title, string Time, string Description, string UsedIngredients, string MissingIngredients);

    // レシピ詳細1件ぶんのデータ。AI連携ができたらClaudeの詳細レシピから組み立てる想定。
    public record RecipeDetailData(string Title, string Servings, string Time, List<string> Ingredients, List<string> Steps, string Tip);

    public partial class recipi : UserControl
    {
        readonly MainForm? _main;

        public recipi()
        {
            InitializeComponent();
        }

        // MainForm(シェル)から生成されるときに呼ばれるコンストラクタ。
        // 画面が表示された(Load)タイミングで、まず候補一覧を表示する。
        public recipi(MainForm main) : this()
        {
            _main = main;
            Load += (s, e) => ShowList();
        }

        // ===== モード①：候補一覧（S-04） =====
        void ShowList()
        {
            // AI連携は未実装（Claude APIキー未取得のため）。仮データで見た目だけ確認する。
            var candidates = GetMockCandidates();

            statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
            contentArea.Controls.Clear();

            foreach (var c in candidates)
                contentArea.Controls.Add(MakeCandidateCard(c));
        }

        // 動作確認用の仮データ（AI連携ができたら本物の提案結果に差し替える）
        static List<RecipeCandidateData> GetMockCandidates() =>
        [
            new("鶏もものトマト煮", "約25分", "期限が近い鶏もも肉を活用", "鶏もも・玉ねぎ", "トマト缶"),
            new("さばの味噌煮", "約20分", "定番の一品", "さば・味噌", ""),
        ];

        // 候補1件ぶんのカードを組み立てる
        Panel MakeCandidateCard(RecipeCandidateData c)
        {
            var card = new Panel { Width = 830, Height = 130, BackColor = Color.White, Margin = new Padding(2, 4, 2, 4) };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(255, 208, 224), 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
            };

            var titleLabel = new Label { Text = c.Title, Font = new Font("Yu Gothic UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(107, 74, 85), Width = 500, Height = 26, Top = 12, Left = 12 };
            var timeLabel = new Label { Text = c.Time, Font = new Font("Yu Gothic UI", 9F), ForeColor = Color.FromArgb(177, 138, 150), Width = 100, Height = 26, Top = 12, Left = 700, TextAlign = ContentAlignment.MiddleRight };
            var descLabel = new Label { Text = c.Description, Font = new Font("Yu Gothic UI", 9F), ForeColor = Color.FromArgb(177, 138, 150), Width = 780, Height = 22, Top = 38, Left = 12 };

            // 使用食材・不足食材をチップ風に表示
            string chipText = $"使用: {c.UsedIngredients}" + (string.IsNullOrEmpty(c.MissingIngredients) ? "" : $"　不足: {c.MissingIngredients}");
            var chipLabel = new Label { Text = chipText, Font = new Font("Yu Gothic UI", 9F), ForeColor = Color.FromArgb(63, 158, 126), Width = 780, Height = 22, Top = 60, Left = 12 };

            var makeBtn = new Button { Text = "これを作る", Width = 200, Height = 36, Top = 86, Left = 618, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 143, 177), ForeColor = Color.White, Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            makeBtn.FlatAppearance.BorderSize = 0;
            makeBtn.Click += (s, e) => ShowDetail(c);

            card.Controls.AddRange([titleLabel, timeLabel, descLabel, chipLabel, makeBtn]);
            return card;
        }

        // ===== モード②：レシピ詳細（S-05） =====
        void ShowDetail(RecipeCandidateData candidate)
        {
            // AI連携は未実装（Claude APIキー未取得のため）。仮データで見た目だけ確認する。
            var detail = GetMockDetail(candidate);

            statusLabel.Text = $"レシピ詳細：{detail.Title}";
            contentArea.Controls.Clear();

            contentArea.Controls.Add(MakeBackButton());
            contentArea.Controls.Add(MakeInfoLabel($"{detail.Servings} / {detail.Time}"));

            contentArea.Controls.Add(MakeSectionLabel("■ 材料"));
            foreach (var ing in detail.Ingredients)
                contentArea.Controls.Add(MakeBodyLabel($"　・{ing}"));

            contentArea.Controls.Add(MakeSectionLabel("■ 手順"));
            for (int i = 0; i < detail.Steps.Count; i++)
                contentArea.Controls.Add(MakeBodyLabel($"　{i + 1}. {detail.Steps[i]}"));

            if (!string.IsNullOrEmpty(detail.Tip))
                contentArea.Controls.Add(MakeBodyLabel($"■ コツ: {detail.Tip}"));

            var doneBtn = new Button
            {
                Text = "✨ 作った！食材を成仏させる",
                Width = 500,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(127, 212, 184),
                ForeColor = Color.White,
                Font = new Font("Yu Gothic UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 16, 0, 0)
            };
            doneBtn.FlatAppearance.BorderSize = 0;
            // 在庫消費（成仏）処理はDB接続ができてから実装。今は図鑑画面へ遷移するだけ。
            doneBtn.Click += (s, e) => _main?.NavigateTo("dex");
            contentArea.Controls.Add(doneBtn);
        }

        // 動作確認用の仮詳細データ（AI連携ができたら本物のレシピ詳細に差し替える）
        static RecipeDetailData GetMockDetail(RecipeCandidateData c) => c.Title switch
        {
            "鶏もものトマト煮" => new(c.Title, "2人前", c.Time,
                ["鶏もも肉 300g（在庫）", "玉ねぎ 1個（在庫）", "トマト缶 1缶"],
                ["鶏もも肉を一口大に切る", "玉ねぎを炒める", "トマト缶を加えて煮込む"],
                "弱火でじっくり"),
            _ => new(c.Title, "2人前", c.Time,
                ["さば 1切れ（在庫）", "味噌 大さじ2"],
                ["さばに切れ目を入れる", "味噌だれで煮る"],
                ""),
        };

        // 「← 候補にもどる」ボタン：押すと候補一覧に戻る
        Button MakeBackButton()
        {
            var btn = new Button
            {
                Text = "← 候補にもどる",
                Width = 140,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(177, 138, 150),
                Font = new Font("Yu Gothic UI", 9F),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 8)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(255, 208, 224);
            btn.Click += (s, e) => ShowList();
            return btn;
        }

        static Label MakeInfoLabel(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F),
            ForeColor = Color.FromArgb(177, 138, 150),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        };

        static Label MakeSectionLabel(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(239, 110, 152),
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 4)
        };

        static Label MakeBodyLabel(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 10F),
            ForeColor = Color.FromArgb(107, 74, 85),
            AutoSize = true,
            Width = 780,
            Margin = new Padding(4, 1, 4, 1)
        };
    }
}
