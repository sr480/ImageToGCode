using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class StepInterpolator : IInterpolator
    {
        public Pixel GetPixel(Image image, Vector position)
        {
            if (position.X > image.Width - 1 || position.Y > image.Height - 1 || position.X < 0 || position.Y < 0)
                throw new Exception("Point is out of image dimension");

            List<Pixel> neighbors = new List<Pixel>();
            neighbors.Add(image.GetPixel((int)position.X, (int)position.Y + 1));
            neighbors.Add(image.GetPixel((int)position.X + 1, (int)position.Y + 1));
            neighbors.Add(image.GetPixel((int)position.X + 1, (int)position.Y));
            neighbors.Add(image.GetPixel((int)position.X, (int)position.Y));

            Pixel closest = neighbors.OrderBy(p => (p - position).Length).First();

            return new Pixel(closest.Intensity, position);
        }
    }
}
