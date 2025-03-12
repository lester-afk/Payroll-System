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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            
        }
        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();

        private string empCount;
        private string attCount;
        private void frmMain_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // Set timer to tick every second
            timer1.Start(); // Start the timer

            GetEmpTotal();
            GetAttTotal();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt");
        }

       private void GetEmpTotal()
        {
            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT COUNT(*) AS TOTAL
                                    FROM employee ";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@TOTAL", empCount);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblEmp.Text = reader["TOTAL"].ToString();
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

        private void GetAttTotal()
        {
            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT COUNT(*) AS TOTAL, a_date
                                    FROM attendance WHERE a_date = @CurrentDay ";

                    MySqlCommand cmd = new MySqlCommand(query, opencon.connection);
                    cmd.Parameters.AddWithValue("@TOTAL", attCount);
                    cmd.Parameters.AddWithValue("@CurrentDay", DateTime.Now.ToString("yyyy/MM/dd"));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblAttendance.Text = reader["TOTAL"].ToString();
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

        private void btnSemester_Click(object sender, EventArgs e)
        {
            Dashboard openDashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();

            if (openDashboard != null)
            {
                // Pass the open Dashboard instance to frmSemester
                frmSemester formSemester = new frmSemester(openDashboard);
                formSemester.ShowDialog();
            }
        }

        private void btnSY_Click(object sender, EventArgs e)
        {
            Dashboard openDashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();

            if (openDashboard != null)
            {
                // Pass the open Dashboard instance to frmSemester
                frmSchoolYear formShoolYear = new frmSchoolYear(openDashboard);
                formShoolYear.ShowDialog();
            }
        }
    }
}
