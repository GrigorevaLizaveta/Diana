using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diana
{
    public partial class SalesD : Form
    {
        public SalesD(string log)
        {
            InitializeComponent();
            Log = log;
        }

        string Log;
        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        int id;

        //Договора
        private void button1_Click(object sender, EventArgs e)
        {
            contracts f = new contracts(id);
            f.Show();
            this.Hide();
        }
        // сотрудники
        private void button3_Click(object sender, EventArgs e)
        {

            emploers f = new emploers();
            f.Show();
            this.Hide();
        }

        //клиенты
        private void button2_Click(object sender, EventArgs e)
        {
            clients f = new clients();
            f.Show();
            this.Hide();
        }

        //номера
        private void button5_Click(object sender, EventArgs e)
        {
            nums f = new nums();
            f.Show();
            this.Hide();
        }

        private void SalesD_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            cmd.CommandText = "select * from Сотрудники where почта = @pr";
            cmd.Parameters.AddWithValue("@pr", Log);
            SqlDataReader reader = cmd.ExecuteReader();
            string name = "";
            string sname = "";
            while (reader.Read())
            {
                name = String.Format("{0}", reader[2]);
                sname = String.Format("{0}", reader[3]);
                id = Convert.ToInt32(reader[0]);
            }
            textBox1.Text = name;
            textBox2.Text = sname;
            reader.Close();
            cmd.CommandText = "select Отдел.название from Отдел, Сотрудники, Должности where почта = @login and Должности.id_должности = Сотрудники.id_должности  and Отдел.id_отдела = Должности.id_отдела ";
            cmd.Parameters.AddWithValue("@login", Log);
            SqlDataReader reader2 = cmd.ExecuteReader();
            string otd = "";
            while (reader2.Read())
            {
                otd = String.Format("{0}", reader2[0]);
            }
            label4.Text = otd;
            reader2.Close();

            if (label4.Text == "Отдел продаж")
            {
                button1.Visible = true;
                button5.Visible = true;
                button2.Visible = true;
                button3.Visible = false;
            }
            if (label4.Text == "Отдел кадров")
            {
                button1.Visible = false;
                button5.Visible = false;
                button2.Visible = false;
                button3.Visible = true;
            }
            if (label4.Text == "Тех. отдел")
            {
                button1.Visible = true;
                button5.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
            }
        }
    }
}
