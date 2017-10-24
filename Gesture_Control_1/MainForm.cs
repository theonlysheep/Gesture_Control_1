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

        private class Item
        {
            public string Name;
            public int Value;
            public Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }

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


            // Reset all components
            manager.DeviceInfo = null;
            manager.Stop = false;

            manager.DeviceInfo = GetCheckedDevice();

            streams.ConfigureStreams();

            //handsRecognition.ActivatedGestures = GetSelectedGestures();
            handsRecognition.SetUpHandModule();
            //handsRecognition.EnableGesturesFromSelection();

            PopulateGestureList();

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
                    manager.FrameNumber++;
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
                    if (manager.Stop == true)
                    {
                        manager.SetStatus("Stopped");
                    }
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

            foreach (object itemChecked in gestureListBox.CheckedItems)
            {
                    activatedGestures.Add(itemChecked.ToString());
    
            }

            return activatedGestures;
        }

        private delegate void UpdateGesturesToListDelegate(string gestureName, int index);
        public void UpdateGesturesToList(string gestureName, int index)
        {
            gestureListBox.Invoke(new UpdateGesturesToListDelegate(delegate (string name, int cmbIndex)
            {
                gestureListBox.Items.Add(new Item(name, cmbIndex));
            }), new object[] { gestureName, index });
        }

        private delegate void ResetGesturesListDelegate();
        public void ResetGesturesList()
        {
            gestureListBox.Invoke(new ResetGesturesListDelegate(delegate ()
                {
                    gestureListBox.Text = "";
                    gestureListBox.Items.Clear();
                    gestureListBox.SelectedIndex = -1;
                    gestureListBox.Enabled = false;
                    gestureListBox.Size = new System.Drawing.Size(100, 20); // ?????????????????
                }), new object[] { });
        }

        private delegate void UpdateGesturesListSizeDelegate();
        public void UpdateGesturesListSize()
        {
            gestureListBox.Invoke(new UpdateGesturesListSizeDelegate(delegate ()
            {
                gestureListBox.Enabled = true;
                gestureListBox.Size = new System.Drawing.Size(200, 150);
                gestureListBox.Visible = true;
            }), new object[] { });

        }

        private delegate void UpdateResultImageDelegate();
        public void UpdateResultImage()
        {

            resultImage.Invoke(new UpdateResultImageDelegate(delegate ()
            {
                resultImage.Invalidate();
            }));

        }

        private void PopulateGestureList()
        {
            if (handsRecognition.HandConfiguration != null)
            {
                // Publish List of Gesture for Selection
                ResetGesturesList();
                int totalNumOfGestures = handsRecognition.HandConfiguration.NumberOfGestures;

                if (totalNumOfGestures > 0)
                {

                    for (int i = 0; i < totalNumOfGestures; i++)
                    {
                        string gestureNameFromIndex = string.Empty;
                        if (handsRecognition.HandConfiguration.QueryGestureNameByIndex(i, out gestureNameFromIndex) ==
                            RS.Status.STATUS_NO_ERROR)
                        {
                            UpdateGesturesToList(gestureNameFromIndex, i);
                        }
                    }

                    UpdateGesturesListSize();
                }
            }
        }

        public void DisplayJoints(RS.Hand.JointData[][] nodes, int numOfHands)
        {
            if (resultBitmap == null) return;
            if (nodes == null) return;


            lock (this)
            {
                int scaleFactor = 1;

                Graphics g = Graphics.FromImage(resultBitmap);

                using (Pen boneColor = new Pen(Color.DodgerBlue, 3.0f))
                {
                    for (int i = 0; i < numOfHands; i++)
                    {
                        if (nodes[i][0] == null) continue;
                        int baseX = (int)nodes[i][0].positionImage.x / scaleFactor;
                        int baseY = (int)nodes[i][0].positionImage.y / scaleFactor;

                        int wristX = (int)nodes[i][0].positionImage.x / scaleFactor;
                        int wristY = (int)nodes[i][0].positionImage.y / scaleFactor;

                        // Display Skeleton
                        for (int j = 1; j < 22; j++)
                        {
                            if (nodes[i][j] == null) continue;
                            int x = (int)nodes[i][j].positionImage.x / scaleFactor;
                            int y = (int)nodes[i][j].positionImage.y / scaleFactor;

                            if (nodes[i][j].confidence <= 0) continue;

                            if (j == 2 || j == 6 || j == 10 || j == 14 || j == 18)
                            {

                                baseX = wristX;
                                baseY = wristY;
                            }

                            g.DrawLine(boneColor, new Point(baseX, baseY), new Point(x, y));
                            baseX = x;
                            baseY = y;
                        }


                        // Display Joints 
                        using (
                            Pen red = new Pen(Color.Red, 3.0f),
                                black = new Pen(Color.Black, 3.0f),
                                green = new Pen(Color.Green, 3.0f),
                                blue = new Pen(Color.Blue, 3.0f),
                                cyan = new Pen(Color.Cyan, 3.0f),
                                yellow = new Pen(Color.Yellow, 3.0f),
                                orange = new Pen(Color.Orange, 3.0f))
                        {
                            Pen currnetPen = black;

                            for (int j = 0; j < RS.Hand.HandData.NUMBER_OF_JOINTS; j++)
                            {
                                float sz = 4;

                                int x = (int)nodes[i][j].positionImage.x / scaleFactor;
                                int y = (int)nodes[i][j].positionImage.y / scaleFactor;

                                if (nodes[i][j].confidence <= 0) continue;

                                //Wrist
                                if (j == 0)
                                {
                                    currnetPen = black;
                                }

                                //Center
                                if (j == 1)
                                {
                                    currnetPen = red;
                                    sz += 4;
                                }

                                //Thumb
                                if (j == 2 || j == 3 || j == 4 || j == 5)
                                {
                                    currnetPen = green;
                                }
                                //Index Finger
                                if (j == 6 || j == 7 || j == 8 || j == 9)
                                {
                                    currnetPen = blue;
                                }
                                //Finger
                                if (j == 10 || j == 11 || j == 12 || j == 13)
                                {
                                    currnetPen = yellow;
                                }
                                //Ring Finger
                                if (j == 14 || j == 15 || j == 16 || j == 17)
                                {
                                    currnetPen = cyan;
                                }
                                //Pinkey
                                if (j == 18 || j == 19 || j == 20 || j == 21)
                                {
                                    currnetPen = orange;
                                }


                                if (j == 5 || j == 9 || j == 13 || j == 17 || j == 21)
                                {
                                    sz += 4;
                                }

                                g.DrawEllipse(currnetPen, x - sz / 2, y - sz / 2, sz, sz);
                            }
                        }
                    }

                }
                g.Dispose();
            }

        }

        public void DisplayExtremities(int numOfHands, RS.Hand.ExtremityData[][] extremitites = null)
        {
            if (resultBitmap == null) return;
            if (extremitites == null) return;

            int scaleFactor = 1;
            Graphics g = Graphics.FromImage(resultBitmap);

            float sz = 8;

            Pen pen = new Pen(Color.Red, 3.0f);
            for (int i = 0; i < numOfHands; ++i)
            {
                for (int j = 0; j < RS.Hand.HandData.NUMBER_OF_EXTREMITIES; ++j)
                {
                    int x = (int)extremitites[i][j].pointImage.x / scaleFactor;
                    int y = (int)extremitites[i][j].pointImage.y / scaleFactor;
                    g.DrawEllipse(pen, x - sz / 2, y - sz / 2, sz, sz);
                }
            }
            pen.Dispose();
        }
        
        private void gestureListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Workaround to include currently selsected item into condsidderation 
            this.BeginInvoke(new Action(() =>
            {
                handsRecognition.EnableGesturesFromSelection();
            }));
            
        }
    }
}
