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
        public event EventHandler<RenderFrameEventArgs> RenderFrame = null;
        public RS.StreamProfileSet StreamProfileSet { get; set; }
        public RS.StreamType StreamType { get; set; }
        private Manager manager = null;
        

        public Streams(Manager mngr)
        {
            StreamProfileSet = null;
            StreamType = RS.StreamType.STREAM_TYPE_ANY;
            manager = mngr;
        }
        
        private void ResetStreamTypes()
        {
            StreamType = RS.StreamType.STREAM_TYPE_ANY;
        }

        public void EnableStreamsFromSelection()
        {
            /* Set Color & Depth Resolution and enable streams */
            if (StreamProfileSet != null)
            {
                /* Optional: Filter the data based on the request */
                manager.SenseManager.CaptureManager.FilterByStreamProfiles(StreamProfileSet);

                /* Enable raw data streaming for specific stream types */

                // Set frame Rate, Height and With for all Sterams
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
        }

        public void RenderStreams(RS.Sample sample)
        {
            /* Render streams */
            EventHandler<RenderFrameEventArgs> render = RenderFrame;
            RS.Image image = null;
            if (StreamType != RS.StreamType.STREAM_TYPE_ANY && render != null)
            {
                // ???????????????????????
                image = sample[StreamType];
                render(this, new RenderFrameEventArgs(0, image));
                render(this, new RenderFrameEventArgs(1, image));
            }
        }
        
    }
}
