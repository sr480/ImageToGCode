using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class CoordinatMotion : RapidMotion
    {
        private const string GCODE_FORMAT = "G1 X{0} Y{1} F{2} S{3}";
        public int Intensity { get; private set; }
        public int Feed { get; private set; }
        public CoordinatMotion(Vector position, int intensity, int feed) : base(position)
        {
            Intensity = intensity;
            Feed = feed;
        }

        public override string ToString()
        {
            return string.Format(GCODE_FORMAT, DoubleToStr(Position.X), DoubleToStr(Position.Y), Feed, Intensity);
        }
    }
}
