using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
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

            Pixel p01 = image.GetPixel((int)position.X, (int)position.Y + 1);
            Pixel p11 = image.GetPixel((int)position.X + 1, (int)position.Y + 1);
            Pixel p10 = image.GetPixel((int)position.X + 1, (int)position.Y);
            Pixel p00 = image.GetPixel((int)position.X, (int)position.Y);

            if(p00==null || p01==null || p10==null || p11 == null)
                return LinearInterpolate(p00 ?? p01 ?? p10 ?? p11, p11 ?? p10 ?? p01 ?? p00, position);

            var k1 = ((int)position.X + 1 - position.X) * ((int)position.Y + 1 - position.Y);
            var k2 = (position.X - (int)position.X) * ((int)position.Y + 1 - position.Y);
            var k3 = ((int)position.X + 1 - position.X) * (position.Y - (int)position.Y);
            var k4 = (position.X - (int)position.X) * (position.Y - (int)position.Y);

            var color = k1 * p00.Intensity + k2 * p10.Intensity + k3 * p01.Intensity + k4 * p11.Intensity;


            return new Pixel(color, position.X, position.Y);
        }

        private Pixel LinearInterpolate(Pixel first, Pixel second, Vector position)
        {
            var result = new Pixel(position.X, position.Y);

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
