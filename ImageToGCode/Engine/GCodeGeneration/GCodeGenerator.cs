using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration
{
    class GCodeGenerator
    {
        private readonly int _MaxPower;
        private readonly int _MaxFeed;
        private readonly IEnumerable<FreeMotionStroke> _Strokes;
        private readonly int _MinPower;
        private readonly int _MinFeed;
        public GCodeGenerator(IEnumerable<FreeMotionStroke> strokes, int minFeed, int feed, int maxPower, int minPower)
        {
            _MinFeed = minFeed;
            _MinPower = minPower;
            _Strokes = strokes;
            _MaxFeed = feed;
            _MaxPower = maxPower;
        }

        
        public List<BaseGCode> GenerateCode()
        {
            var result = new List<BaseGCode>();
            MotionFactory mf = new MotionFactory(_MinFeed, _MaxFeed, _MaxPower, _MinPower);
                        
            result.Add(new BaseGCode("G21"));
            result.Add(new BaseGCode("G90"));
            result.Add(new BaseGCode("M3 S0"));
            result.Add(new BaseGCode(string.Format("F{0}", _MaxFeed)));

            foreach(var stroke in _Strokes)
            {
                result.Add(mf.CreateMotion(stroke));
            }

            result.Add(new BaseGCode("M5"));
            result.Add(new BaseGCode("%"));
            return result;
        }
    }
}