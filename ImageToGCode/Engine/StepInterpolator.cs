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
                return null;

            return new Pixel(image.GetPixel((int)Math.Round(position.X), (int)Math.Round(position.Y)).Intensity, position);
        }

    }
}
