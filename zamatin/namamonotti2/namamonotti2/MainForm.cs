namespace namamonotti2
{
    public partial class MainForm : Form
    {
        UserControl? _currentPanel;
        Button[] _navButtons;
        string[] _screens = ["home", "add", "stock", "recipe", "dex"];
        string[] _screenTitles = ["ホーム（キャラ部屋）", "在庫とうろく", "ざいこ いちらん", "りょうり こうほ・レシピ", "ずかん"];

        public MainForm()
        {
            InitializeComponent();

            _navButtons = [btnHome, btnAdd, btnStock, btnRecipe, btnDex];
            for (int i = 0; i < _navButtons.Length; i++)
            {
                int idx = i;
                _navButtons[i].Click += (s, e) => NavigateTo(_screens[idx]);
            }

            NavigateTo("home");
        }

        public void NavigateTo(string screen)
        {
            _currentPanel?.Dispose();
            mainArea.Controls.Clear();

            int idx = Array.IndexOf(_screens, screen);
            if (idx >= 0) titleLabel.Text = _screenTitles[idx];

            for (int i = 0; i < _navButtons.Length; i++)
            {
                bool active = _screens[i] == screen;
                _navButtons[i].BackColor = active ? Color.White : Color.FromArgb(30, 255, 255, 255);
                _navButtons[i].ForeColor = active ? Color.FromArgb(239, 110, 152) : Color.White;
                _navButtons[i].Font = new Font("Yu Gothic UI", 10F, active ? FontStyle.Bold : FontStyle.Regular);
            }

            UserControl panel = screen switch
            {
                "home" => new home(this),
                "add" => new touroku(this),
                //"zaiko"  => new zaiko(this),// もしくは new zaiko(なんらかの文字);
                "recipe" => new recipi(this),
                "dex" => new zukan(this),
                _ => new home(this)
            };

            panel.Dock = DockStyle.Fill;
            mainArea.Controls.Add(panel);
            _currentPanel = panel;
        }
    }
}
