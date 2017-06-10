using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterCollect
{
    public partial class WaterSet : Form
    {
        public WaterSet()
        {
            InitializeComponent();
        }

        private void WaterSet_Load(object sender, EventArgs e)
        {
            String sql1 = null;
            String conString = MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.mtsics1ConnectionString");
            MySqlHelper my = new MySqlHelper(conString);
            sql1 = "select * from ey_matrtable ";
            DataSet ds = my.ExecuteDataSet(sql1);
            dataGridView1.DataSource = ds.Tables[0];      
           }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = dataGridView1.SelectedRows.Count;
            if (count == 1)
            {
                int a = dataGridView1.CurrentRow.Index;
                textBox6.Text = dataGridView1.Rows[a].Cells[0].Value.ToString();
                textBox1.Text = dataGridView1.Rows[a].Cells[1].Value.ToString();
                textBox2.Text = dataGridView1.Rows[a].Cells[2].Value.ToString();
                textBox3.Text = dataGridView1.Rows[a].Cells[3].Value.ToString();
                textBox4.Text = dataGridView1.Rows[a].Cells[4].Value.ToString();
                textBox5.Text = dataGridView1.Rows[a].Cells[5].Value.ToString();      

            }
        }
        //查询
        private void button3_Click(object sender, EventArgs e)
        {
            String sql1 = null;
            String conString = MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.mtsics1ConnectionString");
            MySqlHelper my = new MySqlHelper(conString);
                //如果料名不为空，根据料名查询
                if (textBox2.Text != "" && textBox2.Text != null)
                {
                    sql1 = "select * from ey_matrtable where matrName like '%" + textBox2.Text + "%'";
                    DataSet ds = my.ExecuteDataSet(sql1);
                    dataGridView1.DataSource = ds.Tables[0];
                }
            else {
                MessageBox.Show("请输入料名查询：例如：焦炭、煤等");
              
            }

        }
        //更新
        private void button2_Click(object sender, EventArgs e)
        {
            String sql1 = null;
            String conString = MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.mtsics1ConnectionString");
            MySqlHelper my = new MySqlHelper(conString);
            //如果料号、料名、水分值都为空，不允许更新 
            if (textBox2.Text != "" && textBox1.Text != "" && textBox5.Text != "")
            {
               
                    sql1 = "update ey_matrtable  set matrNo ='" + textBox1.Text + "' , matrName= '" + textBox2.Text  + "', water_range_less= " + textBox3.Text +  ", water_range_grater=" + textBox4.Text + ", water_value= " + textBox5.Text +"  where  matrNo ='" + textBox1.Text + "' and id = " + textBox6.Text ;
                    my.ExecuteNonQuery(sql1);
                    DataSet ds = my.ExecuteDataSet("select * from ey_matrtable where matrName like '%" + textBox2.Text + "%'");
                    dataGridView1.DataSource = ds.Tables[0];
                
            }
            else
            {
                MessageBox.Show("料号 、 料名 、水分值不可空");

            }


        }
        //删除
        private void button1_Click(object sender, EventArgs e)
        {

            String sql1 = null;
            String conString = MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.mtsics1ConnectionString");
            MySqlHelper my = new MySqlHelper(conString);
            //如果料号、料名、水分值都为空，不允许更新 
            if (textBox6.Text != "")
            {

                sql1 = "delete from ey_matrtable  where  id = " + textBox6.Text;
                my.ExecuteNonQuery(sql1);
                DataSet ds = my.ExecuteDataSet("select * from ey_matrtable where matrName like '%" + textBox2.Text + "%'");
                dataGridView1.DataSource = ds.Tables[0];

            }
            else
            {
                MessageBox.Show("ID 不可空 ");

            }

        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            String sql1 = null;
            String conString = MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.mtsics1ConnectionString");
            MySqlHelper my = new MySqlHelper(conString);
            //如果料号、料名、水分值都为空，不允许更新 
            if (textBox1.Text != "" && textBox2.Text != "" && textBox5.Text != "")
            {

                sql1 = "insert into ey_matrtable (matrNo,matrName,water_range_less,water_range_grater,water_value) values( '"+ textBox1.Text  + "','" + textBox2.Text + "'," +textBox3.Text +" ,"+ textBox4.Text + " ," + textBox5.Text+ " )";
                my.ExecuteNonQuery(sql1);
                DataSet ds = my.ExecuteDataSet("select * from ey_matrtable where matrName like '%" + textBox2.Text + "%'");
                dataGridView1.DataSource = ds.Tables[0];

            }
            else
            {
                MessageBox.Show("料号 、 料名 、水分值不可空 ");

            }
        }

       
    }
}
