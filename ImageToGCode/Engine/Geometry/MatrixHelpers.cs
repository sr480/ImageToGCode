using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.Geometry
{
    class MatrixHelpers
    {
        public static Vector Transform(PointF p, Matrix m)
        {
            //return new Vector(p.X * a00 + p.Y * a01 + dX, p.X * a10 + p.Y * a11 + dY);
            return new Vector(p.X * m.Elements[0] + p.Y * m.Elements[1] + m.Elements[4],
                p.X * m.Elements[2] + p.Y * m.Elements[3] + m.Elements[5]);
        }
    }
}
