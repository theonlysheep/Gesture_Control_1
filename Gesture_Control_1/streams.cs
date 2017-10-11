using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using RS = Intel.RealSense;

namespace streams.cs
{
    class Streams
    {
        // Ereignisdaklaration
        public event EventHandler<UpdateStatusEventArgs> UpdateStatus = null;
        public event EventHandler<RenderFrameEventArgs> RenderFrame = null;

        
        private Manager manager;

        public  RS.StreamProfileSet StreamProfileSet { get; set; }
        public RS.StreamType StreamType { get; set; }

        public bool Synced { get; set; }

        public Streams(Manager mngr)
        {
            manager = mngr;
            manager.DeviceInfo = null;
            StreamProfileSet = null;            
            manager.Stop = false;
            StreamType = RS.StreamType.STREAM_TYPE_ANY;
            Synced = true;
        }

        private void SetStatus(String text)
        {
            EventHandler<UpdateStatusEventArgs> handler = UpdateStatus;
            if (handler != null)
            {
                handler(this, new UpdateStatusEventArgs(text));
            }
        }

        public void StreamColorDepth() /* Stream Color and Depth Synchronously or Asynchronously */
        {
            try
            {
                bool sts = true;
                
                /* Set Color & Depth Resolution and enable streams */
                if (StreamProfileSet != null)
                {
                    /* Optional: Filter the data based on the request */
                    manager.SenseManager.CaptureManager.FilterByStreamProfiles(StreamProfileSet);

                    /* Enable raw data streaming for specific stream types */
                    for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
                    {
                        RS.StreamType st = RS.Capture.StreamTypeFromIndex(s);
                        RS.StreamProfile info = StreamProfileSet[st];
                        if (info.imageInfo.format != 0)
                        {
                            /* For simple request, you can also use sm.EnableStream(...) */
                            RS.DataDesc desc = new RS.DataDesc();
                            desc.streams[st].frameRate.min = desc.streams[st].frameRate.max = info.frameRate.max;
                            desc.streams[st].sizeMin.height = desc.streams[st].sizeMax.height = info.imageInfo.height;
                            desc.streams[st].sizeMin.width = desc.streams[st].sizeMax.width = info.imageInfo.width;
                            desc.streams[st].options = info.options;
                            desc.receivePartialSample = true;
                            RS.SampleReader sampleReader = RS.SampleReader.Activate(manager.SenseManager);
                            sampleReader.EnableStreams(desc);
                        }
                    }
                }

                /* Initialization */
                Timer timer = new Timer();
                timer.UpdateStatus += UpdateStatus;

                SetStatus("Init Started");
                if (manager.SenseManager.Init() >= RS.Status.STATUS_NO_ERROR)
                {
                    /* Reset all properties */
                    manager.SenseManager.CaptureManager.Device.ResetProperties(RS.StreamType.STREAM_TYPE_ANY);

                    SetStatus("Streaming");
                    while (!manager.Stop)
                    {
                        /* Wait until a frame is ready: Synchronized or Asynchronous */
                        if (manager.SenseManager.AcquireFrame(Synced) < RS.Status.STATUS_NO_ERROR)
                            break;

                        /* Display images */
                        RS.Sample sample = manager.SenseManager.Sample;

                        /* Render streams */
                        EventHandler<RenderFrameEventArgs> render = RenderFrame;
                        RS.Image image = null;
                        if (StreamType != RS.StreamType.STREAM_TYPE_ANY && render != null)
                        {
                            image = sample[StreamType];
                            render(this, new RenderFrameEventArgs(0, image));
                        }
                        
                        /* Optional: Show performance tick */

                        if (image != null) timer.Tick(RS.ImageExtension.PixelFormatToString(image.Info.format) + " " + image.Info.width + "x" + image.Info.height);

                        manager.SenseManager.ReleaseFrame();
                    }
                }
                else
                {
                    SetStatus("Init Failed");
                    sts = false;
                }

                manager.SenseManager.Dispose();
                if (sts) SetStatus("Stopped");
            }
            catch (Exception e)
            {
                SetStatus(e.GetType().ToString());
            }
        }
    }
}
