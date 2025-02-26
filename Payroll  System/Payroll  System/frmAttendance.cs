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
            LoadSchedule();
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


        private void searchAtt(string searchDate, string searchValue)
        {
                    string query = @"SELECT
                                    employee.employee_id AS 'Employee ID',
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
                                WHERE attendance.a_date LIKE @Date AND CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                           COALESCE(employee.employee_mname, ''), ' ', 
                                           COALESCE(employee.employee_lname, '')) LIKE @Search ";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");
                    cmd.Parameters.AddWithValue("@Date", "%" + searchDate + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridAttendance.DataSource = dt;
               
        }

        private void searchSched(string searchValue)
        {
           
                    string query = @"SELECT
                                employee.employee_id AS 'Employee ID',
                                CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                       COALESCE(employee.employee_mname, ''), ' ', 
                                       COALESCE(employee.employee_lname, '')) AS 'Full Name',
                                schedule.sched_day AS 'Day',
                                COALESCE(schedule.sched_timeIn, '00:00 tt') AS 'Time In',
                                COALESCE(schedule.sched_timeOut, '00:00 tt') AS 'Time Out',
                                schedule.sched_period AS 'Period',
                                schedule.sched_type AS 'Type',
                                schedule.sched_semester AS 'Semester'
                             FROM employee
                             LEFT JOIN schedule ON employee.employee_id = schedule.employee_id
                             WHERE CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                           COALESCE(employee.employee_mname, ''), ' ', 
                                           COALESCE(employee.employee_lname, '')) LIKE @Search ";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridViewSchedule.DataSource = dt;
                
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                DisplayAttendance(); // Reload all data if search box is empty
                return;
            }
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    searchAtt(dtpDate.Value.ToString("yyyy-MM-dd"), txtSearch.Text.Trim());
                    searchSched(txtSearch.Text.Trim());
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

        

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtFullName.Text = "";
            txtDate.Text = "";
            txtPeriod.Text = "";
            dtpDay.ResetText();
            dtpTimeIn.ResetText();
            dtpTimeOut.ResetText();
            txtType.Text = "";
            txtSemester.Text = "";
            lblStatIn.Show();
            lblStatOut.Show();
            txtStatIn.Show();
            txtStatOut.Show(); 
            DisplayAttendance();
            LoadSchedule();
            txtStatIn.Text = "";
            txtStatOut.Text = "";

        }


        private void DisplayAttendance()
        {
            string searchValue = dtpDate.Value.ToString("yyyy-MM-dd");
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
                                    employee.employee_id AS 'Employee ID',
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
                                WHERE attendance.a_date LIKE @Search ";

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

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            DisplayAttendance();
        }

     

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

        private void LoadSchedule()
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
                                schedule.sched_day AS 'Day',
                                COALESCE(schedule.sched_timeIn, '00:00 tt') AS 'Time In',
                                COALESCE(schedule.sched_timeOut, '00:00 tt') AS 'Time Out',
                                schedule.sched_period AS 'Period',
                                schedule.sched_type AS 'Type',
                                schedule.sched_semester AS 'Semester'
                             FROM employee
                             LEFT JOIN schedule ON employee.employee_id = schedule.employee_id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, opencon.connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        // Check if "Time In" is null or empty
                        if (string.IsNullOrWhiteSpace(row["Time In"].ToString()))
                        {
                            row["Time In"] = ""; // Display nothing if no valid data
                        }
                        else if (TimeSpan.TryParse(row["Time In"].ToString(), out TimeSpan timeIn))
                        {
                            row["Time In"] = DateTime.Today.Add(timeIn).ToString("hh:mm tt"); // Format valid time
                        }
                        else
                        {
                            row["Time In"] = ""; // Fallback to empty if data is invalid
                        }

                        // Check if "Time Out" is null or empty
                        if (string.IsNullOrWhiteSpace(row["Time Out"].ToString()))
                        {
                            row["Time Out"] = ""; // Display nothing if no valid data
                        }
                        else if (TimeSpan.TryParse(row["Time Out"].ToString(), out TimeSpan timeOut))
                        {
                            row["Time Out"] = DateTime.Today.Add(timeOut).ToString("hh:mm tt"); // Format valid time
                        }
                        else
                        {
                            row["Time Out"] = ""; // Fallback to empty if data is invalid
                        }
                    }


                    dataGridViewSchedule.DataSource = dt;
                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridViewSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewSchedule.Rows[e.RowIndex];

                // Check if all relevant cells are empty
                bool isEmptyRow = string.IsNullOrWhiteSpace(selectedRow.Cells["Employee ID"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Full Name"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Day"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Time In"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Time Out"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Period"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Type"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Semester"].Value?.ToString());

                if (isEmptyRow)
                {
                    MessageBox.Show("Selected row is empty. Please select a valid row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    // Populate the fields if the row is not empty
                    txtEmpID.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                    txtFullName.Text = selectedRow.Cells["Full Name"].Value.ToString();
                    txtDate.Text = selectedRow.Cells["Day"].Value.ToString();
                    if (DateTime.TryParse(selectedRow.Cells["Time In"].Value.ToString(), out DateTime timeIn))
                    {
                        dtpTimeIn.Value = timeIn;
                    }

                    if (DateTime.TryParse(selectedRow.Cells["Time Out"].Value.ToString(), out DateTime timeOut))
                    {
                        dtpTimeOut.Value = timeOut;
                    }
                    txtPeriod.Text = selectedRow.Cells["Period"].Value.ToString();
                    txtType.Text = selectedRow.Cells["Type"].Value.ToString();
                    txtSemester.Text = selectedRow.Cells["Semester"].Value.ToString();



                    txtStatIn.Hide();
                    txtStatOut.Hide();
                    lblStatIn.Hide();
                    lblStatOut.Hide();
                    dataGridAttendance.Enabled = false;
                    dtpDay.Hide();
                    txtDate.Show();


                }
            }
        }

        private void dataGridAttendance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridAttendance.Rows[e.RowIndex];

                // Check if all relevant cells are empty
                bool isEmptyRow = string.IsNullOrWhiteSpace(selectedRow.Cells["Employee ID"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Full Name"].Value?.ToString());

                if (isEmptyRow)
                {
                    MessageBox.Show("Selected row is empty. Please select a valid row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        // Populate the fields if the row is not empty
                        txtEmpID.Text = selectedRow.Cells["Employee ID"].Value?.ToString() ?? "";
                        txtFullName.Text = selectedRow.Cells["Full Name"].Value?.ToString() ?? "";

                        // Handle the "Date" column
                        if (selectedRow.Cells["Date"].Value != null && DateTime.TryParse(selectedRow.Cells["Date"].Value.ToString(), out DateTime dateValue))
                        {
                            dtpDay.Text = dateValue.ToShortDateString(); // Format the date as needed
                        }
                        else
                        {
                            txtDate.Text = ""; // Clear the field if the date is invalid or null
                        }

                        // Handle the "Time In" column
                        if (selectedRow.Cells["Time In"].Value != null && DateTime.TryParse(selectedRow.Cells["Time In"].Value.ToString(), out DateTime timeIn))
                        {
                            dtpTimeIn.Value = timeIn;
                        }
                        else
                        {
                            dtpTimeIn.Value = DateTime.Now; // Set a default value if the time is invalid or null
                        }

                        // Handle the "Time Out" column
                        if (selectedRow.Cells["Time Out"].Value != null && DateTime.TryParse(selectedRow.Cells["Time Out"].Value.ToString(), out DateTime timeOut))
                        {
                            dtpTimeOut.Value = timeOut;
                        }
                        else
                        {
                            dtpTimeOut.Value = DateTime.Now; // Set a default value if the time is invalid or null
                        }

                        // Populate other fields
                        txtPeriod.Text = selectedRow.Cells["Period"].Value?.ToString() ?? "";
                        txtStatIn.Text = selectedRow.Cells["In Status"].Value?.ToString() ?? "";
                        txtStatOut.Text = selectedRow.Cells["Out Status"].Value?.ToString() ?? "";

                        // Show/hide controls as needed
                        txtStatIn.Show();
                        txtStatOut.Show();
                        lblStatIn.Show();
                        lblStatOut.Show();
                        dtpTimeIn.Enabled = true;
                        dtpTimeOut.Enabled = true;
                        dataGridViewSchedule.Enabled = false;
                        dtpDay.Show();
                        txtDate.Hide();
                        btnTiimeIn.Enabled = true;
                        btnTimeOut.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while processing the selected row:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void UpdateAttendance(string action)
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    
                    if (action == "TIME IN")
                    {
                        string query = "UPDATE attendance SET employee_id = @EmployeeID, a_date = @Date, a_timeIn = @TimeIn, a_period = @Period, a_statusIn = @StatusIn WHERE employee_id = @EmployeeID AND a_date = @Date AND a_period = @Period";
                        using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text);
                            cmd.Parameters.AddWithValue("@Date", dtpDay.Value);
                            cmd.Parameters.AddWithValue("@TimeIn", dtpTimeIn.Value);
                            cmd.Parameters.AddWithValue("@Period", txtPeriod.Text);
                            cmd.Parameters.AddWithValue("@StatusIn", "Excuse");

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (action == "TIME OUT")
                    {
                        string query = @"UPDATE attendance SET a_timeOut = @TimeOut, a_statusOut = @StatusOut WHERE employee_id = @EmployeeID 
                            AND a_date = @Date AND a_period = @Period"; // Update only the first found record without a Time Out

                        using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                        {
                            cmd.Parameters.AddWithValue("@TimeOut", dtpTimeOut.Value);
                            cmd.Parameters.AddWithValue("@StatusOut", "Excuse");
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text);
                            cmd.Parameters.AddWithValue("@Date", dtpDay.Value);
                            cmd.Parameters.AddWithValue("@Period", txtPeriod.Text);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                MessageBox.Show("No matching Time In record found to Time Out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                   
                    DisplayAttendance();
                    MessageBox.Show("Record has been updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show($"An error occurred while accessing the database:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }


        private void InsertAttendance()
        {
           
        }

        private void UpdateAttendance()
        {
            
        }


        private void btnTiimeIn_Click(object sender, EventArgs e)
        {
            if(btnTiimeIn.Text == "Time In")
            {
                UpdateAttendance("TIME IN");
            }
            else
            {
                MessageBox.Show("Please select a employee on attendance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void btnTimeOut_Click(object sender, EventArgs e)
        {
            if (btnTimeOut.Text == "Time Out")
            {
                UpdateAttendance("TIME OUT");
            }
            else
            {
                MessageBox.Show("Please select a employee on attendance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

}
