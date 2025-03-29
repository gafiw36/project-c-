using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace meatball
{
    public partial class Form4 : Form
    {
        private string name;
        private string phoneNumber;
        private string email;



        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        private void GetDataFromDatabase()
        {
            MySqlConnection conn = databaseConnection();
            try
            {
                conn.Open();
                string query = "SELECT name, number ,email FROM customerdata WHERE number = @number";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@number", this.phoneNumber);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    textBox1.Text = reader.GetString("name");
                    textBox2.Text = reader.GetString("number");
                    textBox6.Text = reader.GetString("email");
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลสำหรับเบอร์โทรศัพท์ที่ระบุ: " + this.phoneNumber, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = string.Empty;
                    textBox2.Text = string.Empty;
                    textBox6.Text = string.Empty;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public Form4(string name, string phoneNumber,string email)
        {

            InitializeComponent();
            this.name = name;
            this.phoneNumber = phoneNumber;
            this.email = email;



            // แสดงข้อมูลใน TextBox หรือ Label ที่ฟอร์ม
            


            // แสดงข้อมูลใน DataGridView2
            showGridView2();
            GetDataFromDatabase();
            

        }

        private void showGridView1()
        {
            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM stockmeatball";
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];

                // ตั้งชื่อคอลัมน์ให้ตรงกับฐานข้อมูล
                dataGridView1.Columns["productname"].HeaderText = "Product Name";
                dataGridView1.Columns["product"].HeaderText = "Product";
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

        private void showGridView2()
        {
            MySqlConnection conn = databaseConnection();
            try
            {
                conn.Open();
                string query = "SELECT id, name, productname, amount, total_Price FROM cart";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView2.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการแสดงข้อมูล: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        private void Form4_Load(object sender, EventArgs e)
        {
            showGridView1();
            showGridView2();
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
        }
        private void RefreshCartDataGrid(string customerName, MySqlConnection conn)
        {
            try
            {
                // สร้างคำสั่ง SQL สำหรับดึงข้อมูลที่ต้องการ
                string selectQuery = @"SELECT id, name, productname, amount, total_price 
                                FROM cart 
                                WHERE name = @name";

                MySqlCommand cmd = new MySqlCommand(selectQuery, conn);
                cmd.Parameters.AddWithValue("@name", customerName);

                // ใช้ MySqlDataAdapter เพื่อเติมข้อมูลลงใน DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // ตั้งค่า DataGridView2 ให้แสดงข้อมูลจาก DataTable
                dataGridView2.DataSource = dt;

                // ปรับแต่งคอลัมน์ให้สวยงาม
                dataGridView2.Columns["id"].HeaderText = "ID"; // ปรับชื่อหัวคอลัมน์ id
                dataGridView2.Columns["name"].HeaderText = "Name";
                dataGridView2.Columns["productname"].HeaderText = "ProductName";
                dataGridView2.Columns["amount"].HeaderText = "Amount";
                dataGridView2.Columns["total_price"].HeaderText = "Total_Price";

                // ป้องกันการแก้ไขโดยตรงใน DataGridView
                dataGridView2.ReadOnly = true;

                // ปรับขนาดคอลัมน์ให้พอดี
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถรีเฟรชข้อมูลในตารางได้: " + ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = databaseConnection();
            string phoneNumber = textBox2.Text.Trim(); // เบอร์โทรจาก textBox2
            string productName = textBox3.Text.Trim();
            string Name = textBox1.Text.Trim(); // ชื่อจาก textBox1
            string email = textBox6.Text.Trim(); // อีเมลจาก textBox6
            int Quantity;
            decimal pricePerUnit = 15m;
            DateTime orderTime = DateTime.Now; // เวลาปัจจุบัน

            // ตรวจสอบการป้อนข้อมูล
            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("กรุณากรอกชื่อ", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text, out Quantity))
            {
                MessageBox.Show("กรุณากรอกจำนวนสินค้าเป็นตัวเลข", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ตรวจสอบว่าค่าที่กรอกไม่เป็นค่าติดลบ
            if (Quantity <= 0)
            {
                MessageBox.Show("กรุณากรอกจำนวนสินค้าให้มากกว่า 0", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // กำหนดราคาเต็ม
            decimal totalPrice = Quantity * pricePerUnit;
            if (Quantity > 30)
            {
                totalPrice -= totalPrice * 0.05m; // ลดราคา 5% หากสั่งซื้อมากกว่า 30
            }

            MySqlTransaction transaction = null;

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // ตรวจสอบว่าเบอร์โทรมีอยู่ในระบบ (เช่น ในตาราง customerdata)
                string checkPhoneQuery = "SELECT COUNT(*) FROM customerdata WHERE number = @number";
                MySqlCommand checkPhoneCmd = new MySqlCommand(checkPhoneQuery, conn, transaction);
                checkPhoneCmd.Parameters.AddWithValue("@number", phoneNumber);
                int phoneCount = Convert.ToInt32(checkPhoneCmd.ExecuteScalar());

                if (phoneCount == 0)
                {
                    MessageBox.Show("ไม่พบเบอร์โทรนี้ในระบบ กรุณาลงทะเบียนก่อน", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    transaction.Rollback();
                    return;
                }

                // ตรวจสอบว่าสินค้าใน stockmeatball มีจำนวนเพียงพอหรือไม่
                string checkStockQuery = "SELECT amount FROM stockmeatball WHERE productname = @productName";
                MySqlCommand checkStockCmd = new MySqlCommand(checkStockQuery, conn, transaction);
                checkStockCmd.Parameters.AddWithValue("@productName", productName);
                int availableStock = Convert.ToInt32(checkStockCmd.ExecuteScalar());

                // ตรวจสอบจำนวนสั่งซื้อเกินจำนวนใน stock หรือไม่
                if (Quantity > availableStock)
                {
                    MessageBox.Show($"จำนวนสินค้าหมดสต้อคแล้ว ({availableStock})", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    transaction.Rollback();
                    return;
                }

                // ตรวจสอบว่ามีการสั่งซื้อสินค้านี้อยู่แล้วหรือไม่ในตาราง cart
                string checkCartQuery = "SELECT COUNT(*) FROM cart WHERE productname = @productName AND name = @name";
                MySqlCommand checkCartCmd = new MySqlCommand(checkCartQuery, conn, transaction);
                checkCartCmd.Parameters.AddWithValue("@productName", productName);
                checkCartCmd.Parameters.AddWithValue("@name", Name);
                int cartCount = Convert.ToInt32(checkCartCmd.ExecuteScalar());

                if (cartCount > 0)
                {
                    // อัพเดทสินค้าที่มีอยู่ในตาราง cart
                    string updateCartQuery = "UPDATE cart SET amount = amount + @amount, total_price = total_price + @total_Price WHERE productname = @productName AND name = @name";
                    MySqlCommand updateCartCmd = new MySqlCommand(updateCartQuery, conn, transaction);
                    updateCartCmd.Parameters.AddWithValue("@amount", Quantity);
                    updateCartCmd.Parameters.AddWithValue("@total_Price", totalPrice);
                    updateCartCmd.Parameters.AddWithValue("@productName", productName);
                    updateCartCmd.Parameters.AddWithValue("@name", Name);
                    int rowsAffected = updateCartCmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("ไม่สามารถอัพเดตตะกร้าสินค้าได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        transaction.Rollback();
                        return;
                    }
                }
                else
                {
                    // เพิ่มสินค้าใหม่ในตาราง cart
                    string insertCartQuery = "INSERT INTO cart (name, number, productname, amount, total_price) VALUES (@name, @number, @productname, @amount, @total_Price)";
                    MySqlCommand insertCmd = new MySqlCommand(insertCartQuery, conn, transaction);
                    insertCmd.Parameters.AddWithValue("@name", Name);
                    insertCmd.Parameters.AddWithValue("@number", phoneNumber);
                    insertCmd.Parameters.AddWithValue("@productname", productName);
                    insertCmd.Parameters.AddWithValue("@amount", Quantity);
                    insertCmd.Parameters.AddWithValue("@total_Price", totalPrice);
                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("ไม่สามารถเพิ่มสินค้าในตะกร้าได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        transaction.Rollback();
                        return;
                    }
                }

                // อัพเดทสต็อกในตาราง stockmeatball
                string updateStockQuery = "UPDATE stockmeatball SET amount = amount - @amount WHERE productname = @productName";
                MySqlCommand updateCmd = new MySqlCommand(updateStockQuery, conn, transaction);
                updateCmd.Parameters.AddWithValue("@amount", Quantity);
                updateCmd.Parameters.AddWithValue("@productName", productName);
                int stockRowsAffected = updateCmd.ExecuteNonQuery();
                if (stockRowsAffected == 0)
                {
                    MessageBox.Show("ไม่สามารถอัพเดตสต็อกได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    transaction.Rollback();
                    return;
                }

                // บันทึกข้อมูลการสั่งซื้อในตาราง orders
                string insertOrderQuery = @"INSERT INTO orders 
(name, number, email, productname, amount, total_price, date_of_use) 
VALUES 
(@name, @number, @email, @productname, @amount, @total_Price, @date_of_use)";
                MySqlCommand insertOrderCmd = new MySqlCommand(insertOrderQuery, conn, transaction);
                insertOrderCmd.Parameters.AddWithValue("@name", Name);
                insertOrderCmd.Parameters.AddWithValue("@number", phoneNumber);
                insertOrderCmd.Parameters.AddWithValue("@email", email); // เพิ่มอีเมล
                insertOrderCmd.Parameters.AddWithValue("@productname", productName);
                insertOrderCmd.Parameters.AddWithValue("@amount", Quantity);
                insertOrderCmd.Parameters.AddWithValue("@total_Price", totalPrice);
                insertOrderCmd.Parameters.AddWithValue("@date_of_use", orderTime);
                insertOrderCmd.ExecuteNonQuery();

                // Commit transaction ถ้าทุกอย่างสำเร็จ
                transaction.Commit();

                MessageBox.Show("บันทึกข้อมูลการสั่งซื้อแล้ว", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // รีเฟรชข้อมูลใน DataGridView2
                RefreshCartDataGrid(Name, conn);
            }
            catch (Exception ex)
            {
                // ถ้ามีข้อผิดพลาด ให้ยกเลิกธุรกรรม
                try
                {
                    transaction?.Rollback();
                }
                catch
                {
                    // Handle rollback failure if necessary
                }

                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell != null)
            {
                int selectedRow = dataGridView2.CurrentCell.RowIndex;
                if (selectedRow >= 0 && selectedRow < dataGridView2.Rows.Count)
                {
                    // แสดงข้อมูลใน textBox3 และ textBox4
                    string productName = dataGridView2.Rows[selectedRow].Cells["productname"].Value.ToString();
                    int currentAmount = Convert.ToInt32(dataGridView2.Rows[selectedRow].Cells["amount"].Value);
                    textBox3.Text = productName;
                    textBox4.Text = currentAmount.ToString(); // ตั้งค่าเริ่มต้นเป็นจำนวนปัจจุบันใน DataGridView
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int selectedRow = dataGridView2.CurrentCell.RowIndex;

            if (selectedRow < 0 || selectedRow >= dataGridView2.Rows.Count)
            {
                MessageBox.Show("กรุณาเลือกแถวที่ต้องการลบ");
                return;
            }

            // แสดงข้อมูลใน textBox3 และ textBox4
            string productName = dataGridView2.Rows[selectedRow].Cells["productname"].Value.ToString();
            int currentAmount = Convert.ToInt32(dataGridView2.Rows[selectedRow].Cells["amount"].Value);
            decimal pricePerUnit = Convert.ToDecimal(dataGridView2.Rows[selectedRow].Cells["total_price"].Value) / currentAmount; // คำนวณราคาต่อหน่วย

            // ตรวจสอบว่ามีจำนวนที่ต้องการลบหรือไม่
            if (!int.TryParse(textBox4.Text, out int amountToDelete) || amountToDelete <= 0 || amountToDelete > currentAmount)
            {
                MessageBox.Show("กรุณากรอกจำนวนที่ต้องการลบอย่างถูกต้องใน textBox4 (ต้องน้อยกว่าหรือเท่ากับจำนวนปัจจุบัน)");
                return;
            }

            // เชื่อมต่อกับฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (amountToDelete == currentAmount)
                        {
                            // ลบแถวทั้งหมดจากตาราง cart
                            string deleteCartQuery = "DELETE FROM cart WHERE id = @id";
                            using (MySqlCommand deleteCartCmd = new MySqlCommand(deleteCartQuery, conn, transaction))
                            {
                                deleteCartCmd.Parameters.AddWithValue("@id", Convert.ToInt32(dataGridView2.Rows[selectedRow].Cells["id"].Value));
                                int rowsAffected = deleteCartCmd.ExecuteNonQuery();

                                // ตรวจสอบว่าการลบสำเร็จหรือไม่
                                if (rowsAffected <= 0)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("ไม่สามารถลบข้อมูลในตาราง cart ได้");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // คำนวณ total_price ที่จะอัปเดต
                            decimal totalPriceToUpdate = amountToDelete * pricePerUnit;

                            // อัปเดตจำนวนและ total_price ในตาราง cart
                            string updateCartQuery = "UPDATE cart SET amount = amount - @amountToDelete, total_price = total_price - @totalPriceToUpdate WHERE id = @id";
                            using (MySqlCommand updateCartCmd = new MySqlCommand(updateCartQuery, conn, transaction))
                            {
                                updateCartCmd.Parameters.AddWithValue("@amountToDelete", amountToDelete);
                                updateCartCmd.Parameters.AddWithValue("@totalPriceToUpdate", totalPriceToUpdate);
                                updateCartCmd.Parameters.AddWithValue("@id", Convert.ToInt32(dataGridView2.Rows[selectedRow].Cells["id"].Value));
                                int rowsAffected = updateCartCmd.ExecuteNonQuery();

                                // ตรวจสอบว่าการอัปเดตสำเร็จหรือไม่
                                if (rowsAffected <= 0)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("ไม่สามารถอัปเดตข้อมูลในตาราง cart ได้");
                                    return;
                                }
                            }
                        }

                        // ตรวจสอบจำนวนสินค้าในตาราง stockmeatball ก่อนลบ
                        string checkStockQuery = "SELECT amount FROM stockmeatball WHERE productname = @productName";
                        using (MySqlCommand checkStockCmd = new MySqlCommand(checkStockQuery, conn))
                        {
                            checkStockCmd.Parameters.AddWithValue("@productName", productName);
                            object result = checkStockCmd.ExecuteScalar();

                            if (result != null && int.TryParse(result.ToString(), out int stockAmount))
                            {
                                if (stockAmount >= 0) // ตรวจสอบว่ามีจำนวนสินค้าหรือไม่
                                {
                                    // อัปเดตข้อมูลในตาราง stockmeatball
                                    string updateStockQuery = "UPDATE stockmeatball SET amount = amount + @amount WHERE productname = @productName";
                                    using (MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, conn, transaction))
                                    {
                                        updateStockCmd.Parameters.AddWithValue("@amount", amountToDelete);
                                        updateStockCmd.Parameters.AddWithValue("@productName", productName);
                                        int stockRowsAffected = updateStockCmd.ExecuteNonQuery();

                                        if (stockRowsAffected > 0)
                                        {
                                            transaction.Commit();
                                            MessageBox.Show("ลบข้อมูลและอัปเดตสต็อกสำเร็จ");
                                        }
                                    }
                                }
                                
                            }
                            
                        }

                        // แสดงข้อมูลใหม่ใน dataGridView2
                        showGridView2();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                    }
                }
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (dataGridView1.Columns.Contains("productname") && row.Cells["productname"]?.Value != null)
                {
                    textBox3.Text = row.Cells["productname"].Value.ToString();
                }
                else
                {
                    MessageBox.Show("ไม่พบคอลัมน์ 'productname' หรือข้อมูลว่าง", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (row.Cells["product"]?.Value != DBNull.Value)
                {
                    try
                    {
                        byte[] imageData = (byte[])row.Cells["product"].Value;
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("เกิดข้อผิดพลาดในการแสดงรูปภาพ: " + ex.Message);
                        pictureBox1.Image = null;
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                    MessageBox.Show("ไม่มีข้อมูลรูปภาพสำหรับรายการที่เลือก");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void searchProduct(string productName)
        {
            MySqlConnection conn = databaseConnection();

            try
            {
                conn.Open();
                string query = "SELECT * FROM stockmeatball WHERE productname LIKE @productname";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@productname", "%" + productName + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "stockmeatball");

                dataGridView1.DataSource = ds.Tables["stockmeatball"];
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

        private void text5_TextChanged(object sender, EventArgs e)
        {
            string productName = textBox5.Text.Trim();
            searchProduct(productName);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // โอนข้อมูลจากตาราง cart ไปยังตาราง orders
            TransferCartToOrders();


            // สร้างและแสดง Form5
            Form5 form5 = new Form5(phoneNumber, name, email);
            form5.Show();
            this.Hide();

        }

        private void TransferCartToOrders()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ดึงข้อมูลจากตาราง customerdata
                    string name = this.name; // Assuming `name` is defined somewhere in the class
                    string emailQuery = "SELECT name, number, email FROM customerdata WHERE name = @name";
                    string customerName = "", phoneNumber = "", email = "";

                    using (MySqlCommand emailCommand = new MySqlCommand(emailQuery, conn))
                    {
                        emailCommand.Parameters.AddWithValue("@name", name);

                        using (MySqlDataReader reader = emailCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customerName = reader.GetString("name");
                                phoneNumber = reader.GetString("number");
                                email = reader.GetString("email");
                            }
                            
                        }
                    }

                    // ดึงข้อมูลจากตาราง cart
                    string selectQuery = "SELECT productname, amount, total_price FROM cart WHERE name = @name";
                    string productNames = "", amounts = "", totalPrices = "";

                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, conn))
                    {
                        selectCommand.Parameters.AddWithValue("@name", name);

                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string productName = reader.GetString("productname");
                                int amount = reader.GetInt32("amount");
                                decimal totalPrice = reader.GetDecimal("total_price");

                                // สะสมข้อมูลทั้งหมดเพื่อคั่นด้วย ,
                                productNames += productName + ",";
                                amounts += amount + ",";
                                totalPrices += totalPrice + ",";
                            }

                            // ตัด , ที่เกินออกจากท้าย
                            if (productNames.Length > 0) productNames = productNames.TrimEnd(',');
                            if (amounts.Length > 0) amounts = amounts.TrimEnd(',');
                            if (totalPrices.Length > 0) totalPrices = totalPrices.TrimEnd(',');
                        }
                    }

                    // เพิ่มข้อมูลลงในตาราง orders
                    string insertQuery = "INSERT INTO orders (name, number, email, productname, amount, total_price) VALUES (@name, @number, @email, @productname, @amount, @total_price)";
                    using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, conn))
                    {
                        insertCommand.Parameters.AddWithValue("@name", customerName);
                        insertCommand.Parameters.AddWithValue("@number", phoneNumber);
                        insertCommand.Parameters.AddWithValue("@email", email);
                        insertCommand.Parameters.AddWithValue("@productname", productNames);
                        insertCommand.Parameters.AddWithValue("@amount", amounts);
                        insertCommand.Parameters.AddWithValue("@total_price", totalPrices);

                        
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
            
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2.CurrentRow.Selected = true;
            textBox3.Text = dataGridView2.Rows[e.RowIndex].Cells["productname"].Value.ToString();
            textBox4.Text = dataGridView2.Rows[e.RowIndex].Cells["amount"].Value.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่า textBox4 มีค่าหรือไม่
            if (!string.IsNullOrEmpty(textBox4.Text))
            {
                // สร้างแถวใหม่ใน DataGridView2
                int amount;
                if (int.TryParse(textBox4.Text, out amount)) // แปลงค่าจาก textBox4 เป็นจำนวน
                {
                    // เพิ่มแถวใหม่ลงใน dataGridView2
                    int rowIndex = dataGridView2.Rows.Add(); // เพิ่มแถวใหม่
                    dataGridView2.Rows[rowIndex].Cells["amount"].Value = amount; // กำหนดค่าจำนวน

                    // เคลียร์ค่าใน textBox4 หลังจากบันทึก
                    textBox4.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("กรุณากรอกจำนวนที่ถูกต้อง"); // แจ้งเตือนหากค่าไม่ถูกต้อง
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลใน textBox4 ก่อนบันทึก");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentCell != null)
            {
                int selectedRow = dataGridView1.CurrentCell.RowIndex;
                if (selectedRow >= 0 && selectedRow < dataGridView1.Rows.Count)
                {
                    // แสดงชื่อผลิตภัณฑ์ใน textBox3
                    string productName = dataGridView1.Rows[selectedRow].Cells["productname"].Value.ToString();
                    textBox3.Text = productName; // แสดงชื่อผลิตภัณฑ์ใน textBox3
                }
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
    }




