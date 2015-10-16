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
            
            Lines.Add(new Line(vectorIncrement, new Vector(0, 0), _Image));

            int i = 1;
            Vector currentVector = vectorIncrement;
            while (currentVector.Length < Math.Sqrt(Math.Pow(_Image.Height - 1, 2) + Math.Pow(_Image.Width - 1, 2)))
            {
                Lines.Add(new Line(currentVector, _Image));
                currentVector = vectorIncrement * i * lineResolution;
                i++;
            }

                /*Parallel.ForEach(Lines, (l =>
                {
                    l.GeneratePixels(pointResolution);
                }));*/

            foreach (var item in Lines)
            {
                item.GeneratePixels(pointResolution);
            }
        }
    }
}
