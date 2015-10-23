using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine
{
    class ImageProcessor
    {
        private readonly Image _image;
        private readonly double _Width;
        private readonly double _Height;
        private readonly double _LineRes;
        private readonly double _PointRes;
        private readonly double _Angle;

        public ImageProcessor(System.Drawing.Bitmap bitmap, double width, double height, double lineRes, double pointRes, double angle, Interpolators.IInterpolator interpolator)
        {
            if (width < 0)
                throw new Exception("Width can not be negative");
            if (height < 0)
                throw new Exception("Height can not be negative");
            if (lineRes < 0)
                throw new Exception("lineRes can not be negative");
            if (pointRes < 0)
                throw new Exception("pointRes can not be negative");

            _Angle = angle;
            _PointRes = pointRes;
            _LineRes = lineRes;
            _Height = height;
            _Width = width;
            _image = new Image(width, height, bitmap, interpolator);
        }

        public ImageByLinesPresenter CreatePresenter()
        {
            var ip = new ImageByLinesPresenter(_image);

            var angleInRadian = AngleToVector.DegToRad(_Angle);
            var mathVector = AngleToVector.GetNormal(angleInRadian);
            var resultingVector = new Vector(Math.Round(mathVector.X, 6), Math.Round(mathVector.Y, 6));

            ip.Present(resultingVector, _LineRes, _PointRes);

            return ip;
        }
    }
}
