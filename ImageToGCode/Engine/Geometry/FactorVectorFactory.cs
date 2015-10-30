using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.Geometry
{
    class FactorVectorFactory
    {
        private double _FactorX;
        private double _FactorY;
        public FactorVectorFactory(double factorX, double factorY)
        {
            _FactorX = factorX;
            _FactorY = factorY;
        }

        public Vector Create(Vector v)
        {
            return new Vector(v.X * _FactorX, v.Y * _FactorY);
        }
        public Vector Create(PointF p)
        {
            return new Vector(p.X * _FactorX, p.Y * _FactorY);
        }
        public Vector Create(double x, double y)
        {
            return new Vector(x * _FactorX, y * _FactorY);
        }
    }
}
