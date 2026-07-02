namespace namamonotti2
{
    public partial class home : UserControl
    {
        readonly MainForm? _main;

        public home()
        {
            InitializeComponent();
        }

        public home(MainForm main) : this()
        {
            _main = main;
        }
    }
}
