using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Payroll__System
{
    public partial class frmEmployee : Form
    {
        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction(); 

        public string EmpID;
        public string file_name;
        public string filepath;
        public string tempPath;

        public frmEmployee()
        {
            InitializeComponent(); 
        }

        private void frmEmployee_Load(object sender, EventArgs e)
        {
            //Employee Register Info
            DisInfo();
            LoadData();
            btnDisable();
            btnClear.Hide();
            btnEdit.Hide();

            //Employee Salary Info
            LoadSalary();
            LockSalary();
            InitializeDateTimePicker();
            btnEdit2.Enabled = false;
            btnClear2.Enabled = false;
            btnAdd2.Enabled = false;
            btnEdit2.Hide();

        }

        // Generate ID
        private void GenID()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int maxnum = Convert.ToInt32(mf.getMaxNumber()); // Ensure this method returns the correct max number
            int idnum = maxnum + 1;
            string ID = $"{Year}-{Month}-{idnum}";
            txtEmpID.Text = ID;
        }

        // Disable form fields
        private void DisInfo()
        {
            txtFname.Enabled = false;
            txtMname.Enabled = false;
            txtLname.Enabled = false;
            txtContact.Enabled = false;
            txtEmail.Enabled = false;
            txtAddress.Enabled = false;
            cboStatus.Enabled = false;
            txtDepartment.Enabled = false;
            txtTitle.Enabled = false;
            txtSalary.Enabled = false;
            txtPerHour.Enabled = false;
            ckboSSS.Enabled = false;
            ckboPagibig.Enabled = false;
            ckboPhilhealth.Enabled = false;
          
        }

        // Enable form fields
        private void EnInfo()
        {
            txtFname.Enabled = true;
            txtMname.Enabled = true;
            txtLname.Enabled = true;
            txtContact.Enabled = true;
            txtEmail.Enabled = true;
            txtAddress.Enabled = true;
            cboStatus.Enabled = true;
            txtDepartment.Enabled = true;
            txtTitle.Enabled = true;
            txtSalary.Enabled = true;
            txtPerHour.Enabled = true;
            ckboSSS.Enabled = true;
            ckboPagibig.Enabled = true;
            ckboPhilhealth.Enabled = true;
         
        }

        // Clear form fields
        private void ClearInfo()
        {
            txtEmpID.Text = "";
            txtFname.Text = "";
            txtMname.Text = "";
            txtLname.Text = "";
            txtContact.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtRfid.Text = "";     
        }

        //Button disable
        private void btnDisable()
        {
            btnClear.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnUpload.Enabled = false;
            btnAddRfid.Enabled = false;

        }

        //Validate email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //DataGrid Load Data
        private void LoadData()
        {
            
            opencon.dbconnect(); 
           
            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT
                                rfid.rfid_tag AS 'RFID Tag',
                                employee.employee_id AS 'Employee ID', 
                                employee.employee_fname AS 'First Name', 
                                employee.employee_mname AS 'Middle Name', 
                                employee.employee_lname AS 'Last Name', 
                                employee.employee_contact AS 'Contact Number', 
                                employee.employee_email AS 'Email', 
                                employee.employee_address AS 'Address',
                                employee.employee_picture AS 'Picture'
                             FROM employee
                             LEFT JOIN rfid ON employee.employee_id = rfid.employee_id";//Use left join instead of inner join to display the employee info

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

        //Fill When Rfid scan
        public void FillRfid(string fillRfid)
        {
            if (txtRfid.Text == fillRfid) 
            {
                MessageBox.Show("Duplicate RFID data detected!", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtRfid.Text = fillRfid;
                
            }
            
        }

        //Search
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadData(); // Reload all data if search box is empty
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT employee_id AS 'Employee ID', employee_fname AS 'First Name', employee_mname AS 'Middle Name', employee_lname AS 'Last Name', employee_contact AS 'Contact Number', employee_email AS 'Email', employee_address AS 'Address', employee_picture AS 'Picture' 
                                 FROM employee
                                 WHERE employee_id LIKE @Search OR employee_fname LIKE @Search OR employee_lname LIKE @Search";

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

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog photo = new OpenFileDialog())
            {
                photo.InitialDirectory = @"C:\Users\Resu\Desktop\Payroll_System\Photo";
                photo.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

                if (photo.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file's name
                    file_name = Path.GetFileName(photo.FileName);

                    // Define the destination directory
                    string destinationDirectory = Path.Combine(Application.StartupPath, "Photo");

                    // Ensure the destination directory exists
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    // Define the full destination path
                    filepath = Path.Combine(destinationDirectory, file_name);

                    try
                    {
                        // Copy the file to the destination directory, overwrite if exists
                        File.Copy(photo.FileName, filepath, true);

                        // Update the picture box
                        employeePicture.ImageLocation = filepath;

                        // Update the text boxes
                        txtPath.Text = filepath;
                        tempPath = filepath; // This will be used when saving to the database
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show($"File operation failed: {ioEx.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError($"File operation failed: {ioEx}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogError($"Unexpected error in btnUpload_Click: {ex}");
                    }
                }
            }

        }

        /// Logs errors to a text file for debugging purposes.
        /// </summary>
        /// <param name="message">Error message to log.</param>
        private void LogError(string message)
        {
            string logFilePath = System.IO.Path.Combine(Application.StartupPath, "error_log.txt");
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }


        bool isAdding = true;
        //Button Add/Save Employee
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isAdding)
            {
                // Prepare the form for adding a new employee
                EnInfo(); // Enable the form fields for input
                GenID();  // Generate a new Employee ID
                btnClear.Enabled = true;
                btnUpload.Enabled = true;
                btnAddRfid.Enabled = true;
                btnAdd.Image = Properties.Resources.icons8_save_30;
                btnAdd.Text = "Save";
                isAdding = false; // Switch state
                btnEdit.Hide();
            }
            else

            {
                EmpID = txtEmpID.Text;
                opencon.dbconnect();
                // Attempt to save the new employee to the database
                if (opencon.OpenConnection())
                {

                    string employeeQuery = @"INSERT INTO employee 
                            (employee_id, employee_fname, employee_mname, employee_lname, employee_contact, employee_email, employee_address, employee_picture) 
                            VALUES 
                            (@EmployeeID, @Fname, @Mname, @Lname, @Contact, @Email, @Address, @Picture)";

                    string rfidQuery = @"INSERT INTO rfid (rfid_tag, employee_id) 
                    VALUES (@RFID, @EmployeeID)";
                    try
                    {
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(txtFname.Text) ||
                            string.IsNullOrWhiteSpace(txtLname.Text) ||
                            string.IsNullOrWhiteSpace(txtContact.Text))
                        {
                            MessageBox.Show("Please fill out all required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(txtRfid.Text))
                        {
                            string checkRfidQuery = @"SELECT COUNT(*) FROM rfid WHERE rfid_tag = @RFID";
                            MySqlCommand checkRfidCmd = new MySqlCommand(checkRfidQuery, opencon.connection);
                            checkRfidCmd.Parameters.AddWithValue("@RFID", txtRfid.Text.Trim());

                            // Execute the query and get the result
                            int rfidExists = Convert.ToInt32(checkRfidCmd.ExecuteScalar());
                            if (rfidExists > 0)
                            {
                                MessageBox.Show("An employee with the same RFID already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                LoadData();
                                return;
                            }
                        }

                        // Check if an employee with the same full name (first, middle, last) already exists
                        string checkQuery = @"SELECT COUNT(*) FROM employee 
                                      WHERE employee_fname = @Fname 
                                      AND employee_mname = @Mname 
                                      AND employee_lname = @Lname";


                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                        checkCmd.Parameters.AddWithValue("@Fname", txtFname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Mname", txtMname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Lname", txtLname.Text.Trim());
                       

                        // Execute the query and get the result
                        int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (employeeExists > 0)
                        {
                            // Employee with the same full name already exists, show error message
                            MessageBox.Show("An employee with the same Information already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            LoadData();
                            return;
                            
                        }
                        else
                        {
                            // Prepare the INSERT query with parameterized values
                            using (MySqlCommand cmd = new MySqlCommand(employeeQuery, opencon.connection))
                            {
                                // Assign values to parameters
                                cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                                //cmd.Parameters.AddWithValue("@RFID", "123"); // Replace with actual RFID if available
                                cmd.Parameters.AddWithValue("@Fname", txtFname.Text.Trim());
                                cmd.Parameters.AddWithValue("@Mname", txtMname.Text.Trim());
                                cmd.Parameters.AddWithValue("@Lname", txtLname.Text.Trim());

                                // Convert employee_contact to long (BIGINT)
                                string contactText = txtContact.Text.Trim();

                                // Check if the contact is numeric
                                if (System.Text.RegularExpressions.Regex.IsMatch(contactText, @"^\d+$"))
                                {
                                    cmd.Parameters.AddWithValue("@Contact", contactText);
                                }
                                else
                                {
                                    MessageBox.Show("Please enter a numeric contact number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                if (!IsValidEmail(txtEmail.Text.Trim()))
                                {
                                    MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                }
                                
                                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("@Picture", tempPath);

                                // Execute the command
                                cmd.ExecuteNonQuery();


                                
                                    using (MySqlCommand cmdRfid = new MySqlCommand(rfidQuery, opencon.connection))
                                    {
                                        cmdRfid.Parameters.AddWithValue("@RFID", txtRfid.Text.Trim());
                                        cmdRfid.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                                        cmdRfid.ExecuteNonQuery();
                                    }
                                


                            }


                            // Confirmation message
                            MessageBox.Show("New record has been added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Reset the form
                            opencon.CloseConnection();
                            ClearInfo();
                            DisInfo();
                            LoadData();
                            LoadSalary();
                            btnDisable();
                            txtPath.Clear();
                            tempPath = null;
                            employeePicture.ImageLocation = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
                            btnAdd.Image = Properties.Resources.add_30;
                            btnAdd.Text = "New Employee";
                            isAdding = true; // Switch back
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

        bool editAdd = true;
        //Button Edit/Update Employee
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(editAdd)
            {
                btnEdit.Image = Properties.Resources.icons8_update_30;
                btnEdit.Text = "     Update";
                editAdd = false;
                EnInfo();
                btnClear.Enabled = true;
                btnUpload.Enabled = true;
                btnDelete.Enabled = true;
                btnAddRfid.Enabled = true;
                btnAdd.Enabled = false;
                

            }
            else 
            {
                EmpID = txtEmpID.Text;
                opencon.dbconnect();
                // Attempt to save the new employee to the database
                if (opencon.OpenConnection())
                {
                    string query = @"UPDATE employee SET employee_id = @EmployeeID, employee_fname = @Fname, employee_mname = @Mname, employee_lname = @Lname, employee_contact = @Contact, employee_email = @Email, employee_address = @Address, employee_picture = @Picture
                                         WHERE employee_id= @EmployeeID";
                   
                    string rfidQuery = @"UPDATE rfid SET rfid_tag = @RFID, employee_id = @EmployeeID WHERE employee_id = @EmployeeID";
                    
                    try
                    {

                        // Check if RFID is being used by another employee (ignoring the current employee)
                        if (!string.IsNullOrWhiteSpace(txtRfid.Text))
                        {
                            string checkRfidQuery = @"SELECT COUNT(*) FROM rfid 
                                              WHERE rfid_tag = @RFID 
                                              AND employee_id != @EmployeeID";

                            MySqlCommand checkRfidCmd = new MySqlCommand(checkRfidQuery, opencon.connection);
                            checkRfidCmd.Parameters.AddWithValue("@RFID", txtRfid.Text.Trim());
                            checkRfidCmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());

                            int rfidExists = Convert.ToInt32(checkRfidCmd.ExecuteScalar());
                            if (rfidExists > 0)
                            {
                                MessageBox.Show("An employee with the same RFID already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                LoadData();
                                return;
                            }
                        }

                        // Check if an employee with the same full name (first, middle, last) already exists
                        string checkQuery = @"SELECT COUNT(*) FROM employee 
                                      WHERE employee_fname = @Fname 
                                      AND employee_mname = @Mname 
                                      AND employee_lname = @Lname
                                      AND employee_id != @EmployeeID";

                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                        checkCmd.Parameters.AddWithValue("@Fname", txtFname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Mname", txtMname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Lname", txtLname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());


                        // Execute the query and get the result
                        int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (employeeExists > 0)
                        {
                            // Employee with the same full name already exists, show error message
                            MessageBox.Show("An employee with the same Information already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            LoadData();
                            return;

                        }
                        

                            using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                            {
                                // Assign values to parameters
                                cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                                cmd.Parameters.AddWithValue("@Fname", txtFname.Text.Trim());
                                cmd.Parameters.AddWithValue("@Mname", txtMname.Text.Trim());
                                cmd.Parameters.AddWithValue("@Lname", txtLname.Text.Trim());

                            // Convert employee_contact to long (BIGINT)
                                string contactText = txtContact.Text.Trim();

                                // Check if the contact is numeric
                                if (System.Text.RegularExpressions.Regex.IsMatch(contactText, @"^\d+$"))
                                {
                                    cmd.Parameters.AddWithValue("@Contact", contactText);
                                }
                                else
                                {
                                    MessageBox.Show("Please enter a numeric contact number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                            if (!IsValidEmail(txtEmail.Text.Trim()))
                                {
                                    MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                }

                                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("@Picture", tempPath);

                                // Execute the command
                                cmd.ExecuteNonQuery();

                               
                            }

                        using (MySqlCommand cmdRfid = new MySqlCommand(rfidQuery, opencon.connection))
                        {
                            cmdRfid.Parameters.AddWithValue("@RFID", txtRfid.Text.Trim());
                            cmdRfid.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                            cmdRfid.ExecuteNonQuery();

                        }
                        
                        // Confirmation message
                        MessageBox.Show("Record has been updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset the form
                        opencon.CloseConnection();
                        ClearInfo();
                        DisInfo();
                        btnEdit.Image = Properties.Resources.icons8_edit_30;
                        btnEdit.Text = "Edit";
                        editAdd = true;
                        btnAdd.Enabled = true;
                        btnAdd.Show();
                        LoadData();
                        LoadSalary();
                        btnDisable();
                        tempPath = null;
                        employeePicture.ImageLocation = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
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


        //Button Delete Employee
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployee.SelectedRows.Count >= 0)
            {
                // Get the Employee ID of the selected row
                DataGridViewRow selectedRow = dataGridViewEmployee.SelectedRows[0];
                string employeeID = selectedRow.Cells["Employee ID"].Value.ToString();

                if (!string.IsNullOrEmpty(employeeID))
                {
                    // Confirm deletion with the user
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this employee?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Initialize the connection
                        opencon.dbconnect();

                        if (opencon.OpenConnection())
                        {
                            try
                            {
                                // SQL DELETE statement to remove the employee record
                                string query = @"DELETE FROM employee WHERE employee_id = @EmployeeID";

                                // Create MySQL command
                                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                                {
                                    // Assign the employee ID to the command parameter
                                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                                    // Execute the DELETE command
                                    cmd.ExecuteNonQuery();
                                }

                                // Inform the user about the successful deletion
                                MessageBox.Show("Employee record has been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                LoadSalary();
                                LoadData();
                                ClearInfo();
                                DisInfo();
                                btnEdit.Text = "Edit";
                                btnDisable();
                                btnAdd.Enabled = true;
                                txtPath.Text = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
                                employeePicture.ImageLocation = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
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
                    MessageBox.Show("No employee record selected for deletion.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an employee record to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //DataGrid Fill Textbox when select
        private void dataGridViewEmployee_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewEmployee.Rows[e.RowIndex];

                // Populate textboxes with data from the selected row
                txtRfid.Text = selectedRow.Cells["RFID TAG"].Value.ToString();
                txtEmpID.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                txtFname.Text = selectedRow.Cells["First Name"].Value.ToString();
                txtMname.Text = selectedRow.Cells["Middle Name"].Value.ToString();
                txtLname.Text = selectedRow.Cells["Last Name"].Value.ToString();
                txtContact.Text = selectedRow.Cells["Contact Number"].Value.ToString();
                txtEmail.Text = selectedRow.Cells["Email"].Value.ToString();
                txtAddress.Text = selectedRow.Cells["Address"].Value.ToString();
                txtPath.Text = selectedRow.Cells["Picture"].Value.ToString();
                //string picPath = selectedRow.Cells["Picture"].Value.ToString();

                //txtPath.Text = picPath.Replace(" ", "\\");
                string defaultImagePath = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
                if (!string.IsNullOrEmpty(txtPath.Text) && System.IO.File.Exists(txtPath.Text))
                {
                    employeePicture.Image = Image.FromFile(txtPath.Text);
                    tempPath = txtPath.Text;
                }
                else
                {
                    employeePicture.Image = Image.FromFile(defaultImagePath);
                }

                
                btnEdit.Enabled = true;
                btnAdd.Enabled = false;
                btnUpload.Enabled = false;
                btnClear.Enabled = true;
                DisInfo();
                btnEdit.Show();
                btnEdit.Image = Properties.Resources.icons8_edit_30;
                btnEdit.Text = "Edit";
                editAdd = true;
                btnAdd.Hide();
                
            }
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInfo();
            btnDisable();
            btnEdit.Image = Properties.Resources.icons8_edit_30;
            btnEdit.Text = "    Edit";
            editAdd = true;
            btnEdit.Hide();
            btnAdd.Show();
            btnAdd.Image = Properties.Resources.add_30;
            btnAdd.Text = "       New Employee";
            isAdding = true; // Switch back
            DisInfo();
            btnAdd.Enabled = true;
            string defaultImagePath = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
            employeePicture.Image = Image.FromFile(defaultImagePath);
        }

        private void btnAddRfid_Click(object sender, EventArgs e)
        {
            frmRfidScan rfidScan = new frmRfidScan(this);

            rfidScan.ShowDialog();
        }

        //==============================================================================================================================================
        //Lock Salary Textbox
        private void LockSalary()
        {
            txtEmpID2.Enabled = false;
            txtFullName.Enabled = false;
            cboStatus.Enabled = false;
            txtTitle.Enabled = false;
            txtDepartment.Enabled = false;
            txtSalary.Enabled = false;
            txtPerHour.Enabled = false;
            dtpDate.Enabled = false;
            btnAddPerHour.Enabled = false;

            txtSSS.Enabled = false;
            txtPagIbig.Enabled = false;
            txtPhilHealth.Enabled = false;

            ckboSSS.Enabled = false;
            ckboPagibig.Enabled = false;
            ckboPhilhealth.Enabled = false;
        }

        private void UnlockSalary()
        {
            cboStatus.Enabled = true;
            txtTitle.Enabled = true;
            txtDepartment.Enabled = true;
            //txtSalary.Enabled = true;
            txtPerHour.Enabled = true;
            dtpDate.Enabled = true;
            btnAddPerHour.Enabled = true;

            ckboSSS.Enabled = true;
            ckboPagibig.Enabled = true;
            ckboPhilhealth.Enabled = true;
        }

        private void InitializeDateTimePicker()

        {
            // Set DateTimePicker to display only MM/dd/yyyy format
            dtpDate.Format = DateTimePickerFormat.Custom;    // Set format to Custom
            dtpDate.CustomFormat = "MM/dd/yyyy";              // Custom format as MM/dd/yyyy
        }

        //PerHour
        private void showPerHour()
        {
            
            lblPerHour.Show();
            txtPerHour.Show();
        }
        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboStatus.Enabled == true)
            {
                if (cboStatus.Text == "Part Time")
                {
                    showPerHour();
                    txtSalary.Enabled = false;
                    txtSalary.Text = "";
                    lblHR.Hide();
                    btnAddPerHour.Hide();
                }
                else if (cboStatus.Text == "Regular")
                {
                    lblPerHour.Hide();
                    txtPerHour.Hide();
                    txtPerHour.Text = "";
                    lblHR.Show();
                    btnAddPerHour.Show();
                    txtSalary.Enabled = true;
                }
            }    
            
        }

        private void LoadSalary()
        {

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT employee.employee_id AS 'Employee ID', " +
                             "CONCAT(employee.employee_fname, ' ', employee.employee_mname, ' ', employee.employee_lname) AS 'Full Name', " +
                             "job.job_status AS 'Job Status', " +
                             "job.job_department AS 'Department', " +
                             "job.job_title AS 'Job Title', " +
                             "job.job_salary AS 'Basic Salary', " +
                             "job.job_hourly_rate AS 'Hourly Rate', " +
                             "job.job_date_hired AS 'Date Hired', " +
                             "contribution.sss AS 'SSS', " +
                             "contribution.pagibig AS 'Pag Ibig', " +
                             "contribution.philhealth AS 'PhilHealth' " +
                             "FROM employee " +
                             "LEFT JOIN job ON employee.employee_id = job.employee_id " +
                             "LEFT JOIN contribution ON employee.employee_id = contribution.employee_id";//Use left join instead of inner join to display the employee info

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, opencon.connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridViewSalary.DataSource = dt;
                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddPerHour_Click(object sender, EventArgs e)
        {
            showPerHour();
        }

        private void frmEmployee_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearSalary()
        {
            txtEmpID2.Text = "";
            txtFullName.Text = "";
            cboStatus.SelectedIndex = -1;
            txtDepartment.SelectedIndex = -1;
            txtTitle.SelectedIndex = -1;
            txtSalary.Text = "";
            txtPerHour.Text = "";
            ckboSSS.Checked = false;
            ckboPagibig.Checked = false;
            ckboPhilhealth.Checked = false;
            dtpDate.ResetText();
            ckboSSS.Checked = false;
            ckboPagibig.Checked = false;
            ckboPhilhealth.Checked = false;
            txtSSS.Clear();
            txtPagIbig.Clear();
            txtPhilHealth.Clear();

        }

        private void btnClear2_Click(object sender, EventArgs e)
        {
            ClearSalary ();
            LockSalary();
            btnAdd2.Text = "    Add Details";
            btnAdd2.Image = Properties.Resources.add_30;
            btnEdit2.Text = "Edit";
            btnEdit2.Image = Properties.Resources.icons8_edit_30;
            btnEdit2.Enabled = false;
            btnEdit2.Hide();
            btnAdd2.Show();
            btnAdd2.Enabled = false;
            btnClear2.Enabled = false;
            btnDefault.Enabled = false;
            btnCustom.Enabled = false;
        }


        bool isAdd = true;
        private void btnAdd2_Click(object sender, EventArgs e)
        {
            if (isAdd)
            {
               
                btnEdit2.Enabled = false;
                btnClear2.Enabled = true;
                UnlockSalary();
                btnAdd2.Image = Properties.Resources.icons8_save_30;
                btnAdd2.Text = "    Save";
                isAdd = false; // Switch state
                btnEdit2.Hide();
                btnDefault.Enabled = true;
                btnCustom.Enabled = true;

            }
            else 
            {
                
                opencon.dbconnect();

                
                if (opencon.OpenConnection())
                {

                    try
                    {
                        string salaryQuery = @"INSERT INTO job 
                            (employee_id, job_status, job_department, job_title, job_salary, job_hourly_rate, job_date_hired) 
                            VALUES 
                            (@EmployeeID, @Status, @Department, @Title, @Salary, @HourlyRate, @DateHired)";

                        string contributionQuery = @"INSERT INTO contribution 
                                (employee_id, sss, pagibig,philhealth) 
                                VALUES 
                                (@EmployeeID, @SSS, @Pagibig, @PhilHealth)";

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(cboStatus.Text))
                        {
                            MessageBox.Show("Please fill out all required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Check if an employee with the same full name (first, middle, last) already exists
                        string checkQuery = @"SELECT COUNT(*) FROM employee 
                                      WHERE employee_fname = @Fname 
                                      AND employee_mname = @Mname 
                                      AND employee_lname = @Lname
                                      AND employee_id != @EmployeeID";


                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                        checkCmd.Parameters.AddWithValue("@Fname", txtFname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Mname", txtMname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Lname", txtLname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());


                        // Execute the query and get the result
                        int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (employeeExists > 0)
                        {
                            // Employee with the same full name already exists, show error message
                            MessageBox.Show("An employee with the same ID already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            // Prepare the INSERT query with parameterized values
                            using (MySqlCommand cmd = new MySqlCommand(salaryQuery, opencon.connection))
                            {
                                // Assign values to parameters
                                cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID2.Text.Trim());
                                cmd.Parameters.AddWithValue("@Status", cboStatus.Text.Trim());
                                cmd.Parameters.AddWithValue("@Department", txtDepartment.Text.Trim());
                                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                                if (!string.IsNullOrWhiteSpace(txtSalary.Text))
                                {

                                    // Convert employee_contact to long (BIGINT)
                                    string salary = txtSalary.Text.Trim();

                                    // Check if the contact is numeric
                                    if (System.Text.RegularExpressions.Regex.IsMatch(salary, @"^\d+$"))
                                    {
                                        cmd.Parameters.AddWithValue("@Salary", salary);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please enter a numeric contact number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Salary", DBNull.Value);
                                }


                                if (!string.IsNullOrWhiteSpace(txtPerHour.Text))
                                {
                                    // Convert employee_contact to long (BIGINT)
                                    string perHour = txtPerHour.Text.Trim();

                                    // Check if the contact is numeric
                                    if (System.Text.RegularExpressions.Regex.IsMatch(perHour, @"^\d+$"))
                                    {
                                        cmd.Parameters.AddWithValue("@HourlyRate", perHour);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Please enter a numeric contact number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    } 
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@HourlyRate", DBNull.Value);
                                }

                                cmd.Parameters.AddWithValue("@DateHired", dtpDate.Value);

                                // Execute the command
                                cmd.ExecuteNonQuery();
                            }

                            using (MySqlCommand cmdCon = new MySqlCommand(contributionQuery, opencon.connection))
                            {
                                cmdCon.Parameters.AddWithValue("@EmployeeID", txtEmpID2.Text.Trim());
                                

                                if (!string.IsNullOrWhiteSpace(txtSSS.Text))
                                {
                                    cmdCon.Parameters.AddWithValue("@SSS", txtSSS.Text.Trim());
                                }
                                else
                                {
                                    cmdCon.Parameters.AddWithValue("@SSS", DBNull.Value);
                                }

                                if (!string.IsNullOrWhiteSpace(txtPagIbig.Text))
                                {
                                    cmdCon.Parameters.AddWithValue("@PagIbig", txtPagIbig.Text.Trim());
                                }
                                else
                                {
                                    cmdCon.Parameters.AddWithValue("@PagIbig", DBNull.Value);
                                }
                                if (!string.IsNullOrWhiteSpace(txtPhilHealth.Text))
                                {
                                    cmdCon.Parameters.AddWithValue("@PhilHealth", txtPhilHealth.Text.Trim());
                                }
                                else
                                {
                                    cmdCon.Parameters.AddWithValue("@PhilHealth", DBNull.Value);
                                }

                                cmdCon.ExecuteNonQuery();
                            }

                            // Confirmation message
                            MessageBox.Show("New record has been added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Reset the form
                            opencon.CloseConnection();
                            ClearSalary();
                            btnAdd2.Image = Properties.Resources.add_30;
                            btnAdd2.Text = "    Add Details";
                            isAdd = true; // Switch back
                            LoadSalary();
                            LockSalary();
                            btnDefault.Enabled = false;
                            btnCustom.Enabled = false;
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

        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch2.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                LoadSalary(); // Reload all data if search box is empty
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
                                job.job_status AS 'Job Status',
                                job.job_department AS 'Department',
                                job.job_title AS 'Job Title',
                                job.job_salary AS 'Basic Salary',
                                job.job_hourly_rate AS 'Hourly Rate',
                                job.job_date_hired AS 'Date Hired' 
                                contribution.sss AS 'SSS', 
                                contribution.pagibig AS 'Pag Ibig', 
                                contribution.philhealth AS 'PhilHealth' 
                            FROM employee
                            LEFT JOIN job ON employee.employee_id = job.employee_id
                            LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
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

                    dataGridViewSalary.DataSource = dt;
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

        bool isEdit = true;

        private void btnEdit2_Click(object sender, EventArgs e)
        {
            if (isEdit)
            {
                btnEdit2.Image = Properties.Resources.icons8_update_30;
                btnEdit2.Text = "     Update";
                isEdit = false;
                btnClear2.Enabled = true;
                btnAdd2.Enabled = false;
                UnlockSalary();
                txtSalary.Enabled = true;
                btnCustom.Enabled = true;
                
                
            }
            else 
            {
                opencon.dbconnect();

                if (opencon.OpenConnection())
                {
                    string salaryQuery = @"UPDATE job SET
                                   employee_id = @EmployeeID, 
                                   job_status = @Status,
                                   job_department = @Department, 
                                   job_title = @Title, 
                                   job_salary = @Salary,    
                                   job_hourly_rate = @HourlyRate,   
                                   job_date_hired = @DateHired
                                   WHERE employee_id = @EmployeeID";

                    string contributionQuery = @"UPDATE contribution SET
                                employee_id = @EmployeeID, sss = @SSS, pagibig = @Pagibig, philhealth = @PhilHealth
                                WHERE employee_id = @EmployeeID";
                    try
                    {
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(cboStatus.Text))
                        {
                            MessageBox.Show("Please fill out the required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Check for duplicate employee by employee ID
                        string checkQuery = @"SELECT COUNT(*) FROM employee 
                                      WHERE employee_fname = @Fname 
                                      AND employee_mname = @Mname 
                                      AND employee_lname = @Lname
                                      AND employee_id != @EmployeeID";

                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                        checkCmd.Parameters.AddWithValue("@Fname", txtFname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Mname", txtMname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Lname", txtLname.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());

                        // Execute the query and get the result
                        int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (employeeExists > 0)
                            {
                                MessageBox.Show("An employee with the same title already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        

                        using (MySqlCommand cmd = new MySqlCommand(salaryQuery, opencon.connection))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", txtEmpID2.Text.Trim());
                            cmd.Parameters.AddWithValue("@Status", cboStatus.Text.Trim());
                            cmd.Parameters.AddWithValue("@Department", txtDepartment.Text.Trim());
                            cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                            if (!string.IsNullOrWhiteSpace(txtSalary.Text))
                            {

                                // Convert employee_contact to long (BIGINT)
                                string salary = txtSalary.Text.Trim();

                                // Check if the contact is numeric
                                if (System.Text.RegularExpressions.Regex.IsMatch(salary, @"^\d+$"))
                                {
                                    cmd.Parameters.AddWithValue("@Salary", salary);
                                }
                                else
                                {
                                    MessageBox.Show("Please enter a numeric contact number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@Salary", DBNull.Value);
                            }


                            if (!string.IsNullOrWhiteSpace(txtPerHour.Text))
                            {
                                // Convert employee_contact to long (BIGINT)
                                string perHour = txtPerHour.Text.Trim();

                                // Check if the contact is numeric
                                if (System.Text.RegularExpressions.Regex.IsMatch(perHour, @"^\d+$"))
                                {
                                    cmd.Parameters.AddWithValue("@HourlyRate", perHour);
                                }
                                else
                                {
                                    MessageBox.Show("Please enter a numeric contact number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@HourlyRate", DBNull.Value);
                            }

                            cmd.Parameters.AddWithValue("@DateHired", dtpDate.Value); // Use DateTimePicker's Value

                            // Execute the command
                            cmd.ExecuteNonQuery();

                            
                        }

                        using (MySqlCommand cmdCon = new MySqlCommand(contributionQuery, opencon.connection))
                        {
                            cmdCon.Parameters.AddWithValue("@EmployeeID", txtEmpID.Text.Trim());
                            cmdCon.Parameters.AddWithValue("@PRID", DBNull.Value);

                            if (!string.IsNullOrWhiteSpace(txtSSS.Text))
                            {
                                cmdCon.Parameters.AddWithValue("@SSS", txtSSS.Text.Trim());
                            }
                            else
                            {
                                cmdCon.Parameters.AddWithValue("@SSS", DBNull.Value);
                            }

                            if (!string.IsNullOrWhiteSpace(txtPagIbig.Text))
                            {
                                cmdCon.Parameters.AddWithValue("@PagIbig", txtPagIbig.Text.Trim());
                            }
                            else
                            {
                                cmdCon.Parameters.AddWithValue("@PagIbig", DBNull.Value);
                            }
                            if (!string.IsNullOrWhiteSpace(txtPhilHealth.Text))
                            {
                                cmdCon.Parameters.AddWithValue("@PhilHealth", txtPhilHealth.Text.Trim());
                            }
                            else
                            {
                                cmdCon.Parameters.AddWithValue("@PhilHealth", DBNull.Value);
                            }

                            cmdCon.ExecuteNonQuery();
                        }

                        MessageBox.Show("Record has been updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearSalary();
                        btnEdit2.Image = Properties.Resources.icons8_edit_30;
                        btnEdit2.Text = "Edit";
                        isEdit = true;
                        btnAdd.Show();
                        LoadSalary();
                        LockSalary();
                        btnCustom.Enabled = false;
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show($"An error occurred while updating the data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (opencon.connection.State == ConnectionState.Open)
                        {
                            opencon.CloseConnection();
                        }
                    }
                }
            }
        }

        private void cboStatus_TextChanged(object sender, EventArgs e)
        {

        }

        private void ConLock()
        {
            btnEdit2.Enabled = true;
            btnAdd2.Enabled = false;
            btnAdd2.Text = "    Add Details";
            btnAdd2.Image = Properties.Resources.add_30;
            btnAdd2.Hide();
            btnEdit2.Show();
            isEdit = true;

        }
        private void ConUnlock()
        {
            btnAdd2.Enabled = true;
            btnEdit2.Enabled = false;
            LockSalary();
            btnEdit2.Text = "Edit";
            btnAdd2.Show();
            btnEdit2.Hide();
            isAdd = true;
            btnEdit2.Image = Properties.Resources.icons8_edit_30;
        }
        private void dataGridViewSalary_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewSalary.Rows[e.RowIndex];

                txtEmpID2.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                txtFullName.Text = selectedRow.Cells["Full Name"].Value.ToString();
                cboStatus.Text = selectedRow.Cells["Job Status"].Value.ToString();
                txtTitle.Text = selectedRow.Cells["Job Title"].Value.ToString();
                txtDepartment.Text = selectedRow.Cells["Department"].Value.ToString();
                txtSalary.Text = selectedRow.Cells["Basic Salary"].Value.ToString();
                txtPerHour.Text = selectedRow.Cells["Hourly Rate"].Value.ToString();
               
                if (!string.IsNullOrWhiteSpace(txtPerHour.Text.Trim()))
                {
                    txtPerHour.Visible = true;
                    lblHR.Visible = false;
                    lblPerHour.Visible = true;
                    btnAddPerHour.Visible = false;
                }
                else
                {
                    txtPerHour.Visible = false;
                    lblHR.Visible = true;
                    lblPerHour.Visible = false;
                    btnAddPerHour.Visible = true;
                }


                if (DateTime.TryParse(selectedRow.Cells["Date Hired"].Value.ToString(), out DateTime dateHired))
                {
                    dtpDate.Value = dateHired;
                }
                btnClear2.Enabled = true;


                if (string.IsNullOrWhiteSpace(cboStatus.Text = selectedRow.Cells["Job Status"].Value.ToString()) &&
                    string.IsNullOrWhiteSpace(txtDepartment.Text = selectedRow.Cells["Department"].Value.ToString()) &&
                    string.IsNullOrWhiteSpace(txtTitle.Text = selectedRow.Cells["Job Title"].Value.ToString()))
                    
                {
                    cboStatus.SelectedIndex = -1;
                    txtDepartment.SelectedIndex = -1;
                    txtTitle.SelectedIndex = -1;
                    btnAdd2.Enabled = true;
                    btnEdit2.Enabled = false;
                    LockSalary();
                    btnEdit2.Text = "Edit";
                    btnAdd2.Show();
                    btnEdit2.Hide();
                    isAdd = true;
                    btnEdit2.Image = Properties.Resources.icons8_edit_30;

                }
                else
                {
                    btnEdit2.Enabled = true;
                    btnAdd2.Enabled = false;
                    btnAdd2.Text = "    Add Details";
                    btnAdd2.Image = Properties.Resources.add_30;
                    btnAdd2.Hide();
                    btnEdit2.Show();
                    isEdit = true;

                }

                if (string.IsNullOrWhiteSpace(txtSSS.Text = selectedRow.Cells["SSS"].Value.ToString()))
                {
                    ConUnlock();
                }
                else
                {
                    ConLock();
                }
                if(string.IsNullOrWhiteSpace(txtPagIbig.Text = selectedRow.Cells["Pag Ibig"].Value.ToString()))
                {
                    ConUnlock();
                }
                else
                {
                    ConLock();
                }
                if(string.IsNullOrWhiteSpace(txtPhilHealth.Text = selectedRow.Cells["PhilHealth"].Value.ToString()))
                {
                    ConUnlock();
                }
                else
                {
                    ConLock();
                }

            }
        }

        private void txtPerHour_TextChanged(object sender, EventArgs e)
        {
            if(txtPerHour.Text == "")
            {
                lblHR.Show();
                btnAddPerHour.Show();
                lblPerHour.Hide();
                txtPerHour.Hide();
            }
            else
            {
                lblHR.Hide();
                btnAddPerHour.Hide();
                lblPerHour.Show();
                txtPerHour.Show();
            }
        }

        private void txtEmpID_TextChanged(object sender, EventArgs e)
        {
            if (txtEmpID.Text == "")
            {
                btnClear.Hide();
                btnEdit.Hide();

            }
            else
            {
                btnClear.Show();
                btnEdit.Show();
            }
        }


        private void ckboSSS_CheckedChanged(object sender, EventArgs e)
        {
            if (ckboSSS.Checked == true)
            {
                txtSSS.Text = "500";
            }else if (ckboSSS.Checked == true && txtSSS.Enabled == true)
            {
                txtSSS.Text = txtSSS.Text;
            }
            else
            {
                txtSSS.Text = "";
            }
        }

        private void ckboPagibig_CheckedChanged(object sender, EventArgs e)
        {
            if(ckboPagibig.Checked == true)
            {
                txtPagIbig.Text = "200";
            }
            else if (ckboPagibig.Checked == true && txtPagIbig.Enabled == true)
            {
                txtPagIbig.Text = txtPagIbig.Text;
            }
            else
            {
                txtPagIbig.Text = "";
            }
        }

        private void ckboPhilhealth_CheckedChanged(object sender, EventArgs e)
        {
            if (ckboPhilhealth.Checked == true)
            {
                txtPhilHealth.Text = "500";
            }
            else if (ckboPhilhealth.Checked == true && txtPhilHealth.Enabled == true)
            {
                txtPhilHealth.Text = txtPhilHealth.Text;
            }
            else
            {
                txtPhilHealth.Text = "";
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            ckboSSS.Checked = true;
            ckboPagibig.Checked = true;
            ckboPhilhealth.Checked = true;

        }

        private void btnCustom_Click(object sender, EventArgs e)
        {
            if (ckboSSS.Checked == true || string.IsNullOrEmpty(txtSSS.Text))
            {
                txtSSS.Text = "";
                txtSSS.Enabled = true;
                ckboSSS.Checked = false;
            }
            else
            {
                txtSSS.Enabled = false;
            }

            if (ckboPagibig.Checked == true || string.IsNullOrEmpty(txtPagIbig.Text))
            {
                txtPagIbig.Text = "";
                txtPagIbig.Enabled = true;
                ckboPagibig.Checked = false;
            }
            else
            {
                txtPagIbig.Enabled = false;
            }

            if (ckboPhilhealth.Checked == true || string.IsNullOrEmpty(txtPhilHealth.Text))
            {
                txtPhilHealth.Text = "";
                txtPhilHealth.Enabled = true;
                ckboPhilhealth.Checked = false;
            }
            else
            {
                txtPhilHealth.Enabled = false;
            }
        }

        //Validation
        //============================================================================================================================================


        private void txtFname_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtLetters(sender, e);
        }

        private void txtMname_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtLetters(sender, e);
        }

        private void txtLname_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtLetters(sender, e);
        }

        private void txtContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(textBox.Text, emailPattern))
                {
                    textBox.ForeColor = System.Drawing.Color.Red; // Indicate invalid email
                }
                else
                {
                    textBox.ForeColor = System.Drawing.Color.Black; // Valid email
                }
            }
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (txtContact.TextLength != 11)
                {
                    textBox.ForeColor = System.Drawing.Color.Red; // Indicate invalid email
                }
                else
                {
                    textBox.ForeColor = System.Drawing.Color.Black; // Valid email
                }
            }
        }

        private void txtSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }

        private void txtPerHour_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }

        private void txtDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtDepartment.Text == "School Staff")
            {
                btnAddPerHour.Enabled = false;
            }
            else
            {
                btnAddPerHour.Enabled = true;
            }
        }
    }
}
