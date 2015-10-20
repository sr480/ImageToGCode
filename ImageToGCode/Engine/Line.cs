using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class Line
    {
        //прямая Ax+By+C=0
        private double A;
        private double B;
        private double C;



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

        public double GetX(double Y)
        {
            return (-C - B * Y) / A;
        }

        public double GetY(double X)
        {
            return (-C - A * X) / B;
        }

        Line(Image image)
        {
            _Pixels = new List<Pixel>();
            _Image = image;
        }

        //конструктор прямой через вектор нормали и точку
        public Line(Vector normalVector, Vector pointToCross, Image image)
            : this(image)
        {
            _NormalVector = normalVector;

            A = _NormalVector.X;
            B = _NormalVector.Y;

            C = -(A * pointToCross.X + B * pointToCross.Y);
        }

        //контструктор прямой через вектор нормали. А координаты точки совпадают с координатами вектора нормали
        public Line(Vector normalVector, Image image)
            : this(image)
        {
            _NormalVector = normalVector;

            A = _NormalVector.X;
            B = _NormalVector.Y;
            C = -(A * A + B * B);
        }

        public void GeneratePixels(double pointResolution)
        {
            //var currentVector = GetFirstVector();

            //направляющий вектор берётся по двум точкам, лежащим на прямой
            //var directionVector = (currentVector - _NormalVector).Normalize()  * pointResolution ;
            var directionVector = _NormalVector.Normalize().Rotate90CCW() * pointResolution; //new Vector(-B,A).Normalize() * pointResolution;
            if (directionVector.X < 0)
                directionVector = directionVector.Reverse();

            double x = 0.0;
            double y = GetY(x);
            

            if (y > _Image.Height - 1)
            {
                y = _Image.Height - 1;
                x = GetX(y);
            }
            else if(y < 0)
            {
                y = 0;
                x = GetX(y);
            }

            var currentVector = new Vector(x, y);
            //пока так
            //IInterpolator inter = new BilinearInterpolator();
            IInterpolator inter = new StepInterpolator();

            //если одно прибавление направляющего вектора выходит за рамки картинки, то меняем направление направляющего вектора
            /*if (inter.GetPixel(_Image, currentVector + directionVector) == null)
                directionVector = directionVector.Reverse();*/

            //наполняем пикселями
            Pixel temp = inter.GetPixel(_Image, currentVector);
            while (temp != null)
            {
                _Pixels.Add(temp);
                currentVector += directionVector;
                temp = inter.GetPixel(_Image, currentVector);
            }

            if(_Pixels.Count == 0)
                Console.WriteLine(  );
        }

        private Vector GetFirstVector()
        {

            double temp;

            //пересечение c OX, y = 0
            temp = -C / A;
            if (0 <= temp && temp <= _Image.Width - 1)
                return new Vector(temp, 0);

            //пересечение с OY, x = 0
            temp = -C / B;
            if (0 <= temp && temp <= _Image.Height - 1)
                return new Vector(0, temp);

            //пересечение с y=max
            temp = (-C - B * (_Image.Height - 1)) / A;
            if (0 <= temp && temp <= _Image.Width - 1)
                return new Vector(temp, _Image.Height - 1);

            //пересечение с x=max
            temp = (-C - A * (_Image.Width - 1)) / B;
            if (0 <= temp && temp <= _Image.Height - 1)
                return new Vector(_Image.Width - 1, temp);

            throw new Exception("Line do not cross image");

        }

        public static Vector GetIntersection(Line l1, Line l2)
        {
            return new Vector(-(l1.C * l2.B - l2.C * l1.B) / (l1.A * l2.B - l2.A * l1.B), -(l1.A * l2.C - l2.A * l1.C) / (l1.A * l2.B - l2.A * l1.B));
        }
    }
}
