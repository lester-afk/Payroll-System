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
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();

        }

        DBconnection opencon = new DBconnection();
        myFunction mf = new myFunction();


        //Password Placeholder Function
        private void passPlaceholder()
        {

            if (lblPass.Text == "")
            {
                lblPass.Text = "PASSWORD";

                txtPassword.ForeColor = Color.FromArgb(55, 140, 231);
            }
        }

        private void passRemovePlaceholder()
        {
            if (lblPass.Text == "PASSWORD")
            {
                lblPass.Text = "";

                txtPassword.ForeColor = Color.Black;
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            passRemovePlaceholder();
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            passPlaceholder();
        }


        //=========================================
        //Username Placeholder Function
        private void userPlaceholder()
        {

            if (txtUsername.Text == "")
            {
                txtUsername.Text = "USERNAME";

                txtUsername.ForeColor = Color.FromArgb(55, 140, 231);
            }
        }

        private void userRemovePlaceholder()
        {
            if (txtUsername.Text == "USERNAME")
            {
                txtUsername.Text = "";

                txtUsername.ForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            userPlaceholder();
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            userRemovePlaceholder();
        }


        private void LogIn_Load(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        static int counter = 0;
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string pass = mf.GetEncrypt(txtPassword.Text.ToString());

            if (mf.checkUser(txtUsername.Text.ToString(), pass.ToString()))
            {
                MessageBox.Show("You have successfully logged in!", "Access Granted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Dashboard dashboard = new Dashboard();
                dashboard.Show();

                this.Close();


            }
            else
            {
                counter++;
                MessageBox.Show("Invalid username or password.", "Access Denied",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Text = "USERNAME";
                txtPassword.Text = "";
                if (counter == 3)
                {
                    MessageBox.Show("You have reached the maximum login attempt, program will now end.", "Max Attempt",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                e.Handled = true; // Prevents the default "ding" sound
                e.SuppressKeyPress = true;
            }
        }
    }
}
