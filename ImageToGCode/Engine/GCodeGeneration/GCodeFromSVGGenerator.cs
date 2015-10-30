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
        private double _factorX;
        private double _factorY;
        MotionFactory _motionFactory;

        public GCodeFromSVGGenerator(Svg.SvgDocument svg, int minFeed, int feed, int maxPower, int minPower)
        {
            _Svg = svg;
            _factorX = _Svg.Width.Value / (_Svg.ViewBox.Width - _Svg.ViewBox.MinX);
            _factorY = _Svg.Height.Value / (_Svg.ViewBox.Height - _Svg.ViewBox.MinY);
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

        private IEnumerable<BaseGCode> GetElements(Svg.SvgElement element)
        {
            foreach (BaseGCode gcode in _motionFactory.CreateMotion(element, _factorX, _factorY))
                yield return gcode;

            foreach (var child in element.Children)
                foreach (var gcode in GetElements(child))
                    yield return gcode;
        }
    }
}
