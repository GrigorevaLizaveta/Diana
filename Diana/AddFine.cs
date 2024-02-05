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
    public partial class AddFine : Form
    {
        public AddFine()
        {
            InitializeComponent();
        }
        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        private void button1_Click(object sender, EventArgs e)
        {
            button1.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddFine_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;

            using (SqlCommand cmd = new SqlCommand("select * from Тип_штрафа", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    listBox1.Items.Add((reader2[1]).ToString());
                }
                reader2.Close();
            }

            cnn.Close();
        }
    }
}
