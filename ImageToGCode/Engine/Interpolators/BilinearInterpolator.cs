using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.Interpolators
{
    /// <summary>
    /// Bilinear Interpolation engine
    /// </summary>
    class BilinearInterpolator : IInterpolator
    {
        public bool TryGetIntensity(Image image, double x, double y, out double intesity)
        {
            if (x > image.ImageWidth - 1 || y > image.ImageHeight - 1 || x < 0 || y < 0)
            {
                intesity = 0.0;
                return false;
            }
            
            int x0 = (int)x;
            int y0 = (int)y;

            Pixel p01 = image.GetImagePixel(x0, y0 + 1);
            Pixel p11 = image.GetImagePixel(x0 + 1, y0 + 1);
            Pixel p10 = image.GetImagePixel(x0 + 1, y0);
            Pixel p00 = image.GetImagePixel(x0, y0);

            if (p00 == null || p01 == null || p10 == null || p11 == null)
            {
                intesity = LinearInterpolate(p00 ?? p01 ?? p10 ?? p11, p11 ?? p10 ?? p01 ?? p00, x, y);
                return true;
            }

            var k1 = (x0 + 1 - x) * (y0 + 1 - y);
            var k2 = (x - x0) * (y0 + 1 - y);
            var k3 = (x0 + 1 - x) * (y - y0);
            var k4 = (x - x0) * (y - y0);

            var color = k1 * p00.Intensity + k2 * p10.Intensity + k3 * p01.Intensity + k4 * p11.Intensity;

            //var color = ((x0 + 1 - x) * (y0 + 1 - y)) * p00.Intensity + ((x - x0) * (y0 + 1 - y)) * p10.Intensity + ((x0 + 1 - x) * (y - y0)) * p01.Intensity + ((x - x0) * (y - y0)) * p11.Intensity;

            intesity = color;
            return true;
        }

        
        //эта штука нужна, если мы попали на границу нашей картинки. Мы не можем взять 4 точки для интерполяции, поэтому берём только две
        private double LinearInterpolate(Pixel first, Pixel second, double x, double y)
        {
            if (first == null || second == null)//если вдруг мы попали в угол картинки. У нас не будет даже двух точек - будет всего одна
                return (first ?? second).Intensity;

            if (first.Y == second.Y)
                return Math.Abs((second.X - x) * first.Intensity + (x - first.X) * second.Intensity);
            if (first.X == second.X)
                return Math.Abs((second.Y - y) * first.Intensity + (y - first.Y) * second.Intensity);

            throw new Exception("Somthing wrong :)");
        }




        public string Description
        {
            get { return "Билинейная интерполяция"; }
        }
    }
}
