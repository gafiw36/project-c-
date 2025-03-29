using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace meatball  //หน้าสต็อค
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private void showGridView1() // เมธอดสำหรับแสดงข้อมูลใน DataGridView
        {
            
            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();
            conn.Open(); 

            MySqlCommand cmd; 
            cmd = conn.CreateCommand(); 
            cmd.CommandText = "SELECT * FROM stockmeatball"; // คำสั่ง SQL สำหรับดึงข้อมูลทั้งหมดจากตาราง stockmeatball

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds); 

            conn.Close(); 

            // กำหนด DataSource ของ DataGridView ให้แสดงข้อมูลจาก DataSet
            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // เปิดหน้าต่างเลือกรูปภาพ
            OpenFileDialog openFileDialog1 = new OpenFileDialog(); // สร้างอ็อบเจ็กต์ OpenFileDialog เพื่อใช้ในการเลือกไฟล์
            openFileDialog1.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp"; // กำหนดฟิลเตอร์ที่แสดงเฉพาะไฟล์รูปภาพที่สามารถเลือกได้
            openFileDialog1.Title = "เลือกรูปภาพ"; // กำหนดชื่อหัวของหน้าต่างเลือกไฟล์

            if (openFileDialog1.ShowDialog() == DialogResult.OK) // เปิดหน้าต่างเลือกไฟล์และตรวจสอบว่าเลือกไฟล์เสร็จแล้วหรือไม่
            {
                // แสดงรูปภาพที่เลือกใน pictureBox
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName); // นำไฟล์ที่เลือกมาแสดงใน PictureBox โดยใช้ Bitmap เพื่อแปลงไฟล์เป็นรูปภาพ
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            // ดึงแถวที่ผู้ใช้เลือกใน DataGridView
            int selectedRow = dataGridView1.CurrentCell.RowIndex;

            // ดึงค่าของคอลัมน์ "id" และ "amount" ในแถวที่เลือก
            int deleteId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);
            int currentAmount = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["amount"].Value);

            // ดึงค่าจำนวนที่ต้องการลบจาก TextBox
            int deleteAmount;
            if (!int.TryParse(textBox2.Text, out deleteAmount) || deleteAmount <= 0) //การตรวจสอบค่าที่ผู้ใช้ป้อนใน textBox2 ว่าสามารถแปลงเป็นตัวเลข (int) ได้หรือไม่ และตัวเลขที่แปลงได้ต้องมีค่ามากกว่า 0 
            {
                MessageBox.Show("กรุณากรอกจำนวนที่ต้องการลบให้ถูกต้อง");
                return;
            }

            if (deleteAmount > currentAmount) //ถ้ามีค่ามกกว่าจำนวนที่มี
            {
                MessageBox.Show("จำนวนที่ต้องการลบมากกว่าจำนวนที่มีอยู่");
                return;
            }

            MySqlConnection conn = databaseConnection();
            conn.Open();

            if (deleteAmount == currentAmount)
            {
                // ถ้าจำนวนที่ลบเท่ากับจำนวนสินค้าทั้งหมด ลบแถวออก
                string sql = "DELETE FROM stockmeatball WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", deleteId);
                cmd.ExecuteNonQuery();
            }
            else
            {
                // ถ้าจำนวนที่ลบน้อยกว่าจำนวนที่มีอยู่ อัปเดตจำนวนสินค้า
                string sql = "UPDATE stockmeatball SET amount = amount - @deleteAmount WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@deleteAmount", deleteAmount);
                cmd.Parameters.AddWithValue("@id", deleteId);
                cmd.ExecuteNonQuery();
            }

            conn.Close();

            MessageBox.Show("อัปเดตจำนวนสินค้าสำเร็จ");
            showGridView1();

            // เคลียร์ข้อมูลใน TextBox
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }



        private void Form9_Load(object sender, EventArgs e)
        {
            showGridView1(); 
            
        }

        private void button3_Click(object sender, EventArgs e) //ยืนยัน
        {
            MySqlConnection conn = databaseConnection(); 
            byte[] imageBytes = null;

            Image image = pictureBox1.Image; // ดึงรูปภาพจาก PictureBox

            if (image != null) // ตรวจสอบว่ามีรูปภาพใน PictureBox หรือไม่
            {
                using (MemoryStream ms = new MemoryStream()) // ใช้ MemoryStream เพื่อแปลงรูปภาพเป็น byte array
                {
                    image.Save(ms, ImageFormat.Jpeg); // บันทึกรูปภาพในฟอร์แมต JPEG ลงใน MemoryStream
                    imageBytes = ms.ToArray(); // แปลงรูปภาพเป็น byte array
                }
            }
            //เก็บตัวแปร
            string productname = textBox1.Text; // ชื่อสินค้า (จาก textBox1)
            int Quantity = int.Parse(textBox2.Text); // จำนวนสินค้า (จาก textBox2)
            int price = int.Parse(textBox3.Text); // ราคาสินค้า (จาก textBox3)

            try
            {
                conn.Open(); // เปิดการเชื่อมต่อฐานข้อมูล
                string checkExistQuery = "SELECT COUNT(*) FROM stockmeatball WHERE productname = @name";
                MySqlCommand checkExistCmd = new MySqlCommand(checkExistQuery, conn);
                checkExistCmd.Parameters.AddWithValue("@name", productname); // กำหนดพารามิเตอร์สำหรับชื่อสินค้า
                int existingProductCount = Convert.ToInt32(checkExistCmd.ExecuteScalar()); // ตรวจสอบว่ามีสินค้าชื่อนี้หรือไม่

                if (existingProductCount > 0) // กรณีที่สินค้ามีอยู่แล้วในฐานข้อมูล
                {
                    string updateStockQuery = "UPDATE stockmeatball SET amount = amount + @amount, price = @price WHERE productname = @name";
                    MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, conn);
                    updateStockCmd.Parameters.AddWithValue("@amount", Quantity); // เพิ่มจำนวนสินค้า
                    updateStockCmd.Parameters.AddWithValue("@price", price); // อัปเดตราคาสินค้า
                    updateStockCmd.Parameters.AddWithValue("@name", productname); // กำหนดชื่อสินค้า
                    int rowsAffected = updateStockCmd.ExecuteNonQuery(); // อัปเดตข้อมูลในฐานข้อมูล

                    if (rowsAffected > 0) 
                    {
                        MessageBox.Show("อัพเดตจำนวนสินค้าและราคาเรียบร้อยแล้ว"); 
                        showGridView1(); 
                    }
                }
                else // กรณีที่สินค้ายังไม่มีในฐานข้อมูล
                {
                    string insertProductQuery = "INSERT INTO stockmeatball (productname, amount, product, price) VALUES (@name, @amount, @image, @price)";
                    MySqlCommand insertProductCmd = new MySqlCommand(insertProductQuery, conn);
                    insertProductCmd.Parameters.AddWithValue("@name", productname); 
                    insertProductCmd.Parameters.AddWithValue("@amount", Quantity); 
                    insertProductCmd.Parameters.AddWithValue("@price", price);
                    insertProductCmd.Parameters.Add("@image", MySqlDbType.LongBlob).Value = imageBytes != null && imageBytes.Length > 0 ? (object)imageBytes : DBNull.Value; 
                    //ตรวจสอบว่าตัวแปร imageBytes มีค่า (ไม่เป็น null) และมีข้อมูลอยู่ (ขนาดมากกว่า 0 ไบต์).ถ้าตรวจสอบแล้วเป็นจริง(true) แสดงว่ามีข้อมูลรูปภาพที่จะอัปโหลด.

                    int rowsInserted = insertProductCmd.ExecuteNonQuery(); // เพิ่มข้อมูลสินค้าใหม่ในฐานข้อมูล

                    if (rowsInserted > 0) // ตรวจสอบว่ามีแถวที่ถูกเพิ่มในฐานข้อมูลหรือไม่
                    {
                        MessageBox.Show("เพิ่มข้อมูลสินค้าสำเร็จแล้ว");
                        showGridView1(); 
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message); 
            }
            finally
            {
                conn.Close(); 
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true; // กำหนดให้แถวที่คลิกใน DataGridView ถูกเลือก

            
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["productname"].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["amount"].Value.ToString();
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["price"].Value.ToString();
        }


        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int stockItem;

            // ตรวจสอบว่าค่าที่กรอกเป็นตัวเลขหรือไม่
            if (int.TryParse(textBox2.Text, out stockItem))
            {
                // ถ้าจำนวนติดลบให้แสดงข้อความแจ้งเตือน และเคลียร์ค่าที่กรอก
                if (stockItem < 0)
                {
                    MessageBox.Show("ไม่สามารถกรอกจำนวนสินค้าติดลบได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();  // ล้างค่าที่กรอก
                }
            }
           
        }

        private void button5_Click_2(object sender, EventArgs e)
        {

            int stockItem;

            // ตรวจสอบว่าค่าที่กรอกเป็นตัวเลขหรือไม่
            if (int.TryParse(textBox2.Text, out stockItem))
            {
                // ถ้าจำนวนติดลบให้แสดงข้อความแจ้งเตือน และเคลียร์ค่าที่กรอก
                if (stockItem < 0)
                {
                    MessageBox.Show("ไม่สามารถกรอกจำนวนสินค้าติดลบได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();  // ล้างค่าที่กรอก
                }
            }
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        // ฟังก์ชันสำหรับเลือกและแสดงรูปภาพใน pictureBox
        /*private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // สร้างตัวแปร openFileDialog เป็นการใช้งาน OpenFileDialog เพื่อเปิดกล่องเลือกไฟล์จากคอมพิวเตอร์

            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            // กำหนด filter สำหรับประเภทไฟล์ที่สามารถเลือกได้ (เฉพาะไฟล์ภาพ .jpg, .jpeg, .png)

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog.FileName;
                // หากผู้ใช้เลือกไฟล์และคลิก OK, จะใช้ไฟล์ที่เลือกมาแสดงใน pictureBox1
            }
        }
        */



    }
}
