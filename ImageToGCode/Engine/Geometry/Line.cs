using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.Geometry
{
    class Line
    {
        //прямая Ax+By+C=0
        protected double A;
        protected double B;
        protected double C;


        public double GetX(double Y)
        {
            return (-C - B * Y) / A;
        }

        public double GetY(double X)
        {
            return (-C - A * X) / B;
        }


        //конструктор прямой через вектор нормали и точку
        public Line (Vector normalVector, Vector pointToCross)
        {
            A = normalVector.X;
            B = normalVector.Y;

            C = -(A * pointToCross.X + B * pointToCross.Y);
        }

        //контструктор прямой через вектор нормали. А координаты точки совпадают с координатами вектора нормали
        public Line(Vector normalVector)
            : this(normalVector, normalVector)
        { }

        public static Vector GetIntersection(Line l1, Line l2)
        {
            return new Vector(-(l1.C * l2.B - l2.C * l1.B) / (l1.A * l2.B - l2.A * l1.B), -(l1.A * l2.C - l2.A * l1.C) / (l1.A * l2.B - l2.A * l1.B));
        }
    }
}
