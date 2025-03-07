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
    public partial class frmPayroll : Form
    {

        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        public frmPayroll()
        {
            InitializeComponent();

            // Attach event handlers for dynamic updates
            txtCADeduction.TextChanged += txtCADeduction_TextChanged;
            txtDeduction.TextChanged += txtDeduction_TextChanged;
        }

        private void GenTeachingID()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int maxnum = Convert.ToInt32(mf.getMaxPR()); // Ensure this method returns the correct max number
            int idnum = maxnum + 1;
            string ID = $"PRT-{Year}-{Month}-{idnum}";
            txtTeachingID.Text = ID;
        }

        private void GenRegularID()
        {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int maxnum = Convert.ToInt32(mf.getMaxPR()); // Ensure this method returns the correct max number
            int idnum = maxnum + 1;
            string ID = $"PRR-{Year}-{Month}-{idnum}";
            txtRegularID.Text = ID;
        }

        private void ClearTeachingSalary()
        {
            txtTeachingID.Clear();
            txtPerHour.Clear();
            txtWorkedHr.Clear();
            txtGrossIncome.Clear();
            txtCashAdvance.Clear();
            txtCADeduction.Clear();
            txtLates.Clear();
            txtLateAmount.Clear();
            txtAbsence.Clear();
            txtAbsencesAmount.Clear();
            txtUndertime.Clear();
            txtUndertimeAmount.Clear();
            txtSSS.Clear();
            txtPagIbig.Clear();
            txtPhilHealth.Clear();
            txtDeduction.Clear();
            txtDescription.Clear();
            txtTotalDeduction.Clear();
            txtNetIncome.Clear();
            txtCutOff.Clear();
        }

        private void ClearRegularSalary()
        {
            txtRegularID.Clear();
            txtSalary.Clear();
            txtRegGrossIncome.Clear();
            txtRegCashAdvance.Clear();
            txtRegCADeduction.Clear();
            txtRegSSS.Clear();
            txtRegPagIbig.Clear();
            txtRegPhilhealth.Clear();
            txtRegDeduction.Clear();
            txtRegDescription.Clear();
            txtRegTotalDeduction.Clear();
            txtRegNetIncome.Clear();
            txtRegCutOff.Clear();
        }

        private void RegularLoad()
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        job.job_department AS 'Department',
                                        job.job_status AS 'Job Status',
                                        job.job_salary AS 'Salary',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        -- Merge cash advance records into a single string
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_id ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Cash Advance ID',
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_amount ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Advance Amount',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance' -- Ensure non-null balance
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    WHERE job.job_salary IS NOT NULL
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_department, 
                                        job.job_status, 
                                        job.job_salary, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth
                                    LIMIT 1000";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridViewEmployee.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void TeachingLoad()
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        job.job_department AS 'Department',
                                        job.job_status AS 'Job Status',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        -- Merge cash advance records into a single string
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_id ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Cash Advance ID',
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_amount ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Advance Amount',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance' -- Ensure non-null balance
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    WHERE job.job_hourly_rate IS NOT NULL
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_department, 
                                        job.job_status, 
                                        job.job_hourly_rate, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth
                                    LIMIT 1000";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridViewEmployee.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEmpID.Clear();
            txtFname.Clear();

            if (cboStatus.Text == "Teaching")
            {
                TeachingLoad();
                gbPerHour.Show();
                gbRegular.Hide();
                ClearRegularSalary();
                gbRegular.Enabled = false;
            }
            else if (cboStatus.Text == "Regular")
            {
                RegularLoad();
                gbPerHour.Hide();
                gbRegular.Show();
                ClearTeachingSalary();
                gbPerHour.Enabled = false;
            }

        }

        private void SearchTeaching(string Search)
        {
            string query = @"SELECT 
                                     employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        job.job_department AS 'Department',
                                        job.job_status AS 'Job Status',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        -- Merge cash advance records into a single string
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_id ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Cash Advance ID',
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_amount ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Advance Amount',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance' -- Ensure non-null balance
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    WHERE CONCAT(COALESCE(employee.employee_fname, ''), ' ',
                                           COALESCE(employee.employee_mname, ''), ' ',
                                           COALESCE(employee.employee_lname, '')) LIKE @Search
                                           AND job.job_hourly_rate IS NOT NULL 
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_department, 
                                        job.job_status, 
                                        job.job_hourly_rate, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth
                                    LIMIT 1000";

            MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
            cmd.Parameters.AddWithValue("@Search", "%" + Search + "%");

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridViewEmployee.DataSource = dt;
        }

        private void SearchRegular(string Search)
        {
            string query = @"SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        job.job_department AS 'Department',
                                        job.job_status AS 'Job Status',
                                        job.job_salary AS 'Salary',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        -- Merge cash advance records into a single string
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_id ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Cash Advance ID',
                                        GROUP_CONCAT(DISTINCT cash_advance.ca_amount ORDER BY cash_advance.ca_id SEPARATOR ', ') AS 'Advance Amount',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance' -- Ensure non-null balance
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    WHERE CONCAT(COALESCE(employee.employee_fname, ''), ' ',
                                           COALESCE(employee.employee_mname, ''), ' ',
                                           COALESCE(employee.employee_lname, '')) LIKE @Search
                                           AND job.job_salary IS NOT NULL
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_department, 
                                        job.job_status, 
                                        job.job_salary, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth
                                    LIMIT 1000";

            MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
            cmd.Parameters.AddWithValue("@Search", "%" + Search + "%");

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridViewEmployee.DataSource = dt;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();
            string statusValue = cboStatus.Text.ToString();

            if (string.IsNullOrEmpty(searchValue) && statusValue == "Teaching")
            {
                TeachingLoad(); // Reload all data if search box is empty
                return;
            }
            else if (string.IsNullOrEmpty(searchValue) && statusValue == "Regular")
            {
                RegularLoad();
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    if (!string.IsNullOrEmpty(searchValue) && statusValue == "Teaching")
                    {
                        SearchTeaching(searchValue);

                    }
                    else if (!string.IsNullOrEmpty(searchValue) && statusValue == "Regular")
                    {
                        SearchRegular(searchValue);

                    }

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

        private void dataGridViewEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewEmployee.Rows[e.RowIndex];
                txtEmpID.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                txtFname.Text = selectedRow.Cells["Full Name"].Value.ToString();
                cboStatus.SelectedValue = selectedRow.Cells["Job Status"].Value.ToString();
                ClearRegularSalary();
                ClearTeachingSalary();
                gbPerHour.Enabled = false;
                gbRegular.Enabled = false;
            }

        }

        private decimal ConvertToDecimal(object value)
        {
            if (value == DBNull.Value || value == null)
            {
                return 0; // Return 0 if the value is NULL
            }
            return Convert.ToDecimal(value);
        }

        private void ComputeRegular()
        {
            if (dataGridViewEmployee.SelectedRows.Count > 0)
            {
                DateTime currentDate = DateTime.Now;

                // Get selected Employee ID
                string employeeId = dataGridViewEmployee.SelectedRows[0].Cells["Employee ID"].Value.ToString();
                decimal monthlyRate = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Salary"].Value);

                // Get Contributions & Deductions (Apply only once)
                decimal sss = ConvertToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["SSS"].Value) / 2;
                decimal pagibig = ConvertToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Pag Ibig"].Value) / 2;
                decimal philhealth = ConvertToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["PhilHealth"].Value) / 2;

                // Load and display cash advance
                decimal totalAdvanceBalance = LoadCashAdvance(employeeId);


                // Determine Payroll Cut-off Dates
                int lastDayOfMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                DateTime startDate, endDate;
                if (currentDate.Day <= 14)
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, 14);
                    txtRegCutOff.Text = "1st Period";
                }
                else
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, lastDayOfMonth);
                    txtRegCutOff.Text = "1st Period";
                }

                // Compute Attendance-Based Deductions
                /* var (absences, absencesAmount, latesMinutes, latesAmount, undertimeMinutes, undertimeAmount)
                     = ComputeAttendanceDeductions(employeeId, startDate, endDate, monthlyRate / 22);*/ // 

                // Compute Salary
                decimal grossSalary = monthlyRate / 2; // Bi-monthly salary
                decimal totalDeductions = sss + pagibig + philhealth;
                decimal netIncome = grossSalary - totalDeductions;

                // Display on UI
                txtSalary.Text = monthlyRate.ToString("N2");
                txtRegGrossIncome.Text = grossSalary.ToString("N2");
                txtRegCashAdvance.Text = totalAdvanceBalance.ToString("N2");
                txtRegSSS.Text = sss.ToString("N2");
                txtRegPagIbig.Text = pagibig.ToString("N2");
                txtRegPhilhealth.Text = philhealth.ToString("N2");
                txtRegNetIncome.Text = netIncome.ToString("N2");
                txtRegTotalDeduction.Text = totalDeductions.ToString("N2");
                GenRegularID();
                //txtAbsence.Text = absences.ToString();
                //txtLates.Text = latesMinutes.ToString();
                //txtUndertime.Text = undertimeMinutes.ToString();
                //txtAbsencesAmount.Text = absencesAmount.ToString("C2");
                //txtLateAmount.Text = latesAmount.ToString("C2");
                //txtUndertimeAmount.Text = undertimeAmount.ToString("C2");
                //txtWorkedDays.Text = "N/A"; // Regular employees are not hourly


            }
        }

        private void ComputeTeaching()
        {
            if (dataGridViewEmployee.SelectedRows.Count > 0)
            {
                DateTime currentDate = DateTime.Now;

                // Get selected Employee ID
                string employeeId = dataGridViewEmployee.SelectedRows[0].Cells["Employee ID"].Value.ToString();
                decimal hourlyRate = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Hourly Rate"].Value);

                // Check if the employee has a regular load
                bool hasRegularLoad = EmployeeHasRegularLoad(employeeId);

                // Get Contributions & Deductions (Only apply if no regular load)
                decimal sss = hasRegularLoad ? 0 : ConvertToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["SSS"].Value) / 2;
                decimal pagibig = hasRegularLoad ? 0 : ConvertToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Pag Ibig"].Value) / 2;
                decimal philhealth = hasRegularLoad ? 0 : ConvertToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["PhilHealth"].Value) / 2;
                decimal totalAdvanceBalance = LoadCashAdvance(employeeId);

                // Determine Payroll Cut-off Dates
                int lastDayOfMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                DateTime startDate, endDate;
                if (currentDate.Day <= 14)
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, 14);
                    txtCutOff.Text = "1st Period";
                }
                else
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, lastDayOfMonth);
                    txtCutOff.Text = "2nd Period";
                }

                // Get Worked Hours
                decimal workedHours = GetWorkedHours(employeeId, startDate, endDate);

                // Compute Attendance-Based Deductions
                var (absences, absencesAmount, latesMinutes, latesAmount, undertimeMinutes, undertimeAmount)
                    = ComputeAttendanceDeductions(employeeId, startDate, endDate, hourlyRate);

                // Compute Salary
                decimal grossSalary = workedHours * hourlyRate;
                decimal totalDeductions = sss + pagibig + philhealth + absencesAmount + latesAmount + undertimeAmount;
                decimal netIncome = grossSalary - totalDeductions;

                // Display on UI
                txtPerHour.Text = hourlyRate.ToString("N2");
                txtWorkedHr.Text = workedHours.ToString();
                txtGrossIncome.Text = grossSalary.ToString("N2");
                txtAbsence.Text = absences.ToString();
                txtLates.Text = latesMinutes.ToString();
                txtUndertime.Text = undertimeMinutes.ToString();
                txtAbsencesAmount.Text = absencesAmount.ToString("N2");
                txtLateAmount.Text = latesAmount.ToString("N2");
                txtUndertimeAmount.Text = undertimeAmount.ToString("N2");
                txtCashAdvance.Text = totalAdvanceBalance.ToString("N2");
                txtSSS.Text = sss.ToString("N2");
                txtPagIbig.Text = pagibig.ToString("N2");
                txtPhilHealth.Text = philhealth.ToString("N2");
                txtNetIncome.Text = netIncome.ToString("N2");
                txtTotalDeduction.Text = totalDeductions.ToString("N2");
                GenTeachingID();
                LoadCashAdvance(employeeId);
            }
        }

        private void TeachingUpdateTDandNI()
        {
            try
            {
                // Parse user input values, default to 0 if invalid
                decimal cashAdvance = string.IsNullOrEmpty(txtCADeduction.Text) ? 0 : Convert.ToDecimal(txtCADeduction.Text);
                decimal otherDeduction = string.IsNullOrEmpty(txtDeduction.Text) ? 0 : Convert.ToDecimal(txtDeduction.Text);
                decimal sss = string.IsNullOrEmpty(txtSSS.Text) ? 0 : Convert.ToDecimal(txtSSS.Text);
                decimal pagibig = string.IsNullOrEmpty(txtPagIbig.Text) ? 0 : Convert.ToDecimal(txtPagIbig.Text);
                decimal philhealth = string.IsNullOrEmpty(txtPhilHealth.Text) ? 0 : Convert.ToDecimal(txtPhilHealth.Text);
                decimal absencesAmount = string.IsNullOrEmpty(txtAbsencesAmount.Text) ? 0 : Convert.ToDecimal(txtAbsencesAmount.Text);
                decimal lateAmount = string.IsNullOrEmpty(txtLateAmount.Text) ? 0 : Convert.ToDecimal(txtLateAmount.Text);
                decimal undertimeAmount = string.IsNullOrEmpty(txtUndertimeAmount.Text) ? 0 : Convert.ToDecimal(txtUndertimeAmount.Text);
                decimal grossSalary = string.IsNullOrEmpty(txtGrossIncome.Text) ? 0 : Convert.ToDecimal(txtGrossIncome.Text);

                // Compute total deductions
                decimal totalDeductions = sss + pagibig + philhealth + absencesAmount + lateAmount + undertimeAmount + cashAdvance + otherDeduction;
                txtTotalDeduction.Text = totalDeductions.ToString("N2");

                // Compute net income
                decimal netIncome = grossSalary - totalDeductions;
                txtNetIncome.Text = netIncome.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please enter numeric values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegularUpdateTDandNI()
        {
            try
            {
                // Parse user input values, default to 0 if invalid
                decimal cashAdvance = string.IsNullOrEmpty(txtRegCADeduction.Text) ? 0 : Convert.ToDecimal(txtRegCADeduction.Text);
                decimal otherDeduction = string.IsNullOrEmpty(txtRegDeduction.Text) ? 0 : Convert.ToDecimal(txtRegDeduction.Text);
                decimal sss = string.IsNullOrEmpty(txtRegSSS.Text) ? 0 : Convert.ToDecimal(txtRegSSS.Text);
                decimal pagibig = string.IsNullOrEmpty(txtRegPagIbig.Text) ? 0 : Convert.ToDecimal(txtRegPagIbig.Text);
                decimal philhealth = string.IsNullOrEmpty(txtRegPhilhealth.Text) ? 0 : Convert.ToDecimal(txtRegPhilhealth.Text);
                decimal grossSalary = string.IsNullOrEmpty(txtRegGrossIncome.Text) ? 0 : Convert.ToDecimal(txtRegGrossIncome.Text);

                // Compute total deductions
                decimal totalDeductions = sss + pagibig + philhealth + cashAdvance + otherDeduction;
                txtRegTotalDeduction.Text = totalDeductions.ToString("N2");

                // Compute net income
                decimal netIncome = grossSalary - totalDeductions;
                txtRegNetIncome.Text = netIncome.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please enter numeric values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (int absences, decimal absencesAmount, int latesMinutes, decimal latesAmount, int undertimeMinutes, decimal undertimeAmount)
        ComputeAttendanceDeductions(string employeeId, DateTime startDate, DateTime endDate, decimal hourlyRate)
        {
            opencon.dbconnect();

            int absences = 0, latesMinutes = 0, undertimeMinutes = 0;
            decimal absencesAmount = 0, latesAmount = 0, undertimeAmount = 0;
            decimal dailyHours = 8; // Default scheduled hours per day

            if (opencon.OpenConnection())
            {
                string query = @"
                                SELECT a.a_date, a.a_timeIn, a.a_timeOut, s.sched_timeIn, s.sched_timeOut
                                FROM attendance a
                                LEFT JOIN schedule s ON a.employee_id = s.employee_id AND a.a_date = s.sched_day
                                WHERE a.employee_id = @EmpID AND a.a_date BETWEEN @StartDate AND @EndDate";

                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                {
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = Convert.ToDateTime(reader["a_date"]);
                            TimeSpan? timeIn = reader["a_timeIn"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(reader["a_timeIn"].ToString());
                            TimeSpan? timeOut = reader["a_timeOut"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(reader["a_timeOut"].ToString());
                            TimeSpan? schedIn = reader["sched_timeIn"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(reader["sched_timeIn"].ToString());
                            TimeSpan? schedOut = reader["sched_timeOut"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(reader["sched_timeOut"].ToString());

                            // **If No Attendance, Count as Absence**
                            if (timeIn == null || timeOut == null)
                            {
                                absences++;
                                absencesAmount += hourlyRate * dailyHours;
                            }
                            else
                            {
                                // **Late Calculation**
                                if (schedIn.HasValue && timeIn.HasValue && timeIn > schedIn)
                                {
                                    int lateMinutes = (int)(timeIn.Value - schedIn.Value).TotalMinutes;
                                    latesMinutes += lateMinutes;
                                    latesAmount += lateMinutes * (hourlyRate / 60);
                                }

                                // **Undertime Calculation**
                                if (schedOut.HasValue && timeOut.HasValue && timeOut < schedOut)
                                {
                                    int undertimeMin = (int)(schedOut.Value - timeOut.Value).TotalMinutes;
                                    undertimeMinutes += undertimeMin;
                                    undertimeAmount += undertimeMin * (hourlyRate / 60);
                                }
                            }
                        }
                    }
                }
                opencon.CloseConnection();
            }

            return (absences, absencesAmount, latesMinutes, latesAmount, undertimeMinutes, undertimeAmount);
        }

        private bool EmployeeHasRegularLoad(string employeeId)
        {

            bool hasRegularLoad = false;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                string query = @"
                            SELECT COUNT(*) 
                            FROM job 
                            WHERE employee_id = @EmpID AND job_status LIKE 'Regular'";

                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                {
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    object result = cmd.ExecuteScalar();
                    hasRegularLoad = Convert.ToInt32(result) > 0;
                }


            }

            opencon.CloseConnection();
            return hasRegularLoad;
        }

        private decimal GetWorkedHours(string employeeId, DateTime startDate, DateTime endDate)
        {
            decimal totalHours = 0;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                string query = "SELECT SUM(TIMESTAMPDIFF(HOUR, a_timeIn, a_timeOut)) FROM attendance WHERE employee_id = @EmpID AND a_date BETWEEN @StartDate AND @EndDate";

                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                {
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    object result = cmd.ExecuteScalar();
                    totalHours = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
            opencon.CloseConnection();
            return totalHours;
        }

        private decimal LoadCashAdvance(string employeeId)
        {
            decimal totalAdvanceBalance = 0;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                string query = @"
                                SELECT SUM(ca_balance) 
                                FROM cash_advance 
                                WHERE employee_id = @EmpID AND ca_balance > 0";

                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                {
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {

                        totalAdvanceBalance = Convert.ToDecimal(result);

                    }
                }

                opencon.CloseConnection();
            }

            // Display the amount in the Cash Advance field
            txtCashAdvance.Text = totalAdvanceBalance.ToString("N2");
            return totalAdvanceBalance;
        }


        /*private decimal GetWorkedDays(string employeeId, DateTime startDate, DateTime endDate)
        {
            decimal totalDays = 0;
            string query = "SELECT COUNT(DISTINCT a_date) FROM attendance WHERE employee_id = @EmpID AND a_date BETWEEN @StartDate AND @EndDate";

            using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
            {
                cmd.Parameters.AddWithValue("@EmpID", employeeId);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                object result = cmd.ExecuteScalar();
                totalDays = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
            return totalDays;
        }*/

        private void txtDeduction_TextChanged(object sender, EventArgs e)
        {
            TeachingUpdateTDandNI();
        }

        private void txtCADeduction_TextChanged(object sender, EventArgs e)
        {
            TeachingUpdateTDandNI();
        }

        private void txtRegDeduction_TextChanged(object sender, EventArgs e)
        {
            RegularUpdateTDandNI();
        }

        private void txtRegCADeduction_TextChanged(object sender, EventArgs e)
        {
            RegularUpdateTDandNI();
            
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (cboStatus.Text == "Regular")
            {
                ComputeRegular();
                gbRegular.Enabled = true;

            }
            else
            {
                ComputeTeaching();
                gbPerHour.Enabled = true;
            }
        }

        private void frmPayroll_Load(object sender, EventArgs e)
        {
            cboStatus.SelectedIndex = 0;
            cboTeachingCutoff.SelectedIndex = 0;
            cboRegularCutOff.SelectedIndex = 0;
            gbPerHour.Enabled = false;
            gbRegular.Enabled = false;
            TeachingPayroll();
            RegularPayroll();
            btnGenerate.Enabled = false;
        }

        private void SaveRegularPayroll()
        {
            string employeeId = txtEmpID.Text.Trim();
            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Please select an employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string payrollId = txtRegularID.Text.Trim();
            decimal cashAdvanceAmount = decimal.TryParse(txtRegCADeduction.Text, out var caAmount) ? caAmount : 0;
            decimal totalDeductions = decimal.TryParse(txtRegTotalDeduction.Text, out var regDeductions) ? regDeductions : 0;
            decimal netIncome = decimal.TryParse(txtRegNetIncome.Text, out var net) ? net : 0;

            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                MySqlTransaction transaction = opencon.connection.BeginTransaction();

                try
                {
                    // Check if an employee already exists
                    string checkQuery = @"SELECT COUNT(*) FROM payroll 
                                      WHERE employee_id = @EmpID AND pr_cutoff = @CutOff";


                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, opencon.connection);
                    checkCmd.Parameters.AddWithValue("@EmpID", employeeId);
                    checkCmd.Parameters.AddWithValue("@CutOff", txtRegCutOff.Text);


                    // Execute the query and get the result
                    int employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (employeeExists > 0)
                    {
                        // Employee with the same full name already exists, show error message
                        MessageBox.Show("An employee with the same Information already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                    else
                    {
                        string payrollQuery = @"
                                            INSERT INTO payroll 
                                            (pr_id, employee_id, pr_date, pr_cutoff, pr_workedhours, pr_grossincome, pr_deductions, pr_netincome, 
                                            pr_lates_minutes, pr_lates_deduction, pr_undertime_minutes, pr_undertime_deduction, pr_absences_days, pr_absence_deduction, pr_job_status)
                                            VALUES 
                                            (@PRID, @EmpID, CURDATE(), @CutOff, NULL, @GrossIncome, @Deductions, @NetIncome, NULL, 
                                            NULL, NULL, NULL, NULL, NULL, 'Regular')";

                        using (MySqlCommand cmd = new MySqlCommand(payrollQuery, opencon.connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@PRID", payrollId);
                            cmd.Parameters.AddWithValue("@EmpID", employeeId);
                            //cmd.Parameters.AddWithValue("@Date", dtpDate.Value);
                            cmd.Parameters.AddWithValue("@CutOff", txtRegCutOff.Text.Trim());
                            cmd.Parameters.AddWithValue("@GrossIncome", decimal.Parse(txtRegGrossIncome.Text));
                            cmd.Parameters.AddWithValue("@Deductions", totalDeductions);
                            cmd.Parameters.AddWithValue("@NetIncome", netIncome);
                            cmd.ExecuteNonQuery();
                        }


                        if (cashAdvanceAmount > 0)
                        {
                            DeductCashAdvance(employeeId, cashAdvanceAmount, payrollId, transaction);
                        }


                        if (totalDeductions > 0)
                        {
                            string deductionQuery = @"
                                                INSERT INTO deduction (employee_id, pr_id, d_description, d_amount)
                                                VALUES (@EmpID, @PRID, @DeductionDesc, @DeductionAmount)";

                            using (MySqlCommand cmd = new MySqlCommand(deductionQuery, opencon.connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmpID", employeeId);
                                cmd.Parameters.AddWithValue("@PRID", payrollId);
                                cmd.Parameters.AddWithValue("@DeductionDesc", txtRegDescription.Text);
                                cmd.Parameters.AddWithValue("@DeductionAmount", totalDeductions);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }


                    transaction.Commit();
                    ClearRegularSalary();
                    ClearTeachingSalary();
                    txtEmpID.Clear();
                    txtFname.Clear();
                    btnGenerate.Enabled = false;
                    RegularPayroll();
                    MessageBox.Show("Regular payroll saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error saving payroll: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void SaveTeachingPayroll()
        {
            string employeeId = txtEmpID.Text.Trim();
            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Please select an employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string payrollId = txtTeachingID.Text.Trim();
            decimal cashAdvanceAmount = decimal.TryParse(txtCADeduction.Text, out var caAmount) ? caAmount : 0;
            decimal totalDeductions = decimal.TryParse(txtTotalDeduction.Text, out var regDeductions) ? regDeductions : 0;
            decimal netIncome = decimal.TryParse(txtNetIncome.Text, out var net) ? net : 0;

            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                MySqlTransaction transaction = opencon.connection.BeginTransaction();

                try
                {

                    bool hasRegularSalary = false;
                    string checkRegularQuery = @"
                                                SELECT COUNT(*) FROM job 
                                                WHERE employee_id = @EmpID AND job_salary IS NOT NULL AND job_hourly_rate IS NOT NULL";

                    using (MySqlCommand checkCmd = new MySqlCommand(checkRegularQuery, opencon.connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@EmpID", employeeId);
                        hasRegularSalary = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                    }

                    // Check if an employee already exists
                    string checkQuery = @"SELECT COUNT(*) FROM payroll 
                                      WHERE employee_id = @EmpID AND pr_cutoff = @CutOff";


                    MySqlCommand Cmdcheck = new MySqlCommand(checkQuery, opencon.connection);
                    Cmdcheck.Parameters.AddWithValue("@EmpID", employeeId);
                    Cmdcheck.Parameters.AddWithValue("@CutOff", txtCutOff.Text);


                    // Execute the query and get the result
                    int employeeExists = Convert.ToInt32(Cmdcheck.ExecuteScalar());

                    if (employeeExists > 0)
                    {
                        // Employee with the same full name already exists, show error message
                        MessageBox.Show("An employee with the same Information already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                    else
                    {
                        string payrollQuery = @"
                                            INSERT INTO payroll 
                                            (pr_id, employee_id, pr_date, pr_cutoff, pr_workedhours, pr_grossincome, pr_deductions, pr_netincome, 
                                            pr_lates_minutes, pr_lates_deduction, pr_undertime_minutes, pr_undertime_deduction, pr_absences_days, pr_absence_deduction, pr_job_status)
                                            VALUES 
                                            (@PRID, @EmpID, CURDATE(), @CutOff, @WorkedHours, @GrossIncome, @Deductions, @NetIncome, 
                                            @LatesMin, @LatesDeduction, @UndertimeMin, @UndertimeDeduction, @Absences, @AbsenceDeduction, 'Teaching')";

                        using (MySqlCommand cmd = new MySqlCommand(payrollQuery, opencon.connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@PRID", payrollId);
                            cmd.Parameters.AddWithValue("@EmpID", employeeId);
                            cmd.Parameters.AddWithValue("@CutOff", txtCutOff.Text.ToString());
                            cmd.Parameters.AddWithValue("@WorkedHours", decimal.Parse(txtWorkedHr.Text));
                            cmd.Parameters.AddWithValue("@GrossIncome", decimal.Parse(txtGrossIncome.Text));
                            cmd.Parameters.AddWithValue("@Deductions", totalDeductions);
                            cmd.Parameters.AddWithValue("@NetIncome", netIncome);
                            cmd.Parameters.AddWithValue("@LatesMin", int.Parse(txtLates.Text));
                            cmd.Parameters.AddWithValue("@LatesDeduction", decimal.Parse(txtLateAmount.Text));
                            cmd.Parameters.AddWithValue("@UndertimeMin", int.Parse(txtUndertime.Text));
                            cmd.Parameters.AddWithValue("@UndertimeDeduction", decimal.Parse(txtUndertimeAmount.Text));
                            cmd.Parameters.AddWithValue("@Absences", int.Parse(txtAbsence.Text));
                            cmd.Parameters.AddWithValue("@AbsenceDeduction", decimal.Parse(txtAbsencesAmount.Text));
                            cmd.ExecuteNonQuery();
                        }

                        // 🔹 3️⃣ Deduct Cash Advance & Insert Deduction if No Regular Salary
                        if (!hasRegularSalary)
                        {
                            // 🔸 Deduct Cash Advance
                            DeductCashAdvance(employeeId, cashAdvanceAmount, payrollId, transaction);

                            // 🔸 Insert Deduction Records
                            string deductionQuery = @"
                                                INSERT INTO deduction (employee_id, pr_id, d_description, d_amount)
                                                VALUES (@EmpID, @PRID, @DeductionDesc, @DeductionAmount)";

                            using (MySqlCommand cmd = new MySqlCommand(deductionQuery, opencon.connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmpID", employeeId);
                                cmd.Parameters.AddWithValue("@PRID", payrollId);
                                cmd.Parameters.AddWithValue("@DeductionDesc", txtDescription.Text);
                                cmd.Parameters.AddWithValue("@DeductionAmount", totalDeductions);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }


                    transaction.Commit();
                    ClearRegularSalary();
                    ClearTeachingSalary();
                    txtEmpID.Clear();
                    txtFname.Clear();
                    btnGenerate.Enabled = false;
                    TeachingPayroll();
                    MessageBox.Show("Teaching payroll saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error saving payroll: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void DeductCashAdvance(string employeeId, decimal availableDeduction, string payrollId, MySqlTransaction transaction)
        {
            try
            {
                string query = @"
                                SELECT ca_id, ca_balance 
                                FROM cash_advance 
                                WHERE employee_id = @EmpID AND ca_balance > 0 
                                ORDER BY ca_id ASC";

                List<(string caId, decimal balance)> advances = new List<(string, decimal)>();

                using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmpID", employeeId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            advances.Add((reader.GetString("ca_id"), reader.GetDecimal("ca_balance")));
                        }
                    }
                }

                if (advances.Count == 0 || availableDeduction <= 0)
                {
                    return; // No cash advance to deduct
                }

                foreach (var advance in advances)
                {
                    if (availableDeduction <= 0) break;

                    decimal deduction = Math.Min(availableDeduction, advance.balance);
                    availableDeduction -= deduction;

                    // Update cash advance balance
                    string updateQuery = @"
                                            UPDATE cash_advance 
                                            SET ca_balance = ca_balance - @Deduction 
                                            WHERE ca_id = @CAID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, opencon.connection, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@Deduction", deduction);
                        updateCmd.Parameters.AddWithValue("@CAID", advance.caId);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Insert record in cash_advance_payment
                    string insertQuery = @"
                                            INSERT INTO cash_advance_payment 
                                            (pr_id, employee_id, ca_id, payment_amount, remaining_balance, payment_date)
                                            VALUES (@PRID, @EmpID, @CAID, @Deduction, @Remaining, CURDATE())";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, opencon.connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@PRID", payrollId);
                        insertCmd.Parameters.AddWithValue("@EmpID", employeeId);
                        insertCmd.Parameters.AddWithValue("@CAID", advance.caId);
                        insertCmd.Parameters.AddWithValue("@Deduction", deduction);
                        insertCmd.Parameters.AddWithValue("@Remaining", advance.balance - deduction);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Error deducting cash advance: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnTeachingSave_Click(object sender, EventArgs e)
        {
            SaveTeachingPayroll();
        }

        private void btnRegSave_Click(object sender, EventArgs e)
        {
            SaveRegularPayroll();
        }

        private void TeachingPayroll()
        {
            DateTime date = DateTime.Now;
            string monthYear = date.ToString("MMMM yyyy");
            string cutoff = cboTeachingCutoff.Text;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        payroll.pr_id AS 'Payroll ID',
                                        DATE_FORMAT(pr_date, '%M %Y') AS 'Month',
                                        payroll.pr_cutoff AS 'Cutoff Period',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        payroll.pr_workedhours AS 'Worked Hours',
                                        payroll.pr_grossincome AS 'Gross Income',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Cash Advance Amount',
                                        cash_advance_payment.payment_amount AS 'Cash Advance Deducted',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance',
                                        deduction.d_amount AS 'Other Deduction',
                                        deduction.d_description AS 'Description',
                                        payroll.pr_lates_minutes AS 'Lates Per Min',
                                        payroll.pr_lates_deduction AS 'Lates In Amount',
                                        payroll.pr_undertime_minutes AS 'Undertime Per Min',
                                        payroll.pr_undertime_deduction AS 'Undertime In Amount',
                                        payroll.pr_absences_days AS 'Absences',
                                        payroll.pr_absence_deduction AS 'Absences In Amount',
                                        payroll.pr_deductions AS 'Overall Deduction',
                                        payroll.pr_netincome AS 'Net Income',
                                        payroll.pr_job_status AS 'Category'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    LEFT JOIN payroll ON employee.employee_id = payroll.employee_id
                                    LEFT JOIN deduction ON payroll.pr_id = deduction.pr_id
                                    LEFT JOIN cash_advance_payment ON payroll.pr_id = cash_advance_payment.pr_id
                                    WHERE payroll.pr_job_status = 'Teaching' AND payroll.pr_cutoff LIKE @CutOff
                                    AND DATE_FORMAT(pr_date, '%M %Y') LIKE @Date
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_hourly_rate, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth,
                                        payroll.pr_id,
                                        cash_advance_payment.payment_amount,
                                        deduction.d_amount,
                                        deduction.d_description
                                    LIMIT 1000";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@CutOff", "%" + cutoff + "%");
                    cmd.Parameters.AddWithValue("@Date", "%" + monthYear + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridTeaching.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void RegularPayroll()
        {
            DateTime date = DateTime.Now;
            string monthYear = date.ToString("MMMM yyyy");
            string cutoff = cboRegularCutOff.Text;
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        payroll.pr_id AS 'Payroll ID',
                                        DATE_FORMAT(pr_date, '%M %Y') AS 'Month',
                                        payroll.pr_cutoff AS 'Cutoff Period',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        payroll.pr_workedhours AS 'Worked Hours',
                                        payroll.pr_grossincome AS 'Gross Income',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Cash Advance Amount',
                                        cash_advance_payment.payment_amount AS 'Cash Advance Deducted',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance',
                                        deduction.d_amount AS 'Other Deduction',
                                        deduction.d_description AS 'Description',
                                        payroll.pr_deductions AS 'Overall Deduction',
                                        payroll.pr_netincome AS 'Net Income',
                                        payroll.pr_job_status AS 'Category'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    LEFT JOIN payroll ON employee.employee_id = payroll.employee_id
                                    LEFT JOIN deduction ON payroll.pr_id = deduction.pr_id
                                    LEFT JOIN cash_advance_payment ON payroll.pr_id = cash_advance_payment.pr_id
                                    WHERE payroll.pr_job_status = 'Regular' AND payroll.pr_cutoff LIKE @CutOff
                                    AND DATE_FORMAT(pr_date, '%M %Y') LIKE @Date
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_hourly_rate, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth,
                                        payroll.pr_id,
                                        cash_advance_payment.payment_amount,
                                        deduction.d_amount,
                                        deduction.d_description
                                    LIMIT 1000";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@CutOff", "%" + cutoff + "%");
                    cmd.Parameters.AddWithValue("@Date", "%" + monthYear + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridRegular.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

        private void txtEmpID_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmpID.Text))
            {
                btnGenerate.Enabled = false;
            }
            else
            {
                btnGenerate.Enabled = true;
            }
        }

        private void txtRegCashAdvance_TextChanged(object sender, EventArgs e)
        {
            if (txtRegCashAdvance.Text == "0.00")
            {
                txtRegCADeduction.Enabled = false;
            }
            else
            {
                txtRegCADeduction.Enabled = true;
            }
        }

        private void txtSearchTeachingSalary_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearchTeachingSalary.Text.Trim();
            DateTime date = DateTime.Now;
            string monthYear = date.ToString("MMMM yyyy");
            string cutoff = cboTeachingCutoff.Text;

            if (string.IsNullOrEmpty(searchValue) && !string.IsNullOrEmpty(cutoff))
            {
                TeachingPayroll(); // Reload all data if search box is empty
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        payroll.pr_id AS 'Payroll ID',
                                        DATE_FORMAT(pr_date, '%M %Y') AS 'Month',
                                        payroll.pr_cutoff AS 'Cutoff Period',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        payroll.pr_workedhours AS 'Worked Hours',
                                        payroll.pr_grossincome AS 'Gross Income',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Cash Advance Amount',
                                        cash_advance_payment.payment_amount AS 'Cash Advance Deducted',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance',
                                        deduction.d_amount AS 'Other Deduction',
                                        deduction.d_description AS 'Description',
                                        payroll.pr_lates_minutes AS 'Lates Per Min',
                                        payroll.pr_lates_deduction AS 'Lates In Amount',
                                        payroll.pr_undertime_minutes AS 'Undertime Per Min',
                                        payroll.pr_undertime_deduction AS 'Undertime In Amount',
                                        payroll.pr_absences_days AS 'Absences',
                                        payroll.pr_absence_deduction AS 'Absences In Amount',
                                        payroll.pr_deductions AS 'Overall Deduction',
                                        payroll.pr_netincome AS 'Net Income',
                                        payroll.pr_job_status AS 'Category'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    LEFT JOIN payroll ON employee.employee_id = payroll.employee_id
                                    LEFT JOIN deduction ON payroll.pr_id = deduction.pr_id
                                    LEFT JOIN cash_advance_payment ON payroll.pr_id = cash_advance_payment.pr_id
                                    WHERE payroll.pr_job_status = 'Teaching' AND payroll.pr_cutoff LIKE @CutOff
                                    AND DATE_FORMAT(pr_date, '%M %Y') LIKE @Date
                                    AND CONCAT(COALESCE(employee.employee_fname, ''), ' ',
                                           COALESCE(employee.employee_mname, ''), ' ',
                                           COALESCE(employee.employee_lname, '')) LIKE @Search
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_hourly_rate, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth,
                                        payroll.pr_id,
                                        cash_advance_payment.payment_amount,
                                        deduction.d_amount,
                                        deduction.d_description
                                    LIMIT 1000";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@CutOff", "%" + cutoff + "%");
                    cmd.Parameters.AddWithValue("@Date", "%" + monthYear + "%");
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridTeaching.DataSource = dt;

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

        private void txtSearchRegularSalary_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearchRegularSalary.Text.Trim();
            DateTime date = DateTime.Now;
            string monthYear = date.ToString("MMMM yyyy");
            string cutoff = cboRegularCutOff.Text;

            if (string.IsNullOrEmpty(searchValue) && !string.IsNullOrEmpty(cutoff))
            {
                TeachingPayroll(); // Reload all data if search box is empty
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT 
                                        employee.employee_id AS 'Employee ID', 
                                        CONCAT(COALESCE(employee.employee_fname, ''), ' ', 
                                               COALESCE(employee.employee_mname, ''), ' ', 
                                               COALESCE(employee.employee_lname, '')) AS 'Full Name', 
                                        payroll.pr_id AS 'Payroll ID',
                                        DATE_FORMAT(pr_date, '%M %Y') AS 'Month',
                                        payroll.pr_cutoff AS 'Cutoff Period',
                                        job.job_hourly_rate AS 'Hourly Rate',
                                        payroll.pr_workedhours AS 'Worked Hours',
                                        payroll.pr_grossincome AS 'Gross Income',
                                        contribution.sss AS 'SSS',
                                        contribution.pagibig AS 'Pag Ibig',
                                        contribution.philhealth AS 'PhilHealth',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Cash Advance Amount',
                                        cash_advance_payment.payment_amount AS 'Cash Advance Deducted',
                                        COALESCE(SUM(cash_advance.ca_balance), 0) AS 'Total Advance Balance',
                                        deduction.d_amount AS 'Other Deduction',
                                        deduction.d_description AS 'Description',
                                        payroll.pr_deductions AS 'Overall Deduction',
                                        payroll.pr_netincome AS 'Net Income',
                                        payroll.pr_job_status AS 'Category'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    LEFT JOIN payroll ON employee.employee_id = payroll.employee_id
                                    LEFT JOIN deduction ON payroll.pr_id = deduction.pr_id
                                    LEFT JOIN cash_advance_payment ON payroll.pr_id = cash_advance_payment.pr_id
                                    WHERE payroll.pr_job_status = 'Regular' AND payroll.pr_cutoff LIKE @CutOff
                                    AND DATE_FORMAT(pr_date, '%M %Y') LIKE @Date
                                    AND CONCAT(COALESCE(employee.employee_fname, ''), ' ',
                                           COALESCE(employee.employee_mname, ''), ' ',
                                           COALESCE(employee.employee_lname, '')) LIKE @Search
                                    GROUP BY 
                                        employee.employee_id, 
                                        employee.employee_fname, 
                                        employee.employee_mname, 
                                        employee.employee_lname, 
                                        job.job_hourly_rate, 
                                        contribution.sss, 
                                        contribution.pagibig, 
                                        contribution.philhealth,
                                        payroll.pr_id,
                                        cash_advance_payment.payment_amount,
                                        deduction.d_amount,
                                        deduction.d_description
                                    LIMIT 1000";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@CutOff", "%" + cutoff + "%");
                    cmd.Parameters.AddWithValue("@Date", "%" + monthYear + "%");
                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridRegular.DataSource = dt;



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

        private void cboTeachingCutoff_SelectedIndexChanged(object sender, EventArgs e)
        {
            TeachingPayroll();
        }

        private void cboRegularCutOff_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegularPayroll();
        }

        private void txtRegCADeduction_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }

        private void txtRegDeduction_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }

        private void txtCADeduction_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }

        private void txtDeduction_KeyPress(object sender, KeyPressEventArgs e)
        {
            mf.txtNumber(sender, e);
        }
    }


}
