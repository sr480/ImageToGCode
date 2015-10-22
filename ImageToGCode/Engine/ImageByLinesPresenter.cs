using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class ImageByLinesPresenter
    {
        private readonly List<ImageLine> _Lines;
        private readonly Image _Image;
        public Image Image
        {
            get
            {
                return _Image;
            }
        }
        public List<ImageLine> Lines { get { return _Lines; } }

        public ImageByLinesPresenter(Image image)
        {
            _Image = image;
            _Lines = new List<ImageLine>();
        }

        public void Present(Vector normalVector, double lineResolution, double pointResolution)
        {
            Lines.Clear();

            Vector vectorIncrement = normalVector.Normalize() * lineResolution;

            Vector startingPoint;
            Vector endPoint;
            Vector currentVector;
            if (vectorIncrement.X < 0) //для углов 91-180 рисуем, начиная с левого верхнего края - координата (0, Ymax)
            {
                startingPoint = new Vector(0, Image.Height - 1);
                endPoint = new Vector(Image.Width - 1, 0);
                vectorIncrement = vectorIncrement.Reverse();
                currentVector = Line.GetIntersection(new Line(vectorIncrement, startingPoint), new Line(new Vector(-vectorIncrement.Y, vectorIncrement.X), new Vector(0, 0)));
            }
            else // для углов 0-90 с точки (0,0)
            {
                startingPoint = new Vector(0, 0);
                endPoint = new Vector(Image.Width - 1, Image.Height - 1);
                currentVector = vectorIncrement;
            }

            Lines.Add(new ImageLine(vectorIncrement, startingPoint, Image));

            var BorderLine = new Line(vectorIncrement, endPoint);
            var IncrementLine = new Line(new Vector(-vectorIncrement.Y, vectorIncrement.X), new Vector(0, 0));
            var Intersection = Line.GetIntersection(BorderLine, IncrementLine);


            while (currentVector.X <= Intersection.X && (currentVector.Y * Intersection.Y < 0 || Math.Abs(currentVector.Y) <= Math.Abs(Intersection.Y)))
            {
                Lines.Add(new ImageLine(currentVector, Image));
                currentVector += vectorIncrement;
            }

            foreach (var item in Lines)
            {
                item.GeneratePixels(pointResolution);
            }

        }
    }
}
