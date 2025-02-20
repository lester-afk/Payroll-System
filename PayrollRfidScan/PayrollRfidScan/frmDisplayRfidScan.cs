using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PayrollRfidScan
{
    public partial class frmDisplayRfidScan : Form
    {
        DBconnection opencon = new DBconnection();
        private frmDisplayInfo formDisplayInfo;
        private System.Windows.Forms.Timer maxLengthTimer;
        private string lastInput = "";
        private string empID;
        private string fullName;
        private string department;
        private string aDate = DateTime.Now.ToString("yyyy/MM/dd");
        private string aTimeIn;
        private string aTimeOut;
        private string aStatusIn;
        private string aStatusOut;
        private string schedIn;
        private string schedOut;
        private string schedPeriod;
        private string headContact;
        private string message;

        public frmDisplayRfidScan(frmDisplayInfo formInfo)
        {
            InitializeComponent();
            this.Size = new Size(SystemInformation.WorkingArea.Width, SystemInformation.WorkingArea.Height);
            maxLengthTimer = new System.Windows.Forms.Timer();
            maxLengthTimer.Interval = 10000; // Set timer interval (800ms)
            maxLengthTimer.Tick += Timer_Tick;
            formDisplayInfo = formInfo;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            maxLengthTimer.Stop();

            if (!string.IsNullOrEmpty(lastInput))
            {
                txtRFID.MaxLength = lastInput.Length;
                LoadEmployee();
                txtRFID.Clear();
            }
            else if (string.IsNullOrEmpty(txtRFID.Text) && btnTimeIn.Focused || btnTimeOut.Focused)
            {
                txtRFID.MaxLength = 16;
                txtRFID.Focus();
            }

            Console.WriteLine($"MaxLength adjusted to: {txtRFID.MaxLength}");
        }

        private string DetermineStatus(string scheduledTime, string actualTime, string action)
        {
            if (DateTime.TryParse(scheduledTime, out DateTime schedule) && DateTime.TryParse(actualTime, out DateTime actual))
            {
                double minutesDifference = (actual - schedule).TotalMinutes;

                if (action == "IN")
                {
                    if (minutesDifference > 15)
                        return "Late";
                    else if (minutesDifference < -15)
                        return "Early In";
                    else
                        return "On Time";
                }
                else if (action == "OUT")
                {
                    if (minutesDifference < -15)
                        return "Under Time";
                    else if (minutesDifference > 15)
                        return "Over Time";
                    else
                        return "On Time";
                }
            }
            else
            {
                MessageBox.Show("Unable to parse scheduled or actual time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtRFID.Clear();
                txtRFID.Focus();
                
            }

            return "Unknown";
        }

        private void InsertOrUpdateAttendance(string action)
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    // Check if there is any previous attendance record without a time out
                    string unfinishedAttendanceQuery = @"
                                                        SELECT COUNT(*) FROM attendance 
                                                        WHERE employee_id = @EmployeeID 
                                                        AND a_date = @Date 
                                                        AND a_timeOut IS NULL";

                    MySqlCommand unfinishedCmd = new MySqlCommand(unfinishedAttendanceQuery, opencon.connection);
                    unfinishedCmd.Parameters.AddWithValue("@EmployeeID", empID);
                    unfinishedCmd.Parameters.AddWithValue("@Date", aDate);

                    int unfinishedRecords = Convert.ToInt32(unfinishedCmd.ExecuteScalar());

                    // If employee has unfinished attendance and is trying to time in again, prevent it
                    if (unfinishedRecords > 0 && action == "TIME IN")
                    {
                        MessageBox.Show("You must Time Out before Timing In for a new period.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Check if the attendance record already exists for the current period
                    string checkQuery = @"
                                        SELECT COUNT(*) FROM attendance 
                                        WHERE employee_id = @EmployeeID 
                                        AND a_date = @Date 
                                        AND a_period = @Period";

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                    checkCmd.Parameters.AddWithValue("@EmployeeID", empID);
                    checkCmd.Parameters.AddWithValue("@Date", aDate);
                    checkCmd.Parameters.AddWithValue("@Period", schedPeriod);

                    int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (recordExists == 0 && action == "TIME IN")
                    {
                        InsertAttendance();
                        formDisplayInfo.ShowDialog();
                        DepartmentHead("TIME IN");
                    }
                    else if (recordExists > 0 && action == "TIME OUT")
                    {
                        UpdateAttendance();
                        formDisplayInfo.ShowDialog();
                        DepartmentHead("TIME OUT");
                    }
                    else
                    {
                        MessageBox.Show("Attendance record already exists", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    CollegeDept();
                    ShsDept();
                    DisplayAttendance();
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
            string query = "INSERT INTO attendance(employee_id, a_date, a_timeIn, a_period, a_statusIn) VALUES (@EmployeeID, @Date, @TimeIn, @Period, @StatusIn)";
            using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", empID);
                cmd.Parameters.AddWithValue("@Date", aDate);
                cmd.Parameters.AddWithValue("@TimeIn", aTimeIn);
                cmd.Parameters.AddWithValue("@Period", schedPeriod);
                cmd.Parameters.AddWithValue("@StatusIn", aStatusIn);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateAttendance()
        {
            string query = @"
        UPDATE attendance 
        SET a_timeOut = @TimeOut, a_statusOut = @StatusOut 
        WHERE employee_id = @EmployeeID 
        AND a_date = @Date 
        AND a_timeOut IS NULL 
        ORDER BY a_period ASC 
        LIMIT 1"; // Update only the first found record without a Time Out

            using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
            {
                cmd.Parameters.AddWithValue("@TimeOut", aTimeOut);
                cmd.Parameters.AddWithValue("@StatusOut", aStatusOut);
                cmd.Parameters.AddWithValue("@EmployeeID", empID);
                cmd.Parameters.AddWithValue("@Date", aDate);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No matching Time In record found to Time Out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void LoadEmployee()
        {
            string searchValue = txtRFID.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                MessageBox.Show("Please enter an RFID again.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT 
                    employee.employee_id AS 'Employee ID', 
                    CONCAT(employee_fname, ' ', employee_mname, ' ', employee_lname) AS 'FULL NAME',
                    job.job_department AS 'Department', 
                    schedule.sched_day AS 'Schedule Day', 
                    schedule.sched_timeIn AS 'Schedule In', 
                    schedule.sched_timeOut AS 'Schedule Out', 
                    schedule.sched_period AS 'Schedule Period'
                    FROM EMPLOYEE 
                    INNER JOIN RFID ON EMPLOYEE.EMPLOYEE_ID = RFID.EMPLOYEE_ID 
                    INNER JOIN JOB ON EMPLOYEE.EMPLOYEE_ID = JOB.EMPLOYEE_ID 
                    LEFT JOIN SCHEDULE ON EMPLOYEE.EMPLOYEE_ID = SCHEDULE.EMPLOYEE_ID 
                    WHERE RFID.RFID_TAG = @RFID 
                    AND SCHEDULE.sched_day = @CurrentDay 
                    ORDER BY SCHEDULE.sched_period ASC";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@RFID", searchValue);
                    cmd.Parameters.AddWithValue("@CurrentDay", DateTime.Now.DayOfWeek.ToString());

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                empID = reader["Employee ID"].ToString();
                                fullName = reader["FULL NAME"].ToString();
                                department = reader["Department"].ToString();
                                schedIn = reader["Schedule In"].ToString();
                                schedOut = reader["Schedule Out"].ToString();
                                schedPeriod = reader["Schedule Period"].ToString();

                                if (!HasAttendanceRecord(empID, schedPeriod))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No employee found with the given RFID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading employee details:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private bool HasAttendanceRecord(string employeeId, string period)
        {
            bool recordExists = false;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT *
                                    FROM attendance 
                                    WHERE employee_id = @EmployeeID 
                                    AND a_date = @Date 
                                    AND a_period = @Period";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy/MM/dd"));
                    cmd.Parameters.AddWithValue("@Period", period);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    recordExists = count > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error checking attendance record: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }

            return recordExists;
        }

        private void frmDisplayEmployee_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // Set timer to tick every second
            timer1.Start(); // Start the timer
            txtRFID.Select();
            comboBox1.SelectedIndex = 0;
            CollegeDept();
            ShsDept();
            DisplayAttendance();
        }

        private void txtRFID_TextChanged(object sender, EventArgs e)
        {
            maxLengthTimer.Stop();
            maxLengthTimer.Start();

            lastInput = txtRFID.Text;
            aTimeIn = DateTime.Now.ToString("HH:mm");
            aTimeOut = DateTime.Now.ToString("HH:mm");

            if (formDisplayInfo != null && !formDisplayInfo.IsDisposed)
            {
                formDisplayInfo.FillRfid(lastInput);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt");

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRFID.Focus();
        }

        private void DepartmentHead(string attendanceAction)
        {
            try
            {
                opencon.dbconnect();

                if (opencon.OpenConnection())
                {
                    string jobTitle = department == "College" ? "Dean" : department == "Senior High School" ? "Principal" : null;
                    if (jobTitle == null)
                    {
                        MessageBox.Show("Department not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string query = @"SELECT employee_contact AS 'Contact' FROM employee 
                             INNER JOIN job ON employee.employee_id = job.employee_id 
                             WHERE job_title = @JobTitle";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@JobTitle", jobTitle);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            headContact = reader["Contact"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Department head contact not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (attendanceAction == "TIME IN")
                    {
                        message = $"{fullName} has Time In at {aTimeIn}. Date: {aDate}";
                    }
                    else if (attendanceAction == "TIME OUT")
                    {
                        message = $"{fullName} has Time Out at {aTimeOut}. Date: {aDate}";
                    }
                    else
                    {
                        MessageBox.Show("Invalid attendance action specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    SendMessage(headContact, message);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                opencon.CloseConnection();
            }
        }

        private string AutoDetectPort()
        {
            try
            {
                string[] availablePorts = SerialPort.GetPortNames();
                if (availablePorts.Length == 0)
                {
                    MessageBox.Show("No COM ports are available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                string testCommand = "AT";

                foreach (string port in availablePorts)
                {
                    using (SerialPort serialPort = new SerialPort(port, 9600))
                    {
                        try
                        {
                            serialPort.Open();
                            serialPort.WriteLine(testCommand + Environment.NewLine);
                            Thread.Sleep(200);

                            string response = serialPort.ReadExisting();

                            if (!string.IsNullOrEmpty(response) && response.Contains("OK"))
                            {

                                Console.WriteLine($"Device detected on {port}");
                                return port;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to open port {port}: {ex.Message}");
                        }
                        finally
                        {
                            if (serialPort.IsOpen)
                                serialPort.Close();
                        }
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during port detection:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private bool SendMessage(string cpNum, string msg)
        {
            string portName = AutoDetectPort();

            if (string.IsNullOrEmpty(portName))
            {
                MessageBox.Show("Device not found. Ensure it is connected and powered on.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                using (SerialPort serialPort = new SerialPort(portName, 9600))
                {
                    serialPort.Open();

                    serialPort.WriteLine("AT" + Environment.NewLine);
                    Thread.Sleep(100);

                    serialPort.WriteLine("AT+CMGF=1" + Environment.NewLine);
                    Thread.Sleep(100);

                    serialPort.WriteLine("AT+CSCS=\"GSM\"" + Environment.NewLine);
                    Thread.Sleep(100);

                    serialPort.WriteLine($"AT+CMGS=\"{cpNum}\"" + Environment.NewLine);
                    Thread.Sleep(100);

                    serialPort.Write(msg + Environment.NewLine);
                    Thread.Sleep(100);

                    serialPort.Write(new byte[] { 26 }, 0, 1); // Ctrl+Z to send the message
                    Thread.Sleep(100);

                    string response = serialPort.ReadExisting();

                    if (response.Contains("ERROR"))
                    {
                        Console.WriteLine("Received an error: " + response);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while sending the message:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CollegeDept()
        {
            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT COUNT(*) AS TOTAL, a_date, job_department
                                    FROM EMPLOYEE LEFT JOIN attendance ON employee.employee_id = attendance.employee_id
                                    LEFT JOIN JOB ON EMPLOYEE.EMPLOYEE_ID = JOB.EMPLOYEE_ID WHERE a_date = @CurrentDay and job_department = 'College'";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@CurrentDay", DateTime.Now.ToString("yyyy/MM/dd"));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblCollegeCount.Text = reader["TOTAL"].ToString();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error checking attendance record: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void ShsDept()
        {
            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT COUNT(*) AS TOTAL, a_date, job_department
                                    FROM EMPLOYEE LEFT JOIN attendance ON employee.employee_id = attendance.employee_id
                                    LEFT JOIN JOB ON EMPLOYEE.EMPLOYEE_ID = JOB.EMPLOYEE_ID WHERE a_date = @CurrentDay and job_department = 'Senior High School'";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@CurrentDay", DateTime.Now.ToString("yyyy/MM/dd"));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblCollegeCount.Text = reader["TOTAL"].ToString();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error checking attendance record: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void DisplayAttendanceFilter()
        {
            string searchValue = dtpDate.Value.ToString("yyyy-MM-dd");
            string searchEmp = txtSearch.Text.Trim();

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
                                WHERE attendance.a_date LIKE @Date AND CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                           COALESCE(employee.employee_mname, ''), ' ', 
                                           COALESCE(employee.employee_lname, '')) LIKE @Search ";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Date", "%" + searchValue + "%");
                    cmd.Parameters.AddWithValue("@Search", "%" + searchEmp + "%");

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

        private void DisplayAttendance()
        {
            string searchValue = dtpDate.Value.ToString("yyyy-MM-dd");

            opencon.dbconnect();

            if (string.IsNullOrWhiteSpace(txtSearch.Text) && searchValue == DateTime.Now.ToString("yyyy-MM-dd")) {
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
                                WHERE attendance.a_date LIKE @Search";

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
            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                DisplayAttendanceFilter();
            }
            else
            {
                DisplayAttendance();
                txtRFID.Clear();
                txtRFID.Focus();
            }
            

        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            string searchValue = dtpDate.Value.ToString("yyyy-MM-dd");
            if (searchValue != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                DisplayAttendanceFilter();
            }
            else
            {
                DisplayAttendance();
                txtRFID.Clear();
                txtRFID.Focus();
            }

        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            
            if (btnTimeIn.Text == "TIME IN" && !string.IsNullOrWhiteSpace(txtRFID.Text))
            {
                aStatusIn = DetermineStatus(schedIn, aTimeIn, "IN");
                InsertOrUpdateAttendance("TIME IN");
            }
            else
            {
                MessageBox.Show("Please scan your RFID on a RFID Reader.", "No RFID Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRFID.Focus();
            }
        }

        private void btnTimeOut_Click(object sender, EventArgs e)
        {
            
            if (btnTimeOut.Text == "TIME OUT" && !string.IsNullOrWhiteSpace(txtRFID.Text))
            {
                aStatusOut = DetermineStatus(schedOut, aTimeOut, "OUT");
                InsertOrUpdateAttendance("TIME OUT");
            }
            else
            {
                MessageBox.Show("Please scan your RFID on a RFID Reader.", "No RFID Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRFID.Focus();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpDate.ResetText();
            txtSearch.Clear();
            txtRFID.Clear();
            txtRFID.Focus();
        }
    }
}