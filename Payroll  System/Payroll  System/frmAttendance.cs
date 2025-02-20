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


namespace Payroll__System
{
    public partial class frmAttendance : Form
    {

        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        public frmAttendance()
        {
            InitializeComponent();
            
        }

        private void frmAttendance_Load(object sender, EventArgs e)
        {
            
            LoadEmployee();
            InitializeDateTimePicker();
            DisplayAttendance();
        }

        private void InitializeDateTimePicker()
        {
            // Set DateTimePicker to display only MM/dd/yyyy format
            dtpDate.Format = DateTimePickerFormat.Custom;    // Set format to Custom
            dtpDate.CustomFormat = "MM/dd/yyyy";
            
        }


        private void LoadEmployee()
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT
                                employee.employee_id AS 'Employee ID', 
                                CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                       COALESCE(employee.employee_mname, ''), ' ', 
                                       COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                job.job_department AS 'Department',
                                job.job_title AS 'Job Title',
                                job.job_status AS 'Job Status'
                             FROM employee
                             LEFT JOIN job ON employee.employee_id = job.employee_id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, opencon.connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridEmp.DataSource = dt;
                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       /* private void LoadAttendance()
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT
                                CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                       COALESCE(employee.employee_mname, ''), ' ', 
                                       COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                attendance.a_date AS 'Date',
                                attendance.a_timeIn AS 'Time In',
                                attendance.a_timeOut AS 'Time Out',
                                attendance.a_period AS 'Period',
                                attendance.a_statusIn AS 'In Status',
                                attendance.a_statusOut AS 'Out Status'
                             FROM employee
                             LEFT JOIN attendance ON employee.employee_id = attendance.employee_id";
  
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, opencon.connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridAttendance.DataSource = dt;
                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }*/

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadEmployee(); // Reload all data if search box is empty
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT 
                                employee.employee_id AS 'Employee ID',
                                CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                       COALESCE(employee.employee_mname, ''), ' ', 
                                       COALESCE(employee.employee_lname, '')) AS 'Full Name',
                                job.job_department AS 'Department',
                                job.job_title AS 'Job Title',
                                job.job_status AS 'Job Status'
                            FROM employee
                            LEFT JOIN job ON employee.employee_id = job.employee_id
                            WHERE employee.employee_id LIKE @Search
                            OR employee.employee_fname LIKE @Search
                            OR employee.employee_lname LIKE @Search
                            OR job.job_status LIKE @Search
                            OR job.job_department LIKE @Search
                            OR job.job_title LIKE @Search";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridEmp.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while searching:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void dataGridEmp_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridEmp.Rows[e.RowIndex];
                txtEmpID.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                txtFullName.Text = selectedRow.Cells["Full Name"].Value.ToString();
            }
        }

        private void txtEmpID_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtEmpID.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                DisplayAttendance() ;// Reload all data if search box is empty
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT
                                CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                       COALESCE(employee.employee_mname, ''), ' ', 
                                       COALESCE(employee.employee_lname, '')) AS 'Full Name',
                                attendance.a_date AS 'Date',
                                attendance.a_timeIn AS 'Time In',
                                attendance.a_timeOut AS 'Time Out',
                                attendance.a_period AS 'Period',
                                attendance.a_statusIn AS 'In Status',
                                attendance.a_statusOut AS 'Out Status'
                             FROM employee
                             LEFT JOIN attendance ON employee.employee_id = attendance.employee_id
                             WHERE employee.employee_id LIKE @Search";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridAttendance.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while searching:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtFullName.Text = "";
            DisplayAttendance();
        }


        private void DisplayAttendance()
        {
            string searchValue = dtpDate.Value.ToString("yyyy-MM-dd");
            string empName = txtFullName.Text.Trim();
           // MessageBox.Show(searchValue, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadEmployee(); // Reload all data if search box is empty
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT
                                    CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                           COALESCE(employee.employee_mname, ''), ' ', 
                                           COALESCE(employee.employee_lname, '')) AS 'Full Name',
                                    COALESCE(attendance.a_date, '') AS 'Date',  
                                    attendance.a_timeIn AS 'Time In',
                                    attendance.a_timeOut AS 'Time Out',
                                    attendance.a_period AS 'Period',
                                    attendance.a_statusIn AS 'In Status',
                                    attendance.a_statusOut AS 'Out Status'
                                FROM employee
                                LEFT JOIN attendance ON employee.employee_id = attendance.employee_id
                                WHERE attendance.a_date LIKE @Search AND CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                           COALESCE(employee.employee_mname, ''), ' ', 
                                           COALESCE(employee.employee_lname, '')) LIKE @Name ";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");
                    cmd.Parameters.AddWithValue("@Name", "%" + empName + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridAttendance.DataSource = dt;

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while searching:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            DisplayAttendance();
        }

        /*private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Define the font and brush for the text
            Font font = new Font("Arial", 12, FontStyle.Regular);
            Brush brush = Brushes.Black;

            // Define the starting position for drawing
            int startX = 50;
            int startY = 100;
            int offsetY = 30; // Vertical spacing between rows

            // Draw the title and date
            e.Graphics.DrawString("Attendance Sheet", new Font("Arial", 28, FontStyle.Bold), brush, new Point(startX, startY));
            e.Graphics.DrawString("Date: " + dtpDate.Value.ToShortDateString(), font, brush, new Point(startX, startY + 40));

            // Adjust the starting Y position for the grid data
            startY += 80;

            // Draw the column headers
            for (int i = 0; i < dataGridAttendance.Columns.Count; i++)
            {
                e.Graphics.DrawString(dataGridAttendance.Columns[i].HeaderText, font, brush, new Point(startX + (i * 150), startY));
            }

            // Draw the rows
            startY += offsetY;
            for (int i = 0; i < dataGridAttendance.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridAttendance.Columns.Count; j++)
                {
                    if (dataGridAttendance.Rows[i].Cells[j].Value != null)
                    {
                        e.Graphics.DrawString(dataGridAttendance.Rows[i].Cells[j].Value.ToString(), font, brush, new Point(startX + (j * 150), startY));
                    }
                }
                startY += offsetY;
            }
        }*/

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Define the font and brush for the text
            Font font = new Font("Arial", 9, FontStyle.Regular);
            Font headerFont = new Font("Arial", 11, FontStyle.Bold);
            Brush brush = Brushes.Black;

            // Define the starting position for drawing
            int startX = 30;
            int startY = 160;
            int offsetY = 30; // Vertical spacing between rows
            int columnWidth = 130; // Default column width

            Image image = Properties.Resources.Header;
            e.Graphics.DrawImage(image, 150, 40, image.Width, image.Height);
            // Draw the title and date
            e.Graphics.DrawString("Attendance Sheet", new Font("Arial", 16, FontStyle.Bold), brush, new Point(30, startY));
            e.Graphics.DrawLine(Pens.Black, startX, startY + 40, 810, startY + 40);
            e.Graphics.DrawString("Date: " + dtpDate.Value.ToShortDateString(), headerFont, brush, new Point(startX, startY + 50));

            // Adjust the starting Y position for the grid data
            startY += 80;

            // Draw the column headers with borderlines (excluding the "Date" column)
            for (int i = 0; i < dataGridAttendance.Columns.Count; i++)
            {
                if (dataGridAttendance.Columns[i].HeaderText != "Date") // Skip the "Date" column
                {
                    string headerText = dataGridAttendance.Columns[i].HeaderText;
                    e.Graphics.DrawString(headerText, headerFont, brush, new Point(startX + 25, startY + 6));
                    e.Graphics.DrawRectangle(Pens.Black, startX, startY, columnWidth, offsetY);
                    startX += columnWidth; // Move to the next column
                }
            }

            // Draw the rows with borderlines (excluding the "Date" column)
            startY += offsetY;
            startX = 30; // Reset X position for rows
            for (int i = 0; i < dataGridAttendance.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridAttendance.Columns.Count; j++)
                {
                    if (dataGridAttendance.Columns[j].HeaderText != "Date") // Skip the "Date" column
                    {
                        if (dataGridAttendance.Rows[i].Cells[j].Value != null)
                        {
                            string cellValue = dataGridAttendance.Rows[i].Cells[j].Value.ToString();
                            e.Graphics.DrawString(cellValue, font, brush, new Point(startX + 5, startY + 5));
                        }
                        e.Graphics.DrawRectangle(Pens.Black, startX, startY, columnWidth, offsetY);
                        startX += columnWidth; // Move to the next column
                    }
                }
                startY += offsetY; // Move to the next row
                startX = 30; // Reset X position for the next row
            }

            e.Graphics.DrawLine(Pens.Black, startX, startY += columnWidth - 100, 810, startY);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }
    }
}
