using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class GCodeFromSVGGenerator
    {
        private readonly int _MaxPower;
        private readonly int _MaxFeed;
        private readonly int _MinPower;
        private readonly int _MinFeed;
        private readonly Svg.SvgDocument _Svg;

        MotionFactory _motionFactory;

        public GCodeFromSVGGenerator(Svg.SvgDocument svg, int minFeed, int feed, int maxPower, int minPower)
        {
            _Svg = svg;
            _MinFeed = minFeed;
            _MinPower = minPower;
            _MaxFeed = feed;
            _MaxPower = maxPower;
            _motionFactory = new MotionFactory(_MinFeed, _MaxFeed, _MaxPower, _MinPower);
        }


        public List<BaseGCode> GenerateCode()
        {
            var result = new List<BaseGCode>();
            MotionFactory mf = new MotionFactory(_MinFeed, _MaxFeed, _MaxPower, _MinPower);

            result.Add(new BaseGCode("G21"));
            result.Add(new BaseGCode("G90"));
            result.Add(new BaseGCode("M3 S0"));
            result.Add(new BaseGCode(string.Format("F{0}", _MaxFeed)));

            result.AddRange(GetElements(_Svg));

            result.Add(new BaseGCode("M5"));
            result.Add(new BaseGCode("%"));
            return result;
        }

        private IEnumerable<BaseGCode> GetElements (object element)
        {
            if (element is Svg.SvgPath)
                return GetElements((Svg.SvgPath)element);

            return GetElements((Svg.SvgElement)element);
        }

        private IEnumerable<BaseGCode> GetElements(Svg.SvgElement element)
        {            
            yield return _motionFactory.CreateMotion(element);

            foreach (var child in element.Children)
                foreach (var gcode in GetElements((object)child))
                    yield return gcode;
        }

        private IEnumerable<BaseGCode> GetElements(Svg.SvgPath path)
        {
            foreach (var segment in path.PathData)
                yield return _motionFactory.CreateMotion(segment);
        }
    }
}
