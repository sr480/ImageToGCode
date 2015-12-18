using ImageToGCode.Engine.GCodeGeneration.VectorProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageToGCode.Engine.Visualisers
{
    class VFileVisual : IRenderable
    {
        private readonly VFile _File;
        public event EventHandler UpdateNeeded;

        public DrawingGroup Drawing { get; private set; }
        
        public VFileVisual(VFile file)
        {
            _File = file;

            foreach (var group in _File.PathGroups)
            {
                group.PropertyChanged += PathGroup_PropertyChanged;
            }
            _File.PropertyChanged += VFile_PropertyChanged;
            Render();            
        }

        void VFile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedGroup")
                Render();            
        }

        void PathGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Engrave")
                Render();
        }



        public void Render()
        {
            Drawing = new DrawingGroup();
            

            //Children.Clear();
            Drawing.Transform = new MatrixTransform();
            
            using (var dc = Drawing.Open())
            {
                if (_File.SelectedGroup != null)
                    RenderPathGroup(_File.SelectedGroup, new Pen(SystemColors.HighlightBrush, 3.0), dc);     

                foreach (var vPathGrp in _File.PathGroups)
                {
                    if (!vPathGrp.Engrave)
                        continue;
                    RenderPathGroup(vPathGrp, new Pen(vPathGrp.Brush, 1.0), dc);                 
                }
            }
            //Drawing.Freeze();

            if(UpdateNeeded != null)
                UpdateNeeded(this, new EventArgs());
        }

        private void RenderPathGroup(VPathGroup pathGroup, Pen pen,DrawingContext dc)
        {
            foreach (var pth in pathGroup.PathList)
            {
                var currentPathData = pth.PathData;

                if (currentPathData.Points.Length == 0)
                    continue;

                System.Drawing.PointF? prevPoint = null;
                System.Drawing.PointF? startPoint = null;

                for (int i = 0; i < currentPathData.Points.Length; i++)
                {
                    var curPthType = currentPathData.Types[i];
                    var curPoint = currentPathData.Points[i];
                    //Find dirst point in path
                    if (Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.Start))
                        startPoint = prevPoint = curPoint;
                    //Draw line on path points
                    else if (Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.Line) ||
                        Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.Bezier))
                    {
                        if (prevPoint.HasValue)
                        {
                            Point start = ToPointConverter.FromPointF(prevPoint.Value);
                            Point end = ToPointConverter.FromPointF(curPoint);
                            dc.DrawLine(pen, start, end);
                        }
                        prevPoint = curPoint;

                        //ClosePath
                        if (Geometry.PathTypeHelper.IsSet(curPthType, System.Drawing.Drawing2D.PathPointType.CloseSubpath))
                        {
                            Point start = ToPointConverter.FromPointF(curPoint);
                            Point end = ToPointConverter.FromPointF(startPoint.Value);
                            dc.DrawLine(pen, start, end);
                        }
                    }
                }
            }
        }


    }
}
