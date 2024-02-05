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
    public partial class emploers : Form
    {
        public emploers()
        {
            InitializeComponent();
        }

        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        DataSet dsot = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();
        BindingSource bind = new BindingSource();

        private void emploers_Load(object sender, EventArgs e)
        {

            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            da.SelectCommand = new SqlCommand("select * from Сотрудники", cnn);
            da.Fill(ds, "Сотрудники");
            dataGridView1.DataSource = ds.Tables["Сотрудники"];
            //Чтение списка названий столбцов таблицы в comboB
            foreach (DataColumn col in ds.Tables["Сотрудники"].Columns)
            {
                comboBox1.Items.Add(col.ColumnName);
            }
            //Установка значений по умолчанию
            comboBox1.SelectedItem = "id_сотрудника";
            ds.Tables["Сотрудники"].DefaultView.Sort = "id_сотрудника";
            //Связывание сеток с 2-мя различными представлениями таблицы
            dataGridView1.DataSource = ds.Tables["Сотрудники"].DefaultView;


            //daot.SelectCommand = new SqlCommand("select* from Отдел", cnn);
            //daot.Fill(dsot, "Отдел");


            da.InsertCommand = new SqlCommand("insert into Сотрудники values ( @id, @dol, @name, @fio, @sname, @birht, @nom, @login)", cnn);
            da.InsertCommand.Parameters.Add("@id", SqlDbType.Int, 1000000, "id_сотрудника");
            da.InsertCommand.Parameters.Add("@dol", SqlDbType.Int, 1000000, "id_должности");
            da.InsertCommand.Parameters.Add("@name", SqlDbType.VarChar, 50, "Имя");
            da.InsertCommand.Parameters.Add("@fio", SqlDbType.VarChar, 50, "Фамилия");
            da.InsertCommand.Parameters.Add("@sname", SqlDbType.VarChar, 50, "Отчество");
            da.InsertCommand.Parameters.Add("@birht", SqlDbType.VarChar, 50, "Дата_рождения");
            da.InsertCommand.Parameters.Add("@nom", SqlDbType.VarChar, 50, "номер");
            da.InsertCommand.Parameters.Add("@login", SqlDbType.VarChar, 50, "почта");
            bind.DataSource = ds.Tables["сотрудники"];
        }

        //Поиск
        private void button5_Click(object sender, EventArgs e)
        {
            string filter = String.Format("{0}>='{1}'", comboBox1.SelectedItem.ToString(), textBox1.Text);
            ds.Tables["Сотрудники"].DefaultView.RowFilter = filter;
        }

        //Назад
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
            Form ifrm = Application.OpenForms[1];
            ifrm.Show();
        }

        //добавить
        private void button1_Click(object sender, EventArgs e)
        {
            int id = 0;
            AddEmp f = new AddEmp();

            //Автоподсчет id
            using (SqlCommand cmd = new SqlCommand("select * from Сотрудники", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    id = (int)reader[0];
                }
                reader.Close();
            }
            f.textBox1.Text = (id + 1).ToString();

            DialogResult result = f.ShowDialog(this);
            if (result == DialogResult.Cancel) return;

            DataRow row = ds.Tables["Сотрудники"].NewRow();

            row[0] = Convert.ToInt32(f.textBox1.Text);
            row[1] = Convert.ToInt32(f.GetId());
            row[2] = Convert.ToString(f.textBox4.Text);
            row[3] = Convert.ToString(f.textBox2.Text);
            row[4] = Convert.ToString(f.textBox3.Text);
            row[5] = Convert.ToString(f.dateTimePicker1.Value);
            row[6] = Convert.ToString(f.textBox5.Text);
            row[7] = Convert.ToString(f.textBox6.Text);

            ds.Tables["Сотрудники"].Rows.Add(row);

            if (ds.Tables["Сотрудники"].GetChanges(DataRowState.Added) != null) da.Update
          (ds.Tables["Сотрудники"]);

            string pasword = "";
            using (SqlCommand cmd = new SqlCommand("select * from Пользователь", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    pasword = reader[1].ToString();
                }
                reader.Close();
            }
            MessageBox.Show("Пароль нового сотрудника: " + pasword);
        }

        //Редакстировать
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                AddEmp f = new AddEmp();
                f.textBox1.Text = kod.ToString();
                f.textBox6.Enabled = false;
                int idd = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Сотрудники where id_сотрудника = @pr", cnn))
                {
                    cmd.Parameters.AddWithValue("@pr", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        f.textBox4.Text = String.Format("{0}", reader[2]);
                        f.textBox2.Text = String.Format("{0}", reader[3]);
                        idd = Convert.ToInt32(reader[1]);
                        f.textBox3.Text = String.Format("{0}", reader[4]);
                        f.dateTimePicker1.Value = DateTime.Parse(reader[5].ToString());
                        f.textBox5.Text = String.Format("{0}", reader[6]);
                        f.textBox6.Text = String.Format("{0}", reader[7]);
                    }
                    reader.Close();
                }

                //Выбор должности
                using (SqlCommand cmd = new SqlCommand("select * from Должности where id_должности = @iddol", cnn))
                {
                    string dolname = "";
                    int idot = 0;
                    cmd.Parameters.AddWithValue("@iddol", idd);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dolname = (reader[2]).ToString();
                        idot = Convert.ToInt32(reader[1]);
                    }
                    f.ControlLists(idot, dolname);
                }
                cnn.Close();
                cnn.Open();

                DialogResult result = f.ShowDialog(this);
                if (result == DialogResult.Cancel) return;

                using (SqlCommand cmd = new SqlCommand("Update Сотрудники set id_должности =@dol, Фамилия=@fio, Имя=@name, Отчество=@ot, номер=@nom, почта = @login, дата_рождения=@prava where id_сотрудника=@id", cnn))
                {
                    cmd.Parameters.AddWithValue("@id", f.textBox1.Text);
                    cmd.Parameters.AddWithValue("@name", f.textBox4.Text);
                    cmd.Parameters.AddWithValue("@fio", f.textBox2.Text);
                    cmd.Parameters.AddWithValue("@ot", f.textBox3.Text);
                    cmd.Parameters.AddWithValue("@prava", f.dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@nom", f.textBox5.Text);
                    cmd.Parameters.AddWithValue("@login", f.textBox6.Text);
                    cmd.Parameters.AddWithValue("@dol", f.GetId());
                    cmd.ExecuteNonQuery();
                }
            }
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Сотрудники", cnn);
            da.Fill(ds, "Сотрудники");
            dataGridView1.DataSource = ds.Tables["Сотрудники"];
        }

        //Удалить
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int index = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                using (SqlCommand cmd = new SqlCommand("delete from Сотрудники where id_сотрудника = @AvtId", cnn))
                {
                    cmd.Parameters.AddWithValue("@AvtId", index);
                    cmd.ExecuteNonQuery();
                }
            }
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Сотрудники", cnn);
            da.Fill(ds, "Сотрудники");
            dataGridView1.DataSource = ds.Tables["Сотрудники"];
        }
    }
}
