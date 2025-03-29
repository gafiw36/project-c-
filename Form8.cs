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

namespace meatball  //ข้อมูลลูกค้า
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private void showGridView1() //ฟังก์ชัน showGridView1 ใช้สำหรับดึงข้อมูลจากฐานข้อมูลและแสดงผลใน DataGridView1
        {
            MySqlConnection conn = databaseConnection(); 
            DataSet ds = new DataSet(); // สร้างตัวแปร DataSet เพื่อเก็บข้อมูลที่ดึงมาจากฐานข้อมูล
            conn.Open(); 

            MySqlCommand cmd; 
            cmd = conn.CreateCommand(); 
            cmd.CommandText = "SELECT * FROM customerdata "; // กำหนดคำสั่ง SQL ให้ดึงข้อมูลทั้งหมดจากตาราง customerdata

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd); 
            adapter.Fill(ds); // ดึงข้อมูลจากฐานข้อมูลและเติมข้อมูลลงใน DataSet (ds)
            conn.Close(); 

            dataGridView1.DataSource = ds.Tables[0].DefaultView; // กำหนด DataSource ของ DataGridView1 ให้แสดงข้อมูลจาก DataTable แรกใน DataSet
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            showGridView1();
        }

        private void button1_Click(object sender, EventArgs e) //ปุ่มยืนยันแก้ไข
        {
            try
            {
                // ตรวจสอบว่ามีการเลือกแถวใน DataGridView หรือไม่
                if (dataGridView1.CurrentCell == null)
                {
                    MessageBox.Show("กรุณาเลือกแถวที่ต้องการแก้ไข");
                    return;
                }

                // ดึงค่า row ที่ถูกเลือก
                int selectedRow = dataGridView1.CurrentCell.RowIndex; // กำหนดตัวแปร selectedRow ให้เก็บดัชนีของแถว (row index) ที่ผู้ใช้เลือกใน dataGridView1
                int editId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value); //editId ใช้เพื่อดึงค่าในคอลัมน์ "id" ของแถวนั้นมาใช้งาน โดยแปลงให้เป็นจำนวนเต็ม(int).

                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();

                    // สร้าง SQL query เพื่ออัปเดตชื่อและเบอร์โทรในตาราง customerdata
                    string sqlCustomer = "UPDATE customerdata SET name = @name, number = @number WHERE id = @id";
                    MySqlCommand cmdCustomer = new MySqlCommand(sqlCustomer, conn);
                    cmdCustomer.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                    cmdCustomer.Parameters.AddWithValue("@number", textBox2.Text.Trim());
                    cmdCustomer.Parameters.AddWithValue("@id", editId);

                    int rowsCustomer = cmdCustomer.ExecuteNonQuery();

                    // สร้าง SQL query เพื่ออัปเดตข้อมูลในตาราง orders
                    string sqlOrders = "UPDATE orders SET name = @name, number = @number WHERE id = @id";
                    MySqlCommand cmdOrders = new MySqlCommand(sqlOrders, conn);
                    cmdOrders.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                    cmdOrders.Parameters.AddWithValue("@number", textBox2.Text.Trim());
                    cmdOrders.Parameters.AddWithValue("@id", editId);

                    int rowsOrders = cmdOrders.ExecuteNonQuery();

                    // ตรวจสอบว่ามีการอัปเดตข้อมูลสำเร็จในหนึ่งในสองตาราง
                    if (rowsCustomer > 0 || rowsOrders > 0)
                    {
                        MessageBox.Show("แก้ไขข้อมูลสำเร็จ");
                        showGridView1();  // อัปเดต DataGridView หลังจากแก้ไขข้อมูลสำเร็จ
                    }
                    else
                    {
                        MessageBox.Show("ไม่พบข้อมูลที่จะแก้ไข");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }



        private void button2_Click(object sender, EventArgs e) //ปุ่มลบ
        {
            // รับหมายเลขแถวที่เลือกใน DataGridView
            int selectedRow = dataGridView1.CurrentCell.RowIndex; // ดึงค่า index ของแถวปัจจุบันที่เลือกใน DataGridView1
            int deleteId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value); // แปลงค่า "id" จากแถวที่เลือกเป็น Integer

            // สร้างการเชื่อมต่อกับฐานข้อมูล
            MySqlConnection conn = databaseConnection(); // เรียกใช้เมธอด databaseConnection() เพื่อเชื่อมต่อกับฐานข้อมูล
            String sql = "DELETE FROM customerdata WHERE id = '" + deleteId + "'"; // สร้างคำสั่ง SQL สำหรับลบข้อมูลในตาราง customerdata โดยระบุเงื่อนไข id
            MySqlCommand cmd = new MySqlCommand(sql, conn); // สร้างวัตถุ MySqlCommand เพื่อเตรียมคำสั่ง SQL และการเชื่อมต่อ

            conn.Open(); 

            int rows = cmd.ExecuteNonQuery(); 
            conn.Close(); 

            if (rows > 0) 
            {
                MessageBox.Show("ลบข้อมูลสำเร็จ"); 
                showGridView1(); 
            }
        }

        private void LoadData(string customerPhone) // ฟังก์ชัน LoadData มีหน้าที่คนหาและโหลดข้อมูลจากฐานข้อมูลตามหมายเลขโทรศัพท์ของลูกค้า (customerPhone)
        {
            
            MySqlConnection conn = databaseConnection();

            try
            {
                conn.Open(); // เปิดการเชื่อมต่อฐานข้อมูล
                string query = "SELECT * FROM customerdata WHERE number LIKE @customerPhone"; // สร้างคำสั่ง SQL เพื่อค้นหาข้อมูลจาก customerdata โดยใช้เงื่อนไข LIKE
                MySqlCommand cmd = new MySqlCommand(query, conn); 
                cmd.Parameters.AddWithValue("@customerPhone", "%" + customerPhone + "%"); //ค้นหาหมายเลขโทรศัพท์ที่มี customerPhone อยู่ในที่ใดที่หนึ่งของหมายเลขโทรศัพท์ทั้งหมด"..

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd); 
                DataSet ds = new DataSet(); 
                adapter.Fill(ds, "orders"); 

                if (ds.Tables["orders"].Rows.Count > 0) 
                {
                    dataGridView1.DataSource = ds.Tables["orders"]; // ถ้ามีข้อมูลให้แสดงใน DataGridView
                }
                else
                {
                    dataGridView1.DataSource = null; // ถ้าไม่พบข้อมูลให้ล้าง DataGridView
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการค้นหา: " + ex.Message); 
            }
            finally
            {
                conn.Close(); 
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // ตรวจสอบให้แน่ใจว่ามีการคลิกที่แถวจริง
            {
                dataGridView1.CurrentRow.Selected = true;
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["name"].Value.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["number"].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
            this.Hide();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // ค้นหาชื่อลูกค้าเมื่อมีการเปลี่ยนแปลงใน TextBox
            string searchValue = textBox3.Text; // ใช้ค่าจาก textBox3
            LoadData(searchValue); // เรียกใช้ LoadData ด้วยหมายเลขโทรศัพท์ที่กรอก
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
