using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.Interpolators
{
    class StepInterpolator : IInterpolator
    {
        public bool TryGetIntensity(Image image, double x, double y, out double intesity)
        {
            if (x > image.ImageWidth - 1 || y > image.ImageHeight - 1 || x < 0 || y < 0)
            {
                intesity = 0.0;
                return false;
            }
            intesity = image.GetImagePixel((int)Math.Round(x), (int)Math.Round(y)).Intensity;
            return true;
        }



        public string Description
        {
            get { return "Метод ближайшего соседа"; }
        }
    }
}
