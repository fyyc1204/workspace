using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace WaterCollect
{
    public partial class Form1 : Form
    {
        MySqlHelper db_hx = new MySqlHelper(MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.db_hxConnectionString"));
        MySqlHelper mtsics1 = new MySqlHelper(MySqlHelper.GetConnectionStringsConfig("WaterCollect.Properties.Settings.mtsics1ConnectionString"));
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Query();

            //获取称重数据

            timer2.Enabled = true;

            timer2.Start(); 


        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void 配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            WaterSet w = new WaterSet();
            w.ShowDialog();
            
        }
        //查询数据
        private void Query(){
            String sql = "select * from tb_temptable order by timenow desc  limit 50 ";
            DataSet ta = mtsics1.ExecuteDataSet(sql);
            dataGridView3.DataSource = ta.Tables[0];
        }
        //计算最终水分
        private Decimal computer( String pihao,  Decimal sfvalue  ) {

                 Decimal finalValue = sfvalue;
                String sql1 = "select * from ey_matrtable where matrNo = " + pihao;

                DataSet ds = mtsics1.ExecuteDataSet(sql1);

                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        if (dr.IsNull("water_range_less") || dr.IsNull("water_range_grater"))
                        {
                            finalValue = Convert.ToDecimal(dr["water_value"].ToString()) + sfvalue;
                            break;
                        }

                        Decimal lessvalue = Convert.ToDecimal(dr["water_range_less"].ToString());

                        Decimal gratervalue = Convert.ToDecimal(dr["water_range_grater"].ToString());

                        Decimal value = Convert.ToDecimal(dr["water_value"].ToString());
                                               
                        if (lessvalue == gratervalue && sfvalue == lessvalue)
                        {
                            finalValue = value + sfvalue;
                        }

                        if (sfvalue > lessvalue && sfvalue <= gratervalue)
                        {
                            finalValue = value + sfvalue;
                        }                        
                    }

                }
                return finalValue;

        }



        private void button4_Click(object sender, EventArgs e)
        {
            String sql = "SELECT pihao , sfvalue FROM tb_hisdata WHERE pihao=539 and sfvalue IS NOT NULL ORDER BY timenow DESC";
             
             DataRow dr = db_hx.ExecuteDataRow(sql);

             if (!(dr == null) && !dr.IsNull("sfvalue") && !dr.IsNull("pihao"))
             {                      
                 Decimal sfvalue =  Convert.ToDecimal(dr["sfvalue"].ToString());

                 //sfvalue = 1;
                 //sfvalue = 4;
                 //sfvalue = 5;
                 //sfvalue = 8;
                 //sfvalue = 17;                 

                 String date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                 String pihao = dr["pihao"].ToString();

                 Decimal finalvalue = computer(pihao, sfvalue);

                 String sql2 = "insert into tb_temptable (pihao,sfvalue,finalvalue,timenow,status) values('" + pihao + "'," + sfvalue + "," + finalvalue + ",'" + date + "','" + "N" + "')";

                 mtsics1.ExecuteNonQuery(sql2);               

             }

        }
        //定时上传数据到ERP
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                String sql = "select * from tb_temptable where status = 'N'";
                DataRow dr =  mtsics1.ExecuteDataRow(sql);
                if (!(dr == null)){

                    String pihao = dr["pihao"].ToString();
                    double water = Convert.ToDouble(dr["finalvalue"].ToString());

                    string date = Convert.ToDateTime(dr["timenow"]).ToString("yyyyMMddHHmmss").Substring(0, 8).Trim();
                    string time = Convert.ToDateTime(dr["timenow"]).ToString("yyyyMMddHHmmss").Substring(8, 6).Trim();

                    string str1 = water.ToString("0.00");
                    String mess = "N" + "###" + pihao + "###" + str1 + "###" + date + "###" + time;
                   
                    Decimal TimeStamp = Convert.ToDecimal(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    String Status = "N";
                    Decimal  SerialNo = Convert.ToDecimal(0);

                    String Header = "192.168.100.92" + "PJKFPTC" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    String QueueId = "PJKFPTC";

                    String sql2 = "insert into tbdipdo (timestamp,serialno,queueid, header,data,status) values( " + TimeStamp + "," + SerialNo + ",'" + QueueId + "','" + Header + "','" + mess + "','" + Status + "')";

                    mtsics1.ExecuteNonQuery(sql2);

                    String sql4 = "update tb_temptable set status='Y' where  pihao = '" + pihao + "'";

                    mtsics1.ExecuteNonQuery(sql4);
                                        
                }     

            }
            timer1.Stop();
            timer2.Enabled = true;

            timer2.Start();
            

        }
        //获取水分称重数据
        private void timer2_Tick(object sender, EventArgs e)
        {

            String sql = "SELECT pihao , sfvalue FROM tb_hisdata WHERE pihao=539 and sfvalue IS NOT NULL ORDER BY timenow DESC";

            DataRow dr = db_hx.ExecuteDataRow(sql);

            if (!(dr == null) && !dr.IsNull("sfvalue") && !dr.IsNull("pihao"))
            {
                Decimal sfvalue = Convert.ToDecimal(dr["sfvalue"].ToString());

                //sfvalue = 1;
                //sfvalue = 4;
                //sfvalue = 5;
                //sfvalue = 8;
                //sfvalue = 17;                 

                String date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                String pihao = dr["pihao"].ToString();

                Decimal finalvalue = computer(pihao, sfvalue);

                String sql2 = "insert into tb_temptable (pihao,sfvalue,finalvalue,timenow,status) values('" + pihao + "'," + sfvalue + "," + finalvalue + ",'" + date + "','" + " N " + "')";

                mtsics1.ExecuteNonQuery(sql2);

            }

            timer2.Stop();
            timer1.Enabled = true;
            timer1.Start();

            Query();
            //String sql3 = "update  tb_hisdata set status = ""  WHERE pihao=539 and sfvalue IS NOT NULL ORDER BY timenow DESC";

           // DataRow drr = db_hx.ExecuteDataRow(sql);

        }
        //dataGridview选单行
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = dataGridView3.SelectedRows.Count;
            if (count == 1)
            {
                int a = dataGridView3.CurrentRow.Index;
                textBox1.Text = dataGridView3.Rows[a].Cells[0].Value.ToString();
                textBox2.Text = dataGridView3.Rows[a].Cells[3].Value.ToString();      

            }
        }
        //修改
        private void button3_Click(object sender, EventArgs e)
        {

            String sql = "update tb_temptable set finalvalue = " + textBox2.Text +  " where  id = " + textBox1.Text ;

            mtsics1.ExecuteNonQuery(sql);

            Query();

        }
        //上传
        private void button1_Click(object sender, EventArgs e)
        {           



        }

      

    }
}
