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

        private void RegularLoad(string loadValue)
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
                                        cash_advance.ca_id AS 'Cash Advance ID',
                                        cash_advance.ca_amount AS 'Advance Amount',
                                        cash_advance.ca_balance AS 'Advance Balance'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    WHERE job.job_status LIKE @Load";

                    // Use the MySqlCommand object with the parameter
                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Load", "%" + loadValue + "%");

                    // Use the MySqlCommand object in the MySqlDataAdapter
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Bind the DataTable to the DataGridView
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
                    // Ensure the connection is closed even if an exception occurs
                    opencon.CloseConnection();
                }
            }
        }

        private void TeachingLoad(string loadValue)
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
                                        cash_advance.ca_id AS 'Cash Advance ID',
                                        cash_advance.ca_amount AS 'Advance Amount',
                                        cash_advance.ca_balance AS 'Advance Balance'
                                    FROM employee
                                    LEFT JOIN job ON employee.employee_id = job.employee_id
                                    LEFT JOIN contribution ON employee.employee_id = contribution.employee_id
                                    LEFT JOIN cash_advance ON employee.employee_id = cash_advance.employee_id
                                    WHERE job.job_status LIKE @Load";

                    // Use the MySqlCommand object with the parameter
                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@Load", "%" + loadValue + "%");

                    // Use the MySqlCommand object in the MySqlDataAdapter
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Bind the DataTable to the DataGridView
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
                    // Ensure the connection is closed even if an exception occurs
                    opencon.CloseConnection();
                }
            }
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStatus.Text == "Part Time")
            {
                TeachingLoad("Part Time");
                gbPerHour.Show();
                gbRegular.Hide();
            }
            else if(cboStatus.Text == "Regular")
            {
                RegularLoad("Regular");
                gbPerHour.Hide();
                gbRegular.Show();
            }
             
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewEmployee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = this.dataGridViewEmployee.Rows[e.RowIndex];
                txtEmpID.Text = selectedRow.Cells["Employee ID"].Value.ToString();
                txtFname.Text = selectedRow.Cells["Full Name"].Value.ToString();
                txtCashAdvance.Text = selectedRow.Cells["Cash Advance ID"].Value.ToString();
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

                // Get Contributions & Deductions
                decimal sss = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["SSS"].Value) / 2; // Split into two cut-offs
                decimal pagibig = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Pag Ibig"].Value) / 2;
                decimal philhealth = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["PhilHealth"].Value) / 2;
                decimal cashadvance = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Advance Amount"].Value);

                // Determine Payroll Cut-off Dates
                int lastDayOfMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                DateTime startDate, endDate;
                if (currentDate.Day <= 14)
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, 14);
                }
                else if (currentDate.Day <= lastDayOfMonth)
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, lastDayOfMonth);
                }
                else
                {
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 31);
                    endDate = new DateTime(currentDate.Year, currentDate.Month + 1, 14);
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
                txtPerHour.Text = hourlyRate.ToString("C2");
                txtWorkedDays.Text = workedHours.ToString();
                txtGrossIncome.Text = grossSalary.ToString("C2");
                txtAbsence.Text = absences.ToString();
                txtLates.Text = latesMinutes.ToString();
                txtUndertime.Text = undertimeMinutes.ToString();
                txtAbsencesAmount.Text = absencesAmount.ToString("C2");
                txtLateAmount.Text = latesAmount.ToString("C2");
                txtUndertimeAmount.Text = undertimeAmount.ToString("C2");
                txtCashAdvance.Text = cashadvance.ToString("C2");
                txtSSS.Text = sss.ToString("C2");
                txtPagIbig.Text = pagibig.ToString("C2");
                txtPhilHealth.Text = philhealth.ToString("C2");
                txtNetIncome.Text = netIncome.ToString("C2");
            }
        }

        private void UpdateTotalDeductionsAndNetIncome()
        {
            try
            {
                // Parse user input values, default to 0 if invalid
                decimal cashAdvance = string.IsNullOrEmpty(txtCashAdvance.Text) ? 0 : Convert.ToDecimal(txtCashAdvance.Text);
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
                txtTotalDeduction.Text = totalDeductions.ToString("C2");

                // Compute net income
                decimal netIncome = grossSalary - totalDeductions;
                txtNetIncome.Text = netIncome.ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please enter numeric values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private (int absences, decimal absencesAmount, int latesMinutes, decimal latesAmount, int undertimeMinutes, decimal undertimeAmount)
        ComputeAttendanceDeductions(string employeeId, DateTime startDate, DateTime endDate, decimal hourlyRate)
        {
            int absences = 0, latesMinutes = 0, undertimeMinutes = 0;
            decimal absencesAmount = 0, latesAmount = 0, undertimeAmount = 0;
            decimal dailyHours = 8; // Default scheduled hours per day

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

                        if (timeIn == null && timeOut == null)
                        {
                            absences++; // Absence (No attendance record)
                            absencesAmount += hourlyRate * dailyHours; // Deduct full day salary
                        }
                        else
                        {
                            if (timeIn > schedIn)
                            {
                                int lateMinutes = (int)(timeIn.Value - schedIn.Value).TotalMinutes;
                                latesMinutes += lateMinutes;
                                latesAmount += lateMinutes * (hourlyRate / 60); // Convert to per-minute deduction
                            }

                            if (timeOut < schedOut)
                            {
                                undertimeMinutes = (int)(schedOut.Value - timeOut.Value).TotalMinutes;
                                undertimeMinutes += undertimeMinutes;
                                undertimeAmount += undertimeMinutes * (hourlyRate / 60);
                            }
                        }
                    }
                }
            }

            return (absences, absencesAmount, latesMinutes, latesAmount, undertimeMinutes, undertimeAmount);
        }




        private decimal GetWorkedDays(string employeeId, DateTime startDate, DateTime endDate)
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
        }

        private decimal GetWorkedHours(string employeeId, DateTime startDate, DateTime endDate)
        {
            decimal totalHours = 0;
            string query = "SELECT SUM(TIMESTAMPDIFF(HOUR, a_timeIn, a_timeOut)) FROM attendance WHERE employee_id = @EmpID AND a_date BETWEEN @StartDate AND @EndDate";

            using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
            {
                cmd.Parameters.AddWithValue("@EmpID", employeeId);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                object result = cmd.ExecuteScalar();
                totalHours = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
            return totalHours;
        }

        private void txtDeduction_TextChanged(object sender, EventArgs e)
        {
            UpdateTotalDeductionsAndNetIncome();
        }

        private void txtCADeduction_TextChanged(object sender, EventArgs e)
        {
            UpdateTotalDeductionsAndNetIncome();
        }
    }


}
