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
using ImageToGCode.Engine.GCodeGeneration.VectorProcessor;
using System.Runtime.InteropServices;

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
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(VFile), typeof(Visualiser),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, null, null));
        public static readonly DependencyProperty MagnificationProperty =
            DependencyProperty.Register("Magnification", typeof(double), typeof(Visualiser),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, null));
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(Visualiser),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, null, null));

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public VFile SelectedFile
        {
            get { return (VFile)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }
        public double Magnification
        {
            get { return (double)GetValue(MagnificationProperty); }
            set { SetValue(MagnificationProperty, value); }
        }

        private Matrix _Transformation;
        private Dictionary<VFile, VFileVisual> _items;
        private VisualGrid _grid;
        static Visualiser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Visualiser), new FrameworkPropertyMetadata(typeof(Visualiser)));
        }
        public Visualiser()
        {
            Focusable = true;
            MouseWheel += Visualiser_MouseWheel;
            MouseDown += Visualiser_MouseDown;
            MouseMove += Visualiser_MouseMove;
            MouseUp += Visualiser_MouseUp;
            
            _Transformation = new Matrix(1, 0, 0, -1, 0, 0);
            this.RenderTransform = new MatrixTransform(_Transformation);
            _items = new Dictionary<VFile, VFileVisual>();
            _grid = new VisualGrid();
            _grid.Render();
        }

        //protected override Visual GetVisualChild(int index)
        //{
        //    if (index < 0 || index >= _items.Count)
        //        throw new ArgumentOutOfRangeException();

        //    return _items.Values.ElementAt(index);
        //}
        //protected override int VisualChildrenCount
        //{
        //    get
        //    {
        //        return _items.Count;
        //    }
        //}
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (Data is IEnumerable<BaseGCode>)
                VisualiseGCode(dc);
            if (Data is VectorProcessorViewModel)
            {
                dc.DrawDrawing(_grid.Drawing);
                VisualiseVector(dc);
                foreach (var visual in _items.Values)
                    dc.DrawDrawing(visual.Drawing);                
            }
        }

        void Visualiser_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _startDragPoint = null;
        }
        int mouseMoveCount = 0;
        void Visualiser_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMoveCount++;
            if (mouseMoveCount % 2 != 0)
                return;
            if (_startDragPoint != null & SelectedFile != null)
            {

                var newPoint = e.GetPosition(this);
                var selectedFile = ((VectorProcessorViewModel)Data).SelectedFile;

                var tsPoint = (_startDragPoint.Value);
                var tePoint = (newPoint);
                var dX = tsPoint.X - tePoint.X;
                var dY = tsPoint.Y - tePoint.Y;


                selectedFile.SetTransform(-(float)dX, -(float)dY);
                var visual = _items[selectedFile];

                var mt = ((MatrixTransform)visual.Drawing.Transform).Matrix;
                mt.Translate(-dX, -dY);
                ((MatrixTransform)visual.Drawing.Transform).Matrix = mt;
                //visual.Render();
                _startDragPoint = newPoint;
                InvalidateVisual();
            }
            if (_startDragPoint != null & SelectedFile == null)
            {
                var newPoint = e.GetPosition(this);

                var tsPoint = (_startDragPoint.Value);
                var tePoint = (newPoint);
                var dX = tsPoint.X - tePoint.X;
                var dY = tsPoint.Y - tePoint.Y;

                _Transformation.Translate(-dX, dY);
                this.RenderTransform = new MatrixTransform(_Transformation);
                _startDragPoint = newPoint;
            }
        }
        Point? _clickPoint;
        Point? _startDragPoint;
        void Visualiser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(Data is VectorProcessorViewModel))
                return;
            var data = ((VectorProcessorViewModel)Data);            

            var curMousePosition = e.GetPosition(this);
            _clickPoint = curMousePosition;

            bool isFileFound = false;
            foreach (var file in data.Files)
            {
                if (file.Boundings.Contains(new System.Drawing.PointF((float)curMousePosition.X, (float)curMousePosition.Y)))
                {
                    data.SelectedFile = file;
                    isFileFound = true;
                    break;
                }
            }
            if (!isFileFound)
                data.SelectedFile = null;

            _startDragPoint = curMousePosition;
        }

        private void Visualiser_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Magnification = Magnification + e.Delta * 0.001;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == MagnificationProperty)
            {
                _Transformation.M11 = (double)e.NewValue;
                _Transformation.M22 = -(double)e.NewValue;
                UpdateTransformations();
            }
            if (e.Property == ActualHeightProperty)
            {
                _Transformation.OffsetY = (double)e.NewValue;
                UpdateTransformations();
            }
            if (e.Property == DataProperty)
            {
                if (e.NewValue is VectorProcessorViewModel)
                    ((VectorProcessorViewModel)e.NewValue).Files.CollectionChanged += Files_CollectionChanged;

                if (e.OldValue is VectorProcessorViewModel)
                    ((VectorProcessorViewModel)e.OldValue).Files.CollectionChanged -= Files_CollectionChanged;
            }
        }

        void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (VFile file in e.NewItems)
                {
                    if (!_items.ContainsKey(file))
                    {
                        var visual = GetVisualForFile(file);
                        _items.Add(file, visual);
                        visual.UpdateNeeded += FileVisualNeedsUpdate;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (VFile file in e.OldItems)
                {
                    if (_items.ContainsKey(file))
                    {
                        var visual = _items[file];
                        ((VFileVisual)visual).UpdateNeeded -= FileVisualNeedsUpdate;
                        _items.Remove(file);
                    }
                }
            }
            InvalidateVisual();
        }

        void FileVisualNeedsUpdate(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private void UpdateFileVisual(VFile file)
        {
            var visual = _items[file];
            if (visual is VFileVisual)
                ((VFileVisual)visual).Render();
        }
        private void UpdateTransformations()
        {
            this.RenderTransform = new MatrixTransform(_Transformation);
        }

        private VFileVisual GetVisualForFile(VFile file)
        {
            return new VFileVisual(file);
        }
        private void VisualiseVector(DrawingContext dc)
        {
            if (Data == null || !(Data is VectorProcessorViewModel) || ((VectorProcessorViewModel)Data).Files.Count == 0)
                return;
            var data = (VectorProcessorViewModel)Data;

            if (_clickPoint != null)
                dc.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1), _clickPoint.Value, 3, 3);
            if (_startDragPoint != null)
                dc.DrawEllipse(Brushes.Green, new Pen(Brushes.Green, 1), _startDragPoint.Value, 3, 3);

            if (data.SelectedFile != null)
            {
                var rect = data.SelectedFile.Boundings;
                dc.DrawRectangle(null, new Pen(Brushes.DarkGray, 2.0), new Rect(new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Top)));
            }
            DrawArrows(dc);
        }
        private void VisualiseGCode(DrawingContext dc)
        {
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

                    Point start = VectorToPoint(firstMotion.Position);
                    Point end = VectorToPoint(curMotion.Position);

                    dc.DrawLine(new Pen(new SolidColorBrush(GCodeToColor(curMotion)), 1.0), start, end);

                    if (curMotion is RapidMotion)
                    {
                        var dir = (curMotion.Position - firstMotion.Position).Normalize();
                        var v1 = dir.Rotate(15.0 / 180.0 * Math.PI) * 5;
                        var v2 = dir.Rotate(-15.0 / 180.0 * Math.PI) * 5;
                        v1 = curMotion.Position - v1;
                        v2 = curMotion.Position - v2;
                        dc.DrawLine(new Pen(new SolidColorBrush(Colors.Magenta), 1.0), end, VectorToPoint(v1));
                        dc.DrawLine(new Pen(new SolidColorBrush(Colors.Magenta), 1.0), end, VectorToPoint(v2));
                    }
                }

                firstMotion = curMotion;
            }

            DrawArrows(dc);
        }

        private void DrawArrows(DrawingContext dc)
        {
            Pen arrowPen = new Pen(Brushes.DarkGray, 1);
            dc.DrawLine(arrowPen, CoordToPoint(0, 0), CoordToPoint(0, 100));
            dc.DrawLine(arrowPen, CoordToPoint(2, 95), CoordToPoint(0, 100));
            dc.DrawLine(arrowPen, CoordToPoint(-2, 95), CoordToPoint(0, 100));
            dc.DrawLine(arrowPen, CoordToPoint(0, 0), CoordToPoint(100, 0));
            dc.DrawLine(arrowPen, CoordToPoint(95, 2), CoordToPoint(100, 0));
            dc.DrawLine(arrowPen, CoordToPoint(95, -2), CoordToPoint(100, 0));

            dc.DrawEllipse(Brushes.Red, new Pen(Brushes.DarkGray, 1), CoordToPoint(0, 0), 3, 3);
        }

        private Color GCodeToColor(BaseMotion motion)
        {
            if (motion is CoordinatMotion)
            {
                var cm = (CoordinatMotion)motion;
                return Color.FromRgb(cm.Color.R, cm.Color.G, cm.Color.B);
            }

            if (motion is RapidMotion)
                return Colors.HotPink;

            return Colors.HotPink;
        }
        private Point VectorToPoint(Geometry.Vector v)
        {
            return new Point(v.X, v.Y);
        }
        private Point CoordToPoint(double x, double y)
        {
            return new Point(x, y);
        }
        private Point PointFToPoint(System.Drawing.PointF pt)
        {
            return new Point(pt.X, pt.Y);
        }
        private Point PointFToPointWOTrans(System.Drawing.PointF pt)
        {
            return new Point(pt.X, pt.Y);
        }

    }
}
