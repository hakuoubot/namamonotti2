using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace namamonotti2
{
    // レシピ1件ぶんのデータ（候補一覧・詳細の両方で使う。Claudeの提案結果をそのまま詰める）
    public record RecipeData(
        string Title, string Time, string Description, string UsedIngredients, string MissingIngredients,
        string Servings, List<string> Ingredients, List<string> Steps, string Tip);

    public partial class recipi : UserControl
    {
        readonly MainForm? _main;

        // Anthropic APIへの問い合わせ用（アプリ全体で使い回す）
        static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };

        // 一覧取得時にAPIから受け取った提案を保持しておき、詳細表示時に再度APIを呼ばずに済むようにする
        List<RecipeData> _candidates = [];

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
        async void ShowList()
        {
            statusLabel.Text = "在庫から作れる料理を考えています…";
            contentArea.Controls.Clear();

            List<(string Name, string Category, int DaysLeft)> inventory;
            try
            {
                inventory = GetInventoryFromDb();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
                contentArea.Controls.Add(MakeErrorLabel($"DB接続エラー: {ex.Message}"));
                return;
            }

            if (inventory.Count == 0)
            {
                statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
                contentArea.Controls.Add(MakeErrorLabel("在庫が空です。「とうろく」から食材を登録してね！"));
                return;
            }

            try
            {
                _candidates = await GetRecipesFromClaudeAsync(inventory);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
                contentArea.Controls.Add(MakeErrorLabel($"レシピ提案の取得に失敗しました: {ex.Message}"));
                return;
            }

            statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
            contentArea.Controls.Clear();
            foreach (var c in _candidates)
                contentArea.Controls.Add(MakeCandidateCard(c));
        }

        // DB(food_Table)から在庫食材を取得する（成仏・廃棄済みは除外、期限が近い順）
        // 期限切れ（ゾンビ状態）の食材は、傷んでいて使えないためレシピ提案の対象から除外する
        static List<(string Name, string Category, int DaysLeft)> GetInventoryFromDb()
        {
            var items = new List<(string, string, int)>();

            using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT FOODNAME, GENRU, DATELANE FROM dbo.food_Table WHERE SITUATION IS NULL ORDER BY DATELANE ASC",
                conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string name = reader["FOODNAME"].ToString() ?? "";
                string category = reader["GENRU"].ToString() ?? "その他";
                DateTime expiry = Convert.ToDateTime(reader["DATELANE"]);
                int daysLeft = (expiry.Date - DateTime.Today).Days;
                if (daysLeft < 0) continue; // ゾンビ（期限切れ）は除外
                items.Add((name, category, daysLeft));
            }
            return items;
        }

        // Claude(Anthropic API)に在庫食材を渡してレシピ提案を2〜3件もらう
        static async Task<List<RecipeData>> GetRecipesFromClaudeAsync(List<(string Name, string Category, int DaysLeft)> inventory)
        {
            string apiKey = ConfigurationManager.AppSettings["AnthropicApiKey"] ?? "";
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("APIキーが設定されていません（App.configのAnthropicApiKeyを確認してください）");

            var inventoryLines = inventory.Select(i => $"- {i.Name}（{i.Category}、残り{i.DaysLeft}日）");
            string prompt = $$"""
                あなたは家庭料理のレシピ提案アシスタントです。以下は冷蔵庫の在庫食材のリストです（期限が近い順）。

                在庫:
                {{string.Join("\n", inventoryLines)}}

                この在庫のうち、期限が近い食材を優先的に使い切れる料理を2〜3品提案してください。
                在庫にない食材を使っても構いませんが、その場合は「不足食材」として明記してください。

                出力は次のJSON形式のみで返してください。説明文やコードブロックの記法（```）は付けないでください。

                {
                  "recipes": [
                    {
                      "title": "料理名",
                      "time": "調理時間の目安（例: 約25分）",
                      "description": "一言説明",
                      "usedIngredients": "使う在庫食材をカンマ区切りで",
                      "missingIngredients": "不足している食材をカンマ区切り（無ければ空文字）",
                      "servings": "分量（例: 2人前）",
                      "ingredients": ["材料と分量のリスト（在庫のものには「（在庫）」を付ける）"],
                      "steps": ["手順を1ステップずつ"],
                      "tip": "コツやポイント（無ければ空文字）"
                    }
                  ]
                }
                """;

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            request.Headers.Add("x-api-key", apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                model = "claude-haiku-4-5-20251001",
                max_tokens = 2000,
                messages = new[] { new { role = "user", content = prompt } }
            }), Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var response = await _http.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Anthropic API エラー ({(int)response.StatusCode}): {responseBody}");

            using var responseDoc = JsonDocument.Parse(responseBody);
            string text = responseDoc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? "";

            // モデルがコードブロック(```json ... ```)で返してきた場合に備えて取り除く
            text = text.Trim();
            if (text.StartsWith("```"))
            {
                int firstNewline = text.IndexOf('\n');
                int lastFence = text.LastIndexOf("```");
                text = text[(firstNewline + 1)..lastFence].Trim();
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var parsed = JsonSerializer.Deserialize<RecipeApiResponse>(text, options)
                ?? throw new InvalidOperationException("レシピ提案の形式を解釈できませんでした");

            return parsed.Recipes.Select(r => new RecipeData(
                r.Title, r.Time, r.Description, r.UsedIngredients, r.MissingIngredients,
                r.Servings, r.Ingredients, r.Steps, r.Tip)).ToList();
        }

        // Claudeからの応答(JSON)を受け取るためだけの入れ物
        record RecipeApiResponse(List<RecipeApiItem> Recipes);
        record RecipeApiItem(
            string Title, string Time, string Description, string UsedIngredients, string MissingIngredients,
            string Servings, List<string> Ingredients, List<string> Steps, string Tip);

        // 候補1件ぶんのカードを組み立てる
        Panel MakeCandidateCard(RecipeData c)
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
        void ShowDetail(RecipeData detail)
        {
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
            // 使った食材を一覧から選んでもらい、選ばれた分だけ成仏（SITUATION更新）させる
            doneBtn.Click += (s, e) =>
            {
                using var form = new CompleteRecipeForm(detail.UsedIngredients);
                if (form.ShowDialog(FindForm()) == DialogResult.OK)
                    _main?.NavigateTo("dex");
            };
            contentArea.Controls.Add(doneBtn);
        }

        // 「← 候補にもどる」ボタン：押すと候補一覧に戻る（再度APIは呼ばず、取得済みの候補をそのまま出す）
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
            btn.Click += (s, e) =>
            {
                statusLabel.Text = "在庫から作れる料理（期限が近い食材を優先）";
                contentArea.Controls.Clear();
                foreach (var c in _candidates)
                    contentArea.Controls.Add(MakeCandidateCard(c));
            };
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

        static Label MakeErrorLabel(string text) => new()
        {
            Text = text,
            Font = new Font("Yu Gothic UI", 9F),
            ForeColor = Color.Red,
            AutoSize = true,
            Margin = new Padding(10)
        };
    }
}
