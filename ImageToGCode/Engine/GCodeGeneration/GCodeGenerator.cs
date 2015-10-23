using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class GCodeGenerator
    {
        private readonly int _MaxPower;
        private readonly int _Feed;
        private readonly IEnumerable<FreeMotionStroke> _Strokes;
        public GCodeGenerator(IEnumerable<FreeMotionStroke> strokes, int feed, int maxPower)
        {
            _Strokes = strokes;
            _Feed = feed;
            _MaxPower = maxPower;
        }

        
        public List<string> GenerateCode()
        {
            var result = new List<string>();
            MotionFactory mf = new MotionFactory();

            result.Add("G21");
            result.Add("G90");
            result.Add("M3 S0");

            foreach(var stroke in _Strokes)
            {
                result.Add(mf.CreateMotion(stroke, _Feed, _MaxPower).ToString());
            }

            result.Add("M5");
            result.Add("%");
            return result;
        }
    }
}
