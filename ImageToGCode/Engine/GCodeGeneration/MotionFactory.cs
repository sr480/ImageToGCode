using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class MotionFactory
    {
        public BaseMotion CreateMotion(FreeMotionStroke stroke, int minFeed, int feed, int maxPower, int minPower)
        {
            if (stroke is IdleStroke)
                return new CoordinatMotion(stroke.DestinationPoint, 0, feed) { Comment = "Idle motion" };
            if (stroke is Stroke)
                return new CoordinatMotion(stroke.DestinationPoint, 
                    (int)Math.Round(minPower + (maxPower - minPower) * ((Stroke)stroke).Intensity),
                    (int)Math.Round(minFeed + (feed - minFeed) * (1 - ((Stroke)stroke).Intensity)));

            if (stroke is FreeMotionStroke)
                return new RapidMotion(stroke.DestinationPoint) { Comment = "New line move" };
            throw new Exception("Unknown stroke type");
        }
    }
}
