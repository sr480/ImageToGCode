using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor.ShortestWay
{
    class GraphicPathFacade
    {
        public PointF StartPoint { get; private set; }
        public PointF EndPoint { get; private set; }

        public GraphicsPath GrapicsPath { get; private set; }

        public GraphicPathFacade(GraphicsPath path)
        {
            var points = path.PathPoints;

            StartPoint = points[0];


            var pathTypes = path.PathTypes;
            if (Geometry.PathTypeHelper.IsSet(pathTypes[pathTypes.Length - 1], System.Drawing.Drawing2D.PathPointType.CloseSubpath))
                EndPoint = StartPoint;
            else
                EndPoint = points[points.Length - 1];

            GrapicsPath = path;
        }
    }
}
