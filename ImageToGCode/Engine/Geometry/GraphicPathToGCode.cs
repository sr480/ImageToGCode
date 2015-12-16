using ImageToGCode.Engine.GCodeGeneration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.Geometry
{
    class GraphicsPathToGCode
    {
        public static IEnumerable<GCodeGeneration.BaseMotion> Generate(PointF startpoint, PointF c1, PointF c2, PointF endPoint)
        {



            return null;
            //PointF? startPoint = null;
            //var curPathData = pth.PathData;

            //for (int i = 0; i < curPathData.Points.Count(); i++)
            //{
            //    var curPthType = curPathData.Types[i];
            //    var curPoint = curPathData.Points[i];

            //    //Rapid move to path start
            //    if (Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.Start))
            //    {
            //        yield return new RapidMotion(new Geometry.Vector(curPoint));
            //        startPoint = curPoint;
            //    }
            //    else if (Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.Line) ||
            //        Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.Bezier))
            //    {
            //        //Move to point on path
            //        yield return new CoordinatMotion(new Geometry.Vector(curPoint), Power, Feed, PathColor);

            //        //Close path
            //        if (Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.CloseSubpath))
            //        {
            //            yield return new CoordinatMotion(new Geometry.Vector(startPoint.Value), Power, Feed, PathColor);
            //        }
            //    }
            //}
        }
        //IEnumerable<BaseMotion> recursive_bezier(float x1, float y1,
        //       float x2, float y2,
        //       float x3, float y3,
        //       float x4, float y4)
        //{
        //    // Вычислить все средние точек отрезков
        //    //----------------------
        //    float x12 = (x1 + x2) / 2;
        //    float y12 = (y1 + y2) / 2;
        //    float x23 = (x2 + x3) / 2;
        //    float y23 = (y2 + y3) / 2;
        //    float x34 = (x3 + x4) / 2;
        //    float y34 = (y3 + y4) / 2;
        //    float x123 = (x12 + x23) / 2;
        //    float y123 = (y12 + y23) / 2;
        //    float x234 = (x23 + x34) / 2;
        //    float y234 = (y23 + y34) / 2;
        //    float x1234 = (x123 + x234) / 2;
        //    float y1234 = (y123 + y234) / 2;

        //    //if (curve_is_flat)
        //    //{
        //    //    yield return new ArcMotion(new Vector(x4, y4), 0.0, 100, 400);
        //    //}
        //    //else
        //    //{
        //    //    // Продолжить деление
        //    //    //----------------------
        //    //    recursive_bezier(x1, y1, x12, y12, x123, y123, x1234, y1234);
        //    //    recursive_bezier(x1234, y1234, x234, y234, x34, y34, x4, y4);
        //    //}
        //}
    }
}
