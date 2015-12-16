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
            if (stroke is IdleStroke)
                yield return new CoordinatMotion(stroke.DestinationPoint, 0, _MaxFeed, System.Drawing.Color.Aquamarine) { Comment = "Idle motion" };
            if (stroke is Stroke)
            {
                int colorIntencity = (int)(255 * ((Stroke)stroke).Intensity);
                yield return new CoordinatMotion(stroke.DestinationPoint,
                    (int)Math.Round(_MinPower + (_MaxPower - _MinPower) * ((Stroke)stroke).Intensity),
                    (int)Math.Round(_MinFeed + (_MaxFeed - _MinFeed) * (1 - ((Stroke)stroke).Intensity)),
                    System.Drawing.Color.FromArgb(colorIntencity, colorIntencity, colorIntencity));
            }
            if (stroke is FreeMotionStroke)
                yield return new RapidMotion(stroke.DestinationPoint) { Comment = "New line move" };
        }
    }
}