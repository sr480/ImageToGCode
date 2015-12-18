using ImageToGCode.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageToGCode.Engine.GCodeGeneration.VectorProcessor
{
    class VFile : INotifyPropertyChanged
    {

        public ObservableCollection<VPathGroup> PathGroups { get; private set; }
        public string FileName { get; private set; }
        public Matrix Transform { get; private set; }
        public RectangleF Boundings { get { return GetBoundingBox(); } }
        public Command MoveUp { get; private set; }
        public Command MoveDown { get; private set; }

        private VPathGroup _SelectedGroup;
        public VPathGroup SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                if (_SelectedGroup == value)
                    return;
                _SelectedGroup = value;
                RaisePropertyChanged("SelectedGroup");
                MoveUp.RaiseCanExecuteChanged();
                MoveDown.RaiseCanExecuteChanged();
            }
        }

        public VFile(string filePath)
        {
            PathGroups = new ObservableCollection<VPathGroup>();

            FileName = filePath.Remove(0, filePath.LastIndexOf('\\') + 1);

            LoadFile(filePath);
            
            Transform = new Matrix(1, 0, 0, 1, 0, 0);

            MoveUp = new Command((x) => MoveUpAction(), (x) => SelectedGroup != null);
            MoveDown = new Command((x) => MoveDownAction(), (x) => SelectedGroup != null);
        }

        private RectangleF GetBoundingBox()
        {
            double minX = double.PositiveInfinity, minY = double.PositiveInfinity;
            double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity;

            foreach (var pg in PathGroups)
                foreach (var path in pg.PathList)
                {
                    //var rect = path.GetBounds();
                    //if (rect.Left < minX) minX = rect.Left;
                    //if (rect.Bottom < minY) minY = rect.Bottom;
                    //if (rect.Top > maxY) maxY = rect.Top;
                    //if (rect.Right > maxX) maxX = rect.Right;

                    foreach(var pt in path.PathPoints)
                    {
                        if (pt.X < minX) minX = pt.X;
                        if (pt.Y < minY) minY = pt.Y;
                        if (pt.Y > maxY) maxY = pt.Y;
                        if (pt.X > maxX) maxX = pt.X;
                    }
                }

            return new RectangleF((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));
        }

        private void LoadFile(string filePath)
        {
            PathGroups.Clear();
            var doc = Svg.SvgDocument.Open(filePath);

            var gcg = new VPathGroupSVGGenerator(doc);
            foreach (var vPath in gcg.GenerateVPathGroups().OrderBy(g => g.PathColor.GetHue()))
            {
                PathGroups.Add(vPath);
            }
        }


        private void MoveUpAction()
        {
            if (SelectedGroup == null)
                throw new Exception("SelectedGroup is null");

            //var groupToMove = SelectedGroup;
            var idx = PathGroups.IndexOf(SelectedGroup);
            if (idx <= 0)
                return;

            PathGroups.Move(idx, idx - 1);
        }
        private void MoveDownAction()
        {
            if (SelectedGroup == null)
                throw new Exception("SelectedGroup is null");

            //var groupToMove = SelectedGroup;
            var idx = PathGroups.IndexOf(SelectedGroup);
            if (idx >= PathGroups.Count - 1)
                return;

            PathGroups.Move(idx, idx + 1);
        }

        public void SetTransform(float dX, float dY)
        {
            Matrix matrix = new Matrix();
            matrix.Translate(dX, dY);
            foreach (var grp in PathGroups)
            {
                foreach (var path in grp.PathList)
                {
                    path.Transform(matrix);
                }
            }

        }
        #region IPropertyChanged
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private void OnPropertyChanged(string propertyName)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
                RaisePropertyChanged(propertyName);
            else
                _synchronizationContext.Post(RaisePropertyChanged, propertyName);
        }
        private void RaisePropertyChanged(object param)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs((string)param));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
