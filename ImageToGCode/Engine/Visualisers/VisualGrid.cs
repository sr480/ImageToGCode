using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace ImageToGCode.Engine.Visualisers
{
    class VisualGrid : INotifyPropertyChanged, IRenderable
    {
        public double XOffset
        {
            get
            {
                return _XOffset;
            }
            set
            {
                if (_XOffset == value)
                    return;
                _XOffset = value;
                RaisePropertyChanged("XOffset");
            }
        }
        public double YOffset
        {
            get
            {
                return _YOffset;
            }
            set
            {
                if (_YOffset == value)
                    return;
                _YOffset = value;
                RaisePropertyChanged("YOffset");
            }
        }
        public double XStep
        {
            get
            {
                return _XStep;
            }
            set
            {
                if (_XStep == value)
                    return;
                _XStep = value;
                RaisePropertyChanged("XStep");
            }
        }
        public double YStep
        {
            get
            {
                return _YStep;
            }
            set
            {
                if (_YStep == value)
                    return;
                _YStep = value;
                RaisePropertyChanged("YStep");
            }
        }

        public bool SnapToGrid { get; set; }

        public DrawingGroup Drawing { get; set; }

        public VisualGrid()
        {
            XStep = 100.0;
            YStep = 100.0;
        }
        
        public void Render()
        {
            Drawing = new DrawingGroup();

            using(var dc = Drawing.Open())
            {
                for(int i = -10; i< 10; i++)
                {
                    dc.DrawLine(new Pen(Brushes.Gray, 1.0), new Point(i * XStep + XOffset, -10 * YStep + YOffset), new Point(i * XStep + XOffset, 10 * YStep + YOffset));
                    dc.DrawLine(new Pen(Brushes.Gray, 1.0), new Point(-10 * XStep + XOffset, i * YStep + YOffset), new Point(10 * XStep + XOffset, i * YStep + YOffset));
                }
            }
            Drawing.Freeze();
        }

        public event EventHandler UpdateNeeded;

        #region IPropertyChanged
        private double _YStep;
        private double _XStep;
        private double _YOffset;
        private double _XOffset;
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
