using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class GCodeTrajectoryCounter
    {
        public double Rapid { get; private set; }
        public double Feed { get; private set; }
        public TimeSpan EstimatedTime { get; private set; }
        private IEnumerable<BaseGCode> _gcode;
        public GCodeTrajectoryCounter(IEnumerable<BaseGCode> gcode)
        {
            _gcode = gcode;
        }

        public void Count()
        {
            Rapid = 0;
            Feed = 0;
            double duration_min = 0.0;

            BaseMotion prevMotion = null;
            foreach(var gc in _gcode)
            {
                if (!(gc is BaseMotion))
                    continue;
                BaseMotion curMotion = (BaseMotion)gc;

                if (prevMotion != null)
                {
                    double distance = (curMotion.Position - prevMotion.Position).Length;
                    if (curMotion is CoordinatMotion)
                    {
                        Feed += distance;
                        duration_min += distance / ((CoordinatMotion)curMotion).Feed;
                    }
                    else
                    {
                        Rapid += distance;
                        duration_min += distance / 20000;
                    }
                }
                prevMotion = curMotion;
            }
            EstimatedTime = TimeSpan.FromMinutes(duration_min);
        }
    }
}
