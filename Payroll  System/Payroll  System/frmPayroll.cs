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
                txtEmpID.Text = selectedRow.Cells["Emplyee ID"].Value.ToString();
                txtFname.Text = selectedRow.Cells["Full Name"].Value.ToString();
                txtFname.Text = selectedRow.Cells["Cash Advance ID"].Value.ToString();
            }

        }

        private void ComputeTeaching()
        {
            if (dataGridViewEmployee.SelectedRows.Count > 0)
            {
                DateTime currentDate = DateTime.Now;


                // Get selected Employee ID
                string employeeId = dataGridViewEmployee.SelectedRows[0].Cells["Employee ID"].Value.ToString();
                string jobStatus = dataGridViewEmployee.SelectedRows[0].Cells["Job Status"].Value.ToString();
                decimal hourlyRate = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Hourly Rate"].Value);

                // Get Contribution & Deductions
                decimal sss = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["SSS"].Value) / 2; // Split into two cut-offs
                decimal pagibig = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Pag Ibig"].Value) / 2;
                decimal philhealth = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["PhilHealth"].Value) / 2;
                decimal cashAdvance = GetCashAdvance(employeeId);

                // Get current date,                           
                int lastDayOfMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                DateTime firstCutOffEnd = new DateTime(currentDate.Year, currentDate.Month, 14);
                DateTime secondCutOffEnd = new DateTime(currentDate.Year, currentDate.Month, lastDayOfMonth);
                DateTime thirdCutOffEnd = new DateTime(currentDate.Year, currentDate.Month, 14).AddMonths(1); // 31st - 14th (next month)

                string payrollPeriod;
                DateTime startDate, endDate;

                if (currentDate.Day <= 14)  // First Cut-off: 1st to 14th
                {
                    payrollPeriod = "1st - 14th";
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    endDate = firstCutOffEnd;
                }
                else if (currentDate.Day <= lastDayOfMonth)  // Second Cut-off: 15th to 30th/31st
                {
                    payrollPeriod = "15th - " + lastDayOfMonth;
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                    endDate = secondCutOffEnd;
                }
                else  // Third Cut-off: 31st - 14th (next month)
                {
                    payrollPeriod = "31st - 14th (Next Month)";
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 31);
                    endDate = thirdCutOffEnd;
                }

                // Get Attendance Data Within Cut-Off Period
                decimal workedHours = GetWorkedHours(employeeId, startDate, endDate);

                // Compute Salary
                decimal partTimeSalary = workedHours * hourlyRate;

                // Compute Total Deductions
                decimal totalDeductions = sss + pagibig + philhealth + cashAdvance;

                // Compute Net Income
                decimal netPartTime = partTimeSalary - totalDeductions;



            }

        }

        private void ComputePayroll()
        {
            if (dataGridViewEmployee.SelectedRows.Count > 0)
            {
                DateTime currentDate = DateTime.Now;
                // Get selected Employee ID
                string employeeId = dataGridViewEmployee.SelectedRows[0].Cells["Employee ID"].Value.ToString();
                string jobStatus = dataGridViewEmployee.SelectedRows[0].Cells["Job Status"].Value.ToString();

                // Get Salary Information
                decimal baseSalary = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Salary"].Value);
                decimal hourlyRate = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Hourly Rate"].Value);

                // Get Contribution & Deductions
                decimal sss = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["SSS"].Value) / 2; // Split into two cut-offs
                decimal pagibig = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["Pag Ibig"].Value) / 2;
                decimal philhealth = Convert.ToDecimal(dataGridViewEmployee.SelectedRows[0].Cells["PhilHealth"].Value) / 2;
                decimal cashAdvance = GetCashAdvance(employeeId);

                // Get current date,                           
                int lastDayOfMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                DateTime firstCutOffEnd = new DateTime(currentDate.Year, currentDate.Month, 14);
                DateTime secondCutOffEnd = new DateTime(currentDate.Year, currentDate.Month, lastDayOfMonth);
                DateTime thirdCutOffEnd = new DateTime(currentDate.Year, currentDate.Month, 14).AddMonths(1); // 31st - 14th (next month)

                string payrollPeriod;
                DateTime startDate, endDate;

                if (currentDate.Day <= 14)  // First Cut-off: 1st to 14th
                {
                    payrollPeriod = "1st - 14th";
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    endDate = firstCutOffEnd;
                }
                else if (currentDate.Day <= lastDayOfMonth)  // Second Cut-off: 15th to 30th/31st
                {
                    payrollPeriod = "15th - " + lastDayOfMonth;
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                    endDate = secondCutOffEnd;
                }
                else  // Third Cut-off: 31st - 14th (next month)
                {
                    payrollPeriod = "31st - 14th (Next Month)";
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 31);
                    endDate = thirdCutOffEnd;
                }

                // Get Attendance Data Within Cut-Off Period
                decimal workedDays = GetWorkedDays(employeeId, startDate, endDate);
                decimal workedHours = GetWorkedHours(employeeId, startDate, endDate);

                // Compute Salary
                decimal regularSalary = baseSalary / 2; // Split into two cut-offs
                decimal partTimeSalary = workedHours * hourlyRate;

                // Compute Total Deductions
                decimal totalDeductions = sss + pagibig + philhealth + cashAdvance;

                // Compute Net Income
                decimal netRegular = regularSalary - totalDeductions;
                decimal netPartTime = partTimeSalary - totalDeductions;

                // Display in UI
                txtRegCutOff.Text = payrollPeriod;
                txtSalary.Text = regularSalary.ToString("N2");
                txtWorkedDays.Text = workedDays.ToString();
                txtRegGrossIncome.Text = regularSalary.ToString("N2");

                txtRegSSS.Text = sss.ToString("N2");
                txtRegPagIbig.Text = pagibig.ToString("N2");
                txtRegPhilhealth.Text = philhealth.ToString("N2");
                txtRegCashAdvance.Text = cashAdvance.ToString("N2");

                txtRegDeduction.Text = totalDeductions.ToString("N2");
                txtRegNetIncome.Text = netRegular.ToString("N2");

                txtPerHour.Text = hourlyRate.ToString("N2");
                txtWorkedHr.Text = workedHours.ToString();
                txtGrossIncome.Text = partTimeSalary.ToString("N2");

                txtTotalDeduction.Text = totalDeductions.ToString("N2");
                txtNetIncome.Text = netPartTime.ToString("N2");

                MessageBox.Show($"Payroll computed for {payrollPeriod}", "Payroll Computation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an employee from the list.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

        private decimal GetCashAdvance(string employeeId)
        {
            decimal advanceAmount = 0;
            string query = "SELECT SUM(ca_amount) FROM cash_advance WHERE employee_id = @EmpID";

            using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
            {
                cmd.Parameters.AddWithValue("@EmpID", employeeId);
                object result = cmd.ExecuteScalar();
                advanceAmount = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
            return advanceAmount;
        }



    }


}
