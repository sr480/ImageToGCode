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
                throw new Exception("Point is out of dimension");

            Pixel p1 = image.GetPixel((int)position.X, (int)position.Y + 1);
            Pixel p2 = image.GetPixel((int)position.X + 1, (int)position.Y + 1);
            Pixel p3 = image.GetPixel((int)position.X + 1, (int)position.Y);
            Pixel p4 = image.GetPixel((int)position.X, (int)position.Y);

            return Interpolate(p1, p2, p3, p4, position);
        }

        public Pixel Interpolate(Pixel lt, Pixel rt, Pixel rb, Pixel lb, Vector dest)
        {
            if (lt.Y != rt.Y)
                throw new Exception("lt and rt must be on one horizont line");
            if (lb.Y != rb.Y)
                throw new Exception("lb and rb must be on one horizont line");
            if (lt.X != lb.X)
                throw new Exception("lt and lb must be on one vertical line");
            if (rt.X != rb.X)
                throw new Exception("rt and rb must be on one vertical line");

            if (lt.X > rt.X)
                throw new Exception("left and right pixels are swaped");
            if (lb.Y > lt.Y)
                throw new Exception("top and bottom pixels are swaped");

            if (dest.X > rt.X | dest.X < lt.X | dest.Y > rt.Y | dest.Y < rb.Y)
                throw new Exception("dest is not between points");

            return GetIntermediateVertical(GetIntermediateHorizontal(lt, rt, dest), GetIntermediateHorizontal(lb, rb, dest), dest);
        }

        private Pixel GetIntermediateHorizontal(Pixel p1, Pixel p2, Vector intermed)
        {
            if (p1.Y != p2.Y)
                throw new Exception("Pixels must be on one horizontal line");

            if (!((p1.X < intermed.X & p2.X > intermed.X) | (p2.X < intermed.X & p1.X > intermed.X)))
                throw new Exception("intermed is not between p1 and p2");

            double k1 = (p2.X - intermed.X) / (p2.X - p1.X);
            double k2 = (intermed.X - p1.X) / (p2.X - p1.X);

            double intencity = k1 * p1.Intensity + k2 * p2.Intensity;
            return new Pixel(intencity, intermed.X, p1.Y);
        }
        private Pixel GetIntermediateVertical(Pixel p1, Pixel p2, Vector intermed)
        {
            if (p1.X != p2.X)
                throw new Exception("Pixels must be on one vertical line");

            if (!((p1.Y < intermed.Y & p2.Y > intermed.Y) | (p2.Y < intermed.Y & p1.Y > intermed.Y)))
                throw new Exception("intermed is not between p1 and p2");

            double k1 = (p2.Y - intermed.Y) / (p2.Y - p1.Y);
            double k2 = (intermed.X - p1.Y) / (p2.Y - p1.Y);

            double intencity = k1 * p1.Intensity + k2 * p2.Intensity;
            return new Pixel(intencity, intermed.X, intermed.Y);
        }
    }
}
