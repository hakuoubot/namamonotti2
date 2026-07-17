using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace namamonotti2
{
    // レシピを「作った」後、実際に使った分量を選んで在庫に反映するためのダイアログ。
    // 在庫一覧を数量入力つきで表示し、レシピの使用食材に名前が近いものは在庫数を初期値にしておく。
    // 入力した数量が在庫数以上なら「成仏」として記録し、在庫数未満ならその分だけ在庫数を減らす（成仏にはしない）。
    public class CompleteRecipeForm : Form
    {
        readonly Panel _listPanel = new() { AutoScroll = true };
        readonly List<(int Id, decimal FoodCount, NumericUpDown Input)> _rows = [];

        public CompleteRecipeForm(string usedIngredients)
        {
            Text = "使った分量を選んで反映";
            Width = 420;
            Height = 420;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var hint = new Label
            {
                Text = "レシピで使った分量を入力してください（在庫数未満なら在庫数を減らすだけにします）。",
                AutoSize = false,
                Size = new Size(370, 32),
                Location = new Point(16, 10)
            };

            _listPanel.SetBounds(16, 44, 370, 256);
            _listPanel.BorderStyle = BorderStyle.FixedSingle;

            var usedNames = usedIngredients
                .Split([',', '、', '，'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                using var cmd = new SqlCommand(
                    "SELECT ID, FOODNAME, FOODCOUNT, UNIT FROM dbo.food_Table WHERE SITUATION IS NULL ORDER BY DATELANE ASC",
                    conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                int y = 4;
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    string name = reader["FOODNAME"].ToString() ?? "";
                    decimal foodCount = Convert.ToDecimal(reader["FOODCOUNT"]);
                    string unit = reader["UNIT"].ToString() ?? "";

                    bool matched = !string.IsNullOrEmpty(name) &&
                        usedNames.Any(u => !string.IsNullOrEmpty(u) && (name.Contains(u) || u.Contains(name)));

                    var nameLabel = new Label
                    {
                        Text = $"{name}（在庫 {foodCount}{unit}）",
                        AutoSize = false,
                        Size = new Size(220, 28),
                        Location = new Point(6, y),
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    bool isWhole = foodCount == Math.Floor(foodCount);
                    var qtyInput = new NumericUpDown
                    {
                        Minimum = 0,
                        Maximum = Math.Max(foodCount, 0),
                        DecimalPlaces = isWhole ? 0 : 2,
                        Increment = isWhole ? 1 : 0.5m,
                        Value = matched ? foodCount : 0,
                        Size = new Size(80, 28),
                        Location = new Point(232, y),
                        TextAlign = HorizontalAlignment.Right
                    };

                    var unitLabel = new Label
                    {
                        Text = unit,
                        AutoSize = false,
                        Size = new Size(40, 28),
                        Location = new Point(316, y),
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    _listPanel.Controls.AddRange(new Control[] { nameLabel, qtyInput, unitLabel });
                    _rows.Add((id, foodCount, qtyInput));
                    y += 32;
                }
            }
            catch (Exception ex)
            {
                _listPanel.Controls.Add(new Label
                {
                    Text = $"在庫の取得に失敗しました: {ex.Message}",
                    AutoSize = true,
                    ForeColor = Color.Red,
                    Location = new Point(6, 4)
                });
            }

            var okButton = new Button { Text = "反映する", DialogResult = DialogResult.OK };
            okButton.SetBounds(16, 310, 175, 36);
            okButton.Click += OkButton_Click;

            var cancelButton = new Button { Text = "キャンセル", DialogResult = DialogResult.Cancel };
            cancelButton.SetBounds(211, 310, 175, 36);

            Controls.AddRange(new Control[] { hint, _listPanel, okButton, cancelButton });
            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        // 「反映する」ボタンの処理：
        // 入力数量が在庫数以上 → SITUATIONを「成仏」に更新（図鑑にカウント）
        // 入力数量が在庫数未満 → FOODCOUNTをその分だけ減らす（在庫に残す、成仏にはしない）
        void OkButton_Click(object? sender, EventArgs e)
        {
            var used = _rows.Where(r => r.Input.Value > 0).ToList();

            if (used.Count == 0)
            {
                MessageBox.Show("使った食材の数量を1つ以上入力してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                conn.Open();

                using var completeCmd = new SqlCommand("UPDATE dbo.food_Table SET SITUATION = @SITUATION WHERE ID = @ID", conn);
                completeCmd.Parameters.Add("@SITUATION", SqlDbType.VarChar).Value = "成仏";
                var completeIdParam = completeCmd.Parameters.Add("@ID", SqlDbType.Int);

                using var reduceCmd = new SqlCommand("UPDATE dbo.food_Table SET FOODCOUNT = @FOODCOUNT WHERE ID = @ID", conn);
                var reduceCountParam = reduceCmd.Parameters.Add("@FOODCOUNT", SqlDbType.Decimal);
                var reduceIdParam = reduceCmd.Parameters.Add("@ID", SqlDbType.Int);

                foreach (var row in used)
                {
                    decimal usedQty = row.Input.Value;
                    if (usedQty >= row.FoodCount)
                    {
                        completeIdParam.Value = row.Id;
                        completeCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reduceCountParam.Value = row.FoodCount - usedQty;
                        reduceIdParam.Value = row.Id;
                        reduceCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"反映に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
