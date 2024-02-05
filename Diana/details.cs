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
    public partial class details : Form
    {
        public details()
        {
            InitializeComponent();
        }

        SqlConnection cnn;
        SqlCommand cmd = new SqlCommand();
        DataSet dstake = new DataSet();
        SqlDataAdapter datake = new SqlDataAdapter();
        BindingSource bind = new BindingSource();
        private void button11_Click(object sender, EventArgs e)
        {
            if (dataGridView3.CurrentCell.ColumnIndex == dataGridView3.Columns[0].DisplayIndex)
            {
                int kod = (int)dataGridView3[dataGridView3.CurrentCellAddress.X, dataGridView3.CurrentCellAddress.Y].Value;
                cnn.Open();
                cmd.Connection = cnn;
                datake.SelectCommand = new SqlCommand("select* from Счет_за_штрафы", cnn);
                datake.Fill(dstake, "Счет_за_штрафы");
                //Программируем адаптер на вставку новых данных
                datake.InsertCommand = new SqlCommand("insert into Счет_за_штрафы values ( @idgive, @iddoc,@data, @sum, @paied)", cnn);
                datake.InsertCommand.Parameters.Add("@idgive", SqlDbType.Int, 1000000, "id_счета");
                datake.InsertCommand.Parameters.Add("@iddoc", SqlDbType.Int, 1000000, "id_нарушения");
                datake.InsertCommand.Parameters.Add("@sum", SqlDbType.VarChar, 50, "сумма");
                datake.InsertCommand.Parameters.Add("@paied", SqlDbType.Int, 1000000, "оплачено");
                datake.InsertCommand.Parameters.Add("@data", SqlDbType.VarChar, 50, "дата");
                bind.DataSource = dstake.Tables["Счет_за_штрафы"];

                int count = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Счет_за_штрафы where id_нарушения = @h", cnn))
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
                    using (SqlCommand cmd = new SqlCommand("select * from Счет_за_штрафы", cnn))
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
                    using (SqlCommand cmd = new SqlCommand("select * from Нарушения where id_нарушения = @c", cnn))
                    {
                        cmd.Parameters.AddWithValue("@c", kod);
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            summ = (reader2[3]).ToString();
                        }
                        reader2.Close();
                    }

                    DataRow row = dstake.Tables["Счет_за_штрафы"].NewRow();

                    row[0] = idact;
                    row[1] = kod;
                    row[3] = summ;
                    row[4] = 1;
                    row[2] = (DateTime.Now.ToShortDateString()).ToString();

                    dstake.Tables["Счет_за_штрафы"].Rows.Add(row);

                    if (dstake.Tables["Счет_за_штрафы"].GetChanges(DataRowState.Added) != null) datake.Update
                  (dstake.Tables["Счет_за_штрафы"]);

                    MessageBox.Show("Оплачено");
                }
                else { MessageBox.Show("Штраф уже был оплачен"); }
                cnn.Close();
                PaintGrid();
            }
        }

        private void details_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(@"Data Source=LAPTOP-19CKPI6G\MSSQLSERVER2;Initial Catalog=Хостел;Integrated Security=True");
        }
        public void PaintGrid()
        {
            cnn.Open();
            int j = 0;
            for (int i = 0; i < dataGridView3.RowCount - 1; i++)
            {
                int kod = (int)dataGridView3[j, i].Value;
                int count = 0;
                using (SqlCommand cmd = new SqlCommand("select * from Счет_за_штрафы where id_нарушения = @h", cnn))
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
                    dataGridView3.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridView3.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                }
            }
            cnn.Close();
        }
    }
}
