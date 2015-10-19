using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class AngleToVector
    {
        public static double DegToRad(double deg)
        {
            return deg / 180.0 * Math.PI;
        }
        public static Vector GetNormal(double angle)
        {
            return new Vector(Math.Cos(angle), Math.Sin(angle));
        }
        public static double GetAngle(Vector v)
        {
            return Math.Acos(v.Normalize().X);
        }
    }
}
