using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class Line
    {
        
        private Vector _NormalVector;
        private Image _Image;

        private List<Pixel> _Pixels;
        public List<Pixel> Pixels
        {
            get
            {
                return _Pixels;
            }
        }

        public Line(Vector normalVector, Image image)
        {
            _NormalVector = normalVector;
            _Image = image;

            _Pixels = new List<Pixel>();
        }

        public void GeneratePixels(double pointResolution)
        {
            var firstVector = GetFirstVector();

            var directionVector = (firstVector - _NormalVector).Normalize()  * pointResolution ;

            //пока так
            IInterpolator inter = new BilinearInterpolator();

            _Pixels.Add(inter.GetPixel(_Image, firstVector));
            
        }

        private Vector GetFirstVector()
        {
            double x = _NormalVector.X;
            double y = _NormalVector.Y;

            double temp;
            
            //пересечение c OX
            temp = (x * x + y * y) / x;
            if (0 <= temp && temp <= _Image.Width - 1)
                return new Vector(temp, 0);

            //пересечение с OY
            temp = (x * x + y * y) / y;
            if (0 <= temp && temp <= _Image.Height - 1)
                return new Vector(0, temp);

            //пересечение с y=max
            temp = (x * x + y * y - y * (_Image.Height - 1)) / x;
            if (0 <= temp && temp <= _Image.Width - 1)
                return new Vector(temp, _Image.Height - 1);

            //пересечение с x=max
            temp = (x * x + y * y - x*(_Image.Width-1)) / y;
            if (0 <= temp && temp <= _Image.Height - 1)
                return new Vector(_Image.Width - 1, temp);

            throw new Exception("Line do not cross image");

        }
    }
}
