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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RS.Session session = RS.Session.CreateInstance();
            if (session != null)
            {
                Application.Run(new MainForm(session));
                session.Dispose();
            }
        }
    }
}
