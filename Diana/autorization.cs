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
    public partial class autorization : Form
    {
        public autorization()
        {
            InitializeComponent();
        }

        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();

        private void button1_Click(object sender, EventArgs e)
        {

            cnn.Open();
            using (SqlCommand cmd = new SqlCommand("select count(*) from Пользователь where почта = @log and пароль = @pr", cnn))
            {
                cmd.Parameters.AddWithValue("@log", textBox1.Text);
                cmd.Parameters.AddWithValue("@pr", textBox2.Text);
                if (((int)cmd.ExecuteScalar()) != 0)
                {
                    MessageBox.Show(String.Format("Добро пожаловать!"));
                    SalesD f = new SalesD(textBox1.Text);
                    f.Show();
                    this.Hide();
                }
                else
                {
                    cnn.Close();
                    MessageBox.Show(String.Format("Неверно введенный пароль!"));
                    textBox1.Clear();
                    textBox2.Clear();
                    cnn.Open();
                }
            }
            cnn.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            cnn.Close();
        }
    }
}
