using ImageToGCode.Engine.GCodeGeneration;
using ImageToGCode.Engine.GCodeGeneration.VectorProcessor;
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
    class VectorProcessorViewModel : INotifyPropertyChanged
    {
        private VFile _SelectedFile;
        public ObservableCollection<Engine.GCodeGeneration.VectorProcessor.VFile> Files { get; private set; }
        public Command OpenSvg { get; private set; }
        public Command RemoveSvg { get; private set; }
        public VFile SelectedFile
        {
            get
            {
                return _SelectedFile;
            }
            set
            {
                if (_SelectedFile == value)
                    return;
                _SelectedFile = value;
                RaisePropertyChanged("SelectedFile");
                RemoveSvg.RaiseCanExecuteChanged();
            }
        }
        public double FilesSquare
        {
            get
            {
                if (Files.Count > 0)
                    return Files.Sum(f => f.Boundings.Width * f.Boundings.Height / 1000000.0);
                return 0.0;
            }
        }
        public VectorProcessorViewModel()
        {
            Files = new ObservableCollection<Engine.GCodeGeneration.VectorProcessor.VFile>();
            OpenSvg = new Command((x) => OpenSvgAction(), (x) => true);
            RemoveSvg = new Command((x) => RemoveSvgAction(), (x) => SelectedFile != null);
        }

        private void OpenSvgAction()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SVG file (*.svg)|*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                var file = new VFile(openFileDialog.FileName);
                if (Files.Count > 0)
                {
                    var prevBounding = Files.Last().Boundings;
                    file.SetTransform(prevBounding.Right + 3, 0);
                }                    
                Files.Add(file);
            }
        }
        private void RemoveSvgAction()
        {
            Files.Remove(SelectedFile);
        }

        public IEnumerable<BaseGCode> Generate()
        {
            yield return new BaseGCode("G21");
            yield return new BaseGCode("G90");

            foreach (var fl in Files)
            {
                yield return new BaseGCode("M3 S0");
                foreach (var g in fl.Generate())
                    yield return g;
                yield return new BaseGCode("M5");
            }
            yield return new BaseGCode("%");
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
