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
        public Timer timer;

        public bool Stop { get; set; }

        public event EventHandler<UpdateStatusEventArgs> UpdateStatus = null;
        
        public int FrameNumber{ get; set; }

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

        public void CreateTimer()
        {
            /* Timer Initialization */
            timer = new Timer();

        }


        //public void ShowPerformanceTick()
        //{
        //    /* Optional: Show performance tick */

        //    if (image != null)
        //    {
        //        timer.Tick(RS.ImageExtension.PixelFormatToString(image.Info.format) + " " + image.Info.width + "x" + image.Info.height);
        //    }
        //}


        public void InitSenseManager()
        {
            if (SenseManager.Init() == RS.Status.STATUS_NO_ERROR)
            {
                SetStatus("SenseManager Init Successfull");                      
            }
            else
            {
                SetStatus("SenseManager Init Failed");
                Stop = true;
            }
        }

        public void SetStatus(String text)
        {
            EventHandler<UpdateStatusEventArgs> handler = UpdateStatus;
            if (handler != null)
            {
                handler(this, new UpdateStatusEventArgs(text));
            }
        }

        public RS.Sample GetSample()
        {
            RS.Sample sample = null;
            /* Wait until a frame is ready: Synchronized or Asynchronous */
            if (SenseManager.AcquireFrame(false) == RS.Status.STATUS_NO_ERROR)
            {
                /* Aquire Frame from Camera */
                sample = SenseManager.Sample;
                return sample;
            }
            else return sample = null;
        }

        // Gets the maximum specified Range of the Device in mm
        public float GetDeviceRange()
        {
            RS.Device device = SenseManager.CaptureManager.Device;

            if (device != null) return device.DepthSensorRange.max;

            else return -1;
        }
    }
}
