using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class SvgToGCodeRenderer : Svg.ISvgRenderer
    {
        private Stack<Svg.ISvgBoundable> _boundables = new Stack<Svg.ISvgBoundable>();

        public List<BaseGCode> GCode;
        private MotionFactorySvg _motionFactory;
        public SvgToGCodeRenderer(MotionFactorySvg mf)
        {
            GCode = new List<BaseGCode>();
            _motionFactory = mf;
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
                Console.WriteLine(pen);
                GCode.AddRange(_motionFactory.CreateMotion(path, pen.Color));
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
            return _motionFactory.Clip;
        }

        public Svg.ISvgBoundable PopBoundable()
        {
            return _boundables.Pop();
        }

        public void RotateTransform(float fAngle, System.Drawing.Drawing2D.MatrixOrder order = System.Drawing.Drawing2D.MatrixOrder.Append)
        {
            _motionFactory.RotateTransform(fAngle);
        }

        public void ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order = System.Drawing.Drawing2D.MatrixOrder.Append)
        {
            _motionFactory.ScaleTransform(sx, sy);
        }

        public void SetBoundable(Svg.ISvgBoundable boundable)
        {
            _boundables.Push(boundable);
        }

        public void SetClip(System.Drawing.Region region, System.Drawing.Drawing2D.CombineMode combineMode = System.Drawing.Drawing2D.CombineMode.Replace)
        {
            _motionFactory.Clip = region;
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
                return _motionFactory.Transform;
            }
            set
            {
                _motionFactory.Transform = value;
            }
        }

        public void TranslateTransform(float dx, float dy, System.Drawing.Drawing2D.MatrixOrder order = System.Drawing.Drawing2D.MatrixOrder.Append)
        {
            _motionFactory.TranslateTransform(dx, dy);
        }

        public void Dispose()
        {
            ;
        }
    }
}
