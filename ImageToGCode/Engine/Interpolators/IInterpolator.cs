using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.Interpolators
{
    interface IInterpolator
    {
        Pixel GetPixel(Image image, Vector position);
    }
}
