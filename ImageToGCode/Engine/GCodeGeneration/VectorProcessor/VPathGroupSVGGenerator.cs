using ImageToGCode.Engine.GCodeGeneration.ImageProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor
{
    class VPathGroupSVGGenerator
    {
        private readonly Svg.SvgDocument _Svg;

        public VPathGroupSVGGenerator(Svg.SvgDocument svg)
        {
            _Svg = svg;
        }


        public List<VPathGroup> GenerateVPathGroups()
        {
            SvgToVPathRenderer gr = new SvgToVPathRenderer(_Svg.Ppi/25.4);
            
            _Svg.Draw(gr);

            return gr.GetResult();
        }
    }
}
