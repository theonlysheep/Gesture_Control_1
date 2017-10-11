using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RS = Intel.RealSense;
using System.Windows.Forms;

namespace streams.cs
{
    public class Manager
    {
        // Global Variables 
        public RS.Session Session { get; set; }
        public RS.SenseManager SenseManager { get; set; }
        public RS.DeviceInfo DeviceInfo { get; set; }

        public bool Stop { get; set; }

        /*
         * Manage Session and SenseManager in central class
        */
        public void CreateSession()
        {
            try
            {
                Session = RS.Session.CreateInstance();

            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.ToString(), "Can not create RealSense session ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DisposeSession()
        {
            try
            {
                if (Session != null)
                {
                    Session.Dispose();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.ToString(), "Can not dispose session", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Create an instance of the SenseManager interface */
        public void CreateSenseManager()
        {

            try
            {
                SenseManager = RS.SenseManager.CreateInstance();

            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.ToString(), "Can not create SenseManager. Failed to create an SDK pipeline object.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DisposeSenseManager()
        {
            try
            {
                if (SenseManager != null)
                {
                    SenseManager.Dispose();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.ToString(), "Can not dispose SenseManager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
         * Mange Devices in central class
         */
        public void HandleDevices()
        {
            /* Optional: Set Input Source */
            if (DeviceInfo != null)
                SenseManager.CaptureManager.FilterByDeviceInfo(DeviceInfo);
        }



    }
}
