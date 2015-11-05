using ImageToGCode.Engine.GCodeGeneration;
using ImageToGCode.Engine.GCodeGeneration.VectorProcessor;
using ImageToGCode.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageToGCode
{
    class VectorProcessorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Engine.GCodeGeneration.VectorProcessor.VPathGroup> PathGroups { get; private set; }
        public Command OpenSvg { get; private set; }
        
        public VectorProcessorViewModel()
        {
            PathGroups = new ObservableCollection<Engine.GCodeGeneration.VectorProcessor.VPathGroup>();
            OpenSvg = new Command((x) => OpenSvgAction(), (x) => true);
        }

        private void OpenSvgAction()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SVG file (*.svg)|*.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                //try { 
                PathGroups.Clear();
                var doc = Svg.SvgDocument.Open(openFileDialog.FileName);

                var gcg = new VPathGroupSVGGenerator(doc);
                foreach (var vPath in gcg.GenerateVPathGroups())
                {
                    PathGroups.Add(vPath);
                }
                //}
                //catch (Exception e)
                //{
                //    System.Windows.MessageBox.Show("Ошибка открытия файла", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                //}
            }
        }

        public IEnumerable<BaseGCode> Generate()
        {
            foreach (var grp in PathGroups)
                foreach (var gc in grp.Generate())
                    yield return gc;
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
