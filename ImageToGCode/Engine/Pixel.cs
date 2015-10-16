using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class Pixel : Vector
    {
        public double Intencity { get; set; }

        public Pixel(double x, double y) : this(0.0, x, y) { }

        public Pixel(double intencity, double x, double y)
            : base(x, y)
        {
            Intencity = intencity;
        }
    }
}
