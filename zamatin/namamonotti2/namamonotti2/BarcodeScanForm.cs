using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace namamonotti2
{
    // Webカメラを使ってバーコードを読み取るためのダイアログ。
    // 読み取りに成功すると ScannedCode に値をセットして DialogResult.OK で閉じる。
    // カメラでの自動認識に加えて、うまく読めない場合のために手入力での代替手段も用意している。
    public class BarcodeScanForm : Form
    {
        readonly PictureBox _preview = new() { Dock = DockStyle.Fill, BackColor = Color.Black, SizeMode = PictureBoxSizeMode.Zoom };
        readonly Label _statusLabel = new() { Dock = DockStyle.Bottom, Height = 32, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Yu Gothic UI", 10F), Text = "バーコードを画面いっぱいに、まっすぐ映してください" };
        readonly Panel _manualPanel = new() { Dock = DockStyle.Bottom, Height = 40 };
        readonly TextBox _manualBox = new() { Dock = DockStyle.Left, Width = 300, Font = new Font("Yu Gothic UI", 10F), PlaceholderText = "またはバーコード番号を直接入力" };
        readonly Button _manualOkBtn = new() { Dock = DockStyle.Right, Width = 90, Text = "この番号で" };
        readonly Button _cancelBtn = new() { Dock = DockStyle.Bottom, Height = 40, Text = "キャンセル" };
        readonly System.Windows.Forms.Timer _timer = new() { Interval = 200 };

        VideoCapture? _capture;
        readonly BarcodeReader _reader = new()
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new[] { BarcodeFormat.EAN_13, BarcodeFormat.EAN_8, BarcodeFormat.UPC_A, BarcodeFormat.UPC_E, BarcodeFormat.CODE_128 }
            }
        };

        public string? ScannedCode { get; private set; }

        public BarcodeScanForm()
        {
            Text = "バーコードスキャン";
            Width = 520;
            Height = 480;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            _manualPanel.Controls.Add(_manualBox);
            _manualPanel.Controls.Add(_manualOkBtn);

            Controls.Add(_preview);
            Controls.Add(_statusLabel);
            Controls.Add(_manualPanel);
            Controls.Add(_cancelBtn);

            _cancelBtn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            _manualOkBtn.Click += (s, e) => AcceptManualCode();
            _manualBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    AcceptManualCode();
                }
            };
            _timer.Tick += Timer_Tick;
            Load += BarcodeScanForm_Load;
            FormClosing += (s, e) => StopCamera();
        }

        void AcceptManualCode()
        {
            string code = _manualBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(code)) return;

            ScannedCode = code;
            StopCamera();
            DialogResult = DialogResult.OK;
            Close();
        }

        void BarcodeScanForm_Load(object? sender, EventArgs e)
        {
            try
            {
                _capture = new VideoCapture(0);
                if (!_capture.IsOpened())
                {
                    _statusLabel.Text = "カメラを開けませんでした。番号を直接入力してください。";
                    return;
                }

                // 解像度が低いとバーコードの縞模様が潰れて認識できないため、できるだけ高解像度で取得する
                _capture.Set(VideoCaptureProperties.FrameWidth, 1920);
                _capture.Set(VideoCaptureProperties.FrameHeight, 1080);

                _timer.Start();
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"カメラの初期化に失敗しました: {ex.Message}";
            }
        }

        void Timer_Tick(object? sender, EventArgs e)
        {
            if (_capture is null || !_capture.IsOpened()) return;

            using var frame = new Mat();
            if (!_capture.Read(frame) || frame.Empty()) return;

            // 認識精度を上げるため、グレースケール化してからデコードに渡す
            using var gray = new Mat();
            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

            using var previewBmp = frame.ToBitmap();
            _preview.Image?.Dispose();
            _preview.Image = (Bitmap)previewBmp.Clone();

            using var grayBmp = gray.ToBitmap();

            try
            {
                var result = _reader.Decode(grayBmp);
                if (result != null && !string.IsNullOrWhiteSpace(result.Text))
                {
                    ScannedCode = result.Text;
                    _statusLabel.Text = $"読み取り成功: {result.Text}";
                    StopCamera();
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch
            {
                // 1フレームのデコード失敗は無視して次のフレームへ
            }
        }

        void StopCamera()
        {
            _timer.Stop();
            _capture?.Release();
            _capture?.Dispose();
            _capture = null;
        }
    }
}
