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
        private double _Feed = 400;
        private bool _EngraveBothDirection;
        private bool _UseFreeZone = true;
        private double _FreeZone = 10;
        private double _LineStep = 0.1;
        private double _AspectRate = 2;
        private bool _KeepAspectRatio = true;
        private ObservableCollection<string> _GCode;
        private string _PathToFile;
        private double _Height = 5;
        private double _Width = 4;
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        private Bitmap _Bitmap;

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
        public Command OpenImage { get; private set; }
        public Command Generate { get; private set; }

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

        public double LineStep
        {
            get { return _LineStep; }
            set
            {
                if (_LineStep == value)
                    return;
                _LineStep = value;
                OnPropertyChanged("LineStep");
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

        public ObservableCollection<string> GCode
        {
            get { return _GCode; }
        }

        public MainViewModel()
        {
            _GCode = new ObservableCollection<string>();
            Generate = new Command((x) => GenerateAction(), (x) => _Bitmap != null);
            OpenImage = new Command((x) => OpenImageAction(), (x) => true);
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
            var gen = new GCodeGenerator(Width, Height, LineStep, FreeZone, Feed, EngraveBothDirection);
            var gCode = gen.Generate(_Bitmap);//(Bitmap)Bitmap.FromFile("D:\\test.png"));
            GCode.Clear();
            var file = System.IO.File.CreateText("D:\\test.nc");
            foreach (var line in gCode)
            {
                GCode.Add(line);
                
                file.WriteLine(line);
                
            }
            file.Close();
        }

        private void CountWidth(double height)
        {
            Width = height * AspectRate;
        }
        private void CountHeight(double width)
        {
            Height = width / AspectRate;
        }

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
    }
}
