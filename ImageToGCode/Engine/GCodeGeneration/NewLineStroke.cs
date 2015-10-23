using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class FreeMotionStroke
    {
        public Vector DestinationPoint { get; private set; }

        public FreeMotionStroke(Vector destinationPoint)
        {
            DestinationPoint = destinationPoint;
        }    
    }
}
