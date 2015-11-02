using System;
using System.Collections.Generic;
using System.Drawing;
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

            SvgToGCodeRenderer gr = new SvgToGCodeRenderer(new MotionFactorySvg(_MinFeed, _MaxFeed, _MaxPower, _MinPower));

            result.Add(new BaseGCode("G21"));
            result.Add(new BaseGCode("G90"));
            result.Add(new BaseGCode("M3 S0"));
            result.Add(new BaseGCode(string.Format("F{0}", _MaxFeed)));
                        
            _Svg.Draw(gr);
            result.AddRange(gr.GCode);

            result.Add(new BaseGCode("M5"));
            result.Add(new BaseGCode("%"));
            return result;
        }
    }
}
