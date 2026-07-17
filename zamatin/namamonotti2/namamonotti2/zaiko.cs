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
                // SITUATIONが入っている行は「成仏」済み（図鑑にカウント済み）なので在庫一覧には出さない
                using var cmd = new SqlCommand(
                    "SELECT ID, FOODNAME, DATELANE, GENRU, FOODCOUNT, UNIT FROM dbo.food_Table WHERE SITUATION IS NULL ORDER BY DATELANE ASC",
                    conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    string name = reader["FOODNAME"].ToString() ?? "";
                    DateTime expiry = Convert.ToDateTime(reader["DATELANE"]);
                    string category = reader["GENRU"].ToString() ?? "その他";
                    decimal count = Convert.ToDecimal(reader["FOODCOUNT"]);
                    string unit = reader["UNIT"].ToString() ?? "";

                    int daysLeft = (expiry.Date - DateTime.Today).Days;
                    var (badgeColor, statusText) = StateFor(daysLeft);

                    contentArea.Controls.Add(MakeRow(id, EmojiFor(category), name, category, count, unit, expiry, statusText, badgeColor, daysLeft));
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

        // 「編集」ボタンの処理：編集用ダイアログを開き、更新されたら一覧を読み直す
        private void EditIngredient(int id, string name, string category, decimal count, string unit, DateTime expiry)
        {
            using var form = new EditItemForm(id, name, category, count, unit, expiry);
            if (form.ShowDialog(FindForm()) == DialogResult.OK)
                LoadIngredients();
        }

        // 「成仏」ボタンの処理：使った分量を入力してもらい、
        // 在庫数以上ならSITUATIONを「成仏」に記録（図鑑にカウント）、
        // 在庫数未満ならその分だけFOODCOUNTを減らすだけ（在庫に残す、成仏にはしない）
        private void CompleteIngredient(int id, string name, decimal foodCount, string unit)
        {
            using var form = new CompleteQuantityForm(name, foodCount, unit);
            if (form.ShowDialog(FindForm()) != DialogResult.OK) return;

            decimal usedQty = form.UsedQty;
            if (usedQty <= 0) return;

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                conn.Open();

                if (usedQty >= foodCount)
                {
                    using var cmd = new SqlCommand("UPDATE dbo.food_Table SET SITUATION = @SITUATION WHERE ID = @ID", conn);
                    cmd.Parameters.Add("@SITUATION", SqlDbType.VarChar).Value = "成仏";
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    using var cmd = new SqlCommand("UPDATE dbo.food_Table SET FOODCOUNT = @FOODCOUNT WHERE ID = @ID", conn);
                    cmd.Parameters.Add("@FOODCOUNT", SqlDbType.Decimal).Value = foodCount - usedQty;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"成仏の記録に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadIngredients(); // 一覧を最新の状態に読み直す
        }

        // 「削除」ボタンの処理：確認のうえ、food_TableのSITUATIONに「廃棄」と記録する（行は削除しない）
        // （成仏とは違い在庫一覧からは消える。図鑑の「救済率」の分母としてはカウントされる）
        private void DeleteIngredient(int id, string name)
        {
            var result = MessageBox.Show(
                $"「{name}」を削除しますか？（在庫一覧から削除されます。成仏としてはカウントされません）",
                "削除の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                using var cmd = new SqlCommand("UPDATE dbo.food_Table SET SITUATION = @SITUATION WHERE ID = @ID", conn);
                cmd.Parameters.Add("@SITUATION", SqlDbType.VarChar).Value = "廃棄";
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadIngredients(); // 一覧を最新の状態に読み直す
        }

        // 1件ぶんの行（Panel）を組み立てる
        Panel MakeRow(int id, string emoji, string name, string category, decimal count, string unit, DateTime expiry, string statusText, Color statusColor, int daysLeft)
        {
            Panel rowPanel = new Panel();
            rowPanel.Size = new Size(850, 76);
            rowPanel.BackColor = Color.White;
            rowPanel.Margin = new Padding(10, 5, 10, 5);

            // 🖼️ ドット絵アイコン（図鑑と同じくパターン1固定。ただし期限切れならカテゴリ問わずゾンビキャラを表示）
            string charKey = daysLeft < 0 ? "ゾンビ" : category;
            PictureBox iconImage = new PictureBox();
            iconImage.Image = PixelArt.Render(PixelArt.GetFirstPattern(charKey), 1, Color.Black, Color.White);
            iconImage.SizeMode = PictureBoxSizeMode.Zoom;
            iconImage.Size = new Size(56, 56);
            iconImage.Location = new Point(10, 10);
            iconImage.BackColor = Color.Transparent;

            // 🏷️ 名前ラベル
            Label nameLabel = new Label();
            nameLabel.Text = $"{name}（{count}{unit}）";
            nameLabel.Font = new Font("Yu Gothic UI", 12, FontStyle.Bold);
            nameLabel.Location = new Point(76, 26);
            nameLabel.AutoSize = true;

            // ⏳ 状態バッジ風のラベル
            Label statusLabel = new Label();
            statusLabel.Text = statusText;
            statusLabel.Font = new Font("Yu Gothic UI", 10);
            statusLabel.BackColor = statusColor;
            statusLabel.Location = new Point(280, 26);
            statusLabel.AutoSize = true;
            statusLabel.Padding = new Padding(5);

            // 🛠️ [編集] ボタン：編集ダイアログを開いてfood_Tableを更新する
            Button editButton = new Button();
            editButton.Text = "編集";
            editButton.Location = new Point(510, 18);
            editButton.Size = new Size(75, 40);
            editButton.Click += (s, e) => EditIngredient(id, name, category, count, unit, expiry);

            // 🙏 [成仏] ボタン：確認のうえ、SITUATIONに記録して使い切ったことを残す（図鑑にカウント）
            Button completeButton = new Button();
            completeButton.Text = "成仏";
            completeButton.Location = new Point(600, 18);
            completeButton.Size = new Size(75, 40);
            completeButton.Click += (s, e) => CompleteIngredient(id, name, count, unit);

            // 🗑️ [削除] ボタン：確認のうえ、food_Tableから該当行を完全に削除する（図鑑への登録は行わない）
            Button deleteButton = new Button();
            deleteButton.Text = "削除";
            deleteButton.Location = new Point(690, 18);
            deleteButton.Size = new Size(75, 40);
            deleteButton.Click += (s, e) => DeleteIngredient(id, name);

            rowPanel.Controls.Add(iconImage);
            rowPanel.Controls.Add(nameLabel);
            rowPanel.Controls.Add(statusLabel);
            rowPanel.Controls.Add(editButton);
            rowPanel.Controls.Add(completeButton);
            rowPanel.Controls.Add(deleteButton);

            return rowPanel;
        }
    }
}
