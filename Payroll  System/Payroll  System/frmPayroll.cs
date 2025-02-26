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
                                        job.job_hourly_rate AS 'Salary',
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
            }
            else
            {
                RegularLoad("Regular");
            }
             
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }

   
}
