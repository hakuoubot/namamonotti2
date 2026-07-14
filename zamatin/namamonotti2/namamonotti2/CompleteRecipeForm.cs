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
    // レシピを「作った」後、実際に使った食材を選んで成仏させるためのダイアログ。
    // 在庫一覧をチェックボックス付きで表示し、レシピの使用食材に名前が近いものは事前にチェックしておく。
    public class CompleteRecipeForm : Form
    {
        readonly CheckedListBox _list = new() { Font = new Font("Yu Gothic UI", 10F), CheckOnClick = true, IntegralHeight = false };
        readonly List<int> _itemIds = [];

        public CompleteRecipeForm(string usedIngredients)
        {
            Text = "使った食材を選んで成仏";
            Width = 380;
            Height = 420;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var hint = new Label
            {
                Text = "レシピで使った食材にチェックを入れてください。",
                AutoSize = true,
                Location = new Point(16, 12)
            };

            _list.SetBounds(16, 40, 330, 260);

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
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    string name = reader["FOODNAME"].ToString() ?? "";
                    string count = reader["FOODCOUNT"].ToString() ?? "";
                    string unit = reader["UNIT"].ToString() ?? "";

                    _itemIds.Add(id);
                    int index = _list.Items.Add($"{name}（{count}{unit}）");

                    bool preChecked = !string.IsNullOrEmpty(name) &&
                        usedNames.Any(u => !string.IsNullOrEmpty(u) && (name.Contains(u) || u.Contains(name)));
                    if (preChecked)
                        _list.SetItemChecked(index, true);
                }
            }
            catch (Exception ex)
            {
                _list.Items.Add($"在庫の取得に失敗しました: {ex.Message}");
            }

            var okButton = new Button { Text = "成仏させる", DialogResult = DialogResult.OK };
            okButton.SetBounds(16, 310, 155, 36);
            okButton.Click += OkButton_Click;

            var cancelButton = new Button { Text = "キャンセル", DialogResult = DialogResult.Cancel };
            cancelButton.SetBounds(191, 310, 155, 36);

            Controls.AddRange(new Control[] { hint, _list, okButton, cancelButton });
            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        // 「成仏させる」ボタンの処理：チェックの入った行だけSITUATIONを更新する
        void OkButton_Click(object? sender, EventArgs e)
        {
            var checkedIds = _list.CheckedIndices.Cast<int>()
                .Where(i => i < _itemIds.Count)
                .Select(i => _itemIds[i])
                .ToList();

            if (checkedIds.Count == 0)
            {
                MessageBox.Show("成仏させる食材を1つ以上選んでください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                using var cmd = new SqlCommand("UPDATE dbo.food_Table SET SITUATION = @SITUATION WHERE ID = @ID", conn);
                cmd.Parameters.Add("@SITUATION", SqlDbType.VarChar).Value = "成仏";
                var idParam = cmd.Parameters.Add("@ID", SqlDbType.Int);

                conn.Open();
                foreach (var id in checkedIds)
                {
                    idParam.Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"成仏の記録に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
