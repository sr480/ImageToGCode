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
        Interpolators.IInterpolator _interpolator;
        public double Width { get; private set; }
        public double Height { get; private set; }
        public int ImageWidth { get { return _pixels.GetLength(0); } }
        public int ImageHeight { get { return _pixels.GetLength(1); } }
        public double ScaleHorizontal { get; private set; }
        public double ScaleVertical { get; private set; }
        //public Image(int width, int height)
        //{
        //    _pixels = new Pixel[width, height];

        //    for (int x = 0; x < width; x++)
        //        for (int y = 0; y < height; y++)
        //            _pixels[x, y] = new Pixel(x, y);
        //}
        public Image(double width, double height, System.Drawing.Bitmap bitmap, Interpolators.IInterpolator interpolator)
        {
            if (interpolator == null)
                throw new ArgumentNullException("interpolator", "interpolator is null.");
            if (bitmap == null)
                throw new ArgumentNullException("bitmap", "bitmap is null.");
            if (Width < 0)
                throw new Exception("Width can't be negative");
            if (Height < 0)
                throw new Exception("Height can't be negative");

            Width = width;
            Height = height;
            _interpolator = interpolator;

            ScaleHorizontal = (bitmap.Width - 1) / Width;
            ScaleVertical = (bitmap.Height - 1) / Height;

            _pixels = new Pixel[bitmap.Width, bitmap.Height];

            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    _pixels[x, y] = new Pixel(bitmap.GetPixel(x, bitmap.Height - y - 1).GetBrightness(), x, y);
        }

        public Pixel GetPixel(double x, double y)
        {
            double intens;
            if(!_interpolator.TryGetIntensity(this, x * ScaleHorizontal, y * ScaleVertical, out intens))
                return null;
            
            return new Pixel(intens, x, y);
        }
        public Pixel GetImagePixel(int x, int y)
        {
            if (x < 0 || y < 0 || x > ImageWidth - 1 || y > ImageHeight - 1)
                return null;
            return _pixels[x, y];
        }
    }
}
