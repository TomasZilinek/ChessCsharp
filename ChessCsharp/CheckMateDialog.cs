using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    public partial class CheckMateDialog : Form
    {
        public CheckMateDialog(bool human_won, string winner_name)
        {
            InitializeComponent();

            titleLabel.Text = human_won ? "Victory" : "Defeat";
            victorLabel.Text = winner_name + " was victorious";
        }
    }
}
