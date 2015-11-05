using ImageToGCode.Engine.GCodeGeneration;
using ImageToGCode.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageToGCode
{
    class ImageProcessorViewModel : INotifyPropertyChanged
    {

        private double _Height = 100;
        private double _Width = 100;

        private int _MinPower = 15;
        private int _MaxPower = 40;

        private Engine.Interpolators.IInterpolator _SelectedInterpolator;
        private List<Engine.Interpolators.IInterpolator> _InterpolatorsSource;

        private double _Angle = 90;

        private double _MinFeed = 4000;
        private double _MaxFeed = 10000;

        private bool _EngraveBothDirection = true;
        private bool _UseFreeZone = true;
        private double _FreeZone = 10;
        private double _LineResolution = 0.2;
        private double _PointResolution = 0.2;
        private double _AspectRate = 2;
        private bool _KeepAspectRatio = true;

        private string _PathToFile;

        private Bitmap _Bitmap;
        private TimeSpan _EstimatedTime;
        private ObservableCollection<BaseGCode> _GCode;


        public string PathToFile
        {
            get
            {
                return _PathToFile;
            }
            set
            {
                if (_PathToFile == value)
                    return;
                _PathToFile = value;
                OnPropertyChanged("_PathToFile");
            }
        }

        public double Width
        {
            get { return _Width; }
            set
            {
                if (_Width == value)
                    return;
                _Width = value;
                OnPropertyChanged("Width");
                if (KeepAspectRatio)
                    CountHeight(value);
                CountEstimatedTime();
            }
        }
        public double Height
        {
            get { return _Height; }
            set
            {
                if (_Height == value)
                    return;
                _Height = value;
                OnPropertyChanged("Height");
                if (KeepAspectRatio)
                    CountWidth(value);
                CountEstimatedTime();
            }
        }

        public double LineResolution
        {
            get { return _LineResolution; }
            set
            {
                if (_LineResolution == value)
                    return;
                _LineResolution = value;
                OnPropertyChanged("LineResolution");
                CountEstimatedTime();
            }
        }
        public double PointResolution
        {
            get
            {
                return _PointResolution;
            }
            set
            {
                if (_PointResolution == value)
                    return;
                _PointResolution = value;
                OnPropertyChanged("PointResolution");
            }
        }
        public double Angle
        {
            get
            {
                return _Angle;
            }
            set
            {
                if (_Angle == value)
                    return;
                _Angle = value;
                OnPropertyChanged("Angle");
            }
        }

        public bool UseFreeZone
        {
            get { return _UseFreeZone; }
            set
            {
                if (_UseFreeZone == value)
                    return;
                _UseFreeZone = value;
                OnPropertyChanged("UseFreeZone");
                CountEstimatedTime();
            }
        }
        public double FreeZone
        {
            get { return _FreeZone; }
            set
            {
                if (_FreeZone == value)
                    return;
                _FreeZone = value;
                OnPropertyChanged("FreeZone");
                CountEstimatedTime();
            }
        }

        public double MaxFeed
        {
            get { return _MaxFeed; }
            set
            {
                if (_MaxFeed == value)
                    return;
                _MaxFeed = value;
                OnPropertyChanged("MaxFeed");
                CountEstimatedTime();
            }
        }
        public double MinFeed
        {
            get
            {
                return _MinFeed;
            }
            set
            {
                if (_MinFeed == value)
                    return;
                _MinFeed = value;
                OnPropertyChanged("MinFeed");
            }
        }
        public bool EngraveBothDirection
        {
            get { return _EngraveBothDirection; }
            set
            {
                _EngraveBothDirection = value;
            }
        }
        public double AspectRate
        {
            get { return _AspectRate; }
            set
            {
                if (_AspectRate == value)
                    return;
                _AspectRate = value;
                OnPropertyChanged("AspectRate");
                if (KeepAspectRatio)
                    CountHeight(Width);
            }
        }
        public bool KeepAspectRatio
        {
            get
            {
                return _KeepAspectRatio;
            }
            set
            {
                if (_KeepAspectRatio == value)
                    return;
                _KeepAspectRatio = value;
                OnPropertyChanged("KeepAspectRatio");
                CountHeight(Width);
            }
        }
        public List<Engine.Interpolators.IInterpolator> InterpolatorsSource
        {
            get
            {
                return _InterpolatorsSource;
            }
        }
        public Engine.Interpolators.IInterpolator SelectedInterpolator
        {
            get
            {
                return _SelectedInterpolator;
            }
            set
            {
                if (_SelectedInterpolator == value)
                    return;
                _SelectedInterpolator = value;
                OnPropertyChanged("SelectedInterpolator");
            }
        }
        public TimeSpan EstimatedTime
        {
            get
            {
                return _EstimatedTime;
            }
            set
            {
                if (_EstimatedTime == value)
                    return;
                _EstimatedTime = value;
                RaisePropertyChanged("EstimatedTime");
            }
        }
        public int MaxPower
        {
            get
            {
                return _MaxPower;
            }
            set
            {
                if (_MaxPower == value)
                    return;
                if (value <= _MinPower)
                    _MinPower = value;//throw new Exception("Max power can't be more then min power");
                _MaxPower = value;
                RaisePropertyChanged("MaxPower");
            }
        }
        public int MinPower
        {
            get
            {
                return _MinPower;
            }
            set
            {
                if (_MinPower == value)
                    return;
                if (value >= _MaxPower)
                    _MaxPower = value;

                _MinPower = value;
                RaisePropertyChanged("MinPower");
            }
        }
        public Command OpenImage { get; private set; }
        public Command Generate { get; private set; }
        public ImageProcessorViewModel(ObservableCollection<BaseGCode> gcode)
        {
            _GCode = gcode;
            Generate = new Command((x) => GenerateAction(), (x) => _Bitmap != null);
            OpenImage = new Command((x) => OpenImageAction(), (x) => true);

            _InterpolatorsSource = new List<Engine.Interpolators.IInterpolator>();
            InterpolatorsSource.Add(new Engine.Interpolators.StepInterpolator());
            InterpolatorsSource.Add(new Engine.Interpolators.BilinearInterpolator());
            SelectedInterpolator = InterpolatorsSource[0];

        }

        private void OpenImageAction()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _Bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                    AspectRate = (double)_Bitmap.Width / (double)_Bitmap.Height;
                    Generate.RaiseCanExecuteChanged();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Ошибка открытия файла", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void GenerateAction()
        {
            _GCode.Clear();
            foreach (var gc in Process())
            {
                _GCode.Add(gc);
            }
        }

        private IEnumerable<BaseGCode> Process()
        {
            var processor = new Engine.ImageProcessor(_Bitmap, Width, Height, LineResolution, PointResolution, Angle, SelectedInterpolator);
            var presenter = processor.CreatePresenter();

            var strokeGenerator = new Engine.GCodeGeneration.ImageProcessor.StrokesFromImageLinesGenerator(presenter.Lines, UseFreeZone, FreeZone, EngraveBothDirection);
            strokeGenerator.GenerateStrokes();

            var gcGen = new Engine.GCodeGeneration.ImageProcessor.GCodeGenerator(strokeGenerator.Strokes, (int)MinFeed, (int)MaxFeed, MaxPower, MinPower);
            var gcode = gcGen.GenerateCode();

            foreach (var str in gcode)
                yield return str;

            yield return new BaseGCode("(----ImageGenerator----)");
            yield return new BaseGCode(string.Format("(FileName: {0})", _PathToFile));
            yield return new BaseGCode(string.Format("(Width: {0} mm, Height: {1} mm)", Width, Height));
            yield return new BaseGCode(string.Format("(Resolution line: {0} mm, point: {1} mm)", LineResolution, PointResolution));
            yield return new BaseGCode(string.Format("(Feed max: {0} mm/min, min: {1} mm/min)", MaxFeed, MinFeed));
            yield return new BaseGCode(string.Format("(Power max: {0}, min: {1})", MaxPower, MinPower));
            yield return new BaseGCode(string.Format("(Angle: {0})", Angle));
            yield return new BaseGCode(string.Format("(Idle zones: {0})", UseFreeZone ? FreeZone : 0.0));
            yield return new BaseGCode(string.Format("(Engrave both directions: {0})", EngraveBothDirection));
        }

        private void CountEstimatedTime()
        {
            double secs = ((Height / LineResolution) * (Width + (UseFreeZone ? FreeZone * 2 : 0))) / ((MinFeed + MaxFeed) / 2.0) * 60.0;
            if (!double.IsInfinity(secs) & !double.IsNaN(secs))
                EstimatedTime = TimeSpan.FromSeconds(Math.Round(secs));
        }
        private void CountWidth(double height)
        {
            Width = height * AspectRate;
        }
        private void CountHeight(double width)
        {
            Height = width / AspectRate;
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
