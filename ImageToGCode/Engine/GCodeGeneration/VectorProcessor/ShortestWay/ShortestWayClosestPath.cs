using ImageToGCode.Engine.GCodeGeneration.VectorProcessor.ShortestWay;
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
        private readonly List<GraphicPathFacade> _paths;
        public ShortestWayClosestPath(List<GraphicsPath> graphicPathCollection)
        {
            _paths = graphicPathCollection.Select(x => new GraphicPathFacade(x)).ToList();
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
            var conv = new PathCollectionToDistanceMatrixConverter(_paths);
            conv.Convert();
            var distanceMatrix = conv.Result;

            var result = new List<GraphicsPath>();
            var included = new HashSet<int>();



            int currentIndex;
            result.Add(conv.GetClosestToZeroPath(out currentIndex).GrapicsPath);
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
                result.Add(_paths[bestIndex].GrapicsPath);
                currentIndex = bestIndex;
            }


            return result;
        }



    }
}
