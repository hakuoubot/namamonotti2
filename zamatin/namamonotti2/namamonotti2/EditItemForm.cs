using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace namamonotti2
{
    // 在庫1件を編集するためのダイアログ。
    // 呼び出し側でIDを保持しておき、「更新する」を押すとその場でfood_TableをUPDATEしてから閉じる。
    public class EditItemForm : Form
    {
        readonly int _id;

        readonly TextBox _nameBox = new() { Font = new Font("Yu Gothic UI", 10F) };
        readonly ComboBox _categoryBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Yu Gothic UI", 10F) };
        readonly NumericUpDown _qtyBox = new() { Minimum = 0, Maximum = 9999, Font = new Font("Yu Gothic UI", 10F) };
        readonly ComboBox _unitBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Yu Gothic UI", 10F) };
        readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Short, Font = new Font("Yu Gothic UI", 10F) };

        public EditItemForm(int id, string name, string category, decimal qty, string unit, DateTime expiry)
        {
            _id = id;

            Text = "在庫を編集";
            Width = 360;
            Height = 320;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            _categoryBox.Items.AddRange(new object[] { "肉", "魚", "野菜", "乳製品", "卵", "その他" });
            _unitBox.Items.AddRange(new object[] { "個", "本", "g", "ml", "袋", "枚" });

            _nameBox.Text = name;
            _categoryBox.SelectedItem = category;
            if (_categoryBox.SelectedIndex < 0) _categoryBox.SelectedIndex = 0;
            _qtyBox.Value = Math.Clamp(qty, _qtyBox.Minimum, _qtyBox.Maximum);
            _unitBox.SelectedItem = unit;
            if (_unitBox.SelectedIndex < 0) _unitBox.SelectedIndex = 0;
            _datePicker.Value = expiry;

            var lblName = new Label { Text = "商品名", AutoSize = true, Location = new Point(16, 16) };
            _nameBox.SetBounds(16, 36, 310, 25);

            var lblCategory = new Label { Text = "カテゴリ", AutoSize = true, Location = new Point(16, 68) };
            _categoryBox.SetBounds(16, 88, 310, 25);

            var lblQty = new Label { Text = "数量", AutoSize = true, Location = new Point(16, 120) };
            _qtyBox.SetBounds(16, 140, 100, 25);
            _unitBox.SetBounds(126, 140, 100, 25);

            var lblExpiry = new Label { Text = "賞味期限", AutoSize = true, Location = new Point(16, 172) };
            _datePicker.SetBounds(16, 192, 310, 25);

            var okButton = new Button { Text = "更新する", DialogResult = DialogResult.OK };
            okButton.SetBounds(16, 230, 145, 36);
            okButton.Click += OkButton_Click;

            var cancelButton = new Button { Text = "キャンセル", DialogResult = DialogResult.Cancel };
            cancelButton.SetBounds(181, 230, 145, 36);

            Controls.AddRange(new Control[]
            {
                lblName, _nameBox, lblCategory, _categoryBox,
                lblQty, _qtyBox, _unitBox, lblExpiry, _datePicker,
                okButton, cancelButton
            });

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        // 「更新する」ボタンの処理：入力チェック→DB更新。失敗時はダイアログを閉じずに再入力させる
        void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nameBox.Text))
            {
                MessageBox.Show("商品名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                using var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["wiz"].ConnectionString);
                using var cmd = new SqlCommand(
                    "UPDATE dbo.food_Table SET FOODNAME=@FOODNAME, GENRU=@GENRU, FOODCOUNT=@FOODCOUNT, UNIT=@UNIT, DATELANE=@DATELANE WHERE ID=@ID",
                    conn);
                cmd.Parameters.Add("@FOODNAME", SqlDbType.NVarChar).Value = _nameBox.Text.Trim();
                cmd.Parameters.Add("@GENRU", SqlDbType.NVarChar).Value = _categoryBox.SelectedItem?.ToString() ?? "";
                cmd.Parameters.Add("@FOODCOUNT", SqlDbType.Int).Value = (int)_qtyBox.Value;
                cmd.Parameters.Add("@UNIT", SqlDbType.NVarChar).Value = _unitBox.SelectedItem?.ToString() ?? "";
                cmd.Parameters.Add("@DATELANE", SqlDbType.Date).Value = _datePicker.Value;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _id;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
