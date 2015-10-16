using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class Image
    {
        Pixel[,] _pixels;

        public int Width { get { return _pixels.GetLength(0); } }
        public int Height { get { return _pixels.GetLength(1); } }

        public Image(int width, int height)
        {
            _pixels = new Pixel[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _pixels[x, y] = new Pixel(x, y);
        }
        public Image(System.Drawing.Bitmap bitmap)
        {
            _pixels = new Pixel[bitmap.Width, bitmap.Height];

            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    _pixels[x, y] = new Pixel(bitmap.GetPixel(x, y).GetBrightness(), x, y);
        }

        public Pixel GetPixel(int x, int y)
        {
            if (x < 0 | y < 0 | x > Width | y > Height)
                return null;
            return _pixels[x, y];
        }

        //public Pixel GetPixel(double x, double y)
        //{ return GetPixel(new Vector(x, y)); }
        //public Pixel GetPixel(Vector dest)
        //{
        //    if (dest.X > Width - 1 || dest.Y > Height - 1 || dest.X < 0 || dest.Y < 0)
        //        throw new Exception("Point is out of dimension");

        //    Pixel p1 = _points[(int)dest.X, (int)dest.Y + 1];
        //    Pixel p2 = _points[(int)dest.X + 1, (int)dest.Y + 1];
        //    Pixel p3 = _points[(int)dest.X + 1, (int)dest.Y];
        //    Pixel p4 = _points[(int)dest.X, (int)dest.Y];

        //    var interpolator = new BilinearInterpolator();

        //    return interpolator.Interpolate(p1, p2, p3, p4, dest);
        //}
    }
}
