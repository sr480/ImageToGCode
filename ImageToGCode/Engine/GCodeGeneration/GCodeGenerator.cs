using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration
{
    class GCodeGenerator
    {
        private readonly int _MaxPower;
        private readonly int _Feed;
        private readonly IEnumerable<FreeMotionStroke> _Strokes;
        private readonly int _MinPower;
        private readonly int _MinFeed;
        public GCodeGenerator(IEnumerable<FreeMotionStroke> strokes, int minFeed, int feed, int maxPower, int minPower)
        {
            _MinFeed = minFeed;
            _MinPower = minPower;
            _Strokes = strokes;
            _Feed = feed;
            _MaxPower = maxPower;
        }

        
        public List<string> GenerateCode()
        {
            var result = new List<string>();
            MotionFactory mf = new MotionFactory();

            result.Add(string.Format("(MinFeed: {0}, MaxFeed: {1})", _MinFeed, _Feed));
            result.Add(string.Format("(MinPower: {0}, MaxPower: {1})", _MinPower, _MaxPower));

            result.Add("G21");
            result.Add("G90");
            result.Add("M3 S0");
            result.Add(string.Format("F{0}", _Feed));

            foreach(var stroke in _Strokes)
            {
                result.Add(mf.CreateMotion(stroke, _MinFeed, _Feed, _MaxPower, _MinPower).ToString());
            }

            result.Add("M5");
            result.Add("%");
            return result;
        }
    }
}
