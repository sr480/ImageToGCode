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
        private ObservableCollection<Engine.GCodeGeneration.BaseGCode> _GCode;
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        #endregion
        public ImageProcessorViewModel ImageProcessor { get; private set; }
        public VectorProcessorViewModel VectorProcessor { get; private set; }

        #region Properties: Input parameters
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

        #region Commands
        public Command Save { get; private set; }
        public Command Generate { get; private set; }
        #endregion

        public ObservableCollection<Engine.GCodeGeneration.BaseGCode> GCode
        {
            get { return _GCode; }
        }

        public MainViewModel()
        {
            _GCode = new ObservableCollection<Engine.GCodeGeneration.BaseGCode>();
            _GCode.CollectionChanged += GCode_CollectionChanged;
            
            ImageProcessor = new ImageProcessorViewModel(_GCode);
            VectorProcessor = new VectorProcessorViewModel();
            
            Save = new Command((x) => SaveGCodeAction(), (x) => GCode.Count > 0);
            Generate = new Command((x) => GenerateAction(), (x) => true);

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
