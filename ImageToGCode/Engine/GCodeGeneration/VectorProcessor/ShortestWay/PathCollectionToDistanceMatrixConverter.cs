using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor.ShortestWay
{
    class PathCollectionToDistanceMatrixConverter
    {
        private readonly List<GraphicPathFacade> _paths;
        public PathCollectionToDistanceMatrixConverter(List<GraphicPathFacade> graphicPathCollection)
        {
            _paths = graphicPathCollection;
        }

        public void Convert()
        {
            Result = GetDistanceMatrix();
        }

        public double[][] Result { get; private set; }

        private double[][] GetDistanceMatrix()
        {
            var result = new double[_paths.Count][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[result.Length];
            }

            for (int i = 0; i < _paths.Count; i++)
            {
                for (int j = 0; j < _paths.Count; j++)
                {
                    result[i][j] = GetDistance(_paths[i], _paths[j]);
                }
            }
            return result;
        }

        private double GetDistance(GraphicPathFacade first, GraphicPathFacade last)
        {
            if (first == last)
                return 0;

            var from = first.EndPoint;
            var to = last.StartPoint;

            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        /*private PointF GetFirstPoint(GraphicsPath path)
        {
            return path.PathPoints[0];
        }

        private PointF GetLastPoint(GraphicsPath path)
        {
            if (Geometry.PathTypeHelper.IsSet(path.PathTypes.Last(), System.Drawing.Drawing2D.PathPointType.CloseSubpath))
                return path.PathPoints[0];

            return path.PathPoints.Last();
        }*/

        public GraphicPathFacade GetClosestToZeroPath(out int index)
        {
            double MinDistanceToZero = double.PositiveInfinity;
            GraphicPathFacade ClosestToZero = null;
            foreach (var item in _paths)
            {
                var dist = Math.Pow(item.StartPoint.X, 2) + Math.Pow(item.StartPoint.Y, 2);

                if (dist < MinDistanceToZero)
                {
                    ClosestToZero = item;
                    MinDistanceToZero = dist;
                }
            }
            index = _paths.IndexOf(ClosestToZero);

            return ClosestToZero;
        }
    }
}
