using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration
{
    class MotionFactory
    {
        private readonly int _MinFeed;
        private readonly int _MaxFeed;
        private readonly int _MaxPower;
        private readonly int _MinPower;
        public MotionFactory (int minFeed, int maxFeed, int maxPower, int minPower)
	    {
            _MinPower = minPower;
         _MaxPower = maxPower;
         _MaxFeed = maxFeed;
         _MinFeed = minFeed;
	    }


        public BaseGCode CreateMotion(FreeMotionStroke stroke)
        {
            if (stroke is IdleStroke)
                return new CoordinatMotion(stroke.DestinationPoint, 0, _MaxFeed) { Comment = "Idle motion" };
            if (stroke is Stroke)
                return new CoordinatMotion(stroke.DestinationPoint, 
                    (int)Math.Round(_MinPower + (_MaxPower - _MinPower) * ((Stroke)stroke).Intensity),
                    (int)Math.Round(_MinFeed + (_MaxFeed - _MinFeed) * (1 - ((Stroke)stroke).Intensity)));

            if (stroke is FreeMotionStroke)
                return new RapidMotion(stroke.DestinationPoint) { Comment = "New line move" };
            throw new Exception("Unknown stroke type");
        }
        public BaseGCode CreateMotion(Svg.SvgElement element)
        {
            if (element is Svg.SvgLine)
                return new CoordinatMotion(new Vector(((Svg.SvgLine)element).EndX.Value, ((Svg.SvgLine)element).EndY.Value), _MaxPower, _MinFeed);
            else
                return new BaseGCode(string.Format("(Unknown element: {0})", element.GetType().ToString()));
        }
        public CoordinatMotion CreateMotion(Svg.Pathing.SvgPathSegment segment)
        {
            return new CoordinatMotion(new Vector(segment.End), _MaxPower, _MaxFeed);
        }
    }
}
