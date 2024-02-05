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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Diana
{
    public partial class contracts : Form
    {
        public contracts(int emp)
        {
            InitializeComponent();
            idEmp = emp;
        }

        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet dsgive = new DataSet();
        SqlDataAdapter dagive = new SqlDataAdapter();
        DataSet dstake = new DataSet();
        SqlDataAdapter datake = new SqlDataAdapter();
        DataSet dsr = new DataSet();
        SqlDataAdapter dar = new SqlDataAdapter();
        BindingSource bind = new BindingSource();
        int idEmp;

        private void contracts_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
            cnn.Open();
            cmd.Connection = cnn;
            da.SelectCommand = new SqlCommand("select* from Договор", cnn);
            da.Fill(ds, "Договор");
            dataGridView1.DataSource = ds.Tables["Договор"];

            //Чтение списка названий столбцов таблицы в comboB
            foreach (DataColumn col in ds.Tables["Договор"].Columns)
            {
                comboBox1.Items.Add(col.ColumnName);
            }
            //Установка значений по умолчанию
            comboBox1.SelectedItem = "id_договора";
            ds.Tables["Договор"].DefaultView.Sort = "id_договора";

            //Связывание сеток с 2-мя различными представлениями таблицы
            dataGridView1.DataSource = ds.Tables["Договор"].DefaultView;

            //Программируем адаптер на вставку новых данных
            da.InsertCommand = new SqlCommand("insert into Договор (id_договора,id_сотрудника,id_номера,id_клиента,сумма) values ( @iddoc, @idsot, @idcar, @idcl, @sum)", cnn);
            da.InsertCommand.Parameters.Add("@iddoc", SqlDbType.Int, 1000000, "id_договора");
            da.InsertCommand.Parameters.Add("@idsot", SqlDbType.Int, 1000000, "id_сотрудника");
            da.InsertCommand.Parameters.Add("@idcar", SqlDbType.Int, 1000000, "id_номера");
            da.InsertCommand.Parameters.Add("@idcl", SqlDbType.Int, 1000000, "id_клиента");
            da.InsertCommand.Parameters.Add("@sum", SqlDbType.VarChar, 50, "сумма");
            bind.DataSource = ds.Tables["Договор"];

            dar.InsertCommand = new SqlCommand("insert into Нарушения values ( @idn, @idc, @idd, @sum, @data)", cnn);
            dar.InsertCommand.Parameters.Add("@idn", SqlDbType.Int, 1000000, "id_нарушения");
            dar.InsertCommand.Parameters.Add("@idc", SqlDbType.Int, 1000000, "id_клиента");
            dar.InsertCommand.Parameters.Add("@idd", SqlDbType.Int, 1000000, "id_договора");
            dar.InsertCommand.Parameters.Add("@sum", SqlDbType.Int, 1000000, "сумма");
            dar.InsertCommand.Parameters.Add("@data", SqlDbType.VarChar, 50, "дата");
            bind.DataSource = dsr.Tables["Нарушения"];
            dar.SelectCommand = new SqlCommand("select* from Нарушения", cnn);
            dar.Fill(dsr, "Нарушения");

              PaintGrid();

            cnn.Close();
        }


        //Назад
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
            ds.Tables["Договор"].DefaultView.RowFilter = filter;
        }

        //Добавить
        private void button1_Click(object sender, EventArgs e)
        {
            cnn.Open();
            int id = 0;
            AddContract f = new AddContract();
            using (SqlCommand cmd = new SqlCommand("select * from Договор", cnn))
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

            using (SqlCommand cmd = new SqlCommand("select * from Клиент", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    f.comboBox1.Items.Add(reader2[2]);
                }
                reader2.Close();
            }

            using (SqlCommand cmd = new SqlCommand("select * from Номер", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader3 = cmd.ExecuteReader();
                while (reader3.Read())
                {
                    f.comboBox3.Items.Add(reader3[1]);
                }
                reader3.Close();
            }


            DialogResult result = f.ShowDialog(this);
            if (result == DialogResult.Cancel) { cnn.Close(); return; };

            int idk = 0;
            using (SqlCommand cmd = new SqlCommand("select * from Клиент where Фамилия = @f", cnn))
            {
                cmd.Parameters.AddWithValue("@f", f.comboBox1.Text);
                cmd.ExecuteNonQuery();
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    idk = Convert.ToInt32(reader2[0]);
                }
                reader2.Close();
            }
            int idc = 0;
            using (SqlCommand cmd = new SqlCommand("select * from Номер where Номер = @c", cnn))
            {
                cmd.Parameters.AddWithValue("@c", f.comboBox3.Text);
                cmd.ExecuteNonQuery();
                SqlDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    idc = Convert.ToInt32(reader2[0]);
                }
                reader2.Close();
            }

            DataRow row = ds.Tables["Договор"].NewRow();

            row[0] = Convert.ToInt32(f.textBox7.Text);
            row[1] = Convert.ToInt32(idEmp);
            row[2] = Convert.ToInt32(idc);
            row[3] = Convert.ToInt32(idk);
            row[4] = Convert.ToString(f.textBox1.Text);


            ds.Tables["Договор"].Rows.Add(row);

            if (ds.Tables["Договор"].GetChanges(DataRowState.Added) != null) da.Update
          (ds.Tables["Договор"]);
            PaintGrid();
            cnn.Close();
        }

        //выдать
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                cnn.Open();
                cmd.Connection = cnn;
                dagive.SelectCommand = new SqlCommand("select* from Выдача_ключа", cnn);
                dagive.Fill(dsgive, "Выдача_ключа");
                //Программируем адаптер на вставку новых данных
                dagive.InsertCommand = new SqlCommand("insert into Выдача_ключа values ( @idgive, @iddoc, @idcar, @data)", cnn);
                dagive.InsertCommand.Parameters.Add("@idgive", SqlDbType.Int, 1000000, "id_выдачи");
                dagive.InsertCommand.Parameters.Add("@iddoc", SqlDbType.Int, 1000000, "id_договора");
                dagive.InsertCommand.Parameters.Add("@idcar", SqlDbType.Int, 1000000, "id_номера");
                dagive.InsertCommand.Parameters.Add("@data", SqlDbType.VarChar, 50, "дата");
                bind.DataSource = dsgive.Tables["Выдача_ключа"];

                int count = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Выдача_ключа where id_договора = @h", cnn))
                {
                    cmd.Parameters.AddWithValue("@h", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        count++;
                    }
                    reader.Close();
                }
                if (count == 0)
                {

                    int idgive = 0;
                    using (SqlCommand cmd = new SqlCommand("select * from Выдача_ключа", cnn))
                    {
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            idgive = (int)reader[0];
                        }
                        reader.Close();
                    }
                    idgive = idgive + 1;

                    int idcar = 0;

                    using (SqlCommand cmd = new SqlCommand("select * from Договор where id_договора = @c", cnn))
                    {
                        cmd.Parameters.AddWithValue("@c", kod);
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            idcar = Convert.ToInt32(reader2[2]);
                        }
                        reader2.Close();
                    }

                    int ids = 0;
                    using (SqlCommand cmd = new SqlCommand("select * from Номер where id_номера = @car", cnn))
                    {
                        cmd.Parameters.AddWithValue("@car", idcar);
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            ids = Convert.ToInt32(reader2[0]);
                        }
                        reader2.Close();
                    }

                    DataRow row = dsgive.Tables["Выдача_ключа"].NewRow();

                    row[0] = idgive;
                    row[1] = kod;
                    row[2] = idcar;
                    row[3] = (DateTime.Now.ToShortDateString()).ToString();


                    dsgive.Tables["Выдача_ключа"].Rows.Add(row);

                    if (dsgive.Tables["Выдача_ключа"].GetChanges(DataRowState.Added) != null) dagive.Update
                  (dsgive.Tables["Выдача_ключа"]);

                    MessageBox.Show("Ключ выдан");

                }
                else { MessageBox.Show("Номер уже занят"); }
                cnn.Close();
            }
        }

        //Принять
        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                cnn.Open();
                cmd.Connection = cnn;
                datake.SelectCommand = new SqlCommand("select* from Принятие_ключа", cnn);
                datake.Fill(dstake, "Принятие_ключа");
                //Программируем адаптер на вставку новых данных
                datake.InsertCommand = new SqlCommand("insert into Принятие_ключа values ( @idgive,  @idcar, @iddoc, @data)", cnn);
                datake.InsertCommand.Parameters.Add("@idgive", SqlDbType.Int, 1000000, "id_приемки");
                datake.InsertCommand.Parameters.Add("@iddoc", SqlDbType.Int, 1000000, "id_договора");
                datake.InsertCommand.Parameters.Add("@idcar", SqlDbType.Int, 1000000, "id_номера");
                datake.InsertCommand.Parameters.Add("@data", SqlDbType.VarChar, 50, "дата");
                bind.DataSource = dstake.Tables["Принятие_ключа"];

                int count = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Принятие_ключа where id_договора = @h", cnn))
                {
                    cmd.Parameters.AddWithValue("@h", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        count++;
                    }
                    reader.Close();
                }
                if (count == 0)
                {

                    int idtake = 0;
                    using (SqlCommand cmd = new SqlCommand("select * from Принятие_ключа", cnn))
                    {
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            idtake = (int)reader[0];
                        }
                        reader.Close();
                    }
                    idtake = idtake + 1;

                    int ids = 0;
                    int idcar = 0;

                    using (SqlCommand cmd = new SqlCommand("select * from Договор where id_договора = @c", cnn))
                    {
                        cmd.Parameters.AddWithValue("@c", kod);
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            idcar = Convert.ToInt32(reader2[2]);
                        }
                        reader2.Close();
                    }


                    DataRow row = dstake.Tables["Принятие_ключа"].NewRow();

                    row[0] = idtake;
                    row[2] = kod;
                    row[1] = idcar;
                    row[3] = (DateTime.Now.ToShortDateString()).ToString();


                    dstake.Tables["Принятие_ключа"].Rows.Add(row);

                    if (dstake.Tables["Принятие_ключа"].GetChanges(DataRowState.Added) != null) datake.Update
                  (dstake.Tables["Принятие_ключа"]);

                    MessageBox.Show("Номер принят");
                    cnn.Close();
                    AddFine(kod);
                }
                else { MessageBox.Show("Ключи уже были отданы"); cnn.Close(); }

                dataGridView1.Update();
                dataGridView1.Refresh();
            }
        }

        public void AddFine(int iddoc)
        {
            cnn.Open();
            int id = 0;
            AddFine form = new AddFine();
            using (SqlCommand cmd = new SqlCommand("select * from Нарушения", cnn))
            {
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    id = (int)reader[0];
                }
                reader.Close();
            }
            form.textBox7.Text = (id + 1).ToString();

            int idc = 0;
            using (SqlCommand cmd = new SqlCommand("select * from Договор where id_договора=@iddog", cnn))
            {
                cmd.Parameters.AddWithValue("@iddog", iddoc);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    idc = (int)reader[3];
                }
                reader.Close();
            }
            form.textBox7.Text = (id + 1).ToString();

            DialogResult result = form.ShowDialog(this);
            if (result == DialogResult.Cancel) { cnn.Close(); return; };

            DataRow row = dsr.Tables["Нарушения"].NewRow();

            row[0] = Convert.ToInt32(form.textBox7.Text);
            row[1] = idc;
            row[2] = iddoc;
            row[3] = 0;
            row[4] = Convert.ToString(form.dateTimePicker1.Value);

            dsr.Tables["Нарушения"].Rows.Add(row);

            if (dsr.Tables["Нарушения"].GetChanges(DataRowState.Added) != null) dar.Update
          (dsr.Tables["Нарушения"]);



            foreach (var item in form.listBox1.SelectedItems)
            {
                // название процедуры
                string sqlExpression = "AddN";
                SqlCommand command = new SqlCommand(sqlExpression, cnn);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@id_n",
                    Value = Convert.ToInt32(form.textBox7.Text)
                };
                // добавляем параметр
                command.Parameters.Add(nameParam);
                // параметр для ввода возраста
                SqlParameter ageParam = new SqlParameter
                {
                    ParameterName = "@yname",
                    Value = item.ToString()
                };
                command.Parameters.Add(ageParam);

                var res = command.ExecuteScalar();
                // если нам не надо возвращать id
                // var result = command.ExecuteNonQuery();

                // Console.WriteLine("Id добавленного объекта: {0}", result);
            }


            cnn.Close();
        }

        //Счет
        private void button11_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                cnn.Open();
                cmd.Connection = cnn;
                datake.SelectCommand = new SqlCommand("select* from Счет_по_договору", cnn);
                datake.Fill(dstake, "Счет_по_договору");
                //Программируем адаптер на вставку новых данных
                datake.InsertCommand = new SqlCommand("insert into Счет_по_договору values ( @idgive, @iddoc, @sum,  @data, @paied)", cnn);
                datake.InsertCommand.Parameters.Add("@idgive", SqlDbType.Int, 1000000, "id_счета");
                datake.InsertCommand.Parameters.Add("@iddoc", SqlDbType.Int, 1000000, "id_договора");
                datake.InsertCommand.Parameters.Add("@sum", SqlDbType.VarChar, 50, "сумма");
                datake.InsertCommand.Parameters.Add("@paied", SqlDbType.Int, 1000000, "оплата");
                datake.InsertCommand.Parameters.Add("@data", SqlDbType.VarChar, 50, "дата");
                bind.DataSource = dstake.Tables["Счет_по_договору"];


                int count = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Счет_по_договору where id_договора = @h", cnn))
                {
                    cmd.Parameters.AddWithValue("@h", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        count++;
                    }
                    reader.Close();
                }
                if (count == 0)
                {
                    int idact = 0;
                    using (SqlCommand cmd = new SqlCommand("select * from Счет_по_договору", cnn))
                    {
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            idact = (int)reader[0];
                        }
                        reader.Close();
                    }
                    idact = idact + 1;

                    string summ = "";

                    using (SqlCommand cmd = new SqlCommand("select * from Договор where id_договора = @c", cnn))
                    {
                        cmd.Parameters.AddWithValue("@c", kod);
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            summ = (reader2[4]).ToString();
                        }
                        reader2.Close();
                    }

                    DataRow row = dstake.Tables["Счет_по_договору"].NewRow();

                    row[0] = idact;
                    row[1] = kod;
                    row[2] = summ;
                    row[4] = 1;
                    row[3] = (DateTime.Now.ToShortDateString()).ToString();


                    dstake.Tables["Счет_по_договору"].Rows.Add(row);

                    if (dstake.Tables["Счет_по_договору"].GetChanges(DataRowState.Added) != null) datake.Update
                  (dstake.Tables["Счет_по_договору"]);

                    MessageBox.Show("Оплачено");
                      PaintGrid();
                }

                else { MessageBox.Show("Счет уже оплачен"); }
                cnn.Close();
            }
        }

        //Штраф
        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                AddFine(kod);
            }
        }

        //Подробности
        private void button4_Click(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value;
                details f = new details();
                f.Show();
                string query = "select * from Выдача_ключа where id_договора = @id";
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@id", kod);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Установите DataTable в DataGridView
                f.dataGridView1.DataSource = dataTable;

                string query2 = "select * from Принятие_ключа where id_договора = @id";
                SqlCommand cmd2 = new SqlCommand(query2, cnn);
                cmd2.Parameters.AddWithValue("@id", kod);

                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                DataTable dataTable2 = new DataTable();
                adapter2.Fill(dataTable2);

                // Установите DataTable в DataGridView
                f.dataGridView2.DataSource = dataTable2;


                string query3 = "select * from Нарушения where id_договора = @id";
                SqlCommand cmd3 = new SqlCommand(query3, cnn);
                cmd3.Parameters.AddWithValue("@id", kod);

                SqlDataAdapter adapter3 = new SqlDataAdapter(cmd3);
                DataTable dataTable3 = new DataTable();
                adapter3.Fill(dataTable3);

                // Установите DataTable в DataGridView
                f.dataGridView3.DataSource = dataTable3;
                f.PaintGrid();
            }
        }

        public void PaintGrid()
        {
            int j = 0;
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                int kod = (int)dataGridView1[j, i].Value;
                int count = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Счет_по_договору where id_договора = @h", cnn))
                {
                    cmd.Parameters.AddWithValue("@h", kod);
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        count++;
                    }
                    reader.Close();
                }
                if (count == 0)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                }
            }
        }
    }
}
