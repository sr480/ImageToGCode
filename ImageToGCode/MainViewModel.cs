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

        private double _CostPerSquareMeter = 160;
        private double _CostPerFeedMeter = 25;
        private double _CutCost;
        private double _MaterialCost;
        private double _Total;

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
                CountTotals();
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

        public double CostPerSquareMeter
        {
            get
            { return _CostPerSquareMeter; }
            set
            {

                if (_CostPerSquareMeter == value)
                    return;
                _CostPerSquareMeter = value;
                RaisePropertyChanged("CostPerSquareMeter");
                CountTotals();
            }
        }
        public double CostPerFeedMeter
        {
            get
            {
                return _CostPerFeedMeter;
            }
            set
            {
                if (_CostPerFeedMeter == value)
                    return;
                _CostPerFeedMeter = value;
                RaisePropertyChanged("CostPerFeedMeter");
                CountTotals();
            }
        }
        public double CutCost
        {
            get { return _CutCost; }
            private set
            {
                if (_CutCost == value)
                    return;
                _CutCost = value;
                RaisePropertyChanged("CutCost");                
            }
        }
        public double MaterialCost { get { return _MaterialCost; }
        set
            {
                if (_MaterialCost == value)
                    return;
                _MaterialCost = value;
                RaisePropertyChanged("MaterialCost");
            }
        }
        public double Total { get { return _Total; }
            set
            {
                if (_Total == value)
                    return;
                _Total = value;
                RaisePropertyChanged("Total");
            }
        }

        #region Commands
        public Command Save
        {
            get; private set;
        }
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

            VectorProcessor.Files.CollectionChanged += VectorFilesCollectionChanged;
        }

        private void VectorFilesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CountTotals();
        }
        #region Command implements

        private void SaveGCodeAction()
        {
            SaveFileDialog svDlg = new SaveFileDialog();
            svDlg.Filter = "LinuxCNC file (*.ngc)|*.ngc|GCode file (*.nc)|*.nc|All file (*.*)|*.*";
            svDlg.FilterIndex = 1;
            svDlg.RestoreDirectory = true;
            if (VectorProcessor.Files.Count == 1)
                svDlg.FileName = VectorProcessor.Files[0].FileName.Remove(VectorProcessor.Files[0].FileName.LastIndexOf('.')) + ".ngc";
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

            CountStatsAction();

            _GCode.Insert(0, new BaseGCode(string.Format("(RapidMotions: {0} mm, FeedMotions: {1} mm)", RapidMotionDistance, FeedMotionDistance)));
            _GCode.Insert(1, new BaseGCode(string.Format("(EstimatedTime: {0})", EstimatedTime)));

        }
        private void CountStatsAction()
        {
            var gcc = new GCodeTrajectoryCounter(_GCode);
            gcc.Count();
            RapidMotionDistance = Math.Round(gcc.Rapid, 2);
            FeedMotionDistance = Math.Round(gcc.Feed, 2);
            EstimatedTime = gcc.EstimatedTime;

            CountTotals();
        }
        #endregion
        private void CountTotals()
        {
            CutCost = CostPerFeedMeter * FeedMotionDistance / 1000;
            MaterialCost = CostPerSquareMeter * VectorProcessor.FilesSquare;
            Total = MaterialCost + CutCost;
        }
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
