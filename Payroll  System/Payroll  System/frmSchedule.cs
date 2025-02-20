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
    public partial class frmSchedule : Form
    {

        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();

        public frmSchedule()
        {
            InitializeComponent();
        }

        private void frmSchedule_Load(object sender, EventArgs e)
        {
            LoadEmployee();
            LoadSchedule();
            InitializeDateTimePicker();
            LockSchedule();
            LockBtn();
            btnReset.Enabled = false;
            btnSelect.Enabled = false;
            dataGridViewSchedule.Enabled = false;
            btnEdit.Hide();
        }

        private void Reset()
        {
            txtEmpID.Clear();
            txtFullName.Clear();
            txtDepartment.Clear();
            txtTitle.Clear();
            txtType.Clear();
            txtSearch2.Clear();
        }

        private void LockSchedule()
        {
            cboDay.Enabled = false;
            dtpTimeIn.Enabled = false;
            dtpTimeOut.Enabled = false;
            cboPeriod.Enabled = false;
            cboSemester.Enabled = false;
            cboType.Enabled = false;
            
        }

        private void UnlockSchedule()
        {
            cboDay.Enabled = true;
            dtpTimeIn.Enabled = true;
            dtpTimeOut.Enabled = true;
            cboPeriod.Enabled = true;
            cboSemester.Enabled = true;
            cboType.Enabled = true;
            dataGridViewSchedule.Enabled = true;
        }
        
        private void LockBtn()
        {
            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            btnClear.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void Clear()
        {
            cboDay.Text = null;
            dtpTimeIn.ResetText();
            dtpTimeOut.ResetText();
            cboSemester.Text = null;
            cboType.Text = null;
            cboPeriod.Text = null;
        }

        private void InitializeDateTimePicker()
        {
            dtpTimeIn.Format = DateTimePickerFormat.Custom;
            dtpTimeIn.CustomFormat = "hh:mm tt";  // For 24-hour format (e.g., 14:30)
            dtpTimeIn.ShowUpDown = true;
            

            dtpTimeOut.Format = DateTimePickerFormat.Custom;
            dtpTimeOut.CustomFormat = "hh:mm tt";  // For 24-hour format (e.g., 14:30)
            dtpTimeOut.ShowUpDown = true;
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

                    dataGridViewEmployee.DataSource = dt;
                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadSchedule()
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

        // Filter DataGridViewSchedule
        private void SelectedEmployee()
        {
            string searchValue = txtEmpID.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadSchedule(); // Reload all data if search box is empty
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
                                schedule.sched_day AS 'Day',
                                COALESCE(schedule.sched_timeIn, '00:00 tt') AS 'Time In',
                                COALESCE(schedule.sched_timeOut, '00:00 tt') AS 'Time Out',
                                schedule.sched_period AS 'Period',
                                schedule.sched_type AS 'Type',
                                schedule.sched_semester AS 'Semester'
                            FROM employee
                            LEFT JOIN schedule ON employee.employee_id = schedule.employee_id
                            WHERE employee.employee_id LIKE @Search";

                        MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                        cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
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

                    
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show($"An error occurred while searching:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        opencon.CloseConnection(); // Ensure connection is closed
                    }
                }
            
        }

        private void dataGridViewEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewEmployee.Rows[e.RowIndex];


                txtEmpID.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                txtFullName.Text = selectedRow.Cells["Full Name"].Value.ToString();
                txtDepartment.Text = selectedRow.Cells["Department"].Value.ToString();
                txtTitle.Text = selectedRow.Cells["Job Title"].Value.ToString();
                txtType.Text = selectedRow.Cells["Job Status"].Value.ToString();

                btnReset.Enabled = true;
                btnSelect.Enabled = true;
                LockBtn();
                LoadSchedule();
                LockSchedule();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpID.Text))
            {
                MessageBox.Show("Please select an available employee.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                UnlockSchedule();
                btnAdd.Enabled = true;
                SelectedEmployee();
                btnClear.Enabled = true;
            }
            
        }

        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch2.Text.Trim();

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

                    dataGridViewEmployee.DataSource = dt;
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

        private void InsertSchedule()
        {
                opencon.dbconnect();

                if (opencon.OpenConnection())
                {
                    string salaryQuery = @"INSERT INTO schedule 
                (employee_id, sched_day, sched_timeIn, sched_timeOut, sched_period, sched_semester,  sched_type) 
                VALUES 
                (@EmployeeID, @Day, @TimeIn, @TimeOut, @Period, @Semester, @Type)";

                try
                {
                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(cboDay.Text) ||
                        string.IsNullOrWhiteSpace(cboSemester.Text) ||
                        string.IsNullOrWhiteSpace(cboType.Text))
                    {
                        MessageBox.Show("Please fill out all required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Create a DateTime value without seconds
                    TimeSpan timeIn = dtpTimeIn.Value.TimeOfDay;
                    TimeSpan timeOut = dtpTimeOut.Value.TimeOfDay;

                    // Format the time to exclude seconds
                    string formattedTimeIn = timeIn.ToString(@"hh\:mm");
                    string formattedTimeOut = timeOut.ToString(@"hh\:mm");

                    // Check if an employee with the same period exist
                    string checkQuery = @"SELECT COUNT(*) FROM schedule WHERE employee_id = @EmployeeID AND sched_day = @Day AND sched_period = @Period And sched_timeIn = @TimeIn";


                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                    checkCmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Day", cboDay.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Period", cboPeriod.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@TimeIn", formattedTimeIn);


                    // Execute the query and get the result
                    int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (employeeExists > 0)
                    {
                        // Employee with the same full name already exists, show error message
                        MessageBox.Show("An employee with the schedule already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {

                        // Prepare the INSERT query with parameterized values
                        using (MySqlCommand cmd = new MySqlCommand(salaryQuery, opencon.connection))
                        {
                            // Assign values to parameters
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                            cmd.Parameters.AddWithValue("@Day", cboDay.Text.Trim());
                            cmd.Parameters.AddWithValue("@TimeIn", formattedTimeIn);
                            cmd.Parameters.AddWithValue("@TimeOut", formattedTimeOut);
                            cmd.Parameters.AddWithValue("@Period", cboPeriod.Text.Trim());
                            cmd.Parameters.AddWithValue("@Semester", cboSemester.Text.Trim());
                            cmd.Parameters.AddWithValue("@Type", cboType.Text.Trim());

                            // Execute the command
                            cmd.ExecuteNonQuery();
                        }

                        // Confirmation message
                        MessageBox.Show("New record has been added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset the form
                        Clear();
                        btnEdit.Hide();
                        btnEdit.Image = Properties.Resources.icons8_edit_30;
                        btnEdit.Text = "   Edit";
                        isEdit = true; // Switch state
                        btnClear.Enabled = false;

                    }
                }
                catch (MySqlException ex)
                {
                    // Detailed exception message
                    MessageBox.Show($"An error occurred while inserting the data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions
                    MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Ensure the connection is closed even if an error occurs
                    if (opencon.connection.State == ConnectionState.Open)
                    {
                        opencon.CloseConnection();
                    }
                }
                }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
                btnClear.Enabled = true;
                InsertSchedule();
                SelectedEmployee();
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            btnEdit.Enabled = false;
            btnClear.Enabled = true;
            btnDelete.Enabled = false;
            btnAdd.Enabled = true;
            btnEdit.Hide();
            btnEdit.Image = Properties.Resources.icons8_edit_30;
            btnEdit.Text = "  Edit";
            isEdit = true;
            btnAdd.Show();
            UnlockSchedule();
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
            Clear();
            LockSchedule();
            LoadSchedule();
            LockBtn();
            btnSelect.Enabled = false;
            btnReset.Enabled = false;
        }

        private void dataGridViewSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewSchedule.Rows[e.RowIndex];

                // Check if all relevant cells are empty
                bool isEmptyRow = string.IsNullOrWhiteSpace(selectedRow.Cells["Day"].Value?.ToString()) &&
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

                    cboDay.Text = selectedRow.Cells["Day"].Value.ToString();
                    if (DateTime.TryParse(selectedRow.Cells["Time In"].Value.ToString(), out DateTime timeIn))
                    {
                        dtpTimeIn.Value = timeIn;
                    }

                    if (DateTime.TryParse(selectedRow.Cells["Time Out"].Value.ToString(), out DateTime timeOut))
                    {
                        dtpTimeOut.Value = timeOut;
                    }
                    cboPeriod.Text = selectedRow.Cells["Period"].Value.ToString();
                    cboType.Text = selectedRow.Cells["Type"].Value.ToString();
                    cboSemester.Text = selectedRow.Cells["Semester"].Value.ToString();
                    /*if (DateTime.TryParse(selectedRow.Cells["Year"].Value.ToString(), out DateTime year))
                    {
                        dtpTimeIn.Value = year;
                    }
                    */


                    btnEdit.Enabled = true;
                    btnAdd.Enabled = false;
                    btnClear.Enabled = true;
                    btnDelete.Enabled = true;
                    LockSchedule();
                    btnAdd.Hide();
                    btnEdit.Show();
                    btnEdit.Text = "   Edit";
                    btnEdit.Image = Properties.Resources.icons8_edit_30;
                    isEdit = true; // Switch state
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewSchedule.Rows.Count > 0) // Check for at least one row
            {
                // Get the selected row
                if (dataGridViewSchedule.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridViewSchedule.SelectedRows[0]; // Get the selected row

                    // Retrieve the employee ID and times from the selected row
                    
                    DateTime.TryParse(selectedRow.Cells["Time In"].Value.ToString(), out DateTime timeIn);
                    DateTime.TryParse(selectedRow.Cells["Time Out"].Value.ToString(), out DateTime timeOut);

                    // Check if both times are valid DateTime
                    if (timeIn != default && timeOut != default)
                    {
                        // Confirm deletion with the user
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this schedule?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            // Initialize the connection
                            opencon.dbconnect();

                            if (opencon.OpenConnection())
                            {
                                try
                                {
                                    // SQL DELETE statement to remove the schedule record
                                    string query = @"DELETE FROM schedule WHERE employee_id = @EmployeeID AND sched_timeIn = @TimeIn AND sched_timeOut = @TimeOut";

                                    // Create MySQL command
                                    using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                                    {
                                        // Assign the parameters to the command
                                        cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                                        cmd.Parameters.AddWithValue("@TimeIn", timeIn);
                                        cmd.Parameters.AddWithValue("@TimeOut", timeOut);

                                        // Execute the DELETE command
                                        cmd.ExecuteNonQuery();
                                    }

                                    // Inform the user about the successful deletion
                                    MessageBox.Show("Employee record has been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Clear the form and reset buttons
                                    Clear();
                                    btnEdit.Enabled = false;
                                    btnClear.Enabled = false;
                                    btnDelete.Enabled = false;
                                    btnAdd.Enabled = true;
                                    SelectedEmployee(); // Reload the schedule to reflect the deletion
                                }
                                catch (MySqlException ex)
                                {
                                    // Handle database-related errors
                                    MessageBox.Show($"An error occurred while deleting the record:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                finally
                                {
                                    // Ensure the database connection is closed
                                    opencon.CloseConnection();
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Time In or Time Out values. Cannot proceed with deletion.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select an employee record to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("The schedule is empty. Please add a record before attempting to delete.", "Empty Schedule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


       bool isEdit = true;

private void btnEdit_Click(object sender, EventArgs e)
{
    if (isEdit)
    {
        btnEdit.Image = Properties.Resources.icons8_update_30;
        btnEdit.Text = "   Update";
        isEdit = false; // Switch state
        UnlockSchedule();
    }
    else
    {
        opencon.dbconnect();
        if (opencon.OpenConnection())
        {
            string query = @"UPDATE schedule SET employee_id = @EmployeeID, sched_day = @Day, " +
                           "sched_timeIn = @TimeIn, sched_timeOut = @TimeOut, sched_period = @Period, " +
                           "sched_semester = @Semester, sched_type = @Type " +
                           "WHERE employee_id = @EmployeeID AND sched_period = @Period AND sched_day = @Day";
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(cboDay.Text) ||
                    string.IsNullOrWhiteSpace(cboSemester.Text) ||
                    string.IsNullOrWhiteSpace(cboType.Text))
                {
                    MessageBox.Show("Please fill out all required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create a DateTime value without seconds
                TimeSpan timeIn = dtpTimeIn.Value.TimeOfDay;
                TimeSpan timeOut = dtpTimeOut.Value.TimeOfDay;

                // Format the time to exclude seconds
                string formattedTimeIn = timeIn.ToString(@"hh\:mm");
                string formattedTimeOut = timeOut.ToString(@"hh\:mm");

                // Prepare the INSERT query with parameterized values
                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                {
                    // Assign values to parameters
                    cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                    cmd.Parameters.AddWithValue("@Day", cboDay.Text.Trim());
                    cmd.Parameters.AddWithValue("@TimeIn", formattedTimeIn);
                    cmd.Parameters.AddWithValue("@TimeOut", formattedTimeOut);
                    cmd.Parameters.AddWithValue("@Period", cboPeriod.Text.Trim());
                    cmd.Parameters.AddWithValue("@Semester", cboSemester.Text.Trim());
                    cmd.Parameters.AddWithValue("@Type", cboType.Text.Trim());

                    // Execute the command
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Clear();
                        SelectedEmployee();
                        btnEdit.Hide();
                        btnEdit.Image = Properties.Resources.icons8_edit_30;
                        btnEdit.Text = "   Edit";
                        isEdit = true; // Switch state
                        btnAdd.Show();
                        btnAdd.Enabled = true;
                        MessageBox.Show("Record has been updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No records were updated. Please check if the record exists.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Detailed exception message
                MessageBox.Show($"An error occurred while updating the data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure the connection is closed even if an error occurs
                if (opencon.connection.State == ConnectionState.Open)
                {
                    opencon.CloseConnection();
                }
            }
        }
        else
        {
            MessageBox.Show("Failed to connect to the database.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

        private void label15_Click(object sender, EventArgs e)
        {

        }
    }
}
