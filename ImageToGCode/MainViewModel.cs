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
using System.Threading.Tasks;

namespace ImageToGCode
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private double _Angle = 45;
        private double _Feed = 400;
        private bool _EngraveBothDirection = true;
        private bool _UseFreeZone = true;
        private double _FreeZone = 10;
        private double _LineResolution = 0.1;
        private double _PointResolution = 0.1;
        private double _AspectRate = 2;
        private bool _KeepAspectRatio = true;
        private ObservableCollection<string> _GCode;
        private string _PathToFile;
        private double _Height = 5;
        private double _Width = 4;
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private Bitmap _Bitmap;
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
                if(KeepAspectRatio)
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
        #endregion
        public Engine.ImageByLinesPresenter Presenter { get; private set; }
        #region Commands
        public Command OpenImage { get; private set; }
        public Command Generate { get; private set; }
        public Command Save { get; private set; }
        #endregion

        public ObservableCollection<string> GCode
        {
            get { return _GCode; }
        }

        public MainViewModel()
        {
            _GCode = new ObservableCollection<string>();
            Generate = new Command((x) => GenerateAction(), (x) => _Bitmap != null);
            OpenImage = new Command((x) => OpenImageAction(), (x) => true);
            Save = new Command((x) => SaveGCodeAction(), (x) => GCode.Count > 0);
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
        private void GenerateAction()
        {
            var processor = new Engine.ImageProcessor(_Bitmap, Width, Height, LineResolution, PointResolution, Angle);
            Presenter = processor.CreatePresenter();

            //var gen = new GCodeGenerator(Width, Height, LineResolution, FreeZone, Feed, EngraveBothDirection);
            //var gCode = gen.Generate(_Bitmap);
            //GCode.Clear();

            //foreach (var line in gCode)
            //    GCode.Add(line);
            //Save.RaiseCanExecuteChanged();
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
