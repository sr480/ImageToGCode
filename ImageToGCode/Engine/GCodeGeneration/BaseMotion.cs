using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration
{
    abstract class BaseMotion : BaseGCode
    {
        public Vector Position { get; private set; }

        public BaseMotion(Vector position) : base("")
        {
            Position = position;
        }

        protected string DoubleToStr(double value)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            return value.ToString("0.###", nfi);
        }
    }
}
