using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.Geometry
{
    class PathTypeHelper
    {
        public static bool IsSet(byte pthType, System.Drawing.Drawing2D.PathPointType flag)
        {
            if (flag == 0)
                return pthType == 0;
            return (((byte)pthType) & ((byte)flag)) == ((byte)flag);
        }
    }
}
