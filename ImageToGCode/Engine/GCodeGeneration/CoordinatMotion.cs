using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration
{
    class CoordinatMotion : BaseMotion
    {
        private const string GCODE_FORMAT = "G1X{0}Y{1}F{2}S{3}";
        private const string GCODE_FORMAT_COMMENT = "G1X{0}Y{1}F{2}S{3};{4}";
        public int Intensity { get; private set; }
        public int Feed { get; private set; }
        public Color Color { get; private set; }

        public CoordinatMotion(Vector position, int intensity, int feed, Color color)
            : base(position)
        {
            Intensity = intensity;
            Feed = feed;
            Color = color;
        }
        public CoordinatMotion(Vector position, int intensity, int feed) : this(position, intensity, feed, Color.Black)
        { }

        public override string ToString()
        {
            if(!string.IsNullOrEmpty(Comment))
                return string.Format(GCODE_FORMAT_COMMENT, DoubleToStr(Position.X), DoubleToStr(Position.Y), 
                    Feed, 
                    Intensity, Comment);
            return string.Format(GCODE_FORMAT, DoubleToStr(Position.X), DoubleToStr(Position.Y), Feed, Intensity);
        }
    }
}
