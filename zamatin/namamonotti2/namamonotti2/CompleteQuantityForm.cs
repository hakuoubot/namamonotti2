using System;
using System.Drawing;
using System.Windows.Forms;

namespace namamonotti2
{
    // 「成仏」ボタンから開く、使った分量を入力するための小さなダイアログ。
    // 入力数量は呼び出し側がUsedQtyプロパティで受け取り、在庫数以上か未満かで成仏/在庫減算を出し分ける。
    public class CompleteQuantityForm : Form
    {
        readonly NumericUpDown _qtyBox;

        public decimal UsedQty => _qtyBox.Value;

        public CompleteQuantityForm(string name, decimal foodCount, string unit)
        {
            Text = "成仏させる分量";
            Width = 320;
            Height = 200;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var hint = new Label
            {
                Text = $"「{name}」（在庫 {foodCount}{unit}）\n使った分量を入力してください。\n在庫数未満ならその分だけ在庫を減らします。",
                AutoSize = false,
                Size = new Size(270, 60),
                Location = new Point(16, 12)
            };

            bool isWhole = foodCount == Math.Floor(foodCount);
            _qtyBox = new NumericUpDown
            {
                Minimum = 0,
                Maximum = Math.Max(foodCount, 0),
                DecimalPlaces = isWhole ? 0 : 2,
                Increment = isWhole ? 1 : 0.5m,
                Value = foodCount,
                Font = new Font("Yu Gothic UI", 10F),
                TextAlign = HorizontalAlignment.Right
            };
            _qtyBox.SetBounds(16, 80, 100, 28);

            var unitLabel = new Label { Text = unit, AutoSize = true, Location = new Point(122, 84) };

            var okButton = new Button { Text = "成仏させる", DialogResult = DialogResult.OK };
            okButton.SetBounds(16, 118, 130, 36);

            var cancelButton = new Button { Text = "キャンセル", DialogResult = DialogResult.Cancel };
            cancelButton.SetBounds(154, 118, 130, 36);

            Controls.AddRange(new Control[] { hint, _qtyBox, unitLabel, okButton, cancelButton });
            AcceptButton = okButton;
            CancelButton = cancelButton;
        }
    }
}
