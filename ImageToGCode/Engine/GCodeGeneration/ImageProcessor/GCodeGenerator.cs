using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration.ImageProcessor
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

        
        public IEnumerable<BaseGCode> GenerateCode()
        {
            MotionFactoryImage mf = new MotionFactoryImage(_MinFeed, _MaxFeed, _MaxPower, _MinPower);

            foreach (var stroke in _Strokes)
                foreach (var item in mf.CreateMotion(stroke))
                    yield return item;
        }
    }
}