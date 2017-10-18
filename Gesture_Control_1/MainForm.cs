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
        private int current_device_iuid = 0;

        // Layout 
        private ToolStripMenuItem[] streamMenue = new ToolStripMenuItem[RS.Capture.STREAM_LIMIT];
        private RadioButton[] streamButtons = new RadioButton[RS.Capture.STREAM_LIMIT];
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


            /* Put stream menu items to array */
            streamMenue[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_COLOR)] = colorMenu;
            streamMenue[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_DEPTH)] = depthMenu;
            streamMenue[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_IR)] = irMenu;

            /* Put stream buttons to array */
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_COLOR)] = radioColor;
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_DEPTH)] = radioDepth;
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_IR)] = radioIR;

            // register event handler 
            manager.UpdateStatus += new EventHandler<UpdateStatusEventArgs>(UpdateStatus);
            streams.RenderFrame += new EventHandler<RenderFrameEventArgs>(RenderFrame);
            FormClosing += new FormClosingEventHandler(FormClosingHandler);
            rgbImage.Paint += new PaintEventHandler(PaintHandler);


            rgbImage.Resize += new EventHandler(ResizeHandler);
            //depthImage.Resize += new EventHandler(ResizeHandler);
            //resultImage.Resize += new EventHandler(ResizeHandler);

            // Fill drop down Menues 
            ResetStreamTypes();
            PopulateDeviceMenu();
            foreach (RadioButton button in streamButtons)
                if (button != null) button.Click += new EventHandler(Stream_Click);

            // Set up Renders für WindowsForms compability
            renders[0].SetHWND(rgbImage);
            //renders[1].SetHWND(depthImage);

            // Initialise Intel Realsense Components
            manager.CreateSession();
            manager.CreateSenseManager();
            //manager.SearchDevices();
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
                PopulateColorDepthMenus(deviceMenu.DropDownItems[0] as ToolStripMenuItem);
            }
            else
            {
                buttonStart.Enabled = false;
                for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
                {
                    if (streamMenue[s] != null)
                    {
                        streamMenue[s].Visible = false;
                        streamButtons[s].Visible = false;
                    }
                }
            }
        }

        // Get entries for color Streams 
        private void PopulateColorDepthMenus(ToolStripMenuItem device_item)
        {
            RS.ImplDesc desc = new RS.ImplDesc();
            desc.group = RS.ImplGroup.IMPL_GROUP_SENSOR;
            desc.subgroup = RS.ImplSubgroup.IMPL_SUBGROUP_VIDEO_CAPTURE;
            desc.iuid = devices_iuid[device_item];
            current_device_iuid = desc.iuid;
            desc.cuids[0] = RS.Capture.CUID;

            profiles.Clear();
            foreach (ToolStripMenuItem menu in streamMenue)
            {
                if (menu != null)
                    menu.DropDownItems.Clear();
            }

            RS.Capture capture;
            RS.DeviceInfo dinfo2 = GetCheckedDevice();
            if (manager.Session.CreateImpl<RS.Capture>(desc, out capture) >= RS.Status.STATUS_NO_ERROR)
            {
                RS.Device device = capture.CreateDevice(dinfo2.didx);
                if (device != null)
                {
                    RS.StreamProfileSet streamProfileSet = new RS.StreamProfileSet();

                    for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
                    {
                        RS.StreamType streamType = RS.Capture.StreamTypeFromIndex(s);
                        if (((int)dinfo2.streams & (int)streamType) != 0 && streamMenue[s] != null)
                        {
                            streamMenue[s].Visible = true;
                            streamButtons[s].Visible = true;
                            int num = device.QueryStreamProfileSetNum(streamType);
                            for (int p = 0; p < num; p++)
                            {
                                if (device.QueryStreamProfileSet(streamType, p, out streamProfileSet) < RS.Status.STATUS_NO_ERROR) break;
                                RS.StreamProfile streamProfile = streamProfileSet[streamType];
                                ToolStripMenuItem sm1 = new ToolStripMenuItem(ProfileToString(streamProfile), null, new EventHandler(Stream_Item_Click));
                                profiles[sm1] = streamProfile;
                                streamMenue[s].DropDownItems.Add(sm1);
                            }
                        }
                        else if (((int)dinfo2.streams & (int)streamType) == 0 && streamMenue[s] != null)
                        {
                            streamMenue[s].Visible = false;
                            streamButtons[s].Visible = false;
                        }
                    }

                    device.Dispose();
                }
                capture.Dispose();
            }
            for (int i = 0; i < RS.Capture.STREAM_LIMIT; i++)
            {
                ToolStripMenuItem menu = streamMenue[i];
                if (menu != null)
                {
                    streamString[i] = new ToolStripMenuItem("None", null, new EventHandler(Stream_Item_Click));
                    profiles[streamString[i]] = new RS.StreamProfile();
                    menu.DropDownItems.Add(streamString[i]);
                    if (menu == colorMenu)
                        (menu.DropDownItems[0] as ToolStripMenuItem).Checked = true;
                    else
                        streamString[i].Checked = true;
                }
            }

            CheckSelection();
        }

        // Get names for Drop down Menu 
        private string ProfileToString(RS.StreamProfile streamProfile)
        {
            string line = "Unknown ";
            if (Enum.IsDefined(typeof(RS.PixelFormat), streamProfile.imageInfo.format))
                line = streamProfile.imageInfo.format.ToString().Substring(13) + " " + streamProfile.imageInfo.width + "x" + streamProfile.imageInfo.height + "x";
            else
                line += streamProfile.imageInfo.width + "x" + streamProfile.imageInfo.height + "x";
            if (streamProfile.frameRate.min != streamProfile.frameRate.max)
            {
                line += (float)streamProfile.frameRate.min + "-" +
                      (float)streamProfile.frameRate.max;
            }
            else
            {
                float fps = (streamProfile.frameRate.min != 0) ? streamProfile.frameRate.min : streamProfile.frameRate.max;
                line += fps;
            }
            line += StreamOptionToString(streamProfile.options);
            return line;
        }

        // Get Stream Type to String 
        private string StreamOptionToString(RS.StreamOption streamOption)
        {
            switch (streamOption)
            {
                case RS.StreamOption.STREAM_OPTION_UNRECTIFIED:
                    return " RAW";
                case (RS.StreamOption)0x20000: // Depth Confidence
                    return " + Confidence";
                case RS.StreamOption.STREAM_OPTION_DEPTH_PRECALCULATE_UVMAP:
                case RS.StreamOption.STREAM_OPTION_STRONG_STREAM_SYNC:
                case RS.StreamOption.STREAM_OPTION_ANY:
                    return "";
                default:
                    return " (" + streamOption.ToString() + ")";
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

            streams.StreamProfileSet = GetStreamSetConfiguration();
            streams.EnableStreamsFromSelection();
            streams.StreamType = GetSelectedStream();
            
            handsRecognition.ActivatedGestures = GetSelectedGestures();
            handsRecognition.SetUpHandCursorModule();
            handsRecognition.RegisterHandEvents();           
            handsRecognition.EnableGesturesFromSelection();

            manager.InitSenseManager();

            //???????????????????
            //manager.SenseManager.CaptureManager.Device.ResetProperties(RS.StreamType.STREAM_TYPE_ANY);

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
                    //frameNumber++; //todo
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

        // The StreamProfile structure describes the video stream configuration parameters.
        private RS.StreamProfile GetStreamProfileConfiguration(ToolStripMenuItem m)
        {
            foreach (ToolStripMenuItem e in m.DropDownItems)
                if (e.Checked) return profiles[e];
            return new RS.StreamProfile();
        }

        private RS.StreamProfile GetStreamConfiguration(RS.StreamType st)
        {
            ToolStripMenuItem menu = streamMenue[RS.Capture.StreamTypeToIndex(st)];
            if (menu != null)
                return GetStreamProfileConfiguration(menu);
            else
                return new RS.StreamProfile();
        }

        // The SetStreamProfileSet function configures the streams parameters. 
        // All streams must be configured. The device streaming starts after a successful configuration.
        private RS.StreamProfileSet GetStreamSetConfiguration()
        {
            RS.StreamProfileSet streamingProfile = new RS.StreamProfileSet();
            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                RS.StreamType st = RS.Capture.StreamTypeFromIndex(s);
                streamingProfile[st] = GetStreamConfiguration(st);
            }
            return streamingProfile;
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
            PopulateColorDepthMenus(sender as ToolStripMenuItem);
        }

        private void ResetStreamTypes()
        {
            streams.StreamType = RS.StreamType.STREAM_TYPE_ANY;
        }

        private void Stream_Item_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menu in streamMenue)
            {
                if (menu != null && menu.DropDownItems.Contains(sender as ToolStripMenuItem))
                {
                    foreach (ToolStripMenuItem e1 in menu.DropDownItems)
                        e1.Checked = (sender == e1);
                }
            }
            ResetStreamTypes();
            CheckSelection();
        }

        // Check if Radio Buttons and Menu Selection fit 
        private void CheckSelection()
        {
            int sumEnabled = 0;
            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                if (streamButtons[s] != null && streamString[s] != null)
                {
                    streamButtons[s].Enabled = !streamString[s].Checked;
                    sumEnabled += streamButtons[s].Enabled ? 1 : 0;
                }
            }



            RS.StreamType selectedStream = GetSelectedStream();
            if (selectedStream != RS.StreamType.STREAM_TYPE_ANY && !streamButtons[RS.Capture.StreamTypeToIndex(selectedStream)].Enabled)
            {
                RS.StreamType st = GetUnselectedStream();
                streamButtons[RS.Capture.StreamTypeToIndex(st)].Checked = true;
                streams.StreamType = st;
            }

        }

        private RS.StreamType GetSelectedStream()
        {
            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                if (streamButtons[s] != null && streamButtons[s].Checked)
                    return RS.Capture.StreamTypeFromIndex(s);
            }
            return RS.StreamType.STREAM_TYPE_ANY;
        }

        private RS.StreamType GetUnselectedStream()
        {
            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                if (streamButtons[s] != null && !streamButtons[s].Checked && streamButtons[s].Enabled)
                    return RS.Capture.StreamTypeFromIndex(s);
            }
            return RS.StreamType.STREAM_TYPE_ANY;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            manager.Stop = true;
        }

        private void Stream_Click(object sender, EventArgs e)
        {
            RS.StreamType selected_stream = GetSelectedStream();
            if (selected_stream != streams.StreamType)
            {
                streams.StreamType = selected_stream;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

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

        //private delegate void UpdatePanelDelegate();
        //public void UpdateMessageBox()
        //{

        //    messageBox.Invoke(new UpdatePanelDelegate(delegate ()
        //    {
        //        messageBox.Invalidate();
        //    }));

        //}

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

    }
}
