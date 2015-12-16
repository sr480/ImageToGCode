using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor
{
    class SvgTransformator 
    {
        public Region Clip { get; set; }
        Matrix _Transform;

        public Matrix Transform
        {
            get { return _Transform; }
            set { _Transform = value; }
        }

        public SvgTransformator(double ppm)
        {
            Clip = new Region(new Rectangle(0, 0, 1200, 1200));
            _Transform = new Matrix((float)(1/ppm), 0, 0, -(float)(1/ppm), 0, 297);
        }

        public GraphicsPath TransformPath(GraphicsPath path)
        {
            var p = (GraphicsPath)path.Clone();            
            p.Flatten(_Transform, 0.1f);
            return p;
        }
        public void RotateTransform(float angle)
        {
            _Transform.Rotate(angle);
        }
        public void ScaleTransform(float scaleX, float scaleY)
        {
            _Transform.Scale(scaleX, scaleY);
        }
        public void TranslateTransform(float dx, float dy)
        {
            _Transform.Translate(dx, dy);
        }
    }
}
