using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class MotionFactorySvg : MotionFactory
    {
        public System.Drawing.Region Clip { get; set; }
        Matrix _Transform;

        public Matrix Transform
        {
            get { return _Transform; }
            set { _Transform = value; }
        }

        public MotionFactorySvg(int minFeed, int maxFeed, int maxPower, int minPower)
            : base(minFeed, maxFeed, maxPower, minPower)
        {
            Clip = new System.Drawing.Region(new System.Drawing.Rectangle(0, 0, 1200, 1200));
            _Transform = new Matrix(0.5f, 0, 0, -0.5f, 0, 900);
        }


        public IEnumerable<BaseGCode> CreateMotion(GraphicsPath path)
        {
            var result = new List<BaseGCode>(1);

            path.Flatten(_Transform, 1f);
            
            for (int i = 0; i < path.PathData.Points.Count(); i++)
            {
                if(path.PathData.Types[i] == 0)
                    result.Add(new RapidMotion(new Vector(path.PathData.Points[i])));
                else
                    result.Add(new CoordinatMotion(new Vector(path.PathData.Points[i]), _MaxPower, _MaxFeed));
            }
            return result;
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
