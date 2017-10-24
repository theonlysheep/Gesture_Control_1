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
        
        public void ResetStreamTypes()
        {
            StreamType = RS.StreamType.STREAM_TYPE_ANY;
        }

        public void ConfigureStreams()
        {
            RS.SampleReader sampleReader = RS.SampleReader.Activate(manager.SenseManager);
            sampleReader.EnableStream(RS.StreamType.STREAM_TYPE_COLOR, 1920, 1080, 30);
            sampleReader.EnableStream(RS.StreamType.STREAM_TYPE_DEPTH, 640, 480, 60);
            sampleReader.EnableStream(RS.StreamType.STREAM_TYPE_IR, 640, 480, 60);
        }

        public void RenderStreams(RS.Sample sample)
        {
            /* Render streams */
            EventHandler<RenderFrameEventArgs> render = RenderFrame;
            RS.Image image = null;
            if (render != null)
            {                
                image = sample[RS.StreamType.STREAM_TYPE_COLOR];
                render(this, new RenderFrameEventArgs(0, image));

                if (StreamType == RS.StreamType.STREAM_TYPE_ANY)
                    return;
                else
                {
                    image = sample[StreamType];
                    render(this, new RenderFrameEventArgs(1, image));
                }                
            }
        }
        
    }
}
