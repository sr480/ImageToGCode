using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.Geometry
{
    class Vector
    {
        private readonly double _Y;
        private readonly double _X;
        public double X { get { return _X; } }
        public double Y { get { return _Y; } }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }
        public static Vector operator *(Vector a, Vector b)
        {
            return new Vector(a.X * b.X, a.Y * b.Y);
        }

        public static Vector operator *(Vector a, double d)
        {
            return new Vector(a.X * d, a.Y * d);
        }


        private double? _Length;

        public double Length
        {
            get
            {
                if (!_Length.HasValue)
                    _Length = Math.Sqrt(X * X + Y * Y);
                return _Length.Value;
            }
        }

        public Vector(PointF point) : this(point.X, point.Y) { }

        public Vector(double x, double y)
        {
            _X = x;
            _Y = y;
        }

        public Vector Normalize()
        {
            return new Vector(X / Length, Y / Length);
        }

        public Vector Reverse()
        {
            return new Vector(-X, -Y);
        }
        public Vector Rotate(double angle)
        {
            return new Vector(X * Math.Cos(angle) - Y * Math.Sin(angle), Y * Math.Cos(angle) + X * Math.Sin(angle));
        }
        public Vector Rotate90CCW()
        {
            return new Vector(-Y, X);
        }
        public double GetAngle()
        {
            return Math.Atan(Y / X);
        }
    }
}
