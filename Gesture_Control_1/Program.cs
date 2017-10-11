using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RS = Intel.RealSense;

namespace streams.cs
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Manager manager = new Manager();

                manager.CreateSession();
                manager.CreateSenseManager();
               
                
                if (manager.SenseManager != null && manager.Session !=null)
                {
                    Application.Run(new MainForm(manager));
                    manager.Session.Dispose();
                    manager.SenseManager.Close();
                    manager.SenseManager.Dispose();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
