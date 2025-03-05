using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payroll__System
{
    class myFunction
    {
        private int maxnum;
        DBconnection con = new DBconnection();
        public int getMaxNumber()
        {
            

            con.dbconnect();
            if (con.OpenConnection() == true)
            {
                string sql = "SELECT COALESCE(MAX(e_num),0) AS MAXNUM FROM employee";


                MySqlCommand da = new MySqlCommand(sql, con.connection);
                MySqlDataReader readinfo;
                readinfo = da.ExecuteReader();
                da.CommandType = System.Data.CommandType.Text;

                if (readinfo.HasRows)
                {
                    readinfo.Read();
                    maxnum = Convert.ToInt32(readinfo["MAXNUM"]);//2


                    readinfo.Close();

                    con.CloseConnection();
                }
            }
            return maxnum;
        }

        public int getMaxCA()
        {


            con.dbconnect();
            if (con.OpenConnection() == true)
            {
                string sql = "SELECT COALESCE(MAX(ca_num),0) AS MAXNUM FROM cash_advance";


                MySqlCommand da = new MySqlCommand(sql, con.connection);
                MySqlDataReader readinfo;
                readinfo = da.ExecuteReader();
                da.CommandType = System.Data.CommandType.Text;

                if (readinfo.HasRows)
                {
                    readinfo.Read();
                    maxnum = Convert.ToInt32(readinfo["MAXNUM"]);//2


                    readinfo.Close();

                    con.CloseConnection();
                }
            }
            return maxnum;
        }

        public int getMaxPR()
        {


            con.dbconnect();
            if (con.OpenConnection() == true)
            {
                string sql = "SELECT COALESCE(MAX(pr_num),0) AS MAXNUM FROM payroll";


                MySqlCommand da = new MySqlCommand(sql, con.connection);
                MySqlDataReader readinfo;
                readinfo = da.ExecuteReader();
                da.CommandType = System.Data.CommandType.Text;

                if (readinfo.HasRows)
                {
                    readinfo.Read();
                    maxnum = Convert.ToInt32(readinfo["MAXNUM"]);//2


                    readinfo.Close();

                    con.CloseConnection();
                }
            }
            return maxnum;
        }

        private bool validtxt;

        public string GetEncrypt(string pass) //pass = sonny
        {
            MD5CryptoServiceProvider cryptedMD5 = new MD5CryptoServiceProvider();
            cryptedMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pass));
            byte[] result = cryptedMD5.Hash;
            StringBuilder str = new StringBuilder();
            for (int i = 1; i < result.Length; i++)
            {
                str.Append(result[i].ToString("x2"));
            }
            return str.ToString();//f83f724f242bbba2b12f85f6091d1f

        }
        public bool checkUser(string uname, string pass) //uname=sonny pass=f83f724f242bbba2b12f85f6091d1f
        {
            con.dbconnect();
            //true
            if (con.OpenConnection() == true)
            {
                string sql = "SELECT * FROM account WHERE (username=" + "'"
                            + uname + "' AND   password=" + "'" + pass + "') LIMIT 1";
                //MySqlDataAdapter is an intermediary between the DataSet and the data source.
                // Dataset represents a complete set of data including the tables
                MySqlDataAdapter da = new MySqlDataAdapter(sql, con.connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    validtxt = false;
                }
                else
                {
                    validtxt = true;
                }

            }
            con.CloseConnection();
            return validtxt;
        }

        public bool addUser(string uname, string pass)
        {
            con.dbconnect();

            if (con.OpenConnection())
            {
                string sql = "INSERT INTO account (username, password) VALUES (@username, @password)";
                MySqlCommand cmd = new MySqlCommand(sql, con.connection);
                cmd.Parameters.AddWithValue("@username", uname);
                cmd.Parameters.AddWithValue("@password", pass);
               
                try
                {
                    cmd.ExecuteNonQuery();
                    con.CloseConnection();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    con.CloseConnection();
                    return false;
                }
            }
            return false;
        }

        public void txtLetters(object sender, KeyPressEventArgs e)
        {
            // Allow only letters and control keys (like Backspace)
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Suppress the character input
            }
        }

        public void txtNumber(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers (0-9) and control keys (like Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Suppress the character input
            }
        }
    }
}
