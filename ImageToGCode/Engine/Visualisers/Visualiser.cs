using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageToGCode.Engine.GCodeGeneration;
using ImageToGCode.Engine.GCodeGeneration.ImageProcessor;

namespace ImageToGCode.Engine.Visualisers
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageToGCode.Engine.Visualisers"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageToGCode.Engine.Visualisers;assembly=ImageToGCode.Engine.Visualisers"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:Visualiser/>
    ///
    /// </summary>
    class Visualiser : Canvas
    {
        public int MinIntensity
        {
            get { return (int)GetValue(MinIntensityProperty); }
            set { SetValue(MinIntensityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinIntensity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinIntensityProperty =
            DependencyProperty.Register("MinIntensity", typeof(int), typeof(Visualiser), new UIPropertyMetadata(0));
        public int MaxIntensity
        {
            get { return (int)GetValue(MaxIntensityProperty); }
            set { SetValue(MaxIntensityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxIntensity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxIntensityProperty =
            DependencyProperty.Register("MaxIntensity", typeof(int), typeof(Visualiser), new UIPropertyMetadata(0));



        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }


        public double Magnification
        {
            get { return (double)GetValue(MagnificationProperty); }
            set { SetValue(MagnificationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Magnification.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MagnificationProperty =
            DependencyProperty.Register("Magnification", typeof(double), typeof(Visualiser),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, null));



        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(Visualiser),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, null, null));

        static Visualiser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Visualiser), new FrameworkPropertyMetadata(typeof(Visualiser)));
        }
        public Visualiser()
        {

        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);            
        }
        protected override void OnRender(DrawingContext dc)
        {
            if (Data is IEnumerable<BaseGCode>)
                VisualiseGCode(dc);
            if (Data is VectorProcessorViewModel)
                VisualiseVector(dc);
        }
        private void VisualiseVector(DrawingContext dc)
        {
            if (Data == null || !(Data is VectorProcessorViewModel) || ((VectorProcessorViewModel)Data).PathGroups.Count == 0)
                return;
            var data = (VectorProcessorViewModel)Data;

            foreach(var vPathGrp in data.PathGroups)
            {
                if (!vPathGrp.Engrave)
                    continue;

                foreach(var pth in vPathGrp.PathList)
                {
                    if (pth.PathData.Points.Count() == 0)
                        continue;

                    System.Drawing.PointF? prevPoint = null;
                    for (int i = 0; i < pth.PathData.Points.Count(); i++)
                    {
                        if (pth.PathData.Types[i] == 0 | pth.PathData.Types[i] == 1)
                        {
                            if (prevPoint.HasValue)
                            {
                                Point start = new Point(prevPoint.Value.X * Magnification,
                                    this.ActualHeight - prevPoint.Value.Y * Magnification);
                                Point end = new Point((pth.PathData.Points[i].X) * Magnification,
                                    this.ActualHeight - pth.PathData.Points[i].Y * Magnification);
                                dc.DrawLine(new Pen(vPathGrp.Brush, 1.0), start, end);
                            }
                            prevPoint = pth.PathData.Points[i];
                        }
                        //else
                            //result.Add(new CoordinatMotion(new Vector(pth.PathData.Points[i]), _MaxPower, _MaxFeed));                        
                    }
                }
            }
        }
        private void VisualiseGCode(DrawingContext dc)
        {
            const double offset = 100;

            if (Data == null || !(Data is IEnumerable<BaseGCode>) || ((IEnumerable<BaseGCode>)Data).Count() == 0)
                return;

            var data = (IList<BaseGCode>)Data;

            BaseMotion firstMotion = null;
            foreach (BaseGCode item in data)
            {
                if (!(item is BaseMotion))
                    continue;

                var curMotion = (BaseMotion)item;

                if (firstMotion != null)
                {

                    Point start = new Point((firstMotion.Position.X + offset) * Magnification,
                        this.ActualHeight - (firstMotion.Position.Y + offset) * Magnification);
                    Point end = new Point((curMotion.Position.X + offset) * Magnification,
                        this.ActualHeight - (curMotion.Position.Y + offset) * Magnification);

                    dc.DrawLine(new Pen(new SolidColorBrush(GCodeToColor(curMotion)), 1.0), start, end);
                }

                firstMotion = curMotion;
            }

            dc.DrawEllipse(Brushes.Green, new Pen(Brushes.Red, 1), new Point(offset, this.ActualHeight - offset), 4, 4);
        }

        private Color GCodeToColor(BaseMotion motion)
        {
            if (motion is CoordinatMotion)
            {
                var cm = (CoordinatMotion)motion;
                if (cm.Intensity == 0.0)
                    return Colors.PaleGreen;


                byte r = (byte)(cm.Color.R * (cm.Intensity - MinIntensity) / (MaxIntensity - MinIntensity));
                byte g = (byte)(cm.Color.G * (cm.Intensity - MinIntensity) / (MaxIntensity - MinIntensity));
                byte b = (byte)(cm.Color.B * (cm.Intensity - MinIntensity) / (MaxIntensity - MinIntensity));
                return Color.FromRgb(r, g, b);
            }

            if (motion is RapidMotion)
                return Colors.HotPink;

            return Colors.HotPink;
        }

        private Color StrokeToColor(FreeMotionStroke stroke)
        {
            if (stroke is IdleStroke)
                return Colors.BlueViolet;
            if (stroke is Stroke)
            {
                if (((Stroke)stroke).Intensity == 0.0)
                    return Colors.PaleGreen;

                byte gcIntence = (byte)(255 - 255 * ((Stroke)stroke).Intensity);
                return Color.FromRgb((byte)gcIntence, (byte)gcIntence, (byte)gcIntence);
            }

            return Colors.HotPink;
        }
    }
}
