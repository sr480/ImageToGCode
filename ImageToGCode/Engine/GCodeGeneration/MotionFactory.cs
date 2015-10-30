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
        public MotionFactory(int minFeed, int maxFeed, int maxPower, int minPower)
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
        public IEnumerable<BaseGCode> CreateMotion(Svg.SvgElement element, double factorX, double factorY)
        {
            var vF = new FactorVectorFactory(factorX, factorY);

            var result = new List<BaseGCode>(1);
            //Line
            if (element is Svg.SvgLine)
                result.Add(new CoordinatMotion(vF.Create(((Svg.SvgLine)element).EndX, ((Svg.SvgLine)element).EndY), _MaxPower, _MaxFeed));
            
            //Path
            else if (element is Svg.SvgPath)
            {
                var path = (Svg.SvgPath)element;
                if (path.PathData.Count != 0)
                {
                    var pathStart = path.PathData.First();
                    result.Add(new RapidMotion(vF.Create(pathStart.Start)));

                    foreach (var segment in ((Svg.SvgPath)element).PathData.Skip(1))
                    {
                        if (segment is Svg.Pathing.SvgClosePathSegment) //Closing the path
                            result.Add(new CoordinatMotion(vF.Create(pathStart.Start), _MaxPower, _MaxFeed));

                        else if (segment is Svg.Pathing.SvgCubicCurveSegment) //Curve segment
                        {
                            var curve = (Svg.Pathing.SvgCubicCurveSegment)segment;
                            var bezier = new Bezier(new Vector(curve.Start), new Vector(curve.End),
                                new Vector(curve.FirstControlPoint), new Vector(curve.SecondControlPoint));
                            //new Bezier(vF.Create(curve.Start), vF.Create(curve.End),
                            //    vF.Create(curve.FirstControlPoint), vF.Create(curve.SecondControlPoint));
                            foreach (var point in bezier.GetInterpolatedPoints())
                                result.Add(new CoordinatMotion(vF.Create(point), _MaxPower, _MaxFeed));
                        }
                        else
                            result.Add(new CoordinatMotion(vF.Create(segment.End), _MaxPower, _MaxFeed));
                    }
                }
            }
            else
                result.Add(new BaseGCode(string.Format("(Unknown element: {0})", element.GetType())));
            return result;
        }
    }
}
