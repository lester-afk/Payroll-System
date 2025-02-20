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

namespace PayrollRfidScan
{
    public partial class frmDisplayInfo : Form
    {
        DBconnection opencon = new DBconnection();
        public frmDisplayInfo()
        {
            InitializeComponent();
        }
        private Timer autoCloseTimer;

        private void frmDisplayInfo_Load(object sender, EventArgs e)
        {
            // Initialize and configure the auto-close timer
            autoCloseTimer = new Timer();
            autoCloseTimer.Interval = 15000; // Set the timer to 30 seconds (30,000 milliseconds)
            autoCloseTimer.Tick += timer1_Tick;
            autoCloseTimer.Start();
            FillInfo();
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

        private void FillInfo()
        {
            string searchValue = txtRfid.Text.Trim(); // Get the RFID value from the text box

            // Check if search value is empty
            if (string.IsNullOrEmpty(searchValue))
            {
                MessageBox.Show("Please enter an RFID value to search.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            opencon.dbconnect(); // Connect to the database

            if (opencon.OpenConnection()) // Ensure the connection is open
            {
                try
                {
                    // SQL query to fetch employee information based on RFID tag
                    string query = "SELECT CONCAT(employee_fname, ' ', employee_mname, ' ', employee_lname) AS 'FULL NAME', EMPLOYEE.employee_id as 'Employee ID', " +
                                   "rfid.rfid_tag AS 'RFID', job.job_department AS 'Department', employee.employee_picture AS 'Picture', " +
                                   "attendance.a_timeIn AS 'Time In', attendance.a_timeOut AS 'Time Out', attendance.a_statusIn AS 'Status In', attendance.a_statusOut AS 'Status Out' " +
                                   "FROM EMPLOYEE INNER JOIN RFID ON EMPLOYEE.EMPLOYEE_ID = RFID.EMPLOYEE_ID " +
                                   "INNER JOIN attendance ON employee.employee_id = attendance.employee_id " +
                                   "LEFT JOIN JOB ON EMPLOYEE.EMPLOYEE_ID = JOB.EMPLOYEE_ID WHERE RFID.RFID_TAG = @RFID " +
                                   "ORDER BY attendance.a_num DESC";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@RFID", searchValue);

                    using (MySqlDataReader reader = cmd.ExecuteReader()) // Use MySqlDataReader to execute the query
                    {
                        if (reader.Read()) // Read the first row if it exists
                        {
                            // Assign values from the database to the labels/textboxes
                            lblName.Text = reader["Full Name"].ToString();     // Display full name in label
                            lblEmpID.Text = reader["Employee ID"].ToString(); // Display employee ID in label
                            lblDepartment.Text = reader["Department"].ToString();  // Display department in label
                            lblRFID.Text = reader["RFID"].ToString();              // Display RFID in the textbox
                            txtPath.Text = reader["Picture"].ToString();
                            // Handle picture (if available)

                            string defaultImagePath = @"C:\Users\Resu\Desktop\Payroll_System\Photo\Pfp.png";
                            if (!string.IsNullOrEmpty(txtPath.Text) && System.IO.File.Exists(txtPath.Text))
                            {
                                employeePicture.Image = Image.FromFile(txtPath.Text);
                            }
                            else
                            {
                                employeePicture.Image = Image.FromFile(defaultImagePath);
                            }

                            txtTimIn.Text = reader["Time In"].ToString();
                            lblStatusIn.Text = reader["Status In"].ToString();
                            txtTimeOut.Text = reader["Time Out"].ToString();
                            lblStatusOut.Text = reader["Status Out"].ToString();
                            
                        }
                        else
                        {
                            MessageBox.Show("No employee found with the given RFID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while searching:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection(); // Ensure the connection is closed
                }
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            autoCloseTimer.Stop();  // Stop the timer to prevent it from ticking again
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void lblName_Click(object sender, EventArgs e)
        {

        }

        
    }
}
