using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.Interpolators
{
    interface IInterpolator
    {
        bool TryGetIntensity(Image image, double x, double y, out double intensity);
        string Description { get; }
    }
}
