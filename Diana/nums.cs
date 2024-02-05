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
    public partial class nums : Form
    {
        public nums()
        {
            InitializeComponent();
        }


        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();
        BindingSource bind = new BindingSource();


        private void nums_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            da.SelectCommand = new SqlCommand("select* from Номер", cnn);
            da.Fill(ds, "Номер");
            dataGridView1.DataSource = ds.Tables["Номер"];

            //Чтение списка названий столбцов таблицы в comboB
            foreach (DataColumn col in ds.Tables["Номер"].Columns)
            {
                comboBox1.Items.Add(col.ColumnName);
            }
            //Установка значений по умолчанию
            comboBox1.SelectedItem = "id_номера";
            ds.Tables["Номер"].DefaultView.Sort = "id_номера";

            //Связывание сеток с 2-мя различными представлениями таблицы
            dataGridView1.DataSource = ds.Tables["Номер"].DefaultView;
            //Программируем адаптер на вставку новых данных
            da.InsertCommand = new SqlCommand("insert into Номер values ( @id, @nom, @year, @k)", cnn);
            da.InsertCommand.Parameters.Add("@id", SqlDbType.Int, 1000000, "id_номера");
            da.InsertCommand.Parameters.Add("@nom", SqlDbType.VarChar, 50, "Номер");
            da.InsertCommand.Parameters.Add("@year", SqlDbType.VarChar, 50, "количество_мест");
            da.InsertCommand.Parameters.Add("@k", SqlDbType.VarChar, 50, "id_категории");
            bind.DataSource = ds.Tables["Номер"];

            cnn.Close();
        }

        //поиск
        private void button7_Click(object sender, EventArgs e)
        {
            string filter = String.Format("{0}>='{1}'", comboBox1.SelectedItem.ToString(), textBox1.Text);
            ds.Tables["Номер"].DefaultView.RowFilter = filter;
        }

        //назад
        private void button14_Click(object sender, EventArgs e)
        {
            this.Close();
            Form ifrm = Application.OpenForms[1];
            ifrm.Show();
        }

        //Добавить
        private void button3_Click(object sender, EventArgs e)
        {
            cnn.Open();
            int id = 0;
            AddNum f = new AddNum();
            using (SqlCommand cmd = new SqlCommand("select * from Номер", cnn))
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

            using (SqlCommand cmd = new SqlCommand("select * from Ценовая_категория", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    f.comboBox3.Items.Add(reader2[0]);
                }
                reader2.Close();
            }


            DialogResult result = f.ShowDialog(this);
            if (result == DialogResult.Cancel) return;
            int ids = 0;

            DataRow row = ds.Tables["Номер"].NewRow();

            row[0] = Convert.ToInt32(f.textBox7.Text);
            row[1] = Convert.ToString(f.textBox4.Text);
            row[2] = Convert.ToString(f.textBox5.Text);
            row[3] = Convert.ToString(f.comboBox3.Text);


            ds.Tables["Номер"].Rows.Add(row);

            if (ds.Tables["Номер"].GetChanges(DataRowState.Added) != null) da.Update
          (ds.Tables["Номер"]);
            cnn.Close();
        }

        //Редактировать
        private void button4_Click(object sender, EventArgs e)
        {
            cnn.Open();
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                AddNum f = new AddNum();
                f.textBox7.Text = kod.ToString();

                using (SqlCommand cmd = new SqlCommand("select * from Ценовая_категория", cnn))
                {
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader2 = cmd.ExecuteReader();
                    while (reader2.Read())
                    {
                        f.comboBox3.Items.Add(reader2[0]);
                    }
                    reader2.Close();
                }

                int ids = 0;
                int mod = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Номер where id_номера = @pr", cnn))
                {
                    cmd.Parameters.AddWithValue("@pr", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        mod = Convert.ToInt32(reader[2]);
                        f.textBox4.Text = String.Format("{0}", reader[1]);
                        ids = Convert.ToInt32(reader[1]);
                        f.textBox5.Text = String.Format("{0}", reader[2]);
                        f.comboBox3.SelectedItem = Convert.ToInt32(reader[3]);
                    }
                    reader.Close();
                }

                cnn.Close();
                cnn.Open();

                DialogResult result = f.ShowDialog(this);
                if (result == DialogResult.Cancel) { cnn.Close(); return; };
                int idsalona = 0;

                using (SqlCommand cmd = new SqlCommand("Update Номер set Номер=@ot, количество_мест=@nom, id_категории= @login where id_номера=@id", cnn))
                {
                    cmd.Parameters.AddWithValue("@id", f.textBox7.Text);
                    cmd.Parameters.AddWithValue("@ot", f.textBox4.Text);
                    cmd.Parameters.AddWithValue("@nom", f.textBox5.Text);
                    cmd.Parameters.AddWithValue("@login", f.comboBox3.SelectedItem);
                    cmd.ExecuteNonQuery();
                }
            }
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Номер", cnn);
            da.Fill(ds, "Номер");
            dataGridView1.DataSource = ds.Tables["Номер"];
            cnn.Close();
        }
    }
}
