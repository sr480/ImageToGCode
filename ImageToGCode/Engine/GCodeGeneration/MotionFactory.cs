using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class MotionFactory
    {
        public BaseMotion CreateMotion(FreeMotionStroke stroke, int feed, int maxPower)
        {
            if (stroke is IdleStroke)
                return new CoordinatMotion(stroke.DestinationPoint, 0, feed);
            if (stroke is Stroke)
                return new CoordinatMotion(stroke.DestinationPoint, (int)Math.Round(maxPower * ((Stroke)stroke).Intensity), feed);
            if (stroke is FreeMotionStroke)
                return new RapidMotion(stroke.DestinationPoint);
            throw new Exception("Unknown stroke type");
        }
    }
}
