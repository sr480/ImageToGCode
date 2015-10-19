﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class Vector
    {
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

        private readonly double _Y;
        private readonly double _X;
        private readonly Lazy<double> _Length;
        public double X { get { return _X; } }
        public double Y { get { return _Y; } }
        public double Length { get { return _Length.Value; } }

        public Vector(double x, double y)
        {
            _X = x;
            _Y = y;
            _Length = new Lazy<double>(() => Math.Sqrt(Math.Pow(X, 2.0) + Math.Pow(Y, 2.0)), false);
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
            return Math.Atan(Y/X);
        }
    }
}
