using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payroll__System
{
    public partial class frmRfidScan : Form
    {
        DBconnection opencon = new DBconnection();

        private frmEmployee formEmployee;
        public frmRfidScan(frmEmployee formEmp)
        {
            InitializeComponent();
            maxLengthTimer = new Timer();
            maxLengthTimer.Interval = 500; // Set timer interval (2000ms = 2 seconds)
            maxLengthTimer.Tick += MaxLengthTimer_Tick;
            formEmployee = formEmp;
        }

        private Timer maxLengthTimer;
        private string lastInput = "";

        private void frmRfidScan_Load(object sender, EventArgs e)
        {
            txtRfid.Focus();
            
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
                MessageBox.Show("Please Scan RFID Card.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRfid.Text = "";
            txtRfid.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string textData = txtRfid.Text;
            
            if (formEmployee != null && !formEmployee.IsDisposed)
            {
                formEmployee.FillRfid(textData);
            }

        }

        private void txtRfid_TextChanged(object sender, EventArgs e)
        {
            txtRfid.Focus();
            txtDisRfid.Text = txtRfid.Text;
            // Stop and restart the timer each time the text changes
            maxLengthTimer.Stop();
            maxLengthTimer.Start();

            // Store the current input
            lastInput = txtRfid.Text;

            //string textData = txtRfid.Text;
            
            if (formEmployee != null && !formEmployee.IsDisposed)
            {
                formEmployee.FillRfid(lastInput);
                
            }
            

        }

        private void MaxLengthTimer_Tick(object sender, EventArgs e)
        {
            maxLengthTimer.Stop();

            // Adjust the MaxLength based on the length of the previous input
            if (!string.IsNullOrEmpty(lastInput))
            {
                txtRfid.MaxLength = lastInput.Length;
                this.Close();
            }
            else if(txtRfid.Text == "")
            {
                // Set a default MaxLength if the input is empty
                txtRfid.MaxLength = 16; // Example default length
                txtRfid.Focus();
            }

            // Optionally, show the updated MaxLength for debugging
            Console.WriteLine($"MaxLength adjusted to: {txtRfid.MaxLength}");
        }

    }
}
