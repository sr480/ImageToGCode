using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.Visualisers
{
    class VisualLine
    {
        private readonly Vector _v1;
        private readonly Vector _v2;
        private readonly double _intensity;

        public Vector V1 { get { return _v1; } }
        public Vector V2 { get { return _v2; } }
        public double Intensity { get { return _intensity; } }

        public VisualLine(double x1, double x2, double y1, double y2, double intensity)
            : this(new Vector(x1, y1), new Vector(x2, y2), intensity) { }

        public VisualLine(Vector v1, Vector v2, double intensity)
        {
            _v1 = v1;
            _v2 = v2;
            _intensity = intensity;
        }
    }
}
