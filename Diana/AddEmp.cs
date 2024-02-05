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
    public partial class AddEmp : Form
    {
        public AddEmp()
        {
            InitializeComponent();
        }

        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();

        private void AddEmp_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            cnn.Close();
            //Добавление значений в список отделов
            fillComBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void fillComBox()
        {
            cnn.Open();
            using (SqlCommand cmd = new SqlCommand("select * from Отдел", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    comboBox1.Items.Add(reader2[1]);
                }
                reader2.Close();
            }
            cnn.Close();
        }
        public string GetId()
        {
            cnn.Open();
            cmd.CommandText = "select * from Должности where название = @pn";
            cmd.Parameters.AddWithValue("@pn", (string)listBox1.SelectedItem);
            SqlDataReader reader3 = cmd.ExecuteReader();
            string dol = "";
            while (reader3.Read())
            {
                dol = reader3[0].ToString();
            }
            reader3.Close();
            cnn.Close();
            return dol;
        }

        public void ControlLists(int idot, string dolname)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            cmd.CommandText = "select * from Отдел where id_отдела = @p";
            cmd.Parameters.AddWithValue("@p", idot);
            SqlDataReader reader3 = cmd.ExecuteReader();
            string name = "";
            while (reader3.Read())
            {
                name = reader3[1].ToString();
            }
            cnn.Close();
            fillComBox();
            comboBox1.SelectedItem = name;
            reader3.Close();
            listBox1.SelectedItem = dolname;
        }

        public void changeIt()
        {
            cnn.Open();
            cmd.CommandText = "select * from Отдел where название = @nam";
            cmd.Parameters.AddWithValue("@nam", comboBox1.SelectedItem.ToString());
            SqlDataReader reader1 = cmd.ExecuteReader();
            string id = "";
            while (reader1.Read())
            {
                id = reader1[0].ToString();
            }
            reader1.Close();

            cmd.CommandText = "select * from Должности where id_отдела = @id";
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            SqlDataReader reader = cmd.ExecuteReader();
            string result = "";
            while (reader.Read())
            {
                result = String.Format("{0}", reader[2]);
                listBox1.Items.Add(result.ToString());
            }
            reader.Close();
            cnn.Close();
        }


        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            changeIt();
        }
    }
}
