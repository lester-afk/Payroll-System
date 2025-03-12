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
    public partial class Dashboard : Form
    {
        // Form instances
        private frmEmployee formEmployee;
        private frmAttendance formAttendance;
        private frmSchedule formSchedule;
        private frmCashAdvance formCashAdvance;
        private frmReports formReports;
        private frmMain formMain;
        private frmPayroll formPayroll;
        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        public Dashboard()
        {
            InitializeComponent();
            this.Size = new Size(SystemInformation.WorkingArea.Width, SystemInformation.WorkingArea.Height);
        }

        private void popUpSide()
        {

            if (sideBarPanel.Width <= 70)
            {

                sideBarPanel.Width = 200;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }
            else
            {

                if (sideBarPanel.Width >= 200)
                {
                    sideBarPanel.Width = 70;

                    pnlAtt.Width = sideBarPanel.Width;
                    pnlCash.Width = sideBarPanel.Width;
                    pnlDash.Width = sideBarPanel.Width;
                    pnlEmp.Width = sideBarPanel.Width;
                    pnlPay.Width = sideBarPanel.Width;
                    pnlSched.Width = sideBarPanel.Width;
                    pnlSettings.Width = sideBarPanel.Width;
                }
            }
        }

        // Method to close all open forms
        private void CloseAllForms()
        {
            if (formMain != null && !formMain.IsDisposed)
                formMain.Close();
            if (formEmployee != null && !formEmployee.IsDisposed)
                formEmployee.Close();
            if (formAttendance != null && !formAttendance.IsDisposed)
                formAttendance.Close();
            if (formSchedule != null && !formSchedule.IsDisposed)
                formSchedule.Close();
            if (formCashAdvance != null && !formCashAdvance.IsDisposed)
                formCashAdvance.Close();
            if (formPayroll != null && !formPayroll.IsDisposed)
                formPayroll.Close();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {

            popUpSide();

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            ActiveSY();
            ActiveSemester();
            lblPayroll.Text = "Payroll System";
            CloseAllForms();

            // Show Employee form
            if (formMain == null || formMain.IsDisposed)
            {
                formMain = new frmMain();
                formMain.MdiParent = this;
                formMain.Show();
                formMain.Dock = DockStyle.Fill;
                lblPayroll.Text = "Dashboard";
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            CloseAllForms();
            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Employee form
            if (formMain == null || formMain.IsDisposed)
            {
                formMain = new frmMain();
                formMain.MdiParent = this;
                formMain.Show();
                formMain.Dock = DockStyle.Fill;
                lblPayroll.Text = "Dashboard";
            }
        }
        private void btnEmployee_Click(object sender, EventArgs e)
        {
            // Close all other forms
            CloseAllForms();

            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Employee form
            if (formEmployee == null || formEmployee.IsDisposed)
            {
                formEmployee = new frmEmployee();
                formEmployee.MdiParent = this;
                formEmployee.Show();
                formEmployee.Dock = DockStyle.Fill;
                lblPayroll.Text = "Employee";
            }
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            // Close all other forms
            CloseAllForms();

            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Attendance form
            if (formAttendance == null || formAttendance.IsDisposed)
            {
                formAttendance = new frmAttendance();
                formAttendance.MdiParent = this;
                formAttendance.Show();
                formAttendance.Dock = DockStyle.Fill;
                lblPayroll.Text = "Attendance";
            }
        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            // Close all other forms
            CloseAllForms();

            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Schedule form
            if (formSchedule == null || formSchedule.IsDisposed)
            {
                formSchedule = new frmSchedule();
                formSchedule.MdiParent = this;
                formSchedule.Show();
                formSchedule.Dock = DockStyle.Fill;
                lblPayroll.Text = "Schedule";
            }
        }

        private void btnCashAdvance_Click(object sender, EventArgs e)
        {
            // Close all other forms
            CloseAllForms();

            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Cash Advance form
            if (formCashAdvance == null || formCashAdvance.IsDisposed)
            {
                formCashAdvance = new frmCashAdvance();
                formCashAdvance.MdiParent = this;
                formCashAdvance.Show();
                formCashAdvance.Dock = DockStyle.Fill;
                lblPayroll.Text = "Cash Advance";
            }
        }
        private void btnPayroll_Click(object sender, EventArgs e)
        {
            // Close all other forms
            CloseAllForms();

            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Reports form
            if (formPayroll == null || formPayroll.IsDisposed)
            {
                formPayroll = new frmPayroll();
                formPayroll.MdiParent = this;
                formPayroll.Show();
                formPayroll.Dock = DockStyle.Fill;
                lblPayroll.Text = "Payroll ";
            }
        }

        /*private void btnPayslip_Click(object sender, EventArgs e)
        {
            // Close all other forms
            CloseAllForms();

            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }

            // Show Reports form
            if (formReports == null || formReports.IsDisposed)
            {
                formReports = new frmReports();
                formReports.MdiParent = this;
                formReports.Show();
                formReports.Dock = DockStyle.Fill;
                lblPayroll.Text = "Payslip Report";
            }
        }*/

        private void btnSettings_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to Log Out?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }


            if (sideBarPanel.Width >= 200)
            {
                sideBarPanel.Width = 70;

                pnlAtt.Width = sideBarPanel.Width;
                pnlCash.Width = sideBarPanel.Width;
                pnlDash.Width = sideBarPanel.Width;
                pnlEmp.Width = sideBarPanel.Width;
                pnlPay.Width = sideBarPanel.Width;
                pnlSched.Width = sideBarPanel.Width;
                pnlSettings.Width = sideBarPanel.Width;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        public void ActiveSY()
        {
            opencon.dbconnect(); // Ensure connection setup

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                SELECT CONCAT(sy_start, '-', sy_end) AS 'ActiveSY'
                FROM school_year
                WHERE is_active = 1";  // Ensure you have a column marking the active school year

                    using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblYear.Text = "S.Y: " + reader["ActiveSY"].ToString();
                        }
                        else
                        {
                            lblYear.Text = "No Active School Year Found.";
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error fetching active school year: {ex.Message}",
                                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection(); // Ensure connection is always closed
                }
            }
        }


        public void ActiveSemester()
        {
            opencon.dbconnect(); // Ensure the database connection is set up

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"
                                    SELECT sem_active_semester
                                    FROM semester
                                    WHERE isActive = 1";  // Ensure you have an "is_active" column

                    using (MySqlCommand cmd = new MySqlCommand(query, opencon.connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblSemester.Text = "Active Semester: " + reader["sem_active_semester"].ToString();
                        }
                        else
                        {
                            lblSemester.Text = "No Active Semester Found.";
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error fetching active semester: {ex.Message}",
                                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }

        }

    }
}
