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

            

            if (!useIdleZones)
                _IdleDistance = 0.0;
            else
                _IdleDistance = idleDistance;

            _DoubleDirections = doubleDirections;
            _UseIdleZones = useIdleZones;
        }

        public void GenerateStrokesNEW()
        {
            bool isReversed = false;

            foreach (var line in _Lines)
            {
                if (line.Pixels.Count == 1 || line.Pixels.Count == 0)
                    continue;//throw new Exception("Пока не придумал что с этим делать");

                LinkedList<FreeMotionStroke> LineStrokes = new LinkedList<FreeMotionStroke>();


                Pixel startPrintingPixel = null;
                Pixel startPixel = null;
                foreach (var pixel in line.Pixels)//цвет штриха всегда вычисляем в прямом порядке
                {
                    if (startPixel == null)
                    {
                        if (pixel.Intensity != 1)
                        {
                            startPixel = pixel;
                            if (startPrintingPixel == null)
                                startPrintingPixel = startPixel;
                        }
                    }
                    else if (Math.Abs(pixel.Intensity - startPixel.Intensity) > SAME_INTENSITY)
                    {
                        if (_DoubleDirections && isReversed)
                            LineStrokes.AddLast(new Stroke(startPixel, 1 - startPixel.Intensity));
                        else
                            LineStrokes.AddLast(new Stroke(pixel, 1 - startPixel.Intensity));

                        startPixel = pixel;
                    }
                }



                Pixel lastPixel = null;
                if (startPixel != null)
                {
                    lastPixel = line.Pixels[line.Pixels.Count - 1];
                    if (startPixel.Intensity == 1)
                        lastPixel = startPixel;
                    
                    if (startPixel != lastPixel)
                    {
                        if (_DoubleDirections && isReversed)
                            LineStrokes.AddLast(new Stroke(startPixel, 1 - startPixel.Intensity));
                        else
                            LineStrokes.AddLast(new Stroke(lastPixel, 1 - startPixel.Intensity));
                    }
                }


                if (LineStrokes.Count != 0)
                {
                    if (_UseIdleZones)
                    {
                        if (_DoubleDirections && isReversed)
                        {
                            LineStrokes.AddFirst(new IdleStroke(LineStrokes.First.Value.DestinationPoint + (line.Pixels[0] - line.Pixels[1]).Normalize() * _IdleDistance));

                            int lastIndex = line.Pixels.Count;
                            LineStrokes.AddLast(new IdleStroke(lastPixel));
                        }
                        else
                        {
                            LineStrokes.AddFirst(new IdleStroke(startPrintingPixel));

                            int lastIndex = line.Pixels.Count;
                            LineStrokes.AddLast(new IdleStroke(LineStrokes.Last.Value.DestinationPoint + (line.Pixels[lastIndex - 1] - line.Pixels[lastIndex - 2]).Normalize() * _IdleDistance));
                        }
                        
                        
                    }

                    IEnumerable<FreeMotionStroke> itemsToAdd;
                    if (_DoubleDirections && isReversed)
                    {
                        int lastIndex = line.Pixels.Count;
                        LineStrokes.AddLast(new FreeMotionStroke(lastPixel + (line.Pixels[lastIndex - 1] - line.Pixels[lastIndex - 2]).Normalize() * _IdleDistance));
                        itemsToAdd = LineStrokes.Reverse();
                    }
                    else
                    {
                        LineStrokes.AddFirst(new FreeMotionStroke(LineStrokes.First.Value.DestinationPoint + (line.Pixels[0] - line.Pixels[1]).Normalize() * _IdleDistance));
                        itemsToAdd = LineStrokes;
                    }

                    Strokes.AddRange(itemsToAdd);
   
                    isReversed = !isReversed;
                }
            }
        }
        
        
        
        
        
        
        /*public void GenerateStrokes()
        {
            bool isReversed = false;

            foreach (var line in _Lines)
            {
                
                if (line.Pixels.Count == 1 || line.Pixels.Count == 0)
                    continue;//throw new Exception("Пока не придумал что с этим делать");

                int strokesCount = Strokes.Count;

                List<Pixel> pixels = GetPixels(line.Pixels, isReversed);


                Pixel startPixel = null;
                foreach (var pixel in pixels)
                {
                    if (startPixel == null)
                    {
                        if (pixel.Intensity != 1)
                        {
                            startPixel = pixel;
                            AddLineStarAndAcceleration(startPixel, pixels[0] - pixels[1]);
                        }
                    }
                    else if(Math.Abs(pixel.Intensity - startPixel.Intensity) > SAME_INTENSITY)
                    {
                        Strokes.Add(new Stroke(pixel, 1 - startPixel.Intensity));
                        startPixel = pixel;
                    }
                }

                if(startPixel != null)
                {
                    var lastPixel = pixels[pixels.Count - 1];
                    if (startPixel.Intensity == 1)
                        lastPixel = startPixel;

                    if (startPixel != lastPixel)
                        Strokes.Add(new Stroke(lastPixel, 1 - startPixel.Intensity));
                    
                    AddLineEndAndDeacceleration(lastPixel, pixels[pixels.Count - 1] - pixels[pixels.Count - 2]);
                }

                if (strokesCount != Strokes.Count)
                    isReversed = !isReversed;
            }
        }

        private List<Pixel> GetPixels(List<Pixel> pixels, bool isReverse)
        {
            if (_DoubleDirections && isReverse)
                return ((IEnumerable<Pixel>)pixels).Reverse().ToList(); //пока так..потом подумать
            return pixels;
        }

        private void AddLineStarAndAcceleration(Pixel firstPixel, Vector direction)
        {
            if (_UseIdleZones)
            {
                Vector startPoint = firstPixel + direction.Normalize() * IdleDistance;

                Strokes.Add(new FreeMotionStroke(startPoint));
                Strokes.Add(new IdleStroke(firstPixel));
            }
            else
                Strokes.Add(new FreeMotionStroke(firstPixel));
        }
        private void AddLineEndAndDeacceleration(Vector lastPixel, Vector direction)
        {
            if (_UseIdleZones) Strokes.Add(new IdleStroke(lastPixel + direction.Normalize() * IdleDistance));
        }*/

        private const double SAME_INTENSITY = 0.1;
    }
}
