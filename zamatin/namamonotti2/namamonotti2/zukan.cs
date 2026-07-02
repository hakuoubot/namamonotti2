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
    public partial class zukan : UserControl
    {
        readonly MainForm? _main;

        public zukan()
        {
            InitializeComponent();
        }

        public zukan(MainForm main) : this()
        {
            _main = main;
        }
    }
}
