using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace meatball //ประวัติการสั่งซื้อ
{
    public partial class Form11 : Form
    {
        // สร้างฟังก์ชันเพื่อเชื่อมต่อฐานข้อมูล
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";
            return new MySqlConnection(connectionString);
        }

        public Form11()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // ค้นหาชื่อลูกค้าเมื่อมีการเปลี่ยนแปลงใน TextBox
            string searchValue = textBox1.Text;
            LoadData(searchValue);
        }

        private void LoadData(string customerPhone)  // ปุ่มค้นหา โดยรับพารามิเตอร์ customerPhone
        {
            MySqlConnection conn = databaseConnection(); 

            try  
            {
                conn.Open();  
                string query = "SELECT * FROM orders WHERE number LIKE @customerPhone";  // คำสั่ง SQL สำหรับค้นหาข้อมูลจากตาราง orders โดยใช้หมายเลขโทรศัพท์
                MySqlCommand cmd = new MySqlCommand(query, conn);  
                cmd.Parameters.AddWithValue("@customerPhone", "%" + customerPhone + "%"); 

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd); 
                DataSet ds = new DataSet();  
                adapter.Fill(ds, "orders");  

                if (ds.Tables["orders"].Rows.Count > 0)  // ตรวจสอบว่ามีข้อมูลหรือไม่
                {
                    dataGridView1.DataSource = ds.Tables["orders"];  // ถ้ามีข้อมูล แสดงใน DataGridView
                }
                else
                {
                    dataGridView1.DataSource = null;  // ถ้าไม่พบข้อมูล ให้เคลียร์ DataGridView
                }
            }
            catch (Exception ex)  // ถ้ามีข้อผิดพลาด
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการค้นหา: " + ex.Message);  // แสดงข้อความผิดพลาด
            }
            finally  
            {
                conn.Close();  // ปิดการเชื่อมต่อกับฐานข้อมูล
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            Form7 form7 = new Form7();
            form7.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
