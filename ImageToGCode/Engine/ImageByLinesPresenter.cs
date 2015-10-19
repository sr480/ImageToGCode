using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class ImageByLinesPresenter
    {
        private readonly List<Line> _Lines;
        private readonly Image _Image;
        public List<Line> Lines { get { return _Lines; } }

        public ImageByLinesPresenter(Image image)
        {
            _Image = image;
            _Lines = new List<Line>();
        }

        public void Present(Vector normalVector, double lineResolution, double pointResolution)
        {
            Lines.Clear();


            Vector vectorIncrement = normalVector.Normalize() * lineResolution;

            Lines.Add(new Line(vectorIncrement, new Vector(0, 0), _Image)); //узнать, что с цветом НЕ ТАК!
            

            var BorderLine = new Line(vectorIncrement, new Vector(_Image.Width - 1, _Image.Height - 1), _Image);
            var IncrementLine = new Line(new Vector(-vectorIncrement.X, vectorIncrement.Y), new Vector(0, 0), _Image);

            var Intersection = Line.GetIntersection(BorderLine, IncrementLine);

            Vector currentVector = vectorIncrement;

            while (currentVector.X < Intersection.X | currentVector.Y < Intersection.Y)
            {
                Lines.Add(new Line(currentVector, _Image));
                currentVector += vectorIncrement;
            }


            var lastVector = currentVector - vectorIncrement;
            Console.WriteLine(lastVector);


            foreach (var item in Lines)
            {
                item.GeneratePixels(pointResolution);
            }
        }
    }
}
