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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace meatball //หน้าลงทะเบียนไปซื้อออร์เดอร์
{
    public partial class Form3 : Form
    {
        private string Phonenumber; // ตัวแปรใช้สำหรับเก็บเบอร์โทรศัพท์

        public Form3()
        {
            InitializeComponent();
        }

        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            // ตรวจสอบว่า textbox1 มีข้อมูลหรือไม่
            if (string.IsNullOrWhiteSpace(textBox1.Text)) 
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์");
                return; 
            }

            string inputPhoneNumber = textBox1.Text.Trim(); // เก็บค่าเบอร์โทรจาก textBox1 ไว้ในตัวแปร

            // ตรวจสอบว่าเบอร์โทรเป็นตัวเลขและมีความยาว 10 ตัวหรือไม่
            if (!long.TryParse(inputPhoneNumber, out _) || inputPhoneNumber.Length != 10)
            {
                MessageBox.Show("กรุณากรอกเบอร์ให้ถูกต้อง"); 
                return; 
            }

            using (MySqlConnection conn = databaseConnection()) 
            {
                conn.Open(); 

                // สร้างคำสั่ง SQL สำหรับตรวจสอบเบอร์โทรในฐานข้อมูล
                string checkQuery = "SELECT name, number, email FROM customerdata WHERE number = @number";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn); 
                checkCmd.Parameters.AddWithValue("@number", inputPhoneNumber); 

                using (MySqlDataReader reader = checkCmd.ExecuteReader()) 
                {
                    if (reader.Read()) // ตรวจสอบว่าพบข้อมูลหรือไม่
                    {
                        // ดึงข้อมูลจากฐานข้อมูลมาเก็บในตัวแปร
                        string Name = reader["name"].ToString(); 
                        string PhoneNumber = reader["number"].ToString(); 
                        string Email = reader["email"].ToString(); 

                        
                        Form4 form4 = new Form4(Name, PhoneNumber, Email); // สร้างออบเจ็กต์ของ Form4 พร้อมส่งข้อมูล
                        form4.Show(); 
                        this.Hide(); 
                    }
                    else 
                    {
                        MessageBox.Show("เบอร์โทรศัพท์ไม่ถูกต้องหรือไม่มีอยู่ในระบบ", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error); // แสดงข้อความแจ้งเตือน
                        textBox1.Clear(); 
                        textBox1.Focus(); 
                    }
                }
            }
        }



        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
