using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ImageToGCode.Engine.Visualisers
{
    static class ToPointConverter
    {
        public static Point FromPointF(System.Drawing.PointF point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
