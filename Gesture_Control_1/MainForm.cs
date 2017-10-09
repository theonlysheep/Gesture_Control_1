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
        private RS.Session session;
        
        private volatile bool closing = false;
        private int current_device_iuid = 0;

        // Layout 
        private ToolStripMenuItem[] streamMenus = new ToolStripMenuItem[RS.Capture.STREAM_LIMIT];
        private RadioButton[] streamButtons = new RadioButton[RS.Capture.STREAM_LIMIT];
        private Dictionary<ToolStripMenuItem, RS.DeviceInfo> devices = new Dictionary<ToolStripMenuItem, RS.DeviceInfo>();
        private Dictionary<ToolStripMenuItem, RS.StreamProfile> profiles = new Dictionary<ToolStripMenuItem, RS.StreamProfile>();
        private Dictionary<ToolStripMenuItem, int> devices_iuid = new Dictionary<ToolStripMenuItem, int>();
        private ToolStripMenuItem[] streamString = new ToolStripMenuItem[RS.Capture.STREAM_LIMIT];

        // Rendering
        private D2D1Render[] renders = new D2D1Render[2] { new D2D1Render(), new D2D1Render() }; // reder for .NET PictureBox
        private RenderStreams renderStreams = new RenderStreams();

        public MainForm(RS.Session session)
        {
            InitializeComponent();

            this.session = session;

            /* Put stream menu items to array */
            streamMenus[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_COLOR)] = colorMenu;
            streamMenus[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_DEPTH)] = depthMenu;
            streamMenus[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_IR)] = irMenu;
           
            /* Put stream buttons to array */
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_COLOR)] = radioColor;
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_DEPTH)] = radioDepth;
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_IR)] = radioIR;

            // register event handler 
            renderStreams.UpdateStatus += new EventHandler<UpdateStatusEventArgs>(UpdateStatusHandler);
            renderStreams.RenderFrame += new EventHandler<RenderFrameEventArgs>(RenderFrameHandler);
            FormClosing += new FormClosingEventHandler(FormClosingHandler);
            rgbImage.Paint += new PaintEventHandler(PaintHandler);

            rgbImage.Resize += new EventHandler(ResizeHandler);
            depthImage.Resize += new EventHandler(ResizeHandler);
            resultImage.Resize += new EventHandler(ResizeHandler);

            ResetStreamTypes();

            // Fill drop down Menues 
            PopulateDeviceMenu();
            foreach (RadioButton button in streamButtons)
                if (button != null) button.Click += new EventHandler(Stream_Click);

            renders[0].SetHWND(rgbImage);

        }

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
                RS.ImplDesc desc1 = session.QueryImpl(desc, i);
                if (desc1 == null)
                    break;
                RS.Capture capture;
                if (session.CreateImpl<RS.Capture>(desc1, out capture) < RS.Status.STATUS_NO_ERROR) continue;
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
                    if (streamMenus[s] != null)
                    {
                        streamMenus[s].Visible = false;
                        streamButtons[s].Visible = false;
                    }
                }
            }
        }

        private void PopulateColorDepthMenus(ToolStripMenuItem device_item)
        {
            RS.ImplDesc desc = new RS.ImplDesc();
            desc.group = RS.ImplGroup.IMPL_GROUP_SENSOR;
            desc.subgroup = RS.ImplSubgroup.IMPL_SUBGROUP_VIDEO_CAPTURE;
            desc.iuid = devices_iuid[device_item];
            current_device_iuid = desc.iuid;
            desc.cuids[0] = RS.Capture.CUID;

            profiles.Clear();
            foreach (ToolStripMenuItem menu in streamMenus)
            {
                if (menu != null)
                    menu.DropDownItems.Clear();
            }

            RS.Capture capture;
            RS.DeviceInfo dinfo2 = GetCheckedDevice();
            if (session.CreateImpl<RS.Capture>(desc, out capture) >= RS.Status.STATUS_NO_ERROR)
            {
                RS.Device device = capture.CreateDevice(dinfo2.didx);
                if (device != null)
                {
                    RS.StreamProfileSet streamProfileSet = new RS.StreamProfileSet();

                    for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
                    {
                        RS.StreamType streamType = RS.Capture.StreamTypeFromIndex(s);
                        if (((int)dinfo2.streams & (int)streamType) != 0 && streamMenus[s] != null)
                        {
                            streamMenus[s].Visible = true;
                            streamButtons[s].Visible = true;
                            int num = device.QueryStreamProfileSetNum(streamType);
                            for (int p = 0; p < num; p++)
                            {
                                if (device.QueryStreamProfileSet(streamType, p, out streamProfileSet) < RS.Status.STATUS_NO_ERROR) break;
                                RS.StreamProfile streamProfile = streamProfileSet[streamType];
                                ToolStripMenuItem sm1 = new ToolStripMenuItem(ProfileToString(streamProfile), null, new EventHandler(Stream_Item_Click));
                                profiles[sm1] = streamProfile;
                                streamMenus[s].DropDownItems.Add(sm1);
                            }
                        }
                        else if (((int)dinfo2.streams & (int)streamType) == 0 && streamMenus[s] != null)
                        {
                            streamMenus[s].Visible = false;
                            streamButtons[s].Visible = false;
                        }
                    }

                    device.Dispose();
                }
                capture.Dispose();
            }
            for (int i = 0; i < RS.Capture.STREAM_LIMIT; i++)
            {
                ToolStripMenuItem menu = streamMenus[i];
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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            menuStrip.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            renderStreams.StreamProfileSet = GetStreamSetConfiguration();
            renderStreams.DeviceInfo = GetCheckedDevice();
            renderStreams.StreamType = GetSelectedStream();
            
            renderStreams.Stop = false;
            System.Threading.Thread thread = new System.Threading.Thread(DoStreaming);
            thread.Start();
            System.Threading.Thread.Sleep(5);

        }
        
        //?????????????????????????????????????????????
        delegate void DoStreamingEnd();
        private void DoStreaming()
        {
            renderStreams.StreamColorDepth();
            Invoke(new DoStreamingEnd(
                delegate
                {
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                    menuStrip.Enabled = true;
                    if (closing) Close();
                }
            ));
        }

       
        // The StreamProfile structure describes the video stream configuration parameters.
        private RS.StreamProfile GetConfiguration(ToolStripMenuItem m)
        {
            foreach (ToolStripMenuItem e in m.DropDownItems)
                if (e.Checked) return profiles[e];
            return new RS.StreamProfile();
        }

        private RS.StreamProfile GetStreamConfiguration(RS.StreamType st)
        {
            ToolStripMenuItem menu = streamMenus[RS.Capture.StreamTypeToIndex(st)];
            if (menu != null)
                return GetConfiguration(menu);
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

        private RS.DeviceInfo GetCheckedDevice()
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
        private void RenderFrameHandler(Object sender, RenderFrameEventArgs e)
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
            renderStreams.Stop = true;
            e.Cancel = buttonStop.Enabled;
            closing = true;
        }

        private void SetStatus(String text)
        {
            statusStripLabel.Text = text;
        }

        private delegate void SetStatusDelegate(String status);
        private void UpdateStatusHandler(Object sender, UpdateStatusEventArgs e)
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

        private void Stream_Item_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menu in streamMenus)
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

        private void ResetStreamTypes()
        {
            renderStreams.StreamType = RS.StreamType.STREAM_TYPE_ANY;
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
                renderStreams.StreamType = st;
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
            renderStreams.Stop = true;
        }

        private void Stream_Click(object sender, EventArgs e)
        {
            RS.StreamType selected_stream = GetSelectedStream();
            if (selected_stream != renderStreams.StreamType)
            {

                renderStreams.StreamType = selected_stream;
            }
        }
    }

}
