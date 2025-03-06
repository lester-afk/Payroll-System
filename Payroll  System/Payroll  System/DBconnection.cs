using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Payroll__System
{
    class DBconnection
    {
        public MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string port;

        public void dbconnect()
        {
            // Initialize connection parameters
            server = "192.168.8.123"; // or your MySQL server address
            port = "3306";        // default MySQL port
            database = "payrollDB";
            uid = "payroll";         // your MySQL username
            password = "payroll";        // your MySQL password

            string connectionString = $"SERVER={server};PORT={port};DATABASE={database};UID={uid};PASSWORD={password};";

            connection = new MySqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                // Handle specific MySQL errors
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact the administrator.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        MessageBox.Show($"Database error {ex.Number}: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error closing connection: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
