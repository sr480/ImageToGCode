using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class StrokesFromImageLinesGenerator
    {
        private List<ImageLine> _Lines;

        public List<FreeMotionStroke> Strokes { get; private set; }

        private double _IdleDistance;
        private bool _DoubleDirections;
        private bool _UseIdleZones;

        public StrokesFromImageLinesGenerator(List<ImageLine> lines, bool useIdleZones, double idleDistance, bool doubleDirections)
        {
            _Lines = lines;
            Strokes = new List<FreeMotionStroke>();

            _IdleDistance = idleDistance;
            _DoubleDirections = doubleDirections;
            _UseIdleZones = useIdleZones;
        }

        public void GenerateStrokes()
        {
            bool IsInverted = false;

            foreach (var line in _Lines)
            {
                if (line.Pixels.Count == 1 || line.Pixels.Count == 0)
                    continue;//throw new Exception("Пока не придумал что с этим делать");

                List<Pixel> pixels;
                if (_DoubleDirections && IsInverted)
                    pixels = ((IEnumerable<Pixel>)line.Pixels).Reverse().ToList(); //пока так..потом подумать
                else
                    pixels = line.Pixels;

                if (_UseIdleZones) //разгон
                {
                    Vector startPoint;

                    startPoint = pixels[0] + (pixels[0] - pixels[1]).Normalize() * _IdleDistance;

                    Strokes.Add(new FreeMotionStroke(startPoint));
                    Strokes.Add(new IdleStroke(pixels[0]));
                }
                else
                    Strokes.Add(new FreeMotionStroke(pixels[0]));

                Pixel startPixel = null;
                foreach (var pixel in line.Pixels)
                {
                    if (startPixel == null)
                        startPixel = pixel;
                    else
                    {
                        if (Math.Abs(pixel.Intensity - startPixel.Intensity) > SameIntensity)
                        {
                            Strokes.Add(new Stroke(pixel, 1 - startPixel.Intensity));
                            startPixel = pixel;
                        }
                    }
                }

                var lastPixel = line.Pixels[line.Pixels.Count - 1];
                if (startPixel != lastPixel)
                {
                    Strokes.Add(new Stroke(lastPixel, 1 - startPixel.Intensity));
                }

                var beforeLastPixel = line.Pixels[line.Pixels.Count - 2];
                var AddingVector = lastPixel - beforeLastPixel;
                var endPoint = lastPixel + AddingVector;
                
                //Strokes.Add(new Stroke(endPoint, 1 - lastPixel.Intensity));

                if (_UseIdleZones) //торможение
                    Strokes.Add(new IdleStroke(endPoint + (endPoint - lastPixel).Normalize() * _IdleDistance));

                IsInverted = !IsInverted;
            }
        }

        private const double SameIntensity = 0.02;
    }
}
