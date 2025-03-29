
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Font.Constants;
using iText.IO.Font;


namespace meatball  //ใบเสร็จ
{
    public partial class Form5 : Form
    {
        private string name;
        private string phoneNumber;
        private string email;

        private string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";

        private MySqlConnection databaseConnection()
        {
            return new MySqlConnection(connectionString);
        }

        private void GetDataFromDatabase() // เมธอดสำหรับดึงข้อมูลจากฐานข้อมูล
        {
            using (MySqlConnection conn = databaseConnection()) // ใช้คำสั่ง using เพื่อปิดการเชื่อมต่ออัตโนมัติหลังใช้งาน
            {
                try
                {
                    conn.Open(); 

                    // ดึงข้อมูลและแสดงใน textBox1, textBox2, textBox3
                    string query = "SELECT name, number, email FROM customerdata WHERE number = @number"; // คำสั่ง SQL สำหรับดึงข้อมูลจากตาราง customerdata
                    MySqlCommand cmd = new MySqlCommand(query, conn); // สร้าง MySqlCommand สำหรับคำสั่ง SQL
                    cmd.Parameters.AddWithValue("@number", this.phoneNumber); // กำหนดค่าพารามิเตอร์ @number จากค่า phoneNumber

                    MySqlDataReader reader = cmd.ExecuteReader(); // ใช้ MySqlDataReader เพื่ออ่านข้อมูลที่ได้จากการ Query
                    if (reader.Read()) 
                    {
                        textBox1.Text = reader.GetString("name"); // ดึงข้อมูลชื่อใน textBox1
                        textBox2.Text = reader.GetString("number"); // ดึงข้อมูลเบอร์โทรใน textBox2
                        textBox3.Text = reader.GetString("email"); // ดึงข้อมูลอีเมลใน textBox3
                    }
                    else 
                    {
                        MessageBox.Show("ไม่พบข้อมูลสำหรับเบอร์โทรศัพท์ที่ระบุ: " + this.phoneNumber, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error); // แสดงข้อความแจ้งเตือน
                        textBox1.Text = string.Empty; 
                        textBox2.Text = string.Empty; 
                        textBox3.Text = string.Empty; 
                    }
                    reader.Close(); 

                    // ดึงข้อมูลและแสดงใน DataGridView3
                    query = "SELECT id, name, productname, amount, total_price FROM cart WHERE name = @name"; // คำสั่ง SQL สำหรับดึงข้อมูลจากตาราง cart
                    cmd = new MySqlCommand(query, conn); 
                    cmd.Parameters.AddWithValue("@name", this.name); 

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd); // ใช้ MySqlDataAdapter เพื่อดึงข้อมูล
                    DataTable table = new DataTable(); // สร้าง DataTable เพื่อเก็บข้อมูลที่ดึงมา
                    adapter.Fill(table); 
                    dataGridView3.DataSource = table; // แสดงข้อมูลใน DataGridView3

                    // คำนวณค่ารวมของ total_price และแสดงใน textBox4
                    decimal totalPriceSum = 0; // ตัวแปรสำหรับเก็บผลรวมของ total_price
                    foreach (DataRow row in table.Rows) // เป็นการใช้ foreach เพื่อวนลูปผ่านทุกแถวใน DataTable ที่ชื่อว่า table.
                    {
                        if (row["total_price"] != DBNull.Value) // ตรวจสอบว่า total_price ไม่ใช่ค่า NULL
                        {
                            totalPriceSum += Convert.ToDecimal(row["total_price"]); // เพิ่มค่าของ total_price เข้าไปในตัวแปรผลรวม
                        }
                    }
                    textBox4.Text = totalPriceSum.ToString("N2"); // แสดงผลรวมใน textBox4 และกำหนดรูปแบบตัวเลขเป็นทศนิยม 2 ตำแหน่ง
                }
                catch (MySqlException ex) // จัดการข้อผิดพลาดที่อาจเกิดจาก MySQL
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message); // แสดงข้อความแจ้งเตือนข้อผิดพลาด
                }
            }
        }


        public Form5(string phoneNumber, string name, string email)
        {
            InitializeComponent();
            this.phoneNumber = phoneNumber;
            this.name = name;
            this.email = email;
            GetDataFromDatabase();
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e) //pdf
        {
            // รับเส้นทางของเดสก์ท็อป
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // กำหนดชื่อไฟล์ PDF
            string fileName = "receipt.pdf";
            // สร้างเส้นทางไฟล์ PDF บนเดสก์ท็อป
            string filePath = System.IO.Path.Combine(desktopPath, fileName);

            // สร้างใบเสร็จ PDF
            GenerateReceiptPDF(filePath);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM cart"; // คำสั่งลบข้อมูลทั้งหมดในตาราง cart
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery(); // ทำการลบข้อมูล
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message); // แสดงข้อผิดพลาดถ้ามี
                }
            }
        }
        private void GenerateReceiptPDF(string filePath)
        {
            try
            {
                if (dataGridView3.DataSource == null)
                {
                    MessageBox.Show("DataGridView3 ไม่มีข้อมูล", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataTable cartData = dataGridView3.DataSource as DataTable;
                if (cartData == null || cartData.Rows.Count == 0)
                {
                    MessageBox.Show("DataGridView3 ไม่มีข้อมูล", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox4.Text))
                {
                    MessageBox.Show("กรุณากรอกข้อมูลในช่องชื่อ, เบอร์โทร หรือราคารวม", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (PdfWriter writer = new PdfWriter(filePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf, PageSize.A4);

                        Paragraph header = new Paragraph("Meatball")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(25)
                            .SetBold();
                        document.Add(header);

                        document.Add(new Paragraph("name: " + textBox1.Text));
                        document.Add(new Paragraph("phonenumber: " + textBox2.Text));
                        document.Add(new Paragraph("date_of_use: " + DateTime.Now.ToString("dd/MM/yyyy")));

                        Table table = new Table(new float[] { 3, 1, 2 }).UseAllAvailableWidth();
                        table.AddHeaderCell(new Cell().Add(new Paragraph("productname").SetBold()));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("amount").SetBold()));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("total_price").SetBold()));

                        decimal totalPriceSum = 0;
                        foreach (DataRow row in cartData.Rows)
                        {
                            if (row["productname"] == DBNull.Value || row["amount"] == DBNull.Value || row["total_price"] == DBNull.Value)
                            {
                                MessageBox.Show("บางแถวไม่มีข้อมูลครบ", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            decimal price = Convert.ToDecimal(row["total_price"]);
                            totalPriceSum += price;

                            table.AddCell(new Cell().Add(new Paragraph(row["productname"].ToString())));
                            table.AddCell(new Cell().Add(new Paragraph(row["amount"].ToString())));
                            table.AddCell(new Cell().Add(new Paragraph(price.ToString("N2"))));
                        }

                        document.Add(table);

                        decimal vatRate = 0.07m;
                        decimal vatAmount = totalPriceSum * vatRate;
                        decimal totalToPay = totalPriceSum + vatAmount;

                        document.Add(new Paragraph("VAT (7%): " + vatAmount.ToString("N2") + " Baht")
                            .SetTextAlignment(TextAlignment.RIGHT)
                            .SetBold());

                        document.Add(new Paragraph("Total Price: " + totalToPay.ToString("N2") + " Baht")
                            .SetTextAlignment(TextAlignment.RIGHT)
                            .SetBold());

                        document.Close();
                    }
                }

                // เปิดไฟล์ PDF ทันที
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = filePath,
                    UseShellExecute = true
                });

                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();

            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
    
}