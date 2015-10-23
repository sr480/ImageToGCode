using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class RapidMotion : BaseMotion
    {
        private const string GCODE_FORMAT = "G0 X{0} Y{1} S0";
        private const string GCODE_FORMAT_COMMENT = "G0 X{0} Y{1} S0; {2}";
        public string Comment { get; set; }
        
        public RapidMotion(Vector position) : base(position)
        { }
        public RapidMotion(Vector position, string comment) : this(position)
        {           
            Comment = comment;
        }

        public override string ToString()
        {
            if(!string.IsNullOrEmpty(Comment))
                return string.Format(GCODE_FORMAT_COMMENT, DoubleToStr(Position.X), DoubleToStr(Position.Y), Comment);
            return string.Format(GCODE_FORMAT, DoubleToStr(Position.X), DoubleToStr(Position.Y));
        }
    }
}
