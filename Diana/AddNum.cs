using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diana
{
    public partial class AddNum : Form
    {
        public AddNum()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            button1.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
