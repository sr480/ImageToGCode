using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.Interpolators
{
    /// <summary>
    /// Bilinear Interpolation engine
    /// </summary>
    class BilinearInterpolator : IInterpolator
    {
        public Pixel GetPixel(Image image, Vector position)
        {
            if (position.X > image.Width - 1 || position.Y > image.Height - 1 || position.X < 0 || position.Y < 0)
                //throw new Exception("Point is out of dimension");
                return null;

            
            int x0 = (int)position.X;
            int y0 = (int)position.Y;

            Pixel p01 = image.GetPixel(x0, y0 + 1);
            Pixel p11 = image.GetPixel(x0 + 1, y0 + 1);
            Pixel p10 = image.GetPixel(x0 + 1, y0);
            Pixel p00 = image.GetPixel(x0, y0);

            if(p00==null || p01==null || p10==null || p11 == null)
                return LinearInterpolate(p00 ?? p01 ?? p10 ?? p11, p11 ?? p10 ?? p01 ?? p00, position);

            var k1 = (x0 + 1 - position.X) * (y0 + 1 - position.Y);
            var k2 = (position.X - x0) * (y0 + 1 - position.Y);
            var k3 = (x0 + 1 - position.X) * (position.Y - y0);
            var k4 = (position.X - x0) * (position.Y - y0);

            var color = k1 * p00.Intensity + k2 * p10.Intensity + k3 * p01.Intensity + k4 * p11.Intensity;

            //var color = ((x0 + 1 - position.X) * (y0 + 1 - position.Y)) * p00.Intensity + ((position.X - x0) * (y0 + 1 - position.Y)) * p10.Intensity + ((x0 + 1 - position.X) * (position.Y - y0)) * p01.Intensity + ((position.X - x0) * (position.Y - y0)) * p11.Intensity;


            return new Pixel(color, position.X, position.Y);
        }

        
        //эта штука нужна, если мы попали на границу нашей картинки. Мы не можем взять 4 точки для интерполяции, поэтому берём только две
        private Pixel LinearInterpolate(Pixel first, Pixel second, Vector position)
        {
            var result = new Pixel(position.X, position.Y);

            
            if (first == null || second == null)//если вдруг мы попали в угол картинки. У нас не будет даже двух точек - будет всего одна
            {
                result.Intensity = (first ?? second).Intensity;
                return result;
            }
            
            

            if (first.Y == second.Y)
            {
                result.Intensity = (second.X - position.X) * first.Intensity + (position.X - first.X) * second.Intensity;
            }
            else if (first.X == second.X)
                result.Intensity = (second.Y - position.Y) * first.Intensity + (position.Y - first.Y) * second.Intensity;

            if (result.Intensity < 0)
                result.Intensity *= -1;

            return result;
        }


    }
}
