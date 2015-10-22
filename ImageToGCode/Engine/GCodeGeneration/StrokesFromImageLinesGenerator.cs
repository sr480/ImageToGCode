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

        public List<Stroke> Strokes { get; private set; }

        private double _IdleDistance;
        private bool _DoubleDirections;
        
        public StrokesFromImageLinesGenerator(List<ImageLine> lines, double idleDistance, bool doubleDirections)
        {
            _Lines = lines;
            Strokes = new List<Stroke>();

            _IdleDistance = idleDistance;
            _DoubleDirections = doubleDirections;
        }

        public void GenerateStrokes()
        {
            bool IsInverted = false;

            foreach (var line in _Lines)
            {
                if (line.Pixels.Count == 1 || line.Pixels.Count == 0)
                    throw new Exception("Пока не придумал что с этим делать");

                
                List<Pixel> pixels;
                if (_DoubleDirections && IsInverted)
                     pixels = ((IEnumerable<Pixel>)line.Pixels).Reverse().ToList(); //пока так..потом подумать
                else
                    pixels = line.Pixels;
                    
                Strokes.Add(new IdleStroke(pixels[0], pixels[1], _IdleDistance));//разгон

                Pixel startPixel = null;
                foreach (var pixel in line.Pixels)
                {
                    if (startPixel == null)
                        startPixel = pixel;
                    else
                    {
                        if(Math.Abs(pixel.Intensity - startPixel.Intensity) > SameIntensity)
                        {
                            Strokes.Add(new Stroke(startPixel, pixel, startPixel.Intensity));
                            startPixel = pixel;
                        }
                    }

                }

                var lastPixel = line.Pixels[line.Pixels.Count - 1];
                if (startPixel != lastPixel)
                {
                    Strokes.Add(new Stroke(startPixel, lastPixel, startPixel.Intensity));
                }

                var beforeLastPixel = line.Pixels[line.Pixels.Count - 2];
                var AddingVector = lastPixel - beforeLastPixel;
                var endPoint = lastPixel + AddingVector;
                Strokes.Add(new Stroke(lastPixel, endPoint, lastPixel.Intensity));

                Strokes.Add(new IdleStroke(endPoint, lastPixel, _IdleDistance));

                IsInverted = !IsInverted;
            }
        }

        private const double SameIntensity = 0.02;
    }
}
