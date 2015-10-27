using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.Interpolators
{
    static class InterpolateHelper
    {
        //пока так...
        private static readonly IInterpolator _instance;
        public static IInterpolator CurrentInterpolator { get { return _instance; } }

        static InterpolateHelper()
        {
            _instance = new BilinearInterpolator();
        }

    }
}
