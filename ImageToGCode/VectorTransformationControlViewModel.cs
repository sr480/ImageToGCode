using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode
{
    enum StartPoints
    {
        LeftTop,
        LeftBot,
        RightTop,
        RightBot
    }
    class VectorTransformationControlViewModel
    {
        public StartPoints StartPoint { get; set; }

    }
}
