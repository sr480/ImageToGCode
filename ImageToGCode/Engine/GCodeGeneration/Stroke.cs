using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration
{
    class Stroke : FreeMotionStroke
    {
        public Vector DestinationPoint { get; private set; }
        public double Intensity { get; private set; } // 1-черный 0 - белый

        public Stroke(Vector destinationPoint, double intensity)
            : base(destinationPoint)
        {
            Intensity = intensity;
        }
    }
}
