using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Payroll__System
{
    public partial class frmSemester : Form
    {
        public frmSemester(Dashboard parent)
        {
            InitializeComponent();
            formDashboard = parent;
        }

        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();
        Dashboard formDashboard;

        private void LoadSem()
        {
            opencon.dbconnect();

            if (opencon.OpenConnection())
            {
                try
                {
                    string query = @"SELECT sem_active_semester AS 'Semester', isActive FROM semester";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, opencon.connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // ✅ Create a new string column for "Active"/"Inactive" status
                    dt.Columns.Add("Status", typeof(string));

                    // ✅ Populate the new column based on isActive values
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Status"] = row["isActive"].ToString() == "1" ? "Active" : "Inactive";
                    }

                    // ✅ Remove the original isActive column
                    dt.Columns.Remove("isActive");

                    dataGridViewSemester.DataSource = dt;

                    opencon.CloseConnection();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while loading data:\n{ex.Message}",
                                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        string semester;
        int isActive;
        private void dataGridViewSemester_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = this.dataGridViewSemester.Rows[e.RowIndex];
                semester = selectedRow.Cells["Semester"].Value.ToString();

               
                isActive = selectedRow.Cells["Status"].Value.ToString() == "Active" ? 1 : 0;
            }
        }

        private void frmSemester_Load(object sender, EventArgs e)
        {
            LoadSem();
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
                MessageBox.Show("Select Semester to activate.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(semester))
            {
                MessageBox.Show("Please select a semester first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Toggle the active state (swap 0 and 1)
            int newActiveState = isActive == 1 ? 0 : 1;

            opencon.dbconnect();
            if (opencon.OpenConnection())
            {
                // Set all to 0 first (ensures only one active semester at a time)
                string resetQuery = "UPDATE semester SET isActive = 0";
                string updateQuery = "UPDATE semester SET isActive = @Active WHERE sem_active_semester = @Semester";

                try
                {
                    using (MySqlCommand resetCmd = new MySqlCommand(resetQuery, opencon.connection))
                    {
                        resetCmd.ExecuteNonQuery();
                    }

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, opencon.connection))
                    {
                        updateCmd.Parameters.AddWithValue("@Semester", semester);
                        updateCmd.Parameters.AddWithValue("@Active", newActiveState);
                        updateCmd.ExecuteNonQuery();
                    }
                    Dashboard openDashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();
                    if (openDashboard != null)
                    {
                        openDashboard.ActiveSemester(); // Call ActiveSY() on the open instance
                        openDashboard.Refresh();
                    }
                    MessageBox.Show("Semester has been activated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    opencon.CloseConnection();
                }
               
                
                LoadSem();
            }
        }

        private void dataGridViewSemester_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = this.dataGridViewSemester.Rows[e.RowIndex];
                semester = selectedRow.Cells["Semester"].Value.ToString();


                isActive = selectedRow.Cells["Status"].Value.ToString() == "Active" ? 1 : 0;
            }
        }
    }
}
