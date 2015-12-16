using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageToGCode.Engine.Geometry;
using System.Drawing;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class ArcMotion : CoordinatMotion
    {
        public ArcMotion(Vector position, double r, int intensity, int feed, Color color)
            : base(position, intensity, feed, color)
        {
            R = r;
        }
        public ArcMotion(Vector position, double r, int intensity, int feed)
            : base(position, intensity, feed)
        {
            R = r;
        }

        public double R { get; set; }
        public bool IsClockWise { get; set; }

    }
}
