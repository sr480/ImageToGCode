using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration.ImageProcessor
{
    class MotionFactoryImage
    {
        protected readonly int _MinFeed;
        protected readonly int _MaxFeed;
        protected readonly int _MaxPower;
        protected readonly int _MinPower;
        public MotionFactoryImage(int minFeed, int maxFeed, int maxPower, int minPower)
        {
            _MinPower = minPower;
            _MaxPower = maxPower;
            _MaxFeed = maxFeed;
            _MinFeed = minFeed;
        }

        public IEnumerable<BaseGCode> CreateMotion(FreeMotionStroke stroke)
        {
            var result = new List<BaseGCode>(1);
            if (stroke is IdleStroke)
                result.Add(new CoordinatMotion(stroke.DestinationPoint, 0, _MaxFeed) { Comment = "Idle motion" });
            if (stroke is Stroke)
                result.Add(new CoordinatMotion(stroke.DestinationPoint,
                    (int)Math.Round(_MinPower + (_MaxPower - _MinPower) * ((Stroke)stroke).Intensity),
                    (int)Math.Round(_MinFeed + (_MaxFeed - _MinFeed) * (1 - ((Stroke)stroke).Intensity))));

            if (stroke is FreeMotionStroke)
                result.Add(new RapidMotion(stroke.DestinationPoint) { Comment = "New line move" });

            if (result.Count == 0)
                throw new Exception("Unknown stroke type");
            return result;
        }   
    }
}