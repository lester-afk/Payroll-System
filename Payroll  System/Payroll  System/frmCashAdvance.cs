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
    public partial class frmCashAdvance : Form
    {

        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        public frmCashAdvance()
        {
            InitializeComponent();
        }

        private void frmCashAdvance_Load(object sender, EventArgs e)
        {

            btnEdit.Hide();
            LoadEmployee();
            btnSelect.Enabled = false;
            btnReset.Enabled = false;
            txtAmount.Enabled = false;
            dtpStart.Enabled = false;
            LockBtn();
            LoadCA();
            txtBalance.Hide();
            lblBalance.Hide();
        }

        private void GenID()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int maxnum = Convert.ToInt32(mf.getMaxCA()); // Ensure this method returns the correct max number
            int idnum = maxnum + 1;
            string ID = $"CA-{Year}{Month}-{idnum}";
            txtCAID.Text = ID;
        }

        private void LockBtn()
        {
            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnDelete.Enabled = false;

        }

        private void Clear()
        {
            GenID();
            txtAmount.Clear();
            txtBalance.Clear();
            dtpStart.ResetText();
            
        }

        private void ClearEmp()
        {
            txtEmpID.Clear();
            txtFullName.Clear();
            txtTitle.Clear();
            txtType.Clear();
            txtDepartment.Clear();
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


        private void LoadCA()
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
                                cash_advance.ca_id AS 'CID',
                                cash_advance.ca_amount AS 'Amount',
                                COALESCE(cash_advance.ca_date, '') AS 'Date',
                                cash_advance.ca_balance AS 'Balance'
                                
                             FROM employee
                             LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id";

                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, opencon.connection);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridViewCashAdvance.DataSource = dt;

                    
                    opencon.CloseConnection();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            
        }

        private void SelectedEmployee()
        {
            string searchValue = txtEmpID.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadCA(); // Reload all data if search box is empty
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
                                cash_advance.ca_id AS 'CID',
                                cash_advance.ca_amount AS 'Amount',
                                COALESCE(cash_advance.ca_date, 'MM/dd/yyyy') AS 'Date',
                                cash_advance.ca_balance AS 'Balance'
                             FROM employee
                             LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                             WHERE employee.employee_id LIKE @Search";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridViewCashAdvance.DataSource = dt;

                    // Format the "Date" column to display only the date part
                    if (dataGridViewCashAdvance.Columns["Date"] != null)
                    {
                        dataGridViewCashAdvance.Columns["Date"].DefaultCellStyle.Format = "d"; // Short date format
                    }

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
                LoadCA();
                txtAmount.Enabled = false;
                dtpStart.Enabled = false;
                dataGridViewCashAdvance.Enabled = false;
                txtCAID.Clear();
                txtAmount.Clear();
                txtBalance.Clear();
                dtpStart.ResetText();
                btnPrint.Enabled = false;
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
                    SelectedEmployee();
                    btnAdd.Enabled = true;
                    btnReset.Enabled = true;
                    txtAmount.Enabled = true;
                    dtpStart.Enabled = true;
                    GenID();
                    dataGridViewCashAdvance.Enabled = true;
                    btnPrint.Enabled = true;
            }

        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                string salaryQuery = @"INSERT INTO cash_advance 
                (ca_id,employee_id, ca_amount, ca_date, ca_balance) 
                VALUES 
                (@CID, @EmployeeID, @Amount, @Date, @Balance)";

                try
                {
                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(txtAmount.Text) ||
                        string.IsNullOrWhiteSpace(dtpStart.Text))
                    {
                        MessageBox.Show("Please fill out all required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Check if an employee with the same cash advance exist
                    string checkQuery = @"SELECT COUNT(*) FROM cash_advance WHERE employee_id = @EmployeeID AND ca_amount = @Amount AND ca_date = @Date";


                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                    checkCmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Date", dtpStart.Value);

                    // Execute the query and get the result
                    int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (employeeExists > 0)
                    {
                        // Employee with the same full name already exists, show error message
                        MessageBox.Show("An employee with the same amount on this date already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {

                        // Prepare the INSERT query with parameterized values
                        using (MySqlCommand cmd = new MySqlCommand(salaryQuery, opencon.connection))
                        {
                            // Assign values to parameters

                            cmd.Parameters.AddWithValue("@CID", txtCAID.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                            cmd.Parameters.AddWithValue("@Date", dtpStart.Value);
                            cmd.Parameters.AddWithValue("@Balance", txtAmount.Text.Trim());


                            // Execute the command
                            cmd.ExecuteNonQuery();
                        }

                        // Confirmation message
                        MessageBox.Show("New record has been added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset the form
                        Clear();
                        SelectedEmployee();
                        txtBalance.Hide();
                        lblBalance.Hide();
                        
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


        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearEmp();
            Clear();
            LockBtn();
            txtAmount.Enabled = false;
            dtpStart.Enabled = false;
            txtBalance.Enabled = false;
            txtBalance.Hide();
            lblBalance.Hide();
            btnReset.Enabled = false;
            LoadCA();
            btnSelect.Enabled = false;
            txtCAID.Clear();
            dataGridViewCashAdvance.Enabled = false;
            btnAdd.Show();
            btnEdit.Hide();
            btnEdit.Image = Properties.Resources.icons8_edit_30;
            btnEdit.Text = "   Edit";
            isEdit = true; // Switch state
        }

        bool isEdit = true;
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (isEdit)
            {
                btnEdit.Image = Properties.Resources.icons8_update_30;
                btnEdit.Text = "   Update";
                isEdit = false; // Switch state
                
                txtAmount.Enabled = true;
                dtpStart.Enabled = true;
                txtBalance.Enabled = true;


            }
            else
            {
                opencon.dbconnect();
                if (opencon.OpenConnection())
                {
                    string query = @"UPDATE cash_advance SET ca_id = @CID, employee_id = @EmployeeID, ca_amount = @Amount, " +
                                    "ca_date = @Date, ca_balance = @Balance " +
                                    "WHERE ca_id = @CID AND employee_id = @EmployeeID ";
                    try
                    {
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(txtCAID.Text) ||
                            string.IsNullOrWhiteSpace(txtAmount.Text) ||
                            string.IsNullOrWhiteSpace(dtpStart.Text))
                        {
                            MessageBox.Show("Please fill out all required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Prepare the INSERT query with parameterized values
                        using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                        {
                            // Assign values to parameters
                            cmd.Parameters.AddWithValue("@CID", txtCAID.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                            cmd.Parameters.AddWithValue("@Date", dtpStart.Value);;
                            cmd.Parameters.AddWithValue("@Balance", txtBalance.Text.Trim());

                            // Execute the command
                            cmd.ExecuteNonQuery();
                            Clear();
                            SelectedEmployee();
                            btnEdit.Image = Properties.Resources.icons8_edit_30;
                            btnEdit.Text = "   Edit";
                            isEdit = true; // Switch state
                            btnEdit.Hide();
                            btnAdd.Show();
                            btnAdd.Enabled = true;
                            txtBalance.Hide();
                            lblBalance.Hide();

                            MessageBox.Show("Record has been updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }
        private void dataGridViewCashAdvance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = this.dataGridViewCashAdvance.Rows[e.RowIndex];

                // Check if all relevant cells are empty
                bool isEmptyRow = string.IsNullOrWhiteSpace(selectedRow.Cells["CID"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Amount"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Date"].Value?.ToString()) &&
                                  string.IsNullOrWhiteSpace(selectedRow.Cells["Balance"].Value?.ToString());

                if (isEmptyRow)
                {
                    MessageBox.Show("Selected row is empty. Please select a valid row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    // Populate the fields if the row is not empty
                    txtCAID.Text = selectedRow.Cells["CID"].Value?.ToString();
                    txtAmount.Text = selectedRow.Cells["Amount"].Value?.ToString();

                    if (DateTime.TryParse(selectedRow.Cells["Date"].Value?.ToString(), out DateTime dateStart))
                    {
                        dtpStart.Value = dateStart;
                    }

                    txtBalance.Text = selectedRow.Cells["Balance"].Value?.ToString();

                    // Enable necessary buttons
                    btnEdit.Enabled = true;
                    btnCancel.Enabled = true;
                    btnDelete.Enabled = true;
                    btnReset.Enabled = true;
                    btnSelect.Enabled = true;
                    txtBalance.Show();
                    lblBalance.Show();

                    // Disable Add and input fields
                    btnAdd.Enabled = false;
                    txtAmount.Enabled = false;
                    dtpStart.Enabled = false;

                    btnAdd.Hide();
                    btnEdit.Show();
                }
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
            btnEdit.Hide();
            btnAdd.Show();
            btnEdit.Image = Properties.Resources.icons8_edit_30;
            btnEdit.Text = "   Edit";
            isEdit = true; // Switch state
            btnAdd.Enabled = true;
            btnCancel.Enabled = false;
            txtAmount.Enabled = false;
            dtpStart.Enabled = false;
            txtBalance.Enabled = false;
            txtBalance.Hide();
            lblBalance.Hide();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewCashAdvance.Rows.Count > 0) // Check for at least one row
            {
                // Get the selected row
                if (dataGridViewCashAdvance.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridViewCashAdvance.SelectedRows[0]; // Get the selected row

                 
                        // Confirm deletion with the user
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this Cash Advance?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            // Initialize the connection
                            opencon.dbconnect();

                            if (opencon.OpenConnection())
                            {
                                try
                                {
                                    // SQL DELETE statement to remove the schedule record
                                    string query = @"DELETE FROM cash_advance WHERE employee_id = @EmployeeID AND ca_id = @CID";

                                    // Create MySQL command
                                    using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                                    {
                                        // Assign the parameters to the command
                                        cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                                        cmd.Parameters.AddWithValue("@CID", txtCAID.Text.Trim());

                                        // Execute the DELETE command
                                        cmd.ExecuteNonQuery();
                                    }

                                    // Inform the user about the successful deletion
                                    MessageBox.Show("Employee record has been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Clear the form and reset buttons
                                    Clear();
                                    btnEdit.Enabled = false;
                                    btnCancel.Enabled = false;
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
                    MessageBox.Show("Please select an employee record to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("The schedule is empty. Please add a record before attempting to delete.", "Empty Schedule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Define the font and brush for the text
            Font font = new Font("Arial", 9, FontStyle.Regular);
            Font headerFont = new Font("Arial", 11, FontStyle.Bold);
            Brush brush = Brushes.Black;

            // Define the starting position for drawing
            int startX = 150;
            int startY = 180;
            int offsetY = 30; // Vertical spacing between rows
            int columnWidth = 130; // Default column width

            Image image = Properties.Resources.Header;
            e.Graphics.DrawImage(image, 150, 40, image.Width, image.Height);

            // Draw the title and date
            e.Graphics.DrawString("Cash Advance Record", new Font("Arial", 16, FontStyle.Bold), brush, new Point(150, startY));

            if (txtEmpID.Text != "" && txtCAID.Text != "")
            {
                e.Graphics.DrawLine(Pens.Black, startX, startY + 40 , 700, startY + 40);
                e.Graphics.DrawString("Name:  " + txtFullName.Text, headerFont, brush, new Point(startX, startY + 50));
                e.Graphics.DrawLine(Pens.Black, startX, startY + 80, 700, startY + 80);
                // Adjust the starting Y position for the grid data
                startY += 100;

                // Draw the column headers with borderlines (excluding the "Date" column)
                for (int i = 0; i < dataGridViewCashAdvance.Columns.Count; i++)
                {
                    if (dataGridViewCashAdvance.Columns[i].HeaderText != "Full Name") // Skip the "Date" column
                    {

                        string headerText = dataGridViewCashAdvance.Columns[i].HeaderText;
                        e.Graphics.DrawString(headerText, headerFont, brush, new Point(startX + 25, startY + 6));
                        e.Graphics.DrawRectangle(Pens.Black, startX, startY, columnWidth, offsetY);
                        startX += columnWidth; // Move to the next column
                    }
                }

                // Draw the rows with borderlines (excluding the "Date" column)
                startY += offsetY;
                startX = 150; // Reset X position for rows
                for (int i = 0; i < dataGridViewCashAdvance.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridViewCashAdvance.Columns.Count; j++)
                    {
                        if (dataGridViewCashAdvance.Columns[j].HeaderText != "Full Name") // Skip the "Date" column
                        {
                            if (dataGridViewCashAdvance.Rows[i].Cells[j].Value != null)
                            {
                                string cellValue = dataGridViewCashAdvance.Rows[i].Cells[j].Value.ToString();
                                
                                
                                    e.Graphics.DrawString(cellValue, font, brush, new Point(startX + 5, startY + 5));
                                
                                 
                            }
                            e.Graphics.DrawRectangle(Pens.Black, startX, startY, columnWidth, offsetY);
                            startX += columnWidth; // Move to the next column
                        }
                    }
                    startY += offsetY; // Move to the next row
                    startX = 150; // Reset X position for the next row
                }
            }
            else
            {
                e.Graphics.DrawLine(Pens.Black, startX, startY + 40, 700, startY + 40);
                // Adjust the starting Y position for the grid data
                startY += 60;

                // Draw the column headers with borderlines (excluding the "Date" column)
                for (int i = 0; i < dataGridViewCashAdvance.Columns.Count; i++)
                {
                    if (dataGridViewCashAdvance.Columns[i].HeaderText != "CID") // Skip the "Date" column
                    {
                        string headerText = dataGridViewCashAdvance.Columns[i].HeaderText;
                        e.Graphics.DrawString(headerText, headerFont, brush, new Point(startX + 25, startY + 6));
                        e.Graphics.DrawRectangle(Pens.Black, startX, startY, columnWidth, offsetY);
                        startX += columnWidth; // Move to the next column
                    }
                }

                // Draw the rows with borderlines (excluding the "Date" column)
                startY += offsetY;
                startX = 150; // Reset X position for rows
                for (int i = 0; i < dataGridViewCashAdvance.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridViewCashAdvance.Columns.Count; j++)
                    {
                        if (dataGridViewCashAdvance.Columns[j].HeaderText != "CID") // Skip the "Date" column
                        {
                            if (dataGridViewCashAdvance.Rows[i].Cells[j].Value != null)
                            {
                                string cellValue = dataGridViewCashAdvance.Rows[i].Cells[j].Value.ToString();
                                e.Graphics.DrawString(cellValue, font, brush, new Point(startX + 5, startY + 5));
                            }
                            e.Graphics.DrawRectangle(Pens.Black, startX, startY, columnWidth, offsetY);
                            startX += columnWidth; // Move to the next column
                        }
                    }
                    startY += offsetY; // Move to the next row
                    startX = 150; // Reset X position for the next row
                }
            }

            e.Graphics.DrawLine(Pens.Black, startX, startY += columnWidth - 100, 700, startY);
        }

        

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
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
    }
}
