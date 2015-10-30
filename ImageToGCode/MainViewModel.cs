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
    class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private double _Magnification = 1.0;
        private int _MinPower = 15;
        private int _MaxPower = 40;
        private Engine.Interpolators.IInterpolator _SelectedInterpolator;
        private Engine.Visualisers.LinesVisualiser _Visualiser;
        private Engine.GCodeGeneration.StrokesFromImageLinesGenerator _StrokeGenerator;

        private Engine.ImageByLinesPresenter _Presenter;
        private List<Engine.Interpolators.IInterpolator> _InterpolatorsSource;
        private double _Angle = 90;
        private double _MinFeed = 4000;
        private double _Feed = 10000;
        private bool _EngraveBothDirection = true;
        private bool _UseFreeZone = true;
        private double _FreeZone = 10;
        private double _LineResolution = 0.2;
        private double _PointResolution = 0.2;
        private double _AspectRate = 2;
        private bool _KeepAspectRatio = true;
        private ObservableCollection<Engine.GCodeGeneration.BaseGCode> _GCode;
        private string _PathToFile;
        private double _Height = 100;
        private double _Width = 100;
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private Bitmap _Bitmap;
        private TimeSpan _EstimatedTime;
        #endregion
        #region Properties: Input parameters
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

        public double Feed
        {
            get { return _Feed; }
            set
            {
                if (_Feed == value)
                    return;
                _Feed = value;
                OnPropertyChanged("Feed");
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
        public double Magnification
        {
            get
            {
                return _Magnification;
            }
            set
            {
                if (_Magnification == value)
                    return;
                _Magnification = value;
                RaisePropertyChanged("Magnification");
            }
        }
        public List<double> MagnificationSource { get; private set; }
        #endregion

        public Engine.ImageByLinesPresenter Presenter
        {
            get
            {
                return _Presenter;
            }
            private set
            {
                if (_Presenter == value)
                    return;
                _Presenter = value;
                OnPropertyChanged("Presenter");
            }
        }
        public Engine.Visualisers.LinesVisualiser Visualiser
        {
            get
            {
                return _Visualiser;
            }
            private set
            {
                if (_Visualiser == value)
                    return;
                _Visualiser = value;
                OnPropertyChanged("Visualiser");
            }
        }
        public Engine.GCodeGeneration.StrokesFromImageLinesGenerator StrokeGenerator
        {
            get
            {
                return _StrokeGenerator;
            }
        }
        #region Commands
        public Command OpenImage { get; private set; }
        public Command OpenSvg { get; private set; }
        public Command Generate { get; private set; }
        public Command Save { get; private set; }
        #endregion

        public ObservableCollection<Engine.GCodeGeneration.BaseGCode> GCode
        {
            get { return _GCode; }
        }

        public MainViewModel()
        {
            _GCode = new ObservableCollection<Engine.GCodeGeneration.BaseGCode>();
            _GCode.CollectionChanged += GCode_CollectionChanged;
            Generate = new Command((x) => GenerateAction(), (x) => _Bitmap != null);
            OpenImage = new Command((x) => OpenImageAction(), (x) => true);
            OpenSvg = new Command((x) => OpenSvgAction(), (x) => true);
            Save = new Command((x) => SaveGCodeAction(), (x) => GCode.Count > 0);

            _InterpolatorsSource = new List<Engine.Interpolators.IInterpolator>();
            InterpolatorsSource.Add(new Engine.Interpolators.StepInterpolator());
            InterpolatorsSource.Add(new Engine.Interpolators.BilinearInterpolator());
            SelectedInterpolator = InterpolatorsSource[0];

            MagnificationSource = new List<double>();
            MagnificationSource.Add(0.5);
            MagnificationSource.Add(1);
            MagnificationSource.Add(2);
            MagnificationSource.Add(4);
            MagnificationSource.Add(10);
            MagnificationSource.Add(20);
        }
        #region Command implements
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
        private void OpenSvgAction()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SVG file (*.svg)|*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                _GCode.Clear();
                try
                {
                    var doc = Svg.SvgDocument.Open(openFileDialog.FileName);

                    var gcg = new GCodeFromSVGGenerator(doc, (int)_MinFeed, (int)_Feed, (int)_MaxPower, (int)_MinPower);
                    foreach (var gc in gcg.GenerateCode())
                    {
                        _GCode.Add(gc);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Ошибка открытия файла", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }
        private void GenerateAction()
        {
            var processor = new Engine.ImageProcessor(_Bitmap, Width, Height, LineResolution, PointResolution, Angle, SelectedInterpolator);
            Presenter = processor.CreatePresenter();

            Visualiser = new Engine.Visualisers.LinesVisualiser(Presenter);
            Visualiser.Visualise();

            _StrokeGenerator = new Engine.GCodeGeneration.StrokesFromImageLinesGenerator(Presenter.Lines, UseFreeZone, FreeZone, EngraveBothDirection);
            //StrokeGenerator.GenerateStrokes();
            StrokeGenerator.GenerateStrokesNEW();

            var gcGen = new Engine.GCodeGeneration.GCodeGenerator(StrokeGenerator.Strokes, (int)MinFeed, (int)Feed, MaxPower, MinPower);
            var gcode = gcGen.GenerateCode();
            _GCode.Clear();
            foreach (var str in gcode)
                _GCode.Add(str);

            _GCode.Add(new BaseGCode(string.Format("(FileName: {0})", _PathToFile)));
            _GCode.Add(new BaseGCode(string.Format("(Width: {0} mm, Height: {1} mm)", Width, Height)));
            _GCode.Add(new BaseGCode(string.Format("(Resolution line: {0} mm, point: {1} mm)", LineResolution, PointResolution)));
            _GCode.Add(new BaseGCode(string.Format("(Feed max: {0} mm/min, min: {1} mm/min)", Feed, MinFeed)));
            _GCode.Add(new BaseGCode(string.Format("(Power max: {0}, min: {1})", MaxPower, MinPower)));
            _GCode.Add(new BaseGCode(string.Format("(Angle: {0})", Angle)));
            _GCode.Add(new BaseGCode(string.Format("(Idle zones: {0})", UseFreeZone?FreeZone:0.0)));
            _GCode.Add(new BaseGCode(string.Format("(Engrave both directions: {0})", EngraveBothDirection)));

        }
        private void SaveGCodeAction()
        {
            SaveFileDialog svDlg = new SaveFileDialog();
            svDlg.Filter = "LinuxCNC file (*.ngc)|*.ngc|GCode file (*.nc)|*.nc|All file (*.*)|*.*";
            svDlg.FilterIndex = 1;
            svDlg.RestoreDirectory = true;
            if (svDlg.ShowDialog() == true)
            {
                try
                {
                    using (var file = System.IO.File.CreateText(svDlg.FileName))
                    {
                        foreach (var line in GCode)
                            file.WriteLine(line);
                        file.Close();
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Ошибка сохранения файла", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }
        #endregion
        private void CountEstimatedTime()
        {
            double secs = ((Height / LineResolution) * (Width + (UseFreeZone ? FreeZone * 2 : 0))) / Feed * 60.0;
            if(!double.IsInfinity(secs) & !double.IsNaN(secs))
                EstimatedTime = TimeSpan.FromSeconds(Math.Round(secs));
        }
        private void GCode_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save.RaiseCanExecuteChanged();
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
