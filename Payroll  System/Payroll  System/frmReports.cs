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
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
        }
        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        

        private void frmReports_Load(object sender, EventArgs e)
        {
            LoadEmployee();
            btnPrint.Enabled = false;
            btnGenerate.Enabled = false;
            btnSelect.Enabled = false;
            btnReset.Enabled = false;
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

       /* private void LoadData()
        {
            string searchValue = txtFullName.Text;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    // Query to fetch employee details and calculations
                    string query = @"SELECT 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name',
                                        job.job_salary AS 'Salary',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        COALESCE(SUM(
                                            CASE 
                                                WHEN a.a_timeIn > s.sched_timeIn THEN TIMESTAMPDIFF(MINUTE, s.sched_timeIn, a.a_timeIn)
                                                ELSE 0
                                            END), 0) AS 'Late Minutes',
                                        COALESCE(SUM(
                                            CASE 
                                                WHEN a.a_timeOut < s.sched_timeOut THEN TIMESTAMPDIFF(MINUTE, a.a_timeOut, s.sched_timeOut)
                                                ELSE 0
                                            END), 0) AS 'Undertime Minutes',
                                        (SELECT COUNT(*)
                                         FROM schedule s2
                                         LEFT JOIN attendance a2 ON s2.employee_id = a2.employee_id 
                                             AND s2.sched_day = DAYNAME(a2.a_date)
                                             AND a2.a_date BETWEEN DATE_SUB(CURDATE(), INTERVAL 10 DAY) AND CURDATE()
                                         WHERE s2.employee_id = employee.employee_id
                                           AND a2.a_timeIn IS NULL) AS 'Absences',
                                        COALESCE(SUM(ca.ca_amount), 0) AS 'Cash Advance',
                                        COALESCE(SUM(ca.ca_balance), 0) AS 'Balance',
                                        COALESCE(SUM(TIMESTAMPDIFF(HOUR, a.a_timeIn, a.a_timeOut)), 0) AS 'Total Worked Hours'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN schedule s ON employee.employee_id = s.employee_id
                                    LEFT JOIN attendance a ON employee.employee_id = a.employee_id 
                                        AND s.sched_day = DAYNAME(a.a_date) 
                                        AND a.a_date BETWEEN DATE_SUB(CURDATE(), INTERVAL 10 DAY) AND CURDATE()
                                    LEFT JOIN cash_advance ca ON employee.employee_id = ca.employee_id
                                    WHERE CONCAT(employee.employee_fname, ' ', employee.employee_lname) LIKE @Search
                                    GROUP BY employee.employee_id, employee.employee_fname, employee.employee_mname, employee.employee_lname,
                                             job.job_salary, job.job_hourly_rate;";

                    // Create a MySqlCommand object
                    using (MySqlCommand command = new MySqlCommand(query, opencon.connection))
                    {
                        // Add parameter to the query
                        command.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                        // Execute the query and get the data reader
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Check if there is data to read
                            {
                                // Populate textboxes with the fetched data
                                txtSalary.Text = reader["Salary"].ToString();
                                txtPerhour.Text = reader["Hourly Rate"].ToString();
                                txtLpm.Text = reader["Late Minutes"].ToString();
                                txtUtpm.Text = reader["Undertime Minutes"].ToString();
                                txtAbs.Text = reader["Absences"].ToString();
                                txtCA.Text = reader["Cash Advance"].ToString();
                                txtCABal.Text = reader["Balance"].ToString();
                                txtWh.Text = reader["Total Worked Hours"].ToString(); // Total worked hours
                            }
                            else
                            {
                                MessageBox.Show("No data found for the specified employee.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        */
        private void dataGridViewEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewEmployee.Rows[e.RowIndex];


               
                txtFullName.Text = selectedRow.Cells["Full Name"].Value.ToString();
                btnSelect.Enabled = true;
                btnReset.Enabled = true;
            }
            }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            btnGenerate.Enabled = true;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = true;
            GeneratePayroll();
        }

        private void GeneratePayroll()
        {
            string searchValue = txtFullName.Text;
            DateTime fromDate = dtpFrom.Value.Date; // Convert to DateTime
            DateTime toDate = dtpTo.Value.Date;     // Convert to DateTime

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT 
                                    CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                           COALESCE(employee.employee_mname, ''), ' ', 
                                           COALESCE(employee.employee_lname, '')) AS 'Full Name',
                                    job.job_salary AS 'Salary',
                                    job.job_hourly_rate AS 'Hourly Rate',
                                    a.a_date,                   -- 🔹 Add this line to fetch the date
                                    a.a_timeIn, 
                                    a.a_timeOut, 
                                    s.sched_timeIn, 
                                    s.sched_timeOut,
                                    ca.ca_amount AS 'Cash Advance',
                                    ca.ca_balance AS 'Balance'
                                FROM employee
                                LEFT JOIN job ON employee.employee_id = job.employee_id
                                LEFT JOIN schedule s ON employee.employee_id = s.employee_id
                                LEFT JOIN attendance a ON employee.employee_id = a.employee_id 
                                    AND s.sched_day = DAYNAME(a.a_date) 
                                    AND a.a_date BETWEEN @FromDate AND @ToDate
                                LEFT JOIN cash_advance ca ON employee.employee_id = ca.employee_id
                                                    WHERE CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                                                       COALESCE(employee.employee_mname, ''), ' ', 
                                                                       COALESCE(employee.employee_lname, '')) LIKE @Search;";

                    using (MySqlCommand command = new MySqlCommand(query, opencon.connection))
                    {
                        command.Parameters.AddWithValue("@Search", "%" + searchValue + "%");
                        command.Parameters.AddWithValue("@FromDate", fromDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@ToDate", toDate.ToString("yyyy-MM-dd"));

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // Initialize variables
                                double salary = 0, hourlyRate = 0, cashAdvance = 0, balance = 0;
                                int totalLateMinutes = 0, totalUndertimeMinutes = 0, totalAbsences = 0;
                                int totalWorkedHours = 0;
                                int daysWithAttendance = 0; // Count days employee has attendance

                                while (reader.Read())
                                {
                                    salary = reader["Salary"] != DBNull.Value ? Convert.ToDouble(reader["Salary"]) : 0.0;

                                    hourlyRate = reader["Hourly Rate"] != DBNull.Value ? Convert.ToDouble(reader["Hourly Rate"]) : 0.0;
                                    cashAdvance = reader["Cash Advance"] != DBNull.Value ? Convert.ToDouble(reader["Cash Advance"]) : 0.0;
                                    balance = reader["Balance"] != DBNull.Value ? Convert.ToDouble(reader["Balance"]) : 0.0;


                                    DateTime? attendanceDate = reader["a_date"] != DBNull.Value ? Convert.ToDateTime(reader["a_date"]) : (DateTime?)null;

                                    // Fix: Read time values as TimeSpan
                                    TimeSpan? timeIn = reader["a_timeIn"] != DBNull.Value ? (TimeSpan?)reader["a_timeIn"] : null;
                                    TimeSpan? timeOut = reader["a_timeOut"] != DBNull.Value ? (TimeSpan?)reader["a_timeOut"] : null;
                                    TimeSpan? schedIn = reader["sched_timeIn"] != DBNull.Value ? (TimeSpan?)reader["sched_timeIn"] : null;
                                    TimeSpan? schedOut = reader["sched_timeOut"] != DBNull.Value ? (TimeSpan?)reader["sched_timeOut"] : null;

                                    // Convert TimeSpan to DateTime (Attach to attendance date)
                                    DateTime? fullTimeIn = (attendanceDate.HasValue && timeIn.HasValue) ? (DateTime?)attendanceDate.Value.Add(timeIn.Value) : null;
                                    DateTime? fullTimeOut = (attendanceDate.HasValue && timeOut.HasValue) ? (DateTime?)attendanceDate.Value.Add(timeOut.Value) : null;
                                    DateTime? fullSchedIn = (attendanceDate.HasValue && schedIn.HasValue) ? (DateTime?)attendanceDate.Value.Add(schedIn.Value) : null;
                                    DateTime? fullSchedOut = (attendanceDate.HasValue && schedOut.HasValue) ? (DateTime?)attendanceDate.Value.Add(schedOut.Value) : null;

                                    // Compute Late Minutes
                                    if (fullTimeIn.HasValue && fullSchedIn.HasValue && fullTimeIn > fullSchedIn)
                                    {
                                        totalLateMinutes += (int)(fullTimeIn.Value - fullSchedIn.Value).TotalMinutes;
                                    }

                                    // Compute Undertime Minutes
                                    if (fullTimeOut.HasValue && fullSchedOut.HasValue && fullTimeOut < fullSchedOut)
                                    {
                                        totalUndertimeMinutes += (int)(fullSchedOut.Value - fullTimeOut.Value).TotalMinutes;
                                    }

                                    // Compute Worked Hours
                                    if (fullTimeIn.HasValue && fullTimeOut.HasValue)
                                    {
                                        totalWorkedHours += (int)(fullTimeOut.Value - fullTimeIn.Value).TotalHours;
                                    }

                                }

                                // Compute Total Working Days in the Range
                                int totalWorkingDays = 0;
                                for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
                                {
                                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) // Exclude weekends
                                    {
                                        totalWorkingDays++;
                                    }
                                }

                                // Compute Absences: Days without Time In
                                totalAbsences = totalWorkingDays - daysWithAttendance;

                                // Compute Gross Salary
                                double grossIncome = salary;

                                // Compute Deductions
                                double lateDeduction = totalLateMinutes * (hourlyRate / 60);
                                double undertimeDeduction = totalUndertimeMinutes * (hourlyRate / 60);
                                double absenceDeduction = totalAbsences * (hourlyRate * 8);
                                double totalDeductions = lateDeduction + undertimeDeduction + absenceDeduction + cashAdvance;

                                // Compute Net Salary
                                double netIncome = grossIncome - totalDeductions;

                                // Assign values to textboxes
                                txtSalary.Text = salary.ToString("N2");
                                txtPerhour.Text = hourlyRate.ToString("N2");

                                txtLpm.Text = totalLateMinutes.ToString();
                                txtUtpm.Text = totalUndertimeMinutes.ToString();
                                txtAbs.Text = totalAbsences.ToString();
                                txtWh.Text = totalWorkedHours.ToString();

                                txtWhia.Text = (totalWorkedHours * hourlyRate).ToString("N2");
                                txtLia.Text = lateDeduction.ToString("N2");
                                txtUtia.Text = undertimeDeduction.ToString("N2");
                                txtAbia.Text = absenceDeduction.ToString("N2");

                                txtCA.Text = cashAdvance.ToString("N2");
                                txtCABal.Text = balance.ToString("N2");
                                txtCADed.Text = cashAdvance.ToString("N2");

                                txtTDed.Text = totalDeductions.ToString("N2");
                                txtNet.Text = netIncome.ToString("N2");
                            }
                            else
                            {
                                MessageBox.Show("No data found for the specified employee and date range.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



    }
}
