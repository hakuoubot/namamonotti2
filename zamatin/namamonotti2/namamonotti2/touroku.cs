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
            // 初期表示：カテゴリ・単位は先頭を選択、賞味期限は「今日+7日」を初期値にしておく
            categoryBox.SelectedIndex = 0;
            unitBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Today.AddDays(7);
        }

        // MainForm(シェル)から生成されるときに呼ばれるコンストラクタ
        public touroku(MainForm main) : this()
        {
            _main = main;
        }

        // 「登録する」ボタンの処理：入力チェック → 確認メッセージ表示 → 入力欄をクリア
        private void saveBtn_Click(object sender, EventArgs e)
        {
            // 必須項目チェック①：商品名が空なら止める
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                MessageBox.Show("商品名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 必須項目チェック②：カテゴリが選ばれていなければ止める
            if (categoryBox.SelectedIndex < 0)
            {
                MessageBox.Show("カテゴリを選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // DB接続は未実装（チームのデータ層決定待ち）。入力内容の確認のみ行う。
            // ここで各入力欄の値を取り出しておく（DBが決まったら、この値をそのままStockItemに詰めて保存する想定）
            string name = nameBox.Text.Trim();
            string category = categoryBox.SelectedItem?.ToString() ?? "";
            decimal qty = qtyBox.Value;
            string unit = unitBox.SelectedItem?.ToString() ?? "";
            DateTime expiry = datePicker.Value;

            // 登録内容をポップアップで確認表示（本来はここでDB保存処理を呼ぶ）
            MessageBox.Show(
                $"「{name}」（{category}）を {qty}{unit}、賞味期限 {expiry:yyyy-MM-dd} で登録しました！",
                "登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 次の入力がしやすいよう、フォームを初期状態に戻してカーソルを商品名欄に戻す
            nameBox.Clear();
            categoryBox.SelectedIndex = 0;
            qtyBox.Value = 1;
            unitBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Today.AddDays(7);
            nameBox.Focus();
        }
    }
}
