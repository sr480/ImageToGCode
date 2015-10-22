using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.Visualisers
{
    class BitmapExtensions
    {
        Bitmap _bitmap;
        public Bitmap Bitmap { get { return _bitmap; } }
        //Dictionary<int, Dictionary<int, List<Pixel>>> groupedPixels = new Dictionary<int, Dictionary<int, List<Pixel>>>();

        public BitmapExtensions(Bitmap bm)
        {
            _bitmap = bm;
        }

        public void SetPixel(int x, double y, Color color)
        {
            if (x > _bitmap.Width - 1 | y > _bitmap.Height - 1)
                throw new Exception("Point out of Dimensions");

            if(y == (int)y)
            {
                Color p = _bitmap.GetPixel(x, (int)y);
                p = Color.FromArgb(
                Mix(p.R, color.R, 1),
                Mix(p.G, color.G, 1),
                Mix(p.B, color.B, 1));
                _bitmap.SetPixel(x, (int)y, p);
                return;
            }

            Color p1 = _bitmap.GetPixel(x, (int)y);
            Color p2 = _bitmap.GetPixel(x, (int)y + 1);

            double k = y - (int)y;

            p1 = Color.FromArgb(
                Mix(p1.R, color.R, k),
                Mix(p1.G, color.G, k),
                Mix(p1.B, color.B, k));
            p2 = Color.FromArgb(
                Mix(p2.R, color.R, 1-k),
                Mix(p2.G, color.G, 1-k),
                Mix(p2.B, color.B, 1-k));

            _bitmap.SetPixel(x, (int)y, p1);
            _bitmap.SetPixel(x, (int)y + 1, p2);
        }
        private byte Mix(byte native, byte mixed, double k)
        {
            return (byte)Math.Round((native + mixed * k) / 2.0);
        }
    }
}
