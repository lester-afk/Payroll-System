using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PayrollRfidScan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmDisplayInfo formInfo = new frmDisplayInfo();
            //Application.Run(formInfo);
            Application.Run(new frmDisplayRfidScan(formInfo));
        }
    }
}
