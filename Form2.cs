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
using MySql.Data.MySqlClient;


namespace meatball //สมัครสมาชิก
{
    public partial class Form2 : Form
    {   //ประกาศตัวแปรใช้ในการเก็บ ชื่อ เบอรื อีเมล
        private string name;
        private string phoneNumber;
        private string email;

        public Form2()
        {
            InitializeComponent(); //เรียกใช้
        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private void button1_Click(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("กรุณากรอกข้อมูล");
                return;
            }
            //ประกาศตัวแปร
            string name = textBox1.Text.Trim(); // รับค่าชื่อจาก textBox1 และตัดช่องว่างส่วนเกิน
            string phoneNumber = textBox3.Text.Trim(); // รับค่าเบอร์โทรจาก textBox3
            string email = textBox2.Text.Trim(); // รับค่าอีเมลจาก textBox2

            // ตรวจสอบความยาวของชื่อให้มากกว่า 4 
            if (name.Length < 4 )
            {
                MessageBox.Show("กรุณากรอกชื่อที่มีความยาวมากกว่า 4 ตัวอักษร");
                return;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"[\u0E00-\u0E7F]"))
            {
                MessageBox.Show("อีเมลต้องไม่ประกอบด้วยอักขระภาษาไทย");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^0\d{9}$"))
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรให้ถูกต้อง ");
                return;
            }


            DateTime dateOfUse = DateTime.Now; // เก็บวันที่และเวลาปัจจุบัน

            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();

                // ** ตรวจสอบว่าข้อมูลซ้ำในระบบหรือไม่ **
                string checkQuery = "SELECT COUNT(*) FROM customerdata WHERE name = @name OR number = @number OR email = @email"; // คำสั่ง SQL ตรวจหาข้อมูลซ้ำ
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn); 
                checkCmd.Parameters.AddWithValue("@name", name); 
                checkCmd.Parameters.AddWithValue("@number", phoneNumber); 
                checkCmd.Parameters.AddWithValue("@email", email); 

                int count = Convert.ToInt32(checkCmd.ExecuteScalar()); // นับจำนวนข้อมูลที่พบในฐานข้อมูล
                if (count > 0) // ถ้าพบข้อมูลซ้ำ
                {
                    MessageBox.Show("มีข้อมูลนี้อยู่ในระบบแล้ว"); 
                }
                else
                {
                    // ** เพิ่มข้อมูลใหม่ในฐานข้อมูล **
                    string insertQuery = "INSERT INTO customerdata (name, number, email, Date_of_use) VALUES (@name, @number, @email, @Date_of_use)"; // คำสั่ง SQL สำหรับเพิ่มข้อมูลใหม่
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@name", name); 
                    insertCmd.Parameters.AddWithValue("@number", phoneNumber); 
                    insertCmd.Parameters.AddWithValue("@email", email); 
                    insertCmd.Parameters.AddWithValue("@Date_of_use", dateOfUse); 
                    insertCmd.ExecuteNonQuery();
                    MessageBox.Show("สมัครสมาชิกสำเร็จ");
                }
            }
            Form1 form1 = new Form1(); 
            form1.Show(); 
            this.Hide(); 
        }



        private void Form2_Load(object sender, EventArgs e)
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
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
