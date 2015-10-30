using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.Geometry
{
    class Bezier
    {
        const double INTERPOLATOR_STEP_MM = 2;

        public Vector Start { get; private set; }
        public Vector End { get; private set; }
        public Vector PStart { get; private set; }
        public Vector PEnd { get; private set; }

        public Bezier(Vector start, Vector end, Vector pStart, Vector pEnd)
        {
            Start = start;
            End = end;
            PStart = pStart;
            PEnd = pEnd;
        }


        public IEnumerable<Vector> GetInterpolatedPoints()
        {
            int steps = GetIteratorSteps();
            
            for(int i = 0; i <= steps; i++)
                yield return GetPoint(1.0 / steps * i);
        }

        private Vector GetPoint(double pos)
        {
            double pos1 = 1 - pos;
            double pos13 = pos1 * pos1 * pos1;
            double pos3 = pos * pos * pos;

            double x = pos13 * Start.X + 3 * pos * pos1 * pos1 * PStart.X + 3 * pos * pos * pos1 * PEnd.X + pos3 * End.X;
            double y = pos13 * Start.Y + 3 * pos * pos1 * pos1 * PStart.Y + 3 * pos * pos * pos1 * PEnd.Y + pos3 * End.Y;
            return new Vector(x, y);
        }

        private int GetIteratorSteps()
        {
            double len = (Vector.GetLength(Start, PStart) + Vector.GetLength(PStart, PEnd) + Vector.GetLength(PEnd, End))*0.75;
            return (int)(len / INTERPOLATOR_STEP_MM);
        }
    }
}
