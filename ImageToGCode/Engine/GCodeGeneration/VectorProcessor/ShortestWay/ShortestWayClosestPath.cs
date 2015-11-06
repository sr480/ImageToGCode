using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor
{
    class ShortestWayClosestPath
    {
        private readonly List<GraphicsPath> _paths;
        public ShortestWayClosestPath(List<GraphicsPath> graphicPathCollection)
        {
            _paths = graphicPathCollection;
        }

        public List<GraphicsPath> PerfectGraphicPathCollection
        {
            get
            {
                return GetResultingCollection();
            }
        }

        private List<GraphicsPath> GetResultingCollection()
        {
            var distanceMatrix = GetDistanceMatrix();

            var result = new List<GraphicsPath>();
            var included = new HashSet<int>();


            double MinDistanceToZero = double.PositiveInfinity;
            GraphicsPath ClosestToZero = null;
            foreach (var item in _paths)
            {
                var dist = Math.Pow(item.PathData.Points[0].X, 2) + Math.Pow(item.PathData.Points[0].Y, 2);

                if (dist < MinDistanceToZero)
                {
                    ClosestToZero = item;
                    MinDistanceToZero = dist;
                }
            }

            result.Add(ClosestToZero);
            int currentIndex = _paths.IndexOf(ClosestToZero);
            included.Add(currentIndex);


            while (included.Count != _paths.Count)
            {
                int bestIndex = -1;
                double bestDistance = double.PositiveInfinity;

                for (int i = 0; i < _paths.Count; i++)
                {
                    if (included.Contains(i) || i == currentIndex)
                        continue;
                    if (bestDistance > distanceMatrix[currentIndex][i])
                    {
                        bestIndex = i;
                        bestDistance = distanceMatrix[currentIndex][i];
                    }
                }

                included.Add(bestIndex);
                result.Add(_paths[bestIndex]);
                currentIndex = bestIndex;
            }


            return result;
        }


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

        private double GetDistance(GraphicsPath first, GraphicsPath last)
        {
            if (first == last)
                return 0;

            var from = GetLastPoint(first);
            var to = GetFirstPoint(last);

            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        private PointF GetFirstPoint(GraphicsPath path)
        {
            return path.PathData.Points[0];
        }

        private PointF GetLastPoint(GraphicsPath path)
        {
            if (Geometry.PathTypeHelper.IsSet(path.PathData.Types.Last(), System.Drawing.Drawing2D.PathPointType.CloseSubpath))
                return path.PathData.Points[0];

            return path.PathData.Points.Last();
        }

    }
}
