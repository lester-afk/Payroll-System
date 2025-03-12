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
    public partial class frmSchoolYear : Form
    {
        public frmSchoolYear(Dashboard parent)
        {
            InitializeComponent();
            formDashboard = parent;
        }
        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        Dashboard formDashboard;
        private void frmSchoolYear_Load(object sender, EventArgs e)
        {
            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.CustomFormat = "yyyy";  // Display only the year
            dtpStart.ShowUpDown = true;

            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.CustomFormat = "yyyy";  // Display only the year
            dtpEnd.ShowUpDown = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to Close?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Select School Year to activate.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            int startYear = dtpStart.Value.Year;
            int endYear = dtpEnd.Value.Year;

            if (startYear >= endYear)
            {
                MessageBox.Show("Start year must be less than end year.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    // First, set all school years to inactive (only one active school year is allowed)
                    string resetQuery = "UPDATE school_year SET is_active = 0";
                    using (MySqlCommand resetCmd = new MySqlCommand(resetQuery, opencon.connection))
                    {
                        resetCmd.ExecuteNonQuery();
                    }

                    // Now, update or insert the selected school year and set it as active
                    string updateQuery = @"
                                            INSERT INTO school_year (sy_start, sy_end, is_active) 
                                            VALUES (@StartYear, @EndYear, 1)
                                            ON DUPLICATE KEY UPDATE is_active = 1";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, opencon.connection))
                    {
                        updateCmd.Parameters.AddWithValue("@StartYear", startYear);
                        updateCmd.Parameters.AddWithValue("@EndYear", endYear);
                        updateCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("School year has been activated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh Dashboard if it's open
                    Dashboard openDashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();
                    if (openDashboard != null)
                    {
                        openDashboard.ActiveSY(); // Call a method to refresh the active year label
                        openDashboard.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
            }
        }

    }
}
