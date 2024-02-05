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
    public partial class clients : Form
    {
        public clients()
        {
            InitializeComponent();
        }

        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();
        BindingSource bind = new BindingSource();

        private void clients_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            da.SelectCommand = new SqlCommand("select* from Клиент", cnn);
            da.Fill(ds, "Клиент");
            dataGridView1.DataSource = ds.Tables["Клиент"];
            //Чтение списка названий столбцов таблицы в comboB
            foreach (DataColumn col in ds.Tables["Клиент"].Columns)
            {
                comboBox1.Items.Add(col.ColumnName);
            }
            //Установка значений по умолчанию
            comboBox1.SelectedItem = "id_клиента";
            ds.Tables["Клиент"].DefaultView.Sort = "id_клиента";
            //Связывание сеток с 2-мя различными представлениями таблицы
            dataGridView1.DataSource = ds.Tables["Клиент"].DefaultView;
            //Программируем адаптер на вставку новых данных
            da.InsertCommand = new SqlCommand("insert into Клиент values ( @id, @name, @fio, @sname, @nom, @login)", cnn);
            da.InsertCommand.Parameters.Add("@id", SqlDbType.Int, 1000000, "id_клиента");
            da.InsertCommand.Parameters.Add("@name", SqlDbType.VarChar, 50, "Имя");
            da.InsertCommand.Parameters.Add("@fio", SqlDbType.VarChar, 50, "Фамилия");
            da.InsertCommand.Parameters.Add("@sname", SqlDbType.VarChar, 50, "Отчество");
            da.InsertCommand.Parameters.Add("@nom", SqlDbType.VarChar, 50, "Телефон");
            da.InsertCommand.Parameters.Add("@login", SqlDbType.VarChar, 50, "Почта");
            bind.DataSource = ds.Tables["Клиент"];
        }

        //назад
        private void button14_Click(object sender, EventArgs e)
        {
            this.Close();
            Form ifrm = Application.OpenForms[1];
            ifrm.Show();
        }

        //поиск
        private void button5_Click(object sender, EventArgs e)
        {
            string filter = String.Format("{0}>='{1}'", comboBox1.SelectedItem.ToString(), textBox1.Text);
            ds.Tables["Клиент"].DefaultView.RowFilter = filter;
        }

        //История
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;

                string query = "select id_договора, сумма, дата_выдачи, дата_возврата from Договор where id_клиента = @id";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@id", kod);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Установите DataTable в DataGridView
                dataGridView2.DataSource = dataTable;
            }
        }

        //Добавить
        private void button3_Click(object sender, EventArgs e)
        {
            int id = 0;
            AddClients f = new AddClients();
            using (SqlCommand cmd = new SqlCommand("select * from Клиент", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    id = (int)reader[0];
                }
                reader.Close();
            }
            f.textBox7.Text = (id + 1).ToString();

            DialogResult result = f.ShowDialog(this);
            if (result == DialogResult.Cancel) return;

            DataRow row = ds.Tables["Клиент"].NewRow();

            row[0] = Convert.ToInt32(f.textBox7.Text);
            row[1] = Convert.ToString(f.textBox1.Text);
            row[2] = Convert.ToString(f.textBox2.Text);
            row[3] = Convert.ToString(f.textBox3.Text);
            row[4] = Convert.ToString(f.textBox5.Text);
            row[5] = Convert.ToString(f.textBox6.Text);

            ds.Tables["Клиент"].Rows.Add(row);

            if (ds.Tables["Клиент"].GetChanges(DataRowState.Added) != null) da.Update
          (ds.Tables["Клиент"]);
        }

        //Редактировать
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                AddClients f = new AddClients();
                f.textBox7.Text = kod.ToString();
                using (SqlCommand cmd = new SqlCommand("select * from Клиент where id_клиента = @pr", cnn))
                {
                    cmd.Parameters.AddWithValue("@pr", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        f.textBox1.Text = String.Format("{0}", reader[1]);
                        f.textBox2.Text = String.Format("{0}", reader[2]);
                        f.textBox3.Text = String.Format("{0}", reader[3]);
                        f.textBox5.Text = String.Format("{0}", reader[4]);
                        f.textBox6.Text = String.Format("{0}", reader[5]);
                    }
                    reader.Close();
                }

                DialogResult result = f.ShowDialog(this);
                if (result == DialogResult.Cancel) return;

                using (SqlCommand cmd = new SqlCommand("Update Клиент set Фамилия=@fio, Имя=@name, Отчество=@ot, Телефон=@nom, Почта = @login where id_клиента=@id", cnn))
                {
                    cmd.Parameters.AddWithValue("@name", f.textBox1.Text);
                    cmd.Parameters.AddWithValue("@fio", f.textBox2.Text);
                    cmd.Parameters.AddWithValue("@prava", f.textBox3.Text);
                    cmd.Parameters.AddWithValue("@nom", f.textBox5.Text);
                    cmd.Parameters.AddWithValue("@login", f.textBox6.Text);
                    cmd.Parameters.AddWithValue("@id", f.textBox7.Text);
                    cmd.ExecuteNonQuery();
                }
            }
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Клиент", cnn);
            da.Fill(ds, "Клиент");
            dataGridView1.DataSource = ds.Tables["Клиент"];
        }
    }
}
