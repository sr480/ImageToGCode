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
            var currentLine = new Line(vectorIncrement, new Vector(0, 0), _Image);
            Lines.Add(currentLine);

            int i = 1;
            Vector currentVector = vectorIncrement;
            while (currentVector.Length < Math.Sqrt(Math.Pow(_Image.Height - 1, 2) + Math.Pow(_Image.Width - 1, 2)))
            {
                currentVector = vectorIncrement * i * lineResolution;
                Lines.Add(new Line(currentVector, _Image));
                i++;
            }

            Parallel.ForEach(Lines, (l =>
                {
                    l.GeneratePixels(pointResolution);
                }));
        }
    }
}
