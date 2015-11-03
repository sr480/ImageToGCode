using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor
{
    class SvgToGCodeRenderer : Svg.ISvgRenderer
    {
        private Stack<Svg.ISvgBoundable> _boundables;
        private SvgTransformator _transformator;

        Dictionary<Color, VPathGroup> _result;

        public SvgToGCodeRenderer()
        {
            _result = new Dictionary<Color, VPathGroup>();
            _transformator = new SvgTransformator();
            _boundables = new Stack<Svg.ISvgBoundable>();
        }

        public List<VPathGroup> GetResult()
        {
            return _result.Values.ToList();
        }

        public float DpiY
        {
            get { return 10; }
        }

        public void DrawImage(System.Drawing.Image image, System.Drawing.RectangleF destRect, System.Drawing.RectangleF srcRect, System.Drawing.GraphicsUnit graphicsUnit)
        {
            Console.WriteLine("TODD: DrawImage");//throw new NotImplementedException();
        }

        public void DrawImageUnscaled(System.Drawing.Image image, System.Drawing.Point location)
        {
            Console.WriteLine("TODD: DrawImageUnscaled");//throw new NotImplementedException();
        }

        public void DrawPath(System.Drawing.Pen pen, System.Drawing.Drawing2D.GraphicsPath path)
        {
            if (pen.Color != Color.Empty)
            {
                VPathGroup group = null;
                if (!_result.TryGetValue(pen.Color, out group))
                {
                    group = new VPathGroup(pen.Color);
                    _result.Add(pen.Color, group);
                }
                group.PathList.Add(_transformator.TransformPath(path));                
            }
        }

        public void FillPath(System.Drawing.Brush brush, System.Drawing.Drawing2D.GraphicsPath path)
        {
            //GCode.AddRange(_motionFactory.CreateMotion(path, Color.GreenYellow)); //Console.WriteLine("TODD: FillPath");//throw new NotImplementedException();
        }

        public Svg.ISvgBoundable GetBoundable()
        {
            return _boundables.Peek();
        }

        public System.Drawing.Region GetClip()
        {
            return _transformator.Clip;
        }

        public Svg.ISvgBoundable PopBoundable()
        {
            return _boundables.Pop();
        }

        public void RotateTransform(float fAngle, System.Drawing.Drawing2D.MatrixOrder order = System.Drawing.Drawing2D.MatrixOrder.Append)
        {
            _transformator.RotateTransform(fAngle);
        }

        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order = System.Drawing.Drawing2D.MatrixOrder.Append)
        {
            _transformator.ScaleTransform(sx, sy);
        }

        public void SetBoundable(Svg.ISvgBoundable boundable)
        {
            _boundables.Push(boundable);
        }

        public void SetClip(System.Drawing.Region region, System.Drawing.Drawing2D.CombineMode combineMode = System.Drawing.Drawing2D.CombineMode.Replace)
        {
            _transformator.Clip = region;
        }

        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get
            {
                return System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            set
            {
                ;
            }
        }

        public System.Drawing.Drawing2D.Matrix Transform
        {
            get
            {
                return _transformator.Transform;
            }
            set
            {
                _transformator.Transform = value;
            }
        }

        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order = System.Drawing.Drawing2D.MatrixOrder.Append)
        {
            _transformator.TranslateTransform(dx, dy);
        }

        public void Dispose()
        {
            ;
        }
    }
}
