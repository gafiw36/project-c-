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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace meatball //รายได้
{
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            // โหลดวัน ใช้ for loop เพิ่มค่าตั้งแต่ 1 ถึง 31 เข้าไปใน comboBox1
            for (int day = 1; day <= 31; day++)
            {
                comboBox1.Items.Add(day.ToString());
            }

            string[] thaiMonths = new string[]
            {
        "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
        "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            };
            //ใช้ Array ของชื่อเดือนภาษาไทย แล้วใช้ foreach เพื่อเพิ่มเข้าไปใน comboBox2
            foreach (string month in thaiMonths)  
            {
                comboBox2.Items.Add(month); 
            }
            
            int currentYear = DateTime.Now.Year;  // ใช้ DateTime.Now.Year เพื่อดึงปีปัจจุบัน
            for (int year = currentYear; year >= 2000; year--) //ใช้ for loop ไล่ค่าปีถอยหลังจากปีปัจจุบันไปจนถึงปี 2000 แล้วเพิ่มเข้าไปใน comboBox3
            {
                comboBox3.Items.Add(year.ToString());
            }
        }
        private int GetMonthNumber(string thaiMonth)
        {
            string[] thaiMonths = new string[]
            {
        "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
        "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            };

            return Array.IndexOf(thaiMonths, thaiMonth) + 1; // ใช้ Array เพื่อหาตำแหน่งของเดือนในอาร์เรย์ แล้วบวก 1 เพราะ Index ของอาร์เรย์เริ่มจาก 0 แปลงเป็นตัวเลขเดือน (1-12)
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // การรับค่าจาก ComboBox
            int selectedDay = comboBox1.SelectedItem != null ? int.Parse(comboBox1.SelectedItem.ToString()) : 0; //เช็คว่าได้เลือกค่าจาก ComboBox1 หรือยัง ถ้าเลือกแล้วจะไม่เป็น null นำค่าที่เลือกมาแปลงเป็นตัวเลข (int).
            int selectedMonth = comboBox2.SelectedItem != null ? GetMonthNumber(comboBox2.SelectedItem.ToString()) : 0;
            int selectedYear = comboBox3.SelectedItem != null ? int.Parse(comboBox3.SelectedItem.ToString()) : 0;

            // Connection string
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=meatball;";

            // Query สำหรับดึงข้อมูลของวัน เดือน ปี
            string queryForDay = "SELECT * FROM orders " +
                                 "WHERE DAY(Date_of_use) = @day AND MONTH(Date_of_use) = @month " +
                                 "AND YEAR(Date_of_use) = @year";

            // Query สำหรับผลรวมรายวัน
            string sumQueryForDay = "SELECT SUM(total_price) AS TotalSum FROM orders " +
                                    "WHERE DAY(Date_of_use) = @day AND MONTH(Date_of_use) = @month " +
                                    "AND YEAR(Date_of_use) = @year";

            // Query สำหรับผลรวมรายเดือน
            string sumQueryForMonth = "SELECT SUM(total_price) AS TotalSum FROM orders " +
                                      "WHERE MONTH(Date_of_use) = @month AND YEAR(Date_of_use) = @year";

            // Query สำหรับผลรวมรายปี
            string sumQueryForYear = "SELECT SUM(total_price) AS TotalSum FROM orders " +
                                     "WHERE YEAR(Date_of_use) = @year";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // ดึงข้อมูลรายวัน
                    if (selectedDay > 0)
                    {
                        MySqlCommand cmdForDay = new MySqlCommand(queryForDay, conn);
                        cmdForDay.Parameters.AddWithValue("@day", selectedDay);
                        cmdForDay.Parameters.AddWithValue("@month", selectedMonth);
                        cmdForDay.Parameters.AddWithValue("@year", selectedYear);

                        MySqlDataAdapter adapterForDay = new MySqlDataAdapter(cmdForDay);
                        DataTable dtForDay = new DataTable();
                        adapterForDay.Fill(dtForDay);
                        dataGridView1.DataSource = dtForDay;

                        MySqlCommand sumCmdForDay = new MySqlCommand(sumQueryForDay, conn);
                        sumCmdForDay.Parameters.AddWithValue("@day", selectedDay);
                        sumCmdForDay.Parameters.AddWithValue("@month", selectedMonth);
                        sumCmdForDay.Parameters.AddWithValue("@year", selectedYear);

                        object totalDayResult = sumCmdForDay.ExecuteScalar();
                        if (totalDayResult != DBNull.Value && totalDayResult != null)
                        {
                            decimal totalDayPrice = Convert.ToDecimal(totalDayResult);
                            textBox1.Text = totalDayPrice.ToString("F2");
                        }
                        else
                        {
                            textBox1.Text = "0.00";
                        }
                    }
                    else
                    {
                        // ถ้าไม่ได้เลือกวันที่ แสดงข้อมูลทั้งหมดในเดือนที่เลือก
                        string queryForMonth = "SELECT * FROM orders " +
                                               "WHERE MONTH(Date_of_use) = @month AND YEAR(Date_of_use) = @year";

                        MySqlCommand cmdForMonth = new MySqlCommand(queryForMonth, conn);
                        cmdForMonth.Parameters.AddWithValue("@month", selectedMonth);
                        cmdForMonth.Parameters.AddWithValue("@year", selectedYear);

                        MySqlDataAdapter adapterForMonth = new MySqlDataAdapter(cmdForMonth);
                        DataTable dtForMonth = new DataTable();
                        adapterForMonth.Fill(dtForMonth);
                        dataGridView1.DataSource = dtForMonth;

                        textBox1.Text = ""; // เคลียร์ค่าใน TextBox1
                    }

                    // คำนวณผลรวมรายเดือน
                    MySqlCommand sumCmdForMonth = new MySqlCommand(sumQueryForMonth, conn);
                    sumCmdForMonth.Parameters.AddWithValue("@month", selectedMonth);
                    sumCmdForMonth.Parameters.AddWithValue("@year", selectedYear);

                    object totalMonthResult = sumCmdForMonth.ExecuteScalar();
                    if (totalMonthResult != DBNull.Value && totalMonthResult != null)
                    {
                        decimal totalMonthPrice = Convert.ToDecimal(totalMonthResult); //แปลงค่า totalMonthResult จาก object ให้เป็นประเภท decimal เพื่อให้สามารถใช้งานทางการคำนวณและแสดงผล.
                        textBox2.Text = totalMonthPrice.ToString("F2");
                    }
                    else
                    {
                        textBox2.Text = "0.00";
                    }

                    // คำนวณผลรวมรายปี
                    MySqlCommand sumCmdForYear = new MySqlCommand(sumQueryForYear, conn);
                    sumCmdForYear.Parameters.AddWithValue("@year", selectedYear);

                    object totalYearResult = sumCmdForYear.ExecuteScalar();
                    if (totalYearResult != DBNull.Value && totalYearResult != null)
                    {
                        decimal totalYearPrice = Convert.ToDecimal(totalYearResult);
                        textBox3.Text = totalYearPrice.ToString("F2");
                    }
                    else
                    {
                        textBox3.Text = "0.00";
                    }

                    // ดึงข้อมูลรายปีและแสดงใน DataGridView1
                    if (selectedYear > 0)
                    {
                        string queryForYear = "SELECT * FROM orders " +
                                              "WHERE YEAR(Date_of_use) = @year";

                        MySqlCommand cmdForYear = new MySqlCommand(queryForYear, conn);
                        cmdForYear.Parameters.AddWithValue("@year", selectedYear);

                        MySqlDataAdapter adapterForYear = new MySqlDataAdapter(cmdForYear);
                        DataTable dtForYear = new DataTable();
                        adapterForYear.Fill(dtForYear);
                        dataGridView1.DataSource = dtForYear;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
    

