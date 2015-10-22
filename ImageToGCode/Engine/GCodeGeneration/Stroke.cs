using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class Stroke
    {
        public Vector StartPoint { get; private set; }
        public Vector EndPoint { get; private set; }
        public double Intensity { get; private set; } // 1-черный 0 - белый

        public Stroke(Vector startPoint, Vector endPoint, double intensity)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Intensity = intensity;
        }
    }
}
