using ImageToGCode.Engine.Geometry;
using ImageToGCode.Engine.Interpolators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class ImageLine : Line
    {
        private Image _Image;

        private List<Pixel> _Pixels;
        public List<Pixel> Pixels
        {
            get
            {
                return _Pixels;
            }
        }

        //конструктор прямой через вектор нормали и точку
        public ImageLine(Vector normalVector, Vector pointToCross, Image image)
            :base(normalVector, pointToCross)
        {
            _Pixels = new List<Pixel>();
            _Image = image;
        }



        //контструктор прямой через вектор нормали. А координаты точки, через которую проходит прямая, совпадают с координатами вектора нормали
        public ImageLine(Vector normalVector, Image image)
            : this(normalVector, normalVector, image)
        {   }

        public void GeneratePixels(double pointResolution)
        {
            var directionVector = new Vector(-B, A).Normalize() * pointResolution; // _NormalVector.Normalize().Rotate90CCW() * pointResolution; 
            if (directionVector.X < 0)
                directionVector = directionVector.Reverse();

            var currentVector = GetFirstVector();
            
            IInterpolator inter = InterpolateHelper.CurrentInterpolator;

            //наполняем пикселями
            Pixel temp = inter.GetPixel(_Image, currentVector);
            while (temp != null)
            {
                _Pixels.Add(temp);
                currentVector += directionVector;
                temp = inter.GetPixel(_Image, currentVector);
            }
        }

        private Vector GetFirstVector()
        {
            double x = 0.0;
            double y = GetY(x);


            if (y > _Image.Height - 1)
            {
                y = _Image.Height - 1;
                x = GetX(y);
            }
            else if (y < 0)
            {
                y = 0;
                x = GetX(y);
            }

            return new Vector(x, y);



            /*double temp;

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

            throw new Exception("Line do not cross image");*/

        }
    }
}
