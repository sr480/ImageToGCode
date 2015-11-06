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
        private TimeSpan _EstimatedTime;
        private double _RapidMotionDistance;
        private double _FeedMotionDistance;
        private double _Magnification = 1.0;
        private ObservableCollection<BaseGCode> _GCode;
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        #endregion
        public ImageProcessorViewModel ImageProcessor { get; private set; }
        public VectorProcessorViewModel VectorProcessor { get; private set; }

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

        public double FeedMotionDistance
        {
            get
            {
                return _FeedMotionDistance;
            }
            set
            {
                if (_FeedMotionDistance == value)
                    return;
                _FeedMotionDistance = value;
                RaisePropertyChanged("FeedMotionDistance");
            }
        }
        public double RapidMotionDistance
        {
            get
            {
                return _RapidMotionDistance;
            }
            set
            {
                if (_RapidMotionDistance == value)
                    return;
                _RapidMotionDistance = value;
                RaisePropertyChanged("RapidMotionDistance");
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

        #region Commands
        public Command Save { get; private set; }
        public Command Generate { get; private set; }
        public Command CountStats { get; private set; }
        #endregion

        public ObservableCollection<BaseGCode> GCode
        {
            get { return _GCode; }
        }

        public MainViewModel()
        {
            _GCode = new ObservableCollection<BaseGCode>();
            _GCode.CollectionChanged += GCode_CollectionChanged;

            ImageProcessor = new ImageProcessorViewModel(_GCode);
            VectorProcessor = new VectorProcessorViewModel();

            Save = new Command((x) => SaveGCodeAction(), (x) => GCode.Count > 0);
            Generate = new Command((x) => GenerateAction(), (x) => true);
            CountStats = new Command((x) => CountStatsAction(), (x) => true);

            MagnificationSource = new List<double>();
            MagnificationSource.Add(0.5);
            MagnificationSource.Add(1);
            MagnificationSource.Add(2);
            MagnificationSource.Add(4);
            MagnificationSource.Add(10);
            MagnificationSource.Add(20);
        }
        #region Command implements

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

        private void GenerateAction()
        {
            _GCode.Clear();
            foreach (var gc in VectorProcessor.Generate())
                _GCode.Add(gc);
        }
        private void CountStatsAction()
        {
            var gcc = new GCodeTrajectoryCounter(_GCode);
            gcc.Count();
            RapidMotionDistance = Math.Round(gcc.Rapid, 2);
            FeedMotionDistance = Math.Round(gcc.Feed, 2);
            EstimatedTime = gcc.EstimatedTime;
        }
        #endregion

        private void GCode_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save.RaiseCanExecuteChanged();
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
