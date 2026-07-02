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
    public partial class touroku : UserControl
    {
        readonly MainForm? _main;

        public touroku()
        {
            InitializeComponent();
            categoryBox.SelectedIndex = 0;
            unitBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Today.AddDays(7);
        }

        public touroku(MainForm main) : this()
        {
            _main = main;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                MessageBox.Show("商品名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (categoryBox.SelectedIndex < 0)
            {
                MessageBox.Show("カテゴリを選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // DB接続は未実装（チームのデータ層決定待ち）。入力内容の確認のみ行う。
            string name = nameBox.Text.Trim();
            string category = categoryBox.SelectedItem?.ToString() ?? "";
            decimal qty = qtyBox.Value;
            string unit = unitBox.SelectedItem?.ToString() ?? "";
            DateTime expiry = datePicker.Value;

            MessageBox.Show(
                $"「{name}」（{category}）を {qty}{unit}、賞味期限 {expiry:yyyy-MM-dd} で登録しました！",
                "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

            nameBox.Clear();
            categoryBox.SelectedIndex = 0;
            qtyBox.Value = 1;
            unitBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Today.AddDays(7);
            nameBox.Focus();
        }
    }
}
