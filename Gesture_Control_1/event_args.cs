
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RS = Intel.RealSense;

namespace streams.cs
{
   public class UpdateStatusEventArgs : EventArgs
    {
        public String text { get; set; }
        
        public UpdateStatusEventArgs(String text)
        {
            this.text = text;
        }
    }

    public class RenderFrameEventArgs : EventArgs
    {
        public int index { get; set; }
        public RS.Image image { get; set; }

        public RenderFrameEventArgs(int index, RS.Image image)
        {
            this.index = index;
            this.image = image;
        }
    }
}
