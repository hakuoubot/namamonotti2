using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace namamonotti2
{
    public partial class zaiko : UserControl
    {
        readonly MainForm? _main;

        public zaiko()
        {
            InitializeComponent();
            LoadIngredients(); // 画面が開いたときにDB(food_Table)からデータを読み込む
        }

        // MainForm(シェル)から生成されるときに呼ばれるコンストラクタ
        public zaiko(MainForm main) : this()
        {
            _main = main;
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

        // 残り日数から「状態バッジ」の文言と色を判定（他の画面と同じ基準）
        static (Color, string) StateFor(int daysLeft) => daysLeft switch
        {
            < 0 => (Color.LightGreen, $"{daysLeft}日（ゾンビ）"),
            <= 1 => (Color.LightPink, $"残り{daysLeft}日（危険）"),
            <= 3 => (Color.LightYellow, $"残り{daysLeft}日（心配）"),
            _ => (Color.LightGreen, $"残り{daysLeft}日（元気）"),
        };

        // DB(food_Table)から在庫食材を取得して、期限が近い順に画面へ並べる
        private void LoadIngredients()
        {
            contentArea.Controls.Clear();

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                using var cmd = new SqlCommand(
                    "SELECT ID, FOODNAME, DATELANE, GENRU, FOODCOUNT, UNIT FROM dbo.food_Table ORDER BY DATELANE ASC",
                    conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    string name = reader["FOODNAME"].ToString() ?? "";
                    DateTime expiry = Convert.ToDateTime(reader["DATELANE"]);
                    string category = reader["GENRU"].ToString() ?? "その他";
                    string count = reader["FOODCOUNT"].ToString() ?? "";
                    string unit = reader["UNIT"].ToString() ?? "";

                    int daysLeft = (expiry.Date - DateTime.Today).Days;
                    var (badgeColor, statusText) = StateFor(daysLeft);

                    contentArea.Controls.Add(MakeRow(id, EmojiFor(category), $"{name}（{count}{unit}）", statusText, badgeColor));
                }
            }
            catch (Exception ex)
            {
                // DB接続に失敗した場合（自分のPCにSQL Serverが無い等）はエラー内容を画面に出す
                var errLabel = new Label
                {
                    Text = $"DB接続エラー: {ex.Message}",
                    Font = new Font("Yu Gothic UI", 9F),
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                contentArea.Controls.Add(errLabel);
            }
        }

        // 「廃棄」ボタンの処理：確認のうえ、food_Tableから該当行を削除する
        // （図鑑への登録はしない。図鑑登録の仕様は別途相談してから対応する）
        private void DeleteIngredient(int id, string name)
        {
            var result = MessageBox.Show(
                $"「{name}」を廃棄しますか？（在庫一覧から削除されます）",
                "廃棄の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                using var cmd = new SqlCommand("DELETE FROM dbo.food_Table WHERE ID = @ID", conn);
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"廃棄に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadIngredients(); // 一覧を最新の状態に読み直す
        }

        // 1件ぶんの行（Panel）を組み立てる
        Panel MakeRow(int id, string emoji, string name, string statusText, Color statusColor)
        {
            Panel rowPanel = new Panel();
            rowPanel.Size = new Size(850, 60);
            rowPanel.BackColor = Color.White;
            rowPanel.Margin = new Padding(10, 5, 10, 5);

            // 🎨 絵文字ラベル
            Label iconLabel = new Label();
            iconLabel.Text = emoji;
            iconLabel.Font = new Font("Segoe UI Emoji", 18);
            iconLabel.Location = new Point(10, 15);
            iconLabel.AutoSize = true;

            // 🏷️ 名前ラベル
            Label nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.Font = new Font("Yu Gothic UI", 12, FontStyle.Bold);
            nameLabel.Location = new Point(80, 18);
            nameLabel.AutoSize = true;

            // ⏳ 状態バッジ風のラベル
            Label statusLabel = new Label();
            statusLabel.Text = statusText;
            statusLabel.Font = new Font("Yu Gothic UI", 10);
            statusLabel.BackColor = statusColor;
            statusLabel.Location = new Point(280, 18);
            statusLabel.AutoSize = true;
            statusLabel.Padding = new Padding(5);

            // 🛠️ [編集] ボタン（要相談のため、今回は表示のみ。DB更新は未実装）
            Button editButton = new Button();
            editButton.Text = "編集";
            editButton.Location = new Point(600, 15);
            editButton.Size = new Size(75, 30);

            // 🗑️ [廃棄] ボタン：確認のうえ、food_Tableから該当行を削除する（図鑑への登録は行わない）
            Button deleteButton = new Button();
            deleteButton.Text = "廃棄";
            deleteButton.Location = new Point(690, 15);
            deleteButton.Size = new Size(75, 30);
            deleteButton.Click += (s, e) => DeleteIngredient(id, name);

            rowPanel.Controls.Add(iconLabel);
            rowPanel.Controls.Add(nameLabel);
            rowPanel.Controls.Add(statusLabel);
            rowPanel.Controls.Add(editButton);
            rowPanel.Controls.Add(deleteButton);

            return rowPanel;
        }
    }
}
