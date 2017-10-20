using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RS = Intel.RealSense;
using SampleDX; // Redering for bitmap
using Intel.RealSense.HandCursor;



namespace streams.cs
{
    public partial class MainForm : Form
    {
        //Global Var
        private Manager manager;
        private Streams streams;
        private HandsRecognition handsRecognition;

        private volatile bool closing = false; 

        // Layout 
        private ToolStripMenuItem[] streamMenue = new ToolStripMenuItem[RS.Capture.STREAM_LIMIT];
        //private RadioButton[] streamButtons = new RadioButton[RS.Capture.STREAM_LIMIT];
        public Dictionary<ToolStripMenuItem, RS.DeviceInfo> devices = new Dictionary<ToolStripMenuItem, RS.DeviceInfo>();
        private Dictionary<ToolStripMenuItem, RS.StreamProfile> profiles = new Dictionary<ToolStripMenuItem, RS.StreamProfile>();
        private Dictionary<ToolStripMenuItem, int> devices_iuid = new Dictionary<ToolStripMenuItem, int>();
        private ToolStripMenuItem[] streamString = new ToolStripMenuItem[RS.Capture.STREAM_LIMIT];

        // Rendering
        private D2D1Render[] renders = new D2D1Render[2] { new D2D1Render(), new D2D1Render() }; // reder for .NET PictureBox

        // Drawing Parameters 
        private Bitmap resultBitmap = null;
        private float penSize = 3.0f;

        // 
        public MainForm(Manager mngr)
        {
            InitializeComponent();
            manager = mngr;
            streams = new Streams(manager);
            handsRecognition = new HandsRecognition(manager, this);
            
            // register event handler 
            manager.UpdateStatus += new EventHandler<UpdateStatusEventArgs>(UpdateStatus);
            streams.RenderFrame += new EventHandler<RenderFrameEventArgs>(RenderFrame);
            FormClosing += new FormClosingEventHandler(FormClosingHandler);

            rgbImage.Paint += new PaintEventHandler(PaintHandler);
            depthImage.Paint += new PaintEventHandler(PaintHandler);
            resultImage.Paint += new PaintEventHandler(ResultPanel_Paint);

            rgbImage.Resize += new EventHandler(ResizeHandler);
            depthImage.Resize += new EventHandler(ResizeHandler);

            radioDepth.Click += new EventHandler(StreamButton_Click);
            radioIR.Click += new EventHandler(StreamButton_Click);


            // Fill drop down Menues 
            streams.ResetStreamTypes();
            PopulateDeviceMenu();
         
            // Set up Renders für WindowsForms compability
            renders[0].SetHWND(rgbImage);
            renders[1].SetHWND(depthImage);
            
            // Initialise Intel Realsense Components
            manager.CreateSession();
            manager.CreateSenseManager();           
            manager.CreateTimer();
        }

        // Get entries for Device Menue 
        private void PopulateDeviceMenu()
        {
            devices.Clear();
            devices_iuid.Clear();

            RS.ImplDesc desc = new RS.ImplDesc();
            desc.group = RS.ImplGroup.IMPL_GROUP_SENSOR;
            desc.subgroup = RS.ImplSubgroup.IMPL_SUBGROUP_VIDEO_CAPTURE;

            deviceMenu.DropDownItems.Clear();

            for (int i = 0; ; i++)
            {

                RS.ImplDesc desc1 = manager.Session.QueryImpl(desc, i);
                if (desc1 == null)
                    break;
                RS.Capture capture;
                if (manager.Session.CreateImpl<RS.Capture>(desc1, out capture) < RS.Status.STATUS_NO_ERROR) continue;
                for (int j = 0; ; j++)
                {
                    RS.DeviceInfo dinfo;
                    if (capture.QueryDeviceInfo(j, out dinfo) < RS.Status.STATUS_NO_ERROR) break;

                    ToolStripMenuItem sm1 = new ToolStripMenuItem(dinfo.name, null, new EventHandler(Device_Item_Click));
                    devices[sm1] = dinfo;
                    devices_iuid[sm1] = desc1.iuid;
                    deviceMenu.DropDownItems.Add(sm1);
                }
                capture.Dispose();
            }
            if (deviceMenu.DropDownItems.Count > 0)
            {
                (deviceMenu.DropDownItems[0] as ToolStripMenuItem).Checked = true;
                
            }
            else
            {
                buttonStart.Enabled = false;
                radioDepth.Visible = false;
                radioIR.Visible = false;
                for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
                {
                    if (streamMenue[s] != null)
                    {
                        streamMenue[s].Visible = false;

                    }
                }
            }
        }
        

        // Start of Program 
        private void buttonStart_Click(object sender, EventArgs e)
        {
            // Configure UI
            menuStrip.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            ActivateGestureCheckboxes(false);

            // Reset all components
            manager.DeviceInfo = null;
            manager.Stop = false;

            manager.DeviceInfo = GetCheckedDevice();

            streams.ConfigureStreams();

            handsRecognition.ActivatedGestures = GetSelectedGestures();
            handsRecognition.SetUpHandCursorModule();
            handsRecognition.RegisterHandEvents();
            handsRecognition.EnableGesturesFromSelection();

            manager.InitSenseManager();
            
            // Thread for Streaming 
            System.Threading.Thread thread1 = new System.Threading.Thread(DoWork);
            thread1.Start();
            System.Threading.Thread.Sleep(5);
        }

        // Worker for threads 
        delegate void DoWorkEnd();
        private void DoWork()
        {
            try
            {
                while (!manager.Stop)
                {
                    RS.Sample sample = manager.GetSample();

                    streams.RenderStreams(sample);
                    //manager.ShowPerformanceTick();
                    handsRecognition.RecogniseHands(sample); //Todo
                    manager.SenseManager.ReleaseFrame();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.ToString(), "Error while Recognition", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Invoke(new DoWorkEnd(
                delegate
                {
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                    menuStrip.Enabled = true;
                    ActivateGestureCheckboxes(true);
                    manager.SenseManager.Close();
                    if (closing) Close();
                }
            ));
        }
        
        public RS.DeviceInfo GetCheckedDevice()
        {
            foreach (ToolStripMenuItem e in deviceMenu.DropDownItems)
            {
                if (devices.ContainsKey(e))
                {
                    if (e.Checked) return devices[e];
                }
            }
            return new RS.DeviceInfo();
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        // Eventhandler Methods
        private void RenderFrame(Object sender, RenderFrameEventArgs e)
        {
            if (e.image == null) return;
            renders[e.index].UpdatePanel(e.image);
        }

        /* Redirect to DirectX Update */
        private void PaintHandler(object sender, PaintEventArgs e)
        {
            renders[(sender == rgbImage) ? 0 : 1].UpdatePanel();
        }

        /* Redirect to DirectX Resize */
        private void ResizeHandler(object sender, EventArgs e)
        {
            renders[(sender == rgbImage) ? 0 : 1].ResizePanel();
        }

        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            manager.Stop = true;
            e.Cancel = buttonStop.Enabled;
            closing = true;
        }

        private void SetStatus(String text)
        {
            statusStripLabel.Text = text;
        }

        private delegate void SetStatusDelegate(String status);
        private void UpdateStatus(Object sender, UpdateStatusEventArgs e)
        {
            // Elemente im Hauptfenster müssen über MainThread bearbeitet werden 
            // Über Invoke, wird aktion vom Hauptthread gestartet
            statusStrip.Invoke(new SetStatusDelegate(SetStatus), new object[] { e.text });
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Device_Item_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem e1 in deviceMenu.DropDownItems)
                e1.Checked = (sender == e1);
           
        }
        
        private RS.StreamType GetSelectedStream()
        {
            if (radioDepth.Checked)
                return RS.StreamType.STREAM_TYPE_DEPTH;

            else if (radioIR.Checked)
                return RS.StreamType.STREAM_TYPE_IR;

            else return RS.StreamType.STREAM_TYPE_ANY;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            manager.Stop = true;
        }

        private void StreamButton_Click(object sender, EventArgs e)
        {
            RS.StreamType selected_stream = GetSelectedStream();
            if (selected_stream != streams.StreamType)
            {
                streams.StreamType = selected_stream;
            }
        }
        
        /*
         * Hands Rcognition Stuff
        */

        // Update Message Box with recognized Gestures 
        private delegate void UpdateGestureInfoEventHandler(string status, Color color);
        public void UpdateGestureInfo(string status, Color color)
        {
            messageBox.Invoke(new UpdateGestureInfoEventHandler(delegate (string s, Color c)
            {
                if (status == String.Empty)
                {
                    messageBox.Text = String.Empty;
                    return;
                }

                if (messageBox.TextLength > 1200)
                {
                    messageBox.Text = String.Empty;
                }

                messageBox.SelectionColor = c;

                messageBox.SelectedText = s;
                messageBox.SelectionColor = messageBox.ForeColor;

                messageBox.SelectionStart = messageBox.Text.Length;
                messageBox.ScrollToCaret();

            }), new object[] { status, color });
        }

        public void DisplayBitmap(Bitmap picture)
        {
            lock (this)
            {
                if (resultBitmap != null)
                    resultBitmap.Dispose();
                resultBitmap = new Bitmap(picture);
            }
        }

        public void DisplayCursor(int numOfHands, Queue<RS.Point3DF32>[] cursorPoints, int[] cursorClick, BodySideType[] handSideType)
        {
            if (resultBitmap == null) return;

            int scaleFactor = 1;
            Graphics g = Graphics.FromImage(resultBitmap);

            Color color = Color.GreenYellow;
            Pen pen = new Pen(color, penSize);

            for (int i = 0; i < numOfHands; ++i)
            {
                float sz = 8;
                int blueColor = (handSideType[i] == BodySideType.BODY_SIDE_LEFT)
                    ? 200
                    : (handSideType[i] == BodySideType.BODY_SIDE_RIGHT) ? 100 : 0;

                /// draw cursor trail

                for (int j = 0; j < cursorPoints[i].Count; j++)
                {
                    float greenPart = (float)((Math.Max(Math.Min(cursorPoints[i].ElementAt(j).z / scaleFactor, 0.7), 0.2) - 0.2) / 0.5);

                    pen.Color = Color.FromArgb(255, (int)(255 * (1 - greenPart)), (int)(255 * greenPart), blueColor);
                    pen.Width = penSize;
                    int x = (int)cursorPoints[i].ElementAt(j).x / scaleFactor;
                    int y = (int)cursorPoints[i].ElementAt(j).y / scaleFactor;
                    g.DrawEllipse(pen, x - sz / 2, y - sz / 2, sz, sz);
                }


                if (0 < cursorClick[i])
                {
                    color = Color.LightBlue;
                    pen = new Pen(color, 10.0f);
                    sz = 32;

                    int x = 0, y = 0;
                    if (cursorPoints[i].Count() > 0)
                    {
                        x = (int)cursorPoints[i].ElementAt(cursorPoints[i].Count - 1).x / scaleFactor;
                        y = (int)cursorPoints[i].ElementAt(cursorPoints[i].Count - 1).y / scaleFactor;
                    }

                    g.DrawEllipse(pen, x - sz / 2, y - sz / 2, sz, sz);
                }
            }
            pen.Dispose();
        }

        private void ResultPanel_Paint(object sender, PaintEventArgs e)
        {
            lock (this)
            {
                if (resultBitmap == null || resultBitmap.Width == 0 || resultBitmap.Height == 0) return;
                Bitmap bitmapNew = new Bitmap(resultBitmap);
                try
                {

                    /* Keep the aspect ratio */
                    Rectangle rc = (sender as PictureBox).ClientRectangle;
                    float xscale = (float)rc.Width / (float)resultBitmap.Width;
                    float yscale = (float)rc.Height / (float)resultBitmap.Height;
                    float xyscale = (xscale < yscale) ? xscale : yscale;
                    int width = (int)(resultBitmap.Width * xyscale);
                    int height = (int)(resultBitmap.Height * xyscale);
                    rc.X = (rc.Width - width) / 2;
                    rc.Y = (rc.Height - height) / 2;
                    rc.Width = width;
                    rc.Height = height;
                    e.Graphics.DrawImage(bitmapNew, rc);
                }
                finally
                {
                    bitmapNew.Dispose();
                }
            }
        }

        public List<string> GetSelectedGestures()
        {
            List<string> activatedGestures = new List<string>();

            foreach (CheckBox checkBox in gestureCheckBoxTable.Controls)
            {
                if (checkBox.Checked)
                {
                    activatedGestures.Add(checkBox.Name);
                }
            }

            return activatedGestures;
        }

        private void ActivateGestureCheckboxes(bool enabled)
        {
            click.Enabled = enabled;
            handOpen.Enabled = enabled;
            handClose.Enabled = enabled;
            fist.Enabled = false; // Not Configured jet 
        }

        private delegate void UpdateResultImageDelegate();
        public void UpdateResultImage()
        {

            resultImage.Invoke(new UpdateResultImageDelegate(delegate ()
            {
                resultImage.Invalidate();
            }));

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
