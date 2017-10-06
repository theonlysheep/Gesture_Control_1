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
        private RenderStreams renderStreams = new RenderStreams();
        private volatile bool closing = false;

        // Layout 
        private ToolStripMenuItem[] streamMenus = new ToolStripMenuItem[RS.Capture.STREAM_LIMIT];
        private RadioButton[] streamButtons = new RadioButton[RS.Capture.STREAM_LIMIT];

        // Rendering
        private D2D1Render[] renders = new D2D1Render[2] { new D2D1Render(), new D2D1Render() }; // reder for .NET PictureBox

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
            streamButtons[RS.Capture.StreamTypeToIndex(RS.StreamType.STREAM_TYPE_IR)] = radioDepth;

            // register event handler 
            renderStreams.UpdateStatus += new EventHandler<UpdateStatusEventArgs>(UpdateStatusHandler);
            renderStreams.RenderFrame += new EventHandler<RenderFrameEventArgs>(RenderFrameHandler);
            FormClosing += new FormClosingEventHandler(FormClosingHandler);
            rgbImage.Paint += new PaintEventHandler(PaintHandler);

            rgbImage.Resize += new EventHandler(ResizeHandler);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {

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
    }
}
