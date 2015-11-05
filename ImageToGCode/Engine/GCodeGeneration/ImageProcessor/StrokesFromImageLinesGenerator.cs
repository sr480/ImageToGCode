using ImageToGCode.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageToGCode.Engine.GCodeGeneration.ImageProcessor
{
    class StrokesFromImageLinesGenerator
    {
        private const double SAME_INTENSITY = 0.1;
        private List<ImageLine> _Lines;

        public List<FreeMotionStroke> Strokes { get; private set; }

        public double IdleDistance
        {
            get
            {
                return _IdleDistance;
            }
        }

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
            bool isReversed = false;

            foreach (var line in _Lines)
            {
                if (line.Pixels.Count == 1 || line.Pixels.Count == 0)
                    continue;//throw new Exception("Пока не придумал что с этим делать");

                LinkedList<FreeMotionStroke> lineStrokes = new LinkedList<FreeMotionStroke>();

                Pixel firstDarkPixel = null;
                Pixel strokeBegining = null;

                List<double> strokeIntensities = new List<double>();

                foreach (var curPixel in line.Pixels)
                {
                    if (strokeBegining == null)
                    {
                        if (curPixel.Intensity != 1)
                        {
                            strokeBegining = curPixel;
                            if (firstDarkPixel == null)
                                firstDarkPixel = strokeBegining;
                            strokeIntensities.Clear();
                        }
                    }
                    else if (Math.Abs(strokeBegining.Intensity - curPixel.Intensity) > SAME_INTENSITY)
                    {
                        AddStroke(lineStrokes, isReversed, strokeBegining, curPixel, 1 - strokeIntensities.Average());
                        strokeBegining = curPixel;

                        strokeIntensities.Clear();
                    }
                    
                    strokeIntensities.Add(curPixel.Intensity);
                }


                if (strokeBegining == null) //Выходим, так как строка вся белая
                    continue;

                Pixel lastPixel = line.Pixels[line.Pixels.Count - 1];

                if (strokeBegining.Intensity == 1)
                    lastPixel = strokeBegining;
                else if(lastPixel != strokeBegining)
                    AddStroke(lineStrokes, isReversed, strokeBegining, lastPixel, 1 - strokeIntensities.Average());
                

                if (isReversed)
                {
                    if (_UseIdleZones)
                    {
                        lineStrokes.AddFirst(new IdleStroke(lastPixel));
                        lineStrokes.AddLast(new IdleStroke(firstDarkPixel + (firstDarkPixel - lastPixel).Normalize() * _IdleDistance));
                        lineStrokes.AddFirst(new FreeMotionStroke(lastPixel + (lastPixel - firstDarkPixel).Normalize() * _IdleDistance));
                    }
                    else
                        lineStrokes.AddFirst(new FreeMotionStroke(lastPixel));
                }
                else
                {
                    if (_UseIdleZones)
                    {
                        lineStrokes.AddFirst(new IdleStroke(firstDarkPixel));
                        lineStrokes.AddLast(new IdleStroke(lastPixel + (lastPixel - firstDarkPixel).Normalize() * _IdleDistance));
                        lineStrokes.AddFirst(new FreeMotionStroke(firstDarkPixel + (firstDarkPixel - lastPixel).Normalize() * _IdleDistance));
                    }
                    else
                        lineStrokes.AddFirst(new FreeMotionStroke(firstDarkPixel));
                }

                Strokes.AddRange(lineStrokes);


                if (_DoubleDirections)
                    isReversed = !isReversed;

            }
        }

        private void AddStroke(LinkedList<FreeMotionStroke> line, bool isReversed, Pixel strokeBegining, Pixel strokeEnd, double intensity)
        {
            if (isReversed)
                line.AddFirst(new Stroke(strokeBegining, intensity));
            else
                line.AddLast(new Stroke(strokeEnd, intensity));
        }


    }
}
