using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace namamonotti2
{
    public partial class zaiko : UserControl
    {
        readonly MainForm? _main;

        public zaiko()
        {
            InitializeComponent();
            LoadTestIngredients(); // 画面が開いたときにデータを読み込む
        }

        // MainForm(シェル)から生成されるときに呼ばれるコンストラクタ
        public zaiko(MainForm main) : this()
        {
            _main = main;
        }

        // テスト用のなまものっちデータを生成して画面に並べる
        private void LoadTestIngredients()
        {
            // 1. 本来はデータベースから取るデータを、仮で3つ用意
            var testData = new List<Tuple<string, string, string, Color>>()
            {
                Tuple.Create("🐟", "魚くん", "残り0日（危険）", Color.LightPink),
                Tuple.Create("🥩", "肉くん", "残り2日（心配）", Color.LightYellow),
                Tuple.Create("🥬", "野菜ちゃん", "残り8日（元気）", Color.LightGreen)
            };

            // 2. データの数だけ、1行ずつ「Panel（横長の枠）」を作って流し込む
            foreach (var item in testData)
            {
                // 横長の行（Panel）を作成
                Panel rowPanel = new Panel();
                rowPanel.Size = new Size(850, 60); // 横幅800、高さ60
                rowPanel.BackColor = Color.White;  // 背景は白
                rowPanel.Margin = new Padding(10, 5, 10, 5); // 上下に少し隙間をあける


                // 🎨 絵文字ラベル
                Label iconLabel = new Label();
                iconLabel.Text = item.Item1;
                iconLabel.Font = new Font("Segoe UI Emoji", 18);
                iconLabel.Location = new Point(10, 15);
                iconLabel.AutoSize = true;

                // 🏷️ 名前ラベル
                Label nameLabel = new Label();
                nameLabel.Text = item.Item2;
                nameLabel.Font = new Font("Yu Gothic UI", 12, FontStyle.Bold);
                nameLabel.Location = new Point(80, 18);
                nameLabel.AutoSize = true;

                // ⏳ 状態バッジ風のラベル
                Label statusLabel = new Label();
                statusLabel.Text = item.Item3;
                statusLabel.Font = new Font("Yu Gothic UI", 10);
                statusLabel.BackColor = item.Item4; // 状態に合わせた色
                statusLabel.Location = new Point(200, 18);
                statusLabel.AutoSize = true;
                statusLabel.Padding = new Padding(5);

                // 🛠️ [編集] ボタン
                Button editButton = new Button();
                editButton.Text = "編集";
                editButton.Location = new Point(500, 15);
                editButton.Size = new Size(75, 30);

                // 🗑️ [廃棄] ボタン
                Button deleteButton = new Button();
                deleteButton.Text = "廃棄";
                deleteButton.Location = new Point(590, 15);
                deleteButton.Size = new Size(75, 30);

                // 行（Panel）にパーツを全部詰め込む
                rowPanel.Controls.Add(iconLabel);
                rowPanel.Controls.Add(nameLabel);
                rowPanel.Controls.Add(statusLabel);
                rowPanel.Controls.Add(editButton);
                rowPanel.Controls.Add(deleteButton);

                // 最後に、デザイナーで作った「contentArea」に行を追加！
                contentArea.Controls.Add(rowPanel);
            }
        }

        private void contentArea_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
