using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace namamonotti2
{
    public partial class touroku : UserControl
    {
        readonly MainForm? _main;

        // Open Food Facts への問い合わせ用（アプリ全体で使い回す）
        static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(8) };

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

        // 「📷 バーコードでスキャン」ボタンの処理：
        // カメラでバーコードを読み取り → Open Food Facts APIで商品名を検索 → 商品名欄に自動入力
        private async void photoScanBtn_Click(object sender, EventArgs e)
        {
            using var scanForm = new BarcodeScanForm();
            if (scanForm.ShowDialog(FindForm()) != DialogResult.OK || string.IsNullOrWhiteSpace(scanForm.ScannedCode))
                return;

            string code = scanForm.ScannedCode;
            photoScanBtn.Enabled = false;
            photoScanBtn.Text = "🔍 商品を検索中...";

            try
            {
                string? productName = await LookupBarcodeAsync(code);
                if (productName != null)
                {
                    nameBox.Text = productName;
                    nameBox.Focus();
                }
                else
                {
                    MessageBox.Show(
                        $"バーコード「{code}」の商品が見つかりませんでした。商品名を手入力してください。",
                        "商品が見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    nameBox.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"商品検索に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                photoScanBtn.Enabled = true;
                photoScanBtn.Text = "📷 バーコードでスキャン";
            }
        }

        // Open Food Facts APIにJANコードを問い合わせて商品名を取得する（見つからなければnull）
        static async Task<string?> LookupBarcodeAsync(string barcode)
        {
            string url = $"https://world.openfoodfacts.org/api/v2/product/{barcode}.json?fields=product_name,product_name_ja";
            using var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;

            using var stream = await res.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            if (!root.TryGetProperty("status", out var status) || status.GetInt32() != 1)
                return null;
            if (!root.TryGetProperty("product", out var product))
                return null;

            string? nameJa = product.TryGetProperty("product_name_ja", out var ja) ? ja.GetString() : null;
            string? name = product.TryGetProperty("product_name", out var en) ? en.GetString() : null;

            string? result = !string.IsNullOrWhiteSpace(nameJa) ? nameJa : name;
            return string.IsNullOrWhiteSpace(result) ? null : result;
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

            using (SqlConnection conn = new SqlConnection
                (ConfigurationManager.ConnectionStrings["wiz"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                StringBuilder sql=new StringBuilder();
                sql.Append(" INSERT INTO");
                sql.Append("     dbo.food_Table ");
                sql.Append(" (");
                sql.Append("     FOODNAME");
                sql.Append("    , DATELANE");
                sql.Append("    , DATE");
                sql.Append("    , GENRU");
                sql.Append("    , FOODCOUNT");
                sql.Append("    , UNIT");
                sql.Append(" )");
                sql.Append("VALUES ");
                sql.Append(" (");
                sql.Append("     @FOODNAME");
                sql.Append("    ,@DATELANE");
                sql.Append("    ,GETDATE()");
                sql.Append("    ,@GENRU");
                sql.Append("    ,@FOODCOUNT");
                sql.Append("    ,@UNIT");
                sql.Append(" )");

                cmd.CommandText = sql.ToString();

                cmd.Parameters.Add("@FOODNAME",SqlDbType.NVarChar).Value=nameBox.Text;
                cmd.Parameters.Add("@DATELANE",SqlDbType.Date).Value=datePicker.Value;
                cmd.Parameters.Add("@GENRU",SqlDbType.NVarChar).Value=categoryBox.Text;
                cmd.Parameters.Add("@FOODCOUNT",SqlDbType.NVarChar).Value = qtyBox.Text;
                cmd.Parameters.Add("@UNIT",SqlDbType.NVarChar).Value=unitBox.Text;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
